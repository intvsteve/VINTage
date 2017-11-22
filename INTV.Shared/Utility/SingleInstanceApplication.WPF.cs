// <copyright file="SingleInstanceApplication.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// WPF-specific implementation of SingleInstanceApplication.
    /// </summary>
    public partial class SingleInstanceApplication : System.Windows.Application
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of SingleInstanceApplication.
        /// </summary>
        public SingleInstanceApplication()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of SingleInstanceApplication.
        /// </summary>
        /// <param name="settings">The application-specific settings to save at exit.</param>
        public SingleInstanceApplication(System.Configuration.ApplicationSettingsBase settings)
        {
            Initialize(settings);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the application settings useful for XAML bindings.
        /// </summary>
        public static System.Configuration.ApplicationSettingsBase SharedAppSettings
        {
            get { return INTV.Shared.Properties.Settings.Default; }
        }

        private static bool AlreadyDisplayedExceptionDialog { get; set; }

        /// <summary>
        /// Gets or sets the unique window activate message to send to the original instance.
        /// </summary>
        private static uint ActivationMessage { get; set; }

        private AppReadyState ReadyState
        {
            get
            {
                return _readyState;
            }

            set
            {
                _readyState |= value;
                if (_readyState == AppReadyState.Ready)
                {
                    this.Dispatcher.BeginInvoke(
                        new Action(() => ExecuteStartupActions()),
                        System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                }
            }
        }
        private AppReadyState _readyState;

        #endregion // Properties

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <typeparam name="T">The type of the application's main window.</typeparam>
        /// <param name="uniqueInstance">A unique identifier for the application, used to enforce the single instance feature.</param>
        /// <param name="settings">The application-specific settings data.</param>
        /// <param name="args">Command line arguments to the application.</param>
        /// <param name="splashScreenImage">Image to display in the splash screen.</param>
        public static void RunApplication<T>(string uniqueInstance, System.Configuration.ApplicationSettingsBase settings, string[] args, string splashScreenImage) where T : System.Windows.Window, new()
        {
            if (IsFirstInstance(uniqueInstance))
            {
                _mainWindowType = typeof(T);
                _splashScreenResource = splashScreenImage;
                var splashScreen = new System.Windows.SplashScreen(splashScreenImage);
                splashScreen.Show(true, false);
                var app = new SingleInstanceApplication(settings);
                ////splashScreen.Close(TimeSpan.FromMilliseconds(444));
                var window = new T();
                app.MainWindow = window;

                // Should this be done in an OnImportsSatisfied instead? On Mac, it's done in the MainWindowController.
                var commandProviders = SingleInstanceApplication.Instance.CommandProviders;
                window.AddCommandsToMainWindow(commandProviders.Select(c => c.Value));

                app.Run(window);
            }
            else
            {
                ActivateOriginalInstance();
            }
        }

        /// <summary>
        /// Determines whether the application is launching for the first time.
        /// </summary>
        /// <param name="uniqueId">An application (and system-wide) unique value.</param>
        /// <returns><c>true</c> iff this is the first instance of the application.</returns>
        /// <remarks>This method uses the time-honored mechanism of creating a named mutex. If the named mutex
        /// already exists, then the application has already launched.
        /// In Windows, this function also registers a message with the system using the supplied unique identifier.</remarks>
        public static bool IsFirstInstance(string uniqueId)
        {
            ActivationMessage = NativeMethods.RegisterWindowMessage(uniqueId);
            bool firstInstance;
            _started = new Mutex(false, uniqueId, out firstInstance);
            return firstInstance;
        }
        private static Mutex _started;

        /// <summary>
        /// This function activates the previous instance of the application.
        /// </summary>
        public static void ActivateOriginalInstance()
        {
            if ((ActivationMessage >= 0xC000) && (ActivationMessage <= 0xFFFF))
            {
                NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, ActivationMessage, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #region IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            try
            {
                InitializePlugins();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error initializing plugins: " + e.Message);
            }
            foreach (var configuration in Configurations)
            {
                var settings = configuration.Value.Settings as System.Configuration.ApplicationSettingsBase;
                if (settings != null)
                {
                    AddSettings(settings);
                }
            }
            ReadyState |= AppReadyState.ImportsStatisfied;
        }

        #endregion //  IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow.Loaded += HandleMainWindowLoaded;
            MainWindow.IsVisibleChanged += HandleIsVisibleChanged;
            MainWindow.SourceInitialized += HandleSourceInitialized;
            MainWindow.Closing += HandleMainWindowClosing;
            MainWindow.Show();
        }

        /// <inheritdoc />
        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            base.OnExit(e);
            foreach (var settings in Settings)
            {
                settings.Save();
            }
        }

        private static string GetPluginsDirectory()
        {
            string pluginsDirectory = null;
            try
            {
                var pluginsBasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(entryAssembly.Location);
                if (!string.IsNullOrEmpty(versionInfo.CompanyName))
                {
                    pluginsBasePath = System.IO.Path.Combine(pluginsBasePath, versionInfo.CompanyName);
                }
                pluginsBasePath = System.IO.Path.Combine(pluginsBasePath, System.IO.Path.GetFileName(entryAssembly.Location));
                pluginsDirectory = System.IO.Path.Combine(pluginsBasePath, "Plugins");
                if (!System.IO.Directory.Exists(pluginsDirectory))
                {
                    System.IO.Directory.CreateDirectory(pluginsDirectory);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to identify plugins directory.");
            }
            return pluginsDirectory;
        }

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        partial void OSInitialize()
        {
            _programDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            _settings.Add(INTV.Shared.Properties.Settings.Default);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void HandleMainWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = sender as System.Windows.Window;
            window.Loaded -= HandleMainWindowLoaded;
            ReadyState |= AppReadyState.MainWindowLoaded;
        }

        private void HandleIsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var window = sender as System.Windows.Window;
            if ((bool)e.NewValue)
            {
                window.IsVisibleChanged -= HandleIsVisibleChanged;
                ReadyState |= AppReadyState.MainWindowVisible;
            }
        }

        private void HandleMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var window = sender as System.Windows.Window;
            var windowState = window.WindowState;
            if (windowState == System.Windows.WindowState.Minimized)
            {
                windowState = System.Windows.WindowState.Normal;
            }
            var restoreBounds = window.RestoreBounds;
            INTV.Shared.Properties.Settings.Default.WindowState = windowState;
            INTV.Shared.Properties.Settings.Default.WindowPosition = restoreBounds.Location;
            INTV.Shared.Properties.Settings.Default.WindowSize = restoreBounds.Size;
        }

        private void HandleSourceInitialized(object sender, EventArgs e)
        {
            var window = sender as System.Windows.Window;
            window.SourceInitialized -= HandleSourceInitialized;
            _windowSource = System.Windows.Interop.HwndSource.FromDependencyObject(sender as System.Windows.Window) as System.Windows.Interop.HwndSource;
            _windowSource.AddHook(ActivateWindowHandler);
            ReadyState |= AppReadyState.MainWindowSourced;
        }
        private System.Windows.Interop.HwndSource _windowSource;

        private IntPtr ActivateWindowHandler(IntPtr windowHandle, int message, IntPtr wordParameter, IntPtr longParameter, ref bool handled)
        {
            if (message == SingleInstanceApplication.ActivationMessage)
            {
                var window = System.Windows.Interop.HwndSource.FromHwnd(windowHandle).RootVisual as System.Windows.Window;
                if (window != null)
                {
                    if (!window.IsVisible)
                    {
                        window.Show();
                    }
                    if (window.WindowState == System.Windows.WindowState.Minimized)
                    {
                        window.WindowState = System.Windows.WindowState.Normal;
                    }
                    window.Activate();
                    window.Topmost = true;  // \__ This nifty trick ensures that our window (very briefly) is topmost, floating above others.
                    window.Topmost = false; // /
                    window.Focus();         // make sure keyboard input is directed to our window
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => ReportExceptionAndExit(e.ExceptionObject as Exception)));
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ReportExceptionAndExit(e.Exception);
            e.Handled = true;
        }

        private void ReportExceptionAndExit(Exception exception)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            // The technique to display window in another thread is nicely described
            // here: http://reedcopsey.com/2011/11/28/launching-a-wpf-window-in-a-separate-thread-part-1/
            var crashDialogThread = new Thread(CrashReportDialogThreadProc);
            crashDialogThread.SetApartmentState(ApartmentState.STA);
            crashDialogThread.IsBackground = true;
            crashDialogThread.Start(exception);
            crashDialogThread.Join();
            Shutdown(-1);
        }

        private void CrashReportDialogThreadProc(object theException)
        {
            // Create and install synchronization context.
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            var exception = theException as Exception;
            var errorShowingCrashDialogMessage = string.Empty;

            try
            {
                // Show the dialog using BeginInvoke so WPF can correctly apply theme / style (if necessary).
                // Without this, the thread will throw a CheckAccess violation when setting the style.
                // NOTE: Assets in the style / theme must be frozen for this thread to be able to use the style / theme.
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    var crashDialog = INTV.Shared.View.ReportDialog.Create(null, null);
                    crashDialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    (crashDialog.DataContext as INTV.Shared.ViewModel.ReportDialogViewModel).Exception = exception;

                    // When the window closes, shut down the dispatcher
                    crashDialog.Closed += (s, x) =>
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    };

                    if (!AlreadyDisplayedExceptionDialog)
                    {
                        AlreadyDisplayedExceptionDialog = true;
                        crashDialog.IsVisibleChanged += (s, e) => crashDialog.Activate();
                        crashDialog.ShowDialog();
                    }
                    else
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    }
                }));

                Dispatcher.Run();
            }
            catch (Exception e)
            {
                errorShowingCrashDialogMessage = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Original Exception Type: {0}{1}Exception While Displaying Dialog: ", exception == null ? "<null>" : exception.GetType().FullName, Environment.NewLine);
                try
                {
                    errorShowingCrashDialogMessage += e.ToString();
                }
                catch (Exception)
                {
                    errorShowingCrashDialogMessage += "Unknown error -- Cannot ToString() the new dialog exception.";
                }
                System.Windows.MessageBox.Show(errorShowingCrashDialogMessage, "Failed to Display Crash Report", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Track application startup to ensure startup actions are only executed when the app is truly ready.
        /// </summary>
        [Flags]
        private enum AppReadyState
        {
            /// <summary>
            /// Application not ready.
            /// </summary>
            None,

            /// <summary>
            /// Application's main window has been marked visible.
            /// </summary>
            MainWindowVisible = 1 << 0,

            /// <summary>
            /// Application's main window is loaded.
            /// </summary>
            MainWindowLoaded = 1 << 1,

            /// <summary>
            /// Application's main window has a source (HWND).
            /// </summary>
            MainWindowSourced = 1 << 2,

            /// <summary>
            /// Application's MEF imports have been satisfied.
            /// </summary>
            ImportsStatisfied = 1 << 3,

            /// <summary>
            /// Indicates application is ready to execute startup actions.
            /// </summary>
            Ready = MainWindowVisible | MainWindowLoaded | MainWindowSourced | ImportsStatisfied
        }
    }
}

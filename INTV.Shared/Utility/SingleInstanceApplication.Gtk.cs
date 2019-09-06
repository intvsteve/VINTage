// <copyright file="SingleInstanceApplication.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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

using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.Properties;
using INTV.Shared.View;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class SingleInstanceApplication
    {
        private const Gdk.WindowState SupportedRestoreStates = Gdk.WindowState.Maximized | Gdk.WindowState.Fullscreen;

        private static readonly SingleInstanceApplication GtkInstance = new SingleInstanceApplication();

        private static bool _initiatedExit;

        #region Properties

        /// <summary>
        /// Gets the strongly typed version of the application instance.
        /// </summary>
        public static SingleInstanceApplication Current
        {
            get { return GtkInstance; }
        }

        internal static string SplashScreenResource
        {
            get { return _splashScreenResource; }
        }

        internal static System.Type MainWindowType
        {
            get { return _mainWindowType; }
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        public Gtk.Window MainWindow
        {
            get
            {
                var window = _mainWindow;
                if (window == null)
                {
                    window = Gtk.Window.ListToplevels().FirstOrDefault(w => w is IFakeDependencyObject);
                }
                return window;
            }

            private set
            {
                _mainWindow = value;
            }
        }
        private Gtk.Window _mainWindow;

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised when the application will exit.
        /// </summary>
        public event System.EventHandler<ExitEventArgs> Exit;

        #endregion // Events

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <typeparam name="T">The type for the main window class.</typeparam>
        /// <param name="uniqueInstance">Unique instance string.</param>
        /// <param name="applicationInfo">The application description.</param>
        /// <param name="args">Command line arguments.</param>
        /// <param name="splashScreenImage">Splash screen image.</param>
        public static void RunApplication<T>(string uniqueInstance, IApplicationInfo applicationInfo, string[] args, string splashScreenImage) where T : Gtk.Window, IMainWindow, new()
        {
            // TODO: Single instance check / activate
            // Documentation of Gtk.Application.Init(progname, ref args) implies that
            // a single instance should be enforced. Sadly, that does not seem to work
            // as expected. Perhaps it's the C#iness of it all.

            // show splash screen
            _mainWindowType = typeof(T);
            _splashScreenResource = splashScreenImage;
            Gtk.Application.Init(uniqueInstance, ref args);

            Instance.AppInfo = applicationInfo;
            SettingsBase.LoadApplicationSettings();
            var window = new T();
            Instance.MainWindow = window;
            Instance.ReadyState |= AppReadyState.MainWindowSourced;

            Gtk.Window.DefaultIcon = window.Icon;
            var splashImage = _mainWindowType.LoadImageResource(_splashScreenResource);
            var splashScreen = new SplashScreen(splashImage); // creating also shows
            splashScreen.TransientFor = window;
            window.Data["SplashScreen"] = splashScreen;
            GLib.Timeout.Add(1000, FinishInitialization);
            Gtk.Application.Run();
            SettingsBase.SaveApplicationSettings();
        }

        private static bool FinishInitialization()
        {
            var settings = Instance.AppInfo.Settings;
            Instance.Initialize(settings);
            var window = Instance.MainWindow;
            InitializeMainWindow(window);
            return false;
        }

        private static void InitializeMainWindow(Gtk.Window window)
        {
            RestoreWindowState(window);

            var mainWindow = window as IMainWindow;
            var primaryComponents = CompositionHelpers.Container.GetExportedValues<IPrimaryComponent>();
            foreach (var component in primaryComponents)
            {
                mainWindow.AddPrimaryComponentVisuals(component, component.GetVisuals());
            }

            var commandProviders = Instance.CommandProviders;
            window.AddCommandsToMainWindow(commandProviders.Select(c => c.Value));
            Instance.ReadyState |= AppReadyState.MainWindowLoaded;

            window.ConfigureEvent += HandleConfigureEvent;
            window.Shown += HandleWindowShown;
            window.DeleteEvent += HandleMainWindowClosed;
            window.Destroyed += HandleMainWindowDestroyed;
            window.WindowStateEvent += HandleMainWindowStateChanged;
            window.ShowAll();
        }

        private static void RestoreWindowState(Gtk.Window window)
        {
            var position = Properties.Settings.Default.WindowPosition;
            var positionValid = false;
            foreach (var desktopWorkArea in Gdk.Global.DesktopWorkareas)
            {
                positionValid = desktopWorkArea.Contains(position);
                if (positionValid)
                {
                    break;
                }
            }
            if (!positionValid)
            {
                var desktop = Gdk.Global.CurrentDesktop;
                var currentDesktopWorkArea = Gdk.Global.DesktopWorkareas[desktop];
                position.X = currentDesktopWorkArea.Left + 40;
                position.Y = currentDesktopWorkArea.Top + 40;
            }
            var size = Properties.Settings.Default.WindowSize;
            var state = Properties.Settings.Default.WindowState;

            window.Move(position.X, position.Y);
            window.Resize(size.Width, size.Height);
            if (state.HasFlag(Gdk.WindowState.Maximized))
            {
                window.Maximize();
            }
            if (state.HasFlag(Gdk.WindowState.Fullscreen))
            {
                window.Fullscreen();
            }
        }

        private static void HandleMainWindowStateChanged(object o, Gtk.WindowStateEventArgs args)
        {
            var window = o as Gtk.Window;
            var windowState = args.Event.NewWindowState & SupportedRestoreStates;
            window.Data[Properties.Settings.WindowStateSettingName] = windowState;
        }

        /// <summary>
        /// Handles the configure event to keep track of size and position for next launch.
        /// </summary>
        /// <param name="o">Object sending the event.</param>
        /// <param name="args">Data for the ConfigureEvent.</param>
        /// <remarks>By default, the event handler for ConfigureEvent will not be called unless
        /// the implementation class has overridden OnConfigureEvent to return <c>false</c>.
        /// Although overriding the OnConfigureEvent to return <c>false</c>works just fine, we're adding
        /// behavior and don't necessarily have access to the implementation of the Window.
        /// (OK, we do, but it's the principle of the thing here.) The documentation of
        /// GLib.ConnectBefore suggests using that attribute breaks enapsulation:
        /// <see href="http://docs.go-mono.com/index.aspx?link=T%3aGLib.ConnectBeforeAttribute"/>.
        /// This solution was found here: <see href="https://stackoverflow.com/questions/16778919/f-mono-gtk-configureevent"/>.
        /// IMO the current behavior seems like a bug -- it's not documented to require this kind of trickery,
        /// and the technique seems to crop up more often than you'd think for something that's considered
        /// borderline bad practice. OTOH, the frequency of ConfigureEvent could have performance implications, so
        /// perhaps the thinking was that you need to work at it to get into trouble, instead of making it too easy.</remarks>
        [GLib.ConnectBefore]
        private static void HandleConfigureEvent(object o, Gtk.ConfigureEventArgs args)
        {
            // HACK Haven't found another way to get both position and size update information to
            // try to retain the 'restored' window size, so we do this hacky thing. The hacky thing
            // is more extensively documented in the ConfigureUpdateEventTimer class.
            var window = o as Gtk.Window;
            ConfigureUpdateEventTimer.Validate(window);
        }

        private static void HandleWindowShown(object sender, System.EventArgs e)
        {
            var window = sender as Gtk.Window;
            window.Shown -= HandleWindowShown;
            var splashScreen = window.Data["SplashScreen"] as Gtk.Window;
            window.Data.Remove("SplashScreen");
            GLib.Timeout.Add(
                1000,
                () =>
                {
                    splashScreen.Hide();
                    splashScreen.Destroy();
                    return false;
                });
            Instance.ReadyState |= AppReadyState.MainWindowVisible;
        }

        private static void HandleMainWindowDestroyed(object sender, System.EventArgs e)
        {
            var window = sender as Gtk.Window;
            HandleExitApplication(window);
        }

        private static void HandleMainWindowClosed(object o, Gtk.DeleteEventArgs args)
        {
            var window = o as Gtk.Window;
            HandleExitApplication(window);
        }

        private static void HandleExitApplication(Gtk.Window window)
        {
            if (!_initiatedExit)
            {
                _initiatedExit = true;
                var exit = Instance.Exit;
                if (exit != null)
                {
                    exit(Instance, new ExitEventArgs());
                }
                if (window.Data.ContainsKey(Properties.Settings.WindowPositionSettingName))
                {
                    var restorePosition = (Gdk.Point)window.Data[Properties.Settings.WindowPositionSettingName];
                    Properties.Settings.Default.WindowPosition = restorePosition;
                }
                if (window.Data.ContainsKey(Properties.Settings.WindowSizeSettingName))
                {
                    var restoreSize = (Gdk.Size)window.Data[Properties.Settings.WindowSizeSettingName];
                    Properties.Settings.Default.WindowSize = restoreSize;
                }
                if (window.Data.ContainsKey(Properties.Settings.WindowStateSettingName))
                {
                    var restoreState = (Gdk.WindowState)window.Data[Properties.Settings.WindowStateSettingName] & SupportedRestoreStates;
                    Properties.Settings.Default.WindowState = restoreState;
                }
                Gtk.Application.Quit();
            }
        }

        private static void OnDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            OSDispatcher.Current.InvokeOnMainDispatcher(() => ReportDomainUnhandledException(e.ExceptionObject as System.Exception));
        }

        private static void HandleGLibUnhandledException(GLib.UnhandledExceptionArgs args)
        {
            OnDomainUnhandledException(null, args);
        }

        private static void ReportDomainUnhandledException(System.Exception exception)
        {
            var crashDialog = INTV.Shared.View.ReportDialog.Create(null, null);
            (crashDialog.DataContext as INTV.Shared.ViewModel.ReportDialogViewModel).Exception = exception;

            if (!AlreadyDisplayedExceptionDialog)
            {
                AlreadyDisplayedExceptionDialog = true;
                crashDialog.ShowDialog(Resources.Strings.ReportDialog_Exit);
                if (!_initiatedExit)
                {
                    _initiatedExit = true;
                    Gtk.Application.Quit(); // sometimes this doesn't exit??? TODO: Raise Exit event here???
                }
            }
        }

        private static string GetPluginsDirectory()
        {
            string pluginsDirectory = null;
            try
            {
                var pluginsBasePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
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
            catch (System.Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to identify plugins directory.");
            }
            return pluginsDirectory;
        }

        /// <summary>
        /// Spawns the startup actions.
        /// </summary>
        private void SpawnStartupActions()
        {
            MainThreadDispatcher.BeginInvoke(new System.Action(() => ExecuteStartupActions()));
        }

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        partial void OSInitialize()
        {
            GLib.ExceptionManager.UnhandledException += HandleGLibUnhandledException;
            _initiatedExit = false;
            _programDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            ////_settings.Add(INTV.Shared.Properties.Settings.Default);
        }

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        partial void OSOnImportsSatisfied()
        {
            // TODO: Determine if these are useless in GTK.
            foreach (var configuration in Configurations.OrderBy(c => c.Metadata.Weight))
            {
                var settings = configuration.Value.Settings as ISettings;
                if (settings != null)
                {
                    AddSettings(settings);
                }
            }
        }

        /// <summary>
        /// HACK class to try to maintain size and position information for window restoration.
        /// </summary>
        /// <remarks>See the extensive notes on the Timer() method.</remarks>
        private class ConfigureUpdateEventTimer
        {
            private const string DataName = "ConfigureUpdateEventTimer";

            private ConfigureUpdateEventTimer(Gtk.Window window)
            {
                Window = window;
            }

            private Gtk.Window Window { get; }
            private bool Continue { get; set; }
            private uint Timer { get; set; }

            internal static void Validate(Gtk.Window window)
            {
                if (window.Data.ContainsKey(DataName))
                {
                    var timer = (ConfigureUpdateEventTimer)window.Data[DataName];
                    timer.Continue = true;
                }
                else
                {
                    var timer = new ConfigureUpdateEventTimer(window);
                    window.Data[DataName] = timer;
                    timer.Timer = GLib.Timeout.Add(248, timer.Tick);
                }
            }

            /// <summary>
            /// Update window size and position stashed for preferences.
            /// </summary>
            /// <returns><c>true</c> if this timer function should continue to be executed.</returns>
            /// <remarks>This is a HACK. Since we get ConfigureEvent updates of intermediate
            /// values during window state transitions to maximize, iconify, etc. we cannot
            /// rely upon those values. We also don't get notifications for simple movement
            /// of the window, either, unlike size requests. And the final straw is that we
            /// also cannot get the 'restore' size and position. So, the HACK is thus:
            /// When a ConfigureEvent arrives, if we haven't already, attach this timer
            /// object to the window. Otherwise, mark the timer's Continue value to <c>true</c>.
            /// When the timer fires, we *MIGHT* update the stored position and size.
            /// The kernel of this HACK is that WindowStateEvent fires in the time between the
            /// last 'valid' ConfigureEvent and the first firing of the timer.</remarks>
            private bool Tick()
            {
                var currentState = (Gdk.WindowState)Window.Data[Properties.Settings.WindowStateSettingName];
                var ignore = (currentState & (SupportedRestoreStates | Gdk.WindowState.Iconified)) != 0;
                var keepGoing = Continue && !ignore;
                Continue = false;
                if (!ignore)
                {
                    int width = 0;
                    int height = 0;
                    Window.GetSize(out width, out height);
                    var newSize = new Gdk.Size(width, height);
                    Window.Data[Properties.Settings.WindowSizeSettingName] = newSize;

                    int top = 0;
                    int left = 0;
                    Window.GetPosition(out left, out top);
                    var newPosition = new Gdk.Point(left, top);
                    Window.Data[Properties.Settings.WindowPositionSettingName] = newPosition;
                }
                if (!keepGoing)
                {
                    Window.Data.Remove(DataName);
                }
                return keepGoing;
            }
        }
    }
}

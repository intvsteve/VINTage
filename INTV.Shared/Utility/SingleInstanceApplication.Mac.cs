// <copyright file="SingleInstanceApplication.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

#if !__UNIFIED__
using INSApplicationDelegate = MonoMac.AppKit.NSApplicationDelegate;
#endif // !__UNIFIED__

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    [Register("SingleInstanceApplication")]
    public partial class SingleInstanceApplication : NSApplication, IFakeDependencyObject, System.ComponentModel.Composition.IPartImportsSatisfiedNotification
    {
        private const string ShowSplashScreenName = "LUI_SHOW_SPLASH_SCREEN";
        private const string MainWindowValueName = "mainWindow";
        private const string FirstResponderValueName = "firstResponder";
        private static bool AlreadyDisplayedExceptionDialog { get; set; }
        private static System.Configuration.ApplicationSettingsBase _launchSettings;
        private SplashScreen _splashScreen;
        private bool _registeredObserver;
        private bool _handledDidFinishLaunching;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.SingleInstanceApplication"/> class.
        /// </summary>
        /// <param name="handle">Handle.</param>
        /// <remarks>Called when created from unmanaged code.</remarks>
        public SingleInstanceApplication(System.IntPtr handle)
            : base(handle)
        {
            Initialize(_launchSettings);
            _launchSettings = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.SingleInstanceApplication"/> class.
        /// </summary>
        /// <param name="coder">Coder.</param>
        /// <remarks>Called when created directly from a XIB file.</remarks>
        [Export("initWithCoder:")]
        public SingleInstanceApplication(NSCoder coder)
            : base(coder)
        {
            Initialize(_launchSettings);
            _launchSettings = null;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Strongly typed version of the application instance.
        /// </summary>
        public static SingleInstanceApplication Current { get { return NSApplication.SharedApplication as SingleInstanceApplication; } }

        /// <inheritdoc />
        public override NSWindow MainWindow
        {
            get
            {
                var window = base.MainWindow;
                if (window == null)
                {
                    window = Windows.FirstOrDefault(w => w is IFakeDependencyObject);
                }
                return window;
            }
        }

        /// <summary>
        /// Gets or sets the last key pressed.
        /// </summary>
        public ushort LastKeyPressed { get; private set; }

        /// <summary>
        /// Gets the timestamp of the last key pressed event.
        /// </summary>
        public double LastKeyPressedTimestamp { get; private set; }

        #region IFakeDependencyObject Properties

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        #endregion // IFakeDependencyObject Properties

        /// <summary>
        /// Gets or sets the delegate.  (General paranoia about MonoMac's NSObject lifetime management.
        /// </summary>
        private INSApplicationDelegate TheDelegate { get; set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised when the application will exit.
        /// </summary>
        public event System.EventHandler<ExitEventArgs> Exit;

        #endregion // Events

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object GetValue (string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        #region IPartImportsSatisfiedNotification

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            try
            {
                InitializePlugins();
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error initializing plugins: " + e.Message);
            }
        }

        #endregion // IPartImportsSatisfiedNotification

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="uniqueInstance">Unique instance string.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="args">Command line arguments.</param>
        /// <param name="splashScreenImage">Splash screen image.</param>
        /// <typeparam name="T">The type for the main window class.</typeparam>
        public static void RunApplication<T>(string uniqueInstance, System.Configuration.ApplicationSettingsBase settings, string[] args, string splashScreenImage) where T : NSWindow
        {
            _mainWindowType = typeof(T);
            _splashScreenResource = splashScreenImage;
            NSApplication.Init();

            // Prevent two copies from running. In order to access NSWorkspace, we have to at least call NSApplication.Init().
            NSUrl previousInstance = null;
            try
            {
                var launchedApps = NSWorkspace.SharedWorkspace.LaunchedApplications;
                if (launchedApps != null)
                {
                    var instanceAlreadyRunning = false;
                    foreach (var launchedApp in launchedApps)
                    {
                        NSObject launchedAppValue;
                        if (launchedApp.TryGetValue((NSString)"NSApplicationBundleIdentifier", out launchedAppValue))
                        {
                            // If bundle identifiers match, then check to see if it's running from another location.
                            var mainBundle = NSBundle.MainBundle;
                            if ((mainBundle.BundleIdentifier == (NSString)launchedAppValue) && launchedApp.TryGetValue((NSString)"NSApplicationPath", out launchedAppValue))
                            {
                                // Found an instance -- check to see if it's an instance from another location.
                                // If so, just activate that one and let this one fizzle out.
                                instanceAlreadyRunning = mainBundle.BundlePath != (NSString)launchedAppValue;
                            }
                        }
                        if (instanceAlreadyRunning)
                        {
                            previousInstance = new NSUrl((NSString)launchedAppValue);
                            break;
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // If this messes up, just try to launch normally.
                previousInstance= null;
            }

            if (previousInstance != null)
            {
                // App is already running -- but from another location. So, just bring that one to front.
                NSWorkspace.SharedWorkspace.LaunchApplication(previousInstance.AbsoluteString);
            }
            else
            {
                NSApplication.Main(args);
            }
        }

        #region NSApplication Overrides

        /// <summary>
        /// Shows the help.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <remarks>Perhaps this is not provided as a standard method because you're encouraged to
        /// buy into the Mac help system? In any case, exporting this method in this manner overrides
        /// the default Objective-C message (instance method) so the system calls it.</remarks>
        [OSExport("showHelp:")]
        public void ShowHelp(NSObject sender)
        {
            var helpUrl = AppInfo.OnlineHelpUrl;
            if (!string.IsNullOrEmpty(helpUrl))
            {
                System.Diagnostics.Process.Start(helpUrl);
            }
        }

        /// <inheritdoc />
        public override void SendEvent(NSEvent theEvent)
        {
            try
            {
                if (theEvent.Handle == System.IntPtr.Zero)
                {
                    return;
                }
                if (theEvent.Type == NSEventType.KeyDown)
                {
                    LastKeyPressed = theEvent.KeyCode;
                    LastKeyPressedTimestamp = theEvent.Timestamp;
                }
                base.SendEvent(theEvent);
            }
            catch (System.InvalidCastException e)
            {
                System.Diagnostics.Debug.WriteLine("Cast failed?! Er, what?: " + e.Message);
            }
        }

        /// <inheritdoc />
        /// <remarks>We're interested in a few specific things happening in the application. We need to do
        /// some extra work when main window is actually assigned, and we also need to know when the
        /// FirstResponder changes so we can appropriately update command availability states.</remarks>
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, System.IntPtr context)
        {
            if (context == this.Handle)
            {
                var changeKindNumber = change.ValueForKey(NSObject.ChangeKindKey) as NSNumber;
#if __UNIFIED__
                var changeKind = (NSKeyValueChange)changeKindNumber.Int32Value;
#else
                var changeKind = (NSKeyValueChange)changeKindNumber.IntValue;
#endif // __UNIFIED__
                NSObject newValue = null;
                switch (changeKind)
                {
                    case NSKeyValueChange.Setting:
                        newValue = change.ValueForKey(NSObject.ChangeNewKey);
                        break;
                    case NSKeyValueChange.Insertion:
                    case NSKeyValueChange.Removal:
                    case NSKeyValueChange.Replacement:
                        break;
                }
                if (newValue != null)
                {
                    switch (keyPath)
                    {
                        case MainWindowValueName:
                            var myMainWindow = newValue as NSWindow;
                            if ((myMainWindow != null) && !_registeredObserver)
                            {
                                _registeredObserver = true;
                                myMainWindow.AddObserver(this, (NSString)FirstResponderValueName, NSKeyValueObservingOptions.New, this.Handle);
                                BeginInvokeOnMainThread(() => HandleDidFinishLaunching(this, System.EventArgs.Empty));
                            }
                            break;
                        case FirstResponderValueName:
                            // When called early during startup, it's possible the MonoMac system hasn't fully initialized yet, which
                            // can cause spurious "InvalidCast" errors.
                            BeginInvokeOnMainThread(() => INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested());
                            break;
                    }
                }
            }
        }

        #endregion // NSApplication Overrides

        public void RaiseApplicationExit()
        {
            HandleWillTerminate(this, System.EventArgs.Empty);
        }

        partial void OSInitialize()
        {
            // There's an annoying mono-ism that can occur that seems innocuous.
            // See discussion here: http://forums.xamarin.com/discussion/2065/assertion-at-debugger-agent-c
            // Since it seems harmless, going to keep this mechanism that disables the attempt to
            // generate a compiled assembly on-the-fly put away for now.
            // Environment.SetEnvironmentVariable("MONO_XMLSERIALIZER_THS", "no");
            TheDelegate = this.Delegate;
            var mainBundle = NSBundle.MainBundle;
            PluginsLocation = GetPluginsDirectory();
            var shouldShowSplashScreen = true;
            var shouldShowSplashScreenString = mainBundle.GetEnvironmentValue<string>(ShowSplashScreenName);
            if (!string.IsNullOrEmpty(shouldShowSplashScreenString))
            {
                bool environmentValue;
                if (!bool.TryParse(shouldShowSplashScreenString, out environmentValue))
                {
                    shouldShowSplashScreen = string.Compare(shouldShowSplashScreenString, "yes", true) == 0;
                }
                else
                {
                    shouldShowSplashScreen = environmentValue;
                }
            }

            if (shouldShowSplashScreen)
            {
                _splashScreen = SplashScreen.Show(_splashScreenResource);
            }
            _programDirectory = System.IO.Path.GetDirectoryName(mainBundle.BundlePath);
#if !__UNIFIED__
            WillTerminate += HandleWillTerminate;
            this.ApplicationShouldTerminateAfterLastWindowClosed = ApplicationShouldTerminateAfterLastWindowClosedPredicate;
            WillPresentError = OnWillPresentError;
#endif // !__UNIFIED__
            AddObserver(this, (NSString)MainWindowValueName, NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, this.Handle);
        }

        private static void OnDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            if (NSThread.IsMain)
            {
                ReportDomainUnhandledException(e.ExceptionObject as System.Exception);
            }
            else
            {
                Current.BeginInvokeOnMainThread (() => ReportDomainUnhandledException(e.ExceptionObject as System.Exception));
            }
        }

        private static void ReportDomainUnhandledException(System.Exception exception)
        {
            var crashDialog = INTV.Shared.View.ReportDialog.Create (null, null);
            (crashDialog.DataContext as INTV.Shared.ViewModel.ReportDialogViewModel).Exception = exception;

            if (!AlreadyDisplayedExceptionDialog) {
                AlreadyDisplayedExceptionDialog = true;
                crashDialog.ShowDialog (Resources.Strings.ReportDialog_Exit);
            }
        }

#if !__UNIFIED__
        private bool ApplicationShouldTerminateAfterLastWindowClosedPredicate(NSApplication sender)
        {
            return true;
        }

        private static NSError OnWillPresentError(NSApplication application, NSError error)
        {
            return error;
        }
#endif // !__UNIFIED__

        private void HandleDidFinishLaunching(object sender, System.EventArgs e)
        {
            if ((MainWindow != null) && MainWindow.IsVisible && !(MainWindow is NSPanel))
            {
                if (!_handledDidFinishLaunching)
                {
                    _handledDidFinishLaunching = true;
                    if (_splashScreen != null)
                    {
                        BeginInvokeOnMainThread(() => {
                            if (_splashScreen != null)
                            {
                                System.Threading.Thread.Sleep(333);
                                _splashScreen.Hide();
                            }
                            _splashScreen = null;
                        });
                    }

                    // Consider putting in a delay here? I.e. instead of BeginInvokeOnMainThread(), use
                    // PerformSelector() with a delay? Is there a race condition that even BeginInvoke() isn't
                    // getting around w.r.t. the Objective-C initialization and the work the startup actions
                    // may need to do? (E.g. the case of NSUserDefaults.SetBool() crashing somewhere in its guts.)
                    BeginInvokeOnMainThread(() => {
                        ExecuteStartupActions();
                        CommandManager.InvalidateRequerySuggested();
                    });
                }
            }
            else
            {
                BeginInvokeOnMainThread(() => HandleDidFinishLaunching(this, System.EventArgs.Empty));
            }
        }

        private void HandleWillTerminate(object sender, System.EventArgs e)
        {
            var exit = Exit;
            if (exit != null)
            {
                exit(this, new ExitEventArgs());
            }
        }

        private static string GetPluginsDirectory()
        {
            string appPluginsDir = null;
            try
            {
                NSError error;
                var appSupportDir = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User, null, true, out error);
                if ((appSupportDir != null) && (error == null))
                {
                    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    var appPluginsDirName = System.IO.Path.GetFileNameWithoutExtension(entryAssembly.Location);
                    appSupportDir = appSupportDir.Append(appPluginsDirName, true);
                    appSupportDir = appSupportDir.Append("Plugins", true);
                    if (NSFileManager.DefaultManager.CreateDirectory(appSupportDir, true, null, out error))
                    {
                        appPluginsDir = appSupportDir.Path;
                    }
                }
            }
            catch (System.Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialize plugins directory.");
            }
            return appPluginsDir;
        }
    }
}

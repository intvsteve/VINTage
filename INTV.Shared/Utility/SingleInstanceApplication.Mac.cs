// <copyright file="SingleInstanceApplication.Mac.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    [MonoMac.Foundation.Register("SingleInstanceApplication")]
    public partial class SingleInstanceApplication : MonoMac.AppKit.NSApplication, IFakeDependencyObject, IPartImportsSatisfiedNotification
    {
        private const string MainWindowValueName = "mainWindow";
        private const string FirstResponderValueName = "firstResponder";
        private static bool AlreadyDisplayedExceptionDialog { get; set; }
        private static System.Configuration.ApplicationSettingsBase _launchSettings;
        private static string _splashScreenResource;
        private SplashScreen _splashScreen;
        private bool _registeredObserver;
        private bool _handledDidFinishLaunching;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.SingleInstanceApplication"/> class.
        /// </summary>
        /// <param name="handle">Handle.</param>
        /// <remarks>Called when created from unmanaged code.</remarks>
        public SingleInstanceApplication(IntPtr handle)
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
        [MonoMac.Foundation.Export("initWithCoder:")]
        public SingleInstanceApplication(MonoMac.Foundation.NSCoder coder)
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
        private NSApplicationDelegate TheDelegate { get; set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised when the application will exit.
        /// </summary>
        public event EventHandler<ExitEventArgs> Exit;

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
        public static void RunApplication<T>(string uniqueInstance, System.Configuration.ApplicationSettingsBase settings, string[] args, string splashScreenImage) where T : MonoMac.AppKit.NSWindow
        {
            _splashScreenResource = splashScreenImage;
            MonoMac.AppKit.NSApplication.Init();
            MonoMac.AppKit.NSApplication.Main(args);
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
                if (theEvent.Handle == IntPtr.Zero)
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
            catch (InvalidCastException e)
            {
                System.Diagnostics.Debug.WriteLine("Cast failed?! Er, what?: " + e.Message);
            }
        }

        /// <inheritdoc />
        /// <remarks>We're interested in a few specific things happening in the application. We need to do
        /// some extra work when main window is actually assigned, and we also need to know when the
        /// FirstResponder changes so we can appropriately update command availability states.</remarks>
        public override void ObserveValue(MonoMac.Foundation.NSString keyPath, MonoMac.Foundation.NSObject ofObject, MonoMac.Foundation.NSDictionary change, IntPtr context)
        {
            if (context == this.Handle)
            {
                var changeKindNumber = change.ValueForKey(NSObject.ChangeKindKey) as NSNumber;
                var changeKind = (NSKeyValueChange)changeKindNumber.IntValue;
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
                                BeginInvokeOnMainThread(() => HandleDidFinishLaunching(this, EventArgs.Empty));
                            }
                            break;
                        case FirstResponderValueName:
                            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                            break;
                    }
                }
            }
        }

        #endregion // NSApplication Overrides

        partial void OSInitialize()
        {
            // There's an annoying mono-ism that can occur that seems innocuous.
            // See discussion here: http://forums.xamarin.com/discussion/2065/assertion-at-debugger-agent-c
            // Since it seems harmless, going to keep this mechanism that disables the attempt to
            // generate a compiled assembly on-the-fly put away for now.
            // Environment.SetEnvironmentVariable("MONO_XMLSERIALIZER_THS", "no");
            _splashScreen = SplashScreen.Show(_splashScreenResource);
            TheDelegate = this.Delegate;
            var mainBundle = MonoMac.Foundation.NSBundle.MainBundle;
            _programDirectory = System.IO.Path.GetDirectoryName(mainBundle.BundlePath);
            WillTerminate += HandleWillTerminate;
            this.ApplicationShouldTerminateAfterLastWindowClosed = ApplicationShouldTerminateAfterLastWindowClosedPredicate;
            AddObserver(this, (MonoMac.Foundation.NSString)MainWindowValueName, MonoMac.Foundation.NSKeyValueObservingOptions.New | MonoMac.Foundation.NSKeyValueObservingOptions.Initial, this.Handle);
            WillPresentError = OnWillPresentError;
        }

        private static NSError OnWillPresentError(NSApplication application, NSError error)
        {
            return error;
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            var crashDialog = INTV.Shared.View.ReportDialog.Create(null, null);
            (crashDialog.DataContext as INTV.Shared.ViewModel.ReportDialogViewModel).Exception = e.ExceptionObject as Exception;

            if (!AlreadyDisplayedExceptionDialog)
            {
                AlreadyDisplayedExceptionDialog = true;
                crashDialog.ShowDialog(Resources.Strings.ReportDialog_Exit);
            }
        }

        private bool ApplicationShouldTerminateAfterLastWindowClosedPredicate(NSApplication sender)
        {
            return true;
        }

        private void HandleDidFinishLaunching(object sender, EventArgs e)
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
                BeginInvokeOnMainThread(() => HandleDidFinishLaunching(this, EventArgs.Empty));
            }
        }

        private void HandleWillTerminate (object sender, EventArgs e)
        {
            var exit = Exit;
            if (exit != null)
            {
                exit(this, new ExitEventArgs());
            }
        }
    }
}

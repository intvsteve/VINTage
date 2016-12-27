// <copyright file="SingleInstanceApplication.cs" company="INTV Funhouse">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Device;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Program;
using INTV.Shared.Resources;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// This class provides a simple way to determine whether an application is being launched for the first time
    /// activate the main window of the other instance. It presumes that the main window will react to a unique
    /// message broadcast throughout the system.
    /// </summary>
    public partial class SingleInstanceApplication
    {
        private const string CheckForUpdatesStartupActionName = "CheckForUpdates";

        private static OSDispatcher _mainDispatcher;
        private static Type _mainWindowType;
        private static string _splashScreenResource;
        private static string _versionString;

        // These actions are executed when main application launch is done. The Tuple describes an Action to execute, and
        // the priority of the task's execution. Typically, synchronous startup actions involve
        // actions that must not run in parallel, such as updating file formats, et. al. The "asynchronous" actions are
        // usually activities such as ROM or device discovery, which are relatively low impact.
        private static ConcurrentDictionary<string, Tuple<Action, StartupTaskPriority>> _postStartupActions = new ConcurrentDictionary<string, Tuple<Action, StartupTaskPriority>>();
        private List<System.Configuration.ApplicationSettingsBase> _settings;

        #region Properties

        /// <summary>
        /// Gets the instance of SingleInstanceApplication.
        /// </summary>
        public static SingleInstanceApplication Instance
        {
            get { return Current as SingleInstanceApplication; }
        }

        /// <summary>
        /// Gets the version of the application in the form Major.Minor.Revision (additional info).
        /// </summary>
        public static string Version
        {
            get
            {
                var versionString = _versionString;
                if (string.IsNullOrEmpty(_versionString))
                {
                    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    versionString = System.Diagnostics.FileVersionInfo.GetVersionInfo(entryAssembly.Location).ProductVersion;
                }
                return versionString;
            }
        }

        /// <summary>
        /// Gets the copyright of the application.
        /// </summary>
        public static string Copyright
        {
            get
            {
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                var copyright = entryAssembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false).OfType<System.Reflection.AssemblyCopyrightAttribute>().FirstOrDefault();
                return copyright.Copyright + " Steven A. Orth";
            }
        }

        /// <summary>
        /// Gets the dispatcher for the main thread of the application.
        /// </summary>
        public static OSDispatcher MainThreadDispatcher
        {
            get { return _mainDispatcher; }
        }

        /// <summary>
        /// Gets the shared settings for the INTV.Shared assembly.
        /// </summary>
        public static SharedSettings SharedSettings
        {
            get { return SharedSettings.Default; }
        }

        /// <summary>
        /// Gets the settings associated with the application.
        /// </summary>
        public IEnumerable<System.Configuration.ApplicationSettingsBase> Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Gets the directory in which the application executable exists. On Mac, this means the application bundle.
        /// </summary>
        public string ProgramDirectory
        {
            get { return _programDirectory; }
        }
        private string _programDirectory;

        /// <summary>
        /// Gets or sets a value indicating whether the application is busy in an action that should block the UI.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                lock (this)
                {
                    return _busy > 0;
                }
            }

            set
            {
                lock (this)
                {
                    if (value)
                    {
                        ++_busy;
                    }
                    else
                    {
                        --_busy;
                    }
#if MAC
                    if ((_busy == 0) || (_busy == 1))
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }
#endif // Mac
                }
            }
        }
        private int _busy;

        /// <summary>
        /// Gets or sets the application information interface.
        /// </summary>
        [System.ComponentModel.Composition.Import]
        public IApplicationInfo AppInfo { get; set; }

        // UNDONE One day, perhaps we can get these via MEF. Not ready yet, though.
        // [System.ComponentModel.Composition.ImportMany]
        // public IEnumerable<Lazy<IPrimaryComponent>> Components;

        /// <summary>
        /// Gets the ROM list.
        /// </summary>
        [System.ComponentModel.Composition.Import]
        public ProgramCollection Roms { get; private set; }

        /// <summary>
        /// Gets or sets the available configurations from various features in the application.
        /// </summary>
        [System.ComponentModel.Composition.ImportMany]
        public IEnumerable<Lazy<IConfiguration, IConfigurationMetadata>> Configurations { get; set; }

        /// <summary>
        /// Gets or sets the command providers.
        /// </summary>
        [System.ComponentModel.Composition.ImportMany]
        public IEnumerable<Lazy<ICommandProvider>> CommandProviders { get; set; }

        /// <summary>
        /// Gets the plugins location.
        /// </summary>
        internal string PluginsLocation { get; private set; }

        #endregion // Properties

        /// <summary>
        /// Add settings to the application which will be saved at shutdown.
        /// </summary>
        /// <param name="settings">Settings to associate with the application.</param>
        public void AddSettings(System.Configuration.ApplicationSettingsBase settings)
        {
            if ((settings != null) && !_settings.Contains(settings))
            {
                _settings.Add(settings);
            }
        }

        /// <summary>
        /// Adds an action to execute once the application's main window has finished loading.
        /// </summary>
        /// <param name="actionId">A unique identifier for the action.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="priority">The priority of the startup task.</param>
        public void AddStartupAction(string actionId, Action action, StartupTaskPriority priority)
        {
            _postStartupActions[actionId] = new Tuple<Action, StartupTaskPriority>(action, priority);
        }

        /// <summary>
        /// Gets the configuration of the given type.
        /// </summary>
        /// <returns>The configuration of the specified type, or <c>null</c> if not found.</returns>
        /// <typeparam name="T">The type of the configuration to get.</typeparam>
        public T GetConfiguration<T>() where T : IConfiguration
        {
            var configuration = Configurations.Select(c => c.Value).OfType<T>().FirstOrDefault();
            return configuration;
        }

        /// <summary>
        /// Gets the history of devices that have been connected to the application.
        /// </summary>
        /// <returns>An enumerable of the devices that have been connected to the application.</returns>
        public IEnumerable<IPeripheral> GetConnectedDevicesHistory()
        {
            var connectedDevicesHistory = Configurations.SelectMany(c => c.Value.ConnectedPeripheralsHistory);
            return connectedDevicesHistory;
        }

        /// <summary>
        /// Gets the command provider of the given type.
        /// </summary>
        /// <returns>The command provider of the specified type, or <c>null</c> if not found.</returns>
        /// <typeparam name="T">The type of the command provider to get.</typeparam>
        public T GetCommandProvider<T>() where T : ICommandProvider
        {
            var commandProvider = CommandProviders.Select(c => c.Value).OfType<T>().FirstOrDefault();
            return commandProvider;
        }

        private void Initialize(System.Configuration.ApplicationSettingsBase settings)
        {
            _settings = new System.Collections.Generic.List<System.Configuration.ApplicationSettingsBase>();
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            PluginsLocation = GetPluginsDirectory();
            OSInitialize();
            _mainDispatcher = OSDispatcher.Current;
            this.DoImport();
            if (settings != null)
            {
                _settings.Add(settings);
            }
            if (INTV.Shared.Properties.Settings.Default.CheckForAppUpdatesAtLaunch)
            {
                AddStartupAction(CheckForUpdatesStartupActionName, () => ApplicationCommandGroup.CheckForUpdatesCommand.Execute(true), StartupTaskPriority.LowestAsyncTaskPriority);
            }
        }

        private void ExecuteStartupActions()
        {
            var actions = new System.Collections.Generic.Dictionary<string, Tuple<Action, StartupTaskPriority>>(_postStartupActions);
            _postStartupActions.Clear();
            var failedActions = new List<Tuple<string, Exception>>();

            // First, do all the synchronous actions.
            foreach (var startupAction in actions.Where(a => a.Value.Item2 >= StartupTaskPriority.LowestSyncTaskPriority).OrderByDescending(a => a.Value.Item2))
            {
                MainThreadDispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            startupAction.Value.Item1();
                        }
                        catch (Exception error)
                        {
                            failedActions.Add(new Tuple<string, Exception>(startupAction.Key, error));
                        }
                    },
                    OSDispatcherPriority.Normal);
            }

            // Report any actions that may have failed.
            ReportStartupActionErrors(failedActions);

            // Next, launch the remaining (asynchronous) actions.
            foreach (var startupAction in actions.Where(a => a.Value.Item2 <= StartupTaskPriority.HighestAsyncTaskPriority).OrderByDescending(a => a.Value.Item2))
            {
                MainThreadDispatcher.BeginInvoke(() => StartupActionWrapper(startupAction.Value.Item1));
            }
        }

        private void StartupActionWrapper(Action startupAction)
        {
            try
            {
                startupAction();
            }
            catch (Exception)
            {
                // Let this silently fail in release builds.
#if DEBUG
                System.Diagnostics.Debug.Assert(false, "Failure in async startup action! Let's see what's going on here, shall we?");
#endif // DEBUG
            }
        }

        private void ReportStartupActionErrors(List<Tuple<string, Exception>> errors)
        {
            var errorsToReport = errors.Where(e => ShouldReportError(e.Item2));
            if (errorsToReport.Any())
            {
                var errorMessage = new System.Text.StringBuilder(Strings.StartupActionError_Summary).AppendLine();
                foreach (var error in errorsToReport)
                {
                    errorMessage.AppendLine().AppendFormat(Strings.StartupActionError_ActionNameMessageFormat, error.Item1).AppendLine();
                    errorMessage.AppendFormat(Strings.StartupActionError_ExceptionMessageFormat, error.Item2.Message).AppendLine();
                }
                errorMessage.AppendLine().AppendFormat(Strings.StartupActionError_Details).AppendLine();
                foreach (var error in errorsToReport)
                {
                    errorMessage.AppendLine().AppendFormat(Strings.StartupActionError_ActionNameMessageFormat, error.Item1).AppendLine();
                    errorMessage.AppendLine(error.Item2.ToString());
                }
                var errorDialog = INTV.Shared.View.ReportDialog.Create(Strings.StartupActionError_Title, Strings.StartupActionError_BaseMessage);
                errorDialog.ReportText = errorMessage.ToString();
                errorDialog.ShowDialog(Strings.Close);
            }
        }

        private bool ShouldReportError(Exception exception)
        {
            var reportError = true;
#if !DEBUG
            // On Mac, there is a rare error that that arises when launching and trying to set a user preference. (WTF?)
            // It may somehow be related to NSUserDefaults?
            reportError = !(exception is InvalidCastException);
#endif // !DEBUG
            return reportError;
        }

        /// <summary>
        /// Operating system-specific initialization.
        /// </summary>
        partial void OSInitialize();

        /// <summary>
        /// Custom plugin initialization during OnImportsSatisfied().
        /// </summary>
        partial void InitializePlugins();
    }
}

﻿// <copyright file="SingleInstanceApplication.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

////#define REPORT_PERFORMANCE
////#define REPORT_PLUGIN_INITIALIZATION

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Device;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Program;
using INTV.Shared.Properties;
using INTV.Shared.Resources;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// This class provides a simple way to determine whether an application is being launched for the first time
    /// activate the main window of the other instance. It presumes that the main window will react to a unique
    /// message broadcast throughout the system.
    /// </summary>
    public partial class SingleInstanceApplication : System.ComponentModel.Composition.IPartImportsSatisfiedNotification
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
        private List<ISettings> _settings = new List<ISettings>();

        #region Properties

        /// <summary>
        /// Gets the instance of SingleInstanceApplication.
        /// </summary>
        public static SingleInstanceApplication Instance
        {
            get { return Current as SingleInstanceApplication; }
        }

        /// <summary>
        /// Gets the application information interface.
        /// </summary>
        public static IApplicationInfo AppInfo { get; private set; }

        /// <summary>
        /// Gets the version of the application in the form Major.Minor.Revision (additional info).
        /// </summary>
        public static string Version
        {
            get
            {
                var versionString = _versionString;
                if (versionString == null)
                {
                    versionString = AppInfo == null ? ApplicationInfo.StandardVersion : AppInfo.Version;
                }
                return versionString;
            }
        }

        /// <summary>
        /// Gets the copyright of the application.
        /// </summary>
        public static string Copyright
        {
            get { return AppInfo == null ? ApplicationInfo.StandardCopyright : AppInfo.Copyright; }
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

        private static bool AlreadyDisplayedExceptionDialog { get; set; }

        /// <summary>
        /// Gets the settings associated with the application.
        /// </summary>
        public IEnumerable<ISettings> Settings
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

        // UNDONE One day, perhaps we can get these via MEF. Not ready yet, though.
        // [System.ComponentModel.Composition.ImportMany]
        // public IEnumerable<Lazy<IPrimaryComponent>> Components
        public IEnumerable<IPrimaryComponent> Components
        {
            get { return CompositionHelpers.Container.GetExportedValues<IPrimaryComponent>(); }
        }

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
        /// Gets or sets simple VINTage plugins.
        /// </summary>
        /// <value>The plugins.</value>
        [System.ComponentModel.Composition.ImportMany]
        public IEnumerable<Lazy<IVINTagePlugin, IVINTagePluginMetadata>> Plugins { get; set; }

        /// <summary>
        /// Gets the plugins location.
        /// </summary>
        public string PluginsLocation { get; private set; }

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
                    SpawnStartupActions();
                }
            }
        }
        private AppReadyState _readyState;

        #endregion // Properties

        /// <summary>
        /// Sets the version string.
        /// </summary>
        /// <param name="newVersionString">The new version string.</param>
        public static void SetVersionString(string newVersionString)
        {
            if (!string.IsNullOrEmpty(newVersionString) && (newVersionString != _versionString))
            {
                _versionString = newVersionString;
            }
        }

        #region IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            foreach (var configuration in Configurations.OrderBy(c => c.Metadata.Weight))
            {
                try
                {
                    configuration.Value.Initialize();
                }
                catch (Exception e)
                {
                    // Should we complain loudly about this?
                    System.Diagnostics.Debug.WriteLine("Error initializing IConfiguration: " + configuration.Metadata.FeatureName + ": \n" + e);
                }
            }
            OSOnImportsSatisfied();
            InitializePlugins();
            ReadyState |= AppReadyState.ImportsStatisfied;
        }

        #endregion //  IPartImportsSatisfiedNotification Members

        /// <summary>
        /// Add settings to the application which will be saved at shutdown.
        /// </summary>
        /// <param name="settings">Settings to associate with the application.</param>
        public void AddSettings(ISettings settings)
        {
            if (_settings.FirstOrDefault(s => s.GetType() == settings.GetType()) == null)
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
        /// Gets an enumerable of the devices attached to the system.
        /// </summary>
        /// <returns>An enumerable of the devices currently attached to the system.</returns>
        public IEnumerable<IPeripheral> GetAttachedDevices()
        {
            var attachedDevices = Enumerable.Empty<IPeripheral>();
            if (ReadyState == AppReadyState.Ready)
            {
                attachedDevices = Components.SelectMany(c => c.AttachedPeripherals);
            }
            return attachedDevices;
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

        private void Initialize(ISettings settings)
        {
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            PluginsLocation = GetPluginsDirectory();
            OSInitialize();
            _mainDispatcher = OSDispatcher.Current;
            this.DoImport();
            if (settings != null)
            {
                _settings.Add(settings);
            }
            AddStartupAction("FileMemoPrimer", FileMemoPrimer.Start, StartupTaskPriority.HighestSyncTaskPriority);
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
            catch (Exception e)
            {
                // Let this quietly fail in release builds.
                var startupActionFaledMessage = "Failure in async startup action! Let's see what's going on here, shall we?" + e;
                ApplicationLogger.RecordDebugTraceMessage(startupActionFaledMessage);
#if DEBUG
                System.Diagnostics.Debug.Assert(false, startupActionFaledMessage);
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
        /// Initialize the plugins.
        /// </summary>
        private void InitializePlugins()
        {
            var plugins = Plugins;
            if (plugins != null)
            {
                foreach (var plugin in plugins.OrderBy(p => p.Metadata.Weight))
                {
                    try
                    {
                        var pluginInstance = plugin.Value;
                        ReportPluginStatus("Initializing plugin: " + plugin.Metadata.Name + ": " + pluginInstance.GetType().Assembly.Location + " " + pluginInstance.GetType().Assembly.FullName);
                        pluginInstance.Initialize();
                    }
                    catch (Exception e)
                    {
                        // Plugins are deemed 'expendable' and we don't really care if any fail to
                        // load. We will quietly report anything that happens, though.
                        ReportPluginStatus("Error while initializing plugin: " + plugin.Metadata.Name + ":\n");
                        ReportPluginStatus(e.ToString());
                    }
                }
            }
        }

        [System.Diagnostics.Conditional("REPORT_PLUGIN_INITIALIZATION")]
        private void ReportPluginStatus(string message)
        {
            ApplicationLogger.RecordDebugTraceMessage(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Operating system-specific initialization.
        /// </summary>
        partial void OSInitialize();

        /// <summary>
        /// Operating system-specific MEF imports satisfied initialization.
        /// </summary>
        partial void OSOnImportsSatisfied();

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

        /// <summary>
        /// Implements an off-thread 'memo initializer' for the discovered Components.
        /// </summary>
        /// <remarks>Initial testing indicates this does not accomplish the ideal goal of 'priming' the memos as hoped. It turns
        /// out that during load of ROM list and MenuLayout, most of that already happens -- on the main thread. :/ The memos,
        /// however, *SEEM* to be pretty fast and accomplish the intended goal - as the primer typically finishes in a few
        /// hundredths of a second when ROMs are on a 'platter' drive. (Caching, CPU, et. al. all heavily influence this, of course.)</remarks>
        private class FileMemoPrimer : AsyncTaskData
        {
            private FileMemoPrimer(IEnumerable<IPrimaryComponent> components)
                : base(null)
            {
#if REPORT_PERFORMANCE
                Stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE

                Components = components;
            }

            private IEnumerable<IPrimaryComponent> Components { get; set; }

#if REPORT_PERFORMANCE
            private System.Diagnostics.Stopwatch Stopwatch { get; set; }
#endif // REPORT_PERFORMANCE

            /// <summary>
            /// Start the memo primer.
            /// </summary>
            internal static void Start()
            {
                var components = CompositionHelpers.Container.GetExportedValues<IPrimaryComponent>();
                var data = new FileMemoPrimer(components);
                var task = new AsyncTaskWithProgress("FileMemoPrimer", false, true, false, 0);
                task.RunTask(data, Prime, PrimeFinished);
            }

            private static void Prime(AsyncTaskData taskData)
            {
                var data = (FileMemoPrimer)taskData;
                foreach (var component in data.Components)
                {
                    component.Initialize();
                }
            }

            private static void PrimeFinished(AsyncTaskData taskData)
            {
#if REPORT_PERFORMANCE
                var data = (FileMemoPrimer)taskData;
                data.Stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine("FileMemoPrimer ran this long: " + data.Stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE
            }
        }
    }
}

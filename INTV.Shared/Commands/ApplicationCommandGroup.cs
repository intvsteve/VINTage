// <copyright file="ApplicationCommandGroup.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Application-wide commands.
    /// </summary>
    public partial class ApplicationCommandGroup : CommandGroup
    {
        /// <summary>
        /// The single instance of this command group.
        /// </summary>
        /// <remarks>Could this be done via MEF?</remarks>
        internal static readonly ApplicationCommandGroup Group = new ApplicationCommandGroup();

        private const string UniqueNameBase = "INTV.Shared.Commands.ApplicationCommandGroup";

        private ApplicationCommandGroup()
            : base(UniqueNameBase, string.Empty, 0.01) // Looks like a bogus weight?
        {
        }

        #region SettingsDialogCommand

        public static readonly ICommand SettingsDialogCommand = new VisualRelayCommand(OnShowSettingsDialog)
        {
            UniqueId = UniqueNameBase + ".PreferencesCommand",
            Name = Resources.Strings.SingleApplicationCommands_Settings,
            ToolTip = Resources.Strings.SingleApplicationCommands_SettingsTip,
            Weight = 0.1,
            SmallIcon = typeof(ApplicationCommandGroup).LoadImageResource("ViewModel/Resources/Images/settings_16xLG.png"),
            LargeIcon = typeof(ApplicationCommandGroup).LoadImageResource("ViewModel/Resources/Images/settings_32xMD.png")
        };

        private static void OnShowSettingsDialog(object parameter)
        {
            var settingsDialog = INTV.Shared.View.SettingsDialog.Create();
            settingsDialog.ShowDialog();
        }

        #endregion // SettingsDialogCommand

        #region ShowOnlineHelpCommand

        /// <summary>
        /// Command to show online help.
        /// </summary>
        public static readonly ICommand ShowOnlineHelpCommand = new VisualRelayCommand(OnShowOnlineHelp, CanShowOnlineHelp)
        {
            UniqueId = UniqueNameBase + ".ShowOnlineHelpCommand",
            Name = Resources.Strings.ShowOnlineHelpCommand_Name,
            Weight = 0.1,
            SmallIcon = typeof(ApplicationCommandGroup).LoadImageResource("Resources/Images/help_16xLG_color.png"),
            LargeIcon = typeof(ApplicationCommandGroup).LoadImageResource("Resources/Images/help_32xMD_color.png"),
            KeyboardShortcutKey = "?",
            KeyboardShortcutModifiers = OSModifierKeys.Menu | OSModifierKeys.Shift
        };

        private static void OnShowOnlineHelp(object parameter)
        {
            var appInfo = SingleInstanceApplication.Instance.AppInfo;
            System.Diagnostics.Process.Start(appInfo.OnlineHelpUrl);
        }

        private static bool CanShowOnlineHelp(object parameter)
        {
            var canExecute = SingleInstanceApplication.Instance != null;
            if (canExecute)
            {
                try
                {
                    var appInfo = SingleInstanceApplication.Instance.AppInfo;
                    canExecute = (appInfo != null) && !string.IsNullOrWhiteSpace(appInfo.OnlineHelpUrl);
                }
                catch (System.ComponentModel.Composition.ImportCardinalityMismatchException)
                {
                }
            }
            return canExecute;
        }

        #endregion // ShowOnlineHelpCommand

        #region CheckForUpdatesCommand

        /// <summary>
        /// Command to check for application updates.
        /// </summary>
        public static readonly VisualRelayCommand CheckForUpdatesCommand = new VisualRelayCommand(OnCheckForUpdates, CanCheckForUpdates)
        {
            UniqueId = UniqueNameBase + ".CheckForUpdatesCommand",
            Name = Resources.Strings.CheckForUpdatesCommand_Name,
            ToolTip = Resources.Strings.CheckForUpdatesCommand_Tip,
            ToolTipTitle = Resources.Strings.CheckForUpdatesCommand_Name,
            ToolTipDescription = Resources.Strings.CheckForUpdatesCommand_TipDescription,
            Weight = 0.6,
            SmallIcon = typeof(ApplicationCommandGroup).LoadImageResource("ViewModel/Resources/Images/refresh_16xLG.png"),
            LargeIcon = typeof(ApplicationCommandGroup).LoadImageResource("ViewModel/Resources/Images/refresh_32xMD.png"),
            MenuParent = RootCommandGroup.ApplicationMenuCommand
        };

        private static void OnCheckForUpdates(object parameter)
        {
            if (CanCheckForUpdates(parameter))
            {
                var checkingAtStartup = false;
                if ((parameter != null) && (parameter.GetType() == typeof(bool)))
                {
                    checkingAtStartup = (bool)parameter;
                }
                var checkForUpdatesTask = new AsyncTaskWithProgress("CheckForUpdates", false, true, !checkingAtStartup, 2);
                checkForUpdatesTask.UpdateTaskTitle(CheckForUpdatesCommand.Name);
                var checkForUpdatesTaskData = new CheckForUpdatesTaskData(checkForUpdatesTask, checkingAtStartup);
                checkForUpdatesTask.RunTask(checkForUpdatesTaskData, CheckForUpdates, CheckForUpdatesComplete);
            }
        }

        private static bool CanCheckForUpdates(object parameter)
        {
            var canExecute = SingleInstanceApplication.Instance != null;
            if (canExecute)
            {
                try
                {
                    var appInfo = SingleInstanceApplication.Instance.AppInfo;
                    canExecute = (appInfo != null) && !string.IsNullOrWhiteSpace(appInfo.VersionCheckUrl);
                }
                catch (System.ComponentModel.Composition.ImportCardinalityMismatchException)
                {
                }
            }
            return canExecute;
        }

        private static void CheckForUpdates(AsyncTaskData taskData)
        {
            var data = (CheckForUpdatesTaskData)taskData;
            if (data.CheckingAtStartup)
            {
                // Wait a little while before checking. This gives a chance for other parallel startup tasks to finish.
                System.Threading.Thread.Sleep(12000);
            }
            var appInfo = SingleInstanceApplication.Instance.AppInfo;
            var versionCheckUrl = appInfo.VersionCheckUrl;
            var webRequest = System.Net.WebRequest.Create(versionCheckUrl);
            webRequest.Proxy = null;
            webRequest.Timeout = 30000;
            var response = webRequest.GetResponse().GetResponseStream();
            var versionStringBuffer = new char[48];
            using (var reader = new System.IO.StreamReader(response))
            {
                if (reader.BaseStream.CanTimeout)
                {
                    reader.BaseStream.ReadTimeout = 20000;
                }
                reader.ReadBlock(versionStringBuffer, 0, versionStringBuffer.Length);
                data.UpdateVersionString = new string(versionStringBuffer);
            }
            var downloadableVersion = new System.Version(data.UpdateVersionString);
            data.UpdateAvailable = downloadableVersion > data.Version;
        }

        private static void CheckForUpdatesComplete(AsyncTaskData taskData)
        {
            var data = (CheckForUpdatesTaskData)taskData;
            var appInfo = SingleInstanceApplication.Instance.AppInfo;
            if (taskData.Error != null)
            {
                var reportError = INTV.Shared.Properties.Settings.Default.ShowDetailedErrors;
                var message = string.Empty;
                if (data.CheckingAtStartup)
                {
                    message = Resources.Strings.CheckForUpdatesCommand_FailedStopAsking;
                }
                else
                {
                    if (reportError)
                    {
                        message = Resources.Strings.CheckForUpdatesCommand_Failed;
                    }
                    else
                    {
                        message = string.Format(Resources.Strings.CheckForUpdatesCommand_FailedFormat, data.Error.Message);
                    }
                }
                if (reportError || !data.CheckingAtStartup)
                {
                    OSMessageBox.Show(message, Resources.Strings.CheckForUpdatesCommand_DialogTitle, reportError ? data.Error : null, OSMessageBoxButton.OK);
                }
            }
            else
            {
                var message = string.Empty;
                if (data.UpdateAvailable)
                {
                    message = string.Format(Resources.Strings.CheckForUpdatesCommand_UpdateAvailableFormat, data.VersionString, appInfo.ProductUrl, data.UpdateVersionString);
                }
                else if (!data.CheckingAtStartup)
                {
                    message = Resources.Strings.CheckForUpdatesCommand_NoUpdates;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    var result = OSMessageBox.Show(message, Resources.Strings.CheckForUpdatesCommand_DialogTitle, data.UpdateAvailable ? OSMessageBoxButton.YesNo : OSMessageBoxButton.OK);
                    if (data.UpdateAvailable && (result == OSMessageBoxResult.Yes))
                    {
                        try
                        {
                            RunExternalProgram.OpenInDefaultProgram(appInfo.ProductUrl);
                        }
                        catch (System.Exception e)
                        {
                            message = string.Format(Resources.Strings.CheckForUpdatesCommand_OpenBrowserFailedFormat, e.Message);
                            OSMessageBox.Show(message, Resources.Strings.CheckForUpdatesCommand_DialogTitle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Async task data used for checking for updates.
        /// </summary>
        private class CheckForUpdatesTaskData : AsyncTaskData
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.Shared.Commands.ApplicationCommandGroup+CheckForUpdatesTaskData"/> class.
            /// </summary>
            /// <param name="task">The task that executes.</param>
            /// <param name="checkingAtStartup">Set to <c>true</c> if we're checking for updates at application startup.</param>
            internal CheckForUpdatesTaskData(AsyncTaskWithProgress task, bool checkingAtStartup)
                : base(task)
            {
                CheckingAtStartup = checkingAtStartup;

                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                VersionString = System.Diagnostics.FileVersionInfo.GetVersionInfo(entryAssembly.Location).FileVersion;
                Version = new System.Version(VersionString);
            }

            /// <summary>
            /// Gets a value indicating whether the check for updates is happening as a result of application startup.
            /// </summary>
            internal bool CheckingAtStartup { get; private set; }

            /// <summary>
            /// Gets the current application file version number.
            /// </summary>
            internal System.Version Version { get; private set; }

            /// <summary>
            /// Gets the current application file version number as a string.
            /// </summary>
            internal string VersionString { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether an update is available.
            /// </summary>
            internal bool UpdateAvailable { get; set; }

            /// <summary>
            /// Gets or sets the update version string.
            /// </summary>
            internal string UpdateVersionString { get; set; }
        }

        #endregion // CheckForUpdatesCommand

        #region ExitCommand

        /// <summary>
        /// Command to close the main application window, which should exit the application.
        /// </summary>
        public static readonly ICommand ExitCommand = new VisualRelayCommand(OnExit)
        {
            UniqueId = UniqueNameBase + ".ExitCommand",
            Name = Resources.Strings.ExitCommand_Name,
            Weight = 1.0,
            SmallIcon = typeof(ApplicationCommandGroup).LoadImageResource("Resources/Images/exit_16xLG_color.png")
        };

        private static void OnExit(object parameter)
        {
            SingleInstanceApplication.Instance.MainWindow.Close();
        }

        #endregion // ExitCommand

        #region CommandGroup

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(SettingsDialogCommand);
            CommandList.Add(CheckForUpdatesCommand);
            AddPlatformCommands();
        }

        /// <summary>
        /// Platform-specific command group additions.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}

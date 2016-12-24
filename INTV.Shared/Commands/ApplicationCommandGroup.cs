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

        private static readonly ICommand SettingsDialogCommand = new VisualRelayCommand(OnShowSettingsDialog)
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
            var appInfo = CompositionHelpers.Container.GetExport<IApplicationInfo>();
            System.Diagnostics.Process.Start(appInfo.Value.OnlineHelpUrl);
        }

        private static bool CanShowOnlineHelp(object parameter)
        {
            var canExecute = SingleInstanceApplication.Instance != null;
            if (canExecute)
            {
                try
                {
                    var appInfo = CompositionHelpers.Container.GetExport<IApplicationInfo>();
                    canExecute = (appInfo != null) && !string.IsNullOrWhiteSpace(appInfo.Value.OnlineHelpUrl);
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
            var appInfo = CompositionHelpers.Container.GetExport<IApplicationInfo>();
            if (CanCheckForUpdates(parameter))
            {
                var checkingAtStartup = false;
                if ((parameter != null) && (parameter.GetType() == typeof(bool)))
                {
                    checkingAtStartup = (bool)parameter;
                }
                ProgressIndicatorViewModel progressIndicator = null;
                if (checkingAtStartup)
                {
                    progressIndicator = ProgressIndicatorViewModel.ApplicationProgressIndicator;
                    progressIndicator.Title = CheckForUpdatesCommand.Name;
                    progressIndicator.AllowsCancel = false;
                    progressIndicator.IsIndeterminate = true;
                    progressIndicator.Show(null);
                }
                var updateAvailable = false;
                var message = string.Empty;
                var versionCheckUrl = appInfo.Value.VersionCheckUrl;
                try
                {
                    var currentDownloadableVersionString = string.Empty;
                    var webRequest = System.Net.WebRequest.Create(versionCheckUrl);
                    webRequest.Proxy = new System.Net.WebProxy(versionCheckUrl);
                    var response = webRequest.GetResponse().GetResponseStream();
                    using (var reader = new System.IO.StreamReader(response))
                    {
                        currentDownloadableVersionString = reader.ReadToEnd();
                    }
                    var downloadableVersion = new System.Version(currentDownloadableVersionString);
                    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    var appVersionString = System.Diagnostics.FileVersionInfo.GetVersionInfo(entryAssembly.Location).FileVersion;
                    var appVersion = new System.Version(appVersionString);
                    updateAvailable = downloadableVersion > appVersion;
                    if (updateAvailable)
                    {
                        message = string.Format(Resources.Strings.CheckForUpdatesCommand_UpdateAvailableFormat, appVersionString, appInfo.Value.ProductUrl, currentDownloadableVersionString);
                    }
                    else
                    {
                        if (checkingAtStartup)
                        {
                            message = null;
                        }
                        else
                        {
                            message = Resources.Strings.CheckForUpdatesCommand_NoUpdates;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    if (checkingAtStartup)
                    {
                        message = string.Format(Resources.Strings.CheckForUpdatesCommand_FailedStopAskingFormat, e.Message);
                    }
                    else
                    {
                        message = string.Format(Resources.Strings.CheckForUpdatesCommand_FailedFormat, e.Message);
                    }
                }
                finally
                {
                    if (progressIndicator != null)
                    {
                        progressIndicator.Hide();
                        progressIndicator = null;
                    }
                }
                if (!string.IsNullOrEmpty(message))
                {
                    var result = OSMessageBox.Show(message, Resources.Strings.CheckForUpdatesCommand_DialogTitle, updateAvailable ? OSMessageBoxButton.YesNo : OSMessageBoxButton.OK);
                    if (updateAvailable && (result == OSMessageBoxResult.Yes))
                    {
                        try
                        {
                            RunExternalProgram.OpenInDefaultProgram(appInfo.Value.ProductUrl);
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

        private static bool CanCheckForUpdates(object parameter)
        {
            var canExecute = SingleInstanceApplication.Instance != null;
            if (canExecute)
            {
                try
                {
                    var appInfo = CompositionHelpers.Container.GetExport<IApplicationInfo>();
                    canExecute = (appInfo != null) && !string.IsNullOrWhiteSpace(appInfo.Value.VersionCheckUrl);
                }
                catch (System.ComponentModel.Composition.ImportCardinalityMismatchException)
                {
                }
            }
            return canExecute;
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

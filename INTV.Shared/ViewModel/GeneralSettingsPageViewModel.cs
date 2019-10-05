// <copyright file="GeneralSettingsPageViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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
using INTV.Shared.Model;
using INTV.Shared.Utility;

#if WIN || GTK
using GeneralSettingsVisualType = INTV.Shared.View.GeneralSettingsPage;
#elif MAC
using GeneralSettingsVisualType = INTV.Shared.View.GeneralSettingsPageController;
#endif // WIN || GTK

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// General settings for an application.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ISettingsPage))]
    [LocalizedName(typeof(Resources.Strings), "GeneralSettingsPage_Title")]
    [Weight(0.0)]
    [Icon(null)]
    public class GeneralSettingsPageViewModel : SettingsPageViewModel<GeneralSettingsVisualType>
    {
        /// <summary>
        /// The default setting for whether to check for updates at launch.
        /// </summary>
        public const bool DefaultCheckForUpdatesAtLaunchSetting = true;

#if DEBUG
        /// <summary>
        /// Default value for ShowDetailedErrors option.
        /// </summary>
        public const bool DefaultShowDetailedErrorsSetting = true;
#else
        /// <summary>
        /// Default value for ShowDetailedErrors option.
        /// </summary>
        public const bool DefaultShowDetailedErrorsSetting = false;
#endif // DEBUG

        #region Property Names

        public const string CheckForAppUpdatesAtLaunchPropertyName = "CheckForAppUpdatesAtLaunch";
        public const string ShowDetailedErrorsPropertyName = "ShowDetailedErrors";
        public const string ClearTemporaryFilesOnStartupPropertyName = "ClearTemporaryFilesOnStartup";

        #endregion // Property Names

        #region UI Strings

        public static readonly string LocalRomsDirLabel = Resources.Strings.GeneralSettingsPage_LocalRomsDirLabel;
        public static readonly string LocalRomsDirTip = Resources.Strings.GeneralSettingsPage_LocalRomsDirTip;
        public static readonly string ShowLocalRomsDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowLocalRomsDirTip;

        public static readonly string ManualsDirLabel = Resources.Strings.GeneralSettingsPage_ManualsDirLabel;
        public static readonly string ManualsDirTip = Resources.Strings.GeneralSettingsPage_ManualsDirTip;
        public static readonly string ShowManualsDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowManualsDirTip;

        public static readonly string OverlaysDirLabel = Resources.Strings.GeneralSettingsPage_OverlaysDirLabel;
        public static readonly string OverlaysDirTip = Resources.Strings.GeneralSettingsPage_OverlaysDirTip;
        public static readonly string ShowOverlaysDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowOverlaysDirTip;

        public static readonly string BoxesDirLabel = Resources.Strings.GeneralSettingsPage_BoxesDirLabel;
        public static readonly string BoxesDirTip = Resources.Strings.GeneralSettingsPage_BoxesDirTip;
        public static readonly string ShowBoxesDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowBoxesDirTip;

        public static readonly string LabelsDirLabel = Resources.Strings.GeneralSettingsPage_LabelsDirLabel;
        public static readonly string LabelsDirTip = Resources.Strings.GeneralSettingsPage_LabelsDirTip;
        public static readonly string ShowLabelsDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowLabelsDirTip;

        public static readonly string BackupDirLabel = Resources.Strings.GeneralSettingsPage_BackupDirLabel;
        public static readonly string BackupDirTip = Resources.Strings.GeneralSettingsPage_BackupDirTip;
        public static readonly string ShowBackupDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowBackupDirTip;

        public static readonly string ErrorLogDirLabel = Resources.Strings.GeneralSettingsPage_ErrorLogDirLabel;
        public static readonly string ErrorLogDirTip = Resources.Strings.GeneralSettingsPage_ErrorLogDirTip;
        public static readonly string ShowErrorLogButtonTip = Resources.Strings.GeneralSettingsPage_ShowErrorLogDirTip;

        public static readonly string TemporaryFilesDirLabel = Resources.Strings.GeneralSettingsPage_TempFilesDirLabel;
        public static readonly string TemporaryFilesDirTip = Resources.Strings.GeneralSettingsPage_TempFilesDirTip;
        public static readonly string ShowTemporaryFilesDirButtonTip = Resources.Strings.GeneralSettingsPage_ShowTempFilesDirTip;

        public static readonly string ShowInExplorerButtonText = Resources.Strings.GeneralSettingsPage_ShowInExplorerButtonText;

        public static readonly string ShowDetailedErrorsPreferenceText = Resources.Strings.GeneralSettingsPage_ShowDetailedErrors;
        public static readonly string ShowDetailedErrorsTip = Resources.Strings.GeneralSettingsPage_ShowDetailedErrors_Tip;

        public static readonly string CheckForUpdatesAtLaunchPreferenceText = Resources.Strings.GeneralSettingsPage_CheckForUpdates;
        public static readonly string CheckForUpdatesAtLaunchTip = Resources.Strings.GeneralSettingsPage_CheckForUpdates_Tip;

        public static readonly string ClearTemporaryFilesOnStartupPreferenceText = Resources.Strings.GeneralSettingsPage_ShowDetailedErrors;
        public static readonly string ClearTemporaryFilesOnStartupTip = Resources.Strings.GeneralSettingsPage_ShowDetailedErrors_Tip;

        #endregion // UI Strings

        /// <summary>
        /// Command to open a directory in the file system.
        /// </summary>
        public static readonly RelayCommand OpenInFileSystemCommand = new RelayCommand(OnOpenInFileSystem)
        {
            UniqueId = "INTV.Shared.ViewModel.GeneralSettingsPageViewModel.OpenInFileSystemCommand"
        };

        private static void OnOpenInFileSystem(object parameter)
        {
            DirectoryToShow whichDirectory;
            if (System.Enum.TryParse<DirectoryToShow>(parameter as string, out whichDirectory))
            {
                switch (whichDirectory)
                {
                    case DirectoryToShow.Roms:
                        RomListConfiguration.Instance.RomsDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.Manuals:
                        RomListConfiguration.Instance.ManualsDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.Overlays:
                        RomListConfiguration.Instance.OverlaysDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.Boxes:
                        RomListConfiguration.Instance.BoxesDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.Labels:
                        RomListConfiguration.Instance.LabelsDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.Backup:
                        RomListConfiguration.Instance.BackupDataDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.ErrorLog:
                        RomListConfiguration.Instance.ErrorLogDirectory.RevealInFileSystem();
                        break;
                    case DirectoryToShow.TemporaryFiles:
                        RomListConfiguration.Instance.TemporaryFilesDirectory.RevealInFileSystem();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Initialize a new instance of the GeneralSettingsPageViewModel type.
        /// </summary>
        public GeneralSettingsPageViewModel()
        {
            _showDetailedErrors = Properties.Settings.Default.ShowDetailedErrors;
            _checkForAppUpdatesAtLaunch = Properties.Settings.Default.CheckForAppUpdatesAtLaunch;
            _clearTemporaryFilesOnStartup = Properties.Settings.Default.ClearTemporaryFilesOnStartup;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check for app updates at launch.
        /// </summary>
        public bool CheckForAppUpdatesAtLaunch
        {
            get { return _checkForAppUpdatesAtLaunch; }
            set { AssignAndUpdateProperty(CheckForAppUpdatesAtLaunchPropertyName, value, ref _checkForAppUpdatesAtLaunch, (n, v) => Properties.Settings.Default.CheckForAppUpdatesAtLaunch = v); }
        }
        private bool _checkForAppUpdatesAtLaunch;

        /// <summary>
        /// Gets or sets a value indicating whether to show detailed errors.
        /// </summary>
        public bool ShowDetailedErrors
        {
            get { return _showDetailedErrors; }
            set { AssignAndUpdateProperty(ShowDetailedErrorsPropertyName, value, ref _showDetailedErrors, (n, v) => Properties.Settings.Default.ShowDetailedErrors = v); }
        }
        private bool _showDetailedErrors;

        /// <summary>
        /// Gets or sets a value indicating whether to clear temporary files location at startup.
        /// </summary>
        public bool ClearTemporaryFilesOnStartup
        {
            get { return _clearTemporaryFilesOnStartup; }
            set { AssignAndUpdateProperty(ClearTemporaryFilesOnStartupPropertyName, value, ref _clearTemporaryFilesOnStartup, (n, v) => Properties.Settings.Default.ClearTemporaryFilesOnStartup = v); }
        }
        private bool _clearTemporaryFilesOnStartup;

        /// <summary>
        /// Gets the ROMs directory.
        /// </summary>
        public string RomsDir
        {
            get { return RomListConfiguration.Instance.RomsDirectory; }
        }

        /// <summary>
        /// Gets the manuals directory.
        /// </summary>
        public string ManualsDir
        {
            get { return RomListConfiguration.Instance.ManualsDirectory; }
        }

        /// <summary>
        /// Gets the overlays directory.
        /// </summary>
        public string OverlaysDir
        {
            get { return RomListConfiguration.Instance.OverlaysDirectory; }
        }

        /// <summary>
        /// Gets the boxes directory.
        /// </summary>
        public string BoxesDir
        {
            get { return RomListConfiguration.Instance.BoxesDirectory; }
        }

        /// <summary>
        /// Gets the labels directory.
        /// </summary>
        public string LabelsDir
        {
            get { return RomListConfiguration.Instance.LabelsDirectory; }
        }

        /// <summary>
        /// Gets the backup directory.
        /// </summary>
        public string BackupDir
        {
            get { return RomListConfiguration.Instance.BackupDataDirectory; }
        }

        /// <summary>
        /// Gets the error log directory.
        /// </summary>
        public string ErrorLogDir
        {
            get { return RomListConfiguration.Instance.ErrorLogDirectory; }
        }

        /// <summary>
        /// Gets the temporary files directory.
        /// </summary>
        public string TempFilesDir
        {
            get { return RomListConfiguration.Instance.TemporaryFilesDirectory; }
        }

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
        }

        /// <summary>
        /// Special directories the general settings dialog is aware of.
        /// </summary>
        private enum DirectoryToShow
        {
            /// <summary>
            /// Not a directory.
            /// </summary>
            None,

            /// <summary>
            /// The ROMs directory.
            /// </summary>
            Roms,

            /// <summary>
            /// The manuals directory.
            /// </summary>
            Manuals,

            /// <summary>
            /// The overlays directory.
            /// </summary>
            Overlays,

            /// <summary>
            /// The boxes directory.
            /// </summary>
            Boxes,

            /// <summary>
            /// The labels directory.
            /// </summary>
            Labels,

            /// <summary>
            /// The backups directory.
            /// </summary>
            Backup,

            /// <summary>
            /// The error log directory.
            /// </summary>
            ErrorLog,

            /// <summary>
            /// The directory to use for temporary data.
            /// </summary>
            TemporaryFiles,
        }
    }
}

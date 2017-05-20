// <copyright file="SettingsPageViewModel.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;
using INTV.Shared.ViewModel;

#if WIN
using SettingsPageVisualType = INTV.LtoFlash.View.SettingsPage;
#elif MAC
using SettingsPageVisualType = INTV.LtoFlash.View.SettingsPageController;
#endif

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for the LTO Flash! settings page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ISettingsPage))]
    [LocalizedName(typeof(Resources.Strings), "SettingsPage_Title")]
    [Weight(0.1)]
    [Icon("Resources/Images/lto_flash_32xLG.png")]
    public sealed partial class SettingsPageViewModel : INTV.Shared.ViewModel.SettingsPageViewModel<SettingsPageVisualType>
    {
        #region Property Names

        public const string ValidateMenuAtStartupSettingName = "ValidateMenuAtStartup";
        public const string SearchForDevicesAtStartupSettingName = "SearchForDevicesAtStartup";
        public const string ReconcileDeviceMenuWithLocalMenuSettingName = "ReconcileDeviceMenuWithLocalMenu";
        public const string ShowAdvancedFeaturesSettingName = "ShowAdvancedFeatures";
        public const string LastActiveDevicePortSettingName = "LastActiveDevicePort";
        public const string RunGCWhenConnectedSettingName = "RunGCWhenConnected";
        public const string AutomaticallyConnectToDevicesSettingName = "AutomaticallyConnectToDevices";
        public const string AddRomsToMenuPropertyName = "AddRomsToMenu";
        public const string ShowFileSystemDetailsSettingName = "ShowFileSystemDetails";
        public const string PromptToAddRomsToMenuPropertyName = "PromptToAddRomsToMenu";
        public const string EnablePortLoggingPropertyName = "EnablePortLogging";
        public const string PromptToInstallFTDIDriverPropertyName = "PromptToInstallFTDIDriver";
        public const string PromptToImportStarterRomsPropertyName = "PromptToImportStarterRoms";
        public const string PromptForFirmwareUpgradeSettingName = "PromptForFirmwareUpgrade";
        public const string VerifyVIDandPIDBeforeConnectingSettingName = "VerifyVIDandPIDBeforeConnecting";

        #endregion // Property Names

        #region UI Strings

        public static readonly string ValidateSettingsPreferenceText = Resources.Strings.SettingsPage_ValidateMenuAtLaunch;
        public static readonly string SearchForDevicesPreferenceText = Resources.Strings.SettingsPage_SearchForDevicesAtStartup;
        public static readonly string AutomaticallyConnectToDevicesPreferenceText = Resources.Strings.SettingsPage_AutomaticallyConnectToDevices;
        public static readonly string ReconcileMenusPreferenceText = Resources.Strings.SettingsPage_ReconcileDeviceMenuWithLocalMenu;
        public static readonly string RunGarbageCollectorPreferenceText = Resources.Strings.SettingsPage_RunGCWhenConnected;
        public static readonly string PromptToAddRomsPreferenceText = Resources.Strings.SettingsPage_PromptToAddRoms;
        public static readonly string AddRomsToMenuPreferenceText = Resources.Strings.SettingsPage_AddRomsToMenu;
        public static readonly string ShowFileSystemDetailsPreferenceText = Resources.Strings.SettingsPage_ShowFileSystemDetails;
        public static readonly string EnableSerialPortLoggingPreferenceText = Resources.Strings.SettingsPage_EnablePortLogging;
        public static readonly string PromptForFirmwareUpgradePreferenceText = Resources.Strings.SettingsPage_PromptForFirmwareUpdate;
        public static readonly string ShowAdvancedFeaturesPreferenceText = Resources.Strings.SettingsPage_ShowAdvancedFeatures;
        public static readonly string VerifyVIDandPIDBeforeConnectingPreferenceText = Resources.Strings.SettingsPage_VerifyVIDandPIDBeforeConnecting;

        #endregion // UI Strings

        /// <summary>
        /// Initializes a new instance of the SettingsPageViewModel type.
        /// </summary>
        public SettingsPageViewModel()
        {
            _validateMenuAtLaunch = Properties.Settings.Default.ValidateMenuAtStartup;
            _searchForDevicesAtStartup = Properties.Settings.Default.SearchForDevicesAtStartup;
            _automaticallyConnectToDevices = Properties.Settings.Default.AutomaticallyConnectToDevices;
            _verifyVIDandPIDBeforeConnecting = Properties.Settings.Default.VerifyVIDandPIDBeforeConnecting;
            _addRomsToMenu = Properties.Settings.Default.AddRomsToMenu;
            _promptToAddRomsToMenu = Properties.Settings.Default.PromptToAddRomsToMenu;
            _reconcileDeviceMenuWithLocalMenu = Properties.Settings.Default.ReconcileDeviceMenuWithLocalMenu;
            _runGarbageCollector = Properties.Settings.Default.RunGCWhenConnected;
            _promptToUpgradeFirmware = Properties.Settings.Default.PromptForFirmwareUpgrade;
            _showFileSystemDetails = Properties.Settings.Default.ShowFileSystemDetails;
            _showAdvancedFeatures = Properties.Settings.Default.ShowAdvancedFeatures;
        }

        #region EnableAdvancedFeaturesCommand

        /// <summary>
        /// Hidden command to enable advanced features.
        /// </summary>
        public static readonly INTV.Shared.ComponentModel.RelayCommand EnableAdvancedFeaturesCommand = new INTV.Shared.ComponentModel.RelayCommand(OnEnableAdvancedFeatures)
        {
            PreferredParameterType = typeof(SettingsPageViewModel)
        };

        private static void OnEnableAdvancedFeatures(object parameter)
        {
            var settingsPageViewModel = (SettingsPageViewModel)parameter;
            settingsPageViewModel.EnableAdvancedFeatures = true;
        }

        #endregion // EnableAdvancedFeaturesCommand

        /// <summary>
        /// Gets or sets a value indicating whether to validate the contents of the menu at startup.
        /// </summary>
        public bool ValidateMenuAtLaunch
        {
            get { return _validateMenuAtLaunch; }
            set { AssignAndUpdateProperty(ValidateMenuAtStartupSettingName, value, ref _validateMenuAtLaunch, (n, v) => Properties.Settings.Default.ValidateMenuAtStartup = v); }
        }
        private bool _validateMenuAtLaunch;

        /// <summary>
        /// Gets or sets a value indicating whether to search for Locutus devices at startup.
        /// </summary>
        public bool SearchForDevicesAtStartup
        {
            get { return _searchForDevicesAtStartup; }
            set { AssignAndUpdateProperty(SearchForDevicesAtStartupSettingName, value, ref _searchForDevicesAtStartup, (n, v) => Properties.Settings.Default.SearchForDevicesAtStartup = v); }
        }
        private bool _searchForDevicesAtStartup;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically connect to devices when they are discovered.
        /// </summary>
        public bool AutomaticallyConnectToDevices
        {
            get { return _automaticallyConnectToDevices; }
            set { AssignAndUpdateProperty(AutomaticallyConnectToDevicesSettingName, value, ref _automaticallyConnectToDevices, (n, v) => Properties.Settings.Default.AutomaticallyConnectToDevices = v); }
        }
        private bool _automaticallyConnectToDevices;

        /// <summary>
        /// Gets or sets a value indicating whether to verify that a device truly appears to be LTO Flash! hardware before attempting to connect to it.
        /// </summary>
        public bool VerifyVIDandPIDBeforeConnecting
        {
            get { return _verifyVIDandPIDBeforeConnecting; }
            set { AssignAndUpdateProperty(VerifyVIDandPIDBeforeConnectingSettingName, value, ref _verifyVIDandPIDBeforeConnecting, (n, v) => Properties.Settings.Default.VerifyVIDandPIDBeforeConnecting = v); }
        }
        private bool _verifyVIDandPIDBeforeConnecting;

        /// <summary>
        /// Gets or sets a value indicating whether to reconcile the device menu with the locally defined menu when attached.
        /// </summary>
        public bool ReconcileDeviceMenuWithLocalMenu
        {
            get { return _reconcileDeviceMenuWithLocalMenu; }
            set { AssignAndUpdateProperty(ReconcileDeviceMenuWithLocalMenuSettingName, value, ref _reconcileDeviceMenuWithLocalMenu, (n, v) => Properties.Settings.Default.ReconcileDeviceMenuWithLocalMenu = v); }
        }
        private bool _reconcileDeviceMenuWithLocalMenu;

        /// <summary>
        /// Gets or sets a value indicating whether to run garbage collection in the background when Locutus attached.
        /// </summary>
        public bool RunGarbageCollector
        {
            get { return _runGarbageCollector; }
            set { AssignAndUpdateProperty("RunGarbageCollector", value, ref _runGarbageCollector, (n, v) => Properties.Settings.Default.RunGCWhenConnected = v); }
        }
        private bool _runGarbageCollector;

        /// <summary>
        /// Gets or sets a value indicating whether to show attached device file system information in the UI.
        /// </summary>
        public bool ShowFileSystemDetails
        {
            get { return _showFileSystemDetails; }
            set { AssignAndUpdateProperty(ShowFileSystemDetailsSettingName, value, ref _showFileSystemDetails, (n, v) => Properties.Settings.Default.ShowFileSystemDetails = v); }
        }
        private bool _showFileSystemDetails;

        /// <summary>
        /// Gets or sets a value indicating whether to prompt the user to upgrade firmware on device connect if the
        /// device's current firmware is older than the available version.
        /// </summary>
        public bool PromptToUpgradeFirmware
        {
            get { return _promptToUpgradeFirmware; }
            set { AssignAndUpdateProperty(PromptForFirmwareUpgradeSettingName, value, ref _promptToUpgradeFirmware, (n, v) => Properties.Settings.Default.PromptForFirmwareUpgrade = v); }
        }
        private bool _promptToUpgradeFirmware;

        /// <summary>
        /// Gets or sets a value indicating whether to show advanced features in the UI.
        /// </summary>
        public bool ShowAdvancedFeatures
        {
            get { return _showAdvancedFeatures; }
            set { AssignAndUpdateProperty(ShowAdvancedFeaturesSettingName, value, ref _showAdvancedFeatures, (n, v) => Properties.Settings.Default.ShowAdvancedFeatures = v); }
        }
        private bool _showAdvancedFeatures;

        /// <summary>
        /// Gets or sets a value indicating whether to show the 'ShowAdvancedFeatures' setting.
        /// </summary>
        public bool EnableAdvancedFeatures
        {
            get { return _showAdvancedFeaturesSetting; }
            set { AssignAndUpdateProperty("EnableAdvancedFeatures", value, ref _showAdvancedFeaturesSetting); }
        }
        private bool _showAdvancedFeaturesSetting;

        /// <summary>
        /// Gets or sets a value indicating whether to add new ROMs to the menu.
        /// </summary>
        public bool AddRomsToMenu
        {
            get { return _addRomsToMenu; }
            set { AssignAndUpdateProperty(AddRomsToMenuPropertyName, value, ref _addRomsToMenu, (n, v) => Properties.Settings.Default.AddRomsToMenu = v); }
        }
        private bool _addRomsToMenu;

        /// <summary>
        /// Gets or sets a value indicating whether to show a prompt to add ROMs to menu.
        /// </summary>
        public bool PromptToAddRomsToMenu
        {
            get { return _promptToAddRomsToMenu; }
            set { AssignAndUpdateProperty(PromptToAddRomsToMenuPropertyName, value, ref _promptToAddRomsToMenu, (n, v) => Properties.Settings.Default.PromptToAddRomsToMenu = v); }
        }
        private bool _promptToAddRomsToMenu;

#if ENABLE_PORT_LOG
        /// <summary>
        /// Gets or sets a value indicating whether to enable serial port logging.
        /// </summary>
        public bool EnablePortLogging
        {
            get { return _enablePortLogging; }
            set { AssignAndUpdateProperty(EnablePortLoggingPropertyName, value, ref _enablePortLogging, (n, v) => Properties.Settings.Default.EnablePortLogging = v); }
        }
        private bool _enablePortLogging;
#endif

        /// <summary>
        /// Gets a value indicating whether or not to show the Enable Port Logging option.
        /// </summary>
        public bool ShowPortLoggingOption
        {
            get
            {
#if ENABLE_PORT_LOG
                return true;
#else
                return false;
#endif
            }
        }

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
        }
    }
}

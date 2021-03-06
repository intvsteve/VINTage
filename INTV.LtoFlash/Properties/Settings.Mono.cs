﻿// <copyright file="Settings.Mono.cs" company="INTV Funhouse">
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

using System.Runtime.Serialization;

namespace INTV.LtoFlash.Properties
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    internal sealed partial class Settings : INTV.Shared.Properties.SettingsBase<Settings>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to verify that the ROMs in the menu layout are all available.
        /// </summary>
        [DataMember]
        public bool ValidateMenuAtStartup
        {
            get { return GetSetting<bool>(ValidateMenuAtStartupSettingName); }
            set { SetSetting(ValidateMenuAtStartupSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to search for connected devices on application launch.
        /// </summary>
        [DataMember]
        public bool SearchForDevicesAtStartup
        {
            get { return GetSetting<bool>(SearchForDevicesAtStartupSettingName); }
            set { SetSetting(SearchForDevicesAtStartupSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to reconcile local and device menu on device connect.
        /// </summary>
        [DataMember]
        public bool ReconcileDeviceMenuWithLocalMenu
        {
            get { return GetSetting<bool>(ReconcileDeviceMenuWithLocalMenuSettingName); }
            set { SetSetting(ReconcileDeviceMenuWithLocalMenuSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show advanced features in the UI.
        /// </summary>
        [DataMember]
        public bool ShowAdvancedFeatures
        {
            get { return GetSetting<bool>(ShowAdvancedFeaturesSettingName); }
            set { SetSetting(ShowAdvancedFeaturesSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the last serial port used to connect to a Locutus device.
        /// </summary>
        [DataMember]
        public string LastActiveDevicePort
        {
            get { return GetSetting<string>(LastActiveDevicePortSettingName); }
            set { UpdateSetting(LastActiveDevicePortSettingName, value, null); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to run garbage collection on a connected Locutus device.
        /// </summary>
        [DataMember]
        public bool RunGCWhenConnected
        {
            get { return GetSetting<bool>(RunGCWhenConnectedSettingName) || true; } // why?
            set { SetSetting(RunGCWhenConnectedSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically connect to any detected Locutus devices.
        /// </summary>
        [DataMember]
        public bool AutomaticallyConnectToDevices
        {
            get { return GetSetting<bool>(AutomaticallyConnectToDevicesSettingName); }
            set { SetSetting(AutomaticallyConnectToDevicesSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically add newly discovered ROMs to the menu layout.
        /// </summary>
        [DataMember]
        public bool AddRomsToMenu
        {
            get { return GetSetting<bool>(AddRomsToMenuPropertyName); }
            set { SetSetting(AddRomsToMenuPropertyName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to prompt to automatically add newly discovered ROMs to the menu layout.
        /// </summary>
        [DataMember]
        public bool PromptToAddRomsToMenu
        {
            get { return GetSetting<bool>(PromptToAddRomsToMenuPropertyName); }
            set { SetSetting(PromptToAddRomsToMenuPropertyName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to log serial port traffic to disk.
        /// </summary>
        [DataMember]
        public bool EnablePortLogging
        {
            get { return GetSetting<bool>(EnablePortLoggingPropertyName); }
            set { SetSetting(EnablePortLoggingPropertyName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to prompt to install the FTDI driver.
        /// </summary>
        [DataMember]
        public bool PromptToInstallFTDIDriver
        {
            get { return GetSetting<bool>(PromptToInstallFTDIDriverPropertyName); }
            set { SetSetting(PromptToInstallFTDIDriverPropertyName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show file system details.
        /// </summary>
        [DataMember]
        public bool ShowFileSystemDetails
        {
            get { return _showFileSystemDetails; }
            set { AssignAndUpdateSetting(ShowFileSystemDetailsSettingName, value, ref _showFileSystemDetails); }
        }
        private bool _showFileSystemDetails; // ^^^Why is this done in this manner???

        /// <summary>
        /// Gets or sets a value indicating whether to prompt to import starter ROMs.
        /// </summary>
        [DataMember]
        public bool PromptToImportStarterRoms
        {
            get { return GetSetting<bool>(PromptToImportStarterRomsPropertyName); }
            set { SetSetting(PromptToImportStarterRomsPropertyName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to prompt to upgrade firmware on device attach that has older firmware than
        /// the version embedded in the shipping product if such has been built in.
        /// </summary>
        [DataMember]
        public bool PromptForFirmwareUpgrade
        {
            get { return GetSetting<bool>(PromptForFirmwareUpgradeSettingName); }
            set { SetSetting(PromptForFirmwareUpgradeSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to verify USB VID and PID before connecting to a serial port.
        /// </summary>
        [DataMember]
        public bool VerifyVIDandPIDBeforeConnecting
        {
            get { return GetSetting<bool>(VerifyVIDandPIDBeforeConnectingSettingName); }
            set { SetSetting(VerifyVIDandPIDBeforeConnectingSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent system sleep during long-running commands.
        /// </summary>
        [DataMember]
        public bool PreventSystemSleepDuringDeviceCommands
        {
            get { return GetSetting<bool>(PreventSystemSleepDuringDeviceCommandsSettingName); }
            set { SetSetting(PreventSystemSleepDuringDeviceCommandsSettingName, value); }
        }

        #region ISettings

        /// <inheritdoc/>
        protected override void InitializeDefaults()
        {
            AddSetting(ValidateMenuAtStartupSettingName, false);
            AddSetting(SearchForDevicesAtStartupSettingName, true);
            AddSetting(ReconcileDeviceMenuWithLocalMenuSettingName, false);
            AddSetting(ShowAdvancedFeaturesSettingName, false);
            AddSetting(LastActiveDevicePortSettingName, null); // string.Empty???
            AddSetting(RunGCWhenConnectedSettingName, true);
            AddSetting(AutomaticallyConnectToDevicesSettingName, true);
            AddSetting(AddRomsToMenuPropertyName, true);
            AddSetting(ShowFileSystemDetailsSettingName, false);
            AddSetting(PromptToAddRomsToMenuPropertyName, true);
            AddSetting(EnablePortLoggingPropertyName, false);
            AddSetting(PromptToInstallFTDIDriverPropertyName, true); // Mac only???
            AddSetting(PromptToImportStarterRomsPropertyName, true);
            AddSetting(PromptForFirmwareUpgradeSettingName, true);
            AddSetting(VerifyVIDandPIDBeforeConnectingSettingName, true);
            AddSetting(PreventSystemSleepDuringDeviceCommandsSettingName, false);
            OSInitializeDefaults();
        }

        #endregion // ISettings

        /// <summary>
        /// OS-specific default setting initialization.
        /// </summary>
        partial void OSInitializeDefaults();
    }
}

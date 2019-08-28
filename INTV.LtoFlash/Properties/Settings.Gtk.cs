// <copyright file="Settings.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2018 All Rights Reserved
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
    /// GTK-specific implementation.
    /// </summary>
    [DataContract(Name = ContractName, Namespace = ContractNamespace)]
    internal sealed partial class Settings
    {
        private const string ContractName = "LocutusSettings";
        private const string ContractNamespace = "https://ltoflash.intvfunhouse.com";

        /// <summary>
        /// Gets or sets the width of the menu layout long name column.
        /// </summary>
        public int MenuLayoutLongNameColWidth
        {
            get { return GetSetting<int>(MenuLayoutLongNameColWidthSettingName); }
            set { SetSetting(MenuLayoutLongNameColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the menu layout short name column.
        /// </summary>
        public int MenuLayoutShortNameColWidth
        {
            get { return GetSetting<int>(MenuLayoutShortNameColWidthSettingName); }
            set { SetSetting(MenuLayoutShortNameColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the menu layout manual column.
        /// </summary>
        public int MenuLayoutManualColWidth
        {
            get { return GetSetting<int>(MenuLayoutManualColWidthSettingName); }
            set { SetSetting(MenuLayoutManualColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the menu layout save data column.
        /// </summary>
        public int MenuLayoutSaveDataColWidth
        {
            get { return GetSetting<int>(MenuLayoutSaveDataColWidthSettingName); }
            set { SetSetting(MenuLayoutSaveDataColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the serial port read block size to use.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int LtoFlashSerialReadChunkSize
        {
            get { return GetSetting<int>(LtoFlashSerialReadChunkSizeSettingName); }
            set { SetSetting(LtoFlashSerialReadChunkSizeSettingName, value); }
        }

        /// <inheritdoc/>
        protected override void InitializeFromSettingsFile()
        {
            InitializeFromSettingsFile<SettingsDto>();
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        partial void OSInitializeDefaults()
        {
            AddSetting(MenuLayoutLongNameColWidthSettingName, 256);
            AddSetting(MenuLayoutShortNameColWidthSettingName, 144);
            AddSetting(MenuLayoutManualColWidthSettingName, 168);
            AddSetting(MenuLayoutSaveDataColWidthSettingName, 128);
            AddSetting(LtoFlashSerialReadChunkSizeSettingName, 0);
        }

        [DataContract(Name = ContractName, Namespace = ContractNamespace)]
        private sealed class SettingsDto : IExtensibleDataObject
        {
            public ExtensionDataObject ExtensionData
            {
                get { return _extensibleDataObject; }
                set { _extensibleDataObject = value; }
            }
            private ExtensionDataObject _extensibleDataObject;

            [DataMember]
            public bool ValidateMenuAtStartup { get; set; }

            [DataMember]
            public bool SearchForDevicesAtStartup { get; set; }

            [DataMember]
            public bool ReconcileDeviceMenuWithLocalMenu { get; set; }

            [DataMember]
            public bool ShowAdvancedFeatures { get; set; }

            [DataMember(EmitDefaultValue = false)]
            public string LastActiveDevicePort { get; set; }

            [DataMember]
            public bool RunGCWhenConnected { get; set; }

            [DataMember]
            public bool AutomaticallyConnectToDevices { get; set; }

            [DataMember]
            public bool AddRomsToMenu { get; set; }

            [DataMember]
            public bool PromptToAddRomsToMenu { get; set; }

            [DataMember]
            public bool EnablePortLogging { get; set; }

            [DataMember]
            public bool PromptToInstallFTDIDriver { get; set; }

            [DataMember]
            public bool ShowFileSystemDetails { get; set; }

            [DataMember]
            public bool PromptToImportStarterRoms { get; set; }

            [DataMember]
            public bool PromptForFirmwareUpgrade { get; set; }

            [DataMember]
            public bool VerifyVIDandPIDBeforeConnecting { get; set; }

            [DataMember]
            public bool PreventSystemSleepDuringDeviceCommands { get; set; }
        }
    }
}

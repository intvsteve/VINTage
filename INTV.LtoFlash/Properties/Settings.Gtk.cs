// <copyright file="Settings.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2021 All Rights Reserved
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
    internal sealed partial class Settings : INTV.Shared.Properties.SettingsBase<Settings>
    {
        private const string ContractName = "LocutusSettings";
        private const string ContractNamespace = "https://ltoflash.intvfunhouse.com";

        /// <inheritdoc/>
        public override double Weight { get; } = 0.8; // see INTV.LtoFlash.Model.Configuration

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

        /// <summary>
        /// Gets or sets the serial port read block size to use.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int LtoFlashSerialWriteChunkSize {
            get { return GetSetting<int> (LtoFlashSerialWriteChunkSizeSettingName); }
            set { SetSetting (LtoFlashSerialWriteChunkSizeSettingName, value); }
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
            AddSetting(MenuLayoutLongNameColWidthSettingName, 256, isApplicationSetting: true);
            AddSetting(MenuLayoutShortNameColWidthSettingName, 144, isApplicationSetting: true);
            AddSetting(MenuLayoutManualColWidthSettingName, 168, isApplicationSetting: true);
            AddSetting(MenuLayoutSaveDataColWidthSettingName, 128, isApplicationSetting: true);
            AddSetting(LtoFlashSerialReadChunkSizeSettingName, 0);
            AddSetting(LtoFlashSerialWriteChunkSizeSettingName, 0);
        }

        [DataContract(Name = ContractName, Namespace = ContractNamespace)]
        private sealed class SettingsDto : IExtensibleDataObject
        {
            /// <inheritdoc/>
            public ExtensionDataObject ExtensionData
            {
                get { return _extensibleDataObject; }
                set { _extensibleDataObject = value; }
            }
            private ExtensionDataObject _extensibleDataObject;

            /// <inheritdoc cref="Settings.ValidateMenuAtStartup"/>
            [DataMember]
            public bool ValidateMenuAtStartup { get; set; }

            /// <inheritdoc cref="Settings.SearchForDevicesAtStartup"/>
            [DataMember]
            public bool SearchForDevicesAtStartup { get; set; }

            /// <inheritdoc cref="Settings.ReconcileDeviceMenuWithLocalMenu"/>
            [DataMember]
            public bool ReconcileDeviceMenuWithLocalMenu { get; set; }

            /// <inheritdoc cref="Settings.ShowAdvancedFeatures"/>
            [DataMember]
            public bool ShowAdvancedFeatures { get; set; }

            /// <inheritdoc cref="Settings.LastActiveDevicePort"/>
            [DataMember(EmitDefaultValue = false)]
            public string LastActiveDevicePort { get; set; }

            /// <inheritdoc cref="Settings.RunGCWhenConnected"/>
            [DataMember]
            public bool RunGCWhenConnected { get; set; }

            /// <inheritdoc cref="Settings.AutomaticallyConnectToDevices"/>
            [DataMember]
            public bool AutomaticallyConnectToDevices { get; set; }

            /// <inheritdoc cref="Settings.AddRomsToMenu"/>
            [DataMember]
            public bool AddRomsToMenu { get; set; }

            /// <inheritdoc cref="Settings.PromptToAddRomsToMenu"/>
            [DataMember]
            public bool PromptToAddRomsToMenu { get; set; }

            /// <inheritdoc cref="Settings.EnablePortLogging"/>
            [DataMember]
            public bool EnablePortLogging { get; set; }

            /// <inheritdoc cref="Settings.PromptToInstallFTDIDriver"/>
            [DataMember]
            public bool PromptToInstallFTDIDriver { get; set; }

            /// <inheritdoc cref="Settings.ShowFileSystemDetails"/>
            [DataMember]
            public bool ShowFileSystemDetails { get; set; }

            /// <inheritdoc cref="Settings.PromptToImportStarterRoms"/>
            [DataMember]
            public bool PromptToImportStarterRoms { get; set; }

            /// <inheritdoc cref="Settings.PromptForFirmwareUpgrade"/>
            [DataMember]
            public bool PromptForFirmwareUpgrade { get; set; }

            /// <inheritdoc cref="Settings.VerifyVIDandPIDBeforeConnecting"/>
            [DataMember]
            public bool VerifyVIDandPIDBeforeConnecting { get; set; }

            /// <inheritdoc cref="Settings.PreventSystemSleepDuringDeviceCommands"/>
            [DataMember]
            public bool PreventSystemSleepDuringDeviceCommands { get; set; }

            /// <inheritdoc cref="Settings.LtoFlashSerialReadChunkSize"/>
            [DataMember(EmitDefaultValue = false)]
            public int LtoFlashSerialReadChunkSize { get; set; }

            /// <inheritdoc cref="Settings.LtoFlashSerialWriteChunkSize"/>
            [DataMember(EmitDefaultValue = false)]
            public int LtoFlashSerialWriteChunkSize { get; set; }
        }
    }
}

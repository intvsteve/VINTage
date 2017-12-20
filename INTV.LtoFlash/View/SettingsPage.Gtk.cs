// <copyright file="SettingsPage.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using INTV.LtoFlash.ViewModel;
using INTV.Shared.View;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SettingsPage : Gtk.Bin, IFakeDependencyObject
    {
        private static readonly string SettingNameProperty = "SettingName";

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            this.Build();
            _searchForDevicesAtStartup.Label = SettingsPageViewModel.SearchForDevicesPreferenceText;
            _searchForDevicesAtStartup.Data[SettingNameProperty] = Properties.Settings.SearchForDevicesAtStartupSettingName;
            _searchForDevicesAtStartup.Active = Properties.Settings.Default.SearchForDevicesAtStartup;

            _automaticallyConnectToDevices.Label = SettingsPageViewModel.AutomaticallyConnectToDevicesPreferenceText;
            _automaticallyConnectToDevices.Data[SettingNameProperty] = Properties.Settings.AutomaticallyConnectToDevicesSettingName;
            _automaticallyConnectToDevices.Active = Properties.Settings.Default.AutomaticallyConnectToDevices;

            _validateVIDandPIDBeforeConnecting.Label = SettingsPageViewModel.VerifyVIDandPIDBeforeConnectingPreferenceText;
            _validateVIDandPIDBeforeConnecting.Data[SettingNameProperty] = Properties.Settings.VerifyVIDandPIDBeforeConnectingSettingName;
            _validateVIDandPIDBeforeConnecting.Active = Properties.Settings.Default.VerifyVIDandPIDBeforeConnecting;

            _runGarbageCollector.Label = SettingsPageViewModel.RunGarbageCollectorPreferenceText;
            _runGarbageCollector.Data[SettingNameProperty] = Properties.Settings.RunGCWhenConnectedSettingName;
            _runGarbageCollector.Active = Properties.Settings.Default.RunGCWhenConnected;

            _promptToAddRoms.Label = SettingsPageViewModel.PromptToAddRomsPreferenceText;
            _promptToAddRoms.Data[SettingNameProperty] = Properties.Settings.PromptToAddRomsToMenuPropertyName;
            _promptToAddRoms.Active = Properties.Settings.Default.PromptToAddRomsToMenu;

            _addRomsToMenu.Label = SettingsPageViewModel.AddRomsToMenuPreferenceText;
            _addRomsToMenu.Data[SettingNameProperty] = Properties.Settings.AddRomsToMenuPropertyName;
            _addRomsToMenu.Active = Properties.Settings.Default.AddRomsToMenu;

            _validateMenuLayoutAtStartup.Label = SettingsPageViewModel.ValidateSettingsPreferenceText;
            _validateMenuLayoutAtStartup.Data[SettingNameProperty] = Properties.Settings.ValidateMenuAtStartupSettingName;
            _validateMenuLayoutAtStartup.Active = Properties.Settings.Default.ValidateMenuAtStartup;

            _reconcileDeviceMenuAndLocalMenu.Label = SettingsPageViewModel.ReconcileMenusPreferenceText;
            _reconcileDeviceMenuAndLocalMenu.Data[SettingNameProperty] = Properties.Settings.ReconcileDeviceMenuWithLocalMenuSettingName;
            _reconcileDeviceMenuAndLocalMenu.Active = Properties.Settings.Default.ReconcileDeviceMenuWithLocalMenu;

            _showFileSystemDetails.Label = SettingsPageViewModel.ShowFileSystemDetailsPreferenceText;
            _showFileSystemDetails.Data[SettingNameProperty] = Properties.Settings.ShowFileSystemDetailsSettingName;
            _showFileSystemDetails.Active = Properties.Settings.Default.ShowFileSystemDetails;

            _enableSerialPortLogging.Label = SettingsPageViewModel.EnableSerialPortLoggingPreferenceText; // show?
            _enableSerialPortLogging.Data[SettingNameProperty] = Properties.Settings.EnablePortLoggingPropertyName;
            _enableSerialPortLogging.Active = Properties.Settings.Default.EnablePortLogging;
            _enableSerialPortLogging.NoShowAll = true;

            _promptForFirmwareUpgrade.Label = SettingsPageViewModel.PromptForFirmwareUpgradePreferenceText;
            _promptForFirmwareUpgrade.Data[SettingNameProperty] = Properties.Settings.PromptForFirmwareUpgradeSettingName;
            _promptForFirmwareUpgrade.Active = Properties.Settings.Default.PromptForFirmwareUpgrade;

            _showAdvancedFeatures.Label = SettingsPageViewModel.ShowAdvancedFeaturesPreferenceText;
            _showAdvancedFeatures.Data[SettingNameProperty] = Properties.Settings.ShowAdvancedFeaturesSettingName;
            _showAdvancedFeatures.Active = Properties.Settings.Default.ShowAdvancedFeatures;
            _showAdvancedFeatures.NoShowAll = true;

            Properties.Settings.Default.PropertyChanged += HandleSettingChanged;
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object DataContext
        {
            get
            {
                return this.GetDataContext();
            }

            set
            {
                this.SetDataContext(value);
                _enableSerialPortLogging.Visible = ((SettingsPageViewModel)value).ShowPortLoggingOption;
            }
        }

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        protected void HandlePreferenceToggled(object sender, System.EventArgs e)
        {
            var feature = sender as Gtk.CheckButton;
            if (feature != null)
            {
                var featureName = feature.Data[SettingNameProperty] as string;
                var newValue = feature.Active;
                var currentValue = (bool)Properties.Settings.Default[featureName];
                if (newValue != currentValue)
                {
                    Properties.Settings.Default[featureName] = newValue;
                }
            }
        }

        private void HandleSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleSettingChangedCore);
        }

        private void HandleSettingChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Gtk.CheckButton settingControl = null;
            switch (e.PropertyName)
            {
                case Properties.Settings.ValidateMenuAtStartupSettingName:
                    settingControl = _validateMenuLayoutAtStartup;
                    break;
                case Properties.Settings.SearchForDevicesAtStartupSettingName:
                    settingControl = _searchForDevicesAtStartup;
                    break;
                case Properties.Settings.VerifyVIDandPIDBeforeConnectingSettingName:
                    settingControl = _validateVIDandPIDBeforeConnecting;
                    break;
                case Properties.Settings.ReconcileDeviceMenuWithLocalMenuSettingName:
                    settingControl = _reconcileDeviceMenuAndLocalMenu;
                    break;
                case Properties.Settings.ShowAdvancedFeaturesSettingName:
                    settingControl = _showAdvancedFeatures;
                    break;
                case Properties.Settings.RunGCWhenConnectedSettingName:
                    settingControl = _runGarbageCollector;
                    break;
                case Properties.Settings.AutomaticallyConnectToDevicesSettingName:
                    settingControl = _automaticallyConnectToDevices;
                    break;
                case Properties.Settings.AddRomsToMenuPropertyName:
                    settingControl = _addRomsToMenu;
                    break;
                case Properties.Settings.ShowFileSystemDetailsSettingName:
                    settingControl = _showFileSystemDetails;
                    break;
                case Properties.Settings.PromptToAddRomsToMenuPropertyName:
                    settingControl = _promptToAddRoms;
                    break;
                case Properties.Settings.EnablePortLoggingPropertyName:
                    settingControl = _enableSerialPortLogging;
                    break;
                case Properties.Settings.PromptForFirmwareUpgradeSettingName:
                    settingControl = _promptForFirmwareUpgrade;
                    break;
                case Properties.Settings.PromptToInstallFTDIDriverPropertyName:
                case Properties.Settings.PromptToImportStarterRomsPropertyName:
                    throw new System.NotImplementedException("Setting not implemented: " + e.PropertyName);
                case Properties.Settings.LastActiveDevicePortSettingName:
                case Properties.Settings.MenuLayoutLongNameColWidthSettingName:
                case Properties.Settings.MenuLayoutShortNameColWidthSettingName:
                case Properties.Settings.MenuLayoutManualColWidthSettingName:
                case Properties.Settings.MenuLayoutSaveDataColWidthSettingName:
                    break;
                default:
                    throw new System.InvalidOperationException("Unrecognized setting: " + e.PropertyName);
            }
            if (settingControl != null)
            {
                settingControl.Active = (bool)Properties.Settings.Default[e.PropertyName];
            }
        }
    }
}

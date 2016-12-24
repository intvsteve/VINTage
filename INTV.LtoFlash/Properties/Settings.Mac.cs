// <copyright file="Settings.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using INTV.Core.ComponentModel;
using MonoMac.Foundation;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Properties
{
    /// <summary>
    /// Mac-specific wrapper from C#-style application settings to NSUserDefaults.
    /// </summary>
    public class Settings : PropertyChangedNotifier
    {
        private Settings()
        {
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.ValidateMenuAtStartupSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.SearchForDevicesAtStartupSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.ReconcileDeviceMenuWithLocalMenuSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.ShowAdvancedFeaturesSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.LastActiveDevicePortSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.RunGCWhenConnectedSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.AutomaticallyConnectToDevicesSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.AddRomsToMenuPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.PromptToAddRomsToMenuPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.EnablePortLoggingPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.PromptToInstallFTDIDriverPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.PromptToImportStarterRomsPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.PromptForFirmwareUpgradeSettingName, RaisePropertyChanged);
        }

        /// <summary>
        /// Get the settings.
        /// </summary>
        public static Settings Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }
        private static Settings _instance;

        private static NSUserDefaults UserDefaults
        {
            get
            {
                if (_userDefaults == null)
                {
                    _userDefaults = NSUserDefaults.StandardUserDefaults;
                    var defaults = new NSMutableDictionary();
                    defaults[SettingsPageViewModel.ValidateMenuAtStartupSettingName] = new NSNumber(false);
                    defaults[SettingsPageViewModel.SearchForDevicesAtStartupSettingName] = new NSNumber(true);
                    defaults[SettingsPageViewModel.ReconcileDeviceMenuWithLocalMenuSettingName] = new NSNumber(false);
                    defaults[SettingsPageViewModel.RunGCWhenConnectedSettingName] = new NSNumber(true);
                    defaults[SettingsPageViewModel.AutomaticallyConnectToDevicesSettingName] = new NSNumber(true);
                    // Don't assign to NULL directly -- MonoMac crashes. Just leave it alone.
                    ////defaults[SettingsPageViewModel.LastActiveDevicePortSettingName] = (NSString)null;
                    defaults[SettingsPageViewModel.ShowAdvancedFeaturesSettingName] = new NSNumber(false);
                    defaults[SettingsPageViewModel.AddRomsToMenuPropertyName] = new NSNumber(true);
                    defaults[SettingsPageViewModel.PromptToAddRomsToMenuPropertyName] = new NSNumber(true);
                    defaults[SettingsPageViewModel.EnablePortLoggingPropertyName] = new NSNumber(false);
                    defaults[SettingsPageViewModel.PromptToInstallFTDIDriverPropertyName] = new NSNumber(true);
                    defaults[SettingsPageViewModel.PromptToImportStarterRomsPropertyName] = new NSNumber(true);
                    defaults[SettingsPageViewModel.PromptForFirmwareUpgradeSettingName] = new NSNumber(true);
                    _userDefaults.RegisterDefaults(defaults);
                }
                return _userDefaults;
            }
        }
        private static NSUserDefaults _userDefaults;

        /// <summary>
        /// Get whether to verify that the ROMs in the menu layout are all available.
        /// </summary>
        public bool ValidateMenuAtStartup
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.ValidateMenuAtStartupSettingName); }
            set { throw new InvalidOperationException("set ValidateMenuAtStartup not supported"); }
        }

        /// <summary>
        /// Get whether to search for connected devices on application launch.
        /// </summary>
        public bool SearchForDevicesAtStartup
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.SearchForDevicesAtStartupSettingName); }
            set { throw new InvalidOperationException("set SearchForDevicesAtStartup not supported"); }
        }

        /// <summary>
        /// Get whether to reconcile local and device menu on device connect.
        /// </summary>
        public bool ReconcileDeviceMenuWithLocalMenu
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.ReconcileDeviceMenuWithLocalMenuSettingName); }
            set { throw new InvalidOperationException("set ReconcileDeviceMenuWithLocalMenu not supported"); }
        }

        /// <summary>
        /// Get whether to show advanced features in the UI.
        /// </summary>
        public bool ShowAdvancedFeatures
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.ShowAdvancedFeaturesSettingName); }
            set { throw new InvalidOperationException("set ShowAdvancedFeatures not supported"); }
        }

        /// <summary>
        /// Get the last serial port used to connect to a Locutus device.
        /// </summary>
        public string LastActiveDevicePort
        {
            get { return UserDefaults.StringForKey(SettingsPageViewModel.LastActiveDevicePortSettingName); }
            set { UpdateProperty(SettingsPageViewModel.LastActiveDevicePortSettingName, value, null, (s, v) => UserDefaults.SetString(v, s)); }
        }

        /// <summary>
        /// Get whether to run garbage collection on a connected Locutus device.
        /// </summary>
        public bool RunGCWhenConnected
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.RunGCWhenConnectedSettingName) || true; }
            set { throw new InvalidOperationException("set RunGCWhenConnected not supported"); }
        }

        /// <summary>
        /// Get whether to automatically connect to any detected Locutus devices.
        /// </summary>
        public bool AutomaticallyConnectToDevices
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.AutomaticallyConnectToDevicesSettingName); }
            set { throw new InvalidOperationException("set AutomaticallyConnectToDevices not supported"); }
        }

        /// <summary>
        /// Get whether to automatically add newly discovered ROMs to the menu layout.
        /// </summary>
        public bool AddRomsToMenu
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.AddRomsToMenuPropertyName); }
            set { throw new InvalidOperationException("set AddRomsToMenu not supported"); }
        }

        /// <summary>
        /// Get whether to prompt to automatically add newly discovered ROMs to the menu layout.
        /// </summary>
        public bool PromptToAddRomsToMenu
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.PromptToAddRomsToMenuPropertyName); }
            set { throw new InvalidOperationException("set PromptToAddRomsToMenu not supported"); }
        }

        /// <summary>
        /// Get whether to log serial port traffic to disk.
        /// </summary>
        public bool EnablePortLogging
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.EnablePortLoggingPropertyName); }
            set { throw new InvalidOperationException("set EnablePortLogging not supported"); }
        }

        /// <summary>
        /// Gets whether to prompt to install the FTDI driver.
        /// </summary>
        public bool PromptToInstallFTDIDriver
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.PromptToInstallFTDIDriverPropertyName); }
            set { UserDefaults.SetBool(value, SettingsPageViewModel.PromptToInstallFTDIDriverPropertyName); }
        }

        /// <summary>
        /// Dummy setting for compatibility with other platforms.
        /// </summary>
        public bool ShowFileSystemDetails
        {
            get { return _showFileSystemDetails; }
            set { AssignAndUpdateProperty(SettingsPageViewModel.ShowFileSystemDetailsSettingName, value, ref _showFileSystemDetails); }
        }
        private bool _showFileSystemDetails;

        /// <summary>
        /// Gets or sets whether to prompt to import starter ROMs.
        /// </summary>
        public bool PromptToImportStarterRoms
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.PromptToImportStarterRomsPropertyName); }
            set { UserDefaults.SetBool(value, SettingsPageViewModel.PromptToImportStarterRomsPropertyName); }
        }

        /// <summary>
        /// Gets or sets whether to prompt to upgrade firmware on device attach that has older firmware than
        /// the version embedded in the shipping product if such has been built in.
        /// </summary>
        public bool PromptForFirmwareUpgrade
        {
            get { return UserDefaults.BoolForKey(SettingsPageViewModel.PromptForFirmwareUpgradeSettingName); }
            set { UserDefaults.SetBool(value, SettingsPageViewModel.PromptForFirmwareUpgradeSettingName); }
        }

        /// <summary>
        /// Save this settings to disk.
        /// </summary>
        public void Save()
        {
            UserDefaults.Synchronize();
        }
    }
}

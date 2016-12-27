// <copyright file="Settings.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Specialized;
using System.Linq;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// Mac-specific Settings-like class.
    /// </summary>
    public class Settings : PropertyChangedNotifier
    {
        private const string _showFTDIWarning = "ShowFTDIWarning";

        #region Constructors

        private Settings()
        {
            NSUserDefaultsObserver.AddPreferenceChangedNotification(RomListSettingsPageViewModel.ValidateAtLaunchPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(RomListSettingsPageViewModel.SearchForRomsAtLaunchPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(RomListSettingsPageViewModel.RomListSearchDirectoriesPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(RomListSettingsPageViewModel.ShowRomDetailsPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(RomListSettingsPageViewModel.DisplayRomFileNameForTitlePropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(GeneralSettingsPageViewModel.ShowDetailedErrorsPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(GeneralSettingsPageViewModel.CheckForAppUpdatesAtLaunchPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(_showFTDIWarning, RaisePropertyChanged);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the "default" settings. Imitates the Default settings as the WPF-style Settings class does.
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
                    defaults[RomListSettingsPageViewModel.ValidateAtLaunchPropertyName] = new NSNumber(true);
                    defaults[RomListSettingsPageViewModel.SearchForRomsAtLaunchPropertyName] = new NSNumber(false);
                    defaults[RomListSettingsPageViewModel.RomListSearchDirectoriesPropertyName] = new NSArray();
                    defaults[RomListSettingsPageViewModel.ShowRomDetailsPropertyName] = new NSNumber(false);
                    defaults[RomListSettingsPageViewModel.DisplayRomFileNameForTitlePropertyName] = new NSNumber(false);
                    defaults[GeneralSettingsPageViewModel.ShowDetailedErrorsPropertyName] = new NSNumber(GeneralSettingsPageViewModel.DefaultShowDetailedErrorsSetting);
                    defaults[GeneralSettingsPageViewModel.CheckForAppUpdatesAtLaunchPropertyName] = new NSNumber(GeneralSettingsPageViewModel.DefaultCheckForUpdatesAtLaunchSetting);
                    defaults[_showFTDIWarning] = new NSNumber(true);
                    _userDefaults.RegisterDefaults(defaults);
                }
                return _userDefaults;
            }
        }
        private static NSUserDefaults _userDefaults;

        /// <summary>
        /// Gets the setting indicating whether to validate the ROMs at startup.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if you attempt to assign the property.</exception>
        public bool RomListValidateAtStartup
        {
            get { return UserDefaults.BoolForKey(RomListSettingsPageViewModel.ValidateAtLaunchPropertyName); }
            set { throw new System.InvalidOperationException("set ValidateAtLaunch not supported"); }
        }

        /// <summary>
        /// Gets the setting indicating whether or not to search for new ROMs when launching the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if you attempt to assign the property.</exception>
        public bool RomListSearchForRomsAtStartup
        {
            get { return UserDefaults.BoolForKey(RomListSettingsPageViewModel.SearchForRomsAtLaunchPropertyName); }
            set { throw new System.InvalidOperationException("set SearchForRomsAtLaunch not supported"); }
        }

        /// <summary>
        /// Gets the list of ROM search directories.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if you attempt to assign the property.</exception>
        public INTV.Shared.Model.SearchDirectories RomListSearchDirectories
        {
            get
            {
                if (_romListSearchDirectories == null)
                {
                    var userDefaults = UserDefaults.StringArrayForKey(RomListSettingsPageViewModel.RomListSearchDirectoriesPropertyName);
                    _romListSearchDirectories = new INTV.Shared.Model.SearchDirectories(userDefaults);
                    _romListSearchDirectories.CollectionChanged += HandleSearchDirectoriesChanged;
                }
                return _romListSearchDirectories;
            }
            set { throw new System.InvalidOperationException("set RomListSearchDirectories not supported"); }
        }
        private INTV.Shared.Model.SearchDirectories _romListSearchDirectories;

        /// <summary>
        /// Gets the setting indicating whether or not to show detailed ROM information in the RomListView.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if you attempt to assign the property.</exception>
        public bool ShowRomDetails
        {
            get { return UserDefaults.BoolForKey(RomListSettingsPageViewModel.ShowRomDetailsPropertyName); }
            set { throw new System.InvalidOperationException("set ShowRomDetails not supported"); }
        }

        /// <summary>
        /// Gets the setting indicating whether or not ROMs are displayed using file name or database name.
        /// </summary>
        public bool DisplayRomFileNameForTitle
        {
            get { return UserDefaults.BoolForKey(RomListSettingsPageViewModel.DisplayRomFileNameForTitlePropertyName); }
            set { throw new System.InvalidOperationException("set DisplayRomFileNameForTitle not supported"); }
        }

        /// <summary>
        /// Gets the setting controlling whether to check for app updates at launch.
        /// </summary>
        public bool CheckForAppUpdatesAtLaunch
        {
            get { return UserDefaults.BoolForKey(GeneralSettingsPageViewModel.CheckForAppUpdatesAtLaunchPropertyName); }
            set { throw new System.InvalidOperationException("set CheckForAppUpdatesAtLaunch NotSupportedException supported"); }
        }

        /// <summary>
        /// Gets the setting controlling whether to show detailed errors.
        /// </summary>
        public bool ShowDetailedErrors
        {
            get { return UserDefaults.BoolForKey(GeneralSettingsPageViewModel.ShowDetailedErrorsPropertyName); }
            set { throw new System.InvalidOperationException("set ShowDetailedErrors not supported"); }
        }

        /// <summary>
        /// Gets the setting indicating whether or not to show the FTDI warning message.
        /// </summary>
        public bool ShowFTDIWarning
        {
            get { return UserDefaults.BoolForKey(_showFTDIWarning); }
            set { UserDefaults.SetBool(value, _showFTDIWarning); }
        }

        #endregion // Properties

        /// <summary>
        /// Saves the settings to disk.
        /// </summary>
        public void Save()
        {
            UserDefaults.Synchronize();
        }

        private void HandleSearchDirectoriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var directories = NSArray.FromStrings(_romListSearchDirectories.ToArray());
            UserDefaults[RomListSettingsPageViewModel.RomListSearchDirectoriesPropertyName] = directories;
        }
    }
}

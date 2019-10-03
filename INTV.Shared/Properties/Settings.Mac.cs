// <copyright file="Settings.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Specialized;
using System.Linq;
using INTV.Shared.Model;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Properties
{
    /// <summary>
    /// Mac-specific Settings-like class.
    /// </summary>
    internal sealed partial class Settings
    {
        private const string ShowFTDIWarningSettingName = "ShowFTDIWarning";
        private INTV.Shared.Model.SearchDirectories _romListSearchDirectories;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not to show the FTDI warning message.
        /// </summary>
        public bool ShowFTDIWarning
        {
            get { return GetSetting<bool>(ShowFTDIWarningSettingName); }
            set { SetSetting(ShowFTDIWarningSettingName, value); }
        }

        #endregion // Properties

        protected override void AddCustomTypeConverters()
        {
            base.AddCustomTypeConverters();
            AddCustomTypeConverter(typeof(SearchDirectories), GetRomListSearchDirectories, SetRomListSearchDirectories, _ => new NSArray());
            AddCustomTypeConverter(typeof(EnabledCompressedArchiveFormats), GetEnabledCompressedArchiveFormats, SetEnabledCompressedArchiveFormats, v => new NSString(v.ToString()));
        }

        private object GetRomListSearchDirectories(string key)
        {
            if (_romListSearchDirectories == null)
            {
                var userDefaults = UserDefaults.StringArrayForKey(key);
                _romListSearchDirectories = new INTV.Shared.Model.SearchDirectories(userDefaults);
                _romListSearchDirectories.CollectionChanged += HandleSearchDirectoriesChanged;
            }
            return _romListSearchDirectories;
        }

        private void SetRomListSearchDirectories(string key, object value)
        {
            var directories = NSArray.FromStrings(((SearchDirectories)value).ToArray());
            UserDefaults[key] = directories;
        }

        private object GetEnabledCompressedArchiveFormats(string key)
        {
            var enabledCompressedArchiveFormats = EnabledCompressedArchiveFormats.None;
            var userDefaultsValue = UserDefaults.StringForKey(key);
            if (!string.IsNullOrEmpty(userDefaultsValue))
            {
                if (!System.Enum.TryParse(userDefaultsValue, out enabledCompressedArchiveFormats))
                {
                    enabledCompressedArchiveFormats = EnabledCompressedArchiveFormats.None;
                }
            }
            return enabledCompressedArchiveFormats;
        }

        private void SetEnabledCompressedArchiveFormats(string key, object value)
        {
            var stringValue = value.ToString();
            UserDefaults[key] = new NSString(stringValue);
        }

        private void HandleSearchDirectoriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetRomListSearchDirectories(RomListSearchDirectoriesSettingName, _romListSearchDirectories);
        }

        /// <summary>
        /// Mac-specific initialization.
        /// </summary>
        partial void OSInitializeDefaults()
        {
            AddSetting(ShowFTDIWarningSettingName, true);
            InitializeUserDefaults();
        }
    }
}

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

namespace INTV.Shared.Properties
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    internal sealed partial class Settings : SettingsBase<Settings>
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to validate the ROMs at startup.
        /// </summary>
        [DataMember]
        public bool RomListValidateAtStartup
        {
            get { return GetSetting<bool>(ValidateAtLaunchSettingName); }
            set { SetSetting(ValidateAtLaunchSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to search for new ROMs when launching the application.
        /// </summary>
        [DataMember]
        public bool RomListSearchForRomsAtStartup
        {
            get { return GetSetting<bool>(SearchForRomsAtLaunchSettingName); }
            set { SetSetting(SearchForRomsAtLaunchSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the list of ROM search directories.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if you attempt to assign the property.</exception>
        [DataMember]
        public INTV.Shared.Model.SearchDirectories RomListSearchDirectories
        {
            get
            {
                return GetSetting<INTV.Shared.Model.SearchDirectories>(RomListSearchDirectoriesSettingName);
            }
            set
            {
                SetSetting(RomListSearchDirectoriesSettingName, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to show detailed ROM information in the RomListView.
        /// </summary>
        [DataMember]
        public bool ShowRomDetails
        {
            get { return GetSetting<bool>(ShowRomDetailsSettingName); }
            set { SetSetting(ShowRomDetailsSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not ROMs are displayed using file name or database name.
        /// </summary>
        [DataMember]
        public bool DisplayRomFileNameForTitle
        {
            get { return GetSetting<bool>(DisplayRomFileNameForTitleSettingName); }
            set { SetSetting(DisplayRomFileNameForTitleSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check for app updates at launch.
        /// </summary>
        [DataMember]
        public bool CheckForAppUpdatesAtLaunch
        {
            get { return GetSetting<bool>(CheckForAppUpdatesAtLaunchSettingName); }
            set { SetSetting(CheckForAppUpdatesAtLaunchSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show detailed errors.
        /// </summary>
        [DataMember]
        public bool ShowDetailedErrors
        {
            get { return GetSetting<bool>(ShowDetailedErrorsSettingName); }
            set { SetSetting(ShowDetailedErrorsSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the maximum number of GZIP members to attempt to parse from a .gz file.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int MaxGZipEntriesSearch
        {
            get { return GetSetting<int>(MaxGZipEntriesSearchSettingName); }
            set { SetSetting(MaxGZipEntriesSearchSettingName, value); }
        }

        #endregion // Properties

        #region ISettings

        /// <inheritdoc/>
        protected override void InitializeDefaults()
        {
            AddSetting(ValidateAtLaunchSettingName, true);
            AddSetting(SearchForRomsAtLaunchSettingName, false);
            AddSetting(RomListSearchDirectoriesSettingName, new INTV.Shared.Model.SearchDirectories());
            AddSetting(ShowRomDetailsSettingName, false);
            AddSetting(DisplayRomFileNameForTitleSettingName, false);
            AddSetting(ShowDetailedErrorsSettingName, DefaultShowDetailedErrorsSetting);
            AddSetting(MaxGZipEntriesSearchSettingName, DefaultMaxGZipEntriesSearchSetting);
            AddSetting(CheckForAppUpdatesAtLaunchSettingName, DefaultCheckForUpdatesAtLaunchSetting);
            OSInitializeDefaults();
        }

        #endregion // ISettings

        /// <summary>
        /// Operating system-specific initialization code is implemented in this method.
        /// </summary>
        partial void OSInitializeDefaults();
    }
}

// <copyright file="Settings.cs" company="INTV Funhouse">
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

using INTV.Shared.Model;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// Common Settings implementation for INTV.Shared.
    /// Defines names of the options. In Windows, these are not used -- these
    /// strings are from the .config file created by Visual Studio. Maybe some
    /// gracious soul with ample free time could write a tool to *generate* this
    /// file for the non-Windows platforms.
    /// </summary>
    internal sealed partial class Settings : ISettings
    {
        #region Setting Names

        #region General Settings

        public const string WindowStateSettingName = "WindowState";
        public const string WindowPositionSettingName = "WindowPosition";
        public const string WindowSizeSettingName = "WindowSize";
        public const string CheckForAppUpdatesAtLaunchSettingName = "CheckForAppUpdatesAtLaunch";
        public const string ShowDetailedErrorsSettingName = "ShowDetailedErrors";
        public const string ClearTemporaryFilesOnStartupSettingName = "ClearTemporaryFilesOnStartup";

        #endregion // General Settings

        #region ROM List Settings

        public const string ValidateAtLaunchSettingName = "RomListValidateAtStartup";
        public const string SearchForRomsAtLaunchSettingName = "RomListSearchForRomsAtStartup";
        public const string RomListSearchDirectoriesSettingName = "RomListSearchDirectories";
        public const string RomListNameColWidthSettingName = "RomListNameColWidth";
        public const string RomListVendorColWidthSettingName = "RomListVendorColWidth";
        public const string RomListYearColWidthSettingName = "RomListYearColWidth";
        public const string RomListFeaturesColWidthSettingName = "RomListFeaturesColWidth";
        public const string RomListPathColWidthSettingName = "RomListPathColWidth";
        ////public const string RomListManualPathColWidthSettingName = "RomListManualPathColWidth";
        public const string ShowRomDetailsSettingName = "ShowRomDetails";
        public const string DisplayRomFileNameForTitleSettingName = "DisplayRomFileNameForTitle";

        #endregion // ROM List Settings

        #region Compressed Archive Access Settings

        public const string EnabledArchiveFormatsSettingName = "EnabledArchiveFormats";
        public const string SearchNestedArchivesSettingName = "SearchNestedArchives";
        public const string MaxArchiveSizeMBSettingName = "MaxArchiveSizeMB";
        public const string MaxGZipEntriesSearchSettingName = "MaxGZipEntriesSearch";

        #endregion // Compressed Archive Access Settings

        #endregion // Setting Names

        /// <summary>The default setting for whether to check for updates at launch.</summary>
        public const bool DefaultCheckForUpdatesAtLaunchSetting = true;

#if DEBUG
        /// <summary>Default value for ShowDetailedErrors option.</summary>
        public const bool DefaultShowDetailedErrorsSetting = true;
#else
        /// <summary>Default value for ShowDetailedErrors option.</summary>
        public const bool DefaultShowDetailedErrorsSetting = false;
#endif // DEBUG

        /// <summary>Default value for ClearTemporaryFilesOnStartup option.</summary>
        public const bool DefaultClearTemporaryFilesOnStartupSetting = false;

        /// <summary>Default value for EnabledArchiveFormats option.</summary>
        public const EnabledCompressedArchiveFormats DefaultEnabledArchiveFormats = EnabledCompressedArchiveFormats.None;

        /// <summary>Default value for SearchNestedArchives option.</summary>
        public const bool DefaultSearchNestedArchives = false;

        /// <summary>Default value for MaxArchiveSizeMBSetting option.</summary>
        public const int DefaultMaxArchiveSizeMBSetting = 32;

        /// <summary>Default value for MaxGZipEntriesSearch option.</summary>
        public const int DefaultMaxGZipEntriesSearchSetting = 4;
    }
}

// <copyright file="RomListSettingsPageViewModel.cs" company="INTV Funhouse">
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

#if WIN || GTK
using RomListSettingsVisualType = INTV.Shared.View.RomListSettingsPage;
#elif MAC
using RomListSettingsVisualType = INTV.Shared.View.RomListSettingsPageController;
#endif // WIN || GTK

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the ROM list settings page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ISettingsPage))]
    [LocalizedName(typeof(Resources.Strings), "RomListSettingsPage_Title")]
    [Weight(0.1)]
    [Icon("Resources/Images/rom_32xMD.png")]
    public sealed partial class RomListSettingsPageViewModel : SettingsPageViewModel<RomListSettingsVisualType>
    {
        #region Property Names

        public const string ValidateAtLaunchPropertyName = "ValidateAtLaunch";
        public const string SearchForRomsAtLaunchPropertyName = "SearchForRomsAtLaunch";
        public const string RomListSearchDirectoriesPropertyName = "RomListSearchDirectories";
        public const string ShowRomDetailsPropertyName = "ShowRomDetails";
        public const string DisplayRomFileNameForTitlePropertyName = "DisplayRomFileNameForTitle";

        #endregion // Property Names

        #region UI Strings

        public static readonly string ValidateSettingsPreferenceText = Resources.Strings.RomListSettingsPage_ValidateAtLaunch;
        public static readonly string ScanForRomsPreferenceText = Resources.Strings.RomListSettingsPage_RescanDirectoriesAtLaunch;
        public static readonly string RomDirectories = Resources.Strings.RomListSettingsPage_RomDirectories;
        public static readonly string ShowRomDetailsPreferenceText = Resources.Strings.RomListSettingsPage_ShowRomDetails;
        public static readonly string DisplayRomFileNameForTitlePreferenceText = Resources.Strings.RomListSettingsPage_DisplayRomFileNameForTitle;

        #endregion // UI Strings

        /// <summary>
        /// Initializes a new instance of RomListSettingsPageViewModel.
        /// </summary>
        public RomListSettingsPageViewModel()
        {
            _validateAtLaunch = Properties.Settings.Default.RomListValidateAtStartup;
            _searchForRomsAtLaunch = Properties.Settings.Default.RomListSearchForRomsAtStartup;
            _showRomDetails = Properties.Settings.Default.ShowRomDetails;
            _displayRomFileNameForTitle = Properties.Settings.Default.DisplayRomFileNameForTitle;
        }

        #region Properties

        /// <summary>
        /// Gets the command to delete an item from the menu.
        /// </summary>
        public static ICommand DeleteSearchDirectoryCommand
        {
            get { return new RelayCommand(DeleteSearchDirectory); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the list of ROMs should be validated when the application launches.
        /// </summary>
        public bool ValidateAtLaunch
        {
            get { return _validateAtLaunch; }
            set { AssignAndUpdateProperty(ValidateAtLaunchPropertyName, value, ref _validateAtLaunch, (n, v) => Properties.Settings.Default.RomListValidateAtStartup = v); }
        }
        private bool _validateAtLaunch;

        /// <summary>
        /// Gets or sets a value indicating whether the application should search in its search directories for new ROMs on launch.
        /// </summary>
        public bool SearchForRomsAtLaunch
        {
            get { return _searchForRomsAtLaunch; }
            set { AssignAndUpdateProperty(SearchForRomsAtLaunchPropertyName, value, ref _searchForRomsAtLaunch, (n, v) => Properties.Settings.Default.RomListSearchForRomsAtStartup = v); }
        }
        private bool _searchForRomsAtLaunch;

        /// <summary>
        /// Gets the list of search directories.
        /// </summary>
        public INTV.Shared.Model.SearchDirectories SearchDirectories
        {
            get { return Properties.Settings.Default.RomListSearchDirectories; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show all the ROM details in the ROM list, such as vendor, date, and features.
        /// </summary>
        public bool ShowRomDetails
        {
            get { return _showRomDetails; }
            set { AssignAndUpdateProperty(ShowRomDetailsPropertyName, value, ref _showRomDetails, (n, v) => Properties.Settings.Default.ShowRomDetails = v); }
        }
        private bool _showRomDetails;

        /// <summary>
        /// Gets or sets a value indicating whether to display the ROM file name for title.
        /// </summary>
        public bool DisplayRomFileNameForTitle
        {
            get { return _displayRomFileNameForTitle; }
            set { AssignAndUpdateProperty(DisplayRomFileNameForTitlePropertyName, value, ref _displayRomFileNameForTitle, (n, v) => Properties.Settings.Default.DisplayRomFileNameForTitle = v); }
        }
        private bool _displayRomFileNameForTitle;

        #region Commands

        #endregion // Commands

        #endregion // Properties

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
        }

        private static void DeleteSearchDirectory(object parameter)
        {
            Properties.Settings.Default.RomListSearchDirectories.Remove(parameter as string);
        }
    }
}

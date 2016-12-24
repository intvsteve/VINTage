// <copyright file="MenuLayoutViewModel.WPF.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class MenuLayoutViewModel
    {
        /// <summary>
        /// The settings to use.
        /// </summary>
        public static readonly System.Configuration.ApplicationSettingsBase Settings = Properties.Settings.Default;

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        partial void Initialize()
        {
            _longNameColumnWidth = Properties.Settings.Default.MenuLayoutLongNameColWidth;
            _shortNameColumnWidth = Properties.Settings.Default.MenuLayoutShortNameColWidth;
            _manualColumnWidth = Properties.Settings.Default.MenuLayoutManualColWidth;
            _saveDataColumnWidth = Properties.Settings.Default.MenuLayoutSaveDataColWidth;
            ViewWidth = Properties.Settings.Default.MenuLayoutWidth;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to edit the currently selected ROM.
        /// </summary>
        /// <remarks>This should only be set if one and only one ROM is selected.</remarks>
        public bool IsEditing
        {
            get { return _isEditing; }
            set { AssignAndUpdateProperty("IsEditing", value, ref _isEditing); }
        }
        private bool _isEditing;

        /// <summary>
        /// Gets a value indicating whether it's possible to edit the color of the selected item(s).
        /// </summary>
        public bool IsColorEditable
        {
            get { return CurrentSelection != null; }
        }

        /// <summary>
        /// Gets or sets the observed actual width of the menu layout control.
        /// </summary>
        public double ViewWidth
        {
            get { return _viewWidth; }
            set { AssignAndUpdateProperty("ViewWidth", value, ref _viewWidth, (p, v) => Properties.Settings.Default.MenuLayoutWidth = v); }
        }
        private double _viewWidth;

        /// <summary>
        /// Gets or sets the width of the 'Long Name' column. When set, the settings information will also be updated.
        /// </summary>
        public double LongNameColumnWidth
        {
            get { return _longNameColumnWidth; }
            set { AssignAndUpdateProperty("LongNameColumnWidth", value, ref _longNameColumnWidth, (n, w) => Properties.Settings.Default.MenuLayoutLongNameColWidth = w); }
        }
        private double _longNameColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'Short Name' column. When set, the settings information will also be updated.
        /// </summary>
        public double ShortNameColumnWidth
        {
            get { return _shortNameColumnWidth; }
            set { AssignAndUpdateProperty("ShortNameColumnWidth", value, ref _shortNameColumnWidth, (n, w) => Properties.Settings.Default.MenuLayoutShortNameColWidth = w); }
        }
        private double _shortNameColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'Manual' column. When set, the settings information will also be updated.
        /// </summary>
        public double ManualColumnWidth
        {
            get { return _manualColumnWidth; }
            set { AssignAndUpdateProperty("ManualColumnWidth", value, ref _manualColumnWidth, (n, w) => Properties.Settings.Default.MenuLayoutManualColWidth = w); }
        }
        private double _manualColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'Save Data' column. When set, the settings information will also be updated.
        /// </summary>
        public double SaveDataColumnWidth
        {
            get
            {
                return _saveDataColumnWidth;
            }

            set
            {
                AssignAndUpdateProperty(
                    "SaveDataColumnWidth",
                    value,
                    ref _saveDataColumnWidth,
                    (n, w) =>
                    {
                        if (w != 0)
                        {
                            Properties.Settings.Default.MenuLayoutSaveDataColWidth = w;
                        }
                    });
            }
        }
        private double _saveDataColumnWidth;
    }
}

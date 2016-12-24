// <copyright file="ProgramDescriptionViewModel.WPF.cs" company="INTV Funhouse">
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

using INTV.Core.ComponentModel;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class ProgramDescriptionViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "IsSelected", value, ref _isSelected); }
        }
        private bool _isSelected;

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        partial void Initialize()
        {
            Properties.Settings.Default.PropertyChanged += HandleSettingChanged;
        }

        private void HandleSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case RomListSettingsPageViewModel.DisplayRomFileNameForTitlePropertyName:
                    this.RaisePropertyChanged(PropertyChanged, "Name");
                    break;
                default:
                    break;
            }
        }
    }
}

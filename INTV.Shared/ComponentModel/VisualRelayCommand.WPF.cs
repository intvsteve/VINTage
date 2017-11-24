// <copyright file="VisualRelayCommand.WPF.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class VisualRelayCommand
    {
        /// <summary>
        /// Gets or sets a value indicating whether the command creates its visual by loading a XAML resource file.
        /// </summary>
        public bool UseXamlResource { get; set; }

        private string GetMenuItemName()
        {
            string menuItemName = null;
            var menuItem = MenuItem.NativeMenuItem;
            if (menuItem != null)
            {
                menuItemName = menuItem.Header as string;
            }
            return menuItemName;
        }

        private void SetMenuItemName(string menuItemName)
        {
            var menuItem = MenuItem.NativeMenuItem;
            if (menuItem != null)
            {
                menuItem.Header = menuItemName;
            }
        }
    }
}

// <copyright file="VisualRelayCommand.Gtk.cs" company="INTV Funhouse">
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

using INTV.Shared.View;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class VisualRelayCommand
    {
        private string GetMenuItemName()
        {
            string menuItemName = MenuItem.Name;
            return menuItemName;
        }

        private void SetMenuItemName(string menuItemName)
        {
            MenuItem.SetName(menuItemName);
        }

        private void HandleRequerySuggested(object sender, System.EventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        partial void Initialize()
        {
            CommandManager.RequerySuggested += HandleRequerySuggested;
        }
    }
}

// <copyright file="OSMenuItem.Gtk.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial struct OSMenuItem
    {
        /// <summary>
        /// Gets the name of the menu item.
        /// </summary>
        public string Name
        {
            get
            {
                var label = IsEmpty ? null : _menuItem.Child as Gtk.Label;
                return label != null ? label.Text : null;
            }
        }

        /// <summary>
        /// Sets the name of the menu item.
        /// </summary>
        /// <param name="name">The new name for the menu item.</param>
        public void SetName(string name)
        {
            var label = IsEmpty ? null : _menuItem.Child as Gtk.Label;
            if (label != null)
            {
                label.Text = name;
            }
        }
    }
}

// <copyright file="OSMenuItem.WPF.cs" company="INTV Funhouse">
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

using NativeMenuItemBase = System.Windows.Controls.Control; // can't use MenuItem because of RibbonSeparator :/

namespace INTV.Shared.View
{
    /// <summary>
    /// WPF-specific implementation.
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
                var name = NativeMenuItem == null ? string.Empty : NativeMenuItem.Header as string;
                return name;
            }
        }

        /// <summary>
        /// Gets the raw menu item type.
        /// </summary>
        public System.Windows.Controls.Control NativeMenuItemBase
        {
            get { return _menuItem; }
        }

        /// <summary>>Wraps a platform-specific menu item in an abstracted menu item base type.</summary>
        /// <param name="menuItem">A platform-specific menu item to wrap in the abstraction.</param>
        /// <returns>The wrapped menu item.</returns>
        public static implicit operator OSMenuItem(NativeMenuItemBase menuItem)
        {
            return new OSMenuItem(menuItem);
        }

        /// <summary>>Unwraps the native menu item from a platform-abstract menu item base.</summary>
        /// <param name="menuItem">The abstracted menu item to convert to a platform-specific menu item.</param>
        /// <returns>The native menu item.</returns>
        public static implicit operator NativeMenuItemBase(OSMenuItem menuItem)
        {
            return menuItem._menuItem;
        }
    }
}

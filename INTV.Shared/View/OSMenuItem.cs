// <copyright file="OSMenuItem.cs" company="INTV Funhouse">
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

#if WIN
using NativeMenuItem = System.Windows.Controls.MenuItem;
using NativeMenuItemBase = System.Windows.Controls.Control; // can't use MenuItem because of RibbonSeparator :/
#elif MAC
#if __UNIFIED__
using NativeMenuItem = AppKit.NSMenuItem;
using NativeMenuItemBase = AppKit.NSMenuItem;
#else
using NativeMenuItem = MonoMac.AppKit.NSMenuItem;
using NativeMenuItemBase = MonoMac.AppKit.NSMenuItem;
#endif // __UNIFIED__
#elif GTK
using NativeMenuItem = Gtk.MenuItem;
using NativeMenuItemBase = Gtk.MenuItem;
#endif // WIN

namespace INTV.Shared.View
{
    /// <summary>
    /// Platform abstraction for native menu item.
    /// </summary>
    public partial struct OSMenuItem
    {
        /// <summary>
        /// The canonical empty menu item.
        /// </summary>
        public static readonly OSMenuItem Empty = new OSMenuItem(null);

        private NativeMenuItemBase _menuItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.OSMenuItem"/> struct.
        /// </summary>
        /// <param name="menuItem">A platform-specific menu item.</param>
        public OSMenuItem(NativeMenuItemBase menuItem)
        {
            _menuItem = menuItem;
        }

        /// <summary>
        /// Gets the native menu item.
        /// </summary>
        public NativeMenuItem NativeMenuItem
        {
            get { return _menuItem as NativeMenuItem; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _menuItem == null; }
        }

        /// <summary>>Wraps a platform-specific menu item in an abstracted menu item.</summary>
        /// <param name="menuItem">A platform-specific menu item to wrap in the abstraction.</param>
        /// <returns>The wrapped menu item.</returns>
        public static implicit operator OSMenuItem(NativeMenuItem menuItem)
        {
            return new OSMenuItem(menuItem);
        }

        /// <summary>>Unwraps the native menu item from a platform-abstract menu item.</summary>
        /// <param name="menuItem">The abstracted menu item to convert to a platform-specific menu item.</param>
        /// <returns>The native menu item.</returns>
        public static implicit operator NativeMenuItem(OSMenuItem menuItem)
        {
            return menuItem._menuItem as NativeMenuItem;
        }
    }
}

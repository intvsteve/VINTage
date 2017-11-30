// <copyright file="OSModifierKeys.Gtk.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Alternative name for modifier keys used in keyboard shortcuts.
    /// </summary>
    [System.Flags]
    public enum OSModifierKeys
    {
        /// <summary>
        /// No modifier keys used.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Shift key.
        /// </summary>
        Shift = Gdk.ModifierType.ShiftMask,

        /// <summary>
        /// The Control key.
        /// </summary>
        Ctrl = Gdk.ModifierType.ControlMask,

        /// <summary>
        /// The Alternate (Alt) key.
        /// </summary>
        Alt = Gdk.ModifierType.Mod1Mask,

        /// <summary>
        /// Pseudonym for the Control (Ctrl) key.
        /// </summary>
        Menu = Gdk.ModifierType.ControlMask,

        /// <summary>
        /// Pseudonym for the Windows key.
        /// </summary>
        OS = Gdk.ModifierType.SuperMask // Ubuntu? (not tested) ... or... Mod4?
    }
}

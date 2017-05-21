// <copyright file="OSModifierKeys.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

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
        Shift = (int)NSEventModifierMask.ShiftKeyMask,

        /// <summary>
        /// The Control key.
        /// </summary>
        Ctrl = (int)NSEventModifierMask.ControlKeyMask,

        /// <summary>
        /// The Alternate (Option) key.
        /// </summary>
        Alt = (int)NSEventModifierMask.AlternateKeyMask,

        /// <summary>
        /// The Command (Apple) key.
        /// </summary>
        Menu = (int)NSEventModifierMask.CommandKeyMask,

        /// <summary>
        /// Pseudonym for the Command (Apple) key.
        /// </summary>
        OS = (int)NSEventModifierMask.CommandKeyMask,
    }
}

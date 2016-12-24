// <copyright file="MenuLayoutSynchronizationMode.cs" company="INTV Funhouse">
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
    /// Describes how the menu layout should display icon states - compare vs. ROM list or compare vs. Locutus File System.
    /// </summary>
    internal enum MenuLayoutSynchronizationMode
    {
        /// <summary>
        /// No special mode. Leave icons alone.
        /// </summary>
        None,

        /// <summary>
        /// Show item discrepancies between menu and ROM list.
        /// </summary>
        RomList,

        /// <summary>
        /// Show item discrepancies when copying files to LTO Flash! device.
        /// </summary>
        ToLtoFlash,

        /// <summary>
        /// Show item discrepancies when making local layout match device layout.
        /// </summary>
        FromLtoFlash,

        /// <summary>
        /// Show item discrepancies between two LTO Flash! devices.
        /// </summary>
        FlashToFlash,
    }
}

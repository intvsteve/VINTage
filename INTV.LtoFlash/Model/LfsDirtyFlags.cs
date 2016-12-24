// <copyright file="LfsDirtyFlags.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// These flags are used for state tracking when the user interface performs operations that
    /// affect the contents or state of the Locutus File System.
    /// </summary>
    [System.Flags]
    public enum LfsDirtyFlags : uint
    {
        /// <summary>
        /// No pending operations.
        /// </summary>
        None = 0,

        /// <summary>
        /// Locutus should ignore these bits. Their meaning is reserved for future use.
        /// </summary>
        ReservedMask = 0x7FFFFFFF,

        /// <summary>
        /// This flag indicates that the file system is being updated, and still has pending operations. If it is set upon
        /// attaching to a device, it indicates that the file system is in an inconsistent state.
        /// </summary>
        FileSystemUpdateInProgress = 1u << 31,
    }
}

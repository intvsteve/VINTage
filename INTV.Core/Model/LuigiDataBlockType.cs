// <copyright file="LuigiDataBlockType.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// This enumerates the documented LUIGI data block types.
    /// </summary>
    public enum LuigiDataBlockType : byte
    {
        /// <summary>
        /// Scramble / descramble key block.
        /// </summary>
        SetScrambleKey = 0x00,

        /// <summary>
        /// Memory map and permissions descriptor block.
        /// </summary>
        MemoryMapAndPermissionsTable = 0x01,

        /// <summary>
        /// A block of data for a ROM.
        /// </summary>
        DataHunk = 0x02,

        /// <summary>
        /// A block of data containing metadata about a ROM.
        /// </summary>
        Metadata = 0x03,

        /// <summary>
        /// End-of-file sentinal block.
        /// </summary>
        EndOfFile = 0xFF,

        /// <summary>
        /// NOT SANCTIONED: Unrecognized LUIGI block type.
        /// </summary>
        UnknownBlockType = 0xFE
    }
}

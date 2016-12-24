// <copyright file="RomComparison.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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
    /// This enumeration specifies different ways to compare two ROMs.
    /// </summary>
    public enum RomComparison
    {
        /// <summary>
        /// When comparing two ROMs, the file formats must match, ROM contents must match, and, in the case of a .bin + .cfg ROM, the .cfg files must exactly match.
        /// </summary>
        Strict,

        /// <summary>
        /// When comparing two ROMs, the file formats must match and the ROM contents must match; differences in .cfg contents are ignored.
        /// </summary>
        StrictRomCrcOnly,

        /// <summary>
        /// When comparing two ROMs, the ROM contents must match, based on a canonical ROM format comparison, which includes all configurable aspects that can be defined,
        /// such as LUIGI feature flags, or .cfg file content.
        /// </summary>
        CanonicalStrict,

        /// <summary>
        /// When comparing two ROMs, the original ROM contents must match, but not necessarily all features, such as LUIGI feature flags or .cfg file content.
        /// </summary>
        CanonicalRomCrcOnly
    }
}

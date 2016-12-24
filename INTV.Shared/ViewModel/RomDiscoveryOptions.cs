// <copyright file="RomDiscoveryOptions.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// These flags describe various descriptors when discovering new ROMs.
    /// </summary>
    [System.Flags]
    internal enum RomDiscoveryOptions
    {
        /// <summary>
        /// Nothing found or to do.
        /// </summary>
        None = 0,

        /// <summary>
        /// Automatically add newly found ROMs.
        /// </summary>
        AddNewRoms = 1 << 0,

        /// <summary>
        /// Detect any changes to the known ROMs.
        /// </summary>
        DetectChanges = 1 << 1,

        /// <summary>
        /// Detect new ROMs.
        /// </summary>
        DetectNewRoms = 1 << 2,

        /// <summary>
        /// Detect missing ROMs.
        /// </summary>
        DetectMissingRoms = 1 << 3,

        /// <summary>
        /// Accumulate a list of files that were not accepted as valid ROMs.
        /// </summary>
        AccumulateRejectedRoms = 1 << 4,

        /// <summary>
        /// All options.
        /// </summary>
        AllOptions = -1
    }
}

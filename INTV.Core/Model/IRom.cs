// <copyright file="IRom.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Interface to an Intellivision program's ROM image.
    /// </summary>
    public interface IRom
    {
        /// <summary>
        /// Gets the format of the ROM.
        /// </summary>
        RomFormat Format { get; }

        /// <summary>
        /// Gets the location of the ROM image.
        /// </summary>
        StorageLocation RomPath { get; }

        /// <summary>
        /// Gets the location of the ROM's configuration data.
        /// </summary>
        StorageLocation ConfigPath { get; }

        /// <summary>
        /// Gets a value indicating whether the ROM is valid or not.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the 32-bit CRC of a ROM image.
        /// </summary>
        uint Crc { get; }

        /// <summary>
        /// Gets the 32-bit CRC of a companion .cfg of a ROM if the ROM format supports it.
        /// </summary>
        uint CfgCrc { get; }

        /// <summary>
        /// Validate the ROM.
        /// </summary>
        /// <returns><c>true</c> if the the ROM is considered valid after the operation.</returns>
        bool Validate();

        /// <summary>
        /// Refreshes the CRC of the ROM image.
        /// </summary>
        /// <returns>The CRC of the ROM.</returns>
        /// <param name="changed">Set to <c>true</c> if the CRC value changed.</param>
        uint RefreshCrc(out bool changed);

        /// <summary>
        /// Refreshes the CRC of the configuration data for the ROM, if one is present.
        /// </summary>
        /// <returns>The CRC of the configuration, if present.</returns>
        /// <param name="changed">Set to <c>true</c> if the CRC value changed.</param>
        uint RefreshCfgCrc(out bool changed);
    }
}

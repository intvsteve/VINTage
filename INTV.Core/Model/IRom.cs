// <copyright file="IRom.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Interface to an Intellivision program's ROM file.
    /// </summary>
    public interface IRom
    {
        /// <summary>
        /// Gets the format of the ROM.
        /// </summary>
        RomFormat Format { get; }

        /// <summary>
        /// Gets the full path to the ROM file.
        /// </summary>
        string RomPath { get; }

        /// <summary>
        /// Gets the full path to the ROM's configuration file.
        /// </summary>
        string ConfigPath { get; }

        /// <summary>
        /// Gets a value indicating whether the ROM is valid or not.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the 32-bit CRC of a ROM file.
        /// </summary>
        uint Crc { get; }

        /// <summary>
        /// Gets the 32-bit CRC of a companion .cfg of a ROM file if the ROM format supports it.
        /// </summary>
        uint CfgCrc { get; }

        /// <summary>
        /// Validate the ROM.
        /// </summary>
        /// <returns><c>true</c> if the the ROM is considered valid after the operation.</returns>
        bool Validate();

        /// <summary>
        /// Refreshes the CRC of the ROM file.
        /// </summary>
        /// <returns>The CRC of the ROM.</returns>
        /// <param name="changed">Set to <c>true</c> if the CRC value changed.</param>
        uint RefreshCrc(out bool changed);

        /// <summary>
        /// Refreshes the CRC of the configuration data file for the ROM, if one is present.
        /// </summary>
        /// <returns>The CRC of the configuration file, if present.</returns>
        /// <param name="changed">Set to <c>true</c> if the CRC value changed.</param>
        uint RefreshCfgCrc(out bool changed);
    }
}

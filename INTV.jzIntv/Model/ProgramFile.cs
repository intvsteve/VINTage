// <copyright file="ProgramFile.cs" company="INTV Funhouse">
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

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// Identifies the various utility programs that are part of jzIntv and its SDK.
    /// </summary>
    public enum ProgramFile
    {
        /// <summary>
        /// The jzIntv emulator application.
        /// </summary>
        Emulator,

        /// <summary>
        /// The intvname utility, which attempts to load enough of a ROM to get the program title and copyright date.
        /// </summary>
        IntvName,

        /// <summary>
        /// Converts IMV to GIF.
        /// </summary>
        ImvToGif,

        /// <summary>
        /// Converts IMV to PPM.
        /// </summary>
        ImvToPpm,

        /// <summary>
        /// The AS-1600 assembler.
        /// </summary>
        As1600,

        /// <summary>
        /// Converts a .bin format ROM to a .rom format ROM.
        /// </summary>
        Bin2Rom,

        /// <summary>
        /// Updates for cgc?
        /// </summary>
        CgcUpdate,

        /// <summary>
        /// Computes the CRC-32 checksum of a file.
        /// </summary>
        Crc32,

        /// <summary>
        /// Disassembler for ...
        /// </summary>
        Dasm0256,

        /// <summary>
        /// Disassembler for CP-1600.
        /// </summary>
        Dasm1600,

        /// <summary>
        /// Disassembler for CP-1600.
        /// </summary>
        Dasm1600o,

        /// <summary>
        /// Not sure.
        /// </summary>
        Dis1600,

        /// <summary>
        /// Not sure.
        /// </summary>
        FromBit,

        /// <summary>
        /// Not sure.
        /// </summary>
        FromBitR,

        /// <summary>
        /// Not sure.
        /// </summary>
        FromHex,

        /// <summary>
        /// Not sure.
        /// </summary>
        Gms2Rom,

        /// <summary>
        /// Merges multiple ROM-format files into a single ROM file.
        /// </summary>
        RomMerge,

        /// <summary>
        /// Converts a ROM format file to a .bin + .cfg file pair.
        /// </summary>
        Rom2Bin,

        /// <summary>
        /// Not sure.
        /// </summary>
        ShowGrom,

        /// <summary>
        /// Not sure.
        /// </summary>
        SplitRom,

        /// <summary>
        /// Not sure.
        /// </summary>
        ToBit,

        /// <summary>
        /// Not sure.
        /// </summary>
        ToBitF,

        /// <summary>
        /// Not sure.
        /// </summary>
        ToBitR,

        /// <summary>
        /// Not sure.
        /// </summary>
        ToHex,

        /// <summary>
        /// Converts a .bin + .cfg file to LUIGI file format for LTO Flash.
        /// </summary>
        Bin2Luigi,

        /// <summary>
        /// Converts a .rom / .cc3 file to LUIGI file format for LTO Flash.
        /// </summary>
        Rom2Luigi,

        /// <summary>
        /// Converts a LUIGI file from LTO Flash to a .bin + .cfg.
        /// </summary>
        Luigi2Bin,
    }
}

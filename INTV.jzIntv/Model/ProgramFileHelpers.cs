// <copyright file="ProgramFileHelpers.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Model;

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// Extension methods for working with the ProgramFile enumeration.
    /// </summary>
    public static class ProgramFileHelpers
    {
        private static readonly Dictionary<ProgramFile, string> ProgramNames = new Dictionary<ProgramFile, string>()
        {
            { ProgramFile.Emulator, "jzintv" },
            { ProgramFile.IntvName, "intvname" },
            { ProgramFile.ImvToGif, "imvtogif" },
            { ProgramFile.ImvToPpm, "imvtoppm" },
            { ProgramFile.As1600, "as1600" },
            { ProgramFile.Bin2Rom, "bin2rom" },
            { ProgramFile.CgcUpdate, "cgc_update" },
            { ProgramFile.Crc32, "crc32" },
            { ProgramFile.Dasm0256, "dasm0256" },
            { ProgramFile.Dasm1600, "dasm1600" },
            { ProgramFile.Dasm1600o, "dasm1600o" },
            { ProgramFile.Dis1600, "dis1600" },
            { ProgramFile.FromBit, "frombit" },
            { ProgramFile.FromBitR, "frombit_r" },
            { ProgramFile.FromHex, "fromhex" },
            { ProgramFile.Gms2Rom, "gms2rom" },
            { ProgramFile.RomMerge, "rom_merge" },
            { ProgramFile.Rom2Bin, "rom2bin" },
            { ProgramFile.ShowGrom, "show_grom" },
            { ProgramFile.SplitRom, "split_rom" },
            { ProgramFile.ToBit, "tobit" },
            { ProgramFile.ToBitF, "tobit_f" },
            { ProgramFile.ToBitR, "tobit_r" },
            { ProgramFile.ToHex, "tohex" },
            { ProgramFile.Bin2Luigi, "bin2luigi" },
            { ProgramFile.Rom2Luigi, "rom2luigi" },
            { ProgramFile.Luigi2Bin, "luigi2bin" },
        };

        /// <summary>
        /// Gets the name of a program given a ProgramFile.
        /// </summary>
        /// <param name="program">The program whose executable name is desired.</param>
        /// <returns>The name of the executable file.</returns>
        public static string ProgramName(this ProgramFile program)
        {
            return ProgramNames[program];
        }

        /// <summary>
        /// Given a <see cref="ProgramFile"/>, return the format of the ROM produced by the program, if applicable.
        /// </summary>
        /// <param name="program">The support program for which the output ROM format is desired.</param>
        /// <returns>The resulting ROM produced by the program, or <see cref="RomFormat.None"/> if not applicable.</returns>
        public static RomFormat OutputFormat(this ProgramFile program)
        {
            var outputFormat = RomFormat.None;
            switch (program)
            {
                case ProgramFile.Bin2Luigi:
                case ProgramFile.Rom2Luigi:
                    outputFormat = RomFormat.Luigi;
                    break;
                case ProgramFile.Bin2Rom:
                    outputFormat = RomFormat.Rom;
                    break;
                case ProgramFile.Rom2Bin:
                case ProgramFile.Luigi2Bin:
                    outputFormat = RomFormat.Bin;
                    break;
            }
            return outputFormat;
        }

        /// <summary>
        /// Retrieve the command line arguments to use for the bin2rom utility based on the target ROM format.
        /// </summary>
        /// <param name="targetFormat">Target ROM format.</param>
        /// <returns>The command line arguments to use.</returns>
        public static string GetCommandLineArgForBin2Rom(this RomFormat targetFormat)
        {
            var arg = string.Empty;
            switch (targetFormat)
            {
                case RomFormat.Intellicart:
                    break;
                case RomFormat.CuttleCart3:
                    arg = "--cc3";
                    break;
                case RomFormat.CuttleCart3Advanced:
                    arg = "--adv";
                    break;
            }
            return arg;
        }
    }
}

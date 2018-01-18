// <copyright file="ProgramDescriptionHelpers.cs" company="INTV Funhouse">
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

using System;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Extension methods for <see cref="ProgramDescription"/>.
    /// </summary>
    public static class ProgramDescriptionHelpers
    {
        /// <summary>
        /// Gets the ROM to use from a <see cref="ProgramDescription"/> for deployment to a device, e.g. Intellicart, CC3, LTO Flash!, emulator, et. al.
        /// </summary>
        /// <param name="programDescription">A <see cref="ProgramDescription"/> that wraps a specific <see cref="IRom"/>.</param>
        /// <returns>The specific <see cref="IRom"/> to use for deployment.</returns>
        /// <remarks>A specific <see cref="IRom"/> may refer to a file that is not presently accessible, e.g. a ROM on a CD-ROM drive, network volume, or other
        /// non-fixed-disk location. In such a case, if the <paramref name="programDescription"/> supplies alternative location(s), the first suitable alternative
        /// location that can be accessed will be used to return a viable <see cref="IRom"/> that, hopefully, is the equivalent of the original.</remarks>
        public static IRom GetRom(this ProgramDescription programDescription)
        {
            var rom = programDescription.Rom;
            var usesCfg = !string.IsNullOrEmpty(rom.ConfigPath);
            if (!rom.RomPath.FileExists() || (usesCfg && !rom.ConfigPath.FileExists()))
            {
                var alternateRomPaths = programDescription.Files.AlternateRomImagePaths;
                var alternateCfgPaths = programDescription.Files.AlternateRomConfigurationFilePaths;

                if (usesCfg && (alternateRomPaths.Count != alternateCfgPaths.Count))
                {
                    throw new InvalidOperationException(Resources.Strings.ProgramDescription_MIssingAlternateCfgFile);
                }

                var foundAlternate = false;
                string romPath = null;
                string cfgPath = null;
                for (var i = 0; (i < alternateRomPaths.Count) && !foundAlternate; ++i)
                {
                    if (alternateRomPaths[i].FileExists())
                    {
                        romPath = alternateRomPaths[i];
                        if (usesCfg)
                        {
                            if ((i < alternateCfgPaths.Count) && alternateCfgPaths[i].FileExists())
                            {
                                // This code assumes (but cannot check -- silly PCL has no Path API) that the .cfg and ROM are in the same directory for the same index.
                                cfgPath = alternateCfgPaths[i];
                                foundAlternate = true;
                            }
                        }
                        else
                        {
                            foundAlternate = true;
                        }
                    }
                }
                if (foundAlternate)
                {
                    rom = new AlternateRom(romPath, cfgPath, rom);
                }
            }
            return rom;
        }
    }
}

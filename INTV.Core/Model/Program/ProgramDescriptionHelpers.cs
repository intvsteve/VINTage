// <copyright file="ProgramDescriptionHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2015-2018 All Rights Reserved
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
using INTV.Core.Restricted.Model.Program;
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
            if (!StreamUtilities.FileExists(rom.RomPath) || (usesCfg && !StreamUtilities.FileExists(rom.ConfigPath)))
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
                    if (StreamUtilities.FileExists(alternateRomPaths[i]))
                    {
                        romPath = alternateRomPaths[i];
                        if (usesCfg)
                        {
                            if ((i < alternateCfgPaths.Count) && StreamUtilities.FileExists(alternateCfgPaths[i]))
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

        /// <summary>
        /// Determines if a given <see cref="IProgramDescription"/> is considered a match based on given criteria.
        /// </summary>
        /// <param name="programDescription">An instance</param>
        /// <param name="programIdentifier">Provides the unique identifier that must match the value that can be determined from <paramref name="programDescription"/>.</param>
        /// <returns><c>true</c> if all of the specified criteria match the corresponding data in <paramref name="programDescription"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="programDescription"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="programIdentifier"/> is invalid.</exception>
        public static bool IsMatchingProgramDescription(this IProgramDescription programDescription, ProgramIdentifier programIdentifier)
        {
            var match = IsMatchingProgramDescription(programDescription, programIdentifier, RomFormat.None, false, null);
            return match;
        }

        /// <summary>
        /// Determines if a given <see cref="IProgramDescription"/> is considered a match based on given criteria.
        /// </summary>
        /// <param name="programDescription">An instance</param>
        /// <param name="programIdentifier">Provides the unique identifier that must match the value that can be determined from <paramref name="programDescription"/>.</param>
        /// <param name="romFormat">If this value is not <see cref="RomFormat.None"/>, then <paramref name="programDescription"/> must have the same ROM format to be a match.</param>
        /// <param name="cfgCrcMustMatch">If <paramref name="romFormat"/> matches and is <see cref="RomFormat.Bin"/>, and this value is <c>true</c>, the CRC of the CFG file must also match.</param>
        /// <returns><c>true</c> if all of the specified criteria match the corresponding data in <paramref name="programDescription"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="programDescription"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="programIdentifier"/> is invalid.</exception>
        public static bool IsMatchingProgramDescription(this IProgramDescription programDescription, ProgramIdentifier programIdentifier, RomFormat romFormat, bool cfgCrcMustMatch)
        {
            var match = IsMatchingProgramDescription(programDescription, programIdentifier, romFormat, cfgCrcMustMatch, null);
            return match;
        }

        /// <summary>
        /// Determines if a given <see cref="IProgramDescription"/> is considered a match based on given criteria.
        /// </summary>
        /// <param name="programDescription">An instance</param>
        /// <param name="programIdentifier">Provides the unique identifier that must match the value that can be determined from <paramref name="programDescription"/>.</param>
        /// <param name="romFormat">If this value is not <see cref="RomFormat.None"/>, then <paramref name="programDescription"/> must have the same ROM format to be a match.</param>
        /// <param name="cfgCrcMustMatch">If <paramref name="romFormat"/> matches and is <see cref="RomFormat.Bin"/>, and this value is <c>true</c>, the CRC of the CFG file must also match.</param>
        /// <param name="code">If specified (not <c>null</c> or empty), and it can be determined that <paramref name="programDescription"/> has a value, value is used in determining match.</param>
        /// <returns><c>true</c> if all of the specified criteria match the corresponding data in <paramref name="programDescription"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="programDescription"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="programIdentifier"/> is invalid.</exception>
        public static bool IsMatchingProgramDescription(this IProgramDescription programDescription, ProgramIdentifier programIdentifier, RomFormat romFormat, bool cfgCrcMustMatch, string code)
        {
            if (programDescription == null)
            {
                throw new ArgumentNullException("programDescription");
            }
            if (programIdentifier == ProgramIdentifier.Invalid)
            {
                throw new ArgumentException("programIdentifier");
            }

            var crcMatch = programDescription.Crc == programIdentifier.DataCrc;
            var cfgCrcsMatch = !cfgCrcMustMatch;
            var romFormatsMatch = romFormat == RomFormat.None; // don't care
            var codesMatch = string.IsNullOrEmpty(code);
            if (programDescription.Rom != null)
            {
                if (!romFormatsMatch)
                {
                    romFormatsMatch = programDescription.Rom.MatchingRomFormat(romFormat, considerOriginalFormat: true);
                }
                crcMatch = programDescription.Rom.MatchesProgramIdentifier(programIdentifier, cfgCrcMustMatch);
            }

            // This may be nearly worthless -- how many XML ProgramInformation implementations are hooked to ProgramDescriptions -- don't they all end up being UserSpecifiedProgramInformation?
            if (!codesMatch && programDescription.ProgramInformation is IntvFunhouseXmlProgramInformation)
            {
                codesMatch = ((IntvFunhouseXmlProgramInformation)programDescription.ProgramInformation).Code == code;
            }
            var match = crcMatch && cfgCrcsMatch && romFormatsMatch && codesMatch;
            return match;
        }
    }
}

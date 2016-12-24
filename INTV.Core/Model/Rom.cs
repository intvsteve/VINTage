// <copyright file="Rom.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Base class for IRom implementations. Also offers a factory.
    /// </summary>
    public abstract class Rom : IRom
    {
        /// <summary>
        /// The maximum size of a ROM.
        /// </summary>
        public const uint MaxROMSize = 0x100000; // 1 MB

        #region IRom

        /// <inheritdoc />
        public abstract RomFormat Format { get; protected set; }

        /// <inheritdoc />
        public virtual string RomPath { get; protected set; }

        /// <inheritdoc />
        public virtual string ConfigPath { get; protected set; }

        /// <inheritdoc />
        public virtual bool IsValid { get; protected set; }

        /// <inheritdoc />
        public abstract uint Crc { get; }

        /// <inheritdoc />
        public abstract uint CfgCrc { get; }

        #endregion // IRom

        /// <summary>
        /// Creates an instance of IRom, which represents a program ROM, given at least one valid file path.
        /// </summary>
        /// <param name="filePath">The path to the ROM file.</param>
        /// <param name="configFilePath">The path to the config file (as determined necessary depending on ROM format).</param>
        /// <returns>If the file at the given path appears to be a valid program ROM, returns an instance of IRom, otherwise <c>null</c>.</returns>
        public static IRom Create(string filePath, string configFilePath)
        {
            Rom rom = LuigiFormatRom.Create(filePath);
            if (rom == null)
            {
                rom = RomFormatRom.Create(filePath);
            }
            if (rom == null)
            {
                rom = BinFormatRom.Create(filePath, configFilePath);
            }
            return rom;
        }

        /// <summary>
        /// Replaces the configuration file path of a .bin format ROM.
        /// </summary>
        /// <param name="rom">The ROM whose configuration file path is to be updated.</param>
        /// <param name="cfgPath">The new .cfg path.</param>
        /// <remarks>>Only applies to BinFormatRoms -- i.e. ROMs using the original .bin + .cfg format.</remarks>
        public static void ReplaceCfgPath(IRom rom, string cfgPath)
        {
            var binFormatRom = AsSpecificRomType<BinFormatRom>(rom);
            if (binFormatRom != null)
            {
                binFormatRom.ReplaceCfgPath(cfgPath);
            }
        }

        /// <summary>
        /// If the given ROM is a LUIGI format ROM, get the LUIGI header.
        /// </summary>
        /// <param name="rom">The ROM whose LUIGI header is desired.</param>
        /// <returns>The LUIGI header, or <c>null</c> if <paramref name="rom"/> does not refer to a LUIGI format ROM.</returns>
        public static LuigiFileHeader GetLuigiHeader(IRom rom)
        {
            LuigiFileHeader header = null;
            var luigiRom = AsSpecificRomType<LuigiFormatRom>(rom);
            if (luigiRom != null)
            {
                header = luigiRom.Header;
            }
            return header;
        }

        /// <summary>
        /// Gets an enumerable of the bytes to ignore for certain comparison operations.
        /// </summary>
        /// <param name="rom">The ROM whose ignorable data ranges for compare are needed.</param>
        /// <param name="excludeFeatureBits">If <c>true</c>, the result includes the range of bytes to ignore that describe ROM features.</param>
        /// <returns>An enumeration containing ranges of bytes to ignore for the purpose of comparing two LUIGI ROMs. For Version 1 and newer,
        /// this range will always include an entry for the UID portion of the LUIGI, which will allow for the comparison of two LUIGI files
        /// created from different sources (e.g. .rom vs. .bin) to actually compare as equivalent.</returns>
        public static IEnumerable<INTV.Core.Utility.Range<int>> GetComparisonIgnoreRanges(IRom rom, bool excludeFeatureBits)
        {
            var luigiFormatRom = AsSpecificRomType<LuigiFormatRom>(rom);
            if (luigiFormatRom != null)
            {
                foreach (var skipRange in luigiFormatRom.GetComparisonIgnoreRanges(excludeFeatureBits))
                {
                    yield return skipRange;
                }
            }
        }

        /// <summary>
        /// Get a specific implementation of IRom.
        /// </summary>
        /// <typeparam name="T">The specific IRom implementation to get.</typeparam>
        /// <param name="rom">The ROM whose specific implementation is desired.</param>
        /// <returns>The <paramref name="rom"/> as a specific implementation, or <c>null</c> if <paramref name="rom"/> is not the requested implementation.</returns>
        internal static T AsSpecificRomType<T>(IRom rom) where T : class, IRom
        {
            var specificRomType = rom as T;
            if (specificRomType == null)
            {
                var xmlRom = rom as XmlRom;
                if (xmlRom != null)
                {
                    specificRomType = xmlRom.ResolvedRom as T;
                }
            }
            if (specificRomType == null)
            {
                var alternateRom = rom as AlternateRom;
                if (alternateRom != null)
                {
                    specificRomType = alternateRom.Alternate as T;
                }
            }
            return specificRomType;
        }

        #region IRom

        /// <inheritdoc />
        public abstract bool Validate();

        /// <inheritdoc />
        public abstract uint RefreshCrc(out bool changed);

        /// <inheritdoc />
        public abstract uint RefreshCfgCrc(out bool changed);

        #endregion // IRom
    }
}

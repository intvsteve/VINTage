// <copyright file="Rom.cs" company="INTV Funhouse">
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

////#define REPORT_PERFORMANCE

using System;
using System.Collections.Generic;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Base class for IRom implementations. Also offers a factory.
    /// </summary>
    public abstract class Rom : IRom
    {
        private static readonly RomFormatMemo Memos = new RomFormatMemo();

        /// <summary>
        /// The maximum size of a ROM.
        /// </summary>
        public const uint MaxROMSize = 0x100000; // 1 MB

#if REPORT_PERFORMANCE
        /// <summary>
        /// Gets or sets the accumulated time spent in the GetRefreshedCrcs() method when measuring performance.
        /// </summary>
        public static TimeSpan AccumulatedRefreshCrcsTime { get; set; }
#endif // REPORT_PERFORMANCE

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
        /// <param name="configFilePath">The path to the configuration file (as determined necessary depending on ROM format).</param>
        /// <returns>If the file at the given path appears to be a valid program ROM, returns an instance of IRom, otherwise <c>null</c>.</returns>
        public static IRom Create(string filePath, string configFilePath)
        {
            Rom rom = null;
            var format = CheckRomFormat(filePath);
            switch (format)
            {
                case RomFormat.Intellicart:
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    rom = RomFormatRom.Create(filePath);
                    break;
                case RomFormat.Bin:
                    rom = BinFormatRom.Create(filePath, configFilePath);
                    break;
                case RomFormat.Luigi:
                    rom = LuigiFormatRom.Create(filePath);
                    break;
                case RomFormat.None:
                    break;
            }
            return rom;
        }

        /// <summary>
        /// Get up-to-date CRC32 values for a ROM's file and, possibly, it's configuration file.
        /// </summary>
        /// <param name="filePath">Absolute path to the ROM file.</param>
        /// <param name="configFilePath">Absolute path to the configuration file (for .bin format ROMs). May be <c>null</c>.</param>
        /// <param name="cfgCrc">Receives the CRC32 of <paramref name="configFilePath"/> if it is valid.</param>
        /// <returns>CRC32 of <paramref name="filePath"/>.</returns>
        public static uint GetRefreshedCrcs(string filePath, string configFilePath, out uint cfgCrc)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            uint romCrc = 0;
            cfgCrc = 0;
            switch (CheckRomFormat(filePath))
            {
                case RomFormat.Bin:
                    romCrc = BinFormatRom.GetCrcs(filePath, configFilePath, out cfgCrc);
                    break;
                case RomFormat.Intellicart:
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    romCrc = RomFormatRom.GetCrcs(filePath, configFilePath, out cfgCrc);
                    break;
                case RomFormat.Luigi:
                    romCrc = LuigiFormatRom.GetCrcs(filePath, configFilePath, out cfgCrc);
                    break;
                case RomFormat.None:
                    break;
            }
#if REPORT_PERFORMANCE
            stopwatch.Stop();
            AccumulatedRefreshCrcsTime += stopwatch.Elapsed;
#endif // REPORT_PERFORMANCE
            return romCrc;
        }

        /// <summary>
        /// Efficiently check to see if the file at the given path is of a known ROM type.
        /// </summary>
        /// <param name="filePath">Absolute path of the file to check.</param>
        /// <returns>Format of the ROM, <c>RomFormat.None</c> if the format cannot be determined.</returns>
        public static RomFormat CheckRomFormat(string filePath)
        {
            var format = RomFormat.None;
            Memos.CheckAddMemo(filePath, null, out format);
            return format;
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

        /// <summary>
        /// Inspects data in the stream to determine if it appears to be a ROM.
        /// </summary>
        /// <param name="stream">The stream containing the data to inspect.</param>
        /// <returns>A <see cref="RomFormat"/> value; a value of <c>RomFormat.None</c> indicates the stream is almost certainly not
        /// a known ROM format. It is possible to get false positive matches resulting in <c>RomFormat.Bin</c> as it is not a
        /// strongly structured format that can be identified with high confidence. It is incumbent upon the caller to do a modicum
        /// of checking, e.g. file name extension checks, or later exception handling, to deal with errors.</returns>
        public static RomFormat GetFormat(System.IO.Stream stream)
        {
            var format = LuigiFormatRom.CheckFormat(stream);
            if (format == RomFormat.None)
            {
                format = RomFormatRom.CheckFormat(stream);
            }
            if (format == RomFormat.None)
            {
                format = BinFormatRom.CheckFormat(stream);
            }
            return format;
        }

        /// <summary>
        /// Check ROM format efficiently using the memo system.
        /// </summary>
        /// <param name="romPath">Absolute path of the ROM whose format is desired.</param>
        /// <returns>ROM format.</returns>
        protected static RomFormat CheckMemo(string romPath)
        {
            RomFormat format;
            Memos.CheckMemo(romPath, out format);
            return format;
        }

        /// <summary>
        /// Add a memo for a ROM.
        /// </summary>
        /// <param name="romPath">Absolute path of the ROM whose memo is to be added.</param>
        /// <param name="format">ROM format.</param>
        protected static void AddMemo(string romPath, RomFormat format)
        {
            Memos.AddMemo(romPath, format);
        }

        private class RomFormatMemo : FileMemo<RomFormat>
        {
            public RomFormatMemo()
                : base(IStorageAccessHelpers.DefaultStorage)
            {
            }

            /// <inheritdoc />
            protected override RomFormat DefaultMemoValue
            {
                get { return RomFormat.None; }
            }

            /// <inheritdoc />
            protected override RomFormat GetMemo(string filePath, object data)
            {
                var format = LuigiFormatRom.CheckFormat(filePath);
                if (format == RomFormat.None)
                {
                    format = RomFormatRom.CheckFormat(filePath);
                }
                if (format == RomFormat.None)
                {
                    format = BinFormatRom.CheckFormat(filePath);
                }
                return format;
            }

            /// <inheritdoc />
            protected override bool IsValidMemo(RomFormat memo)
            {
                return memo != RomFormat.None;
            }
        }
    }
}

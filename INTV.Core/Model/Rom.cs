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
        /// <summary>
        /// The maximum size of a ROM.
        /// </summary>
        public const uint MaxROMSize = 0x100000; // 1 MB

        private static readonly IRom InvalidRomInstance = new CanonicallyInvalidRom();

        private static readonly RomFormatMemo Memos = new RomFormatMemo();

#if REPORT_PERFORMANCE
        /// <summary>
        /// Gets or sets the accumulated time spent in the GetRefreshedCrcs() method when measuring performance.
        /// </summary>
        public static TimeSpan AccumulatedRefreshCrcsTime { get; set; }
#endif // REPORT_PERFORMANCE

        #region IRom

        /// <summary>
        /// Gets the canonical invalid rom.
        /// </summary>
        public static IRom InvalidRom
        {
            get { return InvalidRomInstance; }
        }

        /// <inheritdoc />
        public abstract RomFormat Format { get; protected set; }

        /// <inheritdoc />
        public virtual StorageLocation RomPath { get; protected set; }

        /// <inheritdoc />
        public virtual StorageLocation ConfigPath { get; protected set; }

        /// <inheritdoc />
        public virtual bool IsValid { get; protected set; }

        /// <inheritdoc />
        public abstract uint Crc { get; }

        /// <inheritdoc />
        public abstract uint CfgCrc { get; }

        #endregion // IRom

        /// <summary>
        /// Creates an instance of IRom, which represents a program ROM, given at least one valid location.
        /// </summary>
        /// <param name="romLocation">The location of the ROM image.</param>
        /// <param name="configLocation">The location of the configuration data (as determined necessary depending on ROM format).</param>
        /// <returns>If the image at the given location appears to be a valid program ROM, returns an instance of IRom, otherwise <c>null</c>.</returns>
        public static IRom Create(StorageLocation romLocation, StorageLocation configLocation)
        {
            Rom rom = null;
            var format = CheckRomFormat(romLocation);
            switch (format)
            {
                case RomFormat.Intellicart:
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    rom = RomFormatRom.Create(romLocation);
                    break;
                case RomFormat.Bin:
                    rom = BinFormatRom.Create(romLocation, configLocation);
                    break;
                case RomFormat.Luigi:
                    rom = LuigiFormatRom.Create(romLocation);
                    break;
                case RomFormat.None:
                    break;
            }
            return rom;
        }

        /// <summary>
        /// Get up-to-date CRC32 values for a ROM's image and, possibly, its configuration data.
        /// </summary>
        /// <param name="romLocation">Location of the ROM image.</param>
        /// <param name="configLocation">Location of the configuration data (for .bin format ROMs). May be <c>StorageLocation.InvalidLocation</c>.</param>
        /// <param name="cfgCrc">Receives the CRC32 of <paramref name="configLocation"/> if it is valid.</param>
        /// <returns>CRC32 of <paramref name="romLocation"/>.</returns>
        public static uint GetRefreshedCrcs(StorageLocation romLocation, StorageLocation configLocation, out uint cfgCrc)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            uint romCrc = 0;
            cfgCrc = 0;
            switch (CheckRomFormat(romLocation))
            {
                case RomFormat.Bin:
                    romCrc = BinFormatRom.GetCrcs(romLocation, configLocation, out cfgCrc);
                    break;
                case RomFormat.Intellicart:
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    romCrc = RomFormatRom.GetCrcs(romLocation, configLocation, out cfgCrc);
                    break;
                case RomFormat.Luigi:
                    romCrc = LuigiFormatRom.GetCrcs(romLocation, configLocation, out cfgCrc);
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
        /// Efficiently check to see if the image at the given location is of a known ROM type.
        /// </summary>
        /// <param name="location">Location of the image to check.</param>
        /// <returns>Format of the ROM, <c>RomFormat.None</c> if the format cannot be determined.</returns>
        public static RomFormat CheckRomFormat(StorageLocation location)
        {
            var format = RomFormat.None;
            Memos.CheckAddMemo(location, null, out format);
            return format;
        }

        /// <summary>
        /// Replaces the configuration location of a .bin format ROM.
        /// </summary>
        /// <param name="rom">The ROM whose configuration location is to be updated.</param>
        /// <param name="cfgLocation">The new .cfg location.</param>
        /// <remarks>>Only applies to BinFormatRoms -- i.e. ROMs using the original .bin + .cfg format.</remarks>
        public static void ReplaceCfgPath(IRom rom, StorageLocation cfgLocation)
        {
            var binFormatRom = AsSpecificRomType<BinFormatRom>(rom);
            if (binFormatRom != null)
            {
                binFormatRom.ReplaceCfgPath(cfgLocation);
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
        /// this range will always include an entry for the UID portion of the LUIGI, which will allow for the comparison of two LUIGI images
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
        /// of checking, e.g. location name extension checks, or later exception handling, to deal with errors.</returns>
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
        /// <param name="location">Location of the ROM whose format is desired.</param>
        /// <returns>ROM format.</returns>
        protected static RomFormat CheckMemo(StorageLocation location)
        {
            RomFormat format;
            Memos.CheckMemo(location, out format);
            return format;
        }

        /// <summary>
        /// Add a memo for a ROM.
        /// </summary>
        /// <param name="romPath">Absolute path of the ROM whose memo is to be added.</param>
        /// <param name="format">ROM format.</param>
        protected static void AddMemo(StorageLocation romPath, RomFormat format)
        {
            Memos.AddMemo(romPath, format);
        }

        private class RomFormatMemo : FileMemo<RomFormat>
        {
            public RomFormatMemo()
                : base()
            {
            }

            /// <inheritdoc />
            protected override RomFormat DefaultMemoValue
            {
                get { return RomFormat.None; }
            }

            /// <inheritdoc />
            protected override RomFormat GetMemo(StorageLocation location, object data)
            {
                var format = LuigiFormatRom.CheckFormat(location);
                if (format == RomFormat.None)
                {
                    format = RomFormatRom.CheckFormat(location);
                }
                if (format == RomFormat.None)
                {
                    format = BinFormatRom.CheckFormat(location);
                }
                return format;
            }

            /// <inheritdoc />
            protected override bool IsValidMemo(RomFormat memo)
            {
                return memo != RomFormat.None;
            }
        }

        /// <summary>
        /// Used to indicate an invalid or not-a-ROM.
        /// </summary>
        private class CanonicallyInvalidRom : IRom
        {
            /// <inheritdoc />
            public RomFormat Format
            {
                get { return RomFormat.None; }
            }

            /// <inheritdoc />
            public StorageLocation RomPath
            {
                get { return StorageLocation.InvalidLocation; }
            }

            /// <inheritdoc />
            public StorageLocation ConfigPath
            {
                get { return StorageLocation.InvalidLocation; }
            }

            /// <inheritdoc />
            public bool IsValid
            {
                get { return false; }
            }

            /// <inheritdoc />
            public uint Crc
            {
                get { return 0; }
            }

            /// <inheritdoc />
            public uint CfgCrc
            {
                get { return 0; }
            }

            /// <inheritdoc />
            public bool Validate()
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc />
            public uint RefreshCrc(out bool changed)
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc />
            public uint RefreshCfgCrc(out bool changed)
            {
                throw new InvalidOperationException();
            }
        }
    }
}

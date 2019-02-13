// <copyright file="LuigiFormatRom.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Implementation of Rom for programs in the .luigi format.
    /// </summary>
    internal class LuigiFormatRom : Rom
    {
        private static readonly LuigiUniqueIdMemo UniqueIdMemos = new LuigiUniqueIdMemo();

        #region Constructors

        private LuigiFormatRom()
        {
        }

        #endregion // Constructors

        #region Properties

        #region IRom

        /// <inheritdoc />
        public override RomFormat Format
        {
            get;
            protected set;
        }

        /// <inheritdoc />
        public override uint Crc
        {
            get
            {
                if (_crc == 0)
                {
                    bool changed;
                    _crc = RefreshCrc(out changed);
                }
                return _crc;
            }
        }
        private uint _crc;

        /// <inheritdoc />
        public override uint CfgCrc
        {
            get
            {
                if (_cfgCrc == 0)
                {
                    bool changed;
                    _cfgCrc = RefreshCfgCrc(out changed);
                }
                return _cfgCrc;
            }
        }
        private uint _cfgCrc;

        /// <inheritdoc />
        public override bool Validate()
        {
            IsValid = !string.IsNullOrEmpty(RomPath);
            if (IsValid)
            {
                IsValid = StreamUtilities.FileExists(RomPath);
            }
            return IsValid;
        }

        /// <inheritdoc />
        /// <remarks>Not much to recompute here, as we only have the values from the  header.</remarks>
        public override uint RefreshCrc(out bool changed)
        {
            var crc = _crc;
            if (IsValid)
            {
                uint dontCare;
                bool usedLuigiFileForCrc;
                _crc = GetCrcs(Header, RomPath, null, out dontCare, out usedLuigiFileForCrc);
                if (usedLuigiFileForCrc)
                {
                    crc = _crc; // lazy initialization means on first read, we should never get a change
                }
            }
            changed = crc != Crc;
            return _crc;
        }

        /// <inheritdoc />
        public override uint RefreshCfgCrc(out bool changed)
        {
            var cfgCrc = _cfgCrc;
            bool dontCare;
            GetCrcs(Header, RomPath, null, out _cfgCrc, out dontCare);
            changed = cfgCrc != _cfgCrc;
            return _cfgCrc;
        }

        #endregion // IRom

        /// <summary>
        /// Gets the 24-bit CRC of the file.
        /// </summary>
        public uint Crc24
        {
            get
            {
                if (IsValid)
                {
                    if (_crc24 == 0)
                    {
                        if (StreamUtilities.FileExists(RomPath))
                        {
                            _crc24 = INTV.Core.Utility.Crc24.OfFile(RomPath);
                        }
                    }
                }
                return _crc24;
            }
        }
        private uint _crc24;

        /// <summary>
        /// Gets the LUIGI header.
        /// </summary>
        public LuigiFileHeader Header { get; private set; }

        /// <summary>
        /// Gets the target device unique identifier of the LUIGI ROM.
        /// </summary>
        /// <remarks>This value can be used to determine if the LUIGI is encoded to run on a specific LTO Flash! cartridge.
        /// If the value is a null or empty string, the ROM is not encoded in any way.
        /// If the value is LuigiScrambleKeyBlock.AnyLTOFlashId, the ROM is encoded to run on any LTO Flash! cartridge.
        /// If the value is any other value, then the ROM will only load and execute on the LTO Flash! cartridge
        /// having the same unique ID.</remarks>
        public string TargetDeviceUniqueId
        {
            get
            {
                var targetUniqueId = string.Empty;
                UniqueIdMemos.CheckAddMemo(RomPath, this, out targetUniqueId);
                return targetUniqueId;
            }
        }

        #endregion // Properties

        /// <summary>
        /// Examines the given file and attempts to determine if it is a program in .luigi format.
        /// </summary>
        /// <param name="filePath">The path to the LUIGI file.</param>
        /// <returns>A valid LuigiFormatRom if file is a valid .luigi file, otherwise <c>null</c>.</returns>
        internal static LuigiFormatRom Create(string filePath)
        {
            LuigiFormatRom rom = null;
            if (CheckFormat(filePath) == RomFormat.Luigi)
            {
                LuigiFileHeader header = LuigiFileHeader.GetHeader(filePath);
                if (header != null)
                {
                    rom = new LuigiFormatRom() { Format = RomFormat.Luigi, IsValid = true, RomPath = filePath, Header = header };
                }
            }
            return rom;
        }

        /// <summary>
        /// Checks the format of the ROM at the given absolute path.
        /// </summary>
        /// <param name="filePath">Absolute path to a ROM file.</param>
        /// <returns>The format of the ROM.</returns>
        internal static RomFormat CheckFormat(string filePath)
        {
            var format = CheckMemo(filePath);
            if (format == RomFormat.None)
            {
                using (var file = StreamUtilities.OpenFileStream(filePath))
                {
                    if (file != null)
                    {
                        if (file.Length > 0)
                        {
                            if (LuigiFileHeader.GetHeader(filePath) != null)
                            {
                                format = RomFormat.Luigi;
                                AddMemo(filePath, format);
                            }
                        }
                    }
                }
            }
            return format;
        }

        /// <summary>
        /// Get up-to-date CRC32 values for the ROM.
        /// </summary>
        /// <param name="romPath">Absolute path to the ROM.</param>
        /// <param name="cfgPath">Absolute path to the configuration file (may be <c>null</c>).</param>
        /// <param name="cfgCrc">Receives the CRC32 of the file at <paramref name="cfgPath"/></param>
        /// <returns>The CRC32 of the file at <paramref name="romPath"/>.</returns>
        internal static uint GetCrcs(string romPath, string cfgPath, out uint cfgCrc)
        {
            bool dontCare;
            var romCrc = GetCrcs(LuigiFileHeader.GetHeader(romPath), romPath, cfgPath, out cfgCrc, out dontCare);
            return romCrc;
        }

        private static uint GetCrcs(LuigiFileHeader header, string romPath, string cfgPath, out uint cfgCrc, out bool usedLuigiFileCrc)
        {
            usedLuigiFileCrc = false;
            uint romCrc = 0;
            cfgCrc = 0;
            if (header != null)
            {
                if (header.Version > 0)
                {
                    if ((header.OriginalRomFormat == RomFormat.Bin) || (header.OriginalRomFormat == RomFormat.Rom))
                    {
                        romCrc = header.OriginalRomCrc32; // Report the original file's CRC to help identify the ROM.
                    }
                    if (header.OriginalRomFormat == RomFormat.Bin)
                    {
                        cfgCrc = header.OriginalCfgCrc32;
                    }
                }
            }
            if (romCrc == 0 && StreamUtilities.FileExists(romPath))
            {
                usedLuigiFileCrc = true;
                romCrc = INTV.Core.Utility.Crc32.OfFile(romPath);
            }
            return romCrc;
        }

        /// <summary>
        /// Gets an enumerable of the bytes to ignore for certain comparison operations.
        /// </summary>
        /// <param name="excludeFeatureBits">If <c>true</c>, the result includes the range of bytes to ignore that describe ROM features.</param>
        /// <returns>An enumeration containing ranges of bytes to ignore for the purpose of comparing two LUIGI ROMs. For Version 1 and newer,
        /// this range will always include an entry for the UID portion of the LUIGI, which will allow for the comparison of two LUIGI files
        /// created from different sources (e.g. .rom vs. .bin) to actually compare as equivalent.</returns>
        public IEnumerable<Range<int>> GetComparisonIgnoreRanges(bool excludeFeatureBits)
        {
            if (excludeFeatureBits)
            {
                var rangeStart = LuigiFileHeader.FeatureBytesOffset;
                var rangeStop = rangeStart + LuigiFileHeader.BaseVersionFlagsSize;
                if (Header.Version > 0)
                {
                    rangeStop += LuigiFileHeader.VersionOneAdditionalFeaturesSize;
                }
                yield return new Range<int>(rangeStart, rangeStop - 1);
            }
            if (Header.Version > 0)
            {
                var rangeStart = LuigiFileHeader.UidByteOffset;
                var rangeStop = LuigiFileHeader.UidByteOffset + LuigiFileHeader.VersionOneUidSize;
                yield return new Range<int>(rangeStart, rangeStop - 1); // exclude UID

                rangeStart = rangeStop + LuigiFileHeader.ReservedHeaderBytesSize;
                rangeStop = rangeStart + LuigiFileHeader.HeaderChecksumSize;
                yield return new Range<int>(rangeStart, rangeStop - 1); // exclude CRC8 (it encompasses the UID)
            }
        }

        /// <summary>
        /// Locates the first data block of the requested type in the ROM.
        /// </summary>
        /// <typeparam name="T">The type of LUIGI block to locate.</typeparam>
        /// <returns>The data block, or <c>null</c> if no block of the requested type is in the ROM.</returns>
        internal T LocateDataBlock<T>() where T : LuigiDataBlock
        {
            var dataBlock = default(T);
            if (StreamUtilities.FileExists(RomPath))
            {
                using (var file = StreamUtilities.OpenFileStream(RomPath))
                {
                    if (file != null)
                    {
                        if (file.Length > 0)
                        {
                            var desiredBlockType = LuigiDataBlock.GetBlockType<T>();
                            var luigiHeader = LuigiFileHeader.Inflate(file);
                            var bytesRead = luigiHeader.DeserializeByteCount;

                            // Start looking for desired block immediately after header.
                            var block = LuigiDataBlock.Inflate(file);
                            bytesRead += block.DeserializeByteCount;
                            if (StopIfScrambleKeyBlockFound(desiredBlockType, block.Type))
                            {
                                // Stop looking. If we hit the scramble key, there's nothing more to be looked at.
                                bytesRead = (int)file.Length;
                            }
                            while ((bytesRead < file.Length) && (block.Type != desiredBlockType) && (block.Type != LuigiDataBlockType.EndOfFile))
                            {
                                block = LuigiDataBlock.Inflate(file);
                                bytesRead += block.DeserializeByteCount;
                                if (StopIfScrambleKeyBlockFound(desiredBlockType, block.Type))
                                {
                                    break;
                                }
                            }
                            if (block.Type == desiredBlockType)
                            {
                                dataBlock = block as T;
                            }
                        }
                    }
                }
            }
            return dataBlock;
        }

        private bool StopIfScrambleKeyBlockFound(LuigiDataBlockType desiredBlockType, LuigiDataBlockType currentBlockType)
        {
            var stop = (currentBlockType == LuigiDataBlockType.SetScrambleKey) && (desiredBlockType != LuigiDataBlockType.SetScrambleKey);
            return stop;
        }

        private class LuigiUniqueIdMemo : FileMemo<string>
        {
            public LuigiUniqueIdMemo()
                : base(StreamUtilities.DefaultStorage)
            {
            }

            /// <inheritdoc />
            protected override string DefaultMemoValue
            {
                get { return string.Empty; }
            }

            /// <inheritdoc />
            protected override string GetMemo(string filePath, object data)
            {
                var uniqueIdMemo = string.Empty;
                var rom = (LuigiFormatRom)data;
                var scrambleBlock = rom.LocateDataBlock<LuigiScrambleKeyBlock>();
                if (scrambleBlock != null)
                {
                    uniqueIdMemo = scrambleBlock.UniqueId;
                }
                return uniqueIdMemo;
            }

            /// <inheritdoc />
            /// <remarks>We also cache empty / null here because the unique ID in a LUIGI is not going to change unless the file itself is rebuilt.</remarks>
            protected override bool IsValidMemo(string memo)
            {
                return true;
            }
        }
    }
}

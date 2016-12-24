// <copyright file="LuigiFormatRom.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Implementation of Rom for programs in the .luigi format.
    /// </summary>
    internal class LuigiFormatRom : Rom
    {
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
            IsValid = !string.IsNullOrEmpty(RomPath) && RomPath.FileExists();
            return IsValid;
        }

        /// <inheritdoc />
        /// <remarks>Not much to recompute here, as we only have the values from the  header.</remarks>
        public override uint RefreshCrc(out bool changed)
        {
            var crc = _crc;
            if (IsValid)
            {
                if (Header.Version > 0)
                {
                    if ((Header.OriginalRomFormat == RomFormat.Bin) || (Header.OriginalRomFormat == RomFormat.Rom))
                    {
                        _crc = Header.OriginalRomCrc32; // Report the original file's CRC to help identify the ROM.
                    }
                }
                if (_crc == 0 && RomPath.FileExists())
                {
                    _crc = INTV.Core.Utility.Crc32.OfFile(RomPath);
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
            if (IsValid && (Header.Version > 0) && (Header.OriginalRomFormat == RomFormat.Bin))
            {
                _cfgCrc = Header.OriginalCfgCrc32;
            }
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
                if (IsValid && (_crc24 == 0) && RomPath.FileExists())
                {
                    _crc24 = INTV.Core.Utility.Crc24.OfFile(RomPath);
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
                var scrambleBlock = LocateDataBlock<LuigiScrambleKeyBlock>();
                if (scrambleBlock != null)
                {
                    targetUniqueId = scrambleBlock.UniqueId;
                }
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
            using (var file = filePath.OpenFileStream())
            {
                if ((file != null) && (file.Length > 0))
                {
                    LuigiFileHeader luigiHeader = null;
                    try
                    {
                        if (LuigiFileHeader.PotentialLuigiFile(filePath))
                        {
                            luigiHeader = LuigiFileHeader.Inflate(file);
                            rom = new LuigiFormatRom() { Format = RomFormat.Luigi, IsValid = true, RomPath = filePath, Header = luigiHeader };
                        }
                    }
                    catch (INTV.Core.UnexpectedFileTypeException)
                    {
                    }
                }
            }
            return rom;
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
            if (Header.Version > LuigiFileHeader.CurrentVersion)
            {
                throw new System.InvalidOperationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UnsupportedLuigiVersion_Format, Header.Version));
            }
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
            if (RomPath.FileExists())
            {
                using (var file = RomPath.OpenFileStream())
                {
                    if ((file != null) && (file.Length > 0))
                    {
                        try
                        {
                            var blockType = LuigiDataBlock.GetBlockType<T>();
                            var luigiHeader = LuigiFileHeader.Inflate(file);
                            var bytesRead = luigiHeader.DeserializeByteCount;

                            // Start looking for desired block immediately after header.
                            var block = LuigiDataBlock.Inflate(file);
                            bytesRead += block.DeserializeByteCount;
                            while ((bytesRead < file.Length) && (block.Type != blockType) && (block.Type != LuigiDataBlockType.EndOfFile))
                            {
                                block = LuigiDataBlock.Inflate(file);
                                bytesRead += block.DeserializeByteCount;
                            }
                            if (block.Type == blockType)
                            {
                                dataBlock = block as T;
                            }
                        }
                        catch (INTV.Core.UnexpectedFileTypeException)
                        {
                            // Would be odd if we got this by this point, but perhaps it's a corrupted LUIGI file.
                        }
                    }
                }
            }
            return dataBlock;
        }
    }
}

// <copyright file="RomFormatRom.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

////#define IGNORE_METADATA_FOR_CRC

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Implementation of Rom for programs in the .rom (or compatible) format.
    /// </summary>
    internal class RomFormatRom : Rom
    {
        private const int AutobaudByteSize = 1;
        private const int RomSegmentCountSize = 1;
        private const int RomSegmentCheckByteSize = 1;

        private const int RomSegmentStartAddressHiByteSize = 1;
        private const int RomSegmentStopAddressLoByteSize = 1;

        private const int AccessTableSize = 16;
        private const int FineAddressRestrictionTableSize = 32;
        private const int EnableTablesSize = AccessTableSize + FineAddressRestrictionTableSize;

        private const int Crc16Size = 2;

        private const int HeaderSize = AutobaudByteSize + RomSegmentCountSize + RomSegmentCheckByteSize + EnableTablesSize + Crc16Size;
        
        private const int MinimumSegmentWordCount = 0x100;

        private const int RomSegmentCountOffset = 1;
        private const int InitialRomSegmentOffset = 3;

        private static readonly Dictionary<RomFormat, byte> AutoBaudBytes = new Dictionary<RomFormat, byte>()
        {
            { RomFormat.Intellicart, 0xA8 },
            { RomFormat.CuttleCart3, 0x41 },
            { RomFormat.CuttleCart3Advanced, 0x61 }
        };

        #region Constructors

        private RomFormatRom()
        {
        }

        #endregion // Constructors

        #region Properties

        #region IRom Properties

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
                if (IsValid && (_crc == 0))
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
            get { return 0; }
        }

        #endregion // IRom Properties

        /// <summary>
        /// Gets the metadata from ID tags that may be appended to the end of the .ROM.
        /// </summary>
        public IEnumerable<RomMetadataBlock> Metadata
        {
            get
            {
                var metadata = new List<RomMetadataBlock>();
                try
                {
                    if (IsValid && RomPath.FileExists())
                    {
                        using (var file = RomPath.OpenFileStream())
                        {
                            var offsetIntoFile = GetMetadataOffset(file);
                            while ((offsetIntoFile < file.Length) && (file.Position < file.Length))
                            {
                                var metadataBlock = RomMetadataBlock.Inflate(file);
                                if (metadataBlock == null)
                                {
                                    break;
                                }
                                else
                                {
                                    metadata.Add(metadataBlock);
                                }
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    // Don't bring down the app if parsing for metadata fails.
                    System.Diagnostics.Debug.WriteLine("Getting ROM metadata failed: " + e);
#if DEBUG
                    throw;
#endif // DEBUG
                }
                return metadata;
            }
        }

        #endregion // Properties

        #region IRom

        /// <inheritdoc />
        public override bool Validate()
        {
            IsValid = !string.IsNullOrEmpty(RomPath) && RomPath.FileExists();
            return IsValid;
        }

        /// <inheritdoc />
        public override uint RefreshCrc(out bool changed)
        {
            var crc = _crc;
            if (IsValid && RomPath.FileExists())
            {
                uint dontCare;
                _crc = GetCrcs(Format, RomPath, null, out dontCare);

                if (crc == 0)
                {
                    crc = _crc; // lazy initialization means on first read, we should never get a change
                }
            }
            changed = crc != _crc;
            return _crc;
        }

        /// <inheritdoc />
        public override uint RefreshCfgCrc(out bool changed)
        {
            changed = false;
            return 0;
        }

        #endregion // IRom

        /// <summary>
        /// Examines the given file and attempts to determine if it is a program in .rom format.
        /// </summary>
        /// <param name="filePath">The path to the ROM file.</param>
        /// <returns>A valid RomFormatRom if file is a valid .rom (or compatible) file, otherwise <c>null</c>.</returns>
        internal static RomFormatRom Create(string filePath)
        {
            RomFormatRom rom = null;
            var format = CheckFormat(filePath);
            if (format != RomFormat.None)
            {
                // Valid header, so create the instance. Full validation would require walking
                // all the segments -- essentially most of the ROM.
                rom = new RomFormatRom() { Format = format, IsValid = true, RomPath = filePath };
            }
            return rom;
        }

        /// <summary>
        /// Given an absolute path to a ROM file, attempt to determine the format of the ROM.
        /// </summary>
        /// <param name="filePath">Absolute path to the potential ROM file.</param>
        /// <returns>The format of the file. If it does not appear to be a ROM, then <c>RomFormat.None</c> is returned.</returns>
        internal static RomFormat CheckFormat(string filePath)
        {
            var format = CheckMemo(filePath);
            if (format == RomFormat.None)
            {
                using (var file = filePath.OpenFileStream())
                {
                    if ((file != null) && (file.Length > 0))
                    {
                        byte[] data = new byte[HeaderSize];
                        var numBytesRead = file.Read(data, 0, HeaderSize);
                        if (numBytesRead == HeaderSize)
                        {
                            // Check the header (checks for both .rom and .cc3/.cc3 advanced formats)
                            format = AutoBaudBytes.FirstOrDefault(a => a.Value == data[0]).Key;
                            if (format != RomFormat.None)
                            {
                                // If header appears to be valid, assume the entire ROM is valid. Full validation would require walking
                                // all the segments -- essentially most of the ROM.
                                if ((data[1] ^ data[2]) != 0xFF)
                                {
                                    format = RomFormat.None;
                                }
                            }
                        }
                    }
                }
            }
            return format;
        }

        /// <summary>
        /// Get the Crc32 values of a ROM.
        /// </summary>
        /// <param name="romPath">Absolute path of the ROM whose CRC32 value is desired.</param>
        /// <param name="cfgPath">Absolute path of the configuration (.cfg) file whose CRC32 is desired. May be <c>null</c>, depending on the ROM.</param>
        /// <param name="cfgCrc">Receives the CRC32 of the configuration file, if applicable. Could be zero.</param>
        /// <returns>CRC32 of the ROM file.</returns>
        /// <remarks>Instead of zero, should the Crc32.InitialValue be used as a sentinel 'invalid' value?</remarks>
        internal static uint GetCrcs(string romPath, string cfgPath, out uint cfgCrc)
        {
            uint romCrc = GetCrcs(CheckFormat(romPath), romPath, cfgPath, out cfgCrc);
            return romCrc;
        }

        private static uint GetCrcs(RomFormat format, string romPath, string cfgPath, out uint cfgCrc)
        {
            cfgCrc = 0;
            uint romCrc = 0;

            if (romPath.FileExists())
            {
                byte replacementByte = AutoBaudBytes[format];

#if IGNORE_METADATA_FOR_CRC
                var metadataRange = new List<Range<int>>();
                var metadataOffset = GetMetadataOffset();
                metadataRange.Add(new Range<int>(metadataOffset, int.MaxValue));
                romCrc = Crc32.OfFile(romPath, format != RomFormat.Intellicart, replacementByte, metadataRange);
#else
                romCrc = Crc32.OfFile(romPath, format != RomFormat.Intellicart, replacementByte);
#endif // IGNORE_METADATA_FOR_CRC
            }
            return romCrc;
        }

        private int GetMetadataOffset(System.IO.Stream file)
        {
            var metadataOffset = 0;
            if ((file != null) && (file.Length > 0))
            {
                file.Seek(RomSegmentCountOffset, System.IO.SeekOrigin.Begin);
                var romSegmentsCount = file.ReadByte();

                // Seek past the ROM segments.
                file.Seek(InitialRomSegmentOffset, System.IO.SeekOrigin.Begin); // Seek to beginning of first segment
                for (var i = 0; i < romSegmentsCount; ++i)
                {
                    int segmentLow = (int)file.ReadByte() << 8; // Cast up to int for validity checking; each segment is on a 256 word boundary (in reality, a 2K word boundary)
                    int segmentHigh = ((int)file.ReadByte() << 8) + MinimumSegmentWordCount; // Each segment must be at least 256 words
                    if (segmentHigh < segmentLow)
                    {
                        throw new System.InvalidOperationException(Resources.Strings.RomFormatRom_InvalidFormat);
                    }
                    var segmentDataSize = (2 * (segmentHigh - segmentLow)) + Crc16Size; // segment size is in words, not bytes
                    file.Seek(segmentDataSize, System.IO.SeekOrigin.Current); // skip past data and the CRC
                }

                // Seek past the enable tables and the 16-bit CRC.
                file.Seek(AccessTableSize + FineAddressRestrictionTableSize + Crc16Size, System.IO.SeekOrigin.Current);
                metadataOffset = (int)file.Position;

                // Anything after this will be metadata...
            }
            return metadataOffset;
        }
    }
}

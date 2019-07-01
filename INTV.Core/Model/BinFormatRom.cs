// <copyright file="BinFormatRom.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Implementation of Rom for files in the .bin format.
    /// </summary>
    internal class BinFormatRom : Rom
    {
        private const int MinRomSize = 128; // Was 8192... Some test-related and Hello World type ROMs are pretty small.

        #region Constructors

        private BinFormatRom()
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

        #endregion // IRom Properties

        /// <summary>
        /// Gets the metadata from CFGVAR values that may be included in the .cfg data.
        /// </summary>
        /// <remarks>NOTE: Parsing here is EXTREMELY primitive. The code does not enforce the notion of
        /// the 'vars' section, it just blindly scans for values with 'keys' listed in as1600.txt.
        /// NOTE: The metadata is not cached! This means it will be parsed when requested.</remarks>
        public IEnumerable<CfgVarMetadataBlock> Metadata
        {
            get
            {
                var metadata = Enumerable.Empty<CfgVarMetadataBlock>();
                try
                {
                    if (IsValid)
                    {
                        if (ConfigPath != null)
                        {
                            if (ConfigPath.Exists())
                            {
                                if (MetadataCacheEnabled && (_metadata != null))
                                {
                                    return _metadata;
                                }
                                using (var file = ConfigPath.OpenStream())
                                {
                                    metadata = CfgVarMetadataBlock.InflateAll(file);
                                }
                                if (MetadataCacheEnabled && (_metadata == null))
                                {
                                    _metadata = metadata;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    // Don't bring down the app if parsing for metadata fails.
                    System.Diagnostics.Debug.WriteLine("Getting CFGVAR metadata failed: " + e);
#if DEBUG
                    throw;
#endif // DEBUG
                }
                return metadata;
            }
        }
        private IEnumerable<CfgVarMetadataBlock> _metadata;

        /// <summary>
        /// Gets or sets a value indicating whether to cache metadata. If set to <c>true</c>, metadata will be cached rather than re-parsed.
        /// </summary>
        internal bool MetadataCacheEnabled { get; set; }

        #endregion // Properties

        #region IRom

        /// <inheritdoc />
        public override bool Validate()
        {
            var isValid = RomPath.IsValid;
            if (isValid)
            {
                isValid = RomPath.Exists();
            }
            if (isValid)
            {
                if (ConfigPath.IsValid)
                {
                    isValid = ConfigPath.Exists();
                }
            }
            IsValid = isValid;
            return IsValid;
        }

        /// <inheritdoc />
        public override uint RefreshCrc(out bool changed)
        {
            var crc = _crc;
            if (IsValid)
            {
                if (RomPath.Exists())
                {
                    uint dontCare;
                    _crc = GetCrcs(RomPath, StorageLocation.InvalidLocation, out dontCare);
                    if (crc == 0)
                    {
                        crc = _crc; // lazy initialization means on first read, we should never get a change
                    }
                }
            }
            changed = crc != _crc;
            return _crc;
        }

        /// <inheritdoc />
        public override uint RefreshCfgCrc(out bool changed)
        {
            var cfgCrc = _cfgCrc;
            if (IsValid)
            {
                if (!string.IsNullOrEmpty(ConfigPath.Path))
                {
                    if (ConfigPath.Exists())
                    {
                        GetCrcs(StorageLocation.InvalidLocation, ConfigPath, out _cfgCrc);
                        if (cfgCrc == 0)
                        {
                            cfgCrc = _cfgCrc; // lazy initialization means on first read, we should never get a change
                        }
                    }
                }
            }
            changed = cfgCrc != _cfgCrc;
            return _cfgCrc;
        }

        #endregion // IRom

        /// <summary>
        /// Replace the configuration in the ROM with the one provided.
        /// </summary>
        /// <param name="cfgLocation">The new configuration file path.</param>
        public void ReplaceCfgPath(StorageLocation cfgLocation)
        {
            ConfigPath = cfgLocation;
            _cfgCrc = 0; // force recalculation CfgCrc next time it's requested
        }

        /// <summary>
        /// Examines the given data stream and attempts to determine if it might be a ROM in .bin format.
        /// </summary>
        /// <param name="romLocation">The location of the ROM.</param>
        /// <param name="configLocation">The location of the config data.</param>
        /// <returns>A valid BinFormatRom if the location appears to be a valid .bin (or similar) file, otherwise <c>null</c>.</returns>
        /// <remarks>Apparently, there may be odd-sized images floating around out there, in which case this may fail.</remarks>
        internal static new BinFormatRom Create(StorageLocation romLocation, StorageLocation configLocation)
        {
            BinFormatRom bin = null;
            var format = CheckFormat(romLocation);
            if (format == RomFormat.Bin)
            {
                using (var configFile = configLocation.IsValid ? configLocation.OpenStream() : null)
                {
                    // any valid .bin image will be even sized -- except for some available through the Digital Press. For some reason, these all seem to be a multiple of
                    // 8KB + 1 byte in size. So allow those through, too.
                    var configFileCheck = ((configFile != null) && (configFile.Length > 0)) || configFile == null;
                    if (configFileCheck)
                    {
                        bin = new BinFormatRom() { Format = RomFormat.Bin, IsValid = true, RomPath = romLocation, ConfigPath = configLocation };
                    }
                }
            }
            return bin;
        }

        /// <summary>
        /// Given the location of a ROM file, attempt to determine its format.
        /// </summary>
        /// <param name="location">Location of the potential ROM file.</param>
        /// <returns>The format of the file. If it does not appear to be a ROM, then <c>RomFormat.None</c> is returned.</returns>
        internal static RomFormat CheckFormat(StorageLocation location)
        {
            var format = CheckMemo(location);
            if (format == RomFormat.None)
            {
                using (System.IO.Stream file = location.OpenStream())
                {
                    var fileSizeCheck = file != null;
                    if (fileSizeCheck)
                    {
                        var allowOddRomFileSize = !ProgramFileKind.Rom.HasCustomRomExtension(location.Path);
                        fileSizeCheck = IsValidFileSize(file, allowOddRomFileSize);
                    }
                    if (fileSizeCheck)
                    {
                        format = RomFormat.Bin;
                    }
                }
            }
            return format;
        }

        /// <summary>
        /// Inspects data in the stream to determine if it appears to be a BIN-format ROM.
        /// </summary>
        /// <param name="stream">The stream containing the data to inspect.</param>
        /// <returns><c>RomFormat.Bin</c> if the data in stream might be a BIN-format ROM, otherwise <c>RomFormat.None</c>.</returns>
        /// <remarks>This format check is extremely lax, so it is presumed the caller has done at least a file-name check based
        /// on file extension before calling this method. That is, false positive results can be extremely common.</remarks>
        internal static RomFormat CheckFormat(System.IO.Stream stream)
        {
            var format = RomFormat.None;
            var fileSizeCheck = stream != null;
            if (fileSizeCheck)
            {
                // Coerce allow odd ROM image sizes to true since certain ... shady ... ROM sites commonly
                // have ZIPped up ROMs that somehow have an extra byte at the end.
                var position = stream.Position;
                try
                {
                    fileSizeCheck = IsValidFileSize(stream, allowOddRomFileSize: true);
                }
                finally
                {
                    stream.Seek(position, System.IO.SeekOrigin.Begin);
                }
            }
            if (fileSizeCheck)
            {
                format = RomFormat.Bin;
            }
            return format;
        }

        /// <summary>
        /// Get the Crc32 values of a ROM.
        /// </summary>
        /// <param name="romLocation">Location of the ROM whose CRC32 value is desired.</param>
        /// <param name="cfgLocation">Location of the configuration (.cfg) file whose CRC32 is desired. May be <c>null</c>, depending on the ROM.</param>
        /// <param name="cfgCrc">Receives the CRC32 of the configuration data, if applicable. Could be zero.</param>
        /// <returns>CRC32 of the ROM image.</returns>
        /// <remarks>Instead of zero, should the Crc32.InitialValue be used as a sentinel 'invalid' value?</remarks>
        internal static uint GetCrcs(StorageLocation romLocation, StorageLocation cfgLocation, out uint cfgCrc)
        {
            cfgCrc = 0;
            uint romCrc = 0;
            if (!string.IsNullOrEmpty(romLocation.Path) && romLocation.Exists())
            {
                romCrc = Crc32.OfFile(romLocation);
            }
            if (!string.IsNullOrEmpty(cfgLocation.Path) && cfgLocation.Exists())
            {
                cfgCrc = Crc32.OfFile(cfgLocation);
            }
            return romCrc;
        }

        private static bool IsValidFileSize(System.IO.Stream file, bool allowOddRomFileSize)
        {
            // Any valid .bin image will be even sized -- except for some available through the Digital Press. For some reason,
            // these all seem to be a multiple of 8KB + 1 byte in size. Allow those through, too.
            var fileSizeCheck = file.Length >= MinRomSize;
            if (fileSizeCheck)
            {
                if (allowOddRomFileSize)
                {
                    fileSizeCheck = (file.Length % 2) == 0;
                    if (!fileSizeCheck)
                    {
                        fileSizeCheck = ((file.Length - 1) % MinRomSize) == 0;
                    }
                }
                else
                {
                    fileSizeCheck = (file.Length % MinRomSize) == 0;
                }
            }
            return fileSizeCheck;
        }
    }
}

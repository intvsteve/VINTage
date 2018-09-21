// <copyright file="BinFormatRom.cs" company="INTV Funhouse">
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
        /// Gets the metadata from CFGVAR values that may be included in the .cfg file.
        /// </summary>
        /// <remarks>NOTE: Parsing here is EXTREMELY primitive. The code does not enforce the notion of
        /// the 'vars' section, it just blindly scans for values with 'keys' listed in as1600.txt.
        /// NOTE: The metadata is not cached! This means it will be parsed when requested.</remarks>
        public IEnumerable<CfgVarMetadataBlock> Metadata
        {
            get
            {
                var metadata = new List<CfgVarMetadataBlock>();
                try
                {
                    if (IsValid && StreamUtilities.FileExists(ConfigPath))
                    {
                        using (var file = StreamUtilities.OpenFileStream(ConfigPath))
                        {
                            var cfgFileSize = (int)StreamUtilities.FileSize(ConfigPath);
                            var cfgFileBytes = new byte[cfgFileSize];
                            if (file.Read(cfgFileBytes, 0, cfgFileSize) == cfgFileSize)
                            {
                                // PCLs only support UTF8... Spec says ASCII. Let's hope we don't run into anything *too* weird.
                                var cfgFileContents = System.Text.Encoding.UTF8.GetString(cfgFileBytes, 0, cfgFileBytes.Length).Trim('\0');
                                var cfgFileLines = cfgFileContents.Split(new[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                                foreach (var cfgFileLine in cfgFileLines)
                                {
                                    // Square peg, meet round hole.
                                    var cfgFileLineBytes = System.Text.Encoding.UTF8.GetBytes(cfgFileLine);
                                    using (var cfgFileLineStream = new System.IO.MemoryStream(cfgFileLineBytes))
                                    {
                                        var metadataBlock = CfgVarMetadataBlock.Inflate(cfgFileLineStream);
                                        if (metadataBlock != null)
                                        {
                                            metadata.Add(metadataBlock);
                                        }
                                    }
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

        #endregion // Properties

        #region IRom

        /// <inheritdoc />
        public override bool Validate()
        {
            var isValid = !string.IsNullOrEmpty(RomPath) && StreamUtilities.FileExists(RomPath);
            if (isValid && !string.IsNullOrEmpty(ConfigPath))
            {
                isValid = StreamUtilities.FileExists(ConfigPath);
            }
            IsValid = isValid;
            return IsValid;
        }

        /// <inheritdoc />
        public override uint RefreshCrc(out bool changed)
        {
            var crc = _crc;
            if (IsValid && StreamUtilities.FileExists(RomPath))
            {
                uint dontCare;
                _crc = GetCrcs(RomPath, null, out dontCare);
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
            var cfgCrc = _cfgCrc;
            if (IsValid && !string.IsNullOrEmpty(ConfigPath) && StreamUtilities.FileExists(ConfigPath))
            {
                GetCrcs(null, ConfigPath, out _cfgCrc);
                if (cfgCrc == 0)
                {
                    cfgCrc = _cfgCrc; // lazy initialization means on first read, we should never get a change
                }
            }
            changed = cfgCrc != _cfgCrc;
            return _cfgCrc;
        }

        #endregion // IRom

        /// <summary>
        /// Replace the configuration file path in the ROM with the one provided.
        /// </summary>
        /// <param name="cfgPath">The new configuration file path.</param>
        public void ReplaceCfgPath(string cfgPath)
        {
            ConfigPath = cfgPath;
            _cfgCrc = 0; // force recalculation CfgCrc next time it's requested
        }

        /// <summary>
        /// Examines the given data stream and attempts to determine if it is in .bin format.
        /// </summary>
        /// <param name="filePath">The path to the ROM file.</param>
        /// <param name="configFilePath">The path to the config file.</param>
        /// <returns>A valid BinFormatRom if the file appears to be a valid .bin (or similar) file, otherwise <c>null</c>.</returns>
        /// <remarks>Apparently, there may be odd-sized files floating around out there, in which case this will fail.</remarks>
        internal static new BinFormatRom Create(string filePath, string configFilePath)
        {
            BinFormatRom bin = null;
            var format = CheckFormat(filePath);
            if (format == RomFormat.Bin)
            {
                using (System.IO.Stream configFile = (configFilePath == null) ? null : StreamUtilities.OpenFileStream(configFilePath))
                {
                    // any valid .bin file will be even sized -- except for some available through the Digital Press. For some reason, these all seem to be a multiple of
                    // 8KB + 1 byte in size. So allow those through, too.
                    var configFileCheck = ((configFile != null) && (configFile.Length > 0)) || configFile == null;
                    if (configFileCheck)
                    {
                        bin = new BinFormatRom() { Format = RomFormat.Bin, IsValid = true, RomPath = filePath, ConfigPath = configFilePath };
                    }
                }
            }
            return bin;
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
                using (System.IO.Stream file = StreamUtilities.OpenFileStream(filePath))
                {
                    // any valid .bin file will be even sized -- except for some available through the Digital Press. For some reason, these all seem to be a multiple of
                    // 8KB + 1 byte in size. So allow those through, too.
                    var fileSizeCheck = file != null;
                    if (fileSizeCheck && ProgramFileKind.Rom.HasCustomRomExtension(filePath))
                    {
                        fileSizeCheck = (file.Length >= MinRomSize) && ((file.Length % MinRomSize) == 0);
                    }
                    else if (fileSizeCheck)
                    {
                        fileSizeCheck = (file.Length >= MinRomSize) && (((file.Length % 2) == 0) || (((file.Length - 1) % MinRomSize) == 0));
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
        /// Get the Crc32 values of a ROM.
        /// </summary>
        /// <param name="romPath">Absolute path of the ROM whose CRC32 value is desired.</param>
        /// <param name="cfgPath">Absolute path of the configuration (.cfg) file whose CRC32 is desired. May be <c>null</c>, depending on the ROM.</param>
        /// <param name="cfgCrc">Receives the CRC32 of the configuration file, if applicable. Could be zero.</param>
        /// <returns>CRC32 of the ROM file.</returns>
        /// <remarks>Instead of zero, should the Crc32.InitialValue be used as a sentinel 'invalid' value?</remarks>
        internal static uint GetCrcs(string romPath, string cfgPath, out uint cfgCrc)
        {
            cfgCrc = 0;
            uint romCrc = 0;
            if (!string.IsNullOrEmpty(romPath) && StreamUtilities.FileExists(romPath))
            {
                romCrc = Crc32.OfFile(romPath);
            }
            if (!string.IsNullOrEmpty(cfgPath) && StreamUtilities.FileExists(cfgPath))
            {
                cfgCrc = Crc32.OfFile(cfgPath);
            }
            return romCrc;
        }
    }
}

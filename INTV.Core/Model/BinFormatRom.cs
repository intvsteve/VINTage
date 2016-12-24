// <copyright file="BinFormatRom.cs" company="INTV Funhouse">
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
            var isValid = !string.IsNullOrEmpty(RomPath) && RomPath.FileExists();
            if (isValid && !string.IsNullOrEmpty(ConfigPath))
            {
                isValid = ConfigPath.FileExists();
            }
            IsValid = isValid;
            return IsValid;
        }

        /// <inheritdoc />
        public override uint RefreshCrc(out bool changed)
        {
            var crc = _crc;
            if (IsValid && RomPath.FileExists())
            {
                _crc = Crc32.OfFile(RomPath);
                if (0 == crc)
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
            if (IsValid && !string.IsNullOrEmpty(ConfigPath) && ConfigPath.FileExists())
            {
                _cfgCrc = Crc32.OfFile(ConfigPath);
                if (0 == cfgCrc)
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
            using (System.IO.Stream file = filePath.OpenFileStream(), configFile = (configFilePath == null) ? null : configFilePath.OpenFileStream())
            {
                // any valid .bin file will be even sized -- except for some available through the Digital Press. For some reason, these all seem to be a multiple of
                // 8KB + 1 byte in size. So allow those through, too.
                var configFileCheck = ((configFile != null) && (configFile.Length > 0)) || configFile == null;
                var fileSizeCheck = file != null;
                if (fileSizeCheck && ProgramFileKind.Rom.HasCustomRomExtension(filePath))
                {
                    fileSizeCheck = (file.Length >= MinRomSize) && ((file.Length % MinRomSize) == 0);
                }
                else if (fileSizeCheck)
                {
                    fileSizeCheck = (file.Length >= MinRomSize) && (((file.Length % 2) == 0) || (((file.Length - 1) % MinRomSize) == 0));
                }
                if (configFileCheck && fileSizeCheck)
                {
                    bin = new BinFormatRom() { Format = RomFormat.Bin, IsValid = true, RomPath = filePath, ConfigPath = configFilePath };
                }
            }
            return bin;
        }
    }
}

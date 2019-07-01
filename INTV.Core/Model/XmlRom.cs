// <copyright file="XmlRom.cs" company="INTV Funhouse">
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

using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// This class represents a ROM described by XML, which is identified by one or two separate locations.
    /// </summary>
    /// <remarks>This implementation of IRom will ultimately resolve into either a BinFormatRom, a RomFormatRom, a LuigiFormatRom, or null.</remarks>
    internal class XmlRom : Rom
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of XmlRom.
        /// </summary>
        public XmlRom()
        {
            _romPath = StorageLocation.InvalidLocation;
            _configPath = StorageLocation.InvalidLocation;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the resolved ROM. If the ROM is not valid, this property will be <c>null</c> and IsValid will be <c>false</c>.
        /// </summary>
        public IRom ResolvedRom { get; private set; }

        #region IRom

        /// <inheritdoc />
        public override RomFormat Format
        {
            get { return IsValid ? ResolvedRom.Format : RomFormat.None; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            protected set { }
        }

        /// <inheritdoc />
        public override StorageLocation RomPath
        {
            get
            {
                return IsValid ? ResolvedRom.RomPath : _romPath;
            }

            protected set
            {
                if (!IsValid || (RomPath != value))
                {
                    ResolvedRom = Create(value, ConfigPath);
                    IsValid = ResolvedRom != null;
                    if (!IsValid)
                    {
                        _romPath = value;
                    }
                }
            }
        }
        private StorageLocation _romPath;

        /// <inheritdoc />
        public override StorageLocation ConfigPath
        {
            get
            {
                return IsValid ? ResolvedRom.ConfigPath : _configPath;
            }

            protected set
            {
                if (!IsValid || (ConfigPath != value))
                {
                    if (RomPath.IsValid)
                    {
                        ResolvedRom = Create(RomPath, value);
                    }
                    IsValid = ResolvedRom != null;
                    if (!IsValid)
                    {
                        _configPath = value;
                    }
                }
            }
        }
        private StorageLocation _configPath;

        /// <inheritdoc />
        public override uint Crc
        {
            get { return IsValid ? ResolvedRom.Crc : 0; }
        }

        /// <inheritdoc />
        public override uint CfgCrc
        {
            get { return IsValid ? ResolvedRom.CfgCrc : 0; }
        }

        /// <inheritdoc />
        public override bool Validate()
        {
            var isValid = ResolvedRom != null;
            if (!isValid)
            {
                ResolvedRom = Rom.Create(RomPath, ConfigPath);
            }
            IsValid = ResolvedRom != null;
            if (IsValid)
            {
                isValid = ResolvedRom.Validate();
            }
            return isValid;
        }

        /// <inheritdoc />
        public override uint RefreshCrc(out bool changed)
        {
            changed = false;
            var crc = 0u;
            if (IsValid)
            {
                crc = ResolvedRom.RefreshCrc(out changed);
            }
            return crc;
        }

        /// <inheritdoc />
        public override uint RefreshCfgCrc(out bool changed)
        {
            changed = false;
            var cfgCrc = 0u;
            if (IsValid)
            {
                cfgCrc = ResolvedRom.RefreshCfgCrc(out changed);
            }
            return cfgCrc;
        }

        #endregion // IRom

        #endregion // Properties

        /// <summary>
        /// Updates the RomPath and attempts to resolve the ROM to a known good ROM type.
        /// </summary>
        /// <param name="romLocation">The location of the ROM image.</param>
        /// <remarks>If romPath refers to a ROM requiring separate configuration data, and the configuration
        /// data has not yet been defined, then resolution will fail.</remarks>
        public void UpdateRomPath(StorageLocation romLocation)
        {
            RomPath = romLocation;
        }

        /// <summary>
        /// Updates the ConfigPath and attempts to resolve the ROM to a known good ROM type.
        /// </summary>
        /// <param name="configLocation">The location of the ROM's configuration data.</param>
        /// <remarks>If the RomPath has not yet been set, resolution will fail.</remarks>
        public void UpdateConfigPath(StorageLocation configLocation)
        {
            ConfigPath = configLocation;
        }
    }
}

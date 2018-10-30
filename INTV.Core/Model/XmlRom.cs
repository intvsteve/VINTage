// <copyright file="XmlRom.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// This class represents a ROM described by an XML file, which is identified by one or two separate file system paths.
    /// </summary>
    /// <remarks>This implementation of IRom will ultimately resolve into either a BinFormatRom, a RomFormatRom, or null.</remarks>
    internal class XmlRom : Rom
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of XmlRom.
        /// </summary>
        public XmlRom()
        {
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
        public override string RomPath
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
        private string _romPath;

        /// <inheritdoc />
        public override string ConfigPath
        {
            get
            {
                return IsValid ? ResolvedRom.ConfigPath : _configPath;
            }

            protected set
            {
                if (!IsValid || (ConfigPath != value))
                {
                    if (RomPath != null)
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
        private string _configPath;

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
        /// <param name="romPath">The absolute path to the ROM file.</param>
        /// <remarks>If romPath refers to a file requiring a separate configuration file, and the configuration
        /// file has not yet been defined, then resolution will fail.</remarks>
        public void UpdateRomPath(string romPath)
        {
            RomPath = romPath;
        }

        /// <summary>
        /// Updates the ConfigPath and attempts to resolve the ROM to a known good ROM type.
        /// </summary>
        /// <param name="configPath">The absolute path to the ROM's configuration file.</param>
        /// <remarks>If the RomPath has not yet been set, resolution will fail.</remarks>
        public void UpdateConfigPath(string configPath)
        {
            ConfigPath = configPath;
        }
    }
}

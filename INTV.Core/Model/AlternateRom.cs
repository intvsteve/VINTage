// <copyright file="AlternateRom.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Implementation of Rom for use when the primary ROM file cannot be located, but an alternative copy can be found.
    /// </summary>
    internal sealed class AlternateRom : Rom
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AlternateRom type.
        /// </summary>
        /// <param name="romPath">The alternate path for the ROM described by <paramref name="originalRom"/>.</param>
        /// <param name="configPath">The alternate configuration file path for the ROM described by <paramref name="originalRom"/>.</param>
        /// <param name="originalRom">The original ROM.</param>
        public AlternateRom(string romPath, string configPath, IRom originalRom)
        {
            Alternate = Rom.Create(romPath, configPath);
            RomPath = Alternate.RomPath;
            ConfigPath = Alternate.ConfigPath;
            IsValid = Alternate.IsValid;
            Original = originalRom;
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the alternate representation of the original ROM.
        /// </summary>
        public IRom Alternate { get; private set; }

        /// <summary>
        /// Gets the original ROM.
        /// </summary>
        public IRom Original { get; private set; }

        #region IRom

        /// <inheritdoc />
        public override RomFormat Format
        {
            get { return Alternate.Format; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            protected set { }
        }

        /// <inheritdoc />
        public override bool IsValid
        {
            get { return Alternate.IsValid; }
        }

        /// <inheritdoc />
        public override uint Crc
        {
            get { return Alternate.Crc; }
        }

        /// <inheritdoc />
        public override uint CfgCrc
        {
            get { return Alternate.CfgCrc; }
        }

        /// <inheritdoc />
        public override bool Validate()
        {
            return Alternate.Validate();
        }

        /// <inheritdoc />
        public override uint RefreshCrc(out bool changed)
        {
            return Alternate.RefreshCrc(out changed);
        }

        /// <inheritdoc />
        public override uint RefreshCfgCrc(out bool changed)
        {
            return Alternate.RefreshCfgCrc(out changed);
        }

        #endregion // IRom
    }
}

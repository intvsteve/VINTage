// <copyright file="RomFormatRom.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Implementation of Rom for programs in the .rom (or compatible) format.
    /// </summary>
    internal class RomFormatRom : Rom
    {
        private const int HeaderSize = 53;
        private const int BlockSize = 514;

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
                byte replacementByte = AutoBaudBytes[Format];
                _crc = Crc32.OfFile(RomPath, Format != RomFormat.Intellicart, replacementByte);
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
            using (var file = filePath.OpenFileStream())
            {
                if ((file != null) && (file.Length > 0))
                {
                    byte[] data = new byte[HeaderSize];
                    var numBytesRead = file.Read(data, 0, HeaderSize);
                    if (numBytesRead == HeaderSize)
                    {
                        // Check the header (checks for both .rom and .cc3/.cc3 advanced formats)
                        var format = AutoBaudBytes.FirstOrDefault(a => a.Value == data[0]).Key;
                        if ((format != RomFormat.None) && !((data[1] ^ data[2]) != 0xFF))
                        {
                            // Valid header, so create the instance. Full validation would require walking
                            // all the segments -- essentially most of the ROM.
                            rom = new RomFormatRom() { Format = format, IsValid = true, RomPath = filePath };
                        }
                    }
                }
            }
            return rom;
        }
    }
}

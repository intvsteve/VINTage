// <copyright file="CrcData.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Describes the details of a ROM variant.
    /// </summary>
    public class CrcData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CrcData class.
        /// </summary>
        /// <param name="crc">CRC of the ROM file.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public CrcData(uint crc)
            : this(crc, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CrcData class.
        /// </summary>
        /// <param name="crc">CRC of the ROM file.</param>
        /// <param name="description">A brief description of the ROM variant.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public CrcData(uint crc, string description)
            : this(crc, description, IncompatibilityFlags.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CrcData class.
        /// </summary>
        /// <param name="crc">CRC of the ROM file.</param>
        /// <param name="description">A brief description of the ROM variant.</param>
        /// <param name="incompatibilities">Describes known hardware incompatibilities associated with the ROM.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public CrcData(uint crc, string description, IncompatibilityFlags incompatibilities)
            : this(crc, description, incompatibilities, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CrcData class.
        /// </summary>
        /// <param name="crc">CRC of the ROM file.</param>
        /// <param name="description">A brief description of the ROM variant.</param>
        /// <param name="incompatibilities">Describes known hardware incompatibilities associated with the ROM.</param>
        /// <param name="binConfigTemplate">The configuration template to use if a .cfg file for a .bin cannot be found.</param>
        public CrcData(uint crc, string description, IncompatibilityFlags incompatibilities, int binConfigTemplate)
        {
            Crc = crc;
            Description = description;
            Incompatibilities = incompatibilities;
            BinConfigTemplate = binConfigTemplate;
        }

        /// <summary>
        /// Initializes a new instance of the CrcData class.
        /// </summary>
        /// <param name="crc">CRC of the ROM file.</param>
        /// <param name="data">A Key/Value pair of the ROMs description and known incompatibilities.</param>
        /// <exception cref="ArgumentException">Thrown if crc is zero.</exception>
        public CrcData(uint crc, KeyValuePair<string, IncompatibilityFlags> data)
            : this(crc, data.Key, data.Value)
        {
            if (crc == 0)
            {
                throw new System.ArgumentException(Resources.Strings.CrcData_InvalidCRC);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the 32-bit CRC value of a ROM file.
        /// </summary>
        public uint Crc { get; set; }

        /// <summary>
        /// Gets or sets the description of a ROM variant.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the known incompatibilities of a ROM variant.
        /// </summary>
        public IncompatibilityFlags Incompatibilities { get; set; }

        /// <summary>
        /// Gets or sets the number of the .cfg file template to use for the ROM in case the .cfg is missing.
        /// </summary>
        public int BinConfigTemplate { get; set; }

        #endregion // Properties

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is CrcData))
            {
                return false;
            }
            return Crc == (obj as CrcData).Crc;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Name: '{0}', CRC: 0x{1}", Description, Crc.ToString("X8"));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Crc.GetHashCode();
        }
    }
}

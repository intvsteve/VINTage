// <copyright file="RomComparerStrictCrcOnly.cs" company="INTV Funhouse">
// Copyright (c) 2015-2016 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Implements the simplest form of ROM comparison - are they the same format, and do their CRCs match.
    /// </summary>
    /// <remarks>NOTE: This comparison will treat the same .bin or .luigi ROMs as identical even if the two
    /// ROMs have different configuration files!</remarks>
    public class RomComparerStrictCrcOnly : RomComparer
    {
        /// <summary>
        /// The instance to use for the simplest and fastest ROM comparison.
        /// </summary>
        public static readonly RomComparerStrictCrcOnly Default = new RomComparerStrictCrcOnly();

        #region RomComparer

        /// <inheritdoc />
        public override int Compare(IRom x, IProgramInformation programInformationRomX, IRom y, IProgramInformation programInformationRomY)
        {
            var result = (int)x.Format - (int)y.Format;
            if (result == 0)
            {
                result = (int)x.Crc - (int)y.Crc;
            }
            if (((x.Format == RomFormat.None) && (x.Crc == 0)) || ((x.Crc == 0) && (y.Crc == 0)))
            {
                // Both ROMs in the comparison are missing -- and both have the same CRC - which was zero.
                // In such a case, try comparing the paths.
                result = string.Compare(x.RomPath, y.RomPath);
            }
            return result;
        }

        #endregion // RomComparer
    }
}

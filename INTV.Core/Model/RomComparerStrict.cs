// <copyright file="RomComparerStrict.cs" company="INTV Funhouse">
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
    /// The strictest ROM comparison, in which the ROM format and CRC must match - but, in addition, if a configuration file
    /// is also identifiable, the CRCs of the .cfg associated with the two ROMs must match as well.
    /// </summary>
    public class RomComparerStrict : RomComparerStrictCrcOnly
    {
        /// <summary>
        /// The default, simple comparer.
        /// </summary>
        public static new readonly RomComparerStrict Default = new RomComparerStrict();

        #region RomComparerStrictCrcOnly Overrides

        /// <inheritdoc />
        public override int Compare(IRom x, IProgramInformation programInformationRomX, IRom y, IProgramInformation programInformationRomY)
        {
            var result = base.Compare(x, programInformationRomX, y, programInformationRomY);
            if (result == 0)
            {
                switch (x.Format)
                {
                    case RomFormat.Bin:
                        result = CheckCrcs(x, programInformationRomX, y, programInformationRomY);
                        break;
                    case RomFormat.Luigi:
                        result = CheckCrcs(x, programInformationRomX, y, programInformationRomY);
                        if ((result == 0) && (y.Format == RomFormat.Luigi))
                        {
                            // It's possible LUIGI files' original CRCs match, but the LUIGI files themselves do not.
                            // This once occurred when additional bytes were somehow appended to a downloaded LUIGI. In
                            // that case, all the CRC data matched, so even the "strict" CRC check, which compared the
                            // full 8 bytes of the ID in the header, still matched. However, the additional data at the
                            // end of the corrupted file resulted in failure to load on LTO Flash! hardware. Therefore,
                            // when comparing what appear to be two identical LUIGI files, still compare full file CRCs.
                            result = (int)(Rom.AsSpecificRomType<LuigiFormatRom>(x).Crc24 - Rom.AsSpecificRomType<LuigiFormatRom>(y).Crc24);
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        #endregion //  RomComparerStrictCrcOnly Overrides

        private static int CheckCrcs(IRom x, IProgramInformation programInformationRomX, IRom y, IProgramInformation programInformationRomY)
        {
            var cfgCrcRomX = x.CfgCrc;
            var cfgCrcRomY = y.CfgCrc;
            var result = (int)cfgCrcRomX - (int)cfgCrcRomY;
            if ((result != 0) && ((cfgCrcRomX == 0) || (cfgCrcRomY == 0)))
            {
                if (cfgCrcRomX == 0)
                {
                    var programInfo = programInformationRomX;
                    if (programInfo == null)
                    {
                        programInfo = x.GetProgramInformation();
                    }
                    var cfgFilePath = x.GetStockCfgFile(programInfo);
                    if (!string.IsNullOrEmpty(cfgFilePath))
                    {
                        cfgCrcRomX = INTV.Core.Utility.Crc32.OfFile(cfgFilePath);
                    }
                }
                if (cfgCrcRomY == 0)
                {
                    var programInfo = programInformationRomY;
                    if (programInfo == null)
                    {
                        programInfo = y.GetProgramInformation();
                    }
                    var cfgFilePath = y.GetStockCfgFile(programInfo);
                    if (!string.IsNullOrEmpty(cfgFilePath))
                    {
                        cfgCrcRomY = INTV.Core.Utility.Crc32.OfFile(cfgFilePath);
                    }
                }
                result = (int)cfgCrcRomX - (int)cfgCrcRomY;
            }
            return result;
        }
    }
}

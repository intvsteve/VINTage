// <copyright file="TestRomResources.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTV.TestHelpers.Core.Utility
{
    public static class TestRomResources
    {
        #region BIN+CFG format ROMs

        /// <summary>Intellivision Lives! for Mac did not use file extensions on ROMs.</summary>
        public const string TestBinPathNoFileExtension = "/Resources/tagalong";

        /// <summary>Path for standard .BIN format ROM file / resource.</summary>
        public const string TestBinPath = "/Resources/tagalong.bin";

        /// <summary>CRC32 of <see cref="TestBinPath"/>.</summary>
        public const uint TestBinCrc = 0xECBA3AF7;

        /// <summary>Path for standard .CFG for the .BIN-format ROM at <see cref="TestBinPath"/>.</summary>
        public const string TestCfgPath = "/Resources/tagalong.cfg";

        /// <summary>CRC32 of <see cref="TestCfgPath"/>.</summary>
        public const uint TestCfgCrc = 0x06B5EA3E;

        /// <summary>A duplicate of <see cref="TestBinPath"/>.</summary>
        public const string TestBinMetadataPath = "/Resources/tagalong_metadata.bin";

        /// <summary>Path for a .CFG file containing metadata via [vars].</summary>
        public const string TestCfgMetadataPath = "/Resources/tagalong_metadata.cfg";

        /// <summary>CRC32 of <see cref="TestCfgMetadataPath"/>.</summary>
        public const uint TestMetadataCfgCrc = 0x68C3401C;

        /// <summary>A copy of <see cref="TestBinPath"/> with a different file extension.</summary>
        public const string TestItvPath = "/Resources/tagalong.itv";

        /// <summary>A version of <see cref="TestBinPath"/> with an extra byte of 0xFF appended, simulating some ROMs in the wild that are one-off on size.</summary>
        public const string TestIntPath = "/Resources/tagalong.int";

        #endregion // BIN+CFG format ROMs

        #region Corrupted BIN+CFG format ROMs

        /// <summary>A .CFG file that is weirdly corrupt.</summary>
        public const string TestCfgCorruptMetadataPath = "/Resources/tagalong_metadata_corrupt.cfg";

        /// <summary>A .CFG file with multiple vars sections and may corrupted entries.</summary>
        public const string TestCfgBadMetadataPath = "/Resources/tagalong_metadata_bad.cfg";

        #endregion // Corrupted BIN+CFG format ROMs

        #region ROM format ROMs

        /// <summary>Path for a .ROM format ROM created by using bin2rom with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input.</summary>
        public const string TestRomPath = "/Resources/tagalong.rom";

        /// <summary>CRC32 of <see cref="TestRomPath"/>.</summary>
        public const uint TestRomCrc = 0xFEF0BD41;

        /// <summary>Path for a .ROM format ROM created by using bin2rom --cc3 with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input.</summary>
        public const string TestCc3Path = "/Resources/tagalong.cc3";

        /// <summary>CRC32 of <see cref="TestCc3Path"/>.</summary>
        public const uint TestCc3Crc = 0x511F0AD1;

        /// <summary>Path for a .ROM format ROM created by using bin2rom --adv (or was it hacked?) with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input.</summary>
        public const string TestAdvPath = "/Resources/tagalong.adv";

        /// <summary>CRC32 of <see cref="TestAdvPath"/>.</summary>
        public const uint TestAdvCrc = 0xA56346FC;

        /// <summary>Path for a .ROM format ROM created by using bin2rom with <see cref="TestBinMetadataPath"/> and <see cref="TestCfgMetadataPath"/> as the input.</summary>
        public const string TestRomMetadataPath = "/Resources/tagalong_metadata.rom";

        /// <summary>CRC32 of <see cref="TestRomMetadataPath"/>.</summary>
        public const uint TestRomMetadataCrc = 0xC03B6B9E;

        #endregion // ROM format ROMs

        #region Corrupted ROM format ROMs

        /// <summary>Path for a corrupted version of <see cref="TestRomPath"/>, in which the check byte of the header was hacked.</summary>
        public const string TestRomBadHeaderPath = "/Resources/tagalong.bad";

        /// <summary>Path for a corrupted version of <see cref="TestRomPath"/>, in which the first segment's low and high bytes were hacked.</summary>
        public const string TestRomCorruptedPath = "/Resources/tagalong_corrupt.rom";

        /// <summary>Path for a corrupted version of <see cref="TestRomMetadataPath"/>, in which the CRC of the very last metadata block has been hacked.</summary>
        public const string TestRomMetadataBadCrcPath = "/Resources/tagalong_metadata_badcrc.rom";

        #endregion // Corrupted ROM format ROMs

        #region LUIGI format ROMs

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input.</summary>
        public const string TestLuigiFromBinPath = "/Resources/tagalong.luigi";

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinMetadataPath"/> and <see cref="TestCfgMetadataPath"/> as the input.</summary>
        public const string TestLuigiWithMetadataPath = "/Resources/tagalong_metadata.luigi";

        /// <summary>Path for a .LUIGI format ROM created by using rom2luigi with <see cref="TestRomPath"/>.</summary>
        public const string TestLuigiFromRomPath = "/Resources/tagalong_from_rom.luigi";

        /// <summary>Path for a .LUIGI format ROM that has a version 0x00 header. None of these should exist in the wild.</summary>
        public const string TestLuigiFromBinWithVersionZeroHeaderPath = "/Resources/tagalong_v0.luigi";

        /// <summary>Path for a .LUIGI format ROM that has a version 0xDD header from the future. It's totally a hack.</summary>
        public const string TestLuigiFromFuturePath = "/Resources/tagalong_from_future.luigi";

        /// <summary>Path for a .LUIGI format ROM that has no unique ID. The original .BIN and .CFG CRCs have been zeroed out,
        /// with the header checksum patched up. This is a borderline corrupted LUIGI.</summary>
        public const string TestLuigiWithZeroCrcsPath = "/Resources/tagalong_zero_crc.luigi";

        /// <summary>CRC32 of <see cref="TestLuigiWithZeroCrcsPath"/>.</summary>
        public const uint TestLuigiWithZeroCrcsCrc = 0x77549CE3;

        #endregion // LUIGI format ROMs

        #region Scrambled LUIGI format ROMs

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input,
        /// scrambled to work on any LTO Flash! - but scrambled nonetheless.</summary>
        public const string TestLuigiScrambledForAnyDevicePath = "/Resources/tagalong_any.luigi";

        /// <summary>CRC32 of <see cref="TestLuigiScrambledForAnyDevicePath"/>.</summary>
        public const uint TestLuigiScrambledForAnyDeviceCrc = 0x619CC8FA;

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinMetadataPath"/> and <see cref="TestCfgMetadataPath"/> as the input,
        /// scrambled to work on a specific LTO Flash!.</summary>
        public const string TestLuigiWithMetadatdaScrambledForAnyDevicePath = "/Resources/tagalong_metadata_any.luigi";

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input,
        /// scrambled to work on a specific LTO Flash!.</summary>
        public const string TestLuigiScrambledForDevice0Path = "/Resources/tagalong_dev0.luigi";

        /// <summary>CRC32 of <see cref="TestLuigiScrambledForDevice0Path"/>.</summary>
        public const uint TestLuigiScrambledForDevice0Crc = 0xB7D6E9BB;

        /// <summary>The unique ID for which <see cref="TestLuigiScrambledForDevice0Path"/> is scrambled</summary>
        public const string TestLuigiScrambledForDevice0UniqueId = "471CE1A23325706E8F91CCDA1E5DB8E3";

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input,
        /// scrambled to work on a specific LTO Flash!.</summary>
        public const string TestLuigiScrambledForDevice1Path = "/Resources/tagalong_dev1.luigi";

        /// <summary>The unique ID for which <see cref="TestLuigiScrambledForDevice1Path"/> is scrambled</summary>
        public const string TestLuigiScrambledForDevice1UniqueId = "E20B175F51F43AA7C558AE0BE8A01DF4";

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinMetadataPath"/> and <see cref="TestCfgMetadataPath"/> as the input,
        /// scrambled to work on a specific LTO Flash!.</summary>
        public const string TestLuigiWithMetadatdaScrambledForDevice0Path = "/Resources/tagalong_metadata_dev0.luigi";

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinMetadataPath"/> and <see cref="TestCfgMetadataPath"/> as the input,
        /// scrambled to work on a specific LTO Flash!.</summary>
        public const string TestLuigiWithMetadatdaScrambledForDevice1Path = "/Resources/tagalong_metadata_dev1.luigi";

        #endregion // Scrambled LUIGI format ROMs

        #region Corrupted LUIGI format ROMs

        /// <summary>Path for a .LUIGI format ROM created by using bin2luigi with <see cref="TestBinPath"/> and <see cref="TestCfgPath"/> as the input
        /// whose header CRC has been corrupted.</summary>
        public const string TestLuigiWithBadHeaderCrcPath = "/Resources/tagalong_header_bad_crc.luigi";

        /// <summary>Presently, a copy of <see cref="TestLuigiWithBadHeaderCrcPath"/>.</summary>
        public const string TestLuigiWithBadHeaderPath = "/Resources/tagalong_bad.luigi";

        /// <summary>A copy of <see cref="TestLuigiFromBinPath"/> with an extra trailing <c>null</c> byte.</summary>
        public const string TestLuigiWithExtraNullBytePath = "/Resources/tagalong_extra_null_byte.luigi";

        #endregion // Corrupted LUIGI format ROMs
    }
}

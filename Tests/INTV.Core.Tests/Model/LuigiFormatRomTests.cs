// <copyright file="LuigiFormatRomTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiFormatRomTests
    {
        private const string TestLuigiFromBinPath = "/Resources/tagalong.luigi";
        private const string TestLuigiFromRomPath = "/Resources/tagalong_from_rom.luigi";

        private const string TestLuigiAnyPath = "/Resources/tagalong_any.luigi";
        private const string TestLuigiDev0Path = "/Resources/tagalong_dev0.luigi";
        private const string TestLuigiDev1Path = "/Resources/tagalong_dev1.luigi";

        private const string TestLuigiBadPath = "/Resources/tagalong_bad.luigi";
        private const string TestLuigiWithZeroCrcsPath = "/Resources/tagalong_zero_crc.luigi";
        private const string TestLuigiWithMetadataPath = "/Resources/tagalong_metadata.luigi";
        private const string TestLuigiHeaderHasBadCrcPath = "/Resources/tagalong_header_bad_crc.luigi";
        private const string TestLuigiFromFuturePath = "/Resources/tagalong_from_future.luigi"; // version is hacked to xDD

        private const uint TestBinOrigCrc = 0xECBA3AF7;
        private const uint TestCfgOrigCrc = 0x06B5EA3E;
        private const uint TestRomOrigCrc = 0xFEF0BD41;
        private const uint TestZeroCrcsLuigiCrc = 0x77549CE3;

        private static readonly string ScrambledDev0UniqueId = "471CE1A23325706E8F91CCDA1E5DB8E3";
        private static readonly string ScrambledDev1UniqueId = "E20B175F51F43AA7C558AE0BE8A01DF4";

        [Fact]
        public void LuigiFormatRom_LoadAndValidateRom_RomFormatIdentifiedCorrectly()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.Create(TestLuigiFromBinPath, null);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Luigi, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void LuigiFormatRom_Load_VerifyCrc()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.Create(TestLuigiFromBinPath, null);

            Assert.Equal(TestBinOrigCrc, rom.Crc);
            Assert.Equal(TestCfgOrigCrc, rom.CfgCrc);
        }

        [Fact]
        public void LuigiFormatRomFromFuture_Load_DoesNotLoadAsLuigiFormatRom()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromFuturePath);
            var rom = Rom.Create(TestLuigiFromFuturePath, null);

            // At this time, the ROM *just happens* to pass the .bin format tests! Depending on
            // many factors, differently "fake corrupted" ROMs could fail to load or not.
            // This test is OK with a null or .bin result.
            Assert.True((rom == null) || (rom.Format == RomFormat.Bin));
        }

        [Fact]
        public void LuigiFormatRom_Load_VerifyCrc24()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestLuigiFromBinPath, null));

            Assert.Equal(0x0035f671u, rom.Crc24);
        }

        [Fact]
        public void LuigiFormatRom_LoadStandardLuigi_VerifyTargetDeviceUniqueId()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestLuigiFromBinPath, null));

            Assert.True(string.IsNullOrEmpty(rom.TargetDeviceUniqueId));
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledForAnyLuigi_VerifyTargetDeviceUniqueId()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiAnyPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestLuigiAnyPath, null));

            Assert.Equal(LuigiScrambleKeyBlock.AnyLTOFlashId, rom.TargetDeviceUniqueId);
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledLuigis_VerifyTargetDeviceUniqueIds()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiDev0Path, TestLuigiDev1Path);
            var rom0 = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestLuigiDev0Path, null));
            var rom1 = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestLuigiDev1Path, null));

            Assert.NotEqual(rom0.TargetDeviceUniqueId, rom1.TargetDeviceUniqueId);
            Assert.Equal(ScrambledDev0UniqueId, rom0.TargetDeviceUniqueId);
            Assert.Equal(ScrambledDev1UniqueId, rom1.TargetDeviceUniqueId);
        }

        [Fact]
        public void LuigiFormatRom_LoadLuigiWithZeroCrcs_VerifyCrc()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiWithZeroCrcsPath);
            var rom = Rom.Create(TestLuigiWithZeroCrcsPath, null);

            Assert.Equal(TestZeroCrcsLuigiCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromBinOrigin_CrcsMatch()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromBinPath);

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(TestLuigiFromBinPath, null, out cfgCrc);

            Assert.Equal(TestBinOrigCrc, crc);
            Assert.Equal(TestCfgOrigCrc, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromRomOrigin_CrcsMatch()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromRomPath);

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(TestLuigiFromRomPath, null, out cfgCrc);

            Assert.Equal(TestRomOrigCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromFileWithZeroCrcs_CrcMatchesCrcOfFile()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiWithZeroCrcsPath);

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(TestLuigiWithZeroCrcsPath, null, out cfgCrc);

            Assert.Equal(TestZeroCrcsLuigiCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiWithMetadataPath);

            var rom = Rom.Create(TestLuigiWithMetadataPath, null);
            var metadata = rom.GetLuigiFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void LuigiFormatRom_CheckFormatOfCorruptFile_ThrowsNullReferenceException()
        {
            var storageAccess = LuigiFormatRomTestStorageAccess.Initialize(TestLuigiBadPath);

            var corrupted = storageAccess.IntroduceCorruption(TestLuigiBadPath);

            Assert.True(corrupted);
            Assert.Throws<NullReferenceException>(() => LuigiFormatRom.CheckFormat(TestLuigiBadPath));
        }

        [Theory]
        [InlineData(true, 3)]
        [InlineData(false, 2)]
        public void LuigiFormatRom_GetComparisonIgnoreRanges_ReturnsCorrectNumberOfExcludeRanges(bool excludeFeatureBits, int expectedNumberOfExcludeRanges)
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestLuigiFromBinPath, null));

            // Three ranges of values are ignored in this case:
            // 1. Feature flags (depends on value of excludeFeatureBits)
            // 2. The UID (which for this specific test ROM is the CRC32 of the .BIN and the CRC32 of the .CFG of the original ROM
            // 3. The CRC of the header
            var rangesToIgnore = rom.GetComparisonIgnoreRanges(excludeFeatureBits);

            Assert.Equal(expectedNumberOfExcludeRanges, rangesToIgnore.Count());
        }

        private class LuigiFormatRomTestStorageAccess : CachedResourceStorageAccess<LuigiFormatRomTestStorageAccess>
        {
        }
    }
}

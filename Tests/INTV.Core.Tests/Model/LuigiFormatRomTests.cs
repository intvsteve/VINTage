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
        [Fact]
        public void LuigiFormatRom_LoadAndValidateRom_RomFormatIdentifiedCorrectly()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.Create(TestRomResources.TestLuigiFromBinPath, null);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Luigi, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void LuigiFormatRom_Load_VerifyCrc()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.Create(TestRomResources.TestLuigiFromBinPath, null);

            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void LuigiFormatRomFromFuture_Load_DoesNotLoadAsLuigiFormatRom()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromFuturePath);
            var rom = Rom.Create(TestRomResources.TestLuigiFromFuturePath, null);

            // At this time, the ROM *just happens* to pass the .bin format tests! Depending on
            // many factors, differently "fake corrupted" ROMs could fail to load or not.
            // This test is OK with a null or .bin result.
            Assert.True((rom == null) || (rom.Format == RomFormat.Bin));
        }

        [Fact]
        public void LuigiFormatRom_Load_VerifyCrc24()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestRomResources.TestLuigiFromBinPath, null));

            Assert.Equal(0x0035f671u, rom.Crc24);
        }

        [Fact]
        public void LuigiFormatRom_LoadStandardLuigi_VerifyTargetDeviceUniqueId()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestRomResources.TestLuigiFromBinPath, null));

            Assert.True(string.IsNullOrEmpty(rom.TargetDeviceUniqueId));
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledForAnyLuigi_VerifyTargetDeviceUniqueId()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForAnyDevicePath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestRomResources.TestLuigiScrambledForAnyDevicePath, null));

            Assert.Equal(LuigiScrambleKeyBlock.AnyLTOFlashId, rom.TargetDeviceUniqueId);
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledLuigis_VerifyTargetDeviceUniqueIds()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path, TestRomResources.TestLuigiScrambledForDevice1Path);
            var rom0 = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestRomResources.TestLuigiScrambledForDevice0Path, null));
            var rom1 = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestRomResources.TestLuigiScrambledForDevice1Path, null));

            Assert.NotEqual(rom0.TargetDeviceUniqueId, rom1.TargetDeviceUniqueId);
            Assert.Equal(TestRomResources.TestLuigiScrambledForDevice0UniqueId, rom0.TargetDeviceUniqueId);
            Assert.Equal(TestRomResources.TestLuigiScrambledForDevice1UniqueId, rom1.TargetDeviceUniqueId);
        }

        [Fact]
        public void LuigiFormatRom_LoadLuigiWithZeroCrcs_VerifyCrc()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithZeroCrcsPath);
            var rom = Rom.Create(TestRomResources.TestLuigiWithZeroCrcsPath, null);

            Assert.Equal(TestRomResources.TestLuigiWithZeroCrcsCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromBinOrigin_CrcsMatch()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(TestRomResources.TestLuigiFromBinPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestBinCrc, crc);
            Assert.Equal(TestRomResources.TestCfgCrc, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromRomOrigin_CrcsMatch()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromRomPath);

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(TestRomResources.TestLuigiFromRomPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestRomCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromFileWithZeroCrcs_CrcMatchesCrcOfFile()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithZeroCrcsPath);

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(TestRomResources.TestLuigiWithZeroCrcsPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestLuigiWithZeroCrcsCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithMetadataPath);

            var rom = Rom.Create(TestRomResources.TestLuigiWithMetadataPath, null);
            var metadata = rom.GetLuigiFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void LuigiFormatRom_CheckFormatOfCorruptFile_ThrowsNullReferenceException()
        {
            var storageAccess = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithBadHeaderPath);

            var corrupted = storageAccess.IntroduceCorruption(TestRomResources.TestLuigiWithBadHeaderPath);

            Assert.True(corrupted);
            Assert.Throws<NullReferenceException>(() => LuigiFormatRom.CheckFormat(TestRomResources.TestLuigiWithBadHeaderPath));
        }

        [Theory]
        [InlineData(true, 3)]
        [InlineData(false, 2)]
        public void LuigiFormatRom_GetComparisonIgnoreRanges_ReturnsCorrectNumberOfExcludeRanges(bool excludeFeatureBits, int expectedNumberOfExcludeRanges)
        {
            LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(TestRomResources.TestLuigiFromBinPath, null));

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

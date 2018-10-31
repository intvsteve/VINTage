// <copyright file="RomTests.cs" company="INTV Funhouse">
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
    public class RomTests
    {
        private const string TestBinPath = "/Resources/tagalong.bin";
        private const string TestCfgPath = "/Resources/tagalong.cfg";
        private const string TestRomPath = "/Resources/tagalong.rom";
        private const string TestBadPath = "/Resources/tagalong.bad";
        private const string TestCfgMetadataPath = "/Resources/tagalong_metadata.cfg";
        private const string TestLuigiFromBinPath = "/Resources/tagalong.luigi";

        private const uint TestBinCrc = 0xECBA3AF7;
        private const uint TestCfgCrc = 0x06B5EA3E;
        private const uint TestRomCrc = 0xFEF0BD41;
        private const uint TestMetadataCfgCrc = 0x68C3401C;

        [Fact]
        public void Rom_GetRefreshedCrcsFromNullPaths_ThrowsAgumentNullException()
        {
            RomTestStorageAccess.Initialize(null);

            var cfgCrc = 0u;
            Assert.Throws<ArgumentNullException>(() => Rom.GetRefreshedCrcs(null, null, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromNullBinPathAndValidCfgPath_ThrowsAgumentNullException()
        {
            RomTestStorageAccess.Initialize(TestCfgPath);

            var cfgCrc = 0u;
            Assert.Throws<ArgumentNullException>(() => Rom.GetRefreshedCrcs(null, TestCfgPath, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromBinPathAndNullCfgPath_GetsCorrectBinCrcAndZeroCfgCrc()
        {
            RomTestStorageAccess.Initialize(TestBinPath);

            var cfgCrc = 0u;
            var crc = Rom.GetRefreshedCrcs(TestBinPath, null, out cfgCrc);

            Assert.Equal(TestBinCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Theory]
        [InlineData(TestBinPath, TestCfgPath, TestBinCrc, TestCfgCrc)]
        [InlineData(TestRomPath, TestCfgPath, TestRomCrc, 0u)]
        [InlineData(TestLuigiFromBinPath, null, TestBinCrc, TestCfgCrc)]
        [InlineData(TestBadPath, null, 0u, 0u)]
        public void Rom_GetRefreshedCrcsFromValidBinAndCfgPaths_GetsCorrectBinAndCfgCrcs(string testRomPath, string testCfgPath, uint expectedRomCrc, uint expectedCfgCrc)
        {
            RomTestStorageAccess.Initialize(testRomPath, testCfgPath);

            var cfgCrc = 0u;
            var crc = Rom.GetRefreshedCrcs(testRomPath, testCfgPath, out cfgCrc);

            Assert.Equal(expectedRomCrc, crc);
            Assert.Equal(expectedCfgCrc, cfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNullRomWithNullCfgPath_DoesNotThrow()
        {
            RomTestStorageAccess.Initialize(null);

            Rom.ReplaceCfgPath(null, null);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNullRomWithValidCfgPath_DoesNotThrow()
        {
            RomTestStorageAccess.Initialize(TestCfgPath);

            Rom.ReplaceCfgPath(null, TestCfgPath);
        }

        [Fact]
        public void Rom_ReplaceCfgWithNullCfgPath_RemovesConfigPath()
        {
            RomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestBinPath, rom.RomPath);
            Assert.Equal(TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, null);

            Assert.Null(rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgWithNonexistentCfgPath_UpdatesConfigPath()
        {
            RomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestBinPath, rom.RomPath);
            Assert.Equal(TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);

            var invalidCfgPath = "/Resources/Totes/Bogus/Cfg.cfg";
            Rom.ReplaceCfgPath(rom, invalidCfgPath);

            Assert.Equal(invalidCfgPath, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgWithDifferentValidCfgPath_UpdatesConfigPath()
        {
            RomTestStorageAccess.Initialize(TestBinPath, TestCfgPath, TestCfgMetadataPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestBinPath, rom.RomPath);
            Assert.Equal(TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, TestCfgMetadataPath);

            Assert.Equal(TestCfgMetadataPath, rom.ConfigPath);
            Assert.Equal(TestMetadataCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNonBinFormatRom_HasNoEffect()
        {
            RomTestStorageAccess.Initialize(TestRomPath, TestCfgPath, TestCfgMetadataPath);
            var rom = Rom.Create(TestRomPath, TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestRomPath, rom.RomPath);
            Assert.Null(rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, TestCfgMetadataPath);

            Assert.Null(rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_GetLuigiHeaderFromNullRom_ReturnsNull()
        {
            RomTestStorageAccess.Initialize(null);

            Assert.Null(Rom.GetLuigiHeader(null));
        }

        [Fact]
        public void Rom_GetLuigiHeaderFromLuigiRom_ReturnsHeader()
        {
            RomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.Create(TestLuigiFromBinPath, null);
            Assert.NotNull(rom);

            Assert.NotNull(Rom.GetLuigiHeader(rom));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Rom_GetComparisonIgnoreRangesFromNullRom_ReturnsEmptyEnumerable(bool excludeFeatureBits)
        {
            RomTestStorageAccess.Initialize(null);

            var ignoreRanges = Rom.GetComparisonIgnoreRanges(null, excludeFeatureBits);

            Assert.False(ignoreRanges.Any());
        }

        [Theory]
        [InlineData(TestRomPath, null, true)]
        [InlineData(TestRomPath, null, false)]
        [InlineData(TestBinPath, TestCfgPath, true)]
        [InlineData(TestBinPath, TestCfgPath, false)]
        public void Rom_GetComparisonIgnoreRangesFromNonLuigiRom_ReturnsEmptyEnumerable(string testRomPath, string testCfgPath, bool excludeFeatureBits)
        {
            RomTestStorageAccess.Initialize(testRomPath, testCfgPath);
            var rom = Rom.Create(testRomPath, testCfgPath);
            Assert.NotNull(rom);

            var ignoreRanges = Rom.GetComparisonIgnoreRanges(rom, excludeFeatureBits);

            Assert.False(ignoreRanges.Any());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Rom_GetComparisonIgnoreRangesFromLuigiRom_ReturnsNonEmptyEnumerable(bool excludeFeatureBits)
        {
            RomTestStorageAccess.Initialize(TestLuigiFromBinPath);
            var rom = Rom.Create(TestLuigiFromBinPath, null);
            Assert.NotNull(rom);

            var ignoreRanges = Rom.GetComparisonIgnoreRanges(rom, excludeFeatureBits);

            Assert.True(ignoreRanges.Count() > 0); // Count() forces foreach in Rom.GetComparisonIgnoreRanges to execute
        }

        private class RomTestStorageAccess : CachedResourceStorageAccess<RomTestStorageAccess>
        {
        }
    }
}

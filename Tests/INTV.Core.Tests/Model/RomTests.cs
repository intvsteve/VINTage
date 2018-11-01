﻿// <copyright file="RomTests.cs" company="INTV Funhouse">
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
            RomTestStorageAccess.Initialize(TestRomResources.TestCfgPath);

            var cfgCrc = 0u;
            Assert.Throws<ArgumentNullException>(() => Rom.GetRefreshedCrcs(null, TestRomResources.TestCfgPath, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromBinPathAndNullCfgPath_GetsCorrectBinCrcAndZeroCfgCrc()
        {
            RomTestStorageAccess.Initialize(TestRomResources.TestBinPath);

            var cfgCrc = 0u;
            var crc = Rom.GetRefreshedCrcs(TestRomResources.TestBinPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestBinCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Theory]
        [InlineData(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestBinCrc, TestRomResources.TestCfgCrc)]
        [InlineData(TestRomResources.TestRomPath, TestRomResources.TestCfgPath, TestRomResources.TestRomCrc, 0u)]
        [InlineData(TestRomResources.TestLuigiFromBinPath, null, TestRomResources.TestBinCrc, TestRomResources.TestCfgCrc)]
        [InlineData(TestRomResources.TestRomBadHeaderPath, null, 0u, 0u)]
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
            RomTestStorageAccess.Initialize(TestRomResources.TestCfgPath);

            Rom.ReplaceCfgPath(null, TestRomResources.TestCfgPath);
        }

        [Fact]
        public void Rom_ReplaceCfgWithNullCfgPath_RemovesConfigPath()
        {
            RomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestRomResources.TestBinPath, rom.RomPath);
            Assert.Equal(TestRomResources.TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, null);

            Assert.Null(rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgWithNonexistentCfgPath_UpdatesConfigPath()
        {
            RomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestRomResources.TestBinPath, rom.RomPath);
            Assert.Equal(TestRomResources.TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            var invalidCfgPath = "/Resources/Totes/Bogus/Cfg.cfg";
            Rom.ReplaceCfgPath(rom, invalidCfgPath);

            Assert.Equal(invalidCfgPath, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgWithDifferentValidCfgPath_UpdatesConfigPath()
        {
            RomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestRomResources.TestBinPath, rom.RomPath);
            Assert.Equal(TestRomResources.TestCfgPath, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, TestRomResources.TestCfgMetadataPath);

            Assert.Equal(TestRomResources.TestCfgMetadataPath, rom.ConfigPath);
            Assert.Equal(TestRomResources.TestMetadataCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNonBinFormatRom_HasNoEffect()
        {
            RomTestStorageAccess.Initialize(TestRomResources.TestRomPath, TestRomResources.TestCfgPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(TestRomResources.TestRomPath, TestRomResources.TestCfgPath);
            Assert.NotNull(rom);
            Assert.Equal(TestRomResources.TestRomPath, rom.RomPath);
            Assert.Null(rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, TestRomResources.TestCfgMetadataPath);

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
            RomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.Create(TestRomResources.TestLuigiFromBinPath, null);
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
        [InlineData(TestRomResources.TestRomPath, null, true)]
        [InlineData(TestRomResources.TestRomPath, null, false)]
        [InlineData(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, true)]
        [InlineData(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, false)]
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
            RomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath);
            var rom = Rom.Create(TestRomResources.TestLuigiFromBinPath, null);
            Assert.NotNull(rom);

            var ignoreRanges = Rom.GetComparisonIgnoreRanges(rom, excludeFeatureBits);

            Assert.True(ignoreRanges.Count() > 0); // Count() forces foreach in Rom.GetComparisonIgnoreRanges to execute
        }

        private class RomTestStorageAccess : CachedResourceStorageAccess<RomTestStorageAccess>
        {
        }
    }
}

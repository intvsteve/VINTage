// <copyright file="RomTests.cs" company="INTV Funhouse">
// Copyright (c) 2018-2019 All Rights Reserved
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
using INTV.Core.Model;
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomTests
    {
        [Theory]
        [InlineData(TestRomResources.TestRomPath, RomFormat.Intellicart)]
        [InlineData(TestRomResources.TestCc3Path, RomFormat.CuttleCart3)]
        [InlineData(TestRomResources.TestAdvPath, RomFormat.CuttleCart3Advanced)]
        [InlineData(TestRomResources.TestBinPath, RomFormat.Bin)]
        [InlineData(TestRomResources.TestBinPathNoFileExtension, RomFormat.Bin)]
        [InlineData(TestRomResources.TestCfgPath, RomFormat.None)]
        [InlineData(TestRomResources.TestBinMetadataPath, RomFormat.Bin)]
        [InlineData(TestRomResources.TestLuigiFromBinPath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiFromRomPath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiScrambledForAnyDevicePath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice0Path, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestRomCorruptedPath, RomFormat.Rom)]
        public void RomFormatRom_CheckFormatFromStream_RomFormatIdentifiedCorrectly(string testRomResource, RomFormat expectedRomFormat)
        {
            using (var romData = TestRomResources.OpenResourceStream(testRomResource))
            {
                Assert.NotNull(romData);
                Assert.Equal(expectedRomFormat, Rom.GetFormat(romData));
            }
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromNullPaths_ThrowsAgumentNullException()
        {
            var storage = RomTestStorageAccess.Initialize();

            var cfgCrc = 0u;
            Assert.Throws<ArgumentNullException>(() => Rom.GetRefreshedCrcs(storage.NullLocation, storage.NullLocation, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromInvalidPaths_ThrowsInvalidOperationException()
        {
            RomTestStorageAccess.Initialize();

            var cfgCrc = 0u;
            Assert.Throws<InvalidOperationException>(() => Rom.GetRefreshedCrcs(StorageLocation.InvalidLocation, StorageLocation.InvalidLocation, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromNullBinPathAndValidCfgPath_ThrowsAgumentNullException()
        {
            IReadOnlyList<StorageLocation> paths;
            var storage = RomTestStorageAccess.Initialize(out paths, TestRomResources.TestCfgPath);
            var cfgPath = paths.First();

            var cfgCrc = 0u;
            Assert.Throws<ArgumentNullException>(() => Rom.GetRefreshedCrcs(storage.NullLocation, cfgPath, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromInvalidBinPathAndValidCfgPath_ThrowsInvalidOperationException()
        {
            var cfgPath = RomTestStorageAccess.Initialize(TestRomResources.TestCfgPath).First();

            var cfgCrc = 0u;
            Assert.Throws<InvalidOperationException>(() => Rom.GetRefreshedCrcs(StorageLocation.InvalidLocation, cfgPath, out cfgCrc));
        }

        [Fact]
        public void Rom_GetRefreshedCrcsFromBinPathAndNullCfgPath_GetsCorrectBinCrcAndZeroCfgCrc()
        {
            var romPath = RomTestStorageAccess.Initialize(TestRomResources.TestBinPath).First();

            var cfgCrc = 0u;
            var crc = Rom.GetRefreshedCrcs(romPath, StorageLocation.InvalidLocation, out cfgCrc);

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
            var paths = RomTestStorageAccess.Initialize(testRomPath, testCfgPath);
            var romPath = paths[0];
            var cfgPath = testCfgPath == null ? StorageLocation.InvalidLocation : paths[1];

            var cfgCrc = 0u;
            var crc = Rom.GetRefreshedCrcs(romPath, cfgPath, out cfgCrc);

            Assert.Equal(expectedRomCrc, crc);
            Assert.Equal(expectedCfgCrc, cfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNullRomWithNullCfgPath_DoesNotThrow()
        {
            RomTestStorageAccess.Initialize();

            Rom.ReplaceCfgPath(null, StorageLocation.InvalidLocation);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNullRomWithValidCfgPath_DoesNotThrow()
        {
            var cfgPath = RomTestStorageAccess.Initialize(TestRomResources.TestCfgPath).First();

            Rom.ReplaceCfgPath(null, cfgPath);
        }

        [Fact]
        public void Rom_ReplaceCfgWithNullCfgPath_RemovesConfigPath()
        {
            var paths = RomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.Equal(paths[1], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, StorageLocation.InvalidLocation);

            Assert.False(rom.ConfigPath.IsValid);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgWithNonexistentCfgPath_UpdatesConfigPath()
        {
            IReadOnlyList<StorageLocation> paths;
            var storage = RomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.Equal(paths[1], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            var invalidCfgPath = storage.CreateLocation("/Resources/Totes/Bogus/Cfg.cfg");
            Rom.ReplaceCfgPath(rom, invalidCfgPath);

            Assert.Equal(invalidCfgPath, rom.ConfigPath);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgWithDifferentValidCfgPath_UpdatesConfigPath()
        {
            var paths = RomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.Equal(paths[1], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, paths[2]);

            Assert.Equal(paths[2], rom.ConfigPath);
            Assert.Equal(TestRomResources.TestMetadataCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void Rom_ReplaceCfgOnNonBinFormatRom_HasNoEffect()
        {
            var paths = RomTestStorageAccess.Initialize(TestRomResources.TestRomPath, TestRomResources.TestCfgPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);
            Assert.Equal(paths[0], rom.RomPath);
            Assert.False(rom.ConfigPath.IsValid);
            Assert.Equal(0u, rom.CfgCrc);

            Rom.ReplaceCfgPath(rom, paths[2]);

            Assert.False(rom.ConfigPath.IsValid);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void Rom_GetLuigiHeaderFromNullRom_ReturnsNull()
        {
            RomTestStorageAccess.Initialize();

            Assert.Null(Rom.GetLuigiHeader(null));
        }

        [Fact]
        public void Rom_GetLuigiHeaderFromLuigiRom_ReturnsHeader()
        {
            var romPath = RomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.Create(romPath, StorageLocation.InvalidLocation);
            Assert.NotNull(rom);

            Assert.NotNull(Rom.GetLuigiHeader(rom));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Rom_GetComparisonIgnoreRangesFromNullRom_ReturnsEmptyEnumerable(bool excludeFeatureBits)
        {
            RomTestStorageAccess.Initialize();

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
            var paths = RomTestStorageAccess.Initialize(testRomPath, testCfgPath);
            var romPath = paths[0];
            var cfgPath = testCfgPath == null ? StorageLocation.InvalidLocation : paths[1];
            var rom = Rom.Create(romPath, cfgPath);
            Assert.NotNull(rom);

            var ignoreRanges = Rom.GetComparisonIgnoreRanges(rom, excludeFeatureBits);

            Assert.False(ignoreRanges.Any());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Rom_GetComparisonIgnoreRangesFromLuigiRom_ReturnsNonEmptyEnumerable(bool excludeFeatureBits)
        {
            var romPath = RomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.Create(romPath, StorageLocation.InvalidLocation);
            Assert.NotNull(rom);

            var ignoreRanges = Rom.GetComparisonIgnoreRanges(rom, excludeFeatureBits);

            Assert.True(ignoreRanges.Count() > 0); // Count() forces foreach in Rom.GetComparisonIgnoreRanges to execute
        }

        [Fact]
        public void Rom_CheckInvalidRom_BehavesAsInvalidRom()
        {
            var invalidRom = Rom.InvalidRom;
            bool changed;

            Assert.Equal(RomFormat.None, invalidRom.Format);
            Assert.True(invalidRom.RomPath.IsInvalid);
            Assert.True(invalidRom.ConfigPath.IsInvalid);
            Assert.False(invalidRom.IsValid);
            Assert.Equal(0u, invalidRom.Crc);
            Assert.Equal(0u, invalidRom.CfgCrc);
            Assert.Throws<InvalidOperationException>(() => invalidRom.Validate());
            Assert.Throws<InvalidOperationException>(() => invalidRom.RefreshCrc(out changed));
            Assert.Throws<InvalidOperationException>(() => invalidRom.RefreshCfgCrc(out changed));
        }

        private class RomTestStorageAccess : CachedResourceStorageAccess<RomTestStorageAccess>
        {
        }
    }
}

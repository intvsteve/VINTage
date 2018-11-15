// <copyright file="BinFormatRomTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class BinFormatRomTests
    {
        [Fact]
        public void BinFormatRom_LoadAndValidateRom_RomFormatIdentifiedCorrectly()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithCustomExtension_RomFormatIdentifiedCorrectly()
        {
            var customExtension = ".itv";
            ProgramFileKind.Rom.AddCustomExtension(customExtension);
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestItvPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());

            ProgramFileKind.Rom.RemoveCustomExtension(customExtension);
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithNoFileExtension_RomFormatIdentifiedCorrectly()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPathNoFileExtension, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithOddFileSize_RomFormatIdentifiedCorrectly()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestIntPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithoutCfgPath_RomFormatIdentifiedCorrectly()
        {
            var path = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath).First();
            var rom = Rom.Create(path, null);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_Load_VerifyCrc()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void BinFormatRom_RefreshCfgCrc_NeverRefreshes()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            var changed = true;
            Assert.Equal(TestRomResources.TestCfgCrc, rom.RefreshCfgCrc(out changed));
            Assert.False(changed);
        }

        [Fact]
        public void BinFormatRom_ReplaceCfgPath_CfgPathChanges()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);

            var fakeCfgPath = "/Resources/tugalong.cfg";
            ((BinFormatRom)rom).ReplaceCfgPath(fakeCfgPath);

            Assert.Equal(fakeCfgPath, rom.ConfigPath);
        }

        [Fact]
        public void BinFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(paths[0], paths[1]);

            var metadata = rom.GetBinFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void BinFormatRom_GetMetadataFromCorruptCfgFile_DoesNotThrow()
        {
            var paths = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgCorruptMetadataPath);
            var rom = Rom.Create(paths[0], paths[1]);

            var metadata = rom.GetBinFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void BinFormatRom_GetMetadataFromCorruptCfgFile_ThrowsNullReferenceException()
        {
            IReadOnlyList<string> paths;
            var storageAccess = BinFormatRomTestStorageAccess.Initialize(out paths, TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgBadMetadataPath);
            var rom = Rom.Create(paths[0], paths[1]);

            var corrupted = storageAccess.IntroduceCorruption(paths[1]);

            Assert.True(corrupted);
            Assert.Throws<NullReferenceException>(() => rom.GetBinFileMetadata());
        }

        private class BinFormatRomTestStorageAccess : CachedResourceStorageAccess<BinFormatRomTestStorageAccess>
        {
        }
    }
}

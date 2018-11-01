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
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithCustomExtension_RomFormatIdentifiedCorrectly()
        {
            var customExtension = ".itv";
            ProgramFileKind.Rom.AddCustomExtension(customExtension);
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestItvPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestItvPath, TestRomResources.TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());

            ProgramFileKind.Rom.RemoveCustomExtension(customExtension);
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithNoFileExtension_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPathNoFileExtension, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPathNoFileExtension, TestRomResources.TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithOddFileSize_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestIntPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestIntPath, TestRomResources.TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithoutCfgPath_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, null);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_Load_VerifyCrc()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void BinFormatRom_RefreshCfgCrc_NeverRefreshes()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            var changed = true;
            Assert.Equal(TestRomResources.TestCfgCrc, rom.RefreshCfgCrc(out changed));
            Assert.False(changed);
        }

        [Fact]
        public void BinFormatRom_ReplaceCfgPath_CfgPathChanges()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);

            var fakeCfgPath = "/Resources/tugalong.cfg";
            ((BinFormatRom)rom).ReplaceCfgPath(fakeCfgPath);

            Assert.Equal(fakeCfgPath, rom.ConfigPath);
        }

        [Fact]
        public void BinFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgMetadataPath);

            var metadata = rom.GetBinFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void BinFormatRom_GetMetadataFromCorruptCfgFile_DoesNotThrow()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgCorruptMetadataPath);
            var rom = Rom.Create(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgCorruptMetadataPath);

            var metadata = rom.GetBinFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void BinFormatRom_GetMetadataFromCorruptCfgFile_ThrowsNullReferenceException()
        {
            var storageAccess = BinFormatRomTestStorageAccess.Initialize(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgBadMetadataPath);
            var rom = Rom.Create(TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgBadMetadataPath);

            var corrupted = storageAccess.IntroduceCorruption(TestRomResources.TestCfgBadMetadataPath);

            Assert.True(corrupted);
            Assert.Throws<NullReferenceException>(() => rom.GetBinFileMetadata());
        }

        private class BinFormatRomTestStorageAccess : CachedResourceStorageAccess<BinFormatRomTestStorageAccess>
        {
        }
    }
}

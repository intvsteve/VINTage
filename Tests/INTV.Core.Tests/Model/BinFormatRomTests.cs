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
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class BinFormatRomTests
    {
        private const string TestBinPath = "/Resources/tagalong.bin";
        private const string TestCfgPath = "/Resources/tagalong.cfg";
        private const string TestIntPath = "/Resources/tagalong.int"; // NOTE: has odd number of bytes
        private const string TestItvPath = "/Resources/tagalong.itv";
        private const string TestMacPath = "/Resources/tagalong"; // Intellivision Lives! for Mac did not use file extensions on ROMs

        private const string TestRomMetadataPath = "/Resources/tagalong_metadata.bin";
        private const string TestCfgMetadataPath = "/Resources/tagalong_metadata.cfg";
        private const string TestCfgMetadataBadPath = "/Resources/tagalong_metadata_bad.cfg";
        private const string TestCorruptCfgMetadataPath = "/Resources/tagalong_metadata_corrupt.cfg";

        private const uint TestRomCrc = 0xECBA3AF7;
        private const uint TestCfgCrc = 0x06B5EA3E;

        [Fact]
        public void BinFormatRom_LoadAndValidateRom_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithCustomExtension_RomFormatIdentifiedCorrectly()
        {
            var customExtension = ".itv";
            ProgramFileKind.Rom.AddCustomExtension(customExtension);
            BinFormatRomTestStorageAccess.Initialize(TestItvPath, TestCfgPath);
            var rom = Rom.Create(TestItvPath, TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());

            ProgramFileKind.Rom.RemoveCustomExtension(customExtension);
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithNoFileExtension_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestMacPath, TestCfgPath);
            var rom = Rom.Create(TestMacPath, TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithOddFileSize_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestIntPath, TestCfgPath);
            var rom = Rom.Create(TestIntPath, TestCfgPath);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_LoadAndValidateRomWithoutCfgPath_RomFormatIdentifiedCorrectly()
        {
            BinFormatRomTestStorageAccess.Initialize(TestBinPath);
            var rom = Rom.Create(TestBinPath, null);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Bin, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void BinFormatRom_Load_VerifyCrc()
        {
            BinFormatRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);

            Assert.Equal(TestRomCrc, rom.Crc);
            Assert.Equal(TestCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void BinFormatRom_RefreshCfgCrc_NeverRefreshes()
        {
            BinFormatRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);

            var changed = true;
            Assert.Equal(TestCfgCrc, rom.RefreshCfgCrc(out changed));
            Assert.False(changed);
        }

        [Fact]
        public void BinFormatRom_ReplaceCfgPath_CfgPathChanges()
        {
            BinFormatRomTestStorageAccess.Initialize(TestBinPath, TestCfgPath);
            var rom = Rom.Create(TestBinPath, TestCfgPath);

            var fakeCfgPath = "/Resources/tugalong.cfg";
            ((BinFormatRom)rom).ReplaceCfgPath(fakeCfgPath);

            Assert.Equal(fakeCfgPath, rom.ConfigPath);
        }

        [Fact]
        public void BinFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomMetadataPath, TestCfgMetadataPath);
            var rom = Rom.Create(TestRomMetadataPath, TestCfgMetadataPath);

            var metadata = rom.GetBinFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void BinFormatRom_GetMetadataFromCorruptCfgFile_DoesNotThrow()
        {
            BinFormatRomTestStorageAccess.Initialize(TestRomMetadataPath, TestCorruptCfgMetadataPath);
            var rom = Rom.Create(TestRomMetadataPath, TestCorruptCfgMetadataPath);

            var metadata = rom.GetBinFileMetadata();

            Assert.NotNull(metadata);
        }

        [Fact]
        public void BinFormatRom_GetMetadataFromCorruptCfgFile_ThrowsNullReferenceException()
        {
            var storageAccess = BinFormatRomTestStorageAccess.Initialize(TestRomMetadataPath, TestCfgMetadataBadPath);
            var rom = Rom.Create(TestRomMetadataPath, TestCfgMetadataBadPath);

            var corrupted = storageAccess.IntroduceCorruption(TestCfgMetadataBadPath);

            Assert.True(corrupted);
            Assert.Throws<NullReferenceException>(() => rom.GetBinFileMetadata());
        }

        private class BinFormatRomTestStorageAccess : CachedResourceStorageAccess<BinFormatRomTestStorageAccess>
        {
        }
    }
}

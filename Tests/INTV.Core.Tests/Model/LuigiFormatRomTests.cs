// <copyright file="LuigiFormatRomTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiFormatRomTests
    {
        [Theory]
        [InlineData(TestRomResources.TestRomPath, RomFormat.None)]
        [InlineData(TestRomResources.TestAdvPath, RomFormat.None)]
        [InlineData(TestRomResources.TestBinMetadataPath, RomFormat.None)]
        [InlineData(TestRomResources.TestLuigiFromBinPath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiFromRomPath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiScrambledForAnyDevicePath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath, RomFormat.Luigi)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice0Path, RomFormat.Luigi)]
        public void LuigiFormatRom_CheckFormatFromStream_RomFormatIdentifiedCorrectly(string testRomResource, RomFormat expectedRomFormat)
        {
            using (var romData = TestRomResources.OpenResourceStream(testRomResource))
            {
                Assert.NotNull(romData);
                Assert.Equal(expectedRomFormat, LuigiFormatRom.CheckFormat(romData));
            }
        }

        [Fact]
        public void LuigiFormatRom_LoadAndValidateRom_RomFormatIdentifiedCorrectly()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.Create(romPath, null);

            Assert.NotNull(rom);
            Assert.Equal(RomFormat.Luigi, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void LuigiFormatRom_Load_VerifyCrc()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.Create(romPath, null);

            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);
        }

        [Fact]
        public void LuigiFormatRomFromFuture_Load_DoesNotLoadAsLuigiFormatRom()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromFuturePath).First();
            var rom = Rom.Create(romPath, null);

            // At this time, the ROM *just happens* to pass the .bin format tests! Depending on
            // many factors, differently "fake corrupted" ROMs could fail to load or not.
            // This test is OK with a null or .bin result.
            Assert.True((rom == null) || (rom.Format == RomFormat.Bin));
        }

        [Fact]
        public void LuigiFormatRom_Load_VerifyCrc24()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(romPath, null));

            Assert.Equal(0x0035f671u, rom.Crc24);
        }

        [Fact]
        public void LuigiFormatRom_LoadStandardLuigi_VerifyTargetDeviceUniqueId()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(romPath, null));

            Assert.True(string.IsNullOrEmpty(rom.TargetDeviceUniqueId));
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledForAnyLuigi_VerifyTargetDeviceUniqueId()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForAnyDevicePath).First();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(romPath, null));

            Assert.Equal(LuigiScrambleKeyBlock.AnyLTOFlashId, rom.TargetDeviceUniqueId);
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledLuigis_VerifyTargetDeviceUniqueIds()
        {
            var paths = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path, TestRomResources.TestLuigiScrambledForDevice1Path);
            var rom0 = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(paths[0], null));
            var rom1 = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(paths[1], null));

            Assert.NotEqual(rom0.TargetDeviceUniqueId, rom1.TargetDeviceUniqueId);
            Assert.Equal(TestRomResources.TestLuigiScrambledForDevice0UniqueId, rom0.TargetDeviceUniqueId);
            Assert.Equal(TestRomResources.TestLuigiScrambledForDevice1UniqueId, rom1.TargetDeviceUniqueId);
        }

        [Fact]
        public void LuigiFormatRom_LoadLuigiWithZeroCrcs_VerifyCrc()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithZeroCrcsPath).First();
            var rom = Rom.Create(romPath, null);

            Assert.Equal(TestRomResources.TestLuigiWithZeroCrcsCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromBinOrigin_CrcsMatch()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(romPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestBinCrc, crc);
            Assert.Equal(TestRomResources.TestCfgCrc, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromRomOrigin_CrcsMatch()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromRomPath).First();

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(romPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestRomCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetCrcsFromFileWithZeroCrcs_CrcMatchesCrcOfFile()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithZeroCrcsPath).First();

            uint cfgCrc;
            var crc = LuigiFormatRom.GetCrcs(romPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestLuigiWithZeroCrcsCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void LuigiFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithMetadataPath).First();

            var rom = Rom.Create(romPath, null);
            var metadata = rom.GetLuigiFileMetadata();

            Assert.NotNull(metadata);
            VerifyExpectedMetadata(metadata);
        }

        [Fact]
        public void LuigiFormatRom_CheckFormatOfCorruptFile_ThrowsNullReferenceException()
        {
            IReadOnlyList<string> paths;
            var storageAccess = LuigiFormatRomTestStorageAccess.Initialize(out paths, TestRomResources.TestLuigiWithBadHeaderPath);

            var corrupted = storageAccess.IntroduceCorruption(paths.First());

            Assert.True(corrupted);
            Assert.Throws<NullReferenceException>(() => LuigiFormatRom.CheckFormat(paths.First()));
        }

        [Theory]
        [InlineData(true, 3)]
        [InlineData(false, 2)]
        public void LuigiFormatRom_GetComparisonIgnoreRanges_ReturnsCorrectNumberOfExcludeRanges(bool excludeFeatureBits, int expectedNumberOfExcludeRanges)
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(romPath, null));

            // Three ranges of values are ignored in this case:
            // 1. Feature flags (depends on value of excludeFeatureBits)
            // 2. The UID (which for this specific test ROM is the CRC32 of the .BIN and the CRC32 of the .CFG of the original ROM
            // 3. The CRC of the header
            var rangesToIgnore = rom.GetComparisonIgnoreRanges(excludeFeatureBits);

            Assert.Equal(expectedNumberOfExcludeRanges, rangesToIgnore.Count());
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledForAnyLuigiWithoutMetadataRequestMetadata_VerifyNoMetadataPresent()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForAnyDevicePath).First();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(romPath, null));

            Assert.Null(rom.LocateDataBlock<LuigiMetadataBlock>());
        }

        [Fact]
        public void LuigiFormatRom_LoadScrambledForAnyLuigiWithMetadataRequestTestBlock_VerifyTestBlockNotFound()
        {
            var romPath = LuigiFormatRomTestStorageAccess.Initialize(TestRomResources.TestLuigiWithMetadatdaScrambledForAnyDevicePath).First();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(romPath, null));

            Assert.Null(rom.LocateDataBlock<LuigiTestDataBlock>());
        }

        private void VerifyExpectedMetadata(IProgramMetadata metadata)
        {
            var expectedLongNames = new[] { "Tag-A-Long Todd" };
            Assert.Empty(expectedLongNames.Except(metadata.LongNames, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.LongNames.Except(expectedLongNames, StringComparer.OrdinalIgnoreCase));

            var expectedShortNames = new[] { "Tod" };
            Assert.Empty(expectedShortNames.Except(metadata.ShortNames, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.ShortNames.Except(expectedShortNames, StringComparer.OrdinalIgnoreCase));

            var expectedDescriptions = new[] { "Desc 3", "desc iv", string.Empty, "Description the first.", "Description the Second" };
            Assert.Empty(expectedDescriptions.Except(metadata.Descriptions, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Descriptions.Except(expectedDescriptions, StringComparer.OrdinalIgnoreCase));

            var expectedPublishers = new[] { "Zbiciak Electronics", "Left Turn Only" };
            Assert.Empty(expectedPublishers.Except(metadata.Publishers, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Publishers.Except(expectedPublishers, StringComparer.OrdinalIgnoreCase));

            var expectedProgrammers = new[] { "Joe Zbiciak", "intvnut", "JRMZ" };
            Assert.Empty(expectedProgrammers.Except(metadata.Programmers, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Programmers.Except(expectedProgrammers, StringComparer.OrdinalIgnoreCase));

            var expectedDesigners = new[] { "Unsure", "Unsurely - don't call me surely." };
            Assert.Empty(expectedDesigners.Except(metadata.Designers, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Designers.Except(expectedDesigners, StringComparer.OrdinalIgnoreCase));

            var expectedGraphics = new[] { "JZ", "No, not that JZ" };
            Assert.Empty(expectedGraphics.Except(metadata.Graphics, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Graphics.Except(expectedGraphics, StringComparer.OrdinalIgnoreCase));

            var expectedMusic = new[] { "Joe", "Wait, there's music?" };
            Assert.Empty(expectedMusic.Except(metadata.Music, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Music.Except(expectedMusic, StringComparer.OrdinalIgnoreCase));

            var expectedSoundEffects = new[] { "Joe Zbiciak", "Joseph Zbiciak" };
            Assert.Empty(expectedSoundEffects.Except(metadata.SoundEffects, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.SoundEffects.Except(expectedSoundEffects, StringComparer.OrdinalIgnoreCase));

            var expectedVoices = new[] { "None", "Really, none" };
            Assert.Empty(expectedVoices.Except(metadata.Voices, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Voices.Except(expectedVoices, StringComparer.OrdinalIgnoreCase));

            var expectedDocumentation = new[] { "JZ", "IM14U2C" };
            Assert.Empty(expectedDocumentation.Except(metadata.Documentation, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Documentation.Except(expectedDocumentation, StringComparer.OrdinalIgnoreCase));

            var expectedArtwork = new[] { "N/A", "Boxless ... for now" };
            Assert.Empty(expectedArtwork.Except(metadata.Artwork, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Artwork.Except(expectedArtwork, StringComparer.OrdinalIgnoreCase));

            var date0 = new MetadataDateTimeBuilder(1999).WithMonth(9).WithDay(9).Build();
            var date1 = new MetadataDateTimeBuilder(2001).WithMonth(1).WithDay(1).Build();
            var date2 = new MetadataDateTimeBuilder(1997).WithMonth(5).WithDay(17).WithHour(0).WithMinute(29).WithSecond(24).WithOffset(-7, -12).Build();
            var date3 = new MetadataDateTimeBuilder(1999).WithMonth(10).WithDay(2).WithHour(5).WithMinute(16).WithSecond(18).WithOffset(7, 0).Build();
            var expectedReleaseDates = new[] { date0, date1, date2, date3 };
            Assert.Empty(expectedReleaseDates.Except(metadata.ReleaseDates));
            Assert.Empty(metadata.ReleaseDates.Except(expectedReleaseDates));

            var expectedLicenses = new[] { "PD", "Not GPL" };
            Assert.Empty(expectedLicenses.Except(metadata.Licenses, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Licenses.Except(expectedLicenses, StringComparer.OrdinalIgnoreCase));

            var expectedContactInformation = new[] { "spatula-city.org", "leftturnonly.info" };
            Assert.Empty(expectedContactInformation.Except(metadata.ContactInformation, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.ContactInformation.Except(expectedContactInformation, StringComparer.OrdinalIgnoreCase));

            // Both 'version' and 'versions' are parsed
            var expectedVersions = new[] { "Tutorial", "Learning stuff", "Tutorial 2", "Learning stuff" };
            Assert.Empty(expectedVersions.Except(metadata.Versions, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.Versions.Except(expectedVersions, StringComparer.OrdinalIgnoreCase));

            date0 = new MetadataDateTimeBuilder(2013).WithMonth(10).WithDay(31).WithHour(17).WithMinute(57).WithSecond(4).WithOffset(-7, 0).Build();
            date1 = new MetadataDateTimeBuilder(2013).WithMonth(10).WithDay(31).WithHour(17).WithMinute(57).WithSecond(4).WithOffset(0, -18).Build();
            date2 = new MetadataDateTimeBuilder(2013).WithMonth(10).WithDay(31).WithHour(17).WithMinute(57).WithSecond(60).WithOffset(7, 42).Build();
            date3 = new MetadataDateTimeBuilder(2013).WithMonth(10).WithDay(31).WithHour(17).WithMinute(57).WithSecond(60).WithOffset(0, 55).Build();
            var expectedBuildDates = new[] { date0, date1, date2, date3 };
            Assert.Empty(metadata.BuildDates.Except(expectedBuildDates));
            Assert.Empty(expectedBuildDates.Except(metadata.BuildDates));

            var expectedAdditionalInformation = Enumerable.Empty<string>();
            Assert.Empty(expectedAdditionalInformation.Except(metadata.AdditionalInformation, StringComparer.OrdinalIgnoreCase));
            Assert.Empty(metadata.AdditionalInformation.Except(expectedAdditionalInformation, StringComparer.OrdinalIgnoreCase));
        }

        private class LuigiFormatRomTestStorageAccess : CachedResourceStorageAccess<LuigiFormatRomTestStorageAccess>
        {
        }

        private class LuigiTestDataBlock : LuigiDataBlock
        {
            public const LuigiDataBlockType BlockType = (LuigiDataBlockType)0xFD;

            public LuigiTestDataBlock()
                : base(BlockType)
            {
            }
        }
    }
}

// <copyright file="RomFormatRomTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomFormatRomTests
    {
        [Theory]
        [InlineData(TestRomResources.TestRomPath, RomFormat.Intellicart)]
        [InlineData(TestRomResources.TestCc3Path, RomFormat.CuttleCart3)]
        [InlineData(TestRomResources.TestAdvPath, RomFormat.CuttleCart3Advanced)]
        public void RomFormatRom_LoadAndValidateRom_RomFormatIdentifiedCorrectly(string testRomPath, RomFormat expectedRomFormat)
        {
            RomFormatRomTestStorageAccess.Initialize(testRomPath);
            var rom = Rom.Create(testRomPath, null);

            Assert.NotNull(rom);
            Assert.Equal(expectedRomFormat, rom.Format);
            Assert.True(rom.Validate());
        }

        [Fact]
        public void RomFormatRom_LoadCorruptedRom_ReturnsNullRom()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomBadHeaderPath);
            var rom = Rom.Create(TestRomResources.TestRomBadHeaderPath, null);

            Assert.Null(rom);
        }

        [Fact]
        public void RomFormatRom_Load_VerifyCrc()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath);
            var rom = Rom.Create(TestRomResources.TestRomPath, null);

            Assert.Equal(TestRomResources.TestRomCrc, rom.Crc);
            Assert.Equal(0u, rom.CfgCrc);
        }

        [Fact]
        public void RomFormatRom_RefreshCfgCrc_NeverRefreshes()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath);
            var rom = Rom.Create(TestRomResources.TestRomPath, null);

            var changed = true;
            Assert.Equal(0u, rom.RefreshCfgCrc(out changed));
            Assert.False(changed);
        }

        [Fact]
        public void RomFormatRom_GetCrcs_ReturnsCorrectCrcs()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath);

            uint cfgCrc;
            var crc = RomFormatRom.GetCrcs(TestRomResources.TestRomPath, null, out cfgCrc);

            Assert.Equal(TestRomResources.TestRomCrc, crc);
            Assert.Equal(0u, cfgCrc);
        }

        [Fact]
        public void RomFormatRom_GetMetadata_ReturnsNullMetadata()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomPath);

            var rom = Rom.Create(TestRomResources.TestRomPath, null);
            var metadata = rom.GetRomFileMetadata();

            Assert.Null(metadata);
        }

        [Fact]
        public void RomFormatRom_GetMetadataFromCorruptedRom_BehavesAsExpected()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomCorruptedPath);

            var rom = Rom.Create(TestRomResources.TestRomCorruptedPath, null);
#if DEBUG
            Assert.Throws<InvalidOperationException>(() => rom.GetRomFileMetadata());
#else
            var metadata = rom.GetRomFileMetadata();

            Assert.Null(metadata);
#endif
        }

        [Fact]
        public void RomFormatRom_GetMetadata_ReturnsExpectedMetadata()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomMetadataPath);

            var rom = Rom.Create(TestRomResources.TestRomMetadataPath, null);
            var metadata = rom.GetRomFileMetadata();

            Assert.NotNull(metadata);
            VerifyExpectedMetadata(metadata, lastVersionMetadataIsCorrupt: false);
        }

        [Fact]
        public void RomFormatRom_GetMetadataWithBadCrc_ReturnsExpectedMetadata()
        {
            RomFormatRomTestStorageAccess.Initialize(TestRomResources.TestRomMetadataBadCrcPath);

            var rom = Rom.Create(TestRomResources.TestRomMetadataBadCrcPath, null);
            var metadata = rom.GetRomFileMetadata();

            Assert.NotNull(metadata);
            VerifyExpectedMetadata(metadata, lastVersionMetadataIsCorrupt: true);
        }

        private void VerifyExpectedMetadata(IProgramMetadata metadata, bool lastVersionMetadataIsCorrupt)
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

            var expectedVersions = new[] { "Tutorial", "Learning stuff" };
            if (lastVersionMetadataIsCorrupt)
            {
                Assert.NotEmpty(expectedVersions.Except(metadata.Versions, StringComparer.OrdinalIgnoreCase));
                Assert.Empty(metadata.Versions.Except(expectedVersions, StringComparer.OrdinalIgnoreCase));
            }
            else
            {
                Assert.Empty(expectedVersions.Except(metadata.Versions, StringComparer.OrdinalIgnoreCase));
                Assert.Empty(metadata.Versions.Except(expectedVersions, StringComparer.OrdinalIgnoreCase));
            }

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

        private class RomFormatRomTestStorageAccess : CachedResourceStorageAccess<RomFormatRomTestStorageAccess>
        {
        }
    }
}

// <copyright file="IntvFunhouseXmlProgramInformationTests.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Core.Resources;
using INTV.Core.Restricted.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class IntvFunhouseXmlProgramInformationTests
    {
        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetTitle_HasNoEffect()
        {
            var title = "Intellivision spoken here";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramTitle = title };

            info.Title = "did it change?";

            Assert.Equal(title, info.Title);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetShortName_HasNoEffect()
        {
            var title = "No short names";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramTitle = title };

            info.ShortName = "did it change?";

            Assert.Null(info.ShortName);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_WithProgramVendorAndOriginalProgramVendor_VendorReturnsOriginalProgramVendorAsVendor()
        {
            var programVendor = "Swiper";
            var originalVendor = "No Swiping!";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramVendor = programVendor, OriginalProgramVendor = originalVendor };

            Assert.Equal(originalVendor, info.Vendor);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetVendor_HasNoEffect()
        {
            var programVendor = "Scrawny Brawny";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramVendor = programVendor };

            info.Vendor = "Me! Me! Mi!";

            Assert.Equal(programVendor, info.Vendor);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetYear_UpdatesYear()
        {
            var info = new IntvFunhouseXmlProgramInformation() { YearString = "1988" };

            var newYear = "1999";
            info.Year = newYear;

            Assert.Equal(newYear, info.Year);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetValidJLPSaveData_FeaturesContainCorrectJlpSaveDataSectorCount()
        {
            ushort saveDataSectorCount = 123;
            var info = new IntvFunhouseXmlProgramInformation() { JLPSaveData = saveDataSectorCount.ToString(CultureInfo.InvariantCulture) };

            var features = info.Features;

            Assert.NotNull(features);
            Assert.Equal(saveDataSectorCount, features.JlpFlashMinimumSaveSectors);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetOutOfRangeJLPSaveData_FeaturesContainZeroJlpSaveDataSectorCount()
        {
            ushort saveDataSectorCount = JlpFeaturesHelpers.MaxTheoreticalJlpFlashSectorUsage + 1;
            var info = new IntvFunhouseXmlProgramInformation() { JLPSaveData = saveDataSectorCount.ToString(CultureInfo.InvariantCulture) };

            var features = info.Features;

            Assert.NotNull(features);
            Assert.Equal(0, features.JlpFlashMinimumSaveSectors);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetOverflowSizedJLPSaveData_FeaturesContainZeroJlpSaveDataSectorCount()
        {
            uint saveDataSectorCount = (uint)ushort.MaxValue + 1;
            var info = new IntvFunhouseXmlProgramInformation() { JLPSaveData = saveDataSectorCount.ToString(CultureInfo.InvariantCulture) };

            var features = info.Features;

            Assert.NotNull(features);
            Assert.Equal(0, features.JlpFlashMinimumSaveSectors);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_SetFeatures_UpdatesFeatures()
        {
            var info = new IntvFunhouseXmlProgramInformation();
            var originalFeatures = info.Features;

            info.Features = ProgramFeatures.GetUnrecognizedRomFeatures();
            var newFeatures = info.Features;

            Assert.NotNull(originalFeatures);
            Assert.NotNull(newFeatures);
            Assert.NotEqual(newFeatures, originalFeatures);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithNullCrcString_ThrowsNullReferenceException()
        {
            var info = new IntvFunhouseXmlProgramInformation();
            Assert.True(string.IsNullOrEmpty(info.CrcString));

            Assert.Throws<NullReferenceException>(() => info.Crcs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithNullCrcNotesString_ThrowsNullReferenceException()
        {
            var info = new IntvFunhouseXmlProgramInformation() { CrcString = string.Empty };
            Assert.True(string.IsNullOrEmpty(info.CrcNotesString));

            Assert.Throws<NullReferenceException>(() => info.Crcs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithNullCrcIncompatibilitiesStringString_ThrowsNullReferenceException()
        {
            var info = new IntvFunhouseXmlProgramInformation() { CrcString = string.Empty, CrcNotesString = string.Empty };
            Assert.True(string.IsNullOrEmpty(info.CrcIncompatibilitiesString));

            Assert.Throws<NullReferenceException>(() => info.Crcs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithNullCfgFiles_ThrowsInvalidOperationException()
        {
            var info = new IntvFunhouseXmlProgramInformation()
            {
                CrcString = string.Empty,
                CrcNotesString = string.Empty,
                CrcIncompatibilitiesString = string.Empty
            };
            Assert.True(string.IsNullOrEmpty(info.CfgFiles));

            Assert.Throws<NullReferenceException>(() => info.Crcs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithEmptyCfgFiles_ThrowsFormatException()
        {
            var info = new IntvFunhouseXmlProgramInformation()
            {
                CrcString = string.Empty,
                CrcNotesString = string.Empty,
                CrcIncompatibilitiesString = string.Empty,
                CfgFiles = string.Empty
            };
            Assert.True(string.IsNullOrEmpty(info.CfgFiles));

            Assert.Throws<FormatException>(() => info.Crcs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithOneCrcAndTwoCfgFiles_ThrowsInvalidOperationException()
        {
            var info = new IntvFunhouseXmlProgramInformation()
            {
                CrcString = "0x11223344",
                CrcNotesString = string.Empty,
                CrcIncompatibilitiesString = string.Empty,
                CfgFiles = "0,1"
            };
            Assert.False(string.IsNullOrEmpty(info.CrcString));
            Assert.False(string.IsNullOrEmpty(info.CfgFiles));

            Assert.Throws<InvalidOperationException>(() => info.Crcs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithMalformedCrcIncompatibilitiesString_ReturnsNoIncompatibilities()
        {
            var info = new IntvFunhouseXmlProgramInformation()
            {
                CrcString = "0x99887766",
                CrcNotesString = string.Empty,
                CrcIncompatibilitiesString = "Not a valid number string",
                CfgFiles = "0"
            };
            Assert.False(string.IsNullOrEmpty(info.CrcIncompatibilitiesString));
            Assert.False(string.IsNullOrEmpty(info.CrcString));
            Assert.False(string.IsNullOrEmpty(info.CfgFiles));

            Assert.Equal(IncompatibilityFlags.None, info.Crcs.First().Incompatibilities);
        }

        public static IEnumerable<object[]> AllIncompatibilityFlagsTestData
        {
            get
            {
                var allIncompatibilityFlags = Enum.GetValues(typeof(IncompatibilityFlags)).Cast<IncompatibilityFlags>().Where(f => f != IncompatibilityFlags.None);
                foreach (var incompatibilityFlag in allIncompatibilityFlags)
                {
                    yield return new object[] { incompatibilityFlag };
                }
            }
        }

        [Theory]
        [MemberData("AllIncompatibilityFlagsTestData")]
        public void IntvFunhouseXmlProgramInformation_GetCrcsWithAllCrcIncompatibilities_HasAllKnownIncompatibilities(IncompatibilityFlags expectedIncompatibilityFlag)
        {
            var info = new IntvFunhouseXmlProgramInformation()
            {
                CrcString = "0x22446688",
                CrcNotesString = string.Empty,
                CrcIncompatibilitiesString = "ffffffff",
                CfgFiles = "0"
            };
            Assert.False(string.IsNullOrEmpty(info.CrcIncompatibilitiesString));
            Assert.False(string.IsNullOrEmpty(info.CrcString));
            Assert.False(string.IsNullOrEmpty(info.CfgFiles));

            Assert.True(info.Crcs.First().Incompatibilities.HasFlag(expectedIncompatibilityFlag));
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_AddCrc_ThrowsInvalidOperationException()
        {
            var info = new IntvFunhouseXmlProgramInformation();

            Assert.Throws<InvalidOperationException>(() => info.AddCrc(0, null, IncompatibilityFlags.None));
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_ToString_PrintsProgramTitle()
        {
            var title = "Gimme Shelter";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramTitle = title };

            Assert.Equal(title, info.ToString());
        }

        [Theory]
        [InlineData("Mattel Electronics", "http://intellivisionlives.com/bluesky/games/credits/ecs.shtml#Anchor-Mind-28089")]
        [InlineData("Elektronite", "http://elektronite.net/gallery/credits/ecs.shtml#Anchor-Mind-28089")]
        public void IntvFunhouseXmlProgramInformation_WithVendorAndPartialExternalInfo_ProducesExpectedContactInformation(string vendor, string expectedContactInfo)
        {
            var externalInfo = "credits/ecs.shtml#Anchor-Mind-28089";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramVendor = vendor, ExternalInfo = externalInfo };

            Assert.Equal(expectedContactInfo, info.ContactInformation.First());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_WithProgramAndOriginalVendorsAndPartialExternalInfos_ProducesExpectedContactInformation()
        {
            var externalInfo =
@"        credits/1984b.html#spina
        spina/spina.php";
            var info = new IntvFunhouseXmlProgramInformation() { ProgramVendor = "Intelligentvision", OriginalProgramVendor = "Mattel Electronics", ExternalInfo = externalInfo };

            var expectedContactInfos = new[]
            {
                "http://intellivisionlives.com/bluesky/games/credits/1984b.html#spina",
                "http://intellivision.us/intvgames/spina/spina.php"
            };
            Assert.Equal(expectedContactInfos, info.ContactInformation);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_WithAbsoluteExternalInfo_ProducesExpectedContactInformation()
        {
            var externalInfo = "https://intvfunhouse.com/games/math.php";
            var info = new IntvFunhouseXmlProgramInformation() { ExternalInfo = externalInfo };

            Assert.Equal(externalInfo, info.ContactInformation.First());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_MultipleMusicContributorEntries_ProducesExpectedMusicContributors()
        {
            var info = new IntvFunhouseXmlProgramInformation() { ProgramMusic = "||A|b|b|a||" };

            var expectedMusic = new[] { "A", "b", "b", "a" };
            Assert.Equal(expectedMusic, info.Music);
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformation_OtherInfoEntriesWithProgramAndOriginalVendors_ProducesExpectedAdditionalInfo()
        {
            var otherInfo =
@" Additional programming and enhancements: David Harley
   Box and overlay design: Oliver Puschatzki";
            var info = new IntvFunhouseXmlProgramInformation()
            {
                ProgramVendor = "Intelligentvision",
                OriginalProgramVendor = "Mattel Electronics",
                OtherInfo = otherInfo
            };

            var expectedAdditionalInformation = new[]
            {
                "Additional programming and enhancements: David Harley",
                "Box and overlay design: Oliver Puschatzki",
                string.Format(CultureInfo.CurrentCulture, Strings.AdditionalVendorInfoWebsite_Format, info.OriginalProgramVendor, "http://intellivisionlives.com/bluesky/games/"),
                string.Format(CultureInfo.CurrentCulture, Strings.AdditionalVendorInfoWebsite_Format, info.ProgramVendor, "http://intellivision.us")
            };
            Assert.Equal(expectedAdditionalInformation, info.AdditionalInformation);
        }
    }
}

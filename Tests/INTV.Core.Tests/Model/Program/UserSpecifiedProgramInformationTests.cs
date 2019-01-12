// <copyright file="UserSpecifiedProgramInformationTests.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class UserSpecifiedProgramInformationTests
    {
        [Fact]
        public void UserSpecifiedProgramInformation_ConstructWithZeroCrc_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UserSpecifiedProgramInformation(0u));
        }

        [Fact]
        public void UserSpecifiedProgramInformation_ConstructWithNullProgramInformation_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UserSpecifiedProgramInformation(null));
        }

        [Fact]
        public void UserSpecifiedProgramInformation_ConstructWithNonzeroCrc_CreatesInformationWithDefaultProgramTitle()
        {
            var information = new UserSpecifiedProgramInformation(1u);

            Assert.Equal(ProgramInformation.UnknownProgramTitle, information.Title);
        }

        public static IEnumerable<object[]> CreateUserSpecifiedInformationWithTitle
        {
            get
            {
                yield return new object[] { null, ProgramInformation.UnknownProgramTitle };
                yield return new object[] { string.Empty, ProgramInformation.UnknownProgramTitle };
                yield return new object[] { "Pukeslie", "Pukeslie" };
                yield return new object[] { "Barfsley", "Barfsley" };
                yield return new object[] { "Biscuit", "Biscuit" };
                yield return new object[] { "Patches RIP", "Patches RIP" };
            }
        }

        [Theory]
        [MemberData("CreateUserSpecifiedInformationWithTitle")]
        public void UserSpecifiedProgramInformation_ConstructWithNonzeroCrc_CreatesInformationWithExpectedProgramTitle(string title, string expectedTitle)
        {
            var information = new UserSpecifiedProgramInformation(1u, title);

            Assert.False(information.IsModified);
            Assert.Equal(expectedTitle, information.Title);
        }

        [Fact]
        public void UserSpecifiedProgramInformation_ConstructWithEmptyNonMetadataProgramInformation_CreatesExpectedInformation()
        {
            var sourceInformation = new TestProgramInformation();

            var information = new UserSpecifiedProgramInformation(sourceInformation);

            Assert.False(information.IsModified);
            VerifyInformation(sourceInformation, information);
        }

        [Fact]
        public void UserSpecifiedProgramInformation_ConstructWithAnotherUserSpecifiedProgramInformation_CreatesExpectedInformation()
        {
            var sourceInformation = new UserSpecifiedProgramInformation(12345, "Scoob!", "1966");

            var information = new UserSpecifiedProgramInformation(sourceInformation);

            Assert.False(information.IsModified);
            VerifyInformation(sourceInformation, information);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("^")]
        [InlineData("12")]
        [InlineData("96325")]
        public void UserSpecifiedProgramInformation_ConstructWithNonMetadataProgramInformation_CreatesExpectedInformation(string year)
        {
            var sourceInformation = new TestProgramInformation(0x24680246, null, IncompatibilityFlags.KeyboardComponent)
            {
                Title = "Sumpin' Mysterious",
                Vendor = "Somewhat Mysterious, Inc.",
                Year = year,
                ShortName = "S.M.",
                Features = new ProgramFeaturesBuilder().WithEcsFeatures(EcsFeatures.Synthesizer).Build() as ProgramFeatures
            };
            sourceInformation.AddCrc(0x13579135, "(revised)", IncompatibilityFlags.Tutorvision);
            sourceInformation.SetOrigin(ProgramInformationOrigin.UpdateFragment);

            var information = new UserSpecifiedProgramInformation(sourceInformation);

            Assert.False(information.IsModified);
            VerifyInformation(sourceInformation, information);
        }

        public static IEnumerable<object[]> TestProgramInformationWithMetadataTestData
        {
            get
            {
                var testInformation = new TestProgramInformationMetadata();
                yield return new object[] { testInformation };

                // fill all metadata except one (string fields)
                var dateTimeBuilder = new MetadataDateTimeBuilder(1999);
                foreach (var fieldIdToLeaveEmpty in TestProgramInformationMetadata.StringFieldIds)
                {
                    testInformation = new TestProgramInformationMetadata();
                    testInformation.AddReleaseDate(dateTimeBuilder.WithMonth(1).WithDay(2).Build());
                    testInformation.AddReleaseDate(dateTimeBuilder.WithDay(3).Build());
                    testInformation.AddBuildDate(dateTimeBuilder.WithDay(1).Build());
                    testInformation.AddBuildDate(dateTimeBuilder.WithDay(2).Build());
                    foreach (var fieldId in TestProgramInformationMetadata.StringFieldIds.Where(id => id != fieldIdToLeaveEmpty))
                    {
                        var stringValue = fieldId.ToString();
                        testInformation.AddMetadataValue(fieldId, stringValue + " 0");
                        testInformation.AddMetadataValue(fieldId, stringValue + " 1");
                    }
                    yield return new object[] { testInformation };
                }

                // test with empty build dates
                testInformation = new TestProgramInformationMetadata();
                testInformation.AddReleaseDate(dateTimeBuilder.WithMonth(1).WithDay(2).Build());
                testInformation.AddReleaseDate(dateTimeBuilder.WithDay(3).Build());
                foreach (var fieldId in TestProgramInformationMetadata.StringFieldIds)
                {
                    var stringValue = fieldId.ToString();
                    testInformation.AddMetadataValue(fieldId, stringValue + " 2");
                    testInformation.AddMetadataValue(fieldId, stringValue + " 3");
                }
                yield return new object[] { testInformation };

                // test with empty release dates
                testInformation = new TestProgramInformationMetadata();
                testInformation.AddBuildDate(dateTimeBuilder.WithDay(1).Build());
                testInformation.AddBuildDate(dateTimeBuilder.WithDay(2).Build());
                foreach (var fieldId in TestProgramInformationMetadata.StringFieldIds)
                {
                    var stringValue = fieldId.ToString();
                    testInformation.AddMetadataValue(fieldId, stringValue + " 4");
                    testInformation.AddMetadataValue(fieldId, stringValue + " 5");
                }
                yield return new object[] { testInformation };
            }
        }

        [Theory]
        [MemberData("TestProgramInformationWithMetadataTestData")]
        public void UserSpecifiedProgramInformation_ConstructWithEmptyMetadataProgramInformationWithNullMetadataFields_ThrowsArgumentNullException(IProgramInformation sourceInformation)
        {
            Assert.Throws<ArgumentNullException>(() => new UserSpecifiedProgramInformation(sourceInformation));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("S.M.")]
        public void UserSpecifiedProgramInformation_ConstructWithMetadataProgramInformationWithPopulatedMetadataFields_ContainsExpectedData(string shortName)
        {
            var sourceInformation = new TestProgramInformationMetadata()
            {
                Title = "Sumpin' Mysteriouser",
                Vendor = "Somewhat Mysterious, Inc.",
                Year = "1989",
                ShortName = shortName,
                Features = new ProgramFeaturesBuilder().WithEcsFeatures(EcsFeatures.Synthesizer).Build() as ProgramFeatures
            };
            sourceInformation.AddCrc(0x24680246, null, IncompatibilityFlags.KeyboardComponent);
            sourceInformation.AddCrc(0x13579135, "(fast)", IncompatibilityFlags.Tutorvision);
            sourceInformation.SetOrigin(ProgramInformationOrigin.UpdateFragment);

            var dateTimeBuilder = new MetadataDateTimeBuilder(1999);
            sourceInformation.AddReleaseDate(dateTimeBuilder.WithMonth(1).WithDay(2).Build());
            sourceInformation.AddReleaseDate(dateTimeBuilder.WithDay(3).Build());
            sourceInformation.AddBuildDate(dateTimeBuilder.WithDay(1).Build());
            sourceInformation.AddBuildDate(dateTimeBuilder.WithDay(2).Build());
            foreach (var fieldId in TestProgramInformationMetadata.StringFieldIds)
            {
                var stringValue = fieldId.ToString();
                sourceInformation.AddMetadataValue(fieldId, stringValue + " #1");
                sourceInformation.AddMetadataValue(fieldId, stringValue + " #2");
            }

            var information = new UserSpecifiedProgramInformation(sourceInformation);

            Assert.False(information.IsModified);
            VerifyInformation(sourceInformation, information);
        }

        public static IEnumerable<object[]> SetTitleTestData
        {
            get
            {
                yield return new object[] { null, null, ProgramInformation.UnknownProgramTitle, false };
                yield return new object[] { null, string.Empty, ProgramInformation.UnknownProgramTitle, false };
                yield return new object[] { string.Empty, null, ProgramInformation.UnknownProgramTitle, false };
                yield return new object[] { string.Empty, string.Empty, ProgramInformation.UnknownProgramTitle, false };
                yield return new object[] { "Init: Muh Titles!", "Init: Muh Titles!", "Init: Muh Titles!", false };
                yield return new object[] { "Init: er", "New: Muh Titles!", "Init: er", false };
                yield return new object[] { "Init: Glibly", null, "Init: Glibly", false };
                yield return new object[] { ProgramInformation.UnknownProgramTitle, "New: Glibly", "New: Glibly", true };
                yield return new object[] { null, ProgramInformation.UnknownProgramTitle, ProgramInformation.UnknownProgramTitle, false };
                yield return new object[] { string.Empty, ProgramInformation.UnknownProgramTitle, ProgramInformation.UnknownProgramTitle, false };
            }
        }

        [Theory]
        [MemberData("SetTitleTestData")]
        public void UserSpecifiedProgramInformation_SetTitle_SetsExpectedValue(string initialTitle, string newTitle, string expectedTitle, bool expectedIsModified)
        {
            // USPI behaves weirdly! If you pass null directly to constructor, it will assign "Unknown" -- and prevents empty / null.
            // But, if you send IProgramInformation, it preserves whatever was in there -- even null
            // SETTING is super odd.
            // 1. Disallows any null/empty -- fine
            // 2. Set is only allowed IF
            //  a) backing field is null/empty
            //  b) title == unknown AND new value is unknown
            // THUS: if you create USPI w/ name "FOO" then YOU CANNOT RENAME IT?
            // See: https://github.com/intvsteve/VINTage/issues/242
            var information = new UserSpecifiedProgramInformation(2u, initialTitle);
            Assert.False(information.IsModified);

            information.Title = newTitle;

            Assert.Equal(expectedTitle, information.Title);
            Assert.Equal(expectedIsModified, information.IsModified);
        }

        public static IEnumerable<object[]> SetTitleAfterInitializingViaIProgramInformationTestData
        {
            get
            {
                yield return new object[] { null, null, null, false };
                yield return new object[] { null, string.Empty, null, false };
                yield return new object[] { string.Empty, null, string.Empty, false };
                yield return new object[] { string.Empty, string.Empty, string.Empty, false };
                yield return new object[] { "Init: Muh Titles!", "Init: Muh Titles!", "Init: Muh Titles!", false };
                yield return new object[] { "Init: er", "New: Muh Titles!", "Init: er", false };
                yield return new object[] { "Init: Glibly", null, "Init: Glibly", false };
                yield return new object[] { ProgramInformation.UnknownProgramTitle, "New: Glibly", "New: Glibly", true };
                yield return new object[] { null, ProgramInformation.UnknownProgramTitle, ProgramInformation.UnknownProgramTitle, true };
                yield return new object[] { string.Empty, ProgramInformation.UnknownProgramTitle, ProgramInformation.UnknownProgramTitle, true };
            }
        }

        [Theory]
        [MemberData("SetTitleAfterInitializingViaIProgramInformationTestData")]
        public void UserSpecifiedProgramInformation_SetTitleAfterInitializeViaOtherIProgramInformation_SetsExpectedValue(string initialTitle, string newTitle, string expectedTitle, bool expectedIsModified)
        {
            // USPI behaves weirdly! If you pass null directly to constructor, it will assign "Unknown" -- and prevents empty / null.
            // But, if you send IProgramInformation, it preserves whatever was in there -- even null
            // SETTING is super odd.
            // 1. Disallows any null/empty -- fine
            // 2. Set is only allowed IF
            //  a) backing field is null/empty
            //  b) title == unknown AND new value is unknown
            // THUS: if you create USPI w/ name "FOO" then YOU CANNOT RENAME IT?
            // See: https://github.com/intvsteve/VINTage/issues/242
            var initialInformation = new TestProgramInformation() { Title = initialTitle };
            var information = new UserSpecifiedProgramInformation(initialInformation);
            Assert.False(information.IsModified);

            information.Title = newTitle;

            Assert.Equal(expectedTitle, information.Title);
            Assert.Equal(expectedIsModified, information.IsModified);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "")]
        [InlineData("VEND", "")]
        [InlineData(null, "OR")]
        [InlineData("", "PUB")]
        [InlineData("LISHER", "Sellers")]
        [InlineData("Peter", null)]
        [InlineData("Twist", "TwIst")]
        public void UserProgramInformation_SetVendor_BehavesCorrectly(string initialVendor, string newVendor)
        {
            var initialInformation = new TestProgramInformation() { Vendor = initialVendor };
            var information = new UserSpecifiedProgramInformation(initialInformation);
            Assert.False(information.IsModified);

            information.Vendor = newVendor;

            var expectedVendor = newVendor;
            Assert.Equal(expectedVendor, information.Vendor);
            var expectedIsModified = initialVendor != newVendor;
            Assert.Equal(expectedIsModified, information.IsModified);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "")]
        [InlineData("x", "")]
        [InlineData(null, "19")]
        [InlineData("", "nineteen eighty-four")]
        [InlineData("George", "Orwell")]
        [InlineData("Thos", null)]
        [InlineData("Huxley", "huXley")]
        public void UserProgramInformation_SetYear_BehavesCorrectly(string initialYear, string newYear)
        {
            var initialInformation = new TestProgramInformationMetadata() { Year = initialYear };
            initialInformation.SetAllMetadata();
            var information = new UserSpecifiedProgramInformation(initialInformation);
            Assert.False(information.IsModified);

            information.Year = newYear;

            var expectedYear = newYear;
            Assert.Equal(expectedYear, information.Year);
            var expectedIsModified = initialYear != newYear;
            Assert.Equal(expectedIsModified, information.IsModified);
        }

        public static IEnumerable<object[]> SetFeaturesTestData
        {
            get
            {
                yield return new object[] { null, null, null, false };
                yield return new object[] { ProgramFeatures.GetUnrecognizedRomFeatures(), ProgramFeatures.GetUnrecognizedRomFeatures(), ProgramFeatures.GetUnrecognizedRomFeatures(), false };
                yield return new object[] { null, ProgramFeatures.GetUnrecognizedRomFeatures(), ProgramFeatures.GetUnrecognizedRomFeatures(), true };
                yield return new object[] { ProgramFeatures.GetUnrecognizedRomFeatures(), null, null, true };
                var testFeatures = new ProgramFeaturesBuilder().WithGeneralFeatures(GeneralFeatures.SystemRom).Build();
                yield return new object[] { ProgramFeatures.EmptyFeatures, testFeatures, testFeatures, true };
            }
        }

        [Theory]
        [MemberData("SetFeaturesTestData")]
        public void UserProgramInformation_SetFeatures_BehavesCorrectly(IProgramFeatures initialFeatures, IProgramFeatures newProgramFeatures, IProgramFeatures expectedProgramFeatures, bool expectedIsModified)
        {
            var information = new UserSpecifiedProgramInformation(4, "hey", "1997", (ProgramFeatures)initialFeatures);
            Assert.False(information.IsModified);

            information.Features = (ProgramFeatures)newProgramFeatures;

            Assert.Equal(expectedProgramFeatures, information.Features);
            Assert.Equal(expectedIsModified, information.IsModified);
        }

        [Fact]
        public void UserProgramInformation_SetFeaturesToUnrecognizedFeatures_DoesNotModifyInformation()
        {
            var information = new UserSpecifiedProgramInformation(8);
            Assert.False(information.IsModified);

            information.Features = ProgramFeatures.GetUnrecognizedRomFeatures();

            Assert.Equal(ProgramFeatures.GetUnrecognizedRomFeatures(), information.Features);
            Assert.False(information.IsModified);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "")]
        [InlineData("s", "")]
        [InlineData(null, "S")]
        [InlineData("", "Short")]
        [InlineData("Round", "Cookies")]
        [InlineData("taste", null)]
        [InlineData("Grrreat!", "GRRREAT!")]
        public void UserProgramInformation_SetShortName_BehavesCorrectly(string initialShortName, string newShortName)
        {
            var initialInformation = new TestProgramInformation() { ShortName = initialShortName };
            var information = new UserSpecifiedProgramInformation(initialInformation);
            Assert.False(information.IsModified);

            information.ShortName = newShortName;

            var expectedShortName = newShortName;
            Assert.Equal(expectedShortName, information.ShortName);
            var expectedIsModified = initialShortName != newShortName;
            Assert.Equal(expectedIsModified, information.IsModified);
        }

        [Fact]
        public void UserProgramInformation_AddCrcOfZero_ThrowsArgumentOutOfRangeException()
        {
            var information = new UserSpecifiedProgramInformation(2u);

            Assert.Throws<ArgumentOutOfRangeException>(() => information.AddCrc(0, "bad", IncompatibilityFlags.None));
        }

        [Fact]
        public void UserProgramInformation_AddCrc_AddsCrc()
        {
            var information = new UserSpecifiedProgramInformation(2u);
            var expectedCrcs = information.Crcs.ToList();

            var newCrcData = new CrcData(3u, "Busted", IncompatibilityFlags.LtoFlash);
            information.AddCrc(newCrcData.Crc, newCrcData.Description, newCrcData.Incompatibilities);

            expectedCrcs.Add(newCrcData);
            Assert.Equal(expectedCrcs, information.Crcs);
        }

        [Fact]
        public void UserProgramInformation_AddDuplicateCrc_DoesNotAddCrc()
        {
            var information = new UserSpecifiedProgramInformation(2u);
            var expectedCrcs = information.Crcs.ToList();

            var newCrcData = new CrcData(6u, "REJECTED!", IncompatibilityFlags.LtoFlash);
            information.AddCrc(newCrcData.Crc, newCrcData.Description, newCrcData.Incompatibilities);
            information.AddCrc(newCrcData.Crc, newCrcData.Description, newCrcData.Incompatibilities);

            expectedCrcs.Add(newCrcData);
            Assert.Equal(expectedCrcs, information.Crcs);
        }

        [Fact]
        public void UserProgramInformation_AddLongNameMetadata_AddsLongNameMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddLongName(null));
            Assert.False(information.AddLongName(null));

            Assert.Equal(new string[] { ProgramInformation.UnknownProgramTitle, null }, information.LongNames);
        }

        [Fact]
        public void UserProgramInformation_AddShortNameMetadata_AddsShortNameMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddShortName(null));
            Assert.False(information.AddShortName(null));

            Assert.Equal(new string[] { null }, information.ShortNames);
        }

        [Fact]
        public void UserProgramInformation_AddDescriptionMetadata_AddsDescriptionMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddDescription(null));
            Assert.False(information.AddDescription(null));

            Assert.Equal(new string[] { null }, information.Descriptions);
        }

        [Fact]
        public void UserProgramInformation_AddPublisherMetadata_AddsPublisherMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddPublisher(null));
            Assert.False(information.AddPublisher(null));

            Assert.Equal(new string[] { null }, information.Publishers);
        }

        [Fact]
        public void UserProgramInformation_AddProgrammerMetadata_AddsProgrammerMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddProgrammer(null));
            Assert.False(information.AddProgrammer(null));

            Assert.Equal(new string[] { null }, information.Programmers);
        }

        [Fact]
        public void UserProgramInformation_AddDesignerMetadata_AddsDesignerMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddDesigner(null));
            Assert.False(information.AddDesigner(null));

            Assert.Equal(new string[] { null }, information.Designers);
        }

        [Fact]
        public void UserProgramInformation_AddGraphicsMetadata_AddsGraphicsMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddGraphics(null));
            Assert.False(information.AddGraphics(null));

            Assert.Equal(new string[] { null }, information.Graphics);
        }

        [Fact]
        public void UserProgramInformation_AddMusicMetadata_AddsMusicMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddMusic(null));
            Assert.False(information.AddMusic(null));

            Assert.Equal(new string[] { null }, information.Music);
        }

        [Fact]
        public void UserProgramInformation_AddSoundEffectsMetadata_AddsSoundEffectsMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddSoundEffects(null));
            Assert.False(information.AddSoundEffects(null));

            Assert.Equal(new string[] { null }, information.SoundEffects);
        }

        [Fact]
        public void UserProgramInformation_AddVoiceMetadata_AddsVoiceMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddVoice(null));
            Assert.False(information.AddVoice(null));

            Assert.Equal(new string[] { null }, information.Voices);
        }

        [Fact]
        public void UserProgramInformation_AddDocumentationMetadata_AddsDocumentationMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddDocumentation(null));
            Assert.False(information.AddDocumentation(null));

            Assert.Equal(new string[] { null }, information.Documentation);
        }

        [Fact]
        public void UserProgramInformation_AddArtworkMetadata_AddsArtworkMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddArtwork(null));
            Assert.False(information.AddArtwork(null));

            Assert.Equal(new string[] { null }, information.Artwork);
        }

        [Fact]
        public void UserProgramInformation_AddReleaseDateMetadata_AddsReleaseDateMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddReleaseDate(MetadataDateTime.MinValue));
            Assert.False(information.AddReleaseDate(MetadataDateTime.MinValue));

            Assert.Equal(new[] { MetadataDateTime.MinValue }, information.ReleaseDates);
        }

        [Fact]
        public void UserProgramInformation_AddLicenseMetadata_AddsLicenseMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddLicense(null));
            Assert.False(information.AddLicense(null));

            Assert.Equal(new string[] { null }, information.Licenses);
        }

        [Fact]
        public void UserProgramInformation_AddContactInformatioMetadata_AddsContactInformatioMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddContactInformation(null));
            Assert.False(information.AddContactInformation(null));

            Assert.Equal(new string[] { null }, information.ContactInformation);
        }

        [Fact]
        public void UserProgramInformation_AddVersionMetadata_AddsVersionMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddVersion(null));
            Assert.False(information.AddVersion(null));

            Assert.Equal(new string[] { null }, information.Versions);
        }

        [Fact]
        public void UserProgramInformation_AddBuildDateMetadata_AddsBuildDateMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.True(information.AddBuildDate(MetadataDateTime.MinValue));
            Assert.False(information.AddBuildDate(MetadataDateTime.MinValue));

            Assert.Equal(new[] { MetadataDateTime.MinValue }, information.BuildDates);
        }

        [Fact]
        public void UserProgramInformation_AddNullAdditionalInformationMetadata_ThrowsArgumentNullException()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            Assert.Throws<ArgumentNullException>(() => information.AddAdditionalInformation(null, null));
        }

        [Fact]
        public void UserProgramInformation_AddAdditionalInformationMetadata_AddsAdditionalInformationMetadata()
        {
            var information = new UserSpecifiedProgramInformation(2222u);

            information.AddAdditionalInformation(string.Empty, "first");
            information.AddAdditionalInformation(string.Empty, "second");
            information.AddAdditionalInformation("derp", "other");

            var expectedAdditionalInformation = new[] { ": first, second", "derp: other" };
            Assert.Equal(expectedAdditionalInformation, information.AdditionalInformation);
        }

        private void VerifyInformation(IProgramInformation expectedInformation, UserSpecifiedProgramInformation information)
        {
            var expectedTitle = string.IsNullOrEmpty(expectedInformation.Title) ? ProgramInformation.UnknownProgramTitle : expectedInformation.Title;
            Assert.Equal(expectedInformation.DataOrigin, information.DataOrigin);
            Assert.Equal(expectedInformation.Title, information.Title);
            Assert.Equal(expectedInformation.Vendor, information.Vendor);
            MetadataDateTime dontCare;
            Assert.Equal(GetExpectedYearString(expectedInformation.Year, out dontCare), information.Year);
            Assert.Equal(expectedInformation.Features, information.Features);
            var expectedShortName = expectedInformation.ShortName;
            if (string.IsNullOrEmpty(expectedShortName))
            {
                var metadata = expectedInformation as IProgramMetadata;
                if ((metadata != null) && (metadata.ShortNames != null) && !string.IsNullOrEmpty(metadata.ShortNames.FirstOrDefault()))
                {
                    expectedShortName = metadata.ShortNames.FirstOrDefault();
                }
            }
            Assert.Equal(expectedShortName, information.ShortName);

            Assert.Equal(expectedInformation.Crcs.Count(), information.Crcs.Count());

            foreach (var crcDataPair in expectedInformation.Crcs.Zip(information.Crcs, (e, a) => new KeyValuePair<CrcData, CrcData>(e, a)))
            {
                // TODO: Determine why the BinConfigTemplate is not propagated.
                Assert.Equal(crcDataPair.Key.Crc, crcDataPair.Value.Crc);
                Assert.Equal(crcDataPair.Key.Description, crcDataPair.Value.Description);
                Assert.Equal(crcDataPair.Key.Incompatibilities, crcDataPair.Value.Incompatibilities);
                Assert.Equal(crcDataPair.Key.BinConfigTemplate, crcDataPair.Value.BinConfigTemplate);
            }

            VerifyMetadata(expectedInformation, information);
        }

        private void VerifyMetadata(IProgramInformation expectedInformation, UserSpecifiedProgramInformation information)
        {
            var expectedMetadata = expectedInformation as IProgramMetadata;
            if (expectedMetadata == null)
            {
                var emptyStringData = Enumerable.Empty<string>();
                Assert.Equal(string.IsNullOrEmpty(expectedInformation.Title) ? emptyStringData : new[] { expectedInformation.Title }, information.LongNames);
                Assert.Equal(string.IsNullOrEmpty(expectedInformation.ShortName) ? emptyStringData : new[] { expectedInformation.ShortName }, information.ShortNames);
                Assert.Equal(emptyStringData, information.Descriptions);
                Assert.Equal(string.IsNullOrEmpty(expectedInformation.Vendor) ? emptyStringData : new[] { expectedInformation.Vendor }, information.Publishers);
                Assert.Equal(emptyStringData, information.Programmers);
                Assert.Equal(emptyStringData, information.Designers);
                Assert.Equal(emptyStringData, information.Graphics);
                Assert.Equal(emptyStringData, information.Music);
                Assert.Equal(emptyStringData, information.SoundEffects);
                Assert.Equal(emptyStringData, information.Voices);
                Assert.Equal(emptyStringData, information.Documentation);
                Assert.Equal(emptyStringData, information.Artwork);
                Assert.Equal(emptyStringData, information.Licenses);
                Assert.Equal(emptyStringData, information.ContactInformation);
                Assert.Equal(emptyStringData, information.Versions);
                Assert.Equal(emptyStringData, information.AdditionalInformation);

                var emptyDateData = Enumerable.Empty<MetadataDateTime>();
                var expectedReleaseDateData = emptyDateData;
                MetadataDateTime releaseDate;
                var expectedYearString = GetExpectedYearString(expectedInformation.Year, out releaseDate);
                if (releaseDate != MetadataDateTime.MinValue)
                {
                    expectedReleaseDateData = new[] { releaseDate };
                }
                Assert.Equal(expectedReleaseDateData, information.ReleaseDates);
                Assert.Equal(emptyDateData, information.BuildDates);
            }
            else
            {
                Assert.Equal(expectedMetadata.LongNames, information.LongNames);
                Assert.Equal(expectedMetadata.ShortNames, information.ShortNames);
                Assert.Equal(expectedMetadata.Descriptions, information.Descriptions);
                Assert.Equal(expectedMetadata.Publishers, information.Publishers);
                Assert.Equal(expectedMetadata.Programmers, information.Programmers);
                Assert.Equal(expectedMetadata.Designers, information.Designers);
                Assert.Equal(expectedMetadata.Graphics, information.Graphics);
                Assert.Equal(expectedMetadata.Music, information.Music);
                Assert.Equal(expectedMetadata.SoundEffects, information.SoundEffects);
                Assert.Equal(expectedMetadata.Voices, information.Voices);
                Assert.Equal(expectedMetadata.Documentation, information.Documentation);
                Assert.Equal(expectedMetadata.Artwork, information.Artwork);
                Assert.Equal(expectedMetadata.Licenses, information.Licenses);
                Assert.Equal(expectedMetadata.ContactInformation, information.ContactInformation);
                Assert.Equal(expectedMetadata.Versions, information.Versions);

                var expectedAdditionalInformation = Enumerable.Range(0, expectedMetadata.AdditionalInformation.Count()).Zip(expectedMetadata.AdditionalInformation, (i, s) => string.Format(CultureInfo.CurrentCulture, "{0}: {1}", i, s));
                Assert.Equal(expectedAdditionalInformation, information.AdditionalInformation);

                Assert.Equal(expectedMetadata.ReleaseDates, information.ReleaseDates);
                Assert.Equal(expectedMetadata.BuildDates, information.BuildDates);
            }
        }

        private string GetExpectedYearString(string year, out MetadataDateTime date)
        {
            date = MetadataDateTime.MinValue;
            var yearString = year;
            if (!string.IsNullOrEmpty(year))
            {
                int releaseYear;
                if (int.TryParse(year, out releaseYear))
                {
                    try
                    {
                        var dateBuilder = new MetadataDateTimeBuilder(releaseYear);
                        date = dateBuilder.Build();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        yearString = string.Empty;
                    }
                }
                else
                {
                }
            }
            return yearString;
        }

        private class TestProgramInformation : IProgramInformation
        {
            public TestProgramInformation()
            {
            }

            public TestProgramInformation(uint crc, string description = null, IncompatibilityFlags incompatibilityFlags = IncompatibilityFlags.None)
            {
                if (crc != 0)
                {
                    AddCrc(crc, description, incompatibilityFlags);
                }
            }

            public ProgramInformationOrigin DataOrigin
            {
                get { return _origin; }
            }
            private ProgramInformationOrigin _origin = ProgramInformationOrigin.None;

            public string Title { get; set; }

            public string Vendor { get; set; }

            public string Year { get; set; }

            public ProgramFeatures Features { get; set; }

            public string ShortName { get; set; }

            public IEnumerable<CrcData> Crcs
            {
                get { return _crcs; }
            }
            private List<CrcData> _crcs = new List<CrcData>();

            public bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilityFlags)
            {
                var existingCrc = _crcs.FirstOrDefault(c => c.Crc == newCrc);
                var added = existingCrc == null;
                if (added)
                {
                    var crcData = new CrcData(newCrc, crcDescription, incompatibilityFlags);
                    _crcs.Add(crcData);
                }
                return added;
            }

            public bool ModifyCrc(uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags)
            {
                throw new NotImplementedException();
            }

            public void SetOrigin(ProgramInformationOrigin origin)
            {
                _origin = origin;
            }
        }

        private class TestProgramInformationMetadata : TestProgramInformation, IProgramMetadata
        {
            private static readonly Lazy<IEnumerable<IProgramMetadataFieldId>> LazyStringFieldIds = new Lazy<IEnumerable<IProgramMetadataFieldId>>(() => Enum.GetValues(typeof(IProgramMetadataFieldId)).Cast<IProgramMetadataFieldId>().Except(new[] { IProgramMetadataFieldId.None, IProgramMetadataFieldId.BuildDates, IProgramMetadataFieldId.ReleaseDates }));

            public static IEnumerable<IProgramMetadataFieldId> StringFieldIds
            {
                get { return LazyStringFieldIds.Value; }
            }

            public IEnumerable<string> LongNames
            {
                get { return _longNames; }
            }
            private HashSet<string> _longNames;

            public IEnumerable<string> ShortNames
            {
                get { return _shortNames; }
            }
            private HashSet<string> _shortNames;

            public IEnumerable<string> Descriptions
            {
                get { return _descriptions; }
            }
            private HashSet<string> _descriptions;

            public IEnumerable<string> Publishers
            {
                get { return _publishers; }
            }
            private HashSet<string> _publishers;

            public IEnumerable<string> Programmers
            {
                get { return _programmers; }
            }
            private HashSet<string> _programmers;

            public IEnumerable<string> Designers
            {
                get { return _designers; }
            }
            private HashSet<string> _designers;

            public IEnumerable<string> Graphics
            {
                get { return _graphics; }
            }
            private HashSet<string> _graphics;

            public IEnumerable<string> Music
            {
                get { return _music; }
            }
            private HashSet<string> _music;

            public IEnumerable<string> SoundEffects
            {
                get { return _soundEffects; }
            }
            private HashSet<string> _soundEffects;

            public IEnumerable<string> Voices
            {
                get { return _voices; }
            }
            private HashSet<string> _voices;

            public IEnumerable<string> Documentation
            {
                get { return _documentation; }
            }
            private HashSet<string> _documentation;

            public IEnumerable<string> Artwork
            {
                get { return _artwork; }
            }
            private HashSet<string> _artwork;

            public IEnumerable<MetadataDateTime> ReleaseDates
            {
                get { return _releaseDates; }
            }
            private HashSet<MetadataDateTime> _releaseDates;

            public IEnumerable<string> Licenses
            {
                get { return _licenses; }
            }
            private HashSet<string> _licenses;

            public IEnumerable<string> ContactInformation
            {
                get { return _contactInformation; }
            }
            private HashSet<string> _contactInformation;

            public IEnumerable<string> Versions
            {
                get { return _versions; }
            }
            private HashSet<string> _versions;

            public IEnumerable<MetadataDateTime> BuildDates
            {
                get { return _buildDates; }
            }
            private HashSet<MetadataDateTime> _buildDates;

            public IEnumerable<string> AdditionalInformation
            {
                get { return _additionalInformation; }
            }
            private HashSet<string> _additionalInformation;

            public void SetAllMetadata()
            {
                var dateTimeBuilder = new MetadataDateTimeBuilder(1999);
                AddReleaseDate(dateTimeBuilder.WithMonth(1).WithDay(2).Build());
                AddReleaseDate(dateTimeBuilder.WithDay(3).Build());
                AddBuildDate(dateTimeBuilder.WithDay(1).Build());
                AddBuildDate(dateTimeBuilder.WithDay(2).Build());
                foreach (var fieldId in TestProgramInformationMetadata.StringFieldIds)
                {
                    var stringValue = fieldId.ToString();
                    AddMetadataValue(fieldId, stringValue);
                }
            }

            public void AddMetadataValue(IProgramMetadataFieldId field, string value)
            {
                HashSet<string> data = null;
                var newData = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                switch (field)
                {
                    case IProgramMetadataFieldId.LongNames:
                        if (_longNames == null)
                        {
                            _longNames = newData;
                        }
                        data = _longNames;
                        break;
                    case IProgramMetadataFieldId.ShortNames:
                        if (_shortNames == null)
                        {
                            _shortNames = newData;
                        }
                        data = _shortNames;
                        break;
                    case IProgramMetadataFieldId.Descriptions:
                        if (_descriptions == null)
                        {
                            _descriptions = newData;
                        }
                        data = _descriptions;
                        break;
                    case IProgramMetadataFieldId.Publishers:
                        if (_publishers == null)
                        {
                            _publishers = newData;
                        }
                        data = _publishers;
                        break;
                    case IProgramMetadataFieldId.Programmers:
                        if (_programmers == null)
                        {
                            _programmers = newData;
                        }
                        data = _programmers;
                        break;
                    case IProgramMetadataFieldId.Designers:
                        if (_designers == null)
                        {
                            _designers = newData;
                        }
                        data = _designers;
                        break;
                    case IProgramMetadataFieldId.Graphics:
                        if (_graphics == null)
                        {
                            _graphics = newData;
                        }
                        data = _graphics;
                        break;
                    case IProgramMetadataFieldId.Music:
                        if (_music == null)
                        {
                            _music = newData;
                        }
                        data = _music;
                        break;
                    case IProgramMetadataFieldId.SoundEffects:
                        if (_soundEffects == null)
                        {
                            _soundEffects = newData;
                        }
                        data = _soundEffects;
                        break;
                    case IProgramMetadataFieldId.Voices:
                        if (_voices == null)
                        {
                            _voices = newData;
                        }
                        data = _voices;
                        break;
                    case IProgramMetadataFieldId.Documentation:
                        if (_documentation == null)
                        {
                            _documentation = newData;
                        }
                        data = _documentation;
                        break;
                    case IProgramMetadataFieldId.Artwork:
                        if (_artwork == null)
                        {
                            _artwork = newData;
                        }
                        data = _artwork;
                        break;
                    case IProgramMetadataFieldId.Licenses:
                        if (_licenses == null)
                        {
                            _licenses = newData;
                        }
                        data = _licenses;
                        break;
                    case IProgramMetadataFieldId.ContactInformation:
                        if (_contactInformation == null)
                        {
                            _contactInformation = newData;
                        }
                        data = _contactInformation;
                        break;
                    case IProgramMetadataFieldId.Versions:
                        if (_versions == null)
                        {
                            _versions = newData;
                        }
                        data = _versions;
                        break;
                    case IProgramMetadataFieldId.AdditionalInformation:
                        if (_additionalInformation == null)
                        {
                            _additionalInformation = newData;
                        }
                        data = _additionalInformation;
                        break;
                    default:
                        break;
                }
                data.Add(value);
            }

            public void AddReleaseDate(MetadataDateTime date)
            {
                if (_releaseDates == null)
                {
                    _releaseDates = new HashSet<MetadataDateTime>();
                }
                _releaseDates.Add(date);
            }

            public void AddBuildDate(MetadataDateTime date)
            {
                if (_buildDates == null)
                {
                    _buildDates = new HashSet<MetadataDateTime>();
                }
                _buildDates.Add(date);
            }
        }
    }
}

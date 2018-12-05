// <copyright file="IProgramInformationHelpersTests.cs" company="INTV Funhouse">
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
using INTV.Core.Restricted.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class IProgramInformationHelpersTests
    {
        [Fact]
        public void IProgramInformation_AddCrcToNullInformation_ThrowsNullReferenceException()
        {
            IProgramInformation information = null;

            Assert.Throws<NullReferenceException>(() => information.AddCrc(1u));
        }

        [Fact]
        public void IProgramInformation_AddCrc_AddsTheCrc()
        {
            var information = new TestProgramInformation();

            var crc = 0x00112233u;

            Assert.True(information.AddCrc(crc));
            Assert.Equal(1, information.Crcs.Count());
            Assert.Equal(crc, information.Crcs.First().Crc);
            Assert.Equal(IncompatibilityFlags.None, information.Crcs.First().Incompatibilities);
            Assert.True(string.IsNullOrEmpty(information.Crcs.First().Description));
        }

        [Fact]
        public void IProgramInformation_AddDuplicateCrc_DoesNotAddTheCrc()
        {
            var information = new TestProgramInformation();

            var crc = 0x44556677u;

            Assert.True(information.AddCrc(crc));
            Assert.False(information.AddCrc(crc));
            Assert.Equal(1, information.Crcs.Count());
            Assert.Equal(crc, information.Crcs.First().Crc);
            Assert.Equal(IncompatibilityFlags.None, information.Crcs.First().Incompatibilities);
            Assert.True(string.IsNullOrEmpty(information.Crcs.First().Description));
        }

        [Fact]
        public void IProgramInformation_AddCrcWithDescriptionToNullInformation_ThrowsNullReferenceException()
        {
            IProgramInformation information = null;

            Assert.Throws<NullReferenceException>(() => information.AddCrc(1u, "description"));
        }

        [Fact]
        public void IProgramInformation_AddCrcWithDescription_AddsTheCrc()
        {
            var information = new TestProgramInformation();

            var crc = 0x00112233u;
            var description = "Latest";

            Assert.True(information.AddCrc(crc, description));
            Assert.Equal(1, information.Crcs.Count());
            Assert.Equal(crc, information.Crcs.First().Crc);
            Assert.Equal(IncompatibilityFlags.None, information.Crcs.First().Incompatibilities);
            Assert.Equal(description, information.Crcs.First().Description);
        }

        [Fact]
        public void IProgramInformation_ModifyCrcOnNullInformation_ThrowsNullReferenceException()
        {
            IProgramInformation information = null;

            Assert.Throws<NullReferenceException>(() => information.ModifyCrc(1u, "description", IncompatibilityFlags.Ntsc));
        }

        [Fact]
        public void IProgramInformation_ModifyCrcWithDescription_UpdatesTheCrc()
        {
            var information = new TestProgramInformation();
            var crc = 0x00112233u;
            var description = "Penultimate ROM";
            var incompatibilities = IncompatibilityFlags.IntellivisionII;
            Assert.True(information.AddCrc(crc, description, incompatibilities));

            var newDescription = "Ultimate ROM";
            var newIncompatibilities = IncompatibilityFlags.Tutorvision;
            Assert.True(information.ModifyCrc(crc, newDescription, newIncompatibilities));

            Assert.Equal(1, information.Crcs.Count());
            Assert.Equal(crc, information.Crcs.First().Crc);
            Assert.Equal(newIncompatibilities, information.Crcs.First().Incompatibilities);
            Assert.Equal(newDescription, information.Crcs.First().Description);
        }

        [Fact]
        public void IProgramInformation_GetNameFOrCrcOnNullInformation_ThrowsNullReferenceException()
        {
            IProgramInformation information = null;

            Assert.Throws<NullReferenceException>(() => information.GetNameForCrc(1u));
        }

        [Fact]
        public void IProgramInformation_GetNameForCrcWithNoDescription_ReturnsTitle()
        {
            var information = new TestProgramInformation() { Title = "Mr. Clean" };
            var crc0 = 0xFEDCBA98u;
            Assert.True(information.AddCrc(crc0));
            var crc1 = 0x00112233u;
            Assert.True(information.AddCrc(crc1));

            Assert.Equal(information.Title, information.GetNameForCrc(crc0));
            Assert.Equal(information.Title, information.GetNameForCrc(crc1));
        }

        [Fact]
        public void IProgramInformation_GetNameForCrcWithNoMatches_ThrowsInvalidOperationException()
        {
            var information = new TestProgramInformation() { Title = "Mr. Clean" };
            var crc0 = 0xFEDCBA98u;
            Assert.True(information.AddCrc(crc0));
            var crc1 = 0x00112233u;
            Assert.True(information.AddCrc(crc1));

            Assert.Throws<InvalidOperationException>(() => information.GetNameForCrc(0x11223344u));
        }

        [Fact]
        public void IProgramInformation_GetNameForCrcWithDescription_ReturnsExpectedName()
        {
            var information = new TestProgramInformation() { Title = "Best Game EVAR!" };

            var crc0 = 0x00112233u;
            var description0 = "Penultimate ROM";
            Assert.True(information.AddCrc(crc0, description0));
            var crc1 = 0x44332211u;
            var description1 = "Ultimate ROM";
            Assert.True(information.AddCrc(crc1, description1));

            var name0 = string.Format("{0} ({1})", information.Title, description0);
            var name1 = string.Format("{0} ({1})", information.Title, description1);
            Assert.Equal(name0, information.GetNameForCrc(crc0));
            Assert.Equal(name1, information.GetNameForCrc(crc1));
        }

        [Fact]
        public void IProgramInformation_GetDatabaseCodeOnNullInformation_ReturnsNullCode()
        {
            IProgramInformation information = null;

            Assert.Null(information.GetDatabaseCode());
        }

        [Fact]
        public void IProgramInformation_GetDatabaseCodeOnInformationThatDoesNotSupportCode_ReturnsNullCode()
        {
            var information = new TestProgramInformation() { Title = "No code for you!" };

            Assert.Null(information.GetDatabaseCode());
        }

        [Fact]
        public void IProgramInformation_GetDatabaseCodeOnIntvFunhouseXmlProgramInformation_ReturnsExpectedCode()
        {
            var information = new IntvFunhouseXmlProgramInformation() { Title = "No code for you!", Code = " code " };

            Assert.Equal("code", information.GetDatabaseCode());
        }

        [Fact]
        public void IProgramInformation_GetDatabaseCodeOnUnmergedProgramInformation_ReturnsExpectedCode()
        {
            var information = new UnmergedProgramInformation(
                "dude",
                "The Big Lebowski",
                "Coen Brothers",
                "1998",
                new[] { 1234u },
                new[] { "Duuude!" },
                new[] { 0 },
                ProgramFeatures.GetUnrecognizedRomFeatures());

            Assert.Equal("dude", information.GetDatabaseCode());
        }

        [Fact]
        public void IProgramInformation_MergeNullInformatoin_ThrowsArgumentException()
        {
            IProgramInformation information = null;

            Assert.Throws<ArgumentException>(() => information.Merge(ProgramInformationMergeFieldsFlags.None));
        }

        [Fact]
        public void IProgramInformation_MergeInvalidField_ThrowsArgumentException()
        {
            var information = new TestProgramInformation();
            var field = (ProgramInformationMergeFieldsFlags)(1u << 12);

            Assert.Throws<ArgumentException>(() => information.Merge(field));
        }

        [Fact]
        public void IProgramInformation_MergeInvalidFieldInOtherSources_ThrowsArgumentException()
        {
            var information = new TestProgramInformation();
            var toMerge = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation(), (ProgramInformationMergeFieldsFlags)(1u << 12));

            Assert.Throws<ArgumentException>(() => information.Merge(ProgramInformationMergeFieldsFlags.All, toMerge));
        }

        [Fact]
        public void IProgramInformation_MergeNothing_LeavesInformationUnchanged()
        {
            var information = new TestProgramInformation()
            {
                Title = "Spiffy the Bugslayer",
                Vendor = "Hy-Phen",
                Year = "1990",
                ShortName = "Spiffy",
                Features = (ProgramFeatures)new ProgramFeaturesBuilder().WithIntellicartFeatures(IntellicartCC3Features.SerialPortEnhanced).Build()
            };
            var crcData = new CrcData(0x24u, "v0", IncompatibilityFlags.IntellivisionII);
            Assert.True(information.AddCrc(crcData.Crc, crcData.Description, crcData.Incompatibilities));

            var mergedInformation = information.Merge(ProgramInformationMergeFieldsFlags.All);

            Assert.Equal(information.Title, mergedInformation.Title);
            Assert.Equal(information.Vendor, mergedInformation.Vendor);
            Assert.Equal(information.Year, mergedInformation.Year);
            Assert.Equal(information.Features, mergedInformation.Features);
            Assert.Equal(information.ShortName, mergedInformation.ShortName);
            Assert.Equal(information.Crcs, mergedInformation.Crcs);
        }

        [Fact]
        public void IProgramInformation_MergeEverythingWithOverlappingFields_OnlyMergesFeaturesAndCrcs()
        {
            var information = new TestProgramInformation()
            {
                Title = "Title of the Highest Quality",
                Vendor = "Ven-Dor",
                Year = "1990",
                ShortName = "THQ",
                Features = (ProgramFeatures)new ProgramFeaturesBuilder().WithIntellicartFeatures(IntellicartCC3Features.SerialPortEnhanced).Build()
            };
            var crcData = new CrcData(0x24u, "v0", IncompatibilityFlags.IntellivisionII);
            Assert.True(information.AddCrc(crcData.Crc, crcData.Description, crcData.Incompatibilities));

            var toMerge = new TestProgramInformation()
            {
                Title = "Title of the Higherest Quality",
                Vendor = "Ven-Dor II",
                Year = "1991",
                ShortName = "THQ-II",
                Features = (ProgramFeatures)new ProgramFeaturesBuilder().WithJlpFeatures(JlpFeatures.SerialPortEnhanced).Build()
            };
            var crcDataToMerge = new CrcData(0x25u, "v1", IncompatibilityFlags.Bee3);
            toMerge.AddCrc(crcDataToMerge.Crc, crcDataToMerge.Description, crcDataToMerge.Incompatibilities);

            var mergedInformation = information.Merge(ProgramInformationMergeFieldsFlags.All, new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(toMerge, ProgramInformationMergeFieldsFlags.All));

            var expectedFeatures = new ProgramFeaturesBuilder().WithIntellicartFeatures(IntellicartCC3Features.SerialPortEnhanced).WithJlpFeatures(JlpFeatures.SerialPortEnhanced).Build();
            var expectedCrcs = new[] { crcData, crcDataToMerge };
            Assert.Equal(information.Title, mergedInformation.Title);
            Assert.Equal(information.Vendor, mergedInformation.Vendor);
            Assert.Equal(information.Year, mergedInformation.Year);
            Assert.Equal(expectedFeatures, mergedInformation.Features);
            Assert.Equal(information.ShortName, mergedInformation.ShortName);
            Assert.Equal(expectedCrcs, mergedInformation.Crcs);
        }

        [Fact]
        public void IProgramInformation_MergeTitle_CorrectlyMergesTitle()
        {
            AssertStringFieldIsMerged(ProgramInformationMergeFieldsFlags.Title, info => info.Title = "testTitle", info => info.Title);
        }

        [Fact]
        public void IProgramInformation_MergeVendor_CorrectlyMergesVendor()
        {
            AssertStringFieldIsMerged(ProgramInformationMergeFieldsFlags.Vendor, info => info.Vendor = "testVendor", info => info.Vendor);
        }

        [Fact]
        public void IProgramInformation_MergeYear_CorrectlyMergesYear()
        {
            AssertStringFieldIsMerged(ProgramInformationMergeFieldsFlags.Year, info => info.Year = "2020", info => info.Year);
        }

        [Fact]
        public void IProgramInformation_MergeFeatures_CorrectlyMergesFeatures()
        {
            var emptyInformation = new TestProgramInformation();
            Assert.Null(emptyInformation.Features);
            var features = (ProgramFeatures)new ProgramFeaturesBuilder().WithInitialFeatures(ProgramFeatures.DefaultFeatures).WithTutorvisionCompatibility(FeatureCompatibility.Requires).Build();
            var otherInformationSource = new TestProgramInformation() { Features = features };

            var mergedInformation = emptyInformation.Merge(ProgramInformationMergeFieldsFlags.Features, new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(otherInformationSource, ProgramInformationMergeFieldsFlags.Features));

            Assert.Equal(features, mergedInformation.Features);
        }

        [Fact]
        public void IProgramInformation_MergeShortName_CorrectlyMergesShortName()
        {
            AssertStringFieldIsMerged(ProgramInformationMergeFieldsFlags.ShortName, info => info.ShortName = "Short Name", info => info.ShortName);
        }

        [Fact]
        public void IProgramInformation_MergeCrcs_CorrectlyMergesCrcs()
        {
            var emptyInformation = new TestProgramInformation();
            Assert.Null(emptyInformation.Features);
            var otherInformationSource = new TestProgramInformation();
            Assert.True(otherInformationSource.AddCrc(0x689504u, "Dreading it", IncompatibilityFlags.Pal));

            var mergedInformation = emptyInformation.Merge(ProgramInformationMergeFieldsFlags.Crcs, new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(otherInformationSource, ProgramInformationMergeFieldsFlags.Crcs));

            Assert.Equal(otherInformationSource.Crcs, mergedInformation.Crcs);
        }

        [Fact]
        public void IProgramInformation_MergeAllFieldsFromMultipleSources_CorrectlyMerges()
        {
            var emptyInformation = new TestProgramInformation();
            var titleSource = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation() { Title = "Hey der" }, ProgramInformationMergeFieldsFlags.All);
            var vendorSource = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation() { Title = " Buddy, ", Vendor = "howya doin" }, ProgramInformationMergeFieldsFlags.All);
            var yearSource = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation() { Vendor = "ya hoser", Year = "1984" }, ProgramInformationMergeFieldsFlags.All);
            var features = (ProgramFeatures)new ProgramFeaturesBuilder().WithInitialFeatures(ProgramFeatures.GetUnrecognizedRomFeatures()).WithSuperVideoArcadeCompatibility(FeatureCompatibility.Enhances).Build();
            var featuresSource = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation() { Features = features, Year = "1988" }, ProgramInformationMergeFieldsFlags.All);
            var shortNameSource = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation() { ShortName = "ShortRound" }, ProgramInformationMergeFieldsFlags.All);
            var crcsSource = new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(new TestProgramInformation(), ProgramInformationMergeFieldsFlags.All);
            Assert.True(crcsSource.Item1.AddCrc(0x246801u, "uffdah", IncompatibilityFlags.KeyboardComponent));

            var mergedInformation = emptyInformation.Merge(ProgramInformationMergeFieldsFlags.All, titleSource, vendorSource, yearSource, featuresSource, shortNameSource, crcsSource);

            Assert.Equal(titleSource.Item1.Title, mergedInformation.Title);
            Assert.Equal(vendorSource.Item1.Vendor, mergedInformation.Vendor);
            Assert.Equal(yearSource.Item1.Year, mergedInformation.Year);
            Assert.Equal(featuresSource.Item1.Features, mergedInformation.Features);
            Assert.Equal(shortNameSource.Item1.ShortName, mergedInformation.ShortName);
            Assert.Equal(crcsSource.Item1.Crcs, mergedInformation.Crcs);
        }

        private void AssertStringFieldIsMerged(ProgramInformationMergeFieldsFlags field, Action<TestProgramInformation> otherSourceFieldSetter, Func<IProgramInformation, string> informationFieldGetter)
        {
            var emptyInformation = new TestProgramInformation();
            Assert.Null(informationFieldGetter(emptyInformation));
            var otherInformationSource = new TestProgramInformation();
            otherSourceFieldSetter(otherInformationSource);
            Assert.NotNull(informationFieldGetter(otherInformationSource));

            var mergedInformation = emptyInformation.Merge(field, new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(otherInformationSource, field));

            var expectedValue = informationFieldGetter(otherInformationSource);
            var actualValue = informationFieldGetter(mergedInformation);
            Assert.Equal(expectedValue, actualValue);
        }

        private class TestProgramInformation : ProgramInformation
        {
            /// <inheritdoc />
            public override ProgramInformationOrigin DataOrigin
            {
                get { return ProgramInformationOrigin.None; }
            }

            /// <inheritdoc />
            public override string Title { get; set; }

            /// <inheritdoc />
            public override string Year { get; set; }

            /// <inheritdoc />
            public override ProgramFeatures Features { get; set; }

            /// <inheritdoc />
            public override IEnumerable<CrcData> Crcs
            {
                get { return _crcs.Values; }
            }
            private Dictionary<uint, CrcData> _crcs = new Dictionary<uint, CrcData>();

            /// <inheritdoc />
            public override IEnumerable<string> LongNames
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> ShortNames
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Descriptions
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Publishers
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Programmers
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Designers
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Graphics
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Music
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> SoundEffects
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Voices
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Documentation
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Artwork
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<MetadataDateTime> ReleaseDates
            {
                get { return Enumerable.Empty<MetadataDateTime>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Licenses
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> ContactInformation
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> Versions
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<MetadataDateTime> BuildDates
            {
                get { return Enumerable.Empty<MetadataDateTime>(); }
            }

            /// <inheritdoc />
            public override IEnumerable<string> AdditionalInformation
            {
                get { return Enumerable.Empty<string>(); }
            }

            /// <inheritdoc />
            public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
            {
                var added = !_crcs.ContainsKey(newCrc);
                if (added)
                {
                    _crcs[newCrc] = new CrcData(newCrc, crcDescription, incompatibilities);
                }
                return added;
            }
        }
    }
}

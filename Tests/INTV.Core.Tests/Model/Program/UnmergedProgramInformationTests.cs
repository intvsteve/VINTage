// <copyright file="UnmergedProgramInformationTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class UnmergedProgramInformationTests
    {
        private static readonly string TestCode = "coDe";
        private static readonly string TestTitle = "Episode What?";
        private static readonly string TestVendor = "Venn Door";
        private static readonly string TestYear = "MCMLXXVIII";
        private static readonly ProgramFeatures TestFeatures = (ProgramFeatures)new ProgramFeaturesBuilder().WithIntellivoiceCompatibility(FeatureCompatibility.Enhances).Build();
        private static readonly CrcData[] TestCrcDatas = new[]
        {
            new CrcData(1234u, "first", IncompatibilityFlags.Hive, 1),
            new CrcData(5678u, "second", IncompatibilityFlags.Hive | IncompatibilityFlags.Bee3, 2),
        };

        [Theory]
        [InlineData(NullCrcFields.Crcs)]
        [InlineData(NullCrcFields.Crcs | NullCrcFields.CrcDescriptions)]
        [InlineData(NullCrcFields.Crcs | NullCrcFields.CrcCfgs)]
        [InlineData(NullCrcFields.Crcs | NullCrcFields.CrcDescriptions | NullCrcFields.CrcCfgs)]
        [InlineData(NullCrcFields.CrcDescriptions)]
        [InlineData(NullCrcFields.CrcDescriptions | NullCrcFields.CrcCfgs)]
        [InlineData(NullCrcFields.CrcCfgs)]
        public void UnmergedProgramInformation_CreateWithNullCrcData_ThrowsNullReferenceException(NullCrcFields nullCrcFields)
        {
            Assert.Throws<NullReferenceException>(() => CreateTestData(true, nullCrcFields));
        }

        [Fact]
        public void UnmergedProgramInformation_CreateWithNoCrcDescriptions_UsesEmptyDescriptions()
        {
            var crcs = TestCrcDatas.Select(c => c.Crc).ToArray();
            var crcDescriptions = Enumerable.Empty<string>().ToArray();
            var crcCfgs = TestCrcDatas.Select(c => c.BinConfigTemplate).ToArray();

            var information = new UnmergedProgramInformation(TestCode, TestTitle, TestVendor, TestYear, crcs, crcDescriptions, crcCfgs, TestFeatures);

            Assert.All(information.Crcs, c => string.IsNullOrEmpty(c.Description));
        }

        [Fact]
        public void UnmergedProgramInformation_CreateWithTooFewCrcDescriptions_UsesEmptyDescriptionsForAdditionalCrcs()
        {
            var crcs = TestCrcDatas.Select(c => c.Crc).ToArray();
            var crcDescriptions = new[] { "one" };
            var crcCfgs = TestCrcDatas.Select(c => c.BinConfigTemplate).ToArray();

            var information = new UnmergedProgramInformation(TestCode, TestTitle, TestVendor, TestYear, crcs, crcDescriptions, crcCfgs, TestFeatures);

            Assert.Equal("one", information.Crcs.First().Description);
            Assert.True(string.IsNullOrEmpty(information.Crcs.Last().Description));
        }

        [Fact]
        public void UnmergedProgramInformation_CreateWithTooFewCrcCfgs_ThrowsIndexOutOfRangeException()
        {
            var crcs = TestCrcDatas.Select(c => c.Crc).ToArray();
            var crcDescriptions = TestCrcDatas.Select(c => c.Description).ToArray();
            var crcCfgs = new[] { 8 };
            Assert.Throws<IndexOutOfRangeException>(() => new UnmergedProgramInformation(TestCode, TestTitle, TestVendor, TestYear, crcs, crcDescriptions, crcCfgs, TestFeatures));
        }

        [Fact]
        public void UnmergedProgramInformation_InitializeWithCrcData_CorrectlyInitializesCrcData()
        {
            var information = CreateTestData(initialize: true);

            Assert.Equal(ProgramInformationOrigin.JzIntv, information.DataOrigin);
            Assert.Equal(TestTitle, information.Title);
            Assert.Equal(TestFeatures, information.Features);
            Assert.Null(information.ShortName);
            var expectedCrcs = TestCrcDatas.Select(c => c.Crc);
            Assert.Equal(expectedCrcs, information.Crcs.Select(c => c.Crc));
            var expectedCrcDescriptions = TestCrcDatas.Select(c => c.Description);
            Assert.Equal(expectedCrcDescriptions, information.Crcs.Select(c => c.Description));
            var expectedCrcCfgs = TestCrcDatas.Select(c => c.BinConfigTemplate);
            Assert.Equal(expectedCrcCfgs, information.Crcs.Select(c => c.BinConfigTemplate));
            Assert.Equal(TestVendor, information.Vendor);
            Assert.Equal(TestYear, information.Year);
            Assert.Equal(TestCode, information.Code);
        }

        [Fact]
        public void UnmergedProgramInformation_GetSetTitle_GetsAndSetsTitle()
        {
            var information = CreateTestData(initialize: true);

            var expectedTitle = "Something Wicked The Other Way Arrives";
            information.Title = expectedTitle;

            Assert.Equal(expectedTitle, information.Title);
        }

        [Fact]
        public void UnmergedProgramInformation_GetSetFeatures_GetsAndSetsFeatures()
        {
            var information = CreateTestData(initialize: true);

            var expectedFeatures = new ProgramFeaturesBuilder().WithIntellivisionIICompatibility(FeatureCompatibility.Enhances).WithEcsFeatures(EcsFeatures.Enhances).Build();
            information.Features = (ProgramFeatures)expectedFeatures;

            Assert.Equal(expectedFeatures, information.Features);
        }

        [Fact]
        public void UnmergedProgramInformation_GetSetShortName_GetsAndSetsShortName()
        {
            var information = CreateTestData(initialize: true);

            var expectedShortName = "Not the shortest";
            information.ShortName = expectedShortName;

            Assert.Equal(expectedShortName, information.ShortName);
        }

        [Fact]
        public void UnmergedProgramInformation_GetSetVendor_GetsAndSetsVendor()
        {
            var information = CreateTestData(initialize: true);

            var expectedVendor = "Slovenly Sales, Inc.";
            information.Vendor = expectedVendor;

            Assert.Equal(expectedVendor, information.Vendor);
        }

        [Fact]
        public void UnmergedProgramInformation_GetSetYear_GetsAndSetsYear()
        {
            var information = CreateTestData(initialize: true);

            var expectedYear = "1987";
            information.Year = expectedYear;

            Assert.Equal(expectedYear, information.Year);
        }

        [Fact]
        public void UnmergedProgramInformation_LongNames_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.LongNames);
        }

        [Fact]
        public void UnmergedProgramInformation_ShortNames_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.ShortNames);
        }

        [Fact]
        public void UnmergedProgramInformation_Descriptions_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Descriptions);
        }

        [Fact]
        public void UnmergedProgramInformation_Publishers_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Publishers);
        }

        [Fact]
        public void UnmergedProgramInformation_Programmers_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Programmers);
        }

        [Fact]
        public void UnmergedProgramInformation_Designers_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Designers);
        }

        [Fact]
        public void UnmergedProgramInformation_Graphics_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Graphics);
        }

        [Fact]
        public void UnmergedProgramInformation_Music_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Music);
        }

        [Fact]
        public void UnmergedProgramInformation_SoundEffects_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.SoundEffects);
        }

        [Fact]
        public void UnmergedProgramInformation_Voices_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Voices);
        }

        [Fact]
        public void UnmergedProgramInformation_Documentation_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Documentation);
        }

        [Fact]
        public void UnmergedProgramInformation_Artwork_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Artwork);
        }

        [Fact]
        public void UnmergedProgramInformation_ReleaseDates_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.ReleaseDates);
        }

        [Fact]
        public void UnmergedProgramInformation_Licenses_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Licenses);
        }

        [Fact]
        public void UnmergedProgramInformation_ContactInformation_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.ContactInformation);
        }

        [Fact]
        public void UnmergedProgramInformation_Versions_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.Versions);
        }

        [Fact]
        public void UnmergedProgramInformation_BuildDates_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.BuildDates);
        }

        [Fact]
        public void UnmergedProgramInformation_AdditionalInformation_IsEmpty()
        {
            var information = CreateTestData(initialize: true);

            Assert.Empty(information.AdditionalInformation);
        }

        [Fact]
        public void UnmergedProgramInformation_AddCrc_ThrowsInvalidOperationException()
        {
            var information = CreateTestData(initialize: true);

            Assert.Throws<InvalidOperationException>(() => information.AddCrc(9u, "oof", IncompatibilityFlags.None));
        }

        private static UnmergedProgramInformation CreateTestData(bool initialize, NullCrcFields nullCrcFields = NullCrcFields.None)
        {
            var code = initialize ? TestCode : null;
            var title = initialize ? TestTitle : null;
            var vendor = initialize ? "Venn Door" : null;
            var year = initialize ? "MCMLXXVIII" : null;
            var crcs = nullCrcFields.HasFlag(NullCrcFields.Crcs) ? null : TestCrcDatas.Select(c => c.Crc).ToArray();
            var crcDescriptions = nullCrcFields.HasFlag(NullCrcFields.CrcDescriptions) ? null : TestCrcDatas.Select(c => c.Description).ToArray();
            var crcCfgs = nullCrcFields.HasFlag(NullCrcFields.CrcCfgs) ? null : TestCrcDatas.Select(c => c.BinConfigTemplate).ToArray();
            var features = initialize ? TestFeatures : null;

            var information = new UnmergedProgramInformation(code, title, vendor, year, crcs, crcDescriptions, crcCfgs, features);

            return information;
        }
    }

    [Flags]
    public enum NullCrcFields
    {
        /// <summary>
        /// Don't use any null values for CRC-related constructor arguments.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use a null value for crcs constructor argument.
        /// </summary>
        Crcs = 1 << 0,

        /// <summary>
        /// Use a null value for crcDescriptions constructor argument.
        /// </summary>
        CrcDescriptions = 1 << 1,

        /// <summary>
        /// Use a null value for crcCfgs constructor argument.
        /// </summary>
        CrcCfgs = 1 << 2
    }
}

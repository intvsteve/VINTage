// <copyright file="XmlRomInformationToProgramRomInformationConverterTests.cs" company="INTV Funhouse">
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

namespace INTV.Core.Tests.Model.Program
{
    public class XmlRomInformationToProgramRomInformationConverterTests
    {
        /// <summary>
        /// Default values for database columns. Values not defined assumed to be empty (null) string.
        /// </summary>
        private static readonly Dictionary<XmlRomInformationDatabaseColumnName, string> XmlRomInformationColumnValues = new Dictionary<XmlRomInformationDatabaseColumnName, string>()
        {
            { XmlRomInformationDatabaseColumnName.crc, "3971627767" },
            { XmlRomInformationDatabaseColumnName.crc_2, "3696045007" },
            { XmlRomInformationDatabaseColumnName.platform, "Intellivision" },
            { XmlRomInformationDatabaseColumnName.type, "Program" },
            { XmlRomInformationDatabaseColumnName.ntsc, "2" },
            { XmlRomInformationDatabaseColumnName.pal, "2" },
            { XmlRomInformationDatabaseColumnName.general_features, "2" },
            { XmlRomInformationDatabaseColumnName.kc, "2" },
            { XmlRomInformationDatabaseColumnName.sva, "2" },
            { XmlRomInformationDatabaseColumnName.ivoice, "2" },
            { XmlRomInformationDatabaseColumnName.intyii, "2" },
            { XmlRomInformationDatabaseColumnName.ecs, "2" },
            { XmlRomInformationDatabaseColumnName.tutor, "2" },
            { XmlRomInformationDatabaseColumnName.icart, "2" },
            { XmlRomInformationDatabaseColumnName.cc3, "2" },
            { XmlRomInformationDatabaseColumnName.jlp, "2" },
            { XmlRomInformationDatabaseColumnName.jlp_savegame, "1" },
            { XmlRomInformationDatabaseColumnName.lto_flash, "1" },
            { XmlRomInformationDatabaseColumnName.bee3, "1" },
            { XmlRomInformationDatabaseColumnName.hive, "1" },
            { XmlRomInformationDatabaseColumnName.box_variant, "-1" },
            { XmlRomInformationDatabaseColumnName.screenshot, "-1" },
            { XmlRomInformationDatabaseColumnName.code, "talt" },
            { XmlRomInformationDatabaseColumnName.title, "Tag-A-Long-Tod XML: The Full Monty" },
            { XmlRomInformationDatabaseColumnName.short_name, "Tag-A-Long-Tod XML" },
            { XmlRomInformationDatabaseColumnName.vendor, "Spatula City" },
            { XmlRomInformationDatabaseColumnName.name, "Tutorial 2" },
            { XmlRomInformationDatabaseColumnName.format, XmlRomInformationDatabaseColumn.RomFormatValueBin },
            { XmlRomInformationDatabaseColumnName.origin, XmlRomInformationDatabaseColumn.OriginOther },
            { XmlRomInformationDatabaseColumnName.description, "A tutorial to help one to learn CP-1600 assembly programming." },
            { XmlRomInformationDatabaseColumnName.release_date, "1998-06-12" },
            { XmlRomInformationDatabaseColumnName.source, "http://spatula-city.org" },
            { XmlRomInformationDatabaseColumnName.program, "INTV|nut" },
            { XmlRomInformationDatabaseColumnName.concept, "who|knows" },
            { XmlRomInformationDatabaseColumnName.game_graphics, "Joe|Z" },
            { XmlRomInformationDatabaseColumnName.soundfx, "Un|known" },
            { XmlRomInformationDatabaseColumnName.music, "Un|known" },
            { XmlRomInformationDatabaseColumnName.voices, "in|my|head" },
            { XmlRomInformationDatabaseColumnName.game_docs, "your|pay" },
            { XmlRomInformationDatabaseColumnName.box_art, "he|has|a|glass|jaw" },
            { XmlRomInformationDatabaseColumnName.build_date, "2012-12-12" },
            { XmlRomInformationDatabaseColumnName.other, "Overlays: N/A|Cart shell: N/A" },
            { XmlRomInformationDatabaseColumnName.license, "plate" },
            { XmlRomInformationDatabaseColumnName.contact_info, "in.sane@asylumni.net" },
            { XmlRomInformationDatabaseColumnName.bin_cfg, "[mapping]\n$0000 - $01FF = $5000\n[vars]title=\"boo hoo\"" },
        };

        private static readonly TestProgramInformation XmlProgramInformation = new TestProgramInformation()
        {
            Title = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.title],
            Vendor = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.vendor],
            Year = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.release_date].Split('-').First(),
            ShortName = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.short_name],
        };

        private static readonly TestProgramDescription XmlProgramDescription = new TestProgramDescription()
        {
            Name = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.title],
            ShortName = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.short_name],
            Vendor = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.vendor],
            Year = XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.release_date].Split('-').First(),
        };

        private static readonly Lazy<IProgramFeatures> XmlProgramFeatures = new Lazy<IProgramFeatures>(CreateXmlRomFeatures);

        private static readonly Lazy<IProgramMetadata> XmlProgramMetadata = new Lazy<IProgramMetadata>(CreateXmlRomInformationMetadata);

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertNullXmlInfo_ThrowsNullReferenceException()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.None);

            Assert.Throws<NullReferenceException>(() => converter.Convert(xmlRomInformation));
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithNullDatabaseAndConvertNullXmlInfo_ThrowsNullReferenceException()
        {
            XmlRomInformationToProgramRomInformationConverterTestProgramDatabase database = null;
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(database);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.None);

            Assert.Throws<NullReferenceException>(() => converter.Convert(xmlRomInformation));
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithNullDescriptionsSourceAndConvertNullXmlInfo_ThrowsNullReferenceException()
        {
            IEnumerable<ProgramDescription> descriptionsSource = null;
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.None);

            Assert.Throws<NullReferenceException>(() => converter.Convert(xmlRomInformation));
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfoWithNoColumns_ThrowsInvalidOperationException()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.NoColumns);

            Assert.Throws<InvalidOperationException>(() => converter.Convert(xmlRomInformation));
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfoWithAllEmptyColumns_ThrowsInvalidOperationException()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.EmptyColumns);

            Assert.Throws<InvalidOperationException>(() => converter.Convert(xmlRomInformation));
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfo_ProducesValidConversion()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(XmlProgramInformation, convertedInformation);
            ValidateDescription(XmlProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithEmptyDatabaseAndConvertXmlInfo_ProducesValidConversion()
        {
            var database = new XmlRomInformationToProgramRomInformationConverterTestProgramDatabase();
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(database);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(XmlProgramInformation, convertedInformation);
            ValidateDescription(XmlProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDatabaseContaingMatchAndConvertXmlInfo_ProducesValidConversion()
        {
            var database = new XmlRomInformationToProgramRomInformationConverterTestProgramDatabase();
            var testProgramInformation = new TestProgramInformation();
            database.AddProgram(testProgramInformation);
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(database);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramInformation, convertedInformation);
            ValidateDescription(testProgramInformation.ToDescription(), convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDatabaseContaingMatchWithDefaultFeaturesAndConvertXmlInfo_ProducesValidConversion()
        {
            var database = new XmlRomInformationToProgramRomInformationConverterTestProgramDatabase();
            var testProgramInformation = new TestProgramInformation() { Features = ProgramFeatures.DefaultFeatures.Clone() };
            database.AddProgram(testProgramInformation);
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(database);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramInformation, convertedInformation);
            ValidateDescription(testProgramInformation.ToDescription(), convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDatabaseContaingMatchWithNullTitleAndConvertXmlInfo_ProducesValidConversion()
        {
            var database = new XmlRomInformationToProgramRomInformationConverterTestProgramDatabase();
            var testProgramInformation = new TestProgramInformation() { Title = null };
            database.AddProgram(testProgramInformation);
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(database);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);
            var expectedProgramInformation = new TestProgramInformation(testProgramInformation) { Title = XmlProgramInformation.Title };

            ValidateInformation(expectedProgramInformation, convertedInformation);
            ValidateDescription(expectedProgramInformation.ToDescription(), convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithEmptyDescriptionsSourceAndConvertXmlInfo_ProducesValidConversion()
        {
            var descriptionsSource = Enumerable.Empty<ProgramDescription>();
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(XmlProgramInformation, convertedInformation);
            ValidateDescription(XmlProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDescriptionsSourceContainingMatchAndConvertXmlInfo_ProducesValidConversion()
        {
            var testProgramDescription = new TestProgramDescription();
            var descriptionsSource = new List<IProgramDescription>() { testProgramDescription };
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramDescription.ToInformation(), convertedInformation);
            ValidateDescription(testProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDescriptionsSourceContainingMatchWithInfoAndConvertXmlInfo_ProducesValidConversion()
        {
            var testProgramInformation = new TestProgramInformation();
            var testProgramDescription = new TestProgramDescription(testProgramInformation);
            var descriptionsSource = new List<IProgramDescription>() { testProgramDescription };
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramDescription.ToInformation(), convertedInformation);
            ValidateDescription(testProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDescriptionsSourceContainingMatchWithDefaultFeaturesAndConvertXmlInfo_ProducesValidConversion()
        {
            var testProgramDescription = new TestProgramDescription() { Features = ProgramFeatures.DefaultFeatures.Clone() };
            var descriptionsSource = new List<IProgramDescription>() { testProgramDescription };
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramDescription.ToInformation(), convertedInformation);
            ValidateDescription(testProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDescriptionsSourceContainingMatchWithNullTitleAndConvertXmlInfo_ProducesValidConversion()
        {
            var testProgramDescription = new TestProgramDescription() { Name = null };
            var descriptionsSource = new List<IProgramDescription>() { testProgramDescription };
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);
            var expectedProgramDescription = new TestProgramDescription(testProgramDescription) { Name = XmlProgramDescription.Name };

            ValidateInformation(expectedProgramDescription.ToInformation(), convertedInformation);
            ValidateDescription(expectedProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDescriptionsSourceContainingMatchWithRomWithoutMetadatdaAndConvertXmlInfo_ProducesValidConversion()
        {
            var romPaths = XmlRomInformationToProgramRomInformationConverterTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            Assert.NotNull(rom);
            var testProgramDescription = new TestProgramDescription(rom);
            var descriptionsSource = new List<IProgramDescription>() { testProgramDescription };
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramDescription.ToInformation(), convertedInformation);
            ValidateDescription(testProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateWithDescriptionsSourceContainingMatchWithRomWithMetadatdaAndConvertXmlInfo_ProducesValidConversion()
        {
            var romPaths = XmlRomInformationToProgramRomInformationConverterTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            Assert.NotNull(rom);
            var testProgramDescription = new TestProgramDescription(rom);
            var descriptionsSource = new List<IProgramDescription>() { testProgramDescription };
            var converter = XmlRomInformationToProgramRomInformationConverter.Create(descriptionsSource);

            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            var convertedInformation = converter.Convert(xmlRomInformation);

            ValidateInformation(testProgramDescription.ToInformation(), convertedInformation);
            ValidateDescription(testProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(XmlProgramMetadata.Value, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfoWithMultipleCorruptions_ProducesValidConversion()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();

            var columnsToCorrupt = new[]
            {
                XmlRomInformationDatabaseColumnName.vendor,
                XmlRomInformationDatabaseColumnName.short_name,
                XmlRomInformationDatabaseColumnName.release_date,
                XmlRomInformationDatabaseColumnName.soundfx,
                XmlRomInformationDatabaseColumnName.game_docs
            };
            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated, columnsToCorrupt);
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.build_date).Value = "2020"; // corrupted because it's not YYYY-MM-DD
            var convertedInformation = converter.Convert(xmlRomInformation);
            var builder = new ProgramMetadataBuilder().WithInitialMetadata(XmlProgramMetadata.Value);
            builder.WithPublishers(null).WithShortNames(null).WithReleaseDates(null).WithSoundEffects(null).WithDocumentation(null).WithBuildDates(null);
            var expectedMetadata = builder.Build();
            var expectedProgramInformation = new TestProgramInformation(XmlProgramInformation) { Vendor = null, ShortName = null, Year = null };
            var expectedProgramDescription = new TestProgramDescription(XmlProgramDescription) { Vendor = null, ShortName = null, Year = null };

            ValidateInformation(expectedProgramInformation, convertedInformation);
            ValidateDescription(expectedProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(expectedMetadata, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfoWithStringMetadataCorruption_ProducesValidConversion()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();
            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.program).Value = "| |\n|\t|\r | "; // corruption results in empty metadata
            var convertedInformation = converter.Convert(xmlRomInformation);
            var builder = new ProgramMetadataBuilder().WithInitialMetadata(XmlProgramMetadata.Value);
            var expectedMetadata = builder.WithProgrammers(null).Build();

            ValidateInformation(XmlProgramInformation, convertedInformation);
            ValidateDescription(XmlProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(expectedMetadata, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfoWithPartialReleaseDate_ProducesValidConversion()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();
            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.release_date).Value = "2020"; // corrupted because it's not YYYY-MM-DD
            var convertedInformation = converter.Convert(xmlRomInformation);
            var builder = new ProgramMetadataBuilder().WithInitialMetadata(XmlProgramMetadata.Value);
            var expectedMetadata = builder.WithReleaseDates(null).Build();
            var expectedProgramInformation = new TestProgramInformation(XmlProgramInformation) { Year = null };
            var expectedProgramDescription = new TestProgramDescription(XmlProgramDescription) { Year = null };

            ValidateInformation(expectedProgramInformation, convertedInformation);
            ValidateDescription(expectedProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(expectedMetadata, convertedInformation.Metadata);
        }

        [Fact]
        public void XmlRomInformationToProgramRomInformationConverter_CreateDefaultAndConvertXmlInfoWithZeroReleaseDate_ProducesValidConversion()
        {
            var converter = XmlRomInformationToProgramRomInformationConverter.Create();
            var xmlRomInformation = CreateTestInformationForConversion(XmlInformationKind.FullyPopulated);
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.release_date).Value = "0000-00-00"; // corrupted because it's not valid, but is correct format
            var convertedInformation = converter.Convert(xmlRomInformation);
            var builder = new ProgramMetadataBuilder().WithInitialMetadata(XmlProgramMetadata.Value);
            var expectedMetadata = builder.WithReleaseDates(null).Build();
            var expectedProgramInformation = new TestProgramInformation(XmlProgramInformation) { Year = null };
            var expectedProgramDescription = new TestProgramDescription(XmlProgramDescription) { Year = null };

            ValidateInformation(expectedProgramInformation, convertedInformation);
            ValidateDescription(expectedProgramDescription, convertedInformation);
            ValidateFeatures(XmlProgramFeatures.Value, convertedInformation.Features);
            ValidateMetadata(expectedMetadata, convertedInformation.Metadata);
        }

        private static void ValidateInformation(IProgramInformation expectedInformation, IProgramRomInformation actualInformation)
        {
            Assert.Equal(expectedInformation.Title, actualInformation.Title);
            Assert.Equal(expectedInformation.Vendor, actualInformation.Vendor);
            Assert.Equal(expectedInformation.Year, actualInformation.Year);
            Assert.Equal(expectedInformation.ShortName, actualInformation.ShortName);
        }

        private static void ValidateDescription(IProgramDescription expectedDescription, IProgramRomInformation actualInformation)
        {
            Assert.Equal(expectedDescription.Name, actualInformation.Title);
            Assert.Equal(expectedDescription.ShortName, actualInformation.ShortName);
            Assert.Equal(expectedDescription.Vendor, actualInformation.Vendor);
            Assert.Equal(expectedDescription.Year, actualInformation.Year);
        }

        private static void ValidateFeatures(IProgramFeatures expectedProgramFeatures, IProgramFeatures actualProgramFeatures)
        {
            var compareResult = expectedProgramFeatures.CompareTo(actualProgramFeatures);
            Assert.Equal(0, compareResult);
        }

        private static void ValidateMetadata(IProgramMetadata expectedMetadata, IProgramMetadata actualMetadata)
        {
            AssertCollectionsEquivalent(expectedMetadata.LongNames, actualMetadata.LongNames, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.ShortNames, actualMetadata.ShortNames, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Descriptions, actualMetadata.Descriptions, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Publishers, actualMetadata.Publishers, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Programmers, actualMetadata.Programmers, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Designers, actualMetadata.Designers, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Graphics, actualMetadata.Graphics, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Music, actualMetadata.Music, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.SoundEffects, actualMetadata.SoundEffects, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Voices, actualMetadata.Voices, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Documentation, actualMetadata.Documentation, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Artwork, actualMetadata.Artwork, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.ReleaseDates, actualMetadata.ReleaseDates);
            AssertCollectionsEquivalent(expectedMetadata.Licenses, actualMetadata.Licenses, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.ContactInformation, actualMetadata.ContactInformation, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.Versions, actualMetadata.Versions, StringComparer.OrdinalIgnoreCase);
            AssertCollectionsEquivalent(expectedMetadata.BuildDates, actualMetadata.BuildDates);
            AssertCollectionsEquivalent(expectedMetadata.AdditionalInformation, actualMetadata.AdditionalInformation, StringComparer.OrdinalIgnoreCase);
        }

        private static void AssertCollectionsEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.False(expected.Except(actual).Any());
            Assert.False(actual.Except(expected).Any());
        }

        private static void AssertCollectionsEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            Assert.False(expected.Except(actual, comparer).Any());
            Assert.False(actual.Except(expected, comparer).Any());
        }

        private static XmlRomInformation CreateTestInformationForConversion(XmlInformationKind xmlInformationKind, params XmlRomInformationDatabaseColumnName[] columsToNullOut)
        {
            XmlRomInformation xmlInformation = null;

            switch (xmlInformationKind)
            {
                case XmlInformationKind.None:
                    break;
                case XmlInformationKind.NoColumns:
                    xmlInformation = new XmlRomInformation();
                    break;
                case XmlInformationKind.EmptyColumns:
                    xmlInformation = XmlRomInformation.CreateDefault();
                    break;
                case XmlInformationKind.FullyPopulated:
                    xmlInformation = CreateFullyPopulatedXmlRomInformation(columsToNullOut);
                    break;
            }

            return xmlInformation;
        }

        private static XmlRomInformation CreateFullyPopulatedXmlRomInformation(XmlRomInformationDatabaseColumnName[] columsToNullOut)
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();

            foreach (var columnValue in XmlRomInformationColumnValues)
            {
                xmlRomInformation.GetColumn(columnValue.Key).Value = columnValue.Value;
            }
            foreach (var columnToNullOut in columsToNullOut)
            {
                xmlRomInformation.GetColumn(columnToNullOut).Value = null;
            }

            return xmlRomInformation;
        }

        private static IProgramFeatures CreateXmlRomFeatures()
        {
            var builder = new ProgramFeaturesBuilder();

            builder.WithGeneralFeatures((GeneralFeatures)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.general_features]));
            builder.WithNtscCompatibility((FeatureCompatibility)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.ntsc]));
            builder.WithPalCompatibility((FeatureCompatibility)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.pal]));
            builder.WithKeyboardComponentFeatures((KeyboardComponentFeatures)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.kc]));
            builder.WithSuperVideoArcadeCompatibility((FeatureCompatibility)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.sva]));
            builder.WithIntellivoiceCompatibility((FeatureCompatibility)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.ivoice]));
            builder.WithIntellivisionIICompatibility((FeatureCompatibility)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.intyii]));
            builder.WithEcsFeatures((EcsFeatures)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.ecs]));
            builder.WithTutorvisionCompatibility((FeatureCompatibility)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.tutor]));
            builder.WithIntellicartFeatures((IntellicartCC3Features)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.icart]));
            builder.WithCuttleCart3Features((CuttleCart3Features)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.cc3]));
            builder.WithJlpFeatures((JlpFeatures)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.jlp]));
            builder.WithMinimumFlashSectors(ushort.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.jlp_savegame]));
            builder.WithLtoFlashFeatures((LtoFlashFeatures)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.lto_flash]));
            builder.WithBee3Features((Bee3Features)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.bee3]));
            builder.WithHiveFeatures((HiveFeatures)uint.Parse(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.hive]));

            return builder.Build();
        }

        private static IProgramMetadata CreateXmlRomInformationMetadata()
        {
            var builder = new ProgramMetadataBuilder();

            builder.WithLongNames(new[] { XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.title] });
            builder.WithShortNames(new[] { XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.short_name] });
            builder.WithPublishers(new[] { XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.vendor] });
            builder.WithVersions(new[] { XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.name] });
            builder.WithDescriptions(new[] { XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.description] });
            builder.WithProgrammers(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.program].Split(new[] { '|' }));
            builder.WithDesigners(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.concept].Split(new[] { '|' }));
            builder.WithGraphics(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.game_graphics].Split(new[] { '|' }));
            builder.WithMusic(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.music].Split(new[] { '|' }));
            builder.WithSoundEffects(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.soundfx].Split(new[] { '|' }));
            builder.WithVoices(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.voices].Split(new[] { '|' }));
            builder.WithDocumentation(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.game_docs].Split(new[] { '|' }));
            builder.WithArtwork(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.box_art].Split(new[] { '|' }));
            builder.WithReleaseDates(new[] { new MetadataDateTimeBuilder(1998).WithMonth(6).WithDay(12).Build() });
            builder.WithLicenses(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.license].Split(new[] { '|' }));
            builder.WithContactInformation(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.contact_info].Split(new[] { '|' }));
            builder.WithAdditionalInformation(new List<string>(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.source].Split(new[] { '|' })).Concat(XmlRomInformationColumnValues[XmlRomInformationDatabaseColumnName.other].Split(new[] { '|' })));
            builder.WithBuildDates(new[] { new MetadataDateTimeBuilder(2012).WithMonth(12).WithDay(12).Build() });

            return builder.Build();
        }

        private enum XmlInformationKind
        {
            /// <summary>Do not create any XmlInformation.</summary>
            None,

            /// <summary>Create XmlInformation without any columns.</summary>
            NoColumns,

            /// <summary>Create XmlInformation with all column values set to an empty or <c>null</c> string.</summary>
            EmptyColumns,

            /// <summary>Create XmlInformation with fully populated column data.</summary>
            FullyPopulated
        }

        private class XmlRomInformationToProgramRomInformationConverterTestProgramDatabase : IProgramInformationTable
        {
            public XmlRomInformationToProgramRomInformationConverterTestProgramDatabase()
            {
            }

            #region IProgramInformationTable

            /// <inheritdoc />
            public IEnumerable<IProgramInformation> Programs
            {
                get { return _programs; }
            }
            private List<IProgramInformation> _programs = new List<IProgramInformation>();

            /// <inheritdoc />
            public IProgramInformation FindProgram(uint crc)
            {
                return _programs.FirstOrDefault(p => p.Crcs.Select(c => c.Crc).Contains(crc));
            }

            /// <inheritdoc />
            public IProgramInformation FindProgram(ProgramIdentifier programIdentifier)
            {
                return FindProgram(programIdentifier.DataCrc);
            }

            #endregion // IProgramInformationTable

            public void AddProgram(IProgramInformation programInformation)
            {
                _programs.Add(programInformation);
            }
        }

        private class TestProgramInformation : IProgramInformation
        {
            public TestProgramInformation()
            {
                Title = "Tag-a-long Todd Information for the XmlRomInformationToProgramRomInformationConverter Tests";
                Vendor = "XmlRomInformationToProgramRomInformationConverter Test Information";
                Year = "2017";
                ShortName = "Tagger I";
            }

            public TestProgramInformation(TestProgramInformation otherInformation)
            {
                Title = otherInformation.Title;
                Vendor = otherInformation.Vendor;
                Year = otherInformation.Year;
                ShortName = otherInformation.ShortName;
                Features = otherInformation.Features;
            }

            public ProgramInformationOrigin DataOrigin
            {
                get { return ProgramInformationOrigin.None; }
            }

            public string Title { get; set; }

            public string Vendor { get; set; }

            public string Year { get; set; }

            public ProgramFeatures Features { get; set; }

            public string ShortName { get; set; }

            public IEnumerable<CrcData> Crcs
            {
                get { return new[] { new CrcData(TestRomResources.TestBinCrc, "Duuude") }; }
            }

            public bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilityFlags)
            {
                throw new NotImplementedException();
            }

            public bool ModifyCrc(uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags)
            {
                throw new NotImplementedException();
            }

            public TestProgramDescription ToDescription()
            {
                var description = new TestProgramDescription()
                {
                    Name = Title,
                    ShortName = ShortName,
                    Vendor = Vendor,
                    Year = Year
                };
                return description;
            }
        }

        private class TestProgramDescription : IProgramDescription
        {
            public TestProgramDescription()
            {
                Name = "Tag-a-long Todd Description for the XmlRomInformationToProgramRomInformationConverter Tests";
                ShortName = "Tagger D";
                Vendor = "XmlRomInformationToProgramRomInformationConverter Test Description";
                Year = "2018";
            }

            public TestProgramDescription(TestProgramDescription otherDescription)
            {
                Name = otherDescription.Name;
                ShortName = otherDescription.ShortName;
                Vendor = otherDescription.Vendor;
                Year = otherDescription.Year;
                Features = otherDescription.Features;
            }

            public TestProgramDescription(TestProgramInformation programInformation)
                : this()
            {
                ProgramInformation = programInformation;
            }

            public TestProgramDescription(IRom rom)
                : this()
            {
                Rom = rom;
            }

            public uint Crc
            {
                get { return TestRomResources.TestBinCrc; }
            }

            public IRom Rom { get; private set; }

            public IProgramInformation ProgramInformation { get; private set; }

            public string Name { get; set; }

            public string ShortName { get; set; }

            public string Vendor { get; set; }

            public string Year { get; set; }

            public ProgramFeatures Features { get; set; }

            public TestProgramInformation ToInformation()
            {
                var information = new TestProgramInformation()
                {
                    Title = Name,
                    Vendor = Vendor,
                    Year = Year,
                    ShortName = ShortName,
                };
                return information;
            }
        }

        private class XmlRomInformationToProgramRomInformationConverterTestStorageAccess : CachedResourceStorageAccess<XmlRomInformationToProgramRomInformationConverterTestStorageAccess>
        {
        }
    }
}

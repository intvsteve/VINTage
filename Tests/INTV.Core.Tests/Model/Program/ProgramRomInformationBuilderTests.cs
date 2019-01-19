// <copyright file="ProgramRomInformationBuilderTests.cs" company="INTV Funhouse">
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
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramRomInformationBuilderTests
    {
        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNoFieldsSet_ThrowsInvalidOperationExceptionContainingMissingRequiredFields()
        {
            var builder = new ProgramRomInformationBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            ValidateExceptionMessageContainsFieldNames(exception.Message, new[] { "Id", "Format", "Title", "Features", "Metadata" });
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNoIdField_ThrowsInvalidOperationExceptionContainingMissingRequiredField()
        {
            var builder = new ProgramRomInformationBuilder()
                .WithFormat(RomFormat.Bin)
                .WithTitle("The Title")
                .WithFeatures(ProgramFeatures.DefaultFeatures)
                .WithMetadata(new ProgramMetadataBuilder().Build());

            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            ValidateExceptionMessageContainsFieldNames(exception.Message, new[] { "Id" });
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNoFormatField_ThrowsInvalidOperationExceptionContainingMissingRequiredField()
        {
            var builder = new ProgramRomInformationBuilder()
                .WithId(new ProgramIdentifier(1u, 2u))
                .WithTitle("The Title")
                .WithFeatures(ProgramFeatures.DefaultFeatures)
                .WithMetadata(new ProgramMetadataBuilder().Build());

            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            ValidateExceptionMessageContainsFieldNames(exception.Message, new[] { "Format" });
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNoTitleField_ThrowsInvalidOperationExceptionContainingMissingRequiredField()
        {
            var builder = new ProgramRomInformationBuilder()
                .WithId(new ProgramIdentifier(3u, 4u))
                .WithFormat(RomFormat.Bin)
                .WithFeatures(ProgramFeatures.DefaultFeatures)
                .WithMetadata(new ProgramMetadataBuilder().Build());

            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            ValidateExceptionMessageContainsFieldNames(exception.Message, new[] { "Title" });
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNoFeaturesField_ThrowsInvalidOperationExceptionContainingMissingRequiredField()
        {
            var builder = new ProgramRomInformationBuilder()
                .WithId(new ProgramIdentifier(5u, 6u))
                .WithFormat(RomFormat.Bin)
                .WithTitle("The Title")
                .WithMetadata(new ProgramMetadataBuilder().Build());

            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            ValidateExceptionMessageContainsFieldNames(exception.Message, new[] { "Features" });
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNoMetadataField_ThrowsInvalidOperationExceptionContainingMissingRequiredField()
        {
            var builder = new ProgramRomInformationBuilder()
                .WithId(new ProgramIdentifier(7u, 8u))
                .WithFormat(RomFormat.Bin)
                .WithTitle("The Title")
                .WithFeatures(ProgramFeatures.DefaultFeatures);

            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            ValidateExceptionMessageContainsFieldNames(exception.Message, new[] { "Metadata" });
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithNullTitle_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new ProgramRomInformationBuilder().WithTitle(null));
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithEmptyTitle_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new ProgramRomInformationBuilder().WithTitle(string.Empty));
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithTitle_ResultsInExpectedTitle()
        {
            var builder = CreateBuilderWithRequiredFields("x");

            var expectedTitle = "Set the Title!";
            var programRomInformation = builder.WithTitle(expectedTitle).Build();

            Assert.Equal(expectedTitle, programRomInformation.Title);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithVendor_ResultsInExpectedVendor()
        {
            var builder = CreateBuilderWithRequiredFields("Testing Vendor");

            var expectedVendor = "Build & Sell";
            var programRomInformation = builder.WithVendor(expectedVendor).Build();

            Assert.Equal(expectedVendor, programRomInformation.Vendor);
        }

        [Theory]
        [InlineData(0, null)]
        [InlineData(1899, null)]
        [InlineData(1900, "1900")]
        [InlineData(1980, "1980")]
        [InlineData(2155, "2155")]
        [InlineData(2156, null)]
        public void ProgramRomInformationBuilder_BuildWithYear_ResultsInExpectedYear(int year, string expectedYear)
        {
            var builder = CreateBuilderWithRequiredFields("Testing Year");

            var programRomInformation = builder.WithYear(year).Build();

            Assert.Equal(expectedYear, programRomInformation.Year);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithLongName_ResultsInExpectedLongName()
        {
            var builder = CreateBuilderWithRequiredFields("Testing LongName");

            var expectedLongName = "Supercal... you know the drill. A long name!";
            var programRomInformation = builder.WithLongName(expectedLongName).Build();

            Assert.Equal(expectedLongName, programRomInformation.LongName);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithShortName_ResultsInExpectedShortName()
        {
            var builder = CreateBuilderWithRequiredFields("Testing ShortName");

            var expectedShortName = "ShtName";
            var programRomInformation = builder.WithShortName(expectedShortName).Build();

            Assert.Equal(expectedShortName, programRomInformation.ShortName);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithVariantName_ResultsInExpectedVariantName()
        {
            var builder = CreateBuilderWithRequiredFields("Testing VariantName");

            var expectedVariantName = "Patched";
            var programRomInformation = builder.WithVariantName(expectedVariantName).Build();

            Assert.Equal(expectedVariantName, programRomInformation.VariantName);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithFormat_ResultsInExpectedFormat()
        {
            var builder = CreateBuilderWithRequiredFields("Testing Format");

            var expectedFormat = RomFormat.Luigi;
            var programRomInformation = builder.WithFormat(expectedFormat).Build();

            Assert.Equal(expectedFormat, programRomInformation.Format);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithId_ResultsInExpectedId()
        {
            var builder = CreateBuilderWithRequiredFields("Testing Id");

            var expectedId = new ProgramIdentifier(4u, 8u);
            var programRomInformation = builder.WithId(expectedId).Build();

            Assert.Equal(expectedId, programRomInformation.Id);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithFeatures_ResultsInExpectedFeatures()
        {
            var builder = CreateBuilderWithRequiredFields("Testing Features");

            var expectedFeatures = new ProgramFeaturesBuilder().WithTutorvisionCompatibility(FeatureCompatibility.Requires).Build();
            var programRomInformation = builder.WithFeatures(expectedFeatures).Build();

            Assert.Equal(expectedFeatures, programRomInformation.Features);
        }

        [Fact]
        public void ProgramRomInformationBuilder_BuildWithMetadata_ResultsInExpectedMetadata()
        {
            var builder = CreateBuilderWithRequiredFields("Testing Metadata");

            var expectedMetadata = new ProgramMetadataBuilder().WithDescriptions(new[] { "Itza kool game" }).Build();
            var programRomInformation = builder.WithMetadata(expectedMetadata).Build();

            Assert.Equal(expectedMetadata, programRomInformation.Metadata);
        }

        private void ValidateExceptionMessageContainsFieldNames(string exceptionMessage, IEnumerable<string> expectedMissingFields)
        {
            var knownFieldsInExceptionMessage = new[] { "Id", "Format", "Title", "Features", "Metadata" };
            foreach (var expectedMissingField in knownFieldsInExceptionMessage.Intersect(expectedMissingFields))
            {
                Assert.True(exceptionMessage.Contains(expectedMissingField));
            }
            foreach (var shouldNotBePresentField in knownFieldsInExceptionMessage.Except(expectedMissingFields))
            {
                Assert.False(exceptionMessage.Contains(shouldNotBePresentField));
            }
        }

        private IProgramRomInformationBuilder CreateBuilderWithRequiredFields(string title)
        {
            var builder = new ProgramRomInformationBuilder()
                .WithId(new ProgramIdentifier(9u, 8u))
                .WithFormat(RomFormat.Bin)
                .WithTitle(title)
                .WithFeatures(ProgramFeatures.DefaultFeatures)
                .WithMetadata(new ProgramMetadataBuilder().Build());
            return builder;
        }
    }
}

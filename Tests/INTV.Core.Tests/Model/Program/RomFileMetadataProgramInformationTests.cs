// <copyright file="RomFileMetadataProgramInformationTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class RomFileMetadataProgramInformationTests
    {
        [Fact]
        public void RomFileMetadataProgramInformation_GetDataOrigin_IsProgramInformationOriginRomMetadataBlock()
        {
            var information = new RomFileMetadataProgramInformation(null);

            Assert.Equal(ProgramInformationOrigin.RomMetadataBlock, information.DataOrigin);
        }

        [Fact]
        public void RomFileMetadataProgramInformation_GetSetTitle_IsSetCorrectly()
        {
            var title = "Best ROM Evah!";
            var information = new RomFileMetadataProgramInformation(null) { Title = title };

            Assert.Equal(title, information.Title);
        }

        [Fact]
        public void RomFileMetadataProgramInformation_GetSetVendor_IsSetCorrectly()
        {
            var vendor = "Mattel Electronics";
            var information = new RomFileMetadataProgramInformation(null) { Vendor = vendor };

            Assert.Equal(vendor, information.Vendor);
        }

        [Fact]
        public void RomFileMetadataProgramInformation_GetSetYear_IsSetCorrectly()
        {
            var year = "42"; // this test will fail if we decide enforce data correctness...
            var information = new RomFileMetadataProgramInformation(null) { Year = year };

            Assert.Equal(year, information.Year);
        }

        [Fact]
        public void RomFileMetadataProgramInformation_GetSetFeatures_IsSetCorrectly()
        {
            var features = new ProgramFeaturesBuilder().WithIntellicartFeatures(IntellicartCC3Features.Incompatible) as ProgramFeatures;
            var information = new RomFileMetadataProgramInformation(null) { Features = features };

            Assert.Equal(features, information.Features);
        }

        [Fact]
        public void RomFileMetadataProgramInformation_GetSetShortName_IsSetCorrectly()
        {
            var shortName = "Not really the shortest name - in fact it's too long"; // this test will fail if we decide to enforce name length
            var information = new RomFileMetadataProgramInformation(null) { ShortName = shortName };

            Assert.Equal(shortName, information.ShortName);
        }

        [Fact]
        public void RomFileMetadataProgramInformation_AddCrc_ThrowsInvalidOperationException()
        {
            var information = new RomFileMetadataProgramInformation(null);

            Assert.Throws<InvalidOperationException>(() => information.AddCrc(1u, "blah", IncompatibilityFlags.Ecs));
        }
    }
}

// <copyright file="LuigiFileMetadataProgramInformationTests.cs" company="INTV Funhouse">
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

namespace INTV.Core.Tests.Model.Program
{
    public class LuigiFileMetadataProgramInformationTests
    {
        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithNullHeader_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => new LuigiFileMetadataProgramInformation(null, null));
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_AddCrc_ThrowsNotImplementedException()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.Null(metadataBlock);

            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);

            Assert.Throws<NotImplementedException>(() => information.AddCrc(1u, "version", IncompatibilityFlags.None));
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithNoMetadatda_DataOriginIsCorrect()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.Null(metadataBlock);

            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);

            VerifyMinimalInformation(information);
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithMetadatda_DataOriginIsCorrect()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiWithMetadataPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.NotNull(metadataBlock);

            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);

            VerifyMetadataInformation(information);
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithNoMetadatda_VerifySetTitle()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.Null(metadataBlock);
            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);
            Assert.Null(information.Title);

            var testTitle = "Tag the Toad";
            information.Title = testTitle;

            Assert.Equal(testTitle, information.Title);
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithNoMetadatda_VerifySetVendor()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.Null(metadataBlock);
            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);
            Assert.Null(information.Vendor);

            var testVendor = "Bob the Builder";
            information.Vendor = testVendor;

            Assert.Equal(testVendor, information.Vendor);
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithNoMetadatda_VerifySetYear()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.Null(metadataBlock);
            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);
            Assert.Null(information.Year);

            var testYear = "1989";
            information.Year = testYear;
            Assert.Equal(testYear, information.Year);
        }

        [Fact]
        public void LuigiFileMetadataProgramInformation_CreateWithNoMetadatda_VerifySetFeatures()
        {
            var path = LuigiFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).Single();
            var rom = Rom.AsSpecificRomType<LuigiFormatRom>(Rom.Create(path, null));
            Assert.NotNull(rom);
            var metadataBlock = rom.LocateDataBlock<LuigiMetadataBlock>();
            Assert.Null(metadataBlock);
            var information = new LuigiFileMetadataProgramInformation(rom.Header, metadataBlock);
            var currentFeatures = information.Features;
            Assert.NotNull(currentFeatures);

            var testFeatures = currentFeatures.Clone();
            testFeatures.Ecs = EcsFeatures.Printer;
            information.Features = testFeatures;

            Assert.NotEqual(testFeatures, currentFeatures);
            Assert.True(object.ReferenceEquals(testFeatures, information.Features));
        }

        private void VerifyMinimalInformation(LuigiFileMetadataProgramInformation information)
        {
            Assert.NotNull(information);
            Assert.Equal(ProgramInformationOrigin.LuigiMetadataBlock, information.DataOrigin);
            Assert.NotNull(information.Features);
            Assert.NotEmpty(information.Crcs);
            Assert.Null(information.Title);
            Assert.Null(information.Vendor);
            Assert.Null(information.Year);
            Assert.Throws<NullReferenceException>(() => information.LongNames);
            Assert.Throws<NullReferenceException>(() => information.ShortNames);
            Assert.Throws<NullReferenceException>(() => information.Descriptions);
            Assert.Throws<NullReferenceException>(() => information.Publishers);
            Assert.Throws<NullReferenceException>(() => information.Programmers);
            Assert.Throws<NullReferenceException>(() => information.Designers);
            Assert.Throws<NullReferenceException>(() => information.Graphics);
            Assert.Throws<NullReferenceException>(() => information.Music);
            Assert.Throws<NullReferenceException>(() => information.SoundEffects);
            Assert.Throws<NullReferenceException>(() => information.Voices);
            Assert.Throws<NullReferenceException>(() => information.Documentation);
            Assert.Throws<NullReferenceException>(() => information.Artwork);
            Assert.Throws<NullReferenceException>(() => information.ReleaseDates);
            Assert.Throws<NullReferenceException>(() => information.Licenses);
            Assert.Throws<NullReferenceException>(() => information.ContactInformation);
            Assert.Throws<NullReferenceException>(() => information.Versions);
            Assert.Throws<NullReferenceException>(() => information.BuildDates);
            Assert.Empty(information.AdditionalInformation);
        }

        private void VerifyMetadataInformation(LuigiFileMetadataProgramInformation information)
        {
            Assert.NotNull(information);
            Assert.Equal(ProgramInformationOrigin.LuigiMetadataBlock, information.DataOrigin);
            Assert.NotNull(information.Features);
            Assert.NotEmpty(information.Crcs);
            Assert.NotNull(information.Title);
            Assert.NotNull(information.Vendor);
            Assert.NotNull(information.Year);
            Assert.NotEmpty(information.LongNames);
            Assert.NotEmpty(information.ShortNames);
            Assert.NotEmpty(information.Descriptions);
            Assert.NotEmpty(information.Publishers);
            Assert.NotEmpty(information.Programmers);
            Assert.NotEmpty(information.Designers);
            Assert.NotEmpty(information.Graphics);
            Assert.NotEmpty(information.Music);
            Assert.NotEmpty(information.SoundEffects);
            Assert.NotEmpty(information.Voices);
            Assert.NotEmpty(information.Documentation);
            Assert.NotEmpty(information.Artwork);
            Assert.NotEmpty(information.ReleaseDates);
            Assert.NotEmpty(information.Licenses);
            Assert.NotEmpty(information.ContactInformation);
            Assert.NotEmpty(information.Versions);
            Assert.NotEmpty(information.BuildDates);
            Assert.Empty(information.AdditionalInformation);
        }

        private class LuigiFileMetadataProgramInformationTestStorageAccess : CachedResourceStorageAccess<LuigiFileMetadataProgramInformationTestStorageAccess>
        {
        }
    }
}

// <copyright file="CfgFileMetadataProgramInformationTests.cs" company="INTV Funhouse">
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
using System.Text;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class CfgFileMetadataProgramInformationTests
    {
        [Fact]
        public void CfgFileMetadataProgramInformation_AddCrc_ThrowsNotImplementedException()
        {
            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(null);

            Assert.Throws<NotImplementedException>(() => cfgMetadataInformation.AddCrc(0u, null, IncompatibilityFlags.None));
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_SetFeatures_SetsFeatures()
        {
            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(null);

            var testFeatures = ProgramFeatures.DefaultFeatures.Clone();
            cfgMetadataInformation.Features = testFeatures;

            Assert.NotNull(cfgMetadataInformation.Features);
            Assert.True(object.ReferenceEquals(testFeatures, cfgMetadataInformation.Features));
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_SetTitle_SetsTitle()
        {
            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(null);
            Assert.Null(cfgMetadataInformation.Title);

            var testTitle = "Keep 'er movin'";
            cfgMetadataInformation.Title = testTitle;

            Assert.Equal(testTitle, cfgMetadataInformation.Title);
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_SetVendor_SetsVendor()
        {
            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(null);
            Assert.Null(cfgMetadataInformation.Vendor);

            var testVendor = "Blue Sky Rangers, Inc.";
            cfgMetadataInformation.Vendor = testVendor;

            Assert.Equal(testVendor, cfgMetadataInformation.Vendor);
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_SetYear_SetsYear()
        {
            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(null);
            Assert.Null(cfgMetadataInformation.Year);

            var testYear = "1987";
            cfgMetadataInformation.Year = testYear;

            Assert.Equal(testYear, cfgMetadataInformation.Year);
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_SetYearToBogusValue_SetsYearToBogusValue()
        {
            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(null);
            Assert.Null(cfgMetadataInformation.Year);

            var testYear = "Year Zero!";
            cfgMetadataInformation.Year = testYear;

            Assert.Equal(testYear, cfgMetadataInformation.Year);
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_CreateUsingRomWithUnknownMetadataBlockType_ContainsValidMetadata()
        {
            var romPaths = CfgFileMetadataProgramInformationTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgMetadataPath);
            var rom = Rom.AsSpecificRomType<BinFormatRom>(Rom.Create(romPaths[0], romPaths[1]));
            rom.MetadataCacheEnabled = true;
            Assert.NotNull(rom.Metadata);
            var metadata = (List<CfgVarMetadataBlock>)rom.Metadata;
            metadata.Add(new CfgVarMetadataFeatureCompatibility(CfgVarMetadataIdTag.Invalid));

            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(rom);

            Assert.NotNull(cfgMetadataInformation.Metadata);
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_CreateUsingRomWithJlpNoFlash_ContainsValidMetadata()
        {
            IReadOnlyList<string> romPaths;
            var storage = CfgFileMetadataProgramInformationTestStorageAccess.Initialize(out romPaths, TestRomResources.TestBinPath, TestRomResources.TestCfgMetadataPath);
            using (var cfgFile = storage.Open(romPaths[1]))
            {
                var cfgContent = "[vars]\njlp=2\n";
                var cfgBytes = Encoding.UTF8.GetBytes(cfgContent);
                cfgFile.Write(cfgBytes, 0, cfgBytes.Length);
                cfgFile.SetLength(cfgBytes.Length);
            }
            var rom = Rom.AsSpecificRomType<BinFormatRom>(Rom.Create(romPaths[0], romPaths[1]));
            rom.MetadataCacheEnabled = true;
            Assert.NotNull(rom.Metadata);

            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(rom);

            Assert.NotNull(cfgMetadataInformation.Metadata);
            Assert.Equal((ushort)0, cfgMetadataInformation.Features.JlpFlashMinimumSaveSectors);
        }

        [Fact]
        public void CfgFileMetadataProgramInformation_CreateUsingRomWithZeroSizeJlpFlash_ContainsValidMetadata()
        {
            IReadOnlyList<string> romPaths;
            var storage = CfgFileMetadataProgramInformationTestStorageAccess.Initialize(out romPaths, TestRomResources.TestBinPath, TestRomResources.TestCfgMetadataPath);
            using (var cfgFile = storage.Open(romPaths[1]))
            {
                var cfgContent = "[vars]\njlp=2\njlp_flash=0\n";
                var cfgBytes = Encoding.UTF8.GetBytes(cfgContent);
                cfgFile.Write(cfgBytes, 0, cfgBytes.Length);
                cfgFile.SetLength(cfgBytes.Length);
            }
            var rom = Rom.AsSpecificRomType<BinFormatRom>(Rom.Create(romPaths[0], romPaths[1]));
            rom.MetadataCacheEnabled = true;
            Assert.NotNull(rom.Metadata);

            var cfgMetadataInformation = new CfgFileMetadataProgramInformation(rom);

            Assert.NotNull(cfgMetadataInformation.Metadata);
            Assert.Equal((ushort)0, cfgMetadataInformation.Features.JlpFlashMinimumSaveSectors);
        }

        private class CfgFileMetadataProgramInformationTestStorageAccess : CachedResourceStorageAccess<CfgFileMetadataProgramInformationTestStorageAccess>
        {
        }
    }
}

// <copyright file="ProgramDescriptionTests.cs" company="INTV Funhouse">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramDescriptionTests
    {
        [Fact]
        public void ProgramDescription_CreateWithNoValidArguments_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ProgramDescription(0u, null, null)); // Throws because program information is null.
        }

        [Fact]
        public void ProgramDescription_CreateWithZeroCrcNullRomEmptyInformation_ThrowsInvalidOperationException()
        {
            var information = new TestProgramInformation();

            Assert.Throws<InvalidOperationException>(() => new ProgramDescription(0u, null, information)); // Throws because no CrcData is in the program information.
        }

        [Fact]
        public void ProgramDescription_CreateWithZeroCrcNullRomInformationWithOneCrc_ThrowsInvalidOperationException()
        {
            var information = new TestProgramInformation();

            information.AddCrc(1u);

            Assert.Throws<InvalidOperationException>(() => new ProgramDescription(0u, null, information)); // Throws because no CRC matches the one passed into the constructor.
        }

        [Fact]
        public void ProgramDescription_SetCrcWhenNotInDatabase_UpdatesCrc()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            var newCrc = 0x12346587u;
            description.Crc = newCrc;

            Assert.Equal(newCrc, description.Crc);
        }

        [Fact]
        public void ProgramDescription_SetCrcWhenFoundInDatabase_UpdatesCrc()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            information.AddCrc(crc);
            var newCrc = 0x32547698u;
            information.AddCrc(newCrc);
            var description = new ProgramDescription(crc, null, information);
            var testTable = new TestProgramInformationTable();
            testTable.AddEntries(information);
            var defaultTable = ProgramInformationTable.Default as MergedProgramInformationTable; // Eewwww!
            defaultTable.MergeTable(testTable);

            description.Crc = newCrc;

            Assert.Equal(newCrc, description.Crc);
        }

        [Theory]
        [InlineData(null, "Taterkins")]
        [InlineData("", "Taterkins")]
        [InlineData(" \t\r\n", "Taterkins")]
        [InlineData("Earthworm Jimmeh", "Earthworm Jimmeh")]
        [InlineData("Tah rah rah Boom! Dee Yay! Tah rah rah BOOM! DEE! YAY! Tah rah rah boom dee yay, tah rah rah boom dee yay.", "Tah rah rah Boom! Dee Yay! Tah rah rah BOOM! DEE! YAY! Tah rah r")]
        public void ProgramDescription_SetName_NameIsExpectedValue(string newName, string expectedName)
        {
            var information = new TestProgramInformation() { Title = "Taterkins" };
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            description.Name = newName;

            Assert.Equal(expectedName, description.Name);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" \t\r\n", " \t\r\n")]
        [InlineData("Earthworm Jimmeh", "Earthworm Jimmeh")]
        [InlineData("Tah rah rah Boom! Dee Yay! Tah rah rah BOOM! DEE! YAY! Tah rah rah boom dee yay, tah rah rah boom dee yay.", "Tah rah rah Boom! ")]
        public void ProgramDescription_SetShortName_ShortNameIsExpectedValue(string newShortName, string expectedShortName)
        {
            var information = new TestProgramInformation() { Title = "Taterkins", ShortName = "Tater" };
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            description.ShortName = newShortName;

            Assert.Equal(expectedShortName, description.ShortName);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" \t\r\n", " \t\r\n")]
        [InlineData("Psst! Wanna buy a game?", "Psst! Wanna buy a game?")]
        [InlineData("The Shady Gamer, Purveyor of Shady Games, Inc.", "The Shady Gamer, Purveyor of Sha")]
        public void ProgramDescription_SetVendor_VendorIsExpectedValue(string newVendor, string expectedVendor)
        {
            var information = new TestProgramInformation() { Vendor = "Vendorman" };
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            description.Vendor = newVendor;

            Assert.Equal(expectedVendor, description.Vendor);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" \t\r\n", " \t\r\n")]
        [InlineData("1986", "1986")]
        [InlineData("b", "b")]
        [InlineData("Whenevs, dude", "When")]
        public void ProgramDescription_SetYear_YearIsExpectedValue(string newYear, string expectedYear)
        {
            var information = new TestProgramInformation() { Year = "1984" };
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            description.Year = newYear;

            Assert.Equal(expectedYear, description.Year);
        }

        [Fact]
        public void ProgramDescription_SetFeatures_UpdatesFeatures()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            var newFeatures = new ProgramFeaturesBuilder().WithTutorvisionCompatibility(FeatureCompatibility.Requires).Build() as ProgramFeatures;
            description.Features = newFeatures;

            Assert.True(object.ReferenceEquals(newFeatures, description.Features));
        }

        [Fact]
        public void ProgramDescription_RomIsNullSetProgramSupportFilesWithNullRom_UpdatesFilesAndLeavesRomUnchanged()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            var newFiles = new ProgramSupportFiles(null);
            description.Files = newFiles;

            Assert.Null(description.Rom);
            Assert.True(object.ReferenceEquals(newFiles, description.Files));
        }

        [Fact]
        public void ProgramDescription_RomIsNullSetProgramSupportFilesWithValidRom_UpdatesFilesAndRom()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);
            var romPath = ProgramDescriptionTestStorage.Initialize(TestRomResources.TestRomPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            var newFiles = new ProgramSupportFiles(rom);
            description.Files = newFiles;

            Assert.True(object.ReferenceEquals(newFiles.Rom, description.Rom));
            Assert.True(object.ReferenceEquals(newFiles, description.Files));
        }

        [Fact]
        public void ProgramDescription_ValidRomSetProgramSupportFilesWithNullRom_UpdatesFilesAndLeavesRomUnchanged()
        {
            var romPath = ProgramDescriptionTestStorage.Initialize(TestRomResources.TestRomPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            var information = new TestProgramInformation();
            var crc = rom.Crc;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, rom, information);

            var newFiles = new ProgramSupportFiles(null);
            description.Files = newFiles;

            Assert.True(object.ReferenceEquals(rom, description.Rom));
            Assert.True(object.ReferenceEquals(newFiles, description.Files));
        }

        [Fact]
        public void ProgramDescription_ValidRomSetProgramSupportFilesWithValidRom_UpdatesFilesAndLeavesRomUnchanged()
        {
            var romPaths = ProgramDescriptionTestStorage.Initialize(TestRomResources.TestRomPath, TestRomResources.TestBinPath);
            var rom0 = Rom.Create(romPaths[0], null);
            Assert.NotNull(rom0);
            var information = new TestProgramInformation();
            var crc = rom0.Crc;
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, rom0, information);
            var rom1 = Rom.Create(romPaths[1], null);
            Assert.NotNull(rom1);

            var newFiles = new ProgramSupportFiles(rom1);
            description.Files = newFiles;

            Assert.True(object.ReferenceEquals(rom0, description.Rom));
            Assert.True(object.ReferenceEquals(newFiles, description.Files));
        }

        [Fact]
        public void ProgramDescription_CreateWithMatchingCrcNullRomInformationWithMatchingCrc_CreatesDescription()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5000";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);

            Assert.Equal(crc, description.Crc);
            Assert.Null(description.Rom);
            Assert.NotNull(description.ProgramInformation);
            Assert.Equal(" (" + name + ")", description.Name); // title is null; name from ROM variant is appended in parentheses
            Assert.Equal(information.ShortName, description.ShortName);
            Assert.Equal(information.Vendor, description.Vendor);
            Assert.Equal(information.Year, description.Year);
            Assert.Null(description.Features);
            Assert.NotNull(description.Files);
        }

        [Fact]
        public void ProgramDescription_CreateWithMatchingCrcWithNullRomInformationWithMatchingCrcOneIncompatibility_ThrowsNullReferenceException()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5100";
            information.AddCrc(crc, name, IncompatibilityFlags.Jlp);

            Assert.Throws<NullReferenceException>(() => new ProgramDescription(crc, null, information)); // Throws because there are no features in information.
        }

        [Fact]
        public void ProgramDescription_CreateWithMatchingCrcWithNullRomInformationWithFeaturesAndMatchingCrcOneIncompatibility_RetainsFeatures()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5200";
            information.AddCrc(crc, name, IncompatibilityFlags.Jlp);
            information.Features = new ProgramFeaturesBuilder()
                .WithJlpFeatures(JlpFeatures.Incompatible)
                .WithLtoFlashFeatures(LtoFlashFeatures.Tolerates)
                .WithBee3Features(Bee3Features.Tolerates)
                .WithHiveFeatures(HiveFeatures.Tolerates)
                .Build() as ProgramFeatures;

            var description = new ProgramDescription(crc, null, information);

            Assert.Equal(information.Features, description.Features);
        }

        [Fact]
        public void ProgramDescription_CreateWithMatchingCrcWithNullRomInformationWithFeaturesAndMatchingCrcMultipleIncompatibilities_FeaturesIncludesAllIncompatibilities()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5300";
            information.AddCrc(crc, name, IncompatibilityFlags.None);
            information.AddCrc(crc + 1, name, IncompatibilityFlags.Jlp);
            information.AddCrc(crc + 2, name, IncompatibilityFlags.LtoFlash);
            information.Features = new ProgramFeaturesBuilder().WithPalCompatibility(FeatureCompatibility.Enhances).Build() as ProgramFeatures;

            var description = new ProgramDescription(crc, null, information);

            Assert.Equal(FeatureCompatibility.Enhances, description.Features.Pal);
            Assert.Equal(JlpFeatures.Incompatible, description.Features.Jlp);
            Assert.Equal(LtoFlashFeatures.Incompatible, description.Features.LtoFlash);
        }

        [Fact]
        public void ProgramDescription_ValidateWithNullRom_ThrowsNullReferenceException()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5400";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);

            Assert.Throws<NullReferenceException>(() => ProgramDescription.Validate(description, null, null, reportMessagesChanged: false)); // Throws because ROM is null.
        }

        [Fact]
        public void ProgramDescription_ValidateWithValidRom_ReturnsTrue()
        {
            var romPath = ProgramDescriptionTestStorage.Initialize(TestRomResources.TestRomPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            var information = new TestProgramInformation();
            var crc = rom.Crc;
            var name = "AgletMaster 5500";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, rom, information);

            Assert.True(ProgramDescription.Validate(description, null, null, reportMessagesChanged: false));
        }

        [Fact]
        public void ProgramDescription_ValidateWithValidRomWhoseCfgFileIsMissing_ReturnsFalse()
        {
            var romPath = ProgramDescriptionTestStorage.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            var information = new TestProgramInformation();
            var crc = 0x8C29E37Du;
            var name = "AgletMaster 5600";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, rom, information);
            var v = ProgramDescription.Validate(description, null, null, reportMessagesChanged: false);

            Assert.False(ProgramDescription.Validate(description, null, null, reportMessagesChanged: false));
        }

        [Fact]
        public void ProgramDescription_GetHashCode_ProducesExpectedHashCode()
        {
            var crc = 0x82736455u;
            var information = new TestProgramInformation();
            information.AddCrc(crc);
            var description = new ProgramDescription(crc, null, information);

            var expectedHashCode = crc.GetHashCode();
            Assert.Equal(expectedHashCode, description.GetHashCode());
        }

        [Fact]
        public void ProgramDescription_OperatorEqualsSelf_ReturnsTrue()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5700";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);
            var self = description;

            Assert.True(description == self);
        }

        [Fact]
        public void ProgramDescription_EqualsSelf_ReturnsTrue()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5700a";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);
            var self = description;

            Assert.True(description.Equals(self));
        }

        [Fact]
        public void ProgramDescription_OperatorEqualsNull_ReturnsFalse()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5800";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);

            Assert.False(description == null);
        }

        [Fact]
        public void ProgramDescription_EqualsNull_ReturnsFalse()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5800a";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);

            Assert.False(description.Equals(null));
        }

        // Disable warning CS0252:
        // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'INTV.Core.Model.Program.ProgramDescription'
        // This test wishes to test the operator in this scenario.
#pragma warning disable 252

        [Fact]
        public void ProgramDescription_OperatorNullEquals_ReturnsFalse()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5900";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);
            IProgramDescription other = null;

            Assert.False(other == description);
        }

#pragma warning restore 252

        [Fact]
        public void ProgramDescription_NullEquals_ThrowsNullReferenceException()
        {
            var information = new TestProgramInformation();
            var crc = 1u;
            var name = "AgletMaster 5900a";
            information.AddCrc(crc, name);

            var description = new ProgramDescription(crc, null, information);
            ProgramDescription nullProgramDescription = null;

            Assert.Throws<NullReferenceException>(() => nullProgramDescription.Equals(description));
        }

        [Fact(Skip = "Appears to be a bug. See: https://github.com/intvsteve/VINTage/issues/239")]
        public void ProgramDescription_OperatorEqualsForTwoDifferentDescriptionsWithNullRom_ReturnsFalse()
        {
            var information0 = new TestProgramInformation();
            var crc0 = 1u;
            var name0 = "AgletMaster 5901";
            information0.AddCrc(crc0, name0);
            var information1 = new TestProgramInformation();
            var crc1 = 2u;
            var name1 = "AgletMaster 5902";
            information1.AddCrc(crc1, name1);

            var description0 = new ProgramDescription(crc0, null, information0);
            var description1 = new ProgramDescription(crc1, null, information1);

            Assert.False(description0 == description1);
        }

        [Fact(Skip = "Appears to be a bug. See: https://github.com/intvsteve/VINTage/issues/239")]
        public void ProgramDescription_EqualsForTwoDifferentDescriptionsWithNullRom_ReturnsFalse()
        {
            var information0 = new TestProgramInformation();
            var crc0 = 1u;
            var name0 = "AgletMaster 5901a";
            information0.AddCrc(crc0, name0);
            var information1 = new TestProgramInformation();
            var crc1 = 2u;
            var name1 = "AgletMaster 5902a";
            information1.AddCrc(crc1, name1);

            var description0 = new ProgramDescription(crc0, null, information0);
            var description1 = new ProgramDescription(crc1, null, information1);

            Assert.False(description0.Equals(description1));
        }

        [Fact]
        public void ProgramDescription_OperatorEqualsForTwoInstancesOfEqualDescriptionsWithNullRom_ReturnsTrue()
        {
            var information0 = new TestProgramInformation();
            var crc0 = 3u;
            var name0 = "AgletMaster 5903";
            information0.AddCrc(crc0, name0);
            var information1 = new TestProgramInformation();
            var crc1 = 3u;
            var name1 = "AgletMaster 5903";
            information1.AddCrc(crc1, name1);

            var description0 = new ProgramDescription(crc0, null, information0);
            var description1 = new ProgramDescription(crc1, null, information1);

            Assert.True(description0 == description1);
        }

        [Fact]
        public void ProgramDescription_EqualsForTwoInstancesOfEqualDescriptionsWithNullRom_ReturnsTrue()
        {
            var information0 = new TestProgramInformation();
            var crc0 = 3u;
            var name0 = "AgletMaster 5903a";
            information0.AddCrc(crc0, name0);
            var information1 = new TestProgramInformation();
            var crc1 = 3u;
            var name1 = "AgletMaster 5903";
            information1.AddCrc(crc1, name1);

            var description0 = new ProgramDescription(crc0, null, information0);
            var description1 = new ProgramDescription(crc1, null, information1);

            Assert.True(description0.Equals(description1));
        }

        [Fact]
        public void ProgramDescription_OperatorNotEqualsForTwoInstancesOfEqualDescriptionsWithNullRom_ReturnsFalse()
        {
            var information0 = new TestProgramInformation();
            var crc0 = 4u;
            var name0 = "AgletMaster 5904";
            information0.AddCrc(crc0, name0);
            var information1 = new TestProgramInformation();
            var crc1 = 4u;
            var name1 = "AgletMaster 5904";
            information1.AddCrc(crc1, name1);

            var description0 = new ProgramDescription(crc0, null, information0);
            var description1 = new ProgramDescription(crc1, null, information1);

            Assert.False(description0 != description1);
        }

        [Fact]
        public void ProgramDescription_Copy_ProducesEquivalentCopy()
        {
            var information = new TestProgramInformation();
            var crc = 5u;
            var name = "AgletMaster 5905";
            information.AddCrc(crc, name);

            var description0 = new ProgramDescription(crc, null, information);
            var description1 = description0.Copy();

            Assert.Equal(description0.Name, description1.Name);
            Assert.Equal(description0.ShortName, description1.ShortName);
            Assert.Equal(description0.Vendor, description1.Vendor);
            Assert.Equal(description0.Year, description1.Year);
            Assert.Equal(description0.Features, description1.Features);
            Assert.True(object.ReferenceEquals(description0.Rom, description1.Rom));
            VerifyProgramInformation(description0.ProgramInformation, description1.ProgramInformation);
            VerifyProgramSupportFiles(description0.Files, description1.Files);
            Assert.Equal(description0, description1);
        }

        [Fact]
        public void ProgramDescription_ParseFromXml_ProducesValidProgramDescription()
        {
            ProgramDescriptionTestStorage.Initialize(null);
            var xmlProgramDescription = @"<?xml version=""1.0""?>
  <ProgramDescription>
    <Crc>3971627767</Crc>
    <Name>Tagalong Todd!</Name>
    <Vendor>Zbiciak Electronics</Vendor>
    <Year>1999</Year>
    <Features>
      <Ntsc>Tolerates</Ntsc>
      <Pal>Tolerates</Pal>
      <GeneralFeatures>None</GeneralFeatures>
      <KeyboardComponent>Tolerates</KeyboardComponent>
      <SuperVideoArcade>Tolerates</SuperVideoArcade>
      <Intellivoice>Tolerates</Intellivoice>
      <IntellivisionII>Tolerates</IntellivisionII>
      <Ecs>Tolerates</Ecs>
      <Tutorvision>Tolerates</Tutorvision>
      <Intellicart>Tolerates</Intellicart>
      <CuttleCart3>Tolerates</CuttleCart3>
      <Jlp>Incompatible</Jlp>
      <JlpHardwareVersion>None</JlpHardwareVersion>
      <JlpFlashMinimumSaveSectors>0</JlpFlashMinimumSaveSectors>
      <LtoFlash>Incompatible</LtoFlash>
      <Bee3>Incompatible</Bee3>
      <Hive>Incompatible</Hive>
    </Features>
    <Files>
      <RomImagePath>\Users\tester\Projects\perforce\intellivision\roms\tagalong.bin</RomImagePath>
      <RomConfigurationFilePath>/Users/tester/Projects/MinGW/msys/1.0/home/tester/lui_src/LtoFlash/bin/tools/0.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths />
      <AlternateRomConfigurationFilePaths />
    </Files>
  </ProgramDescription>
";
            ProgramDescription description;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlProgramDescription)))
            {
                var serializer = new XmlSerializer(typeof(ProgramDescription));
                description = serializer.Deserialize(stream) as ProgramDescription;
            }

            Assert.NotNull(description);
            Assert.Equal("Tagalong Todd!", description.Name);
            Assert.Equal("Zbiciak Electronics", description.Vendor);
            Assert.Equal("1999", description.Year);
            Assert.Equal(TestRomResources.TestBinCrc, description.Crc);
            Assert.Equal(ProgramFeatures.DefaultFeatures, description.Features);
            Assert.Equal(@"\Users\tester\Projects\perforce\intellivision\roms\tagalong.bin", description.Files.RomImagePath);
            Assert.Equal(@"/Users/tester/Projects/MinGW/msys/1.0/home/tester/lui_src/LtoFlash/bin/tools/0.cfg", description.Files.RomConfigurationFilePath);
            Assert.Empty(description.Files.AlternateRomImagePaths);
            Assert.Empty(description.Files.AlternateRomConfigurationFilePaths);
            Assert.NotNull(description.Rom);
            Assert.True(description.Rom is XmlRom);
            Assert.Null(((XmlRom)description.Rom).ResolvedRom);
            Assert.Equal(0u, description.Rom.Crc);
            Assert.Equal(0u, description.Rom.CfgCrc);
            Assert.Equal(@"\Users\tester\Projects\perforce\intellivision\roms\tagalong.bin", description.Rom.RomPath);
            Assert.Equal(@"/Users/tester/Projects/MinGW/msys/1.0/home/tester/lui_src/LtoFlash/bin/tools/0.cfg", description.Rom.ConfigPath);
        }

        private void VerifyProgramInformation(IProgramInformation information0, IProgramInformation information1)
        {
            Assert.Equal(information0.DataOrigin, information1.DataOrigin);
            Assert.Equal(information0.Title, information1.Title);
            Assert.Equal(information0.Vendor, information1.Vendor);
            Assert.Equal(information0.Year, information1.Year);
            Assert.Equal(information0.Features, information1.Features);
            Assert.Equal(information0.ShortName, information1.ShortName);
            Assert.Equal(information0.Crcs, information1.Crcs);
        }

        private void VerifyProgramSupportFiles(ProgramSupportFiles files0, ProgramSupportFiles files1)
        {
            Assert.Equal(files0.Rom, files1.Rom);
            Assert.Equal(files0.RomImagePath, files1.RomImagePath);
            Assert.Equal(files0.RomConfigurationFilePath, files1.RomConfigurationFilePath);
            Assert.Equal(files0.AlternateRomImagePaths, files1.AlternateRomImagePaths);
            Assert.Equal(files0.AlternateRomConfigurationFilePaths, files1.AlternateRomConfigurationFilePaths);
            Assert.Equal(files0.BoxImagePaths, files1.BoxImagePaths);
            Assert.Equal(files0.OverlayImagePaths, files1.OverlayImagePaths);
            Assert.Equal(files0.ManualCoverImagePaths, files1.ManualCoverImagePaths);
            Assert.Equal(files0.LabelImagePaths, files1.LabelImagePaths);
            Assert.Equal(files0.ManualPaths, files1.ManualPaths);
            Assert.Equal(files0.SaveDataPaths, files1.SaveDataPaths);
            Assert.Equal(files0.DefaultBoxImagePath, files1.DefaultBoxImagePath);
            Assert.Equal(files0.DefaultOverlayImagePath, files1.DefaultOverlayImagePath);
            Assert.Equal(files0.DefaultManualImagePath, files1.DefaultManualImagePath);
            Assert.Equal(files0.DefaultLabelImagePath, files1.DefaultLabelImagePath);
            Assert.Equal(files0.DefaultManualTextPath, files1.DefaultManualTextPath);
            Assert.Equal(files0.DefaultSaveDataPath, files1.DefaultSaveDataPath);
            Assert.Equal(files0.DefaultLtoFlashDataPath, files1.DefaultLtoFlashDataPath);
            Assert.Equal(files0.DefaultVignettePath, files1.DefaultVignettePath);
            Assert.Equal(files0.DefaultReservedDataPath, files1.DefaultReservedDataPath);
        }

        private class TestProgramInformation : IProgramInformation
        {
            public TestProgramInformation()
            {
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
                get { return _crcs; }
            }
            private List<CrcData> _crcs = new List<CrcData>();

            public bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilityFlags)
            {
                _crcs.Add(new CrcData(newCrc, crcDescription, incompatibilityFlags));
                return true;
            }

            public bool ModifyCrc(uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags)
            {
                throw new NotImplementedException();
            }
        }

        private class TestProgramInformationTable : IProgramInformationTable
        {
            private Dictionary<uint, IProgramInformation> _entries = new Dictionary<uint, IProgramInformation>();

            public IEnumerable<IProgramInformation> Programs
            {
                get { return _entries.Values; }
            }

            public IProgramInformation FindProgram(uint crc)
            {
                var information = _entries.FirstOrDefault(e => e.Key == crc).Value;
                return information;
            }

            public IProgramInformation FindProgram(ProgramIdentifier programIdentifier)
            {
                var information = _entries.FirstOrDefault(e => e.Key == programIdentifier.DataCrc).Value;
                return information;
            }

            internal void AddEntries(TestProgramInformation information)
            {
                foreach (var crcData in information.Crcs.Cast<CrcData>())
                {
                    _entries[crcData.Crc] = information;
                }
            }
        }

        private class ProgramDescriptionTestStorage : CachedResourceStorageAccess<ProgramDescriptionTestStorage>
        {
        }
    }
}

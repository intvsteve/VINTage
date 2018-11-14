// <copyright file="IRomHelpersTests.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.IO;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class IRomHelpersTests
    {
        #region GetProgramFeatures Tests

        [Fact]
        public void IRomHelpers_GetProgramFeaturesWithNullRom_ThrowsNullReferenceException()
        {
            IRom rom = null;

            Assert.Throws<NullReferenceException>(() => rom.GetProgramFeatures());
        }

        [Fact]
        public void IRomHelpers_GetProgramFeaturesForNonLuigiFormatRom_ReturnsUnrecognizedProgramFeatures()
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestCc3Path);
            var rom = Rom.Create(TestRomResources.TestCc3Path, null);
            Assert.NotNull(rom);

            var features = rom.GetProgramFeatures();

            Assert.Equal(ProgramFeatures.GetUnrecognizedRomFeatures(), features);
        }

        [Theory]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForAnyDevicePath)]
        public void IRomHelpers_GetProgramFeaturesForLuigiFormatRom_ReturnsExpectedProgramFeatures(string testLuigiRomPath)
        {
            IRomHelpersTestStorageAccess.Initialize(testLuigiRomPath);
            var rom = Rom.Create(testLuigiRomPath, null);
            Assert.NotNull(rom);

            var features = rom.GetProgramFeatures();

            Assert.NotEqual(ProgramFeatures.GetUnrecognizedRomFeatures(), features);
        }

        #endregion // GetProgramFeatures Tests

        #region GetProgramInformation Tests

        [Fact]
        public void IRomHelpers_GetProgramInformationWithNullRom_ThrowsNullReferenceException()
        {
            IRom rom = null;

            Assert.Throws<NullReferenceException>(() => rom.GetProgramInformation());
        }

        [Fact] // 0,0,0 [no supporting data]
        public void IRomHelpers_GetProgramInforamtionForBinFormatRomWithEmptyIntvnameInformation_ReturnsUnknownProgramInfo()
        {
            var paths = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgPath).ToList();
            var romPath = paths[0];
            var cfgPath = paths[1];
            var rom = Rom.Create(romPath, cfgPath);
            Assert.NotNull(rom);

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath))
            {
                EnsureDatabaseEntryNotPresent(TestRomResources.TestBinCrc);
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.UserDefined,
                    TestRomResources.TestBinCrc,
                    INTV.Core.Resources.Strings.ProgramInformation_DefaultTitle,
                    string.Empty,
                    null,
                    ProgramFeatures.GetUnrecognizedRomFeatures(),
                    null);
            }
        }

        [Fact] // 0, 0, i [i = intvname data]
        public void IRomHelpers_GetProgramInformationForBinFormatRom_UsesIntvnameInformation()
        {
            var paths = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgPath).ToList();
            var romPath = paths[0];
            var cfgPath = paths[1];
            var rom = Rom.Create(romPath, cfgPath);
            Assert.NotNull(rom);
            var name = "Tag-A-Long Todd!";
            var date = "1999";
            var shortName = "Tagger";

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath, name, date, shortName))
            {
                EnsureDatabaseEntryNotPresent(TestRomResources.TestBinCrc);
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.UserDefined,
                    TestRomResources.TestBinCrc,
                    name,
                    date,
                    shortName,
                    ProgramFeatures.GetUnrecognizedRomFeatures(),
                    null);
            }
        }

        [Fact] // 0, m, 0 [m = metadata]
        public void IRomHelpers_GetProgramInformationForBinFormatRomWithMetadata_UsesMetadata()
        {
            var paths = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgMetadataPath).ToList();
            var romPath = paths[0];
            var cfgPath = paths[1];
            var rom = Rom.Create(romPath, cfgPath);
            Assert.NotNull(rom);
            var metadata = rom.GetProgramMetadata();
            Assert.NotNull(metadata);
            var expectedProgramFeatures = ProgramFeatures.GetUnrecognizedRomFeatures();
            expectedProgramFeatures.IntellivisionII = FeatureCompatibility.Incompatible;
            expectedProgramFeatures.Jlp = JlpFeatures.Enhances | JlpFeatures.SaveDataRequired;
            expectedProgramFeatures.JlpFlashMinimumSaveSectors = 128;
            expectedProgramFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp03;
            expectedProgramFeatures.KeyboardComponent = KeyboardComponentFeatures.Incompatible;
            expectedProgramFeatures.LtoFlash = LtoFlashFeatures.Requires | LtoFlashFeatures.LtoFlashMemoryMapped;
            expectedProgramFeatures.Tutorvision = FeatureCompatibility.Incompatible;

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath))
            {
                EnsureDatabaseEntryNotPresent(TestRomResources.TestBinCrc);
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.CfgVarMetadataBlock,
                    TestRomResources.TestBinCrc,
                    metadata.LongNames.First(),
                    metadata.ReleaseDates.First().Date.Year.ToString(CultureInfo.InvariantCulture),
                    metadata.ShortNames.First(),
                    expectedProgramFeatures,
                    metadata.Publishers.First());
            }
        }

        [Fact] // 0, m, i  [m = metadata, i = intvname data]
        public void IRomHelpers_GetProgramInformationForBinFormatRomWithMetadataAndIntvnameData_UsesMetadata()
        {
            var paths = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgMetadataPath).ToList();
            var romPath = paths[0];
            var cfgPath = paths[1];
            var rom = Rom.Create(romPath, cfgPath);
            Assert.NotNull(rom);
            var metadata = rom.GetProgramMetadata();
            Assert.NotNull(metadata);
            var expectedProgramFeatures = ProgramFeatures.GetUnrecognizedRomFeatures();
            expectedProgramFeatures.IntellivisionII = FeatureCompatibility.Incompatible;
            expectedProgramFeatures.Jlp = JlpFeatures.Enhances | JlpFeatures.SaveDataRequired;
            expectedProgramFeatures.JlpFlashMinimumSaveSectors = 128;
            expectedProgramFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp03;
            expectedProgramFeatures.KeyboardComponent = KeyboardComponentFeatures.Incompatible;
            expectedProgramFeatures.LtoFlash = LtoFlashFeatures.Requires | LtoFlashFeatures.LtoFlashMemoryMapped;
            expectedProgramFeatures.Tutorvision = FeatureCompatibility.Incompatible;

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath, "Tag-A-Long Mr. Toad!", "1945", "Tagger"))
            {
                EnsureDatabaseEntryNotPresent(TestRomResources.TestBinCrc);
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.CfgVarMetadataBlock,
                    TestRomResources.TestBinCrc,
                    metadata.LongNames.First(),
                    metadata.ReleaseDates.First().Date.Year.ToString(CultureInfo.InvariantCulture),
                    metadata.ShortNames.First(),
                    expectedProgramFeatures,
                    metadata.Publishers.First());
            }
        }

        [Fact] // d, 0, 0 [d = database]
        public void IRomHelpers_GetProgramInformationForRomFormatRomWithProgramDatabaseEntry_UsesProgramDatabaseEntry()
        {
            var testDatabasePath = "/Databases/IRomHelpers_GetProgramInformationForRomFormatRomWithProgramDatabaseEntry_UsesProgramDatabaseEntry.database";
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomPath).First();
            var testDatabase = new IRomHelpersTestProgramDatabase(testDatabasePath);
            var testRomInfo = new IRomHelpersTestProgramInformation()
            {
                Title = "2113",
                Vendor = "BFS",
                Year = "1997",
                Features = new ProgramFeatures() { JlpHardwareVersion = JlpHardwareVersion.Jlp04 },
                ShortName = "ROHO"
            };
            testRomInfo.AddCrc(TestRomResources.TestRomCrc, "Rock On, Honorable Ones", IncompatibilityFlags.None);
            testDatabase.AddProgram(testRomInfo);
            InitializeTestProgramInfoDatabase(testDatabase);
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath))
            {
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.None,
                    TestRomResources.TestRomCrc,
                    testRomInfo.Title,
                    testRomInfo.Year,
                    testRomInfo.ShortName,
                    testRomInfo.Features,
                    testRomInfo.Vendor);
            }
        }

        [Fact] // d, 0, i [d = database, i = intvname data]
        public void IRomHelpers_GetProgramInformationForBinFormatRomWithProgramDatabaseEntryAndIntvnameData_UsesProgramDatabaseEntry()
        {
            var testDatabasePath = "/Databases/IRomHelpers_GetProgramInformationForRomFormatRomWithProgramDatabaseEntryAndIntvnameData_UsesProgramDatabaseEntry.database";
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath).First();
            var testDatabase = new IRomHelpersTestProgramDatabase(testDatabasePath);
            var testRomInfo = new IRomHelpersTestProgramInformation()
            {
                Title = "i hear only silence now",
                Vendor = "david j",
                Year = "1990",
                Features = new ProgramFeatures() { JlpHardwareVersion = JlpHardwareVersion.Jlp04 },
                ShortName = "eov"
            };
            testRomInfo.AddCrc(TestRomResources.TestBinCrc, "etiquette of violence", IncompatibilityFlags.None);
            testDatabase.AddProgram(testRomInfo);
            InitializeTestProgramInfoDatabase(testDatabase);
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath, "This is a name", "1984", "bob"))
            {
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.None,
                    TestRomResources.TestBinCrc,
                    testRomInfo.Title,
                    testRomInfo.Year,
                    testRomInfo.ShortName,
                    testRomInfo.Features,
                    testRomInfo.Vendor);
            }
        }

        [Fact] // d, m, 0 [d = database, m = metadata]
        public void IRomHelpers_GetProgramInformationForRomFormatRomWithProgramDatabaseEntryAndMetadata_UsesProgramDatabaseEntry()
        {
            var testDatabasePath = "/Databases/IRomHelpers_GetProgramInformationForRomFormatRomWithProgramDatabaseEntryAndMetadata_UsesProgramDatabaseEntry.database";
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomMetadataPath).First();
            var testDatabase = new IRomHelpersTestProgramDatabase(testDatabasePath);
            var testRomInfo = new IRomHelpersTestProgramInformation()
            {
                Title = "Science Friction",
                Vendor = "XTC",
                Year = "1977",
                Features = new ProgramFeatures() { JlpHardwareVersion = JlpHardwareVersion.Jlp04 },
                ShortName = "SF"
            };
            testRomInfo.AddCrc(TestRomResources.TestRomMetadataCrc, "White Music", IncompatibilityFlags.None);
            testDatabase.AddProgram(testRomInfo);
            InitializeTestProgramInfoDatabase(testDatabase);
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            var metadata = rom.GetProgramMetadata() as RomFileMetadataProgramInformation;
            Assert.NotNull(metadata);
            var expectedFeatures = metadata.Features.Clone();
            expectedFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp04;

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath))
            {
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.None,
                    TestRomResources.TestRomMetadataCrc,
                    testRomInfo.Title,
                    testRomInfo.Year,
                    testRomInfo.ShortName,
                    expectedFeatures,
                    testRomInfo.Vendor);
            }
        }

        [Fact] // d, m, i [d = database, m = metadata, i = intvname data]
        public void IRomHelpers_GetProgramInformationForLuigiFromBinFormatRomWithMetadataIntvnameDataAndProgramDatabaseEntry_UsesProgramDatabaseEntry()
        {
            var testDatabasePath = "/Databases/IRomHelpers_GetProgramInformationForLuigiFromBinFormatRomWithMetadataIntvnameDataAndProgramDatabaseEntry_UsesProgramDatabaseEntry.database";
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiWithMetadataPath).First();
            var testDatabase = new IRomHelpersTestProgramDatabase(testDatabasePath);
            var testRomInfo = new IRomHelpersTestProgramInformation()
            {
                Title = "De Doo Doo Doo De Dah Dah Dah",
                Vendor = "The Police",
                Year = "1980",
                Features = new ProgramFeatures() { JlpHardwareVersion = JlpHardwareVersion.Jlp05 },
                ShortName = "IsAllIveGotToSay2U"
            };
            testRomInfo.AddCrc(TestRomResources.TestBinCrc, "Zenyatta Mendatta", IncompatibilityFlags.None);
            testDatabase.AddProgram(testRomInfo);
            InitializeTestProgramInfoDatabase(testDatabase);
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            var metadata = rom.GetProgramMetadata() as LuigiFileMetadataProgramInformation;
            Assert.NotNull(metadata);
            var expectedFeatures = metadata.Features.Clone();
            expectedFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp05;

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath, "Toady FiBodey Modi", "1985", "Z'up"))
            {
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.None,
                    TestRomResources.TestBinCrc,
                    testRomInfo.Title,
                    testRomInfo.Year,
                    testRomInfo.ShortName,
                    expectedFeatures,
                    testRomInfo.Vendor);
            }
        }

        [Fact]
        public void IRomHelpers_GetProgramInformation_InfoSourceHierarchyOperatesAsExpected()
        {
            // NOTE: This only spot checks that primary, secondary, and tertiary sources act as desired for title, short name, and date.
            var testDatabasePath = "/Databases/IRomHelpers_GetProgramInformation_InfoSourceHierarchyOperatesAsExpected.database";
            var testDatabase = new IRomHelpersTestProgramDatabase(testDatabasePath);
            var testRomInfo = new IRomHelpersTestProgramInformation()
            {
                Vendor = "Ammer Einheit Haage",
                Year = "1998"
            };
            testRomInfo.AddCrc(TestRomResources.TestBinCrc, "Odysseus 7", IncompatibilityFlags.None);
            testDatabase.AddProgram(testRomInfo);
            InitializeTestProgramInfoDatabase(testDatabase);
            IEnumerable<string> copiedPaths;
            var storageAccess = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(out copiedPaths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var paths = copiedPaths.ToList();
            var romPath = paths[0];
            var cfgPath = paths[1];
            using (var cfgFile = storageAccess.Open(cfgPath))
            {
                cfgFile.Seek(0, SeekOrigin.End);
                var textToAppend =
@"
[vars]
name = ""Rootin Tootin""
Year = 2112
";
                var buffer = System.Text.Encoding.UTF8.GetBytes(textToAppend);
                cfgFile.Write(buffer, 0, buffer.Length);
            }
            var rom = Rom.Create(romPath, cfgPath);
            Assert.NotNull(rom);
            var metadata = rom.GetProgramMetadata() as CfgFileMetadataProgramInformation;
            Assert.NotNull(metadata);
            var intvnameShortName = "Whee!";

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(romPath, "Rip Van de Coot", "1905", intvnameShortName))
            {
                var programInformation = rom.GetProgramInformation();

                VerifyProgramInformation(
                    programInformation,
                    ProgramInformationOrigin.None,
                    TestRomResources.TestBinCrc,
                    "Rootin Tootin",
                    testRomInfo.Year,
                    intvnameShortName,
                    metadata.Features,
                    testRomInfo.Vendor);
            }
        }

        #endregion // GetProgramInformation Tests

        #region CanExecuteOnDevice Tests

        [Fact]
        public void IRomHelpers_CanExecuteOnDeviceWithNullRom_ReturnsTrue()
        {
            Assert.True(IRomHelpers.CanExecuteOnDevice(null, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0UniqueId)]
        public void IRomHelpers_CanExecuteOnDeviceWithNonLuigiRom_ReturnsTrue(string deviceId)
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestAdvPath);
            var rom = Rom.Create(TestRomResources.TestAdvPath, null);
            Assert.NotNull(rom);

            Assert.True(rom.CanExecuteOnDevice(deviceId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Not a valid Device Unique ID")]
        [InlineData(LuigiScrambleKeyBlock.AnyLTOFlashId)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0UniqueId)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice1UniqueId)]
        public void IRomHelpers_CanExecuteOnDeviceWithNonScrambledLuigiRom_ReturnsTrue(string deviceId)
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestLuigiFromRomPath);
            var rom = Rom.Create(TestRomResources.TestLuigiFromRomPath, null);
            Assert.NotNull(rom);

            Assert.True(rom.CanExecuteOnDevice(deviceId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Not a valid Device Unique ID")]
        [InlineData(LuigiScrambleKeyBlock.AnyLTOFlashId)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0UniqueId)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice1UniqueId)]
        public void IRomHelpers_CanExecuteOnDeviceWithScrambledForAnyLuigiRom_ReturnsTrue(string deviceId)
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForAnyDevicePath);
            var rom = Rom.Create(TestRomResources.TestLuigiScrambledForAnyDevicePath, null);
            Assert.NotNull(rom);

            Assert.True(rom.CanExecuteOnDevice(deviceId));
        }

        [Theory]
        [InlineData(null, true)] // TODO: Is this a bug?
        [InlineData("", true)] // TODO: Is this a bug?
        [InlineData("Not a valid Device Unique ID", false)]
        [InlineData(LuigiScrambleKeyBlock.AnyLTOFlashId, false)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0UniqueId, false)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice1UniqueId, true)]
        public void IRomHelpers_CanExecuteOnDeviceWithScrambledForSpecificLuigiRom_ReturnsTrue(string deviceId, bool expectedCanExecuteOnDevice)
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForDevice1Path);
            var rom = Rom.Create(TestRomResources.TestLuigiScrambledForDevice1Path, null);
            Assert.NotNull(rom);

            Assert.Equal(expectedCanExecuteOnDevice, rom.CanExecuteOnDevice(deviceId));
        }

        #endregion // CanExecuteOnDevice Tests

        #region GetStockCfgFilePath Tests

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        [InlineData(123456)]
        public void IRomHelpers_GetStockCfgFilePathWithInvalidStockPathNumber_ReturnsNull(int stockCfgFileNumber)
        {
            string toolsDir;
            IRomHelpersTestStorageAccess.Initialize(null).WithDefaultToolsDirectory(out toolsDir);

            Assert.Null(IRomHelpers.GetStockCfgFilePath(stockCfgFileNumber));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void IRomHelpers_GetStockCfgFilePath_ReturnsValidPath(int stockCfgFileNumber)
        {
            var storageAccess = IRomHelpersTestStorageAccess.Initialize(null).WithStockCfgResources();

            var stockCfgFilePath = IRomHelpers.GetStockCfgFilePath(stockCfgFileNumber);

            Assert.False(string.IsNullOrEmpty(stockCfgFilePath));
            Assert.True(storageAccess.Exists(stockCfgFilePath));
        }

        #endregion // GetStockCfgFilePath

        #region EnsureCfgFileProvided Tests

        [Fact]
        public void IRomHelpers_EnsureCfgFileProvidedWithNullRom_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => IRomHelpers.EnsureCfgFileProvided(null, null));
        }

        [Theory]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath)]
        [InlineData(TestRomResources.TestRomPath)]
        [InlineData(TestRomResources.TestCc3Path)]
        [InlineData(TestRomResources.TestAdvPath)]
        public void IRomHelpers_EnsureCfgFileProvidedWithNonBinFormatRomAndNoProgramInformation_ReturnsFalse(string romResourcePath)
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(romResourcePath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.False(rom.EnsureCfgFileProvided(null));
        }

        [Theory]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath)]
        [InlineData(TestRomResources.TestRomMetadataPath)]
        public void IRomHelpers_EnsureCfgFileProvidedWithNonBinFormatRomWithProgramInformation_ReturnsFalse(string romResourcePath)
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(romResourcePath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            var programInformation = rom.GetProgramMetadata() as IProgramInformation;
            Assert.NotNull(programInformation);

            Assert.False(rom.EnsureCfgFileProvided(null));
        }

        [Fact]
        public void IRomHelpers_EnsureCfgFileProvidedWhenStockCfgMissing_ReturnsFalse()
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestBinPath).WithStockCfgResources(new[] { 0 });
            var rom = Rom.Create(TestRomResources.TestBinPath, null);
            Assert.NotNull(rom);

            Assert.False(rom.EnsureCfgFileProvided(null));
        }

        [Fact]
        public void IRomHelpers_EnsureCfgFileProvidedWhenStockCfgPresent_ReturnsTrue()
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestBinPath).WithStockCfgResources();
            var rom = Rom.Create(TestRomResources.TestBinPath, null);
            Assert.NotNull(rom);

            Assert.True(rom.EnsureCfgFileProvided(null));
            Assert.False(string.IsNullOrEmpty(rom.ConfigPath));
            Assert.NotEqual(0u, rom.CfgCrc);
        }

        #endregion // EnsureCfgFileProvided Tests

        #region GetLuigiFileMetadata Tests

        [Fact]
        public void IRomHelpers_GetLuigiFileMetadataFromNullRom_ReturnsNull()
        {
            Assert.Null(IRomHelpers.GetLuigiFileMetadata(null));
        }

        [Fact]
        public void IRomHelpers_GetLuigiFileMetadataFromNonLuigiRom_ReturnsNull()
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.Null(rom.GetLuigiFileMetadata());
        }

        [Fact]
        public void IRomHelpers_GetLuigiFileMetadataFromCorruptedLuigiFile_ThrowsNullReferenceException()
        {
            IEnumerable<string> paths;
            var storageAccess = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(out paths, TestRomResources.TestLuigiWithExtraNullBytePath);
            var romPath = paths.First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            Assert.True(storageAccess.IntroduceCorruption(romPath));

            Assert.Throws<NullReferenceException>(() => rom.GetLuigiFileMetadata());
        }

        #endregion // GetLuigiFileMetadata Tests

        #region GetLuigiHeader Tests

        [Fact]
        public void IRomHelpers_GetLuigiHeaderWithNullRom_ReturnsNull()
        {
            Assert.Null(IRomHelpers.GetLuigiHeader(null));
        }

        [Fact]
        public void IRomHelpers_GetLuigiHeaderWithNonLuigiRomFormat_ReturnsNull()
        {
            var rom = EdgeCaseTestingRom.CreateTestingRom(null).WithFormat(RomFormat.None);

            Assert.Null(IRomHelpers.GetLuigiHeader(null));
        }

        [Fact]
        public void IRomHelpers_GetLuigiHeaderWithLuigiRomFormatButNullPath_ReturnsNull()
        {
            var rom = EdgeCaseTestingRom.CreateTestingRom(null).WithFormat(RomFormat.Luigi);

            Assert.Null(IRomHelpers.GetLuigiHeader(null));
        }

        [Fact]
        public void IRomHelpers_GetLuigiHeaderWithLuigiRomFormatAndNonexistentPath_ReturnsNull()
        {
            IRomHelpersTestStorageAccess.Initialize(null);
            var rom = EdgeCaseTestingRom.CreateTestingRom("/Resources/IRomHelpers_GetLuigiHeaderWithLuigiRomFormatAndNonexistentPath_ReturnsNull.luigi").WithFormat(RomFormat.Luigi);

            Assert.Null(IRomHelpers.GetLuigiHeader(null));
        }

        [Fact]
        public void IRomHelpers_GetLuigiHeaderWithLuigiFormatRomFormatWithCorruptHeader_ReturnsNull()
        {
            IEnumerable<string> paths;
            var storageAccess = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(out paths, TestRomResources.TestLuigiWithBadHeaderCrcPath);
            var romPath = paths.First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);
            using (var file = storageAccess.Open(romPath))
            {
                file.WriteByte(1);
            }
            storageAccess.SetLastWriteTimeUtc(romPath, DateTime.UtcNow.AddSeconds(12.3));

            Assert.Null(rom.GetLuigiHeader());
        }

        [Fact]
        public void IRomHelpers_GetLuigiHeaderWithLuigiFormatRom_ReturnsValidLuigiHeader()
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromRomPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.NotNull(rom.GetLuigiHeader());
        }

        #endregion // GetLuigiHeader Tests

        #region IsAlternateRom Tests

        [Fact]
        public void IRomHelpers_IsAlternateRomOnNullRom_ReturnsFalse()
        {
            Assert.False(IRomHelpers.IsAlternateRom(null));
        }

        [Fact]
        public void IRomHelpers_IsAlternateRomOnRomFormatRom_ReturnsFalse()
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.False(rom.IsAlternateRom());
        }

        [Fact]
        public void IRomHelpers_IsAlternateRomOnAlternateRom_ReturnsTrue()
        {
            var romPaths = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestRomPath).ToList();
            var originalRom = Rom.Create(romPaths[0], null);
            Assert.NotNull(originalRom);
            var rom = new AlternateRom(romPaths[1], null, originalRom);
            Assert.NotNull(rom);

            Assert.True(rom.IsAlternateRom());
        }

        #endregion // IsAlternateRom Tests

        #region IsLtoFlashOnlyRom Tests

        [Fact]
        public void IRomHelpers_IsLtoFlashOnlyRomWithNullRom_ReturnsFalse()
        {
            Assert.False(IRomHelpers.IsLtoFlashOnlyRom(null));
        }

        [Theory]
        [InlineData(TestRomResources.TestBinPath, false)]
        [InlineData(TestRomResources.TestRomPath, false)]
        [InlineData(TestRomResources.TestAdvPath, false)]
        [InlineData(TestRomResources.TestCc3Path, false)]
        [InlineData(TestRomResources.TestLuigiFromBinPath, false)]
        [InlineData(TestRomResources.TestLuigiFromRomPath, false)]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath, false)]
        [InlineData(TestRomResources.TestLuigiScrambledForAnyDevicePath, true)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0Path, true)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice1Path, true)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForAnyDevicePath, true)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice0Path, true)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice1Path, true)]
        public void IRomHelpers_IsLtoFlashOnlyRom_ReturnsExpectedResult(string romResourcePath, bool expectedIsLtoFlashOnlyResult)
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(romResourcePath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.Equal(expectedIsLtoFlashOnlyResult, rom.IsLtoFlashOnlyRom());
        }

        #endregion // IsLtoFlashOnlyRom Tests

        #region GetTargetDeviceUniqueId Tests

        [Fact]
        public void IRomHelpers_GetTargetDeviceUniqueIdWithNullRom_ReturnsNull()
        {
            Assert.Null(IRomHelpers.GetTargetDeviceUniqueId(null));
        }

        [Theory]
        [InlineData(TestRomResources.TestBinPath, null)]
        [InlineData(TestRomResources.TestRomPath, null)]
        [InlineData(TestRomResources.TestAdvPath, null)]
        [InlineData(TestRomResources.TestCc3Path, null)]
        [InlineData(TestRomResources.TestLuigiFromBinPath, "")]
        [InlineData(TestRomResources.TestLuigiFromRomPath, "")]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath, "")]
        [InlineData(TestRomResources.TestLuigiScrambledForAnyDevicePath, LuigiScrambleKeyBlock.AnyLTOFlashId)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0Path, TestRomResources.TestLuigiScrambledForDevice0UniqueId)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice1Path, TestRomResources.TestLuigiScrambledForDevice1UniqueId)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForAnyDevicePath, LuigiScrambleKeyBlock.AnyLTOFlashId)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice0Path, TestRomResources.TestLuigiScrambledForDevice0UniqueId)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice1Path, TestRomResources.TestLuigiScrambledForDevice1UniqueId)]
        public void IRomHelpers_GetTargetDeviceUniqueId_ReturnsExpectedTargetUniqueId(string romResourcePath, string expectedtargetUniqueId)
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(romResourcePath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.Equal(expectedtargetUniqueId, rom.GetTargetDeviceUniqueId());
        }

        #endregion // GetTargetDeviceUniqueId Tests

        #region MatchingRomFormat Tests

        [Fact]
        public void IRomHelpers_MatchingRomFormatOfNull_ReturnsFalse()
        {
            Assert.False(IRomHelpers.MatchingRomFormat(null, RomFormat.None, considerOriginalFormat: true));
        }

        public static IEnumerable<object[]> MatchingRomFormatTestData
        {
            get
            {
                var testRomResources = new[]
                {
                    new Tuple<string, RomFormat>(TestRomResources.TestBinPath, RomFormat.Bin),
                    new Tuple<string, RomFormat>(TestRomResources.TestRomPath, RomFormat.Intellicart),
                    new Tuple<string, RomFormat>(TestRomResources.TestCc3Path, RomFormat.CuttleCart3),
                    new Tuple<string, RomFormat>(TestRomResources.TestAdvPath, RomFormat.CuttleCart3Advanced),
                    new Tuple<string, RomFormat>(TestRomResources.TestLuigiFromBinPath, RomFormat.Luigi),
                    new Tuple<string, RomFormat>(TestRomResources.TestLuigiFromRomPath, RomFormat.Luigi),
                    new Tuple<string, RomFormat>(TestRomResources.TestLuigiScrambledForAnyDevicePath, RomFormat.Luigi),
                    new Tuple<string, RomFormat>(TestRomResources.TestLuigiScrambledForDevice0Path, RomFormat.Luigi)
                };

                var romFormats = Enum.GetValues(typeof(RomFormat)).Cast<RomFormat>().Where(f => f != RomFormat.None);
                var considerOriginalFormatValues = new[] { false, true };
                foreach (var testRomResource in testRomResources)
                {
                    foreach (var romFormat in romFormats)
                    {
                        foreach (var considerOriginalFormat in considerOriginalFormatValues)
                        {
                            var expectedMatchResult = testRomResource.Item2 == romFormat;
                            if (!expectedMatchResult)
                            {
                                switch (testRomResource.Item2)
                                {
                                    case RomFormat.CuttleCart3:
                                    case RomFormat.CuttleCart3Advanced:
                                    case RomFormat.Intellicart:
                                        switch (romFormat)
                                        {
                                            case RomFormat.CuttleCart3:
                                            case RomFormat.CuttleCart3Advanced:
                                            case RomFormat.Intellicart:
                                                expectedMatchResult = true;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case RomFormat.Luigi:
                                        if (considerOriginalFormat)
                                        {
                                            switch (testRomResource.Item1)
                                            {
                                                case TestRomResources.TestLuigiFromBinPath:
                                                case TestRomResources.TestLuigiScrambledForAnyDevicePath:
                                                case TestRomResources.TestLuigiScrambledForDevice0Path:
                                                    expectedMatchResult = romFormat == RomFormat.Bin;
                                                    break;
                                                case TestRomResources.TestLuigiFromRomPath:
                                                    expectedMatchResult = romFormat == RomFormat.Rom;
                                                    if (!expectedMatchResult)
                                                    {
                                                        switch (romFormat)
                                                        {
                                                            case RomFormat.CuttleCart3:
                                                            case RomFormat.CuttleCart3Advanced:
                                                            case RomFormat.Intellicart:
                                                                expectedMatchResult = true;
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            yield return new object[] { testRomResource.Item1, romFormat, considerOriginalFormat, expectedMatchResult };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData("MatchingRomFormatTestData")]
        public void IRomHelpers_MatchingRomFormatOfRom_MatchesAsExpected(string romResourcePath, RomFormat format, bool considerOriginalFormat, bool expectedMatchResult)
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(romResourcePath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.Equal(expectedMatchResult, rom.MatchingRomFormat(format, considerOriginalFormat));
        }

        #endregion // MatchingRomFormat Tests

        #region OriginalRom Tests

        [Fact]
        public void IRomHelpers_OriginalRomOfNull_ReturnsNull()
        {
            Assert.Null(IRomHelpers.OriginalRom(null));
        }

        [Theory]
        [InlineData(TestRomResources.TestBinPath)]
        [InlineData(TestRomResources.TestRomPath)]
        [InlineData(TestRomResources.TestAdvPath)]
        [InlineData(TestRomResources.TestCc3Path)]
        [InlineData(TestRomResources.TestLuigiFromBinPath)]
        [InlineData(TestRomResources.TestLuigiFromRomPath)]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath)]
        [InlineData(TestRomResources.TestLuigiScrambledForAnyDevicePath)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice0Path)]
        [InlineData(TestRomResources.TestLuigiScrambledForDevice1Path)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForAnyDevicePath)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice0Path)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForDevice1Path)]
        public void IRomHelpers_OriginalRomOfConcreteRomType_ReturnsInputRom(string romResourcePath)
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(romResourcePath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            Assert.True(object.ReferenceEquals(rom, rom.OriginalRom()));
        }

        [Fact]
        public void IRomHelpers_OriginalRomOfAlternate_ReturnsOriginal()
        {
            var romPaths = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestRomPath).ToList();
            var originalRom = Rom.Create(romPaths[0], null);
            Assert.NotNull(originalRom);
            var rom = new AlternateRom(romPaths[1], null, originalRom);
            Assert.NotNull(rom);

            Assert.True(object.ReferenceEquals(originalRom, rom.OriginalRom()));
        }

        [Fact]
        public void IRomHelpers_OriginalRomOfXmlRom_ReturnsResolvedRom()
        {
            var romPath = IRomHelpersTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomMetadataPath).First();
            var rom = new XmlRom();
            rom.UpdateRomPath(romPath);
            Assert.True(rom.IsValid);
            Assert.NotNull(rom.ResolvedRom);

            Assert.True(object.ReferenceEquals(rom.ResolvedRom, rom.OriginalRom()));
        }

        #endregion // OriginalRom Tests

        private void VerifyProgramInformation(
            IProgramInformation programInformation,
            ProgramInformationOrigin expectedOrigin,
            uint expectedCrc,
            string expectedTitle,
            string expectedYear,
            string expectedShortName,
            IProgramFeatures expectedProgramFeatures,
            string expectedVendor)
        {
            Assert.NotNull(programInformation);
            Assert.Equal(expectedOrigin, programInformation.DataOrigin);
            Assert.Equal(expectedCrc, programInformation.Crcs.First().Crc);
            Assert.Equal(expectedTitle, programInformation.Title);
            Assert.Equal(expectedYear, programInformation.Year);
            Assert.Equal(expectedShortName, programInformation.ShortName);
            Assert.Equal(0, expectedProgramFeatures.CompareTo(programInformation.Features));
            Assert.Equal(expectedVendor, programInformation.Vendor);
        }

        private IProgramInformationTable InitializeTestProgramInfoDatabase(IProgramInformationTable testDatabase)
        {
            var database = ProgramInformationTable.Default as MergedProgramInformationTable;
            Assert.NotNull(database);
            Assert.NotNull(testDatabase);
            var entryCrcsToClear = new HashSet<uint>(testDatabase.Programs.SelectMany(e => e.Crcs.Select(c => c.Crc)));
            foreach (var entryCrc in entryCrcsToClear)
            {
                database.RemoveEntry(entryCrc);
            }
            var conflicts = database.MergeTable(testDatabase);
            Assert.Empty(conflicts);
            return database;
        }

        private bool EnsureDatabaseEntryNotPresent(uint entryCrc)
        {
            var database = ProgramInformationTable.Default as MergedProgramInformationTable;
            var removed = database.RemoveEntry(entryCrc);
            return removed;
        }

        private class IRomHelpersTestStorageAccess : CachedResourceStorageAccess<IRomHelpersTestStorageAccess>
        {
            public static IEnumerable<string> InitializeStorageWithCopiesOfResources(string resourcePath, params string[] additionalResourcePaths)
            {
                IEnumerable<string> copiedResourcePaths;
                InitializeStorageWithCopiesOfResources(out copiedResourcePaths, resourcePath, additionalResourcePaths);
                return copiedResourcePaths;
            }

            public static IRomHelpersTestStorageAccess InitializeStorageWithCopiesOfResources(out IEnumerable<string> copiedResourcePaths, string resourcePath, params string[] additionalResourcePaths)
            {
                var storageAccess = IRomHelpersTestStorageAccess.Initialize(resourcePath, additionalResourcePaths).WithStockCfgResources();

                var fileExtension = Path.GetExtension(resourcePath);
                var randomFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                var directory = Path.GetDirectoryName(resourcePath);
                var randomPath = Path.Combine(directory, Path.ChangeExtension(randomFileName, fileExtension));

                storageAccess.CreateCopyOfResource(resourcePath, randomPath);
                var copiedPaths = new List<string>() { randomPath };
                copiedResourcePaths = copiedPaths;

                if (additionalResourcePaths != null)
                {
                    foreach (var additionalResourcePath in additionalResourcePaths)
                    {
                        fileExtension = Path.GetExtension(additionalResourcePath);
                        randomFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                        directory = Path.GetDirectoryName(additionalResourcePath);
                        randomPath = Path.Combine(directory, Path.ChangeExtension(randomFileName, fileExtension));

                        storageAccess.CreateCopyOfResource(additionalResourcePath, randomPath);
                        copiedPaths.Add(randomPath);
                    }
                }

                return storageAccess;
            }
        }

        private class IRomHelpersTestProgramDatabase : IProgramInformationTable
        {
            public IRomHelpersTestProgramDatabase(string path)
            {
                Path = path;
            }

            private string Path { get; set; }

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

        private class IRomHelpersTestProgramInformation : IProgramInformation
        {
            #region IProgramInformation

            /// <inheritdoc />
            public ProgramInformationOrigin DataOrigin
            {
                get { return ProgramInformationOrigin.None; }
            }

            /// <inheritdoc />
            public string Title { get; set; }

            /// <inheritdoc />
            public string Vendor { get; set; }

            /// <inheritdoc />
            public string Year { get; set; }

            /// <inheritdoc />
            public ProgramFeatures Features { get; set; }

            /// <inheritdoc />
            public string ShortName { get; set; }

            /// <inheritdoc />
            public IEnumerable<CrcData> Crcs
            {
                get { return _crcs; }
            }
            private List<CrcData> _crcs = new List<CrcData>();

            /// <inheritdoc />
            public bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilityFlags)
            {
                var crcData = new CrcData(newCrc, crcDescription, incompatibilityFlags);
                _crcs.Add(crcData);
                return true;
            }

            /// <inheritdoc />
            public bool ModifyCrc(uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags)
            {
                throw new InvalidOperationException();
            }

            #endregion // IProgramInformation
        }

        private class EdgeCaseTestingRom : Rom
        {
            /// <inheritdoc />
            public override RomFormat Format { get; protected set; }

            /// <inheritdoc />
            public override string RomPath { get; protected set; }

            /// <inheritdoc />
            public override string ConfigPath { get; protected set; }

            /// <inheritdoc />
            public override bool IsValid { get; protected set; }

            /// <inheritdoc />
            public override uint Crc
            {
                get { return _crc; }
            }
            private uint _crc;

            /// <inheritdoc />
            public override uint CfgCrc
            {
                get { return _cfgCrc; }
            }
            private uint _cfgCrc;

            public static EdgeCaseTestingRom CreateTestingRom(string romPath)
            {
                var rom = new EdgeCaseTestingRom() { RomPath = romPath };
                return rom;
            }

            /// <inheritdoc />
            public override bool Validate()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override uint RefreshCrc(out bool changed)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override uint RefreshCfgCrc(out bool changed)
            {
                throw new NotImplementedException();
            }

            public EdgeCaseTestingRom WithFormat(RomFormat format)
            {
                Format = format;
                return this;
            }

            public EdgeCaseTestingRom WithConfigPath(string configPath)
            {
                ConfigPath = configPath;
                return this;
            }

            public EdgeCaseTestingRom WithValidity(bool isValid)
            {
                IsValid = isValid;
                return this;
            }

            public EdgeCaseTestingRom WithCrc(uint crc)
            {
                _crc = crc;
                return this;
            }

            public EdgeCaseTestingRom WithCfgCrc(uint cfgCrc)
            {
                _cfgCrc = cfgCrc;
                return this;
            }
        }
    }
}

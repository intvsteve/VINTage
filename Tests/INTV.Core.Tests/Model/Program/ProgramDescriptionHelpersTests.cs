// <copyright file="ProgramDescriptionHelpersTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Restricted.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramDescriptionHelpersTests
    {
        [Fact]
        public void ProgramDescription_GetRomWhenDescriptionIsNull_ThrowsNullReferenceException()
        {
            ProgramDescription description = null;

            Assert.Throws<NullReferenceException>(() => description.GetRom());
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomIsNull_ThrowsNullReferenceException()
        {
            var description = CreateProgramDescription(0x123u, null);

            Assert.Throws<NullReferenceException>(() => description.GetRom());
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomPathsAreNull_ThrowsArgumentNullException()
        {
            ProgramDescriptionHelpersTestStorage.Initialize();
            var rom = new XmlRom();
            var description = CreateProgramDescription(0x456u, rom);

            Assert.Throws<ArgumentNullException>(() => description.GetRom());
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomPathInvalidCfgPathIsNullAndNoAlternatePaths_ReturnsOriginalRom()
        {
            ProgramDescriptionHelpersTestStorage.Initialize();
            var rom = new XmlRom();
            rom.UpdateRomPath("/flargy/bargy/nargy.rom");
            var description = CreateProgramDescription(0x789u, rom);

            Assert.True(object.ReferenceEquals(rom, description.GetRom()));
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomAndCfgPathsInvalidAndNoAlternatePaths_ReturnsOriginalRom()
        {
            ProgramDescriptionHelpersTestStorage.Initialize();
            var rom = new XmlRom();
            rom.UpdateRomPath("/floogy/boogy/noogy.bin");
            rom.UpdateConfigPath("/floogy/boogy/noogy.cfg");
            var description = CreateProgramDescription(0x987u, rom);

            Assert.True(object.ReferenceEquals(rom, description.GetRom()));
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomAndCfgPathsInvalidAndUnbalancedAlternatePaths_ThrowsInvalidOperationException()
        {
            ProgramDescriptionHelpersTestStorage.Initialize();
            var rom = new XmlRom();
            rom.UpdateRomPath("/fleggy/beggy/neggy.bin");
            rom.UpdateConfigPath("/fleggy/beggy/neggy.cfg");
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/biff!");
            var description = CreateProgramDescription(0x654u, rom);
            description.Files = supportFiles;

            var exception = Assert.Throws<InvalidOperationException>(() => description.GetRom());
            Assert.Equal(Resources.Strings.ProgramDescription_MissingAlternateCfgFile, exception.Message);
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomAndCfgPathsInvalidAndAlternatePathsNotFound_ReturnsOriginalRom()
        {
            ProgramDescriptionHelpersTestStorage.Initialize();
            var rom = new XmlRom();
            rom.UpdateRomPath("/flooty/booty/nooty.bin");
            rom.UpdateConfigPath("/flooty/booty/nooty.cfg");
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/banff.bin");
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/barff.bin");
            supportFiles.AddSupportFile(ProgramFileKind.CfgFile, "/banff.cfg");
            supportFiles.AddSupportFile(ProgramFileKind.CfgFile, "/barff.cfg");
            var description = CreateProgramDescription(0x321u, rom);
            description.Files = supportFiles;

            Assert.True(object.ReferenceEquals(rom, description.GetRom()));
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomPathInvalidAndAlternatePathFound_ReturnsAlternateRom()
        {
            var alternatePath = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestCc3Path).First();
            var rom = new XmlRom();
            rom.UpdateRomPath("/floory/boory/noory.rom");
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/grub.bin");
            supportFiles.AddSupportFile(ProgramFileKind.Rom, alternatePath);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/burg.bin");
            var description = CreateProgramDescription(0x135u, rom);
            description.Files = supportFiles;

            var romFromDescription = description.GetRom();

            Assert.False(object.ReferenceEquals(rom, romFromDescription));
            Assert.Equal(alternatePath, romFromDescription.RomPath);
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomPathsInvalidAndAlternatePathsFound_ReturnsAlternateRom()
        {
            var alternatePaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = new XmlRom();
            rom.UpdateRomPath("/fleery/beery/neery.bin");
            rom.UpdateConfigPath("/fleery/beery/neery.cfg");
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/grab.bin");
            supportFiles.AddSupportFile(ProgramFileKind.Rom, alternatePaths[0]);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/barg.bin");
            supportFiles.AddSupportFile(ProgramFileKind.CfgFile, "/grab.cfg");
            supportFiles.AddSupportFile(ProgramFileKind.CfgFile, alternatePaths[1]);
            supportFiles.AddSupportFile(ProgramFileKind.CfgFile, "/barg.cfg");
            var description = CreateProgramDescription(0x246u, rom);
            description.Files = supportFiles;

            var romFromDescription = description.GetRom();

            Assert.False(object.ReferenceEquals(rom, romFromDescription));
            Assert.Equal(alternatePaths[0], romFromDescription.RomPath);
            Assert.Equal(alternatePaths[1], romFromDescription.ConfigPath);
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomPathValidAndNoConfig_ReturnsOriginalRom()
        {
            var romPath = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath).First();
            var rom = new XmlRom();
            rom.UpdateRomPath(romPath);
            var description = CreateProgramDescription(0x357u, rom);

            var romFromDescription = description.GetRom();

            Assert.True(object.ReferenceEquals(rom, romFromDescription));
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomPathValidAndCfgPathInvalidAndNoAlternates_ReturnsOriginalRom()
        {
            var romPath = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath).First();
            var rom = new XmlRom();
            rom.UpdateConfigPath("/flangy/bangy/nangy.cfg");
            rom.UpdateRomPath(romPath);
            var description = CreateProgramDescription(0x468u, rom);

            var romFromDescription = description.GetRom();

            Assert.True(object.ReferenceEquals(rom, romFromDescription));
        }

        [Fact]
        public void ProgramDescription_GetRomWhenRomAndConfigPathsValidAlternatesAreDefined_ReturnsOriginalRom()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var description = CreateProgramDescription(0x579u, rom);

            var romFromDescription = description.GetRom();

            Assert.True(object.ReferenceEquals(rom, romFromDescription));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithNullDescriptionAndInvalidProgramIdentifier_ThrowsArgumentNullException()
        {
            ProgramDescription description = null;

            Assert.Throws<ArgumentNullException>(() => description.IsMatchingProgramDescription(ProgramIdentifier.Invalid));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithInvalidProgramIdentifier_ThrowsArgumentException()
        {
            var description = CreateProgramDescription(0x024u, null);

            Assert.Throws<ArgumentException>(() => description.IsMatchingProgramDescription(ProgramIdentifier.Invalid));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithNonMatchingProgramIdentifier_ReturnsFalse()
        {
            var crc = 0x048u;
            var description = CreateProgramDescription(crc, null);

            Assert.False(description.IsMatchingProgramDescription(new ProgramIdentifier(crc + 1)));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifier_ReturnsTrue()
        {
            var crc = 0x159u;
            var description = CreateProgramDescription(crc, null);

            Assert.True(description.IsMatchingProgramDescription(new ProgramIdentifier(crc)));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithNullDescriptionInvalidProgramIdentifierAndValidRomFormatCfgCrcMustNotMatch_ThrowsArgumentNullException()
        {
            ProgramDescription description = null;

            Assert.Throws<ArgumentNullException>(() => description.IsMatchingProgramDescription(ProgramIdentifier.Invalid, RomFormat.CuttleCart3, cfgCrcMustMatch: false));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithInvalidProgramIdentifierAndValidRomFormatCfgCrcMustNotMatch_ThrowsArgumentException()
        {
            var description = CreateProgramDescription(0x084u, null);

            Assert.Throws<ArgumentException>(() => description.IsMatchingProgramDescription(ProgramIdentifier.Invalid, RomFormat.Luigi, cfgCrcMustMatch: false));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifierButInvalidRomFormat_ReturnsFalse()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);

            var description = CreateProgramDescription(rom.Crc, rom);

            Assert.False(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc), RomFormat.Rom, cfgCrcMustMatch: false));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithPartiallyMatchingProgramIdentifierMatchingRomFormatCfgCrcMustMatch_ReturnsFalse()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);

            var description = CreateProgramDescription(rom.Crc, rom);

            Assert.False(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc), RomFormat.Bin, cfgCrcMustMatch: true));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithPartiallyMatchingProgramIdentifierMatchingRomFormatCfgCrcNeedNottMatch_ReturnsTrue()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);

            var description = CreateProgramDescription(rom.Crc, rom);

            Assert.True(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc), RomFormat.Bin, cfgCrcMustMatch: false));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifierMatchingRomFormatCfgCrcMustMatch_ReturnsTrue()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);

            var description = CreateProgramDescription(rom.Crc, rom);

            Assert.True(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc, rom.CfgCrc), RomFormat.Bin, cfgCrcMustMatch: true));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifierMatchingRomFormatCfgCrcMustMatchWithCodeInfoHasNoCode_ReturnsFalse()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);

            var description = CreateProgramDescription(rom.Crc, rom);

            Assert.False(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc, rom.CfgCrc), RomFormat.Bin, cfgCrcMustMatch: true, code: "tag"));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifierMatchingRomFormatCfgCrcMustMatchWithCodeInfoHasInfoButDoesNotSupportCode_ReturnsFalse()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var code = "tag";
            var information = new IntvFunhouseXmlProgramInformation()
            {
                ProgramTitle = "Tagalong Tod",
                YearString = "2000",
                CrcString = "0x" + rom.Crc.ToString("x8", CultureInfo.InvariantCulture),
                CrcNotesString = string.Empty,
                CrcIncompatibilitiesString = string.Empty,
                CfgFiles = "0",
                Code = code
            };
            var description = new ProgramDescription(rom.Crc, rom, information);

            Assert.False(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc, rom.CfgCrc), RomFormat.Bin, cfgCrcMustMatch: true, code: code));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifierMatchingRomFormatCfgCrcMustMatchWithCodeInfoHasMismatchedCode_ReturnsFalse()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var description = new TestProgramDescription(rom, "tod");

            Assert.False(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc, rom.CfgCrc), RomFormat.Bin, cfgCrcMustMatch: true, code: "tag"));
        }

        [Fact]
        public void ProgramDescription_IsMatchingProgramDescriptionWithMatchingProgramIdentifierMatchingRomFormatCfgCrcMustMatchWithCodeInfoHasMatchingCode_ReturnsTrue()
        {
            var romPaths = ProgramDescriptionHelpersTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var code = "tag";
            var description = new TestProgramDescription(rom, code);

            Assert.True(description.IsMatchingProgramDescription(new ProgramIdentifier(rom.Crc, rom.CfgCrc), RomFormat.Bin, cfgCrcMustMatch: true, code: code));
        }

        private ProgramDescription CreateProgramDescription(uint crc, IRom rom)
        {
            var programDescription = new ProgramDescription(crc, rom, new TestProgramInformation(crc));
            return programDescription;
        }

        private sealed class TestProgramInformation : IProgramInformation
        {
            public TestProgramInformation(uint crc)
            {
                AddCrc(crc, null, IncompatibilityFlags.None);
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

        private sealed class TestProgramDescription : IProgramDescription
        {
            public TestProgramDescription(IRom rom, string code)
            {
                Rom = rom;
                ProgramInformation = new IntvFunhouseXmlProgramInformation()
                {
                    ProgramTitle = "Tagalong Tod",
                    YearString = "2000",
                    CrcString = "0x" + rom.Crc.ToString("x8", CultureInfo.InvariantCulture),
                    CrcNotesString = string.Empty,
                    CrcIncompatibilitiesString = string.Empty,
                    CfgFiles = "0",
                    Code = code
                };
            }

            public uint Crc
            {
                get { return ProgramInformation.Crcs.First().Crc; }
            }

            public IRom Rom { get; set; }

            public IProgramInformation ProgramInformation { get; private set; }

            public string Name { get; set; }

            public string ShortName { get; set; }

            public string Vendor { get; set; }

            public string Year { get; set; }

            public ProgramFeatures Features { get; set; }
        }

        private class ProgramDescriptionHelpersTestStorage : CachedResourceStorageAccess<ProgramDescriptionHelpersTestStorage>
        {
        }
    }
}

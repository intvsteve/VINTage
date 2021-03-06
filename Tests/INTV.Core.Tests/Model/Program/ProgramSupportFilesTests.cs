﻿// <copyright file="ProgramSupportFilesTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramSupportFilesTests
    {
        private static readonly IDictionary<ProgramFileKind, Func<ProgramSupportFiles, string>> ProgramFileKindToGetter = new Dictionary<ProgramFileKind, Func<ProgramSupportFiles, string>>()
        {
            { ProgramFileKind.Rom, f => f.RomImagePath },
            { ProgramFileKind.Box, f => f.DefaultBoxImagePath },
            { ProgramFileKind.Label, f => f.DefaultLabelImagePath },
            { ProgramFileKind.Overlay, f => f.DefaultOverlayImagePath },
            { ProgramFileKind.ManualCover, f => f.DefaultManualImagePath },
            { ProgramFileKind.ManualText, f => f.DefaultManualTextPath },
            { ProgramFileKind.SaveData, f => f.DefaultSaveDataPath },
            { ProgramFileKind.CfgFile, f => f.RomConfigurationFilePath },
            { ProgramFileKind.LuigiFile, f => f.DefaultLtoFlashDataPath },
            { ProgramFileKind.Vignette, f => f.DefaultVignettePath },
            { ProgramFileKind.GenericSupportFile, f => f.DefaultReservedDataPath }
        };

        private static readonly IDictionary<ProgramFileKind, Action<ProgramSupportFiles, string>> ProgramFileKindToSetter = new Dictionary<ProgramFileKind, Action<ProgramSupportFiles, string>>()
        {
            { ProgramFileKind.Rom, (f, p) => f.RomImagePath = p },
            { ProgramFileKind.Box, (f, p) => f.DefaultBoxImagePath = p },
            { ProgramFileKind.Label, (f, p) => f.DefaultLabelImagePath = p },
            { ProgramFileKind.Overlay, (f, p) => f.DefaultOverlayImagePath = p },
            { ProgramFileKind.ManualCover, (f, p) => f.DefaultManualImagePath = p },
            { ProgramFileKind.ManualText, (f, p) => f.DefaultManualTextPath = p },
            { ProgramFileKind.SaveData, (f, p) => f.DefaultSaveDataPath = p },
            { ProgramFileKind.CfgFile, (f, p) => f.RomConfigurationFilePath = p },
            { ProgramFileKind.LuigiFile, (f, p) => f.DefaultLtoFlashDataPath = p },
            { ProgramFileKind.Vignette, (f, p) => f.DefaultVignettePath = p },
            { ProgramFileKind.GenericSupportFile, (f, p) => f.DefaultReservedDataPath = p }
        };

        private static readonly IDictionary<ProgramFileKind, Func<ProgramSupportFiles, IEnumerable<string>>> ProgramFileKindToFilesGetter = new Dictionary<ProgramFileKind, Func<ProgramSupportFiles, IEnumerable<string>>>()
        {
            { ProgramFileKind.Rom, f => f.AlternateRomImagePaths },
            { ProgramFileKind.Box, f => f.BoxImagePaths },
            { ProgramFileKind.Label, f => f.LabelImagePaths },
            { ProgramFileKind.Overlay, f => f.OverlayImagePaths },
            { ProgramFileKind.ManualCover, f => f.ManualCoverImagePaths },
            { ProgramFileKind.ManualText, f => f.ManualPaths },
            { ProgramFileKind.SaveData, f => f.SaveDataPaths },
            { ProgramFileKind.CfgFile, f => f.AlternateRomConfigurationFilePaths },
            { ProgramFileKind.LuigiFile, null },
            { ProgramFileKind.Vignette, null },
            { ProgramFileKind.GenericSupportFile, null }
        };

        private static readonly IEnumerable<KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>> PeripheralsAndPeripheralsHistory = new[]
        {
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(null, null),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(null, Enumerable.Empty<IPeripheral>()),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(Enumerable.Empty<IPeripheral>(), null),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(Enumerable.Empty<IPeripheral>(), Enumerable.Empty<IPeripheral>()),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(null, new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(null, new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(Enumerable.Empty<IPeripheral>(), new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(Enumerable.Empty<IPeripheral>(), new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }, null),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }, null),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }, Enumerable.Empty<IPeripheral>()),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }, Enumerable.Empty<IPeripheral>()),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }, new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }, new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }, new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) }),
            new KeyValuePair<IEnumerable<IPeripheral>, IEnumerable<IPeripheral>>(new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }, new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId), new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice1UniqueId) }),
        };

        private static readonly Lazy<IEnumerable<ProgramFileKind>> AllFileKinds = new Lazy<IEnumerable<ProgramFileKind>>(() => Enum.GetValues(typeof(ProgramFileKind)).Cast<ProgramFileKind>());
        private static readonly Lazy<IEnumerable<ProgramFileKind>> SupportFileKinds = new Lazy<IEnumerable<ProgramFileKind>>(GetSupportFileKinds);

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRom_CreatesEmptySupportFiles()
        {
            var supportFiles = new ProgramSupportFiles(null);

            Assert.Null(supportFiles.Rom);
            Assert.Null(supportFiles.RomImagePath);
            Assert.Null(supportFiles.RomConfigurationFilePath);
            Assert.Empty(supportFiles.AlternateRomImagePaths);
            Assert.Empty(supportFiles.AlternateRomConfigurationFilePaths);
            Assert.Empty(supportFiles.BoxImagePaths);
            Assert.Empty(supportFiles.OverlayImagePaths);
            Assert.Empty(supportFiles.ManualCoverImagePaths);
            Assert.Empty(supportFiles.LabelImagePaths);
            Assert.Empty(supportFiles.ManualPaths);
            Assert.Empty(supportFiles.SaveDataPaths);
            Assert.Null(supportFiles.DefaultBoxImagePath);
            Assert.Null(supportFiles.DefaultOverlayImagePath);
            Assert.Null(supportFiles.DefaultManualImagePath);
            Assert.Null(supportFiles.DefaultLabelImagePath);
            Assert.Null(supportFiles.DefaultManualTextPath);
            Assert.Null(supportFiles.DefaultSaveDataPath);
            Assert.Null(supportFiles.DefaultLtoFlashDataPath);
            Assert.Null(supportFiles.DefaultVignettePath);
            Assert.Null(supportFiles.DefaultReservedDataPath);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithInvalidXmlRomAndSetBinAndCfgPath_CreatesExpectedValies()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var rom = new XmlRom();
            var testConfigPath = "/some/bogus.cfg";
            rom.UpdateConfigPath(testConfigPath);
            var testRomPath = "/some/bogus.bin";
            rom.UpdateRomPath(testRomPath);

            var supportFiles = new ProgramSupportFiles(rom);

            Assert.NotNull(supportFiles.Rom);
            Assert.Equal(testRomPath, supportFiles.RomImagePath);
            Assert.Equal(testConfigPath, supportFiles.RomConfigurationFilePath);
            Assert.Empty(supportFiles.AlternateRomImagePaths);
            Assert.Empty(supportFiles.AlternateRomConfigurationFilePaths);
            Assert.Empty(supportFiles.BoxImagePaths);
            Assert.Empty(supportFiles.OverlayImagePaths);
            Assert.Empty(supportFiles.ManualCoverImagePaths);
            Assert.Empty(supportFiles.LabelImagePaths);
            Assert.Empty(supportFiles.ManualPaths);
            Assert.Empty(supportFiles.SaveDataPaths);
            Assert.Null(supportFiles.DefaultBoxImagePath);
            Assert.Null(supportFiles.DefaultOverlayImagePath);
            Assert.Null(supportFiles.DefaultManualImagePath);
            Assert.Null(supportFiles.DefaultLabelImagePath);
            Assert.Null(supportFiles.DefaultManualTextPath);
            Assert.Null(supportFiles.DefaultSaveDataPath);
            Assert.Null(supportFiles.DefaultLtoFlashDataPath);
            Assert.Null(supportFiles.DefaultVignettePath);
            Assert.Null(supportFiles.DefaultReservedDataPath);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetRomImagePath_CreatesXmlRomAndSetsRomImagePath()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            var testRomPath = "/nothing/to/see/here.rom";
            supportFiles.RomImagePath = testRomPath;

            Assert.NotNull(supportFiles.Rom);
            Assert.True(supportFiles.Rom is XmlRom);
            Assert.Equal(testRomPath, supportFiles.RomImagePath);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetRomImagePathToNull_ThrowsArgumentNullException()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);
            Assert.Null(supportFiles.Rom);
            Assert.Null(supportFiles.RomImagePath);

            Assert.Throws<ArgumentNullException>(() => supportFiles.RomImagePath = null);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetRomConfigurationFilePath_CreatesXmlRomAndSetsConfigurationFilePath()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            var testCfgPath = "/move/along/move/along.cfg";
            supportFiles.RomConfigurationFilePath = testCfgPath;

            Assert.NotNull(supportFiles.Rom);
            Assert.True(supportFiles.Rom is XmlRom);
            Assert.Equal(testCfgPath, supportFiles.RomConfigurationFilePath);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetRomConfigurationFilePathToNull_CreatesXmlRom()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);
            Assert.Null(supportFiles.Rom);
            Assert.Null(supportFiles.RomConfigurationFilePath);

            supportFiles.RomConfigurationFilePath = null;

            Assert.NotNull(supportFiles.Rom);
            Assert.Null(supportFiles.RomConfigurationFilePath);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetAlternateRomImagePathsToNull_ThrowsArgumentNullException()
        {
            var supportFiles = new ProgramSupportFiles(null);

            Assert.Throws<ArgumentNullException>(() => supportFiles.AlternateRomImagePaths = null);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetAlternateRomImagePaths_SetsAlternateRomImagePaths()
        {
            var supportFiles = new ProgramSupportFiles(null);

            var alternatePaths = new List<string>() { "/test/alt/0.bin", "test/alt/0.rom" };
            supportFiles.AlternateRomImagePaths = alternatePaths;

            Assert.Equal(alternatePaths, supportFiles.AlternateRomImagePaths);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetAlternateRomImagePathsTwice_AccumulatesAlternateRomImagePaths()
        {
            var supportFiles = new ProgramSupportFiles(null);

            var alternatePaths = new List<string>() { "/test/alt/0.bin", "test/alt/0.rom" };
            supportFiles.AlternateRomImagePaths = alternatePaths;
            supportFiles.AlternateRomImagePaths = alternatePaths;

            Assert.Equal(alternatePaths.Concat(alternatePaths), supportFiles.AlternateRomImagePaths);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetAlternateRomConfigurationFilePathsToNull_ThrowsArgumentNullException()
        {
            var supportFiles = new ProgramSupportFiles(null);

            Assert.Throws<ArgumentNullException>(() => supportFiles.AlternateRomConfigurationFilePaths = null);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetAlternateRomConfigurationFilePaths_SetsAlternateRomConfigurationFilePaths()
        {
            var supportFiles = new ProgramSupportFiles(null);

            var alternateConfugrationFilePaths = new List<string>() { "/test/alt/0.cfg", "test/alt2/0.cfg" };
            supportFiles.AlternateRomConfigurationFilePaths = alternateConfugrationFilePaths;

            Assert.Equal(alternateConfugrationFilePaths, supportFiles.AlternateRomConfigurationFilePaths);
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomSetAlternateRomConfigurationFilePathsTwice_AccumulatesAlternateRomConfigurationFilePaths()
        {
            var supportFiles = new ProgramSupportFiles(null);

            var alternateConfugrationFilePaths = new List<string>() { "/test/alt/0.cfg", "test/alt2/0.cfg" };
            supportFiles.AlternateRomConfigurationFilePaths = alternateConfugrationFilePaths;
            supportFiles.AlternateRomConfigurationFilePaths = alternateConfugrationFilePaths;

            Assert.Equal(alternateConfugrationFilePaths.Concat(alternateConfugrationFilePaths), supportFiles.AlternateRomConfigurationFilePaths);
        }

        public static IEnumerable<object[]> SetDefaultPathsTestData
        {
            get
            {
                Func<ProgramSupportFiles, string> getter = f => f.DefaultBoxImagePath;
                Action<ProgramSupportFiles, string> setter = (f, p) => f.DefaultBoxImagePath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/box/foo.jpg", getter, setter };

                getter = f => f.DefaultOverlayImagePath;
                setter = (f, p) => f.DefaultOverlayImagePath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/overlay/bar.jpg", getter, setter };

                getter = f => f.DefaultManualImagePath;
                setter = (f, p) => f.DefaultManualImagePath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/manual/baz.jpg", getter, setter };

                getter = f => f.DefaultLabelImagePath;
                setter = (f, p) => f.DefaultLabelImagePath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/cart/foolable.jpg", getter, setter };

                getter = f => f.DefaultManualTextPath;
                setter = (f, p) => f.DefaultManualTextPath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/foo.txt", getter, setter };

                getter = f => f.DefaultSaveDataPath;
                setter = (f, p) => f.DefaultSaveDataPath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/foo.jlp", getter, setter };

                getter = f => f.DefaultLtoFlashDataPath;
                setter = (f, p) => f.DefaultLtoFlashDataPath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/foo.luigi", getter, setter };

                getter = f => f.DefaultVignettePath;
                setter = (f, p) => f.DefaultVignettePath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/vignette/foo.vig", getter, setter };

                getter = f => f.DefaultReservedDataPath;
                setter = (f, p) => f.DefaultReservedDataPath = p;
                yield return new object[] { null, getter, setter };
                yield return new object[] { string.Empty, getter, setter };
                yield return new object[] { "/Users/test/ROMs/foo.dat", getter, setter };
            }
        }

        [Theory]
        [MemberData("SetDefaultPathsTestData")]
        public void ProgramSupportFiles_CreateWithNullRomSetDefaultPath_SetsDefaultPath(string newPath, Func<ProgramSupportFiles, string> getter, Action<ProgramSupportFiles, string> setter)
        {
            var supportFiles = new ProgramSupportFiles(null);

            setter(supportFiles, newPath);

            if (string.IsNullOrEmpty(newPath))
            {
                Assert.Null(getter(supportFiles));
            }
            else
            {
                Assert.Equal(newPath, getter(supportFiles));
            }
        }

        [Theory]
        [MemberData("SetDefaultPathsTestData")]
        public void ProgramSupportFiles_CreateWithNullRomSetDefaultPathThenSetToNull_SetsDefaultPathToNull(string newPath, Func<ProgramSupportFiles, string> getter, Action<ProgramSupportFiles, string> setter)
        {
            var supportFiles = new ProgramSupportFiles(null);

            setter(supportFiles, newPath);
            if (string.IsNullOrEmpty(newPath))
            {
                Assert.Null(getter(supportFiles));
            }
            else
            {
                Assert.Equal(newPath, getter(supportFiles));
            }
            setter(supportFiles, null);

            Assert.Null(getter(supportFiles));
        }

        public static IEnumerable<object[]> DeserializeFromXmlStringTestData
        {
            get
            {
                const string RomFormatRomWithNoAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/tester/ROMs/tagalong.rom</RomImagePath>
      <AlternateRomImagePaths />
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    RomFormatRomWithNoAlternatesAsXml,
                    "/Users/tester/ROMs/tagalong.rom",
                    null,
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string RomFormatRomWithCfgPathAndNoAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/tester/ROMs/tagalong.rom</RomImagePath>
      <RomConfigurationFilePath>/Users/test/projects/Locutus/tools/1.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths />
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    RomFormatRomWithCfgPathAndNoAlternatesAsXml,
                    "/Users/tester/ROMs/tagalong.rom",
                    "/Users/test/projects/Locutus/tools/1.cfg",
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string RomFormatRomWithOneAlternateAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.rom</RomImagePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.rom</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    RomFormatRomWithOneAlternateAsXml,
                    "/Users/test/ROMs/tagalong.rom",
                    null,
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.rom" },
                    Enumerable.Empty<string>()
                };

                const string RomFormatRomWithMultipleAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.rom</RomImagePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.rom</string>
        <string>/Users/test/ROMBackups/tagalong.rom</string>
        <string>/Users/test/ROMBackups2/tagalong.rom</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    RomFormatRomWithMultipleAlternatesAsXml,
                    "/Users/test/ROMs/tagalong.rom",
                    null,
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.rom", "/Users/test/ROMBackups/tagalong.rom", "/Users/test/ROMBackups2/tagalong.rom" },
                    Enumerable.Empty<string>()
                };

                const string BinFormatRomWithNoAlternateAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.bin</RomImagePath>
      <RomConfigurationFilePath>/Users/test/projects/Locutus/tools/0.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths />
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    BinFormatRomWithNoAlternateAsXml,
                    "/Users/test/ROMs/tagalong.bin",
                    "/Users/test/projects/Locutus/tools/0.cfg",
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string BinFormatRomWithOneAlternateAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.bin</RomImagePath>
      <RomConfigurationFilePath>/Users/test/projects/Locutus/tools/0.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.bin</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg</string>
      </AlternateRomConfigurationFilePaths>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    BinFormatRomWithOneAlternateAsXml,
                    "/Users/test/ROMs/tagalong.bin",
                    "/Users/test/projects/Locutus/tools/0.cfg",
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.bin" },
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg" }
                };

                const string BinFormatRomWithMultipleAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.bin</RomImagePath>
      <RomConfigurationFilePath>/Users/test/projects/Locutus/tools/0.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.bin</string>
        <string>/Users/test/ROMBackups/tagalong.bin</string>
        <string>/Users/test/ROMBackups2/tagalong.bin</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg</string>
        <string>/Users/test/ROMBackups/tagalong.cfg</string>
        <string>/Users/test/ROMBackups2/tagalong.cfg</string>
      </AlternateRomConfigurationFilePaths>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    BinFormatRomWithMultipleAlternatesAsXml,
                    "/Users/test/ROMs/tagalong.bin",
                    "/Users/test/projects/Locutus/tools/0.cfg",
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.bin", "/Users/test/ROMBackups/tagalong.bin", "/Users/test/ROMBackups2/tagalong.bin" },
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg", "/Users/test/ROMBackups/tagalong.cfg", "/Users/test/ROMBackups2/tagalong.cfg" }
                };

                const string BinFormatRomWithMultipleAlternatesTooFewCfgAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.bin</RomImagePath>
      <RomConfigurationFilePath>/Users/test/projects/Locutus/tools/0.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.bin</string>
        <string>/Users/test/ROMBackups/tagalong.bin</string>
        <string>/Users/test/ROMBackups2/tagalong.bin</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg</string>
        <string>/Users/test/ROMBackups/tagalong.cfg</string>
      </AlternateRomConfigurationFilePaths>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    BinFormatRomWithMultipleAlternatesTooFewCfgAlternatesAsXml,
                    "/Users/test/ROMs/tagalong.bin",
                    "/Users/test/projects/Locutus/tools/0.cfg",
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.bin", "/Users/test/ROMBackups/tagalong.bin", "/Users/test/ROMBackups2/tagalong.bin" },
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg", "/Users/test/ROMBackups/tagalong.cfg" }
                };

                const string BinFormatRomWithMultipleAlternatesTooFewBinAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.bin</RomImagePath>
      <RomConfigurationFilePath>/Users/test/projects/Locutus/tools/0.cfg</RomConfigurationFilePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.bin</string>
        <string>/Users/test/ROMBackups2/tagalong.bin</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg</string>
        <string>/Users/test/ROMBackups/tagalong.cfg</string>
        <string>/Users/test/ROMBackups2/tagalong.cfg</string>
      </AlternateRomConfigurationFilePaths>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    BinFormatRomWithMultipleAlternatesTooFewBinAlternatesAsXml,
                    "/Users/test/ROMs/tagalong.bin",
                    "/Users/test/projects/Locutus/tools/0.cfg",
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.bin", "/Users/test/ROMBackups2/tagalong.bin" },
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.cfg", "/Users/test/ROMBackups/tagalong.cfg", "/Users/test/ROMBackups2/tagalong.cfg" }
                };

                const string LuigiFormatRomWithNoAlternateAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.luigi</RomImagePath>
      <AlternateRomImagePaths />
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    LuigiFormatRomWithNoAlternateAsXml,
                    "/Users/test/ROMs/tagalong.luigi",
                    null,
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string LuigiFormatRomWithOneAlternateAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.luigi</RomImagePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.luigi</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    LuigiFormatRomWithOneAlternateAsXml,
                    "/Users/test/ROMs/tagalong.luigi",
                    null,
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.luigi" },
                    Enumerable.Empty<string>()
                };

                const string LuigiFormatRomWithMultipleAlternatesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <RomImagePath>/Users/test/ROMs/tagalong.luigi</RomImagePath>
      <AlternateRomImagePaths>
        <string>/Users/test/Documents/LTO Flash/ROMs/tagalong.luigi</string>
        <string>/Users/test/ROMBackups/tagalong.luigi</string>
        <string>/Users/test/ROMBackups2/tagalong.luigi</string>
      </AlternateRomImagePaths>
      <AlternateRomConfigurationFilePaths />
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    LuigiFormatRomWithMultipleAlternatesAsXml,
                    "/Users/test/ROMs/tagalong.luigi",
                    null,
                    new[] { "/Users/test/Documents/LTO Flash/ROMs/tagalong.luigi", "/Users/test/ROMBackups/tagalong.luigi", "/Users/test/ROMBackups2/tagalong.luigi" },
                    Enumerable.Empty<string>()
                };
            }
        }

        [Theory]
        [MemberData("DeserializeFromXmlStringTestData")]
        public void ProgramSupportFiles_DeserializeFromString_CreatesExpectedProgramSupportFiles(string xmlString, string expectedRomImagePath, string expectedRomConfigurationFilePath, IEnumerable<string> expectedAlternateRomImagePaths, IEnumerable<string> expectedAlternateRomConfigurationFilePaths)
        {
            ProgramSupportFilesTestStorage.Initialize();

            var supportFiles = DeserializeFromXmlString(xmlString);

            Assert.Equal(expectedRomImagePath, supportFiles.RomImagePath);
            Assert.Equal(expectedRomConfigurationFilePath, supportFiles.RomConfigurationFilePath);
            Assert.Equal(expectedAlternateRomImagePaths, supportFiles.AlternateRomImagePaths);
            Assert.Equal(expectedAlternateRomConfigurationFilePaths, supportFiles.AlternateRomConfigurationFilePaths);
        }

        public static IEnumerable<object[]> DeserializeFromXmlStringImageSupportFilesTestData
        {
            get
            {
                const string WithBoxImagePathAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <BoxImagePath>/Users/tester/ROMs/boxes/tagalong.jpg</BoxImagePath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithBoxImagePathAsXml,
                    "/Users/tester/ROMs/boxes/tagalong.jpg",
                    null,
                    null,
                    null,
                    new[] { "/Users/tester/ROMs/boxes/tagalong.jpg" },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string WithOverlayImageAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <OverlayImagePath>/Users/tester/ROMs/overlays/tagalong-001.jpg</OverlayImagePath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithOverlayImageAsXml,
                    null,
                    "/Users/tester/ROMs/overlays/tagalong-001.jpg",
                    null,
                    null,
                    Enumerable.Empty<string>(),
                    new[] { "/Users/tester/ROMs/overlays/tagalong-001.jpg" },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string WithManualImagePathAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <ManualImagePath>/Users/tester/ROMs/manuals/tagalong-001-tn.jpg</ManualImagePath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithManualImagePathAsXml,
                    null,
                    null,
                    "/Users/tester/ROMs/manuals/tagalong-001-tn.jpg",
                    null,
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>(),
                    new[] { "/Users/tester/ROMs/manuals/tagalong-001-tn.jpg" },
                    Enumerable.Empty<string>()
                };

                const string WithLabelImagePathAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <LabelImagePath>/Users/tester/ROMs/cart/tagalong-000.jpg</LabelImagePath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithLabelImagePathAsXml,
                    null,
                    null,
                    null,
                    "/Users/tester/ROMs/cart/tagalong-000.jpg",
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>(),
                    new[] { "/Users/tester/ROMs/cart/tagalong-000.jpg" }
                };

                const string WithAllImageSupportFilesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <BoxImagePath>/Users/tester/ROMs/boxes/tagalong-000.jpg</BoxImagePath>
      <OverlayImagePath>/Users/tester/ROMs/overlays/tagalong-000.png</OverlayImagePath>
      <ManualImagePath>/Users/tester/ROMs/manuals/tagalong-000.jpg</ManualImagePath>
      <LabelImagePath>/Users/tester/ROMs/cart/tagalong-000-tn.jpg</LabelImagePath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithAllImageSupportFilesAsXml,
                    "/Users/tester/ROMs/boxes/tagalong-000.jpg",
                    "/Users/tester/ROMs/overlays/tagalong-000.png",
                    "/Users/tester/ROMs/manuals/tagalong-000.jpg",
                    "/Users/tester/ROMs/cart/tagalong-000-tn.jpg",
                    new[] { "/Users/tester/ROMs/boxes/tagalong-000.jpg" },
                    new[] { "/Users/tester/ROMs/overlays/tagalong-000.png" },
                    new[] { "/Users/tester/ROMs/manuals/tagalong-000.jpg" },
                    new[] { "/Users/tester/ROMs/cart/tagalong-000-tn.jpg" }
                };
            }
        }

        [Theory]
        [MemberData("DeserializeFromXmlStringImageSupportFilesTestData")]
        public void ProgramSupportFiles_DeserializeFromString_CreatesExpectedProgramSupportImageFiles(string xmlString, string expectedBoxImagePath, string expectedOverlayImagePath, string expectedManualImagePath, string expectedLabelImagePath, IEnumerable<string> expectedBoxImagePaths, IEnumerable<string> expectedOverlayImagePaths, IEnumerable<string> expectedManualImagePaths, IEnumerable<string> expectedLabelImagePaths)
        {
            ProgramSupportFilesTestStorage.Initialize();

            var supportFiles = DeserializeFromXmlString(xmlString);

            Assert.Equal(expectedBoxImagePath, supportFiles.DefaultBoxImagePath);
            Assert.Equal(expectedOverlayImagePath, supportFiles.DefaultOverlayImagePath);
            Assert.Equal(expectedManualImagePath, supportFiles.DefaultManualImagePath);
            Assert.Equal(expectedLabelImagePath, supportFiles.DefaultLabelImagePath);
            Assert.Equal(expectedBoxImagePaths, supportFiles.BoxImagePaths);
            Assert.Equal(expectedOverlayImagePaths, supportFiles.OverlayImagePaths);
            Assert.Equal(expectedManualImagePaths, supportFiles.ManualCoverImagePaths);
            Assert.Equal(expectedLabelImagePaths, supportFiles.LabelImagePaths);
        }

        public static IEnumerable<object[]> DeserializeFromXmlStringOtherSupportFilesTestData
        {
            get
            {
                const string WithManualAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <ManualPath>/Users/tester/ROMs/manual/tagalong.txt</ManualPath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithManualAsXml,
                    "/Users/tester/ROMs/manual/tagalong.txt",
                    null,
                    null,
                    new[] { "/Users/tester/ROMs/manual/tagalong.txt" },
                    Enumerable.Empty<string>()
                };

                const string WithSaveDataAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <SaveDataPath>/Users/tester/ROMs/savegame/tagalong.jlp</SaveDataPath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithSaveDataAsXml,
                    null,
                    "/Users/tester/ROMs/savegame/tagalong.jlp",
                    null,
                    Enumerable.Empty<string>(),
                    new[] { "/Users/tester/ROMs/savegame/tagalong.jlp" }
                };

                const string WithLtoFlashRomPathAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <LTOFlashROMPath>/Users/tester/Documents/LTO Flash/ROMsCache/ab345eae/tagalong.luigi</LTOFlashROMPath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithLtoFlashRomPathAsXml,
                    null,
                    null,
                    "/Users/tester/Documents/LTO Flash/ROMsCache/ab345eae/tagalong.luigi",
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                };

                const string WithAllOtherSupportFilesAsXml = @"<?xml version=""1.0""?>
    <ProgramSupportFiles>
      <ManualPath>/Users/tester/ROMs/manual/tagalong.txt</ManualPath>
      <SaveDataPath>/Users/tester/ROMs/savegame/tagalong.jlp</SaveDataPath>
      <LTOFlashROMPath>/Users/tester/Documents/LTO Flash/ROMsCache/ab345eae/tagalong.luigi</LTOFlashROMPath>
    </ProgramSupportFiles>
";
                yield return new object[]
                {
                    WithAllOtherSupportFilesAsXml,
                    "/Users/tester/ROMs/manual/tagalong.txt",
                    "/Users/tester/ROMs/savegame/tagalong.jlp",
                    "/Users/tester/Documents/LTO Flash/ROMsCache/ab345eae/tagalong.luigi",
                    new[] { "/Users/tester/ROMs/manual/tagalong.txt" },
                    new[] { "/Users/tester/ROMs/savegame/tagalong.jlp" }
                };
            }
        }

        [Theory]
        [MemberData("DeserializeFromXmlStringOtherSupportFilesTestData")]
        public void ProgramSupportFiles_DeserializeFromString_CreatesExpectedProgramSupportOtherFiles(string xmlString, string expectedManualPath, string expectedSaveDataPath, string expectedLuigiFilePath, IEnumerable<string> expectedManualPaths, IEnumerable<string> expectedSaveDataPaths)
        {
            ProgramSupportFilesTestStorage.Initialize();

            var supportFiles = DeserializeFromXmlString(xmlString);

            Assert.Equal(expectedManualPath, supportFiles.DefaultManualTextPath);
            Assert.Equal(expectedSaveDataPath, supportFiles.DefaultSaveDataPath);
            Assert.Equal(expectedLuigiFilePath, supportFiles.DefaultLtoFlashDataPath);
            Assert.Equal(expectedManualPaths, supportFiles.ManualPaths);
            Assert.Equal(expectedSaveDataPaths, supportFiles.SaveDataPaths);
        }

        public static IEnumerable<object[]> AddSupportFileTestData
        {
            get
            {
                foreach (var supportFileKind in SupportFileKinds.Value)
                {
                    var getter = ProgramFileKindToGetter[supportFileKind];
                    var allFilesGetter = ProgramFileKindToFilesGetter[supportFileKind];
                    yield return new object[] { supportFileKind, null, null, getter, allFilesGetter };
                    var testPath = "/Users/Testy McTesterson/" + supportFileKind + "/testFile.ext";
                    var expectedFilePath = testPath;
                    switch (supportFileKind)
                    {
                        case ProgramFileKind.Rom:
                        case ProgramFileKind.CfgFile:
                            expectedFilePath = null;
                            break;
                        default:
                            break;
                    }
                    yield return new object[] { supportFileKind, testPath, expectedFilePath, getter, allFilesGetter };
                }
            }
        }

        [Theory]
        [MemberData("AddSupportFileTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAddSupportFile_AddsSupportFile(ProgramFileKind whichFile, string filePath, string expectedFilePath, Func<ProgramSupportFiles, string> getter, Func<ProgramSupportFiles, IEnumerable<string>> allFilesGetter)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            supportFiles.AddSupportFile(whichFile, filePath);

            Assert.Equal(expectedFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = new[] { filePath };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }
        }

        [Theory]
        [MemberData("AddSupportFileTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAddSupportFileTwice_AddsTwoSupportFiles(ProgramFileKind whichFile, string filePath, string expectedFilePath, Func<ProgramSupportFiles, string> getter, Func<ProgramSupportFiles, IEnumerable<string>> allFilesGetter)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            supportFiles.AddSupportFile(whichFile, filePath);
            var otherSupportFile = string.IsNullOrEmpty(filePath) ? "/Users/Testy McTesterson/" + whichFile + ".file" : filePath + ".bak";
            supportFiles.AddSupportFile(whichFile, otherSupportFile);

            Assert.Equal(expectedFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = new[] { filePath, otherSupportFile };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }
        }

        public static IEnumerable<object[]> SetDefaultSupportFileTestData
        {
            get
            {
                foreach (var supportFileKind in SupportFileKinds.Value)
                {
                    var getter = ProgramFileKindToGetter[supportFileKind];
                    var setter = ProgramFileKindToSetter[supportFileKind];
                    var allFilesGetter = ProgramFileKindToFilesGetter[supportFileKind];
                    var testPath = "/Users/Testy McTesterson/" + supportFileKind + "/testFile.ext";
                    var expectedFilePath = testPath;
                    switch (supportFileKind)
                    {
                        case ProgramFileKind.Rom:
                        case ProgramFileKind.CfgFile:
                            break;
                        default:
                            yield return new object[] { null, setter, null, getter, allFilesGetter };
                            yield return new object[] { string.Empty, setter, null, getter, allFilesGetter };
                            yield return new object[] { testPath, setter, expectedFilePath, getter, allFilesGetter };
                            break;
                    }
                }
            }
        }

        [Theory]
        [MemberData("SetDefaultSupportFileTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAddSameSupportFileTwice_AddsSupportFileOnce(string filePath, Action<ProgramSupportFiles, string> setter, string expectedFilePath, Func<ProgramSupportFiles, string> getter, Func<ProgramSupportFiles, IEnumerable<string>> allFilesGetter)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            setter(supportFiles, filePath);
            setter(supportFiles, filePath);

            Assert.Equal(expectedFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = string.IsNullOrEmpty(filePath) ? Enumerable.Empty<string>() : new[] { filePath };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }
        }

        public static IEnumerable<object[]> SetNonNullOrEmptyDefaultSupportFileTestData
        {
            get
            {
                foreach (var supportFileKind in SupportFileKinds.Value)
                {
                    var getter = ProgramFileKindToGetter[supportFileKind];
                    var setter = ProgramFileKindToSetter[supportFileKind];
                    var allFilesGetter = ProgramFileKindToFilesGetter[supportFileKind];
                    var testPath = "/Users/Testy McTesterson/" + supportFileKind + "/testFile.ext";
                    var expectedFilePath = testPath;
                    switch (supportFileKind)
                    {
                        case ProgramFileKind.Rom:
                        case ProgramFileKind.CfgFile:
                            break;
                        default:
                            yield return new object[] { testPath, setter, expectedFilePath, getter, allFilesGetter };
                            break;
                    }
                }
            }
        }

        [Theory]
        [MemberData("SetNonNullOrEmptyDefaultSupportFileTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAddSameSupportFileThenSetToNull_ResultsInNullValue(string filePath, Action<ProgramSupportFiles, string> setter, string expectedFilePath, Func<ProgramSupportFiles, string> getter, Func<ProgramSupportFiles, IEnumerable<string>> allFilesGetter)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);
            setter(supportFiles, filePath);
            Assert.Equal(expectedFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = new[] { filePath };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }

            setter(supportFiles, null);

            Assert.Null(getter(supportFiles));
            if (allFilesGetter != null)
            {
                Assert.Equal(Enumerable.Empty<string>(), allFilesGetter(supportFiles));
            }
        }

        [Theory]
        [MemberData("SetNonNullOrEmptyDefaultSupportFileTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAddSameSupportFileThenSetToEmptyString_ResultsInNullValue(string filePath, Action<ProgramSupportFiles, string> setter, string expectedFilePath, Func<ProgramSupportFiles, string> getter, Func<ProgramSupportFiles, IEnumerable<string>> allFilesGetter)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);
            setter(supportFiles, filePath);
            Assert.Equal(expectedFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = new[] { filePath };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }

            setter(supportFiles, string.Empty);

            Assert.Null(getter(supportFiles));
            if (allFilesGetter != null)
            {
                Assert.Equal(Enumerable.Empty<string>(), allFilesGetter(supportFiles));
            }
        }

        [Theory]
        [MemberData("SetNonNullOrEmptyDefaultSupportFileTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAddSameSupportFileThenSetToADifferentFile_ReplacesValue(string filePath, Action<ProgramSupportFiles, string> setter, string expectedFilePath, Func<ProgramSupportFiles, string> getter, Func<ProgramSupportFiles, IEnumerable<string>> allFilesGetter)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);
            setter(supportFiles, filePath);
            Assert.Equal(expectedFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = new[] { filePath };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }

            var newFilePath = "/new" + filePath;
            setter(supportFiles, newFilePath);

            Assert.Equal(newFilePath, getter(supportFiles));
            if (allFilesGetter != null)
            {
                var allFiles = new[] { newFilePath };
                Assert.Equal(allFiles, allFilesGetter(supportFiles));
            }
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomAddBogusSupportFile_ThrowsKeyNotFoundException()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            var bogusSupportFile = ProgramFileKind.NumFileKinds + 1;
            Assert.Throws<KeyNotFoundException>(() => supportFiles.AddSupportFile(bogusSupportFile, "/har/dee/har.hoot"));
        }

        public static IEnumerable<object[]> AllSupportFileKindsTestData
        {
            get
            {
                return AllFileKinds.Value.Select(k => new object[] { k });
            }
        }

        [Theory]
        [MemberData("AllSupportFileKindsTestData")]
        public void ProgramSupportFiles_CreateWithNullRomGetSupportFileState_ReturnsProgramSupportFileStateNone(ProgramFileKind whichFile)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            Assert.Equal(ProgramSupportFileState.None, supportFiles.GetSupportFileState(whichFile));
        }

        public static IEnumerable<object[]> AllSupportFileKindsExceptForRomTestData
        {
            get
            {
                return AllFileKinds.Value.Where(k => k != ProgramFileKind.Rom).Select(k => new object[] { k });
            }
        }

        [Theory]
        [MemberData("AllSupportFileKindsExceptForRomTestData")]
        public void ProgramSupportFiles_CreateWithNullRomAndValidateSupportFileWithNullAndZeroArguments_ReturnsProgramSupportFileStateNone(ProgramFileKind whichFile)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            Assert.Equal(ProgramSupportFileState.None, supportFiles.ValidateSupportFile(whichFile, 0, null, null, null, reportIfModified: false));
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomAndValidateBogusSupportFileWithNullAndZeroArguments_DoesNotThrow()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            var bogusSupportFile = ProgramFileKind.NumFileKinds + 1;

            Assert.Equal(ProgramSupportFileState.None, supportFiles.ValidateSupportFile(bogusSupportFile, 0, null, null, null, reportIfModified: false));
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithNullRomAndValidateRomFileKindWithNullAndZeroArguments_ThrowsNullReferenceException()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var supportFiles = new ProgramSupportFiles(null);

            Assert.Throws<NullReferenceException>(() => supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified: false));
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithXmlRomWithNoRomPathAndValidateRomFileKindWithNullAndZeroArguments_ThrowsArgumentNullException()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var rom = new XmlRom();
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.Throws<ArgumentNullException>(() => supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified: false));
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithXmlRomWithMissingRomPathAndValidateRomFileKindWithNullAndZeroArguments_ReturnsProgramSupportFileStateMissing()
        {
            ProgramSupportFilesTestStorage.Initialize();
            var rom = new XmlRom();
            var testRomPath = "/some/bogus.rom";
            rom.UpdateRomPath(testRomPath);
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.Equal(ProgramSupportFileState.Missing, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified: false));
        }

        [Theory]
        [InlineData(false, ProgramSupportFileState.None)]
        [InlineData(true, ProgramSupportFileState.PresentAndUnchanged)]
        public void ProgramSupportFiles_CreateWithXmlRomWithValidRomPathAndValidateRomFileKindWithNullAndZeroArguments_ReturnsCorrectProgramSupportFileState(bool reportIfModified, ProgramSupportFileState expectedState)
        {
            var testRomPath = ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestRomPath).First();
            var rom = new XmlRom();
            rom.UpdateRomPath(testRomPath);
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.Equal(expectedState, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified));
        }

        [Theory]
        [InlineData(false, 1u, ProgramSupportFileState.None)]
        [InlineData(false, TestRomResources.TestRomCrc, ProgramSupportFileState.None)]
        [InlineData(true, 1u, ProgramSupportFileState.PresentButModified)]
        [InlineData(true, TestRomResources.TestRomCrc, ProgramSupportFileState.PresentAndUnchanged)]
        public void ProgramSupportFiles_CreateWithXmlRomWithValidRomPathAndValidateRomFileKindWithNullArgumentsAndNonzeroCrc_ReturnsCorrectProgramSupportFileState(bool reportIfModified, uint crc, ProgramSupportFileState expectedState)
        {
            var testRomPath = ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestRomPath).First();
            var rom = new XmlRom();
            rom.UpdateRomPath(testRomPath);
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.Equal(expectedState, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, crc, null, null, null, reportIfModified));
        }

        [Theory]
        [InlineData(false, 1u, ProgramSupportFileState.None)]
        [InlineData(false, TestRomResources.TestBinCrc, ProgramSupportFileState.None)]
        [InlineData(true, 1u, ProgramSupportFileState.PresentButModified)]
        [InlineData(true, TestRomResources.TestBinCrc, ProgramSupportFileState.PresentAndUnchanged)]
        public void ProgramSupportFiles_CreateWithXmlBinFormatRomWithValidRomPathValidCfgPathAndValidateRomFileKindWithNullArgumentsAndNonzeroCrc_ReturnsCorrectProgramSupportFileState(bool reportIfModified, uint crc, ProgramSupportFileState expectedState)
        {
            var romPaths = ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.Equal(expectedState, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, crc, null, null, null, reportIfModified));
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithXmlBinFormatRomWithValidModifiedRomPathValidCfgFileAndValidateRomFileKindWithNullArgumentsAndNonzeroCrc_ReturnsProgramSupportFileStatePresentButModified()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = ProgramSupportFilesTestStorage.Initialize(out romPaths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var supportFiles = new ProgramSupportFiles(rom);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            using (var s = storageAccess.Open(romPaths[0]))
            {
                s.Seek(0, SeekOrigin.End);
                var bogusFileBytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                s.Write(bogusFileBytes, 0, bogusFileBytes.Length);
            }

            Assert.Equal(ProgramSupportFileState.PresentButModified, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, TestRomResources.TestBinCrc, null, null, null, reportIfModified: true));
        }

        [Fact]
        public void ProgramSupportFiles_CreateWithXmlBinFormatRomWithValidRomPathValidModifiedCfgFileAndValidateRomFileKindWithNullArgumentsAndNonzeroCrc_ReturnsProgramSupportFileStatePresentButModified()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = ProgramSupportFilesTestStorage.Initialize(out romPaths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(romPaths[0], romPaths[1]);
            var supportFiles = new ProgramSupportFiles(rom);
            Assert.Equal(TestRomResources.TestBinCrc, rom.Crc);
            Assert.Equal(TestRomResources.TestCfgCrc, rom.CfgCrc);

            using (var s = storageAccess.Open(romPaths[1]))
            {
                s.Seek(0, SeekOrigin.End);
                var cfgFileContent = "\n[vars]\nvoice=1";
                var cfgFileBytes = Encoding.UTF8.GetBytes(cfgFileContent);
                s.Write(cfgFileBytes, 0, cfgFileBytes.Length);
            }

            Assert.Equal(ProgramSupportFileState.PresentButModified, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, TestRomResources.TestBinCrc, null, null, null, reportIfModified: true));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ProgramSupportFiles_CreateWithXmlRomWithMissingRomPathAndMissingAlternateAndValidateRomFileKindWithNullAndZeroArguments_ReturnsProgramSupportFileStateMissing(bool reportIfModified)
        {
            ProgramSupportFilesTestStorage.Initialize();
            var rom = new XmlRom();
            var testRomPath = "/some/bogus.rom";
            rom.UpdateRomPath(testRomPath);
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/some/backup/bogus.rom");

            Assert.Equal(ProgramSupportFileState.Missing, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ProgramSupportFiles_CreateWithXmlRomWithMissingRomPathAndValidAlternateAndValidateRomFileKindWithNullAndZeroArguments_ReturnsProgramSupportFileStateMissingWithAlternateFound(bool reportIfModified)
        {
            ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestRomPath);
            var rom = new XmlRom();
            var testRomPath = "/some/bogus.rom";
            rom.UpdateRomPath(testRomPath);
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, "/some/backup/bogus.rom");
            supportFiles.AddSupportFile(ProgramFileKind.Rom, TestRomResources.TestRomPath);

            Assert.Equal(ProgramSupportFileState.MissingWithAlternateFound, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified));
        }

        public static IEnumerable<object[]> ValidateScrambledLuigiWithNullDescriptionTestData
        {
            get
            {
                return PeripheralsAndPeripheralsHistory
                    .Where(d => ((d.Key != null) && d.Key.Any()) || ((d.Value != null) && d.Value.Any()))
                    .Select(d => new object[] { d.Key, d.Value });
            }
        }

        [Theory]
        [MemberData("ValidateScrambledLuigiWithNullDescriptionTestData")]
        public void ProgramSupportFiles_CreateWithScrambledForAnyLtoFlashAndValidateRomFileKindWithPeripheralInformationNullProgramDescription_ThrowsNullReferenceException(IEnumerable<IPeripheral> peripherals, IEnumerable<IPeripheral> connectedPeripheralsHistory)
        {
            var romPath = ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path).First();
            var rom = Rom.Create(romPath, null);
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.Throws<NullReferenceException>(() => supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, peripherals, connectedPeripheralsHistory, reportIfModified: false));
        }

        public static IEnumerable<object[]> ValidateScrambledLuigiRomFileRomStateTestData
        {
            get
            {
                // string luigiPath, IEnumerable<IPeripheral> peripherals, IEnumerable<IPeripheral> connectedPeripheralsHistory, ProgramSupportFileState expectedState
                var luigiPath = TestRomResources.TestLuigiFromBinPath; // not scrambled
                var crc = TestRomResources.TestBinCrc;
                foreach (var peripheralsTestData in PeripheralsAndPeripheralsHistory)
                {
                    yield return new object[] { luigiPath, crc, peripheralsTestData.Key, peripheralsTestData.Value, ProgramSupportFileState.PresentAndUnchanged };
                }

                luigiPath = TestRomResources.TestLuigiScrambledForAnyDevicePath;
                crc = 0x83bda1cd; // scrambled ROMs don't retain original CRC - so embedded CRC is different
                foreach (var peripheralsTestData in PeripheralsAndPeripheralsHistory)
                {
                    var peripherals = peripheralsTestData.Key;
                    var connectedPeripheralsHistory = peripheralsTestData.Value;
                    var expectedState = (peripherals != null) && peripherals.Any() ? ProgramSupportFileState.RequiredPeripheralAvailable : ProgramSupportFileState.RequiredPeripheralNotAttached;
                    yield return new object[] { luigiPath, crc, peripherals, connectedPeripheralsHistory, expectedState };
                }

                luigiPath = TestRomResources.TestLuigiScrambledForDevice1Path;
                crc = 0xb0439381; // scrambled ROMs don't retain original CRC - so embedded CRC is different
                foreach (var peripheralsTestData in PeripheralsAndPeripheralsHistory)
                {
                    var peripherals = peripheralsTestData.Key;
                    var connectedPeripheralsHistory = peripheralsTestData.Value;
                    var anyPeripherals = (peripherals != null) && peripherals.Any();
                    var anyPeripheralsHistory = (connectedPeripheralsHistory != null) && connectedPeripheralsHistory.Any();
                    var peripheralsContainsDevice1 = anyPeripherals && peripherals.Any(p => ((MockLtoFlashDevice)p).UniqueId == TestRomResources.TestLuigiScrambledForDevice1UniqueId);
                    var peripheralsHistoryContainsDevice1 = anyPeripheralsHistory && connectedPeripheralsHistory.Any(p => ((MockLtoFlashDevice)p).UniqueId == TestRomResources.TestLuigiScrambledForDevice1UniqueId);
                    var expectedState = peripheralsHistoryContainsDevice1 ? ProgramSupportFileState.RequiredPeripheralNotAttached : ProgramSupportFileState.RequiredPeripheralUnknown;
                    if (anyPeripherals)
                    {
                        expectedState = peripheralsContainsDevice1 ? ProgramSupportFileState.RequiredPeripheralAvailable : ProgramSupportFileState.RequiredPeripheralIncompatible;
                    }
                    yield return new object[] { luigiPath, crc, peripherals, connectedPeripheralsHistory, expectedState };
                }
            }
        }

        [Theory]
        [MemberData("ValidateScrambledLuigiRomFileRomStateTestData")]
        public void ProgramSupportFiles_CreateWithScrambledForAnyLtoFlashAndValidateRomFileKindWithPeripheralInformation_ReturnsCorrectProgramSupportFileState(string luigiPath, uint crc, IEnumerable<IPeripheral> peripherals, IEnumerable<IPeripheral> connectedPeripheralsHistory, ProgramSupportFileState expectedState)
        {
            var romPath = ProgramSupportFilesTestStorage.Initialize(luigiPath).First();
            var rom = Rom.Create(romPath, null);
            var information = new UserSpecifiedProgramInformation(crc);
            var description = new ProgramDescription(crc, rom, information);
            var supportFiles = new ProgramSupportFiles(rom);

            var state = supportFiles.ValidateSupportFile(ProgramFileKind.Rom, crc, description, peripherals, connectedPeripheralsHistory, reportIfModified: true);

            Assert.Equal(expectedState, state);
        }

        [Theory]
        [MemberData("ValidateScrambledLuigiRomFileRomStateTestData")]
        public void ProgramSupportFiles_CreateWithScrambledForAnyLtoFlashAndValidateRomFileKindWithPeripheralInformationAgainWithNullPeripherals_ReturnsCorrectProgramSupportFileState(string luigiPath, uint crc, IEnumerable<IPeripheral> peripherals, IEnumerable<IPeripheral> connectedPeripheralsHistory, ProgramSupportFileState expectedState)
        {
            var romPath = ProgramSupportFilesTestStorage.Initialize(luigiPath).First();
            var rom = Rom.Create(romPath, null);
            var information = new UserSpecifiedProgramInformation(crc);
            var description = new ProgramDescription(crc, rom, information);
            var supportFiles = new ProgramSupportFiles(rom);
            var state = supportFiles.ValidateSupportFile(ProgramFileKind.Rom, crc, description, peripherals, connectedPeripheralsHistory, reportIfModified: true);
            Assert.Equal(expectedState, state);

            Assert.Equal(expectedState, supportFiles.ValidateSupportFile(ProgramFileKind.Rom, crc, null, null, null, reportIfModified: true));
        }

        [Fact]
        public void ProgramSupportFiles_ValidateRomFileWithScrambledAlternateThenAgainWhenFileExistsWithoutPeripherals_ReturnsExpectedState()
        {
            var romPath = ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path).First();
            var rom = new XmlRom();
            rom.UpdateRomPath("/some/bogus.luigi");
            var supportFiles = new ProgramSupportFiles(rom);
            supportFiles.AddSupportFile(ProgramFileKind.Rom, romPath);
            var state = supportFiles.ValidateSupportFile(ProgramFileKind.Rom, 0, null, null, null, reportIfModified: false);
            Assert.Equal(ProgramSupportFileState.MissingWithAlternateFound, state);

            var crc = 0x8c29e37d;
            rom.UpdateRomPath(romPath);
            var peripherals = new[] { new MockLtoFlashDevice(TestRomResources.TestLuigiScrambledForDevice0UniqueId) };
            state = supportFiles.ValidateSupportFile(ProgramFileKind.Rom, crc, null, null, null, reportIfModified: true);
            Assert.Equal(ProgramSupportFileState.PresentAndUnchanged, state);
        }

        [Theory]
        [InlineData(false, ProgramSupportFileState.None)]
        [InlineData(true, ProgramSupportFileState.PresentButModified)]
        public void ProgramSupportFiles_ValidateCorruptedRomFile_ReturnsExpectedState(bool reportIfModified, ProgramSupportFileState expectedState)
        {
            IReadOnlyList<string> paths;
            var storageAccess = ProgramSupportFilesTestStorage.Initialize(out paths, TestRomResources.TestRomMetadataPath);
            var rom = Rom.Create(paths[0], null);
            Assert.Equal(TestRomResources.TestRomMetadataCrc, rom.Crc);
            var supportFiles = new ProgramSupportFiles(rom);

            Assert.True(storageAccess.IntroduceCorruption(paths[0]));
            var state = supportFiles.ValidateSupportFile(ProgramFileKind.Rom, TestRomResources.TestRomMetadataCrc, null, null, null, reportIfModified);

            Assert.Equal(expectedState, state);
        }

        [Fact]
        public void ProgramSupportFiles_CreateAndCopy_CreatesValidCopy()
        {
            var romPath = ProgramSupportFilesTestStorage.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path).First();
            var rom = new XmlRom();
            rom.UpdateRomPath("/some/bogus.itv");
            var supportFiles = new ProgramSupportFiles(rom, "/box/path.jpg", "/manual/path.jpg", "/manual/path.txt", "/overlay/path.png", "/label/path.jpg");
            foreach (var supportFileKind in SupportFileKinds.Value)
            {
                supportFiles.AddSupportFile(supportFileKind, supportFileKind.ToString());
                supportFiles.AddSupportFile(supportFileKind, supportFileKind.ToString() + "2");
            }

            var supportFilesCopy = supportFiles.Copy();

            Assert.True(object.ReferenceEquals(supportFiles.Rom, supportFilesCopy.Rom));
            Assert.Equal(supportFiles.RomImagePath, supportFilesCopy.RomImagePath);
            Assert.Equal(supportFiles.RomConfigurationFilePath, supportFilesCopy.RomConfigurationFilePath);
            Assert.False(object.ReferenceEquals(supportFiles.AlternateRomImagePaths, supportFilesCopy.AlternateRomImagePaths));
            Assert.Equal(supportFiles.AlternateRomImagePaths, supportFilesCopy.AlternateRomImagePaths);
            Assert.False(object.ReferenceEquals(supportFiles.AlternateRomConfigurationFilePaths, supportFilesCopy.AlternateRomConfigurationFilePaths));
            Assert.Equal(supportFiles.AlternateRomConfigurationFilePaths, supportFilesCopy.AlternateRomConfigurationFilePaths);
            Assert.False(object.ReferenceEquals(supportFiles.BoxImagePaths, supportFilesCopy.BoxImagePaths));
            Assert.Equal(supportFiles.BoxImagePaths, supportFilesCopy.BoxImagePaths);
            Assert.False(object.ReferenceEquals(supportFiles.OverlayImagePaths, supportFilesCopy.OverlayImagePaths));
            Assert.Equal(supportFiles.OverlayImagePaths, supportFilesCopy.OverlayImagePaths);
            Assert.False(object.ReferenceEquals(supportFiles.ManualCoverImagePaths, supportFilesCopy.ManualCoverImagePaths));
            Assert.Equal(supportFiles.ManualCoverImagePaths, supportFilesCopy.ManualCoverImagePaths);
            Assert.False(object.ReferenceEquals(supportFiles.LabelImagePaths, supportFilesCopy.LabelImagePaths));
            Assert.Equal(supportFiles.LabelImagePaths, supportFilesCopy.LabelImagePaths);
            Assert.False(object.ReferenceEquals(supportFiles.ManualPaths, supportFilesCopy.ManualPaths));
            Assert.Equal(supportFiles.ManualPaths, supportFilesCopy.ManualPaths);
            Assert.False(object.ReferenceEquals(supportFiles.SaveDataPaths, supportFilesCopy.SaveDataPaths));
            Assert.Equal(supportFiles.SaveDataPaths, supportFilesCopy.SaveDataPaths);
            Assert.Equal(supportFiles.DefaultBoxImagePath, supportFilesCopy.DefaultBoxImagePath);
            Assert.Equal(supportFiles.DefaultOverlayImagePath, supportFilesCopy.DefaultOverlayImagePath);
            Assert.Equal(supportFiles.DefaultManualImagePath, supportFilesCopy.DefaultManualImagePath);
            Assert.Equal(supportFiles.DefaultLabelImagePath, supportFilesCopy.DefaultLabelImagePath);
            Assert.Equal(supportFiles.DefaultManualTextPath, supportFilesCopy.DefaultManualTextPath);
            Assert.Equal(supportFiles.DefaultSaveDataPath, supportFilesCopy.DefaultSaveDataPath);
            Assert.Equal(supportFiles.DefaultLtoFlashDataPath, supportFilesCopy.DefaultLtoFlashDataPath);
            Assert.Equal(supportFiles.DefaultVignettePath, supportFilesCopy.DefaultVignettePath);
            Assert.Equal(supportFiles.DefaultReservedDataPath, supportFilesCopy.DefaultReservedDataPath);
        }

        private static ProgramSupportFiles DeserializeFromXmlString(string xmlString)
        {
            ProgramSupportFiles supportFiles;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ProgramSupportFiles));
                supportFiles = serializer.Deserialize(stream) as ProgramSupportFiles;
            }
            return supportFiles;
        }

        private static IEnumerable<ProgramFileKind> GetSupportFileKinds()
        {
            var exclude = new[] { ProgramFileKind.None, ProgramFileKind.NumFileKinds };
            return AllFileKinds.Value.Except(exclude);
        }

        private class ProgramSupportFilesTestStorage : CachedResourceStorageAccess<ProgramSupportFilesTestStorage>
        {
        }

        private class MockLtoFlashDevice : Peripheral
        {
            public MockLtoFlashDevice(string uniqueId)
            {
                Name = "MockLtoFlashDevice";
                UniqueId = uniqueId;
            }

            public string UniqueId { get; private set; }

            public override IEnumerable<IConnection> Connections
            {
                get
                {
                    return null;
                }
                protected set
                {
                    throw new NotImplementedException();
                }
            }

            public override IEnumerable<IConfigurableFeature> ConfigurableFeatures
            {
                get { yield break; }
            }

            public override bool IsRomCompatible(IProgramDescription programDescription)
            {
                var isCompatible = true;

                // NOTE: The lack of null safety here is intentional, to replicate behavior of the
                // actual LTO Flash! device implementation.
                if (programDescription.Rom.IsLtoFlashOnlyRom())
                {
                    isCompatible = programDescription.Rom.CanExecuteOnDevice(UniqueId);
                }
                return isCompatible;
            }
        }
    }
}

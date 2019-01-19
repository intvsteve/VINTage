// <copyright file="ProgramFileKindHelpersTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramFileKindHelpersTests
    {
        public static IEnumerable<object[]> AllFileKinds
        {
            get
            {
                foreach (var fileKind in Enum.GetValues(typeof(ProgramFileKind)).Cast<ProgramFileKind>())
                {
                    yield return new object[] { fileKind };
                }
            }
        }

        [Fact]
        public void ProgramFileKind_FileKinds_HasExpectedNumberOfValues()
        {
            Assert.Equal((int)ProgramFileKind.NumFileKinds, ProgramFileKindHelpers.FileKinds.Count());
        }

        [Fact]
        public void ProgramFileKind_RomFileExtensionsUsingCfgFilesWithNoCustomExtensionsRegistered_IsNotEmpty()
        {
            var expectedExtensions = new[] { ".bin", ".itv", ".int" };

            Assert.False(expectedExtensions.Except(ProgramFileKindHelpers.RomFileExtensionsThatUseCfgFiles).Any());
        }

        [Fact]
        public void ProgramFileKind_SupportFileKinds_ContainsExpectedValues()
        {
            var expectedKinds = new[] { ProgramFileKind.Box, ProgramFileKind.ManualCover, ProgramFileKind.ManualText, ProgramFileKind.Overlay, ProgramFileKind.Label, ProgramFileKind.SaveData, ProgramFileKind.CfgFile, ProgramFileKind.Vignette, ProgramFileKind.GenericSupportFile };

            AssertCollectionsAreEquivalent(expectedKinds, ProgramFileKindHelpers.SupportFileKinds);
        }

        [Fact]
        public void ProgramFileKind_SupportFileSuffixes_ContainsExpectedValues()
        {
            var expectedSuffixes = new[] { "_box", "_label", "_overlay", "_manual", string.Empty };

            AssertCollectionsAreEquivalent(expectedSuffixes, ProgramFileKindHelpers.SupportFileSuffixes);
        }

        [Fact]
        public void ProgramFileKind_SupportFileSubdirectories_ContainsExpectedValues()
        {
            var expectedSubdirectories = new[] { "box", "boxes", "label", "labels", "cart", "overlay", "overlays", "manual", "manuals", "savedata", "savegame", "savegames" };

            AssertCollectionsAreEquivalent(expectedSubdirectories, ProgramFileKindHelpers.SupportFileSubdirectories);
        }

        [Fact]
        public void ProgramFileKind_SupportFileExtensions_ContainsExpectedValues()
        {
            var expectedFileExtensions = new[] { ".jpg", ".bmp", ".png", ".gif", ".txt", ".jlp", ".cfg", string.Empty, };

            AssertCollectionsAreEquivalent(expectedFileExtensions, ProgramFileKindHelpers.SupportFileExtensions);
        }

        [Theory]
        [InlineData(ProgramFileKind.None, null)]
        [InlineData(ProgramFileKind.Rom, null)]
        [InlineData(ProgramFileKind.Box, "_box")]
        [InlineData(ProgramFileKind.Label, "_label")]
        [InlineData(ProgramFileKind.Overlay, "_overlay")]
        [InlineData(ProgramFileKind.ManualCover, "_manual")]
        [InlineData(ProgramFileKind.ManualText, "")]
        [InlineData(ProgramFileKind.SaveData, "")]
        [InlineData(ProgramFileKind.CfgFile, "")]
        [InlineData(ProgramFileKind.LuigiFile, "")]
        [InlineData(ProgramFileKind.Vignette, "")]
        [InlineData(ProgramFileKind.GenericSupportFile, "")]
        public void ProgramFileKind_GetSuffix_ReturnsCorrectSuffix(ProgramFileKind fileKind, string expectedSuffix)
        {
            Assert.Equal(expectedSuffix, fileKind.GetSuffix());
        }

        [Theory]
        [InlineData(ProgramFileKind.NumFileKinds)]
        [InlineData((ProgramFileKind)(-1))]
        public void ProgramFileKind_GetSuffixForUnsupportedFileKind_ThrowsKeyNotFoundException(ProgramFileKind fileKind)
        {
            Assert.Throws<KeyNotFoundException>(() => fileKind.GetSuffix());
        }

        public static IEnumerable<object[]> GetSubdirectoriesTestData
        {
            get
            {
                yield return new object[] { ProgramFileKind.None, Enumerable.Empty<string>() };
                yield return new object[] { ProgramFileKind.Rom, Enumerable.Empty<string>() };
                yield return new object[] { ProgramFileKind.Box, new List<string>() { "box", "boxes" } };
                yield return new object[] { ProgramFileKind.Label, new List<string>() { "label", "labels", "cart" } };
                yield return new object[] { ProgramFileKind.Overlay, new List<string>() { "overlay", "overlays" } };
                yield return new object[] { ProgramFileKind.ManualCover, new List<string>() { "manual", "manuals" } };
                yield return new object[] { ProgramFileKind.ManualText, new List<string>() { "manual", "manuals" } };
                yield return new object[] { ProgramFileKind.SaveData, new List<string>() { "savedata", "savegame", "savegames" } };
                yield return new object[] { ProgramFileKind.CfgFile, Enumerable.Empty<string>() };
                yield return new object[] { ProgramFileKind.LuigiFile, Enumerable.Empty<string>() };
                yield return new object[] { ProgramFileKind.Vignette, Enumerable.Empty<string>() };
                yield return new object[] { ProgramFileKind.GenericSupportFile, Enumerable.Empty<string>() };
            }
        }

        [Theory]
        [MemberData("GetSubdirectoriesTestData")]
        public void ProgramFileKind_GetSubdirectories_ReturnsCorrectSubdirectories(ProgramFileKind fileKind, IEnumerable<string> expectedSubdirecories)
        {
            AssertCollectionsAreEquivalent(expectedSubdirecories, fileKind.GetSubdirectories());
        }

        [Theory]
        [InlineData(ProgramFileKind.NumFileKinds)]
        [InlineData((ProgramFileKind)(-1))]
        public void ProgramFileKind_GetSubdirectoriesForUnsupportedFileKind_ThrowsKeyNotFoundException(ProgramFileKind fileKind)
        {
            Assert.Throws<KeyNotFoundException>(() => fileKind.GetSubdirectories());
        }

        public static IEnumerable<object[]> FileExtensionsTestData
        {
            get
            {
                yield return new object[] { ProgramFileKind.None, new List<string>() { string.Empty } };
                yield return new object[] { ProgramFileKind.Rom, new List<string>() { ".rom", ".cc3", ".bin", ".itv", ".int", ".luigi" } };
                yield return new object[] { ProgramFileKind.Box, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } };
                yield return new object[] { ProgramFileKind.Label, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } };
                yield return new object[] { ProgramFileKind.Overlay, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } };
                yield return new object[] { ProgramFileKind.ManualCover, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } };
                yield return new object[] { ProgramFileKind.ManualText, new List<string>() { ".txt" } };
                yield return new object[] { ProgramFileKind.SaveData, new List<string>() { ".jlp" } };
                yield return new object[] { ProgramFileKind.CfgFile, new List<string>() { ".cfg" } };
                yield return new object[] { ProgramFileKind.LuigiFile, new List<string>() { ".luigi" } };
                yield return new object[] { ProgramFileKind.Vignette, new List<string>() { string.Empty } };
                yield return new object[] { ProgramFileKind.GenericSupportFile, new List<string>() { string.Empty } };
            }
        }

        [Theory]
        [MemberData("FileExtensionsTestData")]
        public void ProgramFileKind_FileExtensions_ReturnsCorrectFileExtensions(ProgramFileKind fileKind, IEnumerable<string> expectedFileExtensions)
        {
            AssertCollectionsAreEquivalent(expectedFileExtensions, fileKind.FileExtensions());
        }

        [Theory]
        [InlineData(ProgramFileKind.NumFileKinds)]
        [InlineData((ProgramFileKind)(-1))]
        public void ProgramFileKind_FileExtensionsForUnsupportedFileKind_ThrowsKeyNotFoundException(ProgramFileKind fileKind)
        {
            Assert.Throws<KeyNotFoundException>(() => fileKind.FileExtensions());
        }

        [Theory]
        [InlineData(ProgramFileKind.None, "")]
        [InlineData(ProgramFileKind.Rom, ".rom")]
        [InlineData(ProgramFileKind.Box, ".jpg")]
        [InlineData(ProgramFileKind.Label, ".jpg")]
        [InlineData(ProgramFileKind.Overlay, ".jpg")]
        [InlineData(ProgramFileKind.ManualCover, ".jpg")]
        [InlineData(ProgramFileKind.ManualText, ".txt")]
        [InlineData(ProgramFileKind.SaveData, ".jlp")]
        [InlineData(ProgramFileKind.CfgFile, ".cfg")]
        [InlineData(ProgramFileKind.LuigiFile, ".luigi")]
        [InlineData(ProgramFileKind.Vignette, "")]
        [InlineData(ProgramFileKind.GenericSupportFile, "")]
        public void ProgramFileKind_FileExtension_ReturnsCorrectFileExtension(ProgramFileKind fileKind, string expectedFileExtension)
        {
            Assert.Equal(expectedFileExtension, fileKind.FileExtension());
        }

        [Theory]
        [InlineData(ProgramFileKind.NumFileKinds)]
        [InlineData((ProgramFileKind)(-1))]
        public void ProgramFileKind_FileExtensionForUnsupportedFileKind_ThrowsKeyNotFoundException(ProgramFileKind fileKind)
        {
            Assert.Throws<KeyNotFoundException>(() => fileKind.FileExtension());
        }

        public static IEnumerable<object[]> HasCorrectFileExtensionTestData
        {
            get
            {
                var kindsWithEmptyFileExtension = new[] { ProgramFileKind.None, ProgramFileKind.Vignette, ProgramFileKind.GenericSupportFile };
                foreach (var kindWithEmptyFileExtension in kindsWithEmptyFileExtension)
                {
                    var testPaths = GenerateTestFilePaths(string.Empty).ToList();
                    var failingData = new List<string>(testPaths.Take(4)).Concat(testPaths.Skip(6).Take(3)).ToList();
                    foreach (var testFilePath in failingData)
                    {
                        yield return new object[] { kindWithEmptyFileExtension, testFilePath, false };
                    }
                    var passingData = new List<string>(testPaths.Skip(4).Take(2)).Concat(testPaths.Skip(9)).ToList();
                    foreach (var testFilePath in passingData)
                    {
                        yield return new object[] { kindWithEmptyFileExtension, testFilePath, true };
                    }
                }

                var numberOfInvalidTestPathsAtStart = 7;
                var romFileExtensions = new[] { ".rom", ".cc3", ".bin", ".itv", ".int", ".luigi" };
                foreach (var romFileExtension in romFileExtensions)
                {
                    var testPaths = GenerateTestFilePaths(romFileExtension);
                    foreach (var testFilePath in testPaths.Take(numberOfInvalidTestPathsAtStart))
                    {
                        yield return new object[] { ProgramFileKind.Rom, testFilePath, false };
                    }
                    foreach (var testFilePath in testPaths.Skip(numberOfInvalidTestPathsAtStart))
                    {
                        yield return new object[] { ProgramFileKind.Rom, testFilePath, true };
                    }
                }

                var kindsUsingGraphicsExtensions = new[] { ProgramFileKind.Box, ProgramFileKind.Label, ProgramFileKind.Overlay, ProgramFileKind.ManualCover };
                var graphicsFileExtensions = new[] { ".jpg", ".bmp", ".png", ".gif" };
                foreach (var kindUinsgGraphicsExtensions in kindsUsingGraphicsExtensions)
                {
                    foreach (var graphicsFileExtension in graphicsFileExtensions)
                    {
                        var testPaths = GenerateTestFilePaths(graphicsFileExtension);
                        foreach (var testFilePath in testPaths.Take(numberOfInvalidTestPathsAtStart))
                        {
                            yield return new object[] { kindUinsgGraphicsExtensions, testFilePath, false };
                        }
                        foreach (var testFilePath in testPaths.Skip(numberOfInvalidTestPathsAtStart))
                        {
                            yield return new object[] { kindUinsgGraphicsExtensions, testFilePath, true };
                        }
                    }
                }

                var kindsWithOneExtension = new[] { ProgramFileKind.ManualText, ProgramFileKind.SaveData, ProgramFileKind.CfgFile, ProgramFileKind.LuigiFile };
                var extensionsForKindsWithOneExtension = new[] { ".txt", ".jlp", ".cfg", ".luigi" };
                foreach (var kindWithExtensionToTest in kindsWithOneExtension.Zip(extensionsForKindsWithOneExtension, (k, e) => new { Kind = k, Extension = e }))
                {
                    var testPaths = GenerateTestFilePaths(kindWithExtensionToTest.Extension);
                    foreach (var testFilePath in testPaths.Take(7))
                    {
                        yield return new object[] { kindWithExtensionToTest.Kind, testFilePath, false };
                    }
                    foreach (var testFilePath in testPaths.Skip(7))
                    {
                        yield return new object[] { kindWithExtensionToTest.Kind, testFilePath, true };
                    }
                }
            }
        }

        private static IEnumerable<string> GenerateTestFilePaths(string extension)
        {
            yield return null;
            yield return ".";
            yield return @"I:/ntelli/vision/files/test" + extension + ".goof-upper";
            yield return @"I:/ntelli/vision/files/test" + extension.ToUpperInvariant() + ".goof-upper";
            yield return @"I:/ntelli/vision/files/test" + extension + "messer-upper";
            yield return @"I:/ntelli/vision/files/test" + extension.ToUpperInvariant() + "messer-upper";
            yield return string.Empty;
            yield return extension;
            yield return extension.ToUpperInvariant();
            yield return @"I:/ntelli/vision/files/test" + extension;
            yield return @"I:/ntelli/vision/files/test" + extension.ToUpperInvariant();
        }

        [Theory]
        [MemberData("HasCorrectFileExtensionTestData")]
        public void ProgramFileKind_HasCorrectExtension_ReturnsCorrectFileExtensions(ProgramFileKind fileKind, string filePath, bool expectedHasCorrectExtension)
        {
            Assert.Equal(expectedHasCorrectExtension, fileKind.HasCorrectExtension(filePath));
        }

        [Theory]
        [InlineData(ProgramFileKind.NumFileKinds)]
        [InlineData((ProgramFileKind)(-1))]
        public void ProgramFileKind_HasCorrectExtensionForUnsupportedFileKind_ThrowsKeyNotFoundException(ProgramFileKind fileKind)
        {
            Assert.Throws<KeyNotFoundException>(() => fileKind.HasCorrectExtension(@"I:\ntellivision\Rules.forever"));
        }

        [Theory]
        [MemberData("AllFileKinds")]
        public void ProgramFileKind_HasCustomRomExtensionForNullPath_ReturnsFalse(ProgramFileKind fileKind)
        {
            Assert.False(fileKind.HasCustomRomExtension(null));
        }

        [Theory]
        [MemberData("AllFileKinds")]
        public void ProgramFileKind_HasCustomRomExtensionForEmptyPath_ReturnsFalseExceptForRomKind(ProgramFileKind fileKind)
        {
            var expectedResult = fileKind == ProgramFileKind.Rom;
            Assert.Equal(expectedResult, fileKind.HasCustomRomExtension(string.Empty));
        }

        [Theory]
        [InlineData("NoExtension", true)] // we support empty file extension due to Mac versions of Intellivision Lives / Rocks
        [InlineData("Pathological.", false)]
        [InlineData(".", false)]
        [InlineData("", true)]
        [InlineData(null, false)]
        [InlineData("goofy.bin", false)]
        [InlineData("goofy.ROM", false)]
        [InlineData("goofy.cc3", false)]
        [InlineData("goofy.itv", false)]
        [InlineData("goofy.int", false)]
        [InlineData("goofy.luigi", false)]
        [InlineData("goofy.rhombus", false)]
        public void ProgramFileKind_HasCustomRomExtensionForRomAndNoCustomExtensionsRegistered_ReturnsProperResult(string pathToCheck, bool expectedResult)
        {
            Assert.Equal(expectedResult, ProgramFileKind.Rom.HasCustomRomExtension(pathToCheck));
        }

        [Theory]
        [InlineData("game.bin", false)]
        [InlineData("game.game", true)]
        public void ProgramFileKind_HasCustomRomExtensionForRomWithCustomExtensionRegistered_ReturnsProperResult(string pathToCheck, bool expectedResult)
        {
            var customExtension = ".game";
            try
            {
                Assert.False(ProgramFileKind.Rom.HasCustomRomExtension(pathToCheck));
                ProgramFileKind.Rom.AddCustomExtension(customExtension);
                Assert.Equal(expectedResult, ProgramFileKind.Rom.HasCustomRomExtension(pathToCheck));
            }
            finally
            {
                ProgramFileKind.Rom.RemoveCustomExtension(customExtension);
            }
        }

        [Fact]
        public void ProgramFileKind_RemoveCustomExtensionForEmptyRomFileExtension_HasNoEffect()
        {
            var testPath = "Astrosmash";
            Assert.True(ProgramFileKind.Rom.HasCustomRomExtension(testPath));

            ProgramFileKind.Rom.RemoveCustomExtension(string.Empty);

            Assert.True(ProgramFileKind.Rom.HasCustomRomExtension(testPath));
        }

        [Fact]
        public void ProgramFileKind_AddCustomExtensionForNonRomKind_HasNoEffect()
        {
            var testPath = "image.mng";
            Assert.False(ProgramFileKind.Overlay.HasCustomRomExtension(testPath));

            ProgramFileKind.Overlay.AddCustomExtension(".mng");

            Assert.False(ProgramFileKind.Overlay.HasCustomRomExtension(testPath));
        }

        [Fact]
        public void ProgramFileKind_RemoveCustomExtensionForEmptyManualCoverFileExtension_HasNoEffect()
        {
            var testPath = "image.jpg";
            Assert.False(ProgramFileKind.ManualCover.HasCustomRomExtension(testPath));

            ProgramFileKind.ManualCover.RemoveCustomExtension(string.Empty);

            Assert.False(ProgramFileKind.ManualCover.HasCustomRomExtension(testPath));
        }

        [Fact]
        public void ProgramFileKind_RemoveCustomExtensionForNonEmptyManualCoverFileExtension_HasNoEffect()
        {
            var testPath = "image.jpg";
            Assert.False(ProgramFileKind.ManualCover.HasCustomRomExtension(testPath));

            ProgramFileKind.ManualCover.RemoveCustomExtension(".ps");

            Assert.False(ProgramFileKind.ManualCover.HasCustomRomExtension(testPath));
        }

        private static void AssertCollectionsAreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.Empty(expected.Except(actual));
            Assert.Empty(actual.Except(expected));
        }
    }
}

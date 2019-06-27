// <copyright file="PathUtilsTests.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class PathUtilsTests
    {
        [Fact]
        public void PathUtils_FixUpUriPathWithNullPath_ThrowsNullReferenceException()
        {
            Uri path = null;

            Assert.Throws<NullReferenceException>(() => path.FixUpUriPath());
        }

        [Theory]
        [InlineData("file:///../", @"\")]
        [InlineData("file:///", @"\")]
        [InlineData(@"file:///h\e\l\p", @"\h\e\l\p")]
        [InlineData(@"file:///h:\e\l\p", @"h:\e\l\p")]
        [InlineData("file:///a/./b/../c/d/", @"\a\c\d\")]
        public void PathUtils_FixUpUriPath_ReturnsExpectedPathString(string uriString, string expectedPathString)
        {
            var pathUri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            var pathString = pathUri.FixUpUriPath();

            Assert.Equal(expectedPathString, pathString);
        }

        [Theory]
        [InlineData(null, @"don't care", @"")]
        [InlineData(@"", @"don't care", @"")]
        [InlineData(@"c", null, @"c")]
        [InlineData(@"c", @"", @"c")]
        [InlineData(@"c:\testing", @"z:\xyz", @"c:\testing")]
        [InlineData(@"c:\testing", @"C:\testing", @"..\testing")]
        [InlineData(@"c:\a\b\c\d\", @"C:\a\b", @"c\d\")]
        [InlineData(@"c:\a\b/c\d\e/", @"C:\a\b\e\f", @"..\..\c\d\e\")]
        [InlineData(@"z:/a\b\c", @"z:\", @"a\b\c")]
        [InlineData(@"z:\a\b\c", @"z:/x/", @"..\a\b\c")]
        public void PathUtils_GetRelativePath_ReturnsExpectedRelativePath(string file, string relativeTo, string expectedRelativePath)
        {
            var relativePath = PathUtils.GetRelativePath(file, relativeTo);

            Assert.Equal(expectedRelativePath, relativePath);
        }

        public static IEnumerable<object[]> GetCommonPathTestData
        {
            get
            {
                yield return new object[] { new[] { @"aaa" }, @"aaa" };
                yield return new object[] { new[] { @"c:\a", @"d:\a" }, string.Empty };
                yield return new object[] { new[] { @"C:\a", @"c:\a\b\" }, @"c:\a" };
                yield return new object[] { new[] { @"d:/foo/bar\baz\buzz", @"D:\foo\bar\", @"d:\fOo\bar\baz\gooogoogah gah" }, @"d:\foo\bar" };
                yield return new object[] { new[] { @"a:\first", @"a:\first\second", @"c:\boo!" }, string.Empty };
            }
        }

        [Fact]
        public void PathUtils_GetCommonPathWithEmptyPathsCollection_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => PathUtils.GetCommonPath(Enumerable.Empty<string>()));
        }

        [Theory]
        [MemberData("GetCommonPathTestData")]
        public void PathUtils_GetCommonPath_ReturnsExpectedCommonPath(IEnumerable<string> paths, string expectedCommonPath)
        {
            var commonPath = PathUtils.GetCommonPath(paths);

            Assert.Equal(expectedCommonPath.ToUpperInvariant(), commonPath.ToUpperInvariant());
        }

        [Theory]
        [InlineData("data", false)]
        [InlineData("data.bak", true)]
        [InlineData("CLOCK$", false)]
        [InlineData(@"b\", false)]
        public void PathUtils_GetUniqueBackupFilePath_ReturnsFileThatDoesNotExist(string fileName, bool forceCollision)
        {
            using (var tempDir = new TemporaryDirectory())
            {
                var filePath = Path.Combine(tempDir.Path, fileName);
                if (filePath.Last() != '\\')
                {
                    File.Create(filePath).Dispose();
                    if (forceCollision)
                    {
                        var collidingFilePath = Path.Combine(tempDir.Path, fileName + ".bak");
                        File.Create(collidingFilePath).Dispose();
                    }
                }

                var uniqueName = filePath.GetUniqueBackupFilePath();

                Assert.False(File.Exists(uniqueName));
                Assert.Equal(PathUtils.BackupSuffix, Path.GetExtension(uniqueName));
                Assert.NotEqual(filePath, uniqueName);
            }
        }

        [Theory]
        [InlineData("data")]
        [InlineData("data.bak")]
        [InlineData("CLOCK$")]
        [InlineData(@"b\")]
        public void PathUtils_EnsureUniqueFileName_ReturnsFileThatDoesNotExist(string fileName)
        {
            using (var tempDir = new TemporaryDirectory())
            {
                var filePath = Path.Combine(tempDir.Path, fileName);
                if (filePath.Last() != '\\')
                {
                    File.Create(filePath).Dispose();
                }

                var uniqueName = filePath.EnsureUniqueFileName();

                Assert.False(File.Exists(uniqueName));
                Assert.NotEqual(filePath, uniqueName);
            }
        }

        [Theory]
        [InlineData("COM1", "COM1_")]
        [InlineData("CO>M1", "CO_M1")]
        public void PathUtils_EnsureUniqueFileNameWithInvalidName_ReturnsValidName(string fileName, string expectedFileName)
        {
            var validatedFileName = fileName.EnsureValidFileName();

            Assert.Equal(expectedFileName, validatedFileName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void PathUtils_GetTimeString_ReturnsTimeString(bool isUtc)
        {
            var timeString = PathUtils.GetTimeString(isUtc);

            var dateTime = PathUtils.GetDateTimeFromString(timeString);

            Assert.NotEqual(DateTime.MinValue, dateTime);
        }

        [Theory] // strings are of form YYYY-MM-DD-HH-MM-SS-mmm
        [InlineData("")]
        [InlineData("ABCD-09-08-13-45-32-654")]
        [InlineData("2000-w#-08-13-45-32-654")]
        [InlineData("2000-09-=+-13-45-32-654")]
        [InlineData("2000-09-08-[]-45-32-654")]
        [InlineData("2000-09-08-13-.-32-654")]
        [InlineData("2000-09-08-13-45- -654")]
        [InlineData("2000-09-08-13-45-32-^")]
        public void PathUtils_GetTimeStringFromInvalidString_ReturnsDateTimeMin(string timeString)
        {
            var dateTime = PathUtils.GetDateTimeFromString(timeString);

            Assert.Equal(DateTime.MinValue, dateTime);
        }

        [Fact]
        public void PathUtils_IsPathInSupportedArchiveWithValidPath_ReturnsTrue()
        {
            var testArchive = TestResource.TagalongBCLRNNYNGZip;
            var testArchivePath = string.Empty;

            using (var zip = testArchive.ExtractToTemporaryFile(out testArchivePath))
            {
                var pathsToTest = new[]
                    {
                        testArchivePath,
                        Path.Combine(testArchivePath, "dir/"),
                        Path.Combine(testArchivePath, "dir", "sub.zip"),
                        Path.Combine(testArchivePath, "dir", "sub.zip", "file.txt"),
                    };
                Assert.All(pathsToTest, p => p.IsPathInSupportedArchive());
            }
        }

        [Fact]
        public void PathUtils_IsPathInSupportedArchiveWithBogusPath_ReturnsFalse()
        {
            var bogusPath = @"X:\bogus\path\to\a thing that shall not be\a.zip\not here\readme.txt";

            var isInArchive = bogusPath.IsPathInSupportedArchive();

            Assert.False(isInArchive);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PathUtils_IsPathInSupportedArchiveWithNullOrEmptyPath_ReturnsFalse(string path)
        {
            var isInArchive = path.IsPathInSupportedArchive();

            Assert.False(isInArchive);
        }

        [Fact]
        public void PathUtils_IsPathInSupportedArchiveToExistingFile_ReturnsFalse()
        {
            using (var tempDir = new TemporaryDirectory())
            {
                var path = Path.Combine(tempDir.Path, "JustADirectory.zip");
                Assert.True(Directory.CreateDirectory(path).Exists);
                path = Path.Combine(path, "readme.txt");
                File.Create(path).Dispose();
                Assert.True(File.Exists(path));

                var isInArchive = path.IsPathInSupportedArchive();

                Assert.False(isInArchive);
            }
        }

        [Fact]
        public void PathUtils_IsPathInSupportedArchiveToDirectoryPath_ReturnsFalse()
        {
            using (var tempDir = new TemporaryDirectory())
            {
                var path = Path.Combine(tempDir.Path, "NotAValid.zip");
                Assert.True(Directory.CreateDirectory(path).Exists);
                path = Path.Combine(path, "goober.txt");

                var isInArchive = path.IsPathInSupportedArchive();

                Assert.False(isInArchive);
            }
        }

        #region OS-specific tests

        [Fact]
        public void PathUtils_GetDocumentsDirectory_ReturnsExpectedDirectory()
        {
            var documentsDirectory = PathUtils.GetDocumentsDirectory();

            // totally OS-specific...
            var expectedDocumentsDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            Assert.Equal(expectedDocumentsDirectory, documentsDirectory);
        }

        [Fact]
        public void PathUtils_IsPathOnRemovableDeviceUnRootedPath_ThrowsArgumentException()
        {
            var unrootedPath = "a/b/c";

            Assert.Throws<ArgumentException>(() => unrootedPath.IsPathOnRemovableDevice());
        }

        [Fact]
        public void PathUtils_IsPathOnRemovableDeviceUncPath_ThrowsArgumentException()
        {
            var path = @"\\vectronically\shared\documents\readem.txt";

            Assert.Throws<ArgumentException>(() => path.IsPathOnRemovableDevice());
        }

        [Fact]
        public void PathUtils_IsPathOnRemovableDeviceUsingUnknownVolume_ReturnsTrue()
        {
            var potentialWindowsDiskVolumeNames = Enumerable.Range('A', 26).Reverse().Select(x => (char)x + @":\");
            var drives = DriveInfo.GetDrives().Select(d => d.Name);
            var path = potentialWindowsDiskVolumeNames.Except(drives).First();

            var isOnRemovableDevice = path.IsPathOnRemovableDevice();

            Assert.True(isOnRemovableDevice);
        }

        public static IEnumerable<object[]> SpecialDiskLocationsTestData
        {
            get
            {
                yield return new object[] { Path.GetTempPath() };
                yield return new object[] { NativeMethods.GetDownloadsPath() };
                yield return new object[] { Environment.GetFolderPath(System.Environment.SpecialFolder.History) };
                yield return new object[] { Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache) };
                yield return new object[] { Environment.GetFolderPath(System.Environment.SpecialFolder.CDBurning) };
            }
        }

        [Theory]
        [MemberData("SpecialDiskLocationsTestData")]
        public void PathUtils_IsPathOnRemovableDeviceUsingTempDir_ReturnsTrue(string specialPath)
        {
            var isOnRemovableDevice = specialPath.IsPathOnRemovableDevice();

            Assert.True(isOnRemovableDevice);
        }

        [Fact]
        public void PathUtils_RevealPathInFileSystem_Succeeds()
        {
            var path = Path.GetTempPath();

            path.RevealInFileSystem(); // not crashing == pass
        }

        [Fact]
        public void PathUtils_RevealPathsInFileSystem_Succeeds()
        {
            using (var rootTempDir = new TemporaryDirectory())
            {
                var subdirectoryPath = Path.Combine(rootTempDir.Path, "subdir");
                var missingSubdirectoryPath = Path.Combine(rootTempDir.Path, "missingSubdir");
                var paths = new List<string>() { subdirectoryPath, missingSubdirectoryPath };
                Directory.CreateDirectory(subdirectoryPath);
                for (var i = 0; i < 5; ++i)
                {
                    var path = Path.Combine(rootTempDir.Path, "file_" + i + ".txt");
                    File.Create(path).Dispose();
                    if ((i % 2) == 0)
                    {
                        paths.Add(path);
                    }
                }

                paths.RevealInFileSystem(); // not crashing == pass
            }
        }

        [Theory]
        [InlineData(@"", @"")] // Windows-specific behavior
        [InlineData(@"x:\r", @"x:\r")] // Windows-specific behavior
        [InlineData(@"/usr/local/settings/why lookie there", @"/usr/local/settings/why lookie there")] // Windows-specific behavior
        [InlineData(@"file:///c/docs/vint/settings.txt", @"file:///c/docs/vint/settings.txt")] // Windows-specific behavior
        public void PathUtils_ResolvePathForSettings(string path, string expectedPath)
        {
            var resolvedPath = PathUtils.ResolvePathForSettings(path);

            Assert.Equal(expectedPath, resolvedPath);
        }

        #endregion OS-specific tests
    }
}

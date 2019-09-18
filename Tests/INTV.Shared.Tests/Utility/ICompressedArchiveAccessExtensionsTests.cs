// <copyright file="ICompressedArchiveAccessExtensionsTests.cs" company="INTV Funhouse">
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
    public class ICompressedArchiveAccessExtensionsTests
    {
        // NOTE: This must be manually updated when new archive formats are added!
        private static readonly string[] SupportedCompressedArchiveFileExtensions = new[] { ".zip", ".tar", ".gz", ".gzip" };

        private static readonly IDictionary<string, CompressedArchiveFormat> FileExtensionToCompressedArchiveFormatMap = new Dictionary<string, CompressedArchiveFormat>()
        {
            { ".zip", CompressedArchiveFormat.Zip },
            { ".tar", CompressedArchiveFormat.Tar },
            { ".gz", CompressedArchiveFormat.GZip },
            { ".gzip", CompressedArchiveFormat.GZip },
        };

        [Fact]
        public void ICompressedArchiveAccess_GetStorageAccessWithNullPath_ReturnsDefaultStorageAccess()
        {
            string path = null;
            Assert.Null(path.GetStorageAccess());
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("////////////")]
        [InlineData(@"C:\;lskdjf;alswkfj;lek;lekjfl;sakejfsa")] // hope nobody has that path!
        public void ICompressedArchiveAccess_GetStorageAccessWithNonexistingNonArchivePath_ReturnsDefaultStorageAccess(string path)
        {
            Assert.Null(path.GetStorageAccess());
        }

        public static IEnumerable<object[]> CompressedArchiveTestResources
        {
            get
            {
                foreach (var compressedArchiveTestResource in TestResource.GetAvailableTestResources(IsCompressedArchiveTestResource))
                {
                    yield return new object[] { compressedArchiveTestResource };
                }
            }
        }

        [Theory]
        [MemberData("CompressedArchiveTestResources")]
        public void ICompressedArchiveAccess_GetStorageAccessFromNonExistentCompressedArchivePath_ThrowsIOException(TestResource compressedArchiveTestResource)
        {
            var invalidDirectoryPathToResource = "this/will/not/be/found/" + compressedArchiveTestResource.Name;
            var invalidFilePathToResource = Path.GetTempPath() + compressedArchiveTestResource.Name;

            Assert.Throws<DirectoryNotFoundException>(() => invalidDirectoryPathToResource.GetStorageAccess());
            Assert.Throws<FileNotFoundException>(() => invalidFilePathToResource.GetStorageAccess());
        }

        public static IEnumerable<object[]> UniqueCompressedArchiveFormatTestResources
        {
            get
            {
                var yieldedArchiveFormats = new List<string>();
                var compressedArchiveTestResources = TestResource.GetAvailableTestResources(IsCompressedArchiveTestResource);
                foreach (var compressedArchiveTestResource in compressedArchiveTestResources)
                {
                    var formatFileExtension = SupportedCompressedArchiveFileExtensions.FirstOrDefault(extension => compressedArchiveTestResource.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
                    if (!yieldedArchiveFormats.Contains(formatFileExtension))
                    {
                        yieldedArchiveFormats.Add(formatFileExtension);
                        var format = FileExtensionToCompressedArchiveFormatMap[formatFileExtension];
                        yield return new object[] { compressedArchiveTestResource, format };
                    }
                }
            }
        }

        [Theory]
        [MemberData("UniqueCompressedArchiveFormatTestResources")]
        public void ICompressedArchiveAccess_GetStorageAccessFromKnownArchiveKind_ReturnsExpectedArchiveKind(TestResource testResource, CompressedArchiveFormat expectedCompressedStorageAccessFormat)
        {
            string archiveFilePath;
            using (testResource.ExtractToTemporaryFile(out archiveFilePath))
            {
                    var storageAccess = archiveFilePath.GetStorageAccess();
                    var compressedArchiveStorageAccess = storageAccess as ICompressedArchiveAccess;

                    Assert.NotNull(storageAccess);
                    Assert.NotNull(compressedArchiveStorageAccess);
                    Assert.Equal(expectedCompressedStorageAccessFormat, compressedArchiveStorageAccess.Format);
                    compressedArchiveStorageAccess.Dispose();
            }
        }

        public static IEnumerable<object[]> GetRootArchivePathIsNotToArchiveTestData
        {
            get
            {
                yield return new object[] { null, false, false };
                yield return new object[] { "", false, false };
                yield return new object[] { Path.Combine(Path.GetTempPath(), "file.txt"), false, false };
                yield return new object[] { Path.Combine(Path.GetTempPath(), "not here.zip"), false, false };
                yield return new object[] { ".txt", false, true };
                yield return new object[] { @"subdir/tricky.tar/games/intellivision/game.rom", true, false };
                yield return new object[] { @"subdir/tricky.tar/games/intellivision/game.rom", true, true };
            }
        }

        [Theory]
        [MemberData("GetRootArchivePathIsNotToArchiveTestData")]
        public void ICompressedArchiveAccess_GetRootArchivePathUsingPathNotToOrWithinArchive_ReturnsNull(string path, bool useTempDir, bool useTempFile)
        {
            using (var tempDirectory = new TemporaryDirectory())
            using (var temporaryFile = new TemporaryFile(".txt", useTempFile))
            {
                if (useTempDir)
                {
                    path = Path.Combine(tempDirectory.Path, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    if (useTempFile)
                    {
                        File.Create(path);
                    }
                }
                else if (useTempFile)
                {
                    path = temporaryFile.FilePath;
                }

                Assert.Null(path.GetRootArchivePath());
            }
        }

        public static IEnumerable<object[]> GetStorageAccessToNestedArchivePathTestData
        {
            get
            {
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, null, CompressedArchiveFormat.Zip, false };
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, "tagalong_dir/tagalong.luigi", CompressedArchiveFormat.Zip, false };
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, "tagalong_dir/", CompressedArchiveFormat.Zip, true };
                yield return new object[] { TestResource.TagalongNestedZip, CompressedArchiveFormat.Zip, false, "tagalong.zip", CompressedArchiveFormat.Zip, true };
                yield return new object[] { TestResource.TagalongBinGZip, CompressedArchiveFormat.GZip, false, null, CompressedArchiveFormat.GZip, false };
                yield return new object[] { TestResource.TagalongBinGZip, CompressedArchiveFormat.GZip, false, "tagalong.bin", CompressedArchiveFormat.GZip, false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, null, CompressedArchiveFormat.Tar, true };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, "tagalong_dir/tagalong.rom", CompressedArchiveFormat.Tar, false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, "tagalong_dir/", CompressedArchiveFormat.Tar, true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, null, CompressedArchiveFormat.GZip, false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/bin/tagalong.bin", CompressedArchiveFormat.Tar, false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/bin/", CompressedArchiveFormat.Tar, true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, false, "tagalong_msys2.tar", CompressedArchiveFormat.Tar, true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/tagalong.zip/tagalong.cfg", CompressedArchiveFormat.Zip, false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/tagalong.zip", CompressedArchiveFormat.Zip, true };
            }
        }

        [Theory]
        [MemberData("GetStorageAccessToNestedArchivePathTestData")]
        public void ICompressedArchiveAccess_GetRootArchivePathOfPathToOrWithinArchive(TestResource testResource, CompressedArchiveFormat format, bool isNestedArchivePath, string path, CompressedArchiveFormat nestedArchiveFormat, bool isPathToContainer)
        {
            string archiveFilePath;
            using (testResource.ExtractToTemporaryFile(out archiveFilePath))
            {
                var nestedPath = string.IsNullOrEmpty(path) ? archiveFilePath : Path.Combine(archiveFilePath, path);

                var rootArchivePath = nestedPath.GetRootArchivePath();

                Assert.Equal(0, PathComparer.Instance.Compare(archiveFilePath.NormalizePathSeparators(), rootArchivePath.NormalizePathSeparators()));
            }
        }

        [Fact]
        public void ICompressedArchiveAccess_ListWithNullArchive_ThrowsArgumentNullException()
        {
            ICompressedArchiveAccess archive = null;

            Assert.Throws<ArgumentNullException>(() => archive.ListEntries(null, includeContainers: false));
            Assert.Throws<ArgumentNullException>(() => archive.ListContents(null, includeContainers: false));
        }

        [Fact]
        public void ICompressedArchiveAccess_ListWithMalformedLocationInContainer_ThrowsArgumentException()
        {
            using (var zip = CompressedArchiveAccess.Open(TestResource.TagalongZip.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                Assert.Throws<ArgumentException>(() => zip.ListEntries("where's the beef!", includeContainers: true));
                Assert.Throws<ArgumentException>(() => zip.ListContents("did you check under the dash?", includeContainers: true));
            }
        }

        [Theory]
        [InlineData("Hobgoblins abound/")]
        [InlineData(@"nothing/to\see\here\")]
        public void ICompressedArchiveAccess_ListItemsWithNonexistentLocation_ReturnsEmpty(string location)
        {
            using (var zip = CompressedArchiveAccess.Open(TestResource.TagalongZip.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries0 = zip.ListEntries(location, includeContainers: false);
                var entries1 = zip.ListContents(location, includeContainers: false);

                Assert.Empty(entries0);
                Assert.Empty(entries1);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(".")]
        [InlineData("/")]
        [InlineData(@"\")]
        public void ICompressedArchiveAccess_ListItemsAtRoot_ReturnsExpectedItems(string root)
        {
            var testZipResource = TestResource.TagalongZip;

            using (var zip = CompressedArchiveAccess.Open(testZipResource.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries0 = zip.ListEntries(root, includeContainers: false);
                var entries1 = zip.ListContents(root, includeContainers: false);

                var expectedEntryNames = testZipResource.ArchiveContents.ToList();
                Assert.Equal(expectedEntryNames.Count, entries0.Count());
                Assert.Equal(expectedEntryNames.Count, entries1.Count());
                Assert.Equal(expectedEntryNames, entries0.Select(e => e.Name));
                Assert.Equal(expectedEntryNames, entries1);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ICompressedArchive_ListInArchiveContainingItemsInSubdirectory_FiltersAsExpected(bool includeContainers)
        {
            var zipContainingOneSubdirectory = TestResource.TagalongDirZip;

            using (var zip = CompressedArchiveAccess.Open(zipContainingOneSubdirectory.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(null, includeContainers);

                if (includeContainers)
                {
                    Assert.NotEmpty(entries);
                }
                else
                {
                    Assert.Empty(entries);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ICompressedArchive_ListInZipArchiveContainingNestedZipArchive_FiltersAsExpected(bool includeContainers)
        {
            var zipContainingOneNestedZip = TestResource.TagalongNestedZip;

            using (var zip = CompressedArchiveAccess.Open(zipContainingOneNestedZip.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(null, includeContainers);

                if (includeContainers)
                {
                    Assert.NotEmpty(entries);
                }
                else
                {
                    Assert.Empty(entries);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ICompressedArchive_ListInGZipArchiveContainingNestedTarArchive_FiltersAsExpected(bool includeContainers)
        {
            var tgzResource = TestResource.TagalongMsys2Tgz;

            string tgzFile; // necessary to get archive being identified via file extension
            using (tgzResource.ExtractToTemporaryFile(out tgzFile))
            {
                using (var tgz = CompressedArchiveAccess.Open(tgzFile, CompressedArchiveAccessMode.Read))
                {
                    var entries = tgz.ListEntries(null, includeContainers);

                    if (includeContainers)
                    {
                        Assert.NotEmpty(entries);
                    }
                    else
                    {
                        Assert.Empty(entries);
                    }
                }
            }
        }

        [Fact]
        public void ICompressedArchive_ListInLocationNoRecursion_ReturnsExpectedItems()
        {
            var zipContainingOneSubdirectory = TestResource.TagalongDirZip;
            var location = "tagalong_dir/";

            using (var zip = CompressedArchiveAccess.Open(zipContainingOneSubdirectory.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, includeContainers: false);

                var expectedEntries = zipContainingOneSubdirectory.ArchiveContents.Skip(1);
                Assert.Equal(expectedEntries, entries.Select(e => e.Name));
            }
        }

        [Fact]
        public void ICompressedArchive_ListInNestedArchiveLocationNoRecursion_ReturnsExpectedItems()
        {
            var zipContainingOneNestedZip = TestResource.TagalongNestedZip;
            var location = @"tagalong.zip\";

            using (var zip = CompressedArchiveAccess.Open(zipContainingOneNestedZip.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, false);

                var expectedEntries = new[] { "tagalong.zip/tagalong.bin", "tagalong.zip/tagalong.cfg" };
                Assert.Equal(expectedEntries, entries.Select(e => e.Name));
            }
        }

        [Fact]
        public void ICompressedArchive_ListInGZipArchiveContainingNestedTarArchiveFolderLocationNoRecursion_ReturnsExpectedItems()
        {
            var tgzResource = TestResource.TagalongMsys2Tgz;
            var location = "tagalong_msys2.tar/bin/";

            string tgzFile;
            using (tgzResource.ExtractToTemporaryFile(out tgzFile))
            {
                using (var tgz = CompressedArchiveAccess.Open(tgzFile, CompressedArchiveAccessMode.Read))
                {
                    var entries = tgz.ListEntries(location, includeContainers: true);

                    var expectedEntries = new[] { "tagalong_msys2.tar/bin/tagalong.bin", "tagalong_msys2.tar/bin/tagalong.cfg" };
                    Assert.Equal(expectedEntries, entries.Select(e => e.Name));
                }
            }
        }

        [Fact]
        public void ICompressedArchive_ListInDeeplyNestedArchiveLocationNoRecursion_ReturnsExpectedItems()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var location = @"extra_nest/tagalong_msys2.tgz\tagalong_msys2.tar/tagalong.zip\";

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, false);

                var expectedEntries = new[] { "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin", "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg" };
                Assert.Equal(expectedEntries, entries.Select(e => e.Name));
            }
        }

        [Fact]
        public void ICompressedArchive_ListRecursivelyWithoutContainers_ReturnsExpectedItems()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var expectedEntryNames = new[]
            {
                "extra_nest/tagalong_metadata.bin",
                "extra_nest/tagalong_metadata.cfg",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.cfg",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.luigi",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.rom",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg",
                "tagalong_dev0.luigi",
                "tagalong_dev1.luigi",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(locationInArchive: null, includeContainers: false, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListFolderRecursivelyWithoutContainers_ReturnsExpectedItems()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var location = @"extra_nest\";
            var expectedEntryNames = new[]
            {
                "extra_nest/tagalong_metadata.bin",
                "extra_nest/tagalong_metadata.cfg",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.cfg",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.luigi",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.rom",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, includeContainers: false, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListRecursivelyWithContainers_ReturnsExpectedItems()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var expectedEntryNames = new[]
            {
                "extra_nest/",
                "extra_nest/tagalong_metadata.bin",
                "extra_nest/tagalong_metadata.cfg",
                "extra_nest/tagalong_msys2.tgz",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.cfg",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.luigi",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.rom",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg",
                "tagalong_dev0.luigi",
                "tagalong_dev1.luigi",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(locationInArchive: null, includeContainers: true, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListTgzRecursivelyWithContainers_ReturnsExpectedItems()
        {
            var tgzNested = TestResource.TagalongWin10TarGz;
            var tgzFilePath = string.Empty;
            var expectedEntryNames = new[]
            {
                "tagalong_w10.tar",
                "tagalong_w10.tar/meta/",
                "tagalong_w10.tar/meta/tagalong_metadata.bin",
                "tagalong_w10.tar/meta/tagalong_metadata.cfg",
                "tagalong_w10.tar/meta/tagalong_metadata.luigi",
                "tagalong_w10.tar/meta/tagalong_metadata.rom",
                "tagalong_w10.tar/scram/",
                "tagalong_w10.tar/scram/tagalong_any.luigi",
                "tagalong_w10.tar/scram/tagalong_dev0.luigi",
                "tagalong_w10.tar/scram/tagalong_dev1.luigi",
                "tagalong_w10.tar/tagalong.bin",
                "tagalong_w10.tar/tagalong.cfg",
                "tagalong_w10.tar/tagalong.rom",
            };

            using (tgzNested.ExtractToTemporaryFile(out tgzFilePath))
            using (var tgz = CompressedArchiveAccess.Open(tgzFilePath, CompressedArchiveAccessMode.Read))
            {
                var entries = tgz.ListEntries(locationInArchive: null, includeContainers: true, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListTgzManyDirectoriesFromRootRecursively_ReturnsExpectedItems()
        {
            var tgzManyDirs = TestResource.TagalongTgzManyDirs;
            var tgzFilePath = string.Empty;

            using (tgzManyDirs.ExtractToTemporaryFile(out tgzFilePath))
            using (var tgz = CompressedArchiveAccess.Open(tgzFilePath, CompressedArchiveAccessMode.Read))
            {
                var entries = tgz.ListEntries(locationInArchive: null, includeContainers: true, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(tgzManyDirs.ArchiveContents, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListTgzManyDirsInSubdirRecursivelyOnlyFiles_ReturnsExpectedItems()
        {
            var tgzManyDirs = TestResource.TagalongTgzManyDirs;
            var tgzFilePath = string.Empty;
            var location = "tagalong_many_dirs.tar/rootSub/sub3/";
            var expectedEntryNames = new[]
            {
                "tagalong_many_dirs.tar/rootSub/sub3/._.DS_Store",
                "tagalong_many_dirs.tar/rootSub/sub3/.DS_Store",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub6/tagalong.bad",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub6/tagalong_metadata_any.luigi",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub7/._.DS_Store",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub7/.DS_Store",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub7/subSubSub0/tagalong_metadata.bin",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub7/subSubSub0/tagalong_metadata.cfg",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub7/tagalong_any.luigi",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub8/tagalong_corrupt.rom",
                "tagalong_many_dirs.tar/rootSub/sub3/subSub8/tagalong_v0.luigi",
                "tagalong_many_dirs.tar/rootSub/sub3/tagalong_from_rom.luigi",
            };

            using (tgzManyDirs.ExtractToTemporaryFile(out tgzFilePath))
            using (var tgz = CompressedArchiveAccess.Open(tgzFilePath, CompressedArchiveAccessMode.Read))
            {
                var entries = tgz.ListEntries(location, includeContainers: false, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListTgzManyDirsInSubdirRecursivelyWithContainers_ReturnsExpectedItems()
        {
            var tgzManyDirs = TestResource.TagalongTgzManyDirs;
            var tgzFilePath = string.Empty;
            var location = "tagalong_many_dirs.tar/rootSub/sub0/";
            var expectedEntryNames = new[]
            {
                "tagalong_many_dirs.tar/rootSub/sub0/._.DS_Store",
                "tagalong_many_dirs.tar/rootSub/sub0/.DS_Store",
                "tagalong_many_dirs.tar/rootSub/sub0/subSub0/",
                "tagalong_many_dirs.tar/rootSub/sub0/subSub0/tagalong_metadata.bin",
                "tagalong_many_dirs.tar/rootSub/sub0/subSub0/tagalong_metadata.cfg",
                "tagalong_many_dirs.tar/rootSub/sub0/subSub0/tagalong_metadata.luigi",
                "tagalong_many_dirs.tar/rootSub/sub0/subSub0/tagalong_metadata.rom",
                "tagalong_many_dirs.tar/rootSub/sub0/subSub1/",
                "tagalong_many_dirs.tar/rootSub/sub0/tagalong_any.luigi",
            };

            using (tgzManyDirs.ExtractToTemporaryFile(out tgzFilePath))
            using (var tgz = CompressedArchiveAccess.Open(tgzFilePath, CompressedArchiveAccessMode.Read))
            {
                var entries = tgz.ListEntries(location, includeContainers: true, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListFolderRecursivelyWithContainers_ReturnsExpectedItems()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var location = @"extra_nest\";
            var expectedEntryNames = new[]
            {
                "extra_nest/tagalong_metadata.bin",
                "extra_nest/tagalong_metadata.cfg",
                "extra_nest/tagalong_msys2.tgz",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.cfg",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.luigi",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.rom",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, includeContainers: true, recurse: true);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void ICompressedArchive_ListDeeplyNestedFolder_ListsCorrectContents(bool includeContainers, bool recurse)
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var location = @"extra_nest\tagalong_msys2.tgz/tagalong_msys2.tar/bin/";
            var expectedEntryNames = new[]
            {
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin/tagalong.cfg",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, includeContainers, recurse);
                var entryNames = entries.Select(e => e.Name);

                Assert.Equal(expectedEntryNames, entryNames);
            }
        }

        [Fact]
        public void ICompressedArchive_ListRootEntriesWithContanersNonRecursively_ReportsCorrectMetadata()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var expectedEntryNames = new[]
            {
                "extra_nest/",
                "tagalong_dev0.luigi",
                "tagalong_dev1.luigi",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(null, includeContainers: true, recurse: false);
                Assert.Equal(expectedEntryNames.Length, entries.Count());

                for (int i = 0; i < expectedEntryNames.Length; ++i)
                {
                    var entry = entries.ElementAt(i);
                    Assert.Equal(expectedEntryNames[i], entry.Name);
                    if (entry.IsDirectory)
                    {
                        Assert.Equal(-1, entry.Length);
                        Assert.Equal(DateTime.MinValue, entry.LastModificationTime);
                    }
                    else
                    {
                        Assert.True(entry.Length > 0);
                        Assert.NotEqual(DateTime.MinValue, entry.LastModificationTime);
                    }
                }
            }
        }

        [Fact]
        public void ICompressedArchive_ListDeeplyNestedEntries_ReportsCorrectMetadata()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var location = @"extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/";
            var expectedEntryNames = new[]
            {
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin",
                "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg",
            };

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zip.ListEntries(location, includeContainers: false, recurse: false);
                Assert.Equal(expectedEntryNames.Length, entries.Count());

                for (int i = 0; i < expectedEntryNames.Length; ++i)
                {
                    var entry = entries.ElementAt(i);
                    Assert.Equal(expectedEntryNames[i], entry.Name);
                    Assert.True(entry.Length > 0);
                    Assert.NotEqual(DateTime.MinValue, entry.LastModificationTime);
                }
            }
        }

        [Fact]
        public void ICompressedArchive_ListNonexistentNestedArchive_ThrowsFileNotFoundException()
        {
            var zipWithMultiNesting = TestResource.TagalongZipWithManyNests;
            var location = @"extra_nest/tagalong_msys2.tgz/tagalong_mystery.tar/tagalong.zip/";

            using (var zip = CompressedArchiveAccess.Open(zipWithMultiNesting.OpenResourceForReading(), CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                Assert.Throws<FileNotFoundException>(() => zip.ListEntries(location, includeContainers: false, recurse: false));
            }
        }

        [Fact]
        public void ICompressedArchive_ListNonexistentDirectoryInNestedArchive_ReturnsEmptyList()
        {
            var gzipWithSubdirectory = TestResource.TagalongWin10TarGz;
            var location = @"tagalong_w10.tar/scrambler/";
            var gzipFile = string.Empty;

            using (gzipWithSubdirectory.ExtractToTemporaryFile(out gzipFile))
            using (var gzip = CompressedArchiveAccess.Open(gzipFile, CompressedArchiveAccessMode.Read))
            {
                var entries = gzip.ListContents(location, includeContainers: true);

                Assert.Empty(entries);
            }
        }

        private static bool IsCompressedArchiveTestResource(TestResource testResource)
        {
            var isCompressedArchiveTestResource = SupportedCompressedArchiveFileExtensions.FirstOrDefault(extension => testResource.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) != null;
            return isCompressedArchiveTestResource;
        }
    }
}

// <copyright file="StorageLocationExtensionsTests.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public sealed class StorageLocationExtensionsTests : ICompressedArchiveTest, IDisposable
    {
        public StorageLocationExtensionsTests()
        {
            Storage = new StorageAccess();
            INTV.Core.Utility.IStorageAccessHelpers.Initialize(Storage);
            TempDir = new TemporaryDirectory();
            TempFile = new TemporaryFile(".test", createFile: true);
            TempPathOnly = new TemporaryFile(".fake", createFile: false);
        }

        #region State

        private StorageAccess Storage { get; set; }

        private TemporaryDirectory TempDir { get; set; }

        private TemporaryFile TempFile { get; set; }

        private TemporaryFile TempPathOnly { get; set; }

        public void Dispose()
        {
            INTV.Core.Utility.IStorageAccessHelpers.Remove(Storage);
            if (TempPathOnly != null)
            {
                var tempPathOnly = TempPathOnly;
                TempPathOnly = null;
                if (tempPathOnly != null)
                {
                    tempPathOnly.Dispose();
                }
            }
            if (TempFile != null)
            {
                var tempFile = TempFile;
                TempFile = null;
                if (tempFile != null)
                {
                    tempFile.Dispose();
                }
            }
            if (TempDir != null)
            {
                var tempDir = TempDir;
                TempDir = null;
                if (tempDir != null)
                {
                    tempDir.Dispose();
                }
            }
        }

        #endregion // State

        #region CreateFromFilePath Tests

        [Fact]
        public void StorageLocationExtensions_CreateFromNullFilePath_HasNullPath()
        {
            string nullPath = null;

            var storageLocation = nullPath.CreateStorageLocationFromPath();

            Assert.Null(storageLocation.Path);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromFilePath_HasDefaultStorageAccess(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.NotNull(storageLocation.StorageAccess);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNullFilePath_IsNullIsTrue()
        {
            string nullPath = null;

            var storageLocation = nullPath.CreateStorageLocationFromPath();

            Assert.True(storageLocation.IsNull);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void StorageLocationExtensions_CreateFromNullOrEmptyFilePath_IsValidIsFalse(string nullOrEmpty)
        {
            var storageLocation = nullOrEmpty.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromFilePath_IsInvalidIsFalse(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsInvalid);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNullFilePath_IsContainerIsFalse()
        {
            string nullPath = null;

            var storageLocation = nullPath.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsContainer);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromNonArchiveFilePath_UsesDefaultStorageIsTrue(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.True(storageLocation.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNullFilePath_LengthCausesArgumentNullException()
        {
            string nullPath = null;

            var storageLocation = nullPath.CreateStorageLocationFromPath();

            Assert.Throws<ArgumentNullException>(() => storageLocation.Length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromNonNullFilePath_HasNonNullPath(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.NotNull(storageLocation.Path);
        }

        [Theory]
        [InlineData("")]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromNonNullFilePath_IsNullIsFalse(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsNull);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromNonEmptyFilePath_IsEmptyIsFalse(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsEmpty);
        }

        [Theory]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromNonNullOrEmptyFilePath_IsValidIsTrue(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.True(storageLocation.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sub/directory/file.txt")]
        [InlineData(@"c:\test\data\")]
        [InlineData(@"c:\test\data\file.txt")]
        [InlineData("/usr/local/")]
        [InlineData("/usr/local/sub/directory/file.txt")]
        public void StorageLocationExtensions_CreateFromAnyFilePath_IsInvalidIsFalse(string path)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsInvalid);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(@"c:\test\data\", true)]
        [InlineData("/usr/local/", true)]
        public void StorageLocationExtensions_CreateFromNullOrDirectoryLikeFilePath_IsContainerIsAsExpected(string path, bool expectedIsContainer)
        {
            var storageLocation = path.CreateStorageLocationFromPath();

            Assert.Equal(expectedIsContainer, storageLocation.IsContainer);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromEmptyFilePath_LengthCausesArgumentException()
        {
            var storageLocation = string.Empty.CreateStorageLocationFromPath();

            Assert.Throws<ArgumentException>(() => storageLocation.Length);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromRelativePath_IsContainer()
        {
            var relativePath = "sub/directory/file.txt";

            var storageLocation = relativePath.CreateStorageLocationFromPath();

            Assert.Equal(false, storageLocation.IsContainer);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromRelativePath_LengthThrows()
        {
            var relativePath = "sub/directory/file.txt";

            var storageLocation = relativePath.CreateStorageLocationFromPath();

            Assert.Throws<FileNotFoundException>(() => storageLocation.Length);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNonexistentAbsolutePath_IsContainerThrows()
        {
            var storageLocation = TempPathOnly.FilePath.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsContainer);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNonexistentAbsoluteFilePath_LengthThrowsFileNotFoundException()
        {
            var storageLocation = TempPathOnly.FilePath.CreateStorageLocationFromPath();

            Assert.Throws<FileNotFoundException>(() => storageLocation.Length);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNonexistentAbsoluteDirectoryPath_LengthThrowsFileNotFoundException()
        {
            var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()) + Path.DirectorySeparatorChar;

            var storageLocation = directory.CreateStorageLocationFromPath();

            Assert.Throws<FileNotFoundException>(() => storageLocation.Length);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteFilePath_IsContainerReturnsFalse()
        {
            var storageLocation = TempFile.FilePath.CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsContainer);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteDirectoryPath_IsContainerReturnsTrue()
        {
            var storageLocation = TempDir.Path.CreateStorageLocationFromPath();

            Assert.True(storageLocation.IsContainer);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteFilePath_LengthReturnsExpectedValue()
        {
            var storageLocation = TempFile.FilePath.CreateStorageLocationFromPath();

            Assert.Equal(0L, storageLocation.Length);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteDirectoryPath_Length()
        {
            var storageLocation = TempDir.Path.CreateStorageLocationFromPath();

            Assert.Throws<FileNotFoundException>(() => storageLocation.Length);
        }

        public static IEnumerable<object[]> CreateStorageLocationFromArchivePathTestData
        {
            get
            {
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, "tagalong_dir/tagalong.luigi", false };
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, "tagalong_dir/", true };
                yield return new object[] { TestResource.TagalongNestedZip, CompressedArchiveFormat.Zip, false, "tagalong.zip", true };
                yield return new object[] { TestResource.TagalongBinGZip, CompressedArchiveFormat.GZip, false, "tagalong.bin", false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, "tagalong_dir/tagalong.rom", false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, "tagalong_dir/", true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/bin/tagalong.bin", false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/bin/", true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, false, "tagalong_msys2.tar", true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/tagalong.zip/tagalong.cfg", false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/tagalong.zip", true };
            }
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromCompressedArchiveNameWithNoPath_UsesDefaultStorageAccess(TestResource testResource, CompressedArchiveFormat format, bool isNestedArchivePath, string path, bool isPathToContainer)
        {
            var storageLocation = testResource.Name.CreateStorageLocationFromPath();

            Assert.True(storageLocation.UsesDefaultStorage);
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathIntoCompressedArchivePath_UsesCompressedArchiveStorageAccess(TestResource testResource, CompressedArchiveFormat format, bool isNestedArchivePath, string path, bool isPathToContainer)
        {
            string archivePath;

            using (testResource.ExtractToTemporaryFile(out archivePath))
            {
                var pathIntoArchive = Path.Combine(archivePath, path);

                var storageLocation = pathIntoArchive.CreateStorageLocationFromPath();

                Assert.True(storageLocation.StorageAccess is ICompressedArchiveAccess);
                ((ICompressedArchiveAccess)storageLocation.StorageAccess).Dispose();
            }
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathToCompressedArchive_UsesCompressedArchiveStorageAccessOfCorrectFormat(TestResource testResource, CompressedArchiveFormat format, bool isNestedArchivePath, string path, bool isPathToContainer)
        {
            string archivePath;

            using (testResource.ExtractToTemporaryFile(out archivePath))
            {
                var storageLocation = archivePath.CreateStorageLocationFromPath();

                var storageAccess = storageLocation.StorageAccess as ICompressedArchiveAccess;
                Assert.Equal(format, storageAccess.Format);
                storageAccess.Dispose();
            }
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathToCompressedArchive_IsContainerIsTrue(TestResource testResource, CompressedArchiveFormat format, bool isNestedArchivePath, string path, bool isPathToContainer)
        {
            string archivePath;

            using (testResource.ExtractToTemporaryFile(out archivePath))
            {
                var storageLocation = archivePath.CreateStorageLocationFromPath();

                Assert.True(storageLocation.IsContainer);
                ((ICompressedArchiveAccess)storageLocation.StorageAccess).Dispose();
            }
        }

        #endregion // CreateFromFilePath Tests

        #region Combine Tests

        [Fact]
        public void StorageLocationExtensions_CombineWithInvalidLocation_ThrowsInvalidOperationException()
        {
            var location = StorageLocation.InvalidLocation;

            Assert.Throws<InvalidOperationException>(() => location.Combine(null));
        }

        public static IEnumerable<object[]> CombineStorageLocationPathTestData
        {
            get
            {
                // Combine with null.
                yield return new object[] { null, null, null, null };
                yield return new object[] { null, null, new string[] { }, null };
                yield return new object[] { null, null, new string[] { string.Empty, string.Empty }, string.Empty };
                yield return new object[] { null, null, new string[] { "a", string.Empty }, "a" };

                // Combine with empty.
                yield return new object[] { null, string.Empty, null, null };
                yield return new object[] { null, string.Empty, new string[] { }, null };
                yield return new object[] { null, string.Empty, new string[] { string.Empty, string.Empty }, string.Empty };
                yield return new object[] { null, string.Empty, new string[] { string.Empty, "x" }, "x" };

                // Combine with relative.
                yield return new object[] { null, "relative", null, "relative" };
                yield return new object[] { null, "relative", new string[] { }, "relative" };
                yield return new object[] { null, "relative", new string[] { string.Empty, string.Empty }, "relative" };
                yield return new object[] { null, "relative", new string[] { "a", string.Empty, "b.zip", "c" }, "relative/a/b.zip/c" };

                // Combine with absolute, nonexistent.
                var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                yield return new object[] { null, nonExistentPath, null, nonExistentPath };
                yield return new object[] { null, nonExistentPath, new string[] { }, nonExistentPath };
                yield return new object[] { null, nonExistentPath, new string[] { string.Empty, string.Empty, string.Empty }, nonExistentPath };
                yield return new object[] { null, nonExistentPath, new string[] { "a", string.Empty, "b.zip", "c" }, Path.Combine(nonExistentPath, "a", string.Empty, "b.zip", "c") };

                // Combine with absolute, existent.
                yield return new object[] { null, "<<::TEMPDIR::>>", null, null };
                yield return new object[] { null, "<<::TEMPDIR::>>", new string[] { }, null };
                yield return new object[] { null, "<<::TEMPDIR::>>", new string[] { string.Empty, string.Empty }, null };
                yield return new object[] { null, "<<::TEMPDIR::>>", new string[] { "a", "b.zip", "c" }, "a/b.zip/c" };

                // Combine with absolute "archive", nonexistent.
                var nonExistentArchivePath = Path.Combine(Path.GetTempPath(), "whistler.zip");
                yield return new object[] { null, nonExistentArchivePath, null, nonExistentArchivePath };
                yield return new object[] { null, nonExistentArchivePath, new string[] { }, nonExistentArchivePath };
                yield return new object[] { null, nonExistentArchivePath, new string[] { string.Empty }, nonExistentArchivePath };
                yield return new object[] { null, nonExistentArchivePath, new string[] { "subdir", "nested.tar" }, Path.Combine(nonExistentArchivePath, "subdir/nested.tar") };

                // Combine with actual archive.
                yield return new object[] { TestResource.TagalongEmptyZip, null, null, null };
                yield return new object[] { TestResource.TagalongEmptyZip, null, new string[] { }, null };
                yield return new object[] { TestResource.TagalongEmptyZip, null, new string[] { string.Empty, string.Empty }, null };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, "tagalong_dir/", new string[] { }, "tagalong_dir/" };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, "tagalong_dir/", new string[] { string.Empty, string.Empty }, "tagalong_dir/" };
                yield return new object[] { TestResource.TagalongZipWithManyNests, "extra_nest/tagalong_msys2.tgz", new string[] { "tagalong_msys2.tar", "tagalong.zip", "tagalong.bin" }, "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin" };
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineNullLocationWithPath_ProducesExpectedPathAndStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            var location = StorageLocation.Null;
            using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
            {
                var newStorageLocation = location.Combine(pathElement, pathElements);

                Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineEmptyLocationWithPath_ProducesExpectedPathAndStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            var location = StorageLocation.Empty;
            using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
            {
                var newStorageLocation = location.Combine(pathElement, pathElements);

                Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineRelativeLocationWithPath_ProducesExpectedPathAndStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            var location = "some/relative/location/".CreateStorageLocationFromPath();
            using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
            {
                var newStorageLocation = location.Combine(pathElement, pathElements);

                Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineAbsoluteNonexistentLocationWithPath_ProducesExpectedPathAndStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            var location = TempPathOnly.FilePath.CreateStorageLocationFromPath();
            using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
            {
                var newStorageLocation = location.Combine(pathElement, pathElements);

                Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineAbsoluteExistentLocationWithPath_ProducesExpectedPathAndStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            var location = TempDir.Path.CreateStorageLocationFromPath();
            using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
            {
                var newStorageLocation = location.Combine(pathElement, pathElements);

                Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("extra_nest/", false)]
        [InlineData("extra_nest/tagalong_metadata.bin", false)]
        [InlineData("extra_nest/tagalong_msys2.tgz", true)]
        [InlineData("extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.rom", true)]
        [InlineData("tagalong_dev0.luigi", false)]
        [InlineData("extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip", true)]
        [InlineData("extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.cfg", true)]
        [InlineData("extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tag-a-long.zip", true)] // does not exist, may break if / when caching implemented
        [InlineData("extra_nest/tag-a-long_msys2.tgz/tagalong_msys2.tar/tag-a-long.zip", true)] // does not exist, may break if / when caching implemented
        public void StorageLocationExtensions_CombinePathIntoArchiveWithRelativePath_ProducesExpectedPathAndStorageAccess(string relativePath, bool shouldStorageAccessChange)
        {
            string archiveRoot;
            using (TestResource.TagalongZipWithManyNests.ExtractToTemporaryFile(out archiveRoot))
            {
                var location = archiveRoot.CreateStorageLocationFromPath();
                Assert.False(location.UsesDefaultStorage);

                var newStorageLocation = location.Combine(relativePath);

                var expectedNewPath = string.IsNullOrEmpty(relativePath) ? archiveRoot : Path.Combine(archiveRoot, relativePath);
                Assert.Equal(expectedNewPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.False(location.UsesDefaultStorage);
                Assert.Equal(shouldStorageAccessChange, !object.ReferenceEquals(location.StorageAccess, newStorageLocation.StorageAccess));
            }
        }

        #endregion // Combine Tests

        #region GetFileName Tests

        [Fact]
        public void StorageLocationExtensions_GetFileNameFromInvalidLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.InvalidLocation.GetFileName());
        }

        [Fact]
        public void StorageLocationExtensions_GetFileNameFromNullLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.Null.GetFileName());
        }

        [Fact]
        public void StorageLocationExtensions_GetFileNameFromEmptyLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.Empty.GetFileName());
        }

        [Theory]
        [InlineData("/", "")]
        [InlineData("/a/b/c/", "")]
        [InlineData("/a/b/c/d.txt", "d.txt")]
        [InlineData("a/b.zip/f", "f")]
        [InlineData(@"C:\local\path.txt", "path.txt")]
        [InlineData(@"\\", "")]
        [InlineData(@"\\server\loc", "loc")]
        [InlineData(@"X:", "")]
        [InlineData(@"Z:\", "")]
        [InlineData(@"P:\stuff/things\data.tar.gz", "data.tar.gz")]
        public void StorageLocationExtensions_GetFileName_ReturnsExpectedFileName(string path, string expectedFileName)
        {
            var location = path.CreateStorageLocationFromPath();

            var fileName = location.GetFileName();

            Assert.Equal(expectedFileName, fileName);
        }

        #endregion // GetFileName Tests

        private IDisposable PrepareExpectedPathAndPathElementForTest(StorageLocation location, TestResource archiveResource, ref string expectedPath, ref string pathElement)
        {
            IDisposable temporaryLocation = null;
            if (archiveResource != null)
            {
                string archivePath;
                temporaryLocation = archiveResource.ExtractToTemporaryFile(out archivePath);
                if (string.IsNullOrEmpty(pathElement))
                {
                    pathElement = archivePath;
                }
                else
                {
                    pathElement = Path.Combine(archivePath, pathElement);
                }
                if (string.IsNullOrEmpty(expectedPath))
                {
                    expectedPath = archivePath;
                }
                else
                {
                    expectedPath = Path.Combine(archivePath, expectedPath);
                }
            }
            else if (pathElement == "<<::TEMPDIR::>>")
            {
                pathElement = TempDir.Path;
                if (string.IsNullOrEmpty(expectedPath))
                {
                    expectedPath = TempDir.Path;
                }
                else
                {
                    expectedPath = Path.Combine(TempDir.Path, expectedPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(expectedPath));
                    File.Create(expectedPath).Dispose();
                }
            }
            else
            {
                if (location.IsEmpty)
                {
                    if (expectedPath == null)
                    {
                        expectedPath = string.Empty;
                    }
                }
                else if (location.IsValid)
                {
                    if (string.IsNullOrEmpty(expectedPath))
                    {
                        expectedPath = location.Path;
                    }
                    else
                    {
                        expectedPath = Path.Combine(location.Path, expectedPath);
                    }
                }
            }
            return temporaryLocation;
        }
    }
}

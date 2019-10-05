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
using System.Linq;
using INTV.Core.Utility;
using INTV.Shared.CompressedArchiveAccess;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.CompressedArchiveAccess;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public sealed class StorageLocationExtensionsTests : IDisposable, ICompressedArchiveTest
    {
        public StorageLocationExtensionsTests()
        {
            Storage = new StorageAccess();
            INTV.Core.Utility.IStorageAccessHelpers.Initialize(Storage);
        }

        #region State

        private StorageAccess Storage { get; set; }

        public void Dispose()
        {
            INTV.Core.Utility.IStorageAccessHelpers.Remove(Storage);
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
            var storageLocation = TemporaryFile.GenerateUniqueFilePath("INTV_StorageLocationExtensions_CreateFromNonexistentAbsolutePath_IsContainerThrows", ".test").CreateStorageLocationFromPath();

            Assert.False(storageLocation.IsContainer);
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromNonexistentAbsoluteFilePath_LengthThrowsFileNotFoundException()
        {
            var storageLocation = TemporaryFile.GenerateUniqueFilePath("INTV_StorageLocationExtensions_CreateFromNonexistentAbsoluteFilePath_LengthThrowsFileNotFoundException", ".test").CreateStorageLocationFromPath();

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
            using (var tempFile = new TemporaryFile(".test", createFile: true))
            {
                var storageLocation = tempFile.FilePath.CreateStorageLocationFromPath();

                Assert.False(storageLocation.IsContainer);
            }
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteDirectoryPath_IsContainerReturnsTrue()
        {
            string directoryPath;
            using (TestResource.Directory(out directoryPath))
            {
                var storageLocation = directoryPath.CreateStorageLocationFromPath();

                Assert.True(storageLocation.IsContainer);
            }
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteFilePath_LengthReturnsExpectedValue()
        {
            using (var tempFile = new TemporaryFile(".test", createFile: true))
            {
                var storageLocation = tempFile.FilePath.CreateStorageLocationFromPath();

                Assert.Equal(0L, storageLocation.Length);
            }
        }

        [Fact]
        public void StorageLocationExtensions_CreateFromValidAbsoluteDirectoryPath_Length()
        {
            string directoryPath;
            using (TestResource.Directory(out directoryPath))
            {
                var storageLocation = directoryPath.CreateStorageLocationFromPath();

                Assert.Throws<FileNotFoundException>(() => storageLocation.Length);
            }
        }

        public static IEnumerable<object[]> CreateStorageLocationFromArchivePathTestData
        {
            get
            {
                // test archive resource, archive format, is container referred to by relative path to nested a archive, relative path in archive, relative is container
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, "tagalong_dir/tagalong.luigi", false };
                yield return new object[] { TestResource.TagalongDirZip, CompressedArchiveFormat.Zip, false, "tagalong_dir/", true };
                yield return new object[] { TestResource.TagalongNestedZip, CompressedArchiveFormat.Zip, true, "tagalong.zip", true };
                yield return new object[] { TestResource.TagalongBinGZip, CompressedArchiveFormat.GZip, false, "tagalong.bin", false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, "tagalong_dir/tagalong.rom", false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, CompressedArchiveFormat.Tar, false, "tagalong_dir/", true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, false, "tagalong_msys2.tar/bin/tagalong.bin", false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, false, "tagalong_msys2.tar/bin/", true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar", true };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, false, "tagalong_msys2.tar/tagalong.zip/tagalong.cfg", false };
                yield return new object[] { TestResource.TagalongMsys2Tgz, CompressedArchiveFormat.GZip, true, "tagalong_msys2.tar/tagalong.zip", true };
            }
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromCompressedArchiveNameWithNoPath_UsesDefaultStorageAccess(TestResource testResource, CompressedArchiveFormat _1, bool _2, string _3, bool _4)
        {
            var storageLocation = testResource.Name.CreateStorageLocationFromPath();

            Assert.True(storageLocation.UsesDefaultStorage);
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathIntoCompressedArchivePath_UsesCompressedArchiveStorageAccess(TestResource testResource, CompressedArchiveFormat _1, bool _2, string path, bool _4)
        {
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var pathIntoArchive = Path.Combine(archivePath, path);

            var storageLocation = pathIntoArchive.CreateStorageLocationFromPath();

            Assert.True(storageLocation.StorageAccess is ICompressedArchiveAccess);
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathToCompressedArchive_UsesCompressedArchiveStorageAccessOfCorrectFormat(TestResource testResource, CompressedArchiveFormat format, bool _2, string _3, bool _4)
        {
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var storageLocation = archivePath.CreateStorageLocationFromPath();

            var storageAccess = storageLocation.StorageAccess as ICompressedArchiveAccess;
            Assert.Equal(format, storageAccess.Format);
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathToCompressedArchive_IsContainerIsTrue(TestResource testResource, CompressedArchiveFormat _1, bool _2, string _3, bool _4)
        {
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var storageLocation = archivePath.CreateStorageLocationFromPath();

            Assert.True(storageLocation.IsContainer);
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathWithinCompressedArchive_IsContainerIsCorrect(TestResource testResource, CompressedArchiveFormat _1, bool _2, string pathWithinArchive, bool expectedIsContainer)
        {
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var storageLocation = Path.Combine(archivePath, pathWithinArchive).CreateStorageLocationFromPath();

            Assert.Equal(expectedIsContainer, storageLocation.IsContainer);
        }

        [Theory]
        [MemberData("CreateStorageLocationFromArchivePathTestData")]
        public void StorageLocationExtensions_CreateFromPathWithinCompressedArchiveIfPathIsToNestedContainer_IsToNestedArchive(TestResource testResource, CompressedArchiveFormat _1, bool isToNestedArchive, string pathWithinArchive, bool _4)
        {
            if (isToNestedArchive)
            {
                string archivePath;
                testResource.ExtractToTemporaryFile(out archivePath);
                var storageLocation = Path.Combine(archivePath, pathWithinArchive).CreateStorageLocationFromPath();

                var parentStorageAccess = storageLocation.StorageAccess.GetParentStorageAccess();

                Assert.NotNull(parentStorageAccess);
                Assert.True(parentStorageAccess is ICompressedArchiveAccess);
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
            var location = TemporaryFile.GenerateUniqueFilePath("INTV_StorageLocationExtensions_CombineAbsoluteNonexistentLocationWithPath_ProducesExpectedPathAndStorageAccess", ".test").CreateStorageLocationFromPath();
            using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
            {
                var newStorageLocation = location.Combine(pathElement, pathElements);

                Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineAbsoluteNonexistentLocationWithPathWhenArchiveAccessDisabled_ProducesExpectedPathAndDefaultStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            try
            {
                ICompressedArchiveAccessExtensions.DisableCompressedArchiveAccess();
                var location = TemporaryFile.GenerateUniqueFilePath("INTV_StorageLocationExtensions_CombineAbsoluteNonexistentLocationWithPath_ProducesExpectedPathAndStorageAccess", ".test").CreateStorageLocationFromPath();
                using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
                {
                    var newStorageLocation = location.Combine(pathElement, pathElements);

                    Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                    Assert.True(newStorageLocation.UsesDefaultStorage);
                }
            }
            finally
            {
                ICompressedArchiveAccessExtensions.EnableCompressedArchiveAccess();
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineAbsoluteExistentLocationWithPath_ProducesExpectedPathAndStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            string directoryPath;
            using (TestResource.Directory(out directoryPath))
            {
                var location = directoryPath.CreateStorageLocationFromPath();
                using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
                {
                    var newStorageLocation = location.Combine(pathElement, pathElements);

                    Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                    Assert.Equal(archiveResource == null, newStorageLocation.UsesDefaultStorage);
                }
            }
        }

        [Theory]
        [MemberData("CombineStorageLocationPathTestData")]
        public void StorageLocationExtensions_CombineAbsoluteExistentLocationWithPathWhenArchiveAccessDisabled_ProducesExpectedPathAndDefaultStorageAccess(TestResource archiveResource, string pathElement, string[] pathElements, string expectedPath)
        {
            try
            {
                ICompressedArchiveAccessExtensions.DisableCompressedArchiveAccess();
                string directoryPath;
                using (TestResource.Directory(out directoryPath))
                {
                    var location = directoryPath.CreateStorageLocationFromPath();
                    using (PrepareExpectedPathAndPathElementForTest(location, archiveResource, ref expectedPath, ref pathElement))
                    {
                        var newStorageLocation = location.Combine(pathElement, pathElements);

                        Assert.Equal(expectedPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                        Assert.True(newStorageLocation.UsesDefaultStorage);
                    }
                }
            }
            finally
            {
                ICompressedArchiveAccessExtensions.EnableCompressedArchiveAccess();
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
            TestResource.TagalongZipWithManyNests.ExtractToTemporaryFile(out archiveRoot);
            var location = archiveRoot.CreateStorageLocationFromPath();
            Assert.False(location.UsesDefaultStorage);

            var newStorageLocation = location.Combine(relativePath);

            var expectedNewPath = string.IsNullOrEmpty(relativePath) ? archiveRoot : Path.Combine(archiveRoot, relativePath);
            Assert.Equal(expectedNewPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
            Assert.False(location.UsesDefaultStorage);
            Assert.Equal(shouldStorageAccessChange, !object.ReferenceEquals(location.StorageAccess, newStorageLocation.StorageAccess));
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
        public void StorageLocationExtensions_CombinePathIntoArchiveWithRelativePathWhenNestedArchiveAccessDisabled_ProducesExpectedPathAndDoesNotChangeStorageAccess(string relativePath, bool _2)
        {
            try
            {
                ICompressedArchiveAccessExtensions.DisableNestedArchiveAccess();
                string archiveRoot;
                TestResource.TagalongZipWithManyNests.ExtractToTemporaryFile(out archiveRoot);
                var location = archiveRoot.CreateStorageLocationFromPath();
                Assert.False(location.UsesDefaultStorage);

                var newStorageLocation = location.Combine(relativePath);

                var expectedNewPath = string.IsNullOrEmpty(relativePath) ? archiveRoot : Path.Combine(archiveRoot, relativePath);
                Assert.Equal(expectedNewPath.NormalizePathSeparators(), newStorageLocation.Path.NormalizePathSeparators());
                Assert.False(location.UsesDefaultStorage);
                Assert.True(object.ReferenceEquals(location.StorageAccess, newStorageLocation.StorageAccess));
            }
            finally
            {
                ICompressedArchiveAccessExtensions.EnableNestedArchiveAccess();
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

        #region GetFileNameWithoutExtension Tests

        [Fact]
        public void StorageLocationExtensions_GetFileNameWithoutExtensionFromInvalidLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.InvalidLocation.GetFileNameWithoutExtension());
        }

        [Fact]
        public void StorageLocationExtensions_GetFileNameWithoutExtensionFromNullLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.Null.GetFileNameWithoutExtension());
        }

        [Fact]
        public void StorageLocationExtensions_GetFileNameWithoutExtensionFromEmptyLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.Empty.GetFileNameWithoutExtension());
        }

        [Theory]
        [InlineData("/", "")]
        [InlineData("/a/b/c/", "")]
        [InlineData("/a/b/c/d.txt", "d")]
        [InlineData("a/b.zip/f", "f")]
        [InlineData(@"C:\local\path.txt", "path")]
        [InlineData(@"\\", "")]
        [InlineData(@"\\server\loc", "loc")]
        [InlineData(@"X:", "")]
        [InlineData(@"Z:\", "")]
        [InlineData(@"P:\stuff/things\data.tar.gz", "data.tar")]
        public void StorageLocationExtensions_GetFileNameWithoutExtension_ReturnsExpectedFileName(string path, string expectedFileNameWithoutExtension)
        {
            var location = path.CreateStorageLocationFromPath();

            var fileNameWithoutExtension = location.GetFileNameWithoutExtension();

            Assert.Equal(expectedFileNameWithoutExtension, fileNameWithoutExtension);
        }

        #endregion // GetFileNameWithoutExtension Tests

        #region GetExtension Tests

        [Fact]
        public void StorageLocationExtensions_GetExtensionFromInvalidLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.InvalidLocation.GetExtension());
        }

        [Fact]
        public void StorageLocationExtensions_GetExtensionFromNullLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.Null.GetExtension());
        }

        [Fact]
        public void StorageLocationExtensions_GetExtensionFromEmptyLocation_ReturnsNull()
        {
            Assert.Null(StorageLocation.Empty.GetExtension());
        }

        [Theory]
        [InlineData("/", "")]
        [InlineData("/a/b/c/", "")]
        [InlineData("/a/b/c/d.txt", ".txt")]
        [InlineData("a/b.zip/f", "")]
        [InlineData(@"C:\local\path.txt", ".txt")]
        [InlineData(@"\\", "")]
        [InlineData(@"\\server\loc", "")]
        [InlineData(@"X:", "")]
        [InlineData(@"Z:\", "")]
        [InlineData(@"P:\stuff/things\data.tar.gz", ".gz")]
        public void StorageLocationExtensions_GetExtension_ReturnsExpectedExtension(string path, string expectedExtension)
        {
            var location = path.CreateStorageLocationFromPath();

            var extension = location.GetExtension();

            Assert.Equal(expectedExtension, extension);
        }

        #endregion // GetExtension Tests

        #region ChangeExtension Tests

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionOnInvalidLocation_RemainsInvalid()
        {
            var location = StorageLocation.InvalidLocation.ChangeExtension(".foo");

            Assert.Null(location.Path);
            Assert.True(location.IsInvalid);
        }

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionOnNullLocation_RemainsNull()
        {
            var location = StorageLocation.Null.ChangeExtension(".bar");

            Assert.Null(location.Path);
            Assert.True(location.IsNull);
        }

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionOnEmptyLocation_RemainsEmpty()
        {
            var location = StorageLocation.Empty.ChangeExtension(".baz");

            Assert.Empty(location.Path);
            Assert.True(location.IsEmpty);
        }

        [Theory]
        [InlineData("/file.txt", "a", "/file.a")]
        [InlineData("/a/b/c/.o", "..b", "/a/b/c/..b")]
        [InlineData("a/b.zip/f/", ".x.y.z", "a/b.zip/f/.x.y.z")]
        [InlineData(@"C:\local\path", "b", @"C:\local\path.b")]
        [InlineData(@"C:\local\path", "b", @"C:\local\path.b")]
        [InlineData(@"C:\local\path.a.b.c.", "b", @"C:\local\path.a.b.c.b")]
        [InlineData(@"C:\local\path.a.b.c..", "b", @"C:\local\path.a.b.c..b")]
        [InlineData(@"C:\local\path.a.b.c.d", "b", @"C:\local\path.a.b.c.b")]
        public void StorageLocationExtensions_ChangeExtension_ReturnsExpectedPath(string path, string newExtension, string expectedPath)
        {
            var location = path.CreateStorageLocationFromPath();

            var newLocation = location.ChangeExtension(newExtension);

            Assert.Equal(expectedPath.NormalizePathSeparators(), newLocation.Path.NormalizePathSeparators());
        }

        [Theory]
        [InlineData("/file.txt", "/file")]
        [InlineData("/a/b/c/.o", "/a/b/c/")]
        [InlineData("a/b.zip/f/", "a/b.zip/f/")]
        [InlineData(@"C:\local\path", @"C:\local\path")]
        [InlineData(@"C:\local\path.a.b.c.", @"C:\local\path.a.b.c")]
        [InlineData(@"C:\local\path.a.b.c..", @"C:\local\path.a.b.c.")]
        [InlineData(@"C:\local\path.a.b.c.d", @"C:\local\path.a.b.c")]
        public void StorageLocationExtensions_ChangeExtensionWithNullExtension_RemovesExtension(string path, string expectedPath)
        {
            var location = path.CreateStorageLocationFromPath();

            var newLocation = location.ChangeExtension(null);

            Assert.Equal(expectedPath.NormalizePathSeparators(), newLocation.Path.NormalizePathSeparators());
        }

        [Theory]
        [InlineData("/file.txt", "/file.")]
        [InlineData("/a/b/c/.o", "/a/b/c/.")]
        [InlineData("a/b.zip/f/", "a/b.zip/f/.")]
        [InlineData(@"C:\local\path", @"C:\local\path.")]
        [InlineData(@"C:\local\path.a.b.c.", @"C:\local\path.a.b.c.")]
        [InlineData(@"C:\local\path.a.b.c..", @"C:\local\path.a.b.c..")]
        [InlineData(@"C:\local\path.a.b.c.d", @"C:\local\path.a.b.c.")]
        public void StorageLocationExtensions_ChangeExtensionWithEmptyExtension_RemovesCharactersAfterLastPeriodAddsPeriodIfNone(string path, string expectedPath)
        {
            var location = path.CreateStorageLocationFromPath();

            var newLocation = location.ChangeExtension(string.Empty);

            Assert.Equal(expectedPath.NormalizePathSeparators(), newLocation.Path.NormalizePathSeparators());
        }

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionFromArchiveToNonArchive_ChangeStorageAccessToDefault()
        {
            string archivePath;
            TestResource.TagalongMultipleNested.ExtractToTemporaryFile(out archivePath);
            var location = archivePath.CreateStorageLocationFromPath();
            Assert.True(location.StorageAccess is ICompressedArchiveAccess);

            var newLocation = location.ChangeExtension(".txt");

            Assert.True(newLocation.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionFromNonArchiveToArchive_ChangesStorageAccessToNonDefault()
        {
            string archivePath;
            TestResource.TagalongMultipleNested.ExtractToTemporaryFile(out archivePath);
            var path = Path.ChangeExtension(archivePath, "txt");
            var location = path.CreateStorageLocationFromPath();
            Assert.True(location.UsesDefaultStorage);

            var newLocation = location.ChangeExtension(".zip");

            Assert.True(newLocation.StorageAccess is ICompressedArchiveAccess);
        }

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionFromOneArchiveToAnother_ChangesStorageAccess()
        {
            string zipArchivePath;
            string tarArchivePath;
            using (TestResource.TagalongZip.ExtractToTemporaryFile(out zipArchivePath))
            using (TestResource.TagalongTar.ExtractToTemporaryFile(out tarArchivePath))
            {
                File.Move(tarArchivePath, Path.Combine(Path.GetDirectoryName(zipArchivePath), Path.GetFileName(tarArchivePath)));
                var zipLocation = zipArchivePath.CreateStorageLocationFromPath();
                Assert.False(zipLocation.UsesDefaultStorage);
                var storageAccess = zipLocation.StorageAccess as ICompressedArchiveAccess;
                var initialArchiveFormat = storageAccess.Format;

                var tarLocation = zipLocation.ChangeExtension(".tar");

                storageAccess = tarLocation.StorageAccess as ICompressedArchiveAccess;
                Assert.NotNull(storageAccess);
                Assert.NotEqual(initialArchiveFormat, storageAccess.Format);
            }
        }

        [Fact]
        public void StorageLocationExtensions_ChangeExtensionFromOneNestedArchiveToAnotherViaFileExtension_ChangesStorageAccess()
        {
            string archivePath;
            TestResource.TagalongMultipleNested.ExtractToTemporaryFile(out archivePath);
            var path = Path.Combine(archivePath, "tagalong.tar");
            var location = path.CreateStorageLocationFromPath();
            Assert.False(location.UsesDefaultStorage);
            var storageAccess = location.StorageAccess as ICompressedArchiveAccess;
            var initialArchiveFormat = storageAccess.Format;

            var newLocation = location.ChangeExtension(".zip");

            storageAccess = newLocation.StorageAccess as ICompressedArchiveAccess;
            Assert.NotNull(storageAccess);
            Assert.NotEqual(initialArchiveFormat, storageAccess.Format);
        }

        #endregion // ChangeExtension Tests

        #region AddSuffix Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData(" b")]
        [InlineData(".biff")]
        public void StorageLocationExtensions_AddSuffixToInvalidLocation_(string suffix)
        {
            var location = StorageLocation.InvalidLocation.AddSuffix(suffix);

            Assert.Null(location.Path);
            Assert.True(location.IsInvalid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData(" b")]
        [InlineData(".biff")]
        public void StorageLocationExtensions_AddSuffixToNullLocation_(string suffix)
        {
            var location = StorageLocation.Null.AddSuffix(suffix);

            Assert.Null(location.Path);
            Assert.True(location.IsNull);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData(" b")]
        [InlineData(".biff")]
        public void StorageLocationExtensions_AddSuffixToEmptyLocation_(string suffix)
        {
            var location = StorageLocation.Empty.AddSuffix(suffix);

            Assert.Empty(location.Path);
            Assert.True(location.IsEmpty);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData(" b")]
        [InlineData(".biff")]
        public void StorageLocationExtensions_AddSuffixToLocation_AddsSuffix(string suffix)
        {
            var path = "/path/to/something";
            var location = path.CreateStorageLocationFromPath().AddSuffix(suffix);

            var expectedLocationPath = path + suffix;
            Assert.Equal(expectedLocationPath, location.Path);
            Assert.True(location.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_AddSuffixToCausePathToAppearAsNonexistentArchive_AddsSuffixRetainsDefaultStorage()
        {
            var filePath = TemporaryFile.GenerateUniqueFilePath(string.Empty, string.Empty);
            var location = filePath.CreateStorageLocationFromPath();
            Assert.True(location.UsesDefaultStorage);

            var suffix = "_biff.zip";
            var newLocation = location.AddSuffix(suffix);

            Assert.Equal(filePath + suffix, newLocation.Path);
            Assert.True(newLocation.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_AddSuffixCausesPathToMatchArchive_StorageAccessChanges()
        {
            string archiveLocation;
            TestResource.TagalongBinCfgTar.ExtractToTemporaryFile(out archiveLocation);
            var path = archiveLocation.Substring(0, archiveLocation.LastIndexOf('_'));
            var location = path.CreateStorageLocationFromPath();
            Assert.True(location.UsesDefaultStorage);

            var newLocation = location.AddSuffix("_bc.tar");

            Assert.False(newLocation.UsesDefaultStorage);
            Assert.True(newLocation.StorageAccess is ICompressedArchiveAccess);
        }

        [Fact]
        public void StorageLocationExtensions_AddSuffixCausesPathToNoLongerMatchArchive_StorageAccessChanges()
        {
            string archiveLocation;
            TestResource.TagalongBinGZip.ExtractToTemporaryFile(out archiveLocation);
            var location = archiveLocation.CreateStorageLocationFromPath();
            Assert.False(location.UsesDefaultStorage);
            Assert.True(location.StorageAccess is ICompressedArchiveAccess);

            var newLocation = location.AddSuffix("booger.tar");

            Assert.True(newLocation.UsesDefaultStorage);
            Assert.False(newLocation.StorageAccess is ICompressedArchiveAccess);
        }

        #endregion // AddSuffix Tests

        #region GetContainingLocation Tests

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfInvalidLocation_ReturnsInvalidLocation()
        {
            Assert.True(StorageLocation.InvalidLocation.GetContainingLocation().IsInvalid);
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfNullLocation_ReturnsNullLocation()
        {
            Assert.True(StorageLocation.Null.GetContainingLocation().IsNull);
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfEmptyLocation_ReturnsEmptyLocation()
        {
            Assert.True(StorageLocation.Empty.GetContainingLocation().IsEmpty);
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfSingleElementRelativeLocation_ReturnsEmptyLocation()
        {
            var location = "abcde".CreateStorageLocationFromPath();

            var containingLocation = location.GetContainingLocation();

            Assert.True(containingLocation.IsEmpty);
        }

        [Theory]
        [InlineData("x/")]
        [InlineData("/a/abc/")]
        [InlineData(@"V:\aaa\bbb\")]
        [InlineData(@"def\")]
        public void StorageLocationExtensions_GetContainingLocationOfLocationEndingInSeparator_ReturnsLocationWithoutSeparator(string path)
        {
            var location = path.CreateStorageLocationFromPath();

            var containingLocation = location.GetContainingLocation();

            var expectedLocation = path.Substring(0, path.Length - 1).NormalizePathSeparators();
            Assert.Equal(expectedLocation, containingLocation.Path.NormalizePathSeparators());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("//server")]
        [InlineData("A:/")]
        [InlineData(@"\")]
        [InlineData(@"V:\")]
        [InlineData(@"\\server")]
        public void StorageLocationExtensions_GetContainingLocationOfRootLocation_ReturnsNullLocation(string path)
        {
            var location = path.CreateStorageLocationFromPath();

            Assert.True(location.GetContainingLocation().IsNull);
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfArchiveEndingInSeparator_ReturnsArchiveWithSameStorageAccess()
        {
            string archivePath;
            TestResource.TagalongZip.ExtractToTemporaryFile(out archivePath);
            var path = archivePath + Path.DirectorySeparatorChar;
            var location = path.CreateStorageLocationFromPath();

            var containingLocation = location.GetContainingLocation();

            Assert.True(object.ReferenceEquals(location.StorageAccess, containingLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfArchive_ReturnsLocationWithDefaultStorageAccess()
        {
            string archivePath;
            TestResource.TagalongDirZip.ExtractToTemporaryFile(out archivePath);
            var location = archivePath.CreateStorageLocationFromPath();

            var containingLocation = location.GetContainingLocation();

            Assert.True(containingLocation.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfDirectoryWithinArchive_ReturnsLocationWithSameStorageAccess()
        {
            string archivePath;
            TestResource.TagalongDirZip.ExtractToTemporaryFile(out archivePath);
            var location = Path.Combine(archivePath, "tagalong_dir/").CreateStorageLocationFromPath();

            var containingLocation = location.GetContainingLocation();

            Assert.True(object.ReferenceEquals(location.StorageAccess, containingLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_GetContainingLocationOfNestedArchive_ReturnsLocationWithStorageAccessOfContainingArchive()
        {
            string archivePath;
            TestResource.TagalongZipWithManyNests.ExtractToTemporaryFile(out archivePath);
            var location = Path.Combine(archivePath, "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip").CreateStorageLocationFromPath();

            var containingLocation = location.GetContainingLocation();

            Assert.False(object.ReferenceEquals(location.StorageAccess, containingLocation.StorageAccess));
        }

        #endregion // GetContainingLocation

        #region AlterContainingLocation Tests

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfInvalidLocation_ReturnsInvalidLocation()
        {
            Assert.True(StorageLocation.InvalidLocation.AlterContainingLocation(null).IsInvalid);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfNullLocation_ReturnsNullLocation()
        {
            Assert.True(StorageLocation.Null.AlterContainingLocation(null).IsNull);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfEmptyLocation_ReturnsEmptyLocation()
        {
            Assert.True(StorageLocation.Empty.AlterContainingLocation(null).IsEmpty);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfSingleElementRelativeLocationToNull_ThrowsArgumentNullException()
        {
            var location = "abcde".CreateStorageLocationFromPath();

            Assert.Throws<ArgumentNullException>(()  => location.AlterContainingLocation(null));
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfSingleElementRelativeLocationToEmpty_ReturnsSameLocation()
        {
            var location = "abcde".CreateStorageLocationFromPath();

            var alteredLocation = location.AlterContainingLocation(string.Empty);

            Assert.Equal(location, alteredLocation);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfSingleElementRelativeLocationToNewRelativeLocation_ReturnsLocationWithNewContainingPath()
        {
            var location = "abcde".CreateStorageLocationFromPath();

            var newContainingPath = "new parent";
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            var expectedPath = Path.Combine(newContainingPath, "abcde").NormalizePathSeparators();
            Assert.Equal(expectedPath, alteredLocation.Path.NormalizePathSeparators());
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfSingleElementRelativeLocationToNewAbsoluteLocation_ReturnsLocationWithNewContainingPath()
        {
            var location = "abcde".CreateStorageLocationFromPath();

            var newContainingPath = Path.GetTempPath();
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            var expectedPath = Path.Combine(newContainingPath, "abcde").NormalizePathSeparators();
            Assert.Equal(expectedPath, alteredLocation.Path.NormalizePathSeparators());
        }

        [Theory]
        [InlineData("x/")]
        [InlineData("/a/abc/")]
        [InlineData(@"V:\aaa\bbb\")]
        [InlineData(@"def\")]
        [InlineData(@"\\server\")]
        [InlineData(@"\\server\share\")]
        [InlineData(@"\\server\share\folder\")]
        public void StorageLocationExtensions_AlterContainingLocationOfLocationEndingInSeparatorToNewRelativeLocation_ReturnsNewRelativeLocationOnly(string path)
        {
            var location = path.CreateStorageLocationFromPath();

            var newContainingPath = "origami/brain/";
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            var expectedPath = newContainingPath.NormalizePathSeparators();
            Assert.Equal(expectedPath, alteredLocation.Path.NormalizePathSeparators());
        }

        [Theory]
        [InlineData("x/")]
        [InlineData("/a/abc/")]
        [InlineData(@"V:\aaa\bbb\")]
        [InlineData(@"def\")]
        ////[InlineData(@"\\server\")] // excluded -- PathComparer throws ArgumentException due to UNC rules
        [InlineData(@"\\server\share\")]
        [InlineData(@"\\server\share\folder\")]
        public void StorageLocationExtensions_AlterContainingLocationOfLocationEndingInSeparatorToAbsoluteLocation_ReturnsNewAbsoluteLocationOnly(string path)
        {
            var location = path.CreateStorageLocationFromPath();

            var newContainingPath = Path.GetTempPath();
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            var expectedPath = newContainingPath.NormalizePathSeparators();
            Assert.Equal(expectedPath, alteredLocation.Path.NormalizePathSeparators());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("//")]
        [InlineData("A:/")]
        [InlineData(@"\")]
        [InlineData(@"V:\")]
        [InlineData(@"\\")]
        public void StorageLocationExtensions_AlterContainingLocationOfRootNonUNCLocationToRelativeLocation_ReturnsEmptyLocation(string path)
        {
            var location = path.CreateStorageLocationFromPath();

            var alteredLocation = location.AlterContainingLocation(string.Empty);

            Assert.True(alteredLocation.IsEmpty);
        }

        [Theory]
        [InlineData(@"\\", "")]
        [InlineData("//server", "server")]
        [InlineData(@"\\server", "server")]
        [InlineData(@"\\server\share", "share")]
        [InlineData(@"\\server\share\folder", "folder")]
        [InlineData(@"\\server\share\folder\subfolder", "subfolder")]
        public void StorageLocationExtensions_AlterContainingLocationOfUNCLocationToEmpty_ReturnsLocationWithPathRootRemoved(string path, string expectedPath)
        {
            var location = path.CreateStorageLocationFromPath();

            var alteredLocation = location.AlterContainingLocation(string.Empty);

            Assert.Equal(expectedPath, alteredLocation.Path);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfArchiveToEmpty_ReturnsLocationWithFileNameAndDefaultStorageAccess()
        {
            string archivePath;
            TestResource.TagalongZip.ExtractToTemporaryFile(out archivePath);
            var location = archivePath.CreateStorageLocationFromPath();
            Assert.False(location.UsesDefaultStorage);

            var alteredLocation = location.AlterContainingLocation(string.Empty);

            var expectedPath = Path.GetFileName(archivePath);
            Assert.Equal(expectedPath, alteredLocation.Path);
            Assert.True(alteredLocation.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfArchiveToNonArchiveLocation_ReturnsLocationWithDefaultStorageAccess()
        {
            string archivePath;
            string directoryPath;
            TestResource.TagalongDirZip.ExtractToTemporaryFile(out archivePath);
            TestResource.Directory(out directoryPath);
            var location = archivePath.CreateStorageLocationFromPath();
            Assert.False(location.UsesDefaultStorage);

            var alteredLocation = location.AlterContainingLocation(directoryPath);

            Assert.True(alteredLocation.Path.NormalizePathSeparators().StartsWith(directoryPath.NormalizePathSeparators()));
            Assert.True(alteredLocation.UsesDefaultStorage);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfDirectoryWithinSameArchive_ReturnsLocationWithSameStorageAccess()
        {
            string archivePath;
            TestResource.TagalongTgzManyDirs.ExtractToTemporaryFile(out archivePath);
            var location = Path.Combine(archivePath, "tagalong_many_dirs.tar/rootSub/sub0/.DS_Store").CreateStorageLocationFromPath();

            var newContainingPath = Path.Combine(archivePath, "tagalong_many_dirs.tar/rootSub/sub1");
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            Assert.True(object.ReferenceEquals(location.StorageAccess, alteredLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfArchiveToOneInDifferentArchive_ReturnsLocationWithStorageAccessOfNewContainingArchive()
        {
            string archivePath;
            TestResource.TagalongZipWithManyNests.ExtractToTemporaryFile(out archivePath);
            var location = Path.Combine(archivePath, "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/tagalong.zip/tagalong.bin").CreateStorageLocationFromPath();
            Assert.Equal(CompressedArchiveFormat.Zip, ((ICompressedArchiveAccess)location.StorageAccess).Format);

            var newContainingPath = Path.Combine(archivePath, "extra_nest/tagalong_msys2.tgz/tagalong_msys2.tar/bin");
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            Assert.False(object.ReferenceEquals(location.StorageAccess, alteredLocation.StorageAccess));
            Assert.Equal(CompressedArchiveFormat.Tar, ((ICompressedArchiveAccess)alteredLocation.StorageAccess).Format);
        }

        [Fact]
        public void StorageLocationExtensions_AlterContainingLocationOfArchiveToDifferentLocationOnDisk_ReturnsLocationWithStorageAccessOfNewContainingArchive()
        {
            string archivePath0;
            string archivePath1;
            TestResource.TagalongZip.ExtractToTemporaryFile(out archivePath0);
            TestResource.TagalongZip.ExtractToTemporaryFile(out archivePath1);
            var location = archivePath0.CreateStorageLocationFromPath();

            var newContainingPath = Path.GetDirectoryName(archivePath1);
            var alteredLocation = location.AlterContainingLocation(newContainingPath);

            Assert.False(object.ReferenceEquals(location.StorageAccess, alteredLocation.StorageAccess));
            Assert.False(alteredLocation.UsesDefaultStorage);
        }

        #endregion // AlterContainingLocation Tests

        #region EnumerateFiles Tests

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesOnDisk_ReturnsExpectedFiles()
        {
            var numberOfFiles = 12;
            string directoryPath;
            using (DirectoryContainingFiles(numberOfFiles, out directoryPath))
            {
                var files = directoryPath.CreateStorageLocationFromPath().EnumerateFiles();

                Assert.Equal(numberOfFiles, files.Count());
            }
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesOnDiskWithFileExtension_ReturnsFilesWithCorrectExtension()
        {
            var numberOfFiles = 16;
            string directoryPath;
            using (DirectoryContainingFiles(numberOfFiles, out directoryPath, ".txt", ".luigi", ".rom", ".bin", ".cfg"))
            {
                var fileExtension = ".cfg";
                var files = directoryPath.CreateStorageLocationFromPath().EnumerateFiles(fileExtension).ToList();

                Assert.All(files, f => Assert.Equal(fileExtension, Path.GetExtension(f.Path)));
            }
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesOnDiskWithFileExtensionNotFoundInDirectory_ReturnsEmtpyFileList()
        {
            var numberOfFiles = 16;
            string directoryPath;
            using (DirectoryContainingFiles(numberOfFiles, out directoryPath, ".txt", ".luigi", ".rom", ".bin", ".cfg"))
            {
                var fileExtension = ".pdf";
                var files = directoryPath.CreateStorageLocationFromPath().EnumerateFiles(fileExtension);

                Assert.Empty(files);
            }
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesAtRootOfArchiveWithOnlyFiles_ReturnsExpectedFiles()
        {
            var testResource = TestResource.TagalongZip;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);
            var files = archivePath.CreateStorageLocationFromPath().EnumerateFiles();

            Assert.Equal(testResource.ArchiveContents.Count(), files.Count());
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesAtRootOfArchiveWithOnlyArchive_ReturnsNoFiles()
        {
            var testResource = TestResource.TagalongMsys2Tgz;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);
            var files = archivePath.CreateStorageLocationFromPath().EnumerateFiles();

            Assert.Empty(files);
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesAtRootOfArchiveWithOnlyDirectories_ReturnsNoFiles()
        {
            var testResource = TestResource.TagalongDirLuigiRomTar;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);
            var files = archivePath.CreateStorageLocationFromPath().EnumerateFiles();

            Assert.Empty(files);
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesInNestedArchiveWithFilesDirsAndArchives_ReturnsOnlyFiles()
        {
            var testResource = TestResource.TagalongMsys2Tgz;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var files = Path.Combine(archivePath, "tagalong_msys2.tar/tagalong.zip").CreateStorageLocationFromPath().EnumerateFiles();

            // kinda hacky -- we "know" this because of the known layout of the test resource.
            Assert.Equal(2, files.Count());
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesInDirectoryInNestedArchive_ReturnsExpectedFiles()
        {
            var testResource = TestResource.TagalongMultipleNested;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var files = Path.Combine(archivePath, "tagalong.tgz/tagalong.tar/bin/").CreateStorageLocationFromPath().EnumerateFiles();

            // kinda hacky -- we "know" this because of the known layout of the test resource.
            Assert.Equal(2, files.Count());
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesInDirectoryMissingTrailingSeparatorInNestedArchive_ReturnsExpectedFiles()
        {
            var testResource = TestResource.TagalongMultipleNested;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);
            var location = Path.Combine(archivePath, "tagalong.tgz/tagalong.tar/bin").CreateStorageLocationFromPath();

            var files = location.EnumerateFiles();

            // kinda hacky -- we "know" this because of the known layout of the test resource.
            Assert.Equal(2, files.Count());
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesInArchiveWithFileExtension_ReturnsFilesWithCorrectExtension()
        {
            var testResource = TestResource.TagalongCC3RomTar;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var fileExtension = ".rom";
            var files = archivePath.CreateStorageLocationFromPath().EnumerateFiles(fileExtension).ToList();

            Assert.All(files, f => Assert.Equal(fileExtension, Path.GetExtension(f.Path)));
        }

        [Fact]
        public void StorageLocationExtensions_EnumerateFilesInArchiveWithFileExtensionNotFoundInDirectory_ReturnsEmtpyFileList()
        {
            var testResource = TestResource.TagalongCC3RomTar;
            string archivePath;
            testResource.ExtractToTemporaryFile(out archivePath);

            var fileExtension = ".pdf";
            var files = archivePath.CreateStorageLocationFromPath().EnumerateFiles(fileExtension);

            Assert.Empty(files);
        }

        #endregion // EnumerateFiles Tests

        #region CopyFile Tests

        [Fact]
        public void StorageAccessExtensions_CopyFileOnNullStorageLocation_ThrowsArgumentNullException()
        {
            StorageLocation location = null;

            Assert.Throws<ArgumentNullException>(() => location.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileOnInvalidStorageAccess_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => StorageLocation.InvalidLocation.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileOnNullPathStorageAccess_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => StorageLocation.Null.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileOnEmptyPathStorageAccess_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => StorageLocation.Empty.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileOnUnRootedStorageAccess_ThrowsArgumentException()
        {
            var location = "some/kind/of/path".CreateStorageLocationFromPath();

            Assert.Throws<ArgumentException>(() => location.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileOnStorageAccessOfDirectory_ThrowsArgumentException()
        {
            var location = Path.GetTempPath().CreateStorageLocationFromPath();

            Assert.Throws<ArgumentException>(() => location.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileToNullDestinationLocation_ThrowsArgumentNullException()
        {
            var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).CreateStorageLocationFromPath();

            Assert.Throws<ArgumentNullException>(() => location.CopyFile(null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileToInvalidDestinationLocation_ThrowsInvalidOperationException()
        {
            var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).CreateStorageLocationFromPath();

            Assert.Throws<InvalidOperationException>(() => location.CopyFile(StorageLocation.InvalidLocation));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileToNullPathDestinationLocation_ThrowsArgumentException()
        {
            var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).CreateStorageLocationFromPath();

            Assert.Throws<ArgumentException>(() => location.CopyFile(StorageLocation.Null));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileToEmptyPathDestinationLocation_ThrowsArgumentException()
        {
            var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).CreateStorageLocationFromPath();

            Assert.Throws<ArgumentException>(() => location.CopyFile(StorageLocation.Empty));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileToUnRootedDestinationLocation_ThrowsArgumentException()
        {
            var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).CreateStorageLocationFromPath();
            var destination = "a/path/to/nowhere".CreateStorageLocationFromPath();

            Assert.Throws<ArgumentException>(() => location.CopyFile(destination));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFile_Succeeds()
        {
            string directoryPath;
            using (TestResource.Directory(out directoryPath))
            {
                var testFile = Path.Combine(directoryPath, "test.file");
                File.Create(testFile).Dispose();
                var source = testFile.CreateStorageLocationFromPath();
                var targetFile = Path.Combine(directoryPath, "test-copy.file");
                var target = targetFile.CreateStorageLocationFromPath();

                source.CopyFile(target);

                Assert.True(File.Exists(targetFile));
            }
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileWhenTargetExists_ThrowsIOException()
        {
            string directoryPath;
            using (TestResource.Directory(out directoryPath))
            {
                var sourceFile = Path.Combine(directoryPath, "test.file");
                File.Create(sourceFile).Dispose();
                var targetFile = Path.Combine(directoryPath, "test-copy.file");
                File.Create(targetFile).Dispose();
                var source = sourceFile.CreateStorageLocationFromPath();
                var target = targetFile.CreateStorageLocationFromPath();

                Assert.Throws<IOException>(() => source.CopyFile(target));
            }
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileWhenTargetExistsOverwriteIsTrue_Succeeds()
        {
            string directoryPath;
            using (TestResource.Directory(out directoryPath))
            {
                var sourceFile = Path.Combine(directoryPath, "test.file");
                var dataLength = 128L;
                using (var s = File.Create(sourceFile))
                {
                    var data = new byte[dataLength];
                    s.Write(data, 0, data.Length);
                }
                var targetFile = Path.Combine(directoryPath, "test-copy.file");
                File.Create(targetFile).Dispose();
                var source = sourceFile.CreateStorageLocationFromPath();
                var target = targetFile.CreateStorageLocationFromPath();

                source.CopyFile(target, overwrite: true);

                var fileInfo = new FileInfo(targetFile);
                Assert.True(fileInfo.Exists);
                Assert.Equal(dataLength, fileInfo.Length);
            }
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileToArchiveDestinationLocation_ThrowsNotSupportedException()
        {
            var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).CreateStorageLocationFromPath();
            string archivePath;
            TestResource.TagalongZip.ExtractToTemporaryFile(out archivePath);
            var destination = archivePath.CreateStorageLocationFromPath();

            Assert.Throws<NotSupportedException>(() => location.CopyFile(destination));
        }

        [Fact]
        public void StorageAccessExtensions_CopyFileInNestedArchiveToDirectory_CopiesFile()
        {
            string directoryPath;
            string rootArchivePath;
            TestResource.Directory(out directoryPath);
            TestResource.TagalongMultipleNested.ExtractToTemporaryFile(out rootArchivePath);
            var sourceLocation = Path.Combine(rootArchivePath, "tagalong.tgz/tagalong.tar/tagalong.zip/tagalong.cfg").CreateStorageLocationFromPath();

            var destinationLocation = Path.Combine(directoryPath, "tagalong.cfg").CreateStorageLocationFromPath();
            sourceLocation.CopyFile(destinationLocation);

            var fileInfo = new FileInfo(destinationLocation.Path);
            Assert.True(fileInfo.Exists);
            Assert.True(fileInfo.Length > 12);
        }

        #endregion // CopyFile Tests

        #region EnsureUnique Tests

        [Fact]
        public void StorageLocationExtensions_EnsureUniquePathOnInvalidLocation_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => StorageLocation.InvalidLocation.EnsureUnique());
        }

        [Fact]
        public void StorageLocationExtensions_EnsureUniquePathForNonexistentFile_ReturnsFile()
        {
            var fileName = TemporaryFile.GenerateUniqueFilePath(string.Empty, ".txt");
            var location = fileName.CreateStorageLocationFromPath();

            var uniqueLocation = location.EnsureUnique();

            Assert.Equal(fileName, uniqueLocation.Path);
            Assert.False(uniqueLocation.Exists());
        }

        [Fact]
        public void StorageLocationExtensions_EnsureUniquePath_ReturnsUniquePath()
        {
            using (var tempFile = new TemporaryFile(".txt", createFile: true))
            {
                var location = tempFile.FilePath.CreateStorageLocationFromPath();

                var uniqueLocation = location.EnsureUnique();

                Assert.True(uniqueLocation.Path.Length > tempFile.FilePath.Length);
                Assert.False(uniqueLocation.Exists());
            }
        }

        [Fact]
        public void StorageLocationExtensions_EnsureUniquePathForNonexistentFileInArchive_ReturnsUniquePath()
        {
            string archivePath;
            TestResource.TagalongDirZip.ExtractToTemporaryFile(out archivePath);
            var location = archivePath.CreateStorageLocationFromPath().Combine("nothing to see here.txt");

            var uniqueLocation = location.EnsureUnique();

            Assert.Equal(location.Path, uniqueLocation.Path);
            Assert.False(uniqueLocation.Exists());
            Assert.True(object.ReferenceEquals(location.StorageAccess, uniqueLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_EnsureUniquePathInArchive_ReturnsUniquePath()
        {
            string archivePath;
            TestResource.TagalongDirZip.ExtractToTemporaryFile(out archivePath);
            var location = archivePath.CreateStorageLocationFromPath().Combine("tagalong_dir/tagalong.luigi");
            Assert.True(location.Exists());

            var uniqueLocation = location.EnsureUnique();

            Assert.True(uniqueLocation.Path.Length > location.Path.Length);
            Assert.False(uniqueLocation.Exists());
            Assert.True(object.ReferenceEquals(location.StorageAccess, uniqueLocation.StorageAccess));
        }

        #endregion // EnsureUnique Tests

        #region NormalizeSeparators Tests

        [Fact]
        public void StorageLocationExtensions_NormalizeSeparatorsOnInvalidStorageLocation_RetainsStorageAccess()
        {
            var location = StorageLocation.InvalidLocation;

            var normalizedLocation = location.NormalizeSeparators();

            Assert.True(object.ReferenceEquals(location.StorageAccess, normalizedLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_NormalizeSeparatorsOnNullStorageLocation_RetainsStorageAccess()
        {
            var location = StorageLocation.Null;

            var normalizedLocation = location.NormalizeSeparators();

            Assert.True(object.ReferenceEquals(location.StorageAccess, normalizedLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_NormalizeSeparatorsOnEmptyStorageLocation_RetainsStorageAccess()
        {
            var location = StorageLocation.Empty;

            var normalizedLocation = location.NormalizeSeparators();

            Assert.True(object.ReferenceEquals(location.StorageAccess, normalizedLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_NormalizeSeparatorsOnNormalPath_RetainsStorageAccess()
        {
            var location = TemporaryFile.GenerateUniqueFilePath(string.Empty, ".thag").CreateStorageLocationFromPath();

            var normalizedLocation = location.NormalizeSeparators();

            Assert.False(normalizedLocation.Path.Contains('\\'));
            Assert.True(object.ReferenceEquals(location.StorageAccess, normalizedLocation.StorageAccess));
        }

        [Fact]
        public void StorageLocationExtensions_NormalizeSeparatorsInArchive_RetainsStorageAccess()
        {
            string archivePath;
            TestResource.TagalongEmptyZip.ExtractToTemporaryFile(out archivePath);
            var location = archivePath.CreateStorageLocationFromPath();

            var normalizedLocation = location.NormalizeSeparators();

            Assert.False(normalizedLocation.Path.Contains('\\'));
            Assert.True(object.ReferenceEquals(location.StorageAccess, normalizedLocation.StorageAccess));
        }

        #endregion // NormalizeSeparators Tests

        #region Combine Tests Helpers

        private static IDisposable PrepareExpectedPathAndPathElementForTest(StorageLocation location, TestResource archiveResource, ref string expectedPath, ref string pathElement)
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
                temporaryLocation = TestResource.Directory(out pathElement);
                if (string.IsNullOrEmpty(expectedPath))
                {
                    expectedPath = pathElement;
                }
                else
                {
                    expectedPath = Path.Combine(pathElement, expectedPath);
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

        #endregion // Combine Tests Helpers

        #region EnumerateFiles Tests Helpers

        private static IDisposable DirectoryContainingFiles(int numFiles, out string directoryPath, params string[] fileExtensions)
        {
            var directory = TestResource.Directory(out directoryPath);
            var fileExtensionsForGeneration = new List<string>(fileExtensions);
            if (!fileExtensions.Any())
            {
                fileExtensionsForGeneration.Add(".txt");
            }
            var numFileExtensions = fileExtensionsForGeneration.Count;
            for (var i = 0; i < numFiles; ++i)
            {
                var fileExtension = fileExtensionsForGeneration[i % numFileExtensions];
                var fileName = "test-" + i.ToString("D4") + fileExtension;
                File.Create(Path.Combine(directoryPath, fileName)).Dispose();
            }
            return directory;
        }

        #endregion // EnumerateFiles Tests Helpers
    }
}

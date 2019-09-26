// <copyright file="StorageLocationTests.cs" company="INTV Funhouse">
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
using System.IO;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class StorageLocationTests
    {
        // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
        private static readonly System.DateTime FileNotFoundTime = new System.DateTime(1601, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        [Fact]
        public void StorageLocation_OpenInvalidFilePath_ReturnsNull()
        {
            var storage = new StorageAccess();

            var path = TemporaryFile.GenerateUniqueFilePath("fake", ".file");

            Assert.Null(storage.Open(path));
        }

        [Fact]
        public void StorageLocation_OpenValidFilePath_ReturnsValidStream()
        {
            var storage = new StorageAccess();
            using (var tempFile = new TemporaryFile(".file", createFile: true))
            using (var stream = storage.Open(tempFile.FilePath))
            {
                Assert.NotNull(stream);
            }
        }

        [Fact]
        public void StorageLocation_OpenValidDirectoryPath_ReturnsNull()
        {
            var storage = new StorageAccess();

            var path = Path.GetTempPath();

            Assert.Null(storage.Open(path));
        }

        [Fact]
        public void StorageLocation_InvalidFilePathExists_ReturnsFalse()
        {
            var storage = new StorageAccess();

            var path = TemporaryFile.GenerateUniqueFilePath("fake", ".file");

            Assert.False(storage.Exists(path));
        }

        [Fact]
        public void StorageLocation_ValidFilePathExists_ReturnsTrue()
        {
            using (var tempFile = new TemporaryFile(".file", createFile: true))
            {
                var storage = new StorageAccess();

                Assert.True(storage.Exists(tempFile.FilePath));
            }
        }

        [Fact]
        public void StorageLocation_ValidDirectoryPathExists_ReturnsFalse()
        {
            var storage = new StorageAccess();

            var path = Path.GetTempPath();

            Assert.False(storage.Exists(path));
        }

        [Fact]
        public void StorageLocation_InvalidFilePathFileSize_ThrowsFileNotFoundException()
        {
            var storage = new StorageAccess();

            var path = TemporaryFile.GenerateUniqueFilePath("fake", ".file");

            Assert.Throws<FileNotFoundException>(() => storage.Size(path));
        }

        [Fact]
        public void StorageLocation_ValidFilePathFileSize_ReturnsExpectedSize()
        {
            using (var tempFile = new TemporaryFile(".file", createFile: true))
            {
                var storage = new StorageAccess();

                Assert.Equal(0L, storage.Size(tempFile.FilePath));
            }
        }

        [Fact]
        public void StorageLocation_ValidDirectoryPathFileSize_ThrowsFileNotFoundException()
        {
            var storage = new StorageAccess();

            var path = Path.GetTempPath();

            Assert.Throws<FileNotFoundException>(() => storage.Size(path));
        }

        [Fact]
        public void StorageLocation_InvalidFilePathLastWriteTime_ReturnsMinimumTime()
        {
            var storage = new StorageAccess();

            var path = TemporaryFile.GenerateUniqueFilePath("fake", ".file");

            Assert.Equal(FileNotFoundTime, storage.LastWriteTimeUtc(path));
        }

        [Fact]
        public void StorageLocation_ValidFilePathLastWriteTimeForNewlyCreatedFile_LastWriteTimeIsCurrent()
        {
            var start = DateTime.UtcNow;
            System.Threading.Thread.Sleep(1);
            using (var tempFile = new TemporaryFile(".file", createFile: true))
            {
                var storage = new StorageAccess();

                Assert.True(start <= storage.LastWriteTimeUtc(tempFile.FilePath));
            }
        }

        [Fact]
        public void StorageLocation_ValidDirectoryPathLastWriteTime_ReturnsValidTime()
        {
            var storage = new StorageAccess();

            var path = Path.GetTempPath();

            Assert.Equal(DateTime.UtcNow.Year, storage.LastWriteTimeUtc(path).Year);
        }

        [Theory]
        [InlineData("a", false)]
        [InlineData("/", true)]
        [InlineData(@"\", true)]
        [InlineData(@"\foo.zip", false)]
        [InlineData(null, true)]
        public void StorageLocation_IsLocationAContainer_ReturnsExpectedValue(string path, bool expectedIsContainer)
        {
            var storage = new StorageAccess();
            path = path ?? Path.GetTempPath();

            Assert.Equal(expectedIsContainer, storage.IsLocationAContainer(path));
        }

        [Fact]
        public void StorageLocation_IsLocationAContainerOnFreshTemporaryDirectory_IsTrue()
        {
            string path;
            using (TestResource.Directory(out path))
            {
                var storage = new StorageAccess();

                Assert.Equal(true, storage.IsLocationAContainer(path));
            }
        }
    }
}

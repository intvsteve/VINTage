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
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class StorageLocationExtensionsTests
    {
        // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
        private static readonly DateTime FileNotFoundTime = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void StorageLocation_OpenFromInvalidStorageLocation_ThrowsInvalidOperationException()
        {
            var storageLocation = StorageLocation.InvalidLocation;

            Assert.Throws<InvalidOperationException>(() => storageLocation.OpenStream());
        }

        [Fact]
        public void StorageLocation_NullstorageLocationExists_ReturnsFalse()
        {
            StorageLocation nullStorageLocation = null;

            Assert.False(nullStorageLocation.Exists());
        }

        [Fact]
        public void StorageLocation_InvalidStorageLocationExists_ReturnsFalse()
        {
            var storageLocation = StorageLocation.InvalidLocation;

            Assert.False(storageLocation.Exists());
        }

        [Fact]
        public void StorageLocation_InvalidStorageLocationSize_ReturnsNegativeOne()
        {
            var storageLocation = StorageLocation.InvalidLocation;

            Assert.Equal(-1L, storageLocation.Size());
        }

        [Fact]
        public void StorageLocation_InvalidStorageLocationLastModifcationTime_ReturnsExpectedFileNotFoundTime()
        {
            var storageLocation = StorageLocation.InvalidLocation;

            Assert.Equal(FileNotFoundTime, storageLocation.LastModificationTimeUtc());
        }

        [Fact]
        public void StorageLocation_OpenValidLocation_OpensStream()
        {
            var storageAccess = new PrivateStorageAccess();
            var testFileName = "/Open/Valid/a/b/craBapple_" + new Guid();

            using (storageAccess.OpenOrCreate(testFileName, -1))
            {
                var location = new StorageLocation(testFileName, storageAccess);

                Assert.NotNull(location.OpenStream());
            }
        }

        [Fact]
        public void StorageLocation_ValidLocationExists_ReturnsTrue()
        {
            var storageAccess = new PrivateStorageAccess();
            var testFileName = "/Location/Exists/a/b/craBapple_" + new Guid();

            using (storageAccess.OpenOrCreate(testFileName, -1))
            {
                var location = new StorageLocation(testFileName, storageAccess);

                Assert.True(location.Exists());
            }
        }

        [Fact]
        public void StorageLocation_InvalidLocationExists_ReturnsFalse()
        {
            var storageAccess = new PrivateStorageAccess();
            var testFileName = "/Invalid/Location/a/b/craBapple_" + new Guid();

            using (storageAccess.OpenOrCreate(testFileName, -1))
            {
                var location = new StorageLocation(testFileName + "/" + new Guid(), storageAccess);

                Assert.False(location.Exists());
            }
        }

        [Fact]
        public void StorageLocation_ValidLocationSize_ReturnsCorrectSize()
        {
            var storageAccess = new PrivateStorageAccess();
            var testFileName = "/Data/Size/a/b/craBapple_" + new Guid();
            var testFileSize = 0x1000;

            using (storageAccess.OpenOrCreate(testFileName, testFileSize))
            {
                var location = new StorageLocation(testFileName, storageAccess);

                Assert.Equal(testFileSize, location.Size());
            }
        }

        [Fact]
        public void StorageLocation_ValidLocationCheckModificationTime_ReturnsExpectedTime()
        {
            var storageAccess = new PrivateStorageAccess();
            var testFileName = "/Last/Modified/a/b/craBapple_" + new Guid();

            using (storageAccess.OpenOrCreate(testFileName, -1))
            {
                var lastModifed = DateTime.UtcNow;
                storageAccess.SetLastWriteTimeUtc(testFileName, lastModifed);

                var location = new StorageLocation(testFileName, storageAccess);

                Assert.Equal(lastModifed, location.LastModificationTimeUtc());
            }
        }

        private sealed class PrivateStorageAccess : TestStorageAccess
        {
        }
    }
}

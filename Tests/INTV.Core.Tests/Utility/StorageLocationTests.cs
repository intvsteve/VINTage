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

using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class StorageLocationTests
    {
        [Fact]
        public void StorageLocation_InvalidLocation_UsesInvalidStorage()
        {
            var storageLocation = StorageLocation.InvalidLocation;

            Assert.Null(storageLocation.Path);
            Assert.NotNull(storageLocation.StorageAccess);
        }

        [Fact]
        public void StorageLocation_CompareInvalidLocationToSelf_ReturnsEqual()
        {
            var invalidLocation = StorageLocation.InvalidLocation;

            Assert.True(invalidLocation.Equals(StorageLocation.InvalidLocation));
            Assert.True(StorageLocation.InvalidLocation.Equals(invalidLocation));
            Assert.True(invalidLocation == StorageLocation.InvalidLocation);
            Assert.True(StorageLocation.InvalidLocation == invalidLocation);
            Assert.False(invalidLocation != StorageLocation.InvalidLocation);
            Assert.False(StorageLocation.InvalidLocation != invalidLocation);
        }

        [Fact]
        public void StorageLocation_CompareTwoInvalidLocations_ReturnEqual()
        {
            var storageLocation0 = StorageLocation.InvalidLocation;
            var storageLocation1 = StorageLocation.InvalidLocation;

            Assert.True(storageLocation0.Equals(storageLocation1));
            Assert.True(storageLocation0.Equals(storageLocation1));
            Assert.False(storageLocation0 != storageLocation1);
            Assert.False(storageLocation1 != storageLocation0);
        }

        [Fact]
        public void StorageLocation_CompareTwoInvalidLocationsOneAsObject_ReturnEqual()
        {
            var storageLocation0 = StorageLocation.InvalidLocation;
            object storageLocation1 = StorageLocation.InvalidLocation;

            Assert.True(storageLocation0.Equals(storageLocation1));
        }

        [Fact]
        public void StorageLocation_CreateDefaultInstance_UsesDefaultStorageAccess()
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var storageLocation = new StorageLocation();

                Assert.NotNull(storageLocation.StorageAccess);
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData(null, "", false)]
        [InlineData("", null, false)]
        [InlineData("", "", true)]
        [InlineData("a", "A", true)]
        [InlineData("b", "c", false)]
        public void StorageLocation_CompareUsingDefaultStorage_ReturnsExpectedResult(string path0, string path1, bool expectedEquality)
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location0 = new StorageLocation(path0);
                var location1 = new StorageLocation(path1);

                Assert.Equal(expectedEquality, location0.Equals(location1));
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData(null, "", false)]
        [InlineData("", null, false)]
        [InlineData("", "", true)]
        [InlineData("a", "A", true)]
        [InlineData("b", "c", false)]
        public void StorageLocation_GetHashCodeOfTwoLocations_AreEqualAsExpected(string path0, string path1, bool expectedHashEquality)
        {
            var hash0 = new StorageLocation(path0 + "/").GetHashCode();
            var hash1 = new StorageLocation(path1 + "/").GetHashCode();

            Assert.Equal(expectedHashEquality, hash0 == hash1);
        }

        [Fact]
        public void StorageLocation_CompareTwoWithSamePathDifferentStorageAccess_ReturnNotEqual()
        {
            var storageAccessA = new PrivateStorageAccessA();
            var storageAccessB = new PrivateStorageAccessB();
            var path = "/a/b/c/d/foo.bar";

            var locationA = new StorageLocation(path, storageAccessA);
            var locationB = new StorageLocation(path, storageAccessB);

            Assert.False(locationA == locationB);
        }

        [Fact]
        public void StorageLocation_GetHashCodesWithSamePathDifferentStorageAccess_ReturnDifferentHashCodes()
        {
            var storageAccessA = new PrivateStorageAccessA();
            var storageAccessB = new PrivateStorageAccessB();
            var path = "/a/b/c/d/foo.bar";

            var hashA = new StorageLocation(path, storageAccessA).GetHashCode();
            var hashB = new StorageLocation(path, storageAccessB).GetHashCode();

            Assert.NotEqual(hashA, hashB);
        }

        private sealed class PrivateStorageAccessA : TestStorageAccess
        {
        }

        private sealed class PrivateStorageAccessB : TestStorageAccess
        {
        }
    }
}

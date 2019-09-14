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
            Assert.False(storageLocation.UsesDefaultStorage);
            Assert.True(storageLocation.IsInvalid);
            Assert.True(storageLocation.IsNull);
            Assert.False(storageLocation.IsEmpty);
            Assert.False(storageLocation.IsValid);
            Assert.False(storageLocation.UsesDefaultStorage);
            Assert.False(storageLocation.IsContainer);
            Assert.False(storageLocation.Exists());
            Assert.False(storageLocation.StorageAccess.Exists(null));
            Assert.False(storageLocation.StorageAccess.IsLocationAContainer(null));
        }

        [Fact]
        public void StorageLocation_CompareInvalidLocationToSelf_ReturnsEqual()
        {
            var invalidLocation = StorageLocation.InvalidLocation;

            Assert.True(invalidLocation.Equals(StorageLocation.InvalidLocation));
            Assert.True(StorageLocation.InvalidLocation.Equals(invalidLocation));
            Assert.True(invalidLocation == StorageLocation.InvalidLocation);
            Assert.True(StorageLocation.InvalidLocation == invalidLocation);
            Assert.False(StorageLocation.InvalidLocation != invalidLocation);
            Assert.False(invalidLocation != StorageLocation.InvalidLocation);
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
                Assert.True(storageLocation.UsesDefaultStorage);
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
            var hash0 = new StorageLocation(path0).GetHashCode();
            var hash1 = new StorageLocation(path1).GetHashCode();

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

        [Theory]
        [InlineData("")]
        [InlineData("a/b/c")]
        public void StorageLocation_HasNonNullPath_IsNullIsFalse(string path)
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location = new StorageLocation(path);

                Assert.False(location.IsNull);
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Theory]
        [InlineData("")]
        public void StorageLocation_HasEmptyPathViaConstant_IsEmptyIsTrue(string path)
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location = new StorageLocation(path);

                Assert.True(location.IsEmpty);
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Fact]
        public void StorageLocation_HasEmptyPathViaStringDotEmpty_IsEmptyIsTrue()
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location = new StorageLocation(string.Empty);

                Assert.True(location.IsEmpty);
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Fact]
        public void StorageLocation_HasNonEmptyPath_IsEmptyIsFalse()
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location = new StorageLocation("biff");

                Assert.False(location.IsEmpty);
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(@"/", true)]
        [InlineData(@"\", true)]
        [InlineData(@"/a", false)]
        [InlineData(@"\a", false)]
        [InlineData(@"b/", true)]
        [InlineData(@"b\", true)]
        [InlineData(@" c/ ", false)]
        [InlineData(@" c\ ", false)]
        public void StorageLocation_WithPathCheckIsContainer_ReturnsExpectedValue(string path, bool expectedIsContainer)
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location = new StorageLocation(path);

                Assert.Equal(expectedIsContainer, location.IsContainer);
                Assert.Equal(expectedIsContainer, location.IsContainer); // second call covers cached result
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Fact]
        public void StorageLocation_CopyWithNewPath_RetainsStorageAccess()
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location0Path = "Something/wicked";
                var location0 = new StorageLocation(location0Path, storageAccess);
                var location1Path = "Quoth/the/raven/Nevermore";
                var location1 = StorageLocation.CopyWithNewPath(location0, location1Path);

                Assert.True(object.ReferenceEquals(location0.StorageAccess, location1.StorageAccess));
                Assert.Equal(location0Path, location0.Path);
                Assert.Equal(location1Path, location1.Path);
                Assert.NotEqual(location0.Path, location1.Path);
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        [Fact]
        public void StorageLocation_NullStorageLocation_HasNullPathIsNotValidIsNotInvalidIsNull()
        {
            var nullLocation = StorageLocation.Null;

            Assert.Null(nullLocation.Path);
            Assert.False(nullLocation.IsValid);
            Assert.False(nullLocation.IsInvalid);
            Assert.True(nullLocation.IsNull);
            Assert.False(nullLocation.IsEmpty);
            Assert.False(nullLocation.IsContainer);
        }

        [Fact]
        public void StorageLocation_EmptyStorageLocation_HasExpectedValues()
        {
            var emptyLocation = StorageLocation.Empty;

            Assert.NotNull(emptyLocation.Path);
            Assert.False(emptyLocation.IsValid);
            Assert.False(emptyLocation.IsInvalid);
            Assert.False(emptyLocation.IsNull);
            Assert.True(emptyLocation.IsEmpty);
            Assert.False(emptyLocation.IsContainer);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        public void StorageLocation_ToString_ReturnsPath(string path)
        {
            var storageAccess = new PrivateStorageAccessA();
            try
            {
                Assert.True(IStorageAccessHelpers.Initialize(storageAccess));

                var location = new StorageLocation(path);

                Assert.Equal(path, location.ToString());
            }
            finally
            {
                Assert.True(IStorageAccessHelpers.Remove(storageAccess));
            }
        }

        private sealed class PrivateStorageAccessA : TestStorageAccess
        {
        }

        private sealed class PrivateStorageAccessB : TestStorageAccess
        {
        }
    }
}

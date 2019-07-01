// <copyright file="IStorageAccessHelpersTests.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class IStorageAccessHelpersTests
    {
        [Fact]
        public void IStorageAccess_RegisterNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => IStorageAccessHelpers.Initialize(null));
        }

        [Fact]
        public void IStorageAccess_RemoveNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => IStorageAccessHelpers.Remove(null));
        }

        [Fact]
        public void IStorageAccess_InitializeCheckDefaultAndRemoveStorage_Succeeds()
        {
            var storageAcces = new MyPrivateStorageAccess();

            // We use a privately defined type for the storage access to check initialize and remove, which will
            // also guarantee that there is a default storage access at least for the duration of this test.
            Assert.True(IStorageAccessHelpers.Initialize(storageAcces));
            Assert.False(IStorageAccessHelpers.Initialize(storageAcces));
            Assert.NotNull(IStorageAccessHelpers.DefaultStorageAccess);
            Assert.True(IStorageAccessHelpers.Remove(storageAcces));
            Assert.False(IStorageAccessHelpers.Remove(storageAcces));
        }

        private sealed class MyPrivateStorageAccess : TestStorageAccess
        {
        }
    }
}

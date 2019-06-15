// <copyright file="TemporaryDirectoryTests.cs" company="INTV Funhouse">
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
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class TemporaryDirectoryTests
    {
        [Fact]
        public void TemporaryDirectory_Create_EnsureDeleted()
        {
            string directoryPath;
            using (var temporaryDirectory = new TemporaryDirectory())
            {
                directoryPath = temporaryDirectory.Path;
                Assert.True(Directory.Exists(directoryPath));
            }
            Assert.False(Directory.Exists(directoryPath));
        }

        [Fact]
        public void TemporaryDirectory_CreateThenForceFinalizerToRun_EnsureDeleted()
        {
            string directoryPath = UseTemporaryDirectoryWihoutDisposing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.False(Directory.Exists(directoryPath));
        }

        private static string UseTemporaryDirectoryWihoutDisposing()
        {
            var temporaryDirectory = new TemporaryDirectory();
            var directoryPath = temporaryDirectory.Path;
            return directoryPath;
        }
    }
}

// <copyright file="StreamUtilitiesTests.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class StreamUtilitiesTests
    {
        // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
        private static readonly DateTime FileNotFoundTime = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void StreamUtilities_RegisterNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => StreamUtilities.Initialize(null));
        }

        [Fact]
        public void StreamUtilitiesWithTestStorage_CallOpenFileStream_ReturnsValidStream()
        {
            var storage = new TestStorageAccess();
            var testPath = @"~/open_create_path.dat";
            using (var stream = StreamUtilities.OpenFileStream(testPath, storage))
            {
                Assert.True(StreamUtilities.FileExists(testPath, storage));
                Assert.NotNull(stream);
            }
            Assert.False(StreamUtilities.FileExists(testPath, storage));
        }

        [Fact]
        public void StreamUtilitiesWithTestStorage_CallFileExistsWithNonexistentPath_ReturnsFalse()
        {
            var storage = new TestStorageAccess();
            Assert.False(StreamUtilities.FileExists(@"SomeInvalidPathThatDoesNotExist", storage));
        }

        [Fact]
        public void StreamUtilitiesWithTestStorage_CallSizeWithNonexistentPath_ThrowsFileNotFoundException()
        {
            var storage = new TestStorageAccess();
            Assert.Throws<FileNotFoundException>(() => StreamUtilities.FileSize(@"SomeBogusPathThatHasNoSize", storage));
        }

        [Fact]
        public void StreamUtilitiesWithTestStorage_CallLastWriteTimeUtcWithNonexistentPath_ReturnsDefaultDateTime()
        {
            var storage = new TestStorageAccess();
            Assert.Equal(FileNotFoundTime, StreamUtilities.LastFileWriteTimeUtc(@"SomeSillyPathWithNoLastWriteTime", storage));
        }

        [Fact]
        public void StreamUtilitiesWithTestStorage_CallSizeAfterCreatingSizedStream_ReturnsExpectedSize()
        {
            var storage = new TestStorageAccess();
            var testPath = @"~/.test_file.dat";
            var testSize = 64;
            using (var testStream = TestStorageAccess.OpenOrCreate(testPath, testSize))
            {
                Assert.Equal(testSize, StreamUtilities.FileSize(testPath, storage));
            }
        }

        [Fact]
        public void StreamUtilitiesWithTestStorage_CallLastWriteTimeUtcAfterWritingToStream_ReturnsReasonableLastWriteTime()
        {
            var storage = new TestStorageAccess();
            var testPath = @"~/test_file_to_write.dat";
            using (var stream = StreamUtilities.OpenFileStream(testPath, storage))
            {
                var beforeWrite = DateTime.UtcNow;
                int numBytesToWrite = 128;
                for (byte i = 0; i < numBytesToWrite; ++i)
                {
                    stream.WriteByte(i);
                }
                Assert.Equal(numBytesToWrite, StreamUtilities.FileSize(testPath, storage));
                var afterWrite = DateTime.UtcNow;
                var lastWrite = StreamUtilities.LastFileWriteTimeUtc(testPath, storage);
                Assert.True(lastWrite >= beforeWrite);
                Assert.True(lastWrite <= afterWrite);
            }
        }
    }
}

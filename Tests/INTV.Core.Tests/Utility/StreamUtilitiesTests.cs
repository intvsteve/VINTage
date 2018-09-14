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
        public void StreamUtilitiesNotInitialized_CallOpenFileStream_ThrowsNullReferenceException()
        {
            Assert.True(StreamUtilities.Initialize(null));
            Assert.Throws<NullReferenceException>(() => StreamUtilities.OpenFileStream(@"BogusOpenPath"));
        }

        [Fact]
        public void StreamUtilitiesNotInitialized_CallFileExists_ThrowsNullReferenceException()
        {
            Assert.True(StreamUtilities.Initialize(null));
            Assert.Throws<NullReferenceException>(() => StreamUtilities.FileExists(@"BogusExistsPath"));
        }

        [Fact]
        public void StreamUtilitiesNotInitialized_CallSize_ThrowsNullReferenceException()
        {
            Assert.True(StreamUtilities.Initialize(null));
            Assert.Throws<NullReferenceException>(() => StreamUtilities.Size(@"BogusSizePath"));
        }

        [Fact]
        public void StreamUtilitiesNotInitialized_CallLastWriteTimeUtc_ThrowsNullReferenceException()
        {
            Assert.True(StreamUtilities.Initialize(null));
            Assert.Throws<NullReferenceException>(() => StreamUtilities.LastFileWriteTimeUtc(@"BogusLastWritePath"));
        }

        [Fact]
        public void StreamUtilitiesInitialized_CallOpenFileStream_ReturnsValidStream()
        {
            Assert.True(StreamUtilities.Initialize(new TestStorageAccess()));
            var testPath = @"~/open_create_path.dat";
            using (var stream = StreamUtilities.OpenFileStream(testPath))
            {
                Assert.True(StreamUtilities.FileExists(testPath));
                Assert.NotNull(stream);
            }
            Assert.False(StreamUtilities.FileExists(testPath));
        }

        [Fact]
        public void StreamUtilitiesInitialized_CallFileExistsWithNonexistentPath_ReturnsFalse()
        {
            Assert.True(StreamUtilities.Initialize(new TestStorageAccess()));
            Assert.False(StreamUtilities.FileExists(@"SomeInvalidPathThatDoesNotExist"));
        }

        [Fact]
        public void StreamUtilitiesInitialized_CallSizeWithNonexistentPath_ThrowsFileNotFoundException()
        {
            Assert.True(StreamUtilities.Initialize(new TestStorageAccess()));
            Assert.Throws<FileNotFoundException>(() => StreamUtilities.Size(@"SomeBogusPathThatHasNoSize"));
        }

        [Fact]
        public void StreamUtilitiesInitialized_CallLastWriteTimeUtcWithNonexistentPath_ReturnsDefaultDateTime()
        {
            Assert.True(StreamUtilities.Initialize(new TestStorageAccess()));
            Assert.Equal(FileNotFoundTime, StreamUtilities.LastFileWriteTimeUtc(@"SomeSillyPathWithNoLastWriteTime"));
        }

        [Fact]
        public void StreamUtilitiesInitialized_CallSizeAfterCreatingSizedStream_ReturnsExpectedSize()
        {
            Assert.True(StreamUtilities.Initialize(new TestStorageAccess()));
            var testPath = @"~/.test_file.dat";
            var testSize = 64;
            using (var testStream = TestStorageAccess.OpenOrCreate(testPath, testSize))
            {
                Assert.Equal(testSize, StreamUtilities.Size(testPath));
            }
        }

        [Fact]
        public void StreamUtilitiesInitialized_CallLastWriteTimeUtcAfterWritingToStream_ReturnsReasonableLastWriteTime()
        {
            Assert.True(StreamUtilities.Initialize(new TestStorageAccess()));
            var testPath = @"~/test_file_to_write.dat";
            using (var stream = StreamUtilities.OpenFileStream(testPath))
            {
                var beforeWrite = DateTime.UtcNow;
                int numBytesToWrite = 128;
                for (byte i = 0; i < numBytesToWrite; ++i)
                {
                    stream.WriteByte(i);
                }
                Assert.Equal(numBytesToWrite, StreamUtilities.Size(testPath));
                var afterWrite = DateTime.UtcNow;
                var lastWrite = StreamUtilities.LastFileWriteTimeUtc(testPath);
                Assert.True(lastWrite >= beforeWrite);
                Assert.True(lastWrite <= afterWrite);
            }
        }
    }
}

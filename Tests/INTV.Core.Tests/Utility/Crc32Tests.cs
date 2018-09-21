// <copyright file="Crc32Tests.cs" company="INTV Funhouse">
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

using System.IO;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class Crc32Tests
    {
        [Fact]
        public void Crc32_OfNull_ThrowsNullReferenceException()
        {
            Assert.Throws<System.NullReferenceException>(() => Crc32.OfStream(null));
        }

        [Fact]
        public void Crc32_OfBlock_IsCorrect()
        {
            var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var crc32 = Crc32.OfBlock(testData);
            Assert.Equal(0x6035A035u, crc32);
        }

        [Fact]
        public void Crc32_OfBlockWithIgnoreRange_IsCorrect()
        {
            var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var ignoreRanges = new[] { new Range<int>(0, 1) };
            var crc32 = Crc32.OfBlock(testData, testData.Length, ignoreRanges, Crc32.InitialValue);
            Assert.Equal(0x5EAF6107u, crc32);
        }

        [Fact]
        public void Crc32_OfStream_IsCorrect()
        {
            using (var testData = new MemoryStream(new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }))
            {
                var crc32 = Crc32.OfStream(testData);
                Assert.Equal(0x6035A035u, crc32);
            }
        }

        [Fact]
        public void Crc32_OfStreamWithIgnoreRange_IsCorrect()
        {
            using (var testData = new MemoryStream(new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }))
            {
                var ignoreRanges = new[] { new Range<int>(1, 2) };
                var crc32 = Crc32.OfStream(testData, ignoreRanges);
                Assert.Equal(0xB4DA8CCAu, crc32);
            }
        }

        [Fact]
        public void Crc32_OfFile_IsCorrect()
        {
            // We use a privately defined type for the storage access to check initialize and remove, which will
            // hopefully guarantee that we use the expected storage during this test.
            var storageAcces = new Crc32TestStorageAccess();
            StreamUtilities.Initialize(storageAcces);
            var testFileName = "~/Crc32_OfFile_IsCorrect.dat";
            using (var fileStream = StreamUtilities.OpenFileStream(testFileName))
            {
                var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
                fileStream.Write(testData, 0, testData.Length);
                var crc32 = Crc32.OfFile(testFileName);
                Assert.Equal(0x6035A035u, crc32);
            }
        }

        [Fact]
        public void Crc32_OfFileWithIgnoreRange_IsCorrect()
        {
            // We use a privately defined type for the storage access to check initialize and remove, which will
            // hopefully guarantee that we use the expected storage during this test.
            var storageAcces = new Crc32TestStorageAccess();
            StreamUtilities.Initialize(storageAcces);
            var testFileName = "~/Crc32_OfFileWithIgnoreRange_IsCorrect.dat";
            using (var fileStream = StreamUtilities.OpenFileStream(testFileName))
            {
                var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
                fileStream.Write(testData, 0, testData.Length);
                var ignoreRanges = new[] { new Range<int>(1, 2) };
                var crc32 = Crc32.OfFile(testFileName, ignoreRanges);
                Assert.Equal(0xB4DA8CCAu, crc32);
            }
        }

        [Fact]
        public void Crc32_OfFileWithAlternateFirstByte_IsCorrect()
        {
            // We use a privately defined type for the storage access to check initialize and remove, which will
            // hopefully guarantee that we use the expected storage during this test.
            var storageAcces = new Crc32TestStorageAccess();
            var testFileName = "~/Crc32_OfFileWithAlternateFirstByte_IsCorrect.dat";
            StreamUtilities.Initialize(storageAcces);
            using (var fileStream = StreamUtilities.OpenFileStream(testFileName))
            {
                var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
                fileStream.Write(testData, 0, testData.Length);
                var crc32 = Crc32.OfFile(testFileName, replaceFirstByte: true, alternateFirstByte: 0x42);
                Assert.Equal(0x066F5C62u, crc32);
            }
        }

        private class Crc32TestStorageAccess : TestStorageAccess
        {
        }
    }
}

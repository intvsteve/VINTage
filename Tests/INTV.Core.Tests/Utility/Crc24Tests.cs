// <copyright file="Crc24Tests.cs" company="INTV Funhouse">
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
    public class Crc24Tests
    {
        [Fact]
        public void Crc24_OfNullStream_ThrowsNullReferenceException()
        {
            Assert.Throws<System.NullReferenceException>(() => Crc24.OfStream(null, 0));
        }

        [Fact]
        public void Crc24_OfStreamWithInitialValue_IsCorrect()
        {
            using (var testData = new MemoryStream(new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }))
            {
                testData.Seek(0, SeekOrigin.Begin);
                var crc24 = Crc24.OfStream(testData, Crc24.InitialValue);
                Assert.Equal(0x004F40DAu, crc24);
            }
        }

        [Fact]
        public void Crc24_OfNullFile_ThrowsArgumentNullException()
        {
            var storageAcces = new Crc24TestStorageAccess();
            StreamUtilities.Initialize(storageAcces);
            Assert.Throws<System.ArgumentNullException>(() => Crc24.OfFile(null));
        }

        [Fact]
        public void Crc24_OfFile_IsCorrect()
        {
            // We use a privately defined type for the storage access to check initialize and remove, which will
            // hopefully guarantee that we use the expected storage during this test.
            var storageAcces = new Crc24TestStorageAccess();
            StreamUtilities.Initialize(storageAcces);
            var testFileName = "~/Crc24_OfFile_IsCorrect.dat";
            using (var fileStream = StreamUtilities.OpenFileStream(testFileName))
            {
                var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
                fileStream.Write(testData, 0, testData.Length);
                var crc24 = Crc24.OfFile(testFileName);
                Assert.Equal(0x004F40DAu, crc24);
            }
        }

        private class Crc24TestStorageAccess : TestStorageAccess
        {
        }
    }
}

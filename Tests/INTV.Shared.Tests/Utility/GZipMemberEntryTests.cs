// <copyright file="GZipMemberEntryTests.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using INTV.Shared.Utility;
using INTV.TestHelpers.Core.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class GZipMemberEntryTests
    {
        private const int MinimumEntryDeserializeLength = 10;
        private const int CorruptMagicOffset = 1;
        private const int CorruptCompressionMethodOffset = 2;
        private const int CorruptReservedBitsOffset = 3;

        private static readonly string StockEntryName = "file.dat"; // copied from GZipMemberEntry

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GZipMemberEntry_CreateEmptyEntryWithBadName_ThrowsArgumentException(string badName)
        {
            Assert.Throws<ArgumentException>(() => GZipMemberEntry.CreateEmptyEntry(badName));
        }

        [Fact]
        public void GZipMemberEntry_CreateEmpty_CreatesEntryWithGivenName()
        {
            var name = "whee";

            var entry = GZipMemberEntry.CreateEmptyEntry(name);

            Assert.NotNull(entry);
            Assert.Equal(name, entry.Name);
        }

        [Fact]
        public void GZipMemberEntry_GetAllMemberEntriesWithNullStream_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => GZipMemberEntry.GetMemberEntries(null));
        }

        [Fact]
        public void GZipMemberEntry_Serialize_ThrowsNotSupportedException()
        {
            var bogusEntry = new GZipMemberEntry();
            INTV.Core.Utility.BinaryWriter bogusWriter = null;

            Assert.Throws<NotSupportedException>(() => bogusEntry.Serialize(bogusWriter));
        }

        [Fact]
        public void GZipMemberEntry_InflateUsingBinaryReader_ReturnsValidEntry()
        {
            var binGZip = TestResource.TagalongBinGZip;

            using (var reader = new INTV.Core.Utility.BinaryReader(binGZip.OpenResourceForReading()))
            {
                var entry = GZipMemberEntry.Inflate(reader);

                VerifyGZipMemberEntry(entry, binGZip.ArchiveContents.First(), 0, expectedLength: null);
            }
        }

        public static IEnumerable<object[]> GetAllMemberEntriesFromGZipContainingFileNamesTestData
        {
            get
            {
                yield return new object[] { TestResource.TagalongBinGZip, 1024L, TestRomResources.TestBinCrc };
                yield return new object[] { TestResource.TagalongCfgGZip, 34L, TestRomResources.TestCfgCrc };
            }
        }

        [Theory]
        [MemberData("GetAllMemberEntriesFromGZipContainingFileNamesTestData")]
        public void GZipMemberEntry_GetAllMemberEntriesFromGZipContainingFileNames_GetsMemberExpectedEntryData(TestResource testResource, long expectedLength, uint expectedCrc32)
        {
            using (var stream = testResource.OpenResourceForReading())
            {
                var entry = GZipMemberEntry.GetMemberEntries(stream).Single();

                VerifyGZipMemberEntry(entry, testResource.ArchiveContents.First(), expectedCrc32, expectedLength, GZipOS.Unix);
            }
        }

        public static IEnumerable<object[]> GetAllMemberEntriesFromStreamContainingTwoConcatenatedGZipsTestData
        {
            get
            {
                yield return new object[] { TestResource.TagalongBinCfgNNGZip, new[] { new VerificationData(StockEntryName, TestRomResources.TestBinCrc, 1024), new VerificationData(MakeStockEntryName(1), TestRomResources.TestCfgCrc, 34) } };
                yield return new object[] { TestResource.TagalongBinCfgNYGZip, new[] { new VerificationData(StockEntryName, TestRomResources.TestBinCrc, 1024), new VerificationData("tagalong.cfg", TestRomResources.TestCfgCrc, 34) } };
                yield return new object[] { TestResource.TagalongBinCfgYNGZip, new[] { new VerificationData("tagalong.bin", TestRomResources.TestBinCrc, 1024), new VerificationData(StockEntryName, TestRomResources.TestCfgCrc, 34) } };
                yield return new object[] { TestResource.TagalongBinCfgYYGZip, new[] { new VerificationData("tagalong.bin", TestRomResources.TestBinCrc, 1024), new VerificationData("tagalong.cfg", TestRomResources.TestCfgCrcUnix, 32) } };
            }
        }

        [Theory]
        [MemberData("GetAllMemberEntriesFromStreamContainingTwoConcatenatedGZipsTestData")]
        public void GZipMemberEntry_GetAllMemberEntriesFromStreamContainingTwoConcatenatedGZips_GetsMembersWithExpectedEntryNames(TestResource testResource, IEnumerable<VerificationData> verificationData)
        {
            using (var stream = testResource.OpenResourceForReading())
            {
                var entries = GZipMemberEntry.GetMemberEntries(stream).ToList();
                var verifyEntriesData = verificationData.ToList();

                for (var i = 0; i < entries.Count; ++i)
                {
                    var verifyData = verifyEntriesData[i];
                    VerifyGZipMemberEntry(entries[i], verifyData.ExpectedName, verifyData.ExpectedCrc32, verifyData.ExpectedLength, checkModificationDate: verifyData.CheckLastModificationTime);
                }
            }
        }

        [Fact]
        public void GZipMemberEntry_GetAllMemberEntriesFromStreamContainingConcatenatedGZipsWithMaxLimitZero_GetsZeroEntries()
        {
            using (var stream = TestResource.TagalongBCLRNYNYGZip.OpenResourceForReading())
            {
                var entries = GZipMemberEntry.GetMemberEntries(stream, maxNumberOfEntries: 0);

                Assert.False(entries.Any());
            }
        }

        [Fact]
        public void GZipMemberEntry_GetAllMemberEntriesFromStreamContainingFourConcatenatedGZipsWithMaxLimitThree_GetsThreeEntries()
        {
            using (var stream = TestResource.TagalongBCLRYNNYGZip.OpenResourceForReading())
            {
                var entries = GZipMemberEntry.GetMemberEntries(stream, maxNumberOfEntries: 3).ToList();

                Assert.Equal(3, entries.Count);
            }
        }

        [Fact]
        public void GZipMemberEntry_GetAllMemberEntriesFromStreamContainingTwoConcatenatedGZipsWithMaxLimitThree_GetsTwoEntries()
        {
            using (var stream = TestResource.TagalongBinCfgNYGZip.OpenResourceForReading())
            {
                var entries = GZipMemberEntry.GetMemberEntries(stream, maxNumberOfEntries: 3).ToList();

                Assert.Equal(2, entries.Count);
            }
        }

        [Fact]
        public void GZipMemberEntry_GetAllMemberEntriesFromStreamContainingOneGZipWithMaxLimitFour_GetsOneEntry()
        {
            using (var stream = TestResource.TagalongCfgGZip.OpenResourceForReading())
            {
                var entries = GZipMemberEntry.GetMemberEntries(stream, maxNumberOfEntries: 4).ToList();

                Assert.Equal(1, entries.Count);
            }
        }

        public static IEnumerable<object[]> GetAllMemberEntriesFromFileTestData
        {
            get
            {
                yield return new object[] { TestResource.TagalongBCLRNNNNGZip };
                yield return new object[] { TestResource.TagalongBCLRNNYNGZip };
                yield return new object[] { TestResource.TagalongBCLRNYNNGZip };
                yield return new object[] { TestResource.TagalongBCLRNYNYGZip };
                yield return new object[] { TestResource.TagalongBCLRNYYNGZip };
                yield return new object[] { TestResource.TagalongBCLRYNNYGZip };
                yield return new object[] { TestResource.TagalongBCLRYNYNGZip };
                yield return new object[] { TestResource.TagalongBCLRYYYYGZip };
            }
        }

        [Theory]
        [MemberData("GetAllMemberEntriesFromFileTestData")]
        public void GZipMemberEntry_GetAllMemberEntriesFromFile_GetsMembersWithExpectedEntryNames(TestResource testResource)
        {
            var testResourcePath = ExtractTestResourceToTemporaryFile(testResource);
            try
            {
                IEnumerable<GZipMemberEntry> entries = null;
                using (var stream = new FileStream(testResourcePath, FileMode.Open, FileAccess.Read))
                {
                    entries = GZipMemberEntry.GetMemberEntries(stream);
                }

                // .cfg file CRCs are zero because they are inconsistent based on line endings.
                // .luigi file CRCs are zero merely because they haven't been computed :P
                var expectedCrc32s = new[] { TestRomResources.TestBinCrc, 0u, 0u, TestRomResources.TestRomCrc };
                var expectedNames = testResource.ArchiveContents.ToList();
                var i = 0;
                foreach (var entry in entries)
                {
                    // Expected length is loosely checked -- any value indicates a > 0 check.
                    VerifyGZipMemberEntry(entry, expectedNames[i], expectedCrc32s[i], expectedLength: 2, checkModificationDate: false, checkOffset: i > 0);
                    ++i;
                }
            }
            finally
            {
                var testTempDirectory = Path.GetDirectoryName(testResourcePath);
                if (testResourcePath.StartsWith(Path.GetTempPath()) && new DirectoryInfo(testTempDirectory).Exists)
                {
                    try
                    {
                        Directory.Delete(testTempDirectory, recursive: true);
                    }
                    catch
                    {
                        // trash left behind in temp directory
                    }
                }
            }
        }

        [Fact]
        public void GZipMemberEntry_GetAllMemberEntriesFromBadStream_OnlyReturnsSomeEntries()
        {
            var testResourcePath = ExtractTestResourceToTemporaryFile(TestResource.TagalongBinCfgYYGZip);
            try
            {
                using (var stream = new TestFileStream(testResourcePath, FileMode.Open, FileAccess.Read))
                {
                    var entries = GZipMemberEntry.GetMemberEntries(stream);

                    Assert.True(entries.Count() < 2);
                }
            }
            finally
            {
                var testTempDirectory = Path.GetDirectoryName(testResourcePath);
                if (testResourcePath.StartsWith(Path.GetTempPath()) && new DirectoryInfo(testTempDirectory).Exists)
                {
                    try
                    {
                        Directory.Delete(testTempDirectory, recursive: true);
                    }
                    catch
                    {
                        // trash left behind in temp dir
                    }
                }
            }
        }

        [Fact]
        public void GZipMemberEntry_InflateStreamWithCorruptMagicKey_ThrowsInvalidOperationException()
        {
            using (var stream = CreateCorruptedGZipStream(TestResource.TagalongCfgGZip, CorruptMagicOffset))
            {
                Assert.Throws<InvalidOperationException>(() => GZipMemberEntry.Inflate(stream));
            }
        }

        [Fact]
        public void GZipMemberEntry_InflateStreamWithCorruptCompressionMethod_ThrowsInvalidOperationException()
        {
            using (var stream = CreateCorruptedGZipStream(TestResource.TagalongCfgGZip, CorruptCompressionMethodOffset))
            {
                Assert.Throws<InvalidOperationException>(() => GZipMemberEntry.Inflate(stream));
            }
        }

        [Fact]
        public void GZipMemberEntry_InflateStreamWithCorruptFlags_ThrowsInvalidOperationException()
        {
            using (var stream = CreateCorruptedGZipStream(TestResource.TagalongCfgGZip, CorruptReservedBitsOffset))
            {
                Assert.Throws<InvalidOperationException>(() => GZipMemberEntry.Inflate(stream));
            }
        }

        private static void VerifyGZipMemberEntry(GZipMemberEntry entry, string expectedName, uint? expectedCrc32, long? expectedLength, GZipOS? expectedOperatingSystem = null, bool checkModificationDate = true, bool checkOffset = false)
        {
            Assert.NotNull(entry);
            Assert.False(entry.IsDirectory);
            Assert.Equal(expectedName, entry.Name);
            if (checkModificationDate)
            {
                Assert.NotEqual(DateTime.MinValue, entry.LastModificationTime);
            }
            if (expectedOperatingSystem.HasValue)
            {
                Assert.Equal(expectedOperatingSystem, entry.OperatingSystem);
            }
            if (expectedLength.HasValue)
            {
                Assert.True(entry.Length > 0);
            }
            else
            {
                Assert.Equal(-1, entry.Length);
            }
            if (expectedCrc32.HasValue && (expectedCrc32.Value != 0))
            {
                Assert.Equal(expectedCrc32.Value, entry.Crc32);
            }
            if (checkOffset)
            {
                Assert.True(entry.Offset > 0);
            }
            Assert.Equal(-1, entry.SerializeByteCount);
            Assert.True(entry.DeserializeByteCount >= MinimumEntryDeserializeLength);
        }

        private static Stream CreateCorruptedGZipStream(TestResource testResource, int offsetToCorrupt)
        {
            var stream = new MemoryStream();
            using (var resourceStream = testResource.OpenResourceForReading())
            {
                resourceStream.CopyTo(stream);
            }
            stream.Seek(offsetToCorrupt, SeekOrigin.Begin);
            stream.WriteByte(0xFF);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private static string ExtractTestResourceToTemporaryFile(TestResource testResource)
        {
            var subdirectoryName = "INTV_Test_TempDir_" + Guid.NewGuid();
            var tempFileDirectory = Path.Combine(Path.GetTempPath(), subdirectoryName);
            var resourceFileName = testResource.Name.Substring(TestResource.ResourcePrefix.Length);
            var tempFilePath = Path.Combine(tempFileDirectory, resourceFileName);

            if (Directory.CreateDirectory(tempFileDirectory).Exists)
            {
                using (var resourceStream = testResource.OpenResourceForReading())
                using (var fileStream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            return tempFilePath;
        }

        private static string MakeStockEntryName(int index)
        {
            var baseFileName = Path.GetFileNameWithoutExtension(StockEntryName);
            var originalFileExtension = Path.GetExtension(StockEntryName);
            var name = string.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", baseFileName, index, originalFileExtension);
            return name;
        }

        public class VerificationData
        {
            public VerificationData(string expectedName, uint expectedCrc32, long expectedLength)
            {
                ExpectedName = expectedName;
                ExpectedCrc32 = expectedCrc32;
                ExpectedLength = expectedLength;
                var baseFileName = Path.GetFileNameWithoutExtension(StockEntryName);
                CheckLastModificationTime = !expectedName.StartsWith(baseFileName);
            }

            public string ExpectedName { get; private set; }
            public uint ExpectedCrc32 { get; private set; }
            public long ExpectedLength { get; private set; }
            public bool CheckLastModificationTime { get; private set; }
        }

        private class TestFileStream : FileStream
        {
            private int _numRead = 0;

            public TestFileStream(string path, FileMode mode, FileAccess access)
                : base(path, mode, access)
            {
            }

            public override int ReadByte()
            {
                if (++_numRead > 20)
                {
                    throw new IOException();
                }
                return base.ReadByte();
            }
        }
    }
}

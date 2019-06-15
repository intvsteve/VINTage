// <copyright file="GZipNativeAccessTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Utility;
using INTV.Shared.Utility;
using INTV.TestHelpers.Core.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class GZipNativeAccessTests
    {
        [Fact]
        public void GZipNativeAccess_OpenNonGZip_ThrowsInvalidDataException()
        {
            var nonGZipResource = TestResource.TextEmbeddedResourceFile;

            using (var stream = nonGZipResource.OpenResourceForReading())
            {
                Assert.Throws<InvalidDataException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read));
            }
        }

        [Fact]
        public void GZipNativeAccess_OpenWithInvalidMode_ThrowsArgumentOutOfRangeException()
        {
            var gzipResource = TestResource.TagalongBinGZip;

            using (var stream = gzipResource.OpenResourceForReading())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, (CompressedArchiveAccessMode)100));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessMode.Create)]
        [InlineData(CompressedArchiveAccessMode.Update)]
        public void GZipNativeAccess_OpenNonEmptyGZipForModification_ThrowsInvalidOperationException(CompressedArchiveAccessMode mode)
        {
            var gzipResource = TestResource.TagalongBinGZip;

            using (var stream = gzipResource.OpenResourceForReading())
            {
                Assert.Throws<InvalidOperationException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, mode));
            }
        }

        [Fact]
        public void GZipNativeAccess_DeleteEntry_ThrowsNotSupportedException()
        {
            var gzipResource = TestResource.TagalongCfgGZip;
            var entryName = gzipResource.ArchiveContents.First();

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read))
            {
                Assert.Throws<NotSupportedException>(() => gzip.DeleteEntry(entryName));
            }
        }

        [Fact]
        public void GZipNativeAccess_CreateEntryWhenOpenInDecompressMode_ThrowsInvalidOperationException()
        {
            var gzipResource = TestResource.TagalongCfgGZip;

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read))
            {
                Assert.Throws<InvalidOperationException>(() => gzip.CreateEntry("derp"));
            }
        }

        [Fact]
        public void GZipNativeAccess_OpenSingleMemberFileWithEntryName_HasExpectedContents()
        {
            var gzipResource = TestResource.TagalongBinGZip;

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read))
            {
                var entry = gzip.Entries.Single();
                var crc = 0u;
                using (var entryStream = gzip.Open(entry.Name))
                {
                    crc = Crc32.OfStream(entryStream, fromStartOfStream: false);
                }

                Assert.Equal(gzipResource.ArchiveContents.First(), entry.Name);
                Assert.Equal(TestRomResources.TestBinCrc, crc);
            }
        }

        [Fact]
        public void GZipNativeAccess_OpenSecondMemberEntry_HasExpectedContents()
        {
            var gzipResource = TestResource.TagalongBinCfgYYGZip;

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read))
            {
                var entry = gzip.Entries.Last();
                var crc = 0u;
                using (var entryStream = gzip.OpenEntry(entry))
                {
                    crc = Crc32.OfStream(entryStream, fromStartOfStream: false);
                }

                Assert.Equal(gzipResource.ArchiveContents.Last(), entry.Name);
                Assert.Equal(TestRomResources.TestCfgCrcUnix, crc);
            }
        }

        [Fact]
        public void GZipNativeAccess_IsArchive_IsFalse()
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create))
            {
                Assert.False(gzip.IsArchive);
            }
        }

        [Fact]
        public void GZipNativeAccess_IsCompressed_IsTrue()
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create))
            {
                Assert.True(gzip.IsCompressed);
            }
        }

        [Fact]
        public void GZipNativeAccess_CreateEntryInEmptyStream_Succeeds()
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create))
            {
                Assert.NotNull(gzip.CreateEntry("newEntry"));
            }
        }

        [Fact]
        public void GZipNativeAccess_CreateSecondEntry_ThrowsNotSupportedException()
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create))
            {
                gzip.CreateEntry("newEntry");

                Assert.Throws<NotSupportedException>(() => gzip.CreateEntry("newEntry"));
            }
        }

        [Fact]
        public void GZipNativeAccess_OpenFromDisk_HasExpectedContent()
        {
            var testResourcePath = ExtractTestResourceToTemporaryFile(TestResource.TagalongBinCfgNNGZip);
            try
            {
                using (var gzip = CompressedArchiveAccess.Open(testResourcePath, CompressedArchiveAccessMode.Read))
                {
                    var expectedCrc32s = new[] { TestRomResources.TestBinCrc, TestRomResources.TestCfgCrc };
                    var i = 0;
                    foreach (var entry in gzip.Entries)
                    {
                        using (var entryStream = gzip.OpenEntry(entry))
                        {
                            var crc = Crc32.OfStream(entryStream, fromStartOfStream: false);

                            Assert.Equal(expectedCrc32s[i], crc);
                        }
                        ++i;
                    }
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
        public void GZipNativeAccess_WriteRomResourceToGZip_ProducesExpectedResult()
        {
            var inputCrc = 0u;
            var inputLength = 0L;

            // Create in-memory GZIP
            var newMemoryStream = new MemoryStream();
            var copyMemoryStream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(newMemoryStream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create))
            {
                var testResourceName = "INTV.TestHelpers.Core.Resources.tagalong.luigi";
                var newGZipEntryName = "tagalong.luigi";
                var entry = gzip.CreateEntry(newGZipEntryName);
                using (var gzipStream = gzip.Open(entry.Name))
                using (var sourceStream = typeof(TestRomResources).Assembly.GetManifestResourceStream(testResourceName))
                {
                    sourceStream.CopyTo(gzipStream);
                    inputCrc = Crc32.OfStream(sourceStream);
                    inputLength = sourceStream.Length;
                }

                // Now, rewind and see if we can extract it!
                newMemoryStream.Seek(0, SeekOrigin.Begin);
                newMemoryStream.CopyTo(copyMemoryStream);
            }

            using (var gzip = CompressedArchiveAccess.Open(copyMemoryStream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read))
            {
                Assert.True(inputLength > copyMemoryStream.Length); // let's assume some kind of compression happened!
                Assert.True(gzip.Entries.Any());
                var entry = gzip.Entries.Single();
                Assert.False(string.IsNullOrEmpty(entry.Name));
                using (var gzipStream = gzip.OpenEntry(entry))
                {
                    var extractedEntryCrc = Crc32.OfStream(gzipStream, fromStartOfStream: false);
                    Assert.Equal(inputCrc, extractedEntryCrc);
                }
            }
        }

        [Fact]
        public void GZipNativeAccess_WriteRomResourceToGZipFile_ProducesExpectedResult()
        {
            var gzipFileName = TemporaryFile.GenerateUniqueFilePath("tagalong", ".luigi.gz");

            using (TemporaryFile.CreateTemporaryFileWithPath(gzipFileName, createEmptyFile: false))
            {
                // Create on-disk GZIP
                var inputLength = 0L;
                var fileStream = new FileStream(gzipFileName, FileMode.Create, FileAccess.Write);
                using (var gzip = CompressedArchiveAccess.Open(fileStream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create))
                {
                    var testResourceName = "INTV.TestHelpers.Core.Resources.tagalong.luigi";
                    var newGZipEntryName = "tagalong.luigi";
                    var entry = gzip.CreateEntry(newGZipEntryName);
                    using (var gzipStream = gzip.Open(entry.Name))
                    using (var sourceStream = typeof(TestRomResources).Assembly.GetManifestResourceStream(testResourceName))
                    {
                        sourceStream.CopyTo(gzipStream);
                        inputLength = sourceStream.Length;
                    }
                }

                // Now, see if we can extract it!
                var extractedRomPath = Path.Combine(Path.GetDirectoryName(gzipFileName), Path.GetFileNameWithoutExtension(gzipFileName));
                using (TemporaryFile.CreateTemporaryFileWithPath(extractedRomPath, createEmptyFile: false))
                {
                    var fileInfo = new FileInfo(gzipFileName);
                    Assert.True(fileInfo.Exists);
                    Assert.True(inputLength > fileInfo.Length); // Compressed we must be! On this, all depends.
                    using (var gzip = CompressedArchiveAccess.Open(gzipFileName, CompressedArchiveAccessMode.Read))
                    {
                        Assert.True(gzip.Entries.Any());
                        var entry = gzip.Entries.Single();
                        Assert.False(string.IsNullOrEmpty(entry.Name));
                        using (var outputFileStream = new FileStream(extractedRomPath, FileMode.Create, FileAccess.Write))
                        using (var gzipStream = gzip.OpenEntry(entry))
                        {
                            gzipStream.CopyTo(outputFileStream);
                        }
                    }

                    // Verify we have a valid LUIGI and it's got what we expect inside. Trust, but verify!
                    LuigiFileHeader header = null;
                    using (var outputFileStream = new FileStream(extractedRomPath, FileMode.Open, FileAccess.Read))
                    {
                        header = LuigiFileHeader.Inflate(outputFileStream);
                    }

                    Assert.NotNull(header);
                    Assert.Equal(RomFormat.Bin, header.OriginalRomFormat);
                    Assert.Equal(TestRomResources.TestBinCrc, header.OriginalRomCrc32);
                }
            }
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
    }
}

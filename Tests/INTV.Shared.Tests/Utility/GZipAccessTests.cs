// <copyright file="GZipAccessTests.cs" company="INTV Funhouse">
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
    public class GZipAccessTests
    {
        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_OpenNonGZip_ThrowsInvalidDataException(CompressedArchiveAccessImplementation implementation)
        {
            var nonGZipResource = TestResource.TextEmbeddedResourceFile;

            using (var stream = nonGZipResource.OpenResourceForReading())
            {
                Assert.Throws<InvalidDataException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read, implementation));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_OpenWithInvalidMode_ThrowsArgumentOutOfRangeException(CompressedArchiveAccessImplementation implementation)
        {
            var gzipResource = TestResource.TagalongBinGZip;

            using (var stream = gzipResource.OpenResourceForReading())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, (CompressedArchiveAccessMode)100, implementation));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessMode.Create, CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessMode.Create, CompressedArchiveAccessImplementation.SharpZipLib)]
        [InlineData(CompressedArchiveAccessMode.Update, CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessMode.Update, CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_OpenNonEmptyGZipForModification_ThrowsInvalidOperationException(CompressedArchiveAccessMode mode, CompressedArchiveAccessImplementation implementation)
        {
            var gzipResource = TestResource.TagalongBinGZip;

            using (var stream = gzipResource.OpenResourceForReading())
            {
                Assert.Throws<InvalidOperationException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, mode, implementation));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_DeleteEntry_ThrowsNotSupportedException(CompressedArchiveAccessImplementation implementation)
        {
            var gzipResource = TestResource.TagalongCfgGZip;
            var entryName = gzipResource.ArchiveContents.First();

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read, implementation))
            {
                Assert.Throws<NotSupportedException>(() => gzip.DeleteEntry(entryName));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_CreateEntryWhenOpenInDecompressMode_ThrowsInvalidOperationException(CompressedArchiveAccessImplementation implementation)
        {
            var gzipResource = TestResource.TagalongCfgGZip;

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read, implementation))
            {
                Assert.Throws<InvalidOperationException>(() => gzip.CreateEntry("derp"));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_OpenSingleMemberFileWithEntryName_HasExpectedContents(CompressedArchiveAccessImplementation implementation)
        {
            var gzipResource = TestResource.TagalongBinGZip;

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read, implementation))
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

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_OpenSecondMemberEntry_HasExpectedContents(CompressedArchiveAccessImplementation implementation)
        {
            var gzipResource = TestResource.TagalongBinCfgYYGZip;

            var stream = gzipResource.OpenResourceForReading();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read, implementation))
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

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_IsArchive_IsFalse(CompressedArchiveAccessImplementation implementation)
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create, implementation))
            {
                Assert.False(gzip.IsArchive);
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_IsCompressed_IsTrue(CompressedArchiveAccessImplementation implementation)
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create, implementation))
            {
                Assert.True(gzip.IsCompressed);
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_CreateEntryInEmptyStream_Succeeds(CompressedArchiveAccessImplementation implementation)
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create, implementation))
            {
                Assert.NotNull(gzip.CreateEntry("newEntry"));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_CreateSecondEntry_ThrowsNotSupportedException(CompressedArchiveAccessImplementation implementation)
        {
            var stream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create, implementation))
            {
                gzip.CreateEntry("newEntry");

                Assert.Throws<NotSupportedException>(() => gzip.CreateEntry("newEntry"));
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_OpenFromDisk_HasExpectedContent(CompressedArchiveAccessImplementation implementation)
        {
            string testResourcePath;
            using (TestResource.TagalongBinCfgNNGZip.ExtractToTemporaryFile(out testResourcePath))
            using (var gzip = CompressedArchiveAccess.Open(testResourcePath, CompressedArchiveAccessMode.Read, implementation))
            {
                var expectedCrc32s = new[] { TestRomResources.TestBinCrc, TestRomResources.TestCfgCrc };
                var i = 0;
                foreach (var entry in gzip.Entries)
                {
                    using (var entryStream = gzip.OpenEntry(entry))
                    using (var validationStream = new MemoryStream())
                    {
                        // Some implementations, e.g. SharpZipLib, inflate the ENTIRE contents of ALL members of a multi-member entry into one giant output,
                        // so we need to read to an intermediate stream, then verify the output.
                        entryStream.CopyTo(validationStream);
                        validationStream.SetLength(entry.Length);
                        var crc = Crc32.OfStream(validationStream);
                        Assert.Equal(expectedCrc32s[i], crc);
                    }
                    ++i;
                }
            }
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_WriteRomResourceToGZip_ProducesExpectedResult(CompressedArchiveAccessImplementation implementation)
        {
            var inputCrc = 0u;
            var inputLength = 0L;

            // Create in-memory GZIP
            var newMemoryStream = new MemoryStream();
            var copyMemoryStream = new MemoryStream();
            using (var gzip = CompressedArchiveAccess.Open(newMemoryStream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create, implementation))
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

            using (var gzip = CompressedArchiveAccess.Open(copyMemoryStream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Read, implementation))
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

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveAccessImplementation.SharpZipLib)]
        public void GZipAccess_WriteRomResourceToGZipFile_ProducesExpectedResult(CompressedArchiveAccessImplementation implementation)
        {
            var gzipFileName = TemporaryFile.GenerateUniqueFilePath("tagalong", ".luigi.gz");

            using (TemporaryFile.CreateTemporaryFileWithPath(gzipFileName, createEmptyFile: false))
            {
                // Create on-disk GZIP
                var inputLength = 0L;
                var fileStream = new FileStream(gzipFileName, FileMode.Create, FileAccess.Write);
                using (var gzip = CompressedArchiveAccess.Open(fileStream, CompressedArchiveFormat.GZip, CompressedArchiveAccessMode.Create, implementation))
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
                    using (var gzip = CompressedArchiveAccess.Open(gzipFileName, CompressedArchiveAccessMode.Read, implementation))
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
    }
}

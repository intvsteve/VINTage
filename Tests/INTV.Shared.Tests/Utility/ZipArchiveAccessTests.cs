// <copyright file="ZipArchiveAccessTests.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class ZipArchiveAccessTests
    {
        [Fact]
        public void ZipArchiveAccess_OpenForRead_HasExpectedContents()
        {
            var zipResource = TestResource.TagalongZip;

            var zipStream = zipResource.OpenResourceForReading();
            using (var zipArchive = CompressedArchiveAccess.Open(zipStream, CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var entries = zipArchive.Entries;

                Assert.True(zipArchive.IsArchive);
                Assert.True(zipArchive.IsCompressed);
                Assert.Equal(zipResource.ArchiveContents.Count(), entries.Count());
                Assert.Equal(zipResource.ArchiveContents, entries.Select(e => e.Name));
                Assert.All(entries, e => Assert.True(e.Length > 0));
                Assert.All(entries, e => Assert.True(e.Length > 0));
                Assert.All(entries, e => Assert.True(e.LastModificationTime.Year > 1978));
                Assert.All(entries, e => Assert.False(e.IsDirectory));
            }
        }

        [Fact]
        public void ZipArchiveAccess_OpenEntryForRead_SuccessfullyOpensEntry()
        {
            var zipResource = TestResource.TagalongDirZip;

            var zipStream = zipResource.OpenResourceForReading();
            using (var zipArchive = CompressedArchiveAccess.Open(zipStream, CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Read))
            {
                var zipEntry = zipArchive.FindEntry(zipResource.ArchiveContents.First(e => e.EndsWith(".luigi")));
                var entries = zipArchive.Entries;

                Assert.NotNull(zipEntry);
                Assert.Equal(zipResource.ArchiveContents, entries.Select(e => e.Name));
                Assert.True(entries.Any(e => e.IsDirectory));
                using (var zipEntryStream = zipArchive.OpenEntry(zipEntry))
                {
                    Assert.NotNull(zipEntryStream);
                    VerifyLuigiZipEntry(zipEntryStream);
                }
            }
        }

        /// <summary>
        /// Tests archive creation AND validates the contents - more than a unit test really should.
        /// </summary>
        /// <remarks>This test actually covers multiple areas - file and directory entry creation, as well
        /// as testing the Windows path separator rather than the standard forward slash. Further,
        /// it validates the results of zip creation by also verifying the contents afterwards.</remarks>
        [Fact]
        public void ZipArchiveAccess_CreateNewZip_SuccessfullyAddDirectoryAndFileEntries()
        {
            var zipTestEntryName = @"test\entry";
            var zipTestEntryContent = "Here is some text to write!";
            var testZipFilePath = TemporaryFile.GenerateUniqueFilePath("ZipTest", ".zip");

            var zipStream = new FileStream(testZipFilePath, FileMode.Create, FileAccess.Write);
            using (var zipArchive = CompressedArchiveAccess.Open(zipStream, CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Create))
            {
                var zipDirectoryEntryName = Path.GetDirectoryName(zipTestEntryName) + Path.DirectorySeparatorChar;
                var zipDirectoryEntry = zipArchive.CreateEntry(zipDirectoryEntryName);
                var zipEntry = zipArchive.CreateEntry(zipTestEntryName);
                using (var zipEntryStream = zipArchive.OpenEntry(zipEntry))
                {
                    var textToWrite = Encoding.UTF8.GetBytes(zipTestEntryContent);
                    zipEntryStream.Write(textToWrite, 0, textToWrite.Length);
                }
            }

            Assert.True(File.Exists(testZipFilePath));
            VerifyTestZipContents(testZipFilePath, zipTestEntryName, zipTestEntryContent);
        }

        [Fact]
        public void ZipArchiveAccess_DeleteAnEntryFromExistingZipArchive_RemoveEntry()
        {
            var numberOfEntriesToCreate = 4;
            var zipTestEntryNameFormat = "testEntry_{0}";
            var zipTestEntryContentFormat = "Here is some text to write for {0}!";
            var testZipFilePath = TemporaryFile.GenerateUniqueFilePath("ZipTest", ".zip");
            var zipStream = new FileStream(testZipFilePath, FileMode.Create, FileAccess.Write);
            using (var zipArchive = CompressedArchiveAccess.Open(zipStream, CompressedArchiveFormat.Zip, CompressedArchiveAccessMode.Create))
            {
                for (var i = 0; i < numberOfEntriesToCreate; ++i)
                {
                    var entryName = string.Format(CultureInfo.CurrentCulture, zipTestEntryNameFormat, i);
                    var entry = zipArchive.CreateEntry(entryName);
                    using (var zipEntryStream = zipArchive.OpenEntry(entry))
                    {
                        var zipTestEntryContent = string.Format(CultureInfo.CurrentCulture, zipTestEntryContentFormat, entryName);
                        var textToWrite = Encoding.UTF8.GetBytes(zipTestEntryContent);
                        zipEntryStream.Write(textToWrite, 0, textToWrite.Length);
                    }
                }
            }
            VerifyNumberOfEntries(testZipFilePath, numberOfEntriesToCreate);

            var numberOfEntriesToDelete = 2;
            using (var zipArchive = CompressedArchiveAccess.Open(testZipFilePath, CompressedArchiveAccessMode.Update))
            {
                var entriesToDelete = zipArchive.Entries.Take(numberOfEntriesToDelete).Select(e => e.Name).ToList();
                foreach (var entry in entriesToDelete)
                {
                    zipArchive.DeleteEntry(entry);
                }
            }

            using (var tempFile = TemporaryFile.CreateTemporaryFileWithPath(testZipFilePath, createEmptyFile: false))
            {
                VerifyNumberOfEntries(testZipFilePath, numberOfEntriesToCreate - numberOfEntriesToDelete);
            }
        }

        private static void VerifyLuigiZipEntry(Stream stream)
        {
            var magicKey = new byte[3] { (byte)'L', (byte)'T', (byte)'O' };
            var reader = new INTV.Core.Utility.BinaryReader(stream);
            byte[] header = reader.ReadBytes(magicKey.Length);
            Assert.True(header.SequenceEqual(magicKey));
        }

        private static void VerifyTestZipContents(string testZipFilePath, string zipTestEntryName, string zipTestEntryContent)
        {
            using (var tempFile = TemporaryFile.CreateTemporaryFileWithPath(testZipFilePath, createEmptyFile: false))
            using (var zipArchive = CompressedArchiveAccess.Open(testZipFilePath, CompressedArchiveAccessMode.Read))
            {
                var entries = zipArchive.Entries;
                Assert.True(entries.Any(e => e.IsDirectory));

                var fileEntryStream = zipArchive.Open(zipTestEntryName);
                using (var streamReader = new StreamReader(fileEntryStream, Encoding.UTF8))
                {
                    var entryContent = streamReader.ReadToEnd();
                    Assert.Equal(zipTestEntryContent, entryContent);
                }
            }
        }

        private static void VerifyNumberOfEntries(string testZipFilePath, int expectedNumberOfEntries)
        {
            using (var zipArchive = CompressedArchiveAccess.Open(testZipFilePath, CompressedArchiveAccessMode.Read))
            {
                var numberOfEntries = zipArchive.Entries.Count();
                Assert.Equal(expectedNumberOfEntries, numberOfEntries);
            }
        }
    }
}

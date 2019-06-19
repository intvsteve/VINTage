// <copyright file="TarArchiveAccessTests.cs" company="INTV Funhouse">
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
using System.IO;
using System.Linq;
using INTV.Core.Utility;
using INTV.Shared.Utility;
using INTV.TestHelpers.Core.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class TarArchiveAccessTests
    {
        [Fact]
        public void TarArchiveAccess_OpenNonTar_ThrowsInvalidDataException()
        {
            var nonGZipResource = TestResource.TextEmbeddedResourceFile;

            using (var stream = nonGZipResource.OpenResourceForReading())
            {
                Assert.Throws<System.IO.InvalidDataException>(() => CompressedArchiveAccess.Open(stream, CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Read));
            }
        }

        [Fact]
        public void TarArchiveAccess_OpenInUpdateMode_ThrowsInvalidOperationException()
        {
            var tarResource = TestResource.TagalongBinCfgTar;

            Assert.Throws<InvalidOperationException>(() => CompressedArchiveAccess.Open(tarResource.OpenResourceForReading(), CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Update));
        }

        [Fact]
        public void TarArchiveAccess_OpenInInvalidMode_ThrowsArgumentOutOfRangeException()
        {
            var tarResource = TestResource.TagalongBinCfgTar;

            Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveAccess.Open(tarResource.OpenResourceForReading(), CompressedArchiveFormat.Tar, (CompressedArchiveAccessMode)100));
        }

        [Fact]
        public void TarArchiveAccess_DeleteEntry_ThrowsNotSupportedException()
        {
            var tarResource = TestResource.TagalongBinCfgTar;
            var entryName = tarResource.ArchiveContents.First();

            using (var tar = CompressedArchiveAccess.Open(tarResource.OpenResourceForReading(), CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Read))
            {
                Assert.Throws<NotSupportedException>(() => tar.DeleteEntry(entryName));
            }
        }

        public static IEnumerable<object[]> OpenTarTestData
        {
            get
            {
                yield return new object[] { TestResource.TagalongBinCfgTar, false };
                yield return new object[] { TestResource.TagalongCC3RomTar, false };
                yield return new object[] { TestResource.TagalongDirLuigiRomTar, true };
            }
        }

        [Theory]
        [MemberData("OpenTarTestData")]
        public void TarArchiveAccess_OpenTar_HasExpectedEntries(TestResource testResource, bool expectAtLeastOneDirectoryEntry)
        {
            using (var tar = CompressedArchiveAccess.Open(testResource.OpenResourceForReading(), CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Read))
            {
                Assert.True(tar.IsArchive);
                Assert.False(tar.IsCompressed);
                var expectedEntryNames = testResource.ArchiveContents.ToList();
                Assert.Equal(expectedEntryNames.Count, tar.Entries.Count());
                Assert.Equal(expectAtLeastOneDirectoryEntry, tar.Entries.Any(e => e.IsDirectory));
                var i = 0;
                foreach (var entry in tar.Entries)
                {
                    Assert.Equal(expectedEntryNames[i], entry.Name);
                    Assert.True(entry.LastModificationTime.Year > 1977);
                    if (!entry.IsDirectory)
                    {
                        Assert.True(entry.Length > 0);
                    }
                    ++i;
                }
            }
        }

        [Fact]
        public void TarArchiveAccess_OpenEntryWithEntryName_HasExpectedContents()
        {
            var tarResource = TestResource.TagalongDirLuigiRomTar;

            using (var tar = CompressedArchiveAccess.Open(tarResource.OpenResourceForReading(), CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Read))
            {
                var entry = tar.Entries.Last();
                Assert.Equal(tarResource.ArchiveContents.Last(), entry.Name);

                var crc = 0u;
                using (var entryStream = tar.Open(entry.Name))
                {
                    crc = Crc32.OfStream(entryStream);
                }

                Assert.Equal(TestRomResources.TestRomCrc, crc);
            }
        }

        [Fact]
        public void TarArchiveAccess_CreateEntryWhenOpenInReadMode_ThrowsInvalidOperationException()
        {
            var tarResource = TestResource.TagalongBinCfgTar;

            using (var tar = CompressedArchiveAccess.Open(tarResource.OpenResourceForReading(), CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Read))
            {
                Assert.Throws<InvalidOperationException>(() => tar.CreateEntry("derp"));
            }
        }

        [Fact]
        public void TarArchiveAccess_CreateEntryWithNullName_ThrowsArgumentNullException()
        {
            using (var tar = CompressedArchiveAccess.Open(new MemoryStream(), CompressedArchiveFormat.Tar, CompressedArchiveAccessMode.Create))
            {
                Assert.Throws<ArgumentNullException>(() => tar.CreateEntry(null));
            }
        }

        [Fact]
        public void TarArchiveAccess_CreateAndAddEntries_AddsEntries()
        {
            using (var temporaryDirectory = new TemporaryDirectory())
            {
                var entryNames = new[] { "tagalong.adv", "sub/tagalong.luigi", @"sub\tagalong.rom" };
                var entryCrcs = new List<uint>();
                var tarFilePath = Path.Combine(temporaryDirectory.Path, "test_tagalong.tar");
                using (var tar = CompressedArchiveAccess.Open(tarFilePath, CompressedArchiveAccessMode.Create))
                {
                    foreach (var entryName in entryNames)
                    {
                        using (var newEntryData = TestResource.OpenExternalResourceForReading(typeof(TestRomResources), Path.GetFileName(entryName)))
                        {
                            var newEntry = tar.CreateEntry(entryName);
                            Assert.NotNull(newEntry);
                            using (var entryStream = tar.OpenEntry(newEntry))
                            {
                                newEntryData.CopyTo(entryStream);
                            }
                            entryCrcs.Add(Crc32.OfStream(newEntryData));
                        }
                    }
                }

                using (var tar = CompressedArchiveAccess.Open(tarFilePath, CompressedArchiveAccessMode.Read))
                {
                    Assert.Equal(entryNames.Length, tar.Entries.Count());
                    for (var i = 0; i < entryNames.Length; ++i)
                    {
                        var entry = tar.Entries.ElementAt(i);
                        Assert.Equal(entryNames[i], entry.Name);
                        using (var entryStream = tar.OpenEntry(entry))
                        {
                            var crc = Crc32.OfStream(entryStream);
                            Assert.Equal(entryCrcs[i], crc);
                        }
                    }
                }
            }
        }

        [Fact]
        public void TarArchiveAccess_CreateTarAndAddFileEntriesUsingAbsolutePaths_AddsEntries()
        {
            using (var temporaryDirectory = new TemporaryDirectory())
            {
                var entryNames = new[] { "tagalong.bin", "tagalong.cfg", "tagalong.rom" };
                var entryCrcs = new List<uint>();
                for (var i = 0; i < entryNames.Length; ++i)
                {
                    using (var fileStream = new FileStream(Path.Combine(temporaryDirectory.Path, entryNames[i]), FileMode.Create, FileAccess.Write))
                    using (var fileData = TestResource.OpenExternalResourceForReading(typeof(TestRomResources), entryNames[i]))
                    {
                        fileData.CopyTo(fileStream);
                        entryCrcs.Add(Crc32.OfStream(fileData));
                        entryNames[i] = fileStream.Name;
                    }
                }

                var tarFilePath = Path.Combine(temporaryDirectory.Path, "test_tagalong.tar");
                using (var tar = CompressedArchiveAccess.Open(tarFilePath, CompressedArchiveAccessMode.Create))
                {
                    foreach (var entryName in entryNames)
                    {
                        var entry = tar.CreateEntry(entryName);
                        using (tar.OpenEntry(entry))
                        {
                            Assert.True(File.Exists(entryName));
                        }
                    }
                }

                using (var tar = CompressedArchiveAccess.Open(tarFilePath, CompressedArchiveAccessMode.Read))
                {
                    Assert.Equal(entryNames.Length, tar.Entries.Count());
                    for (var i = 0; i < entryNames.Length; ++i)
                    {
                        var entry = tar.Entries.ElementAt(i);
                        Assert.Equal(Path.GetFileName(entryNames[i]), entry.Name);
                        using (var entryData = tar.OpenEntry(entry))
                        {
                            var crc = Crc32.OfStream(entryData);
                            Assert.Equal(entryCrcs[i], crc);
                        }
                    }
                }
            }
        }

        [Fact]
        public void TarArchiveAccess_CreateTarAndAddDirectory_AddsEntries()
        {
            using (var temporaryDirectory = new TemporaryDirectory())
            {
                var rootTagalongs = new[] { "tagalong.int", "tagalong.cfg", "tagalong.adv" };
                var expectedEntryNames = new List<string>(rootTagalongs);
                var expectedEntryCrcs = new List<uint>();
                for (var i = 0; i < rootTagalongs.Length; ++i)
                {
                    using (var fileStream = new FileStream(Path.Combine(temporaryDirectory.Path, rootTagalongs[i]), FileMode.Create, FileAccess.Write))
                    using (var fileData = TestResource.OpenExternalResourceForReading(typeof(TestRomResources), rootTagalongs[i]))
                    {
                        fileData.CopyTo(fileStream);
                        expectedEntryCrcs.Add(Crc32.OfStream(fileData));
                        rootTagalongs[i] = fileStream.Name;
                    }
                }
                var metadataTagalongs = new[] { "tagalong_metadata.bin", "tagalong_metadata.cfg", "tagalong_metadata.luigi", "tagalong_metadata.rom" };
                var subdirectory0 = @"tagalongs_metadata\";
                expectedEntryNames.Add(subdirectory0);
                expectedEntryCrcs.Add(0);
                subdirectory0 = Path.Combine(temporaryDirectory.Path, subdirectory0);
                Assert.True(Directory.CreateDirectory(subdirectory0).Exists);
                foreach (var metadataTagalong in metadataTagalongs)
                {
                    using (var fileStream = new FileStream(Path.Combine(subdirectory0, metadataTagalong), FileMode.Create, FileAccess.Write))
                    using (var fileData = TestResource.OpenExternalResourceForReading(typeof(TestRomResources), metadataTagalong))
                    {
                        fileData.CopyTo(fileStream);
                        expectedEntryNames.Add("tagalongs_metadata/" + metadataTagalong);
                        expectedEntryCrcs.Add(Crc32.OfStream(fileData));
                    }
                }
                var scrambledTagalongs = new[] { "tagalong_any.luigi", "tagalong_dev0.luigi", "tagalong_dev1.luigi", "tagalong_metadata_any.luigi", "tagalong_metadata_dev0.luigi", "tagalong_metadata_dev1.luigi" };
                var subdirectory1 = "tagalongs_scrambled/";
                expectedEntryNames.Add(subdirectory1.Replace('/', '\\'));
                expectedEntryCrcs.Add(0);
                subdirectory1 = Path.Combine(temporaryDirectory.Path, subdirectory1);
                Assert.True(Directory.CreateDirectory(subdirectory1).Exists);
                foreach (var scrambledTagalong in scrambledTagalongs)
                {
                    using (var fileStream = new FileStream(Path.Combine(subdirectory1, scrambledTagalong), FileMode.Create, FileAccess.Write))
                    using (var fileData = TestResource.OpenExternalResourceForReading(typeof(TestRomResources), scrambledTagalong))
                    {
                        fileData.CopyTo(fileStream);
                        expectedEntryNames.Add("tagalongs_scrambled/" + scrambledTagalong);
                        expectedEntryCrcs.Add(Crc32.OfStream(fileData));
                    }
                }

                var tarFilePath = Path.Combine(temporaryDirectory.Path, "test_tagalong_with_subdirectories.tar");
                using (var tar = CompressedArchiveAccess.Open(tarFilePath, CompressedArchiveAccessMode.Create))
                {
                    foreach (var rootTagalong in rootTagalongs)
                    {
                        var entry = tar.CreateEntry(rootTagalong);
                        using (tar.OpenEntry(entry))
                        {
                            Assert.True(File.Exists(rootTagalong));
                        }
                    }
                    var subdirectory0Entry = tar.CreateEntry(subdirectory0);
                    using (tar.OpenEntry(subdirectory0Entry))
                    {
                        Assert.True(Directory.Exists(subdirectory0));
                    }
                    var subdirectory1Entry = tar.CreateEntry(subdirectory1);
                    using (tar.OpenEntry(subdirectory1Entry))
                    {
                        Assert.True(Directory.Exists(subdirectory1));
                    }
                }

                using (var tar = CompressedArchiveAccess.Open(tarFilePath, CompressedArchiveAccessMode.Read))
                {
                    Assert.Equal(expectedEntryNames.Count, tar.Entries.Count());
                    for (int i = 0; i < expectedEntryNames.Count; ++i)
                    {
                        var entry = tar.Entries.ElementAt(i);
                        Assert.Equal(expectedEntryNames[i], entry.Name);
                        Assert.Equal(expectedEntryCrcs[i] == 0, entry.IsDirectory);
                        if (!entry.IsDirectory)
                        {
                            using (var entryData = tar.OpenEntry(entry))
                            {
                                var crc = Crc32.OfStream(entryData);
                                Assert.Equal(expectedEntryCrcs[i], crc);
                            }
                        }
                    }
                }
            }
        }
    }
}

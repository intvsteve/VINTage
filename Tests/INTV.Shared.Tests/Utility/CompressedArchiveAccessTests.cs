﻿// <copyright file="CompressedArchiveAccessTests.cs" company="INTV Funhouse">
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
using System.Reflection;
using INTV.Core.Utility;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class CompressedArchiveAccessTests : ICompressedArchiveTest
    {
        [Fact]
        public void CompressedArchiveAccess_RegisterFactory_Succeeds()
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();
            var implementation = this.GetFakeCompressedArchiveAccessImplementationForTest();

            var succeeded = CompressedArchiveAccess.RegisterFactory(format, implementation, (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementation));

            Assert.True(succeeded);
        }

        [Fact]
        public void CompressedArchiveAccess_RegisterFactoryTwice_SucceedsThenFails()
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();
            var implementation = this.GetFakeCompressedArchiveAccessImplementationForTest();
            Assert.True(CompressedArchiveAccess.RegisterFactory(format, implementation, (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementation)));

            var succeeded = CompressedArchiveAccess.RegisterFactory(format, implementation, (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementation));

            Assert.False(succeeded);
        }

        [Fact]
        public void CompressedArchiveAccess_RegisterWithForbiddenFormat_ThrowsArgumentOutOfRangeException()
        {
            var implementation = this.GetFakeCompressedArchiveAccessImplementationForTest();

            Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveAccess.RegisterFactory(
                CompressedArchiveFormat.None,
                implementation,
                (s, m) => TestCompressedArchiveAccess.Create(s, m, CompressedArchiveFormat.None, implementation)));
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.None)]
        [InlineData(CompressedArchiveAccessImplementation.Any)]
        public void CompressedArchiveAccess_RegisterFactoryWithForbiddenImplementationValue_ThrowsArgumentOutOfRangeException(CompressedArchiveAccessImplementation implementation)
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();

            Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveAccess.RegisterFactory(
                format,
                implementation,
                (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementation)));
        }

        [Theory]
        [InlineData(CompressedArchiveAccessMode.Read, FileMode.Open)]
        [InlineData(CompressedArchiveAccessMode.Create, FileMode.Create)]
        [InlineData(CompressedArchiveAccessMode.Update, FileMode.OpenOrCreate)]
        public void CompressedArchiveAccess_ConvertCompressedArchiveAccessModeToFileMode_ReturnsExpectedFileMode(CompressedArchiveAccessMode mode, FileMode expectedFileMode)
        {
            Assert.Equal(expectedFileMode, TestCompressedArchiveAccess.ConvertModeToFileMode(mode));
        }

        [Theory]
        [InlineData(CompressedArchiveAccessMode.Read, FileAccess.Read)]
        [InlineData(CompressedArchiveAccessMode.Create, FileAccess.Write)]
        [InlineData(CompressedArchiveAccessMode.Update, FileAccess.ReadWrite)]
        public void CompressedArchiveAccess_ConvertCompressedArchiveAccessModeToFileMode_ReturnsExpectedFileAccess(CompressedArchiveAccessMode mode, FileAccess expectedFileAccess)
        {
            Assert.Equal(expectedFileAccess, TestCompressedArchiveAccess.ConvertModeToFileAccess(mode));
        }

        [Theory]
        [InlineData(CompressedArchiveAccessMode.Read)]
        [InlineData(CompressedArchiveAccessMode.Create)]
        [InlineData(CompressedArchiveAccessMode.Update)]
        public void CompressedArchiveAccess_OpenUnknownFormatFromStream_ThrowsNotSupportedException(CompressedArchiveAccessMode mode)
        {
            Assert.Throws<NotSupportedException>(() => CompressedArchiveAccess.Open(Stream.Null, this.GetFakeCompressedArchiveFormatForTest(), mode));
        }

        [Fact]
        public void CompressedArchiveAccess_OpenForReadFromStream_RootLocationIsNull()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.Null(((CompressedArchiveAccess)archive).RootLocation);
                Assert.Equal(format, ((TestCompressedArchiveAccess)archive).Format);
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenForReadFromStreamAndDeleteNonexistentEntry_ReturnsFalse()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.False(archive.DeleteEntry("booger"));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromStreamIStorageAccessFindNonexistentEntry_ReturnsNull()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.Null(archive.FindEntry("glrb"));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromStreamIStorageAccessOpenOnInvalidEntryByName_ReturnsNull()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.Null(archive.Open("fleen"));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromStreamIStorageAccessExistsForInvalidEntryByName_ReturnsFalse()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.False(archive.Exists("Klern"));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromStreamIStorageAccessSizeOfInvalidEntryByName_ReturnsZero()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.Equal(0L, archive.Size("pLEf"));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromStreamIStorageAccessLastWriteTimeUtcOfInvalidEntryByName_ReturnsMin()
        {
            var format = RegisterFakeFormatForTest();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
                Assert.Equal(DateTime.MinValue, archive.LastWriteTimeUtc("Scrallette"));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenActualEntryUsingIStoragAccess_ReturnsValidStream()
        {
            var format = RegisterFakeFormatForTest(registerFormat: true);

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                var testArchive = (TestCompressedArchiveAccess)archive;
                var firstEntryName = testArchive.AddFakeEntries(4);
                IStorageAccess storageAccess = archive;
                Assert.NotNull(storageAccess.Open(firstEntryName));
                Assert.Equal(format, testArchive.Format);
                Assert.Equal(format.GetPreferredCompressedArchiveImplementation(), testArchive.Implementation);
            }
        }

        [Fact]
        public void CompressedArchiveAccess_GetActualEntrySizeUsingIStoragAccess_ReturnsNonzeroValue()
        {
            var format = RegisterFakeFormatForTest(registerFormat: true);

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                var testArchive = (TestCompressedArchiveAccess)archive;
                var firstEntryName = testArchive.AddFakeEntries(2);
                IStorageAccess storageAccess = archive;
                Assert.NotEqual(0L, storageAccess.Size(firstEntryName));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_GetActualEntryLastWriteTimeUsingIStoragAccess_ReturnsNonMinimumValue()
        {
            var format = RegisterFakeFormatForTest(registerFormat: true);

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                var testArchive = (TestCompressedArchiveAccess)archive;
                var firstEntryName = testArchive.AddFakeEntries(5);
                IStorageAccess storageAccess = archive;
                Assert.NotEqual(DateTime.MinValue, storageAccess.LastWriteTimeUtc(firstEntryName));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_DeleteActualEntry_ReturnsTrue()
        {
            var format = RegisterFakeFormatForTest(registerFormat: true);

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Update))
            {
                var testArchive = (TestCompressedArchiveAccess)archive;
                var firstEntryName = testArchive.AddFakeEntries(3);
                Assert.True(archive.DeleteEntry(firstEntryName));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_RegisterTwoImplementationsAndCreateUsingEitherOne_BothSucceed()
        {
            var format = RegisterFakeFormatForTest(registerFormat: true);
            var initialImplementation = format.GetPreferredCompressedArchiveImplementation();

            using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read))
            {
                Assert.NotNull(archive);
            }

            Assert.True(format.AddImplementation(this.GetFakeCompressedArchiveAccessImplementationForTest(), makePreferred: true));
            var newImplementation = format.GetPreferredCompressedArchiveImplementation();
            Assert.NotEqual(initialImplementation, newImplementation);
            Assert.True(CompressedArchiveAccess.RegisterFactory(format, newImplementation, (s, m) => TestCompressedArchiveAccess.Create(s, m, format, newImplementation)));
            Assert.NotNull(CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Create));
        }

        [Fact]
        public void CompressedArchiveAccess_RegisterMultipleImplementationsAndCreateWithSpecificImplementation_CreatesUsingSelectedImplementation()
        {
            var format = RegisterFakeFormatForTest(registerFormat: true);

            var implementations = new[]
                {
                    format.GetPreferredCompressedArchiveImplementation(),
                    this.GetFakeCompressedArchiveAccessImplementationForTest(),
                    this.GetFakeCompressedArchiveAccessImplementationForTest()
                };
            Assert.True(format.AddImplementation(implementations[1], makePreferred: false));
            Assert.True(format.AddImplementation(implementations[2], makePreferred: false));
            Assert.True(CompressedArchiveAccess.RegisterFactory(format, implementations[1], (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementations[1])));
            Assert.True(CompressedArchiveAccess.RegisterFactory(format, implementations[2], (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementations[2])));

            foreach (var implementation in implementations)
            {
                using (var archive = CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Create, implementation) as TestCompressedArchiveAccess)
                {
                    Assert.NotNull(archive);
                    Assert.Equal(implementation, archive.Implementation);
                }
            }
        }

        [Fact]
        public void CompressedArchiveAccess_ForceFinalizer()
        {
            var format = RegisterFakeFormatForTest();

            CompressedArchiveAccess.Open(Stream.Null, format, CompressedArchiveAccessMode.Read);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromFileWithUnknownExtension_ThrowsInvalidOperationException()
        {
            var format = RegisterFakeFormatForTest();

            using (var tempFile = new TemporaryFile(".fake", createFile: true))
            {
                Assert.Throws<InvalidOperationException>(() => CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Read));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromFileWithNoFactoryRegistered_ThrowsNotSupportedException()
        {
            var format = this.RegisterTestCompressedArchiveFormat();
            var fileExtension = format.FileExtensions().First();

            using (var tempFile = new TemporaryFile(fileExtension, createFile: true))
            {
                Assert.Throws<NotSupportedException>(() => CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Read));
            }
        }

        [Fact]
        public void CompressedArchiveAccess_OpenFromFileWithFakeFile_SucceedsAndHasExpectedFeatures()
        {
            var format = this.RegisterFakeFormatForTest(registerFormat: true);
            var fileExtension = format.FileExtensions().First();

            using (var tempFile = new TemporaryFile(fileExtension, createFile: true))
            {
                using (var archive = CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Read))
                {
                    Assert.NotNull(archive);
                    Assert.True(archive.IsArchive);
                    Assert.True(archive.IsCompressed);
                    Assert.Equal(format, archive.Format);
                    Assert.False(archive.Entries.Any());
                }
            }
        }

        [Fact]
        public void CompressedArchiveAccess_FileBasedArchiveFindEntryUsingAbsolutePath_Succeeds()
        {
            var format = this.RegisterFakeFormatForTest(registerFormat: true);
            var fileExtension = format.FileExtensions().First();

            using (var tempFile = new TemporaryFile(fileExtension, createFile: true))
            {
                using (var archive = CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Read))
                {
                    var testArchive = TestCompressedArchiveAccess.GetFromCompressedArchiveFileAccess(archive);
                    var firstEntryName = testArchive.AddFakeEntries(3);
                    var firstEntryFullpath = Path.Combine(tempFile.FilePath, firstEntryName);
                    Assert.NotNull(archive.FindEntry(firstEntryFullpath));
                }
            }
        }

        [Fact]
        public void CompressedArchiveAccess_FileBasedArchiveCreateEntry_Succeeds()
        {
            var format = this.RegisterFakeFormatForTest(registerFormat: true);
            var fileExtension = format.FileExtensions().First();

            using (var tempFile = new TemporaryFile(fileExtension, createFile: false))
            {
                using (var archive = CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Create))
                {
                    var newEntryName = "FindTesting123";
                    var newEntry = archive.CreateEntry(newEntryName);
                    Assert.NotNull(newEntry);
                    Assert.Equal(newEntryName, newEntry.Name);
                }
            }
        }

        [Fact]
        public void CompressedArchiveAccess_FileBasedArchiveOpenEntry_Succeeds()
        {
            var format = this.RegisterFakeFormatForTest(registerFormat: true);
            var fileExtension = format.FileExtensions().First();

            using (var tempFile = new TemporaryFile(fileExtension, createFile: true))
            {
                using (var archive = CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Read))
                {
                    var testArchive = TestCompressedArchiveAccess.GetFromCompressedArchiveFileAccess(archive);
                    var openEntryName = testArchive.AddFakeEntries(2);
                    var openEntry = archive.FindEntry(openEntryName);
                    Assert.NotNull(archive.OpenEntry(openEntry));
                }
            }
        }

        [Fact]
        public void CompressedArchiveAccess_FileBasedArchiveDeleteEntry_Succeeds()
        {
            var format = this.RegisterFakeFormatForTest(registerFormat: true);
            var fileExtension = format.FileExtensions().First();

            using (var tempFile = new TemporaryFile(fileExtension, createFile: true))
            {
                using (var archive = CompressedArchiveAccess.Open(tempFile.FilePath, CompressedArchiveAccessMode.Update))
                {
                    var testArchive = TestCompressedArchiveAccess.GetFromCompressedArchiveFileAccess(archive);
                    var entryName = testArchive.AddFakeEntries(2);
                    Assert.True(archive.DeleteEntry(entryName));
                }
            }
        }

        private CompressedArchiveFormat RegisterFakeFormatForTest(bool registerFormat = false, string firstEntryName = null, bool isArchive = true, bool isCompressed = true)
        {
            var format = registerFormat ? this.RegisterTestCompressedArchiveFormat() : this.GetFakeCompressedArchiveFormatForTest();
            var implementation = registerFormat ? format.GetPreferredCompressedArchiveImplementation() : this.GetFakeCompressedArchiveAccessImplementationForTest();
            var registered = CompressedArchiveAccess.RegisterFactory(format, implementation, (s, m) => TestCompressedArchiveAccess.Create(s, m, format, implementation, firstEntryName, isArchive, isCompressed));
            Assert.True(registered);
            return format;
        }

        private class TestCompressedArchiveAccess : CompressedArchiveAccess
        {
            private readonly Dictionary<string, TestCompressedArchiveEntry> _entries = new Dictionary<string, TestCompressedArchiveEntry>();

            private TestCompressedArchiveAccess()
            {
            }

            public override bool IsArchive
            {
                get { return true; }
            }
            private bool _isArchive = true;

            public override bool IsCompressed
            {
                get { return _isCompressed; }
            }
            private bool _isCompressed = true;

            public override IEnumerable<ICompressedArchiveEntry> Entries
            {
                get { return _entries.Values; }
            }

            public override CompressedArchiveFormat Format
            {
                get { return _format; }
            }
            private CompressedArchiveFormat _format;

            public CompressedArchiveAccessImplementation Implementation { get; private set; }

            private CompressedArchiveAccessMode Mode { get; set; }

            public static TestCompressedArchiveAccess Create(
                Stream stream,
                CompressedArchiveAccessMode mode,
                CompressedArchiveFormat format,
                CompressedArchiveAccessImplementation implementation,
                string firstEntryName = null,
                bool isArchive = true,
                bool isCompressed = true)
            {
                var testArchiveAccess = new TestCompressedArchiveAccess() { Mode = mode, _format = format, Implementation = implementation };
                testArchiveAccess._isArchive = isArchive;
                testArchiveAccess._isCompressed = isCompressed;
                if (!string.IsNullOrWhiteSpace(firstEntryName))
                {
                    testArchiveAccess.CreateAndAddEntry(firstEntryName);
                }
                return testArchiveAccess;
            }

            public static TestCompressedArchiveAccess GetFromCompressedArchiveFileAccess(ICompressedArchiveAccess archive)
            {
                var fileAccessArchiveType = typeof(INTV.Shared.Utility.CompressedArchiveAccess).Assembly.GetType("INTV.Shared.Utility.CompressedArchiveAccess+CompressedArchiveFileAccess");
                var instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var property = fileAccessArchiveType.GetProperty("CompressedArchiveAccess", instanceFlags);
                var testArchive = property.GetValue(archive) as TestCompressedArchiveAccess;
                return testArchive;
            }

            public static FileMode ConvertModeToFileMode(CompressedArchiveAccessMode mode)
            {
                return CompressedArchiveAccessModeToFileMode(mode);
            }

            public static FileAccess ConvertModeToFileAccess(CompressedArchiveAccessMode mode)
            {
                return CompressedArchiveAccessModeToFileAccess(mode);
            }

            public string AddFakeEntries(int numEntriesToAdd, string fileExtension = ".entry")
            {
                string firstEntryName = null;
                for (var i = 0; i < numEntriesToAdd; ++i)
                {
                    var name = "TestCompressedArchiveEntry_" + i.ToString("000000") + fileExtension;
                    CreateAndAddEntry(name);
                    if (firstEntryName == null)
                    {
                        firstEntryName = name;
                    }
                }
                return firstEntryName;
            }

            public void SetIsArchive(bool isArchive)
            {
                _isArchive = isArchive;
            }

            public void SetIsCompressed(bool isCompressed)
            {
                _isCompressed = isCompressed;
            }

            public override Stream OpenEntry(ICompressedArchiveEntry entry)
            {
                Stream stream = null;
                TestCompressedArchiveEntry testEntry;
                if (_entries.TryGetValue(entry.Name, out testEntry))
                {
                    stream = Stream.Null;
                }
                return stream;
            }

            public override ICompressedArchiveEntry CreateEntry(string name)
            {
                VerifyWriteAccess();
                var entry = CreateAndAddEntry(name);
                return entry;
            }

            protected override void Dispose(bool disposing)
            {
                // nothing disposable in this version
            }

            protected override bool DeleteEntry(ICompressedArchiveEntry entry)
            {
                VerifyUpdateAccess();
                TestCompressedArchiveEntry testEntry;
                var hasEntry = _entries.TryGetValue(entry.Name, out testEntry);
                if (hasEntry)
                {
                    _entries.Remove(testEntry.Name);
                }
                return hasEntry;
            }

            private TestCompressedArchiveEntry CreateAndAddEntry(string name)
            {
                TestCompressedArchiveEntry entry = null;
                if (FindEntry(name) == null)
                {
                    entry = new TestCompressedArchiveEntry(name, isDirectory: false);
                    _entries[name] = entry;
                }
                return entry;
            }

            private void VerifyReadAccess()
            {
                if (Mode == CompressedArchiveAccessMode.Create)
                {
                    throw new InvalidOperationException("Read operation not allowed. Opened in Create mode.");
                }
            }

            private void VerifyWriteAccess()
            {
                if (Mode == CompressedArchiveAccessMode.Read)
                {
                    throw new InvalidOperationException("Write operation not allowed. Opened in Read mode.");
                }
            }

            private void VerifyUpdateAccess()
            {
                if (Mode != CompressedArchiveAccessMode.Update)
                {
                    throw new InvalidOperationException("Operation not allowed. Not opened in Update mode.");
                }
            }

            private class TestCompressedArchiveEntry : CompressedArchiveEntry
            {
                public TestCompressedArchiveEntry(string name, bool isDirectory)
                {
                    Name = name;
                    Length = -1;
                    LastModificationTime = DateTime.UtcNow;
                    IsDirectory = isDirectory;
                }
            }
        }
    }
}

// <copyright file="UserSpecifiedProgramInformationTableTests.cs" company="INTV Funhouse">
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
using System.Text;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class UserSpecifiedProgramInformationTableTests
    {
        [Theory]
        [InlineData(0)]
        public void UserSpecifiedProgramInformationTable_Initialize_CreatesExpectedDatabase(int numberOfEntries)
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/initialize_with_no_entries_path.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, numberOfEntries);

            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath);

            Assert.NotNull(database);
            Assert.Equal(numberOfEntries, database.Programs.Count());
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_InitializeFromCorruptDatabase_CreatesExpectedDatabase()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/initialize_from_corrupt_database.xml";
            var numberOfEntries = 0;
            storageAccess.AddDatabaseFile(databaseFilePath, numberOfEntries);
            storageAccess.IntroduceCorruption(databaseFilePath);

            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath);

            Assert.NotNull(database);
            Assert.Equal(numberOfEntries, database.Programs.Count());
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_InitializeNonExistentDatabase_CreatesExpectedDatabase()
        {
            UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(null);
            var databaseFilePath = "/nobody/home.xml";

            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath);

            Assert.NotNull(database);
            Assert.Empty(database.Programs);
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_AddDatabaseEntry_AddsEntry()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/add_entry_to_database.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(0x98765432, "My dev ROM");

            Assert.True(database.AddEntry(entry));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_AddDuplicateDatabaseEntry_DoesNotAddEntry()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/add_duplicate_entry_to_database.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(0x23457689, "My dev ROM");
            Assert.True(database.AddEntry(entry));

            entry = new UserSpecifiedProgramInformation(0x23457689, "My other dev ROM");
            Assert.False(database.AddEntry(entry));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntryWithNullNewAndExistingEntry_ReturnsFalse()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_null_entries.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;

            Assert.False(database.GroupWithExistingEntry(null, null));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntryWithNewEntryAndNullExistingEntry_ReturnsFalse()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_new_entry_null_existing.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;

            var newEntry = new UserSpecifiedProgramInformation(0x23457689, "My new dev ROM");
            Assert.False(database.GroupWithExistingEntry(newEntry, null));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntryWithNullNewEntry_ThrowsNullReferenceException()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_null_new_entry.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(0x23457689, "My dev ROM");
            Assert.True(database.AddEntry(entry));

            Assert.Throws<NullReferenceException>(() => database.GroupWithExistingEntry(null, entry));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntry_GroupsEntries()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_existing_groups.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(0x23457689, "My dev ROM");
            Assert.True(database.AddEntry(entry));

            var newEntry = new UserSpecifiedProgramInformation(0x98765432, "My other dev ROM");
            Assert.True(database.GroupWithExistingEntry(newEntry, entry));
            var expectedCrcs = new[] { 0x23457689u, 0x98765432u };
            Assert.Equal(expectedCrcs, entry.Crcs.Select(c => c.Crc));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntryWithOverlappingCrcs_MergesCrcs()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_existing_with_overlapping_crcs_groups.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(1, "My dev ROM");
            entry.AddCrc(3);
            entry.AddCrc(5);
            Assert.True(database.AddEntry(entry));

            var newEntry = new UserSpecifiedProgramInformation(2, "My other dev ROM");
            newEntry.AddCrc(3);
            newEntry.AddCrc(4);
            newEntry.AddCrc(5);
            newEntry.AddCrc(6);
            Assert.True(database.GroupWithExistingEntry(newEntry, entry));
            var expectedCrcs = new[] { 1u, 2u, 3u, 4u, 5u, 6u };
            Assert.Equal(expectedCrcs, entry.Crcs.OrderBy(c => c.Crc).Select(c => c.Crc));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntryWithDuplicate_DoesNotGroupEntry()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_existing_new_is_crc_match_does_not_group.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(0x23457689, "My dev ROM");
            Assert.True(database.AddEntry(entry));

            var newEntry = new UserSpecifiedProgramInformation(0x23457689, "My other dev ROM");
            Assert.False(database.GroupWithExistingEntry(newEntry, entry));
        }

        [Fact]
        public void UserSpecifiedProgramInformationTable_GroupWithExistingEntryWithNoMatchingEntry_DoesNotGroupEntry()
        {
            IReadOnlyList<string> romPaths;
            var storageAccess = UserSpecifiedProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var databaseFilePath = "/testing/database/group_nonexisting_entry_does_not_group.xml";
            storageAccess.AddDatabaseFile(databaseFilePath, 0);
            var database = UserSpecifiedProgramInformationTable.Initialize(databaseFilePath) as UserSpecifiedProgramInformationTable;
            var entry = new UserSpecifiedProgramInformation(0x98765432, "My dev ROM");
            Assert.True(database.AddEntry(entry));

            var newEntry = new UserSpecifiedProgramInformation(0x23457689, "My other dev ROM");
            Assert.False(database.GroupWithExistingEntry(entry, newEntry));
        }

        private static string CreateDatabaseXmlString(int numberOfEntries)
        {
            var databaseStringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>").AppendLine();
            databaseStringBuilder.AppendFormat("<{0}>", UserSpecifiedProgramInformationTable.XmlRootName).AppendLine();
#if false
            databaseStringBuilder.AppendFormat("  <{0}>", UserSpecifiedProgramInformationTable.XmlRomsCollectionName);

            for (var i = 0; i < numberOfEntries; ++i)
            {
            }

            databaseStringBuilder.AppendFormat("  </{0}>", UserSpecifiedProgramInformationTable.XmlRomsCollectionName);
#endif
            databaseStringBuilder.AppendFormat("</{0}>", UserSpecifiedProgramInformationTable.XmlRootName).AppendLine();
            return databaseStringBuilder.ToString();
        }

        private class UserSpecifiedProgramInformationTableTestsStorageAccess : CachedResourceStorageAccess<UserSpecifiedProgramInformationTableTestsStorageAccess>
        {
            private readonly HashSet<Stream> _databaseStreamsCache = new HashSet<Stream>();

            public void AddDatabaseFile(string databaseFilePath, int numberOfEntries)
            {
                lock (_databaseStreamsCache)
                {
                    using (var databaseStream = OpenOrCreate(databaseFilePath, -1))
                    {
                        var databaseString = CreateDatabaseXmlString(numberOfEntries);
                        var databaseBytes = Encoding.UTF8.GetBytes(databaseString);
                        databaseStream.Write(databaseBytes, 0, databaseBytes.Length);
                        _databaseStreamsCache.Add(Open(databaseFilePath));
                        databaseStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
        }
    }
}

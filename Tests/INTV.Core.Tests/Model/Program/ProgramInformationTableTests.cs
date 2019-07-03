// <copyright file="ProgramInformationTableTests.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramInformationTableTests
    {
        [Fact]
        public void ProgramInformationTable_Initialize_()
        {
            IReadOnlyList<StorageLocation> romPaths;
            var storageAccess = ProgramInformationTableTestsStorageAccess.Initialize(out romPaths, null);
            var database0FilePath = storageAccess.CreateLocation("/testing/database/user_specified_database_0.xml");
            storageAccess.AddDatabaseFile(database0FilePath, 2);
            var database1FilePath = storageAccess.CreateLocation("/testing/database/user_specified_database_1.xml");
            storageAccess.AddDatabaseFile(database1FilePath, 3);
            var database2FilePath = storageAccess.CreateLocation("/testing/database/user_specified_bogus_database.xml");

            var database = ProgramInformationTable.Initialize(
                new[]
                {
                    new ProgramInformationTableDescriptor(database0FilePath, p => UserSpecifiedProgramInformationTable.Initialize(p)),
                    new ProgramInformationTableDescriptor(database1FilePath, p => UserSpecifiedProgramInformationTable.Initialize(p)),
                    new ProgramInformationTableDescriptor(database2FilePath, p => UserSpecifiedProgramInformationTable.Initialize(p)),
                });

            Assert.NotNull(database);
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

        private class ProgramInformationTableTestsStorageAccess : CachedResourceStorageAccess<ProgramInformationTableTestsStorageAccess>
        {
            private readonly HashSet<Stream> _databaseStreamsCache = new HashSet<Stream>();

            public void AddDatabaseFile(StorageLocation databaseFilePath, int numberOfEntries)
            {
                lock (_databaseStreamsCache)
                {
                    using (var databaseStream = OpenOrCreate(databaseFilePath.Path, -1))
                    {
                        var databaseString = CreateDatabaseXmlString(numberOfEntries);
                        var databaseBytes = Encoding.UTF8.GetBytes(databaseString);
                        databaseStream.Write(databaseBytes, 0, databaseBytes.Length);
                        _databaseStreamsCache.Add(Open(databaseFilePath.Path));
                        databaseStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
        }
    }
}

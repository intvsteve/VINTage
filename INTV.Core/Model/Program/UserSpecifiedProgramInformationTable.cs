// <copyright file="UserSpecifiedProgramInformationTable.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This information table stores locally specified program information. This is typically for
    /// program the user has developed, or others have distributed only in ROM format.
    /// </summary>
    [System.Xml.Serialization.XmlRoot(XmlRootName)]
    public class UserSpecifiedProgramInformationTable : ProgramInformationTable
    {
        /// <summary>Name of the root XML element in a user-specified program information database.</summary>
        internal const string XmlRootName = "UserSpecifiedProgramInformationTable";

        /// <summary>Name of XML element that stores the collection of user-specified program information objects in a user-specified program information database.</summary>
        internal const string XmlRomsCollectionName = "Programs";

        /// <summary>Name of XML element that stores a single user-specified program information object in a user-specified program information database.</summary>
        internal const string XmlRomInfoName = "UserSpecifiedProgramInformation";

        private readonly List<UserSpecifiedProgramInformation> _programs;

        #region Constructors

        private UserSpecifiedProgramInformationTable()
        {
            _programs = new List<UserSpecifiedProgramInformation>();
        }

        #endregion // Constructors

        #region IProgramInformationTable

        /// <inheritdoc />
        public override IEnumerable<IProgramInformation> Programs
        {
            get { return _programs; }
        }

        #endregion // IProgramInformationTable

        /// <summary>
        /// Loads or creates a new instance of the user-defined program information table.
        /// </summary>
        /// <param name="localFilePath">Absolute path to a file storing the user-specified program information.</param>
        /// <returns>If the file path is valid, the returned table contains the user's locally defined program information. Otherwise, a new,
        /// empty table is returned.</returns>
        public static IProgramInformationTable Initialize(string localFilePath)
        {
            UserSpecifiedProgramInformationTable table = null;
            try
            {
                using (var fileStream = StreamUtilities.OpenFileStream(localFilePath))
                {
                    if (fileStream != null)
                    {
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(UserSpecifiedProgramInformationTable));
                        table = serializer.Deserialize(fileStream) as UserSpecifiedProgramInformationTable;
                    }
                }
            }
            catch (Exception)
            {
                // TODO Silently fail to load user-defined ROMs for now. Eventually need to report back to user once this is supported.
                table = null;
            }
            if (table == null)
            {
                // Return an empty instance, which can be harmlessly merged into the master in-memory database.
                table = new UserSpecifiedProgramInformationTable();
            }
            return table;
        }

        /// <summary>
        /// Add a new entry to the information table.
        /// </summary>
        /// <param name="program">The program to add to the table.</param>
        /// <returns><c>true</c> if the program was added. If it was not, then a match based on CRC was found.</returns>
        public bool AddEntry(UserSpecifiedProgramInformation program)
        {
            // Check the incoming program's CRCs against the CRCs of existing entries. If any CRC in the incoming program matches a CRC of an entry, then  it's a match!
            // TODO: Move to also include CfgCrc where appropriate.  Ideally we would canonicalize CFG file content in memory and then check.
            var existingEntry = _programs.FirstOrDefault(e => e.Crcs.FirstOrDefault(c => program.Crcs.FirstOrDefault(crc => crc.Crc == c.Crc) != null) != null);
            bool addedEntry = existingEntry == null;
            if (addedEntry)
            {
                _programs.Add(program);
            }
            return addedEntry;
        }

        /// <summary>
        /// Group an entry with an existing one in the table.
        /// </summary>
        /// <param name="program">The program to be combined with another in the table.</param>
        /// <param name="groupWithEntry">The entry to group with.</param>
        /// <returns><c>true</c> if the new entry was merged into the existing one.</returns>
        public bool GroupWithExistingEntry(UserSpecifiedProgramInformation program, UserSpecifiedProgramInformation groupWithEntry)
        {
            // TODO: Improvement should be done regarding CFG CRCs.
            var newCrcs = Enumerable.Empty<CrcData>();
            if (_programs.Contains(groupWithEntry))
            {
                var groupEntryCrcs = groupWithEntry.Crcs.Select(c => c.Crc).ToList();
                newCrcs = program.Crcs.Where(c => !groupEntryCrcs.Contains(c.Crc));
            }
            var groupedWithEntry = newCrcs.Any();
            if (groupedWithEntry)
            {
                foreach (var crc in newCrcs)
                {
                    groupWithEntry.AddCrc(crc.Crc, crc.Description, crc.Incompatibilities);
                }
            }
            return groupedWithEntry;
        }
    }
}

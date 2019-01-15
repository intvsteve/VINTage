// <copyright file="MergedProgramInformationTable.cs" company="INTV Funhouse">
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

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This implementation of ProgramInformationTable provides a way to coalesce several PITs into a single table.
    /// </summary>
    internal class MergedProgramInformationTable : ProgramInformationTable
    {
        private readonly ConcurrentDictionary<uint, IProgramInformation> _programs = new ConcurrentDictionary<uint, IProgramInformation>();

        /// <inheritdoc />
        public override IEnumerable<IProgramInformation> Programs
        {
            get { return _programs.Values; }
        }

        /// <inheritdoc />
        public override IProgramInformation FindProgram(ProgramIdentifier programIdentifier)
        {
            var programInformation = base.FindProgram(programIdentifier);
            System.Diagnostics.Debug.WriteLineIf((programIdentifier.OtherData != 0) && (programInformation != null), "Support for ProgramIdentifier lookups not implemented.");
            return programInformation;
        }

        /// <summary>
        /// Merges another IProgramInformationTable into an existing one, based on the CRC entries of each table.
        /// </summary>
        /// <param name="tableToMerge">The table to merge into this one.</param>
        /// <returns>An enumerable of any conflicting entries. The keys are the entries from tableToMerge, while the value entries are the preexisting ones in this table.</returns>
        internal List<KeyValuePair<IProgramInformation, IProgramInformation>> MergeTable(IProgramInformationTable tableToMerge)
        {
            List<KeyValuePair<IProgramInformation, IProgramInformation>> conflictingEntries = new List<KeyValuePair<IProgramInformation, IProgramInformation>>();

            if (tableToMerge != null)
            {
                foreach (var programInfo in tableToMerge.Programs)
                {
                    foreach (var crcEntry in programInfo.Crcs)
                    {
                        if (_programs.ContainsKey(crcEntry.Crc))
                        {
                            conflictingEntries.Add(new KeyValuePair<IProgramInformation, IProgramInformation>(programInfo, _programs[crcEntry.Crc]));
                        }
                        else
                        {
                            _programs[crcEntry.Crc] = programInfo;
                        }
                    }
                }
            }

            return conflictingEntries;
        }

        /// <summary>
        /// Removes an entry from the database if possible.
        /// </summary>
        /// <param name="entryCrc">The unique ID (typically the CRC32 of a ROM) to remove from the database.</param>
        /// <returns><c>true</c> if the entry was removed, false otherwise.</returns>
        /// <remarks>This is presently only exposed for testing purposes.</remarks>
        internal bool RemoveEntry(uint entryCrc)
        {
            IProgramInformation entry;
            var removed = _programs.TryRemove(entryCrc, out entry);
            return removed;
        }

        /// <inheritdoc />
        protected override IProgramInformation FindProgramCore(ProgramIdentifier programIdentifier)
        {
            IProgramInformation programInfo;
            if (!_programs.TryGetValue(programIdentifier.DataCrc, out programInfo))
            {
                programInfo = null;
            }
            return programInfo;
        }
    }
}

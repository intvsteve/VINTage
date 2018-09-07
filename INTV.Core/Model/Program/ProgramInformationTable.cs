// <copyright file="ProgramInformationTable.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Provides access to a table intended for use as the default lookup table
    /// </summary>
    public abstract class ProgramInformationTable : IProgramInformationTable
    {
        private static readonly Lazy<MergedProgramInformationTable> Instance = new Lazy<MergedProgramInformationTable>(() => new MergedProgramInformationTable());

        #region Properties

        /// <summary>
        /// Gets the default program information table.
        /// </summary>
        public static IProgramInformationTable Default
        {
            get { return Instance.Value; }
        }

        #region IProgramInformationTable

        /// <inheritdoc />
        public abstract IEnumerable<IProgramInformation> Programs { get; }

        #endregion // IProgramInformationTable

        #endregion Properties

        /// <summary>
        /// Initializes the general program information database system with a specific string decoder and program information table descriptions.
        /// </summary>
        /// <param name="localInfoTables">Descriptions of how to access database tables.</param>
        /// <returns>The initialized program information table.</returns>
        public static IProgramInformationTable Initialize(ProgramInformationTableDescriptor[] localInfoTables)
        {
            List<IProgramInformationTable> localTables = new List<IProgramInformationTable>();
            var conflicts = new List<KeyValuePair<IProgramInformation, IProgramInformation>>();
            var instance = Instance.Value;
            foreach (var localInfoTable in localInfoTables)
            {
                var localTable = localInfoTable.Factory(localInfoTable.FilePath);
                conflicts.AddRange(instance.MergeTable(localTable));
                localTables.Add(localTable);
            }
            var confilictingWithIntvFunhouse = instance.MergeTable(INTV.Core.Restricted.Model.Program.IntvFunhouseXmlProgramInformationTable.Instance);
            if (confilictingWithIntvFunhouse.Any())
            {
                System.Diagnostics.Debug.WriteLine("Found conflicts with INTV Funhouse database.");
            }
            var conflictingWithJzIntv = instance.MergeTable(UnmergedProgramInformationTable.Instance);
            if (conflictingWithJzIntv.Any())
            {
                System.Diagnostics.Debug.WriteLine("Found conflicts with jzIntv / Unmerged ROMs database.");
            }
            return instance;
        }

        #region IProgramInformationTable

        /// <inheritdoc />
        /// <remarks>The default implementation simply looks in the default database for a program entry.</remarks>
        public virtual IProgramInformation FindProgram(uint crc)
        {
            return Default.FindProgram(crc);
        }

        /// <inheritdoc />
        /// <remarks>The default implementation simply looks in the default database for a program entry.</remarks>
        public virtual IProgramInformation FindProgram(ProgramIdentifier programIdentifier)
        {
            return Default.FindProgram(programIdentifier);
        }

        #endregion // IProgramInformationTable
    }
}

// <copyright file="IntvFunhouseXmlProgramInformationTable.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Model;
using INTV.Core.Model.Program;

namespace INTV.Core.Restricted.Model.Program
{
    /// <summary>
    /// This class reflects the relevant parts of the contents of the 'gameinfo' table from intvfunhouse.com.
    /// </summary>
    /// <remarks>This class must be public in order for Xml.Serialization to work. Do not use it directly.</remarks>
    /// <remarks>The XML copy of this database requires the a transformation of the raw database from the website
    /// in order for the XML serialization feature to work. The xslt also removes a great deal of unneeded data.</remarks>
    [System.Xml.Serialization.XmlRoot("program_info")]
    public class IntvFunhouseXmlProgramInformationTable : ProgramInformationTable
    {
        private static IntvFunhouseXmlProgramInformationTable _instance = LoadFromXmlResource();

        private IntvFunhouseXmlProgramInformation[] _programInfos;

        #region Properties

        /// <summary>
        /// Gets the INTV Funhouse data table.
        /// </summary>
        public static IntvFunhouseXmlProgramInformationTable Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets or sets the program information array. See class documentation for more information.
        /// </summary>
        [System.Xml.Serialization.XmlArray("programs")]
        [System.Xml.Serialization.XmlArrayItem("gameinfo")]
        public IntvFunhouseXmlProgramInformation[] XmlPrograms
        {
            get { return _programInfos; }
            set { _programInfos = value; }
        }

        /// <inheritdoc/>
        public override IEnumerable<INTV.Core.Model.Program.IProgramInformation> Programs
        {
            get { return _programInfos; }
        }

        #endregion // Properties

        /// <summary>
        /// Initialize the program information table from the embedded XML resource.
        /// </summary>
        /// <returns>The program information table.</returns>
        public static IntvFunhouseXmlProgramInformationTable LoadFromXmlResource()
        {
            using (var resourceStream = typeof(IRom).Assembly.GetManifestResourceStream(typeof(IRom).Namespace + ".Resources.intvfunhouse_gameinfo.xml"))
            {
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(IntvFunhouseXmlProgramInformationTable));
                var programInfoTable = xmlSerializer.Deserialize(resourceStream) as IntvFunhouseXmlProgramInformationTable;
                return programInfoTable;
            }
        }
 
        /// <summary>
        /// Adds an entry to the table if it does not already exist.
        /// </summary>
        /// <param name="programInfo">The program information to add.</param>
        /// <returns><c>true</c> if the entry was added, otherwise <c>false</c>.</returns>
        public bool AddEntry(IProgramInformation programInfo)
        {
            throw new System.NotImplementedException(Resources.Strings.ProgramInformationTable_NoEditsToDefaultDatabase);
        }
   }
}

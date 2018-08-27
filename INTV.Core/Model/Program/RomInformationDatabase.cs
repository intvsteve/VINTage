// <copyright file="RomInformationDatabase.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

using System.Linq;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This class represents the rominfo database as exported from phpMyAdmin at INTV Funhouse.
    /// </summary>
    /// <remarks>NOTE: This may be fragile, depending on the version of MySQL and phpMyAdmin used at the website.</remarks>
    [System.Xml.Serialization.XmlRoot(DatabaseRootName)]
    public class RomInformationDatabase
    {
        private const string DatabaseRootName = "pma_xml_export";
        private const string DatabaseName = "database";
        private const string RomInfoEntryXmlName = "table";

        /// <summary>
        /// Initializes a new instance of <see cref="RomInformationDatabase"/>.
        /// </summary>
        public RomInformationDatabase()
        {
            RomInfos = Enumerable.Empty<RomInformation>().ToArray();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the collection of <see cref="RomInformation"/> objects in the database.
        /// </summary>
        [System.Xml.Serialization.XmlArray(DatabaseName)]
        [System.Xml.Serialization.XmlArrayItem(RomInfoEntryXmlName, typeof(RomInformation))]
        public RomInformation[] RomInfos { get; set; }

        #endregion // Properties
    }
}

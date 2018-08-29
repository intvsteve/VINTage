// <copyright file="XmlRomInformation.cs" company="INTV Funhouse">
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
    /// This class represents an entry from the rominfo database at INTV Funhouse.
    /// </summary>
    [System.Xml.Serialization.XmlType("table")]
    public class XmlRomInformation
    {
        /// <summary>
        /// Expected name of each "table" entry in the database. phpMyAdmin imports / exports the MySQL database as a
        /// collection of tables, each named "rominfo" using a name attribute.
        /// </summary>
        public const string RomInfoDatabaseEntryName = "rominfo";

        private const string RomInfoNameAttributeName = "name";

        /// <summary>
        /// Initializes a new instance of <see cref="XmlRomInformation"/>.
        /// </summary>
        public XmlRomInformation()
        {
            RomInfoDatabaseColumns = Enumerable.Empty<XmlRomInformationDatabaseColumn>().ToArray();
        }

        #region Properties

        #region XML Properties

        /// <summary>
        /// Gets or sets the name of the database entry. This is presumed to be the value of <see cref="RomInfoDatabaseEntryName"/>.
        /// </summary>
        [System.Xml.Serialization.XmlAttribute(RomInfoNameAttributeName)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ROM info database columns array.
        /// </summary>
        [System.Xml.Serialization.XmlElement(XmlRomInformationDatabaseColumn.RomInfoColumnXmlTypeName, typeof(XmlRomInformationDatabaseColumn))]
        public XmlRomInformationDatabaseColumn[] RomInfoDatabaseColumns { get; set; }

        #endregion // XML Properties

        #endregion // Properties
    }
}

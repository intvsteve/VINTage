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

using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Creates a new instance of <see cref="XmlRomInformation"/> with default column values.
        /// </summary>
        /// <returns>A new instance of <see cref="XmlRomInformation"/>.</returns>
        public static XmlRomInformation CreateDefault()
        {
            var columnNames = Enum.GetValues(typeof(XmlRomInformationDatabaseColumnName)).Cast<XmlRomInformationDatabaseColumnName>();
            var xmlRomInformation = new XmlRomInformation { RomInfoDatabaseColumns = columnNames.Select(n => new XmlRomInformationDatabaseColumn(n)).ToArray() };
            return xmlRomInformation;
        }

        /// <summary>
        /// Gets a column.
        /// </summary>
        /// <param name="name">Identifies the column to get.</param>
        /// <returns>The database column, <c>null</c> if the column identified by <paramref name="name"/> cannot be found.</returns>
        public XmlRomInformationDatabaseColumn GetColumn(XmlRomInformationDatabaseColumnName name)
        {
            return GetColumn(name, requiredColumn: false);
        }

        /// <summary>
        /// Gets a column.
        /// </summary>
        /// <param name="name">Identifies the column to get.</param>
        /// <param name="requiredColumn">If <c>true</c>, the column must be present. If not, an exception is thrown.</param>
        /// <returns>The database column, <c>null</c> if the column identified by <paramref name="name"/> cannot be found and <paramref name="requiredColumn"/> is <c>false</c>.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if <paramref name="requiredColumn"/> is <c>true</c> and the column identified by <paramref name="name"/> cannot be found.</exception>
        public XmlRomInformationDatabaseColumn GetColumn(XmlRomInformationDatabaseColumnName name, bool requiredColumn)
        {
            if (requiredColumn)
            {
                return RomInfoDatabaseColumns.First(c => c.Name == name.ToString());
            }
            return RomInfoDatabaseColumns.FirstOrDefault(c => c.Name == name.ToString());
        }

        /// <summary>
        /// Adds the given column with the given data.
        /// </summary>
        /// <param name="name">Identifies the column to add.</param>
        /// <param name="value">The value to assign to the column.</param>
        /// <remarks>No validation of <paramref name="value"/> is done.</remarks>
        /// <exception cref="ArgumentException">Thrown if the column already exists.</exception>
        public void AddColumn(XmlRomInformationDatabaseColumnName name, string value)
        {
            if (RomInfoDatabaseColumns != null)
            {
                if (GetColumn(name) != null)
                {
                    throw new ArgumentException();
                }
            }

            var column = new XmlRomInformationDatabaseColumn(name);
            if (RomInfoDatabaseColumns == null)
            {
                RomInfoDatabaseColumns = new List<XmlRomInformationDatabaseColumn>(new[] { column }).ToArray();
            }
            else
            {
                var replacementColumns = new List<XmlRomInformationDatabaseColumn>(RomInfoDatabaseColumns);
                replacementColumns.Add(column);
                RomInfoDatabaseColumns = replacementColumns.ToArray();
            }
        }

        /// <summary>
        /// Removes the given column.
        /// </summary>
        /// <param name="name">Identifies the column to remove.</param>
        /// <returns><c>true</c> if the column is removed, <c>false</c> otherwise.</returns>
        public bool RemoveColumn(XmlRomInformationDatabaseColumnName name)
        {
            var removed = (RomInfoDatabaseColumns != null) && (GetColumn(name) != null);
            if (removed)
            {
                var replacementColumns = RomInfoDatabaseColumns.Where(c => c.Name != name.ToString());
                RomInfoDatabaseColumns = replacementColumns.ToArray();
            }
            return removed;
        }
    }
}

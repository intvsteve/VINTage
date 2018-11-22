// <copyright file="XmlRomInformationDatabaseColumn.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.Linq;
using INTV.Core.Resources;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    [System.Xml.Serialization.XmlType(RomInfoColumnXmlTypeName)]
    public class XmlRomInformationDatabaseColumn
    {
        /// <summary>
        /// The XML element name of a rominfo property in the MySql database as exported by phpMyAdmin.
        /// </summary>
        internal const string RomInfoColumnXmlTypeName = "column";

        private const string RomInfoColumnNameAttributeName = "name";

        /// <summary>
        /// Default values for database columns. Values not defined assumed to be empty (null) string.
        /// </summary>
        private static readonly Dictionary<XmlRomInformationDatabaseColumnName, string> DefaultColumnValues = new Dictionary<XmlRomInformationDatabaseColumnName, string>()
        {
                  { XmlRomInformationDatabaseColumnName.crc, "0" },
                  { XmlRomInformationDatabaseColumnName.crc_2, "0" },
                  { XmlRomInformationDatabaseColumnName.platform, "Intellivision" },
                  { XmlRomInformationDatabaseColumnName.type, "Program" },
                  { XmlRomInformationDatabaseColumnName.release_date, "0000-00-00" },
                  { XmlRomInformationDatabaseColumnName.ntsc, "-1" },
                  { XmlRomInformationDatabaseColumnName.pal, "-1" },
                  { XmlRomInformationDatabaseColumnName.general_features, "-1" },
                  { XmlRomInformationDatabaseColumnName.kc, "-1" },
                  { XmlRomInformationDatabaseColumnName.sva, "-1" },
                  { XmlRomInformationDatabaseColumnName.ivoice, "-1" },
                  { XmlRomInformationDatabaseColumnName.intyii, "-1" },
                  { XmlRomInformationDatabaseColumnName.ecs, "-1" },
                  { XmlRomInformationDatabaseColumnName.tutor, "-1" },
                  { XmlRomInformationDatabaseColumnName.icart, "-1" },
                  { XmlRomInformationDatabaseColumnName.cc3, "-1" },
                  { XmlRomInformationDatabaseColumnName.jlp, "-1" },
                  { XmlRomInformationDatabaseColumnName.jlp_savegame, "-1" },
                  { XmlRomInformationDatabaseColumnName.lto_flash, "-1" },
                  { XmlRomInformationDatabaseColumnName.bee3, "-1" },
                  { XmlRomInformationDatabaseColumnName.hive, "-1" },
                  { XmlRomInformationDatabaseColumnName.box_variant, "-1" },
                  { XmlRomInformationDatabaseColumnName.screenshot, "-1" },
        };

        /// <summary>
        /// Initialize a new instance of <see cref="XmlRomInformationDatabaseColumn"/>.
        /// </summary>
        public XmlRomInformationDatabaseColumn()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="XmlRomInformationDatabaseColumn"/> for a specific column.
        /// </summary>
        /// <param name="name">The name of the database column to create.</param>
        public XmlRomInformationDatabaseColumn(XmlRomInformationDatabaseColumnName name)
        {
            Name = name.ToString();
            string defaultValue;
            if (DefaultColumnValues.TryGetValue(name, out defaultValue))
            {
                Value = defaultValue;
            }
            else
            {
                Value = string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <remarks>The setter must be public so XML serialization can be used to set the value</remarks>
        [System.Xml.Serialization.XmlAttribute(RomInfoColumnNameAttributeName)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the string value stored in the column.
        /// </summary>
        /// <remarks>The setter must be public so XML serialization can be used to set the value</remarks>
        [System.Xml.Serialization.XmlText]
        public string Value { get; set; }
    }
}

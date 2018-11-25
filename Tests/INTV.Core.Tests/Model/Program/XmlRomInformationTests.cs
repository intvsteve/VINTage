// <copyright file="XmlRomInformationTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class XmlRomInformationTests
    {
        [Fact]
        public void XmlRomInformation_CreateDefault_CreatesEmptyXmlRomInformation()
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();

            Assert.NotNull(xmlRomInformation);
            Assert.Null(xmlRomInformation.Name);
            foreach (var column in xmlRomInformation.RomInfoDatabaseColumns)
            {
                Assert.NotNull(column.Value);
            }
        }

        [Fact]
        public void XmlRomInformation_CreateDefaultGetColumn_AllColumnsPresent()
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();
            Assert.NotNull(xmlRomInformation);

            var columnNames = Enum.GetValues(typeof(XmlRomInformationDatabaseColumnName)).Cast<XmlRomInformationDatabaseColumnName>();

            foreach (var columnName in columnNames)
            {
                Assert.NotNull(xmlRomInformation.GetColumn(columnName));
            }
        }

        [Fact]
        public void XmlRomInformation_CreateDefaultGetColumnAsRequired_AllColumnsPresent()
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();
            Assert.NotNull(xmlRomInformation);

            var columnNames = Enum.GetValues(typeof(XmlRomInformationDatabaseColumnName)).Cast<XmlRomInformationDatabaseColumnName>();

            foreach (var columnName in columnNames)
            {
                Assert.NotNull(xmlRomInformation.GetColumn(columnName, requiredColumn: true));
            }
        }

        [Fact]
        public void XmlRomInformation_AddColumnToEmptyInstance_AddsColumn()
        {
            var xmlRomInformation = new XmlRomInformation();
            Assert.Equal(0, xmlRomInformation.RomInfoDatabaseColumns.Length);

            var value = "value";
            xmlRomInformation.AddColumn(XmlRomInformationDatabaseColumnName.format, value);

            Assert.NotNull(xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.format));
            Assert.Equal(value, xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.format).Value);
        }

        [Fact]
        public void XmlRomInformation_AddColumnToInstanceWithNullColumns_AddsColumn()
        {
            var xmlRomInformation = new XmlRomInformation();
            xmlRomInformation.RomInfoDatabaseColumns = null;
            Assert.Null(xmlRomInformation.RomInfoDatabaseColumns);

            var value = "value";
            xmlRomInformation.AddColumn(XmlRomInformationDatabaseColumnName.format, value);

            Assert.NotNull(xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.format));
            Assert.Equal(value, xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.format).Value);
        }

        [Fact]
        public void XmlRomInformation_AddColumnToDefaultInstance_ThrowsArgumentException()
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();

            Assert.Throws<ArgumentException>(() => xmlRomInformation.AddColumn(XmlRomInformationDatabaseColumnName.format, null));
        }

        [Fact]
        public void XmlRomInformation_RemoveColumnFromEmptyInstance_ReturnsFalse()
        {
            var xmlRomInformation = new XmlRomInformation();
            Assert.Equal(0, xmlRomInformation.RomInfoDatabaseColumns.Length);

            Assert.False(xmlRomInformation.RemoveColumn(XmlRomInformationDatabaseColumnName.format));
        }

        [Fact]
        public void XmlRomInformation_RemoveColumnFromInstanceWithNullColumns_ReturnsFalse()
        {
            var xmlRomInformation = new XmlRomInformation();
            xmlRomInformation.RomInfoDatabaseColumns = null;
            Assert.Null(xmlRomInformation.RomInfoDatabaseColumns);

            Assert.False(xmlRomInformation.RemoveColumn(XmlRomInformationDatabaseColumnName.format));
        }

        [Fact]
        public void XmlRomInformation_RemoveColumnFromDefaultInstance_ReturnsTrue()
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();

            Assert.True(xmlRomInformation.RemoveColumn(XmlRomInformationDatabaseColumnName.game_docs));
        }

        [Fact]
        public void XmlRomInformation_RemoveColumnTwiceFromDefaultInstance_ReturnsFalse()
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();

            Assert.True(xmlRomInformation.RemoveColumn(XmlRomInformationDatabaseColumnName.game_docs));
            Assert.False(xmlRomInformation.RemoveColumn(XmlRomInformationDatabaseColumnName.game_docs));
        }
    }
}

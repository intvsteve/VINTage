// <copyright file="XmlRomInformationDatabaseColumnTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class XmlRomInformationDatabaseColumnTests
    {
        [Fact]
        public void XmlRomInformationDatabaseColumn_Create_HasNullNameAndValue()
        {
            var column = new XmlRomInformationDatabaseColumn();

            Assert.Null(column.Name);
            Assert.Null(column.Value);
        }

        public static IEnumerable<object[]> XmlRomInformationDatabaseColumnTestData
        {
            get
            {
                var columnNames = Enum.GetValues(typeof(XmlRomInformationDatabaseColumnName)).Cast<XmlRomInformationDatabaseColumnName>();
                foreach (var columnName in columnNames)
                {
                    yield return new object[] { columnName };
                }
            }
        }

        [Theory]
        [MemberData("XmlRomInformationDatabaseColumnTestData")]
        public void XmlRomInformationDatabaseColumn_CreateWithColumnName_ValueNotNull(XmlRomInformationDatabaseColumnName name)
        {
            var column = new XmlRomInformationDatabaseColumn(name);

            Assert.Equal(name.ToString(), column.Name);
            Assert.NotNull(column.Value);
        }
    }
}

﻿// <copyright file="XmlRomInformationDatabaseColumnNameHelpersTests.cs" company="INTV Funhouse">
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
    public class XmlRomInformationDatabaseColumnNameHelpersTests
    {
        public static IEnumerable<object[]> StringToXmlRomInformationDatabaseColumnNameTestData
        {
            get
            {
                yield return new object[] { null, XmlRomInformationDatabaseColumnName.Invalid };
                yield return new object[] { string.Empty, XmlRomInformationDatabaseColumnName.Invalid };
                yield return new object[] { "ArgleBargle fleepnarg", XmlRomInformationDatabaseColumnName.Invalid };

                foreach (var enumValue in Enum.GetValues(typeof(XmlRomInformationDatabaseColumnName)))
                {
                    yield return new object[] { enumValue.ToString(), enumValue };
                }
            }
        }

        [Theory]
        [MemberData("StringToXmlRomInformationDatabaseColumnNameTestData")]
        public void XmlRomInformationDatabaseColumnName_ToXmlRomInformationDatabaseColumnName_ProducesExpectedEnumValue(string columnName, XmlRomInformationDatabaseColumnName expectedColumnName)
        {
            Assert.Equal(expectedColumnName, columnName.ToXmlRomInformationDatabaseColumnName());
        }
    }
}
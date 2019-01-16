// <copyright file="IntvFunhouseXmlProgramInformationTableTests.cs" company="INTV Funhouse">
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
using INTV.Core.Restricted.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class IntvFunhouseXmlProgramInformationTableTests
    {
        [Fact]
        public void IntvFunhouseXmlProgramInformationTable_EnsureTableLoads_TableIsNotEmpty()
        {
            var table = IntvFunhouseXmlProgramInformationTable.Instance;

            Assert.NotNull(table);
            Assert.True(table.XmlPrograms.Any());
            Assert.True(table.Programs.Any());
        }

        [Fact]
        public void IntvFunhouseXmlProgramInformationTable_AddEntry_ThrowsNotImplementedException()
        {
            var table = IntvFunhouseXmlProgramInformationTable.Instance;
            Assert.NotNull(table);

            Assert.Throws<NotImplementedException>(() => table.AddEntry(null));
        }
    }
}

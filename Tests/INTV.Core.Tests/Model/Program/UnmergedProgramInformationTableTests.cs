// <copyright file="UnmergedProgramInformationTableTests.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class UnmergedProgramInformationTableTests
    {
        [Theory]
        [InlineData(0xCBCE86F7u)]
        [InlineData(0xC72E969Au)]
        [InlineData(0xB2A488E3u)]
        [InlineData(0xDE7579Du)]
        public void UnmergedProgramInformationTable_FindProgramThatExistsInDatabase_ReturnsValidEntry(uint crc)
        {
            Assert.NotNull(UnmergedProgramInformationTable.Instance.FindProgram(crc));
        }

        [Theory]
        [InlineData(1u)]
        [InlineData(2u)]
        [InlineData(3u)]
        [InlineData(4u)]
        public void UnmergedProgramInformationTable_FindProgramThatDoesNotInDatabase_ReturnsInvalidEntry(uint crc)
        {
            Assert.Null(UnmergedProgramInformationTable.Instance.FindProgram(crc));
        }
    }
}

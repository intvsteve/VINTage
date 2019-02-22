// <copyright file="RomInfoIndexTests.cs" company="INTV Funhouse">
// Copyright (c) 2018-2019 All Rights Reserved
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
using INTV.Core.Model;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomInfoIndexTests
    {
        [Fact]
        public void RomInfoEnumerable_IndexWithNoneWithNullEnumerable_ReturnsEmptyString()
        {
            IEnumerable<string> results = null;
            Assert.Empty(results.GetRomInfoString(RomInfoIndex.None));
        }

        [Theory]
        [InlineData(RomInfoIndex.Name)]
        [InlineData(RomInfoIndex.Copyright)]
        [InlineData(RomInfoIndex.ShortName)]
        [InlineData(RomInfoIndex.NumEntries)]
        public void RomInfoEnumerable_IndexNotNoneWithNullEnumerable_ThrowsArgumentNullException(RomInfoIndex infoIndex)
        {
            IEnumerable<string> results = null;
            Assert.Throws<ArgumentNullException>(() => results.GetRomInfoString(infoIndex));
        }

        [Fact]
        public void RomInfoEnumerable_IndexIsBogusButInTheEnumerable_ReturnsExpectedValue()
        {
            var testValues = new[] { "0", "a", "b", "z", "arg", "gra", "flern" };
            var testIndex = RomInfoIndex.NumEntries + 2;

            Assert.Equal("gra", testValues.GetRomInfoString(testIndex));
        }

        [Fact]
        public void RomInfoEnumerable_ValueAtIndexHasExtraSpaces_ReturnsDeSpacedAndTrimmedValue()
        {
            var testValues = new[] { "  The  Show    Must   Go     ONNNNNNNNNNN!         ", "@Copr 1978" };

            Assert.Equal("The Show Must Go ONNNNNNNNNNN!", testValues.GetRomInfoString(RomInfoIndex.Name));
        }

        [Theory]
        [InlineData(RomInfoIndex.Name)]
        [InlineData(RomInfoIndex.Copyright)]
        [InlineData(RomInfoIndex.ShortName)]
        public void RomInfoEnumerable_BogusDataData_ValuesAreRetained(RomInfoIndex romInfoIndex)
        {
            var badStringBytes = new byte[255];
            for (var i = 0; i < 255; ++i)
            {
                badStringBytes[i] = (byte)(255 - i);
            }
            var badString = System.Text.Encoding.UTF8.GetString(badStringBytes, 0, badStringBytes.Length);

            var testValues = Enumerable.Repeat(badString, (int)RomInfoIndex.NumEntries).ToArray();

            var expectedResult = System.Text.RegularExpressions.Regex.Replace(badString, @"\s+", " ").Trim();
            Assert.Equal(expectedResult, testValues.GetRomInfoString(romInfoIndex));
        }
    }
}

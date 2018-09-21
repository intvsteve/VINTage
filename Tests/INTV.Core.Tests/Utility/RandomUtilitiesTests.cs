// <copyright file="RandomUtilitiesTests.cs" company="INTV Funhouse">
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

using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class RandomUtilitiesTests
    {
        [Fact]
        public void RandomUtilities_Next32_CodeCoverage()
        {
            var random = RandomUtilities.Next32();
            Assert.NotEmpty(random.ToString()); // totally bogus "test"
        }

        [Fact]
        public void RandomUtilities_Next24_CodeCoverage()
        {
            for (int i = 0; i < 10000; ++i)
            {
                var random = RandomUtilities.Next24();
                Assert.Equal(0u, random & 0xFF000000); // 24-bit random should never have anything in the upper byte
            }
        }

        [Fact]
        public void RandomUtilities_Next16_CodeCoverage()
        {
            var random = RandomUtilities.Next16();
            Assert.NotEmpty(random.ToString()); // totally bogus "test"
        }
    }
}

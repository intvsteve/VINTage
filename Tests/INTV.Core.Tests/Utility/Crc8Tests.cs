// <copyright file="Crc8Tests.cs" company="INTV Funhouse">
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
    public class Crc8Tests
    {
        [Fact]
        public void Crc8_OfNull_ThrowsNullReferenceException()
        {
            Assert.Throws<System.NullReferenceException>(() => Crc8.OfBlock(null));
        }

        [Fact]
        public void Crc8_OfBlock_IsCorrect()
        {
            var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var crc8 = Crc8.OfBlock(testData);
            Assert.Equal(0xE9, crc8);
        }
    }
}

// <copyright file="Crc16Tests.cs" company="INTV Funhouse">
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
    public class Crc16Tests
    {
        [Fact]
        public void Crc16_OfNull_ThrowsNullReferenceException()
        {
            Assert.Throws<System.NullReferenceException>(() => Crc16.OfBlock(null, Crc16.InitialValue));
        }

        [Fact]
        public void Crc16_UpdateFromInitialValue_IsCorrect()
        {
            var crc16 = Crc16.Update(Crc16.InitialValue, 4);
            Assert.Equal(0xA174, crc16);
        }

        [Fact]
        public void Crc16_OfBlock_IsCorrect()
        {
            var testData = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            var crc16 = Crc16.OfBlock(testData, Crc16.InitialValue);
            Assert.Equal(0x877F, crc16);
        }
    }
}

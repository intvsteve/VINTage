// <copyright file="CrcDataTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class CrcDataTests
    {
        [Fact]
        public void CrcData_WithZeroChecksum_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CrcData(0u, new KeyValuePair<string, IncompatibilityFlags>("d", IncompatibilityFlags.CuttleCart3)));
        }

        [Fact]
        public void CrcData_WithInitialChecksumDescriptionAndIncompatibilty_HasCorrectValues()
        {
            var crc = 0x12345678u;
            var description = "Steak-Umms";
            var incompatibilityFlags = IncompatibilityFlags.Intellicart;
            var crcData = new CrcData(crc, new KeyValuePair<string, IncompatibilityFlags>(description, incompatibilityFlags));

            Assert.Equal(crc, crcData.Crc);
            Assert.Equal(description, crcData.Description);
            Assert.Equal(incompatibilityFlags, crcData.Incompatibilities);
        }

        [Fact]
        public void CrcData_WithInitialChecksum_RetainsChecksum()
        {
            var crc = 0x12345678u;

            var crcData = new CrcData(crc);

            Assert.Equal(crc, crcData.Crc);
            Assert.True(string.IsNullOrEmpty(crcData.Description));
            Assert.Equal(IncompatibilityFlags.None, crcData.Incompatibilities);
        }

        [Fact]
        public void CrcData_EqualsNull_ReturnsFalse()
        {
            var crcData = new CrcData(0x12345678u);

            Assert.False(crcData.Equals(null));
        }

        [Fact]
        public void CrcData_EqualsNonCrcDataObject_ReturnsFalse()
        {
            var crcData = new CrcData(0x12345678u);

            Assert.False(crcData.Equals(new object()));
        }

        [Fact]
        public void CrcData_EqualsAnotherCrcDataObject_ReturnsFalse()
        {
            var crcData0 = new CrcData(0xBAADF00Du);
            var crcData1 = new CrcData(0xDEADBEEFu);

            Assert.False(crcData0.Equals(crcData1));
        }

        [Fact]
        public void CrcData_EqualsSelft_ReturnsTrue()
        {
            var crcData = new CrcData(0x12345678u);

            Assert.True(crcData.Equals(crcData));
        }

        [Fact]
        public void CrcData_GetHashCode_ReturnsExpectedValue()
        {
            var crc = 0x12345678u;
            var crcData = new CrcData(crc);

            Assert.Equal(crc.GetHashCode(), crcData.GetHashCode());
        }

        [Fact]
        public void CrcData_ToString_ReturnsExpectedString()
        {
            var crc = 0x12345678u;
            var description = "The battle is over.";
            var crcData = new CrcData(crc, description);

            var expectedString = string.Format(CultureInfo.InvariantCulture, "Name: '{0}', CRC: 0x{1}", description, crc.ToString("X8", CultureInfo.InvariantCulture));
            Assert.Equal(description, crcData.Description);
            Assert.Equal(expectedString, crcData.ToString());
        }
    }
}

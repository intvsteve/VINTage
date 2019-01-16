// <copyright file="ProgramIdentifierTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramIdentifierTests
    {
        [Fact]
        public void ProgramIdentifier_CreateWithDataCrc_CreatesProperIdentifier()
        {
            var dataPart = 0xFEDBADu;
            var expectedId = ((ulong)dataPart) << 32;

            var identifier = new ProgramIdentifier(dataPart);

            Assert.Equal(dataPart, identifier.DataCrc);
            Assert.Equal(0u, identifier.OtherData);
            Assert.Equal(expectedId, identifier.Id);
        }

        [Fact]
        public void ProgramIdentifier_CreateWithDataAndOtherCrcs_CreatesProperIdentifier()
        {
            var dataPart = 0xFEDCBA98u;
            var otherPart = 0x76543210u;
            var expectedId = (((ulong)dataPart) << 32) | otherPart;

            var identifier = new ProgramIdentifier(dataPart, otherPart);

            Assert.Equal(dataPart, identifier.DataCrc);
            Assert.Equal(otherPart, identifier.OtherData);
            Assert.Equal(expectedId, identifier.Id);
        }

        [Fact]
        public void ProgramIdentifier_CreateWithId_CreatesProperIdentifier()
        {
            var expectedId = 0x01234567FEDCBA98u;
            var expectedData = (uint)(expectedId >> 32);
            var expectedOtherData = (uint)(expectedId & 0xFFFFFFFF);

            var identifier = new ProgramIdentifier(expectedId);

            Assert.Equal(expectedData, identifier.DataCrc);
            Assert.Equal(expectedOtherData, identifier.OtherData);
            Assert.Equal(expectedId, identifier.Id);
        }

        public static IEnumerable<object[]> ProgramIdentifierEqualityTestData
        {
            get
            {
                yield return new object[] { ProgramIdentifier.Invalid, new ProgramIdentifier(0u, 0u), true };
                yield return new object[] { new ProgramIdentifier(0u, 0u), ProgramIdentifier.Invalid, true };
                yield return new object[] { ProgramIdentifier.Invalid, new ProgramIdentifier(0u, 1u), false };
                yield return new object[] { new ProgramIdentifier(1u, 0u), ProgramIdentifier.Invalid, false };
                yield return new object[] { new ProgramIdentifier(1u), new ProgramIdentifier(1u, 0u), true };
                yield return new object[] { new ProgramIdentifier(1u), new ProgramIdentifier(1u, 1u), false };
            }
        }

        [Theory]
        [MemberData("ProgramIdentifierEqualityTestData")]
        public void ProgramIdentifier_OperatorEqual_ComparesIdentifiersCorrectly(ProgramIdentifier lhs, ProgramIdentifier rhs, bool expectedEqual)
        {
            Assert.Equal(expectedEqual, lhs == rhs);
        }

        [Theory]
        [MemberData("ProgramIdentifierEqualityTestData")]
        public void ProgramIdentifier_OperatorNotEqual_ComparesIdentifiersCorrectly(ProgramIdentifier lhs, ProgramIdentifier rhs, bool expectedEqual)
        {
            Assert.NotEqual(expectedEqual, lhs != rhs);
        }

        public static IEnumerable<object[]> ProgramIdentifierObjectEqualityTestData
        {
            get
            {
                yield return new object[] { ProgramIdentifier.Invalid, null, false };
                yield return new object[] { ProgramIdentifier.Invalid, "program id", false };
                yield return new object[] { ProgramIdentifier.Invalid, 3.14, false };
                yield return new object[] { ProgramIdentifier.Invalid, new object(), false };
                yield return new object[] { ProgramIdentifier.Invalid, new ProgramIdentifier(0u, 0u), true };
                yield return new object[] { new ProgramIdentifier(0u, 0u), ProgramIdentifier.Invalid, true };
                yield return new object[] { new ProgramIdentifier(0u, 0u), ProgramIdentifier.Invalid, true };
                yield return new object[] { ProgramIdentifier.Invalid, new ProgramIdentifier(0u, 1u), false };
                yield return new object[] { new ProgramIdentifier(1u, 0u), ProgramIdentifier.Invalid, false };
                yield return new object[] { new ProgramIdentifier(1u), new ProgramIdentifier(1u, 0u), true };
                yield return new object[] { new ProgramIdentifier(1u), new ProgramIdentifier(1u, 1u), false };
            }
        }

        [Theory]
        [MemberData("ProgramIdentifierObjectEqualityTestData")]
        public void ProgramIdentifier_EqualsObject_ComparesAsExpected(ProgramIdentifier identifier, object other, bool expectedEqual)
        {
            Assert.Equal(expectedEqual, identifier.Equals(other));
        }

        [Fact]
        public void ProgramIdentifier_ImplicitOperator_CreatesProperIdentifier()
        {
            var crc = 0x48736251u;
            var expectedId = ((ulong)crc) << 32;

            ProgramIdentifier identifier = crc;
            Assert.Equal(crc, identifier.DataCrc);
            Assert.Equal(0u, identifier.OtherData);
            Assert.Equal(expectedId, identifier.Id);
        }

        [Fact]
        public void ProgramIdentifier_ToString_ProducesCorrectString()
        {
            var dataPart = 0xFEDCBA98u;
            var otherPart = 0x76543210u;
            var expectedString = string.Format(CultureInfo.InvariantCulture, "{0:X8},{1:X8}", dataPart, otherPart);

            var identifier = new ProgramIdentifier(dataPart, otherPart);

            Assert.Equal(expectedString, identifier.ToString());
        }

        [Fact]
        public void ProgramIdentifier_GetHashcode_ProducesCorrectHash()
        {
            var dataPart = 0x123u;
            var otherPart = 0x456u;
            var identifier = new ProgramIdentifier(dataPart, otherPart);

            var combinedParts = (((ulong)dataPart) << 32) | otherPart;
            var expectedHash = combinedParts.GetHashCode();

            Assert.Equal(expectedHash, identifier.GetHashCode());
        }
    }
}

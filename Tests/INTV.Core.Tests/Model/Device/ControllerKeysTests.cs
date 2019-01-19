// <copyright file="ControllerKeysTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Device;
using Xunit;

namespace INTV.Core.Tests.Model.Device
{
    /// <summary>
    /// Tests for ControllerKeysHelpers.
    /// </summary>
    public partial class ControllerKeysTests
    {
        [Theory]
        [MemberData("AllControllerInputsTestData")]
        public void ControllerKeys_ToDisplayStringForControllerKeys_DisplayStringNotEmpty(ControllerKeys key)
        {
            Assert.False(string.IsNullOrEmpty(key.ToDisplayString()));
        }

        public static IEnumerable<object[]> DeadKeyTestValues
        {
            get
            {
                return DeadKeyValues.Select(c => new object[] { c });
            }
        }

        [Theory]
        [MemberData("DeadKeyTestValues")]
        public void ControllerKeys_ToDisplayStringForDeadKeys_DisplayStringEmpty(ControllerKeys key)
        {
            Assert.True(string.IsNullOrEmpty(key.ToDisplayString()));
        }

        [Theory]
        [InlineData(new ControllerKeys[] { }, "<none>")] // NOTE: If the TODO of putting this string into resources happens, this can fail on localized systems unless mitigated
        [InlineData(new[] { ControllerKeys.DiscE }, "E")]
        [InlineData(new[] { ControllerKeys.Keypad0, ControllerKeys.KeypadEnter }, "0+enter")]
        public void ControllerKeys_ToDisplayStringForCollection_ProducesExpectedDisplayString(ControllerKeys[] keys, string expectedDisplayString)
        {
            Assert.Equal(expectedDisplayString, keys.ToDisplayString());
        }

        [Fact]
        public void ControllerKeys_ToDisplayStringForNullCollection_ThrowsArgumentNullException()
        {
            IEnumerable<ControllerKeys> keys = null;

            Assert.Throws<ArgumentNullException>(() => keys.ToDisplayString());
        }

        [Theory]
        [MemberData("AllControllerInputsTestData")]
        public void ControllerKeys_ToHardwareBits_ReturnsValidBits(ControllerKeys key)
        {
            Assert.NotEqual((byte)0, key.ToHardwareBits());
        }

        [Theory]
        [MemberData("AllControllerKeysWithInactiveFlagSetTestData")]
        public void ControllerKeys_WithNoneActiveSetToHardwareBits_ReturnsValidBits(ControllerKeys key)
        {
            Assert.Equal((byte)0, key.ToHardwareBits());
        }

        [Theory]
        [InlineData(null, (byte)0xFF)]
        [InlineData(new ControllerKeys[] { }, (byte)0xFF)]
        [InlineData(new[] { ControllerKeys.DiscWSW }, (byte)0x09)]
        [InlineData(new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9 }, (byte)0xA5)]
        [InlineData(new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad3 }, (byte)0xA5)]
        [InlineData(new[] { ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS, ControllerKeys.KeypadClear }, (byte)0xE9)]
        public void ControllerKeys_ToHardwareBitsForCollection_ProducesExpectedHardwareBits(ControllerKeys[] keys, byte expectedHardwareBits)
        {
            Assert.Equal(expectedHardwareBits, keys.ToHardwareBits());
        }

        [Theory]
        [MemberData("IsReservedKeyCombinationTestData")]
        public void ControllerKeys_IsReservedKeyCombinationFromKeys_ProducesExpectedResult(IEnumerable<ControllerKeys> keys, bool expectedIsReservedKeyCombination)
        {
            Assert.Equal(expectedIsReservedKeyCombination, keys.IsReservedKeyCombination());
        }

        [Theory]
        [InlineData((byte)0, false)]
        [InlineData((byte)0xFF, false)]
        [InlineData((byte)0xA5, true)]
        [InlineData((byte)0xA7, false)]
        [InlineData((byte)0xC5, false)]
        [InlineData((byte)0xC7, false)]
        public void ControllerKeys_IsReservedKeyCombinationFromHardwareBits_ProducesExpectedResults(byte hardwareBits, bool expectedIsReservedKeyCombination)
        {
            Assert.Equal(expectedIsReservedKeyCombination, hardwareBits.IsReservedKeyCombination());
        }

        [Theory]
        [MemberData("GetDiscInputsTestData")]
        public void ControllerKeys_GetDiscInputs_ReturnsCorrectInputs(byte hardwareBits, IEnumerable<ControllerKeys> expectedDiscInputs)
        {
            var discInputs = hardwareBits.GetDiscInputs();

            Assert.False(expectedDiscInputs.Except(discInputs).Any());
            Assert.False(discInputs.Except(expectedDiscInputs).Any());
        }

        [Theory]
        [MemberData("GetDiscInputsIncludingAdjacentTestData")]
        public void ControllerKeys_GetDiscInputsIncludingAdjacentInputs_ReturnsCorrectInputs(byte hardwareBits, sbyte adjacentDistance, IEnumerable<ControllerKeys> expectedDiscInputs)
        {
            var discInputs = hardwareBits.GetDiscInputs(adjacentDistance);

            Assert.False(expectedDiscInputs.Except(discInputs).Any());
            Assert.False(discInputs.Except(expectedDiscInputs).Any());
        }

        [Theory]
        [InlineData(9)]
        [InlineData(8)]
        [InlineData(-8)]
        [InlineData(-9)]
        [InlineData(sbyte.MaxValue)]
        [InlineData(sbyte.MinValue)]
        public void ControllerKeys_GetDiscInputsIncludingOutOfAdjacentInputs_ThrowsArgumentOutOfRangeException(sbyte outOfRangeAdjacency)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((byte)0x01).GetDiscInputs(outOfRangeAdjacency));
        }

        [Theory]
        [MemberData("GetKeypadInputsTestData")]
        public void ControllerKeys_GetKeypadInputs_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeypadInputs)
        {
            var keypadInputs = hardwareBits.GetKeypadInputs();

            Assert.False(expectedKeypadInputs.Except(keypadInputs).Any());
            Assert.False(keypadInputs.Except(expectedKeypadInputs).Any());
        }

        [Theory]
        [MemberData("GetActionKeyInputsTestData")]
        public void ControllerKeys_GetActionKeyInputs_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedActionKeyInputs)
        {
            var actionKeyInputs = hardwareBits.GetActionKeyInputs();

            Assert.False(expectedActionKeyInputs.Except(actionKeyInputs).Any());
            Assert.False(actionKeyInputs.Except(expectedActionKeyInputs).Any());
        }

        [Theory]
        [MemberData("GetDiscPlusActionKeyAliasesIncludingAdjacentTestData")]
        public void ControllerKeys_GetDiscPlusActionKeyAliases_ReturnsExpectedKeys(byte hardwareBits, sbyte adjacentDistance, IEnumerable<ControllerKeys> expectedDiscInputs)
        {
            var discPlusActionKeyInputs = hardwareBits.GetDiscPlusActionKeyAliases(adjacentDistance);

            Assert.False(expectedDiscInputs.Except(discPlusActionKeyInputs).Any());
            Assert.False(discPlusActionKeyInputs.Except(expectedDiscInputs).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithoutAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBits_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            var keys = hardwareBits.FromHardwareBits();

            Assert.False(expectedKeys.Except(keys).Any());
            Assert.False(keys.Except(expectedKeys).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithNullFilter_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            IEnumerable<ControllerKeys> filter = null;
            var keys = hardwareBits.FromHardwareBits(filter);

            Assert.False(expectedKeys.Except(keys).Any());
            Assert.False(keys.Except(expectedKeys).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithEmptyFilter_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            var keys = hardwareBits.FromHardwareBits(Enumerable.Empty<ControllerKeys>());

            Assert.False(keys.Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithKeypadValuesFilter_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            var keys = hardwareBits.FromHardwareBits(KeypadValues);
            var filteredExpectedKeys = expectedKeys.Intersect(KeypadValues);

            Assert.False(filteredExpectedKeys.Except(keys).Any());
            Assert.False(keys.Except(filteredExpectedKeys).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithActionKeyValuesFilter_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            var keys = hardwareBits.FromHardwareBits(ActionKeyValues);
            var filteredExpectedKeys = expectedKeys.Intersect(ActionKeyValues);

            Assert.False(filteredExpectedKeys.Except(keys).Any());
            Assert.False(keys.Except(filteredExpectedKeys).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithDiscDirectionValuesFilter_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            var keys = hardwareBits.FromHardwareBits(DiscDirectionValues);
            var filteredExpectedKeys = expectedKeys.Intersect(DiscDirectionValues);

            Assert.False(filteredExpectedKeys.Except(keys).Any());
            Assert.False(keys.Except(filteredExpectedKeys).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithPredicate_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            Predicate<ControllerKeys> filter = k => k == ControllerKeys.Keypad1;
            var keys = hardwareBits.FromHardwareBits(filter);
            var filteredExpectedKeys = expectedKeys.Where(k => k == ControllerKeys.Keypad1);

            Assert.False(filteredExpectedKeys.Except(keys).Any());
            Assert.False(keys.Except(filteredExpectedKeys).Any());
        }

        [Theory]
        [MemberData("AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData")]
        public void ControllerKeys_FromHardwareBitsWithNullPredicate_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeys)
        {
            Predicate<ControllerKeys> filter = null;
            var keys = hardwareBits.FromHardwareBits(filter);

            Assert.False(expectedKeys.Except(keys).Any());
            Assert.False(keys.Except(expectedKeys).Any());
        }

        [Fact]
        public void ControllerKeys_FromHardwareBitsAlternate_ReturnsExpectedKeys()
        {
            var hardwareBits = (byte)0x03;
            var keys = hardwareBits.FromHardwareBitsAlternate();
            var expectedKeys = new[] { ControllerKeys.DiscSSE, ControllerKeys.DiscS };

            Assert.False(expectedKeys.Except(keys).Any());
            Assert.False(keys.Except(expectedKeys).Any());
        }
    }
}

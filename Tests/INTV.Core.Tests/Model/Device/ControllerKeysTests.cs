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
    public class ControllerKeysTests
    {
        private static IEnumerable<ControllerKeys> AllValues
        {
            get
            {
                var allControllerKeysValues = Enum.GetValues(typeof(ControllerKeys)).Cast<ControllerKeys>().Distinct();
                return allControllerKeysValues;
            }
        }

        private static IEnumerable<ControllerKeys> DeadKeyValues
        {
            get
            {
                yield return ControllerKeys.None;
                yield return ControllerKeys.ActionKeyActive;
                yield return ControllerKeys.NoneActive;
            }
        }

        private static IEnumerable<ControllerKeys> AllControllerInputValues
        {
            get
            {
                return AllValues.Except(DeadKeyValues);
            }
        }

        private static IEnumerable<ControllerKeys> KeypadValues
        {
            get
            {
                return AllValues.Where(c => c.HasFlag(ControllerKeys.KeypadActive));
            }
        }

        private static IEnumerable<ControllerKeys> ActionKeyValues
        {
            get
            {
                return AllValues.Where(c => c.HasFlag(ControllerKeys.ActionKeyActive));
            }
        }

        private static IEnumerable<ControllerKeys> DiscDirectionValues
        {
            get
            {
                return AllValues.Where(c => c.HasFlag(ControllerKeys.DiscActive));
            }
        }

        public static IEnumerable<object[]> AllControllerInputsTestData
        {
            get
            {
                return AllControllerInputValues.Select(c => new object[] { c });
            }
        }

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

        public static IEnumerable<object[]> AllControllerKeysWithInactiveFlagSetTestData
        {
            get
            {
                return AllValues.Select(c => new object[] { c | ControllerKeys.NoneActive });
            }
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

        public static IEnumerable<object[]> IsReservedKeyCombinationTestData
        {
            get
            {
                yield return new object[] { null, false };
                yield return new object[] { Enumerable.Empty<ControllerKeys>(), false };
                yield return new object[] { new[] { ControllerKeys.KeypadEnter }, false };
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9 }, true };
                yield return new object[] { new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad7 }, true };
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop }, true }; // Bet you didn't expect that!
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomRight }, false };
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9, ControllerKeys.DiscN }, true }; // Bet you didn't expect that!
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9, ControllerKeys.DiscS }, true }; // Bet you didn't expect that!
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9, ControllerKeys.DiscE }, false };
                yield return new object[] { new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9, ControllerKeys.DiscW }, false };
                yield return new object[] { new[] { ControllerKeys.KeypadEnter | ControllerKeys.Keypad2 }, false };
                yield return new object[] { new[] { ControllerKeys.DiscSW | ControllerKeys.DiscW }, false };
            }
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

        public static IEnumerable<object[]> GetDiscInputsTestData
        {
            get
            {
                yield return new object[] { (byte)0, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x84, new[] { ControllerKeys.DiscN } };
                yield return new object[] { (byte)0xA0, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x06, new[] { ControllerKeys.DiscE, ControllerKeys.DiscN, ControllerKeys.DiscENE } };
                yield return new object[] { (byte)0x03, new[] { ControllerKeys.DiscS, ControllerKeys.DiscE, ControllerKeys.DiscSSE } };
            }
        }

        [Theory]
        [MemberData("GetDiscInputsTestData")]
        public void ControllerKeys_GetDiscInputs_ReturnsCorrectInputs(byte hardwareBits, IEnumerable<ControllerKeys> expectedDiscInputs)
        {
            var discInputs = hardwareBits.GetDiscInputs();

            Assert.False(expectedDiscInputs.Except(discInputs).Any());
            Assert.False(discInputs.Except(expectedDiscInputs).Any());
        }

        public static IEnumerable<object[]> GetDiscInputsIncludingAdjacentTestData
        {
            get
            {
                yield return new object[] { (byte)0x04, (sbyte)0, new[] { ControllerKeys.DiscN } }; // NORTH
                yield return new object[] { (byte)0x04, (sbyte)1, new[] { ControllerKeys.DiscNNW, ControllerKeys.DiscN, ControllerKeys.DiscNNE } };
                yield return new object[] { (byte)0x04, (sbyte)-2, new[] { ControllerKeys.DiscNW, ControllerKeys.DiscNNW, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE } };
                yield return new object[] { (byte)0x04, (sbyte)7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscS) };
                yield return new object[] { (byte)0x04, (sbyte)-7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscS) };
                yield return new object[] { (byte)0x01, (sbyte)0, new[] { ControllerKeys.DiscS } }; // SOUTH
                yield return new object[] { (byte)0x01, (sbyte)3, new[] { ControllerKeys.DiscWSW, ControllerKeys.DiscSW, ControllerKeys.DiscSSW, ControllerKeys.DiscS, ControllerKeys.DiscSSE, ControllerKeys.DiscSE, ControllerKeys.DiscESE } };
                yield return new object[] { (byte)0x01, (sbyte)-4, new[] { ControllerKeys.DiscW, ControllerKeys.DiscWSW, ControllerKeys.DiscSW, ControllerKeys.DiscSSW, ControllerKeys.DiscS, ControllerKeys.DiscSSE, ControllerKeys.DiscSE, ControllerKeys.DiscESE, ControllerKeys.DiscE } };
                yield return new object[] { (byte)0x01, (sbyte)7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscN) };
                yield return new object[] { (byte)0x01, (sbyte)-7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscN) };
                yield return new object[] { (byte)0x02, (sbyte)0, new[] { ControllerKeys.DiscE } }; // EAST
                yield return new object[] { (byte)0x02, (sbyte)5, DiscDirectionValues.Except(new[] { ControllerKeys.DiscNW, ControllerKeys.DiscWNW, ControllerKeys.DiscW, ControllerKeys.DiscWSW, ControllerKeys.DiscSW }) };
                yield return new object[] { (byte)0x02, (sbyte)-6, DiscDirectionValues.Except(new[] { ControllerKeys.DiscWNW, ControllerKeys.DiscW, ControllerKeys.DiscWSW }) };
                yield return new object[] { (byte)0x02, (sbyte)7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscW) };
                yield return new object[] { (byte)0x02, (sbyte)-7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscW) };
                yield return new object[] { (byte)0x08, (sbyte)0, new[] { ControllerKeys.DiscW } }; // WEST
                yield return new object[] { (byte)0x08, (sbyte)2, new[] { ControllerKeys.DiscNW, ControllerKeys.DiscWNW, ControllerKeys.DiscW, ControllerKeys.DiscWSW, ControllerKeys.DiscSW } };
                yield return new object[] { (byte)0x08, (sbyte)-3, new[] { ControllerKeys.DiscNNW, ControllerKeys.DiscNW, ControllerKeys.DiscWNW, ControllerKeys.DiscW, ControllerKeys.DiscWSW, ControllerKeys.DiscSW, ControllerKeys.DiscSSW } };
                yield return new object[] { (byte)0x08, (sbyte)7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscE) };
                yield return new object[] { (byte)0x08, (sbyte)-7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscE) };
                yield return new object[] { (byte)0x11, (sbyte)0, new[] { ControllerKeys.DiscSSW } }; // SOUTHxSOUTHWEST
                yield return new object[] { (byte)0x11, (sbyte)-1, new[] { ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW } };
                yield return new object[] { (byte)0x11, (sbyte)6, DiscDirectionValues.Except(new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE }) };
                yield return new object[] { (byte)0x11, (sbyte)7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscNNE) };
                yield return new object[] { (byte)0x11, (sbyte)-7, DiscDirectionValues.Where(d => d != ControllerKeys.DiscNNE) };
            }
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

        public static IEnumerable<object[]> GetKeypadInputsTestData
        {
            get
            {
                yield return new object[] { (byte)0x00, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0xFF, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x0C, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0xA0, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x06, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x03, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x81, new[] { ControllerKeys.Keypad1 } };
                yield return new object[] { (byte)0x41, new[] { ControllerKeys.Keypad2 } };
                yield return new object[] { (byte)0x21, new[] { ControllerKeys.Keypad3 } };
                yield return new object[] { (byte)0x82, new[] { ControllerKeys.Keypad4 } };
                yield return new object[] { (byte)0x42, new[] { ControllerKeys.Keypad5 } };
                yield return new object[] { (byte)0x22, new[] { ControllerKeys.Keypad6 } };
                yield return new object[] { (byte)0x84, new[] { ControllerKeys.Keypad7 } };
                yield return new object[] { (byte)0x44, new[] { ControllerKeys.Keypad8 } };
                yield return new object[] { (byte)0x24, new[] { ControllerKeys.Keypad9 } };
                yield return new object[] { (byte)0x88, new[] { ControllerKeys.KeypadClear } };
                yield return new object[] { (byte)0x48, new[] { ControllerKeys.Keypad0 } };
                yield return new object[] { (byte)0x28, new[] { ControllerKeys.KeypadEnter } };
                yield return new object[] { (byte)0xA5, new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9 } };
                yield return new object[] { (byte)0xA4, new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9 } }; // Disk N + Top Action Key
                yield return new object[] { (byte)0xA2, new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6 } }; // Disk E + Top Action Key
                yield return new object[] { (byte)0xA1, new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3 } }; // Disk S + Top Action Key
            }
        }

        [Theory]
        [MemberData("GetKeypadInputsTestData")]
        public void ControllerKeys_GetKeypadInputs_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedKeypadInputs)
        {
            var keypadInputs = hardwareBits.GetKeypadInputs();

            Assert.False(expectedKeypadInputs.Except(keypadInputs).Any());
            Assert.False(keypadInputs.Except(expectedKeypadInputs).Any());
        }

        public static IEnumerable<object[]> GetActionKeyInputsTestData
        {
            get
            {
                yield return new object[] { (byte)0x00, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0xFF, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x0C, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0x28, Enumerable.Empty<ControllerKeys>() };
                yield return new object[] { (byte)0xA0, new[] { ControllerKeys.ActionKeyTop } };
                yield return new object[] { (byte)0x60, new[] { ControllerKeys.ActionKeyBottomLeft } };
                yield return new object[] { (byte)0xC0, new[] { ControllerKeys.ActionKeyBottomRight } };
                yield return new object[] { (byte)0xE0, new[] { ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight } };
            }
        }

        [Theory]
        [MemberData("GetActionKeyInputsTestData")]
        public void ControllerKeys_GetActionKeyInputs_ReturnsExpectedKeys(byte hardwareBits, IEnumerable<ControllerKeys> expectedActionKeyInputs)
        {
            var actionKeyInputs = hardwareBits.GetActionKeyInputs();

            Assert.False(expectedActionKeyInputs.Except(actionKeyInputs).Any());
            Assert.False(actionKeyInputs.Except(expectedActionKeyInputs).Any());
        }

        public static IEnumerable<object[]> GetDiscPlusActionKeyAliasesIncludingAdjacentTestData
        {
            get
            {
                yield return new object[] { (byte)0x04, (sbyte)0, Enumerable.Empty<ControllerKeys>() }; // NORTH
                yield return new object[] { (byte)0xA4, (sbyte)1, new[] { ControllerKeys.ActionKeyTop, ControllerKeys.DiscN } };
                yield return new object[] { (byte)0xC4, (sbyte)-2, new[] { ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN } };
                yield return new object[] { (byte)0x64, (sbyte)7, new[] { ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN } };
                yield return new object[] { (byte)0x04, (sbyte)-7, Enumerable.Empty<ControllerKeys>() };
            }
        }

        [Theory]
        [MemberData("GetDiscPlusActionKeyAliasesIncludingAdjacentTestData")]
        public void ControllerKeys_GetDiscPlusActionKeyAliases_ReturnsExpectedKeys(byte hardwareBits, sbyte adjacentDistance, IEnumerable<ControllerKeys> expectedDiscInputs)
        {
            var discPlusActionKeyInputs = hardwareBits.GetDiscPlusActionKeyAliases(adjacentDistance);

            Assert.False(expectedDiscInputs.Except(discPlusActionKeyInputs).Any());
            Assert.False(discPlusActionKeyInputs.Except(expectedDiscInputs).Any());
        }
    }
}

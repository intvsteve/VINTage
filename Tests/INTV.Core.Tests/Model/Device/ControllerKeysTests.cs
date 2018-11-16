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
                var allControllerKeysValues = Enum.GetValues(typeof(ControllerKeys)).Cast<ControllerKeys>();
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
    }
}

// <copyright file="ControllerKeysTests.Data.cs" company="INTV Funhouse">
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
using System.Text;
using System.Threading.Tasks;
using INTV.Core.Model.Device;

namespace INTV.Core.Tests.Model.Device
{
    /// <summary>
    /// Test data for the ControllerKeysHelpers tests.
    /// </summary>
    public partial class ControllerKeysTests
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

        public static IEnumerable<object[]> AllControllerKeysWithInactiveFlagSetTestData
        {
            get
            {
                return AllValues.Select(c => new object[] { c | ControllerKeys.NoneActive });
            }
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

        public static IEnumerable<object[]> AllTheoreticallyPossibleHardwareBitsValuesWithoutAllMatchesTestData
        {
            get
            {
                var allTheoreticallyPossibleHardwareBitsValues = Enumerable.Range((byte)0, 256);
                foreach (var hardwareBits in allTheoreticallyPossibleHardwareBitsValues)
                {
                    IEnumerable<ControllerKeys> expectedKeys = null;
                    switch (hardwareBits)
                    {
                        case 0x00:
                        case 0x10:
                        case 0x20:
                        case 0x30:
                        case 0x40:
                        case 0x50:
                        case 0x80:
                        case 0x90:
                        case 0xFF:
                            expectedKeys = Enumerable.Empty<ControllerKeys>();
                            break;
                        case 0x01:
                            expectedKeys = new[] { ControllerKeys.DiscS };
                            break;
                        case 0x02:
                            expectedKeys = new[] { ControllerKeys.DiscE };
                            break;
                        case 0x03:
                            // NOTE: this is an artifact of the ordering of data in the enum ... DiscS is not included, as not all keys are included.
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscSSE };
                            break;
                        case 0x04:
                            expectedKeys = new[] { ControllerKeys.DiscN };
                            break;
                        case 0x05:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0x06:
                            // NOTE: this is an artifact of the ordering of data in the enum ... DiscS is not included, as not all keys are included.
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE };
                            break;
                        case 0x07:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE };
                            break;
                        case 0x08:
                            expectedKeys = new[] { ControllerKeys.DiscW };
                            break;
                        case 0x09:
                            expectedKeys = new[] { ControllerKeys.DiscWSW, ControllerKeys.DiscS };
                            break;
                        case 0x0A:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x0B:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW };
                            break;
                        case 0x0C:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscW };
                            break;
                        case 0x0D:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW };
                            break;
                        case 0x0E:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x0F:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW };
                            break;
                        case 0x11:
                            expectedKeys = new[] { ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x12:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x13:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE };
                            break;
                        case 0x14:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x15:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS };
                            break;
                        case 0x16:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE };
                            break;
                        case 0x17:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE };
                            break;
                        case 0x18:
                            expectedKeys = new[] { ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x19:
                            expectedKeys = new[] { ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW };
                            break;
                        case 0x1A:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW };
                            break;
                        case 0x1B:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW };
                            break;
                        case 0x1C:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW };
                            break;
                        case 0x1D:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW };
                            break;
                        case 0x1E:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW };
                            break;
                        case 0x1F:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW };
                            break;
                        case 0x21:
                            expectedKeys = new[] { ControllerKeys.Keypad3 };
                            break;
                        case 0x22:
                            expectedKeys = new[] { ControllerKeys.Keypad6 };
                            break;
                        case 0x23:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6 };
                            break;
                        case 0x24:
                            expectedKeys = new[] { ControllerKeys.Keypad9 };
                            break;
                        case 0x25:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9 };
                            break;
                        case 0x26:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9 };
                            break;
                        case 0x27:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9 };
                            break;
                        case 0x28:
                            expectedKeys = new[] { ControllerKeys.KeypadEnter };
                            break;
                        case 0x29:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.KeypadEnter };
                            break;
                        case 0x2A:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.KeypadEnter };
                            break;
                        case 0x2B:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.KeypadEnter };
                            break;
                        case 0x2C:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.KeypadEnter };
                            break;
                        case 0x2D:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter };
                            break;
                        case 0x2E:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter };
                            break;
                        case 0x2F:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter };
                            break;
                        case 0x31:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x32:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x33:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x34:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x35:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x36:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x37:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x38:
                            expectedKeys = new[] { ControllerKeys.KeypadEnter, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x39:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.KeypadEnter, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x3A:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.KeypadEnter, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x3B:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.KeypadEnter, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x3C:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x3D:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x3E:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x3F:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x41:
                            expectedKeys = new[] { ControllerKeys.Keypad2 };
                            break;
                        case 0x42:
                            expectedKeys = new[] { ControllerKeys.Keypad5 };
                            break;
                        case 0x43:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5 };
                            break;
                        case 0x44:
                            expectedKeys = new[] { ControllerKeys.Keypad8 };
                            break;
                        case 0x45:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8 };
                            break;
                        case 0x46:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8 };
                            break;
                        case 0x47:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8 };
                            break;
                        case 0x48:
                            expectedKeys = new[] { ControllerKeys.Keypad0 };
                            break;
                        case 0x49:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad0 };
                            break;
                        case 0x4A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad0 };
                            break;
                        case 0x4B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad0 };
                            break;
                        case 0x4C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad0 };
                            break;
                        case 0x4D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.Keypad0 };
                            break;
                        case 0x4E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0 };
                            break;
                        case 0x4F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0 };
                            break;
                        case 0x51:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x52:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x53:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x54:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x55:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x56:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x57:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x58:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x59:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad0, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x5A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad0, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x5B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad0, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x5C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x5D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x5E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x5F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x60:
                        case 0x70:
                            expectedKeys = new[] { ControllerKeys.ActionKeyBottomLeft };
                            break;
                        case 0x61:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3 };
                            break;
                        case 0x62:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6 };
                            break;
                        case 0x63:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5 };
                            break;
                        case 0x64:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9 };
                            break;
                        case 0x65:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8 };
                            break;
                        case 0x66:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8 };
                            break;
                        case 0x67:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8 };
                            break;
                        case 0x68:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.KeypadEnter };
                            break;
                        case 0x69:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad0 };
                            break;
                        case 0x6A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0 };
                            break;
                        case 0x6B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0 };
                            break;
                        case 0x6C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0 };
                            break;
                        case 0x6D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0 };
                            break;
                        case 0x6E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0 };
                            break;
                        case 0x6F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0 };
                            break;
                        case 0x71:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x72:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x73:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x74:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x75:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x76:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x77:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x78:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x79:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x7A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x7B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x7C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x7D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x7E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x7F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x81:
                            expectedKeys = new[] { ControllerKeys.Keypad1 };
                            break;
                        case 0x82:
                            expectedKeys = new[] { ControllerKeys.Keypad4 };
                            break;
                        case 0x83:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4 };
                            break;
                        case 0x84:
                            expectedKeys = new[] { ControllerKeys.Keypad7 };
                            break;
                        case 0x85:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7 };
                            break;
                        case 0x86:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7 };
                            break;
                        case 0x87:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7 };
                            break;
                        case 0x88:
                            expectedKeys = new[] { ControllerKeys.KeypadClear };
                            break;
                        case 0x89:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.KeypadClear };
                            break;
                        case 0x8A:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.KeypadClear };
                            break;
                        case 0x8B:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.KeypadClear };
                            break;
                        case 0x8C:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.KeypadClear };
                            break;
                        case 0x8D:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.KeypadClear };
                            break;
                        case 0x8E:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear };
                            break;
                        case 0x8F:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear };
                            break;
                        case 0x91:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x92:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x93:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x94:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x95:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x96:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x97:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x98:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x99:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.KeypadClear, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x9A:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.KeypadClear, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x9B:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.KeypadClear, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x9C:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x9D:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x9E:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x9F:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xA0:
                        case 0xB0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyTop };
                            break;
                        case 0xA1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3 };
                            break;
                        case 0xA2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6 };
                            break;
                        case 0xA3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4 };
                            break;
                        case 0xA4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9 };
                            break;
                        case 0xA5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7 };
                            break;
                        case 0xA6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7 };
                            break;
                        case 0xA7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7 };
                            break;
                        case 0xA8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter };
                            break;
                        case 0xA9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.KeypadClear };
                            break;
                        case 0xAA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear };
                            break;
                        case 0xAB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear };
                            break;
                        case 0xAC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xAD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xAE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xAF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xB1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.ActionKeyTop, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xB2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xB3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xB4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xB5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xB6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xB7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xB8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xB9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xBA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xBB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xBC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xBD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xBE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xBF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xC0:
                        case 0xD0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyBottomRight };
                            break;
                        case 0xC1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2 };
                            break;
                        case 0xC2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5 };
                            break;
                        case 0xC3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4 };
                            break;
                        case 0xC4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8 };
                            break;
                        case 0xC5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7 };
                            break;
                        case 0xC6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7 };
                            break;
                        case 0xC7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7 };
                            break;
                        case 0xC8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0 };
                            break;
                        case 0xC9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.KeypadClear };
                            break;
                        case 0xCA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear };
                            break;
                        case 0xCB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear };
                            break;
                        case 0xCC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear };
                            break;
                        case 0xCD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear };
                            break;
                        case 0xCE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear };
                            break;
                        case 0xCF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear };
                            break;
                        case 0xD1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xD2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xD3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xD4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xD5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xD6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xD7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xD8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xD9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xDA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xDB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xDC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xDD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xDE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xDF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xE0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft };
                            break;
                        case 0xE1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3 };
                            break;
                        case 0xE2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6 };
                            break;
                        case 0xE3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4 };
                            break;
                        case 0xE4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9 };
                            break;
                        case 0xE5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7 };
                            break;
                        case 0xE6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7 };
                            break;
                        case 0xE7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7 };
                            break;
                        case 0xE8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter };
                            break;
                        case 0xE9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.KeypadClear };
                            break;
                        case 0xEA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear };
                            break;
                        case 0xEB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear };
                            break;
                        case 0xEC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xED:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xEE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xEF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear };
                            break;
                        case 0xF0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight };
                            break;
                        case 0xF1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xF2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xF3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xF4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xF5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xF6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xF7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xF8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xF9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xFA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xFB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xFC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xFD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xFE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        default:
                            break;
                    }

                    yield return new object[] { hardwareBits, expectedKeys };
                }
            }
        }

        public static IEnumerable<object[]> AllTheoreticallyPossibleHardwareBitsValuesWithAllMatchesTestData
        {
            get
            {
                var allTheoreticallyPossibleHardwareBitsValues = Enumerable.Range((byte)0, 256);
                foreach (var hardwareBits in allTheoreticallyPossibleHardwareBitsValues)
                {
                    IEnumerable<ControllerKeys> expectedKeys = null;
                    switch (hardwareBits)
                    {
                        case 0x00:
                        case 0x10:
                        case 0x20:
                        case 0x30:
                        case 0x40:
                        case 0x50:
                        case 0x80:
                        case 0x90:
                        case 0xFF:
                            expectedKeys = Enumerable.Empty<ControllerKeys>();
                            break;
                        case 0x01:
                            expectedKeys = new[] { ControllerKeys.DiscS };
                            break;
                        case 0x02:
                            expectedKeys = new[] { ControllerKeys.DiscE };
                            break;
                        case 0x03:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x04:
                            expectedKeys = new[] { ControllerKeys.DiscN };
                            break;
                        case 0x05:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0x06:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0x07:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x08:
                            expectedKeys = new[] { ControllerKeys.DiscW };
                            break;
                        case 0x09:
                            expectedKeys = new[] { ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x0A:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x0B:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x0C:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x0D:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x0E:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x0F:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x11:
                            expectedKeys = new[] { ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x12:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x13:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x14:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x15:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x16:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x17:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x18:
                            expectedKeys = new[] { ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x19:
                            expectedKeys = new[] { ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x1A:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x1B:
                            expectedKeys = new[] { ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x1C:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x1D:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x1E:
                            expectedKeys = new[] { ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x1F:
                            expectedKeys = DiscDirectionValues;
                            break;
                        case 0x21:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.DiscS };
                            break;
                        case 0x22:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.DiscE };
                            break;
                        case 0x23:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x24:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.DiscN };
                            break;
                        case 0x25:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0x26:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0x27:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x28:
                            expectedKeys = new[] { ControllerKeys.KeypadEnter, ControllerKeys.DiscW };
                            break;
                        case 0x29:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.KeypadEnter, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x2A:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.KeypadEnter, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x2B:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.KeypadEnter, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x2C:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x2D:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x2E:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x2F:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x31:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x32:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x33:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x34:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x35:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x36:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x37:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x38:
                            expectedKeys = new[] { ControllerKeys.KeypadEnter, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x39:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.KeypadEnter, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x3A:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.KeypadEnter, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x3B:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.KeypadEnter, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x3C:
                            expectedKeys = new[] { ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x3D:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x3E:
                            expectedKeys = new[] { ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x3F:
                            expectedKeys = new[] { ControllerKeys.Keypad3, ControllerKeys.Keypad6, ControllerKeys.Keypad9, ControllerKeys.KeypadEnter }.Concat(DiscDirectionValues);
                            break;
                        case 0x41:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.DiscS };
                            break;
                        case 0x42:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.DiscE };
                            break;
                        case 0x43:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x44:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.DiscN };
                            break;
                        case 0x45:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0x46:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0x47:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x48:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.DiscW };
                            break;
                        case 0x49:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad0, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x4A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad0, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x4B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad0, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x4C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x4D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x4E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x4F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x51:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x52:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x53:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x54:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x55:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x56:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x57:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x58:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x59:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad0, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x5A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad0, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x5B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad0, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x5C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x5D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x5E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x5F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad5, ControllerKeys.Keypad8, ControllerKeys.Keypad0 }.Concat(DiscDirectionValues);
                            break;
                        case 0x60:
                        case 0x70:
                            expectedKeys = new[] { ControllerKeys.ActionKeyBottomLeft };
                            break;
                        case 0x61:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS };
                            break;
                        case 0x62:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE };
                            break;
                        case 0x63:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x64:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN };
                            break;
                        case 0x65:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0x66:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0x67:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x68:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscW };
                            break;
                        case 0x69:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x6A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x6B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x6C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x6D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x6E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x6F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x71:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x72:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x73:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x74:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x75:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x76:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x77:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x78:
                            expectedKeys = new[] { ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x79:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x7A:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x7B:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x7C:
                            expectedKeys = new[] { ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x7D:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x7E:
                            expectedKeys = new[] { ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x7F:
                            expectedKeys = new[] { ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyBottomLeft }.Concat(DiscDirectionValues);
                            break;
                        case 0x81:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.DiscS };
                            break;
                        case 0x82:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.DiscE };
                            break;
                        case 0x83:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x84:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.DiscN };
                            break;
                        case 0x85:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0x86:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0x87:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0x88:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.DiscW };
                            break;
                        case 0x89:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.KeypadClear, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x8A:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.KeypadClear, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0x8B:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.KeypadClear, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0x8C:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x8D:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x8E:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x8F:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0x91:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x92:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x93:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x94:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0x95:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x96:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0x97:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0x98:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x99:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.KeypadClear, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x9A:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.KeypadClear, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x9B:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.KeypadClear, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0x9C:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x9D:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x9E:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0x9F:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad4, ControllerKeys.Keypad7, ControllerKeys.KeypadClear }.Concat(DiscDirectionValues);
                            break;
                        case 0xA0:
                        case 0xB0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyTop };
                            break;
                        case 0xA1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.ActionKeyTop, ControllerKeys.DiscS };
                            break;
                        case 0xA2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE };
                            break;
                        case 0xA3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0xA4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN };
                            break;
                        case 0xA5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0xA6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0xA7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0xA8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscW };
                            break;
                        case 0xA9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0xAA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0xAB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0xAC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xAD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xAE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xAF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xB1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.ActionKeyTop, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xB2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xB3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xB4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xB5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xB6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xB7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xB8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xB9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xBA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xBB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xBC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xBD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xBE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xBF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop }.Concat(DiscDirectionValues);
                            break;
                        case 0xC0:
                        case 0xD0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyBottomRight };
                            break;
                        case 0xC1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS };
                            break;
                        case 0xC2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE };
                            break;
                        case 0xC3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0xC4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN };
                            break;
                        case 0xC5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0xC6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0xC7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0xC8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscW };
                            break;
                        case 0xC9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0xCA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0xCB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0xCC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xCD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xCE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xCF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xD1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xD2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xD3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xD4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xD5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xD6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xD7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xD8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xD9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xDA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xDB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xDC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xDD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xDE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xDF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.ActionKeyBottomRight }.Concat(DiscDirectionValues);
                            break;
                        case 0xE0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight };
                            break;
                        case 0xE1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS };
                            break;
                        case 0xE2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE };
                            break;
                        case 0xE3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0xE4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN };
                            break;
                        case 0xE5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscS };
                            break;
                        case 0xE6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE };
                            break;
                        case 0xE7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS };
                            break;
                        case 0xE8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscW };
                            break;
                        case 0xE9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0xEA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscW };
                            break;
                        case 0xEB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW };
                            break;
                        case 0xEC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xED:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xEE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xEF:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscNNW };
                            break;
                        case 0xF0:
                            expectedKeys = new[] { ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight };
                            break;
                        case 0xF1:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xF2:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xF3:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xF4:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE };
                            break;
                        case 0xF5:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xF6:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE };
                            break;
                        case 0xF7:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW };
                            break;
                        case 0xF8:
                            expectedKeys = new[] { ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xF9:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xFA:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xFB:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscSE, ControllerKeys.DiscSSE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW };
                            break;
                        case 0xFC:
                            expectedKeys = new[] { ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xFD:
                            expectedKeys = new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscS, ControllerKeys.DiscSSW, ControllerKeys.DiscSW, ControllerKeys.DiscWSW, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        case 0xFE:
                            expectedKeys = new[] { ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.ActionKeyTop, ControllerKeys.ActionKeyBottomLeft, ControllerKeys.ActionKeyBottomRight, ControllerKeys.DiscN, ControllerKeys.DiscNNE, ControllerKeys.DiscNE, ControllerKeys.DiscENE, ControllerKeys.DiscE, ControllerKeys.DiscESE, ControllerKeys.DiscW, ControllerKeys.DiscWNW, ControllerKeys.DiscNW, ControllerKeys.DiscNNW };
                            break;
                        default:
                            break;
                    }

                    yield return new object[] { hardwareBits, expectedKeys };
                }
            }
        }
    }
}

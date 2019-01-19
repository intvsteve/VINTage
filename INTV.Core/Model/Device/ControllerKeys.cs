// <copyright file="ControllerKeys.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.Core.Model.Device
{
    /* Listed below are the raw and inverted raw values in hex and binary for the bits */
    /* that are cleared (set) in hardware when the controller is providing input.      */
    /* ------------ KEYPAD INPUT ------------ */
    /* [1]: 0x81 : 10000001 : 0x7E : 01111110 */
    /* [2]: 0x41 : 01000001 : 0xBE : 10111110 */
    /* [3]: 0x21 : 00100001 : 0xDE : 11011110 */
    /* [4]: 0x82 : 10000010 : 0x7D : 01111101 */
    /* [5]: 0x42 : 01000010 : 0xBD : 10111101 */
    /* [6]: 0x22 : 00100010 : 0xDD : 11011101 */
    /* [7]: 0x84 : 10000100 : 0x7B : 01111011 */
    /* [8]: 0x44 : 01000100 : 0xBB : 10111011 */
    /* [9]: 0x24 : 00100100 : 0xDB : 11011011 */
    /* [C]: 0x88 : 10001000 : 0x77 : 01110111 */
    /* [0]: 0x48 : 01001000 : 0xB7 : 10110111 */
    /* [E]: 0x28 : 00101000 : 0xD7 : 11010111 */
    /* ---------- ACTION KEY INPUT ---------- */
    /* TOP: 0xA0 : 10100000 : 0x5F : 01011111 */
    /* BL:  0x60 : 01100000 : 0x9F : 10011111 */
    /* BR:  0xC0 : 11000000 : 0x3F : 00111111 */
    /* ------- DIRECTIONAL DISC INPUT ------- */
    /* N:   0x04 : 00000100 : 0xFB : 11111011 */
    /* NNE: 0x14 : 00010100 : 0xEB : 11101011 */
    /* NE:  0x16 : 00010110 : 0xE9 : 11101001 */
    /* ENE: 0x06 : 00000110 : 0xF9 : 11111001 */
    /* E:   0x02 : 00000010 : 0xFD : 11111101 */
    /* ESE: 0x12 : 00010010 : 0xED : 11101101 */
    /* SE:  0x13 : 00010011 : 0xEC : 11101100 */
    /* SSE: 0x03 : 00000011 : 0xFC : 11111100 */
    /* S:   0x01 : 00000001 : 0xFE : 11111110 */
    /* SSW: 0x11 : 00010001 : 0xEE : 11101110 */
    /* SW:  0x19 : 00011001 : 0xE6 : 11100110 */
    /* WSW: 0x09 : 00001001 : 0xF6 : 11110110 */
    /* W:   0x08 : 00001000 : 0xF7 : 11110111 */
    /* WNW: 0x18 : 00011000 : 0xE7 : 11100111 */
    /* NW:  0x1C : 00011100 : 0xE3 : 11100011 */
    /* NNW: 0x0C : 00001100 : 0xF3 : 11110011 */
    /* -------------------------------------- */

    /// <summary>
    /// Controller virtual keys.
    /// </summary>
    public enum ControllerKeys : byte
    {
        /// <summary>No active inputs.</summary>
        None = 0,

        /// <summary>Consider keypad keys active.</summary>
        KeypadActive = 1 << 4,

        /// <summary>Keypad button '0'.</summary>
        Keypad0 = 0 | KeypadActive,

        /// <summary>Keypad button '1'.</summary>
        Keypad1 = 1 | KeypadActive,

        /// <summary>Keypad button '2'.</summary>
        Keypad2 = 2 | KeypadActive,

        /// <summary>Keypad button '3'.</summary>
        Keypad3 = 3 | KeypadActive,

        /// <summary>Keypad button '4'.</summary>
        Keypad4 = 4 | KeypadActive,

        /// <summary>Keypad button '5'.</summary>
        Keypad5 = 5 | KeypadActive,

        /// <summary>Keypad button '6'.</summary>
        Keypad6 = 6 | KeypadActive,

        /// <summary>Keypad button '7'.</summary>
        Keypad7 = 7 | KeypadActive,

        /// <summary>Keypad button '8'.</summary>
        Keypad8 = 8 | KeypadActive,

        /// <summary>Keypad button '9'.</summary>
        Keypad9 = 9 | KeypadActive,

        /// <summary>Keypad button 'clear'.</summary>
        KeypadClear = 10 | KeypadActive,

        /// <summary>Keypad button 'enter'.</summary>
        KeypadEnter = 11 | KeypadActive,

        /// <summary>Consider side action buttons active.</summary>
        ActionKeyActive = 1 << 5,

        /// <summary>Top action key.</summary>
        ActionKeyTop = 1 | ActionKeyActive,

        /// <summary>Lower left action key.</summary>
        ActionKeyBottomLeft = 2 | ActionKeyActive,

        /// <summary>Lower right action key.</summary>
        ActionKeyBottomRight = 3 | ActionKeyActive,

        /// <summary>Consider direction disc input active.</summary>
        DiscActive = 1 << 6,

        /// <summary>Disc direction 'east'.</summary>
        DiscE = 0 | DiscActive,

        /// <summary>Disc direction 'east-northeast'.</summary>
        DiscENE = 1 | DiscActive,

        /// <summary>Disc direction 'northeast'.</summary>
        DiscNE = 2 | DiscActive,

        /// <summary>Disc direction 'north-northeast'.</summary>
        DiscNNE = 3 | DiscActive,

        /// <summary>Disc direction 'north'.</summary>
        DiscN = 4 | DiscActive,

        /// <summary>Disc direction 'north-northwest'.</summary>
        DiscNNW = 5 | DiscActive,

        /// <summary>Disc direction 'northwest'.</summary>
        DiscNW = 6 | DiscActive,

        /// <summary>Disc direction 'west-northwest'.</summary>
        DiscWNW = 7 | DiscActive,

        /// <summary>Disc direction 'west'.</summary>
        DiscW = 8 | DiscActive,

        /// <summary>Disc direction 'west-southwest'.</summary>
        DiscWSW = 9 | DiscActive,

        /// <summary>Disc direction 'southwest'.</summary>
        DiscSW = 10 | DiscActive,

        /// <summary>Disc direction 'south-southwest'.</summary>
        DiscSSW = 11 | DiscActive,

        /// <summary>Disc direction 'south'.</summary>
        DiscS = 12 | DiscActive,

        /// <summary>Disc direction 'south-southeast'.</summary>
        DiscSSE = 13 | DiscActive,

        /// <summary>Disc direction 'southeast'.</summary>
        DiscSE = 14 | DiscActive,

        /// <summary>Disc direction 'east-southeast'.</summary>
        DiscESE = 15 | DiscActive,

        /// <summary>Consider no controller inputs active.</summary>
        NoneActive = 1 << 7
    }

    /// <summary>
    /// Extension methods for working with ControllerKeys virtual controller keys enumeration.
    /// </summary>
    public static class ControllerKeysHelpers
    {
        private static readonly Dictionary<ControllerKeys, byte> ControllerKeyHardwareBits = new Dictionary<ControllerKeys, byte>()
        {
            { ControllerKeys.Keypad1, 0x81 },
            { ControllerKeys.Keypad2, 0x41 },
            { ControllerKeys.Keypad3, 0x21 },
            { ControllerKeys.Keypad4, 0x82 },
            { ControllerKeys.Keypad5, 0x42 },
            { ControllerKeys.Keypad6, 0x22 },
            { ControllerKeys.Keypad7, 0x84 },
            { ControllerKeys.Keypad8, 0x44 },
            { ControllerKeys.Keypad9, 0x24 },
            { ControllerKeys.KeypadClear, 0x88 },
            { ControllerKeys.Keypad0, 0x48 },
            { ControllerKeys.KeypadEnter, 0x28 },
            { ControllerKeys.ActionKeyTop, 0xA0 },
            { ControllerKeys.ActionKeyBottomLeft, 0x60 },
            { ControllerKeys.ActionKeyBottomRight, 0xC0 },
            { ControllerKeys.DiscN, 0x04 },
            { ControllerKeys.DiscNNE, 0x14 },
            { ControllerKeys.DiscNE, 0x16 },
            { ControllerKeys.DiscENE, 0x06 },
            { ControllerKeys.DiscE, 0x02 },
            { ControllerKeys.DiscESE, 0x12 },
            { ControllerKeys.DiscSE, 0x13 },
            { ControllerKeys.DiscSSE, 0x03 },
            { ControllerKeys.DiscS, 0x01 },
            { ControllerKeys.DiscSSW, 0x11 },
            { ControllerKeys.DiscSW, 0x19 },
            { ControllerKeys.DiscWSW, 0x09 },
            { ControllerKeys.DiscW, 0x08 },
            { ControllerKeys.DiscWNW, 0x18 },
            { ControllerKeys.DiscNW, 0x1C },
            { ControllerKeys.DiscNNW, 0x0C }
        };

        private static readonly Dictionary<ControllerKeys, string> ControllerKeyNames = new Dictionary<ControllerKeys, string>()
        {
            { ControllerKeys.Keypad1, "1" },
            { ControllerKeys.Keypad2, "2" },
            { ControllerKeys.Keypad3, "3" },
            { ControllerKeys.Keypad4, "4" },
            { ControllerKeys.Keypad5, "5" },
            { ControllerKeys.Keypad6, "6" },
            { ControllerKeys.Keypad7, "7" },
            { ControllerKeys.Keypad8, "8" },
            { ControllerKeys.Keypad9, "9" },
            { ControllerKeys.KeypadClear, "clear" },
            { ControllerKeys.Keypad0, "0" },
            { ControllerKeys.KeypadEnter, "enter" },
            { ControllerKeys.ActionKeyTop, "TOP" },
            { ControllerKeys.ActionKeyBottomLeft, "LEFT" },
            { ControllerKeys.ActionKeyBottomRight, "RIGHT" },
            { ControllerKeys.DiscN, "N" },
            { ControllerKeys.DiscNNE, "NNE" },
            { ControllerKeys.DiscNE, "NE" },
            { ControllerKeys.DiscENE, "ENE" },
            { ControllerKeys.DiscE, "E" },
            { ControllerKeys.DiscESE, "ESE" },
            { ControllerKeys.DiscSE, "SE" },
            { ControllerKeys.DiscSSE, "SSE" },
            { ControllerKeys.DiscS, "S" },
            { ControllerKeys.DiscSSW, "SSW" },
            { ControllerKeys.DiscSW, "SW" },
            { ControllerKeys.DiscWSW, "WSW" },
            { ControllerKeys.DiscW, "W" },
            { ControllerKeys.DiscWNW, "WNW" },
            { ControllerKeys.DiscNW, "NW" },
            { ControllerKeys.DiscNNW, "NNW" }
        };

        private static readonly byte[] ReservedKeyCombinations = new[]
        {
            (new[] { ControllerKeys.Keypad1, ControllerKeys.Keypad9 }).ToHardwareBits()
        };

        /// <summary>
        /// Gets a display string for a specific controller virtual key.
        /// </summary>
        /// <param name="key">The controller key.</param>
        /// <returns>The string to display.</returns>
        public static string ToDisplayString(this ControllerKeys key)
        {
            string keyName = null;
            if (!ControllerKeyNames.TryGetValue(key, out keyName))
            {
                keyName = string.Empty;
            }
            return keyName;
        }

        /// <summary>
        /// Gets a display string for an enumerable containing controller virtual keys.
        /// </summary>
        /// <param name="keys">The controller keys to include in the display string.</param>
        /// <returns>The display string, with multiple entries separated by a '+' character.</returns>
        public static string ToDisplayString(this IEnumerable<ControllerKeys> keys)
        {
            var keyNames = keys.OrderBy(k => k).Select(k => k.ToDisplayString());
            var keyString = string.Join("+", keyNames);
            if (string.IsNullOrEmpty(keyString))
            {
                keyString = "<none>"; // TODO: Put <none> controller key string into resources
            }
            return keyString;
        }

        /// <summary>
        /// Converts a virtual key to the corresponding hardware bits.
        /// </summary>
        /// <param name="key">The controller key for which hardware bits are desired.</param>
        /// <returns>The hardware bits.</returns>
        public static byte ToHardwareBits(this ControllerKeys key)
        {
            byte hardwareBits = 0;
            if (!key.HasFlag(ControllerKeys.NoneActive))
            {
                ControllerKeyHardwareBits.TryGetValue(key, out hardwareBits);
            }
            return hardwareBits;
        }

        /// <summary>
        /// Converts the virtual keys to the corresponding hardware bits.
        /// </summary>
        /// <param name="keys">The controller keys for which hardware bits are desired.</param>
        /// <returns>The hardware bits.</returns>
        public static byte ToHardwareBits(this IEnumerable<ControllerKeys> keys)
        {
            byte hardwareBits = 0xFF; // none
            if ((keys != null) && keys.Any())
            {
                hardwareBits = (byte)keys.Aggregate(0, (bits, key) => bits | key.ToHardwareBits());
            }
            return hardwareBits;
        }

        /// <summary>
        /// Converts the given hardware bits to ControllerKeys virtual key flags.
        /// </summary>
        /// <returns>The hardware bits.</returns>
        /// <param name="keys">The keys that are active.</param>
        public static IEnumerable<ControllerKeys> FromHardwareBits(this byte keys)
        {
            return keys.FromHardwareBits(ControllerKeyHardwareBits, false);
        }

        /// <summary>
        /// Converts the given hardware bits to ControllerKeys virtual key flags checking in an alternative order.
        /// </summary>
        /// <returns>The hardware bits.</returns>
        /// <param name="keys">The keys that are active.</param>
        public static IEnumerable<ControllerKeys> FromHardwareBitsAlternate(this byte keys)
        {
            return keys.FromHardwareBits(ControllerKeyHardwareBits.Reverse(), false);
        }

        /// <summary>
        /// Get all the virtual controller keys that match the given hardware bits as individual keys, including only the keys in <paramref name="filter"/>.
        /// </summary>
        /// <param name="keys">The hardware bits to break out into an enumerable of controller keys.</param>
        /// <param name="filter">Enumerable of keys that should be considered for matching. A <c>null</c> filter matches all keys.</param>
        /// <returns>The controller keys described by the hardware bits that are also in <paramref name="filter"/>.</returns>
        public static IEnumerable<ControllerKeys> FromHardwareBits(this byte keys, IEnumerable<ControllerKeys> filter)
        {
            return keys.FromHardwareBits(k => (filter == null) || filter.Contains(k));
        }

        /// <summary>
        /// Get all the virtual controller keys that match the given hardware bits as individual keys, including only the keys in <paramref name="filter"/>.
        /// </summary>
        /// <param name="keys">The hardware bits to break out into an enumerable of controller keys.</param>
        /// <param name="filter">Enumerable of keys that should be considered for matching. A <c>null</c> filter matches all keys.</param>
        /// <returns>The controller keys described by the hardware bits that are also in <paramref name="filter"/>.</returns>
        public static IEnumerable<ControllerKeys> FromHardwareBits(this byte keys, System.Predicate<ControllerKeys> filter)
        {
            var allControllerKeys = new List<ControllerKeys>();
            foreach (var entry in ControllerKeyHardwareBits)
            {
                if (((keys & entry.Value) == entry.Value) && ((filter == null) || filter(entry.Key)))
                {
                    allControllerKeys.Add(entry.Key);
                }
            }
            return allControllerKeys;
        }

        /// <summary>
        /// Determines if the given set of controller inputs, described as virtual controller keys, corresponds to a combination reserved by the system.
        /// </summary>
        /// <param name="keys">The controller inputs to test.</param>
        /// <returns><c>true</c> if the inputs correspond to a reserved combination; otherwise, <c>false</c>.</returns>
        public static bool IsReservedKeyCombination(this IEnumerable<ControllerKeys> keys)
        {
            var isReserved = IsReservedKeyCombination(keys.ToHardwareBits());
            return isReserved;
        }

        /// <summary>
        /// Determines if the given set of controller inputs, described in hardware, corresponds to a combination reserved by the system.
        /// </summary>
        /// <param name="keys">The controller input to test.</param>
        /// <returns><c>true</c> if the input corresponds to a reserved combination; otherwise, <c>false</c>.</returns>
        public static bool IsReservedKeyCombination(this byte keys)
        {
            var isReserved = ReservedKeyCombinations.Contains(keys);
            return isReserved;
        }

        /// <summary>
        /// Gets the keypad inputs.
        /// </summary>
        /// <param name="keys">The controller input to get keypad inputs from.</param>
        /// <returns>The keypad inputs.</returns>
        public static IEnumerable<ControllerKeys> GetKeypadInputs(this byte keys)
        {
            var allKeys = keys.FromHardwareBits(ControllerKeyHardwareBits, true);
            var keypadInputMatches = allKeys.Where(k => k.HasFlag(ControllerKeys.KeypadActive));
            return keypadInputMatches;
        }

        /// <summary>
        /// Gets all the disc inputs that match the given hardware bit pattern.
        /// </summary>
        /// <param name="keys">The controller input to get disc directional inputs from.</param>
        /// <returns>All the matching disc inputs. The check is a simple bitwise AND operation.</returns>
        public static IEnumerable<ControllerKeys> GetDiscInputs(this byte keys)
        {
            var allKeys = keys.FromHardwareBits(ControllerKeyHardwareBits, true);
            var discInputMatches = allKeys.Where(k => k.HasFlag(ControllerKeys.DiscActive));
            return discInputMatches;
        }

        /// <summary>
        /// Gets the disc inputs, including inputs from adjacent locations, from a hardware bit pattern for a directional disc input.
        /// </summary>
        /// <param name="discDirection">A bit pattern that exactly describes a single controller disc direction.</param>
        /// <param name="adjacentDistance">A value from 0-7 indicating how many additional adjacent disc directions should be included in the output. See remarks.</param>
        /// <returns>The disc inputs, along with potential additional adjacent directional disc inputs.</returns>
        /// <remarks>A non-zero value for the <paramref name="adjacentDistance"/> input may be used to simulate a
        /// glitchy controller. For example, a value of 1 for <paramref name="adjacentDistance"/> and a disc
        /// direction of 'east' will return three values - E, ENE, and ESE. A value of 2 produces five values in
        /// the results - E, ESE, ENE, SE and NE. As is easy to see, values larger than 1 for<paramref name="adjacentDistance"/>
        /// are usually excessive.
        /// NOTE: If the discDirection bit pattern matches more than one disc input, the function will return an empty list.</remarks>
        public static IEnumerable<ControllerKeys> GetDiscInputs(this byte discDirection, sbyte adjacentDistance)
        {
            const short MaxDirection = (short)(ControllerKeys.DiscESE & ~ControllerKeys.DiscActive);
            if (System.Math.Abs(adjacentDistance) > 7)
            {
                throw new System.ArgumentOutOfRangeException("adjacentDistance", adjacentDistance, Resources.Strings.ControllerKeys_AdjacentDistanceTooLarge);
            }
            var discKey = ControllerKeyHardwareBits.FirstOrDefault(k => k.Value == discDirection).Key;
            var discInputMatches = new List<ControllerKeys>();
            if (discKey.HasFlag(ControllerKeys.DiscActive))
            {
                var discInput = discKey;
                discInputMatches.Add(discInput);
                short direction = (short)(discInput & ~ControllerKeys.DiscActive);
                for (int i = 1; i <= System.Math.Abs(adjacentDistance); ++i)
                {
                    var directionToCheck = direction + i;
                    if (directionToCheck > MaxDirection)
                    {
                        directionToCheck %= MaxDirection + 1;
                    }
                    System.Diagnostics.Debug.Assert((directionToCheck >= 0) && (directionToCheck <= MaxDirection), "Error computing adjacent direction.");
                    var keyToCheck = ControllerKeys.DiscActive | (ControllerKeys)directionToCheck;
                    discInputMatches.Add(keyToCheck);
                    directionToCheck = direction - i;
                    if (directionToCheck < 0)
                    {
                        directionToCheck = MaxDirection + directionToCheck + 1;
                    }
                    keyToCheck = ControllerKeys.DiscActive | (ControllerKeys)directionToCheck;
                    discInputMatches.Add(keyToCheck);
                }
            }
            return discInputMatches.Distinct();
        }

        /// <summary>
        /// Gets the action key inputs.
        /// </summary>
        /// <param name="keys">The controller input to test.</param>
        /// <returns>The action key inputs.</returns>
        public static IEnumerable<ControllerKeys> GetActionKeyInputs(this byte keys)
        {
            var allKeys = keys.FromHardwareBits(ControllerKeyHardwareBits, true);
            var actionKeyMatches = allKeys.Where(k => k.HasFlag(ControllerKeys.ActionKeyActive));
            return actionKeyMatches;
        }

        /// <summary>
        /// Gets the disc plus action key aliases.
        /// </summary>
        /// <param name="keys">The controller input to examine.</param>
        /// <param name="adjacentDistance">A value from 0-7 indicating how many additional adjacent disc directions should be included in the output. See remarks.</param>
        /// <returns>The disc plus action key aliases.</returns>
        /// <remarks>For more details about <paramref name="adjacentDistance"/>, <see cref="GetDiscInputs(byte, sbyte)"/>. This function
        /// will determine if the hardware bits in the <paramref name="keys"/> input may result in the same bit pattern
        /// as a controller disc direction combined with any combination of action keys. Using a nonzero value for
        /// the <paramref name="adjacentDistance"/> values can be used to account for the possibility of a glitchy hardware
        /// controller as well. Using a value of 0 or 1, which is reasonable, will likely produce identical results because
        /// of how disc input is encoded. Larger values of 2 or 3 are essentially not useful, because multiple disc inputs
        /// such as ENE and SSE are not reasonably likely. Such a controller would essentially be defective.</remarks>
        public static IEnumerable<ControllerKeys> GetDiscPlusActionKeyAliases(this byte keys, sbyte adjacentDistance)
        {
            var firstMatchingDiscPlusActionKeyInput = Enumerable.Empty<ControllerKeys>();
            var actionKeys = ControllerKeyHardwareBits.Keys.Where(k => k.HasFlag(ControllerKeys.ActionKeyActive) && ((k & ~ControllerKeys.ActionKeyActive) != 0));
            var discDirections = keys.GetDiscInputs();
            foreach (var discDirection in discDirections)
            {
                // Get some 'slop' around each to account for glitchy controllers.
                var possibleInputs = discDirection.ToHardwareBits().GetDiscInputs(adjacentDistance).ToList();
                possibleInputs.AddRange(actionKeys);
                var allCombinations = Enumerable.Range(1, (1 << possibleInputs.Count) - 1).Select(i => possibleInputs.Where((input, j) => ((1 << j) & i) != 0).ToList()).Where(combo => (combo.ToHardwareBits() & 0xC0) != 0);
                var discPlusActionKeyMatch = allCombinations.FirstOrDefault(combo => combo.ToHardwareBits() == keys);
                if ((discPlusActionKeyMatch != null) && discPlusActionKeyMatch.Any())
                {
                    firstMatchingDiscPlusActionKeyInput = discPlusActionKeyMatch;
                    break;
                }
            }
            return firstMatchingDiscPlusActionKeyInput;
        }

        private static IEnumerable<ControllerKeys> FromHardwareBits(this byte keys, IEnumerable<KeyValuePair<ControllerKeys, byte>> entries, bool getAllMatches)
        {
            var controllerKeys = new List<ControllerKeys>();
            if (keys != 0xFF)
            {
                byte accumulatedKeys = 0;
                foreach (var entry in entries)
                {
                    if ((keys & entry.Value) == entry.Value)
                    {
                        controllerKeys.Add(entry.Key);
                        accumulatedKeys |= entry.Value;
                        if (!getAllMatches && (accumulatedKeys == keys))
                        {
                            // We've got them all, so break out.
                            break;
                        }
                    }
                }
            }
            return controllerKeys;
        }
    }
}

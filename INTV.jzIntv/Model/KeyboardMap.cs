// <copyright file="KeyboardMap.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// The supported standard keyboard maps.
    /// </summary>
    public enum KeyboardMap
    {
        /// <summary>The default keymap.</summary>
        Default = 0,

        /// <summary>The one-player mode keymap (left controller only).</summary>
        LeftControllerOnly,

        /// <summary>The ECS keymap.</summary>
        EcsKeyboard,

        /// <summary>Command keys keymap (access special features such as quit, reset, pause).</summary>
        CommandKeys
    }

    /// <summary>
    /// Helper methods for the KeyboardMap enumeration.
    /// </summary>
    public static class KeyboardMapHelpers
    {
        private static readonly Dictionary<KeyboardMap, string> DisplayValues = new Dictionary<KeyboardMap, string>()
        {
            { KeyboardMap.Default, INTV.jzIntv.Resources.Strings.KeyboardMap_Default },
            { KeyboardMap.LeftControllerOnly, INTV.jzIntv.Resources.Strings.KeyboardMap_LeftControllerOnly },
            { KeyboardMap.EcsKeyboard, INTV.jzIntv.Resources.Strings.KeyboardMap_EcsKeyboard },
            { KeyboardMap.CommandKeys, INTV.jzIntv.Resources.Strings.KeyboardMap_CommandKeys },
        };

        /// <summary>
        /// Gets a display string for a KeyboardMap value.
        /// </summary>
        /// <param name="keyboardMap">The keyboard map value to get a user interface display string for.</param>
        /// <returns>The display string.</returns>
        public static string ToDisplayString(this KeyboardMap keyboardMap)
        {
            return DisplayValues[keyboardMap];
        }

        /// <summary>
        /// Converts a string stored in settings to one of the enumeration's values.
        /// </summary>
        /// <param name="setting">The setting string to convert to an enum value.</param>
        /// <returns>The converted value, or KeyboardMap.Default if the string does not correspond to a valid value.</returns>
        public static KeyboardMap FromSettingString(string setting)
        {
            KeyboardMap map;
            if (string.IsNullOrEmpty(setting) || !System.Enum.TryParse<KeyboardMap>(setting, out map))
            {
                map = KeyboardMap.Default;
            }
            return map;
        }
    }
}

// <copyright file="DisplayMode.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
    /// The display modes supported by jzIntv.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>The default display mode for jzIntv mode.</summary>
        Default = Windowed,

        /// <summary>Play in windowed mode.</summary>
        Windowed = 0,

        /// <summary>Play in full-screen mode.</summary>
        Fullscreen = 1
    }

    /// <summary>
    /// Helper methods for the DisplayMode enumeration.
    /// </summary>
    public static class DisplayModeHelpers
    {
        private static readonly Dictionary<DisplayMode, string> DisplayModeDisplayStrings = new Dictionary<DisplayMode, string>()
        {
            { DisplayMode.Windowed, "Windowed" },
            { DisplayMode.Fullscreen, "Full Screen" },
        };

        /// <summary>
        /// Converts the given value to a string for display to the user.
        /// </summary>
        /// <param name="displayMode">The value to convert.</param>
        /// <returns>The display string.</returns>
        public static string ToDisplayString(this DisplayMode displayMode)
        {
            return DisplayModeDisplayStrings[displayMode];
        }

        /// <summary>
        /// Given a language-invariant string used to store DisplayMode, convert it back to the enumeration value.
        /// </summary>
        /// <param name="displayModeSetting">The string to convert to a DisplayMode value.</param>
        /// <returns>The DisplayMode value described by the string, or the default value if the input string is not a valid DisplayMode string.</returns>
        public static DisplayMode FromSettingString(string displayModeSetting)
        {
            DisplayMode displayMode;
            if (string.IsNullOrWhiteSpace(displayModeSetting) || !System.Enum.TryParse<DisplayMode>(displayModeSetting, out displayMode))
            {
                displayMode = DisplayMode.Default;
            }
            return displayMode;
        }
    }
}

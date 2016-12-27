// <copyright file="ColorHelpers.cs" company="INTV Funhouse">
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
using System.Linq;

using IntvColor = INTV.Core.Model.Stic.Color;

#if WIN || WIN_XP
using OSColor = System.Windows.Media.Color;
#elif MAC
#if __UNIFIED__
using OSColor = AppKit.NSColor;
#else
using OSColor = MonoMac.AppKit.NSColor;
#endif
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Extension methods for working with the Intellivision colors and operating system visual colors.
    /// </summary>
    public static partial class ColorHelpers
    {
        /// <summary>
        /// Color for the "Intellivision Gold" for the original Master Component's appearance. Just kinda eyeballed it.
        /// </summary>
        public static readonly OSColor IntellivisionGold = FromRgb(0xef, 0xe4, 0xb0);

        private static readonly Dictionary<IntvColor, OSColor> IntvToOSColorDictionary = new Dictionary<IntvColor, OSColor>()
        {
            { IntvColor.NotAColor, FromRgb(0x66, 0x66, 0x66) },
            { IntvColor.Black, FromRgb(0x00, 0x00, 0x00) },
            { IntvColor.Blue, FromRgb(0x00, 0x2d, 0xff) },
            { IntvColor.Red, FromRgb(0xff, 0x3d, 0x10) },
            { IntvColor.Tan, FromRgb(0xc9, 0xcf, 0xab) },
            { IntvColor.DarkGreen, FromRgb(0x38, 0x6b, 0x3f) },
            { IntvColor.Green, FromRgb(0x00, 0xa7, 0x56) },
            { IntvColor.Yellow, FromRgb(0xfa, 0xea, 0x50) },
            { IntvColor.White, FromRgb(0xff, 0xfc, 0xff) },
            { IntvColor.Grey, FromRgb(0xBD, 0xAC, 0xC8) },
            { IntvColor.Cyan, FromRgb(0x24, 0xB8, 0xFF) },
            { IntvColor.Orange, FromRgb(0xFF, 0xB4, 0x1F) },
            { IntvColor.Brown, FromRgb(0x54, 0x6E, 0x00) },
            { IntvColor.Pink, FromRgb(0xFF, 0x4E, 0x57) },
            { IntvColor.LightBlue, FromRgb(0xA4, 0x96, 0xFF) },
            { IntvColor.YellowGreen, FromRgb(0x75, 0xCC, 0x80) },
            { IntvColor.Purple, FromRgb(0xB5, 0x1A, 0x58) }
        };

        private static readonly Dictionary<IntvColor, string> ColorNames = new Dictionary<IntvColor, string>()
        {
            { IntvColor.NotAColor, Resources.Strings.Color_NotAColor },
            { IntvColor.Black, Resources.Strings.Color_Black },
            { IntvColor.Blue, Resources.Strings.Color_Blue },
            { IntvColor.Red, Resources.Strings.Color_Red },
            { IntvColor.Tan, Resources.Strings.Color_Tan },
            { IntvColor.DarkGreen, Resources.Strings.Color_DarkGreen },
            { IntvColor.Green, Resources.Strings.Color_Green },
            { IntvColor.Yellow, Resources.Strings.Color_Yellow },
            { IntvColor.White, Resources.Strings.Color_White },
            { IntvColor.Grey, Resources.Strings.Color_Grey },
            { IntvColor.Cyan, Resources.Strings.Color_Cyan },
            { IntvColor.Orange, Resources.Strings.Color_Orange },
            { IntvColor.Brown, Resources.Strings.Color_Brown },
            { IntvColor.Pink, Resources.Strings.Color_Pink },
            { IntvColor.LightBlue, Resources.Strings.Color_LightBlue },
            { IntvColor.YellowGreen, Resources.Strings.Color_YellowGreen },
            { IntvColor.Purple, Resources.Strings.Color_Purple }
        };

        /// <summary>
        /// Converts an Intellivision color to an OS-specific color.
        /// </summary>
        /// <param name="color">The Intellivision color to convert.</param>
        /// <returns>The corresponding OS-specific color.</returns>
        public static OSColor ToColor(this IntvColor color)
        {
            return IntvToOSColorDictionary[color];
        }

        /// <summary>
        /// Converts a specific OS color to an Intellivision color.
        /// </summary>
        /// <param name="color">The OS-specific color to convert.</param>
        /// <returns>The Intellivision color.</returns>
        public static IntvColor FromColor(this OSColor color)
        {
            var intvColor = IntvColor.NotAColor;
            if (IntvToOSColorDictionary.ContainsValue(color))
            {
                intvColor = IntvToOSColorDictionary.First(c => c.Value == color).Key;
            }
            return intvColor;
        }

        /// <summary>
        /// Converts an Intellivision color to a user-readable string.
        /// </summary>
        /// <param name="color">The color whose display name is desired.</param>
        /// <returns>The name of the color.</returns>
        public static string ToDisplayString(this IntvColor color)
        {
            return ColorNames[color];
        }

        /// <summary>
        /// Attempts to convert a string into an Intellivision color.
        /// </summary>
        /// <param name="colorName">The string to attempt to convert back to an Intellivision color.</param>
        /// <returns>The Intellivision color, or NotAColor if the conversion fails.</returns>
        public static IntvColor FromDisplayString(this string colorName)
        {
            var color = IntvColor.NotAColor;
            color = ColorNames.First(c => c.Value == colorName).Key;
            return color;
        }
    }
}

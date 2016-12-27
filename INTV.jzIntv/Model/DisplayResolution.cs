// <copyright file="DisplayResolution.cs" company="INTV Funhouse">
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

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// The supported resolutions for jzIntv.
    /// </summary>
    public enum DisplayResolution
    {
        /// <summary>The default display resolution to use.</summary>
        Default = Resolution320x240x16bpp,

        /// <summary>Resolution of 320x200 pixels, 8-bit color depth.</summary>
        Resolution320x200x8bpp = 0,

        /// <summary>Resolution of 640x480 pixels, 8-bit color depth.</summary>
        Resolution640x480x8bpp = 1,

        /// <summary>Resolution of 320x240 pixels, 16-bit color depth.</summary>
        Resolution320x240x16bpp = 2,

        /// <summary>Resolution of 1024x768 pixels, 8-bit color depth.</summary>
        Resolution1024x768x8bpp = 3,

        /// <summary>Resolution of 1680x1050 pixels, 8-bit color depth.</summary>
        Resolution1680x1050x8bpp = 4,

        /// <summary>Resolution of 800x400 pixels, 16-bit color depth.</summary>
        Resolution800x400x16bpp = 5,

        /// <summary>Resolution of 1600x1200 pixels, 32-bit color depth.</summary>
        Resolution1600x1200x32bpp = 6,

        /// <summary>Resolution of 3280x1200 pixels, 32-bit color depth.</summary>
        Resolution3280x1200x32bpp = 7,
    }

    /// <summary>
    /// Helper methods for the DisplayResolution enumeration.
    /// </summary>
    public static class DisplayResolutionHelpers
    {
        private static readonly Dictionary<DisplayResolution, string> ResolutionStrings = new Dictionary<DisplayResolution, string>()
        {
            { DisplayResolution.Resolution320x200x8bpp, "320x200,8bpp" },
            { DisplayResolution.Resolution640x480x8bpp, "640x480,8bpp" },
            { DisplayResolution.Resolution1024x768x8bpp, "1024x768,8bpp" },
            { DisplayResolution.Resolution1680x1050x8bpp, "1680x1050,8bpp" },
            { DisplayResolution.Resolution800x400x16bpp, "800x400,16bpp" },
            { DisplayResolution.Resolution1600x1200x32bpp, "1600x1200,32bpp" },
            { DisplayResolution.Resolution3280x1200x32bpp, "3280x1200,32bpp" },
            { DisplayResolution.Resolution320x240x16bpp, "320x240,16bpp" },
        };

        /// <summary>
        /// Convert the given resolution to a "long"-format command line argument string.
        /// </summary>
        /// <param name="resolution">The value to convert to a long-format command line argument.</param>
        /// <returns>The long-format command line argument string.</returns>
        public static string ToLongCommandLineArgumentString(this DisplayResolution resolution)
        {
            var resolutionString = string.Empty;
            if (!ResolutionStrings.TryGetValue(resolution, out resolutionString))
            {
                resolutionString = ResolutionStrings[DisplayResolution.Default];
            }
            return resolutionString;
        }

        /// <summary>
        /// Convert the given resolution to a short command line string.
        /// </summary>
        /// <param name="resolution">The resolution to convert.</param>
        /// <returns>The short command line argument string, which is just a number.</returns>
        public static string ToShortCommandLineArgumentString(this DisplayResolution resolution)
        {
            return ((int)resolution).ToString();
        }

        /// <summary>
        /// Convert a long-format resolution string to one of the predefined resolution values.
        /// </summary>
        /// <param name="value">The long-format command line string to convert.</param>
        /// <returns>The resolution described by the long-form command line argument string; uses default value if not recognized.</returns>
        public static DisplayResolution FromLongCommandLineArgumentString(string value)
        {
            var resolution = DisplayResolution.Default;
            var valueIndex = ResolutionStrings.Values.ToList().IndexOf(value);
            if (valueIndex >= 0)
            {
                resolution = ResolutionStrings.ElementAt(valueIndex).Key;
            }
            return resolution;
        }

        /// <summary>
        /// Converts the given resolution value to a user-friendly display string.
        /// </summary>
        /// <param name="resolution">The resolution to convert.</param>
        /// <returns>The display string.</returns>
        public static string ToDisplayString(this DisplayResolution resolution)
        {
            var displayString = string.Empty;
            switch (resolution)
            {
                case DisplayResolution.Resolution320x200x8bpp:
                    displayString = "320x200, 8 bit";
                    break;
                case DisplayResolution.Resolution640x480x8bpp:
                    displayString = "640x480, 8 bit";
                    break;
                case DisplayResolution.Resolution1024x768x8bpp:
                    displayString = "1024x768, 8 bit";
                    break;
                case DisplayResolution.Resolution1680x1050x8bpp:
                    displayString = "1680x1050, 8 bit";
                    break;
                case DisplayResolution.Resolution800x400x16bpp:
                    displayString = "800x400, 16 bit";
                    break;
                case DisplayResolution.Resolution1600x1200x32bpp:
                    displayString = "1600x1200, 32 bit";
                    break;
                case DisplayResolution.Resolution3280x1200x32bpp:
                    displayString = "3280x1200, 32 bit";
                    break;
                case DisplayResolution.Resolution320x240x16bpp:
                default:
                    displayString = "320x240, 16 bit";
                    break;
            }
            return displayString;
        }
    }
}

// <copyright file="FgBgBackgroundColor.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.Core.Model.Stic
{
    /// <summary>
    /// Background colors for FG/BG mode.
    /// </summary>
    public enum FgBgBackgroundColor : ushort
    {
        /// <summary>
        /// Not a valid color.
        /// </summary>
        NotAColor = Color.NotAColor,

        /// <summary>
        /// Black for background color.
        /// </summary>
        Black = 0x00 << 9,

        /// <summary>
        /// Blue for background color.
        /// </summary>
        Blue = 0x01 << 9,

        /// <summary>
        /// Red for background color.
        /// </summary>
        Red = 0x02 << 9,

        /// <summary>
        /// Tan for background color.
        /// </summary>
        Tan = 0x03 << 9,

        /// <summary>
        /// DarkGreen for background color.
        /// </summary>
        DarkGreen = 0x10 << 9,

        /// <summary>
        /// Green for background color.
        /// </summary>
        Green = 0x11 << 9,

        /// <summary>
        /// Yellow for background color.
        /// </summary>
        Yellow = 0x12 << 9,

        /// <summary>
        /// White for background color.
        /// </summary>
        White = 0x13 << 9,

        /// <summary>
        /// Grey for background color.
        /// </summary>
        Grey = 0x08 << 9,

        /// <summary>
        /// Cyan for background color.
        /// </summary>
        Cyan = 0x09 << 9,

        /// <summary>
        /// Orange for background color.
        /// </summary>
        Orange = 0x0A << 9,

        /// <summary>
        /// Brown for background color.
        /// </summary>
        Brown = 0x0B << 9,

        /// <summary>
        /// Pink for background color.
        /// </summary>
        Pink = 0x18 << 9,

        /// <summary>
        /// LightBlue for background color.
        /// </summary>
        LightBlue = 0x19 << 9,

        /// <summary>
        /// YellowGreen for background color.
        /// </summary>
        YellowGreen = 0x1A << 9,

        /// <summary>
        /// Purple for background color.
        /// </summary>
        Purple = 0x1B << 9
    }
}

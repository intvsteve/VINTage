// <copyright file="CsForegroundColor.cs" company="INTV Funhouse">
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
    /// Foreground colors for color stack mode.
    /// </summary>
    public enum CsForegroundColor : ushort
    {
        /// <summary>
        /// Not a valid color.
        /// </summary>
        NotAColor = Color.NotAColor,

        /// <summary>
        /// Black for foreground color.
        /// </summary>
        Black = Color.Black,

        /// <summary>
        /// Blue for foreground color.
        /// </summary>
        Blue = Color.Blue,

        /// <summary>
        /// Red for foreground color.
        /// </summary>
        Red = Color.Red,

        /// <summary>
        /// Tan for foreground color.
        /// </summary>
        Tan = Color.Tan,

        /// <summary>
        /// DarkGreen for foreground color.
        /// </summary>
        DarkGreen = Color.DarkGreen,

        /// <summary>
        /// Green for foreground color.
        /// </summary>
        Green = Color.Green,

        /// <summary>
        /// Yellow for foreground color.
        /// </summary>
        Yellow = Color.Yellow,

        /// <summary>
        /// White for foreground color.
        /// </summary>
        White = Color.White,
        
        /// <summary>
        /// Grey for foreground color.
        /// </summary>
        Grey = 0x1000,

        /// <summary>
        /// Cyan for foreground color.
        /// </summary>
        Cyan,

        /// <summary>
        /// Orange for foreground color.
        /// </summary>
        Orange,

        /// <summary>
        /// Brown for foreground color.
        /// </summary>
        Brown,

        /// <summary>
        /// Pink for foreground color.
        /// </summary>
        Pink,

        /// <summary>
        /// LightBlue for foreground color.
        /// </summary>
        LightBlue,

        /// <summary>
        /// YellowGreen for foreground color.
        /// </summary>
        YellowGreen,

        /// <summary>
        /// Purple for foreground color.
        /// </summary>
        Purple,
    }
}

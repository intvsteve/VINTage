// <copyright file="FgBgForegroundColor.cs" company="INTV Funhouse">
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
    /// Foreground colors for FG/BG mode.
    /// </summary>
    public enum FgBgForegroundColor : byte
    {
        /// <summary>
        /// Not a valid color.
        /// </summary>
        NotAColor = Color.NotAColor,

        /// <summary>
        /// Black foreground color.
        /// </summary>
        BlackFG = Color.Black,

        /// <summary>
        /// Blue foreground color.
        /// </summary>
        BlueFG = Color.Blue,

        /// <summary>
        /// Red foreground color.
        /// </summary>
        RedFG = Color.Red,

        /// <summary>
        /// Tan foreground color.
        /// </summary>
        TanFG = Color.Tan,

        /// <summary>
        /// DarkGreen foreground color.
        /// </summary>
        DarkGreenFG = Color.DarkGreen,

        /// <summary>
        /// Green foreground color.
        /// </summary>
        GreenFG = Color.Green,

        /// <summary>
        /// Yellow foreground color.
        /// </summary>
        YellowFG = Color.Yellow,

        /// <summary>
        /// White foreground color.
        /// </summary>
        WhiteFG = Color.White
    }
}

// <copyright file="MobColor.cs" company="INTV Funhouse">
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
    /// Colors for MOBs.
    /// </summary>
    public enum MobColor : ushort
    {
        /// <summary>
        /// Not a valid color.
        /// </summary>
        NotAColor = Color.NotAColor,

        /// <summary>
        /// Black for MOBs.
        /// </summary>
        Black = CsForegroundColor.Black,

        /// <summary>
        /// Blue for MOBs.
        /// </summary>
        Blue = CsForegroundColor.Blue,

        /// <summary>
        /// Red for MOBs.
        /// </summary>
        Red = CsForegroundColor.Red,

        /// <summary>
        /// Tan for MOBs.
        /// </summary>
        Tan = CsForegroundColor.Tan,

        /// <summary>
        /// DarkGreen for MOBs.
        /// </summary>
        DarkGreen = CsForegroundColor.DarkGreen,

        /// <summary>
        /// Green for MOBs.
        /// </summary>
        Green = CsForegroundColor.Green,

        /// <summary>
        /// Yellow for MOBs.
        /// </summary>
        Yellow = CsForegroundColor.Yellow,

        /// <summary>
        /// White for MOBs.
        /// </summary>
        White = CsForegroundColor.White,

        /// <summary>
        /// Grey for MOBs.
        /// </summary>
        Grey = CsForegroundColor.Grey,

        /// <summary>
        /// Cyan for MOBs.
        /// </summary>
        Cyan = CsForegroundColor.Cyan,

        /// <summary>
        /// Orange for MOBs.
        /// </summary>
        Orange = CsForegroundColor.Orange,

        /// <summary>
        /// Brown for MOBs.
        /// </summary>
        Brown = CsForegroundColor.Brown,

        /// <summary>
        /// PinkFG for MOBs.
        /// </summary>
        PinkFG = CsForegroundColor.Pink,

        /// <summary>
        /// LightBlue for MOBs.
        /// </summary>
        LightBlue = CsForegroundColor.LightBlue,

        /// <summary>
        /// YellowGreen for MOBs.
        /// </summary>
        YellowGreen = CsForegroundColor.YellowGreen,

        /// <summary>
        /// Purple for MOBs.
        /// </summary>
        Purple = CsForegroundColor.Purple
    }
}

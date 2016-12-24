// <copyright file="Color.cs" company="INTV Funhouse">
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
    /// The basic colors available in the STIC. These can be used when the bits are all contiguously used.
    /// </summary>
    public enum Color : byte
    {
        /// <summary>
        /// Not a valid color.
        /// </summary>
        NotAColor = 0xFF,

        /// <summary>
        /// Black for contiguous storage.
        /// </summary>
        Black = 0,

        /// <summary>
        /// Blue for contiguous storage.
        /// </summary>
        Blue,

        /// <summary>
        /// Red for contiguous storage.
        /// </summary>
        Red,

        /// <summary>
        /// Tan for contiguous storage.
        /// </summary>
        Tan,

        /// <summary>
        /// DarkGreen for contiguous storage.
        /// </summary>
        DarkGreen,

        /// <summary>
        /// Green for contiguous storage.
        /// </summary>
        Green,

        /// <summary>
        /// Yellow for contiguous storage.
        /// </summary>
        Yellow,

        /// <summary>
        /// White for contiguous storage.
        /// </summary>
        White,

        /// <summary>
        /// Grey for contiguous storage.
        /// </summary>
        Grey,

        /// <summary>
        /// Cyan for contiguous storage.
        /// </summary>
        Cyan,

        /// <summary>
        /// Orange for contiguous storage.
        /// </summary>
        Orange,

        /// <summary>
        /// Brown for contiguous storage.
        /// </summary>
        Brown,

        /// <summary>
        /// Pink for contiguous storage.
        /// </summary>
        Pink,

        /// <summary>
        /// LightBlue for contiguous storage.
        /// </summary>
        LightBlue,

        /// <summary>
        /// YellowGreen for contiguous storage.
        /// </summary>
        YellowGreen,

        /// <summary>
        /// Purple for contiguous storage.
        /// </summary>
        Purple
    }
}

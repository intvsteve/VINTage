// <copyright file="Mode.cs" company="INTV Funhouse">
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
    /// Operating modes for the STIC. Note that Colored Squares is really just a special formatting of the BACKTAB data,
    /// and not treated as a specific mode of the STIC.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// The STIC uses a four-element color stack to determine the background color of the tiles in the BACKTAB.
        /// </summary>
        ColorStack,

        /// <summary>
        /// The STIC uses each element in the BACKTAB to determine the background color of each tile.
        /// </summary>
        ForegroundBackground
    }
}

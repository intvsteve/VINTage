// <copyright file="ForkKind.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Forks supported in the LFS.
    /// </summary>
    public enum ForkKind
    {
        /// <summary>
        /// Special value indicating an invalid fork.
        /// </summary>
        None = -1,

        /// <summary>
        /// Sentinel value.
        /// </summary>
        FirstKind = 0,

        /// <summary>
        /// A fork that stores a program ROM.
        /// </summary>
        Program = FirstKind,

        /// <summary>
        /// Synonym for Game ROM fork.
        /// </summary>
        Game = Program,

        /// <summary>
        /// A fork that stores a manual.
        /// </summary>
        Manual,

        /// <summary>
        /// A fork that stores JLP Flash data.
        /// </summary>
        JlpFlash,

        /// <summary>
        /// A fork that stores a vignette.
        /// </summary>
        Vignette,

        /// <summary>
        /// This fork is reserved for future use.
        /// </summary>
        Reserved4,

        /// <summary>
        /// This fork is reserved for future use.
        /// </summary>
        Reserved5,

        /// <summary>
        /// This fork is reserved for future use.
        /// </summary>
        Reserved6,

        /// <summary>
        /// The number of forks supported by LFS files.
        /// </summary>
        NumberOfForkKinds
    }
}

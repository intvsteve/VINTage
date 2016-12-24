// <copyright file="RomListColumn.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// The columns that may be displayed in a list of ROMs.
    /// </summary>
    public enum RomListColumn
    {
        /// <summary>
        /// Not a valid column.
        /// </summary>
        None = 0,

        /// <summary>
        /// Program title.
        /// </summary>
        Title,

        /// <summary>
        /// Vendor who developed or sold the program.
        /// </summary>
        Vendor,

        /// <summary>
        /// Typically the copyright date, though in some cases year of release.
        /// </summary>
        Year,

        /// <summary>
        /// Special features the program may use or require (e.g. Intellivoice, ECS).
        /// </summary>
        Features,

        /// <summary>
        /// The path to the ROM on disk.
        /// </summary>
        RomFile,

        /// <summary>
        /// The path to a text file containing program instructions.
        /// </summary>
        ManualFile
    }
}

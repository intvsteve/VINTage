// <copyright file="AddRomsFromFilesEndEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

using System;
using System.Collections.Generic;

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Event argument passed when adding ROMs has completed.
    /// </summary>
    public class AddRomsFromFilesEndEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="duplicateRomPaths">Duplicate ROMs that were discovered during the add and not included in the update to the ROM list.</param>
        internal AddRomsFromFilesEndEventArgs(IEnumerable<string> duplicateRomPaths)
        {
            DuplicateRomPaths = duplicateRomPaths;
        }

        /// <summary>
        /// Gets an enumerable of the paths to ROMs that were already in the list. Note that these are not duplicate disk paths, but rather
        /// ROMs having the same CRC as those already in the list.
        /// </summary>
        public IEnumerable<string> DuplicateRomPaths { get; private set; }
    }
}

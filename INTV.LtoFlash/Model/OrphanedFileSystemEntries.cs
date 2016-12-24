// <copyright file="OrphanedFileSystemEntries.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class gathers orphaned file system objects.
    /// </summary>
    public class OrphanedFileSystemEntries : Tuple<IEnumerable<IGlobalFileSystemEntry>, IEnumerable<IGlobalFileSystemEntry>, IEnumerable<IGlobalFileSystemEntry>>
    {
        /// <summary>
        /// Initialize a new instance of OrphanedFileSystemEntries.
        /// </summary>
        /// <param name="orphanedForks">File system Fork entries that are not referenced by any files.</param>
        /// <param name="orphanedFiles">File system file entries that are not referenced by any directories.</param>
        /// <param name="orphanedDirectories">File system directory entries that are not referenced by the root or any other directories.</param>
        public OrphanedFileSystemEntries(IEnumerable<IGlobalFileSystemEntry> orphanedForks, IEnumerable<IGlobalFileSystemEntry> orphanedFiles, IEnumerable<IGlobalFileSystemEntry> orphanedDirectories)
            : base(orphanedForks, orphanedFiles, orphanedDirectories)
        {
        }

        /// <summary>
        /// Gets the set of orphaned forks discovered in a FileSystem. These forks are not associated with a file.
        /// </summary>
        public IEnumerable<IGlobalFileSystemEntry> OrphanedForks
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the orphaned files discovered in a FileSystem. These files are not associated with a directory.
        /// </summary>
        public IEnumerable<IGlobalFileSystemEntry> OrphanedFiles
        {
            get { return Item2; }
        }

        /// <summary>
        /// Gets the orphaned directories discovered in a FileSystem. These directories cannot be associated with a parent file, or the parent file is not associated with any directory.
        /// </summary>
        public IEnumerable<IGlobalFileSystemEntry> OrphanedDirectories
        {
            get { return Item3; }
        }
    }
}

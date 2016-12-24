// <copyright file="IGlobalFileSystemEntry.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This interface describes basic information about entries in one of the global tables used by the Locutus File System.
    /// </summary>
    public interface IGlobalFileSystemEntry
    {
        /// <summary>
        /// Gets the flat size of the entry to use when updating a file system on the device.
        /// </summary>
        int EntryUpdateSize { get; }

        /// <summary>
        /// Gets a unique descriptor of the entry. This meaning of this value is not defined or used by the file system.
        /// </summary>
        uint Uid { get; }

        /// <summary>
        /// Gets a user-readable name of the entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the FileSystem of which the entry is a member. A FileSystem is the set of all the global tables used by the file system.
        /// </summary>
        FileSystem FileSystem { get; }

        /// <summary>
        /// Creates a clone of the file system entry.
        /// </summary>
        /// <param name="fileSystem">The file system to clone into.</param>
        /// <returns>The cloned object.</returns>
        IGlobalFileSystemEntry Clone(FileSystem fileSystem);
    }
}

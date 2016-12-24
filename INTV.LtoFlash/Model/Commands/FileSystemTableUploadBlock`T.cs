// <copyright file="FileSystemTableUploadBlock`T.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Describes a block of data that can be uploaded to Locutus device.
    /// </summary>
    /// <typeparam name="T">The data type of the file system object.</typeparam>
    internal sealed class FileSystemTableUploadBlock<T> : System.Tuple<uint, ContiguousFileSystemEntries<T>> where T : IGlobalFileSystemEntry
    {
        /// <summary>
        /// Creates a new instance of FileSystemTableUploadBlock.
        /// </summary>
        /// <param name="baseGlobalFileSystemNumber">The global file system number of the first entry in a contiguous block of entries.</param>
        /// <param name="entries">The entries in the contiguous block of objects to be uploaded.</param>
        public FileSystemTableUploadBlock(uint baseGlobalFileSystemNumber, ContiguousFileSystemEntries<T> entries)
            : base(baseGlobalFileSystemNumber, entries)
        {
        }

        /// <summary>
        /// Gets the global file system number of the first object in the list.
        /// </summary>
        public uint GlobalFileSystemNumber
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the entries in the contiguous block of objects to be uploaded.
        /// </summary>
        public ContiguousFileSystemEntries<T> Entries
        {
            get { return Item2; }
        }
    }
}

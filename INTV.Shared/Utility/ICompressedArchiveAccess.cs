// <copyright file="ICompressedArchiveAccess.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.IO;
using INTV.Core.Utility;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// A basic interface that provides access to an archive, which may or may not be compressed.
    /// </summary>
    public interface ICompressedArchiveAccess : IStorageAccess, IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the instance is an archive.
        /// </summary>
        bool IsArchive { get; }

        /// <summary>
        /// Gets a value indicating whether the instance is compressed.
        /// </summary>
        bool IsCompressed { get; }

        /// <summary>
        /// Gets the format of the compressed archive.
        /// </summary>
        CompressedArchiveFormat Format { get; }

        /// <summary>
        /// Gets an enumerable of the entries contained in the archive.
        /// </summary>
        IEnumerable<ICompressedArchiveEntry> Entries { get; }

        /// <summary>
        /// Finds an entry with the given name.
        /// </summary>
        /// <param name="name">The name of the entry in the archive to find.</param>
        /// <returns>The entry, or <c>null</c> if not found.</returns>
        ICompressedArchiveEntry FindEntry(string name);

        /// <summary>
        /// Opens a stream that provides access to the contents of the entry.
        /// </summary>
        /// <param name="entry">The entry to get a stream for.</param>
        /// <returns>A stream that may be used to access the contents of the entry.</returns>
        /// <remarks>Whether the stream provides read or write access (or both) is implementation-defined.</remarks>
        Stream OpenEntry(ICompressedArchiveEntry entry);

        /// <summary>
        /// Creates a new entry in the archive with the given name. Use <see cref="OpenEntry(ICompressedArchiveEntry)"/> to provide content.
        /// </summary>
        /// <param name="name">The name of the new entry in the archive.</param>
        /// <returns>The new entry.</returns>
        /// <remarks>Whether a new entry can be created depends on the implementation.</remarks>
        ICompressedArchiveEntry CreateEntry(string name);

        /// <summary>
        /// Deletes an entry from the archive.
        /// </summary>
        /// <param name="name">The name of the entry to remove from the archive.</param>
        /// <returns><c>true</c> if the entry was removed, <c>false</c> otherwise.</returns>
        bool DeleteEntry(string name);
    }
}

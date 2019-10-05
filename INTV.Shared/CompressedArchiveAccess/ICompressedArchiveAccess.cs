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

namespace INTV.Shared.CompressedArchiveAccess
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

        /// <summary>
        /// Extracts an entry in the archive to a file.
        /// </summary>
        /// <param name="entry">The entry to extract.</param>
        /// <param name="destinationFilePath">The absolute path to the extraction location that is not in an other compressed archive.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="destinationFilePath"/> has a zero-length path, contains only white space, or contains
        /// one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars()"/>, or <paramref name="destinationFilePath"/> specifies a directory.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entry"/> or <paramref name="destinationFilePath"/> is <c>null</c>.</exception>
        /// <exception cref="PathTooLongException">Thrown if the specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the path specified in <paramref name="destinationFilePath"/>
        /// is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="InvalidDataException">Thrown if the entry is missing from the archive, or is corrupt and cannot be read, or the entry has been compressed by using a compression method that is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the archive that the entry belongs to has been disposed.</exception>
        /// <exception cref="IOException">Thrown if <paramref name="destinationFilePath"/> exists, or an I/O error occurs, or the entry is open for writing.</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="destinationFilePath"/> is an invalid format, or the archive for this entry was opened in Create mode,
        /// which does not permit the retrieval of entries, or if <paramref name="destinationFilePath"/> is in an archive.</exception>
        void ExtractEntry(ICompressedArchiveEntry entry, string destinationFilePath);

        /// <summary>
        /// Extracts an entry in the archive to a file.
        /// </summary>
        /// <param name="entry">The entry to extract.</param>
        /// <param name="destinationFilePath">The absolute path to the extraction location that is not in an other compressed archive.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrite an existing file at the destination location.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="destinationFilePath"/> has a zero-length path, contains only white space, or contains
        /// one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars()"/>, or <paramref name="destinationFilePath"/> specifies a directory.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entry"/> or <paramref name="destinationFilePath"/> is <c>null</c>.</exception>
        /// <exception cref="PathTooLongException">Thrown if the specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the path specified in <paramref name="destinationFilePath"/>
        /// is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="InvalidDataException">Thrown if the entry is missing from the archive, or is corrupt and cannot be read, or the entry has been compressed by using a compression method that is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the archive that the entry belongs to has been disposed.</exception>
        /// <exception cref="IOException">Thrown if <paramref name="destinationFilePath"/> exists and <paramref name="overwrite"/> is <c>false</c>, or an I/O error occurs, or the entry is open for writing.</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="destinationFilePath"/> is an invalid format, or the archive for this entry was opened in Create mode,
        /// which does not permit the retrieval of entries, or if <paramref name="destinationFilePath"/> is in an archive.</exception>
        void ExtractEntry(ICompressedArchiveEntry entry, string destinationFilePath, bool overwrite);
    }
}

// <copyright file="ZipArchiveAccess.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Options to optimize for speed or size when compressing data in the ZIP archive.
    /// </summary>
    public enum ZipArchiveCompressionMethod
    {
        /// <summary>Compress entries as much as possible, which will take longer.</summary>
        MaximumCompression,

        /// <summary>Compress entries quickly, which will consume more space.</summary>
        FastestCompression,

        /// <summary>Do not compress entries.</summary>
        NoCompression
    }

    /// <summary>
    /// Provides a .NET-agnostic mechanism to access ZIP archives.
    /// </summary>
    public sealed partial class ZipArchiveAccess : IDisposable
    {
        private IDisposable _zipArchiveObject; // the backing object

        /// <inheritdoc cref="ZipArchiveAccess(Stream, CompressedArchiveAccessMode)"/>
        /// <remarks>Accesses the stream using <see cref="CompressedArchiveAccessMode.Read"/></remarks>
        public ZipArchiveAccess(Stream stream)
            : this(stream, CompressedArchiveAccessMode.Read)
        {
        }

        /// <summary>
        /// Initialize a new instance of the type from the given stream.
        /// </summary>
        /// <param name="stream">Stream containing data in ZIP archive format.</param>
        /// <param name="mode">The access mode to use for ZIP operations.</param>
        public ZipArchiveAccess(Stream stream, CompressedArchiveAccessMode mode)
        {
            Mode = mode;
            _zipArchiveObject = Open(stream, mode);
        }

        /// <summary>
        /// Gets the names of the items in the ZIP archive.
        /// </summary>
        public IEnumerable<string> FileNames
        {
            get { return GetFileEntryNames(); }
        }

        /// <summary>
        /// Gets the access mode that describes actions that can be performed on the archive.
        /// </summary>
        public CompressedArchiveAccessMode Mode { get; private set; }

        /// <summary>
        /// Determines whether the file with the given name exists in the archive.
        /// </summary>
        /// <param name="fileName">The name of an entry in the archive, e.g. it's archive-relative path.</param>
        /// <returns><c>true</c> if an entry with the given name exists, <c>false</c> otherwise.</returns>
        public bool FileExists(string fileName)
        {
            return FileEntryExists(fileName);
        }

        /// <summary>
        /// Opens a read-only stream to the given file entry.
        /// </summary>
        /// <param name="fileName">The name of an entry in the archive to open, e.g. it's archive-relative path.</param>
        /// <returns>The stream to the entry.</returns>
        public Stream Open(string fileName)
        {
            return OpenFileEntry(fileName);
        }

        /// <summary>
        /// Adds a new file entry to the ZIP archive using the given compression method.
        /// </summary>
        /// <param name="fileName">The name of the file entry to create.</param>
        /// <param name="compressionMethod">The compression method to use.</param>
        /// <returns>A stream to which the file entry's contents can be written.</returns>
        public Stream Add(string fileName, ZipArchiveCompressionMethod compressionMethod)
        {
            return CreateAndOpenFileEntry(fileName, compressionMethod);
        }

        /// <summary>
        /// Deletes a file entry from the archive.
        /// </summary>
        /// <param name="fileName">The name of the file entry to remove from the archive.</param>
        public void Delete(string fileName)
        {
            DeleteFileEntry(fileName);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            if (_zipArchiveObject != null)
            {
                var zipArchiveObject = _zipArchiveObject;
                _zipArchiveObject = null;
                if (zipArchiveObject != null)
                {
                    zipArchiveObject.Dispose();
                }
            }
        }

        #endregion // IDisposable
    }
}

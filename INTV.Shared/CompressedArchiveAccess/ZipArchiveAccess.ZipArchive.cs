// <copyright file="ZipArchiveAccess.ZipArchive.cs" company="INTV Funhouse">
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
using System.IO.Compression;
using System.Linq;

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// Wraps access to the native ZipArchive implementation from System.IO.Compression in .NET 4.5 and later.
    /// </summary>
    internal sealed partial class ZipArchiveAccess
    {
        private ZipArchive ZipArchive
        {
            get { return (ZipArchive)_zipArchiveObject; }
        }

        /// <inheritdoc />
        protected override bool DeleteEntry(ICompressedArchiveEntry entry)
        {
            var zipEntry = entry as ZipEntry;
            var deleted = zipEntry != null;
            if (deleted)
            {
                zipEntry.Entry.Delete();
            }
            return deleted;
        }

        private static IDisposable Open(Stream stream, CompressedArchiveAccessMode mode)
        {
            var zipArchive = new ZipArchive(stream, (ZipArchiveMode)mode);
            return zipArchive;
        }

        private IEnumerable<ICompressedArchiveEntry> GetArchiveEntries()
        {
            return ZipArchive.Entries.Select(e => new ZipEntry(e));
        }

        private Stream OpenZipEntry(ICompressedArchiveEntry entry)
        {
            Stream stream = null;
            var zipEntry = entry as ZipEntry;
            if (zipEntry != null)
            {
                stream = zipEntry.Entry.Open();
            }
            return stream;
        }

        private ICompressedArchiveEntry CreateZipEntry(string fileName, ZipArchiveCompressionMethod compressionMethod)
        {
            ZipEntry entry = null;
            var zipArchiveEntry = ZipArchive.CreateEntry(fileName, (CompressionLevel)compressionMethod);
            if (zipArchiveEntry != null)
            {
                entry = new ZipEntry(zipArchiveEntry);
            }
            return entry;
        }

        /// <summary>
        /// Wraps <see cref="System.IO.Compression.ZipArchiveEntry"/> to expose as <see cref="ICompressedArchiveEntry"/>.
        /// </summary>
        private class ZipEntry : CompressedArchiveEntry
        {
            /// <summary>
            /// Initializes a new instance of <see cref="ZipEntry"/>.
            /// </summary>
            /// <param name="entry">The <see cref="System.IO.Compression.ZipArchiveEntry"/> to wrap.</param>
            public ZipEntry(ZipArchiveEntry entry)
            {
                _entry = entry;
            }

            /// <summary>
            /// Gets the wrapped <see cref="System.IO.Compression.ZipArchiveEntry"/>.
            /// </summary>
            public ZipArchiveEntry Entry
            {
                get { return _entry; }
            }
            private ZipArchiveEntry _entry;

            #region ICompressedArchiveEntry

            /// <inheritdoc />
            public override string Name
            {
                get { return Entry.FullName; }
            }

            /// <inheritdoc />
            public override long Length
            {
                get { return Entry.Length; }
            }

            /// <inheritdoc />
            public override DateTime LastModificationTime
            {
                get { return Entry.LastWriteTime.UtcDateTime; }
            }

            /// <inheritdoc />
            public override bool IsDirectory
            {
                get
                {
                    var isDirectory = string.IsNullOrEmpty(Entry.Name) && !string.IsNullOrEmpty(Entry.FullName)
                        && ((Entry.FullName.Last() == Path.DirectorySeparatorChar) || (Entry.FullName.Last() == Path.AltDirectorySeparatorChar));
                    return isDirectory;
                }
            }

            #endregion // ICompressedArchiveEntry
        }
    }
}

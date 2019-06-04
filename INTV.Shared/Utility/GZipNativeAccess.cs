// <copyright file="GZipNativeAccess.cs" company="INTV Funhouse">
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
using System.Text;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Provides access to a GZIP-formatted stream. Multiple-entry treatment should not be expected.
    /// </summary>
    internal sealed class GZipNativeAccess : CompressedArchiveAccess
    {
        private GZipNativeAccess(Stream stream, CompressionMode mode)
        {
            Mode = mode;
            GZipStream = new GZipStream(stream, mode, leaveOpen: true);
            var fileStream = stream as FileStream;
            if (fileStream != null)
            {
                RootLocation = fileStream.Name;
                Entry = new GZipEntry(fileStream);
            }
        }

        /// <inheritdoc />
        public override bool IsArchive
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override bool IsCompressed
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override IEnumerable<ICompressedArchiveEntry> Entries
        {
            get
            {
                if (Entry != null)
                {
                    yield return Entry;
                }
                else
                {
                    yield break;
                }
            }
        }

        private GZipStream GZipStream { get; set; }

        private CompressionMode Mode { get; set; }

        private GZipEntry Entry { get; set; }

        public static GZipNativeAccess Create(Stream stream, CompressedArchiveAccessMode mode)
        {
            var compressionMode = CompressedArchiveAccessModeToCompressionMode(mode);
            var gzipNativeAccess = new GZipNativeAccess(stream, compressionMode);
            return gzipNativeAccess;
        }

        /// <inheritdoc />
        public override Stream OpenEntry(ICompressedArchiveEntry entry)
        {
            // TODO: keep track of where we are in multi-member stream, etc.
            return GZipStream;
        }

        /// <inheritdoc />
        public override ICompressedArchiveEntry CreateEntry(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (GZipStream != null)
            {
                var gzipStream = GZipStream;
                GZipStream = null;
                if (gzipStream != null)
                {
                    gzipStream.Dispose();
                }
            }
        }

        /// <inheritdoc />
        protected override bool DeleteEntry(ICompressedArchiveEntry entry)
        {
            throw new NotSupportedException(Resources.Strings.CompressedArchiveAccess_GZipDeleteEntryNotSupported);
        }

        private static CompressionMode CompressedArchiveAccessModeToCompressionMode(CompressedArchiveAccessMode mode)
        {
            var compressionMode = CompressionMode.Compress;
            switch (mode)
            {
                case CompressedArchiveAccessMode.Read:
                    compressionMode = CompressionMode.Decompress;
                    break;
                case CompressedArchiveAccessMode.Create:
                case CompressedArchiveAccessMode.Update:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return compressionMode;
        }

        private class GZipEntry : ICompressedArchiveEntry
        {
            public GZipEntry(FileStream gzipFileStream)
            {
                var gzipFileLocation = gzipFileStream.Name;
                if (!string.IsNullOrEmpty(gzipFileLocation))
                {
                    Name = Path.GetFileNameWithoutExtension(gzipFileLocation);
                    Length = -1;
                    LastModificationTime = DateTime.MinValue;
                    FilePath = Path.Combine(Path.GetDirectoryName(gzipFileLocation), Name);
                }
            }

            public GZipEntry(string filePath)
            {
                var fileInfo = new FileInfo(filePath);
                Name = Path.GetFileNameWithoutExtension(filePath);
                Length = fileInfo.Length;
                LastModificationTime = fileInfo.LastWriteTimeUtc;
                FilePath = filePath;
            }

            /// <inheritdoc />
            public string Name { get; private set; }

            /// <inheritdoc />
            public long Length { get; private set; }

            /// <inheritdoc />
            public DateTime LastModificationTime { get; private set; }

            /// <inheritdoc />
            public bool IsDirectory
            {
                get { return false; }
            }

            /// <summary>
            /// Gets the absolute path to the entry, if available.
            /// </summary>
            public string FilePath { get; private set; }
        }
    }
}

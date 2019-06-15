// <copyright file="GZipAccessNative.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Provides access to a GZIP-formatted stream. Multiple-entry treatment is semi-supported. Don't have your hopes up too high, though.
    /// </summary>
    internal sealed class GZipAccessNative : CompressedArchiveAccess
    {
        private GZipAccessNative(Stream stream, CompressionMode mode)
        {
            Mode = mode;
            BaseStream = stream;
            var fileStream = stream as FileStream;
            if (fileStream != null)
            {
                RootLocation = fileStream.Name;
            }
            _entries = GZipMemberEntry.GetMemberEntries(stream, Properties.Settings.Default.MaxGZipEntriesSearch).ToList();
            switch (mode)
            {
                case CompressionMode.Decompress:
                    if (!_entries.Any())
                    {
                        throw new InvalidDataException(Resources.Strings.GZipAccess_NoEntriesFound);
                    }
                    break;
                case CompressionMode.Compress:
                    if (_entries.Any())
                    {
                        throw new InvalidOperationException(Resources.Strings.GZipAccess_EntriesAlreadyPresent);
                    }
                    break;
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
            get { return _entries; }
        }
        private List<GZipMemberEntry> _entries = new List<GZipMemberEntry>();

        private Stream BaseStream { get; set; }

        private CompressionMode Mode { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="GZipAccessNative"/> using the given mode.
        /// </summary>
        /// <param name="stream">Stream containing data in GZIP compressed format.</param>
        /// <param name="mode">The access mode to use for GZIP operations.</param>
        /// <returns>A new instance of <see cref="GZipAccessNative"/>.</returns>
        /// <remarks>The GZIP implementation assumes ownership of <paramref name="stream"/> and will dispose it.</remarks>
        public static GZipAccessNative Create(Stream stream, CompressedArchiveAccessMode mode)
        {
            var compressionMode = CompressedArchiveAccessModeToCompressionMode(mode);
            var gzipNativeAccess = new GZipAccessNative(stream, compressionMode);
            return gzipNativeAccess;
        }

        /// <inheritdoc />
        public override Stream OpenEntry(ICompressedArchiveEntry entry)
        {
            GZipStream entryStream = null;
            var gzipEntry = GetEntry(entry.Name) as GZipMemberEntry;
            if (gzipEntry != null)
            {
                BaseStream.Seek(gzipEntry.Offset, SeekOrigin.Begin);
                entryStream = new GZipStream(BaseStream, Mode, leaveOpen: true);
            }
            return entryStream;
        }

        /// <inheritdoc />
        public override ICompressedArchiveEntry CreateEntry(string name)
        {
            if (Mode != CompressionMode.Compress)
            {
                throw new InvalidOperationException(Resources.Strings.GZipAccess_InvalidModeForCreateEntryError);
            }
            if (_entries.Count > 0)
            {
                throw new NotSupportedException(Resources.Strings.GZipAccess_MultipleMembersNotSupportedError);
            }
            var entry = GZipMemberEntry.CreateEmptyEntry(name);
            _entries.Add(entry);
            return entry;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (BaseStream != null)
            {
                var baseStream = BaseStream;
                BaseStream = null;
                if (baseStream != null)
                {
                    baseStream.Dispose();
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
    }
}

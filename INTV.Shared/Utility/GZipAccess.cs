// <copyright file="GZipAccess.cs" company="INTV Funhouse">
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
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Provides common implementation for GZIP access.
    /// </summary>
    internal abstract class GZipAccess : CompressedArchiveAccess
    {
        protected GZipAccess(Stream stream, CompressedArchiveAccessMode mode)
        {
            IsReadOnly = mode == CompressedArchiveAccessMode.Read;
            BaseStream = stream;
            var fileStream = stream as FileStream;
            if (fileStream != null)
            {
                RootLocation = fileStream.Name;
            }
            _entries = GZipMemberEntry.GetMemberEntries(stream, Properties.Settings.Default.MaxGZipEntriesSearch).ToList();
            if (IsReadOnly)
            {
                if (!_entries.Any())
                {
                    throw new InvalidDataException(Resources.Strings.GZipAccess_NoEntriesFound);
                }
            }
            else
            {
                if (_entries.Any())
                {
                    throw new InvalidOperationException(Resources.Strings.GZipAccess_EntriesAlreadyPresent);
                }
            }
        }

        /// <inheritdoc />
        public sealed override bool IsArchive
        {
            get { return false; }
        }

        /// <inheritdoc />
        public sealed override bool IsCompressed
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override CompressedArchiveFormat Format
        {
            get { return CompressedArchiveFormat.GZip; }
        }

        /// <inheritdoc />
        public sealed override IEnumerable<ICompressedArchiveEntry> Entries
        {
            get { return _entries; }
        }
        private List<GZipMemberEntry> _entries = new List<GZipMemberEntry>();

        protected Stream BaseStream { get; private set; }

        protected bool IsReadOnly { get; private set; }

        /// <inheritdoc />
        public override Stream OpenEntry(ICompressedArchiveEntry entry)
        {
            Stream entryStream = null;
            var gzipEntry = GetEntry(entry.Name) as GZipMemberEntry;
            if (gzipEntry != null)
            {
                BaseStream.Seek(gzipEntry.Offset, SeekOrigin.Begin);
                entryStream = OpenStreamForEntry(gzipEntry);
            }
            return entryStream;
        }

        /// <inheritdoc />
        public override ICompressedArchiveEntry CreateEntry(string name)
        {
            if (IsReadOnly)
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

        /// <summary>
        /// Validates the provided mode.
        /// </summary>
        /// <param name="mode">The mode to validate.</param>
        /// <returns>The value of <paramref name="mode"/> if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported value for <paramref name="mode"/> is provided.</exception>
        protected static CompressedArchiveAccessMode ValidateMode(CompressedArchiveAccessMode mode)
        {
            switch (mode)
            {
                case CompressedArchiveAccessMode.Read:
                    break;
                case CompressedArchiveAccessMode.Create:
                    break;
                case CompressedArchiveAccessMode.Update:
                    throw new InvalidOperationException(Resources.Strings.GZipAccess_InvalidMode);
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, Resources.Strings.GZipAccess_InvalidMode);
            }
            return mode;
        }

        /// <summary>
        /// Opens a stream for the given entry.
        /// </summary>
        /// <param name="entry">The entry to open the stream for.</param>
        /// <returns>The stream to access the entry.</returns>
        /// <remarks>Note that <see cref="BaseStream"/> will already be position to the beginning of the entry by the caller.</remarks>
        protected abstract Stream OpenStreamForEntry(GZipMemberEntry entry);

        /// <inheritdoc />
        protected override bool DeleteEntry(ICompressedArchiveEntry entry)
        {
            throw new NotSupportedException(Resources.Strings.CompressedArchiveAccess_GZipDeleteEntryNotSupported);
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
    }
}

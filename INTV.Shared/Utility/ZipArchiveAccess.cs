﻿// <copyright file="ZipArchiveAccess.cs" company="INTV Funhouse">
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
    internal sealed partial class ZipArchiveAccess : CompressedArchiveAccess
    {
        private IDisposable _zipArchiveObject; // the backing object

        /// <inheritdoc cref="ZipArchiveAccess(Stream, CompressedArchiveAccessMode)"/>
        /// <remarks>Accesses the stream using <see cref="CompressedArchiveAccessMode.Read"/></remarks>
        private ZipArchiveAccess(Stream stream)
            : this(stream, CompressedArchiveAccessMode.Read)
        {
        }

        /// <summary>
        /// Initialize a new instance of the type from the given stream.
        /// </summary>
        /// <param name="stream">Stream containing data in ZIP archive format.</param>
        /// <param name="mode">The access mode to use for ZIP operations.</param>
        private ZipArchiveAccess(Stream stream, CompressedArchiveAccessMode mode)
        {
            Mode = mode;
            _zipArchiveObject = Open(stream, mode);
        }

        /// <summary>
        /// Gets the access mode that describes actions that can be performed on the archive.
        /// </summary>
        public CompressedArchiveAccessMode Mode { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ZipArchiveAccess"/> using the given mode.
        /// </summary>
        /// <param name="stream">Stream containing data in ZIP archive format.</param>
        /// <param name="mode">The access mode to use for ZIP operations.</param>
        /// <returns>A new instance of <see cref="ZipArchiveAccess"/>.</returns>
        public static ZipArchiveAccess Create(Stream stream, CompressedArchiveAccessMode mode)
        {
            return new ZipArchiveAccess(stream, mode);
        }

        #region ICompressedArchiveAccess

        /// <inheritdoc />
        public override bool IsArchive
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsCompressed
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override IEnumerable<ICompressedArchiveEntry> Entries
        {
            get { return GetArchiveEntries(); }
        }

        /// <inheritdoc />
        public override Stream OpenEntry(ICompressedArchiveEntry entry)
        {
            return OpenZipEntry(entry);
        }

        /// <inheritdoc />
        public override ICompressedArchiveEntry CreateEntry(string name)
        {
            return CreateZipEntry(name, ZipArchiveCompressionMethod.MaximumCompression);
        }

        #endregion  // ICompressedArchiveAccess

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
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

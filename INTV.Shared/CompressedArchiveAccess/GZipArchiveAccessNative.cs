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
using System.IO;
using System.IO.Compression;

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// Provides access to a GZIP-formatted stream. Multiple-entry treatment is semi-supported. Don't have your hopes up too high, though.
    /// </summary>
    internal sealed class GZipArchiveAccessNative : GZipAccess
    {
        private GZipArchiveAccessNative(Stream stream, CompressedArchiveAccessMode mode)
            : base(stream, mode)
        {
            Mode = CompressedArchiveAccessModeToCompressionMode(mode);
        }

        private CompressionMode Mode { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="GZipArchiveAccessNative"/> using the given mode.
        /// </summary>
        /// <param name="stream">Stream containing data in GZIP compressed format.</param>
        /// <param name="mode">The access mode to use for GZIP operations.</param>
        /// <returns>A new instance of <see cref="GZipArchiveAccessNative"/>.</returns>
        /// <remarks>The GZIP implementation assumes ownership of <paramref name="stream"/> and will dispose it.</remarks>
        public static GZipArchiveAccessNative Create(Stream stream, CompressedArchiveAccessMode mode)
        {
            var gzipAccess = new GZipArchiveAccessNative(stream, ValidateMode(mode));
            return gzipAccess;
        }

        /// <inheritdoc />
        protected override Stream OpenStreamForEntry(GZipMemberEntry entry)
        {
            var entryStream = new GZipStream(BaseStream, Mode, leaveOpen: true);
            return entryStream;
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
            }
            return compressionMode;
        }
    }
}

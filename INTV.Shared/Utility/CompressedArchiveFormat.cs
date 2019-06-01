// <copyright file="CompressedArchiveFormat.cs" company="INTV Funhouse">
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
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// The various archive formats that can be used. Note that not all formats are archives
    /// of multiple files (e.g. GZIP), nor are all compressed (e.g. tar).
    /// </summary>
    public enum CompressedArchiveFormat
    {
        /// <summary>
        /// Not a valid compressed archive format.
        /// </summary>
        None,

        /// <summary>
        /// The .ZIP format. See https://support.pkware.com/display/PKZIP/APPNOTE
        /// </summary>
        Zip,

        /// <summary>
        /// GNU gzip format. See https://www.gnu.org/software/gzip/
        /// </summary>
        GZip,

        /// <summary>
        /// Tape Archive format. See https://www.gnu.org/software/tar/manual/html_node/Standard.html
        /// </summary>
        Tar,

        /// <summary>
        /// The bzip2 format. See https://web.archive.org/web/20180801004107/http://www.bzip.org/
        /// </summary>
        BZip2,
    }

    /// <summary>
    /// Extension methods and helpers for <see cref="CompressedArchiveFormat"/>.
    /// </summary>
    public static class CompressedArchiveFormatExtensions
    {
        private static readonly Lazy<IDictionary<CompressedArchiveFormat, IEnumerable<string>>> CompressedArchiveFormatFileExtensions = new Lazy<IDictionary<CompressedArchiveFormat, IEnumerable<string>>>(GetCompressedArchiveFormatFileExtensions);
        private static readonly Lazy<IDictionary<CompressedArchiveFormat, IEnumerable<CompressedArchiveAccessImplementation>>> CompressedArchiveAccessImplementations = new Lazy<IDictionary<CompressedArchiveFormat, IEnumerable<CompressedArchiveAccessImplementation>>>(GetCompressedArchiveFormatImplementations);

        private static readonly IEnumerable<CompressedArchiveFormat> AvailableFormats = new[]
        {
            CompressedArchiveFormat.Zip,
        };

        /// <summary>
        /// Checks whether the given <see cref="CompressedArchiveFormat"/> is currently supported.
        /// </summary>
        /// <param name="format">The compressed archive format to check availability for.</param>
        /// <returns><c>true</c> if the archive format is supported, <c>false</c> otherwise.</returns>
        public static bool IsCompressedArchiveFormatSupported(this CompressedArchiveFormat format)
        {
            return AvailableFormats.Contains(format);
        }

        /// <summary>
        /// Gets the file extension(s) commonly used for the given compressed archive format.
        /// </summary>
        /// <param name="format">The compressed archive format whose file extensions are desired.</param>
        /// <returns>The file extensions. If an unexpected value for <paramref name="format"/> is provided, an empty enumerable is returned.</returns>
        public static IEnumerable<string> FileExtensions(this CompressedArchiveFormat format)
        {
            IEnumerable<string> fileExtensions;
            if (!format.IsCompressedArchiveFormatSupported() || !CompressedArchiveFormatFileExtensions.Value.TryGetValue(format, out fileExtensions))
            {
                fileExtensions = Enumerable.Empty<string>();
            }
            return fileExtensions;
        }

        /// <summary>
        /// Gets the available implementations of the given compressed archive format.
        /// </summary>
        /// <param name="format">The compressed archive format whose available implementations are desired.</param>
        /// <returns>The available implementations. If unsupported, an empty enumerable is returned.</returns>
        public static IEnumerable<CompressedArchiveAccessImplementation> GetAvailableCompressedArchiveImplementations(this CompressedArchiveFormat format)
        {
            IEnumerable<CompressedArchiveAccessImplementation> implementations;
            if (!format.IsCompressedArchiveFormatSupported() || !CompressedArchiveAccessImplementations.Value.TryGetValue(format, out implementations))
            {
                implementations = Enumerable.Empty<CompressedArchiveAccessImplementation>();
            }
            return implementations;
        }

        /// <summary>
        /// Gets the preferred implementation of the given compressed archive format.
        /// </summary>
        /// <param name="format">The format whose preferred implementation is desired.</param>
        /// <returns>The preferred implementation, or <see cref="CompressedArchiveAccessImplementation.None"/> if none is available.</returns>
        public static CompressedArchiveAccessImplementation GetPreferredCompressedArchiveImplementation(this CompressedArchiveFormat format)
        {
            var preferredImplementation = format.GetAvailableCompressedArchiveImplementations().FirstOrDefault();
            return preferredImplementation;
        }

        private static IDictionary<CompressedArchiveFormat, IEnumerable<string>> GetCompressedArchiveFormatFileExtensions()
        {
            var compressedArchiveFormatFileExtensions = new Dictionary<CompressedArchiveFormat, IEnumerable<string>>()
            {
                { CompressedArchiveFormat.None, Enumerable.Empty<string>() },
                { CompressedArchiveFormat.Zip, new[] { ".zip" } },
                { CompressedArchiveFormat.GZip, new[] { ".gz" } },
                { CompressedArchiveFormat.Tar, new[] { ".tar" } },
                { CompressedArchiveFormat.BZip2, new[] { ".bz2" } },
            };
            return compressedArchiveFormatFileExtensions;
        }

        private static IDictionary<CompressedArchiveFormat, IEnumerable<CompressedArchiveAccessImplementation>> GetCompressedArchiveFormatImplementations()
        {
            var compressedArchiveFormatImplementations = new Dictionary<CompressedArchiveFormat, IEnumerable<CompressedArchiveAccessImplementation>>()
            {
                { CompressedArchiveFormat.None, Enumerable.Empty<CompressedArchiveAccessImplementation>() },
                { CompressedArchiveFormat.Zip, new[] { CompressedArchiveAccessImplementation.Native, CompressedArchiveAccessImplementation.SharpZipLib } },
                { CompressedArchiveFormat.GZip, new[] { CompressedArchiveAccessImplementation.SharpZipLib } },
                { CompressedArchiveFormat.Tar, new[] { CompressedArchiveAccessImplementation.SharpZipLib } },
                { CompressedArchiveFormat.BZip2, new[] { CompressedArchiveAccessImplementation.SharpZipLib } },
            };
            return compressedArchiveFormatImplementations;
        }
    }
}

// <copyright file="CompressedArchiveAccess.CompressedArchiveFileAccess.cs" company="INTV Funhouse">
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

////#define OPEN_NESTED_FORMAT_IMMEDIATELY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

//create a FileMemo<> in CompressedArchiveFileAccess so we avoid re-creating archives all the time (problems with testing??)

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// Implementation of <see cref="CompressedArchiveFileAccess"/>.
    /// </summary>
    public abstract partial class CompressedArchiveAccess
    {
        /// <summary>
        /// Wraps a compressed archive accessor created from a file stream.
        /// </summary>
        private class CompressedArchiveFileAccess : CompressedArchiveAccess
        {
            private readonly object _lock = new object();

            /// <summary>
            /// Initializes a new instance of <see cref="CompressedArchiveFileAccess"/>.
            /// </summary>
            /// <param name="filePath">The absolute path to the compressed archive file.</param>
            /// <param name="compressedArchiveAccess">The compressed archive to wrap.</param>
            private CompressedArchiveFileAccess(string filePath, ICompressedArchiveAccess compressedArchiveAccess)
            {
                RootLocation = filePath;
                CompressedArchiveAccess = compressedArchiveAccess;
            }

            /// <inheritdoc />
            public override bool IsArchive
            {
                get { return CompressedArchiveAccess.IsArchive; }
            }

            /// <inheritdoc />
            public override bool IsCompressed
            {
                get { return CompressedArchiveAccess.IsCompressed; }
            }

            /// <inheritdoc />
            public override CompressedArchiveFormat Format
            {
                get { return CompressedArchiveAccess.Format; }
            }

            /// <inheritdoc />
            public override IEnumerable<ICompressedArchiveEntry> Entries
            {
                get { return CompressedArchiveAccess.Entries; }
            }

            private ICompressedArchiveAccess CompressedArchiveAccess { get; set; }

            private Stream Stream { get; set; }

            /// <summary>
            /// Creates an instance of <see cref="CompressedArchiveFileAccess"/>, depending on <paramref name="mode"/>.
            /// </summary>
            /// <param name="filePath">The absolute path for the compressed archive.</param>
            /// <param name="mode">The access mode to use for operations on the compressed archive.</param>
            /// <param name="implementation">If not <c>null</c>, use a specific implementation if possible. Otherwise, use default, or any.</param>
            /// <returns>An instance of <see cref="CompressedArchiveFileAccess"/> that provides access to the compressed archive located at <paramref name="filePath"/>.</returns>
            /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="filePath"/> is <c>null</c>.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the file is accessed incorrectly for the given <paramref name="mode"/>.</exception>
            /// <exception cref="System.ArgumentException">Thrown if <paramref name="filePath"/> is empty or white space, contains invalid characters, or is otherwise invalid</exception>
            /// <exception cref="System.NotSupportedException">Thrown if <paramref name="filePath"/> refers to a non-file device, e.g. a COM port, et. al.</exception>
            /// <exception cref="System.IO.FileNotFoundException">Thrown if <paramref name="filePath"/> cannot be found.</exception>
            /// <exception cref="System.IO.IOException">Thrown if an I/O error occurs.</exception>
            /// <exception cref="System.Security.SecurityException">Thrown if the requested action on <paramref name="filePath"/> cannot be performed, e.g. no read / create access is granted, et. al.</exception>
            /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the directory cannot be found, e.g. an unavailable network drive forms part of the file path.</exception>
            /// <exception cref="System.UnauthorizedAccessException">Thrown if access is denied, e.g. read/write access requested for a read-only file or directory.</exception>
            /// <exception cref="System.IO.PathTooLongException">Thrown if <paramref name="filePath"/> is too long.</exception>
            /// <exception cref="System.InvalidOperationException">Thrown if it is not possible to create an instance of <see cref="CompressedArchiveFileAccess"/> from the file at <paramref name="filePath"/>,
            /// despite the file appearing to be valid.</exception>
            public static CompressedArchiveFileAccess Create(string filePath, CompressedArchiveAccessMode mode, CompressedArchiveAccessImplementation? implementation)
            {
                var fileMode = CompressedArchiveAccessModeToFileMode(mode);
                var fileAccess = CompressedArchiveAccessModeToFileAccess(mode);

                var successfullyAccessedFormats = new List<CompressedArchiveFormat>();
                var fileStream = new FileStream(filePath, fileMode, fileAccess);
                var formats = filePath.GetCompressedArchiveFormatsFromFileName();
                ICompressedArchiveAccess compressedArchiveAccess = null;

#if OPEN_NESTED_FORMAT_IMMEDIATELY
                Stream stream = fileStream;
                foreach (var format in formats)
                {
                    compressedArchiveAccess = Utility.CompressedArchiveAccess.Open(stream, format, mode);
                    if (compressedArchiveAccess != null)
                    {
                        successfullyAccessedFormats.Add(format);
                        if (!compressedArchiveAccess.IsArchive)
                        {
                            var fileName = Path.GetFileName(filePath);
                            fileName = Path.GetFileNameWithoutExtension(fileName);
                            var entry = compressedArchiveAccess.FindEntry(fileName);
                            stream = entry == null ? null : compressedArchiveAccess.OpenEntry(entry);
                            if (stream == null)
                            {
                                compressedArchiveAccess = null;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
#else
                var format = formats.FirstOrDefault();
                if (format != CompressedArchiveFormat.None)
                {
                    compressedArchiveAccess = Shared.CompressedArchiveAccess.CompressedArchiveAccess.Open(fileStream, format, mode, implementation);
                }
#endif

                if (compressedArchiveAccess == null)
                {
                    var identifiedFormats = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, formats);
                    var successfullyCreatedFormats = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, successfullyAccessedFormats);
                    var failedFormats = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, formats.Except(successfullyAccessedFormats));
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_UnableToProcessError_Format, filePath, identifiedFormats, successfullyCreatedFormats, failedFormats);
                    throw new InvalidOperationException(errorMessage);
                }

                var compressedArchiveFileAccess = new CompressedArchiveFileAccess(filePath, compressedArchiveAccess) { Stream = fileStream };
                return compressedArchiveFileAccess;
            }

            /// <inheritdoc />
            public override Stream OpenEntry(ICompressedArchiveEntry entry)
            {
                return CompressedArchiveAccess.OpenEntry(entry);
            }

            /// <inheritdoc />
            public override ICompressedArchiveEntry CreateEntry(string name)
            {
                return CompressedArchiveAccess.CreateEntry(name);
            }

            /// <inheritdoc />
            protected override bool DeleteEntry(ICompressedArchiveEntry entry)
            {
                return CompressedArchiveAccess.DeleteEntry(entry.Name);
            }

            /// <inheritdoc />
            protected override void DisposeCore(bool disposing)
            {
                lock (_lock)
                {
                    if (CompressedArchiveAccess != null)
                    {
                        CompressedArchiveAccess.Dispose();
                        CompressedArchiveAccess = null;
                    }
                    if (Stream != null)
                    {
                        Stream.Dispose();
                        Stream = null;
                    }
                }
            }
        }
    }
}

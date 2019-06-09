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
using System.Globalization;
using System.IO;
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
        private static readonly Lazy<IDictionary<CompressedArchiveFormat, List<string>>> CompressedArchiveFormatFileExtensions = new Lazy<IDictionary<CompressedArchiveFormat, List<string>>>(InitializeCompressedArchiveFormatFileExtensions);
        private static readonly Lazy<IDictionary<CompressedArchiveFormat, IList<CompressedArchiveAccessImplementation>>> CompressedArchiveAccessImplementations = new Lazy<IDictionary<CompressedArchiveFormat, IList<CompressedArchiveAccessImplementation>>>(InitializeCompressedArchiveFormatImplementations);

        private static readonly HashSet<CompressedArchiveFormat> AvailableFormats = new HashSet<CompressedArchiveFormat>()
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
            var fileExtensions = Enumerable.Empty<string>();
            List<string> registeredFileExtensions;
            if (format.IsCompressedArchiveFormatSupported() && CompressedArchiveFormatFileExtensions.Value.TryGetValue(format, out registeredFileExtensions))
            {
                fileExtensions = registeredFileExtensions;
            }
            return fileExtensions;
        }

        /// <summary>
        /// Gets the compressions and archive formats based entirely on file extension(s) of the given file.
        /// </summary>
        /// <param name="fileName">The file path to check.</param>
        /// <returns>The formats presumed to be in use by the file with the given name.</returns>
        public static IEnumerable<CompressedArchiveFormat> GetCompressedArchiveFormatsFromFileName(this string fileName)
        {
            var formats = new List<CompressedArchiveFormat>();
            var fileExtension = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            while (!string.IsNullOrEmpty(fileExtension))
            {
                var format = fileExtension.GetCompressedArchiveFormatFromFileExtension();
                if (format == CompressedArchiveFormat.None)
                {
                    break;
                }
                formats.Add(format);
                fileExtension = Path.GetExtension(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }
            return formats;
        }

        /// <summary>
        /// Gets the compression / archive format based solely on the given file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension to check.</param>
        /// <returns>The compression / archive format, based on file extension. If no match is found, <see cref="CompressedArchiveFormat.None"/> is returned.</returns>
        public static CompressedArchiveFormat GetCompressedArchiveFormatFromFileExtension(this string fileExtension)
        {
            var format = CompressedArchiveFormat.None;
            if (fileExtension.FirstOrDefault() == '.')
            {
                var compressedArchiveFormatFileExtensions = CompressedArchiveFormatFileExtensions.Value;
                format = compressedArchiveFormatFileExtensions.FirstOrDefault(f => f.Value.FirstOrDefault(e => StringComparer.OrdinalIgnoreCase.Compare(e, fileExtension) == 0) != null).Key;
            }
            return format;
        }

        /// <summary>
        /// Gets the available implementations of the given compressed archive format.
        /// </summary>
        /// <param name="format">The compressed archive format whose available implementations are desired.</param>
        /// <returns>The available implementations. If unsupported, an empty enumerable is returned.</returns>
        public static IEnumerable<CompressedArchiveAccessImplementation> GetAvailableCompressedArchiveImplementations(this CompressedArchiveFormat format)
        {
            var implementations = Enumerable.Empty<CompressedArchiveAccessImplementation>();
            IList<CompressedArchiveAccessImplementation> registeredImplementations;
            if (format.IsCompressedArchiveFormatSupported() && CompressedArchiveAccessImplementations.Value.TryGetValue(format, out registeredImplementations))
            {
                implementations = registeredImplementations;
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

        /// <summary>
        /// Associates a file extension with a previously registered <see cref="CompressedArchiveFormat"/>.
        /// </summary>
        /// <param name="format">The compressed archive format with which to associate a file extension.</param>
        /// <param name="fileExtension">The file extension to associate with <paramref name="format"/>.</param>
        /// <param name="makeDefault">If <c>true</c>, treat <paramref name="fileExtension"/> as the default, meaning the first in the enumerable of values returned from <see cref="FileExtensions(CompressedArchiveFormat)"/>.</param>
        /// <returns><c>true</c> if the file extension was added to those associated with <paramref name="format"/>, <c>false</c> otherwise.</returns>
        /// <remarks>Note that if a particular file extension is not already the default, you can make it so by calling this method and setting <paramref name="makeDefault"/>
        /// to <c>true</c>. In this usage, the function will always return <c>false</c> if <paramref name="fileExtension"/> is already associated with <paramref name="format"/>.
        /// Note also that file extensions are not case sensitive.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="format"/> is <see cref="CompressedArchiveFormat.None"/>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="fileExtension"/> is null, empty, whitespace, contains invalid characters,
        /// does not begin with a period, or contains multiple periods. This exception is also thrown if <paramref name="format"/> has not already
        /// been registered as an available format, either as a format automatically included in the implementation, or registered via
        /// <see cref="RegisterCompressedArchiveFormat(CompressedArchiveFormat, IEnumerable{string}, IEnumerable{CompressedArchiveAccessImplementation})"/>.
        /// This exception is also thrown if <paramref name="fileExtension"/> is already in use by a previously registered <paramref name="format"/>.</exception>
        public static bool AddFileExtension(this CompressedArchiveFormat format, string fileExtension, bool makeDefault)
        {
            if (format == CompressedArchiveFormat.None)
            {
                throw new ArgumentOutOfRangeException("format");
            }
            if (string.IsNullOrWhiteSpace(fileExtension) || (fileExtension.First() != '.') || Path.GetInvalidFileNameChars().Intersect(fileExtension).Any())
            {
                throw new ArgumentException(Resources.Strings.CompressedArchiveFormat_InvalidFileExtensionError, "fileExtension");
            }
            if (fileExtension.Count(c => c == '.') > 1)
            {
                throw new ArgumentException(Resources.Strings.CompressedArchiveFormat_CompountFileExtensionsNotSupportedError, "fileExtension");
            }

            List<string> existingFileExtensionsForFormat;
            var formatAlreadyRegistered = CompressedArchiveFormatFileExtensions.Value.TryGetValue(format, out existingFileExtensionsForFormat);
            var formatAlreadyUsingExtension = CompressedArchiveFormatFileExtensions.Value.FirstOrDefault(e => e.Value.Contains(fileExtension, StringComparer.OrdinalIgnoreCase));

            bool? added = null;
            if (formatAlreadyRegistered)
            {
                if ((formatAlreadyUsingExtension.Key == CompressedArchiveFormat.None) || (formatAlreadyUsingExtension.Key == format))
                {
                    added = AddOrUpdateCompressedArchiveFormatData(fileExtension, makeDefault, existingFileExtensionsForFormat, (f, e) => ((List<string>)e).FindIndex(0, x => StringComparer.OrdinalIgnoreCase.Compare(x, f) == 0));
                }
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveFormat_FormatIsNotRegisteredError_Format, format);
                throw new ArgumentException(message, "format");
            }
            if (!added.HasValue)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveFomrat_FileExtensionAlreadyInUseError_Format, fileExtension, formatAlreadyUsingExtension.Key);
                throw new ArgumentException(message, "fileExtension");
            }
            return added.Value;
        }

        /// <summary>
        ///  Associates an implementation with a previously registered <see cref="CompressedArchiveFormat"/>.
        /// </summary>
        /// <param name="format">The compressed archive format with which to associate an implementation.</param>
        /// <param name="implementation">The implementation to associate with the compressed archive format</param>
        /// <param name="makePreferred">If <c>true</c>, <paramref name="implemenatation"/> becomes the implementation returned by <see cref="GetPreferredCompressedArchiveImplementation(CompressedArchiveFormat)"/>.</param>
        /// <returns><c>true</c> if <paramref name="implementation"/> was added, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="format"/> is <see cref="CompressedArchiveFormat.None"/>. Also thrown if the
        /// value of <paramref name="implementation"/> is <see cref="CompressedArchiveAccessImplementation.Any"/> or <see cref="CompressedArchiveAccessImplementation.None"/>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="format"/>has not already been registered as an available format,
        /// either as a format automatically included in the implementation, or registered via
        /// <see cref="RegisterCompressedArchiveFormat(CompressedArchiveFormat, IEnumerable{string}, IEnumerable{CompressedArchiveAccessImplementation})"/>.</exception>
        public static bool AddImplementation(this CompressedArchiveFormat format, CompressedArchiveAccessImplementation implementation, bool makePreferred)
        {
            if (format == CompressedArchiveFormat.None)
            {
                throw new ArgumentOutOfRangeException("format");
            }
            if ((implementation == CompressedArchiveAccessImplementation.None) || (implementation == CompressedArchiveAccessImplementation.Any))
            {
                throw new ArgumentOutOfRangeException("implementation");
            }

            IList<CompressedArchiveAccessImplementation> existingImplementationsForFormat;
            var formatAlreadyRegistered = CompressedArchiveAccessImplementations.Value.TryGetValue(format, out existingImplementationsForFormat);

            var added = false;
            if (formatAlreadyRegistered)
            {
                added = AddOrUpdateCompressedArchiveFormatData(implementation, makePreferred, existingImplementationsForFormat, (i, e) => e.IndexOf(i));
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveFormat_FormatIsNotRegisteredError_Format, format);
                throw new ArgumentException(message, "format");
            }
            return added;
        }

        /// <summary>
        /// Registers a <see cref="CompressedArchiveFormat"/> for use.
        /// </summary>
        /// <param name="format">The format to register.</param>
        /// <param name="fileExtensions">The file extensions to associate with <paramref name="format"/>.</param>
        /// <param name="implementations">The implementations to associate with <paramref name="format"/>.</param>
        /// <returns><c>true</c> if <paramref name="format"/> was newly registered, <c>false</c> otherwise.</returns>
        /// <remarks>If this method is called multiple times with the same <paramref name="format"/> and other arguments, no net effect results. If it is
        /// called with values in the <paramref name="fileExtensions"/> and <paramref name="implementations"/> arguments that are not already associated
        /// with <paramref name="format"/>, they will be added, subject to the restrictions documented in the <see cref="AddFileExtension(CompressedArchiveFormat, string, bool)"/>
        /// and <see cref="AddImplementation(CompressedArchiveFormat, CompressedArchiveAccessImplementation, bool)"/> methods respectively. Note that in this
        /// usage, the default / preferred file extension and implementation will not be changed.</remarks>
        public static bool RegisterCompressedArchiveFormat(this CompressedArchiveFormat format, IEnumerable<string> fileExtensions, IEnumerable<CompressedArchiveAccessImplementation> implementations)
        {
            if (format == CompressedArchiveFormat.None)
            {
                throw new ArgumentOutOfRangeException("format");
            }
            if (fileExtensions == null)
            {
                throw new ArgumentNullException("fileExtensions");
            }
            if (!fileExtensions.Any())
            {
                throw new ArgumentException(Resources.Strings.CompressedArchiveFormat_FileExtensionRequiredError, "fileExtensions");
            }
            if (implementations == null)
            {
                throw new ArgumentNullException("implementations");
            }
            if (!implementations.Any())
            {
                throw new ArgumentException(Resources.Strings.CompressedArchiveFormat_ImplementationRequired, "implementations");
            }

            if (!CompressedArchiveFormatFileExtensions.Value.Keys.Contains(format))
            {
                CompressedArchiveFormatFileExtensions.Value[format] = new List<string>();
            }
            foreach (var fileExtension in fileExtensions)
            {
                format.AddFileExtension(fileExtension, makeDefault: false);
            }

            if (!CompressedArchiveAccessImplementations.Value.Keys.Contains(format))
            {
                CompressedArchiveAccessImplementations.Value[format] = new List<CompressedArchiveAccessImplementation>();
            }
            foreach (var implementation in implementations)
            {
                format.AddImplementation(implementation, makePreferred: false);
            }

            var registered = AvailableFormats.Add(format);

            return registered;
        }

        private static bool AddOrUpdateCompressedArchiveFormatData<T>(T data, bool makeDefault, IList<T> existingData, Func<T, IList<T>, int> find)
        {
            var index = find(data, existingData);
            var added = index < 0;

            if (makeDefault)
            {
                if (index > 0)
                {
                    existingData.RemoveAt(index);
                    existingData.Insert(0, data);
                }
                else if (added)
                {
                    if (existingData.Count > 0)
                    {
                        existingData.Insert(0, data);
                    }
                }
            }
            else if (added)
            {
                existingData.Add(data);
            }

            return added;
        }

        private static IDictionary<CompressedArchiveFormat, List<string>> InitializeCompressedArchiveFormatFileExtensions()
        {
            var compressedArchiveFormatFileExtensions = new Dictionary<CompressedArchiveFormat, List<string>>()
            {
                { CompressedArchiveFormat.None, new List<string>() },
                { CompressedArchiveFormat.Zip, new List<string>() { ".zip" } },
                { CompressedArchiveFormat.GZip, new List<string>() { ".gz" } },
                { CompressedArchiveFormat.Tar, new List<string>() { ".tar" } },
                { CompressedArchiveFormat.BZip2, new List<string>() { ".bz2" } },
            };
            return compressedArchiveFormatFileExtensions;
        }

        private static IDictionary<CompressedArchiveFormat, IList<CompressedArchiveAccessImplementation>> InitializeCompressedArchiveFormatImplementations()
        {
            var compressedArchiveFormatImplementations = new Dictionary<CompressedArchiveFormat, IList<CompressedArchiveAccessImplementation>>()
            {
                { CompressedArchiveFormat.None, new List<CompressedArchiveAccessImplementation>() },
                { CompressedArchiveFormat.Zip, new[] { CompressedArchiveAccessImplementation.Native, CompressedArchiveAccessImplementation.SharpZipLib } },
                { CompressedArchiveFormat.GZip, new[] { CompressedArchiveAccessImplementation.SharpZipLib } },
                { CompressedArchiveFormat.Tar, new[] { CompressedArchiveAccessImplementation.SharpZipLib } },
                { CompressedArchiveFormat.BZip2, new[] { CompressedArchiveAccessImplementation.SharpZipLib } },
            };
            return compressedArchiveFormatImplementations;
        }
    }
}

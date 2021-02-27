// <copyright file="PathUtils.Mono.cs" company="INTV Funhouse">
// Copyright (c) 2021 All Rights Reserved
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    public static partial class PathUtils
    {
        /// <summary>
        /// Enumerates the files in the given directory matching the provided file extension.
        /// </summary>
        /// <param name="directory">The directory in which to enumerate files.</param>
        /// <param name="fileExtension">The file extension to match.</param>
        /// <returns>The files in <paramref name="directory"/> whose file names end with <paramref name="fileExtension"/>.</returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="directory"/> is a zero-length string,
        /// contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars(),
        /// or if <paramref name="fileExtension"/> does not contain a valid file extension.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="directory"/> or <paramref name="fileExtension"/> is <c>null</c>.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if <paramref name="directory"/> is invalid, such as referring to an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException">Thrown if <paramref name="directory"/> is a file name.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown if <paramref name="directory"/> combined with a resulting matching file name
        /// specifies a path whose length exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than
        ///  248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">Thrown if the caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
        /// <remarks>The Mono implementation of <see cref="Diectory.EnumerateFiles(string, string, SearchOption)"/> does not appear to work correctly.
        /// This implementation works around the issue in a case-insensitive manner.</remarks>
        public static IEnumerable<string> EnumerateFilesWithPattern(this string directory, string fileExtension)
        {
            var files = Directory.EnumerateFiles(directory);
            var matches = files.Where(f => f.EndsWith(fileExtension, PathComparer.DefaultPolicy)).ToList();
            return matches;
        }
    }
}

// <copyright file="StorageLocationExtensions.Mono.cs" company="INTV Funhouse">
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
    /// Mono-specific implementation.
    /// </summary>
    public static partial class StorageLocationExtensions
    {
        /// <summary>
        /// Enumerates files with the given extension. Necessary workaround for case-sensitivity in Mono.
        /// </summary>
        /// <param name="directory">The directory to enumerate.</param>
        /// <param name="fileExtensionMatch">The file extension to match. All files match if empty.</param>
        /// <returns>The files in the given directory.</returns>
        private static IEnumerable<string> OSEnumerateFiles(string directory, string fileExtensionMatch)
        {
            // Workaround for non-Windows. Don't care about file system case sensitivity.
            var files = Directory.EnumerateFiles(directory);
            if (!string.IsNullOrEmpty(fileExtensionMatch))
            {
                files = files.Where(f => f.EndsWith(fileExtensionMatch, StringComparison.InvariantCultureIgnoreCase));
            }
            return files;
        }
    }
}

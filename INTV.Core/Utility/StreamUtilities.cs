// <copyright file="StreamUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Utility functions for working with the Stream type.
    /// </summary>
    public static class StreamUtilities
    {
        private static IStorageAccess _storageAccess;

        /// <summary>
        /// If an application wishes to access data in some storage, e.g. a file system, it must register this interface.
        /// </summary>
        /// <param name="storageAccess">The storage access interface.</param>
        /// <returns><c>true</c> if initialization was successful.</returns>
        public static bool Initialize(IStorageAccess storageAccess)
        {
            _storageAccess = storageAccess;
            return true;
        }

        /// <summary>
        /// Opens a Stream using an absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <returns>A Stream for accessing the contents of the file.</returns>
        /// <remarks>Requires a valid function to have been registered via the Initialize method.</remarks>
        public static Stream OpenFileStream(this string filePath)
        {
            return _storageAccess.Open(filePath);
        }

        /// <summary>
        /// Verifies that a file exists at the given absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <returns><c>true</c> if the file exists at the given path.</returns>
        public static bool FileExists(this string filePath)
        {
            return _storageAccess.Exists(filePath);
        }

        /// <summary>
        /// Gets the size, in bytes, of the file at the given absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <returns>Length of the file, in bytes.</returns>
        public static long Size(this string filePath)
        {
            return _storageAccess.Size(filePath);
        }

        /// <summary>
        /// Gets the last modification time of the file at the given path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <returns>Last modification time of the file, in UTC.</returns>
        public static DateTime LastFileWriteTimeUtc(this string filePath)
        {
            return _storageAccess.LastWriteTimeUtc(filePath);
        }
    }
}

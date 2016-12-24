// <copyright file="StreamUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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
        private static Func<string, System.IO.Stream> _openFileStreamFunc;
        private static Func<string, bool> _fileExistsFunc;

        /// <summary>
        /// If the application wishes to access files via their Stream API, it must register the proper mechanism to do so.
        /// </summary>
        /// <param name="openFileStreamFunc">The function to call to open a file stream.</param>
        /// <param name="fileExistsFunc">The function to call to verify that the given file exists.</param>
        /// <returns><c>true</c> if initialization was successful.</returns>
        public static bool Initialize(Func<string, System.IO.Stream> openFileStreamFunc, Func<string, bool> fileExistsFunc)
        {
            _openFileStreamFunc = openFileStreamFunc;
            _fileExistsFunc = fileExistsFunc;
            return true;
        }

        /// <summary>
        /// Opens a Stream using an absolute file path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <returns>A Stream for accessing the contents of the file.</returns>
        /// <remarks>Requires a valid function to have been registered via the Initialize method.</remarks>
        public static Stream OpenFileStream(this string filePath)
        {
            return _openFileStreamFunc(filePath);
        }

        /// <summary>
        /// Verifies that a file exists at the given absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <returns><c>true</c> if the file exists at the given path.</returns>
        public static bool FileExists(this string filePath)
        {
            return _fileExistsFunc(filePath);
        }
    }
}

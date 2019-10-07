// <copyright file="TemporaryDirectory.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Helper class to act as a temporary directory that deletes itself when disposed.
    /// </summary>
    public sealed class TemporaryDirectory : IDisposable
    {
        /// <summary>
        /// Initialize a new instance of <see cref="TemporaryDirectory"/>.
        /// </summary>
        public TemporaryDirectory()
            : this(GenerateUniqueDirectoryPath())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TemporaryDirectory"/> with a specified path.
        /// </summary>
        /// <param name="temporaryDirectoryPath">The absolute path to use for the temporary directory.</param>
        public TemporaryDirectory(string temporaryDirectoryPath)
        {
            Path = temporaryDirectoryPath;
            Directory.CreateDirectory(Path);
        }

        ~TemporaryDirectory()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the absolute path to use for the temporary directory.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Generates a unique directory path.
        /// </summary>
        /// <returns>A unique directory path.</returns>
        public static string GenerateUniqueDirectoryPath()
        {
            var directoryPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "VINT_" + Guid.NewGuid());
            return directoryPath;
        }

        #region IDispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // The catch is a CYA that shouldn't be triggered. Rest is covered.
        private void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                if (Path.StartsWith(System.IO.Path.GetTempPath(), PathComparer.DefaultPolicy))
                {
                    try
                    {
                        if (Directory.Exists(Path))
                        {
                            Directory.Delete(Path, recursive: true);
                        }
                    }
                    catch
                    {
                    }
                }
                Path = null;
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        #endregion // IDispose
    }
}

// <copyright file="TemporaryFile.cs" company="INTV Funhouse">
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

namespace INTV.TestHelpers.Shared.Utility
{
    /// <summary>
    /// Helper class to act as a temporary file that deletes itself.
    /// </summary>
    public sealed class TemporaryFile : IDisposable
    {
        /// <summary>
        /// Initialize a new instance of <paramref name="TemporaryFile"/>.
        /// </summary>
        /// <param name="fileExtension">The file extension for the file name.</param>
        /// <param name="createFile">If <c>true</c>, create the file on disk; otherwise only create the path initially.</param>
        public TemporaryFile(string fileExtension, bool createFile)
        {
            FilePath = GenerateUniqueFilePath("INTV_Test_TempFile_", fileExtension);
            if (createFile)
            {
                CreateTempFileOnDisk(FilePath);
            }
        }

        ~TemporaryFile()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the absolute path to use for the temporary file.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Generates a unique file path using a prefix and file extension.
        /// </summary>
        /// <param name="fileNamePrefix">The prefix for the temporary file's name.</param>
        /// <param name="fileExtension">The file extension to use.</param>
        /// <returns>A unique file name.</returns>
        public static string GenerateUniqueFilePath(string fileNamePrefix, string fileExtension)
        {
            var filePath = Path.Combine(Path.GetTempPath(), fileNamePrefix + Guid.NewGuid() + fileExtension);
            return filePath;
        }

        /// <summary>
        /// Create an instance of <see cref="TemporaryFile"/> with a predefined path.
        /// </summary>
        /// <param name="path">The absolute path for the file.</param>
        /// <param name="createEmptyFile">If <c>true</c>, create the file on disk; otherwise only create the path initially.</param>
        /// <returns>The temporary file instance.</returns>
        public static TemporaryFile CreateTemporaryFileWithPath(string path, bool createEmptyFile)
        {
            var temporaryFile = new TemporaryFile(string.Empty, createFile: false) { FilePath = path };
            if (createEmptyFile)
            {
                CreateTempFileOnDisk(path);
            }
            return temporaryFile;
        }

        #region IDispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        private static void CreateTempFileOnDisk(string filePath)
        {
            using (var tmp = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                tmp.Flush();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                try
                {
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                }
                catch
                {
                }
                FilePath = null;
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        #endregion // IDispose
    }
}

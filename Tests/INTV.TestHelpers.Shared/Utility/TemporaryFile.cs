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
            CreatedFile = createFile;
            FilePath = Path.Combine(Path.GetTempPath(), "TestCompressedArchive_" + Guid.NewGuid() + fileExtension);
            if (createFile)
            {
                using (var tmp = new FileStream(FilePath, FileMode.OpenOrCreate))
                {
                    tmp.Flush();
                }
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

        private bool CreatedFile { get; set; }

        #region IDispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
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

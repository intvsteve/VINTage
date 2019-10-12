// <copyright file="CompressedArchive.EntryMemo.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using INTV.Core.Utility;
using INTV.Shared.Utility;

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// Implementation of EntryMemo.
    /// </summary>
    public abstract partial class CompressedArchive
    {
        private static string GetHashedName(string name)
        {
            var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name));
            var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);
            var baseName = Path.GetFileNameWithoutExtension(name);
            var extension = Path.GetExtension(name);
            name = string.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", baseName, hashString, extension);
            return name;
        }

        /// <summary>
        /// This memo implementation is used to extract an archive's entries to a temporary location to simplify accessing entries.
        /// </summary>
        /// <remarks><para>The <see cref="IStorageAccess"/> interface implementation aspect of the <see cref="CompressedArchive"/>
        /// class cannot assume that when <see cref="IStorageAccess.Open(string)"/> is called that the caller is aware
        /// of some of the special behaviors of accessing entries within an archive, such as the inability to seek
        /// to arbitrary locations within the entry's data stream.</para>
        /// <para>In fact, one of the primary usages of <see cref="IStorageAccess.Open(string)"/> is for ROM identification,
        /// which frequently seeks around an entry's data stream. This memo aims to deterministically extract the requested
        /// entries to a temporary location for subsequent access. The determinism is build around how the temporary locations
        /// are determined. Rather than using a randomly generated temporary location, a hash of the absolute path used to access
        /// the archive or entries within it is used, such that as long as the archive remains in use, accessing the same
        /// entry will simply become a normal file system access, avoiding repeated overhead of deflating an entry, for example.</para></remarks>
        private class EntryMemo : FileMemo<string>
        {
            /// <summary>
            /// Initialize a new instance of the <see cref="EntryMemo"/>.
            /// </summary>
            /// <param name="archivePath">The absolute path to the archive.</param>
            public EntryMemo(string archivePath)
            {
                var tempDirName = GetHashedName(archivePath);
                var tempDirPath = GetTemporaryDirectoryPath(tempDirName);
                _tempDir = new Lazy<TemporaryDirectory>(() => new TemporaryDirectory(tempDirPath));
            }

            /// <inheritdoc />
            protected override string DefaultMemoValue
            {
                get { return null; }
            }

            private TemporaryDirectory TempDir
            {
                get { return _tempDir.Value; }
            }
            private Lazy<TemporaryDirectory> _tempDir;

            /// <summary>
            /// Gets the absolute path to the file this memo represents.
            /// </summary>
            /// <param name="memo">The memo.</param>
            /// <returns>The absolute path to the archive entry that has been extracted and is tracked via this memo.</returns>
            internal string GetMemoPath(string memo)
            {
                var memoPath = Path.Combine(TempDir.Path, memo);
                return memoPath;
            }

            /// <inheritdoc />
            protected override string GetMemo(StorageLocation location, object data)
            {
                var memoData = data as EntryMemoData;
                var memo = GetHashedName(location.Path);
                var memoPath = GetMemoPath(memo);
                if (!File.Exists(memoPath))
                {
                    memoData.Archive.ExtractEntryFromStream(memoData.Entry, memoData.ResourceStream, memoPath, overwrite: false);
                }
                return memo;
            }

            /// <inheritdoc />
            protected override bool IsValidMemo(string memo)
            {
                var isValid = !string.IsNullOrEmpty(memo);
                return isValid;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            private static string GetTemporaryDirectoryPath(string archiveName)
            {
                // During tests, RomListConfiguration may not be initialized. If so, just
                // use a normal temporary directory.
                var rootTempDirPath = string.Empty;
                if (Model.RomListConfiguration.Instance == null)
                {
                    rootTempDirPath = TemporaryDirectory.GenerateUniqueDirectoryPath();
                }
                else
                {
                    rootTempDirPath = Model.RomListConfiguration.Instance.TemporaryFilesDirectory;
                }
                var tempDirPath = Path.Combine(rootTempDirPath, archiveName);
                return tempDirPath;
            }
        }

        private class EntryMemoData
        {
            /// <summary>
            /// Initialize the instance of memo creation data.
            /// </summary>
            /// <param name="archive">The archive used for extraction.</param>
            /// <param name="entry">The entry to extract.</param>
            /// <param name="resourceStream">The resource stream of the entry.</param>
            public EntryMemoData(CompressedArchive archive, ICompressedArchiveEntry entry, Stream resourceStream)
            {
                Archive = archive;
                Entry = entry;
                ResourceStream = resourceStream;
            }

            /// <summary>
            /// Gets the compressed archive.
            /// </summary>
            public CompressedArchive Archive { get; private set; }

            /// <summary>
            /// Gets the entry.
            /// </summary>
            public ICompressedArchiveEntry Entry { get; private set; }

            /// <summary>
            /// Gets the resource stream.
            /// </summary>
            public Stream ResourceStream { get; private set; }
        }
    }
}

// <copyright file="TarAccessSharpZipLib.cs" company="INTV Funhouse">
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
using System.Threading;
using ICSharpCode.SharpZipLib.Tar;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Provides access to TAR archives using the SharpZipLib implementation.
    /// </summary>
    /// <remarks>The SharpZipLib library does not have strong support for pure in-memory manipulation
    /// of TAR data, nor does it seem to support appending new entries existing ones. Therefore, usage
    /// should focus on creating and reading TAR files as on-disk entities. More specifically, read-only
    /// operations on TAR in-memory-only streams appears to work, while write operations only seem to work
    /// when using actual disk-based file streams. A glimpse into the implementation in SharpZipLib reveals
    /// numerous places that assume to be dealing with file-based streams.</remarks>
    internal sealed class TarAccessSharpZipLib : CompressedArchiveAccess
    {
        /// <summary>
        /// Initialize a new instance of the type from the given stream.
        /// </summary>
        /// <param name="stream">Stream containing data in TAR format.</param>
        /// <param name="mode">The access mode to use for TAR operations.</param>
        private TarAccessSharpZipLib(Stream stream, CompressedArchiveAccessMode mode)
        {
            IsReadOnly = mode == CompressedArchiveAccessMode.Read;
            _entries = ListTarEntries(stream);
            switch (mode)
            {
                case CompressedArchiveAccessMode.Create:
                    TarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateOutputTarArchive(stream);
                    TarArchive.SetKeepOldFiles(keepExistingFiles: false);
                    var fileStream = stream as FileStream;
                    if (fileStream != null)
                    {
                        RootLocation = fileStream.Name;
                    }
                    break;
                default:
                    TarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(stream);
                    break;
            }
            TarArchive.IsStreamOwner = true;
        }

        /// <inheritdoc />
        public override bool IsArchive
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsCompressed
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override CompressedArchiveFormat Format
        {
            get { return CompressedArchiveFormat.Tar; }
        }

        /// <inheritdoc />
        public override IEnumerable<ICompressedArchiveEntry> Entries
        {
            get { return _entries; }
        }
        private List<TarArchiveEntry> _entries;

        private bool IsReadOnly { get; set; }

        private TarArchive TarArchive { get; set; }

        private TemporaryDirectory ExtractLocation { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="TarAccessSharpZipLib"/> using the given mode.
        /// </summary>
        /// <param name="stream">Stream containing data in TAR format.</param>
        /// <param name="mode">The access mode to use for TAR operations.</param>
        /// <returns>A new instance of <see cref="TarAccessSharpZipLib"/>.</returns>
        /// <remarks>The TAR implementation assumes ownership of <paramref name="stream"/> and will dispose it.</remarks>
        public static TarAccessSharpZipLib Create(Stream stream, CompressedArchiveAccessMode mode)
        {
            var tarAccess = new TarAccessSharpZipLib(stream, ValidateMode(mode));
            return tarAccess;
        }

        /// <inheritdoc />
        public override Stream OpenEntry(ICompressedArchiveEntry entry)
        {
            Stream stream = null;
            var tarArchiveEntry = _entries.FirstOrDefault(e => e.Name == entry.Name);
            if (tarArchiveEntry != null)
            {
                if (IsReadOnly)
                {
                    lock (TarArchive)
                    {
                        if (ExtractLocation == null)
                        {
                            ExtractLocation = new TemporaryDirectory();
                        }
                    }
                    TarArchive.ExtractContents(ExtractLocation.Path);
                    var filePath = Path.Combine(ExtractLocation.Path, tarArchiveEntry.Name);
                    stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                else
                {
                    stream = tarArchiveEntry.OpenOutputStream(this);
                }
            }
            return stream;
        }

        /// <inheritdoc />
        public override ICompressedArchiveEntry CreateEntry(string name)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(Resources.Strings.TarArchiveAccess_InvalidModeForCreateEntryError);
            }
            var root = TarArchive.RootPath;
            var isRooted = Path.IsPathRooted(name);
            var tarEntry = isRooted ? TarEntry.CreateEntryFromFile(name) : TarEntry.CreateTarEntry(name);
            if (isRooted)
            {
                if (!string.IsNullOrEmpty(RootLocation))
                {
                    var relativeName = PathUtils.GetRelativePath(tarEntry.File, Path.GetDirectoryName(RootLocation));
                    tarEntry.Name = relativeName;
                }
            }
            var newEntry = new TarArchiveEntry(tarEntry);
            _entries.Add(newEntry);
            return newEntry;
        }

        /// <summary>
        /// Validates the provided mode.
        /// </summary>
        /// <param name="mode">The mode to validate.</param>
        /// <returns>The value of <paramref name="mode"/> if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported value for <paramref name="mode"/> is provided.</exception>
        private static CompressedArchiveAccessMode ValidateMode(CompressedArchiveAccessMode mode)
        {
            switch (mode)
            {
                case CompressedArchiveAccessMode.Read:
                    break;
                case CompressedArchiveAccessMode.Create:
                    break;
                case CompressedArchiveAccessMode.Update:
                    throw new InvalidOperationException(Resources.Strings.TarArchiveAccess_InvalidMode);
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, Resources.Strings.TarArchiveAccess_InvalidMode);
            }
            return mode;
        }

        /// <inheritdoc />
        protected override bool DeleteEntry(ICompressedArchiveEntry entry)
        {
            throw new NotSupportedException(Resources.Strings.CompressedArchiveAccess_TarDeleteEntryNotSupported);
        }

        /// <inheritdoc />
        protected override void DisposeCore(bool disposing)
        {
            if (TarArchive != null)
            {
                IDisposable tarArchive = TarArchive;
                TarArchive = null;
                if (tarArchive != null)
                {
                    tarArchive.Dispose();
                }
            }
            if (ExtractLocation != null)
            {
                var extractLocation = ExtractLocation;
                ExtractLocation = null;
                if (extractLocation != null)
                {
                    extractLocation.Dispose();
                }
            }
        }

        private static List<TarArchiveEntry> ListTarEntries(Stream stream)
        {
            var initialPosition = stream.Position;
            var entries = new List<TarArchiveEntry>();
            if (stream.Length > 0)
            {
                try
                {
                    using (var tarInputStream = new TarInputStream(stream) { IsStreamOwner = false })
                    {
                        var tarEntry = tarInputStream.GetNextEntry();
                        while (tarEntry != null)
                        {
                            entries.Add(new TarArchiveEntry(tarEntry));
                            tarEntry = tarInputStream.GetNextEntry();
                        }
                    }
                    stream.Seek(initialPosition, SeekOrigin.Begin);
                }
                catch (TarException e)
                {
                    throw new InvalidDataException(e.Message, e);
                }
            }
            return entries;
        }

        /// <summary>
        /// A simple wrapper around <see cref="ICSharpCode.SharpZipLib.Tar.TarEntry"/>
        /// </summary>
        private class TarArchiveEntry : CompressedArchiveEntry
        {
            /// <summary>
            /// Initializes a new instance of <see cref="TarArchiveEntry"/>.
            /// </summary>
            /// <param name="tarEntry">The TarEntry to wrap.</param>
            public TarArchiveEntry(TarEntry tarEntry)
            {
                TarEntry = tarEntry;
            }

            /// <inheritdoc />
            public override string Name
            {
                get { return TarEntry.Name; }
            }

            /// <inheritdoc />
            public override long Length
            {
                get { return TarEntry.Size; }
            }

            /// <inheritdoc />
            public override DateTime LastModificationTime
            {
                get { return TarEntry.ModTime; }
            }

            /// <inheritdoc />
            public override bool IsDirectory
            {
                get { return TarEntry.IsDirectory; }
            }

            private TarEntry TarEntry { get; set; }

            /// <summary>
            /// Open a stream to write data to the entry.
            /// </summary>
            /// <param name="tarAccess">The archive that will contain this entry.</param>
            /// <returns>The stream to write to.</returns>
            public Stream OpenOutputStream(TarAccessSharpZipLib tarAccess)
            {
                var stream = new OutputStream(tarAccess, this);
                return stream;
            }

            /// <summary>
            /// This implementation is, hopefully, not TOO clever....
            /// </summary>
            /// <remarks>SharpZipLib doesn't provide a very Stream-friendly way to access entries - it requires
            /// a file. What this stream does is present a memory stream to be written to that, when disposed,
            /// writes to a temporary file and then triggers writing the entry to the TarArchive.</remarks>
            private class OutputStream : MemoryStream
            {
                private int _disposed;
                private TarAccessSharpZipLib _tarAccess;
                private TarArchiveEntry _tarArchiveEntry;

                /// <summary>
                /// Initializes a new instance of <see cref="OutputStream"/>.
                /// </summary>
                /// <param name="tarAccess">The TAR archive to which the contents of <paramref name="tarArchiveEntry"/> is to be written.</param>
                /// <param name="tarArchiveEntry">A previously created <see cref="TarEntry"/> that will be written to.</param>
                public OutputStream(TarAccessSharpZipLib tarAccess, TarArchiveEntry tarArchiveEntry)
                {
                    _tarAccess = tarAccess;
                    _tarArchiveEntry = tarArchiveEntry;
                }

                /// <inheritdoc />
                /// <remarks>This is the really tricky clever part. It's built around the assumption that we can, at this point of
                /// the disposal, still seek back to the beginning of the stream and commit the update that was started in the
                /// constructor. It also assumes that other updates are going to be OK with being committed (if there are any).</remarks>
                protected override void Dispose(bool disposing)
                {
                    if (Interlocked.Increment(ref _disposed) == 1)
                    {
                        var recurse = false;
                        var copyToTemporaryFile = true;
                        var isRooted = Path.IsPathRooted(_tarArchiveEntry.TarEntry.File);
                        if (isRooted)
                        {
                            recurse = Directory.Exists(_tarArchiveEntry.TarEntry.File);
                            copyToTemporaryFile = !recurse && !File.Exists(_tarArchiveEntry.TarEntry.File);
                        }
                        if (copyToTemporaryFile)
                        {
                            // operating on something that thus far is in-memory only
                            using (var temporaryDirectory = new TemporaryDirectory())
                            {
                                var subdirectory = Path.GetDirectoryName(_tarArchiveEntry.Name);
                                if (!string.IsNullOrEmpty(subdirectory))
                                {
                                    Directory.CreateDirectory(Path.Combine(temporaryDirectory.Path, subdirectory));
                                }
                                var entryFilePath = Path.Combine(temporaryDirectory.Path, _tarArchiveEntry.Name);

                                Seek(0, SeekOrigin.Begin);
                                using (var fileStream = new FileStream(entryFilePath, FileMode.Create, FileAccess.Write))
                                {
                                    CopyTo(fileStream);
                                }
                                var tarEntry = TarEntry.CreateEntryFromFile(entryFilePath);
                                tarEntry.Name = _tarArchiveEntry.Name;
                                recurse = IsDirectoryName(tarEntry.Name);
                                _tarAccess.TarArchive.WriteEntry(tarEntry, recurse);
                                _tarArchiveEntry.TarEntry = tarEntry;
                            }
                        }
                        else
                        {
                            // already operating on an existing on-disk entity
                            var tarEntry = _tarArchiveEntry.TarEntry;
                            var tarArchive = _tarAccess.TarArchive;
                            var rootPath = tarArchive.RootPath;
                            if (recurse)
                            {
                                if (!string.IsNullOrEmpty(_tarAccess.RootLocation))
                                {
                                    tarArchive.RootPath = Path.GetDirectoryName(_tarAccess.RootLocation);
                                }
                            }
                            try
                            {
                                tarArchive.WriteEntry(tarEntry, recurse);
                            }
                            finally
                            {
                                if (recurse)
                                {
                                    tarArchive.RootPath = rootPath;
                                }
                            }
                        }
                        base.Dispose(disposing);
                    }
                }
            }
        }
    }
}

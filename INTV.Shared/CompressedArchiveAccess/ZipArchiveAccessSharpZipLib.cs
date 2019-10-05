// <copyright file="ZipArchiveAccessSharpZipLib.cs" company="INTV Funhouse">
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
using ICSharpCode.SharpZipLib.Zip;

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// Provides access to ZIP file contents using SharpZipLib. See: https://github.com/icsharpcode/SharpZipLib
    /// </summary>
    /// <remarks>NOTE: Unlike the standard .NET ZIP API, the SharpZipLib implementation requires read/write access to the
    /// underlying stream when creating a new <see cref="ICSharpCode.SharpZipLib.Zip.ZipFile"/>. (The .NET version only
    /// requires write access.) This may be a snag if consumers of this interface make the assumption that write is sufficient.</remarks>
    internal sealed class ZipArchiveAccessSharpZipLib : CompressedArchive
    {
        /// <summary>
        /// Initialize a new instance of the type from the given stream.
        /// </summary>
        /// <param name="stream">Stream containing data in ZIP archive format.</param>
        /// <param name="mode">The access mode to use for ZIP operations.</param>
        private ZipArchiveAccessSharpZipLib(Stream stream, CompressedArchiveAccessMode mode)
        {
            Mode = mode;
            ZipFile = Open(stream, mode);
        }

        #region ICompressedArchiveAccess

        /// <inheritdoc />
        public override bool IsArchive
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsCompressed
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override CompressedArchiveFormat Format
        {
            get { return CompressedArchiveFormat.Zip; }
        }

        /// <inheritdoc />
        public override IEnumerable<ICompressedArchiveEntry> Entries
        {
            get
            {
                foreach (ZipEntry zipEntry in ZipFile)
                {
                    yield return new ZipArchiveEntry(zipEntry);
                }
            }
        }

        #endregion ICompressedArchiveAccess

        /// <summary>
        /// Gets the access mode that describes actions that can be performed on the archive.
        /// </summary>
        public CompressedArchiveAccessMode Mode { get; private set; }

        private ZipFile ZipFile { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ZipArchiveAccessSharpZipLib"/> using the given mode.
        /// </summary>
        /// <param name="stream">Stream containing data in ZIP archive format.</param>
        /// <param name="mode">The access mode to use for ZIP operations.</param>
        /// <returns>A new instance of <see cref="ZipArchiveAccessSharpZipLib"/>.</returns>
        /// <remarks>The ZIP archive assumes ownership of <paramref name="stream"/> and will dispose it.</remarks>
        public static ZipArchiveAccessSharpZipLib Create(Stream stream, CompressedArchiveAccessMode mode)
        {
            return new ZipArchiveAccessSharpZipLib(stream, mode);
        }

        #region ICompressedArchiveAccess

        /// <inheritdoc />
        public override Stream OpenEntry(ICompressedArchiveEntry entry)
        {
            Stream stream = null;
            var zipEntry = ZipFile.GetEntry(entry.Name);
            switch (Mode)
            {
                case CompressedArchiveAccessMode.Read:
                case CompressedArchiveAccessMode.Update:
                    stream = ZipFile.GetInputStream(zipEntry);
                    break;
                case CompressedArchiveAccessMode.Create:
                    stream = new OutputDataSource(ZipFile, zipEntry);
                    break;
            }
            return stream;
        }

        /// <inheritdoc />
        /// <remarks>When using SharpZipLib, we create empty, dummy entries and immediately commit them. Subsequently when an
        /// entry is opened, and written to, the entry is populated. See the documentation for <see cref="OutputDataSource"/>.</remarks>
        public override ICompressedArchiveEntry CreateEntry(string name)
        {
            ZipFile.BeginUpdate();
            if (IsDirectoryName(name))
            {
                ZipFile.AddDirectory(name);
            }
            else
            {
                var dataSource = new DummyDataSource();
                ZipFile.Add(dataSource, name);
            }
            ZipFile.CommitUpdate();

            var zipArchiveEntry = GetEntry(name);
            return zipArchiveEntry;
        }

        /// <inheritdoc />
        protected override bool DeleteEntry(ICompressedArchiveEntry entry)
        {
            var zipEntry = entry as ZipArchiveEntry;
            var deleted = zipEntry != null;
            if (deleted)
            {
                deleted = zipEntry.ZipEntry != null;
                if (deleted)
                {
                    ZipFile.BeginUpdate();
                    ZipFile.Delete(zipEntry.ZipEntry);
                    ZipFile.CommitUpdate();
                }
            }
            return deleted;
        }

        #endregion ICompressedArchiveAccess

        #region IDisposable

        /// <inheritdoc />
        protected override void DisposeCore(bool disposing)
        {
            if (ZipFile != null)
            {
                IDisposable zipFile = ZipFile;
                ZipFile = null;
                if (zipFile != null)
                {
                    zipFile.Dispose();
                }
            }
        }

        #endregion // IDisposable

        private static ZipFile Open(Stream stream, CompressedArchiveAccessMode mode)
        {
            try
            {
                ZipFile zipFile;
                switch (mode)
                {
                    case CompressedArchiveAccessMode.Create:
                        zipFile = ZipFile.Create(stream);
                        zipFile.IsStreamOwner = true; // This creation mode does NOT assume ownership of the stream.
                        break;
                    default:
                        zipFile = new ZipFile(stream); // This creation mode DOES assume ownership of the stream.
                        break;
                }
                return zipFile;
            }
            catch (ZipException e)
            {
                throw new InvalidDataException(e.Message, e); // to be consistent with the native implementation.
            }
        }

        /// <summary>
        /// This implementation of <see cref="IStaticDataSource"/> is used only to create empty dummy entries.
        /// </summary>
        private class DummyDataSource : IStaticDataSource
        {
            /// <inheritdoc />
            public Stream GetSource()
            {
                return new MemoryStream();
            }
        }

        /// <summary>
        /// This implementation of <see cref="IStaticDataSource"/> is, hopefully, not TOO clever....
        /// </summary>
        /// <remarks>SharpZipLib presents a substantially different way to add entries than the native .NET implementation.
        /// Hopefully the tricks done in the <see cref="Dispose(bool)"/> implementation are not going to be a problem.
        /// The (admittedly brittle) implementation here begins an update of the ZipFile when the data source is created.
        /// This object is created when the caller opens a stream to a previously created ZipEntry. The assumption is that
        /// the standard using(var stream = zip.OpenEntry(entry)) approach is used to write an entry in its entirety, followed
        /// by disposing the stream. Upon disposal, this object commits the update, thus writing the output to the ZIP file.</remarks>
        private class OutputDataSource : MemoryStream,  IStaticDataSource
        {
            private int _disposed;
            private ZipFile _zipFile;

            /// <summary>
            /// Initializes a new instance of <see cref="OutputDataSource"/>.
            /// </summary>
            /// <param name="zipFile">The ZIP archive to which the contents of <paramref name="zipEntry"/> is to be written.</param>
            /// <param name="zipEntry">A previously created <see cref="ZipEntry"/> that will be written to.</param>
            public OutputDataSource(ZipFile zipFile, ZipEntry zipEntry)
            {
                _zipFile = zipFile;
                _zipFile.BeginUpdate();
                _zipFile.Add(this, zipEntry.Name);
            }

            /// <inheritdoc />
            public Stream GetSource()
            {
                return this;
            }

            /// <inheritdoc />
            /// <remarks>This is the really tricky clever part. It's built around the assumption that we can, at this point of
            /// the disposal, still seek back to the beginning of the stream and commit the update that was started in the
            /// constructor. It also assumes that other updates are going to be OK with being committed (if there are any).</remarks>
            protected override void Dispose(bool disposing)
            {
                if (Interlocked.Increment(ref _disposed) == 1)
                {
                    Seek(0, SeekOrigin.Begin);
                    _zipFile.CommitUpdate();
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// A simple wrapper around <see cref="ICSharpCode.SharpZipLib.Zip.ZipEntry"/>.
        /// </summary>
        private class ZipArchiveEntry : CompressedArchiveEntry
        {
            /// <summary>
            /// Initializes a new instance of <see cref="ZipArchiveEntry"/>.
            /// </summary>
            /// <param name="zipEntry">The ZipEntry to wrap.</param>
            public ZipArchiveEntry(ZipEntry zipEntry)
            {
                ZipEntry = zipEntry;
            }

            /// <inheritdoc />
            public override string Name
            {
                get { return ZipEntry.Name; }
            }

            /// <inheritdoc />
            public override long Length
            {
                get { return ZipEntry.Size; }
            }

            /// <inheritdoc />
            public override DateTime LastModificationTime
            {
                get { return ZipEntry.DateTime; }
            }

            /// <inheritdoc />
            public override bool IsDirectory
            {
                get { return ZipEntry.IsDirectory; }
            }

            /// <summary>
            /// Gets the wrapped <see cref="ZipEntry"/> instance.
            /// </summary>
            internal ZipEntry ZipEntry { get; private set; }
        }
    }
}

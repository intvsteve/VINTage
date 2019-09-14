// <copyright file="TestStorageAccess.cs" company="INTV Funhouse">
// Copyright (c) 2018-2019 All Rights Reserved
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
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using INTV.Core.Utility;

namespace INTV.TestHelpers.Core.Utility
{
    /// <summary>
    /// Implements <see cref="IStorageAccess"/> for testing purposes.
    /// </summary>
    public class TestStorageAccess : IStorageAccess
    {
        // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
        private static readonly DateTime FileNotFoundTime = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Lazy<ConcurrentDictionary<string, TestStorageStream>> FakeFileSystem = new Lazy<ConcurrentDictionary<string, TestStorageStream>>();

        private static ConcurrentDictionary<string, TestStorageStream> FileSystem
        {
            get { return FakeFileSystem.Value; }
        }

        /// <summary>
        /// Gets a storage location with a null path.
        /// </summary>
        public StorageLocation NullLocation
        {
            get { return new StorageLocation(null, this); }
        }

        /// <summary>
        /// Creates a new storage location.
        /// </summary>
        /// <param name="path">The location in the storage.</param>
        /// <returns>The new location.</returns>
        public StorageLocation CreateLocation(string path)
        {
            var location = new StorageLocation(path, this);
            return location;
        }

        /// <summary>
        /// Opens or creates a stream, initializing with bytes 0xFF if <paramref name="initialDataSize"/> is greater than zero.
        /// </summary>
        /// <param name="storageLocation">Location of the stream.</param>
        /// <param name="initialDataSize">Initial data size of the stream (in bytes).</param>
        /// <returns>The stream, initialized.</returns>
        /// <remarks>The capacity of the stream will be rounded up to the nearest multiple of 64 bytes.</remarks>
        public Stream OpenOrCreate(string storageLocation, int initialDataSize)
        {
            lock (FileSystem)
            {
                return TestStorageStream.OpenOrCreate(FileSystem, storageLocation, initialDataSize);
            }
        }

        /// <summary>
        /// Sets the last write time (UTC) on a previously created stream.
        /// </summary>
        /// <param name="storageLocation">Location of the stream.</param>
        /// <param name="lastWriteTimeUtc">The last write time (in UTC) to set.</param>
        /// <exception cref="System.FileNotFoundException">Thrown if stream at <paramref name="location"/> does not exist.</exception>
        public void SetLastWriteTimeUtc(string storageLocation, DateTime lastWriteTimeUtc)
        {
            lock (FileSystem)
            {
                TestStorageStream stream;
                if (!FileSystem.TryGetValue(storageLocation, out stream))
                {
                    throw new FileNotFoundException();
                }
                stream.LastWriteTimeUtc = lastWriteTimeUtc;
            }
        }

        /// <summary>
        /// Renames an existing stream.
        /// </summary>
        /// <param name="storageLocation">The current location for the stream.</param>
        /// <param name="newStorageLocation">The new location for the stream.</param>
        /// <exception cref="System.FileNotFoundException">Thrown if stream at <paramref name="location"/> does not exist.</exception>
        public void Rename(string storageLocation, string newStorageLocation)
        {
            lock (FileSystem)
            {
                TestStorageStream stream;
                if (!FileSystem.TryRemove(storageLocation, out stream))
                {
                    throw new FileNotFoundException();
                }
                stream.Location = newStorageLocation;
                FileSystem[newStorageLocation] = stream;
            }
        }

        /// <inheritdoc />
        public Stream Open(string storageLocation)
        {
            lock (FileSystem)
            {
                return TestStorageStream.OpenOrCreate(FileSystem, storageLocation, -1);
            }
        }

        /// <inheritdoc />
        public bool Exists(string storageLocation)
        {
            return FileSystem.ContainsKey(storageLocation);
        }

        /// <inheritdoc />
        public long Size(string storageLocation)
        {
            var size = 0L;
            TestStorageStream stream;
            if (FileSystem.TryGetValue(storageLocation, out stream))
            {
                size = stream.Length;
            }
            else
            {
                throw new FileNotFoundException("File not found", storageLocation);
            }
            return size;
        }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc(string storageLocation)
        {
            var lastWriteTimeUtc = FileNotFoundTime;
            TestStorageStream stream;
            if (FileSystem.TryGetValue(storageLocation, out stream))
            {
                lastWriteTimeUtc = stream.LastWriteTimeUtc;
            }
            return lastWriteTimeUtc;
        }

        /// <inheritdoc />
        public bool IsLocationAContainer(string storageLocation)
        {
            var isContainer = !string.IsNullOrEmpty(storageLocation);
            if (isContainer)
            {
                var lastCharacter = storageLocation.Last();
                isContainer = (lastCharacter == Path.DirectorySeparatorChar) || (lastCharacter == Path.AltDirectorySeparatorChar);
            }
            return isContainer;
        }

        /// <summary>
        /// Introduces a corruption into the file system by setting an existing location's data to <c>null</c>.
        /// </summary>
        /// <param name="storageLocation">The location in the storage access to corrupt.</param>
        /// <returns><c>true</c> if the corruption was successfully introduced.</returns>
        public bool IntroduceCorruption(string storageLocation)
        {
            TestStorageStream stream;
            FileSystem.TryGetValue(storageLocation, out stream);
            var mischiefManaged = FileSystem.TryUpdate(storageLocation, null, stream);
            return mischiefManaged;
        }

        private class TestStorageStream : MemoryStream
        {
            private int _usageCount = 0;

            private TestStorageStream(ConcurrentDictionary<string, TestStorageStream> fileSystem, string location)
            {
                FileSystem = fileSystem;
                Location = location;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists", Justification = "Not using base.Seek() triggers CA complaint. This class simply does not override it.")]
            private TestStorageStream(ConcurrentDictionary<string, TestStorageStream> fileSystem, string location, int initialDataSize)
                : base(((initialDataSize / 64) + 1) * 64)
            {
                FileSystem = fileSystem;
                Location = location;
                base.Write(Enumerable.Repeat((byte)0xFF, initialDataSize).ToArray(), 0, initialDataSize);
                base.Seek(0, SeekOrigin.Begin);
            }

            public string Location { get; set; }

            public DateTime LastWriteTimeUtc { get; set; }

            private ConcurrentDictionary<string, TestStorageStream> FileSystem { get; set; } // << REALLY do not like this!

            public static TestStorageStream OpenOrCreate(ConcurrentDictionary<string, TestStorageStream> fileSystem, string location, int initialSizeInBytes)
            {
                TestStorageStream stream;
                if (!fileSystem.TryGetValue(location, out stream))
                {
                    if (initialSizeInBytes > 0)
                    {
                        stream = new TestStorageStream(fileSystem, location, initialSizeInBytes);
                    }
                    else
                    {
                        stream = new TestStorageStream(fileSystem, location);
                    }
                    if (!fileSystem.TryAdd(location, stream))
                    {
                        throw new InvalidOperationException();
                    }
                }
                stream.Open();
                return stream;
            }

            /// <inheritdoc />
            public override void Write(byte[] buffer, int offset, int count)
            {
                LastWriteTimeUtc = DateTime.UtcNow;
                base.Write(buffer, offset, count);
            }

            /// <inheritdoc />
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                LastWriteTimeUtc = DateTime.UtcNow;
                return base.WriteAsync(buffer, offset, count, cancellationToken);
            }

            /// <inheritdoc />
            public override void WriteByte(byte value)
            {
                LastWriteTimeUtc = DateTime.UtcNow;
                base.WriteByte(value);
            }

            protected override void Dispose(bool disposing)
            {
                var refCount = Interlocked.Decrement(ref _usageCount);
                if (refCount == 0)
                {
                    TestStorageStream dontCare;
                    FileSystem.TryRemove(Location, out dontCare); // ICK!
                    base.Dispose(disposing);
                }
            }

            private void Open()
            {
                Interlocked.Increment(ref _usageCount);
                Seek(0, SeekOrigin.Begin);
            }
        }
    }
}

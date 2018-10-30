// <copyright file="TestStorageAccess.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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
        /// <summary>
        /// Opens or creates a stream, initializing with bytes 0xFF if <paramref name="initialDataSize"/> is greater than zero.
        /// </summary>
        /// <param name="storageLocation">Location of the stream.</param>
        /// <param name="initialDataSize">Initial data size of the stream (in bytes).</param>
        /// <returns>The stream, initialized.</returns>
        /// <remarks>The capacity of the stream will be rounded up to the nearest multiple of 64 bytes.</remarks>
        public static Stream OpenOrCreate(string storageLocation, int initialDataSize)
        {
            return TestStorageStream.OpenOrCreate(storageLocation, initialDataSize);
        }

        /// <summary>
        /// Gets the last write time (UTC) on a previously created stream.
        /// </summary>
        /// <param name="storageLocation">Location of the stream.</param>
        /// <returns>Last modification time of the file, in UTC.</returns>
        public static DateTime GetLastWriteTimeUtc(string storageLocation)
        {
            return TestStorageStream.GetLastWriteTimeUtc(storageLocation);
        }

        /// <summary>
        /// Sets the last write time (UTC) on a previously created stream.
        /// </summary>
        /// <param name="storageLocation">Location of the stream.</param>
        /// <param name="lastWriteTimeUtc">The last write time (in UTC) to set.</param>
        /// <exception cref="System.FileNotFoundException">Thrown if stream at <paramref name="location"/> does not exist.</exception>
        public static void SetLastWriteTimeUtc(string storageLocation, DateTime lastWriteTimeUtc)
        {
            TestStorageStream.SetLastWriteTimeUtc(storageLocation, lastWriteTimeUtc);
        }

        /// <summary>
        /// Renames an existing stream.
        /// </summary>
        /// <param name="storageLocation">The current location for the stream.</param>
        /// <param name="newStorageLocation">The new location for the stream.</param>
        /// <exception cref="System.FileNotFoundException">Thrown if stream at <paramref name="location"/> does not exist.</exception>
        public static void Rename(string storageLocation, string newStorageLocation)
        {
            TestStorageStream.Rename(storageLocation, newStorageLocation);
        }

        /// <inheritdoc />
        public Stream Open(string storageLocation)
        {
            return TestStorageStream.OpenOrCreate(storageLocation);
        }

        /// <inheritdoc />
        public bool Exists(string storageLocation)
        {
            return TestStorageStream.Exists(storageLocation);
        }

        /// <inheritdoc />
        public long Size(string storageLocation)
        {
            return TestStorageStream.GetSize(storageLocation);
        }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc(string storageLocation)
        {
            return TestStorageStream.GetLastWriteTimeUtc(storageLocation);
        }

        /// <summary>
        /// Introduces a corruption into the file system by setting an existing location's data to <c>null</c>.
        /// </summary>
        /// <param name="storageLocation">The location in the storage access to corrupt.</param>
        /// <returns><c>true</c> if the corruption was successfully introduced.</returns>
        public bool IntroduceCorruption(string storageLocation)
        {
            var mischiefManaged = TestStorageStream.IntroduceCorruption(storageLocation);
            return mischiefManaged;
        }

        private class TestStorageStream : MemoryStream
        {
            private static readonly Lazy<ConcurrentDictionary<string, TestStorageStream>> FakeFileSystem = new Lazy<ConcurrentDictionary<string, TestStorageStream>>();

            // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
            private static readonly DateTime FileNotFoundTime = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            private int _usageCount = 0;

            private TestStorageStream(string location)
            {
                Location = location;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists", Justification = "Not using base.Seek() triggers CA complaint. This class simply does not override it.")]
            private TestStorageStream(string location, int initialDataSize)
                : base(((initialDataSize / 64) + 1) * 64)
            {
                Location = location;
                base.Write(Enumerable.Repeat((byte)0xFF, initialDataSize).ToArray(), 0, initialDataSize);
                base.Seek(0, SeekOrigin.Begin);
            }

            private static ConcurrentDictionary<string, TestStorageStream> FileSystem
            {
                get { return FakeFileSystem.Value; }
            }

            public string Location { get; private set; }

            public DateTime LastWriteTimeUtc { get; private set; }

            public static TestStorageStream OpenOrCreate(string location)
            {
                return OpenOrCreate(location, -1);
            }

            public static TestStorageStream OpenOrCreate(string location, int initialSizeInBytes)
            {
                lock (FileSystem)
                {
                    TestStorageStream stream;
                    if (!FileSystem.TryGetValue(location, out stream))
                    {
                        if (initialSizeInBytes > 0)
                        {
                            stream = new TestStorageStream(location, initialSizeInBytes);
                        }
                        else
                        {
                            stream = new TestStorageStream(location);
                        }
                        if (!FileSystem.TryAdd(location, stream))
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    stream.Open();
                    return stream;
                }
            }

            public static bool Exists(string location)
            {
                return FileSystem.ContainsKey(location);
            }

            public static long GetSize(string location)
            {
                var size = 0L;
                TestStorageStream stream;
                if (FileSystem.TryGetValue(location, out stream))
                {
                    size = stream.Length;
                }
                else
                {
                    throw new FileNotFoundException("File not found", location);
                }
                return size;
            }

            public static DateTime GetLastWriteTimeUtc(string location)
            {
                var lastWriteTimeUtc = FileNotFoundTime;
                TestStorageStream stream;
                if (FileSystem.TryGetValue(location, out stream))
                {
                    lastWriteTimeUtc = stream.LastWriteTimeUtc;
                }
                return lastWriteTimeUtc;
            }

            public static void SetLastWriteTimeUtc(string location, DateTime lastWriteTimeUtc)
            {
                lock (FileSystem)
                {
                    TestStorageStream stream;
                    if (!FileSystem.TryGetValue(location, out stream))
                    {
                        throw new FileNotFoundException();
                    }
                    stream.LastWriteTimeUtc = lastWriteTimeUtc;
                }
            }

            public static void Rename(string location, string newLocation)
            {
                lock (FileSystem)
                {
                    TestStorageStream stream;
                    if (!FileSystem.TryRemove(location, out stream))
                    {
                        throw new FileNotFoundException();
                    }
                    stream.Location = newLocation;
                    FileSystem[newLocation] = stream;
                }
            }

            public static bool IntroduceCorruption(string location)
            {
                TestStorageStream stream;
                FileSystem.TryGetValue(location, out stream);
                var mischiefManaged = FileSystem.TryUpdate(location, null, stream);
                return mischiefManaged;
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
                    FileSystem.TryRemove(Location, out dontCare);
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

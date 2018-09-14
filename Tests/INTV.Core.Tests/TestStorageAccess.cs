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

namespace INTV.Core.Tests
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
        public static Stream OpenOrCreate(string storageLocation, int initialDataSize)
        {
            return TestStorageStream.OpenOrCreate(storageLocation, initialDataSize);
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

        private class TestStorageStream : MemoryStream
        {
            private static readonly ConcurrentDictionary<string, TestStorageStream> Instances = new ConcurrentDictionary<string, TestStorageStream>();

            // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
            private static readonly DateTime FileNotFoundTime = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            private TestStorageStream(string location)
            {
                Location = location;
            }

            private TestStorageStream(string location, int initialDataSize)
                : base(Enumerable.Repeat((byte)0xFF, initialDataSize).ToArray())
            {
                Location = location;
            }

            public string Location { get; private set; }

            public DateTime LastWriteTimeUtc { get; private set; }

            public static TestStorageStream OpenOrCreate(string location)
            {
                return OpenOrCreate(location, -1);
            }

            public static TestStorageStream OpenOrCreate(string location, int initialSizeInBytes)
            {
                lock (Instances)
                {
                    TestStorageStream stream;
                    if (!Instances.TryGetValue(location, out stream))
                    {
                        if (initialSizeInBytes > 0)
                        {
                            stream = new TestStorageStream(location, initialSizeInBytes);
                        }
                        else
                        {
                            stream = new TestStorageStream(location);
                        }
                        if (!Instances.TryAdd(location, stream))
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    return stream;
                }
            }

            public static bool Exists(string location)
            {
                return Instances.ContainsKey(location);
            }

            public static long GetSize(string location)
            {
                var size = 0L;
                TestStorageStream stream;
                if (Instances.TryGetValue(location, out stream))
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
                if (Instances.TryGetValue(location, out stream))
                {
                    lastWriteTimeUtc = stream.LastWriteTimeUtc;
                }
                return lastWriteTimeUtc;
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
                TestStorageStream dontCare;
                Instances.TryRemove(Location, out dontCare);
                base.Dispose(disposing);
            }
        }
    }
}

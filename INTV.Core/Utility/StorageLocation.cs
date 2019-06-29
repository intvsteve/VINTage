// <copyright file="StorageLocation.cs" company="INTV Funhouse">
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// This type provides an abstraction to use for identifying the location of data in some kind of storage.
    /// Think of it as an abstracted file path, e.g. a 'file path' is a named location of a disk-based storage mechanism,
    /// whereas a URL string could be 'storage location' for data "stored" on the internet.
    /// </summary>
    /// <remarks>Though incorrect in case-sensitive file systems, locations are considered equal in a case-insensitive manner.</remarks>
    public struct StorageLocation : IEquatable<StorageLocation>, IComparable<StorageLocation>
    {
        /// <summary>
        /// The invalid storage location.
        /// </summary>
        public static readonly StorageLocation InvalidLocation = new StorageLocation(null, new InvalidStorageAccess());

        /// <summary>
        /// Initializes a new instance of <see cref="StorageLocation"/> with a location, using default storage.
        /// </summary>
        /// <param name="path">The path in the default storage.</param>
        public StorageLocation(string path)
            : this(path, IStorageAccessHelpers.DefaultStorage)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StorageLocation"/> with a location, using the given storage.
        /// </summary>
        /// <param name="path">The path in the storage.</param>
        /// <param name="storageAccess">The storage access to use.</param>
        public StorageLocation(string path, IStorageAccess storageAccess)
        {
            _path = path;
            _storageAccess = storageAccess;
        }

        /// <summary>
        /// Gets the location (path) within the storage.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }
        private string _path;

        /// <summary>
        /// Gets the storage for the path.
        /// </summary>
        public IStorageAccess StorageAccess
        {
            get { return _storageAccess.GetStorageAccess(); }
        }
        private IStorageAccess _storageAccess;

        /// <summary>
        /// Equality operator for <see cref="StorageAccess"/>.
        /// </summary>
        /// <param name="lhs">The value on the left hand side of the equality operator.</param>
        /// <param name="rhs">The value on the right hand side of the equality operator.</param>
        /// <returns><c>true</c> if the values are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(StorageLocation lhs, StorageLocation rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Inequality operator for <see cref="StorageAccess"/>.
        /// </summary>
        /// <param name="lhs">The value on the left hand side of the inequality operator.</param>
        /// <param name="rhs">The value on the right hand side of the inequality operator.</param>
        /// <returns><c>true</c> if the values are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(StorageLocation lhs, StorageLocation rhs)
        {
            return !(lhs == rhs);
        }
#if false
        /// <summary>
        /// Places a location specified by <paramref name="path"/> into a <see cref="StorageLocation"/> in the default storage.
        /// </summary>
        /// <param name="path">A location in the default storage.</param>
        /// <returns>A <see cref="StorageLocation"/> for <paramref name="path"/> that will use the default storage.</returns>
        public static implicit operator StorageLocation(string path)
        {
            return new StorageLocation(path);
        }
#endif
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_path != null)
            {
                hashCode = _path.ToUpperInvariant().GetHashCode();
            }
            if (_storageAccess != IStorageAccessHelpers.DefaultStorage)
            {
                hashCode = CombineHashCodes(hashCode, _storageAccess.GetHashCode());
            }
            return hashCode;
        }

        /// <inheritdoc/>
        public int CompareTo(StorageLocation other)
        {
            var result = object.ReferenceEquals(StorageAccess, other.StorageAccess) ? 0 : -1;
            if (result == 0)
            {
                result = StringComparer.OrdinalIgnoreCase.Compare(_path, other._path);
            }
            return result;
        }

        /// <inheritdoc/>
        public bool Equals(StorageLocation other)
        {
            return CompareTo(other) == 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var equal = false;
            if (obj is StorageLocation)
            {
                equal = Equals((StorageLocation)obj);
            }
            return equal;
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private class InvalidStorageAccess : IStorageAccess
        {
            // See https://docs.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetime?view=netframework-4.0
            private static readonly System.DateTime FileNotFoundTime = new System.DateTime(1601, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            /// <inheritdoc/>
            public System.IO.Stream Open(string storageLocation)
            {
                throw new System.InvalidOperationException();
            }

            /// <inheritdoc/>
            public bool Exists(string storageLocation)
            {
                return false;
            }

            /// <inheritdoc/>
            public long Size(string storageLocation)
            {
                return -1;
            }

            /// <inheritdoc/>
            public System.DateTime LastWriteTimeUtc(string storageLocation)
            {
                return FileNotFoundTime;
            }
        }
    }
}

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

////#define ENABLE_IMPLICIT_CONVERSIONS

using System;
using System.Linq;

namespace INTV.Core.Utility
{
    /// <summary>
    /// This type provides an abstraction to use for identifying the location of data in some kind of storage.
    /// Think of it as an abstracted file path, e.g. a 'file path' is a named location of a disk-based storage mechanism,
    /// whereas a URL string could be 'storage location' for data "stored" on the internet.
    /// </summary>
    /// <remarks>Though incorrect in case-sensitive file systems, locations are considered equal in a case-insensitive manner.</remarks>
    [System.Diagnostics.DebuggerDisplay("{Path,nq}, {_storageAccess}")]
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
            : this(path, storageAccess, isContainer: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageLocation"/> with a location, storage, and whether or not location is a container.
        /// </summary>
        /// <param name="path">The path in the storage.</param>
        /// <param name="storageAccess">The storage access to use.</param>
        /// <param name="isContainer">Indicates whether <paramref name="path"/> refers to a container (e.g. directory or archive).
        /// A value of <c>null</c> indicates the status is not specified and will be determined later.</param>
        public StorageLocation(string path, IStorageAccess storageAccess, bool? isContainer)
        {
            _path = path;
            _storageAccess = storageAccess;
            _isContainer = isContainer;
        }

        /// <summary>
        /// Gets a null location that will use the default storage access. Primarily for testing purposes.
        /// </summary>
        public static StorageLocation Null
        {
            get { return new StorageLocation(); }
        }

        /// <summary>
        /// Gets an empty location that will use the default storage access. Mainly for testing purposes.
        /// </summary>
        public static StorageLocation Empty
        {
            get { return new StorageLocation(string.Empty); }
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
        /// Gets a value indicating whether <see cref="Path"/> is <c>null</c>.
        /// </summary>
        public bool IsNull
        {
            get { return _path == null; }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Path"/> is an empty string.
        /// </summary>
        public bool IsEmpty
        {
            get { return _path == string.Empty; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the storage location is valid.
        /// </summary>
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(_path); }
        }

        /// <summary>
        /// Gets a value indicating whether the location is a canonical invalid storage location.
        /// </summary>
        public bool IsInvalid
        {
            get { return _storageAccess is InvalidStorageAccess; }
        }

        /// <summary>
        /// Gets a value indicating whether the location is container of other locations.
        /// </summary>
        /// <remarks>A container is either a traditional file system directory, an archive file (e.g. .zip, .tar), or a
        /// directory within an archive.</remarks>
        public bool IsContainer
        {
            get
            {
                if (!IsValid)
                {
                    return false;
                }
                if (!_isContainer.HasValue)
                {
                    _isContainer = StorageAccess.IsLocationAContainer(Path);
                }
                return _isContainer.Value;
            }
        }
        private bool? _isContainer;

        /// <summary>
        /// Gets a value indicating whether the location is using the default pseudo-storage as its storage access.
        /// </summary>
        public bool UsesDefaultStorage
        {
            get { return _storageAccess == IStorageAccessHelpers.DefaultStorage; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="StorageLocation"/> using a new path but using the same <see cref="StorageAccess"/>.
        /// </summary>
        /// <returns>A new <see cref="StorageLocation"/> that refers to <paramref name="newPath"/>.</returns>
        /// <param name="location">The location whose <see cref="StorageAccess"/> will be retained.</param>
        /// <param name="newPath">The new path.</param>
        public static StorageLocation CopyWithNewPath(StorageLocation location, string newPath)
        {
            return new StorageLocation(newPath, location._storageAccess);
        }

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

#if ENABLE_IMPLICIT_CONVERSIONS

        /// <summary>
        /// Implicit converter that places a location specified by <paramref name="path"/> into a <see cref="StorageLocation"/> in the default storage.
        /// </summary>
        /// <param name="path">A location in the default storage.</param>
        /// <returns>A <see cref="StorageLocation"/> for <paramref name="path"/> that will use the default storage.</returns>
        public static implicit operator StorageLocation(string path)
        {
            return new StorageLocation(path);
        }

        /// <summary>
        /// Implicit converter that returns the path from a <see cref="StorageLocation"/>.
        /// </summary>
        /// <param name="location">A storage location whose path is returned as a string.</param>
        /// <returns>The <see cref="StorageLocation.Path"/>, regardless of validity of the location.</returns>
        public static implicit operator string(StorageLocation location)
        {
            return location.Path;
        }

#endif // ENABLE_IMPLICIT_CONVERSIONS

        /// <inheritdoc/>
        public override string ToString()
        {
            return Path;
        }

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

            /// <inheritdoc/>
            public bool IsLocationAContainer(string storageLocation)
            {
                return false;
            }
        }
    }
}

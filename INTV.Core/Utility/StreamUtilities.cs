// <copyright file="StreamUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Utility functions for working with the Stream type.
    /// </summary>
    public static class StreamUtilities
    {
        /// <summary>
        /// Special value to signify the default <see cref="IStorageAccess"/>.
        /// </summary>
        internal const IStorageAccess DefaultStorage = null;

        private static readonly Lazy<ConcurrentDictionary<Type, IStorageAccess>> RegisteredStorageAccessProviders = new Lazy<ConcurrentDictionary<Type, IStorageAccess>>(() => new ConcurrentDictionary<Type, IStorageAccess>());
        private static readonly Lazy<IStorageAccess> DefaultStorageAccessProvider = new Lazy<IStorageAccess>(GetDefaultStorageAccess);

        /// <summary>
        /// Gets the default storage access.
        /// </summary>
        internal static IStorageAccess DefaultStorageAccess
        {
            get { return DefaultStorageAccessProvider.Value; }
        }

        /// <summary>
        /// If an application wishes to access data in some storage, e.g. a file system, it must register this interface.
        /// </summary>
        /// <param name="storageAccess">The storage access interface.</param>
        /// <returns><c>true</c> if initialization was successful, <c>false</c> if an instance of the <see cref="IStorageAccess"/> type has already been registered.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="storageAccess"/> is <c>null</c>.</exception>
        public static bool Initialize(IStorageAccess storageAccess)
        {
            if (storageAccess == null)
            {
                throw new ArgumentNullException();
            }
            return RegisteredStorageAccessProviders.Value.TryAdd(storageAccess.GetType(), storageAccess);
        }

        /// <summary>
        /// Attempt to remove the storage access.
        /// </summary>
        /// <param name="storageAccess">The <see cref="IStorageAccess"/> to remove.</param>
        /// <returns><c>true</c> if <paramref name="storageAccess"/> was successfully removed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="storageAccess"/> is <c>null</c>.</exception>
        public static bool Remove(IStorageAccess storageAccess)
        {
            if (storageAccess == null)
            {
                throw new ArgumentNullException();
            }
            var removed = false;
            IStorageAccess current;
            if (RegisteredStorageAccessProviders.Value.TryGetValue(storageAccess.GetType(), out current))
            {
                if (object.ReferenceEquals(current, storageAccess))
                {
                    removed = RegisteredStorageAccessProviders.Value.TryRemove(storageAccess.GetType(), out current);
                }
            }
            return removed;
        }

        /// <summary>
        /// Opens a Stream using an absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <param name="storageAccess">The storage access to use; if <c>null</c> the default storage access is used.</param>
        /// <returns>A Stream for accessing the contents of the file.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="StreamUtilities.Initialize(IStorageAccess)"/> method.</remarks>
        public static Stream OpenFileStream(string filePath, IStorageAccess storageAccess = DefaultStorage)
        {
            return GetStorageAccess(storageAccess).Open(filePath);
        }

        /// <summary>
        /// Verifies that a file exists at the given absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <param name="storageAccess">The storage access to use; if <c>null</c> the default storage access is used.</param>
        /// <returns><c>true</c> if the file exists at the given path.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="StreamUtilities.Initialize(IStorageAccess)"/> method.</remarks>
        public static bool FileExists(string filePath, IStorageAccess storageAccess = DefaultStorage)
        {
            return GetStorageAccess(storageAccess).Exists(filePath);
        }

        /// <summary>
        /// Gets the size, in bytes, of the file at the given absolute path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <param name="storageAccess">The storage access to use; if <c>null</c> the default storage access is used.</param>
        /// <returns>Length of the file, in bytes.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="StreamUtilities.Initialize(IStorageAccess)"/> method.</remarks>
        public static long FileSize(string filePath, IStorageAccess storageAccess = DefaultStorage)
        {
            return GetStorageAccess(storageAccess).Size(filePath);
        }

        /// <summary>
        /// Gets the last modification time of the file at the given path.
        /// </summary>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <param name="storageAccess">The storage access to use; if <c>null</c> the default storage access is used.</param>
        /// <returns>Last modification time of the file, in UTC.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="StreamUtilities.Initialize(IStorageAccess)"/> method.</remarks>
        public static DateTime LastFileWriteTimeUtc(string filePath, IStorageAccess storageAccess = DefaultStorage)
        {
            return GetStorageAccess(storageAccess).LastWriteTimeUtc(filePath);
        }

        private static IStorageAccess GetStorageAccess(IStorageAccess specificStorageAccess)
        {
            IStorageAccess storageAccess = specificStorageAccess;
            if (specificStorageAccess == DefaultStorage)
            {
                storageAccess = DefaultStorageAccess;
            }
            return storageAccess;
        }

        private static IStorageAccess GetDefaultStorageAccess()
        {
            var storageAccess = RegisteredStorageAccessProviders.Value.FirstOrDefault().Value;
            return storageAccess;
        }
    }
}

// <copyright file="StorageLocationExtensions.cs" company="INTV Funhouse">
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Extension methods for <see cref="StorageLocation"/>.
    /// </summary>
    public static class StorageLocationExtensions
    {
        /// <summary>
        /// Opens a Stream using the given storage location.
        /// </summary>
        /// <param name="location">A storage location.</param>
        /// <returns>A Stream for accessing the contents of the data.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="IStorageAccessHelpers.Initialize(IStorageAccess)"/> method.</remarks>
        public static Stream OpenStream(this StorageLocation location)
        {
            return location.StorageAccess.Open(location.Path);
        }

        /// <summary>
        /// Verifies that data exists at the given storage location.
        /// </summary>
        /// <param name="location">A storage location.</param>
        /// <returns><c>true</c> if data exists at the given location.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="IStorageAccessHelpers.Initialize(IStorageAccess)"/> method.</remarks>
        public static bool Exists(this StorageLocation location)
        {
            return (location != null) && location.IsValid && location.StorageAccess.Exists(location.Path);
        }

        /// <summary>
        /// Gets the size, in bytes, of the data at the given absolute path.
        /// </summary>
        /// <param name="location">A storage location.</param>
        /// <returns>Size of the data at the given location, in bytes, if available. A value of -1 indicates size information is not available.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="IStorageAccessHelpers.Initialize(IStorageAccess)"/> method.</remarks>
        public static long Size(this StorageLocation location)
        {
            return location.StorageAccess.Size(location.Path);
        }

        /// <summary>
        /// Gets the last modification time of the data at the given path.
        /// </summary>
        /// <param name="location">A storage location.</param>
        /// <returns>Last modification time of the data at the given location, in UTC, or <see cref="System.DateTime.MinValue"/> if the value is not available.</returns>
        /// <remarks>Requires a valid <see cref="IStorageAccess"/> to have been registered via <see cref="IStorageAccessHelpers.Initialize(IStorageAccess)"/> method.</remarks>
        public static DateTime LastModificationTimeUtc(this StorageLocation location)
        {
            return location.StorageAccess.LastWriteTimeUtc(location.Path);
        }
    }
}

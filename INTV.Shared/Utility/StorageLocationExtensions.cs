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

using System.IO;
using INTV.Core.Utility;

namespace INTV.Shared.Utility
{
    public static class StorageLocationExtensions
    {
        /// <summary>
        /// Gets the file name from the given location.
        /// </summary>
        /// <param name="location">The location whose file name is desired.</param>
        /// <returns>The file name.</returns>
        /// <remarks>Follows the behavior of <see cref="System.IO.Path.GetFileName(string)"/>.</remarks>
        public static string GetFileName(this StorageLocation location)
        {
            var fileName = Path.GetFileName(location.Path);
            return fileName;
        }

        /// <summary>
        /// Gets the extension from the given location.
        /// </summary>
        /// <param name="location">The location whose extension is desired.</param>
        /// <returns>The extension.</returns>
        /// <remarks>Follows the behavior of <see cref="System.IO.Path.GetExtension(string)"/>.</remarks>
        public static string GetExtension(this StorageLocation location)
        {
            var extension = Path.GetExtension(location.Path);
            return extension;
        }

        /// <summary>
        /// Changes the extension of the location.
        /// </summary>
        /// <param name="location">The location whose extension is to be changed.</param>
        /// <param name="extension">The new extension.</param>
        /// <returns>A location with the new extension.</returns>
        /// <remarks>Follows the rules of <see cref="System.IO.Path.ChangeExtension(string, string)"/>.</remarks>
        public static StorageLocation ChangeExtension(this StorageLocation location, string extension)
        {
            var storageAccess = location.UsesDefaultStorage ? IStorageAccessHelpers.DefaultStorage : location.StorageAccess;
            var path = Path.ChangeExtension(location.Path, extension);
            var locationWithNewExtension = new StorageLocation(path, storageAccess);
            return locationWithNewExtension;
        }

        /// <summary>
        /// Adds a suffix to a location's path.
        /// </summary>
        /// <param name="location">A location whose path will be used to have a suffix appended.</param>
        /// <param name="suffix">The suffix to append.</param>
        /// <returns>A location with a path having the given suffix appended.</returns>
        public static StorageLocation AddSuffix(this StorageLocation location, string suffix)
        {
            var storageAccess = location.UsesDefaultStorage ? IStorageAccessHelpers.DefaultStorage : location.StorageAccess;
            var updatedPath = location.Path + suffix;
            var updatedLocation = new StorageLocation(updatedPath, storageAccess);
            return updatedLocation;
        }

        /// <summary>
        /// Gets the containing location of the given location.
        /// </summary>
        /// <param name="location">The location whose containing location is desired.</param>
        /// <returns>A location for the container of the given location.</returns>
        public static StorageLocation GetContainingLocation(this StorageLocation location)
        {
            var storageAccess = location.UsesDefaultStorage ? IStorageAccessHelpers.DefaultStorage : location.StorageAccess;
            var directory = Path.GetDirectoryName(location.Path);
            var containingLocation = new StorageLocation(directory, storageAccess);
            return containingLocation;
        }

        /// <summary>
        /// Alters the location containing the file to use a new containing location.
        /// </summary>
        /// <param name="location">The location whose container is to be altered.</param>
        /// <param name="newContainingLocation">The new containing location.</param>
        /// <returns>A location with the same file name, but using the updated container location.</returns>
        public static StorageLocation AlterContainingLocation(this StorageLocation location, string newContainingLocation)
        {
            var storageAccess = location.UsesDefaultStorage ? IStorageAccessHelpers.DefaultStorage : location.StorageAccess;
            var fileName = Path.GetFileName(location.Path);
            var locationWithContainer = new StorageLocation(Path.Combine(newContainingLocation, fileName), storageAccess);
            return locationWithContainer;
        }

        /// <summary>
        /// Returns a location that will be unique within its container.
        /// </summary>
        /// <param name="location">The location that needs to be unique.</param>
        /// <returns>A unique location.</returns>
        /// <remarks>Really only works on disk paths...</remarks>
        public static StorageLocation EnsureUnique(this StorageLocation location)
        {
            var storageAccess = location.UsesDefaultStorage ? IStorageAccessHelpers.DefaultStorage : location.StorageAccess;
            var uniquePath = location.Path.EnsureUniqueFileName();
            var uniqueLocation = new StorageLocation(uniquePath, storageAccess);
            return uniqueLocation;
        }
    }
}

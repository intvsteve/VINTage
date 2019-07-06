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
using System.Collections.Generic;
using System.IO;
using INTV.Core.Utility;

namespace INTV.Shared.Utility
{
    public static class StorageLocationExtensions
    {
        /// <summary>
        /// Appends one or more additional elements to the path in the given location.
        /// </summary>
        /// <param name="location">An existing storage location.</param>
        /// <param name="pathElement">The first element to add to the path in <paramref name="location"/>.</param>
        /// <param name="pathElements">Additional elements to append.</param>
        /// <returns>A StorageLocation with the additional path elements appended to its path, using the same StorageAcess.</returns>
        /// <remarks>If <paramref name="location"/> is a null or empty location, then only the elements defined in the
        /// <paramref name="pathElement"/> and additional <paramref name="pathElements"/> parameters will be used.
        /// Also <see cref="System.IO.Path.Combine"/> for further information.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="location"/> is an invalid StorageLocation.</exception>
        public static StorageLocation Combine(this StorageLocation location, string pathElement, params string[] pathElements)
        {
            if (location.IsInvalid)
            {
                throw new InvalidOperationException();
            }
            var elements = new List<string>();
            if (location.IsValid)
            {
                elements.Add(location.Path);
            }
            elements.Add(pathElement);
            if (pathElements.Length > 0)
            {
                elements.AddRange(pathElements);
            }
            var path = Path.Combine(elements.ToArray());
            var newLocation = StorageLocation.CopyWithNewPath(location, path);
            return newLocation;
        }

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
        /// Gets the file name from the given location without its file extension.
        /// </summary>
        /// <param name="location">The storage location whose file name without extension is desired.</param>
        /// <returns>The file name without extension.</returns>
        /// <remarks>Follows the behavior of <see cref="System.IO.Path.GetFielnameWithoutExtension(string)"/>.</remarks>
        public static string GetFileNameWithoutExtension(this StorageLocation location)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(location.Path);
            return fileNameWithoutExtension;
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
            var path = Path.ChangeExtension(location.Path, extension);
            var locationWithNewExtension = StorageLocation.CopyWithNewPath(location, path);
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
            var updatedPath = location.Path + suffix;
            var updatedLocation = StorageLocation.CopyWithNewPath(location, updatedPath);
            return updatedLocation;
        }

        /// <summary>
        /// Gets the containing location of the given location.
        /// </summary>
        /// <param name="location">The location whose containing location is desired.</param>
        /// <returns>A location for the container of the given location.</returns>
        /// <remarks>Vollows the rules of <see cref="System.IO.Path.GetDirectoryname(string)"/>.</remarks>
        public static StorageLocation GetContainingLocation(this StorageLocation location)
        {
            var directory = Path.GetDirectoryName(location.Path);
            var containingLocation = StorageLocation.CopyWithNewPath(location, directory);
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
            var fileName = location.GetFileName();
            var locationWithContainer = StorageLocation.CopyWithNewPath(location, Path.Combine(newContainingLocation, fileName));
            return locationWithContainer;
        }

        #region PathUtils Behaviors

        /// <summary>
        /// Returns a location that will be unique within its container.
        /// </summary>
        /// <param name="location">The location that needs to be unique.</param>
        /// <returns>A unique location.</returns>
        /// <remarks>Really only works on disk paths...</remarks>
        public static StorageLocation EnsureUnique(this StorageLocation location)
        {
            var uniquePath = location.Path.EnsureUniqueFileName();
            var uniqueLocation = StorageLocation.CopyWithNewPath(location, uniquePath);
            return uniqueLocation;
        }

        /// <summary>
        /// Creates a StorageLocation that uses normalized path separators (forward slash).
        /// </summary>
        /// <param name="location">A <see cref="StorageLocation"/> whose path elements are to be separated using a forward slash.</param>
        /// <returns>The given StorageLocation using normalized path separators.</returns>
        public static StorageLocation NormalizeSeparators(this StorageLocation location)
        {
            var normalizedLocation = location;
            if (location.IsValid)
            {
                var normalizedPath = location.Path.Replace('\\', '/');
                normalizedLocation = StorageLocation.CopyWithNewPath(location, normalizedPath);
            }
            return normalizedLocation;
        }

        #endregion // PathUtils Behaviors
    }
}

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
using System.Globalization;
using System.IO;
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Extension methods to make working with <see cref="StorageLocation"/> simpler.
    /// </summary>
    public static partial class StorageLocationExtensions
    {
        /// <summary>
        /// Creates a new <see cref="StorageLocation"/> from a path.
        /// </summary>
        /// <param name="path">The path from which to create the storage location instance.</param>
        /// <returns>A new <see cref="StorageLocation"/> with appropriately initialized state.</returns>
        /// <remarks>It is assumed that <paramref name="path"/> refers to an existing file in the file system. It is not required - though
        /// the resulting storage location will always use the default storage access.</remarks>
        /// <remarks>This method may throw various sorts of exceptions as it does file path operations to determine
        /// the proper storage access to use. Some are documented.</remarks>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> contains invalid characters.</exception>
        public static StorageLocation CreateStorageLocationFromPath(this string path)
        {
            var storageAccess = path.GetStorageAccess();
            var archiveAccess = storageAccess as ICompressedArchiveAccess;
            var storageLocation = new StorageLocation(path, storageAccess);
            return storageLocation;
        }

        /// <summary>
        /// Appends one or more additional elements to the path in the given location.
        /// </summary>
        /// <param name="location">An existing storage location.</param>
        /// <param name="pathElement">The first element to add to the path in <paramref name="location"/>.</param>
        /// <param name="pathElements">Additional elements to append.</param>
        /// <returns>A <see cref="StorageLocation"/> with the additional path elements appended to its path. If necessary, the storage access will be updated.</returns>
        /// <remarks>If <paramref name="location"/> is a null or empty location, then only the elements defined in the
        /// <paramref name="pathElement"/> and additional <paramref name="pathElements"/> parameters will be used. If <paramref name="pathElement"/>
        /// or any values in <paramref name="pathElements"/> are absolute (rooted) paths, then regardless of the path already
        /// in <paramref name="location.Path"/>, the absolute path in the other arguments will take precedence.
        /// Also <see cref="System.IO.Path.Combine"/> for further information. Also note that this method's behavior with
        /// regard to relative path arguments such as <c>..</c> and <c>.</c> is untested - and should be considered unsupported.</remarks>
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
            if (!string.IsNullOrEmpty(pathElement))
            {
                elements.Add(pathElement);
            }
            if ((pathElements != null) && (pathElements.Length > 0))
            {
                elements.AddRange(pathElements);
            }

            var newPath = (elements.Count > 0) ? Path.Combine(elements.ToArray()) : location.Path;
            var newLocation = GetStorageLocationForChangedPath(location, newPath);
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
            var fileName = location.IsValid ? Path.GetFileName(location.Path) : null;
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
            var fileNameWithoutExtension = location.IsValid ? Path.GetFileNameWithoutExtension(location.Path) : null;
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
            var extension = location.IsValid ? Path.GetExtension(location.Path) : null;
            return extension;
        }

        /// <summary>
        /// Changes the extension of the location.
        /// </summary>
        /// <param name="location">The location whose extension is to be changed.</param>
        /// <param name="extension">The new extension.</param>
        /// <returns>A location with the new extension.</returns>
        /// <remarks>Follows the rules of <see cref="System.IO.Path.ChangeExtension(string, string)"/>. Note that if
        /// <paramref name="location"/> refers to a location that causes a change in storage access, the returned
        /// location will use the appropriate storage access.</remarks>
        public static StorageLocation ChangeExtension(this StorageLocation location, string extension)
        {
            var newLocation = location;
            if (location.IsValid)
            {
                var newPath = Path.ChangeExtension(location.Path, extension);
                newLocation = GetStorageLocationForChangedPath(location, newPath);
            }
            return newLocation;
        }

        /// <summary>
        /// Adds a suffix to a location's path.
        /// </summary>
        /// <param name="location">A location whose path will be used to have a suffix appended.</param>
        /// <param name="suffix">The suffix to append.</param>
        /// <returns>A location with a path having the given suffix appended.</returns>
        /// <remarks>No strong validation of the new path is done.</remarks>
        public static StorageLocation AddSuffix(this StorageLocation location, string suffix)
        {
            var updatedLocation = location;
            if (location.IsValid)
            {
                var updatedPath = location.Path + suffix;
                updatedLocation = GetStorageLocationForChangedPath(location, updatedPath);
            }
            return updatedLocation;
        }

        /// <summary>
        /// Gets the containing location of the given location.
        /// </summary>
        /// <param name="location">The location whose containing location is desired.</param>
        /// <returns>A location for the container of the given location.</returns>
        /// <remarks>Follows the rules of <see cref="System.IO.Path.GetDirectoryname(string)"/>.</remarks>
        public static StorageLocation GetContainingLocation(this StorageLocation location)
        {
            var containingLocation = location;
            if (location.IsValid)
            {
                var directory = Path.GetDirectoryName(location.Path);
                containingLocation = GetStorageLocationForChangedPath(location, directory);
            }
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
            var updatedLocation = location;
            if (location.IsValid)
            {
                var fileName = location.GetFileName();
                var newPath = Path.Combine(newContainingLocation, fileName);
                updatedLocation = GetStorageLocationForChangedPath(location, newPath);
            }
            return updatedLocation;
        }

        /// <summary>
        /// Enumerates the entries at the given location.
        /// </summary>
        /// <param name="location">The location at which to enumerate entries.</param>
        /// <returns>The entries at the provided storage location.</returns>
        /// <remarks>This method attempts to seamlessly handle standard file system locations as well as locations within an archive.
        /// Note the following idiosyncrasy: When listing files within an archive, recall that nested archives are treated as
        /// directories on a traditional file system. Thus calling this method in an archive will NOT include nested archives
        /// in the list of returned files.</remarks>
        public static IEnumerable<StorageLocation> EnumerateFiles(this StorageLocation location)
        {
            var files = location.EnumerateFiles(fileExtensionMatch: null);
            return files;
        }

        /// <summary>
        /// Enumerates the entries at the given location.
        /// </summary>
        /// <param name="location">The location at which to enumerate entries.</param>
        /// <param name="fileExtensionMatch">If not empty, only include items with the provided file extension in the results.</param>
        /// <returns>The entries at the provided storage location.</returns>
        /// <remarks>This method attempts to seamlessly handle standard file system locations as well as locations within an archive.
        /// Note the following idiosyncrasy: When listing files within an archive, recall that nested archives are treated as
        /// directories on a traditional file system. Thus, calling this method in an archive location with a file extension
        /// argument of .zip will NOT include those entries!</remarks>
        public static IEnumerable<StorageLocation> EnumerateFiles(this StorageLocation location, string fileExtensionMatch)
        {
            var files = Enumerable.Empty<StorageLocation>();
            if (location.UsesDefaultStorage && Directory.Exists(location.Path))
            {
                files = OSEnumerateFiles(location.Path, fileExtensionMatch).Select(f => new StorageLocation(f, location.StorageAccess));
            }
            else if (location.StorageAccess is ICompressedArchiveAccess)
            {
                var rootArchivePath = string.Empty;
                var pathRelativeToArchive = GetArchiveRelativeDirectoryPath(location.Path, out rootArchivePath);
                if (!string.IsNullOrEmpty(rootArchivePath))
                {
                    var archiveStorage = location.StorageAccess as ICompressedArchiveAccess;
                    if (PathComparer.Instance.Compare(location.Path.NormalizePathSeparators(), rootArchivePath) != 0)
                    {
                        archiveStorage = rootArchivePath.GetStorageAccess() as ICompressedArchiveAccess;
                    }
                    if (archiveStorage != null)
                    {
                        var entries = archiveStorage.ListEntries(pathRelativeToArchive, includeContainers: false, recurse: false);
                        files = GetStorageLocations(rootArchivePath, entries);
                        if (!string.IsNullOrEmpty(fileExtensionMatch))
                        {
                            files = files.Where(f => f.Path.EndsWith(fileExtensionMatch, StringComparison.InvariantCultureIgnoreCase));
                        }
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Copy the specified file to the given destination.
        /// </summary>
        /// <param name="storageLocation">The storage location to copy from.</param>
        /// <param name="destinationLocation">The destination location to copy to.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> has a
        /// zero-length path, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars()"/>,
        /// or <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> specifies a directory.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> has a <c>null</c> path.</exception>
        /// <exception cref="PathTooLongException">Thrown if the specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the path specified in <paramref name="storageLocation"/> or <paramref name="destinationLocation"/>
        /// is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="storageLocation"/> was not found.</exception>
        /// <exception cref="IOException">Thrown if <paramref name="destinationLocation"/> exists, or an I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> is
        /// is an invalid format, or if <paramref name="destinationLocation"/> is in an archive.</exception>
        /// <remarks>At this time, this method does not support copying a file into a location that is within an <see cref="ICompressedArchiveAccess"/> instance.</remarks>
        public static void CopyFile(this StorageLocation storageLocation, StorageLocation destinationLocation)
        {
            storageLocation.CopyFile(destinationLocation, overwrite: false);
        }

        /// <summary>
        /// Copy the specified file to the given destination, optionally overwriting the existing file.
        /// </summary>
        /// <param name="storageLocation">The storage location to copy from.</param>
        /// <param name="destinationLocation">The destination location to copy to.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrite an existing file at the destination location.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> has a
        /// zero-length path, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars()"/>,
        /// or <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> specifies a directory.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> has a <c>null</c> path.</exception>
        /// <exception cref="PathTooLongException">Thrown if the specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the path specified in <paramref name="storageLocation"/> or <paramref name="destinationLocation"/>
        /// is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="storageLocation"/> was not found.</exception>
        /// <exception cref="IOException">Thrown if <paramref name="destinationLocation"/> exists and <paramref name="overwrite"/> is <c>false</c>, or an I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="storageLocation"/> or <paramref name="destinationLocation"/> is
        /// is an invalid format, or if <paramref name="destinationLocation"/> is in an archive.</exception>
        /// <remarks>At this time, this method does not support copying a file into a location that is within an <see cref="ICompressedArchiveAccess"/> instance.</remarks>
        public static void CopyFile(this StorageLocation storageLocation, StorageLocation destinationLocation, bool overwrite)
        {
            if (storageLocation == null)
            {
                throw new ArgumentNullException("storageLocation");
            }
            if (storageLocation.IsInvalid)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.StorageArgumentPathIsInvalid, "storageLocation");
                throw new InvalidOperationException(errorMessage);
            }
            if (!storageLocation.IsValid)
            {
                throw new ArgumentException("storageLocation");
            }
            if (!Path.IsPathRooted(storageLocation.Path))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.PathMustBeRooted_Format, storageLocation);
                throw new ArgumentException("storageLocation", errorMessage);
            }
            if (storageLocation.IsContainer)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.PathDoesNotReferToAFile_Format, storageLocation);
                throw new ArgumentException("storageLocation", errorMessage);
            }
            if (destinationLocation == null)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.PathContainsInvalidCharacters_Format, destinationLocation);
                throw new ArgumentNullException("destinationLocation");
            }
            if (destinationLocation.IsInvalid)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.StorageArgumentPathIsInvalid, "destinationLocation");
                throw new InvalidOperationException(errorMessage);
            }
            if (!destinationLocation.IsValid)
            {
                throw new ArgumentException("destinationLocation");
            }
            if (!Path.IsPathRooted(destinationLocation.Path))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.PathMustBeRooted_Format, destinationLocation);
                throw new ArgumentException("destinationLocation", errorMessage);
            }
            if (destinationLocation.StorageAccess is ICompressedArchiveAccess)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.DestinationPathIsInArchive_Format, destinationLocation);
                throw new NotSupportedException(errorMessage);
            }

            if (storageLocation.UsesDefaultStorage)
            {
                File.Copy(storageLocation.Path, destinationLocation.Path, overwrite);
            }
            else
            {
                var archiveAccess = storageLocation.StorageAccess as ICompressedArchiveAccess;
                if (archiveAccess != null)
                {
                    var entry = archiveAccess.FindEntry(storageLocation.Path);
                    archiveAccess.ExtractEntry(entry, destinationLocation.Path, overwrite);
                }
            }
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
                var normalizedPath = location.Path.NormalizePathSeparators();
                normalizedLocation = StorageLocation.CopyWithNewPath(location, normalizedPath);
            }
            return normalizedLocation;
        }

        #endregion // PathUtils Behaviors

        /// <summary>
        /// Given an absolute path, get the archive-relative directory path and root archive path for a location within an archive.
        /// </summary>
        /// <param name="path">An absolute path that includes additional location within an archive.</param>
        /// <param name="rootArchivePath">Receives the root archive path.</param>
        /// <returns>The archive-relative directory path.</returns>
        /// <remarks>The intent of this function is, given a path to a location within an archive, to extract the portion of
        /// the path that is relative to the root archive, and the root archive path itself.  For example, given the following
        /// Windows-style path:
        /// C:/sub/directory/archive.zip/path/within/archive/excellent_game.rom
        /// the returned archive-relative path would be:
        /// path/within/archive/excellent_game.rom
        /// and the root archive path would be:
        /// C:/sub/directory/archive.zip
        /// </remarks>
        private static string GetArchiveRelativeDirectoryPath(this string path, out string rootArchivePath)
        {
            // A null or empty value indicates path is not within an archive.
            string archiveRelativeDirectory = null;
            rootArchivePath = path.GetMostDeeplyNestedArchivePath();
            if (!string.IsNullOrEmpty(rootArchivePath))
            {
                archiveRelativeDirectory = "/";
                if (PathComparer.Instance.Compare(rootArchivePath, path.NormalizePathSeparators()) != 0)
                {
                    archiveRelativeDirectory = PathUtils.GetRelativePath(path, rootArchivePath).NormalizePathSeparators();
                    if (archiveRelativeDirectory.Last() != '/')
                    {
                        archiveRelativeDirectory += '/';
                    }
                }
            }
            return archiveRelativeDirectory;
        }

        private static IEnumerable<StorageLocation> GetStorageLocations(string rootArchivePath, IEnumerable<ICompressedArchiveEntry> entries)
        {
            IStorageAccess storageAccess = rootArchivePath.GetStorageAccess();
            foreach (var entry in entries)
            {
                var absolutePath = Path.Combine(rootArchivePath, entry.Name);
                var storageLocation = new StorageLocation(absolutePath, storageAccess, entry.IsDirectory);
                yield return storageLocation;
            }
        }

        private static StorageLocation GetStorageLocationForChangedPath(StorageLocation currentLocation, string newPath)
        {
            var newLocation = StorageLocation.InvalidLocation;
            if (DoesNewLocationRequireDifferentStorageAccess(currentLocation, newPath))
            {
                newLocation = newPath.CreateStorageLocationFromPath();
            }
            else
            {
                newLocation = StorageLocation.CopyWithNewPath(currentLocation, newPath);
            }
            return newLocation;
        }

        private static bool DoesNewLocationRequireDifferentStorageAccess(StorageLocation currentLocation, string newPath)
        {
            var currentPath = currentLocation.Path.NormalizePathSeparators();
            newPath = newPath.NormalizePathSeparators();
            if (PathComparer.Instance.Compare(currentPath, newPath) == 0)
            {
                return false;
            }

            // Check if current and new are both null / empty / relative.
            if (!currentLocation.IsValid || !Path.IsPathRooted(currentPath))
            {
                if (string.IsNullOrEmpty(newPath) || !Path.IsPathRooted(newPath))
                {
                    return false; // both are null / empty / relative
                }
                return true; // current is null / empty / relative, but new is absolute
            }

            if (currentLocation.UsesDefaultStorage)
            {
                return !string.IsNullOrEmpty(newPath.GetRootArchivePath()); // new path refers to or is within an archive
            }

            // Current location refers to an archive. Check if new location refers to same one.
            var currentNestedArchivePath = currentPath.GetMostDeeplyNestedArchivePath().NormalizePathSeparators();
            var newNestedArchivePath = newPath.GetMostDeeplyNestedArchivePath().NormalizePathSeparators();
            return PathComparer.Instance.Compare(currentNestedArchivePath, newNestedArchivePath) != 0;
        }
    }
}

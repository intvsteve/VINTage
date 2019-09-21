// <copyright file="ICompressedArchiveAccessExtensions.cs" company="INTV Funhouse">
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
    /// Extension methods to simplify navigating the contents of compressed archives.
    /// </summary>
    public static class ICompressedArchiveAccessExtensions
    {
        /// <summary>
        /// Gets the storage access to use for the given location.
        /// </summary>
        /// <param name="filePath">The absolute file path to get a storage access for.</param>
        /// <returns>The storage access to use to access the given path.</returns>
        /// <remarks>For a file path that refers to a nested archive, e.g. a .zip inside a .tar inside a .gz, an appropriate
        /// storage access will be returned. Note that such a situation may cause extraction of intermediate archive contents
        /// to temporary locations on the disk. In an error condition arises that results in a nested archive failing to be
        /// located, this method will return the default storage access.</remarks>
        public static IStorageAccess GetStorageAccess(this string filePath)
        {
            filePath = filePath.NormalizePathSeparators();
            var storageAccess = IStorageAccessHelpers.DefaultStorage;
            var rootCompressedArchivePath = filePath.GetRootArchivePath().NormalizePathSeparators();
            if (!string.IsNullOrEmpty(rootCompressedArchivePath))
            {
                storageAccess = CompressedArchiveAccess.Open(rootCompressedArchivePath, CompressedArchiveAccessMode.Read);
                var nestedAchiveLocation = filePath.GetMostDeeplyNestedArchivePath();
                if (PathComparer.Instance.Compare(rootCompressedArchivePath, nestedAchiveLocation) != 0)
                {
                    nestedAchiveLocation = PathUtils.GetRelativePath(nestedAchiveLocation, rootCompressedArchivePath).NormalizePathSeparators();
                    var dontCare = string.Empty;
                    storageAccess = GetNestedCompressedArchive(storageAccess as ICompressedArchiveAccess, nestedAchiveLocation, ref dontCare);
                }
                if (storageAccess == null)
                {
                    storageAccess = IStorageAccessHelpers.DefaultStorage;
                }
            }
            return storageAccess;
        }

        /// <summary>
        /// Given a path that may refer to an entry within a compressed archive, locate the path to the archive file on disk.
        /// </summary>
        /// <param name="path">The path that may refer to an entry within a compressed archive.</param>
        /// <returns>The path to the root archive on disk, or <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown if bad characters, et. al.</exception>
        /// <exception cref="PathTooLongException">Thrown if the path is too long. Sad!</exception>
        public static string GetRootArchivePath(this string path)
        {
            string rootArchivePath = null;
            if (!string.IsNullOrEmpty(path) && Path.IsPathRooted(path))
            {
                // First, check the path itself to see looks like something that is an archive. Note that this can
                // produce a false positive if a directory is (pathologically) named like an archive. For example,
                // X:/foo/bar.zip/baz, where bar.zip is a directory, not a ZIP file, will have a non-empty result below.
                path = path.NormalizePathSeparators();
                if (path.IsInNestedContainer())
                {
                    while (!string.IsNullOrEmpty(path) && !File.Exists(path))
                    {
                        path = Path.GetDirectoryName(path);
                    }
                    if (!string.IsNullOrEmpty(path) && File.Exists(path) && path.GetCompressedArchiveFormatsFromFileName().Any())
                    {
                        rootArchivePath = path;
                    }
                }
            }
            return rootArchivePath;
        }

        /// <summary>
        /// Gets the path to the most deeply nested archive from the given path.
        /// </summary>
        /// <param name="filePath">An absolute file path that refers to a location within an existing archive on disk.</param>
        /// <returns>The most deeply nested archive described by <paramref name="filePath"/>.</returns>
        public static string GetMostDeeplyNestedArchivePath(this string filePath)
        {
            filePath = filePath.NormalizePathSeparators();
            var nestedAchiveLocation = filePath.GetRootArchivePath();
            if (!string.IsNullOrEmpty(nestedAchiveLocation))
            {
                string dontCare;
                nestedAchiveLocation = filePath.GetMostDeeplyNestedContainerLocation(out dontCare);
            }
            return nestedAchiveLocation;
        }

        /// <summary>
        /// Gets the parent storage access of the given compressed archive access.
        /// </summary>
        /// <param name="storageAccess">The storage access to get the parent storage access of.</param>
        /// <returns>The parent storage access.</returns>
        public static IStorageAccess GetParentStorageAccess(this IStorageAccess storageAccess)
        {
            var parentStorageAccess = IStorageAccessHelpers.DefaultStorage;
            var nestedStorageAccess = storageAccess as NestedCompressedArchiveAccess;
            if (nestedStorageAccess != null)
            {
                parentStorageAccess = nestedStorageAccess.ParentArchiveAccess;
            }
            if (parentStorageAccess == null)
            {
                parentStorageAccess = IStorageAccessHelpers.DefaultStorage;
            }
            return parentStorageAccess;
        }

        /// <summary>
        /// List the contents of the given compressed archive.
        /// </summary>
        /// <param name="compressedArchiveAccess">An instance of <see cref="ICompressedArchiveAccess"/> whose contents are to be listed.</param>
        /// <param name="locationInArchive">A location relative to the root of the archive. The special values <c>null</c>, <c>string.Empty</c>, '\', '/', or '.' indicate the root. Otherwise, must end with a directory separator character.</param>
        /// <param name="includeContainers">If <c>true</c>, include entries that may contain other entries, such as other compressed archives and directories.</param>
        /// <returns>The list of entries, which may include entries that could contain more items, depending on the value of <paramref name="includeContainers"/>.</returns>
        /// <remarks>NOTE: Entry names are always relative to <paramref name="compressedArchiveAccess"/>. Path separators will be normalized to forward slash.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="compressedArchiveAccess"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locationInArchive"/> is malformed i.e. is not null or empty, or does not end with a directory separator character.</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="locationInArchive"/> identifies a nested archive that cannot be located.</exception>
        public static IEnumerable<string> ListContents(this ICompressedArchiveAccess compressedArchiveAccess, string locationInArchive, bool includeContainers)
        {
            return compressedArchiveAccess.ListContents(locationInArchive, includeContainers, recurse: false);
        }

        /// <summary>
        /// List the contents of the given compressed archive.
        /// </summary>
        /// <param name="compressedArchiveAccess">An instance of <see cref="ICompressedArchiveAccess"/> whose contents are to be listed.</param>
        /// <param name="locationInArchive">A location relative to the root of the archive. The special values <c>null</c>, <c>string.Empty</c>, '\', '/', or '.' indicate the root. Otherwise, must end with a directory separator character.</param>
        /// <param name="includeContainers">If <c>true</c>, include entries that may contain other entries, such as other compressed archives and directories.</param>
        /// <param name="recurse">If <c>true</c>, list all contents from <paramref name="locationInArchive"/> and below, recursively. The contents of nested archives will also be listed.</param>
        /// <returns>The list of entries, which may include entries that could contain more items, depending on the value of <paramref name="includeContainers"/>.
        /// Entry names are always relative to <paramref name="compressedArchiveAccess"/>. Path separators will be normalized to forward slash.</returns>
        /// <remarks>NOTE: Large and /or deeply nested archives may incur performance and disk penalties. Use with care!</remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="compressedArchiveAccess"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locationInArchive"/> is malformed i.e. is not null or empty, or does not end with a directory separator character.</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="locationInArchive"/> identifies a nested archive that cannot be located.</exception>
        public static IEnumerable<string> ListContents(this ICompressedArchiveAccess compressedArchiveAccess, string locationInArchive, bool includeContainers, bool recurse)
        {
            var entries = compressedArchiveAccess.ListEntries(locationInArchive, includeContainers, recurse);
            var contents = entries.Select(e => e.Name);
            return contents;
        }

        /// <summary>
        /// List the contents of the given compressed archive.
        /// </summary>
        /// <param name="compressedArchiveAccess">An instance of <see cref="ICompressedArchiveAccess"/> whose contents are to be listed.</param>
        /// <param name="locationInArchive">A location relative to the root of the archive. The special values <c>null</c>, <c>string.Empty</c>, '\', '/', or '.' indicate the root. Otherwise, must end with a directory separator character.</param>
        /// <param name="includeContainers">If <c>true</c>, include entries that may contain other entries, such as other compressed archives and directories.</param>
        /// <returns>The list of entries, which may include entries that could contain more items, depending on the value of <paramref name="includeContainers"/>.</returns>
        /// <remarks>NOTE: Entry names are always relative to <paramref name="compressedArchiveAccess"/>. Path separators will be normalized to forward slash.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="compressedArchiveAccess"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locationInArchive"/> is malformed i.e. is not null or empty, or does not end with a directory separator character.</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="locationInArchive"/> identifies a nested archive that cannot be located.</exception>
        public static IEnumerable<ICompressedArchiveEntry> ListEntries(this ICompressedArchiveAccess compressedArchiveAccess, string locationInArchive, bool includeContainers)
        {
            return compressedArchiveAccess.ListEntries(locationInArchive, includeContainers, recurse: false);
        }

        /// <summary>
        /// List the contents of the given compressed archive.
        /// </summary>
        /// <param name="compressedArchiveAccess">An instance of <see cref="ICompressedArchiveAccess"/> whose contents are to be listed.</param>
        /// <param name="locationInArchive">A location relative to the root of the archive. The special values <c>null</c>, <c>string.Empty</c>, '\', '/', or '.' indicate the root. Otherwise, must end with a directory separator character.</param>
        /// <param name="includeContainers">If <c>true</c>, include entries that may contain other entries, such as other compressed archives and directories.</param>
        /// <param name="recurse">If <c>true</c>, list all contents from <paramref name="locationInArchive"/> and below, recursively. The contents of nested archives will also be listed.</param>
        /// <returns>The list of entries, which may include entries that could contain more items, depending on the value of <paramref name="includeContainers"/>.
        /// Entry names are always relative to <paramref name="compressedArchiveAccess"/>. Path separators will be normalized to forward slash.</returns>
        /// <remarks>NOTE: Large and / or deeply nested archives may incur performance and disk penalties. Use with care!</remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="compressedArchiveAccess"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="locationInArchive"/> is malformed i.e. is not null or empty, or does not end with a directory separator character.</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="locationInArchive"/> identifies a nested archive that cannot be located.</exception>
        public static IEnumerable<ICompressedArchiveEntry> ListEntries(this ICompressedArchiveAccess compressedArchiveAccess, string locationInArchive, bool includeContainers, bool recurse)
        {
            var entries = new List<ICompressedArchiveEntry>(ListEntriesInCompressedArchive(compressedArchiveAccess, locationInArchive, includeContainers || recurse));
            if (recurse)
            {
                // Uses a simple queue to process nested contents in a breadth-first-ish fashion.
                // Nested archives are kept around as discovered so subsequent listings that may cause actual extraction to temporary
                // locations on disk is somewhat mitigated.
                var nestedCompressedArchives = new Dictionary<string, ICompressedArchiveAccess>();
                var containers = new Queue<ICompressedArchiveEntry>(entries.Where(e => e.IsDirectory || e.Name.IsContainer()));
                while (containers.Any())
                {
                    var container = containers.Dequeue();
                    ICompressedArchiveAccess nestedCompressedArchiveAccess;
                    string nestedArchiveRelativeLocation;
                    var nestedAchiveLocation = container.Name.GetMostDeeplyNestedContainerLocation(out nestedArchiveRelativeLocation);
                    if (nestedAchiveLocation != null)
                    {
                        if (!nestedCompressedArchives.TryGetValue(nestedAchiveLocation, out nestedCompressedArchiveAccess))
                        {
                            var dontCare = string.Empty;
                            nestedCompressedArchiveAccess = GetNestedCompressedArchive(compressedArchiveAccess, nestedAchiveLocation, ref dontCare);
                            nestedCompressedArchives[nestedAchiveLocation] = nestedCompressedArchiveAccess;
                        }
                    }
                    else
                    {
                        nestedCompressedArchiveAccess = compressedArchiveAccess;
                    }

                    var childEntries = ListEntriesInCompressedArchive(nestedCompressedArchiveAccess, nestedArchiveRelativeLocation, includeContainers: true).Select(e => e.MakeAbsoluteEntry(nestedAchiveLocation));
                    entries.AddRange(childEntries);
                    var childContainers = childEntries.Where(e => e.IsDirectory || e.Name.IsContainer());
                    foreach (var childContainer in childContainers)
                    {
                        containers.Enqueue(childContainer);
                    }
                }
                if (!includeContainers)
                {
                    entries = entries.Where(e => !e.IsDirectory && !e.Name.IsContainer()).ToList();
                }
                foreach (var nestedCompresedArchive in nestedCompressedArchives.Values)
                {
                    nestedCompresedArchive.Dispose();
                }
            }
            return entries.OrderBy(e => e.Name);
        }

        private static IEnumerable<ICompressedArchiveEntry> ListEntriesInCompressedArchive(ICompressedArchiveAccess compressedArchiveAccess, string locationInArchive, bool includeContainers)
        {
            if (compressedArchiveAccess == null)
            {
                throw new ArgumentNullException("compressedArchiveAccess");
            }
            if (!string.IsNullOrEmpty(locationInArchive) && (locationInArchive != "."))
            {
                if ((locationInArchive.Last() != '\\') && (locationInArchive.Last() != '/'))
                {
                    throw new ArgumentException("locationInArchive");
                }
            }

            // If location is within a nested archive, do what is necessary to get access to the nested archive from the supplied archive.
            var originalArchiveAccess = compressedArchiveAccess;
            locationInArchive = locationInArchive.NormalizePathSeparators();
            var normalizedLocationInArchive = locationInArchive;
            var locationIsInNestedContainer = locationInArchive.IsInNestedContainer();
            if (locationIsInNestedContainer)
            {
                normalizedLocationInArchive = string.Empty;
                compressedArchiveAccess = GetNestedCompressedArchive(compressedArchiveAccess, locationInArchive, ref normalizedLocationInArchive);
                if (compressedArchiveAccess == null)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_NestedArchiveNotFound, locationInArchive);
                    throw new FileNotFoundException(message, locationInArchive);
                }
            }

            var entries = compressedArchiveAccess.Entries.Select(e => new NormalizedCompressedArchiveEntry(e, normalizedLocationInArchive, locationIsInNestedContainer));
            if (!includeContainers)
            {
                entries = entries.Where(e => !e.IsDirectory && !e.Name.IsContainer());
            }

            if (locationInArchive.IsRootLocation())
            {
                var virtualEntriesToAdd = new List<NormalizedCompressedArchiveEntry>();
                if (includeContainers)
                {
                    // When listing, there are cases in which there may no direct entry for a directory. Add one.
                    var entryNames = entries.Select(e => e.Name).ToList();
                    var nestedEntryNames = entryNames.Select(n => n.GetArchiveRelativePathSegments()).Where(s => s.Length > 1).Select(s => s.First() + '/').Distinct().ToList();
                    nestedEntryNames.RemoveAll(n => entryNames.Contains(n));
                    virtualEntriesToAdd.AddRange(nestedEntryNames.Select(n => new NormalizedCompressedArchiveEntry(n, locationInArchive, locationIsInNestedContainer)));
                }
                entries = entries.Where(e => e.Name.GetArchiveRelativePathSegments().Length < 2);
                entries = entries.Concat(virtualEntriesToAdd);
            }
            else
            {
                entries = entries.Where(e => e.Name.StartsWith(locationInArchive, PathComparer.DefaultPolicy) && (e.Name.Length > locationInArchive.Length));
                entries = entries.Where(e => e.Name.Substring(locationInArchive.Length + 1).GetArchiveRelativePathSegments().Length < 2);
            }

            if (!object.ReferenceEquals(compressedArchiveAccess, originalArchiveAccess))
            {
                compressedArchiveAccess.Dispose();
            }

            return entries;
        }

        private static NestedCompressedArchiveAccess GetNestedCompressedArchive(ICompressedArchiveAccess compressedArchiveAccess, string locationInArchive, ref string nestedArchiveLocation)
        {
            var nestedArchiveLocations = locationInArchive.GetNestedContainerLocations();
            var entryName = nestedArchiveLocations.First();
            if (string.IsNullOrEmpty(nestedArchiveLocation))
            {
                nestedArchiveLocation = entryName + '/';
            }
            else
            {
                nestedArchiveLocation += entryName + '/';
            }
            var nestedArchiveEntry = compressedArchiveAccess.Entries.FirstOrDefault(e => e.Name.NormalizePathSeparators() == entryName);

            NestedCompressedArchiveAccess nestedCompressedArchive = null;
            if (nestedArchiveEntry != null)
            {
                nestedCompressedArchive = NestedCompressedArchiveAccess.Create(compressedArchiveAccess, nestedArchiveEntry);
                if (nestedCompressedArchive != null)
                {
                    // If there is more than one nested archive format on the location, e.g. it's a .tar.gz, or if we did not consume the entire location
                    // in the archive, then we need to recurse into this newly located archive to get to the ultimate nested archive.
                    if ((nestedCompressedArchive.NestedArchiveFormats.Count() > 1) || (nestedArchiveLocations.Length > 1))
                    {
                        locationInArchive = string.Join("/", nestedArchiveLocations, 1, nestedArchiveLocations.Length - 1) + '/';
                        if (locationInArchive.IsInNestedContainer())
                        {
                            nestedCompressedArchive = GetNestedCompressedArchive(nestedCompressedArchive, locationInArchive, ref nestedArchiveLocation);
                        }
                    }
                }
            }

            return nestedCompressedArchive;
        }

        private static bool IsRootLocation(this string location)
        {
            var isRootLocation = string.IsNullOrEmpty(location) || (location == ".") || (location == "/") || (location == @"\");
            return isRootLocation;
        }

        private static bool IsContainer(this string entryName)
        {
            var formats = entryName.GetCompressedArchiveFormatsFromFileName();
            var allFormatsSupported = formats.Any() && formats.All(f => f.IsCompressedArchiveFormatSupported());
            return allFormatsSupported;
        }

        private static bool IsInNestedContainer(this string location)
        {
            var segments = location.GetArchiveRelativePathSegments();
            var anySegmentIsCompressedArchive = segments.Any(s => s.GetCompressedArchiveFormatsFromFileName().Any());
            return anySegmentIsCompressedArchive;
        }

        private static string GetMostDeeplyNestedContainerLocation(this string location, out string nestedArchiveRelativeLocation)
        {
            nestedArchiveRelativeLocation = null;
            string mostDeeplyNestedLocation = null;
            var segments = location.GetArchiveRelativePathSegments();
            var lastArchiveSegmentIndex = Array.FindLastIndex(segments, s => s.GetCompressedArchiveFormatsFromFileName().Any());
            if (lastArchiveSegmentIndex >= 0)
            {
                mostDeeplyNestedLocation = string.Join("/", segments, 0, ++lastArchiveSegmentIndex) + "/";
                if (lastArchiveSegmentIndex < segments.Length)
                {
                    nestedArchiveRelativeLocation = string.Join("/", segments, lastArchiveSegmentIndex, segments.Length - lastArchiveSegmentIndex) + "/";
                }
            }
            else
            {
                nestedArchiveRelativeLocation = location;
            }
            return mostDeeplyNestedLocation;
        }

        private static string[] GetNestedContainerLocations(this string location)
        {
            var nestedContainerLocations = new List<string>();
            var segments = GetArchiveRelativePathSegments(location);
            var offset = 0;
            do
            {
                var indexOfFirstArchiveSegment = Array.FindIndex(segments, offset, s => s.GetCompressedArchiveFormatsFromFileName().Any());
                if (indexOfFirstArchiveSegment >= 0)
                {
                    var nestedContainerLocation = string.Join("/", segments, offset, ++indexOfFirstArchiveSegment - offset);
                    nestedContainerLocations.Add(nestedContainerLocation);
                }
                offset = indexOfFirstArchiveSegment;
            }
            while (offset >= 0);
            return nestedContainerLocations.ToArray();
        }

        private static string[] GetArchiveRelativePathSegments(this string archivePath)
        {
            var segments = string.IsNullOrEmpty(archivePath) ? Enumerable.Empty<string>().ToArray() : archivePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return segments;
        }

        private static ICompressedArchiveEntry MakeAbsoluteEntry(this ICompressedArchiveEntry entry, string location)
        {
            ((NormalizedCompressedArchiveEntry)entry).MakeAbsoluteEntryName(location);
            return entry;
        }

        /// <summary>
        /// This wrapper ensures that the temporary location that a nested archive is extracted to can be
        /// properly cleaned up upon disposal of the nested archive.
        /// </summary>
        private class NestedCompressedArchiveAccess : CompressedArchiveAccess
        {
            private NestedCompressedArchiveAccess(ICompressedArchiveAccess parentArchiveAccess, ICompressedArchiveAccess nestedArchiveAccess, TemporaryDirectory temporaryLocation)
            {
                ParentArchiveAccess = parentArchiveAccess;
                NestedAchiveAccess = nestedArchiveAccess;
                TemporaryLocation = temporaryLocation;
            }

            /// <inheritdoc />
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public override bool IsArchive
            {
                get { return NestedAchiveAccess.IsArchive; }
            }

            /// <inheritdoc />
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public override bool IsCompressed
            {
                get { return NestedAchiveAccess.IsCompressed; }
            }

            /// <inheritdoc />
            public override CompressedArchiveFormat Format
            {
                get { return NestedAchiveAccess.Format; }
            }

            /// <inheritdoc />
            public override IEnumerable<ICompressedArchiveEntry> Entries
            {
                get { return NestedAchiveAccess.Entries; }
            }

            public IEnumerable<CompressedArchiveFormat> NestedArchiveFormats { get; private set; }

            public ICompressedArchiveAccess ParentArchiveAccess { get; set; }

            private ICompressedArchiveAccess NestedAchiveAccess { get; set; }

            private TemporaryDirectory TemporaryLocation { get; set; }

            /// <summary>
            /// Creates an instance of <see cref="ICompressedArchiveAccess"/> that can be used to access a nested archive.
            /// </summary>
            /// <param name="parentArchiveAccess">The parent archive, which contains the nested archive indicated by <paramref name="entry"/>.</param>
            /// <param name="entry">The entry within <paramref name="parentArchiveAccess"/> that indicates a nested archive.</param>
            /// <returns>An instance of <see cref="NestedCompressedArchiveAccess"/>, which provides access to the nested archive.</returns>
            /// <remarks>This wrapper type takes care of cleaning up any temporary files that may be required to access the nested archive.
            /// For example, some data streams used to access an archive are not navigable from the parent stream (GZIP).</remarks>
            public static NestedCompressedArchiveAccess Create(ICompressedArchiveAccess parentArchiveAccess, ICompressedArchiveEntry entry)
            {
                NestedCompressedArchiveAccess nestedCompressedArchive = null;
                var entryName = entry.Name;
                var nestedArchiveFormats = Path.GetExtension(entryName).GetCompressedArchiveFormatsFromFileExtension();
                var nestedArchiveFormat = nestedArchiveFormats.FirstOrDefault();

                if (nestedArchiveFormat.IsCompressedArchiveFormatSupported())
                {
                    TemporaryDirectory temporaryLocation = null;
                    var entryData = parentArchiveAccess.OpenEntry(entry);

                    // always extract nested archives??
                    if (FormatMustBeExtracted(parentArchiveAccess.Format) || FormatMustBeExtracted(nestedArchiveFormat))
                    {
                        // We can't fully navigate the nested stream, so extract to disk, then proceed.
                        temporaryLocation = new TemporaryDirectory();
                        var temporaryEntryFilePath = Path.Combine(temporaryLocation.Path, entryName);
                        if (Directory.CreateDirectory(Path.GetDirectoryName(temporaryEntryFilePath)).Exists)
                        {
                            System.Diagnostics.Debug.WriteLine("Extracted entry " + entryName + " to " + temporaryLocation.Path);
                            var fileStream = new FileStream(temporaryEntryFilePath, FileMode.Create, FileAccess.ReadWrite);
                            entryData.CopyTo(fileStream);
                            fileStream.Seek(0, SeekOrigin.Begin);
                            entryData.Dispose();
                            entryData = fileStream;
                        }
                    }

                    var compressedArchive = CompressedArchiveAccess.Open(entryData, nestedArchiveFormat, CompressedArchiveAccessMode.Read);
                    nestedCompressedArchive = new NestedCompressedArchiveAccess(parentArchiveAccess, compressedArchive, temporaryLocation);
                    nestedCompressedArchive.NestedArchiveFormats = nestedArchiveFormats;
                }

                return nestedCompressedArchive;
            }

            /// <inheritdoc />
            public override Stream OpenEntry(ICompressedArchiveEntry entry)
            {
                return NestedAchiveAccess.OpenEntry(entry);
            }

            /// <inheritdoc />
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public override ICompressedArchiveEntry CreateEntry(string name)
            {
                return NestedAchiveAccess.CreateEntry(name);
            }

            /// <inheritdoc />
            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            protected override bool DeleteEntry(ICompressedArchiveEntry entry)
            {
                return NestedAchiveAccess.DeleteEntry(entry.Name);
            }

            /// <inheritdoc />
            protected override void DisposeCore(bool disposing)
            {
                if (NestedAchiveAccess != null)
                {
                    var nestedAchiveAccess = NestedAchiveAccess;
                    NestedAchiveAccess = null;
                    if (nestedAchiveAccess != null)
                    {
                        nestedAchiveAccess.Dispose();
                    }
                }

                if (ParentArchiveAccess != null)
                {
                    var parentArchiveAccess = ParentArchiveAccess;
                    ParentArchiveAccess = null;
                    if (parentArchiveAccess != null)
                    {
                        if (parentArchiveAccess is NestedCompressedArchiveAccess)
                        {
                            parentArchiveAccess.Dispose();
                        }
                    }
                }

                if (TemporaryLocation != null)
                {
                    var temporaryLocation = TemporaryLocation;
                    TemporaryLocation = null;
                    if (temporaryLocation != null)
                    {
                        temporaryLocation.Dispose();
                    }
                }
            }

            private static bool FormatMustBeExtracted(CompressedArchiveFormat format)
            {
                // Perhaps we should always extract nested archives??
                var requiresExtraction = false;
                switch (format)
                {
                    case CompressedArchiveFormat.GZip:
                    case CompressedArchiveFormat.Tar:
                        // GZIP, TAR streams must be extracted.
                        requiresExtraction = true;
                        break;
                    default:
                        break;
                }
                return requiresExtraction;
            }
        }

        /// <summary>
        /// Wraps a <see cref="CompressedArchiveEntry"/> and presents it with a normalized entry name.
        /// </summary>
        private class NormalizedCompressedArchiveEntry : CompressedArchiveEntry
        {
            /// <summary>
            /// Wraps an instance of <see cref="ICompressedArchiveEntry"/> with a standardized name.
            /// </summary>
            /// <param name="originalEntry">The wrapped entry.</param>
            /// <param name="locationInArchive">The location within an archive used to make an 'absolute' path for nested archive item access.</param>
            /// <param name="locationIsInNestedContainer">If <c>true</c>, indicates that the wrapped entry is within a nested archive and that
            /// <paramref name="locationInArchive"/> should be prepended to the entry's name.</param>
            public NormalizedCompressedArchiveEntry(ICompressedArchiveEntry originalEntry, string locationInArchive, bool locationIsInNestedContainer)
                : this(originalEntry.Name, locationInArchive, locationIsInNestedContainer)
            {
                Original = originalEntry;
            }

            /// <summary>
            /// Creates a 'virtual' entry with the given name. All such entries are treated as directories.
            /// </summary>
            /// <param name="name">The name of the entry.</param>
            /// <param name="locationInArchive">The location within an archive used to make an 'absolute' path for nested archive item access.</param>
            /// <param name="locationIsInNestedContainer">If <c>true</c>, indicates that the wrapped entry is within a nested archive and that
            /// <paramref name="locationInArchive"/> should be prepended to the entry's name.</param>
            /// <remarks>This constructor is used to create virtual 'directory' entries when listing nested archive contents. For example, ZIP archives
            /// may not contain explicit entries for directories. These virtual entries are useful when recursively listing the contents of archives.</remarks>
            public NormalizedCompressedArchiveEntry(string name, string locationInArchive, bool locationIsInNestedContainer)
            {
                var prefix = locationIsInNestedContainer ? locationInArchive : string.Empty;
                _name = prefix + name.NormalizePathSeparators();
            }

            /// <inheritdoc />
            public override string Name
            {
                get { return _name; }
            }
            private string _name;

            /// <inheritdoc />
            public override long Length
            {
                get { return Original == null ? -1 : Original.Length; }
            }

            /// <inheritdoc />
            public override DateTime LastModificationTime
            {
                get { return Original == null ? DateTime.MinValue : Original.LastModificationTime; }
            }

            /// <inheritdoc />
            public override bool IsDirectory
            {
                get { return Original == null ? true : Original.IsDirectory; }
            }

            public ICompressedArchiveEntry Original { get; private set; }

            public void MakeAbsoluteEntryName(string prefix)
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    _name = prefix + _name;
                }
            }
        }
    }
}

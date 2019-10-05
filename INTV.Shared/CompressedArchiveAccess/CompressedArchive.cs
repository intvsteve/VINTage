// <copyright file="CompressedArchiveAccess.cs" company="INTV Funhouse">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using INTV.Shared.Utility;

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// This delegate defines the factory method for creating an instance of <see cref="ICompressedArchiveAccess"/>.
    /// </summary>
    /// <param name="stream">The stream from which to create the instance.</param>
    /// <param name="mode">The access mode requested for the compressed archive.</param>
    /// <returns>An instance of <see cref="ICompressedArchiveAccess"/>, or <c>null</c> if creation fails.</returns>
    /// <remarks>Note that ownership of <paramref name="stream"/> is not strongly defined at this time. Caveat emptor.
    /// Also, some implementations may throw exceptions. Typical exceptions to plan for are:
    /// <see cref="System.ArgumentException"/>, <see cref="System.ArgumentNullException"/>, <see cref="System.ArgumentOutOfRangeException"/>,
    /// <see cref="System.InvalidOperationException"/>, <see cref="System.IO.FileFormatException"/>, <see cref="System.NotSupportedException"/> and <see cref="System.IO.IOException"/>.</remarks>
    public delegate ICompressedArchiveAccess CompressedArchiveAccessFactory(Stream stream, CompressedArchiveAccessMode mode);

    /// <summary>
    /// This class provides a partial implementation of <see cref="ICompressedArchiveAccess"/> and static helpers for factory method registration
    /// and creating an instance from a file.
    /// </summary>
    public abstract partial class CompressedArchive : ICompressedArchiveAccess
    {
        // TODO: Consider a cache of instances of compressed archive access instances based on path?
        private static readonly Lazy<ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory>> Factories = new Lazy<ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory>>(InitializeCompressedArchiveFactories);

        ~CompressedArchive()
        {
            Dispose(false);
        }

        /// <summary>
        /// Register a factory method for a specific implementation and type of compressed archive access type.
        /// </summary>
        /// <param name="format">The format for which the factory is being registered.</param>
        /// <param name="implementation">The implementation for which the factory is being registered.</param>
        /// <param name="factory">The factory method.</param>
        /// <returns><c>true</c> if the factory was successfully registered; <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the value of <paramref name="format"/> or <paramref name="implementation"/> is not valid.</exception>
        public static bool RegisterFactory(CompressedArchiveFormat format, CompressedArchiveImplementation implementation, CompressedArchiveAccessFactory factory)
        {
            if (format == CompressedArchiveFormat.None)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_InvalidCompressionFormatTypeErrorMessage_Format, format);
                throw new ArgumentOutOfRangeException("format", message);
            }
            if ((implementation == CompressedArchiveImplementation.None) || (implementation == CompressedArchiveImplementation.Any))
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_InvalidCompressionImplementationTypeErrorMessage_Format, implementation, format);
                throw new ArgumentOutOfRangeException("implementation", message);
            }

            var registered = Factories.Value.TryAdd(new CompressedArchiveIdentifier(format, implementation), factory);
            return registered;
        }

        /// <summary>
        /// Opens or creates an instance of <see cref="ICompressedArchiveAccess"/> based on the value of <paramref name="mode"/> from a file.
        /// </summary>
        /// <param name="filePath">The absolute path for the compressed archive.</param>
        /// <param name="mode">The access mode to use for operations on the compressed archive.</param>
        /// <param name="implementation">If not <c>null</c>, use a specific implementation if possible. Otherwise, use default, or any.</param>
        /// <returns>An instance of the compressed archive.</returns>
        /// <remarks>The format of the compressed archive accessor is determined via file extension.</remarks>
        /// <exception cref="System.NotSupportedException">Thrown if it is not possible to locate a factory for the given <paramref name="filePath"/>, or
        /// if <paramref name="filePath"/> was opened with an unsupported file sharing mode in use.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="filePath"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an invalid combination of file access and mode is used.</exception>
        /// <exception cref="System.ArgumentException">Thrown if invalid file access, sharing, and mode combinations are used.</exception>
        /// <exception cref="System.IO.FileFormatException">Thrown if archive was opened for reading, but is of zero size.</exception>
        /// <exception cref="System.IO.IOException">Thrown if <paramref name="filePath"/> is not empty and archive was opened in Create mode.</exception>
        public static ICompressedArchiveAccess Open(string filePath, CompressedArchiveAccessMode mode, CompressedArchiveImplementation? implementation = null)
        {
            var archive = CompressedArchiveFile.Create(filePath, mode, implementation);
            return archive;
        }

        /// <summary>
        /// Opens or creates an instance of <see cref="ICompressedArchiveAccess"/> based on the value of <paramref name="mode"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream to use for the compressed archive accessor.</param>
        /// <param name="format">The format of the compressed archive.</param>
        /// <param name="mode">The access mode to use for operations on the compressed archive.</param>
        /// <param name="implementation">If not <c>null</c>, use a specific implementation if possible. Otherwise, use default, or any.</param>
        /// <returns>An instance of a compressed archive accessor for the given format.</returns>
        /// <remarks>The archive takes ownership of <paramref name="stream"/> and will dispose it.</remarks>
        /// <exception cref="System.NotSupportedException">Thrown if it is not possible to locate a factory for the given <paramref name="format"/>, or
        /// if <paramref name="stream"/> was opened with an unsupported file sharing mode in use.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an invalid combination of file access and mode is used.</exception>
        /// <exception cref="System.ArgumentException">Thrown if invalid file access, sharing, and mode combinations are used.</exception>
        /// <exception cref="System.IO.FileFormatException">Thrown if archive was opened for reading, but is of zero size.</exception>
        /// <exception cref="System.IO.IOException">Thrown if <paramref name="stream"/> is not empty and archive was opened in Create mode.</exception>
        public static ICompressedArchiveAccess Open(Stream stream, CompressedArchiveFormat format, CompressedArchiveAccessMode mode, CompressedArchiveImplementation? implementation = null)
        {
            if (!implementation.HasValue)
            {
                implementation = format.GetPreferredCompressedArchiveImplementation();
            }
            var identifier = new CompressedArchiveIdentifier(format, implementation.Value);

            CompressedArchiveAccessFactory factory;
            if (!Factories.Value.TryGetValue(identifier, out factory))
            {
                identifier = new CompressedArchiveIdentifier(format, CompressedArchiveImplementation.Any);
                Factories.Value.TryGetValue(identifier, out factory);
            }

            if (factory == null)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_UnsupportedFormatErrorMessage_Format, format);
                throw new NotSupportedException(message);
            }
            var archive = factory(stream, mode);
            return archive;
        }

        /// <summary>
        /// Gets or sets the root file system location of the compressed archive.
        /// </summary>
        /// <remarks>If <c>null</c>, the compressed archive accessor was created directly from a <see cref="Stream"/> and does not have a file system location.</remarks>
        public string RootLocation { get; protected set; }

        #region ICompressedArchiveAccess

        /// <inheritdoc />
        public abstract bool IsArchive { get; }

        /// <inheritdoc />
        public abstract bool IsCompressed { get; }

        /// <inheritdoc />
        public abstract CompressedArchiveFormat Format { get; }

        /// <inheritdoc />
        public abstract IEnumerable<ICompressedArchiveEntry> Entries { get; }

        /// <inheritdoc />
        public virtual ICompressedArchiveEntry FindEntry(string name)
        {
            var entry = GetEntry(name);
            return entry;
        }

        /// <inheritdoc />
        public abstract Stream OpenEntry(ICompressedArchiveEntry entry);

        /// <inheritdoc />
        public abstract ICompressedArchiveEntry CreateEntry(string name);

        /// <inheritdoc />
        public bool DeleteEntry(string name)
        {
            var entry = GetEntry(name);
            var removedEntry = entry != null;
            if (removedEntry)
            {
                removedEntry = DeleteEntry(entry);
            }
            return removedEntry;
        }

           //Consider also using TemporaryFileCollection along with TemporaryDirectory / FileMemo<>

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Thrown if entry is a directory.</exception>
        public void ExtractEntry(ICompressedArchiveEntry entry, string destinationFilePath)
        {
            ExtractEntry(entry, destinationFilePath, overwrite: false);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Thrown if entry is a directory.</exception>
        public void ExtractEntry(ICompressedArchiveEntry entry, string destinationFilePath, bool overwrite)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            if (!destinationFilePath.ValidatePath(Directory.Exists(destinationFilePath), "destinationFilePath"))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.PathContainsInvalidCharacters_Format, destinationFilePath);
                throw new ArgumentException("destinationFilePath", errorMessage);
            }
            if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_ExtractEntry_DestinationDirectoryDoesNotExist_Format, Path.GetDirectoryName(destinationFilePath));
                throw new DirectoryNotFoundException(errorMessage);
            }
            if (!overwrite && File.Exists(destinationFilePath))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CopyFile_DestinationAlreadyExists_Format, destinationFilePath);
                throw new IOException(errorMessage);
            }
            if (entry.IsDirectory)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_ExtractEntry_EntryIsADirectory_Format, entry.Name);
                throw new InvalidOperationException(errorMessage);
            }

            var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
            using (var resourceStream = OpenEntry(entry))
            using (var fileStream = new FileStream(destinationFilePath, fileMode, FileAccess.Write))
            {
                resourceStream.CopyTo(fileStream);
            }
            try
            {
                // Attempt to preserve original modification time.
                File.SetLastWriteTimeUtc(destinationFilePath, entry.LastModificationTime);

                // TODO: Cache the result?
                // TODO: Explore preserving permissions where possible?
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        #endregion // ICompressedArchiveAccess

        #region IStorageAccess

        /// <inheritdoc />
        public Stream Open(string storageLocation)
        {
            Stream stream = null;
            var entry = GetEntry(storageLocation);
            if (entry != null)
            {
                return OpenEntry(entry);
            }
            return stream;
        }

        /// <inheritdoc />
        public bool Exists(string storageLocation)
        {
            var exists = GetEntry(storageLocation) != null;
            return exists;
        }

        /// <inheritdoc />
        public long Size(string storageLocation)
        {
            var size = 0L;
            var entry = GetEntry(storageLocation);
            if (entry != null)
            {
                size = entry.Length;
            }
            return size;
        }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc(string storageLocation)
        {
            var lastWriteTime = DateTime.MinValue;
            var entry = GetEntry(storageLocation);
            if (entry != null)
            {
                lastWriteTime = entry.LastModificationTime;
            }
            return lastWriteTime;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown if <paramref name="storageLocation"/> contains invalid characters.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="storageLocation"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="storageLocation"/> is <c>string.Empty</c>, or
        /// <paramref name="storageLocation"/> is an absolute path, but the archive does not have a root location (e.g. is an in-memory only,
        /// nested archive).</exception>
        /// <remarks>This method does not always verify the existence of <paramref name="storageLocation"/>, only its apparent container-ness.
        /// Depending on whether the archive is nested, or was opened from a disk location directly, can also affect the results.
        /// This method DOES NOT check verify the contents of nested containers if given a path within a nested archive. Generally:
        /// <list type="bullet">
        /// <item><description>If <paramref name="storageLocation"/> ends with a path separator, return <c>true</c></description></item>
        /// <item><description>If <paramref name="storageLocation"/> ends with file extension matching the file extension of a recognized compressed archive format, return <c>true</c></description></item>
        /// <item><description>Otherwise, return <c>false</c></description></item>
        /// </list>
        /// This means that the method may return a value of <c>true</c> for a location that appears to be, or be within, a nested archive which in fact
        /// does not exist.</remarks>
        public bool IsLocationAContainer(string storageLocation)
        {
            var lastCharacter = storageLocation.Last();
            var isLocationAContainer = (lastCharacter == Path.DirectorySeparatorChar) || (lastCharacter == Path.AltDirectorySeparatorChar);
            if (!isLocationAContainer)
            {
                storageLocation = ResolveArchiveRelativeLocation(storageLocation);
                var entry = GetEntry(storageLocation);
                if (entry != null)
                {
                    isLocationAContainer = entry.IsDirectory;
                    if (!entry.IsDirectory)
                    {
                        isLocationAContainer = Path.GetExtension(storageLocation).GetCompressedArchiveFormatsFromFileExtension().Any();
                    }
                }
                else
                {
                    isLocationAContainer = Path.GetExtension(storageLocation).GetCompressedArchiveFormatsFromFileExtension().Any();
                }
            }
            return isLocationAContainer;
        }

        #endregion // IStorageAccess

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable

        /// <summary>
        /// Given an entry name, determine if it is for a directory.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns><c>true</c> if <paramref name="name"/> indicates a directory.</returns>
        protected static bool IsDirectoryName(string name)
        {
            var lastCharacter = name.Last();
            var isDirectory = (lastCharacter == Path.DirectorySeparatorChar) || (lastCharacter == Path.AltDirectorySeparatorChar);
            return isDirectory;
        }

        /// <summary>
        /// Converts a <see cref="CompressedArchiveAccessMode"/> to an appropriate <see cref="FileMode"/>.
        /// </summary>
        /// <param name="mode">A compressed archive access mode.</param>
        /// <returns>The appropriate <see cref="FileMode"/> to use.</returns>
        protected static FileMode CompressedArchiveAccessModeToFileMode(CompressedArchiveAccessMode mode)
        {
            var fileMode = FileMode.Open;
            switch (mode)
            {
                case CompressedArchiveAccessMode.Create:
                    fileMode = FileMode.Create;
                    break;
                case CompressedArchiveAccessMode.Update:
                    fileMode = FileMode.OpenOrCreate;
                    break;
                default:
                    break;
            }
            return fileMode;
        }

        /// <summary>
        /// Converts a <see cref="CompressedArchiveAccessMode"/> to an appropriate <see cref="FileAccess"/>.
        /// </summary>
        /// <param name="mode">A compressed archive access mode.</param>
        /// <returns>The appropriate <see cref="FileAccess"/> to use.</returns>
        protected static FileAccess CompressedArchiveAccessModeToFileAccess(CompressedArchiveAccessMode mode)
        {
            var fileAccess = FileAccess.Read;
            switch (mode)
            {
                case CompressedArchiveAccessMode.Create:
                    fileAccess = FileAccess.Write;
                    break;
                case CompressedArchiveAccessMode.Update:
                    fileAccess = FileAccess.ReadWrite;
                    break;
                default:
                    break;
            }
            return fileAccess;
        }

        /// <summary>
        /// Ensure the base type's underlying resources are correctly disposed.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, Dispose() is being called, otherwise, the finalizer is executing.</param>
        protected virtual void Dispose(bool disposing)
        {
            DisposeCore(disposing);
        }

        /// <summary>
        /// Ensure the implementations underlying resources are correctly disposed.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, Dispose() is being called, otherwise, the finalizer is executing.</param>
        protected abstract void DisposeCore(bool disposing);

        /// <summary>
        /// Deletes an entry from the archive.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        /// <returns><c>true</c> if the entry was removed, <c>false</c> otherwise.</returns>
        protected abstract bool DeleteEntry(ICompressedArchiveEntry entry);

        /// <summary>
        /// Gets the file entry with the given name.
        /// </summary>
        /// <param name="name">The name of the entry within the archive.</param>
        /// <returns>The entry, or <c>null</c> if not found.</returns>
        protected ICompressedArchiveEntry GetEntry(string name)
        {
            if (Path.IsPathRooted(name))
            {
                name = ResolveArchiveRelativeLocation(name);
            }
            var entry = Entries.FirstOrDefault(e => PathComparer.Instance.Compare(e.Name, name) == 0);
            return entry;
        }

        private static ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory> InitializeCompressedArchiveFactories()
        {
            var factories = new ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory>(new CompressedArchiveIdentifier());
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.Zip, CompressedArchiveImplementation.Native)] = ZipArchiveAccess.Create;
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.Zip, CompressedArchiveImplementation.SharpZipLib)] = ZipArchiveAccessSharpZipLib.Create;
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.GZip, CompressedArchiveImplementation.Native)] = GZipArchiveAccessNative.Create;
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.GZip, CompressedArchiveImplementation.SharpZipLib)] = GZipArchiveAccessSharpZipLib.Create;
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.Tar, CompressedArchiveImplementation.SharpZipLib)] = TarArchiveAccessSharpZipLib.Create;
            return factories;
        }

        private string ResolveArchiveRelativeLocation(string storageLocation)
        {
            storageLocation = storageLocation.NormalizePathSeparators();
            if (Path.IsPathRooted(storageLocation))
            {
                var rootLocation = RootLocation;
                if (string.IsNullOrEmpty(rootLocation))
                {
                    rootLocation = storageLocation.GetMostDeeplyNestedArchivePath();
                }
                if (string.IsNullOrEmpty(rootLocation))
                {
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_IsContainerFailed_NoRootLocation_Format, storageLocation);
                    throw new InvalidOperationException(errorMessage);
                }
                if (rootLocation.Length == (storageLocation.Length + 1))
                {
                    // storageLocation refers directly to nested archive, e.g. foo.zip - and a trailing slash as appended.
                    // Remove it. This means the path is actually referring to itself -- the root of a nested archive.
                    if (rootLocation.Last() == '/')
                    {
                        rootLocation = rootLocation.Substring(0, rootLocation.Length - 1);
                    }
                }
                if (!storageLocation.StartsWith(rootLocation.NormalizePathSeparators(), PathComparer.DefaultPolicy))
                {
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_IsContainerFailed_LocationCannotExistInArchive_Format, storageLocation, RootLocation);
                    throw new ArgumentException(errorMessage);
                }
                storageLocation = PathUtils.GetRelativePath(storageLocation, rootLocation);
            }
            return storageLocation;
        }
    }
}

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

////#define OPEN_NESTED_FORMAT_IMMEDIATELY

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace INTV.Shared.Utility
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
    /// <see cref="System.InvalidOperationException"/>, <see cref="System.FileFormatException"/>, <see cref="System.NotSupportedException"/> and <see cref="System.IOException"/>.</remarks>
    public delegate ICompressedArchiveAccess CompressedArchiveAccessFactory(Stream stream, CompressedArchiveAccessMode mode);

    /// <summary>
    /// This class provides a partial implementation of <see cref="ICompressedArchiveAccess"/> and static helpers for factory method registration
    /// and creating an instance from a file.
    /// </summary>
    public abstract class CompressedArchiveAccess : ICompressedArchiveAccess
    {
        private static readonly Lazy<ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory>> Factories = new Lazy<ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory>>(InitializeCompressedArchiveFactories);

        ~CompressedArchiveAccess()
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
        public static bool RegisterFactory(CompressedArchiveFormat format, CompressedArchiveAccessImplementation implementation, CompressedArchiveAccessFactory factory)
        {
            if (format == CompressedArchiveFormat.None)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_InvalidCompressionFormatTypeErrorMessage_Format, format);
                throw new ArgumentOutOfRangeException("format", message);
            }
            if ((implementation == CompressedArchiveAccessImplementation.None) || (implementation == CompressedArchiveAccessImplementation.Any))
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
        /// <exception cref="System.NotSupportedException">Thrown if it is not possible to locate a factory for the given <paramref name="format"/>, or
        /// if <paramref name="stream"/> was opened with an unsupported file sharing mode in use.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an invalid combination of file access and mode is used.</exception>
        /// <exception cref="System.ArgumentException">Thrown if invalid file access, sharing, and mode combinations are used.</exception>
        /// <exception cref="System.FileFormatException">Thrown if archive was opened for reading, but is of zero size.</exception>
        /// <exception cref="System.IOException">Thrown if <paramref name="stream"/> is not empty and archive was opened in Create mode.</exception>
        public static ICompressedArchiveAccess Open(string filePath, CompressedArchiveAccessMode mode, CompressedArchiveAccessImplementation? implementation = null)
        {
            var archive = CompressedArchiveFileAccess.Create(filePath, mode, implementation);
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
        /// <exception cref="System.FileFormatException">Thrown if archive was opened for reading, but is of zero size.</exception>
        /// <exception cref="System.IOException">Thrown if <paramref name="stream"/> is not empty and archive was opened in Create mode.</exception>
        public static ICompressedArchiveAccess Open(Stream stream, CompressedArchiveFormat format, CompressedArchiveAccessMode mode, CompressedArchiveAccessImplementation? implementation = null)
        {
            if (!implementation.HasValue)
            {
                implementation = format.GetPreferredCompressedArchiveImplementation();
            }
            var identifier = new CompressedArchiveIdentifier(format, implementation.Value);

            CompressedArchiveAccessFactory factory;
            if (!Factories.Value.TryGetValue(identifier, out factory))
            {
                identifier = new CompressedArchiveIdentifier(format, CompressedArchiveAccessImplementation.Any);
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
        /// Ensure the implementations underlying resources are correctly disposed.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, Dispose() is being called, otherwise, the finalizer is executing.</param>
        protected abstract void Dispose(bool disposing);

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
            if (!string.IsNullOrEmpty(RootLocation) && Path.IsPathRooted(name))
            {
                name = PathUtils.GetRelativePath(name, RootLocation);
            }
            var entry = Entries.FirstOrDefault(e => PathComparer.Instance.Compare(e.Name, name) == 0);
            return entry;
        }

        private static ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory> InitializeCompressedArchiveFactories()
        {
            var factories = new ConcurrentDictionary<CompressedArchiveIdentifier, CompressedArchiveAccessFactory>(new CompressedArchiveIdentifier());
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.Zip, CompressedArchiveAccessImplementation.Native)] = ZipArchiveAccess.Create;
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.Zip, CompressedArchiveAccessImplementation.SharpZipLib)] = ZipArchiveAccessSharpZipLib.Create;
            factories[new CompressedArchiveIdentifier(CompressedArchiveFormat.GZip, CompressedArchiveAccessImplementation.Native)] = GZipNativeAccess.Create;
            return factories;
        }

        /// <summary>
        /// Wraps a compressed archive accessor created from a file stream.
        /// </summary>
        private class CompressedArchiveFileAccess : CompressedArchiveAccess
        {
            private readonly object _lock = new object();

            /// <summary>
            /// Initializes a new instance of <see cref="CompressedArchiveFileAccess"/>.
            /// </summary>
            /// <param name="filePath">The absolute path to the compressed archive file.</param>
            /// <param name="compressedArchiveAccess">The compressed archive to wrap.</param>
            private CompressedArchiveFileAccess(string filePath, ICompressedArchiveAccess compressedArchiveAccess)
            {
                RootLocation = filePath;
                CompressedArchiveAccess = compressedArchiveAccess;
            }

            /// <inheritdoc />
            public override bool IsArchive
            {
                get { return CompressedArchiveAccess.IsArchive; }
            }

            /// <inheritdoc />
            public override bool IsCompressed
            {
                get { return CompressedArchiveAccess.IsCompressed; }
            }

            /// <inheritdoc />
            public override IEnumerable<ICompressedArchiveEntry> Entries
            {
                get { return CompressedArchiveAccess.Entries; }
            }

            private ICompressedArchiveAccess CompressedArchiveAccess { get; set; }

            private Stream Stream { get; set; }

            /// <summary>
            /// Creates an instance of <see cref="CompressedArchiveFileAccess"/>, depending on <paramref name="mode"/>.
            /// </summary>
            /// <param name="filePath">The absolute path for the compressed archive.</param>
            /// <param name="mode">The access mode to use for operations on the compressed archive.</param>
            /// <param name="implementation">If not <c>null</c>, use a specific implementation if possible. Otherwise, use default, or any.</param>
            /// <returns>An instance of <see cref="CompressedArchiveFileAccess"/> that provides access to the compressed archive located at <paramref name="filePath"/>.</returns>
            /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="filePath"/> is <c>null</c>.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the file is accessed incorrectly for the given <paramref name="mode"/>.</exception>
            /// <exception cref="System.ArgumentException">Thrown if <paramref name="filePath"/> is empty or white space, contains invalid characters, or is otherwise invalid</exception>
            /// <exception cref="System.NotSupportedException">Thrown if <paramref name="filePath"/> refers to a non-file device, e.g. a COM port, et. al.</exception>
            /// <exception cref="System.IO.FileNotFoundException">Thrown if <paramref name="filePath"/> cannot be found.</exception>
            /// <exception cref="System.IO.IOException">Thrown if an I/O error occurs.</exception>
            /// <exception cref="System.Security.SecurityException">Thrown if the requested action on <paramref name="filePath"/> cannot be performed, e.g. no read / create access is granted, et. al.</exception>
            /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the directory cannot be found, e.g. an unavailable network drive forms part of the file path.</exception>
            /// <exception cref="System.UnauthorizedAccessException">Thrown if access is denied, e.g. read/write access requested for a read-only file or directory.</exception>
            /// <exception cref="System.IO.PathTooLongException">Thrown if <paramref name="filePath"/> is too long.</exception>
            /// <exception cref="System.InvalidOperationException">Thrown if it is not possible to create an instance of <see cref="CompressedArchiveFileAccess"/> from the file at <paramref name="filePath"/>,
            /// despite the file appearing to be valid.</exception>
            public static CompressedArchiveFileAccess Create(string filePath, CompressedArchiveAccessMode mode, CompressedArchiveAccessImplementation? implementation)
            {
                var fileMode = CompressedArchiveAccessModeToFileMode(mode);
                var fileAccess = CompressedArchiveAccessModeToFileAccess(mode);
                var fileName = Path.GetFileName(filePath);

                var successfullyAccessedFormats = new List<CompressedArchiveFormat>();
                var fileStream = new FileStream(filePath, fileMode, fileAccess);
                var formats = filePath.GetCompressedArchiveFormatsFromFileName();
                ICompressedArchiveAccess compressedArchiveAccess = null;

#if OPEN_NESTED_FORMAT_IMMEDIATELY
                Stream stream = fileStream;
                foreach (var format in formats)
                {
                    compressedArchiveAccess = Utility.CompressedArchiveAccess.Open(stream, format, mode);
                    if (compressedArchiveAccess != null)
                    {
                        successfullyAccessedFormats.Add(format);
                        if (!compressedArchiveAccess.IsArchive)
                        {
                            fileName = Path.GetFileNameWithoutExtension(fileName);
                            var entry = compressedArchiveAccess.FindEntry(fileName);
                            stream = entry == null ? null : compressedArchiveAccess.OpenEntry(entry);
                            if (stream == null)
                            {
                                compressedArchiveAccess = null;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
#else
                var format = formats.FirstOrDefault();
                if (format != CompressedArchiveFormat.None)
                {
                    compressedArchiveAccess = Utility.CompressedArchiveAccess.Open(fileStream, format, mode, implementation);
                }
#endif

                if (compressedArchiveAccess == null)
                {
                    var identifiedFormats = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, formats);
                    var successfullyCreatedFormats = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, successfullyAccessedFormats);
                    var failedFormats = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, formats.Except(successfullyAccessedFormats));
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.CompressedArchiveAccess_UnableToProcessError_Format, filePath, identifiedFormats, successfullyCreatedFormats, failedFormats);
                    throw new InvalidOperationException(errorMessage);
                }

                var compressedArchiveFileAccess = new CompressedArchiveFileAccess(filePath, compressedArchiveAccess) { Stream = fileStream };
                return compressedArchiveFileAccess;
            }

            /// <inheritdoc />
            public override Stream OpenEntry(ICompressedArchiveEntry entry)
            {
                return CompressedArchiveAccess.OpenEntry(entry);
            }

            /// <inheritdoc />
            public override ICompressedArchiveEntry CreateEntry(string name)
            {
                return CompressedArchiveAccess.CreateEntry(name);
            }

            /// <inheritdoc />
            protected override bool DeleteEntry(ICompressedArchiveEntry entry)
            {
                return CompressedArchiveAccess.DeleteEntry(entry.Name);
            }

            /// <inheritdoc />
            protected override void Dispose(bool disposing)
            {
                lock (_lock)
                {
                    if (CompressedArchiveAccess != null)
                    {
                        CompressedArchiveAccess.Dispose();
                        CompressedArchiveAccess = null;
                    }
                    if (Stream != null)
                    {
                        Stream.Dispose();
                        Stream = null;
                    }
                }
            }
        }

        private struct CompressedArchiveIdentifier : IEqualityComparer<CompressedArchiveIdentifier>, IComparable<CompressedArchiveIdentifier>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="CompressedArchiveIdentifier"/>.
            /// </summary>
            /// <param name="format">The compressed archive format to use in the identifier.</param>
            /// <param name="implementation">The compressed archive access implementation kind to use in the identifier.</param>
            public CompressedArchiveIdentifier(CompressedArchiveFormat format, CompressedArchiveAccessImplementation implementation)
            {
                _format = format;
                _implementation = implementation;
            }

            /// <summary>
            /// Gets the format used in the identifier.
            /// </summary>
            public CompressedArchiveFormat Format
            {
                get { return _format; }
            }
            private CompressedArchiveFormat _format;

            /// <summary>
            /// Gets the implementation used in the identifier.
            /// </summary>
            public CompressedArchiveAccessImplementation Implementation
            {
                get { return _implementation; }
            }
            private CompressedArchiveAccessImplementation _implementation;

            /// <inheritdoc />
            public int CompareTo(CompressedArchiveIdentifier other)
            {
                var result = Format - other.Format;
                if (result == 0)
                {
                    if (Implementation != CompressedArchiveAccessImplementation.Any)
                    {
                        if (other.Implementation != CompressedArchiveAccessImplementation.Any)
                        {
                            result = Implementation - other.Implementation;
                        }
                    }
                }
                return result;
            }

            /// <inheritdoc />
            public bool Equals(CompressedArchiveIdentifier x, CompressedArchiveIdentifier y)
            {
                return x.CompareTo(y) == 0;
            }

            /// <inheritdoc />
            public int GetHashCode(CompressedArchiveIdentifier obj)
            {
                return obj.Format.GetHashCode();
            }
        }
    }
}

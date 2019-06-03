// <copyright file="ZipArchiveAccess.xp.cs" company="INTV Funhouse">
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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Implements a .NET 4.0 mechanism to access ZIP archives in a manner that does not require
    /// external libraries. It is present solely to support Windows xp, as all other platforms can
    /// use the ZipArchive API provided in .NET 4.5 and later. This is a somewhat risky venture, in
    /// that it is only hand-tested and uses reflection to call an internal API. Although .NET 4.0
    /// is unlikely to change. ;-)
    /// Adapted from techniques originally found at CodeProject here:
    /// https://www.codeproject.com/Articles/209731/Csharp-use-Zip-archives-without-external-libraries
    /// Informed and augmented by examining reference source, here:
    /// https://referencesource.microsoft.com/#WindowsBase/Base/MS/Internal/IO/Zip/ZipArchive.cs
    /// </summary>
    internal sealed partial class ZipArchiveAccess : CompressedArchiveAccess
    {
        private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string ZipArchiveNamespace = "MS.Internal.IO.Zip.";

        private static readonly Lazy<Type> ZipArchiveType = new Lazy<Type>(() => typeof(System.IO.Packaging.Package).Assembly.GetType(ZipArchiveNamespace + "ZipArchive"));
        private static readonly Lazy<Type> ZipCompresssionEnumType = new Lazy<Type>(() => ZipArchiveType.Value.Assembly.GetType(ZipArchiveNamespace + "CompressionMethodEnum"));
        private static readonly Lazy<Type> ZipDeflateOptionEnumType = new Lazy<Type>(() => ZipArchiveType.Value.Assembly.GetType(ZipArchiveNamespace + "DeflateOptionEnum"));
        private static readonly Lazy<MethodInfo> OpenFromStreamMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("OpenOnStream", StaticFlags));
        private static readonly Lazy<MethodInfo> GetFilesMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("GetFiles", InstanceFlags));
        private static readonly Lazy<MethodInfo> FileExistsMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("FileExists", InstanceFlags));
        private static readonly Lazy<MethodInfo> GetFileMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("GetFile", InstanceFlags));
        private static readonly Lazy<MethodInfo> AddFileMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("AddFile", InstanceFlags));
        private static readonly Lazy<MethodInfo> DeleteFileMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("DeleteFile", InstanceFlags));
        private static readonly Lazy<MethodInfo> FlushMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("Flush", InstanceFlags));

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException">Thrown if entry has a null name.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the entry's name is too long.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if archive was opened in read mode.</exception>
        protected override bool DeleteEntry(ICompressedArchiveEntry entry)
        {
            var deleted = entry is ZipFileInfo;
            if (deleted)
            {
                DeleteFileMethod.Value.Invoke(_zipArchiveObject, new object[] { entry.Name });
            }
            return deleted;
        }

        /// <summary>
        /// Wraps the internal open method.
        /// </summary>
        /// <param name="stream">A stream containing a ZIP archive.</param>
        /// <param name="mode">The mode in which to access the archive.</param>
        /// <returns>The native object representing a ZIP archive.</returns>
        /// <exception cref="System.FileFormatException">Thrown if archive was opened for reading, but is of zero size.</exception>
        /// <exception cref="System.IOException">Thrown if <paramref name="stream"/> is not empty and archive was opened in Create mode.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an invalid combination of file access and mode is used.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if an invalid file sharing mode is in use.</exception>
        /// <exception cref="System.ArgumentException">Thrown if invalid file access, sharing, and mode combinations are used.</exception>
        private static IDisposable Open(Stream stream, CompressedArchiveAccessMode mode)
        {
            var fileMode = CompressedArchiveAccessModeToFileMode(mode);
            var fileAccess = CompressedArchiveAccessModeToFileAccess(mode);
            var streaming = mode == CompressedArchiveAccessMode.Create; // actually, eventually turns into -> use async option for stream access
            var zipArchiveObject = OpenFromStreamMethod.Value.Invoke(null, new object[] { stream, fileMode, fileAccess, streaming }) as IDisposable;
            return zipArchiveObject;
        }

        private static CompressionMethodEnum ZipArchiveCompressionMethodToCompressionMethodEnum(ZipArchiveCompressionMethod compressionMethod)
        {
            var compressionMethodEnum = CompressionMethodEnum.Deflated;
            switch (compressionMethod)
            {
                case ZipArchiveCompressionMethod.NoCompression:
                    compressionMethodEnum = CompressionMethodEnum.Stored;
                    break;
                default:
                    break;
            }
            return compressionMethodEnum;
        }

        private static DeflateOptionEnum ZipArchiveCompressionMethodToDeflateOptionEnum(ZipArchiveCompressionMethod compressionMethod)
        {
            var deflateOptionEnum = DeflateOptionEnum.Normal;
            switch (compressionMethod)
            {
                case ZipArchiveCompressionMethod.MaximumCompression:
                    deflateOptionEnum = DeflateOptionEnum.Maximum;
                    break;
                case ZipArchiveCompressionMethod.FastestCompression:
                    deflateOptionEnum = DeflateOptionEnum.SuperFast;
                    break;
                case ZipArchiveCompressionMethod.NoCompression:
                    deflateOptionEnum = DeflateOptionEnum.None;
                    break;
                default:
                    break;
            }
            return deflateOptionEnum;
        }

        private static object ZipArchiveCompressionMethodToNativeCompressionMethodEnum(ZipArchiveCompressionMethod compressionMethod)
        {
            var compressionMethodEnum = ZipArchiveCompressionMethodToCompressionMethodEnum(compressionMethod);
            var nativeCompressionMethodEnum = ZipCompresssionEnumType.Value.GetField(compressionMethodEnum.ToString()).GetValue(null);
            return nativeCompressionMethodEnum;
        }

        private static object ZipArchiveCompressionMethodToNativeDeflateOptionEnum(ZipArchiveCompressionMethod compressionMethod)
        {
            var deflateOptionEnum = ZipArchiveCompressionMethodToDeflateOptionEnum(compressionMethod);
            var nativeDeflateOptionEnum = ZipDeflateOptionEnumType.Value.GetField(deflateOptionEnum.ToString()).GetValue(null);
            return nativeDeflateOptionEnum;
        }

        /// <summary>
        /// Gets the entries in the archive.
        /// </summary>
        /// <returns>An enumerable of the entries in the archive.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if archive was opened in write mode.</exception>
        private IEnumerable<ZipFileInfo> GetArchiveEntries()
        {
            var zipFileInfoObjects = GetFilesMethod.Value.Invoke(_zipArchiveObject, null) as IEnumerable;
            foreach (var zipFileInfoObject in zipFileInfoObjects)
            {
                yield return new ZipFileInfo(zipFileInfoObject);
            }
        }

        private Stream OpenZipEntry(ICompressedArchiveEntry entry)
        {
            var zipFileInfo = entry as ZipFileInfo;
            var stream = zipFileInfo == null ? null : zipFileInfo.GetStream(FileMode.Open, FileAccess.Read);
            return stream;
        }

        /// <summary>
        /// Creates a new ZIP entry in the archive.
        /// </summary>
        /// <param name="fileName">The name of the entry in the archive.</param>
        /// <param name="compressionMethod">The compression method to use.</param>
        /// <returns>The new archive entry.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fileName"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="fileName"/> is too long, or <paramref name="compressionMethod"/> indicates improper compression or deflate mode.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if archive was opened in read mode, or <paramref name="fileName"/> already exists in the archive.</exception>
        private ICompressedArchiveEntry CreateZipEntry(string fileName, ZipArchiveCompressionMethod compressionMethod)
        {
            var compression = ZipArchiveCompressionMethodToNativeCompressionMethodEnum(compressionMethod);
            var deflate = ZipArchiveCompressionMethodToNativeDeflateOptionEnum(compressionMethod);
            var zipFileInfoObject = AddFileMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName, compression, deflate });
            var zipFileInfo = new ZipFileInfo(zipFileInfoObject);
            return zipFileInfo;
        }

        /// <summary>
        /// Gets a <see cref="ZipFileInfo"/> for the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the entry in the archive.</param>
        /// <returns>The entry in the archive.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fileName"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="fileName"/> is too long.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if archive was opened in write mode, or <paramref name="fileName"/> does not exist in the archive.</exception>
        private ZipFileInfo GetFile(string fileName)
        {
            var zipFileInfoObject = GetFileMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName });
            var zipFileInfo = new ZipFileInfo(zipFileInfoObject);
            return zipFileInfo;
        }

        private bool FileEntryExists(string fileName)
        {
            bool exists = (bool)FileExistsMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName });
            return exists;
        }

        /// <summary>
        /// These values specify if data is compressed or not.
        /// </summary>
        private enum CompressionMethodEnum : ushort
        {
            /// <summary>No compression - just a stored copy.</summary>
            Stored = 0,

            /// <summary>Standard ZIP compression.</summary>
            Deflated = 8
        }

        /// <summary>
        /// These values specify speed vs. size tradeoff for compression.
        /// </summary>
        private enum DeflateOptionEnum : byte
        {
            /// <summary>Standard compression.</summary>
            Normal = 0,

            /// <summary>Maximum compression (slowest).</summary>
            Maximum = 2,

            /// <summary>Some compression, good speed.</summary>
            Fast = 4,

            /// <summary>Fastest speed, low compression.</summary>
            SuperFast = 6,

            /// <summary>Not applicable.</summary>
            None = 0xFF
        }

        /// <summary>
        /// Implements access to the internal ZipFileInfo type.
        /// </summary>
        private class ZipFileInfo : ICompressedArchiveEntry
        {
            private static readonly Lazy<Type> ZipFileInfoType = new Lazy<Type>(() => typeof(System.IO.Packaging.Package).Assembly.GetType(ZipArchiveNamespace + "ZipFileInfo"));
            private static readonly Lazy<MethodInfo> GetStreamMethod = new Lazy<MethodInfo>(() => ZipFileInfoType.Value.GetMethod("GetStream", InstanceFlags));
            private static readonly ConcurrentDictionary<string, PropertyInfo> Properties = new ConcurrentDictionary<string, PropertyInfo>();

            /// <summary>
            /// Initializes a new instance of <see cref="ZipFileInfo"/>.
            /// </summary>
            /// <param name="nativeObject">The native zip file information to wrap.</param>
            public ZipFileInfo(object nativeObject)
            {
                NativeObject = nativeObject;
            }

            #region ICompressedArchiveEntry

            /// <inheritdoc />
            public string Name
            {
                get { return GetPropertyValue<string>("Name"); }
            }

            /// <inheritdoc />
            /// <remarks>With enough effort, it may be possible to extract this value.
            /// See: https://referencesource.microsoft.com/#WindowsBase/Base/MS/Internal/IO/Zip/ZipIOCentralDirectoryFileHeader.cs,9d73c7b389b47091 </remarks>
            public long Length
            {
                get { return -1; }
            }

            /// <inheritdoc />
            public DateTime LastModificationTime
            {
                get { return LastModFileDateTime; }
            }

            /// <inheritdoc />
            public bool IsDirectory
            {
                get { return FolderFlag; }
            }

            #endregion // ICompressedArchiveEntry

            /// <summary>
            /// Gets the last modification time of the file entry.
            /// </summary>
            public DateTime LastModFileDateTime
            {
                get { return GetPropertyValue<DateTime>("LastModFileDateTime"); }
            }

            /// <summary>
            /// Gets the compression method used for the entry.
            /// </summary>
            public CompressionMethodEnum CompressionMethod
            {
                get { return (CompressionMethodEnum)GetPropertyValue<ushort>("CompressionMethod"); }
            }

            /// <summary>
            /// Gets the deflate option for the entry.
            /// </summary>
            public DeflateOptionEnum DeflateOption
            {
                get { return (DeflateOptionEnum)GetPropertyValue<byte>("DeflateOption"); }
            }

            /// <summary>
            /// Gets a value indicating whether or not the entry represents a folder.
            /// </summary>
            public bool FolderFlag
            {
                get { return GetPropertyValue<bool>("FolderFlag"); }
            }

            /// <summary>
            /// Gets a value indicating whether or not the entry represents a volume label.
            /// </summary>
            public bool VolumeLabelFlag
            {
                get { return GetPropertyValue<bool>("VolumeLabelFlag"); }
            }

            private object NativeObject { get; set; }

            /// <summary>
            /// Gets a stream to operate on the underlying item in the ZIP archive.
            /// </summary>
            /// <param name="mode">The file mode to use.</param>
            /// <param name="access">The file access mode to use.</param>
            /// <returns>A stream for input or output or both, depending on <paramref name="mode"/>, <paramref name="access"/> and how the owning archive was opened.</returns>
            /// <exception cref="System.InvalidOperationException">Thrown if archive mode is incompatible with <paramref name="mode"/> or <paramref name="access"/>.</exception>
            /// <exception cref="System.ArgumentException">Thrown if an invalid combination of <paramref name="mode"/> and <paramref name="access"/> is requested.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="mode"/> or <paramref name="access"/> are unsupported.</exception>
            public Stream GetStream(FileMode mode, FileAccess access)
            {
                var stream = (Stream)GetStreamMethod.Value.Invoke(NativeObject, new object[] { mode, access });
                return stream;
            }

            private T GetPropertyValue<T>(string propertyName)
            {
                PropertyInfo property;
                if (!Properties.TryGetValue(propertyName, out property))
                {
                    property = ZipFileInfoType.Value.GetProperty(propertyName, InstanceFlags);
                    Properties.TryAdd(propertyName, property);
                }
                var value = property.GetValue(NativeObject, null);
                return (T)value;
            }
        }
    }
}

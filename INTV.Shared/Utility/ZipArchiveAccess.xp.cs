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
using System.Linq;
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
    public sealed partial class ZipArchiveAccess
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
        ////private static readonly Lazy<MethodInfo> FlushMethod = new Lazy<MethodInfo>(() => ZipArchiveType.Value.GetMethod("Flush", InstanceFlags));

        internal static void Testing()
        {
            var path = @"D:\Users\Steve\Downloads\LTO_Flash_4764.zip";
            var clonePath = @"D:\Users\Steve\Downloads\LTO_Flash_4764-clone2.zip";
            ////          path = clonePath;
            ////path = @"D:\Users\Steve\Projects\appletSource1.zip";
            ////            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            ////            using (var zip = new ZipArchiveAccess(stream))
            using (var zip = new ZipArchiveAccess(stream, ZipArchiveAccessMode.Update))
            {
                ////zip.Delete("release_notes.txt");
                ////zip.Delete("goover");
                using (var clone = new FileStream(clonePath, FileMode.CreateNew, FileAccess.Write))
                {
                    using (var zipClone = new ZipArchiveAccess(clone, ZipArchiveAccessMode.Create))
                    {
                        foreach (var file in zip.FileNames)
                        {
                            System.Diagnostics.Debug.WriteLine(file);
                            using (var copy = zipClone.Add(file, ZipArchiveCompressionMethod.MaximumCompression))
                            {
                                using (var s = zip.OpenFileEntry(file))
                                {
                                    s.CopyTo(copy);
                                }
                            }
                        }
                        var exists = zip.FileExists("release_notes.txt");
                        using (var s = zip.OpenFileEntry("release_notes.txt"))
                        {
                            var reader = new StreamReader(s);
                            var text = reader.ReadToEnd();
                        }
                        exists = zip.FileExists("goober");
                    }
                }
            }
        }

        // FileFormatException - open mode, zero size
        // IOException - create(new) - not empty
        // ArgumentNullException null stream
        // ArgumentOutOfRangeException - bad combo if mode and access
        // NotSupportedException - bad sharing options
        // ArgumentException more bad mode / access / share combos
        private static IDisposable Open(Stream stream, ZipArchiveAccessMode mode)
        {
            var fileMode = ZipArchiveAccessModeToFileMode(mode);
            var fileAccess = ZipArchiveAccessModeToFileAccess(mode);
            var streaming = mode == ZipArchiveAccessMode.Create; // actually, eventually turns into -> use async option for stream access
            var zipArchiveObject = OpenFromStreamMethod.Value.Invoke(null, new object[] { stream, fileMode, fileAccess, streaming }) as IDisposable;
            return zipArchiveObject;
        }

        private static FileMode ZipArchiveAccessModeToFileMode(ZipArchiveAccessMode mode)
        {
            var fileMode = FileMode.Open;
            switch (mode)
            {
                case ZipArchiveAccessMode.Create:
                    fileMode = FileMode.Create;
                    break;
                case ZipArchiveAccessMode.Update:
                    fileMode = FileMode.OpenOrCreate;
                    break;
                default:
                    break;
            }
            return fileMode;
        }

        private static FileAccess ZipArchiveAccessModeToFileAccess(ZipArchiveAccessMode mode)
        {
            var fileAccess = FileAccess.Read;
            switch (mode)
            {
                case ZipArchiveAccessMode.Create:
                    fileAccess = FileAccess.Write;
                    break;
                case ZipArchiveAccessMode.Update:
                    fileAccess = FileAccess.ReadWrite;
                    break;
                default:
                    break;
            }
            return fileAccess;
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

        // InvalidOperationException - opened in write mode, file does not exist
        private IEnumerable<ZipFileInfo> GetFiles()
        {
            var zipFileInfoObjects = GetFilesMethod.Value.Invoke(_zipArchiveObject, null) as IEnumerable;
            foreach (var zipFileInfoObject in zipFileInfoObjects)
            {
                yield return new ZipFileInfo(zipFileInfoObject);
            }
        }

        // ArgumentNullException - null file name
        // ArgumentOutOfRangeException - file name too long
        // InvalidOperationException - opened in write mode, file does not exist
        private ZipFileInfo GetFile(string fileName)
        {
            var zipFileInfoObject = GetFileMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName });
            var zipFileInfo = new ZipFileInfo(zipFileInfoObject);
            return zipFileInfo;
        }

        private IEnumerable<string> GetFileEntryNames()
        {
            return GetFiles().Select(f => f.Name).OrderBy(p => p, PathComparer.Instance);
        }

        private bool FileEntryExists(string fileName)
        {
            bool exists = (bool)FileExistsMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName });
            return exists;
        }

        private Stream OpenFileEntry(string fileName)
        {
            var zipFileInfo = GetFile(fileName);
            var stream = zipFileInfo.GetStream(FileMode.Open, FileAccess.Read);
            return stream;
        }

        // ArgumentNullException - null file name
        // ArgumentOutOfRangeException - file name too long, invalid compression, deflation
        // InvalidOperationException - opened in Read mode, file already exists
        private Stream CreateAndOpenFileEntry(string fileName, ZipArchiveCompressionMethod compressionMethod)
        {
            var compression = ZipArchiveCompressionMethodToNativeCompressionMethodEnum(compressionMethod);
            var deflate = ZipArchiveCompressionMethodToNativeDeflateOptionEnum(compressionMethod);
            var zipFileInfoObject = AddFileMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName, compression, deflate });
            var zipFileInfo = new ZipFileInfo(zipFileInfoObject);
            var fileMode = ZipArchiveAccessModeToFileMode(Mode);
            var fileAccess = ZipArchiveAccessModeToFileAccess(Mode);
            var stream = zipFileInfo.GetStream(fileMode, fileAccess);
            return stream;
        }

        // ArgumentNullException - null file name
        // ArgumentOutOfRangeException - file name too long
        // InvalidOperationException - opened in Read mode
        private void DeleteFileEntry(string fileName)
        {
            DeleteFileMethod.Value.Invoke(_zipArchiveObject, new object[] { fileName });
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

        private class ZipFileInfo
        {
            private static readonly Lazy<Type> ZipFileInfoType = new Lazy<Type>(() => typeof(System.IO.Packaging.Package).Assembly.GetType(ZipArchiveNamespace + "ZipFileInfo"));
            private static readonly Lazy<MethodInfo> GetStreamMethod = new Lazy<MethodInfo>(() => ZipFileInfoType.Value.GetMethod("GetStream", InstanceFlags));
            private static readonly ConcurrentDictionary<string, PropertyInfo> Properties = new ConcurrentDictionary<string, PropertyInfo>();

            public ZipFileInfo(object nativeObject)
            {
                NativeObject = nativeObject;
            }

            public string Name
            {
                get { return GetPropertyValue<string>("Name"); }
            }

            public DateTime LastModFileDateTime
            {
                get { return GetPropertyValue<DateTime>("LastModFileDateTime"); }
            }

            public CompressionMethodEnum CompressionMethod
            {
                get { return (CompressionMethodEnum)GetPropertyValue<ushort>("CompressionMethod"); }
            }

            public DeflateOptionEnum DeflateOption
            {
                get { return (DeflateOptionEnum)GetPropertyValue<byte>("DeflateOption"); }
            }

            public bool FolderFlag
            {
                get { return GetPropertyValue<bool>("FolderFlag"); }
            }

            public bool VolumeLabelFlag
            {
                get { return GetPropertyValue<bool>("VolumeLabelFlag"); }
            }

            private object NativeObject { get; set; }

            // InvalidOperationException - file mode Create but can't write
            // ArgumentException - FileMode CreateNew/Append/Truncate / access
            // ArgumentOutOfRangeException - bad file mode / access
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

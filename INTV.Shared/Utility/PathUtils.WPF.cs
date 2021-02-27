// <copyright file="PathUtils.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2015-2021 All Rights Reserved
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public static partial class PathUtils
    {
        /// <summary>
        /// Default file extension for a program.
        /// </summary>
        public static readonly string ProgramSuffix = ".exe";

        /// <summary>
        /// Enumerates the files in the given directory matching the provided file extension.
        /// </summary>
        /// <param name="directory">The directory in which to enumerate files.</param>
        /// <param name="fileExtension">The file extension to match.</param>
        /// <returns>The files in <paramref name="directory"/> whose file names end with <paramref name="fileExtension"/>.</returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="directory"/> is a zero-length string,
        /// contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars(),
        /// or if <paramref name="fileExtension"/> does not contain a valid file extension.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="directory"/> or <paramref name="fileExtension"/> is <c>null</c>.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if <paramref name="directory"/> is invalid, such as referring to an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException">Thrown if <paramref name="directory"/> is a file name.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown if <paramref name="directory"/> combined with a resulting matching file name
        /// specifies a path whose length exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than
        ///  248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">Thrown if the caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFilesWithPattern(this string directory, string fileExtension)
        {
            var searchPattern = fileExtension;
            switch (fileExtension[0])
            {
                case '*':
                    break;
                default:
                    searchPattern = "*" + fileExtension;
                    break;
            }
            var results = Directory.EnumerateFiles(directory, searchPattern, SearchOption.TopDirectoryOnly);
            return results;
        }

        /// <summary>
        /// Gets the Documents directory.
        /// </summary>
        /// <returns>The documents directory.</returns>
        public static string GetDocumentsDirectory()
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            return documentsPath;
        }

        /// <summary>
        /// Gets whether or not the given path exists on a removable device.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path refers to a file on some type of removable device.</returns>
        public static bool IsPathOnRemovableDevice(this string path)
        {
            if (!Path.IsPathRooted(path))
            {
                throw new System.ArgumentException(Resources.Strings.PathNotRootedError, path);
            }
            var root = Path.GetPathRoot(path);
            var driveInfo = new DriveInfo(root);
            var isRemovable = false;
            switch (driveInfo.DriveType)
            {
                case DriveType.Fixed:
                    isRemovable = false;
                    break;
                case DriveType.Unknown:
                case DriveType.NoRootDirectory:
                case DriveType.Removable:
                case DriveType.Network:
                case DriveType.CDRom:
                case DriveType.Ram:
                default:
                    isRemovable = true;
                    break;
            }
            if (!isRemovable)
            {
                var ephemeralPath = System.IO.Path.GetTempPath(); // treat temp dir as ephemeral
                isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
            }
            if (!isRemovable)
            {
                var ephemeralPath = NativeMethods.GetDownloadsPath(); // treat downloads path as ephemeral
                isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
            }
            if (!isRemovable)
            {
                var ephemeralPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.History);
                isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
            }
            if (!isRemovable)
            {
                var ephemeralPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache);
                isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
            }
            if (!isRemovable)
            {
                var ephemeralPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CDBurning);
                isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
            }
            return isRemovable;
        }

        /// <summary>
        /// Reveals the given path in file system.
        /// </summary>
        /// <param name="path">Absolute path to the file to show in Explorer.</param>
        public static void RevealInFileSystem(this string path)
        {
            RevealInFileSystem(new[] { path });
        }

        /// <summary>
        /// Reveals the given paths in the file system.
        /// </summary>
        /// <param name="files">Absolute paths to the files to show in Explorer.</param>
        /// <remarks>Adapted from this article: http://ask.webatall.com/windows/16424_programatically-select-multiple-files-in-windows-explorer.html </remarks>
        public static void RevealInFileSystem(this IEnumerable<string> files)
        {
            var folders = new Dictionary<string, List<string>>(PathComparer.Instance);
            foreach (var file in files.Where(f => File.Exists(f) || Directory.Exists(f)))
            {
                var folder = File.GetAttributes(file).HasFlag(FileAttributes.Directory) ? file : Path.GetDirectoryName(file);
                List<string> filesInFolder;
                if (folders.TryGetValue(folder, out filesInFolder))
                {
                    filesInFolder.Add(file);
                }
                else
                {
                    folders[folder] = new List<string>(new[] { file });
                }
            }

            foreach (var folderAndFiles in folders)
            {
                var folder = folderAndFiles.Key;
                var filesToSelect = folderAndFiles.Value;
                var directory = NativeMethods.ILCreateFromPath(folder);

                var filesToSelectIntPtrs = new System.IntPtr[filesToSelect.Count];
                for (int i = 0; i < filesToSelect.Count; i++)
                {
                    filesToSelectIntPtrs[i] = NativeMethods.ILCreateFromPath(filesToSelect[i]);
                }

                NativeMethods.SHOpenFolderAndSelectItems(directory, (uint)filesToSelect.Count, filesToSelectIntPtrs, 0);
                NativeMethods.ReleaseComObject(directory);
                NativeMethods.ReleaseComObject(filesToSelectIntPtrs);
            }
        }

        /// <summary>
        /// Resolves the path for settings.
        /// </summary>
        /// <param name="path">The file path to resolve into settings-friendly format.</param>
        /// <returns>The path for settings.</returns>
        public static string ResolvePathForSettings(string path)
        {
            // TODO: Consider supporting Url syntax.
            return path;
        }

        private static string OSFixUpSeparators(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}

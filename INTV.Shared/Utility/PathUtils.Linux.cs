// <copyright file="PathUtils.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Linux-specific implementation.
    /// </summary>
    public static partial class PathUtils
    {
        /// <summary>
        /// Default file extension for a program.
        /// </summary>
        public static readonly string ProgramSuffix = string.Empty;

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
                throw new System.ArgumentException("Path not rooted.", path);
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
//            {
//                var ephemeralPath = NativeMethods.GetDownloadsPath(); // treat downloads path as ephemeral
//                isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
//            }
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

            // Use technique from StackOverflow...
            // See: https://stackoverflow.com/questions/22637461/how-to-open-file-location-or-open-folder-location-in-monodevelop-gtk
            foreach (var folder in folders.Keys.Distinct(PathComparer.Instance))
            {
                // Given the unpredictability of things... just open folder? How many times?
                // gnome-open? full path opens in app.... but it hangs if you give directory?
                // nautilus?
                // configure the command to run? Ugh!
                Uri path;
                if (Uri.TryCreate(folder, UriKind.RelativeOrAbsolute, out path))
                {
                    System.Diagnostics.Process.Start(path.AbsoluteUri);
                }
            }
        }

        private static string OSFixUpSeparators(string path)
        {
            return path;
        }
    }
}

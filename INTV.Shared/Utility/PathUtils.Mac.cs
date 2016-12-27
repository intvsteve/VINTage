// <copyright file="PathUtils.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class PathUtils
    {
        /// <summary>
        /// Determines if is file path is a URL presented as a string.
        /// </summary>
        /// <param name="path">An absolute path to test to see if it's a file URL-style path.</param>
        /// <returns><c>true</c> if is path is a file URL as a string; otherwise, <c>false</c>.</returns>
        public static bool IsFileUrlAsString(this string path)
        {
            return path.StartsWith(System.Uri.UriSchemeFile + System.Uri.SchemeDelimiter);
        }

        /// <summary>
        /// Gets the Documents directory.
        /// </summary>
        /// <returns>The documents directory.</returns>
        public static string GetDocumentsDirectory()
        {
            var documentsPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.All).FirstOrDefault().Path;
            return documentsPath;
        }

        /// <summary>
        /// Gets whether or not the given path exists on a removable device.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path refers to a file on some type of removable device.</returns>
        public static bool IsPathOnRemovableDevice(this string path)
        {
            var isRemovable = true;
            var writeable = false;
            var unmountable = false;
            var description = string.Empty;
            var fileSystem = string.Empty;
            var check = NSApplication.CheckForIllegalCrossThreadCalls;
            try
            {
                NSApplication.CheckForIllegalCrossThreadCalls = false;
                var succeeded = NSWorkspace.SharedWorkspace.GetFileSystemInfo(path, out isRemovable, out writeable, out unmountable, out description, out fileSystem);
                isRemovable = !succeeded || isRemovable;
                if (!isRemovable)
                { // probably wrong
                    var ephemeralPath = System.IO.Path.GetTempPath(); // treat temp dir as ephemeral
                    isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
                }
                if (!isRemovable)
                {
                    var downloadPaths = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DownloadsDirectory, NSSearchPathDomain.All);
                    foreach (var downloadPath in downloadPaths)
                    {
                        isRemovable = path.StartsWith(downloadPath.Path, System.StringComparison.OrdinalIgnoreCase);
                        if (isRemovable)
                        {
                            break;
                        }
                    }
                }
                if (!isRemovable)
                {
                    var trashPaths = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.TrashDirectory, NSSearchPathDomain.All);
                    foreach (var trashPath in trashPaths)
                    {
                        isRemovable = path.StartsWith(trashPath.Path, System.StringComparison.OrdinalIgnoreCase);
                        if (isRemovable)
                        {
                            break;
                        }
                    }
                }
                if (!isRemovable)
                {
                    // not implemented
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
                    // not implemented
                    var ephemeralPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CDBurning);
                    isRemovable = !string.IsNullOrEmpty(ephemeralPath) && path.StartsWith(ephemeralPath, System.StringComparison.OrdinalIgnoreCase);
                }
            }
            finally
            {
                NSApplication.CheckForIllegalCrossThreadCalls = check;
            }
            return isRemovable;
        }

        /// <summary>
        /// Reveals the given path in file system.
        /// </summary>
        /// <param name="path">Absolute path to the file to show in Finder.</param>
        public static void RevealInFileSystem(this string path)
        {
            RevealInFileSystem(new[] { path });
        }

        /// <summary>
        /// Reveals the given paths in the file system.
        /// </summary>
        /// <param name="files">Absolute paths to the files to show in Finder.</param>
        public static void RevealInFileSystem(this IEnumerable<string> files)
        {
            NSWorkspace.SharedWorkspace.ActivateFileViewer(files.Select(f => NSUrl.FromFilename(f)).ToArray());
        }
    }
}

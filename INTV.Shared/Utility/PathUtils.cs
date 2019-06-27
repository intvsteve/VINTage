// <copyright file="PathUtils.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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
    /// Useful methods for working with file paths.
    /// </summary>
    public static partial class PathUtils
    {
        /// <summary>
        /// Default file extension for a backup copy of a file.
        /// </summary>
        public static readonly string BackupSuffix = ".bak";

        /// <summary>
        /// Given a Uri form of a file path, return one suitable for use with file system APIs.
        /// </summary>
        /// <param name="path">A file Uri to convert to a file system path.</param>
        /// <returns>An absolute file system path with appropriate separators.</returns>
        /// <remarks>In Windows, at least some file / path APIs only support the traditional backslash separator. This method takes a Uri and
        /// ensures that the string form of the file path returned uses appropriate path separators for the platform.</remarks>
        public static string FixUpUriPath(this Uri path)
        {
            var pathString = OSFixUpSeparators(Uri.UnescapeDataString(path.AbsolutePath));
            return pathString;
        }

        /// <summary>
        /// Computes the relative path between two paths.
        /// </summary>
        /// <param name="file">The path for which to compute a relative path.</param>
        /// <param name="relativeTo">The reference path for computing the relative path.</param>
        /// <returns>The path to file as a path relative to the relativeTo path.</returns>
        /// <remarks>Uses Uri tricks, which means it will break on Linux.</remarks>
        public static string GetRelativePath(string file, string relativeTo)
        {
            if (string.IsNullOrEmpty(file))
            {
                return string.Empty;
            }
            else if (string.IsNullOrEmpty(relativeTo))
            {
                return file;
            }
            Uri pathUri = new Uri(file);

            // Folders must end in a slash
            if (!relativeTo.EndsWith(Path.DirectorySeparatorChar.ToString(), PathComparer.DefaultPolicy) && !relativeTo.EndsWith(Path.AltDirectorySeparatorChar.ToString(), PathComparer.DefaultPolicy))
            {
                relativeTo += Path.DirectorySeparatorChar;
            }
            Uri relativeToUri = new Uri(relativeTo);
            return OSFixUpSeparators(Uri.UnescapeDataString(relativeToUri.MakeRelativeUri(pathUri).ToString()));
        }

        /// <summary>
        /// Gets the common path.
        /// </summary>
        /// <returns>The common path.</returns>
        /// <param name="paths">The paths from which to extract a common path.</param>
        /// <remarks>Modified from the C# entry from RosettaCode.org.</remarks>
        /// <see cref="http://rosettacode.org/wiki/Find_Common_Directory_Path#C.23"/>
        public static string GetCommonPath(IEnumerable<string> paths)
        {
            char[] separator = new[] { Path.DirectorySeparatorChar };
            if (Path.DirectorySeparatorChar != Path.AltDirectorySeparatorChar)
            {
                paths = paths.Select(p => p.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
            }
            var longestCommonPath = string.Empty;
            var separatedPaths = paths.First(p => p.Length == paths.Max(p2 => p2.Length)).Split(separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var element in separatedPaths)
            {
                if (longestCommonPath.Length == 0)
                {
                    var firstElement = element;
                    if (!Path.IsPathRooted(element))
                    {
                        firstElement = Path.DirectorySeparatorChar + element;
                    }
#if WIN
                    else
                    {
                        firstElement += Path.DirectorySeparatorChar;
                    }
#endif // WIN
                    if (paths.All(p => p.StartsWith(firstElement, PathComparer.DefaultPolicy)))
                    {
                        longestCommonPath = firstElement;
                    }
                }
                else if (paths.All(p => p.StartsWith(Path.Combine(longestCommonPath, element), PathComparer.DefaultPolicy)))
                {
                    longestCommonPath = Path.Combine(longestCommonPath, element);
                }
                else
                {
                    break;
                }
            }
            if (string.IsNullOrEmpty(longestCommonPath) && (paths.Count() == 1))
            {
                longestCommonPath = paths.First();
            }
            return longestCommonPath;
        }

        // UNDONE Haven't determined a need for the following code yet -- or even if it's correct.
#if false
        /// <summary>
        /// Finds the longest possible common path given two file paths.
        /// </summary>
        /// <param name="pathA">The first path.</param>
        /// <param name="pathB">The second path.</param>
        /// <returns>The portion of the path shared between the two paths.</returns>
        private static string GetCommonBasePath(string pathA, string pathB)
        {
            string commonBasePath = string.Empty;
            var consistentPathA = pathA.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var consistentPathB = pathB.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var pathAElements = consistentPathA.Split(Path.DirectorySeparatorChar);
            var pathBElements = consistentPathB.Split(Path.DirectorySeparatorChar);
            int numCommonElements = 0;
            for (int i = 0; i < Math.Min(pathAElements.Length, pathBElements.Length); ++i)
            {
                if (PathComparer.Instance.Equals(pathAElements[i], pathBElements[i]))
                {
                    ++numCommonElements;
                }
                else
                {
                    break;
                }
            }
            if (numCommonElements > 0)
            {
                commonBasePath = string.Join(Path.DirectorySeparatorChar.ToString(), pathAElements, 0, numCommonElements);
            }
            return commonBasePath;
        }

        private static string GetLongestCommonSubstring(this IEnumerable<string> strings)
        {
            if ((strings == null) || !strings.Any())
            {
                return null;
            }
            var commonSubstrings = new HashSet<string>(strings.First().GetSubstrings());
            foreach (string str in strings.Skip(1))
            {
                commonSubstrings.IntersectWith(str.GetSubstrings());
                if (commonSubstrings.Count == 0)
                {
                    return null;
                }
            }
            return commonSubstrings.OrderByDescending(s => s.Length).First();
        }

        private static IEnumerable<string> GetSubstrings(this string str)
        {
            for (int c = 0; c < str.Length - 1; c++)
            {
                for (int cc = 1; c + cc <= str.Length; cc++)
                {
                    yield return str.Substring(c, cc);
                }
            }
        }
#endif // false

        /// <summary>
        /// Gets a 'backup' file path for the given path.
        /// </summary>
        /// <param name="path">An absolute path for which to generate a backup path.</param>
        /// <returns>The backup path.</returns>
        public static string GetUniqueBackupFilePath(this string path)
        {
            path = (path + BackupSuffix).EnsureUniqueFileName();
            return path;
        }

        /// <summary>
        /// Ensures that a file name is unique in a directory.
        /// </summary>
        /// <param name="path">The path to check for uniqueness.</param>
        /// <returns>A unique file path, made unique using the current system time.</returns>
        public static string EnsureUniqueFileName(this string path)
        {
            var extension = Path.GetExtension(path);
            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var validfileName = filename.EnsureValidFileName();
            if (validfileName != filename)
            {
                path = Path.Combine(directory, validfileName);
            }
            while (File.Exists(path))
            {
                System.Threading.Thread.Sleep(1);
                var fileName = Path.GetFileNameWithoutExtension(path) + "_" + GetTimeString() + extension;
                path = Path.Combine(directory, fileName);
            }
            return path;
        }

        /// <summary>
        /// Ensures that a file name is valid (no invalid characters or reserved name).
        /// </summary>
        /// <param name="filename">The file name to validate.</param>
        /// <returns>The valid file name.</returns>
        /// <remarks>Invalid characters will be replaced with underscores. An invalid file name will have an underscore appended.</remarks>
        public static string EnsureValidFileName(this string filename)
        {
            var validatedName = string.IsNullOrEmpty(filename) ? "Unknown" : filename;
            foreach (var invalidChar in Path.GetInvalidPathChars())
            {
                validatedName = validatedName.Replace(invalidChar, '_');
            }
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                validatedName = validatedName.Replace(invalidChar, '_');
            }
            var reservedNames = new[]
            {
                "CLOCK$", "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            // Don't care about case sensitivity here.
            if (reservedNames.Contains(validatedName, StringComparer.InvariantCultureIgnoreCase))
            {
                validatedName += "_";
            }
            return validatedName;
        }

        /// <summary>
        /// Gets a string representing the current local time in the format YYYY-MM-DD-HH-MM-SS-NNN.
        /// </summary>
        /// <returns>The time as a specially formatted string.</returns>
        public static string GetTimeString()
        {
            return GetTimeString(false);
        }

        /// <summary>
        /// Gets a string representing the current time in the format YYYY-MM-DD-HH-MM-SS-NNN.
        /// </summary>
        /// <param name="useUTC">If <c>true</c>, use UTC, otherwise use local time.</param>
        /// <returns>The time as a specially formatted string.</returns>
        public static string GetTimeString(bool useUTC)
        {
            var now = useUTC ? DateTime.UtcNow : DateTime.Now;
            var dateTimeForFileName = now.Year.ToString("D4") + "-" + now.Month.ToString("D2") + "-" + now.Day.ToString("D2");
            dateTimeForFileName += "-" + now.Hour.ToString("D2") + "-" + now.Minute.ToString("D2") + "-" + now.Second.ToString("D2") + "-" + now.Millisecond.ToString("D3");
            return dateTimeForFileName;
        }

        /// <summary>
        /// Given a file path, attempt to parse the file name as a date-time string as generated from the <see cref="GetTimeString()"/>
        /// method in this class.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>A <see cref="DateTime"/> value created from the give file name. If all the elements cannot be determined, then
        /// a value of DateTime.MinValue is returned.</returns>
        public static DateTime GetDateTimeFromString(string path)
        {
            var directoryName = Path.GetFileName(path);
            var elements = directoryName.Split('-');

            int year = -1;
            var valid = (elements.Length == 7) && int.TryParse(elements[0], out year);

            int month = -1;
            valid = valid && int.TryParse(elements[1], out month);

            int day = -1;
            valid = valid && int.TryParse(elements[2], out day);

            int hour = -1;
            valid = valid && int.TryParse(elements[3], out hour);

            int minute = -1;
            valid = valid && int.TryParse(elements[4], out minute);

            int second = -1;
            valid = valid && int.TryParse(elements[5], out second);

            int millisecond = -1;
            valid = valid && int.TryParse(elements[6], out millisecond);

            var result = DateTime.MinValue;
            if (valid)
            {
                result = new DateTime(year, month, day, hour, minute, second, millisecond);
            }

            return result;
        }
    }
}

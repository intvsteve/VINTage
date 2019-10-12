// <copyright file="ProgramFileKindHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using System.Linq;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Extension methods for the ProgramFileKind enumeration.
    /// </summary>
    public static class ProgramFileKindHelpers
    {
        /// <summary>
        /// Lookup table of a file kind to file system extensions (case-insensitive).
        /// </summary>
        private static readonly Lazy<ConcurrentDictionary<ProgramFileKind, IEnumerable<string>>> FileExtensionsForFileKind = new Lazy<ConcurrentDictionary<ProgramFileKind, IEnumerable<string>>>(GetFileExtensionsForFileKind);

        /// <summary>
        /// Lookup table of a file kind to a file suffix for support files (box, label, overlay, or manual images, for example).
        /// </summary>
        private static readonly Lazy<ConcurrentDictionary<ProgramFileKind, string>> FileSuffixForFileKind = new Lazy<ConcurrentDictionary<ProgramFileKind, string>>(GetFileSuffixForFileKind);

        /// <summary>
        /// Lookup table of a file kind to a list of subdirectories in which the support file kind may found.
        /// </summary>
        private static readonly Lazy<ConcurrentDictionary<ProgramFileKind, IEnumerable<string>>> FileSubdirectoriesForFileKind = new Lazy<ConcurrentDictionary<ProgramFileKind, IEnumerable<string>>>(GetFileSubdirectoriesForFileKind);

        private static readonly Lazy<ConcurrentBag<string>> RomsThatUseCfgFiles = new Lazy<ConcurrentBag<string>>(() => new ConcurrentBag<string>(new[] { ".bin", ".itv", ".int" }));
        private static readonly Lazy<ConcurrentBag<ProgramFileKind>> SupportFileKindsBag = new Lazy<ConcurrentBag<ProgramFileKind>>(() => new ConcurrentBag<ProgramFileKind>(new[] { ProgramFileKind.Box, ProgramFileKind.ManualCover, ProgramFileKind.ManualText, ProgramFileKind.Overlay, ProgramFileKind.Label, ProgramFileKind.SaveData, ProgramFileKind.CfgFile, ProgramFileKind.Vignette, ProgramFileKind.GenericSupportFile }));
        private static readonly Lazy<HashSet<string>> CustomRomExtensions = new Lazy<HashSet<string>>(() => new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { string.Empty });
        private static readonly Lazy<IEnumerable<string>> SupportFileExtensionsData = new Lazy<IEnumerable<string>>(() => SupportFileKindsBag.Value.SelectMany(kind => FileExtensionsForFileKind.Value[kind]).Distinct());
        private static readonly Lazy<IEnumerable<string>> SupportFileSuffixesData = new Lazy<IEnumerable<string>>(() => SupportFileKindsBag.Value.Select(kind => FileSuffixForFileKind.Value[kind]).Distinct());
        private static readonly Lazy<IEnumerable<string>> SupportFileSubdirectoriesData = new Lazy<IEnumerable<string>>(() => SupportFileKindsBag.Value.SelectMany(kind => FileSubdirectoriesForFileKind.Value[kind]).Distinct());

        #region Properties

        public static IEnumerable<ProgramFileKind> FileKinds
        {
            get
            {
                return FileExtensionsForFileKind.Value.Keys;
            }
        }

        /// <summary>
        /// Gets the user-defined ROM file extensions that use .cfg files.
        /// </summary>
        public static IEnumerable<string> RomFileExtensionsThatUseCfgFiles
        {
            get
            {
                foreach (var extension in RomsThatUseCfgFiles.Value)
                {
                    yield return extension;
                }
                lock (CustomRomExtensions.Value)
                {
                    foreach (var extension in CustomRomExtensions.Value)
                    {
                        yield return extension;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the kinds of support files.
        /// </summary>
        public static IEnumerable<ProgramFileKind> SupportFileKinds
        {
            get { return SupportFileKindsBag.Value; }
        }

        /// <summary>
        /// Gets an enumerable of support file suffixes.
        /// </summary>
        public static IEnumerable<string> SupportFileSuffixes
        {
            get
            {
                return SupportFileSuffixesData.Value;
            }
        }

        /// <summary>
        /// Gets an enumerable of the subdirectories in which support files may be found.
        /// </summary>
        public static IEnumerable<string> SupportFileSubdirectories
        {
            get
            {
                return SupportFileSubdirectoriesData.Value;
            }
        }

        /// <summary>
        /// Gets an enumerable of the file extensions for support files.
        /// </summary>
        public static IEnumerable<string> SupportFileExtensions
        {
            get
            {
                return SupportFileExtensionsData.Value;
            }
        }

        #endregion // Properties

        /// <summary>
        /// Get the suffix to append to a file name for a particular <see cref="ProgramFileKind"/>, such as a manual or overlay.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which a suffix is desired.</param>
        /// <returns>The suffix to use. May be empty.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="fileKind"/> does not support a suffix.</exception>
        public static string GetSuffix(this ProgramFileKind fileKind)
        {
            return FileSuffixForFileKind.Value[fileKind];
        }

        /// <summary>
        /// Get the subdirectories into which to store a support file for a particular <see cref="ProgramFileKind"/>, such as a manual or overlay.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which a subdirectory is desired.</param>
        /// <returns>The subdirectories that may be used. File kinds that refer to known ROM types, or are unrecognized, will return an empty enumerable.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="fileKind"/> is not valid.</exception>
        public static IEnumerable<string> GetSubdirectories(this ProgramFileKind fileKind)
        {
            return FileSubdirectoriesForFileKind.Value[fileKind];
        }

        /// <summary>
        /// Get the file extension for a program file.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which an extension is desired.</param>
        /// <returns>The file extensions. May be empty.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="fileKind"/> is not supported.</exception>
        public static IEnumerable<string> FileExtensions(this ProgramFileKind fileKind)
        {
            return FileExtensionsForFileKind.Value[fileKind];
        }

        /// <summary>
        /// Gets the default file extension for a program file kind.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which an extension is desired.</param>
        /// <returns>The file extension.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="fileKind"/> is not supported.</exception>
        public static string FileExtension(this ProgramFileKind fileKind)
        {
            return fileKind.FileExtensions().First();
        }

        /// <summary>
        /// Checks whether the file extension is valid for the given program file kind.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which the extension is to be validated.</param>
        /// <param name="fileName">The file name to be checked.</param>
        /// <returns>If the file has an extension that is valid for the given file kind, returns <c>true</c>; <c>false</c> otherwise.</returns>
        /// <remarks>Note that the <paramref name="fileName"/> argument should be exactly that - just the file name. If the name includes directory
        /// portions, the check may fail if any part of the entry contains a period.</remarks>
        public static bool HasCorrectExtension(this ProgramFileKind fileKind, string fileName)
        {
            var fileTypes = fileKind.FileExtensions();
            var extension = GetExtension(fileName);
            var hasStandardExtension = (extension != null) && fileTypes.Any(e => e.Equals(extension, System.StringComparison.OrdinalIgnoreCase));
            return hasStandardExtension;
        }

        /// <summary>
        /// Determines if the given file has a custom ROM extension.
        /// </summary>
        /// <returns><c>true</c> if the file has a custom rom extension; otherwise, <c>false</c>.</returns>
        /// <param name="fileKind">File kind.</param>
        /// <param name="fileName">File path.</param>
        /// <remarks>Note that the <paramref name="fileName"/> argument should be exactly that - just the file name. If the name includes directory
        /// portions, the check may fail if any part of the entry contains a period.</remarks>
        public static bool HasCustomRomExtension(this ProgramFileKind fileKind, string fileName)
        {
            var hasCustomRomExtension = false;
            if (fileName != null)
            {
                if (fileKind == ProgramFileKind.Rom)
                {
                    var extension = GetExtension(fileName);
                    if (extension == null)
                    {
                        extension = string.Empty;
                    }
                    lock (CustomRomExtensions.Value)
                    {
                        hasCustomRomExtension = CustomRomExtensions.Value.Any(e => e.Equals(extension, System.StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            return hasCustomRomExtension;
        }

        /// <summary>
        /// Adds a custom extension to recognize as a .bin format ROM that may use a .cfg file.
        /// </summary>
        /// <param name="fileKind">The file kind for which to add a custom extension.</param>
        /// <param name="extension">The custom file extension to associate with the given file kind.</param>
        public static void AddCustomExtension(this ProgramFileKind fileKind, string extension)
        {
            if ((fileKind == ProgramFileKind.Rom) && !string.IsNullOrEmpty(extension))
            {
                lock (CustomRomExtensions.Value)
                {
                    CustomRomExtensions.Value.Add(extension);
                }
            }
        }

        /// <summary>
        /// Removes the custom extension.
        /// </summary>
        /// <param name="fileKind">The file kind for which to remove a custom extension.</param>
        /// <param name="extension">The custom file extension to no longer associate with the given file kind.</param>
        public static void RemoveCustomExtension(this ProgramFileKind fileKind, string extension)
        {
            if ((fileKind == ProgramFileKind.Rom) && !string.IsNullOrEmpty(extension))
            {
                lock (CustomRomExtensions.Value)
                {
                    CustomRomExtensions.Value.Remove(extension);
                }
            }
        }

        private static string GetExtension(string filePath)
        {
            string extension = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                var indexOfLastPeriod = filePath.LastIndexOf('.');
                if (indexOfLastPeriod >= 0)
                {
                    extension = filePath.Substring(indexOfLastPeriod);
                }
                else
                {
                    extension = string.Empty;
                }
            }
            return extension;
        }

        private static ConcurrentDictionary<ProgramFileKind, IEnumerable<string>> GetFileExtensionsForFileKind()
        {
            var data = new Dictionary<ProgramFileKind, IEnumerable<string>>()
            {
                { ProgramFileKind.None, new[] { string.Empty } },
                { ProgramFileKind.Rom, new[] { ".rom", ".cc3", ".bin", ".itv", ".int", ".luigi" } },
                { ProgramFileKind.Box, new[] { ".jpg", ".bmp", ".png", ".gif" } },
                { ProgramFileKind.Label, new[] { ".jpg", ".bmp", ".png", ".gif" } },
                { ProgramFileKind.Overlay, new[] { ".jpg", ".bmp", ".png", ".gif" } },
                { ProgramFileKind.ManualCover, new[] { ".jpg", ".bmp", ".png", ".gif" } },
                { ProgramFileKind.ManualText, new[] { ".txt" } },
                { ProgramFileKind.SaveData, new[] { ".jlp" } },
                { ProgramFileKind.CfgFile, new[] { ".cfg" } },
                { ProgramFileKind.LuigiFile, new[] { ".luigi" } },
                { ProgramFileKind.Vignette, new[] { string.Empty } },
                { ProgramFileKind.GenericSupportFile, new[] { string.Empty } },
            };
            return new ConcurrentDictionary<ProgramFileKind, IEnumerable<string>>(data);
        }

        private static ConcurrentDictionary<ProgramFileKind, string> GetFileSuffixForFileKind()
        {
            var data = new Dictionary<ProgramFileKind, string>()
            {
                { ProgramFileKind.None, null },
                { ProgramFileKind.Rom, null },
                { ProgramFileKind.Box, "_box" },
                { ProgramFileKind.Label, "_label" },
                { ProgramFileKind.Overlay, "_overlay" },
                { ProgramFileKind.ManualCover, "_manual" },
                { ProgramFileKind.ManualText, string.Empty },
                { ProgramFileKind.SaveData, string.Empty },
                { ProgramFileKind.CfgFile, string.Empty },
                { ProgramFileKind.LuigiFile, string.Empty },
                { ProgramFileKind.Vignette, string.Empty },
                { ProgramFileKind.GenericSupportFile, string.Empty },
            };
            return new ConcurrentDictionary<ProgramFileKind, string>(data);
        }

        private static ConcurrentDictionary<ProgramFileKind, IEnumerable<string>> GetFileSubdirectoriesForFileKind()
        {
            var data = new Dictionary<ProgramFileKind, IEnumerable<string>>()
            {
                { ProgramFileKind.None, Enumerable.Empty<string>() },
                { ProgramFileKind.Rom, Enumerable.Empty<string>() },
                { ProgramFileKind.Box, new[] { "box", "boxes" } },
                { ProgramFileKind.Label, new[] { "label", "labels", "cart" } },
                { ProgramFileKind.Overlay, new[] { "overlay", "overlays" } },
                { ProgramFileKind.ManualCover, new[] { "manual", "manuals" } },
                { ProgramFileKind.ManualText, new[] { "manual", "manuals" } },
                { ProgramFileKind.SaveData, new[] { "savedata", "savegame", "savegames" } },
                { ProgramFileKind.CfgFile, Enumerable.Empty<string>() },
                { ProgramFileKind.LuigiFile, Enumerable.Empty<string>() },
                { ProgramFileKind.Vignette, Enumerable.Empty<string>() },
                { ProgramFileKind.GenericSupportFile, Enumerable.Empty<string>() },
            };
            return new ConcurrentDictionary<ProgramFileKind, IEnumerable<string>>(data);
        }
    }
}

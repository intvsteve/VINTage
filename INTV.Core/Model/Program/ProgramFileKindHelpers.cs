// <copyright file="ProgramFileKindHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
        public static readonly Dictionary<ProgramFileKind, List<string>> FileExtensionsForFileKind = new Dictionary<ProgramFileKind, List<string>>()
        {
            { ProgramFileKind.None, new List<string>() { string.Empty } },
            { ProgramFileKind.Rom, new List<string>() { ".rom", ".cc3", ".bin", ".itv", ".int", ".luigi" } },
            { ProgramFileKind.Box, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } },
            { ProgramFileKind.Label, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } },
            { ProgramFileKind.Overlay, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } },
            { ProgramFileKind.ManualCover, new List<string>() { ".jpg", ".bmp", ".png", ".gif" } },
            { ProgramFileKind.ManualText, new List<string>() { ".txt" } },
            { ProgramFileKind.SaveData, new List<string>() { ".jlp" } },
            { ProgramFileKind.CfgFile, new List<string>() { ".cfg" } },
            { ProgramFileKind.LuigiFile, new List<string>() { ".luigi" } },
            { ProgramFileKind.Vignette, new List<string>() { string.Empty } },
            { ProgramFileKind.GenericSupportFile, new List<string>() { string.Empty } },
        };

        /// <summary>
        /// Lookup table of a file kind to a file suffix for support files (box, label, overlay, or manual images, for example).
        /// </summary>
        public static readonly Dictionary<ProgramFileKind, string> FileSuffixForFileKind = new Dictionary<ProgramFileKind, string>()
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

        /// <summary>
        /// Lookup table of a file kind to a list of subdirectories in which the support file kind may found.
        /// </summary>
        public static readonly Dictionary<ProgramFileKind, List<string>> FileSubdirectoriesForFileKind = new Dictionary<ProgramFileKind, List<string>>()
        {
            { ProgramFileKind.None, null },
            { ProgramFileKind.Rom, null },
            { ProgramFileKind.Box, new List<string>() { "box", "boxes" } },
            { ProgramFileKind.Label, new List<string>() { "label", "labels", "cart" } },
            { ProgramFileKind.Overlay, new List<string>() { "overlay", "overlays" } },
            { ProgramFileKind.ManualCover, new List<string>() { "manual", "manuals" } },
            { ProgramFileKind.ManualText, new List<string>() { "manual", "manuals" } },
            { ProgramFileKind.SaveData, new List<string>() { "savedata", "savegame", "savegames" } },
            { ProgramFileKind.CfgFile, null },
            { ProgramFileKind.LuigiFile, null },
            { ProgramFileKind.Vignette, null },
            { ProgramFileKind.GenericSupportFile, null },
        };

        private static readonly List<string> RomsThatUseCfgFiles = new List<string>() { ".bin", ".itv", ".int" };

        private static readonly ProgramFileKind[] SupportFileKindsArray = new[] { ProgramFileKind.Box, ProgramFileKind.ManualCover, ProgramFileKind.ManualText, ProgramFileKind.Overlay, ProgramFileKind.Label, ProgramFileKind.SaveData, ProgramFileKind.CfgFile, ProgramFileKind.Vignette, ProgramFileKind.GenericSupportFile };

        private static HashSet<string> _customRomExtensions = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { string.Empty };
        private static IEnumerable<string> _supportFileExtensions;
        private static IEnumerable<string> _supportFileSuffixes;
        private static IEnumerable<string> _supportFileSubdirectories;

        #region Properties

        /// <summary>
        /// Gets the user-defined ROM file extensions that use .cfg files.
        /// </summary>
        public static IEnumerable<string> RomFileExtensionsThatUseCfgFiles
        {
            get
            {
                foreach (var extension in RomsThatUseCfgFiles)
                {
                    yield return extension;
                }
                foreach (var extension in _customRomExtensions)
                {
                    yield return extension;
                }
            }
        }

        /// <summary>
        /// Gets the kinds of support files.
        /// </summary>
        public static IEnumerable<ProgramFileKind> SupportFileKinds
        {
            get { return SupportFileKindsArray; }
        }

        /// <summary>
        /// Gets an enumerable of support file suffixes.
        /// </summary>
        public static IEnumerable<string> SupportFileSuffixes
        {
            get
            {
                if (_supportFileSuffixes == null)
                {
                    _supportFileSuffixes = SupportFileKindsArray.Select(kind => FileSuffixForFileKind[kind]).Distinct();
                }
                return _supportFileSuffixes;
            }
        }

        /// <summary>
        /// Gets an enumerable of the subdirectories in which support files may be found.
        /// </summary>
        public static IEnumerable<string> SupportFileSubdirectories
        {
            get
            {
                if (_supportFileSubdirectories == null)
                {
                    _supportFileSubdirectories = SupportFileKindsArray.SelectMany(kind => FileSubdirectoriesForFileKind[kind]).Distinct();
                }
                return _supportFileSubdirectories;
            }
        }

        /// <summary>
        /// Gets an enumerable of the file extensions for support files.
        /// </summary>
        private static IEnumerable<string> SupportFileExtensions
        {
            get
            {
                if (_supportFileExtensions == null)
                {
                    _supportFileExtensions = SupportFileKindsArray.SelectMany(kind => FileExtensionsForFileKind[kind]).Distinct();
                }
                return _supportFileExtensions;
            }
        }

        #endregion // Properties

        /// <summary>
        /// Get the file extension for a program file.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which an extension is desired.</param>
        /// <returns>The file extensions. May be empty.</returns>
        public static List<string> FileExtensions(this ProgramFileKind fileKind)
        {
            return FileExtensionsForFileKind[fileKind];
        }

        /// <summary>
        /// Gets the default file extension for a program file kind.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which an extension is desired.</param>
        /// <returns>The file extension.</returns>
        public static string FileExtension(this ProgramFileKind fileKind)
        {
            return fileKind.FileExtensions().First();
        }

        /// <summary>
        /// Checks whether the file extension is valid for the given program file kind.
        /// </summary>
        /// <param name="fileKind">The kind of program file for which the extension is to be validated.</param>
        /// <param name="filePath">The file path to be checked.</param>
        /// <returns>If the file has an extension that is valid for the given file kind, returns <c>true</c>; <c>false</c> otherwise.</returns>
        public static bool HasCorrectExtension(this ProgramFileKind fileKind, string filePath)
        {
            var fileTypes = fileKind.FileExtensions();
            var extension = GetExtension(filePath);
            var hasStandardExtension = (fileTypes != null) && (extension != null) && fileTypes.Any(e => e.Equals(extension, System.StringComparison.OrdinalIgnoreCase));
            return hasStandardExtension;
        }

        /// <summary>
        /// Determines if the given file has a custom ROM extension.
        /// </summary>
        /// <returns><c>true</c> if the file has a custom rom extension; otherwise, <c>false</c>.</returns>
        /// <param name="fileKind">File kind.</param>
        /// <param name="filePath">File path.</param>
        public static bool HasCustomRomExtension(this ProgramFileKind fileKind, string filePath)
        {
            var hasCustomRomExtension = false;
            if ((filePath != null) && (fileKind == ProgramFileKind.Rom))
            {
                var extension = GetExtension(filePath);
                if (extension == null)
                {
                    extension = string.Empty;
                }
                hasCustomRomExtension = _customRomExtensions.Any(e => e.Equals(extension, System.StringComparison.OrdinalIgnoreCase));
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
                _customRomExtensions.Add(extension);
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
                _customRomExtensions.Remove(extension);
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
    }
}

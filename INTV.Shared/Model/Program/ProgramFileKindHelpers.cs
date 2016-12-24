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

////#define ENABLE_DEBUG_OUTPUT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using INTV.Core.Model.Program;

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Extension methods for the ProgramFileKind enumeration.
    /// </summary>
    public static class ProgramFileKindHelpers
    {
        private static HashSet<string> _romFileBlacklist = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "Desktop DB" };

        public static void AddFileToBlacklist(string file)
        {
            _romFileBlacklist.Add(file);
        }

        public static void RemoveFileFromBlacklist(string file)
        {
            _romFileBlacklist.Remove(file);
        }

        /// <summary>
        /// Checks whether a file path is a program ROM file.
        /// </summary>
        /// <param name="fileKind">The kind of program file to check.</param>
        /// <param name="filePath">The file path to inspect to determine if it is of the indicated program file kind.</param>
        /// <returns><c>true</c> if the given file is of the given type.</returns>
        public static bool IsProgramFile(this ProgramFileKind fileKind, string filePath)
        {
            var isProgram = fileKind.IsProgramSupportFile(filePath, null);
            return isProgram;
        }

        /// <summary>
        /// Checks whether the file is of the desired kind for a program.
        /// </summary>
        /// <param name="fileKind">The kind of file to look for.</param>
        /// <param name="filePath">The file to check.</param>
        /// <param name="rootFile">When not null or empty, require the file name of the given file path to begin with this value.</param>
        /// <returns><c>true</c> if the file is a ROM file or a support file for the given root file.</returns>
        public static bool IsProgramSupportFile(this ProgramFileKind fileKind, string filePath, string rootFile)
        {
            // First, check if file is in the blacklist.
            if (!string.IsNullOrEmpty(filePath) && _romFileBlacklist.Contains(Path.GetFileName(filePath), INTV.Shared.Utility.PathComparer.Instance))
            {
                return false;
            }

            // Now, check if it has appropriate extension. If rootFile is not null or empty, check that FilePath has the same 'base name'.
            var hasExpectedExtension = fileKind.HasCorrectExtension(filePath);
            var hasCustomRomExtension = !hasExpectedExtension && fileKind.HasCustomRomExtension(filePath);
            if (hasCustomRomExtension)
            {
                DebugOutput("We have a custom extension!");
            }
            bool isProgramFile = (hasExpectedExtension || hasCustomRomExtension) && (string.IsNullOrEmpty(rootFile) || Path.GetFileName(filePath).StartsWith(Path.GetFileNameWithoutExtension(rootFile), StringComparison.InvariantCultureIgnoreCase));
            if (isProgramFile)
            {
                // Check if file name has proper suffix.
                var suffix = INTV.Core.Model.Program.ProgramFileKindHelpers.FileSuffixForFileKind[fileKind];
                isProgramFile = string.IsNullOrEmpty(suffix) || Path.GetFileNameWithoutExtension(filePath).EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);

                if (!isProgramFile)
                {
                    // Check if it's in a proper directory.
                    var subdirectoriesForFile = INTV.Core.Model.Program.ProgramFileKindHelpers.FileSubdirectoriesForFileKind[fileKind];
                    isProgramFile = (subdirectoriesForFile == null) || subdirectoriesForFile.Contains(Path.GetFileName(Path.GetDirectoryName(filePath)), INTV.Shared.Utility.PathComparer.Instance);
                }
            }
            return isProgramFile;
        }

        /// <summary>
        /// Given a file path, attempt to determine what kind of file it is.
        /// </summary>
        /// <param name="filePath">The file name to check.</param>
        /// <returns>The kind of program file the path points to.</returns>
        public static ProgramFileKind ToProgramFileKind(this string filePath)
        {
            var fileKind = ProgramFileKind.None;

            // Do this the easy way... just test each of the kinds.
            foreach (var kind in INTV.Core.Model.Program.ProgramFileKindHelpers.FileExtensionsForFileKind.Keys)
            {
                if (kind.IsProgramFile(filePath))
                {
                    fileKind = kind;
                    break;
                }
            }
            return fileKind;
        }

        /// <summary>
        /// Given a file path, attempt to locate a .cfg file for it, if appropriate.
        /// </summary>
        /// <param name="romFilePath">A path to a program ROM file.</param>
        /// <returns>The path to a ROM file's associated .cfg file, if one can be found.</returns>
        public static string GetConfigFilePath(this string romFilePath)
        {
            string configFile = null;
            var romFileExtension = Path.GetExtension(romFilePath);
            string extension = Core.Model.Program.ProgramFileKindHelpers.RomFileExtensionsThatUseCfgFiles.FirstOrDefault(ext => string.Compare(ext, romFileExtension, true) == 0);
            if (extension != null)
            {
#if WIN
                var searchPattern = "*" + ProgramFileKind.CfgFile.FileExtension();
                var filesNextToRom = Directory.EnumerateFiles(Path.GetDirectoryName(romFilePath), searchPattern);
#else
                // Workaround for non-Windows, though I suppose this is necessary if there's ever a Linux port.
                var searchPattern = ProgramFileKind.CfgFile.FileExtension();
                var files = Directory.EnumerateFiles(Path.GetDirectoryName(romFilePath));
                var filesNextToRom = files.Where(f => f.EndsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase));
#endif
                var possibleConfigFile = Path.Combine(Path.GetDirectoryName(romFilePath), Path.GetFileNameWithoutExtension(romFilePath) + ProgramFileKind.CfgFile.FileExtension());
                configFile = filesNextToRom.FirstOrDefault(f => string.Compare(f, possibleConfigFile, true) == 0);
                if (string.IsNullOrEmpty(configFile))
                {
                    possibleConfigFile = romFilePath + ProgramFileKind.CfgFile.FileExtension();
                    configFile = filesNextToRom.FirstOrDefault(f => string.Compare(f, possibleConfigFile, true) == 0);
                }
            }
            return configFile;
        }

        /// <summary>
        /// Get the program file kind, as well as the ROM file name, given a support file.
        /// </summary>
        /// <param name="supportFile">The support file to try to extract a program ROM file from, as well as the support file kind.</param>
        /// <param name="romFile">Set to the ROM file, or empty.</param>
        /// <returns>The kind of support file.</returns>
        public static ProgramFileKind GetFileKindAndBaseRomFileFromProgramSupportFile(this string supportFile, out string romFile)
        {
            romFile = string.Empty;
            var fileKind = supportFile.ToProgramFileKind();
            if (fileKind >= ProgramFileKind.SupportFile)
            {
                // We're a support file, so try to strip out stuff that doesn't belong.
                // First, try to strip out subdirectory.
                string file = Path.GetFileNameWithoutExtension(supportFile);
                string directory = Path.GetDirectoryName(supportFile);
                string subdirectory = Path.GetFileName(directory);
                if (INTV.Core.Model.Program.ProgramFileKindHelpers.SupportFileSubdirectories.Contains(subdirectory, INTV.Shared.Utility.PathComparer.Instance))
                {
                    directory = Path.GetDirectoryName(directory); // strip the subdirectory
                    romFile = Path.Combine(directory, file);
                }

                // Try to strip out any suffix that may be appended to the filename.
                foreach (var suffix in INTV.Core.Model.Program.ProgramFileKindHelpers.SupportFileSuffixes)
                {
                    if (!string.IsNullOrEmpty(suffix) && file.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        romFile = Path.Combine(directory,  file.Substring(0, file.Length - suffix.Length));
                        break;
                    }
                }
                if (string.IsNullOrEmpty(romFile))
                {
                    // We didn't do anything, but we have a valid support file. Do our best and just use the file w/o extension.
                    romFile = Path.Combine(directory, file);
                }
            }
            return fileKind;
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

// <copyright file="ProgramSupportFilesHelpers.cs" company="INTV Funhouse">
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
using System.Linq;
using System.Text;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Flags used to filter the set of files to include when enumerating files in <see cref="ProgramSupportFiles"/>.
    /// </summary>
    [System.Flags]
    public enum ProgramSupportFilesInclusionFlags : uint
    {
        /// <summary>
        /// Include the default set of files. This will be the minimum set of files necessary to
        /// completely define a ROM - the ROM image itself and possibly a configuration file path.
        /// </summary>
        Default = 0,

        /// <summary>
        /// When this flag is set, if a specific file kind supports multiple values, all values will be returned.
        /// For example, if a ROM has an alternate location for its files, or if multiple box images are available,
        /// all will be returned.
        /// </summary>
        Alternates = 1 << 0,

        /// <summary>
        /// Apply all inclusion flags.
        /// </summary>
        All = 0xFFFFFFFF,
    }

    /// <summary>
    /// This class provides helper methods for working with <see cref="ProgramSupportFiles"/>.
    /// </summary>
    public static class ProgramSupportFilesHelpers
    {
        /// <summary>
        /// Get an enumerable of the file paths for desired support files.
        /// </summary>
        /// <param name="files">The program support files whose file paths are to be enumerated.</param>
        /// <param name="kinds">The kinds of files to include in the enumeration.</param>
        /// <param name="inclusionFlags">Flags to determine which files to include, such as alternate values.</param>
        /// <returns>An enumerable of the files that were requested.</returns>
        /// <remarks>If the <paramref name="kinds"/> argument is <c>null</c> or empty, the function behaves as if it contains the
        /// <see cref="ProgramFileKind.Rom"/> and <see cref="ProgramFileKind.CfgFile"/> values.</remarks>
        public static IEnumerable<string> GetSupportFilePaths(this ProgramSupportFiles files, IEnumerable<ProgramFileKind> kinds, ProgramSupportFilesInclusionFlags inclusionFlags)
        {
            if ((kinds == null) || !kinds.Any())
            {
                kinds = new[] { ProgramFileKind.Rom, ProgramFileKind.CfgFile };
            }
            if (kinds.Contains(ProgramFileKind.Rom))
            {
                if (!string.IsNullOrEmpty(files.RomImagePath))
                {
                    yield return files.RomImagePath;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.AlternateRomImagePaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.CfgFile))
            {
                if (!string.IsNullOrEmpty(files.RomConfigurationFilePath))
                {
                    yield return files.RomConfigurationFilePath;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.AlternateRomConfigurationFilePaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.LuigiFile))
            {
                if (!string.IsNullOrEmpty(files.DefaultLtoFlashDataPath))
                {
                    yield return files.DefaultLtoFlashDataPath;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    // Access to full list of these is not public
                }
            }
            if (kinds.Contains(ProgramFileKind.ManualText))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.ManualPaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(files.DefaultManualTextPath))
                    {
                        yield return files.DefaultManualTextPath;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Box))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.BoxImagePaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(files.DefaultBoxImagePath))
                    {
                        yield return files.DefaultBoxImagePath;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Label))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.LabelImagePaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(files.DefaultLabelImagePath))
                    {
                        yield return files.DefaultLabelImagePath;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Overlay))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.OverlayImagePaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(files.DefaultOverlayImagePath))
                    {
                        yield return files.DefaultOverlayImagePath;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.ManualCover))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.ManualCoverImagePaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(files.DefaultManualImagePath))
                    {
                        yield return files.DefaultManualImagePath;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.SaveData))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.SaveDataPaths.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(files.DefaultSaveDataPath))
                    {
                        yield return files.DefaultSaveDataPath;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Vignette))
            {
                if (!string.IsNullOrEmpty(files.DefaultVignettePath))
                {
                    yield return files.DefaultVignettePath;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    // Access to full list of these is not public
                }
            }
            if (kinds.Contains(ProgramFileKind.GenericSupportFile))
            {
                if (!string.IsNullOrEmpty(files.DefaultReservedDataPath))
                {
                    yield return files.DefaultReservedDataPath;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    // Access to full list of these is not public
                }
            }
        }
    }
}

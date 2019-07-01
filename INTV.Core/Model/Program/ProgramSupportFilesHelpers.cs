// <copyright file="ProgramSupportFilesHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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
using INTV.Core.Utility;

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
        public static IEnumerable<StorageLocation> GetSupportFilePaths(this ProgramSupportFiles files, IEnumerable<ProgramFileKind> kinds, ProgramSupportFilesInclusionFlags inclusionFlags)
        {
            if ((kinds == null) || !kinds.Any())
            {
                kinds = new[] { ProgramFileKind.Rom, ProgramFileKind.CfgFile };
            }
            if (kinds.Contains(ProgramFileKind.Rom))
            {
                if (files.RomImageLocation.IsValid)
                {
                    yield return files.RomImageLocation;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.AlternateRomImageLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.CfgFile))
            {
                if (files.RomConfigurationLocation.IsValid)
                {
                    yield return files.RomConfigurationLocation;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.AlternateRomConfigurationLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.LuigiFile))
            {
                if (files.DefaultLtoFlashDataLocation.IsValid)
                {
                    yield return files.DefaultLtoFlashDataLocation;
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
                    foreach (var alternate in files.ManualLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (files.DefaultManualTextLocation.IsValid)
                    {
                        yield return files.DefaultManualTextLocation;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Box))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.BoxImageLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (files.DefaultBoxImageLocation.IsValid)
                    {
                        yield return files.DefaultBoxImageLocation;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Label))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.LabelImageLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (files.DefaultLabelImageLocation.IsValid)
                    {
                        yield return files.DefaultLabelImageLocation;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Overlay))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.OverlayImageLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (files.DefaultOverlayImageLocation.IsValid)
                    {
                        yield return files.DefaultOverlayImageLocation;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.ManualCover))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.ManualCoverImageLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (files.DefaultManualCoverImageLocation.IsValid)
                    {
                        yield return files.DefaultManualCoverImageLocation;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.SaveData))
            {
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    foreach (var alternate in files.SaveDataLocations.Where(p => p.IsValid))
                    {
                        yield return alternate;
                    }
                }
                else
                {
                    if (files.DefaultSaveDataLocation.IsValid)
                    {
                        yield return files.DefaultSaveDataLocation;
                    }
                }
            }
            if (kinds.Contains(ProgramFileKind.Vignette))
            {
                if (files.DefaultVignetteLocation.IsValid)
                {
                    yield return files.DefaultVignetteLocation;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    // Access to full list of these is not public
                }
            }
            if (kinds.Contains(ProgramFileKind.GenericSupportFile))
            {
                if (files.DefaultReservedDataLocation.IsValid)
                {
                    yield return files.DefaultReservedDataLocation;
                }
                if (inclusionFlags.HasFlag(ProgramSupportFilesInclusionFlags.Alternates))
                {
                    // Access to full list of these is not public
                }
            }
        }
    }
}

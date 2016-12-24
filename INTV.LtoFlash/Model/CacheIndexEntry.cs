// <copyright file="CacheIndexEntry.cs" company="INTV Funhouse">
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

using System;
using System.IO;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Describes an entry in the ROMs cache.
    /// </summary>
    public class CacheIndexEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.CacheIndexEntry"/> class.
        /// </summary>
        public CacheIndexEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.CacheIndexEntry"/> class.
        /// </summary>
        /// <param name="rom">The ROM described the entry.</param>
        /// <param name="romAbsolutePath">Absolute path to the ROM.</param>
        public CacheIndexEntry(IRom rom, string romAbsolutePath)
        {
            var romStagingArea = SingleInstanceApplication.Instance.GetConfiguration<Configuration>().RomsStagingAreaPath;

            RomPath = romAbsolutePath.Substring(romStagingArea.Length + 1);
            RomCrc32 = rom.Crc;
            RomSize = (uint)(new FileInfo(romAbsolutePath)).Length;

            if (rom.Format == RomFormat.Bin)
            {
                var cfgFilePath = Path.ChangeExtension(romAbsolutePath, ProgramFileKind.CfgFile.FileExtension());
                if (File.Exists(cfgFilePath))
                {
                    CfgPath = cfgFilePath.Substring(romStagingArea.Length + 1);
                    CfgCrc32 = INTV.Core.Utility.Crc32.OfFile(cfgFilePath);
                    CfgSize = (uint)(new FileInfo(cfgFilePath)).Length;
                }
            }

            var luigiFilePath = rom.GetLtoFlashFilePath();
            LuigiPath = luigiFilePath.Substring(romStagingArea.Length + 1);
            if (!System.IO.File.Exists(luigiFilePath) && LuigiFileHeader.PotentialLuigiFile(romAbsolutePath))
            {
                // This has been known to happen in the transition between naming conventions, but also arises generally when
                // a ROM in the main ROM list has always been a LUIGI file. We have an original LUIGI file,
                // and find a cache entry, and are updating it -- we may not have the newly named LUIGI file yet. So, just copy it.
                // What happens is, *in the cache* we found foo.luigi, but the cached file path has become foo_luigi.luigi.
                if (System.IO.File.Exists(romAbsolutePath))
                {
                    System.IO.File.Copy(romAbsolutePath, luigiFilePath, true);
                }
            }
            LuigiCrc24 = INTV.Core.Utility.Crc24.OfFile(luigiFilePath);
            LuigiSize = (uint)(new FileInfo(luigiFilePath)).Length;
        }

        /// <summary>
        /// Gets or sets the relative path to the ROM within the cache directory.
        /// </summary>
        public string RomPath { get; set; }

        /// <summary>
        /// Gets or sets the 32-bit CRC of the ROM's binary.
        /// </summary>
        public uint RomCrc32 { get; set; }

        /// <summary>
        /// Gets or sets the size of the ROM binary, in bytes.
        /// </summary>
        public uint RomSize { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the ROM's .cfg file in the cache directory, if applicable.
        /// </summary>
        public string CfgPath { get; set; }

        /// <summary>
        /// Gets or sets the 32-bit CRC of the ROM's .cfg file, if applicable.
        /// </summary>
        public uint CfgCrc32 { get; set; }

        /// <summary>
        /// Gets or sets the size of the ROM's .cfg file, in bytes, if applicable.
        /// </summary>
        public uint CfgSize { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the LUIGI file for the ROM in the cache directory.
        /// </summary>
        public string LuigiPath { get; set; }

        /// <summary>
        /// Gets or sets the UID (a 24-bit CRC) of the LUIGI file.
        /// </summary>
        /// <remarks>This value should match the UID of a corresponding fork in a Locutus File System.</remarks>
        public uint LuigiCrc24 { get; set; }

        /// <summary>
        /// Gets or sets the size of the LUIGI file, in bytes.
        /// </summary>
        public uint LuigiSize { get; set; }

        /// <summary>
        /// Call this to restore or recreate a missing .cfg file for an entry, if appropriate.
        /// </summary>
        /// <returns><c>true</c>, if the .cfg file was restored, <c>false</c> otherwise.</returns>
        public bool RestoreCfgFile()
        {
            var recreatedCfgFile = false;
            var extension = Path.GetExtension(RomPath);
            if (ProgramFileKindHelpers.RomFileExtensionsThatUseCfgFiles.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
            {
                var cacheDir = System.IO.Path.GetDirectoryName(CacheIndex.Path);
                var cfgPath = Path.Combine(cacheDir, Path.ChangeExtension(RomPath, ProgramFileKind.CfgFile.FileExtension()));
                if (!File.Exists(cfgPath))
                {
                    var programInfo = ProgramInformationTable.Default.FindProgram(RomCrc32);
                    var cfgFilePath = INTV.Shared.Model.IRomHelpers.GenerateStockCfgFile(RomCrc32, Path.Combine(cacheDir, RomPath), programInfo);
                    recreatedCfgFile = !string.IsNullOrEmpty(cfgFilePath);
                    if (recreatedCfgFile)
                    {
                        CfgCrc32 = INTV.Core.Utility.Crc32.OfFile(cfgFilePath);
                        CfgPath = cfgPath.Substring(cacheDir.Length + 1);
                        CfgSize = (uint)(new FileInfo(cfgFilePath)).Length;
                    }
                }
            }
            return recreatedCfgFile;
        }
    }
}

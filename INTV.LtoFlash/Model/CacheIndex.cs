// <copyright file="CacheIndex.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.JzIntv.Model;
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class creates a simple indexing mechanism to look up ROMs based on their LUIGI file 24-bit CRC.
    /// </summary>
    [System.Xml.Serialization.XmlRoot]
    public class CacheIndex
    {
        private CacheIndex()
        {
            Entries = new List<CacheIndexEntry>();
        }

        /// <summary>
        /// Gets the instance of the index service.
        /// </summary>
        public static CacheIndex Instance
        {
            get
            {
                if (_instance == null)
                {
                    var directory = System.IO.Path.GetDirectoryName(CacheIndex.Path);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }
                    if (File.Exists(CacheIndex.Path))
                    {
                        try
                        {
                            using (var fileStream = FileUtilities.OpenFileStream(CacheIndex.Path))
                            {
                                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CacheIndex), new Type[] { typeof(CacheIndexEntry) });
                                _instance = serializer.Deserialize(fileStream) as CacheIndex;
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Happens if file is somehow corrupt. If so, re-create it.
                        }
                    }
                    bool createEmptyIndex = _instance == null;
                    if (createEmptyIndex)
                    {
                        _instance = new CacheIndex();

                        // if there are any files in the cache, try to rebuild it.
                        var rebuildIndexErrorMessages = new List<string>();
                        var cacheEntries = GatherCacheData(System.IO.Path.GetDirectoryName(CacheIndex.Path), rebuildIndexErrorMessages);
                        foreach (var entry in cacheEntries)
                        {
                            _instance.AddEntry(entry, false);
                        }
                        _instance.Save();
                        ReportRebuildIndexErrors(rebuildIndexErrorMessages);
                    }
                }
                return _instance;
            }
        }
        private static CacheIndex _instance;

        /// <summary>
        /// Gets the path on disk to the index file.
        /// </summary>
        public static string Path
        {
            get
            {
                if (_path == null)
                {
                    _path = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<Configuration>().RomsStagingAreaIndexPath;
                }
                return _path;
            }
        }
        private static string _path;

        /// <summary>
        /// Gets or sets the entries in the index.
        /// </summary>
        public List<CacheIndexEntry> Entries { get; set; }

        /// <summary>
        /// Looks up an entry in the index in which the CRC and size both match.
        /// </summary>
        /// <param name="luigiCrc24">The 24-bit CRC of the LUIGI file.</param>
        /// <param name="luigiSize">The size of the LUIGI file, in bytes.</param>
        /// <returns>The matching entry in the index, or <c>null</c> if no match is found.</returns>
        public static CacheIndexEntry Find(uint luigiCrc24, uint luigiSize)
        {
            return Instance.FindEntry(luigiCrc24, luigiSize);
        }

        /// <summary>
        /// Looks up an entry in the index in which the original ROM 32-bit CRC matches the given value.
        /// </summary>
        /// <param name="romCrc32">The 32-bit CRC of the original ROM.</param>
        /// <returns>The matching entry in the index, or <c>null</c> if no match is found.</returns>
        public static CacheIndexEntry FindUsingRomCrc32(uint romCrc32)
        {
            return Instance.FindEntryUsingRomCrc32(romCrc32);
        }

        /// <summary>
        /// Gathers cache data to rebuild it if the index file is missing.
        /// </summary>
        /// <param name="cacheIndexDirectory">The director for which to rebuild the index.</param>
        /// <param name="rebuildIndexErrorMessages">Accumulates any errors encountered during the rebuild..</param>
        /// <returns>An enumerable containing index entries from the rebuild process.</returns>
        public static IEnumerable<CacheIndexEntry> GatherCacheData(string cacheIndexDirectory, IList<string> rebuildIndexErrorMessages)
        {
            var jzIntvConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.JzIntv.Model.Configuration>();
            var cachedRoms = (new[] { cacheIndexDirectory }).IdentifyRomFiles(() => false, f => { }).ToList();
            var cachedLuigiRoms = cachedRoms.Where(r => r.Format == RomFormat.Luigi).ToList();
            var nonLuigiRoms = cachedRoms.Except(cachedLuigiRoms).ToList();

            var restoredCacheEntries = new List<CacheIndexEntry>();
            var cacheDirectoryLength = cacheIndexDirectory.Length + 1; // for cutting backslash out later...

            using (var comparer = CanonicalRomComparerStrict.Default)
            {
                foreach (var cachedLuigiRom in cachedLuigiRoms)
                {
                    // Get the LUIGI header.
                    LuigiFileHeader luigiHeader = LuigiFileHeader.GetHeader(cachedLuigiRom.RomPath);

                    // Initialize the LUIGI part of the cache entry.
                    var cacheEntry = new CacheIndexEntry();
                    cacheEntry.LuigiPath = cachedLuigiRom.RomPath.Path.Substring(cacheDirectoryLength);
                    cacheEntry.LuigiCrc24 = INTV.Core.Utility.Crc24.OfFile(cachedLuigiRom.RomPath);
                    cacheEntry.LuigiSize = (uint)(new FileInfo(cachedLuigiRom.RomPath.Path)).Length;

                    // Now fetch or recreate the ROM if missing. RomFormat.None indicates that the
                    // original has been located on disk. Otherwise, indicates the format to recreate from the LUIGI file.
                    var originalRomFormat = RomFormat.None;

                    if (luigiHeader.Version > 0)
                    {
                        // LUIGI format 1 and later contains information about the original ROM.
                        cacheEntry.RomCrc32 = luigiHeader.OriginalRomCrc32;
                        var originalRom = nonLuigiRoms.FirstOrDefault(r => (r.Format == luigiHeader.OriginalRomFormat) && r.IsEquivalentTo(cachedLuigiRom, comparer));
                        if (originalRom != null)
                        {
                            cacheEntry.RomPath = originalRom.RomPath.Path.Substring(cacheDirectoryLength);
                            cacheEntry.RomSize = (uint)(new FileInfo(originalRom.RomPath.Path)).Length;
                            restoredCacheEntries.Add(cacheEntry);
                            nonLuigiRoms.RemoveAll(r => (r.Format == luigiHeader.OriginalRomFormat) && r.IsEquivalentTo(cachedLuigiRom, comparer));
                        }
                        else
                        {
                            // Write down the format to reconstruct.
                            originalRomFormat = luigiHeader.OriginalRomFormat;
                        }
                    }
                    else
                    {
                        // Check for a .bin or .rom file.
                        originalRomFormat = RomFormat.Rom;
                        var originalFile = System.IO.Path.ChangeExtension(cachedLuigiRom.RomPath.Path, originalRomFormat.FileExtension());
                        if (!File.Exists(originalFile))
                        {
                            originalRomFormat = RomFormat.Bin;
                            foreach (var binFormatExtension in ProgramFileKindHelpers.RomFileExtensionsThatUseCfgFiles)
                            {
                                if (!string.IsNullOrEmpty(binFormatExtension))
                                {
                                    originalFile = System.IO.Path.ChangeExtension(cachedLuigiRom.RomPath.Path, binFormatExtension);
                                }
                                else
                                {
                                    originalFile = cachedLuigiRom.RomPath.GetFileNameWithoutExtension();
                                }
                                if (File.Exists(originalFile))
                                {
                                    break;
                                }
                            }
                        }
                        if (File.Exists(originalFile))
                        {
                            cacheEntry.RomCrc32 = INTV.Core.Utility.Crc32.OfFile(new StorageLocation(originalFile));
                            cacheEntry.RomPath = originalFile.Substring(cacheDirectoryLength);
                            cacheEntry.RomSize = (uint)(new FileInfo(originalFile)).Length;

                            if (originalRomFormat == RomFormat.Bin)
                            {
                                var cfgPath = cachedLuigiRom.RomPath.ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                                if (!cfgPath.Exists())
                                {
                                    cacheEntry.RestoreCfgFile();
                                }
                            }

                            restoredCacheEntries.Add(cacheEntry);
                            nonLuigiRoms.RemoveAll(r => (r.Format == originalRomFormat) && r.IsEquivalentTo(cachedLuigiRom, comparer));
                            originalRomFormat = RomFormat.None;
                        }
                    }

                    if (originalRomFormat != RomFormat.None)
                    {
                        // Need to recreate the ROM from LUIGI.
                        var sourcePath = "\"" + cachedLuigiRom.RomPath.GetFileNameWithoutExtension() + "\"";
                        var workingDir = cachedLuigiRom.RomPath.GetContainingLocation();
                        var conversionApps = jzIntvConfiguration.GetConverterApps(cachedLuigiRom, luigiHeader.OriginalRomFormat);
                        var conversionResult = 0;
                        foreach (var conversionApp in conversionApps)
                        {
                            var argument = luigiHeader.OriginalRomFormat.GetCommandLineArgForBin2Rom() + sourcePath;
                            conversionResult = INTV.Shared.Utility.RunExternalProgram.Call(conversionApp.Item1, argument, workingDir.Path);
                            if (conversionResult != 0)
                            {
                                rebuildIndexErrorMessages.Add("Failed to reconstruct " + sourcePath + luigiHeader.OriginalRomFormat.FileExtension());
                                break;
                            }
                        }
                        if (conversionResult == 0)
                        {
                            cacheEntry.RomPath = System.IO.Path.ChangeExtension(cacheEntry.LuigiPath, luigiHeader.OriginalRomFormat.FileExtension());
                            cacheEntry.RomSize = (uint)(new FileInfo(System.IO.Path.Combine(cacheIndexDirectory, cacheEntry.RomPath))).Length;

                            if (originalRomFormat == RomFormat.Bin)
                            {
                                var cfgPath = cachedLuigiRom.RomPath.ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                                if (!cfgPath.Exists())
                                {
                                    cacheEntry.RestoreCfgFile();
                                }
#if false
                            if (File.Exists(cfgPath))
                            {
                                cacheEntry.CfgCrc32 = INTV.Core.Utility.Crc32.OfFile(cfgPath);
                                cacheEntry.CfgPath = cfgPath.Substring(cacheDirectoryLength);
                                cacheEntry.CfgSize = (uint)(new FileInfo(cfgPath)).Length;
                            }
#endif // false
                            }

                            restoredCacheEntries.Add(cacheEntry);
                        }
                    }
                }

                foreach (var nonLuigiRom in nonLuigiRoms)
                {
                    var cacheEntry = new CacheIndexEntry();
                    cacheEntry.RomPath = nonLuigiRom.RomPath.Path.Substring(cacheDirectoryLength);
                    cacheEntry.RomCrc32 = nonLuigiRom.Crc;
                    cacheEntry.RomSize = (uint)(new FileInfo(nonLuigiRom.RomPath.Path)).Length;

                    if (nonLuigiRom.Format == RomFormat.Bin)
                    {
                        var cfgPath = nonLuigiRom.RomPath.ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                        if (!cfgPath.Exists())
                        {
                            cacheEntry.RestoreCfgFile();
#if false
                        var programInfo = ProgramInformationTable.Default.FindProgram(cacheEntry.RomCrc32);
                        var cfgFilePath = nonLuigiRom.GenerateStockCfgFile(programInfo);
                        if (string.Compare(cfgFilePath, cfgPath, true) != 0)
                        {
                            System.Diagnostics.Debug.WriteLine("LSDKFLSKDFJLSJDF");
                        }
#endif // false
                        }
#if false
                    if (File.Exists(cfgPath))
                    {
                        cacheEntry.CfgCrc32 = INTV.Core.Utility.Crc32.OfFile(cfgPath);
                        cacheEntry.CfgPath = cfgPath.Substring(cacheDirectoryLength);
                        cacheEntry.CfgSize = (uint)(new FileInfo(cfgPath)).Length;
                    }
#endif // false
                    }

                    var sourcePath = nonLuigiRom.RomPath.GetFileNameWithoutExtension();
                    var workingDir = System.IO.Path.GetDirectoryName(nonLuigiRom.RomPath.Path);
                    var conversionApp = jzIntvConfiguration.GetConverterApps(nonLuigiRom, RomFormat.Luigi).First();
                    var result = INTV.Shared.Utility.RunExternalProgram.Call(conversionApp.Item1, "\"" + sourcePath + "\"", workingDir);
                    if (result != 0)
                    {
                        rebuildIndexErrorMessages.Add(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RestoreCacheIndex_FailedToReconstructFormat, sourcePath + RomFormat.Luigi.FileExtension()));
                    }
                    else
                    {
                        cacheEntry.LuigiPath = System.IO.Path.ChangeExtension(cacheEntry.RomPath, RomFormat.Luigi.FileExtension());
                        var fullLuigiPath = System.IO.Path.Combine(cacheIndexDirectory, cacheEntry.LuigiPath);
                        cacheEntry.LuigiCrc24 = INTV.Core.Utility.Crc24.OfFile(new StorageLocation(fullLuigiPath));
                        cacheEntry.LuigiSize = (uint)(new FileInfo(fullLuigiPath)).Length;
                        restoredCacheEntries.Add(cacheEntry);
                    }
                }
            }

            return restoredCacheEntries;
        }

        /// <summary>
        /// Adds an entry to the index if appropriate.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        public void AddEntry(CacheIndexEntry entry)
        {
            AddEntry(entry, true);
        }

        /// <summary>
        /// Finds an entry in the index, if possible.
        /// </summary>
        /// <param name="luigiCrc24">The 24-bit CRC of the LUIG file.</param>
        /// <param name="luigiSize">The size, in bytes, of the LUIGI file.</param>
        /// <returns>The cache index entry matching the given criteria.</returns>
        public CacheIndexEntry FindEntry(uint luigiCrc24, uint luigiSize)
        {
            var match = Entries.FirstOrDefault(e => (e.LuigiCrc24 == luigiCrc24) && (e.LuigiSize == luigiSize));
            if ((match != null) && match.RestoreCfgFile())
            {
                Save();
            }
            return match;
        }

        /// <summary>
        /// Finds an entry in the index, if possible.
        /// </summary>
        /// <param name="romCrc32">The 32-bit CRC of the original ROM to locate.</param>
        /// <returns>The cache index entry matching the given criteria.</returns>
        public CacheIndexEntry FindEntryUsingRomCrc32(uint romCrc32)
        {
            var match = Entries.FirstOrDefault(e => e.RomCrc32 == romCrc32);
            return match;
        }

        private static void ReportRebuildIndexErrors(IList<string> rebuildIndexErrorMessages)
        {
            if (rebuildIndexErrorMessages.Any())
            {
                SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() =>
                    {
                        var message = Resources.Strings.RestoreCacheIndex_ErrorMessage;
                        var failedRestoreBuilder = new System.Text.StringBuilder();
                        foreach (var errorMessage in rebuildIndexErrorMessages)
                        {
                            failedRestoreBuilder.AppendLine(errorMessage);
                        }
                        var errorDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.RestoreCacheIndex_ErrorTitle, message);
                        errorDialog.ReportText = failedRestoreBuilder.ToString();
                        errorDialog.ShowSendEmailButton = false;
                        errorDialog.ShowDialog(Resources.Strings.Close);
                    });
            }
        }

        private void AddEntry(CacheIndexEntry entry, bool save)
        {
            var anyMatching = Entries.Any(e => (e.LuigiCrc24 == entry.LuigiCrc24) && (e.LuigiSize == entry.LuigiSize) && (e.RomCrc32 == entry.RomCrc32) && (e.RomSize == entry.RomSize));
            if (!anyMatching)
            {
                Entries.Add(entry);
                if (save)
                {
                    Save();
                }
            }
        }

        private void Save()
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CacheIndex), new Type[] { typeof(CacheIndex) });
            using (var fileStream = new FileStream(CacheIndex.Path, FileMode.Create))
            {
                serializer.Serialize(fileStream, this);
            }
        }
    }
}

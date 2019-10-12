// <copyright file="IRomHelpers.cs" company="INTV Funhouse">
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

////#define REPORT_PERFORMANCE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Shared.CompressedArchiveAccess;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;

namespace INTV.Shared.Model
{
    /// <summary>
    /// Extension methods to assist with working with IRom.
    /// </summary>
    public static class IRomHelpers
    {
#if REPORT_PERFORMANCE
        private static TimeSpan _accumulatedCacheCheckTime = TimeSpan.Zero;
        private static TimeSpan _accumulatedGetPathTime = TimeSpan.Zero;
        private static TimeSpan _accumulatedGetCrcsTime = TimeSpan.Zero;
        private static TimeSpan _accumulatedCacheRecheckTime = TimeSpan.Zero;
#endif // REPORT_PERFORMANCE

        /// <summary>
        /// Reset internal performance timing data.
        /// </summary>
        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        public static void ResetAccumulatedTimes()
        {
#if REPORT_PERFORMANCE
            _accumulatedCacheCheckTime = TimeSpan.Zero;
            _accumulatedGetPathTime = TimeSpan.Zero;
            _accumulatedGetCrcsTime = TimeSpan.Zero;
            _accumulatedCacheRecheckTime = TimeSpan.Zero;
            Rom.AccumulatedRefreshCrcsTime = TimeSpan.Zero;
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Report performance data to a logger or debug output.
        /// </summary>
        /// <param name="logger">A logger to record into. May be <c>null</c>.</param>
        /// <param name="prefix">Prefix to include in each line of the output.</param>
        /// <remarks>If <param name="logger"/> is <c>null</c>, output is reported to debug output.</remarks>
        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        public static void ReportAccumulatedTimes(Logger logger, string prefix)
        {
            Action<string> logIt = (o) => System.Diagnostics.Debug.WriteLine(o.ToString());
            if (logger == null)
            {
                logIt = logger.Log;
            }
            prefix = prefix == null ? string.Empty : prefix + " ";
#if REPORT_PERFORMANCE
            logIt(prefix + "Total   Prepare.CacheLookup.Total ------: " + _accumulatedCacheCheckTime.ToString());
            logIt(prefix + "Total   Prepare.CacheLookup.GetPath ----: " + _accumulatedGetPathTime.ToString());
            logIt(prefix + "Total   Prepare.CacheLookup.GetCrcs ----: " + _accumulatedGetCrcsTime.ToString());
            logIt(prefix + "Total    Prepare.CacheLookup.Rom.GetCrcs: " + Rom.AccumulatedRefreshCrcsTime.ToString());
            logIt(prefix + "Total   Prepare.CacheLookup.CacheRecheck: " + _accumulatedCacheRecheckTime.ToString());
#else
            logIt(prefix + "REPORT_PERFORMANCE has not been #defined in:" + typeof(IRomHelpers).FullName);
#endif // REPORT_PERFORMANCE
        }

        #region Comparison

        /// <summary>
        /// Compares two ROMs using the specified compare implementation.
        /// </summary>
        /// <param name="rom">The 'reference' ROM to compare against.</param>
        /// <param name="otherRom">The ROM to compare to <paramref name="rom"/>.</param>
        /// <param name="comparer">The comparison implementation to use.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> and <paramref name="otherRom"/> are considered equal using <paramref name="comparer"/>.</returns>
        public static bool IsEquivalentTo(this IRom rom, IRom otherRom, RomComparer comparer)
        {
            return rom.IsEquivalentTo(otherRom, null, comparer);
        }

        /// <summary>
        /// Compares two ROMs using the specified compare implementation.
        /// </summary>
        /// <param name="rom">The 'reference' ROM to compare against.</param>
        /// <param name="otherRom">The ROM to compare to <paramref name="rom"/>.</param>
        /// <param name="otherProgramInfo">Pre-fetched program information, which may make certain comparisons slightly faster.</param>
        /// <param name="comparer">The comparison implementation to use.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> and <paramref name="otherRom"/> are considered equal using <paramref name="comparer"/>.</returns>
        public static bool IsEquivalentTo(this IRom rom, IRom otherRom, IProgramInformation otherProgramInfo, RomComparer comparer)
        {
            var result = comparer.Compare(rom, null, otherRom, otherProgramInfo);
            var equivalent = result == 0;
            return equivalent;
        }

        /// <summary>
        /// Compares two ROMs using the specified compare implementation.
        /// </summary>
        /// <param name="rom">The 'reference' ROM to compare against.</param>
        /// <param name="otherRom">The ROM to compare to <paramref name="rom"/>.</param>
        /// <param name="comparisonMode">One of the <see cref="RomComparison"/> mode values. Based on this value, the appropriate default implementation of <see cref="RomComparer"/> will be used.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> and <paramref name="otherRom"/> are considered equal using <paramref name="comparisonMode"/>.</returns>
        public static bool IsEquivalentTo(this IRom rom, IRom otherRom, RomComparison comparisonMode)
        {
            return rom.IsEquivalentTo(otherRom, null, comparisonMode);
        }

        /// <summary>
        /// Inspects two ROMs and determines if they are the same ROM, to the best it is capable of doing.
        /// </summary>
        /// <param name="rom">The reference ROM.</param>
        /// <param name="otherRom">The ROM to compare with.</param>
        /// <param name="programInfo">Program information used for comparison.</param>
        /// <param name="comparisonMode">Specifies the <see cref="RomComparison"/> mode to use to determine equivalence.</param>
        /// <returns>If the two ROMs are equivalent, returns <c>true</c>, otherwise returns <c>false</c>.</returns>
        /// <remarks>Because the same ROM may be packaged in up to three different file formats at this time (.bin + .cfg, .rom, and .luigi), it can be difficult to
        /// determine if two ROMs are, indeed, the same. The <paramref name="comparisonMode"/> defines how to compare the two ROMs</remarks>
        public static bool IsEquivalentTo(this IRom rom, IRom otherRom, IProgramInformation programInfo, RomComparison comparisonMode)
        {
            var equivalent = object.ReferenceEquals(rom, otherRom);
            if (!equivalent)
            {
                var comparer = RomComparer.GetComparer(comparisonMode);
                equivalent = rom.IsEquivalentTo(otherRom, programInfo, comparer);
            }
            return equivalent;
        }

        #endregion // Comparison

        #region Copy to Local ROMs Directory

        /// <summary>
        /// Copies the files in an instance of IRom to the local ROMs directory, returning a new instance of <see cref="IRom"/> for the new location.
        /// </summary>
        /// <param name="rom">The ROM to copy to the local ROMs directory.</param>
        /// <returns>A new <see cref="IRom"/> using the new paths to which the original ROM was copied.</returns>
        public static IRom CopyToLocalRomsDirectory(this IRom rom)
        {
            return rom.CopyToLocalRomsDirectory(null);
        }

        /// <summary>
        /// Copies the files in an instance of IRom to the local ROMs directory, returning a new instance of <see cref="IRom"/> for the new location.
        /// </summary>
        /// <param name="rom">The ROM to copy to the local ROMs directory.</param>
        /// <param name="destinationPath">Destination directory for the copy operation.</param>
        /// <returns>A new <see cref="IRom"/> using the new paths to which the original ROM was copied.</returns>
        public static IRom CopyToLocalRomsDirectory(this IRom rom, string destinationPath)
        {
            var currentRomPath = rom.RomPath;
            var localRomPath = new StorageLocation(destinationPath);
            var romNeedsCopy = currentRomPath.IsLocalCopyNeeded(ref localRomPath);
            if (romNeedsCopy)
            {
                localRomPath = localRomPath.EnsureUnique();
            }
            var currentCfgPath = rom.ConfigPath;
            if (!currentCfgPath.IsValid)
            {
                currentCfgPath = rom.GetStockCfgFile(null);
            }
            var localCfgPath = new StorageLocation((destinationPath == null) ? null : Path.ChangeExtension(destinationPath, ProgramFileKind.CfgFile.FileExtension()));
            var cfgNeedsCopy = currentCfgPath.IsValid && currentCfgPath.IsLocalCopyNeeded(ref localCfgPath);
            if (cfgNeedsCopy)
            {
                localCfgPath = localRomPath.ChangeExtension(ProgramFileKind.CfgFile.FileExtension()); // preserve unique name if needed
            }
            if (romNeedsCopy)
            {
                currentRomPath.CopyFile(localRomPath, true);
            }
            if (cfgNeedsCopy)
            {
                currentCfgPath.CopyFile(localCfgPath, true);
            }
            rom = INTV.Core.Model.Rom.Create(localRomPath, localCfgPath);
            return rom;
        }

        /// <summary>
        /// Checks whether the given <see cref="IRom"/> refers to a stock configuration file path.
        /// </summary>
        /// <param name="rom">The <see cref="IRom"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> refers to a stock .cfg file, rather than one residing next to the ROM.</returns>
        /// <remarks>This function will always return <c>false</c> for non-.bin-format ROMs.</remarks>
        public static bool IsUsingStockCfgFilePath(this IRom rom)
        {
            var isStockCfgFilePath = false;
            if (rom.ConfigPath.IsValid && (rom.RomPath.GetContainingLocation().CompareTo(rom.ConfigPath.GetContainingLocation()) != 0))
            {
                var jzIntv = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<JzIntv.Model.Configuration>();
                isStockCfgFilePath = PathComparer.Instance.Compare(Path.GetDirectoryName(rom.ConfigPath.Path), jzIntv.DefaultToolsDirectory) == 0;
            }
            return isStockCfgFilePath;
        }

        private static bool IsLocalCopyNeeded(this StorageLocation location, ref StorageLocation localLocation)
        {
            var localRomsDirectory = RomListConfiguration.Instance.RomsDirectory;
            var fileName = localLocation.GetFileName() ?? location.GetFileName();
            localLocation = new StorageLocation(Path.Combine(localRomsDirectory, fileName));
            var copyNeeded = !localLocation.Exists(); // if not present, we'll need to copy
            if (!copyNeeded)
            {
                // file present -- see if it's different
                copyNeeded = INTV.Core.Utility.Crc32.OfFile(localLocation) != INTV.Core.Utility.Crc32.OfFile(location);
            }
            return copyNeeded;
        }

        #endregion // Copy to Local ROMs Directory

        /// <summary>
        /// Shows the file system file chooser to select one or more ROM files.
        /// </summary>
        /// <param name="multiselect">If <c>true</c>, instructs the file browser to allow the user to select multiple files.</param>
        /// <returns>The files selected, or an empty enumerable.</returns>
        public static IEnumerable<string> BrowseForRoms(bool multiselect)
        {
            return BrowseForRoms(multiselect, null);
        }

        /// <summary>
        /// Shows the file system file chooser to select one or more ROM files.
        /// </summary>
        /// <param name="multiselect">If <c>true</c>, instructs the file browser to allow the user to select multiple files.</param>
        /// <param name="prompt">The prompt to show in the file browser. If <c>null</c> or empty, a default will be used.</param>
        /// <returns>The files selected, or an empty enumerable.</returns>
        /// <remarks>This should not be in the Model namespace - it should be in <see cref="FileDialogHelpers"/>.
        /// <see href="https://github.com/intvsteve/VINTage/issues/331"/></remarks>
        public static IEnumerable<string> BrowseForRoms(bool multiselect, string prompt)
        {
            var selectedFiles = System.Linq.Enumerable.Empty<string>();
            var fileBrowser = FileDialogHelpers.Create();
            fileBrowser.IsFolderBrowser = false;
            fileBrowser.AddFilter(Resources.Strings.FileDialog_SelectRomFilesFilter, ProgramFileKind.Rom.FileExtensions());
            fileBrowser.AddFilter(FileDialogHelpers.AllFilesFilter, new string[] { ".*" });
            if (!string.IsNullOrEmpty(prompt))
            {
                fileBrowser.Title = prompt;
            }
            else
            {
                fileBrowser.Title = multiselect ? Resources.Strings.FileDialog_SelectFilesPrompt : Resources.Strings.FileDialog_SelectFilePrompt;
            }
            fileBrowser.EnsureFileExists = true;
            fileBrowser.EnsurePathExists = true;
            fileBrowser.Multiselect = multiselect;
            var result = fileBrowser.ShowDialog();
            if (result == FileBrowserDialogResult.Ok)
            {
                selectedFiles = fileBrowser.FileNames;
            }
            return selectedFiles;
        }

        /// <summary>
        /// Generate a stock .cfg file for a ROM given its program information.
        /// </summary>
        /// <param name="rom">The ROM for which a config file is needed.</param>
        /// <param name="programInfo">The program information for the ROM.</param>
        /// <returns>The absolute path to the config file for the ROM.</returns>
        public static StorageLocation GenerateStockCfgFile(this IRom rom, IProgramInformation programInfo)
        {
            StorageLocation cfgFilePath = StorageLocation.InvalidLocation;
            if ((rom.Format == RomFormat.Bin) && !rom.ConfigPath.Exists())
            {
                cfgFilePath = GenerateStockCfgFile(rom.Crc, rom.RomPath, programInfo);
            }
            return cfgFilePath;
        }

        /// <summary>
        /// Generate a stock configuration file for a ROM based on the file's CRC.
        /// </summary>
        /// <param name="crc">The CRC32 of the ROM.</param>
        /// <param name="romPath">The absolute path to the ROM file.</param>
        /// <param name="programInfo">The program information for the ROM.</param>
        /// <returns>The absolute path to the config file for the ROM.</returns>
        public static StorageLocation GenerateStockCfgFile(uint crc, StorageLocation romPath, IProgramInformation programInfo)
        {
            StorageLocation cfgFilePath = StorageLocation.InvalidLocation;
            var stockCfgFilePath = Core.Model.IRomHelpers.GetStockCfgFile(crc, romPath, programInfo);
            if (stockCfgFilePath.Exists())
            {
                System.Exception exception = null;
                try
                {
                    cfgFilePath = romPath.ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                    File.Copy(stockCfgFilePath.Path, cfgFilePath.Path, true);
                }
                catch (System.NotSupportedException e)
                {
                    exception = e;
                }
                catch (IOException e)
                {
                    exception = e;
                }
                catch (System.UnauthorizedAccessException e)
                {
                    exception = e;
                }
                if (exception != null)
                {
                    var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.GenerateStockCfgFileFailed_MessageFormat, programInfo.Title);
                    INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.GenerateStockCfgFileFailed_Title, exception, INTV.Shared.View.OSMessageBoxButton.OK);
                }
            }
            return cfgFilePath;
        }

        /// <summary>
        /// Gets the fully qualified path of the staging area directory for the ROM.
        /// </summary>
        /// <param name="rom">The ROM whose staging area is requested.</param>
        /// <param name="baseRomStagingArea">The base directory for the staging area.</param>
        /// <returns>The fully qualified path to the staging area.</returns>
        /// <remarks>The staging area directory will be created if it does not exist. The staging area is a directory path generated
        /// from the ROM's original file system location.</remarks>
        public static string GetStagingAreaPath(this IRom rom, string baseRomStagingArea)
        {
            var stagingAreaDirectory = System.IO.Path.GetDirectoryName(rom.RomPath.Path).GetHashCode().ToString("x8");
            var isAlternateRom = rom.IsAlternateRom();
            if (isAlternateRom)
            {
                stagingAreaDirectory = System.IO.Path.GetDirectoryName(rom.OriginalRom().RomPath.Path).GetHashCode().ToString("x8");
            }
            var stagingAreaPath = System.IO.Path.Combine(baseRomStagingArea, stagingAreaDirectory);
            if (!System.IO.Directory.Exists(stagingAreaPath))
            {
                System.IO.Directory.CreateDirectory(stagingAreaPath);
            }
            return stagingAreaPath;
        }

        /// <summary>
        /// Gets the fully qualified path to the cached ROM file.
        /// </summary>
        /// <param name="rom">The ROM whose cached file path is requested.</param>
        /// <param name="romStagingAreaPath">The staging area directory for the ROM.</param>
        /// <returns>The fully qualified path to the cached ROM file.</returns>
        public static StorageLocation GetCachedRomFilePath(this IRom rom, string romStagingAreaPath)
        {
            var cachedRomPath = rom.RomPath.AlterContainingLocation(romStagingAreaPath);

            // If the source file is lying about it's file extension, patch that up, too.
            var defaultExtension = rom.Format.FileExtension();
            if (cachedRomPath.GetExtension() != defaultExtension)
            {
                cachedRomPath = cachedRomPath.ChangeExtension(defaultExtension);
            }
            return cachedRomPath;
        }

        /// <summary>
        /// Gets the fully qualified path to the cached ROM file's configuration file, if applicable.
        /// </summary>
        /// <param name="rom">The ROM whose cached configuration file is requested.</param>
        /// <param name="romStagingAreaPath">The staging area directory for the ROM.</param>
        /// <returns>The fully qualified path to the cached ROM's configuration file. If the ROM does not
        /// have a configuration file, then <c>null</c> is returned.</returns>
        public static StorageLocation GetCachedConfigFilePath(this IRom rom, string romStagingAreaPath)
        {
            // NOTE: BUG ? : There's a goofy thing in that files w/o a cfg use the bin path... why?
            var cachedConfigPath = StorageLocation.InvalidLocation;
            if (rom.ConfigPath.IsValid && rom.RomPath.IsValid && (rom.RomPath != rom.ConfigPath))
            {
                cachedConfigPath = new StorageLocation(System.IO.Path.Combine(romStagingAreaPath, rom.ConfigPath.GetFileName()));
                if (rom.IsUsingStockCfgFilePath())
                {
                    cachedConfigPath = new StorageLocation(Path.Combine(romStagingAreaPath, rom.RomPath.GetFileName())).ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                }
            }
            return cachedConfigPath;
        }

        /// <summary>
        /// Gets the fully qualified path to the expected output file produced by the conversion tool.
        /// </summary>
        /// <param name="rom">The ROM whose conversion output file is desired.</param>
        /// <param name="romStagingAreaPath">The staging area directory for the ROM.</param>
        /// <param name="programFileKind">Target program file kind.</param>
        /// <returns>The fully qualified path to the expected output file.</returns>
        public static StorageLocation GetOutputFilePath(this IRom rom, string romStagingAreaPath, INTV.Core.Model.Program.ProgramFileKind programFileKind)
        {
            return rom.GetOutputFilePath(romStagingAreaPath, programFileKind, true);
        }

        /// <summary>
        /// Gets the fully qualified path to the expected output file produced by the conversion tool.
        /// </summary>
        /// <param name="rom">The ROM whose conversion output file is desired.</param>
        /// <param name="romStagingAreaPath">The staging area directory for the ROM.</param>
        /// <param name="programFileKind">Target program file kind.</param>
        /// <param name="includeOriginalFormat">If <c>true</c>, include the original ROM file format as part of the output file name.</param>
        /// <returns>The fully qualified path to the expected output file.</returns>
        public static StorageLocation GetOutputFilePath(this IRom rom, string romStagingAreaPath, INTV.Core.Model.Program.ProgramFileKind programFileKind, bool includeOriginalFormat)
        {
            var cachedPath = rom.GetCachedRomFilePath(romStagingAreaPath);
            var extension = cachedPath.GetExtension();
            if (includeOriginalFormat)
            {
                var originalExtension = rom.Format.FileExtension();
                var suffix = originalExtension.Replace('.', '_');
                cachedPath = cachedPath.AddSuffix(suffix + originalExtension);
            }
            cachedPath = cachedPath.ChangeExtension(extension);
            var path = cachedPath.ChangeExtension(programFileKind.FileExtension());
            return path;
        }

        /// <summary>
        /// Determines if the source ROM is already in the cache.
        /// </summary>
        /// <param name="rom">The ROM whose presence in the cache is being tested.</param>
        /// <param name="romStagingAreaPath">The staging area directory for the ROM.</param>
        /// <param name="changed">Set to <c>true</c> if the ROM is different than what's in the cache.</param> 
        /// <returns><c>true</c> if all necessary elements for the ROM are in the cache.</returns>
        /// <remarks>To be considered current in the cache, the source and cached files must have the same CRC32 checksum. If a ROM also
        /// has a configuration file, the CRC32 checksums of the configuration files must also match.</remarks>
        public static bool IsInCache(this IRom rom, string romStagingAreaPath, out bool changed)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            changed = true;
            var cachedRomPath = rom.GetCachedRomFilePath(romStagingAreaPath);
            bool fileInCache = cachedRomPath.Exists();
#if REPORT_PERFORMANCE
            stopwatch2.Stop();
            _accumulatedGetPathTime += stopwatch2.Elapsed;
#endif // REPORT_PERFORMANCE
            if (fileInCache)
            {
#if REPORT_PERFORMANCE
                stopwatch2.Restart();
#endif // REPORT_PERFORMANCE
                bool romChanged, cfgChanged = false;
                uint preexistingCfgCrc;
                uint preexistingCrc = Rom.GetRefreshedCrcs(cachedRomPath, rom.ConfigPath, out preexistingCfgCrc);
#if REPORT_PERFORMANCE
                stopwatch2.Stop();
                _accumulatedGetCrcsTime += stopwatch2.Elapsed;
                stopwatch2.Restart();
#endif // REPORT_PERFORMANCE
                fileInCache = (rom.RefreshCrc(out romChanged) == preexistingCrc) && rom.IsConfigFileInCache(romStagingAreaPath, out cfgChanged); // use CanonicalRomComparerStrict.Default here?
                if (fileInCache && !romChanged && !cfgChanged)
                {
                    if (rom.Format == RomFormat.Luigi)
                    {
                        changed = !rom.RomPath.Exists() || (INTV.Core.Utility.Crc32.OfFile(rom.RomPath) != INTV.Core.Utility.Crc32.OfFile(cachedRomPath));
                    }
                    else if (rom.Format == RomFormat.Bin)
                    {
                        // when the preexistingRom is created, it recomputes the .cfg file's CRC, so see if it's changed
                        cfgChanged = rom.CfgCrc != preexistingCfgCrc;
                    }
                }
                changed = romChanged || cfgChanged;
#if REPORT_PERFORMANCE
                stopwatch2.Stop();
                _accumulatedCacheRecheckTime += stopwatch2.Elapsed;
#endif // REPORT_PERFORMANCE
            }
#if REPORT_PERFORMANCE
            stopwatch.Stop();
            _accumulatedCacheCheckTime += stopwatch.Elapsed;
#endif // REPORT_PERFORMANCE
            return fileInCache;
        }

        /// <summary>
        /// Determines if the source ROM's configuration file is already in the cache.
        /// </summary>
        /// <param name="rom">The ROM whose configuration file's presence in the cache is being tested.</param>
        /// <param name="romStagingAreaPath">The staging area directory for the ROM.</param>
        /// <param name="changed">If set to <c>true</c>, the config file changed.</param>
        /// <returns><c>true</c> if the configuration file is present and both CRC32 checksums match. If a ROM does not
        /// have a configuration file, then the function will also return <c>true</c>.</returns>
        public static bool IsConfigFileInCache(this IRom rom, string romStagingAreaPath, out bool changed)
        {
            changed = true;
            var configFilePath = rom.GetCachedConfigFilePath(romStagingAreaPath);
            bool configFilePathInCache = configFilePath.IsInvalid || configFilePath.Exists();
            if (configFilePathInCache)
            {
                if (configFilePath.IsValid)
                {
                    var sourceConfigCrc = rom.RefreshCfgCrc(out changed);
                    var destConfigCrc = INTV.Core.Utility.Crc32.OfFile(configFilePath);
                    configFilePathInCache = sourceConfigCrc == destConfigCrc;
                }
                else
                {
                    // No .cfg file in cache. It's changed if there is one on the original ROM, otherwise, not.
                    if (rom.Format == RomFormat.Luigi)
                    {
                        var cachedLuigiPath = rom.GetCachedRomFilePath(romStagingAreaPath);
                        if (cachedLuigiPath.Exists())
                        {
                            uint luigiCfgCrc;
                            Rom.GetRefreshedCrcs(cachedLuigiPath, StorageLocation.InvalidLocation, out luigiCfgCrc);
                            changed = rom.CfgCrc != luigiCfgCrc;
                        }
                    }
                    else
                    {
                        changed = rom.CfgCrc != 0;
                    }
                }
            }
            return configFilePathInCache;
        }

        /// <summary>
        /// Clears the read-only attributes on a file.
        /// </summary>
        /// <param name="file">The file whose read-only attribute is to be cleared.</param>
        public static void ClearReadOnlyAttribute(this string file)
        {
            if (!string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))
            {
                // just in case the file's read-only attribute was set (we usually strip it)
                var attributes = System.IO.File.GetAttributes(file);
                attributes &= ~System.IO.FileAttributes.ReadOnly;
                System.IO.File.SetAttributes(file, attributes);
            }
        }

        /// <summary>
        /// Given a list of files and / or directories, enumerate what appear to be valid program ROM files.
        /// </summary>
        /// <param name="files">A list of files and / or directories to inspect to locate ROM files.</param>
        /// <param name="acceptCancellation">Delegate to call to determine if the operation should accept being cancelled by the user.</param>
        /// <param name="progressFunc">Delegate to call to update progress of the operation.</param>
        /// <returns>An enumerable of valid program ROM files.</returns>
        public static IEnumerable<IRom> IdentifyRomFiles(this IEnumerable<string> files, Func<bool> acceptCancellation, Action<string> progressFunc)
        {
            var locations = files.ConvertPathsToStorageLocations();
            var potentialRomFilePaths = GetPotentialProgramRomFiles(locations, acceptCancellation, progressFunc).Where(f => ProgramFileKind.Rom.IsProgramFile(f.Path));
            foreach (var romFilePath in potentialRomFilePaths)
            {
                if (acceptCancellation())
                {
                    yield break;
                }
                UpdateProgressForGetPotentialRoms(romFilePath, progressFunc);
                var configFilePath = INTV.Shared.Model.Program.ProgramFileKindHelpers.GetConfigFilePath(romFilePath);
                if (configFilePath.Exists())
                {
                    INTV.Core.Model.IRom programRom = null;
                    try
                    {
                        programRom = INTV.Core.Model.Rom.Create(romFilePath, configFilePath);
                    }
                    catch (IOException)
                    {
                        // TODO: Report error here -- this has been found to occur of scanning SkyDrive / OneDrive in Windows 8.1.
                    }
                    if (programRom != null)
                    {
                        yield return programRom;
                    }
                }
                else
                {
                    INTV.Core.Model.IRom programRom = null;
                    try
                    {
                        programRom = INTV.Core.Model.Rom.Create(romFilePath, StorageLocation.InvalidLocation);
                    }
                    catch (IOException)
                    {
                        // TODO: Report error here -- this has been found to occur of scanning SkyDrive / OneDrive in Windows 8.1.
                    }
                    if (programRom != null)
                    {
                        yield return programRom;
                    }
                    else
                    {
                        // Might be a lone 'bin' file, such as EXEC or GROM. Run a checksum. This will result in a second checksum later, but oh, well.
                        IProgramInformation programInfo = null;
                        try
                        {
                            var crc = INTV.Core.Utility.Crc32.OfFile(romFilePath);
                            programInfo = ProgramInformationTable.Default.FindProgram(crc);
                        }
                        catch (IOException)
                        {
                            // TODO: Report error here -- this has been found to occur of scanning SkyDrive / OneDrive in Windows 8.1.
                        }
                        if (programInfo != null)
                        {
                            if (INTV.Core.Model.Rom.CheckRomFormat(romFilePath) != RomFormat.None)
                            {
                                yield return INTV.Core.Model.Rom.Create(romFilePath, StorageLocation.InvalidLocation);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function will return a ROM file given a path to a file that might be a ROM.
        /// </summary>
        /// <param name="romLocation">The location of an alleged ROM file.</param>
        /// <returns>If identifiable as a ROM, the ROM. Otherwise, <c>null</c>.</returns>
        /// <remarks>This function cannot defend against 'malicious' users who circumvent file filters or have files with extensions
        /// that match the supported types, but in fact do not contain an actual ROM. Such files will likely just crash the Intellivision.</remarks>
        public static IRom GetRomFromPath(this StorageLocation romLocation)
        {
            IRom programRom = null;
            var configFilePath = INTV.Shared.Model.Program.ProgramFileKindHelpers.GetConfigFilePath(romLocation);
            if (configFilePath.Exists())
            {
                try
                {
                    programRom = INTV.Core.Model.Rom.Create(romLocation, configFilePath);
                }
                catch (IOException)
                {
                    // TODO: Report error here -- this has been found to occur of scanning SkyDrive / OneDrive in Windows 8.1.
                }
            }
            if (programRom == null)
            {
                try
                {
                    programRom = INTV.Core.Model.Rom.Create(romLocation, StorageLocation.InvalidLocation);
                }
                catch (IOException)
                {
                    // TODO: Report error here -- this has been found to occur of scanning SkyDrive / OneDrive in Windows 8.1.
                }
                if (programRom == null)
                {
                    // Might be a lone 'bin' file, such as EXEC or GROM. Run a checksum. This will result in a second checksum later, but oh, well.
                    IProgramInformation programInfo = null;
                    try
                    {
                        var crc = INTV.Core.Utility.Crc32.OfFile(romLocation);
                        programInfo = ProgramInformationTable.Default.FindProgram(crc);
                    }
                    catch (IOException)
                    {
                        // TODO: Report error here -- this has been found to occur of scanning SkyDrive / OneDrive in Windows 8.1.
                    }
                    if (programInfo != null)
                    {
                        if (INTV.Core.Model.Rom.CheckRomFormat(romLocation) != RomFormat.None)
                        {
                            programRom = INTV.Core.Model.Rom.Create(romLocation, StorageLocation.InvalidLocation);
                        }
                    }
                }
            }
            return programRom;
        }

        private static IEnumerable<StorageLocation> ConvertPathsToStorageLocations(this IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (Directory.Exists(file))
                {
                    yield return file.CreateStorageLocationFromPath();
                }
                else
                {
                    var formats = file.GetCompressedArchiveFormatsFromFileName();
                    if (!formats.Any() || formats.All(f => f.IsCompressedArchiveFormatSupportedAndEnabled()))
                    {
                        yield return file.CreateStorageLocationFromPath();
                    }
                }
            }
        }

        private static IEnumerable<StorageLocation> GetPotentialProgramRomFiles(IEnumerable<StorageLocation> files, Func<bool> acceptCancellation, Action<string> progressFunc)
        {
            foreach (var file in files)
            {
                if (acceptCancellation())
                {
                    yield break;
                }
                if (file.ContainerExists())
                {
                    foreach (var subdirectoryFile in GetPotentialProgramRomFiles(file.EnumerateFiles("*.*", recurse: true), acceptCancellation, progressFunc))
                    {
                        if (acceptCancellation())
                        {
                            yield break;
                        }
                        var potentialRomLocations = GetPotentialProgramRomFilesInCompressedArchive(subdirectoryFile);
                        if (potentialRomLocations != null)
                        {
                            foreach (var potentialRomLocation in potentialRomLocations)
                            {
                                yield return UpdateProgressForGetPotentialRoms(potentialRomLocation, progressFunc);
                            }
                        }
                        else
                        {
                            yield return UpdateProgressForGetPotentialRoms(subdirectoryFile, progressFunc);
                        }
                    }
                }
                else if (file.Exists())
                {
                    var potentialRomLocations = GetPotentialProgramRomFilesInCompressedArchive(file);
                    if (potentialRomLocations != null)
                    {
                        foreach (var potentialRomLocation in potentialRomLocations)
                        {
                            yield return UpdateProgressForGetPotentialRoms(potentialRomLocation, progressFunc);
                        }
                    }
                    else
                    {
                        if (file.Size() <= Rom.MaxROMSize)
                        {
                            yield return UpdateProgressForGetPotentialRoms(file, progressFunc);
                        }
                    }
                }
            }
        }

        private static StorageLocation UpdateProgressForGetPotentialRoms(StorageLocation location, Action<string> progressFunction)
        {
            if (progressFunction != null)
            {
                progressFunction(location.Path);
            }
            return location;
        }

        private static IEnumerable<StorageLocation> GetPotentialProgramRomFilesInCompressedArchive(StorageLocation file)
        {
            IEnumerable<StorageLocation> potentialRomFilesInCompressedArchive = null;
            var compressedArchiveAccess = file.StorageAccess as ICompressedArchiveAccess;
            if ((compressedArchiveAccess != null) && file.IsContainer)
            {
                potentialRomFilesInCompressedArchive = file.EnumerateFiles();
            }
            return potentialRomFilesInCompressedArchive;
        }
    }
}

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

////#define REPORT_OLD_LUIGI_FILES
////#define REPORT_PERFORMANCE
////#define RECORD_PREPARE_FOR_DEPLOYMENT_VISITS

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Extension methods for IRom.
    /// </summary>
    public static class IRomHelpers
    {
#if REPORT_PERFORMANCE
        private static INTV.Shared.Utility.Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger(System.IO.Path.Combine(Configuration.Instance.ErrorLogDirectory, "LtoFlash.PrepareForDeploymentLog.txt"));
                }
                return _logger;
            }
        }
        private static INTV.Shared.Utility.Logger _logger;

        private static TimeSpan _accumulatedPrepareTime { get; set; }
        private static TimeSpan _accumulatedPrepareValidateTime { get; set; }
        private static TimeSpan _accumulatedPrepareSetupTime { get; set; }
        private static TimeSpan _accumulatedPrepareConverterAppsTime { get; set; }
        private static TimeSpan _accumulatedPrepareStagingTime { get; set; }
        private static TimeSpan _accumulatedPrepareCacheLookupTime { get; set; }
        private static TimeSpan _accumulatedPrepareLuigiHeaderTime { get; set; }
        private static TimeSpan _accumulatedPrepareCachedChangedTime { get; set; }
        private static TimeSpan _accumulatedPrepareLuigiUpdateTime { get; set; }
#if RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> PrepareForDeploymentVisits = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
#endif // RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
#endif // REPORT_PERFORMANCE

        /// <summary>
        /// Prepares a ROM for deployment to a Locutus device.
        /// </summary>
        /// <param name="rom">The ROM being prepared.</param>
        /// <param name="updateMode">Specifies the behavior of the LUIGI file generation.</param>
        /// <returns>The fully qualified path of the prepared output data file to deploy to Locutus.</returns>
        public static string PrepareForDeployment(this IRom rom, LuigiGenerationMode updateMode)
        {
#if REPORT_PERFORMANCE
#if RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
            int visits;
            if (PrepareForDeploymentVisits.TryGetValue(rom.RomPath, out visits))
            {
                PrepareForDeploymentVisits[rom.RomPath] = ++visits;
            }
            else
            {
                PrepareForDeploymentVisits[rom.RomPath] = 1;
            }
#endif // RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
            var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            rom.Validate();
#if REPORT_PERFORMANCE
            stopwatch2.Stop();
            _accumulatedPrepareValidateTime += stopwatch2.Elapsed;
#endif // REPORT_PERFORMANCE
            if ((updateMode == LuigiGenerationMode.Passthrough) && (rom.Format == RomFormat.Luigi))
            {
                return rom.RomPath.Path;
            }
#if REPORT_PERFORMANCE
            stopwatch2.Restart();
            var stopwatch3 = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            var jzIntvConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.JzIntv.Model.Configuration>();
            var converterApps = jzIntvConfiguration.GetConverterApps(rom, RomFormat.Luigi);
            if (!converterApps.Any())
            {
                converterApps = new[] { new Tuple<string, RomFormat>(JustCopy, RomFormat.Luigi) };
            }
            var converterApp = converterApps.First(); // rom.GetConverterApp(jzIntvConfiguration);
            if ((converterApp.Item1 != JustCopy) && (string.IsNullOrEmpty(converterApp.Item1) || !System.IO.File.Exists(converterApp.Item1)) && (rom.Format != RomFormat.Luigi))
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_ConversionToolNotFound_Error_Format, converterApp);
                throw new LuigiFileGenerationException(message, Resources.Strings.RomToLuigiFailed_ConversionToolNotFound_Error_Description);
            }
#if REPORT_PERFORMANCE
            stopwatch3.Stop();
            _accumulatedPrepareConverterAppsTime += stopwatch3.Elapsed;
            stopwatch3.Restart();
#endif // REPORT_PERFORMANCE
            var romStagingArea = SingleInstanceApplication.Instance.GetConfiguration<Configuration>().RomsStagingAreaPath;
            var stagingAreaPath = rom.GetStagingAreaPath(romStagingArea);
            var cachedRomPath = rom.GetCachedRomFilePath(stagingAreaPath);
            var cachedConfigPath = rom.GetCachedConfigFilePath(stagingAreaPath);
            var luigiFile = rom.GetOutputFilePath(stagingAreaPath, ProgramFileKind.LuigiFile);
#if REPORT_PERFORMANCE
            stopwatch3.Stop();
            _accumulatedPrepareStagingTime += stopwatch3.Elapsed;
            stopwatch3.Restart();
#endif // REPORT_PERFORMANCE

            bool createLuigiFile = true;
            bool changed;
            bool isSourceFileInCache = rom.IsInCache(stagingAreaPath, out changed);
#if REPORT_PERFORMANCE
            stopwatch3.Stop();
            _accumulatedPrepareCacheLookupTime += stopwatch3.Elapsed;
#endif // REPORT_PERFORMANCE

#if REPORT_PERFORMANCE
            stopwatch2.Stop();
            _accumulatedPrepareSetupTime += stopwatch2.Elapsed;

            stopwatch2.Restart();
#endif // REPORT_PERFORMANCE
            var luigiHeader = rom.GetLuigiHeader();
            if (luigiHeader != null)
            {
                // If the given ROM is already a LUIGI file, see if we can determine whether it's already in our cache.
#if REPORT_OLD_LUIGI_FILES
                System.Diagnostics.Debug.Assert(luigiHeader.Version > 0, "Really, you've got some OLD LUIGI files. Delete them.");
#endif // REPORT_OLD_LUIGI_FILES
                var crc24 = INTV.Core.Utility.Crc24.OfFile(rom.RomPath);
                var size = rom.RomPath.Size();
                var entry = CacheIndex.Find(crc24, (uint)size);
                isSourceFileInCache = entry != null;
                if (isSourceFileInCache)
                {
                    // Cases have been found in which, by moving files around on disk, the staging area path can change.
                    // The result of this, though, is that the *new* path in the cache is different than the extant one
                    // found in the cache. In this case, if the entry's location is different than the newly computed
                    // one, ignore the cache entry and make a new one by acting as if the file is not in the cache.
                    // FIXME This is a lazy fix. A better fix would be to remove the cached files and existing cache
                    // entry and re-add this new one. Or patch up the existing entry. Hell, maybe scrap the entire cache
                    // altogether as it's a bit of a bug farm and creating LUIGI files isn't all *that* expensive.
                    var stagingDirectory = System.IO.Path.GetFileName(stagingAreaPath);
                    var stagingPathChanged = !entry.LuigiPath.StartsWith(stagingDirectory);
                    if (stagingPathChanged)
                    {
                        isSourceFileInCache = false;
                    }
                    else
                    {
                        luigiFile = new StorageLocation(System.IO.Path.Combine(romStagingArea, entry.LuigiPath));
                        cachedRomPath = new StorageLocation(System.IO.Path.Combine(romStagingArea, entry.RomPath));
                        if (!string.IsNullOrEmpty(entry.CfgPath))
                        {
                            cachedConfigPath = new StorageLocation(System.IO.Path.Combine(romStagingArea, entry.CfgPath));
                        }
                    }
                }
            }
#if REPORT_PERFORMANCE
            stopwatch2.Stop();
            _accumulatedPrepareLuigiHeaderTime += stopwatch2.Elapsed;

            stopwatch2.Restart();
#endif // REPORT_PERFORMANCE

            if (isSourceFileInCache)
            {
                createLuigiFile = changed || !luigiFile.Exists();
            }
            if (!isSourceFileInCache || changed)
            {
                cachedRomPath.Path.ClearReadOnlyAttribute();
                cachedConfigPath.Path.ClearReadOnlyAttribute();
                rom.RomPath.CopyFile(cachedRomPath, overwrite: true);
                if (cachedConfigPath.IsValid && rom.ConfigPath.IsValid && rom.ConfigPath.Exists() && (rom.ConfigPath != rom.RomPath))
                {
                    rom.ConfigPath.CopyFile(cachedConfigPath, overwrite: true);
                }
                else if (!rom.ConfigPath.Exists() && cachedConfigPath.Exists())
                {
                    // The ROM's configuration file path doesn't exist, but there's one in the cache. Remove it.
                    FileUtilities.DeleteFile(cachedConfigPath.Path, false, 2);
                    cachedConfigPath = StorageLocation.InvalidLocation; // this is OK, because the ClearReadOnlyAttribute() extension method is null-safe
                }
                cachedRomPath.Path.ClearReadOnlyAttribute();
                cachedConfigPath.Path.ClearReadOnlyAttribute();
            }
#if REPORT_PERFORMANCE
            stopwatch2.Stop();
            _accumulatedPrepareCachedChangedTime += stopwatch2.Elapsed;

            stopwatch2.Restart();
#endif // REPORT_PERFORMANCE

            if (createLuigiFile || ((updateMode == LuigiGenerationMode.FeatureUpdate) || (updateMode == LuigiGenerationMode.Reset)))
            {
                var argument = "\"" + cachedRomPath + "\"" + " \"" + luigiFile + "\"";
                var result = -1;
                if (JustCopy == converterApp.Item1)
                {
                    rom.RomPath.CopyFile(luigiFile, overwrite: true);
                    result = 0;
                }
                else
                {
                    result = INTV.Shared.Utility.RunExternalProgram.Call(converterApp.Item1, argument, stagingAreaPath);
                }
                if (result == 0)
                {
                    if (!luigiFile.Exists())
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_OutputFileNotFound_Error_Format, rom.RomPath, luigiFile.GetFileNameWithoutExtension());
                        throw new LuigiFileGenerationException(message, Resources.Strings.RomToLuigiFailed_OutputFileNotFound_Error_Description_Format);
                    }
                    else if (luigiFile.Size() > Device.TotalRAMSize)
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_TooLarge_Error_Message_Format, rom.RomPath, luigiFile);
                        throw new LuigiFileGenerationException(message, Resources.Strings.RomToLuigiFailed_TooLarge_Description);
                    }

                    var description = INTV.Shared.Model.Program.ProgramCollection.Roms.FirstOrDefault(d => rom.IsEquivalentTo(d.Rom, RomComparerStrict.Default));
                    if (description != null)
                    {
                        LuigiFeatureFlags features = LuigiFeatureFlags.None;
#if DEBUG
                        var complainAboutOldLuigiFile = false;
#endif // DEBUG
                        luigiHeader = LuigiFileHeader.GetHeader(luigiFile);
                        features = description.Features.LuigiFeaturesLo;
#if DEBUG
                        var isRecognizedRom = !description.Features.GeneralFeatures.HasFlag(GeneralFeatures.UnrecognizedRom);
                        var hasFlagsFromCfgFile = luigiHeader.Features.HasFlag(LuigiFeatureFlags.FeatureFlagsExplicitlySet);
                        complainAboutOldLuigiFile = isRecognizedRom && hasFlagsFromCfgFile && ((luigiHeader.Features & ~LuigiFeatureFlags.FeatureFlagsExplicitlySet) != features);
#endif // DEBUG
#if DEBUG
                        if (complainAboutOldLuigiFile)
                        {
                            var message = "Known ROM has explicit flags from utility that are different than those set by LUI:\n\n";
                            message += "  LUI: " + features + "\n";
                            message += "  Utility: " + (luigiHeader.Features & ~LuigiFeatureFlags.FeatureFlagsExplicitlySet);
                            INTV.Shared.View.OSMessageBox.Show(message, "Feature Flags Inconsistency");
                        }
#endif // DEBUG
                        if (luigiHeader.WouldModifyFeatures(features, updateMode == LuigiGenerationMode.FeatureUpdate))
                        {
                            using (var file = System.IO.File.Open(luigiFile.Path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                            {
                                luigiHeader.UpdateFeatures(features, updateMode == LuigiGenerationMode.FeatureUpdate);
                                file.Seek(0, System.IO.SeekOrigin.Begin);
                                luigiHeader.Serialize(new Core.Utility.BinaryWriter(file));
                            }
                        }
                    }
                    try
                    {
                        var cacheIndexEntry = new CacheIndexEntry(rom, cachedRomPath.Path);
                        CacheIndex.Instance.AddEntry(cacheIndexEntry);
                    }
                    catch (Exception e)
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.PrepareForDeployment_ErrorCreatingCacheEntryFormat, rom.RomPath);
                        throw new LuigiFileGenerationException(message, e.Message, e);
                    }
                }
                else
                {
                    var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_InvalidOperation_Error_Message_Format, result);
                    var description = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_Error_Description_Format, converterApp);
                    throw new LuigiFileGenerationException(message, description);
                }
            }
            else
            {
                // If this is a different ROM that produces the same LUIGI, add an entry.
                var crc24 = INTV.Core.Utility.Crc24.OfFile(luigiFile);
                var size = (uint)luigiFile.Size();
                if (CacheIndex.Find(crc24, size) == null)
                {
                    try
                    {
                        var cacheIndexEntry = new CacheIndexEntry(rom, cachedRomPath.Path);
                        CacheIndex.Instance.AddEntry(cacheIndexEntry);
                    }
                    catch (Exception e)
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.PrepareForDeployment_ErrorCreatingCacheEntryFormat, rom.RomPath);
                        throw new LuigiFileGenerationException(message, e.Message, e);
                    }
                }
            }
            ////catch (System.IO.PathTooLongException e)
            ////catch (System.IO.IOException e)
            ////catch (UnauthorizedAccessException e)
            ////catch (InvalidOperationException e)
            ////catch (LuigiFileGenerationException e)
#if REPORT_PERFORMANCE
            stopwatch2.Stop();
            _accumulatedPrepareLuigiUpdateTime += stopwatch2.Elapsed;
#endif // REPORT_PERFORMANCE

            if (!luigiFile.Exists())
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_Error_Description_Format, rom);
                var description = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_InvalidOutputFileFormat, luigiFile);
                throw new LuigiFileGenerationException(message, description);
            }

#if REPORT_PERFORMANCE
            stopwatch.Stop();
#endif // REPORT_PERFORMANCE
            return luigiFile.Path;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                _accumulatedPrepareTime += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Gets the application to use to convert the ROM file.
        /// </summary>
        /// <param name="rom">The ROM file to be converted.</param>
        /// <param name="jzIntvConfiguration">The configuration used to locate the proper conversion tool.</param>
        /// <returns>The fully qualified path to the converter program.</returns>
        public static string GetConverterApp(this IRom rom, INTV.JzIntv.Model.Configuration jzIntvConfiguration)
        {
            string converterApp = null;
            switch (rom.Format)
            {
                case RomFormat.Bin:
                    converterApp = jzIntvConfiguration.GetProgramPath(INTV.JzIntv.Model.ProgramFile.Bin2Luigi);
                    break;
                case RomFormat.Intellicart:
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    converterApp = jzIntvConfiguration.GetProgramPath(INTV.JzIntv.Model.ProgramFile.Rom2Luigi);
                    break;
                case RomFormat.Luigi:
                    converterApp = JustCopy;
                    break;
            }
            return converterApp;
        }

        private const string JustCopy = "!!::copy::!!";

        /// <summary>
        /// Get the expected path for a LUIGI file for the given ROM.
        /// </summary>
        /// <param name="rom">The ROM whose LUIGI path is desired.</param>
        /// <returns>The fully qualified path to the LUIGI file for the ROM.</returns>
        public static string GetLtoFlashFilePath(this IRom rom)
        {
            var romStagingArea = SingleInstanceApplication.Instance.GetConfiguration<Configuration>().RomsStagingAreaPath;
            var stagingAreaPath = rom.GetStagingAreaPath(romStagingArea);
            return rom.GetOutputFilePath(stagingAreaPath, ProgramFileKind.LuigiFile).Path;
        }

        private static readonly Dictionary<System.Type, string> ExceptionErrorLookupTable = new Dictionary<System.Type, string>()
        {
            { typeof(System.IO.PathTooLongException), Resources.Strings.RomToLuigiFailed_PathTooLong_Description },
            { typeof(System.IO.IOException), Resources.Strings.RomToLuigiFailed_IOException_Description },
            { typeof(System.UnauthorizedAccessException), Resources.Strings.RomToLuigiFailed_UnauthorizedAccess_Error_Description },
            { typeof(System.InvalidOperationException), Resources.Strings.RomToLuigiFailed_InvalidOperation_Error_Description },
        };

        /// <summary>
        /// Report an error caused during LUIGI conversion.
        /// </summary>
        /// <param name="exception">The error.</param>
        /// <param name="failedToAdd">Enumeration of the ROM files that were not added to the menu if that option was chosen.</param>
        public static void ReportAddItemsError(System.Exception exception, IDictionary<string, IDictionary<string, IList<System.Tuple<string, string>>>> failedToAdd)
        {
            var message = string.Empty;
            var title = string.Empty;
            var luigiException = exception as LuigiFileGenerationException;
            if (luigiException != null)
            {
                message = Resources.Strings.AddItemsOperation_FailedToPrepareMessage;
                title = Resources.Strings.LuigiFileError_Title;
            }
            else if ((exception != null) && !ExceptionErrorLookupTable.TryGetValue(exception.GetType(), out message))
            {
                message = Resources.Strings.AddItemsOperation_Failed;
                title = Resources.Strings.AddItemsRejected_ErrorAddingROMsTitle;
            }
            else if (failedToAdd.Any())
            {
                message = Resources.Strings.AddItemsRejected_ErrorAddingROMsMessage;
                title = Resources.Strings.AddItemsRejected_ErrorAddingROMsTitle;
            }

            string failedToAddText = null;
            if (failedToAdd.Any())
            {
                var errorDetailBuilder = new System.Text.StringBuilder();
                if (luigiException != null)
                {
                    errorDetailBuilder.AppendLine(luigiException.Description).AppendLine();
                }
                errorDetailBuilder.AppendLine(Resources.Strings.AddItemsOperation_FailedToAddMessage);
                errorDetailBuilder.AppendLine(Resources.Strings.AddItemsOperation_FailedToAddReportHeader).AppendLine();

                // Files not added are arranged by target directory, then within that, by failure type.
                foreach (var fileNotAdded in failedToAdd)
                {
                    errorDetailBuilder.AppendLine(fileNotAdded.Key); // destination folder
                    var errorReasonsAndFiles = fileNotAdded.Value;
                    foreach (var errorReasonAndFiles in errorReasonsAndFiles)
                    {
                        errorDetailBuilder.AppendLine("  " + errorReasonAndFiles.Key);
                        var fileNamesAndPaths = errorReasonAndFiles.Value;
                        foreach (var fileNameAndPath in fileNamesAndPaths)
                        {
                            errorDetailBuilder.AppendLine("    " + fileNameAndPath.Item1 + ": " + fileNameAndPath.Item2);
                        }
                    }
                    errorDetailBuilder.AppendLine();
                }
                failedToAddText = errorDetailBuilder.AppendLine().ToString();
            }
            var errorDialog = INTV.Shared.View.ReportDialog.Create(title, message);
            errorDialog.ReportText = failedToAddText;
            if (SingleInstanceApplication.SharedSettings.ShowDetailedErrors || string.IsNullOrEmpty(failedToAddText))
            {
                errorDialog.Exception = exception;
            }
            errorDialog.ShowSendEmailButton = (exception != null) && !(exception is LuigiFileGenerationException) && !(exception is System.IO.IOException);
            errorDialog.ShowDialog(Resources.Strings.OK);
        }

        /// <summary>
        /// Reset internal performance timing data.
        /// </summary>
        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        internal static void ResetAccumulatedTimes()
        {
#if REPORT_PERFORMANCE
            _accumulatedPrepareTime = TimeSpan.Zero;
            _accumulatedPrepareValidateTime = TimeSpan.Zero;
            _accumulatedPrepareSetupTime = TimeSpan.Zero;
            _accumulatedPrepareConverterAppsTime = TimeSpan.Zero;
            _accumulatedPrepareStagingTime = TimeSpan.Zero;
            _accumulatedPrepareCacheLookupTime = TimeSpan.Zero;
            _accumulatedPrepareLuigiHeaderTime = TimeSpan.Zero;
            _accumulatedPrepareCachedChangedTime = TimeSpan.Zero;
            _accumulatedPrepareLuigiUpdateTime = TimeSpan.Zero;
            INTV.Shared.Model.IRomHelpers.ResetAccumulatedTimes();
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Report performance data to a logger or debug output.
        /// </summary>
        /// <param name="logger">A logger to record into. May be <c>null</c>.</param>
        /// <param name="prefix">Prefix to include in each line of the output.</param>
        /// <remarks>If <param name="logger"/> is <c>null</c>, output is reported to debug output.</remarks>
        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        internal static void ReportAccumulatedTimes(Logger logger, string prefix)
        {
            ReportAccumulatedTimes(logger != null, logger, prefix);
        }

        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        private static void ReportAccumulatedTimes(bool log, Logger logger, string prefix)
        {
            Action<string> logIt = (o) => System.Diagnostics.Debug.WriteLine(o.ToString());
            if (log)
            {
                if (logger != null)
                {
                    logIt = logger.Log;
                }
#if REPORT_PERFORMANCE
                else
                {
                    logIt = _logger.Log;
                }
#endif // REPORT_PERFORMANCE
            }
            prefix = prefix == null ? string.Empty : prefix + " ";
#if REPORT_PERFORMANCE
            logIt(prefix + "Total PrepareForDeployment --------------: " + _accumulatedPrepareTime.ToString());
            logIt(prefix + "Total PrepareForDeploymentValidate ------: " + _accumulatedPrepareValidateTime.ToString());
            logIt(prefix + "Total PrepareForDeploymentSetup ---------: " + _accumulatedPrepareSetupTime.ToString());
            logIt(prefix + "Total  PrepareForDeploymentGetApps ------: " + _accumulatedPrepareConverterAppsTime.ToString());
            logIt(prefix + "Total  PrepareForDeploymentStaging ------: " + _accumulatedPrepareStagingTime.ToString());
            logIt(prefix + "Total  PrepareForDeploymenCacheLookup ---: " + _accumulatedPrepareCacheLookupTime.ToString());
            INTV.Shared.Model.IRomHelpers.ReportAccumulatedTimes(logger, prefix);
            logIt(prefix + "Total  PrepareForDeploymentLuigiHeader --: " + _accumulatedPrepareLuigiHeaderTime.ToString());
            logIt(prefix + "Total  PrepareForDeploymentCacheChanged -: " + _accumulatedPrepareCachedChangedTime.ToString());
            logIt(prefix + "Total  PrepareForDeploymentLuigiUpdate --: " + _accumulatedPrepareLuigiUpdateTime.ToString());
#else
            logIt(prefix + "REPORT_PERFORMANCE has not been #defined in:" + typeof(IRomHelpers).FullName);
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Reset internal prepare for deployment performance data.
        /// </summary>
        [System.Diagnostics.Conditional("RECORD_PREPARE_FOR_DEPLOYMENT_VISITS")]
        internal static void ResetPrepareForDeploymentVisits()
        {
#if RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
#endif // RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
        }

        /// <summary>
        /// Report prepare for deployment performance data to a logger or debug output.
        /// </summary>
        /// <param name="logger">A logger to record into. May be <c>null</c>.</param>
        /// <remarks>If <param name="logger"/> is <c>null</c>, output is reported to debug output.</remarks>
        [System.Diagnostics.Conditional("RECORD_PREPARE_FOR_DEPLOYMENT_VISITS")]
        internal static void ReportPrepareForDeploymentVisits(Logger logger)
        {
            Action<string> logIt = (o) => System.Diagnostics.Debug.WriteLine(o.ToString());
            if (logger == null)
            {
                logIt = logger.Log;
            }
#if RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
            foreach (var visit in PrepareForDeploymentVisits)
            {
                Logger.Log("PrepareForDeployment visits: " + visit.Key + ": " + visit.Value);
            }
#else
            logIt("RECORD_PREPARE_FOR_DEPLOYMENT_VISITS has not been #defined in:" + typeof(IRomHelpers).FullName);
#endif // RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
        }
    }
}

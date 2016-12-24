// <copyright file="DeviceHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2015-2016 All Rights Reserved
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
using System.Globalization;
using System.Linq;
using INTV.Core.Model;
using INTV.Shared.Model;
using INTV.Shared.Model.Device;
using INTV.Shared.Utility;

namespace INTV.Intellicart.Model
{
    /// <summary>
    /// Helper functions for working with an Intellicart.
    /// </summary>
    public static class DeviceHelpers
    {
        #region DownloadRom

        /// <summary>
        /// Downloads a ROM to an Intellicart.
        /// </summary>
        /// <param name="intellicart">The Intellicart to load the ROM onto.</param>
        /// <param name="programName">Name of the program being downloaded.</param>
        /// <param name="rom">The ROM to load.</param>
        /// <param name="errorHandler">Error handler function.</param>
        public static void DownloadRom(this IntellicartModel intellicart, string programName, IRom rom, Action<string, Exception> errorHandler)
        {
            var title = string.Format(CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_Title_Format, programName);
            var task = new AsyncTaskWithProgress(title, true, true, 0);
            var taskData = new DownloadTaskData(task, intellicart, programName, rom);
            taskData.ErrorHandler = errorHandler;
            task.RunTask(taskData, DownloadRom, DownloadRomComplete);
        }

        private static void DownloadRom(AsyncTaskData taskData)
        {
            SingleInstanceApplication.Instance.IsBusy = true;
            var data = (DownloadTaskData)taskData;
            var romPath = PrepareRom(data);
            data.UpdateTaskProgress(0, string.Format(CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_Update_Format, romPath));
            var cancelled = data.AcceptCancelIfRequested();
            if (!cancelled)
            {
                using (var rom = System.IO.File.OpenRead(romPath))
                {
                    var configData = new Dictionary<string, object>() { { SerialPortConnection.PortNameConfigDataName, data.Intellicart.SerialPort } };
                    using (var port = SerialPortConnection.Create(configData))
                    {
                        port.BaudRate = data.Intellicart.BaudRate;
                        port.WriteTimeout = data.Intellicart.Timeout * 1000;

                        // default port settings are 8,N,1 with no handshaking
                        var bytesRemaining = (int)rom.Length;
                        var totalBytes = rom.Length;
                        var bytesPerSecond = data.Intellicart.BaudRate / 8;
                        var bytesWritten = 0;
                        var estimatedDownloadTime = ((double)rom.Length / bytesPerSecond) + 4; // give it time to finish
                        var estimatedTimeRemaining = estimatedDownloadTime;
                        var percentDone = 0.0;

                        // Would like to respond to cancel requests somewhat quickly. So, let's
                        // write out small enough chunks even at the slowest baud rate...
                        var bytesPerWrite = (data.Intellicart.BaudRate / 2400) * 128;
                        System.Diagnostics.Debug.Assert(bytesPerWrite > 0, "How did we get zero bytes to write?!");
                        port.Open();
                        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                        byte[] buffer = new byte[bytesPerWrite];
                        while (!cancelled && (bytesRemaining > 0))
                        {
                            var updateText = string.Format(CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_UpdateTitle_Format, data.Name, Math.Max(0, (int)estimatedTimeRemaining));
                            data.UpdateTaskTitle(updateText);
                            bytesPerWrite = Math.Min(bytesPerWrite, bytesRemaining);
                            var bytesRead = rom.Read(buffer, 0, bytesPerWrite);
                            bytesWritten += bytesRead;
                            bytesRemaining -= bytesRead;
                            updateText = string.Format(CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_Update_Format, romPath);
                            estimatedTimeRemaining = estimatedDownloadTime - stopwatch.Elapsed.TotalSeconds;
                            percentDone = stopwatch.Elapsed.TotalSeconds / estimatedDownloadTime;
                            data.UpdateTaskProgress(percentDone, updateText);
                            port.WriteStream.Write(buffer, 0, bytesRead);
                            cancelled = data.AcceptCancelIfRequested();
                        }

                        // If we close the port too soon after writing, the Intellicart will time out reading data from the stream.
                        // This is likely due to buffering, and that the streams get disposed when the port and file streams are
                        // disposed. On Mac in particular, the write out to the port may complete far more quickly than what the
                        // math would indicate, based on observation. This implies one of the following:
                        //  a) Even though the synchronous write was called, the underlying implementation is asynchronous
                        //  b) It could be that the driver itself is "lying" to the underlying implementation
                        //  c) Buffering, either in the driver, kernel, or other API layers, misleads the higher-level APIs
                        var timedOut = false;
                        while (!cancelled && !timedOut)
                        {
                            var message = string.Format(Resources.Strings.DownloadRom_UpdateTitle_Format, data.Name, Math.Max(0, (int)estimatedTimeRemaining));
                            data.UpdateTaskTitle(message);
                            System.Threading.Thread.Sleep(250);
                            cancelled = data.AcceptCancelIfRequested();
                            var updateText = string.Format(CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_Update_Format, romPath);
                            estimatedTimeRemaining = estimatedDownloadTime - stopwatch.Elapsed.TotalSeconds;
                            percentDone = Math.Max(100, stopwatch.Elapsed.TotalSeconds / estimatedDownloadTime);
                            data.UpdateTaskProgress(percentDone, updateText);
                            timedOut = estimatedTimeRemaining < 0;
                        }
                    }
                }
            }
        }

        private static void DownloadRomComplete(AsyncTaskData taskData)
        {
            try
            {
                var data = (DownloadTaskData)taskData;
                if ((data.ErrorHandler != null) && (data.Error != null))
                {
                    data.ErrorHandler(Resources.Strings.DownloadRom_ErrorMessage, data.Error);
                }
                else
                if (taskData.Cancelled)
                {
                    INTV.Shared.View.OSMessageBox.Show(Resources.Strings.DownloadRom_CancelMessage, Resources.Strings.DownloadRom_CancelTitle);
                }
            }
            finally
            {
                SingleInstanceApplication.Instance.IsBusy = false;
            }
        }

        private const string JustCopy = "!!::copy::!!";

        private static string PrepareRom(DownloadTaskData data)
        {
            var rom = data.Rom;
            data.UpdateTaskProgress(0, string.Format(CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_PrepareUpdateFormat, rom.RomPath));
            var jzIntvConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.JzIntv.Model.Configuration>();
            var converterApps = jzIntvConfiguration.GetConverterApps(rom, RomFormat.Rom);
            if (!converterApps.Any())
            {
                converterApps = new[] { new Tuple<string, RomFormat>(JustCopy, RomFormat.Rom) };
            }
            var converterApp = converterApps.First(); // rom.GetConverterApp(jzIntvConfiguration);
            if ((converterApp.Item1 != JustCopy) && (string.IsNullOrEmpty(converterApp.Item1) || !System.IO.File.Exists(converterApp.Item1)) && (rom.Format != RomFormat.Rom))
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_PrepareFailedErrorFormat, converterApp);
                throw new InvalidOperationException(message);
            }
            var romStagingArea = Configuration.Instance.RomsStagingAreaPath;
            var stagingAreaPath = rom.GetStagingAreaPath(romStagingArea);
            var cachedRomPath = rom.GetCachedRomFilePath(stagingAreaPath);
            var cachedConfigPath = rom.GetCachedConfigFilePath(stagingAreaPath);
            var romFile = rom.GetOutputFilePath(stagingAreaPath, INTV.Core.Model.Program.ProgramFileKind.Rom, false);

            bool createRomFile = true;
            bool changed;
            bool isSourceFileInCache = rom.IsInCache(stagingAreaPath, out changed);

            if (isSourceFileInCache)
            {
                createRomFile = !System.IO.File.Exists(romFile);
            }
            else
            {
                cachedRomPath.ClearReadOnlyAttribute();
                cachedConfigPath.ClearReadOnlyAttribute();
                System.IO.File.Copy(rom.RomPath, cachedRomPath, true);
                if (!string.IsNullOrWhiteSpace(cachedConfigPath) && !string.IsNullOrEmpty(rom.ConfigPath) && System.IO.File.Exists(rom.ConfigPath) && (rom.ConfigPath != rom.RomPath))
                {
                    System.IO.File.Copy(rom.ConfigPath, cachedConfigPath, true);
                }
                cachedRomPath.ClearReadOnlyAttribute();
                cachedConfigPath.ClearReadOnlyAttribute();
            }

            if (createRomFile)
            {
                foreach (var converter in converterApps)
                {
                    var argument = "\"" + cachedRomPath + "\"";
                    var result = -1;
                    if (JustCopy == converterApp.Item1)
                    {
                        System.IO.File.Copy(rom.RomPath, cachedRomPath, true);
                        result = 0;
                    }
                    else
                    {
                        result = INTV.Shared.Utility.RunExternalProgram.Call(converter.Item1, argument, stagingAreaPath);
                    }
                    if (result == 0)
                    {
                        cachedRomPath = System.IO.Path.ChangeExtension(cachedRomPath, converter.Item2.FileExtension());
                    }
                    else
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_RomConversionToolFailedErrorFormat, System.IO.Path.GetFileName(converterApp.Item1), result);
                        throw new InvalidOperationException(message);
                    }
                }
                if (!System.IO.File.Exists(romFile))
                {
                    var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_RomConversionOutputFileNotFoundErrorFormat, rom.RomPath, System.IO.Path.GetFileNameWithoutExtension(romFile));
                    throw new InvalidOperationException(message);
                }
                else if ((new System.IO.FileInfo(romFile)).Length > IntellicartModel.MaxROMSize)
                {
                    var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadRom_RomTooLargeErrorFormat, rom.RomPath, romFile);
                    throw new InvalidOperationException(message);
                }
            }

            // UNDONE Not sure if we should handle any of these explicitly. Haven't encountered them during testing.
            ////catch (System.IO.PathTooLongException e)
            ////catch (System.IO.IOException e)
            ////catch (UnauthorizedAccessException e)
            ////catch (InvalidOperationException e)
            return romFile;
        }

        #endregion // DownloadRom
    }
}

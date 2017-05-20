// <copyright file="DeviceHelpers.UpdateFirmware.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for UpdateFirmware.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region UpdateFirmware

        /// <summary>
        /// Update firmware on the device.
        /// </summary>
        /// <param name="device">Target of the command.</param>
        /// <param name="firmwarePath">Path on local file system to the firmware update.</param>
        /// <param name="newVersion">New version of the firmware, used for validation.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void UpdateFirmware(this Device device, string firmwarePath, int newVersion, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var data = new Tuple<string, int>(firmwarePath, newVersion);
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_UpdateFirmware_Title,
                    Data = data,
                    OnSuccess = (c, p, r) => device.GetFirmwareRevisions(onCompleteHandler, errorHandler),
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(UpdateFirmware, 0.55);
            }
        }

        /// <summary>
        /// Protocol commands necessary for UpdateFirmware.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> UpdateFirmwareProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsUploadDataBlockToRam,
            ProtocolCommandId.FirmwareValidateImageInRam,
            ProtocolCommandId.FirmwareEraseSecondary,
            ProtocolCommandId.FirmwareProgramSecondary,
            ProtocolCommandId.FirmwareGetRevisions
        };

        private static void UpdateFirmware(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var customData = data.Data as Tuple<string, int>;
            var firmwareUpdateFile = customData.Item1;
            var succeeded = false;
            UploadDataBlockToRam.Create(0, new FileSystemFile(firmwareUpdateFile), 0).Execute(device.Port, data, out succeeded);
            if (succeeded)
            {
                ValidateFirmwareImageInRam.Instance.Execute(device.Port, data, out succeeded);
                if (succeeded)
                {
                    // Check to see if there's an error database file. If so, and it's different than the one
                    // we've cached in the FirmwareUpdates directory, copy it over so we'll have a matching error_db.yaml.
                    // We're not going to complain if this part fails, so sit on any exceptions that may happen.
                    try
                    {
                        var configuration = Configuration.Instance;
                        var sourceErrorDatabaseFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(firmwareUpdateFile), ErrorLog.ErrorDatabaseFileName);
                        if (System.IO.File.Exists(sourceErrorDatabaseFilePath))
                        {
                            if (!System.IO.Directory.Exists(configuration.FirmwareUpdatesDirectory))
                            {
                                System.IO.Directory.CreateDirectory(configuration.FirmwareUpdatesDirectory);
                            }
                            var destinationErrorDatabaseFilePath = System.IO.Path.Combine(configuration.FirmwareUpdatesDirectory, ErrorLog.ErrorDatabaseFileName);
                            var crcOfSource = INTV.Core.Utility.Crc32.OfFile(sourceErrorDatabaseFilePath);
                            var crcOfDestination = 0u;
                            if (System.IO.File.Exists(destinationErrorDatabaseFilePath))
                            {
                                crcOfDestination = INTV.Core.Utility.Crc32.OfFile(destinationErrorDatabaseFilePath);
                                if (crcOfSource != crcOfDestination)
                                {
                                    var backupPath = destinationErrorDatabaseFilePath.GetUniqueBackupFilePath();
                                    System.IO.File.Move(destinationErrorDatabaseFilePath, backupPath);
                                }
                            }
                            if ((crcOfDestination == 0) || (crcOfSource != crcOfDestination))
                            {
                                System.IO.File.Copy(sourceErrorDatabaseFilePath, destinationErrorDatabaseFilePath);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to copy error database file: " + e);
                    }
                }
                else
                {
                    data.FailureMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UpdateFirmwareCommand_ValidationFailedFormat, firmwareUpdateFile);
                }
            }
            else
            {
                data.FailureMessage = Resources.Strings.DeviceMultistageCommand_UpdateFirmware_DownloadFailed;
            }
            if (succeeded && device.ShouldRemoveSecondaryFirmware(data, out succeeded))
            {
                device.RemoveSecondaryFirmware(data, out succeeded);
            }
            if (succeeded && !device.ShouldRemoveSecondaryFirmware(data, out succeeded))
            {
                // Secondary is gone -- so now update firmware!
                ProgramSecondaryFirmware.Instance.Execute(device.Port, data, out succeeded);
                data.UpdateTaskProgress(0, Resources.Strings.ProtocolCommandId_WaitForBeaconPseudoCommand_Title);
                succeeded = device.WaitForBeacon(ProtocolCommand.WaitForBeaconTimeout * 2) && succeeded;
            }
            if (succeeded)
            {
                var newRevisions = QueryFirmwareRevisions.Instance.Execute(device.Port, data, out succeeded) as FirmwareRevisions;
                if (succeeded)
                {
                    var deployedVersion = newRevisions.Current;
                    succeeded = deployedVersion == customData.Item2;
                }
            }
            if (!succeeded && string.IsNullOrWhiteSpace(data.FailureMessage))
            {
                data.FailureMessage = Resources.Strings.DeviceMultistageCommand_UpdateFirmware_Failed;
            }
        }

        #endregion // UpdateFirmware

        #region Firmware Command Support Functions

        private static bool ShouldRemoveSecondaryFirmware(this Device device, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            var firmwareRevisions = QueryFirmwareRevisions.Instance.Execute(device.Port, taskData, out succeeded) as FirmwareRevisions;
            var shouldRemove = succeeded && (firmwareRevisions.Secondary != FirmwareRevisions.UnavailableFirmwareVersion);
            return shouldRemove;
        }

        private static object RemoveSecondaryFirmware(this Device device, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            var result = EraseSecondaryFirmware.Instance.Execute(device.Port, taskData, out succeeded);
            if (succeeded)
            {
                device.WaitForBeacon(ProtocolCommand.WaitForBeaconTimeout * 2);
            }
            return result;
        }

        #endregion // Firmware Command Support Functions
    }
}

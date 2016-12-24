// <copyright file="DeviceHelpers.GetErrorAndCrashLogs.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
    /// Implementation for GetErrorAndCrashLogs.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region GetErrorAndCrashLogs

        /// <summary>
        /// Retrieves the error and crash logs as needed from the target device.
        /// </summary>
        /// <param name="device">Target of the command.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void GetErrorAndCrashLogs(this Device device, Action<ErrorLog, CrashLog> onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_GetErrorAndCrashLogs_Title,
                    OnSuccess = (c, p, r) =>
                    {
                        var results = (Tuple<ErrorLog, CrashLog>)r;
                        if (onCompleteHandler != null)
                        {
                            onCompleteHandler(results.Item1, results.Item2);
                        }
                        device.ErrorLog = results.Item1;
                        device.CrashLog = results.Item2;
                    },
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(GetErrorAndCrashLogs);
            }
        }

        /// <summary>
        /// Protocol commands needed for getting the crash and error logs.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> GetErrorAndCrashLogsProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.Ping,
            ProtocolCommandId.DownloadErrorLog,
            ProtocolCommandId.DownloadCrashLog,
            ProtocolCommandId.EraseCrashLog
        };

        private static void GetErrorAndCrashLogs(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var succeeded = false;
            data.UpdateTaskProgress(0, Resources.Strings.DeviceMultistageCommand_GetErrorAndCrashLogs_LocatingInformation);
            var currentStatus = (DeviceStatusResponse)(DeviceStatusResponse)Commands.Ping.Instance.Execute(device.Port, data, out succeeded);

            var errorLog = device.ErrorLog;

            // NOTE: If debugging via LtoFlashViewModel.InjectFirmwareCrashCommand, force true here
            if (succeeded && currentStatus.HardwareStatus.HasFlag(HardwareStatusFlags.NewErrorLogAvailable))
            {
                errorLog = Commands.DownloadErrorLog.Instance.Execute(device.Port, data, out succeeded) as ErrorLog;
            }

            // NOTE: If debugging via LtoFlashViewModel.InjectFirmwareCrashCommand, force true here
            CrashLog crashLog = device.CrashLog;
            if (succeeded && currentStatus.HardwareStatus.HasFlag(HardwareStatusFlags.NewCrashLogAvailable))
            {
                crashLog = Commands.DownloadCrashLog.Instance.Execute(device.Port, data, out succeeded) as CrashLog;
                if (succeeded)
                {
                    Commands.EraseCrashLog.Instance.Execute(device.Port, data, out succeeded);
                }
            }
            if (succeeded)
            {
                data.Result = new Tuple<ErrorLog, CrashLog>(errorLog, crashLog);
            }
        }

        #endregion // GetErrorAndCrashLogs
    }
}

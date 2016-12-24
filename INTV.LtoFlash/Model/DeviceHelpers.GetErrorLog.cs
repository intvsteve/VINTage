// <copyright file="DeviceHelpers.GetErrorLog.cs" company="INTV Funhouse">
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
    /// Implementation for GetErrorLog.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region GetErrorLog

        /// <summary>
        /// Unconditionally retrieves the error log from the target device.
        /// </summary>
        /// <param name="device">Target of the command.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        /// <remarks>NOTE: On some older (pre-release) versions of the firmware, <see cref=">HardwareStatusFlags.NewErrorLogAvailable"/>
        /// was not available. This method does not check that flag -- it will read the entire error log. Reading the error log on production
        /// and later versions of the firmware clears HardwareStatusFlags.NewErrorLogAvailable. Using the <see cref="GetErrorAndCrashLogs"/>
        /// method may be a better approach. Also note that this method is presently only directly used in DEBUG builds as a test facility.</remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void GetErrorLog(this Device device, Action<ErrorLog> onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.DownloadErrorLog)
                {
                    OnSuccess = (c, p, r) =>
                    {
                        device.ErrorLog = (ErrorLog)r;
                        if (onCompleteHandler != null)
                        {
                            onCompleteHandler(device.ErrorLog);
                        }
                    },
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(GetErrorLog);
            }
        }

        /// <summary>
        /// Protocol commands necessary for GetErrorLog.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> GetErrorLogProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.DownloadErrorLog
        };

        private static void GetErrorLog(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var succeeded = false;
            data.Result = Commands.DownloadErrorLog.Instance.Execute(device.Port, data, out succeeded);
        }

        #endregion // GetErrorLog
    }
}

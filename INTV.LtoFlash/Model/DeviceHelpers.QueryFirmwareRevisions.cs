// <copyright file="DeviceHelpers.QueryFirmwareRevisions.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for QueryFirmwareRevisions.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region QueryFirmwareRevisions

        /// <summary>
        /// Get the factory, active, and secondary firmware versions.
        /// </summary>
        /// <param name="device">Target of the command.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void GetFirmwareRevisions(this Device device, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.FirmwareGetRevisions)
                {
                    OnSuccess = (c, p, r) =>
                    {
                        device.FirmwareRevisions = (FirmwareRevisions)r;
                        if (onCompleteHandler != null)
                        {
                            onCompleteHandler(c, p, r);
                        }
                    },
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(GetFirmwareRevisions);
            }
        }

        /// <summary>
        /// Protocol commands necessary for GetFirmwareRevisions.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> GetFirmwareRevisionsProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.FirmwareGetRevisions
        };

        private static void GetFirmwareRevisions(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var succeeded = false;
            data.Result = QueryFirmwareRevisions.Instance.Execute(device.Port, data, out succeeded);
        }

        #endregion // QueryFirmwareRevisions
    }
}

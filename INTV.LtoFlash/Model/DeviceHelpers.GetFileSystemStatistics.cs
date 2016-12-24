// <copyright file="DeviceHelpers.GetFileSystemStatistics.cs" company="INTV Funhouse">
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
using INTV.LtoFlash.Model;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for GetFileSystemStatistics.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region GetFileSystemStatistics

        /// <summary>
        /// Retrieves the file system statistics from a device.
        /// </summary>
        /// <param name="device">The target Locutus device whose file system statistics are to be retrieved.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void GetFileSystemStatistics(this Device device, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.LfsGetStatistics)
                {
                    OnSuccess = (c, p, r) => device.FileSystemStatistics = (FileSystemStatistics)r,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(GetFileSystemStatistics);
            }
        }

        /// <summary>
        /// Protocol commands necessary for GetFileSystemStatistics.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> GetFileSystemStatisticsProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsGetStatistics
        };

        private static void GetFileSystemStatistics(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var succeeded = false;
            ProtocolCommandId.Ping.GetProgressTitle();
            data.Result = Commands.GetFileSystemStatistics.Instance.Execute(device.Port, data, out succeeded);
        }

        #endregion // GetFileSystemStatistics
    }
}

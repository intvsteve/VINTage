// <copyright file="DeviceHelpers.SetFileSystemFlags.cs" company="INTV Funhouse">
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
    /// Implementation for SetFileSystemFlags.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region SetFileSystemFlags

        /// <summary>
        /// Sets the file system dirty flags on a Locutus device.
        /// </summary>
        /// <param name="device">The target Locutus device whose file system dirty flags are to be set.</param>
        /// <param name="flags">The flags to set.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void SetFileSystemFlags(this Device device, LfsDirtyFlags flags, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.LfsSetFileSystemStatusFlags)
                {
                    Data = flags,
                    OnSuccess = (c, p, r) => device.FileSystemFlags = flags,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(SetFileSystemFlags);
            }
        }

        /// <summary>
        /// Protocol commands necessary for SetFileSystemFlags.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> SetFileSystemFlagsProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsSetFileSystemStatusFlags
        };

        private static void SetFileSystemFlags(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            data.Succeeded = SetDirtyFlags.Create((LfsDirtyFlags)data.Data).Execute<bool>(device.Port, data);
        }

        #endregion // SetFileSystemFlags
    }
}

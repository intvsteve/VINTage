// <copyright file="DeviceHelpers.UpdateDeviceName.cs" company="INTV Funhouse">
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
    /// Implementation for UpdateDeviceName. 
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region UpdateDeviceName

        /// <summary>
        /// Sets the device's name.
        /// </summary>
        /// <param name="device">The device whose name is to be set.</param>
        /// <param name="newName">The new name for the device.</param>
        /// <param name="errorHandler">Error handler to report any problems.</param>
        public static void UpdateDeviceName(this Device device, string newName, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_UpdateDeviceName_Title,
                    FailureMessage = Resources.Strings.SetDeviceNameCommand_ErrorMessage,
                    OnFailure = errorHandler,
                    OnSuccess = (c, p, r) => device.CustomName = newName,
                    Data = newName
                };
                executeCommandTaskData.StartTask(UpdateDeviceName);
            }
        }

        /// <summary>
        /// Protocol commands necessary for UpdateDeviceName.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> UpdateDeviceNameProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsUploadDataBlockToRam,
            ProtocolCommandId.LfsUpdateGftFromRam
        };

        private static void UpdateDeviceName(AsyncTaskData taskData)
        {
            // Skip setting dirty flags on this -- is that too risky?
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var newName = data.Data as string;
            var rootFile = device.FileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
            rootFile.ShortName = newName;
            var succeeded = false;
            data.Result = UploadDataBlockToRam.Create(0, FileSystemHelpers.ToFileByteSerializer(rootFile), 0).Execute(device.Port, data, out succeeded);
            if (succeeded)
            {
                data.Result = UpdateGlobalFileTable.Create(0, new[] { rootFile }).Execute(device.Port, data, out succeeded);
            }
        }

        #endregion // UpdateDeviceName
    }
}

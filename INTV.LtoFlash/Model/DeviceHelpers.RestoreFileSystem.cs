// <copyright file="DeviceHelpers.RestoreFileSystem.cs" company="INTV Funhouse">
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
    /// Implementation for RestoreFileSystem.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region RestoreFileSystem

        /// <summary>
        /// Executes a restore of the file system on a Locutus device.
        /// </summary>
        /// <param name="device">The device whose data is to be restored from a selected backup.</param>
        /// <param name="restoreFromDirectory">The directory from which to do the restore.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Function to call if an error occurs. If it returns <c>true</c>, the error is considered handled</param>
        public static void RestoreFileSystem(this Device device, string restoreFromDirectory, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_RestoreFileSystem_Title,
                    ProgressUpdateMode = ExecuteDeviceCommandProgressUpdateMode.Custom,
                    Data = restoreFromDirectory,
                    OnSuccess = onCompleteHandler,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(RestoreFileSystem, true, 0.5);
            }
        }

        /// <summary>
        /// Protocol commands necessary for BackupFileSystem.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> RestoreFileSystemProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsGetFileSystemStatusFlags,
            ProtocolCommandId.LfsSetFileSystemStatusFlags,
            ProtocolCommandId.LfsUploadDataBlockToRam,
            ProtocolCommandId.LfsChecksumDataBlockInRam,
            ProtocolCommandId.LfsDeleteDirectory,
            ProtocolCommandId.LfsDeleteFile,
            ProtocolCommandId.LfsDeleteFork,
            ProtocolCommandId.LfsCreateForkFromRam,
            ProtocolCommandId.LfsUpdateGftFromRam,
            ProtocolCommandId.LfsUpdateGdtFromRam,
            ProtocolCommandId.LfsDownloadGlobalTables
        };

        private static void RestoreFileSystem(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var succeeded = false;

            try
            {
                var restoreDirectory = data.Data as string;
                var menuLayoutPath = System.IO.Path.Combine(restoreDirectory, Configuration.Instance.DefaultMenuLayoutFileName);
                var menuLayout = MenuLayout.Load(menuLayoutPath);

                // First bool is whether to reset menu position data (false => RESTORE IT).
                // Second bool is whether to update root file name (true => RESTORE IT).
                data.Data = new Tuple<MenuLayout, bool, bool>(menuLayout, false, true);
                SyncHostToDevice(data);
                succeeded = data.Succeeded;
            }
            finally
            {
                data.Succeeded = succeeded;
            }
        }

        #endregion RestoreFileSystem
    }
}

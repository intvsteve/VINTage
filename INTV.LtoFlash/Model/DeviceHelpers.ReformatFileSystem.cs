// <copyright file="DeviceHelpers.ReformatFileSystem.cs" company="INTV Funhouse">
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
    /// Implementation for ReformatFileSystem.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region ReformatFileSystem

        /// <summary>
        /// Reformats the file system on a Locutus device.
        /// </summary>
        /// <param name="device">The target Locutus device whose file system is to be reformatted.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        /// <param name="onComplete">Action to execute when reformat operation is complete. This value may be <c>null</c>.</param>
        public static void ReformatFileSystem(this Device device, DeviceCommandErrorHandler errorHandler, Action onComplete)
        {
            if (device.IsSafeToStartCommand())
            {
                var reformatFileSystemTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.LfsReformatFileSystem)
                {
                    OnSuccess = (c, p, r) =>
                    {
                        var results = (Tuple<FileSystem, FileSystemStatistics>)r;
                        device.FileSystem = results.Item1;
                        device.FileSystemStatistics = results.Item2;
                        if (onComplete != null)
                        {
                            onComplete();
                        }
                    },
                    OnFailure = errorHandler
                };
                reformatFileSystemTaskData.StartTask(ReformatFileSystem);
            }
        }

        /// <summary>
        /// Protocol commands necessary for ReformatFileSystem.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> ReformatFileSystemProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsGetFileSystemStatusFlags,
            ProtocolCommandId.LfsSetFileSystemStatusFlags,
            ProtocolCommandId.LfsReformatFileSystem,
            ProtocolCommandId.LfsGetStatistics
        };

        private static void ReformatFileSystem(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            data.Succeeded = SetDirtyFlags.Create(LfsDirtyFlags.FileSystemUpdateInProgress).Execute<bool>(device.Port, data);
            if (data.Succeeded)
            {
                data.Succeeded = Reformat.Instance.Execute<bool>(device.Port, data);
            }
            if (data.Succeeded)
            {
                data.Succeeded = SetDirtyFlags.Create(LfsDirtyFlags.None).Execute<bool>(device.Port, data);
            }
            if (data.Succeeded)
            {
                var fileSystem = DownloadFileSystemTables.Instance.Execute<FileSystem>(device.Port, data);
                if (data.Succeeded)
                {
                    var fileSystemStatistics = Model.Commands.GetFileSystemStatistics.Instance.Execute<FileSystemStatistics>(device.Port, data);
                    data.Result = new Tuple<FileSystem, FileSystemStatistics>(fileSystem, fileSystemStatistics);
                }
            }
        }

        #endregion // ReformatFileSystem
    }
}

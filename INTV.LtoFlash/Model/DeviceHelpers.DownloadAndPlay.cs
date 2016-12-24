// <copyright file="DeviceHelpers.DownloadAndPlay.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for DownloadAndPlay.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region DownloadAndPlay

        /// <summary>
        /// Downloads a ROM to Locutus to be executed immediately.
        /// </summary>
        /// <param name="device">The target Locutus device to receive and execute the given ROM.</param>
        /// <param name="programRom">The ROM to play.</param>
        /// <param name="programName">The friendly name of the program to run.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void DownloadAndPlay(this Device device, IRom programRom, string programName, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.DownloadAndPlay)
                {
                    Title = string.Format(Resources.Strings.DeviceCommand_DownloadAndPlay_TitleFormat, programName),
                    ProgressUpdateMode = ExecuteDeviceCommandProgressUpdateMode.Custom,
                    FailureMessage = string.Format(Resources.Strings.DeviceCommand_DownloadAndPlay_FailedFormat, programName),
                    OnFailure = errorHandler,
                    Data = programRom
                };
                executeCommandTaskData.StartTask(DownloadAndPlay);
            }
        }

        /// <summary>
        /// Protocol commands necessary for DownloadAndPlay.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> DownloadAndPlayProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.DownloadAndPlay,
        };

        private static void DownloadAndPlay(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var rom = data.Data as IRom;
            var luigiFilePath = rom.PrepareForDeployment(LuigiGenerationMode.Passthrough);
            var luigiFile = new FileSystemFile(luigiFilePath);
            data.Succeeded = Commands.DownloadAndLaunch.Create(luigiFile).Execute<bool>(device.Port, data);
        }

        #endregion // DownloadAndPlay
    }
}

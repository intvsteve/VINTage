﻿// <copyright file="DeviceHelpers.SetConfiguration.cs" company="INTV Funhouse">
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
    /// Implementation for SetConfiguration.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region SetConfiguration

        /// <summary>
        /// Updates the device configuration settings.
        /// </summary>
        /// <param name="device">The target Locutus device whose configuration is to be set.</param>
        /// <param name="newConfigurationFlags">The new configuration data.</param>
        /// <param name="onCompleteHandler">Success handler, used to report successful execution of the command.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void SetConfiguration(this Device device, DeviceStatusFlags newConfigurationFlags, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.SetConfiguration)
                {
                    Data = newConfigurationFlags,
                    OnSuccess = onCompleteHandler,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(SetConfiguration);
            }
        }

        /// <summary>
        /// Protocol commands necessary for SetConfiguration.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> SetConfigurationProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.SetConfiguration
        };

        private static void SetConfiguration(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var flags = (DeviceStatusFlags)data.Data;
            data.Succeeded = Commands.SetConfiguration.Create(flags).Execute<bool>(device.Port, data);
        }

        #endregion // SetConfiguration
    }
}

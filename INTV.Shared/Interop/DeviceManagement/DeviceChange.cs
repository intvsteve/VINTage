// <copyright file="DeviceChange.cs" company="INTV Funhouse">
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

#define REPORT_EMPTY_DEVICE_NAMES

using System;
using System.Collections.Generic;
using INTV.Core.Model.Device;

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// This class provides a means to process the WM_DEVICECHANGE message.
    /// </summary>
    public static partial class DeviceChange
    {
        #region Configuration Data Parameter Names

        /// <summary>
        /// Key to use the arrival to or departure from or other change in state to a device in the system.
        /// </summary>
        /// <remarks>The value stored with this key is typically an implementation-specific name for the device. For example,
        /// it could be the port name for a serial port, an IP address, or the base name used for a named pipe connection.</remarks>
        public const string SystemReportedDeviceName = "Name";

        /// <summary>
        /// Key to use to indicate that the system is reporting the arrival of a new device in the system.
        /// </summary>
        /// <remarks>The value stored with this key should be an int having the value zero. It is used to indicate the
        /// spontaneous arrival of a device in the system, so interested clients can make decisions regarding what to
        /// do in response to the event.</remarks>
        public const string SystemReportedDeviceArrival = "Arrived";

        /// <summary>
        /// Key to use to indicate that the system is reporting the departuer of a device from the system.
        /// </summary>
        /// <remarks>The value stored with this key should be an int having the value zero. It is used to indicate the
        /// spontaneous departure of a device from the system, so interested clients can make decisions regarding what to
        /// do in response to the event.</remarks>
        public const string SystemReportedDeviceDeparture = "Departed";

        #endregion Configuration Data Parameter Names

        /// <summary>
        /// This event is raised when a device has been added to the system.
        /// </summary>
        public static event EventHandler<DeviceChangeEventArgs> DeviceAdded;

        /// <summary>
        /// This event is raised when a device has been removed from the system.,
        /// </summary>
        public static event EventHandler<DeviceChangeEventArgs> DeviceRemoved;

        /// <summary>
        /// Report that a device has been added to or detected in the system.
        /// </summary>
        /// <param name="sender">Entity raising the event.</param>
        /// <param name="deviceName">The name of the device.</param>
        /// <param name="deviceType">The type of the device.</param>
        /// <param name="deviceState">The device-specific state information.</param>
        public static void ReportDeviceAdded(object sender, string deviceName, ConnectionType deviceType, IDictionary<string, object> deviceState)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceName), "Null or empty device name is not allowed.");
            DebugOutputIf(string.IsNullOrEmpty(deviceName), "ReportDeviceAdded received empty device name.");

            INTV.Shared.Utility.SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() =>
                {
                    var deviceAdded = DeviceAdded;
                    if (deviceAdded != null)
                    {
                        System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceName), "Null or empty device name is not allowed.");
                        DebugOutputIf(string.IsNullOrEmpty(deviceName), "ReportDeviceAdded (dispatched to main thread) received empty device name.");
                        if (deviceState == null)
                        {
                            deviceState = new Dictionary<string, object>();
                        }
                        deviceState[SystemReportedDeviceName] = deviceName;
                        deviceAdded(sender, new DeviceChangeEventArgs(deviceName, deviceType, deviceState));
                    }
                }));
        }

        /// <summary>
        /// Report that a device removal has been detected, or is no longer available.
        /// </summary>
        /// <param name="sender">Entity raising the event.</param>
        /// <param name="deviceName">The name of the device.</param>
        /// <param name="deviceType">The type of the device.</param>
        /// <param name="deviceState">The device-specific state information.</param>
        public static void ReportDeviceRemoved(object sender, string deviceName, ConnectionType deviceType, IDictionary<string, object> deviceState)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceName), "Null or empty device name is not allowed.");
            DebugOutputIf(string.IsNullOrEmpty(deviceName), "ReportDeviceRemoved received empty device name.");

            INTV.Shared.Utility.SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() =>
                {
                    var deviceRemoved = DeviceRemoved;
                    if (deviceRemoved != null)
                    {
                        System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(deviceName), "Null or empty device name is not allowed.");
                        DebugOutputIf(string.IsNullOrEmpty(deviceName), "ReportDeviceRemoved (dispatched to main thread) received empty device name.");
                        if (deviceState == null)
                        {
                            deviceState = new Dictionary<string, object>();
                        }
                        deviceState[SystemReportedDeviceName] = deviceName;
                        deviceRemoved(sender, new DeviceChangeEventArgs(deviceName, deviceType, deviceState));
                    }
                }));
        }

        /// <summary>
        /// Used for the system to report that a device has been added (arrived).
        /// </summary>
        /// <param name="sender">Entity raising the event.</param>
        /// <param name="deviceName">The name of the device.</param>
        /// <param name="deviceType">The type of the device.</param>
        /// <remarks>Operating-system-specific code should use this to report the arrival of a device to the system.</remarks>
        public static void SystemReportsDeviceAdded(object sender, string deviceName, ConnectionType deviceType)
        {
            var configData = new Dictionary<string, object>()
            {
                { DeviceChange.SystemReportedDeviceName, deviceName },
                { DeviceChange.SystemReportedDeviceArrival, 0 }
            };
            ReportDeviceAdded(sender, deviceName, deviceType, configData);
        }

        /// <summary>
        /// Used for the system to report that a device has been removed (departed).
        /// </summary>
        /// <param name="sender">Entity raising the event.</param>
        /// <param name="deviceName">The name of the device.</param>
        /// <param name="deviceType">The type of the device.</param>
        /// <remarks>Operating-system-specific code should use this to report the departure of a device from the system.</remarks>
        public static void SystemReportsDeviceRemoved(object sender, string deviceName, ConnectionType deviceType)
        {
            var configData = new Dictionary<string, object>()
            {
                { DeviceChange.SystemReportedDeviceName, deviceName },
                { DeviceChange.SystemReportedDeviceDeparture, 0 }
            };
            ReportDeviceRemoved(sender, deviceName, deviceType, configData);
        }

        /// <summary>
        /// Determines if is device state change was reported by operating system-specific sources.
        /// </summary>
        /// <returns><c>true</c> if is device change was reported by the system; otherwise, <c>false</c>.</returns>
        /// <param name="deviceState">Device state data.</param>
        public static bool IsDeviceChangeFromSystem(IDictionary<string, object> deviceState)
        {
            object value = null;
            bool isFromSystem = deviceState.TryGetValue(DeviceChange.SystemReportedDeviceArrival, out value) || deviceState.TryGetValue(DeviceChange.SystemReportedDeviceDeparture, out value);
            if (isFromSystem && (value != null) && (value.GetType() == typeof(int)))
            {
                isFromSystem = (int)value == 0;
            }
            return isFromSystem;
        }

        [System.Diagnostics.Conditional("REPORT_EMPTY_DEVICE_NAMES")]
        private static void DebugOutputIf(bool condition, object message)
        {
            System.Diagnostics.Debug.WriteLineIf(condition, message);
        }
    }
}

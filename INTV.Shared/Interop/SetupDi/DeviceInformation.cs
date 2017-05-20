// <copyright file="DeviceInformation.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using System.Globalization;
using System.Text.RegularExpressions;

namespace INTV.Shared.Interop.SetupDi
{
    /// <summary>
    /// Very basic wrapper class to get some information about devices in the system.
    /// </summary>
    public partial class DeviceInformation : IDisposable
    {
        /// <summary>Class name to use when looking for serial (and LPT?) ports on the system.</summary>
        public static readonly string SerialPort = "Ports";

        /// <summary>Class name to use when looking for USB devices on the system.</summary>
        public static readonly string Usb = "USB";

        /// <summary>Property to use to get the name of a device.</summary>
        public static readonly object DisplayNameProperty = NativeMethods.DEVPKEY_NAME;

        private IntPtr _deviceInfoSet;
        private object _deviceInfoData;

        /// <summary>
        /// Initialize a new instance of DeviceInformation.
        /// </summary>
        /// <param name="deviceInfoSet">The device information set.</param>
        /// <param name="deviceInfoData">The device information data.</param>
        private DeviceInformation(IntPtr deviceInfoSet, object deviceInfoData)
        {
            _deviceInfoSet = deviceInfoSet;
            _deviceInfoData = deviceInfoData;
        }

        ~DeviceInformation()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the device ID of the device.
        /// </summary>
        public object InstanceId { get; private set; }

        /// <summary>
        /// Retrieves an enumeration of the devices that match the given class, product, and vendor ID.
        /// </summary>
        /// <param name="className">The class of devices to enumerate.</param>
        /// <param name="propertyForId">The property to use to retrieve vendor and property ID values. If <c>null</c>, <see cref="DeviceInstanceId"/> is used.</param>
        /// <param name="propertyForName">The property to use to retrieve the name. If <c>null</c>, <see cref="DisplayNameProperty"/> is used.</param>
        /// <param name="isMatchingDevice">A filter function to determine if a device should be included in the results The value of propertyForId is sent to this function..</param>
        /// <returns>An enumeration of devices of the given class with matching vendor and product IDs.</returns>
        /// <remarks>It is assumed that the value returned via <paramref name="propertyForId"/> will be of a format that can be split on backslash characters, and that the
        /// vendor and product ID values are hexadecimal string values in the property. Perhaps this is only going to work for
        /// USB devices.... but that's all that's required at this point in time. <see cref="https://msdn.microsoft.com/en-us/windows/hardware/drivers/install/device-identification-strings"/></remarks>
        public static IEnumerable<DeviceInformation> GetDeviceInformation(string className, object propertyForId, object propertyForName, Func<object, bool> isMatchingDevice)
        {
            var devices = new List<DeviceInformation>();
            var matchingClassIds = NativeMethods.GetClassGuids(className);
            foreach (var classId in matchingClassIds)
            {
                var deviceInfoSet = NativeMethods.SetupDiGetClassDevs(classId);
                if (deviceInfoSet != IntPtr.Zero)
                {
                    uint memberIndex = 0;
                    var deviceInfoData = NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, memberIndex);
                    while (deviceInfoData != null)
                    {
                        var deviceInstanceId = NativeMethods.GetDeviceProperty(deviceInfoSet, deviceInfoData, propertyForId ?? NativeMethods.DEVPKEY_Device_InstanceId);
                        if ((isMatchingDevice == null) || isMatchingDevice(deviceInstanceId))
                        {
                            var deviceInformation = new DeviceInformation(deviceInfoSet, deviceInfoData);
                            deviceInformation.InstanceId = deviceInstanceId;
                            deviceInformation.Name = NativeMethods.GetDeviceProperty(deviceInfoSet, deviceInfoData, propertyForName ?? NativeMethods.DEVPKEY_NAME) as string;
                            if (deviceInformation.Name == null)
                            {
                                deviceInformation.Name = deviceInstanceId as string;
                            }
                            devices.Add(deviceInformation);
                        }
                        ++memberIndex;
                        deviceInfoData = NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, memberIndex);
                    }
                }
            }
            return devices;
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_deviceInfoSet != IntPtr.Zero)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(_deviceInfoSet);
                _deviceInfoSet = IntPtr.Zero;
            }
        }

        #endregion // IDisposable
    }
}

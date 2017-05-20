// <copyright file="SerialConnectionPolicy.WPF.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using INTV.Core.Model.Device;
using INTV.Shared.Interop.SetupDi;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Windows-specific implementation.
    /// </summary>
    public sealed partial class SerialConnectionPolicy
    {
        private static readonly string VendorAndProductString = string.Format(CultureInfo.InvariantCulture, "VID_{0:X4}&PID_{1:X4}", Device.UsbVendorId, Device.UsbProductId);

        private IEnumerable<DeviceInformation> _ltoFlashUsbDevices;
        private IEnumerable<DeviceInformation> _ltoFlashSerialDevices;

        private IEnumerable<DeviceInformation> LtoFlashUsbDevices
        {
            get
            {
                lock (this)
                {
                    if (_ltoFlashUsbDevices == null)
                    {
                        _ltoFlashUsbDevices = DeviceInformation.GetDeviceInformation(DeviceInformation.Usb, null, DeviceInformation.DeviceBusReportedDescription, IsMatchingDevice).Where(d => IsLtoFlashDevice(d.Name));
                    }
                    return _ltoFlashUsbDevices;
                }
            }
        }

        private IEnumerable<DeviceInformation> LtoFlashSerialDevices
        {
            get
            {
                lock (this)
                {
                    if (_ltoFlashSerialDevices == null)
                    {
                        _ltoFlashSerialDevices = DeviceInformation.GetDeviceInformation(DeviceInformation.SerialPort, DeviceInformation.DeviceParentProperty, null, IsMatchingDevice);
                    }
                    return _ltoFlashSerialDevices;
                }
            }
        }

        /// <summary>
        /// Resets the cached device data.
        /// </summary>
        internal void Reset()
        {
            lock (this)
            {
                _ltoFlashUsbDevices = null;
                _ltoFlashSerialDevices = null;
            }
        }

        private static bool IsMatchingDevice(object deviceIdValue)
        {
            var isMatchingDevice = false;
            var deviceIdString = deviceIdValue as string;
            if (!string.IsNullOrEmpty(deviceIdString))
            {
                isMatchingDevice = deviceIdString.Contains(VendorAndProductString);
            }
            return isMatchingDevice;
        }

        private static bool IsLtoFlashDevice(string deviceName)
        {
            var isMatch = deviceName == Device.UsbProductName; // This is case-sensitive!
            if (!isMatch && (INTV.Shared.Utility.OSVersion.Current.Major == 6) && (INTV.Shared.Utility.OSVersion.Current.Minor == 0))
            {
                // Stupid Windows Vista. Doesn't have the proper name available to us. So essentially, revert to the Windows xp mode.
                isMatch = true;
            }
            return isMatch;
        }

        private bool OSExclusiveAccess(IConnection connection)
        {
            // Get LTO Flash! USB devices. The instance ID will be useful. These are filtered by devices with the name 'LTO Flash!'
            var ltoFlashDevices = LtoFlashUsbDevices;

            // Next, get serial ports that have matching USB vendor and product IDs. Note that we use the parent as the 'instance id'.
            var connectedLtoFlashSerialPorts = LtoFlashSerialDevices;

            // Find the serial port with matching COM[n] name.
            var portWithSameName = connectedLtoFlashSerialPorts.FirstOrDefault(p => p.Name.Contains('(' + connection.Name + ')'));

            // If the instance ID of a USB device matches the parent of the port with the matching name, then the input port really is a LTO Flash! device.
            var isLtoFlashPort = (portWithSameName != null) && (ltoFlashDevices.FirstOrDefault(d => d.InstanceId.Equals(portWithSameName.InstanceId)) != null);
            return isLtoFlashPort;
        }
    }
}

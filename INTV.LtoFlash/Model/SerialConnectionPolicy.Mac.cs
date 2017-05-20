// <copyright file="SerialConnectionPolicy.Mac.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model.Device;
using INTV.Shared.Interop.IOKit;

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public sealed partial class SerialConnectionPolicy
    {
        /// <summary>
        /// No-op. Other platforms may cache information if checking access is not performant.
        /// </summary>
        internal void Reset()
        {
        }

        private bool OSExclusiveAccess(IConnection connection)
        {
            var connectedDeviceSerialNumbers = GetConnectedDeviceSerialNumbers();
            var exclusive = IsLtoFlashSerialPort(connection.Name, connectedDeviceSerialNumbers);
            return exclusive;
        }

        /// <summary>
        /// Determines if the given serial port is on a LTO Flash! device.
        /// </summary>
        /// <param name="portName">Serial port name.</param>
        /// <param name="ltoFlashDeviceSerialNumbers">The USB serial numbers of connected LTO Flash! devices connected to the system.</param>
        /// <returns><c>true</c> the given serial port is from an LTO Flash! device; otherwise, <c>false</c>.</returns>
        /// <remarks>Requires that the serial port name ends with a USB serial number from the device. See the remarks on the GetConnectedDevices method for more details.</remarks>
        private static bool IsLtoFlashSerialPort(string portName, IEnumerable<string> ltoFlashDeviceSerialNumbers)
        {
            return ltoFlashDeviceSerialNumbers.Any(s => portName.EndsWith(s));
        }

        /// <summary>
        /// Gets the USB serial numbers of connected LTO Flash! devices.
        /// </summary>
        /// <returns>The USB serial numbers of LTO Flash! devices currently connected to the system.</returns>
        /// <remarks>This technique uses the IOKit to enumerate USB devices connected to the system that have the
        /// vendor and product IDs matching what the LTO Flash! hardware uses. The matches are then refined further
        /// by examining the user-visible vendor and product strings, which are programmed into the chip during
        /// LTO Flash! device manufacture. These disambiguate actual LTO Flash! devices from other devices connected
        /// to the system that may be using the same USB to Serial chipset from FTDI.
        /// NOTE: If LTO Flash! hardware is ever revised to use a different USB-to-serial chipset, or driver
        /// behavior changes w.r.t. port naming, etc. this will need to be revised.</remarks>
        private static IEnumerable<string> GetConnectedDeviceSerialNumbers()
        {
            var connectedDevices = new List<string>();
            using (var masterPort = new IOMachPort())
            {
                using (var iterator = masterPort.GetUSBServicesIterator(Device.UsbVendorId, Device.UsbProductId))
                {
                    if ((iterator != null) && iterator.IsValid)
                    {
                        var validEntry = true;
                        do
                        {
                            using (var deviceEntry = iterator.Next<IORegistryEntry>())
                            {
                                validEntry = deviceEntry != null;
                                if (validEntry)
                                {
                                    var vendor = deviceEntry.GetProperty<NSString>(IOKitHelpers.kUSBVendorString);
                                    var product = deviceEntry.GetProperty<NSString>(IOKitHelpers.kUSBProductString);
                                    if ((vendor == Device.UsbVendorName) && (product == Device.UsbProductName))
                                    {
                                        // The serial port for the FTDI chip are named based off the USB serial number.
                                        var deviceSerial = deviceEntry.GetProperty<NSString>(IOKitHelpers.kUSBSerialNumberString);
                                        if (!string.IsNullOrEmpty(deviceSerial))
                                        {
                                            connectedDevices.Add(deviceSerial);
                                        }
                                    }
                                }
                            }
                        } while(validEntry);
                    }
                }
            }
            return connectedDevices;
        }
    }
}

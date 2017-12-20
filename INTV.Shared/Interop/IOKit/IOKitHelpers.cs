// <copyright file="IOKitHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

////#define ENABLE_DEBUG_OUTPUT

using System.Collections.Generic;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// Various helper methods working with IOKit.
    /// </summary>
    public static class IOKitHelpers
    {
        #region USBSpec.h

        /// <summary>The key used to get the USB Vendor Name property from an instance of IOKitRegistryEntry.</summary>
        public static readonly string KUSBVendorString = "USB Vendor Name";

        /// <summary>The key used to get the USB Product Name property from an instance of IOKitRegistryEntry.</summary>
        public static readonly string KUSBProductString = "USB Product Name";

        /// <summary>The key used to get the USB Serial Number property from an instance of IOKitRegistryEntry.</summary>
        public static readonly string KUSBSerialNumberString = "USB Serial Number";

        #endregion // USBSpec.h

        /// <summary>
        /// Enumerates the serial ports.
        /// </summary>
        /// <param name="iterator">An IOKit enumerator object.</param>
        /// <param name="exclusionFilter">A function that will filter the entries in the returned enumerable.</param>
        /// <returns>An enumerable of the serial ports in the system that are not excluded by <paramref name="exclusionFilter"/>.</returns>
        public static IEnumerable<string> EnumerateSerialPorts(this IOIterator iterator, System.Predicate<string> exclusionFilter)
        {
            var ports = new List<string>();

            if ((iterator != null) && iterator.IsValid)
            {
                var validEntry = true;
                do
                {
                    using (var serialPortRegistryEntry = iterator.Next<IORegistryEntry>())
                    {
                        validEntry = serialPortRegistryEntry != null;
                        if (validEntry)
                        {
                            var portName = CreateSerialPortName(serialPortRegistryEntry.GetProperty<NSString>(NativeMethods.kIOTTYDeviceKey));
                            DebugOutput("Discovered serial port: " + portName);
                            if ((exclusionFilter == null) || !exclusionFilter(portName))
                            {
                                ports.Add(portName);
                            }
                        }
                    }
                }
                while (validEntry);
            }

            return ports;
        }

        /// <summary>
        /// A filter function that will exclude any Bluetooth serial ports.
        /// </summary>
        /// <param name="portName">The name of the port to check.</param>
        /// <returns><c>true</c> if the port is a Bluetooth serial port.</returns>
        public static bool BluetoothPortsExclusionFilter(string portName)
        {
            return portName.ToLowerInvariant().Contains("bluetooth");
        }

        private static string CreateSerialPortName(string ioKitSerialPortName)
        {
            var serialPortName = "/dev/cu." + ioKitSerialPortName;
            return serialPortName;
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

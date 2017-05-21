// <copyright file="SerialPortConnection.Mac.cs" company="INTV Funhouse">
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
using System.Linq;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.Interop.IOKit;
using INTV.Shared.Interop.DeviceManagement;

namespace INTV.Shared.Model.Device
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class SerialPortConnection
    {
        static partial void UpdateAvailablePorts()
        {
            var ports = Enumerable.Empty<string>();
            switch(DeviceManagementInterfaceKindHelpers.GetKind())
            {
                case DeviceManagementInterfaceKind.IOKit:
                    // Need to set up an Autorelease pool here since we may be doing
                    // this on a worker thread, and it makes some IOKit calls. Specifically,
                    // some NSObject instances may be created, and thus the need for the pool.
                    using (var pool = new NSAutoreleasePool())
                    {
                        using (var masterPort = new IOMachPort())
                        {
                            using (var iterator = masterPort.GetRS232SerialServicesIterator())
                            {
                                DebugOutput("Got iterator of: " + iterator);
                                ports = iterator.EnumerateSerialPorts(IOKitHelpers.BluetoothPortsExclusionFilter);
                            }
                        }
                    }
                    break;
                case DeviceManagementInterfaceKind.Dev:
                    // NOTE: The MONO serial implementation lists only /dev/tty* entries. So we filter by that, then substitute since we want cu instead of tty.
                    ports = System.IO.Ports.SerialPort.GetPortNames().Where(p => p.StartsWith("/dev/tty.") && !IOKitHelpers.BluetoothPortsExclusionFilter(p)).Select(p => System.Text.RegularExpressions.Regex.Replace(p, "^/dev/tty", "/dev/cu"));
                    break;
            }
            _availablePorts = ports.ToArray();
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void OpenPort()
        {
            try
            {
                Port.Open();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                var desiredBaudRate = Port.BaudRate;
                Port.BaudRate = 9600;
                Port.Open();
                var stream = Port.BaseStream;
                // This HORRIBLE HACK is needed because we need to get the file descriptor in order to use ioctl to set
                // the baud rate to what we want. If this ever stops working, we're boned.
                System.Reflection.FieldInfo fi = stream.GetType().GetField("fd", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var fd = (int)fi.GetValue(stream);
                var result = NativeMethods.ioctl(fd, NativeMethods.IOSSIOSPEED, ref desiredBaudRate);
                if (result != 0)
                {
                    throw;
                }
            }
        }
    }
}

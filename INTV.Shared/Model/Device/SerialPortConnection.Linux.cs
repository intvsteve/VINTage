// <copyright file="SerialPortConnection.Linux.cs" company="INTV Funhouse">
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

using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Mono.Unix;

namespace INTV.Shared.Model.Device
{
    /// <summary>
    /// Linux-specific implementation.
    /// </summary>
    public partial class SerialPortConnection
    {
        private static ConcurrentDictionary<string, string> _devTtyPathToDeviceId = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets the serial port by-id style path from a standard /dev/tty* style path.
        /// </summary>
        /// <param name="devTtyPath">A serial port device path path of the traditional /dev/tty* style.</param>
        /// <returns>The serial port from /dev/serial/by-id for the given /dev/tty* style path.</returns>
        public static string GetSerialPortIdFromDevTtyPath(string devTtyPath)
        {
            string portIdString;
            if (!string.IsNullOrEmpty(devTtyPath) && _devTtyPathToDeviceId.TryGetValue(devTtyPath, out portIdString))
            {
                portIdString = System.IO.Path.GetFileName(portIdString);
            }
            else
            {
                portIdString = null;
            }
            return portIdString;
        }

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        static partial void UpdateAvailablePorts()
        {
            // We'll just see how this holds up... It may only work for USB-serial devices, and not hard-wired serial ports.
            // If the need for detecting those arises, may need to call out to ioctl functions to do it.
            _devTtyPathToDeviceId.Clear();
            if (Directory.Exists("/dev/serial/"))
            {
                var serialDevices = Directory.EnumerateFiles("/dev/serial/by-id/");
                foreach (var serialDevice in serialDevices)
                {
                    var linkInfo = UnixFileSystemInfo.GetFileSystemEntry(serialDevice) as UnixSymbolicLinkInfo;
                    _devTtyPathToDeviceId[linkInfo.GetContents().FullName] = serialDevice;
                }
            }
            _availablePorts = _devTtyPathToDeviceId.Keys.ToArray();
        }

        private void OpenPort()
        {
            Port.Open();
        }
    }
}

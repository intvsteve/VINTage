// <copyright file="SerialConnectionPolicy.Gtk.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Device;
using INTV.Shared.Model.Device;

namespace INTV.LtoFlash.Model
{
    public sealed partial class SerialConnectionPolicy
    {
        private static readonly string SanitizedVendorName = Device.UsbVendorName.Replace(' ', '_');
        private static readonly string SanitizedProductName = Device.UsbProductName.Replace(' ', '_').Replace('!', '_');

        /// <summary>
        /// No-op. Other platforms may cache information if checking access is not performant.
        /// </summary>
        internal void Reset()
        {
        }

        private bool OSExclusiveAccess(IConnection connection)
        {
            var isLtoFlashPort = connection.Type == INTV.Core.Model.Device.ConnectionType.Serial;
            if (isLtoFlashPort)
            {
                var portIdString = SerialPortConnection.GetSerialPortIdFromDevTtyPath(connection.Name);
                if (!string.IsNullOrEmpty(portIdString))
                {
                    var portIdParts = portIdString.Split('-');
                    if (portIdParts.Length >= 3)
                    {
                        var vendorAndName = portIdParts[1];
                        isLtoFlashPort = vendorAndName.StartsWith(SanitizedVendorName + '_' + SanitizedProductName);
                    }
                }
            }
            return isLtoFlashPort;
        }
    }
}

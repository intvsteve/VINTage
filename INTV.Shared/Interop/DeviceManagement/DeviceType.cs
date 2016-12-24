// <copyright file="DeviceType.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// Values in the DeviceBroadcastHeader structure's DeviceType field.
    /// </summary>
    public enum DeviceType : uint
    {
        /// <summary>
        /// Class of devices. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.
        /// </summary>
        DBT_DEVTYP_DEVICEINTERFACE = 0x00000005,

        /// <summary>
        /// File system handle. This structure is a DEV_BROADCAST_HANDLE structure.
        /// </summary>
        DBT_DEVTYP_HANDLE = 0x00000006,

        /// <summary>
        /// OEM- or IHV-defined device type. This structure is a DEV_BROADCAST_OEM structure.
        /// </summary>
        DBT_DEVTYP_OEM = 0x00000000,

        /// <summary>
        /// Port device (serial or parallel). This structure is a DEV_BROADCAST_PORT structure.
        /// </summary>
        DBT_DEVTYP_PORT = 0x00000003,

        /// <summary>
        /// Logical volume. This structure is a DEV_BROADCAST_VOLUME structure.
        /// </summary>
        DBT_DEVTYP_VOLUME = 0x00000002,
    }
}

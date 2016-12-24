// <copyright file="DeviceChangeEvent.cs" company="INTV Funhouse">
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
    /// The event that has occurred. This parameter can be one of the following values from the Dbt.h header file.
    /// </summary>
    public enum DeviceChangeEvent
    {
        /// <summary>
        /// A request to change the current configuration (dock or undock) has been canceled.
        /// </summary>
        DBT_CONFIGCHANGECANCELED = 0x0019,

        /// <summary>
        /// The current configuration has changed, due to a dock or undock.
        /// </summary>
        DBT_CONFIGCHANGED = 0x0018,

        /// <summary>
        /// A custom event has occurred.
        /// </summary>
        DBT_CUSTOMEVENT = 0x8006,

        /// <summary>
        /// A device or piece of media has been inserted and is now available.
        /// </summary>
        DBT_DEVICEARRIVAL = 0x8000,

        /// <summary>
        /// Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        /// </summary>
        DBT_DEVICEQUERYREMOVE = 0x8001,

        /// <summary>
        /// A request to remove a device or piece of media has been canceled.
        /// </summary>
        DBT_DEVICEQUERYREMOVEFAILED = 0x8002,

        /// <summary>
        /// A device or piece of media has been removed.
        /// </summary>
        DBT_DEVICEREMOVECOMPLETE = 0x8004,

        /// <summary>
        /// A device or piece of media is about to be removed. Cannot be denied.
        /// </summary>
        DBT_DEVICEREMOVEPENDING = 0x8003,

        /// <summary>
        /// A device-specific event has occurred.
        /// </summary>
        DBT_DEVICETYPESPECIFIC = 0x8005,

        /// <summary>
        /// A device has been added to or removed from the system.
        /// </summary>
        DBT_DEVNODES_CHANGED = 0x0007,

        /// <summary>
        /// Permission is requested to change the current configuration (dock or undock).
        /// </summary>
        DBT_QUERYCHANGECONFIG = 0x0017,

        /// <summary>
        /// The meaning of this message is user-defined.
        /// </summary>
        DBT_USERDEFINED = 0xFFFF
    }
}

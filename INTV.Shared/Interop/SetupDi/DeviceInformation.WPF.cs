// <copyright file="DeviceInformation.WPF.cs" company="INTV Funhouse">
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
using System.Linq;
using System.Text;

namespace INTV.Shared.Interop.SetupDi
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class DeviceInformation
    {
        /// <summary>Property to use to get the parent of a device. Useful for USB - Serial devices, for example.</summary>
        public static readonly object DeviceParentProperty = NativeMethods.PKEY_Device_Parent;

        /// <summary>Property to use to get the 'bus reported' name of a device.</summary>
        public static readonly object DeviceBusReportedDescription = NativeMethods.DEVPKEY_Device_BusReportedDeviceDesc;

        /// <summary>Property to use to get a device's instance ID.</summary>
        public static readonly object DeviceInstanceId = NativeMethods.DEVPKEY_Device_InstanceId;
    }
}

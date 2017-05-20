// <copyright file="NativeMethods.xp.cs" company="INTV Funhouse">
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
    /// Windows xp-specific implementation.
    /// </summary>
    internal static partial class NativeMethods
    {
        /// <summary>Expose property for device name.</summary>
        internal static readonly object DEVPKEY_NAME = SPDRP_PROPERTY.SPDRP_FRIENDLYNAME;

        /// <summary>Expose property for device description.</summary>
        internal static readonly object DEVPKEY_Device_BusReportedDeviceDesc = SPDRP_PROPERTY.SPDRP_DEVICEDESC;

        /// <summary>Expose property for device hardware ID.</summary>
        internal static readonly object DEVPKEY_Device_InstanceId = SPDRP_PROPERTY.SPDRP_HARDWAREID;

        /// <summary>
        /// Gets a device property.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to a device information set that contains a device instance for which to retrieve a device instance property.</param>
        /// <param name="deviceInfoData">A SP_DEVINFO_DATA structure.</param>
        /// <param name="propertyKey">The property to read.</param>
        /// <returns>The value of the property.</returns>
        /// <remarks>Actual parsing of the raw data returned by the p/Invoked method is only implemented for a few data types.</remarks>
        internal static object GetDeviceProperty(IntPtr deviceInfoSet, object deviceInfoData, object propertyKey)
        {
            object propertyValue = GetDeviceRegistryProperty(deviceInfoSet, deviceInfoData, propertyKey);
            return propertyValue;
        }
    }
}

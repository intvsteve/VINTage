// <copyright file="HardwareStatusFlags.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// These flags describe various hardware status bits on a Locutus device.
    /// </summary>
    [System.Flags]
    public enum HardwareStatusFlags : uint
    {
        /// <summary>
        /// No status flags are set.
        /// </summary>
        None,

        /// <summary>
        /// When set, indicates that Locutus is plugged into an Intellivision console that is in the powered on state.
        /// </summary>
        ConsolePowerOn = 1 << 0,

        /// <summary>
        /// When set, indicates that a new error log is available on the device.
        /// </summary>
        /// <remarks>NOTE: This is only available in firmware 1438 and later.</remarks>
        NewErrorLogAvailable = 1 << 1,

        /// <summary>
        /// When set, indicates that a crash log is available on the device.
        /// </summary>
        /// <remarks>NOTE: This is only available in firmware 1438 and later.</remarks>
        NewCrashLogAvailable = 1 << 2,

        /// <summary>
        /// This mask indentifies which hardware status flags are reserved for future use.
        /// </summary>
        ReservedMask = 0xFFFFFFF8,

        /// <summary>
        /// All hardware-related flags.
        /// </summary>
        AllFlags = 0xFFFFFFFF
    }

    /// <summary>
    /// These methods assist with conversion operations on HardwareStatusFlags.
    /// </summary>
    internal static class HardwareStatusFlagsHelpers
    {
        /// <summary>
        /// Places hardware status flags into the more generic DeviceStatusFlagsLo bit array.
        /// </summary>
        /// <param name="hardwareStatus">The hardware status flags to convert.</param>
        /// <returns>The flags placed in a DeviceStatusFlagsLo bit array.</returns>
        internal static DeviceStatusFlagsLo ToDeviceStatusFlagsLo(this HardwareStatusFlags hardwareStatus)
        {
            var deviceStatusFlags = (DeviceStatusFlagsLo)hardwareStatus;
            return deviceStatusFlags;
        }
    }
}

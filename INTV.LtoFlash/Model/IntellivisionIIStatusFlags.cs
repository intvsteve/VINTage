// <copyright file="IntellivisionIIStatusFlags.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// These flags describe how Locutus will attempt to patch compatibility with the Intellivision II console.
    /// </summary>
    [System.Flags]
    public enum IntellivisionIIStatusFlags : byte
    {
        /// <summary>
        /// No status flags are set.
        /// </summary>
        None,

        /// <summary>
        /// When set, this flag indicates that Locutus should attempt to patch only ROMs known to have compatibility problems with the Intellivision II.
        /// </summary>
        Conservative = 1 << 0,

        /// <summary>
        /// When set, Locutus will always attempt to bypass the Intellivision II lockout check.
        /// </summary>
        Aggressive = 1 << 1,

        /// <summary>
        /// The default value for this option.
        /// </summary>
        Default = Aggressive,

        /// <summary>
        /// This mask identifies which Intellivision II status flags are reserved for future use.
        /// </summary>
        ReservedMask = 0xFC,

        /// <summary>
        /// All Intellivision II-related flags.
        /// </summary>
        AllFlags = 0xFF
    }

    /// <summary>
    /// These methods assist with conversion operations on IntellivisionIIStatusFlags.
    /// </summary>
    internal static class IntellivisionIIStatusFlagsHelpers
    {
        private static readonly Dictionary<IntellivisionIIStatusFlags, string> ModeToStringsTable = new Dictionary<IntellivisionIIStatusFlags, string>()
        {
            { IntellivisionIIStatusFlags.None, Resources.Strings.IntellivisionIICompatibilityMode_Disabled },
            { IntellivisionIIStatusFlags.Conservative, Resources.Strings.IntellivisionIICompatibilityMode_Limited },
            { IntellivisionIIStatusFlags.Aggressive, Resources.Strings.IntellivisionIICompatibilityMode_Full },
            { IntellivisionIIStatusFlags.Aggressive | IntellivisionIIStatusFlags.Conservative, Resources.Strings.IntellivisionIICompatibilityMode_Full } // default on new device
        };

        /// <summary>
        /// Places Intellivision II status flags into the more generic <see cref="DeviceStatusFlagsLo"/> bit array.
        /// </summary>
        /// <param name="intellivisionIIStatus">The Intellivision II status flags to convert.</param>
        /// <returns>The flags placed in a <see cref="DeviceStatusFlagsLo"/> bit array.</returns>
        internal static DeviceStatusFlagsLo ToDeviceStatusFlagsLo(this IntellivisionIIStatusFlags intellivisionIIStatus)
        {
            var deviceStatusFlags = (DeviceStatusFlagsLo)((ulong)intellivisionIIStatus << DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset);
            return deviceStatusFlags;
        }

        /// <summary>
        /// Produce <see cref="DeviceStatusFlagsLo"/> containing updated <see cref="IntellivisionIIStatusFlags"/> for a given <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> whose updated flags are desired.</param>
        /// <param name="newIntellivisionIIFlags">New <see cref="IntellivisionIIStatusFlags"/> to apply to <paramref name="device"/>.</param>
        /// <returns>A new set of <see cref="DeviceStatusFlagsLo"/> with updated IntellivisionIIStatusFlags for <paramref name="device"/>.</returns>
        internal static DeviceStatusFlagsLo UpdateStatusFlags(this Device device, IntellivisionIIStatusFlags newIntellivisionIIFlags)
        {
            var deviceStatusFlags = device.ComposeStatusFlags() & ~DeviceStatusFlagsLo.IntellivisionIIStatusMask;
            deviceStatusFlags |= newIntellivisionIIFlags.ToDeviceStatusFlagsLo();
            return deviceStatusFlags;
        }

        /// <summary>
        /// Converts status flags into a display string.
        /// </summary>
        /// <param name="intellivisionIIStatus">The Intellivision II status flags to convert.</param>
        /// <returns>A display string for the flags.</returns>
        internal static string ToDisplayString(this IntellivisionIIStatusFlags intellivisionIIStatus)
        {
            return ModeToStringsTable[intellivisionIIStatus];
        }

        /// <summary>
        /// Converts a string into Intellivision II status flags.
        /// </summary>
        /// <param name="displayString">The string to convert.</param>
        /// <returns>The appropriate IntellivisionIIStatusFlags.</returns>
        internal static IntellivisionIIStatusFlags FromDisplayString(string displayString)
        {
            var intellivisionIIStatus = ModeToStringsTable.FirstOrDefault(k => k.Value == displayString as string).Key;
            return intellivisionIIStatus;
        }
    }
}

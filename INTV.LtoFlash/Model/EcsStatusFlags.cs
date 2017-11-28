// <copyright file="EcsStatusFlags.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.LtoFlash.Model
{
    [System.Flags]
    public enum EcsStatusFlags : byte
    {
        /// <summary>
        /// No compatibility mode is set. ECS ROMs remain fully enabled, if present.
        /// </summary>
        None,

        /// <summary>
        /// When this flag is set, the ECS ROMs remain enabled for ROMs that are known to either require
        /// the ECS to function, or that may use ECS features if present, e.g. extra sound channels.
        /// I.e. ECS ROM is DISABLED ONLY FOR KNOWN INCOMPATIBLE titles.
        /// </summary>
        EnabledForRequiredAndOptional = 1 << 0,

        /// <summary>
        /// When this flag is set, the ECS ROMs remain enabled only for ROMs that indicate the ECS is required
        /// or optional. I.e. ROMS remain enabled only when a game is KNOWN to require or be enhanced by the ECS.
        /// </summary>
        EnabledForRequired = 1 << 1,

        /// <summary>
        /// When both levels of ECS ROMs restriction are set, they will be disabled at all times
        /// UNLESS a game is known to REQUIRE the ECS.
        /// </summary>
        Disabled = EnabledForRequired | EnabledForRequiredAndOptional,

        /// <summary>
        /// The default value for this option.
        /// </summary>
        Default = EnabledForRequired,

        /// <summary>
        /// This mask identifies which ECS status flags are reserved for future use.
        /// </summary>
        ReservedMask = 0xFC,

        /// <summary>
        /// This mask identifies all ECS-related status flags.
        /// </summary>
        AllFlags = 0xFF
    }

    /// <summary>
    /// Extension methods to help work with EcsStatusFlags.
    /// </summary>
    internal static class EcsStatusFlagsHelpers
    {
        private static readonly Dictionary<EcsStatusFlags, string> ModeToStringsTable = new Dictionary<EcsStatusFlags, string>()
        {
            { EcsStatusFlags.None, Resources.Strings.EcsCompatibilityMode_Enabled },
            { EcsStatusFlags.EnabledForRequiredAndOptional, Resources.Strings.EcsCompatibilityMode_Limited },
            { EcsStatusFlags.EnabledForRequired, Resources.Strings.EcsCompatibilityMode_Strict },
            { EcsStatusFlags.Disabled, Resources.Strings.EcsCompatibilityMode_Disabled } // default on new device
        };

        /// <summary>
        /// Produce <see cref="DeviceStatusFlagsLo"/> containing the given <see cref="EcsStatusFlags"/>.
        /// </summary>
        /// <param name="ecsStatus">The flags to convert.</param>
        /// <returns>The given EcsStatusFlags represented as DeviceStatusFlagsLo.</returns>
        internal static DeviceStatusFlagsLo ToDeviceStatusFlagsLo(this EcsStatusFlags ecsStatus)
        {
            var deviceStatusFlags = (DeviceStatusFlagsLo)((ulong)ecsStatus << DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset);
            return deviceStatusFlags;
        }

        /// <summary>
        /// Produce <see cref="DeviceStatusFlagsLo"/> containing updated <see cref="EcsStatusFlags"/> for a given <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> whose updated flags are desired.</param>
        /// <param name="newEcsFlags">New <see cref="EcsStatusFlags"/> to apply to <paramref name="device"/>.</param>
        /// <returns>A new set of <see cref="DeviceStatusFlagsLo"/> with updated EcsStatusFlags for <paramref name="device"/>.</returns>
        internal static DeviceStatusFlagsLo UpdateStatusFlags(this Device device, EcsStatusFlags newEcsFlags)
        {
            var deviceStatusFlags = device.ComposeStatusFlags() & ~DeviceStatusFlagsLo.EcsStatusMask;
            deviceStatusFlags |= newEcsFlags.ToDeviceStatusFlagsLo();
            return deviceStatusFlags;
        }

        /// <summary>
        /// Given <see cref="EcsStatusFlags"/>, produce a string representation for the user interface.
        /// </summary>
        /// <param name="ecsStatus">The flags to convert to a user-readable string.</param>
        /// <returns>The display string.</returns>
        internal static string ToDisplayString(this EcsStatusFlags ecsStatus)
        {
            return ModeToStringsTable[ecsStatus];
        }

        /// <summary>
        /// Given a display string, convert it to valid <see cref="EcsStatusFlags"/> if possible.
        /// </summary>
        /// <param name="displayString">The display string to convert.</param>
        /// <returns>The converted <see cref="EcsStatusFlags"/>.</returns>
        internal static EcsStatusFlags FromDisplayString(string displayString)
        {
            var ecsStatus = ModeToStringsTable.FirstOrDefault(k => k.Value == displayString as string).Key;
            return ecsStatus;
        }
    }
}

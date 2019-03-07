// <copyright file="SaveMenuPositionFlags.cs" company="INTV Funhouse">
// Copyright (c) 2015-2019 All Rights Reserved
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
    /// These flags describe how Locutus will behave with regards to saving the menu position in the face of power or reset situations.
    /// </summary>
    [System.Flags]
    public enum SaveMenuPositionFlags : byte
    {
        /// <summary>
        /// Never save the menu position.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Only retain menu position while console power is on.
        /// </summary>
        DuringSessionOnly = 1 << 0,

        /// <summary>
        /// Reserved. If only this bit is set, behave as DuringSessionOnly.
        /// </summary>
        Reserved = 1 << 1,

        /// <summary>
        /// Always retain menu position.
        /// </summary>
        Always = DuringSessionOnly | Reserved,

        /// <summary>
        /// Pseudonym for the default setting.
        /// </summary>
        Default = DuringSessionOnly,

        /// <summary>
        /// All the possible valid flags.
        /// </summary>
        AllFlags = 3
    }

    /// <summary>
    /// Extension methods to assist with conversion operations.
    /// </summary>
    internal static class SaveMenuPositionFlagsHelpers
    {
        private static readonly Dictionary<SaveMenuPositionFlags, string> ModeToStringsTable = new Dictionary<SaveMenuPositionFlags, string>()
        {
            { SaveMenuPositionFlags.Always, Resources.Strings.SaveMenuPosition_Always },
            { SaveMenuPositionFlags.DuringSessionOnly, Resources.Strings.SaveMenuPosition_SessionOnly },
            { SaveMenuPositionFlags.Reserved, Resources.Strings.SaveMenuPosition_SessionOnly }, // treat as DuringSessionOnly
            { SaveMenuPositionFlags.Never, Resources.Strings.SaveMenuPosition_Never }
        };

        /// <summary>
        /// Places save menu position flags into the more generic <see cref="DeviceStatusFlagsLo"/> bit array.
        /// </summary>
        /// <param name="saveMenuPosition">The <see cref="SaveMenuPositionFlags"/> to convert.</param>
        /// <returns>The flags placed in a <see cref="DeviceStatusFlagsLo"/> bit array.</returns>
        internal static DeviceStatusFlagsLo ToDeviceStatusFlagsLo(this SaveMenuPositionFlags saveMenuPosition)
        {
            var deviceStatusFlags = (DeviceStatusFlagsLo)((ulong)saveMenuPosition << DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset);
            return deviceStatusFlags;
        }

        /// <summary>
        /// Produce <see cref="DeviceStatusFlagsLo"/> containing updated <see cref="SaveMenuPositionFlags"/> for a given <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> whose updated flags are desired.</param>
        /// <param name="newSaveMenuPositionFlags">New <see cref="SaveMenuPositionFlags"/> to apply to <paramref name="device"/>.</param>
        /// <returns>A new set of <see cref="DeviceStatusFlagsLo"/> with updated SaveMenuPositionFlags for <paramref name="device"/>.</returns>
        internal static DeviceStatusFlagsLo UpdateStatusFlags(this Device device, SaveMenuPositionFlags newSaveMenuPositionFlags)
        {
            var deviceStatusFlags = device.ComposeStatusFlagsLo() & ~DeviceStatusFlagsLo.SaveMenuPositionMask;
            deviceStatusFlags |= newSaveMenuPositionFlags.ToDeviceStatusFlagsLo();
            return deviceStatusFlags;
        }

        /// <summary>
        /// Converts <see cref="SaveMenuPositionFlags"/> into a display string.
        /// </summary>
        /// <param name="saveMenuPosition">The flags to convert.</param>
        /// <returns>A display string for the value.</returns>
        internal static string ToDisplayString(this SaveMenuPositionFlags saveMenuPosition)
        {
            return ModeToStringsTable[saveMenuPosition];
        }

        /// <summary>
        /// Converts a string into <see cref="SaveMenuPositionFlags"/> value.
        /// </summary>
        /// <param name="displayString">The string to convert.</param>
        /// <returns>The appropriate <see cref="SaveMenuPositionFlags"/> value.</returns>
        internal static SaveMenuPositionFlags FromDisplayString(string displayString)
        {
            var saveMenuPosition = ModeToStringsTable.FirstOrDefault(k => k.Value == displayString as string).Key;
            return saveMenuPosition;
        }
    }
}

// <copyright file="ShowTitleScreenFlags.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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
    /// Show LTO Flash! title screen configuration option.
    /// </summary>
    [System.Flags]
    public enum ShowTitleScreenFlags : byte
    {
        /// <summary>
        /// Never show the title screen -- immediately show the menu.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Only show the title screen when the system is initially powered up.
        /// </summary>
        OnPowerUp = 1 << 0,

        /// <summary>
        /// Reserved. (Behaves as OnPowerUp.)
        /// </summary>
        Reserved = 1 << 1,

        /// <summary>
        /// Always show the title screen whenever the system is powered on or reset to the menu.
        /// </summary>
        Always = OnPowerUp | Reserved,

        /// <summary>
        /// The default value for this option.
        /// </summary>
        Default = Always,

        /// <summary>
        /// All the valid flags for ShowTitleScreen.
        /// </summary>
        AllFlags = 3
    }

    /// <summary>
    /// Extension methods to assist with conversion operations.
    /// </summary>
    internal static class ShowTitleScreenFlagsHelpers
    {
        private static readonly Dictionary<ShowTitleScreenFlags, string> ModeToStringsTable = new Dictionary<ShowTitleScreenFlags, string>()
        {
            { ShowTitleScreenFlags.Always, Resources.Strings.ShowTitleScreen_Always },
            { ShowTitleScreenFlags.OnPowerUp, Resources.Strings.ShowTitleScreen_OnPowerUp },
            { ShowTitleScreenFlags.Never, Resources.Strings.ShowTitleScreen_Never }
        };

        /// <summary>
        /// Places save menu position flags into the more generic <see cref="DeviceStatusFlagsLo"/> bit array.
        /// </summary>
        /// <param name="showTitleScreen">The <see cref="ShowTitleScreenFlags"/> to convert.</param>
        /// <returns>The flags placed in a <see cref="DeviceStatusFlagsLo"/> bit array.</returns>
        internal static DeviceStatusFlagsLo ToDeviceStatusFlagsLo(this ShowTitleScreenFlags showTitleScreen)
        {
            var deviceStatusFlags = (DeviceStatusFlagsLo)((ulong)showTitleScreen << DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset);
            return deviceStatusFlags;
        }

        /// <summary>
        /// Produce <see cref="DeviceStatusFlagsLo"/> containing updated <see cref="ShowTitleScreenFlags"/> for a given <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> whose updated flags are desired.</param>
        /// <param name="newTitleScreenFlags">New <see cref="ShowTitleScreenFlags"/> to apply to <paramref name="device"/>.</param>
        /// <returns>A new set of <see cref="DeviceStatusFlagsLo"/> with updated ShowTitleScreenFlags for <paramref name="device"/>.</returns>
        internal static DeviceStatusFlagsLo UpdateStatusFlags(this Device device, ShowTitleScreenFlags newTitleScreenFlags)
        {
            var deviceStatusFlags = device.ComposeStatusFlagsLo() & ~DeviceStatusFlagsLo.ShowTitleScreenMask;
            deviceStatusFlags |= newTitleScreenFlags.ToDeviceStatusFlagsLo();
            return deviceStatusFlags;
        }

        /// <summary>
        /// Converts <see cref="ShowTitleScreenFlags"/> into a display string.
        /// </summary>
        /// <param name="showTitleScreen">The <see cref="ShowTitleScreenFlags"/> value to convert.</param>
        /// <returns>A display string for the value.</returns>
        internal static string ToDisplayString(this ShowTitleScreenFlags showTitleScreen)
        {
            return ModeToStringsTable[showTitleScreen];
        }

        /// <summary>
        /// Converts a string into <see cref="ShowTitleScreenFlags"/> value.
        /// </summary>
        /// <param name="displayString">The string to convert.</param>
        /// <returns>The appropriate <see cref="ShowTitleScreenFlags"/> value.</returns>
        internal static ShowTitleScreenFlags FromDisplayString(string displayString)
        {
            var showTitleScreen = ModeToStringsTable.FirstOrDefault(k => k.Value == displayString as string).Key;
            return showTitleScreen;
        }
    }
}

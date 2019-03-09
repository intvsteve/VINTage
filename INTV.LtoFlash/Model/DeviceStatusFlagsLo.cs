// <copyright file="DeviceStatusFlagsLo.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// These flags identify the low 8 bytes (64 bits) of status flags available from a Locutus device.
    /// </summary>
    /// <remarks>Presently, these flags consist of three separate subdivisions: Hardware status (four bytes),
    /// Intellivision II status (one byte), and ECS status (one byte). The remaining two bytes are reserved.</remarks>
    [System.Flags]
    public enum DeviceStatusFlagsLo : ulong
    {
        /// <summary>
        /// No status flags are set.
        /// </summary>
        None = 0,

        #region Hardware Flags

        /// <summary>
        /// This mask identifies all hardware-related status flags.
        /// </summary>
        HardwareStatusFlagsMask = (ulong)HardwareStatusFlags.AllFlags << DeviceStatusFlagsLoHelpers.HardwareFlagsOffset,

        /// <summary>
        /// This mask identifies which hardware status flags are reserved for future use.
        /// </summary>
        HardwareStatusFlagsReservedMask = (ulong)HardwareStatusFlags.ReservedMask << DeviceStatusFlagsLoHelpers.HardwareFlagsOffset,

        /// <summary>
        /// When set, indicates that Locutus is plugged into an Intellivision console that is in the powered on state.
        /// </summary>
        ConsolePowerOn = (ulong)HardwareStatusFlags.ConsolePowerOn,

        #endregion // Hardware Flags

        #region Intellivision II Flags

        /// <summary>
        /// This mask identifies all Intellivision II-related status flags.
        /// </summary>
        IntellivisionIIStatusMask = (ulong)IntellivisionIIStatusFlags.AllFlags << DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset,

        /// <summary>
        /// When set, this flag indicates that Locutus should attempt to patch only ROMs known to have compatibility problems with the Intellivision II.
        /// </summary>
        IntellivisionIIStatusConservative = (ulong)IntellivisionIIStatusFlags.Conservative << DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset,

        /// <summary>
        /// When set, Locutus will always attempt to bypass the Intellivision II lockout check.
        /// </summary>
        IntellivisionIIStatusAggressive = (ulong)IntellivisionIIStatusFlags.Aggressive << DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset,

        /// <summary>
        /// This mask identifies which Intellivision II status flags are reserved for future use.
        /// </summary>
        IntellivisionIIStatusReservedMask = (ulong)IntellivisionIIStatusFlags.ReservedMask << DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset,

        #endregion // Intellivision II Flags

        #region ECS Flags

        /// <summary>
        /// This mask identifies all ECS-related status flags.
        /// </summary>
        EcsStatusMask = (ulong)EcsStatusFlags.AllFlags << DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset,

        /// <summary>
        /// When this flag is set, the ECS ROMs remain enabled for ROMs that are known to either require
        /// the ECS to function, or that may use ECS features if present, e.g. extra sound channels.
        /// </summary>
        EcsStatusEnabledForRequiredAndOptional = (ulong)EcsStatusFlags.EnabledForRequiredAndOptional << DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset,

        /// <summary>
        /// When this flag is set, the ECS ROMs remain enabled only for ROMs that indicate the ECS is required.
        /// </summary>
        EcsStatusEnabledForRequired = (ulong)EcsStatusFlags.EnabledForRequired << DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset,

        /// <summary>
        /// When both levels of ECS ROMs restriction are set, they will be disabled at all times.
        /// </summary>
        EcsStatusDisabled = (ulong)EcsStatusFlags.Disabled << DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset,

        /// <summary>
        /// This mask identifies which ECS status flags are reserved for future use.
        /// </summary>
        EcsStatusReservedMask = (ulong)EcsStatusFlags.ReservedMask << DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset,

        #endregion // ECS Flags

        #region Title Screen Flags

        /// <summary>
        /// This mask identifies the title screen-related status flags.
        /// </summary>
        ShowTitleScreenMask = (ulong)ShowTitleScreenFlags.AllFlags << DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset,

        /// <summary>
        /// When set, title screen shown on initial power up, no other times.
        /// </summary>
        ShowTitleScreenOnPowerUp = (ulong)ShowTitleScreenFlags.OnPowerUp << DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset,

        /// <summary>
        /// When set, title screen is always shown, even when soft reset.
        /// </summary>
        ShowTitleScreenAlways = (ulong)ShowTitleScreenFlags.Always << DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset,

        #endregion // Title Screen Flags

        #region Save Menu Position Flags

        /// <summary>
        /// This mask identifies the save menu position-related status flags.
        /// </summary>
        SaveMenuPositionMask = (ulong)SaveMenuPositionFlags.AllFlags << DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset,

        /// <summary>
        /// When set, menu position is only retained while console power is on.
        /// </summary>
        SaveMenuPositionDuringSessionOnly = (ulong)SaveMenuPositionFlags.DuringSessionOnly << DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset,

        /// <summary>
        /// When set, menu position is always saved.
        /// </summary>
        SaveMenuPositionAlways = (ulong)SaveMenuPositionFlags.Always << DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset,

        #endregion // Save Menu Position Flags

        #region Background GC

        /// <summary>
        /// When set, device executes garbage collection in the background while at the menu.
        /// </summary>
        BackgroundGC = 1ul << DeviceStatusFlagsLoHelpers.BackgroundGCBitsOffset,

        #endregion // Background GC

        #region Keyclicks

        /// <summary>
        /// When set, menu emits keyclicks when navigated.
        /// </summary>
        Keyclicks = 1ul << DeviceStatusFlagsLoHelpers.KeyclicksBitsOffset,

        #endregion // Keyclicks

        #region Configuration Menu on Cartridge

        /// <summary>
        /// When set, the configuration menu on the cartridge can be accessed - which is the default.
        /// </summary>
        EnableCartConfig = 1ul << DeviceStatusFlagsLoHelpers.EnableCartConfigBitsOffset,

        #endregion // Configuration Menu on Cartridge

        #region Zero JLP RAM on Program Launch

        /// <summary>
        /// When set, the RAM for programs using JLP additional RAM is zeroed - which is the default. Otherwise, it is randomized.
        /// </summary>
        ZeroJlpRam = 1ul << DeviceStatusFlagsLoHelpers.ZeroJlpRamBitsOffset,

        #endregion // Zero JLP RAM on Program Launch

        /// <summary>
        /// This mask captures all reserved status bits.
        /// </summary>
        ReservedMask = 0xFFC0000000000000ul | HardwareStatusFlagsReservedMask | IntellivisionIIStatusReservedMask | EcsStatusReservedMask
    }

    /// <summary>
    /// Extension methods to make working with DeviceStatusFlagsLo easier.
    /// </summary>
    internal static class DeviceStatusFlagsLoHelpers
    {
        #region Hardware Status Bits

        /// <summary>
        /// Location in the bit array for hardware-related status bits.
        /// </summary>
        internal const int HardwareFlagsOffset = 0;

        private const int HardwareFlagsCount = 32;

        #endregion // Hardware Status Bits

        #region User-Configurable Flags

        /// <summary>
        /// Mask to retrieve status flags that are configurable, i.e. those which can be set and stored on a Locutus device,
        /// as opposed to "live" hardware status flags that are read-only.
        /// </summary>
        private const ulong ConfigurableFlagsMask = 0xFFFFFFFF00000000ul;

        private const int ConfigurableFlagsOffset = HardwareFlagsOffset + HardwareFlagsCount; // (32)

        #region Intellivision II Bits

        /// <summary>
        /// Location in the bit array where Intellivision II-related flags begin.
        /// </summary>
        internal const int IntellivisionIIStatusBitsOffset = ConfigurableFlagsOffset; // (32)

        private const int IntellivisionIIStatusBitCount = 8;

        #endregion // Intellivision II Bits

        #region ECS Bits

        /// <summary>
        /// Location in the bit array where ECS-related flags begin.
        /// </summary>
        internal const int EcsStatusBitsOffset = IntellivisionIIStatusBitsOffset + IntellivisionIIStatusBitCount; // (40)

        private const int EcsStatusBitCount = 8;

        #endregion // ECS Bits

        #region Title Screen Bits

        /// <summary>
        /// Location in the bit array where title screen-related flags begin.
        /// </summary>
        internal const int ShowTitleScreenBitsOffset = EcsStatusBitsOffset + EcsStatusBitCount; // (48)

        private const int ShowTitleScreenBitCount = 2;

        #endregion // Title Screen Bits

        #region Save Menu Position Bits

        /// <summary>
        /// Location in the bit array where save menu position-related flags begin.
        /// </summary>
        internal const int SaveMenuPositionBitsOffset = ShowTitleScreenBitsOffset + ShowTitleScreenBitCount; // (50)

        private const int SaveMenuPositionBitCount = 2;

        #endregion // Save Menu Position Bits

        #region Background GC Bits

        /// <summary>
        /// Location in the bit array where the background garbage collection bit is set.
        /// </summary>
        internal const int BackgroundGCBitsOffset = SaveMenuPositionBitsOffset + SaveMenuPositionBitCount; // (52)

        private const int BackgroundGCBitCount = 1;

        #endregion // Background GC Bits

        #region Keyclicks Bits

        /// <summary>
        /// Location in the bit array where the keyclicks bit is set.
        /// </summary>
        internal const int KeyclicksBitsOffset = BackgroundGCBitsOffset + BackgroundGCBitCount; // (53)

        private const int KeyclicksBitCount = 1;

        #endregion // Keyclicks Bits

        #region Enable Cartridge Configuration Menu Bits

        /// <summary>
        /// Location in the bit array where the bit to enable or disable on-cartridge configuration menu is set.
        /// </summary>
        internal const int EnableCartConfigBitsOffset = KeyclicksBitsOffset + KeyclicksBitCount; // (54)

        private const int EnableCartConfigBitCount = 1;

        #endregion // Enable Cartridge Configuration Menu Bits

        #region Zero JLP RAM

        /// <summary>
        /// Location in the bit array where the bit to enable or disable on-cartridge JLP RAM randomization / zeroing is set.
        /// </summary>
        internal const int ZeroJlpRamBitsOffset = EnableCartConfigBitsOffset + EnableCartConfigBitCount; // (55)

        private const int ZerJlpRamBitCount = 1;

        #endregion // Zero JLP RAM

        #endregion // User-Configurable Flags

        /// <summary>
        /// Gets the hardware status flags out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the hardware status flags part from.</param>
        /// <returns>The hardware status flags.</returns>
        internal static HardwareStatusFlags ToHardwareStatusFlags(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var hardwareStatusFlags = (HardwareStatusFlags)(deviceStatusLo & DeviceStatusFlagsLo.HardwareStatusFlagsMask);
            return hardwareStatusFlags;
        }

        /// <summary>
        /// Gets the Intellivision II status flags out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the Intellivision II status flags part from.</param>
        /// <returns>The Intellivision II flags.</returns>
        internal static IntellivisionIIStatusFlags ToIntellivisionIICompatibilityFlags(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var intellivisionIIFlags = (IntellivisionIIStatusFlags)((ulong)(deviceStatusLo & DeviceStatusFlagsLo.IntellivisionIIStatusMask) >> IntellivisionIIStatusBitsOffset);

            // For Intellivision II, there are only three valid values. If both 'Conservative' and 'Aggressive' are
            // set, treat it as 'Aggressive'. Or, if the flags are all set, it's an uninitialized, new device
            // and the flags should be treated as the default, which is 'aggressive'.
            if ((intellivisionIIFlags == IntellivisionIIStatusFlags.AllFlags) ||
                (intellivisionIIFlags == (IntellivisionIIStatusFlags.Aggressive | IntellivisionIIStatusFlags.Conservative)))
            {
                intellivisionIIFlags = IntellivisionIIStatusFlags.Default;
            }
            return intellivisionIIFlags & ~IntellivisionIIStatusFlags.ReservedMask;
        }

        /// <summary>
        /// Gets the ECS status flags out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the ECS status flags part from.</param>
        /// <returns>The ECS flags.</returns>
        internal static EcsStatusFlags ToEcsCompatibilityFlags(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var ecsFlags = (EcsStatusFlags)((ulong)(deviceStatusLo & DeviceStatusFlagsLo.EcsStatusMask) >> EcsStatusBitsOffset);

            // If all the flags are set, we must be connected to a new, uninitialized device.
            // Set the flags to the default.
            // (Actually, we can't tell uninitialized device from 'Disabled' mode...)
            if (ecsFlags == EcsStatusFlags.AllFlags)
            {
                ecsFlags = EcsStatusFlags.Default;
            }
            return ecsFlags & ~EcsStatusFlags.ReservedMask;
        }

        /// <summary>
        /// Gets the title screen behavior flags out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the title screen status flags part from.</param>
        /// <returns>The title screen flags.</returns>
        internal static ShowTitleScreenFlags ToShowTitleScreenFlags(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var showTitleScreenFlags = (ShowTitleScreenFlags)((ulong)(deviceStatusLo & DeviceStatusFlagsLo.ShowTitleScreenMask) >> ShowTitleScreenBitsOffset);
            if (showTitleScreenFlags == ShowTitleScreenFlags.Reserved)
            {
                showTitleScreenFlags = ShowTitleScreenFlags.OnPowerUp;
            }
            return showTitleScreenFlags;
        }

        /// <summary>
        /// Gets the save menu position flags out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the save menu position status flags part from.</param>
        /// <returns>The save menu position flags.</returns>
        internal static SaveMenuPositionFlags ToSaveMenuPositionFlags(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var saveMenuPositionFlags = (SaveMenuPositionFlags)((ulong)(deviceStatusLo & DeviceStatusFlagsLo.SaveMenuPositionMask) >> SaveMenuPositionBitsOffset);
            if (saveMenuPositionFlags == SaveMenuPositionFlags.Reserved)
            {
                saveMenuPositionFlags = SaveMenuPositionFlags.DuringSessionOnly;
            }
            return saveMenuPositionFlags;
        }

        /// <summary>
        /// Gets the background garbage collection flag out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the background GC flag from.</param>
        /// <returns>If <c>true</c>, background garbage collection is enabled.</returns>
        internal static bool ToBackgroundGC(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var backgroundGC = deviceStatusLo.HasFlag(DeviceStatusFlagsLo.BackgroundGC);
            return backgroundGC;
        }

        /// <summary>
        /// Gets the keyclicks flag out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the keyclicks flag from.</param>
        /// <returns>If <c>true</c>, keyclicks are enabled on the device.</returns>
        internal static bool ToKeyclicks(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var keyclicks = deviceStatusLo.HasFlag(DeviceStatusFlagsLo.Keyclicks);
            return keyclicks;
        }

        /// <summary>
        /// Gets whether the on-cartridge configuration menu can be accessed out of <see cref="DeviceStatusFlagsLo"/>;.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the onboard configuration menu access from.</param>
        /// <returns>If <c>true</c>, onboard configuration menu can be accessed.</returns>
        internal static bool ToEnableOnboardConfigMenu(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var enableOnboardConfigMenu = deviceStatusLo.HasFlag(DeviceStatusFlagsLo.EnableCartConfig);
            return enableOnboardConfigMenu;
        }

        /// <summary>
        /// Gets whether the on-cartridge JLP RAM is zeroed out or randomized out of <see cref="DeviceStatusFlagsLo"/>;.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the JLP RAM zeroing setting from.</param>
        /// <returns>If <c>true</c>, onboard JLP RAM is initialized to zero on program launch, otherwise it is randomized.</returns>
        internal static bool ToZeroJlpRam(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var zeroJlpRam = deviceStatusLo.HasFlag(DeviceStatusFlagsLo.ZeroJlpRam);
            return zeroJlpRam;
        }

        /// <summary>
        /// Gets the configuration flags out of <see cref="DeviceStatusFlagsLo"/>.
        /// </summary>
        /// <param name="deviceStatusLo">The device status flags to get the configuration flags part from.</param>
        /// <returns>The configuration flags.</returns>
        internal static uint ToConfigurationFlags(this DeviceStatusFlagsLo deviceStatusLo)
        {
            var flags = (ulong)(deviceStatusLo | DeviceStatusFlagsLo.ReservedMask) & ConfigurableFlagsMask;
            return (uint)(((ulong)flags >> ConfigurableFlagsOffset) & 0x00000000FFFFFFFF);
        }

        /// <summary>
        /// Composes a set of <see cref="DeviceStatusFlagsLo"/> from a Locutus <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> from which a set of <see cref="DeviceStatusFlagsLo"/> is to be composed.</param>
        /// <returns>Status flags properly composed into <see cref="DeviceStatusFlagsLo"/>.</returns>
        internal static DeviceStatusFlagsLo ComposeStatusFlagsLo(this Device device)
        {
            var deviceStatusFlagsLo = (device.IntvIICompatibility | IntellivisionIIStatusFlags.ReservedMask).ToDeviceStatusFlagsLo() |
                                      (device.EcsCompatibility | EcsStatusFlags.ReservedMask).ToDeviceStatusFlagsLo() |
                                       device.ShowTitleScreen.ToDeviceStatusFlagsLo() | device.SaveMenuPosition.ToDeviceStatusFlagsLo() |
                                      (device.HardwareStatus | HardwareStatusFlags.ReservedMask).ToDeviceStatusFlagsLo();
            if (device.BackgroundGC)
            {
                deviceStatusFlagsLo |= DeviceStatusFlagsLo.BackgroundGC;
            }
            if (device.Keyclicks)
            {
                deviceStatusFlagsLo |= DeviceStatusFlagsLo.Keyclicks;
            }
            return deviceStatusFlagsLo;
        }
    }
}

// <copyright file="DeviceStatusFlagsHi.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// These flags identify the high 8 bytes of status flags available from a Locutus device.
    /// </summary>
    [System.Flags]
    public enum DeviceStatusFlagsHi : ulong
    {
        /// <summary>
        /// No status flags are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// This flag is used internally by Locutus to indicate that the file system menu position data should be
        /// considered invalid. The UI should not manipulate this flag directly, and it should always be set.
        /// Because of this, it is still considered part of <see cref="ReservedMask"/>.
        /// </summary>
        ResetMenuHistory = 1ul << 62,

        /// <summary>
        /// Indicates flags have been initialized. Set to zero to force 'factory reset'.
        /// </summary>
        FlagsHaveBeenSet = 1ul << 63,

        /// <summary>
        /// All bits are reserved.
        /// </summary>
        ReservedMask = 0x7FFFFFFFFFFFFFFF,

        /// <summary>
        /// Default, expected flags for DeviceStatusFlagsHi. If FlagsHaveBeenSet is ever cleared and sent
        /// to a Locutus device, the device will reset all configuration flags to factory defaults.
        /// </summary>
        Default = ReservedMask | FlagsHaveBeenSet
    }

    /// <summary>
    /// Extension methods to ease working with DeviceStatusFlagsHi.
    /// </summary>
    internal static class DeviceStatusFlagsHiHelpers
    {
        /// <summary>
        /// The firmware-version-specific configurable features.
        /// </summary>
        /// <remarks>Only configurable features introduced after the initial product release are included in this dictionary.</remarks>
        private static readonly Dictionary<DeviceStatusFlagsHi, int> VersionSpecificConfigurableFeatures = new Dictionary<DeviceStatusFlagsHi, int>()
        {
        };

        /// <summary>
        /// Gets the minimum required firmware version for the given configurable feature.
        /// </summary>
        /// <param name="feature">The feature whose minimum required firmware version is desired.</param>
        /// <returns>The minimum firmware version required for the feature; or <c>0</c> if the feature is available in all firmware versions.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="feature"/> specified more than one configurable feature, or hardware status.</exception>
        internal static int GetMinimumRequiredFirmareVersionForFeature(this DeviceStatusFlagsHi feature)
        {
            ValidateFeatureBits(feature);
            int requiredFirmwareVersion;
            if (!VersionSpecificConfigurableFeatures.TryGetValue(feature, out requiredFirmwareVersion))
            {
                requiredFirmwareVersion = 0;
            }
            return requiredFirmwareVersion;
        }

        /// <summary>
        /// Determines if the given configurable feature is available in the specified firmware revision.
        /// </summary>
        /// <param name="feature">The feature whose availability is desired.</param>
        /// <param name="currentFirmwareVersion">Current firmware version.</param>
        /// <returns><c>true</c> if the configurable feature is available for the specified firmware version; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="feature"/> specified more than one configurable feature, or hardware status.</exception>
        internal static bool IsConfigurableFeatureAvailable(this DeviceStatusFlagsHi feature, int currentFirmwareVersion)
        {
            ValidateFeatureBits(feature);
            var featureAvailable = (currentFirmwareVersion > 0) && (VersionSpecificConfigurableFeatures.Count > 0) && (currentFirmwareVersion >= feature.GetMinimumRequiredFirmareVersionForFeature());
            return featureAvailable;
        }

        /// <summary>
        /// Gets the low 32 bits from the status.
        /// </summary>
        /// <param name="deviceStatusHi">The device status flags to get the low part from.</param>
        /// <returns>The lower 32 bits of the status flags.</returns>
        internal static uint GetLowBits(this DeviceStatusFlagsHi deviceStatusHi)
        {
            return (uint)((ulong)deviceStatusHi & 0x00000000FFFFFFFF);
        }

        /// <summary>
        /// Gets the high 32 bits of the status.
        /// </summary>
        /// <param name="deviceStatusHi">The device status flags to get the high part from.</param>
        /// <returns>The upper 32 bits of the status flags.</returns>
        internal static uint GetHighBits(this DeviceStatusFlagsHi deviceStatusHi)
        {
            return (uint)(((ulong)deviceStatusHi >> 32) & 0x00000000FFFFFFFF);
        }

        private static void ValidateFeatureBits(DeviceStatusFlagsHi feature)
        {
            var numFeatures = 0;
            if (feature != DeviceStatusFlagsHi.None)
            {
                if ((feature & DeviceStatusFlagsHi.ResetMenuHistory) != DeviceStatusFlagsHi.None)
                {
                    numFeatures += 2; // Totally bogus - should not have this flag!
                }
                if ((feature & DeviceStatusFlagsHi.FlagsHaveBeenSet) != DeviceStatusFlagsHi.None)
                {
                    numFeatures += 2; // Totally bogus - should not have this flag!
                }
            }
            if (numFeatures > 1)
            {
                throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}

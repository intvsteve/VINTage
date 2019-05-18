// <copyright file="DeviceStatusFlags.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Structure to store device status flags as a single entity.
    /// </summary>
    public struct DeviceStatusFlags
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance with only <see cref="Lo"/> flags set.
        /// </summary>
        /// <param name="lo">The <see cref="DeviceStatusFlagsLo"/> to store.</param>
        public DeviceStatusFlags(DeviceStatusFlagsLo lo)
            : this(lo, DeviceStatusFlagsHi.None)
        {
        }

        /// <summary>
        /// Initialize a new instance with only <see cref="Hi"/> flags set.
        /// </summary>
        /// <param name="hi">The <see cref="DeviceStatusFlagsHi"/> to store.</param>
        public DeviceStatusFlags(DeviceStatusFlagsHi hi)
            : this(DeviceStatusFlagsLo.None, hi)
        {
        }

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="lo">The <see cref="DeviceStatusFlagsLo"/> to store.</param>
        /// <param name="hi">The <see cref="DeviceStatusFlagsHi"/> to store.</param>
        public DeviceStatusFlags(DeviceStatusFlagsLo lo, DeviceStatusFlagsHi hi)
            : this()
        {
            _lo = lo;
            _hi = hi;
        }

        #endregion // Constructors

        #region Defined flag values

        /// <summary>Empty status flags.</summary>
        public static readonly DeviceStatusFlags None = new DeviceStatusFlags();

        /// <summary>Hardware status flag for console power on.</summary>
        public static readonly DeviceStatusFlags ConsolePowerOn = new DeviceStatusFlags(HardwareStatusFlags.ConsolePowerOn.ToDeviceStatusFlagsLo());

        /// <summary>Hardware status flag for new error log available.</summary>
        public static readonly DeviceStatusFlags NewErrorLogAvailable = new DeviceStatusFlags(HardwareStatusFlags.NewErrorLogAvailable.ToDeviceStatusFlagsLo());

        /// <summary>Hardware status flag for new crash log available.</summary>
        public static readonly DeviceStatusFlags NewCrashLogAvailable = new DeviceStatusFlags(HardwareStatusFlags.NewCrashLogAvailable.ToDeviceStatusFlagsLo());

        /// <summary>Hardware status flag indicating file system updates are allowed while console power is on.</summary>
        public static readonly DeviceStatusFlags GrabbedForMenuUpdate = new DeviceStatusFlags(HardwareStatusFlags.GrabbedForMenuUpdate.ToDeviceStatusFlagsLo());

        /// <summary>Intellivision II compatibility flag indicating that Locutus should attempt to patch only ROMs known to have compatibility problems with the Intellivision II.</summary>
        public static readonly DeviceStatusFlags IntellivisionIIStatusConservative = new DeviceStatusFlags(DeviceStatusFlagsLo.IntellivisionIIStatusConservative);

        /// <summary>Intellivision II compatibility flag indicating that Locutus will always attempt to bypass the Intellivision II lockout check.</summary>
        public static readonly DeviceStatusFlags IntellivisionIIStatusAggressive = new DeviceStatusFlags(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive);

        /// <summary>ECS compatibility flag indicating that Locutus will enable the ECS ROMs only for program ROMs that indicate the ECS is required or optional (enhances behavior of program).</summary>
        public static readonly DeviceStatusFlags EcsStatusEnabledForRequiredAndOptional = new DeviceStatusFlags(DeviceStatusFlagsLo.EcsStatusEnabledForRequiredAndOptional);

        /// <summary>ECS compatibility flag indicating that Locutus will enable the ECS ROMs only for program ROMs that indicate the ECS is required.</summary>
        public static readonly DeviceStatusFlags EcsStatusEnabledForRequired = new DeviceStatusFlags(DeviceStatusFlagsLo.EcsStatusEnabledForRequired);

        /// <summary>ECS compatibility flag indicating that Locutus will always disable the ECS ROMs.</summary>
        public static readonly DeviceStatusFlags EcsStatusDisabled = new DeviceStatusFlags(DeviceStatusFlagsLo.EcsStatusDisabled);

        #endregion // Defined flag values

        /// <summary>
        /// Gets the first 64 bits of device status flags.
        /// </summary>
        public DeviceStatusFlagsLo Lo
        {
            get { return _lo; }
        }
        private readonly DeviceStatusFlagsLo _lo;

        /// <summary>
        /// Gets the second 64 bits of device status flags.
        /// </summary>
        public DeviceStatusFlagsHi Hi
        {
            get { return _hi; }
        }
        private readonly DeviceStatusFlagsHi _hi;

        #region Bitwise AND operators

        /// <summary>
        /// Computes a bitwise AND of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by ANDing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator &(DeviceStatusFlags lhs, DeviceStatusFlags rhs)
        {
            return new DeviceStatusFlags(lhs.Lo & rhs.Lo, lhs.Hi & rhs.Hi);
        }

        /// <summary>
        /// Computes a bitwise AND of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose Lo flags computed by ANDing the corresponding parts of the two given flags together.
        /// The Hi bits will be <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static DeviceStatusFlags operator &(DeviceStatusFlags lhs, DeviceStatusFlagsLo rhs)
        {
            return new DeviceStatusFlags(lhs.Lo & rhs);
        }

        /// <summary>
        /// Computes a bitwise AND of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose Hi flags computed by ANDing the corresponding parts of the two given flags together.
        /// The Lo bits will be <see cref="DeviceStatusFlagsLo.None"/>.</returns>
        public static DeviceStatusFlags operator &(DeviceStatusFlags lhs, DeviceStatusFlagsHi rhs)
        {
            return new DeviceStatusFlags(lhs.Hi & rhs);
        }

        /// <summary>
        /// Computes a bitwise AND of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose Lo flags computed by ANDing the corresponding parts of the two given flags together.
        /// The Hi bits will be <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static DeviceStatusFlags operator &(DeviceStatusFlagsLo lhs, DeviceStatusFlags rhs)
        {
            return rhs & lhs;
        }

        /// <summary>
        /// Computes a bitwise AND of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose Lo flags computed by ANDing the corresponding parts of the two given flags together.
        /// The Hi bits will be <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static DeviceStatusFlags operator &(DeviceStatusFlagsHi lhs, DeviceStatusFlags rhs)
        {
            return rhs & lhs;
        }

        #endregion //  Bitwise AND operators

        #region Bitwise OR operators

        /// <summary>
        /// Computes a bitwise OR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by ORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator |(DeviceStatusFlags lhs, DeviceStatusFlags rhs)
        {
            return new DeviceStatusFlags(lhs.Lo | rhs.Lo, lhs.Hi | rhs.Hi);
        }

        /// <summary>
        /// Computes a bitwise OR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by ORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator |(DeviceStatusFlags lhs, DeviceStatusFlagsLo rhs)
        {
            return new DeviceStatusFlags(lhs.Lo | rhs, lhs.Hi);
        }

        /// <summary>
        /// Computes a bitwise OR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by ORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator |(DeviceStatusFlags lhs, DeviceStatusFlagsHi rhs)
        {
            return new DeviceStatusFlags(lhs.Lo, lhs.Hi | rhs);
        }

        /// <summary>
        /// Computes a bitwise OR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by ORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator |(DeviceStatusFlagsLo lhs, DeviceStatusFlags rhs)
        {
            return rhs | lhs;
        }

        /// <summary>
        /// Computes a bitwise OR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by ORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator |(DeviceStatusFlagsHi lhs, DeviceStatusFlags rhs)
        {
            return rhs | lhs;
        }

        #endregion // Bitwise OR operators

        #region Bitwise XOR operators

        /// <summary>
        /// Computes a bitwise XOR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by XORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator ^(DeviceStatusFlags lhs, DeviceStatusFlags rhs)
        {
            return new DeviceStatusFlags(lhs.Lo ^ rhs.Lo, lhs.Hi ^ rhs.Hi);
        }

        /// <summary>
        /// Computes a bitwise XOR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by XORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator ^(DeviceStatusFlags lhs, DeviceStatusFlagsLo rhs)
        {
            return new DeviceStatusFlags(lhs.Lo ^ rhs, lhs.Hi ^ DeviceStatusFlagsHi.None);
        }

        /// <summary>
        /// Computes a bitwise XOR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by XORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator ^(DeviceStatusFlags lhs, DeviceStatusFlagsHi rhs)
        {
            return new DeviceStatusFlags(lhs.Lo ^ DeviceStatusFlagsLo.None, lhs.Hi ^ rhs);
        }

        /// <summary>
        /// Computes a bitwise XOR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by XORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator ^(DeviceStatusFlagsLo lhs, DeviceStatusFlags rhs)
        {
            return rhs ^ lhs;
        }

        /// <summary>
        /// Computes a bitwise XOR of the flags in the two instances.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns>A <see cref="DeviceStatusFlags"/> whose value is computed by XORing the corresponding parts of the two given flags together.</returns>
        public static DeviceStatusFlags operator ^(DeviceStatusFlagsHi lhs, DeviceStatusFlags rhs)
        {
            return rhs ^ lhs;
        }

        #endregion //  Bitwise XOR operators

        #region Equality operators

        /// <summary>
        /// Equality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal.</returns>
        public static bool operator ==(DeviceStatusFlags lhs, DeviceStatusFlags rhs)
        {
            return (lhs.Lo == rhs.Lo) && (lhs.Hi == rhs.Hi);
        }

        /// <summary>
        /// Equality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Hi"/> is <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static bool operator ==(DeviceStatusFlags lhs, DeviceStatusFlagsLo rhs)
        {
            return (lhs.Lo == rhs) && (lhs.Hi == DeviceStatusFlagsHi.None);
        }

        /// <summary>
        /// Equality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Hi"/> is <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static bool operator ==(DeviceStatusFlagsLo lhs, DeviceStatusFlags rhs)
        {
            return rhs == lhs;
        }

        /// <summary>
        /// Equality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Lo"/> is <see cref="DeviceStatusFlagsLo.None"/>.</returns>
        public static bool operator ==(DeviceStatusFlags lhs, DeviceStatusFlagsHi rhs)
        {
            return (lhs.Lo == DeviceStatusFlagsLo.None) && (lhs.Hi == rhs);
        }

        /// <summary>
        /// Equality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Lo"/> is <see cref="DeviceStatusFlagsLo.None"/>.</returns>
        public static bool operator ==(DeviceStatusFlagsHi lhs, DeviceStatusFlags rhs)
        {
            return rhs == lhs;
        }

        #endregion // Equality operators

        #region Inequality operators

        /// <summary>
        /// Inequality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal.</returns>
        public static bool operator !=(DeviceStatusFlags lhs, DeviceStatusFlags rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Inequality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Hi"/> is <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static bool operator !=(DeviceStatusFlags lhs, DeviceStatusFlagsLo rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Inequality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Hi"/> is <see cref="DeviceStatusFlagsHi.None"/>.</returns>
        public static bool operator !=(DeviceStatusFlagsLo lhs, DeviceStatusFlags rhs)
        {
            return !(rhs == lhs);
        }

        /// <summary>
        /// Inequality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Lo"/> is <see cref="DeviceStatusFlagsLo.None"/>.</returns>
        public static bool operator !=(DeviceStatusFlags lhs, DeviceStatusFlagsHi rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Inequality operator for <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="lhs">Left side value provided to operator.</param>
        /// <param name="rhs">Right side value provided to operator.</param>
        /// <returns><c>true</c> if all flags in both instances are equal. Note that this implies that <see cref="DeviceStatusFlags.Lo"/> is <see cref="DeviceStatusFlagsLo.None"/>.</returns>
        public static bool operator !=(DeviceStatusFlagsHi lhs, DeviceStatusFlags rhs)
        {
            return !(rhs == lhs);
        }

        #endregion // Inequality operators

        #region Bitwise Complement operator

        /// <summary>
        /// Computes the bitwise complement of the flags.
        /// </summary>
        /// <param name="flags">The flags to compute the complement of.</param>
        /// <returns>The bitwise complement of the flags.</returns>
        public static DeviceStatusFlags operator ~(DeviceStatusFlags flags)
        {
            return new DeviceStatusFlags(~flags.Lo, ~flags.Hi);
        }

        #endregion // Bitwise Complement operator

        #region Parsing

        /// <summary>
        /// Parses the given string to produce a new instance of <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>A new instance of <see cref="DeviceStatusFlags"/> containing the flags specified by <paramref name="value"/>.</returns>
        /// <remarks>The string must be of the format: Lo: loFlag0, loFlag1, ... loFlagN; Hi: hiFlag0, hiFlag1, ... hiFlagM.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty, contains only whitespace, or either portion (high or low)
        /// is not present or is empty or whitespace. Also thrown if the portions of the string are not prefixed with 'Lo:' and 'Hi:'.</exception>
        public static DeviceStatusFlags Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            var allParts = value.Split(new[] { ';' });
            ValidateStringPartsForParse(allParts, validatePartsType: false);

            var firstParts = allParts[0].Split(new[] { ':' });
            var firstPartType = ValidateStringPartsForParse(firstParts, validatePartsType: true);

            var secondParts = allParts[1].Split(new[] { ':' });
            var secondPartType = ValidateStringPartsForParse(secondParts, validatePartsType: true);

            if (firstPartType == secondPartType)
            {
                throw new ArgumentException();
            }

            var lowFlagsString = firstPartType == FlagsStringPart.Lo ? firstParts[1] : secondParts[1];
            var lowFlags = (DeviceStatusFlagsLo)Enum.Parse(typeof(DeviceStatusFlagsLo), lowFlagsString.Trim());

            var highFlagsString = firstPartType == FlagsStringPart.Hi ? firstParts[1] : secondParts[1];
            var highFlags = (DeviceStatusFlagsHi)Enum.Parse(typeof(DeviceStatusFlagsHi), highFlagsString.Trim());

            var flags = new DeviceStatusFlags(lowFlags, highFlags);
            return flags;
        }

        /// <summary>
        /// Parses the given string to produce a new instance of <see cref="DeviceStatusFlags"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="flags">The <see cref="DeviceStatusFlags"/> containing the flags specified by <paramref name="value"/>,
        /// or <see cref="DeviceStatusFlags.None"/> if <paramref name="value"/> is not valid.</param>
        /// <returns><c>true</c> if <paramref name="value"/> was successfully parsed.</returns>
        public static bool TryParse(string value, out DeviceStatusFlags flags)
        {
            var success = true;
            try
            {
                flags = Parse(value);
            }
            catch (Exception)
            {
                flags = DeviceStatusFlags.None;
                success = false;
            }
            return success;
        }

        #endregion // Parsing

        #region HasFlag

        /// <summary>
        /// Determines whether one or more bit fields are set in the current instance.
        /// </summary>
        /// <param name="flags">Flags to check.</param>
        /// <returns><c>true</c> if the bit field or bit fields that are set in <paramref name="flags"/> are also set in the current instance; otherwise, <c>false</c>.</returns>
        public bool HasFlag(DeviceStatusFlags flags)
        {
            return HasFlag(flags.Lo) && HasFlag(flags.Hi);
        }

        /// <summary>
        /// Determines whether one or more bit fields are set in the current instance.
        /// </summary>
        /// <param name="loFlags">Flags to check.</param>
        /// <returns><c>true</c> if the bit field or bit fields that are set in <paramref name="loFlags"/> are also set in the current instance; otherwise, <c>false</c>.</returns>
        public bool HasFlag(DeviceStatusFlagsLo loFlags)
        {
            return Lo.HasFlag(loFlags);
        }

        /// <summary>
        /// Determines whether one or more bit fields are set in the current instance.
        /// </summary>
        /// <param name="hiFlags">Flags to check.</param>
        /// <returns><c>true</c> if the bit field or bit fields that are set in <paramref name="hiFlags"/> are also set in the current instance; otherwise, <c>false</c>.</returns>
        public bool HasFlag(DeviceStatusFlagsHi hiFlags)
        {
            return Hi.HasFlag(hiFlags);
        }

        #endregion // HasFlag

        /// <summary>
        /// Gets the minimum required firmware version for the given configurable feature.
        /// </summary>
        /// <returns>The minimum firmware version required for the feature; or <c>0</c> if the feature is available in all firmware versions.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="feature"/> specified more than one configurable feature, or hardware status.</exception>
        public int GetMinimumRequiredFirmareVersionForFeature()
        {
            if ((Lo != DeviceStatusFlagsLo.None) && (Hi != DeviceStatusFlagsHi.None))
            {
                throw new ArgumentOutOfRangeException();
            }
            var minimumRequireFirmwareVersion = 0;
            if (Lo != DeviceStatusFlagsLo.None)
            {
                minimumRequireFirmwareVersion = Lo.GetMinimumRequiredFirmareVersionForFeature();
            }
            else if (Hi != DeviceStatusFlagsHi.None)
            {
                minimumRequireFirmwareVersion = Hi.GetMinimumRequiredFirmareVersionForFeature();
            }
            return minimumRequireFirmwareVersion;
        }

        /// <summary>
        /// Determines if the given configurable feature is available in the specified firmware revision.
        /// </summary>
        /// <param name="currentFirmwareVersion">Current firmware version.</param>
        /// <returns><c>true</c> if the configurable feature is available for the specified firmware version; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="feature"/> has multiple configurable features specified.</exception>
        public bool IsConfigurableFeatureAvailable(int currentFirmwareVersion)
        {
            if ((Lo != DeviceStatusFlagsLo.None) && (Hi != DeviceStatusFlagsHi.None))
            {
                throw new ArgumentOutOfRangeException();
            }
            var featureAvailable = currentFirmwareVersion > 0;
            if (Lo != DeviceStatusFlagsLo.None)
            {
                featureAvailable = Lo.IsConfigurableFeatureAvailable(currentFirmwareVersion);
            }
            else if (Hi != DeviceStatusFlagsHi.None)
            {
                featureAvailable = Hi.IsConfigurableFeatureAvailable(currentFirmwareVersion);
            }
            return featureAvailable;
        }

        #region object overrides

        /// <inheritdoc />
        public override string ToString()
        {
            var flagsString = "Lo: " + Lo.ToString() + "; Hi: " + Hi.ToString();
            return flagsString;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var equals = obj is DeviceStatusFlags;
            if (equals)
            {
                var other = (DeviceStatusFlags)obj;
                equals = (Lo == other.Lo) && (Hi == other.Hi);
            }
            return equals;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return CombineHashCodes(Lo.GetHashCode(), Hi.GetHashCode());
        }

        #endregion // object overrides

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private static FlagsStringPart ValidateStringPartsForParse(string[] parts, bool validatePartsType)
        {
            var partsType = FlagsStringPart.None;
            if (parts.Length != 2)
            {
                throw new ArgumentException();
            }

            if (validatePartsType)
            {
                var partNames = new List<string>() { "LO", "HI" };
                var partIndex = partNames.IndexOf(parts[0].Trim().ToUpperInvariant());
                if (partIndex < 0)
                {
                    throw new ArgumentException();
                }
                else
                {
                    partsType = (FlagsStringPart)(partIndex + 1);
                }
            }

            return partsType;
        }

        private enum FlagsStringPart
        {
            /// <summary>String portion is invalid.</summary>
            None,

            /// <summary>String portion describes <see cref="DeviceStatusFlagsLo"/>.</summary>
            Lo,

            /// <summary>String portion describes <see cref="DeviceStatusFlagsHi"/>.</summary>
            Hi
        }
    }
}

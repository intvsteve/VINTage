// <copyright file="EcsFeatures.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Flags that describe ECS features used by an Intellivision program.
    /// </summary>
    [System.Flags]
    public enum EcsFeatures : uint
    {
        /// <summary>
        /// Program is incompatible with the ECS.
        /// </summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>
        /// Program does not require any ECS features, but will run with the ECS installed.
        /// </summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>
        /// Program has enhanced features when ECS is present,
        /// but will function without it.
        /// </summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>
        /// Program requires the ECS to be installed in order to run.
        /// </summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>
        /// Program uses the ECS's synthesizer keyboard add-on.
        /// </summary>
        Synthesizer = 1 << 2,

        /// <summary>
        /// Program supports or requires tape drive connected to the ECS.
        /// </summary>
        Tape = 1 << 3,

        /// <summary>
        /// Program supports using a printer attached to the ECS.
        /// </summary>
        Printer = 1 << 4,

        /// <summary>
        /// Program is enhanced if serial port is connected.
        /// </summary>
        SerialPortEnhanced = 1 << EcsFeaturesHelpers.SerialPortOffset, // 5

        /// <summary>
        /// Program requires using ECS's serial port.
        /// </summary>
        SerialPortRequired = 1 << (EcsFeaturesHelpers.SerialPortOffset + 1), // 6
    }

    /// <summary>
    /// Extension methods for EcsFeatures.
    /// </summary>
    public static class EcsFeaturesHelpers
    {
        /// <summary>
        /// The default compatibility mode for the ECS.
        /// </summary>
        public const EcsFeatures Default = EcsFeatures.Tolerates;

        /// <summary>
        /// The bit offset to apply to get the serial port compatibility flags.
        /// </summary>
        public const int SerialPortOffset = 5;

        /// <summary>
        /// Bit mask to apply to get the serial port compatibility flags.
        /// </summary>
        public const EcsFeatures SerialPortMask = (EcsFeatures)(FeatureCompatibilityHelpers.CompatibilityMask << SerialPortOffset);

        /// <summary>
        /// Mask to use to retrieve basic feature mask from larger flag sets.
        /// </summary>
        public const uint FeaturesMask = 0x7F;

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const EcsFeatures ValidFeaturesMask = (EcsFeatures)FeaturesMask;

        /// <summary>
        /// Converts EcsFeatures to LuigiFeatureFlags.
        /// </summary>
        /// <param name="features">The ECS features to convert.</param>
        /// <returns>The ECS feature flags represented as LuigiFeatureFlags.</returns>
        public static LuigiFeatureFlags ToLuigiFeatureFlags(this EcsFeatures features)
        {
            return ((FeatureCompatibility)features & FeatureCompatibilityHelpers.ValidFeaturesMask).ToLuigiFeatureFlags(FeatureCategory.Ecs);
        }
    }
}

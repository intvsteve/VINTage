// <copyright file="KeyboardComponentFeatures.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
    /// Flags that describe Keyboard Component features used by an Intellivision program.
    /// </summary>
    [System.Flags]
    public enum KeyboardComponentFeatures : uint
    {
        /// <summary>
        /// Program is incompatible with the Keyboard Component.
        /// </summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>
        /// Program does not require any Keyboard Component features.
        /// </summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>
        /// Program has enhanced features when Keyboard Component is present,
        /// but will function without it.
        /// </summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>
        /// Program requires the Keyboard Component in order to run.
        /// </summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>
        /// Program optionally makes use of the integrated cassette.
        /// </summary>
        TapeOptional = 1 << KeyboardComponentFeaturesHelpers.TapeOffset, // 2

        /// <summary>
        /// Program was delivered on cassette, and / or must access the 1149's tape unit in order to run.
        /// </summary>
        TapeRequired = 1 << (KeyboardComponentFeaturesHelpers.TapeOffset + 1), // 3

        /// <summary>
        /// Program uses the 1149's microphone.
        /// </summary>
        Microphone = 1 << KeyboardComponentFeaturesHelpers.MicrophoneOffset, // 4

        /// <summary>
        /// Program is cannot run if the Microsoft Basic cartridge is installed.
        /// </summary>
        BasicIncompatible = 1 << KeyboardComponentFeaturesHelpers.BasicOffset, // 5

        /// <summary>
        /// Program is unaffected by the presence of the Microsoft Basic cartridge.
        /// </summary>
        BasicTolerated = 1 << (KeyboardComponentFeaturesHelpers.BasicOffset + 1), // 6

        /// <summary>
        /// Program requires the Microsoft Basic cartridge in order to run.
        /// </summary>
        BasicRequired = 1 << (KeyboardComponentFeaturesHelpers.BasicOffset + 2), // 7

        /// <summary>
        /// Program uses or requires a printer attached to the 1149.
        /// </summary>
        Printer = 1 << KeyboardComponentFeaturesHelpers.PrinterOffset, // 8
    }

    /// <summary>
    /// Extension methods for KeyboardComponentFeatures.
    /// </summary>
    public static class KeyboardComponentFeaturesHelpers
    {
        /// <summary>
        /// The default compatibility mode for the Keyboard Component.
        /// </summary>
        public const KeyboardComponentFeatures Default = KeyboardComponentFeatures.Tolerates;

        /// <summary>
        /// The bit offset to apply to get the cassette tape compatibility flags.
        /// </summary>
        public const int TapeOffset = 2;

        /// <summary>
        /// The number of bits reserved for the tape usage feature.
        /// </summary>
        public const int TapeFeatureBitCount = 2;

        /// <summary>
        /// The bit offset for the microphone feature.
        /// </summary>
        public const int MicrophoneOffset = TapeOffset + TapeFeatureBitCount;

        /// <summary>
        /// Mask for extracting cassette compatibility.
        /// </summary>
        public const KeyboardComponentFeatures TapeMask = KeyboardComponentFeatures.TapeOptional | KeyboardComponentFeatures.TapeRequired;

        /// <summary>
        /// The bit offset to apply to get the BASIC cartridge compatibility flags.
        /// </summary>
        public const int BasicOffset = MicrophoneOffset + 1;

        /// <summary>
        /// The number of bits reserved for the MS-Basic feature.
        /// </summary>
        public const int BasicFeatureBitCount = 3;

        /// <summary>
        /// The bit offset to apply to get the printer usage feature.
        /// </summary>
        public const int PrinterOffset = BasicOffset + BasicFeatureBitCount;

        /// <summary>
        /// Mask to use to retrieve basic feature mask from larger flag sets.
        /// </summary>
        public const uint FeaturesMask = 0x1FF;

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const KeyboardComponentFeatures ValidFeaturesMask = (KeyboardComponentFeatures)FeaturesMask;

        /// <summary>
        /// Mask for extracting Microsoft Basic cartridge compatibility.
        /// </summary>
        public const KeyboardComponentFeatures BasicMask = KeyboardComponentFeatures.BasicIncompatible | KeyboardComponentFeatures.BasicTolerated | KeyboardComponentFeatures.BasicRequired;

        /// <summary>
        /// Converts KeyboardComponent features to LuigiFeatureFlags.
        /// </summary>
        /// <param name="features">The features to convert.</param>
        /// <returns>The compatibility represented as appropriate LuigiFeatureFlags.</returns>
        /// <remarks>The additional aspects described by <see cref="KeyboardComponentFeatures"/> are not retained in
        /// <see cref="LuigiFeatureFlags"/>, only basic compatibility.</remarks>
        public static LuigiFeatureFlags ToLuigiFeatureFlags(this KeyboardComponentFeatures features)
        {
            var compatibilty = (FeatureCompatibility)features & FeatureCompatibilityHelpers.ValidFeaturesMask;
            var luigiFeatureFlags = compatibilty.ToLuigiFeatureFlags(FeatureCategory.KeyboardComponent);
            return luigiFeatureFlags;
        }
    }
}

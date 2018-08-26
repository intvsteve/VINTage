// <copyright file="LtoFlashFeatures.cs" company="INTV Funhouse">
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
    /// Flags that describe features provided by the LTO Flash! cartridge and which are used by an Intellivision program.
    /// </summary>
    [System.Flags]
    public enum LtoFlashFeatures : uint
    {
        /// <summary>Program is incompatible with LTO Flash! technology.</summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>Program uses no LTO Flash!-specific features.</summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>Program uses LTO Flash!-specific acceleration features if present.</summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>Program requires LTO Flash!-specific acceleration features.</summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>Program uses JLP save-data feature.</summary>
        SaveDataOptional = 1 << LtoFlashFeaturesHelpers.SaveDataOffset, // 2

        /// <summary>Program uses JLP save-data feature.</summary>
        SaveDataRequired = 1 << (LtoFlashFeaturesHelpers.SaveDataOffset + 1), // 3

        /// <summary>Program uses bank switching to access additional device memory.</summary>
        Bankswitching = 1 << LtoFlashFeaturesHelpers.BankswitchingOffset, // 4

        /// <summary>Program uses additional 16-bit RAM on the device.</summary>
        SixteenBitRAM = 1 << LtoFlashFeaturesHelpers.SixteenBitRAMOffset, // 5

        /// <summary>Program uses LTO Flash!'s serial port capabilities.</summary>
        UsbPortEnhanced = 1 << LtoFlashFeaturesHelpers.UsbPortOffset, // 6

        /// <summary>Program requires LTO Flash!'s serial port capabilities.</summary>
        UsbPortRequired = 1 << (LtoFlashFeaturesHelpers.UsbPortOffset + 1), // 7

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit0 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset, // 8

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit1 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 1,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit2 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 2,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit3 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 3,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit4 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 4,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit5 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 5,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit6 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 6,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit7 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 7,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit8 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 8,

        /// <summary>Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P</summary>
        FsdBit9 = 1 << LtoFlashFeaturesHelpers.SaveDataSectorOffset + 9,

        /// <summary>Program enables the LTO Flash! memory mapper.</summary>
        LtoFlashMemoryMapped = 1 << LtoFlashFeaturesHelpers.LtoFlashMemoryMappedOffset, // 18
    }

    /// <summary>
    /// Extension methods for LtoFlashFeatures.
    /// </summary>
    public static class LtoFlashFeaturesHelpers
    {
        /// <summary>The maximum size of a file in the LTO Flash file system in bytes.</summary>
        public const uint MaxFileSize = Rom.MaxROMSize;

        /// <summary>The maximum number of flash save data sectors that can be assigned on LTO Flash!.</summary>
        public const uint MaxFlashSaveDataSectorsCount = MaxFileSize / JlpFeaturesHelpers.BytesPerSector;

        /// <summary>The default compatibility mode for LTO Flash! features.</summary>
        public const LtoFlashFeatures Default = LtoFlashFeatures.Incompatible;

        /// <summary>
        /// Base bit mask for JLP Flash save data sector count. Use this to build a mask when
        /// encoding flash sector usage counts within the bits of a larger 'features' bit array.
        /// </summary>
        public const ushort FlashBaseSaveDataSectorsCountMask = JlpFeaturesHelpers.JlpFlashBaseSaveDataSectorsCountMask;

        /// <summary>The number of bits reserved for storing flash data sector count.</summary>
        public const int FlashSaveDataSectorsBitCount = 10;

        /// <summary>The bit offset to flags describing the SaveData feature.</summary>
        public const int SaveDataOffset = 2;

        /// <summary>The number of bits reserved for FlashSaveData features.</summary>
        public const int SaveDataFeatureBitCount = 2;

        /// <summary>The bit offset to flag describing whether bankswitching is used.</summary>
        public const int BankswitchingOffset = SaveDataOffset + SaveDataFeatureBitCount; // 4

        /// <summary>The bit offset to flag describing whether additional onboard 16-bit RAM is used.</summary>
        public const int SixteenBitRAMOffset = BankswitchingOffset + 1; // 5

        /// <summary>The bit offset to apply to get the serial port compatibility flags.</summary>
        public const int UsbPortOffset = SixteenBitRAMOffset + 1; // 6

        /// <summary>The number of bits reserved for serial port features.</summary>
        public const int UsbPortFeatureBitCount = 2;

        /// <summary>Number of bits to shift to extract flash sector usage from feature bits.</summary>
        public const int SaveDataSectorOffset = UsbPortOffset + UsbPortFeatureBitCount; // 8

        /// <summary>The number of bits reserved to store how many save data sectors are used.</summary>
        public const int SaveDataSectorBits = 10;

        /// <summary>The number of bits to shift to extract whether LTO Flash memory mapping is enabled.</summary>
        public const int LtoFlashMemoryMappedOffset = SaveDataSectorOffset + SaveDataSectorBits;

        /// <summary>Bit mask to apply to get the save data compatibility flags.</summary>
        public const LtoFlashFeatures SaveDataMask = (LtoFlashFeatures)(FeatureCompatibilityHelpers.CompatibilityMask << SaveDataOffset);

        /// <summary>This mask can be used to recover the number of sectors a program using JLP Flash is using.</summary>
        public const LtoFlashFeatures SaveDataSectorCountMask = (LtoFlashFeatures)(FlashBaseSaveDataSectorsCountMask << SaveDataSectorOffset);

        /// <summary>Bit mask to apply to get the serial port compatibility flags.</summary>
        public const LtoFlashFeatures UsbPortMask = (LtoFlashFeatures)(FeatureCompatibilityHelpers.CompatibilityMask << UsbPortOffset);

        /// <summary>Mask to check for valid feature flags. Note this is only used right now for purposes of serialization.</summary>
        /// <remarks>All the bits that shadow JLP features are excluded.</remarks>
        public const LtoFlashFeatures ValidFeaturesMask = LtoFlashFeatures.Incompatible | LtoFlashFeatures.Tolerates | LtoFlashFeatures.Enhances |
            LtoFlashFeatures.Requires | LtoFlashFeatures.SaveDataOptional | LtoFlashFeatures.SaveDataRequired | LtoFlashFeatures.Bankswitching |
            LtoFlashFeatures.SixteenBitRAM | LtoFlashFeatures.UsbPortEnhanced | LtoFlashFeatures.UsbPortRequired | LtoFlashFeatures.LtoFlashMemoryMapped;

        /// <summary>
        /// Mask to use to retrieve basic feature mask from larger flag sets.
        /// </summary>
        public const uint FeaturesMask = (uint)ValidFeaturesMask;

        /// <summary>
        /// Converts LtoFlashFeatures to LuigiFeatureFlags.
        /// </summary>
        /// <param name="features">The LTO Flash! features to convert.</param>
        /// <returns>The LTO Flash! feature flags represented as LuigiFeatureFlags.</returns>
        public static LuigiFeatureFlags ToLuigiFeatureFlags(this LtoFlashFeatures features)
        {
            var luigiFeatureFlags = ((FeatureCompatibility)features & FeatureCompatibilityHelpers.ValidFeaturesMask).ToLuigiFeatureFlags(FeatureCategory.LtoFlash);
            luigiFeatureFlags |= (LuigiFeatureFlags)(((ulong)(features & SaveDataMask) >> SaveDataOffset) << LuigiFeatureFlagsHelpers.JlpFlashMinimumSaveDataSectorsCountOffset);
            luigiFeatureFlags |= (LuigiFeatureFlags)(((ulong)(features & LtoFlashFeatures.LtoFlashMemoryMapped) >> LtoFlashMemoryMappedOffset) << LuigiFeatureFlagsHelpers.LtoFlashMemoryMapperEnabledOffset);
            return luigiFeatureFlags;
        }
    }
}

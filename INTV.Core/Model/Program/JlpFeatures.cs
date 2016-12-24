// <copyright file="JlpFeatures.cs" company="INTV Funhouse">
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
    /// Flags that describe features provided by the JLP cartridge and which are used by an Intellivision program.
    /// </summary>
    [System.Flags]
    public enum JlpFeatures : uint
    {
        /// <summary>
        /// Program is incompatible with JLP technology.
        /// </summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>
        /// Program uses no JLP-specific features, whether present or not.
        /// </summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>
        /// Program uses JLP-specific acceleration features if present.
        /// </summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>
        /// Program requires JLP-specific acceleration features.
        /// </summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>
        /// Program uses device's save-data feature if available.
        /// </summary>
        SaveDataOptional = 1 << JlpFeaturesHelpers.FlashSaveDataOffset, // 2

        /// <summary>
        /// Program uses device's save-data feature.
        /// </summary>
        SaveDataRequired = 1 << (JlpFeaturesHelpers.FlashSaveDataOffset + 1), // 3

        /// <summary>
        /// Program uses bank switching to access additional onboard memory.
        /// </summary>
        Bankswitching = 1 << JlpFeaturesHelpers.BankswitchingOffset, // 4

        /// <summary>
        /// Program uses additional 16-bit RAM on the device.
        /// </summary>
        SixteenBitRAM = 1 << JlpFeaturesHelpers.SixteenBitRAMOffset, // 5

        /// <summary>
        /// Program uses JLP's serial port capabilities if available.
        /// </summary>
        SerialPortEnhanced = 1 << JlpFeaturesHelpers.SerialPortOffset, // 6

        /// <summary>
        /// Program requires JLP's serial port capabilities.
        /// </summary>
        SerialPortRequired = 1 << (JlpFeaturesHelpers.SerialPortOffset + 1), // 7

        /// <summary>
        /// Program makes use of JLP05 LEDs.
        /// </summary>
        UsesLEDs = 1 << JlpFeaturesHelpers.UsesLEDsOffset, // 8

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit0 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit1 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 1,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit2 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 2,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit3 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 3,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit4 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 4,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit5 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 5,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit6 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 6,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit7 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 7,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit8 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 8,

        /// <summary>
        /// Fugly hack to get this enum to serialize... who's the joker who decided to try to make a Portable Class Library anyway? :P
        /// </summary>
        FsdBit9 = 1 << JlpFeaturesHelpers.FlashSaveDataSectorsOffset + 9,
    }

    /// <summary>
    /// Extension methods for JlpFeatures.
    /// </summary>
    public static class JlpFeaturesHelpers
    {
        /// <summary>
        /// The default compatibility mode for JLP flags.
        /// </summary>
        public const JlpFeatures Default = JlpFeatures.Incompatible;

        /// <summary>
        /// Base bit mask for JLP Flash save data sector count. Use this to build a mask when
        /// encoding flash sector usage counts within the bits of a larger 'features' bit array.
        /// </summary>
        public const ushort JlpFlashBaseSaveDataSectorsCountMask = 0x3FF;

        /// <summary>
        /// The number of bits reserved for storing JLP Flash data sector count.
        /// </summary>
        public const int JlpFlashSaveDataSectorsBitCount = 10;

        /// <summary>
        /// The bit offset to flags describing the SaveData feature.
        /// </summary>
        public const int FlashSaveDataOffset = 2;

        /// <summary>
        /// The number of bits reserved for FlashSaveData features.
        /// </summary>
        public const int FlashSaveDataFeatureBitCount = 2;

        /// <summary>
        /// The bit offset to flag describing whether bankswitching is used.
        /// </summary>
        public const int BankswitchingOffset = FlashSaveDataOffset + FlashSaveDataFeatureBitCount; // 4

        /// <summary>
        /// The bit offset to flag describing whether additional onboard 16-bit RAM is used.
        /// </summary>
        public const int SixteenBitRAMOffset = BankswitchingOffset + 1; // 5

        /// <summary>
        /// The bit offset to apply to get the serial port compatibility flags.
        /// </summary>
        public const int SerialPortOffset = SixteenBitRAMOffset + 1; // 6

        /// <summary>
        /// The number of bits reserved for serial port features.
        /// </summary>
        public const int SerialPortFeatureBitCount = 2;

        /// <summary>
        /// The bit offset to to flag describing whether onboard LED feature is used.
        /// </summary>
        public const int UsesLEDsOffset = SerialPortOffset + SerialPortFeatureBitCount; // 8

        /// <summary>
        /// Number of bits to shift to extract JLP Flash sector usage from feature bits.
        /// </summary>
        public const int FlashSaveDataSectorsOffset = UsesLEDsOffset + 1; // 9

        /// <summary>
        /// Bit mask to apply to get the save data compatibility flags.
        /// </summary>
        public const JlpFeatures FlashSaveDataMask = (JlpFeatures)(FeatureCompatibilityHelpers.CompatibilityMask << FlashSaveDataOffset);

        /// <summary>
        /// Bit mask to apply to get the serial port compatibility flags.
        /// </summary>
        public const JlpFeatures SerialPortMask = (JlpFeatures)(FeatureCompatibilityHelpers.CompatibilityMask << SerialPortOffset);

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const JlpFeatures ValidFeaturesMask = (JlpFeatures)0x1FF;

        /// <summary>
        /// This mask can be used to recover the number of sectors a program using JLP Flash is using.
        /// </summary>
        public const JlpFeatures FlashSaveDataSectorsCountMask = (JlpFeatures)(JlpFlashBaseSaveDataSectorsCountMask << FlashSaveDataSectorsOffset);

        // This information is from the JLP Developer's Guide
        #region JLP Flash Save Data Sector Information

        /// <summary>
        /// The number of rows that define a JLP Flash save data sector (rows / sector).
        /// </summary>
        public const uint RowsPerSector = 8;

        /// <summary>
        /// The number of 16-bit words per row in a JLP Flash row (words / row).
        /// </summary>
        public const uint WordsPerRow = 96;

        /// <summary>
        /// The number of bytes per JLP Flash word (bytes / word).
        /// </summary>
        public const uint BytesPerWord = 2;

        /// <summary>
        /// The number of bytes per JLP Flash storage sector (bytes / sector).
        /// </summary>
        public const uint BytesPerSector = RowsPerSector * WordsPerRow * BytesPerWord;

        /// <summary>
        /// The minimum number of JLP Flash sectors to be used, if any.
        /// </summary>
        public const ushort MinJlpFlashSectorUsage = 1;

        /// <summary>
        /// The practical upper limit on JLP Flash sector usage, based on JLP firmware size and current MCU resources.
        /// Note that this would leave virtually no space for the actual game itself.
        /// </summary>
        public const ushort MaxJlpFlashSectorUsage = (ushort)((MaxMcuFlashSize - JlpEstimatedFirmwareSize) / BytesPerSector);

        /// <summary>
        /// A typical JLP game ROM's size (in bytes) is roughly 48 KB.
        /// </summary>
        public const uint TypicalRomSize = 0xC000;

        /// <summary>
        /// Recommended maximum JLP Flash sector count, based on an assumed ROM size that uses 48 KB of flash.
        /// </summary>
        public const ushort RecommendedMaxJlpFlashSectorUsage = (ushort)((MaxMcuFlashSize - JlpEstimatedFirmwareSize - TypicalRomSize) / BytesPerSector);

        #endregion // JLP Flash Save Data Sector Information

        /// <summary>
        /// Maximum practical PIC24 MCU flash size for current JLP boards, in bytes.
        /// </summary>
        private const uint MaxMcuFlashSize = 0x40000;

        /// <summary>
        /// Estimated space reserved for JLP firmware, in bytes.
        /// </summary>
        private const uint JlpEstimatedFirmwareSize = 0x4000;

        /// <summary>
        /// Gets the minimum number of JLP Flash save data sectors encoded in the feature bits.
        /// </summary>
        /// <param name="features">The JLP features including the save data sectors count.</param>
        /// <returns>The save data sectors.</returns>
        public static ushort MinimumFlashSaveDataSectorsCount(this JlpFeatures features)
        {
            var minimumFlashSaveDataSectorsCount = (ushort)((uint)(features & FlashSaveDataSectorsCountMask) >> FlashSaveDataSectorsOffset);
            return minimumFlashSaveDataSectorsCount;
        }

        /// <summary>
        /// Encodes a minimum number of flash save data sectors into JlpFeatures bits.
        /// </summary>
        /// <param name="minimumFlashSaveDataSectorsCount">The minimum number of JLP Flash save data sectors required.</param>
        /// <returns>The minimum number of JLP Flash save data sectors encoded into the feature bits.</returns>
        public static JlpFeatures MinimumFlashSaveDataSectorsCountToJlpFeatures(this ushort minimumFlashSaveDataSectorsCount)
        {
            return (JlpFeatures)(((uint)(minimumFlashSaveDataSectorsCount & JlpFlashBaseSaveDataSectorsCountMask)) << FlashSaveDataSectorsOffset);
        }

        /// <summary>
        /// Converts JlpFeatures to LuigiFeatureFlags.
        /// </summary>
        /// <param name="features">The JLP features to convert.</param>
        /// <returns>The JLP feature flags represented as LuigiFeatureFlags.</returns>
        public static LuigiFeatureFlags ToLuigiFeatureFlags(this JlpFeatures features)
        {
            var luigiFeatureFlags = ((FeatureCompatibility)features & FeatureCompatibilityHelpers.ValidFeaturesMask).ToLuigiFeatureFlags(FeatureCategory.Jlp);
            var minimumFlashSaveDataSectorsCount = features.MinimumFlashSaveDataSectorsCount();
            luigiFeatureFlags |= minimumFlashSaveDataSectorsCount.MinimumFlashSaveDataSectorsCountToLuigiFeatureFlags();
            return luigiFeatureFlags;
        }

        /// <summary>
        /// Computes the size, in bytes, given a number of JLP Flash sectors.
        /// </summary>
        /// <param name="jlpFlashSectors">Jlp flash sectors.</param>
        /// <returns>The flash sectors to convert to a size in bytes.</returns>
        public static uint JlpFlashSectorsToBytes(this ushort jlpFlashSectors)
        {
            var sizeInBytes = jlpFlashSectors * BytesPerSector;
            return sizeInBytes;
        }

        /// <summary>
        /// Computes the size, in kilobytes, given a number of JLP Flash sectors.
        /// </summary>
        /// <param name="jlpFlashSectors">Jlp flash sectors.</param>
        /// <returns>The flash sectors to convert to a size in kilobytes.</returns>
        /// <remarks>Sorry, I'm not calling them kibibytes.</remarks>
        public static float JlpFlashSectorsToKBytes(this ushort jlpFlashSectors)
        {
            var sizeInKBytes = (float)jlpFlashSectors.JlpFlashSectorsToBytes() / 0x400;
            return sizeInKBytes;
        }
    }
}

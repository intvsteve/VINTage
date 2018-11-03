// <copyright file="LuigiFeatureFlags.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// These flags describe the features of a program ROM placed into a LUIGI file.
    /// </summary>
    /// <remarks>These features actually comprise a bit array based upon multiple applications of the more
    /// general FeatureCompatibility flags from INTV.Core.</remarks>
    [System.Flags]
    public enum LuigiFeatureFlags : ulong
    {
        /// <summary>No features reported.</summary>
        None = 0,

        #region Intellivoice Flags

        /// <summary>Intellivoice compatibility mask.</summary>
        IntellivoiceMask = ((ulong)FeatureCompatibilityHelpers.CompatibilityMask) << LuigiFeatureFlagsHelpers.IntellivoiceOffset,

        #endregion // Intellivoice Flags

        #region ECS Flags

        /// <summary>ECS compatibility mask.</summary>
        EcsMask = ((ulong)FeatureCompatibilityHelpers.CompatibilityMask) << LuigiFeatureFlagsHelpers.EcsOffset,

        #endregion // ECS Flags

        #region Intellivision II Flags

        /// <summary>Intellivision II compatibility mask.</summary>
        IntellivisionIIMask = ((ulong)FeatureCompatibilityHelpers.CompatibilityMask) << LuigiFeatureFlagsHelpers.IntellivisionIIOffset,

        #endregion // Intellivision II Flags

        #region Keyboard Component Flags

        /// <summary>Keyboard Component compatibility mask.</summary>
        KeyboardComponentMask = ((ulong)FeatureCompatibilityHelpers.CompatibilityMask) << LuigiFeatureFlagsHelpers.KeyboardComponentOffset,

        #endregion // Keyboard Component Flags

        #region ExtendedPeripheralCompatibilityVersion Flags

        /// <summary>Mask to use to retrieve version of compatibility flags field sub-version.</summary>
        ExtendedPeripheralCompatibilityVersionMask = 0x03ul << LuigiFeatureFlagsHelpers.ExtendedPeripheralCompatibiltyVersionOffset,

        #endregion

        #region TutorVision Flags

        /// <summary>TutorVision compatibility mask.</summary>
        /// <remarks>NOTE: ONLY valid if the ExtendedPeripheralCompatibility version is greater than zero!</remarks>
        TutorVisionMask = ((ulong)FeatureCompatibilityHelpers.CompatibilityMask) << LuigiFeatureFlagsHelpers.TutorVisionOffset,

        #endregion // TutorVision Flags

        #region Reserved Peripheral Flags

        /// <summary>Reserved peripheral flags mask.</summary>
        ReservedMask = 0x0Ful << LuigiFeatureFlagsHelpers.ReservedPeripheralOffset,

        #endregion // Reserved Peripheral Flags

        #region JLP Acceleration Flags

        /// <summary>JLP acceleration features mask.</summary>
        JlpAccelerationMask = ((ulong)FeatureCompatibilityHelpers.CompatibilityMask) << LuigiFeatureFlagsHelpers.JlpAccelerationOffset,

        #endregion // JLP Acceleration Flags

        #region JLP Reserved Flags

        /// <summary>JLP reserved feature bits mask.</summary>
        JlpReservedMask = 0xF << LuigiFeatureFlagsHelpers.JlpReservedOffset,

        #endregion // JLP Reserved Flags

        #region JLP Flash Minimum Sector Count Bits

        /// <summary>JLP flash save data features mask.</summary>
        JlpFlashMinimumSaveDataSectorCountMask = ((ulong)JlpFeaturesHelpers.JlpFlashBaseSaveDataSectorsCountMask) << LuigiFeatureFlagsHelpers.JlpFlashMinimumSaveDataSectorsCountOffset,

        #endregion // JLP Flash Minimum Sector Count Bits

        #region LTO Flash! Flags

        /// <summary>If set, indicates that LTO Flash's memory mapper has been enabled at $1000 - $14FF.</summary>
        LtoFlashMemoryMapperEnabled = 1ul << 32,

        #endregion // LTO Flash! Flags

        #region Utility Flags

        /// <summary>If set, indicates that the LUIGI flags have been explicitly defined via cfg_vars in a .cfg file, for example, rather than via defaults.</summary>
        FeatureFlagsExplicitlySet = 1ul << 63,

        /// <summary>Mask for unused feature bits.</summary>
        UnusedMask = ~(IntellivoiceMask | EcsMask | IntellivisionIIMask | KeyboardComponentMask | ExtendedPeripheralCompatibilityVersionMask | TutorVisionMask | JlpAccelerationMask | JlpReservedMask | JlpFlashMinimumSaveDataSectorCountMask | LtoFlashMemoryMapperEnabled | FeatureFlagsExplicitlySet)

        #endregion // Utility Flags
    }

    /// <summary>
    /// Extension methods for LuigiFeatureFlags.
    /// </summary>
    public static class LuigiFeatureFlagsHelpers
    {
        #region Intellivoice Bits

        /// <summary>Bit shift amount for Intellivoice features.</summary>
        internal const int IntellivoiceOffset = 0;

        private const int IntellivoiceBitCount = 2;

        #endregion // Intellivoice Bits

        #region ECS Bits

        /// <summary>Bit shift amount for ECS features.</summary>
        internal const int EcsOffset = IntellivoiceOffset + IntellivoiceBitCount; // (2)

        private const int EcsBitCount = 2;

        #endregion // ECS Bits

        #region Intellivision II Bits

        /// <summary>Bit shift amount for Intellivision II features.</summary>
        internal const int IntellivisionIIOffset = EcsOffset + EcsBitCount; // (4)

        private const int IntellivisionIIBitCount = 2;

        #endregion // Intellivision II Bits

        #region Keyboard Component Bits

        /// <summary>Bit shift amount for Keyboard Component features.</summary>
        internal const int KeyboardComponentOffset = IntellivisionIIOffset + IntellivisionIIBitCount; // (6)

        private const int KeyboardComponentBitCount = 2;

        #endregion // Keyboard Component Bits

        #region Extended Peripheral Compatibility Version Bits

        /// <summary>Bit shift amount for extended peripheral compatibility bits field sub-version.</summary>
        internal const int ExtendedPeripheralCompatibiltyVersionOffset = KeyboardComponentOffset + KeyboardComponentBitCount; // (8)

        private const int ExtendedPeripheralCompatibiltyVersionBitCount = 2;

        #endregion // Extended Peripheral Compatibility Version Bits

        #region TutorVision Bits

        /// <summary>Bit shift amount for TutorVision features.</summary>
        /// <remarks>TutorVision bits should only be used if the compatibility sub-version is greater than zero.</remarks>
        internal const int TutorVisionOffset = ExtendedPeripheralCompatibiltyVersionOffset + ExtendedPeripheralCompatibiltyVersionBitCount; // (10)

        private const int TutorVisionBitCount = 2;

        #endregion // TutorVision Bits

        #region Reserved Peripheral Bits

        /// <summary>Bit shift amount for reserved peripheral flags.</summary>
        internal const int ReservedPeripheralOffset = TutorVisionOffset + TutorVisionBitCount; // (12)

        private const int ReservedPeripheralBitCount = 4;

        #endregion // Reserved Peripheral Bits

        #region JLP Acceleration Bits

        /// <summary>Bit shift amount for JLP acceleration features.</summary>
        internal const int JlpAccelerationOffset = ReservedPeripheralOffset + ReservedPeripheralBitCount; // (16)

        private const int JlpAccelerationBitCount = 2;

        #endregion // JLP Acceleration Bits

        #region JLP Reserved Bits

        /// <summary>Bit shift amount for JLP reserved features.</summary>
        internal const int JlpReservedOffset = JlpAccelerationOffset + JlpAccelerationBitCount; // (18)

        private const int JlpReservedBitCount = 4;

        #endregion // JLP Reserved Bits

        #region JLP Flash Minimum Sector Count Size

        /// <summary>Bit shift amount for JLP flash minimum save data sector count.</summary>
        internal const int JlpFlashMinimumSaveDataSectorsCountOffset = JlpReservedOffset + JlpReservedBitCount; // (22)

        private const int JlpFlashMinimumSaveDataSectorCountBitCount = 10;

        #endregion // JLP Flash Minimum Sector Count Size

        #region LTO Flash! Bits

        /// <summary>Bit shift amount for LTO Flash! memory map enabled.</summary>
        internal const int LtoFlashMemoryMapperEnabledOffset = 32;

        /// <summary>Bit shift amount for whether feature flags were explicitly set from input source for LUIGI file (e.g. a .cfg file).</summary>
        internal const int LtoFlashFeatureFlagsExplicitlySetOffset = 63;

        #endregion // LTO Flash! Bits

        /// <summary>
        /// Convert LuigiFeatureFlags into a ProgramFeatures object.
        /// </summary>
        /// <param name="featureFlags">The flags to convert,</param>
        /// <returns>ProgramFeatures representing the compatibility modes described by the feature flags.</returns>
        public static ProgramFeatures ToProgramFeatures(this LuigiFeatureFlags featureFlags)
        {
            var programFeatures = new ProgramFeatures();

            var intellivoiceCompatibililty = (uint)(featureFlags & LuigiFeatureFlags.IntellivoiceMask);
            programFeatures.Intellivoice = (FeatureCompatibility)(intellivoiceCompatibililty >> IntellivoiceOffset);

            var ecsCompatibility = (uint)(featureFlags & LuigiFeatureFlags.EcsMask);
            programFeatures.Ecs = (EcsFeatures)(ecsCompatibility >> EcsOffset);

            var intellivisionIICompatibility = (uint)(featureFlags & LuigiFeatureFlags.IntellivisionIIMask);
            programFeatures.IntellivisionII = (FeatureCompatibility)(intellivisionIICompatibility >> IntellivisionIIOffset);

            var keyboardComponentCompatibility = (uint)(featureFlags & LuigiFeatureFlags.KeyboardComponentMask);
            programFeatures.KeyboardComponent = (KeyboardComponentFeatures)(keyboardComponentCompatibility >> KeyboardComponentOffset);

            var extendedPeripheralCompatibilityVersion = featureFlags.ExtendedPeripheralCompatabilityBitsVersion();
            if (extendedPeripheralCompatibilityVersion > 0)
            {
                var tutorvisionCompatibility = (uint)(featureFlags & LuigiFeatureFlags.TutorVisionMask);
                programFeatures.Tutorvision = (FeatureCompatibility)(tutorvisionCompatibility >> TutorVisionOffset);
            }

            if (extendedPeripheralCompatibilityVersion > 1)
            {
                // TBD
            }

            if (extendedPeripheralCompatibilityVersion > 2)
            {
                // TBD
            }

            var jlpAccelerationCompatibility = (uint)(featureFlags & LuigiFeatureFlags.JlpAccelerationMask);
            programFeatures.Jlp = (JlpFeatures)(jlpAccelerationCompatibility >> JlpAccelerationOffset);

            programFeatures.JlpFlashMinimumSaveSectors = featureFlags.MinimumFlashSaveDataSectors();

            if (programFeatures.JlpFlashMinimumSaveSectors > 0)
            {
                programFeatures.Jlp |= JlpFeatures.SaveDataRequired;
            }

            if ((jlpAccelerationCompatibility != 0) || (programFeatures.JlpFlashMinimumSaveSectors > 0))
            {
                if (programFeatures.JlpHardwareVersion == JlpHardwareVersion.None)
                {
                    programFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp03;
                }
            }

            var ltoFlashMemoryMapper = (ulong)(featureFlags & LuigiFeatureFlags.LtoFlashMemoryMapperEnabled);
            if (ltoFlashMemoryMapper != 0)
            {
                programFeatures.LtoFlash |= LtoFlashFeatures.LtoFlashMemoryMapped;
            }

            return programFeatures;
        }

        /// <summary>
        /// Encodes a maximum JLP Flash save data sector count into flags.
        /// </summary>
        /// <param name="sectorCount">The minimum number of JLP flash sectors needed.</param>
        /// <returns>The JLP flash sector count represented as bits in the LuigiFeatureFlags.</returns>
        public static LuigiFeatureFlags MinimumFlashSaveDataSectorsCountToLuigiFeatureFlags(this ushort sectorCount)
        {
            sectorCount &= JlpFeaturesHelpers.JlpFlashBaseSaveDataSectorsCountMask;
            var encodedLuigiFeatureFlags = (LuigiFeatureFlags)(((ulong)sectorCount) << JlpFlashMinimumSaveDataSectorsCountOffset);
            return encodedLuigiFeatureFlags;
        }

        /// <summary>
        /// Extracts the minimum number of JLP Flash save data sectors from the feature flags.
        /// </summary>
        /// <param name="featureFlags">The flags containing the encoded sector count.</param>
        /// <returns>The minimum number of required save data sectors.</returns>
        public static ushort MinimumFlashSaveDataSectors(this LuigiFeatureFlags featureFlags)
        {
            var minimumJlpFlashSaveSectors = (ulong)(featureFlags & LuigiFeatureFlags.JlpFlashMinimumSaveDataSectorCountMask) >> JlpFlashMinimumSaveDataSectorsCountOffset;
            return (ushort)minimumJlpFlashSaveSectors;
        }

        /// <summary>
        /// Extracts the version number of extended peripheral compatibility flags data from the feature flags.
        /// </summary>
        /// <param name="featureFlags">The flags whose extended peripheral compatibility flags are needed.</param>
        /// <returns>The extended peripheral flags version number.</returns>
        public static byte ExtendedPeripheralCompatabilityBitsVersion(this LuigiFeatureFlags featureFlags)
        {
            byte compatibilitySubVersion = (byte)((ulong)(featureFlags & LuigiFeatureFlags.ExtendedPeripheralCompatibilityVersionMask) >> ExtendedPeripheralCompatibiltyVersionOffset);
            return compatibilitySubVersion;
        }
    }
}

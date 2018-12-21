// <copyright file="CuttleCart3Features.cs" company="INTV Funhouse">
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
    /// Flags that describe features provided by the Cuttle Cart 3, and which are used by an Intellivision program.
    /// </summary>
    [System.Flags]
    public enum CuttleCart3Features : uint
    {
        /// <summary>
        /// Program is incompatible with Cuttle Cart 3 technology.
        /// </summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>
        /// Program uses no Cuttle Cart 3 - specific features.
        /// </summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>
        /// Program uses Cuttle Cart 3 -specific features if present.
        /// </summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>
        /// Program uses Cuttle Cart 3 - specific features and requires them to be present.
        /// </summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>
        /// Program uses the bank switching features of the Cuttle Cart 3 to access additional storage. Also compatible with Intellicart.
        /// </summary>
        Bankswitching = 1 << CuttleCart3FeaturesHelpers.BankswitchingOffset, // 2

        /// <summary>
        /// Program uses Cuttle Cart 3's ability to emulate Mattel-style bank switching for large ROMs.
        /// </summary>
        MattelBankswitching = 1 << (CuttleCart3FeaturesHelpers.BankswitchingOffset + 1), // 3

        /// <summary>
        /// Program addresses additional RAM on the Cuttle Cart 3 as 16-bit RAM.
        /// </summary>
        SixteenBitRAM = 1 << CuttleCart3FeaturesHelpers.RamModeOffset, // 4

        /// <summary>
        /// Program addresses additional RAM on the Cuttle Cart 3 as 8-bit RAM.
        /// </summary>
        EightBitRAM = 1 << (CuttleCart3FeaturesHelpers.RamModeOffset + 1), // 5

        /// <summary>
        /// Program has optional additional features using serial port.
        /// </summary>
        SerialPortEnhanced = 1 << CuttleCart3FeaturesHelpers.SerialPortOffset, // 6

        /// <summary>
        /// Program requires access to the serial port on the Cuttle Cart 3.
        /// </summary>
        SerialPortRequired = 1 << (CuttleCart3FeaturesHelpers.SerialPortOffset + 1), // 7
    }

    /// <summary>
    /// Extension methods for CuttleCart3Features.
    /// </summary>
    public static class CuttleCart3FeaturesHelpers
    {
        /// <summary>
        /// The default compatibility mode for the Cuttle Cart 3.
        /// </summary>
        public const CuttleCart3Features Default = CuttleCart3Features.Tolerates;

        /// <summary>
        /// The bit offset to apply to create or access bank switching mode flags.
        /// </summary>
        public const int BankswitchingOffset = 2;

        /// <summary>
        /// Bit mask to apply to get the bankswitching mode flags.
        /// </summary>
        public const CuttleCart3Features BankswitchingMask = (CuttleCart3Features)(FeatureCompatibilityHelpers.CompatibilityMask << BankswitchingOffset);

        /// <summary>
        /// The bit offset to apply to create or access RAM mode flags.
        /// </summary>
        public const int RamModeOffset = BankswitchingOffset + 2;

        /// <summary>
        /// Bit mask to apply to get the RAM mode flags.
        /// </summary>
        public const CuttleCart3Features RamModeMask = (CuttleCart3Features)(FeatureCompatibilityHelpers.CompatibilityMask << RamModeOffset);

        /// <summary>
        /// The bit offset to apply to create or access the serial port usage flags.
        /// </summary>
        public const int SerialPortOffset = RamModeOffset + 2;

        /// <summary>
        /// Bit mask to apply to get the serial port usage flags.
        /// </summary>
        public const CuttleCart3Features SerialPortMask = (CuttleCart3Features)(FeatureCompatibilityHelpers.CompatibilityMask << SerialPortOffset);

        /// <summary>
        /// Mask to use to retrieve basic feature mask from larger flag sets.
        /// </summary>
        public const uint FeaturesMask = 0xFF;

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const CuttleCart3Features ValidFeaturesMask = (CuttleCart3Features)FeaturesMask;
    }
}

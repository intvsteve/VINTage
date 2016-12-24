// <copyright file="IntellicartCC3Features.cs" company="INTV Funhouse">
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
    /// Flags that describe features provided by the Intellicart and Cuttle Cart 3 cartridges, and which are used by an Intellivision program.
    /// </summary>
    [System.Flags]
    public enum IntellicartCC3Features : uint
    {
        /// <summary>
        /// Program is incompatible with Intellicart / Cuttle Cart 3 technology.
        /// </summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>
        /// Program uses no Intellicart or Cuttle Cart 3 - specific features.
        /// </summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>
        /// Program uses Intellicart or Cuttle Cart 3 -specific features if present.
        /// </summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>
        /// Program uses Intellicart or Cuttle Cart 3 - specific features and requires them to be present.
        /// </summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>
        /// Program uses the bank switching features of the Intellicart / Cuttle Cart 3 to access additional storage.
        /// </summary>
        Bankswitching = 1 << 2,

        /// <summary>
        /// Program addresses additional RAM on the Intellicart / Cuttle Cart 3 as 16-bit RAM.
        /// </summary>
        SixteenBitRAM = 1 << 3,

        /// <summary>
        /// Program is enhanced if serial port is connected.
        /// </summary>
        SerialPortEnhanced = 1 << IntellicartCC3FeaturesHelpers.SerialPortOffset, // 4

        /// <summary>
        /// Program requires using the serial port.
        /// </summary>
        SerialPortRequired = 1 << (IntellicartCC3FeaturesHelpers.SerialPortOffset + 1), // 5
    }

    /// <summary>
    /// Extension methods for IntellicartCC3Features.
    /// </summary>
    public static class IntellicartCC3FeaturesHelpers
    {
        /// <summary>
        /// The default compatibility mode for the Intellicart.
        /// </summary>
        public const IntellicartCC3Features Default = IntellicartCC3Features.Tolerates;

        /// <summary>
        /// The bit offset to apply to get the serial port compatibility flags.
        /// </summary>
        public const int SerialPortOffset = 4;

        /// <summary>
        /// Bit mask to apply to get the serial port compatibility flags.
        /// </summary>
        public const IntellicartCC3Features SerialPortMask = (IntellicartCC3Features)(FeatureCompatibilityHelpers.CompatibilityMask << SerialPortOffset);

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const IntellicartCC3Features ValidFeaturesMask = (IntellicartCC3Features)0x3F;
    }
}

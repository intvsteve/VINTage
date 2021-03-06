﻿// <copyright file="Bee3Features.cs" company="INTV Funhouse">
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
    /// Speculative set of feature flags for Marc Ball's yet-to-be-released cartridge platform.
    /// There may be some redundant flags here. Will revisit as more info becomes available.
    /// </summary>
    [System.Flags]
    public enum Bee3Features : uint
    {
        /// <summary>
        /// Program does not use Bee3 technology.
        /// </summary>
        Incompatible = FeatureCompatibility.Incompatible,

        /// <summary>
        /// Placeholder... Intended use is: May use Bee3-specific accelerators only.
        /// </summary>
        Tolerates = FeatureCompatibility.Tolerates,

        /// <summary>
        /// Placeholder.... Intended use is: May use Bee3-specific storage.
        /// </summary>
        Enhances = FeatureCompatibility.Enhances,

        /// <summary>
        /// Placeholder. Intended use is: May use Bee3-specific accelerators AND storage.
        /// </summary>
        Requires = FeatureCompatibility.Requires,

        /// <summary>
        /// Program uses device's save-data feature if available.
        /// </summary>
        SaveDataOptional = 1 << 2,

        /// <summary>
        /// Program requires device's save-data feature.
        /// </summary>
        SaveDataRequired = 1 << 3,

        /// <summary>
        /// Program uses additional 16-bit RAM on the device.
        /// </summary>
        SixteenBitRAM = 1 << 4,
    }

    /// <summary>
    /// Extension methods for Bee3Features.
    /// </summary>
    public static class Bee3FeaturesHelpers
    {
        /// <summary>
        /// The default compatibility mode for Bee3.
        /// </summary>
        public const Bee3Features Default = Bee3Features.Incompatible;

        /// <summary>
        /// Mask to use to retrieve basic feature mask from larger flag sets.
        /// </summary>
        public const uint FeaturesMask = 0x1F;

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const Bee3Features ValidFeaturesMask = (Bee3Features)FeaturesMask;
    }
}

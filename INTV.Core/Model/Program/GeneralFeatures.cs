// <copyright file="GeneralFeatures.cs" company="INTV Funhouse">
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
    /// General-purpose features that are not specific to a platform or target hardware.
    /// </summary>
    [System.Flags]
    public enum GeneralFeatures : uint
    {
        /// <summary>
        /// No special features.
        /// </summary>
        None,

        /// <summary>
        /// Tags a program ROM as not defined in the existing database.
        /// </summary>
        UnrecognizedRom = 1 << 0,

        /// <summary>
        /// Uses page-flipping to access more memory.
        /// </summary>
        PageFlipping = 1 << 1,

        /// <summary>
        /// A cartridge that includes additional on-board RAM that is not based on a specific cartridge technology.
        /// </summary>
        OnboardRam = 1 << 2,

        // Consider flags for test, demo?

        /// <summary>
        /// Identifies ROMs that are part of a peripheral or the system itself.
        /// </summary>
        SystemRom = 1U << 31,
    }

    /// <summary>
    ///  Extension methods for GeneralFeatures.
    /// </summary>
    public static class GeneralFeaturesHelpers
    {
        /// <summary>
        /// Valid flags.
        /// </summary>
        public const GeneralFeatures ValidFeaturesMask = (GeneralFeatures)0x80000007;
    }
}

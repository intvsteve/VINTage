// <copyright file="FeatureCategory.cs" company="INTV Funhouse">
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
    /// Enumerates different categories of feature flags used to describe Intellivision
    /// compatibility modes for hardware and software.
    /// </summary>
    public enum FeatureCategory
    {
        /// <summary>
        /// NTSC video standard compatibility modes.
        /// </summary>
        Ntsc,

        /// <summary>
        /// PAL video standard compatibility modes.
        /// </summary>
        Pal,

        /// <summary>
        /// General-purpose program features.
        /// </summary>
        General,

        /// <summary>
        /// Original Model 1149 Keyboard Component-related features and compatibility flags.
        /// </summary>
        KeyboardComponent,

        /// <summary>
        /// Compatibility descriptor for Sears Super Video Arcade.
        /// </summary>
        SuperVideoArcade,

        /// <summary>
        /// Compatibility descriptor for the Intellivoice.
        /// </summary>
        Intellivoice,

        /// <summary>
        /// Compatibility descriptor for the Intellivision II.
        /// </summary>
        IntellivisionII,

        /// <summary>
        /// Compatibility descriptor and features for the Entertainment Computer System.
        /// </summary>
        Ecs,

        /// <summary>
        /// Compatibility descriptor for the Tutorvision.
        /// </summary>
        Tutorvision,

        /// <summary>
        /// Compatibility descriptor and features for the Intellicart.
        /// </summary>
        Intellicart,

        /// <summary>
        /// Compatibility descriptor and features for the Cuttle Cart 3.
        /// </summary>
        CuttleCart3,

        /// <summary>
        /// Compatibility descriptor and features for the JLP cartridge platform.
        /// </summary>
        Jlp,

        /// <summary>
        /// Compatibility descriptor and features for the LTO Flash! cartridge platform.
        /// </summary>
        LtoFlash,

        /// <summary>
        /// Compatibility descriptor and features for the 'Bee3' cartridge platform.
        /// </summary>
        Bee3,

        /// <summary>
        /// Compatibility descriptor and features for the 'Hive' cartridge platform.
        /// </summary>
        Hive,

        /// <summary>
        /// Sentinel value for number of defined categories.
        /// </summary>
        NumberOfCategories
    }
}

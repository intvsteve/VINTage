// <copyright file="IProgramFeatures.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

using System;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// An interface to describe the features of an Intellivision program.
    /// </summary>
    public interface IProgramFeatures : IComparable, IComparable<IProgramFeatures>
    {
        /// <summary>
        /// Gets NTSC compatibility.
        /// </summary>
        FeatureCompatibility Ntsc { get; }

        /// <summary>
        /// Gets PAL compatibility.
        /// </summary>
        FeatureCompatibility Pal { get; }

        /// <summary>
        /// Gets general features.
        /// </summary>
        GeneralFeatures GeneralFeatures { get; }

        /// <summary>
        /// Gets Keyboard Component features and compatibility.
        /// </summary>
        KeyboardComponentFeatures KeyboardComponent { get; }

        /// <summary>
        /// Gets Sears Super Video Arcade compatibility.
        /// </summary>
        FeatureCompatibility SuperVideoArcade { get; }

        /// <summary>
        /// Gets Intellivoice compatibility.
        /// </summary>
        FeatureCompatibility Intellivoice { get; }

        /// <summary>
        /// Gets Intellivision II compatibility.
        /// </summary>
        FeatureCompatibility IntellivisionII { get; }

        /// <summary>
        /// Gets ECS features and compatibility.
        /// </summary>
        EcsFeatures Ecs { get; }

        /// <summary>
        /// Gets Tutorvision compatibility.
        /// </summary>
        FeatureCompatibility Tutorvision { get; }

        /// <summary>
        /// Gets Intellicart features and compatibility.
        /// </summary>
        IntellicartCC3Features Intellicart { get; }

        /// <summary>
        /// Gets Cuttle Cart 3 features and compatibility.
        /// </summary>
        CuttleCart3Features CuttleCart3 { get; }

        /// <summary>
        /// Gets JLP features and compatibility.
        /// </summary>
        JlpFeatures Jlp { get; }

        /// <summary>
        /// Gets the JLP hardware version.
        /// </summary>
        JlpHardwareVersion JlpHardwareVersion { get; }

        /// <summary>
        /// Gets the minimum number of JLP Flash save data sectors required by a program.
        /// </summary>
        ushort JlpFlashMinimumSaveSectors { get; }

        /// <summary>
        /// Gets LTO Flash features and compatibility.
        /// </summary>
        LtoFlashFeatures LtoFlash { get; }

        /// <summary>
        /// Gets Bee3 features and compatibility.
        /// </summary>
        Bee3Features Bee3 { get; }

        /// <summary>
        /// Gets Hive features and compatibility.
        /// </summary>
        HiveFeatures Hive { get; }

        /// <summary>
        /// Gets program features as compatibility bits for LTO Flash!
        /// </summary>
        LuigiFeatureFlags LuigiFeaturesLo { get; }

        /// <summary>
        /// Gets program features as compatibility bits for LTO Flash!
        /// </summary>
        LuigiFeatureFlags2 LuigiFeaturesHi { get; }
    }
}

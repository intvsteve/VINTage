// <copyright file="IncompatibilityFlags.cs" company="INTV Funhouse">
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

using System;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// These flags describe any known program incompatibilities with hardware.
    /// </summary>
    [Flags]
    public enum IncompatibilityFlags
    {
        /// <summary>
        /// No known incompatibilities have been found.
        /// </summary>
        None = 0,

        /// <summary>
        /// The program is known to be incompatible with NTSC consoles, but functions correctly on PAL consoles.
        /// </summary>
        Ntsc = 1 << 0,

        /// <summary>
        /// The program is known to be incompatible with PAL consoles, but functions correctly on NTSC consoles.
        /// </summary>
        Pal = 1 << 1,

        /// <summary>
        /// The program is incompatible with the Keyboard Component.
        /// </summary>
        KeyboardComponent = 1 << 2,

        /// <summary>
        /// The program is known to have a bug or other compatibility issue with the Sears Super Video Arcade.
        /// </summary>
        SuperVideoArcade = 1 << 3,

        /// <summary>
        /// The program runs on Sears Super Video Arcade, but behaves differently.
        /// </summary>
        SuperVideoArcadeAltered = 1 << 4,

        /// <summary>
        /// The program is not compatible with the Intellivoice module.
        /// </summary>
        Intellivoice = 1 << 5,

        /// <summary>
        /// The program will not run on the Intellivision II console.
        /// </summary>
        IntellivisionII = 1 << 6,

        /// <summary>
        /// Program runs on Intellivision II, but behaves differently.
        /// </summary>
        IntellivisionIIAltered = 1 << 7,

        /// <summary>
        /// The program is incompatible with the ECS.
        /// </summary>
        Ecs = 1 << 8,

        /// <summary>
        /// The program is incompatible with the Tutorvision.
        /// </summary>
        Tutorvision = 1 << 9,

        /// <summary>
        /// The program is incompatible with the Intellicart.
        /// </summary>
        Intellicart = 1 << 10,

        /// <summary>
        /// The program is incompatible with the Cuttle Cart 3.
        /// </summary>
        CuttleCart3 = 1 << 11,

        /// <summary>
        /// The program is incompatible with the JLP hardware.
        /// </summary>
        Jlp = 1 << 12,

        /// <summary>
        /// The program is incompatible with the LTO Flash! hardware.
        /// </summary>
        LtoFlash = 1 << 13,

        /// <summary>
        /// The program is incompatible with the Bee3 hardware.
        /// </summary>
        Bee3 = 1 << 14,

        /// <summary>
        /// The program is incompatible with the Hive multi-cart hardware.
        /// </summary>
        Hive = 1 << 15,
    }

    /// <summary>
    /// Helper methods for the IncompatibilityFlags type.
    /// </summary>
    public static class IncompatibilityFlagsHelpers
    {
        /// <summary>
        /// Expands a set of condensed incompatibility flags to a full-fledged ProgramFeatures set.
        /// </summary>
        /// <param name="incompatibilityFlags">The flags to convert.</param>
        /// <returns>A ProgramFeatures object that includes all incompatibilities.</returns>
        /// <remarks>The default features are contained in the returned features for any incompatibility flag that is not set.</remarks>
        public static IProgramFeatures ToProgramFeatures(this IncompatibilityFlags incompatibilityFlags)
        {
            var programFeatures = new ProgramFeatures();
            return incompatibilityFlags.ApplyFlagsToProgramFeatures(programFeatures);
        }

        /// <summary>
        /// Updates a set of ProgramFeatures to include an incompatibilities set in the given flags.
        /// </summary>
        /// <param name="incompatibilityFlags">Condensed incompatibility information.</param>
        /// <param name="programFeatures">The program features to update.</param>
        /// <returns>The updated program features.</returns>
        public static ProgramFeatures ApplyFlagsToProgramFeatures(this IncompatibilityFlags incompatibilityFlags, ProgramFeatures programFeatures)
        {
            if ((incompatibilityFlags & IncompatibilityFlags.Ntsc) == IncompatibilityFlags.Ntsc)
            {
                programFeatures.Ntsc = FeatureCompatibility.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Pal) == IncompatibilityFlags.Pal)
            {
                programFeatures.Pal = FeatureCompatibility.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.KeyboardComponent) == IncompatibilityFlags.KeyboardComponent)
            {
                programFeatures.KeyboardComponent = KeyboardComponentFeatures.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.SuperVideoArcade) == IncompatibilityFlags.SuperVideoArcade)
            {
                programFeatures.SuperVideoArcade = FeatureCompatibility.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.SuperVideoArcadeAltered) == IncompatibilityFlags.SuperVideoArcadeAltered)
            {
                programFeatures.SuperVideoArcade = FeatureCompatibility.Enhances;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Intellivoice) == IncompatibilityFlags.Intellivoice)
            {
                programFeatures.Intellivoice = FeatureCompatibility.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.IntellivisionII) == IncompatibilityFlags.IntellivisionII)
            {
                programFeatures.IntellivisionII = FeatureCompatibility.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.IntellivisionIIAltered) == IncompatibilityFlags.IntellivisionIIAltered)
            {
                programFeatures.IntellivisionII = FeatureCompatibility.Enhances;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Ecs) == IncompatibilityFlags.Ecs)
            {
                programFeatures.Ecs = EcsFeatures.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Tutorvision) == IncompatibilityFlags.Tutorvision)
            {
                programFeatures.Tutorvision = FeatureCompatibility.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Intellicart) == IncompatibilityFlags.Intellicart)
            {
                programFeatures.Intellicart = IntellicartCC3Features.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.CuttleCart3) == IncompatibilityFlags.CuttleCart3)
            {
                programFeatures.CuttleCart3 = CuttleCart3Features.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Jlp) == IncompatibilityFlags.Jlp)
            {
                programFeatures.Jlp = JlpFeatures.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.LtoFlash) == IncompatibilityFlags.LtoFlash)
            {
                programFeatures.LtoFlash = LtoFlashFeatures.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Bee3) == IncompatibilityFlags.Bee3)
            {
                programFeatures.Bee3 = Bee3Features.Incompatible;
            }
            if ((incompatibilityFlags & IncompatibilityFlags.Hive) == IncompatibilityFlags.Hive)
            {
                programFeatures.Hive = HiveFeatures.Incompatible;
            }
            return programFeatures;
        }
    }
}

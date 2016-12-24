// <copyright file="ProgramFeaturesHelpers.cs" company="INTV Funhouse">
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
    /// Helper methods for the ProgramFeatures type.
    /// </summary>
    public static class ProgramFeaturesHelpers
    {
        /// <summary>
        /// Creates a condensed set of incompatibility features from a full set of ProgramFeatures.
        /// </summary>
        /// <param name="programFeatures">The program features from which to create a condensed set of incompatibility flags.</param>
        /// <returns>The incompatibility flags.</returns>
        public static IncompatibilityFlags ToIncompatibilityFlags(this ProgramFeatures programFeatures)
        {
            var incompatibilityFlags = IncompatibilityFlags.None;
            if (programFeatures.Ntsc == FeatureCompatibility.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Ntsc;
            }
            if (programFeatures.Pal == FeatureCompatibility.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Pal;
            }
            if (programFeatures.KeyboardComponent == KeyboardComponentFeatures.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.KeyboardComponent;
            }
            if (programFeatures.SuperVideoArcade == FeatureCompatibility.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.SuperVideoArcade;
            }
            if (programFeatures.SuperVideoArcade == FeatureCompatibility.Enhances)
            {
                incompatibilityFlags |= IncompatibilityFlags.SuperVideoArcadeAltered;
            }
            if (programFeatures.Intellivoice == FeatureCompatibility.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Intellivoice;
            }
            if (programFeatures.IntellivisionII == FeatureCompatibility.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.IntellivisionII;
            }
            if (programFeatures.IntellivisionII == FeatureCompatibility.Enhances)
            {
                incompatibilityFlags |= IncompatibilityFlags.IntellivisionIIAltered;
            }
            if (programFeatures.Ecs == EcsFeatures.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Ecs;
            }
            if (programFeatures.Tutorvision == FeatureCompatibility.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Tutorvision;
            }
            if (programFeatures.Intellicart == IntellicartCC3Features.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Intellicart;
            }
            if (programFeatures.CuttleCart3 == CuttleCart3Features.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.CuttleCart3;
            }
            if (programFeatures.Jlp == JlpFeatures.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Jlp;
            }
            if (programFeatures.LtoFlash == LtoFlashFeatures.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.LtoFlash;
            }
            if (programFeatures.Bee3 == Bee3Features.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Bee3;
            }
            if (programFeatures.Hive == HiveFeatures.Incompatible)
            {
                incompatibilityFlags |= IncompatibilityFlags.Hive;
            }
            return incompatibilityFlags;
        }
    }
}

// <copyright file="ProgramFeaturesHelpersTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramFeaturesHelpersTests
    {
        [Fact]
        public void ProgramFeatures_ToIncompatibilityFlagsWithnULLFeatures_ThrowsNullReferenceException()
        {
            ProgramFeatures features = null;

            Assert.Throws<NullReferenceException>(() => features.ToIncompatibilityFlags());
        }

        [Fact]
        public void ProgramFeatures_ToIncompatibilityFlagsWhenNoIncompatibilitiesDefined_ReturnsNone()
        {
            var features = new ProgramFeaturesBuilder()
                .WithNtscCompatibility(FeatureCompatibility.Tolerates)
                .WithPalCompatibility(FeatureCompatibility.Tolerates)
                .WithKeyboardComponentFeatures(KeyboardComponentFeatures.Tolerates)
                .WithSuperVideoArcadeCompatibility(FeatureCompatibility.Tolerates)
                .WithIntellivoiceCompatibility(FeatureCompatibility.Tolerates)
                .WithIntellivisionIICompatibility(FeatureCompatibility.Tolerates)
                .WithEcsFeatures(EcsFeatures.Tolerates)
                .WithTutorvisionCompatibility(FeatureCompatibility.Tolerates)
                .WithIntellicartFeatures(IntellicartCC3Features.Tolerates)
                .WithCuttleCart3Features(CuttleCart3Features.Tolerates)
                .WithJlpFeatures(JlpFeatures.Tolerates)
                .WithLtoFlashFeatures(LtoFlashFeatures.Tolerates)
                .WithBee3Features(Bee3Features.Tolerates)
                .WithHiveFeatures(HiveFeatures.Tolerates)
                .Build() as ProgramFeatures;

            var incompatibilityFlags = features.ToIncompatibilityFlags();

            Assert.Equal(IncompatibilityFlags.None, incompatibilityFlags);
        }

        [Fact]
        public void ProgramFeatures_ToIncompatibilityFlagsWithEnhancedFeatures_ReturnsExpected()
        {
            var features = new ProgramFeaturesBuilder()
                .WithNtscCompatibility(FeatureCompatibility.Enhances)
                .WithPalCompatibility(FeatureCompatibility.Enhances)
                .WithKeyboardComponentFeatures(KeyboardComponentFeatures.Enhances)
                .WithSuperVideoArcadeCompatibility(FeatureCompatibility.Enhances)
                .WithIntellivoiceCompatibility(FeatureCompatibility.Enhances)
                .WithIntellivisionIICompatibility(FeatureCompatibility.Enhances)
                .WithEcsFeatures(EcsFeatures.Enhances)
                .WithTutorvisionCompatibility(FeatureCompatibility.Enhances)
                .WithIntellicartFeatures(IntellicartCC3Features.Enhances)
                .WithCuttleCart3Features(CuttleCart3Features.Enhances)
                .WithJlpFeatures(JlpFeatures.Enhances)
                .WithLtoFlashFeatures(LtoFlashFeatures.Enhances)
                .WithBee3Features(Bee3Features.Enhances)
                .WithHiveFeatures(HiveFeatures.Enhances)
                .Build() as ProgramFeatures;

            var incompatibilityFlags = features.ToIncompatibilityFlags();

            Assert.Equal(IncompatibilityFlags.SuperVideoArcadeAltered | IncompatibilityFlags.IntellivisionIIAltered, incompatibilityFlags);
        }

        [Fact]
        public void ProgramFeatures_ToIncompatibilityFlagsWithEmptyFeatures_ReturnsAll()
        {
            var features = ProgramFeatures.EmptyFeatures;

            var incompatibilityFlags = features.ToIncompatibilityFlags();

            var allIncompatibiltyFlags = Enum.GetValues(typeof(IncompatibilityFlags)).Cast<IncompatibilityFlags>().Except(new[] { IncompatibilityFlags.IntellivisionIIAltered, IncompatibilityFlags.SuperVideoArcadeAltered }).Aggregate((all, flag) => all | flag);
            Assert.Equal(allIncompatibiltyFlags, incompatibilityFlags);
        }

        [Fact]
        public void ProgramFeatures_ToIncompatibilityFlagsWithDefaultFeatures_ReturnsExpected()
        {
            var features = ProgramFeatures.DefaultFeatures;

            var incompatibilityFlags = features.ToIncompatibilityFlags();

            var expectedIncompatibilityFlags = IncompatibilityFlags.Jlp | IncompatibilityFlags.LtoFlash | IncompatibilityFlags.Bee3 | IncompatibilityFlags.Hive;
            Assert.Equal(expectedIncompatibilityFlags, incompatibilityFlags);
        }

        [Fact]
        public void ProgramFeatures_SetUnrecongizedRomFeatures_SetsFeaturesForUnrecognizedRom()
        {
            var features = ProgramFeatures.EmptyFeatures;

            features.SetUnrecongizedRomFeatures();

            Assert.True(features.GeneralFeatures.HasFlag(GeneralFeatures.UnrecognizedRom));
            Assert.Equal(FeatureCompatibility.Enhances, features.Ntsc);
            Assert.Equal(FeatureCompatibility.Enhances, features.Pal);
        }

        [Fact]
        public void ProgramFeatures_ClearUnrecongizedRomFeatures_ClearsFeaturesForUnrecognizedRom()
        {
            var features = ProgramFeatures.GetUnrecognizedRomFeatures();

            features.ClearUnrecongizedRomFeatures();

            Assert.False(features.GeneralFeatures.HasFlag(GeneralFeatures.UnrecognizedRom));
            Assert.Equal(FeatureCompatibility.Tolerates, features.Ntsc);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Pal);
        }
    }
}

// <copyright file="IncompatibilityFlagsHelpersTests.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class IncompatibilityFlagsHelpersTests
    {
        public static IEnumerable<object[]> IncompatibilityFlagsAsProgramFeaturesTestData
        {
            get
            {
                foreach (var incompatibilityFlag in Enum.GetValues(typeof(IncompatibilityFlags)).Cast<IncompatibilityFlags>())
                {
                    // Relying on the builder working as expected since we have unit tests for the builder elsewhere.
                    var builder = new ProgramFeaturesBuilder().WithInitialFeatures(new ProgramFeatures());
                    switch (incompatibilityFlag)
                    {
                        case IncompatibilityFlags.None:
                            break;
                        case IncompatibilityFlags.Ntsc:
                            builder.WithNtscCompatibility(FeatureCompatibility.Incompatible);
                            break;
                        case IncompatibilityFlags.Pal:
                            builder.WithPalCompatibility(FeatureCompatibility.Incompatible);
                            break;
                        case IncompatibilityFlags.KeyboardComponent:
                            builder.WithKeyboardComponentFeatures(KeyboardComponentFeatures.Incompatible);
                            break;
                        case IncompatibilityFlags.SuperVideoArcade:
                            builder.WithSuperVideoArcadeCompatibility(FeatureCompatibility.Incompatible);
                            break;
                        case IncompatibilityFlags.SuperVideoArcadeAltered:
                            builder.WithSuperVideoArcadeCompatibility(FeatureCompatibility.Enhances);
                            break;
                        case IncompatibilityFlags.Intellivoice:
                            builder.WithIntellivoiceCompatibility(FeatureCompatibility.Incompatible);
                            break;
                        case IncompatibilityFlags.IntellivisionII:
                            builder.WithIntellivisionIICompatibility(FeatureCompatibility.Incompatible);
                            break;
                        case IncompatibilityFlags.IntellivisionIIAltered:
                            builder.WithIntellivisionIICompatibility(FeatureCompatibility.Enhances);
                            break;
                        case IncompatibilityFlags.Ecs:
                            builder.WithEcsFeatures(EcsFeatures.Incompatible);
                            break;
                        case IncompatibilityFlags.Tutorvision:
                            builder.WithTutorvisionCompatibility(FeatureCompatibility.Incompatible);
                            break;
                        case IncompatibilityFlags.Intellicart:
                            builder.WithIntellicartFeatures(IntellicartCC3Features.Incompatible);
                            break;
                        case IncompatibilityFlags.CuttleCart3:
                            builder.WithCuttleCart3Features(CuttleCart3Features.Incompatible);
                            break;
                        case IncompatibilityFlags.Jlp:
                            builder.WithJlpFeatures(JlpFeatures.Incompatible);
                            break;
                        case IncompatibilityFlags.LtoFlash:
                            builder.WithLtoFlashFeatures(LtoFlashFeatures.Incompatible);
                            break;
                        case IncompatibilityFlags.Bee3:
                            builder.WithBee3Features(Bee3Features.Incompatible);
                            break;
                        case IncompatibilityFlags.Hive:
                            builder.WithHiveFeatures(HiveFeatures.Incompatible);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    yield return new object[] { incompatibilityFlag, builder.Build() };
                }
            }
        }

        [Theory]
        [MemberData("IncompatibilityFlagsAsProgramFeaturesTestData")]
        public void IncompatibilityFlags_ToProgramFeatures_ProducesExpectedProgramFeatures(IncompatibilityFlags incompatibilityFlags, IProgramFeatures expectedFeatures)
        {
            var featuresWithIncompatibility = incompatibilityFlags.ToProgramFeatures();

            Assert.Equal(expectedFeatures, featuresWithIncompatibility);
        }
    }
}

// <copyright file="LuigiFeatureFlagsHelpersTests.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiFeatureFlagsHelpersTests
    {
        public static IEnumerable<object[]> LuigiFeatureFlagsToProgramFeaturesTestData
        {
            get
            {
                for (var i = 0; i < sizeof(LuigiFeatureFlags) * 8; ++i)
                {
                    var featureFlags = (LuigiFeatureFlags)(1ul << i);
                    var expectedProgramFeatures = CreateProgramFeaturesWithoutCoreCompatibility(featureFlags);
                    switch ((ulong)featureFlags)
                    {
                        case 1ul << 0:
                            expectedProgramFeatures.Intellivoice = FeatureCompatibility.Tolerates;
                            break;
                        case 1ul << 1:
                            expectedProgramFeatures.Intellivoice = FeatureCompatibility.Enhances;
                            break;
                        case 1ul << 2:
                            expectedProgramFeatures.Ecs = EcsFeatures.Tolerates;
                            break;
                        case 1ul << 3:
                            expectedProgramFeatures.Ecs = EcsFeatures.Enhances;
                            break;
                        case 1ul << 4:
                            expectedProgramFeatures.IntellivisionII = FeatureCompatibility.Tolerates;
                            break;
                        case 1ul << 5:
                            expectedProgramFeatures.IntellivisionII = FeatureCompatibility.Enhances;
                            break;
                        case 1ul << 6:
                            expectedProgramFeatures.KeyboardComponent = KeyboardComponentFeatures.Tolerates;
                            break;
                        case 1ul << 7:
                            expectedProgramFeatures.KeyboardComponent = KeyboardComponentFeatures.Enhances;
                            break;
                        case 1ul << 8: // Extended feature bits version -- so far version 1 only includes Tutorvision, which, unset, means incompatible
                        case 1ul << 9: // Extended feature bits version -- so far version 1 only includes Tutorvision, which, unset, means incompatible
                            expectedProgramFeatures.Tutorvision = FeatureCompatibility.Incompatible;
                            break;
                        case 1ul << 10: // Tutorvision Tolerates -- ignored because no extended feature bits version is set
                        case 1ul << 11: // Tutorvision Tolerates -- ignored because no extended feature bits version is set
                            break;
                        case 1ul << 16:
                            expectedProgramFeatures.Jlp = JlpFeatures.Tolerates;
                            expectedProgramFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp03;
                            break;
                        case 1ul << 17:
                            expectedProgramFeatures.Jlp = JlpFeatures.Enhances;
                            expectedProgramFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp03;
                            break;
                        case 1ul << 22: // JLP flash data sector usage
                        case 1ul << 23: // JLP flash data sector usage
                        case 1ul << 24: // JLP flash data sector usage
                        case 1ul << 25: // JLP flash data sector usage
                        case 1ul << 26: // JLP flash data sector usage
                        case 1ul << 27: // JLP flash data sector usage
                        case 1ul << 28: // JLP flash data sector usage
                        case 1ul << 29: // JLP flash data sector usage
                        case 1ul << 30: // JLP flash data sector usage
                        case 1ul << 31: // JLP flash data sector usage
                            expectedProgramFeatures.Jlp = JlpFeatures.SaveDataRequired;
                            expectedProgramFeatures.JlpFlashMinimumSaveSectors = (ushort)(1 << (i - 22));
                            expectedProgramFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp03;
                            break;
                        case 1ul << 32:
                            expectedProgramFeatures.LtoFlash = LtoFlashFeatures.LtoFlashMemoryMapped;
                            break;
                        default:
                            break;
                    }
                    yield return new object[] { featureFlags, expectedProgramFeatures };
                }

                yield return new object[] { LuigiFeatureFlags.None, CreateProgramFeaturesWithoutCoreCompatibility(LuigiFeatureFlags.None) };
            }
        }

        [Theory]
        [MemberData("LuigiFeatureFlagsToProgramFeaturesTestData")]
        public void LuigiFeatureFlags_ToProgramFeatures_ProducesExpectedProgramFeatures(LuigiFeatureFlags featureFlags, ProgramFeatures expectedProgramFeatures)
        {
            Assert.Equal(expectedProgramFeatures, featureFlags.ToProgramFeatures());
        }

        [Fact]
        public void LuigiFeatureFlags_FeatureFlagsWithMaximumVersionToProgramFeatures_ProduceCorrectlyFormedProgramFeatures()
        {
            var flagsToTest = LuigiFeatureFlags.ExtendedPeripheralCompatibilityVersionMask; // Initializes to version 3
            var expectedProgramFeatures = CreateProgramFeaturesWithoutCoreCompatibility(flagsToTest);

            var programFeatures = flagsToTest.ToProgramFeatures();

            Assert.Equal(expectedProgramFeatures, programFeatures);
        }

        [Fact]
        public void LuigiFeatureFlags_FeatureFlagsWithLtoMapperEnabledToProgramFeatures_ProduceCorrectlyFormedProgramFeatures()
        {
            var flagsToTest = LuigiFeatureFlags.LtoFlashMemoryMapperEnabled;
            var expectedProgramFeatures = CreateProgramFeaturesWithoutCoreCompatibility(flagsToTest);
            expectedProgramFeatures.LtoFlash |= LtoFlashFeatures.LtoFlashMemoryMapped;

            var programFeatures = flagsToTest.ToProgramFeatures();

            Assert.Equal(expectedProgramFeatures, programFeatures);
        }

        [Theory]
        [InlineData((LuigiFeatureFlags)((int)FeatureCompatibility.Incompatible << LuigiFeatureFlagsHelpers.JlpAccelerationOffset), JlpHardwareVersion.None, JlpFeatures.Incompatible)]
        [InlineData((LuigiFeatureFlags)((int)FeatureCompatibility.Tolerates << LuigiFeatureFlagsHelpers.JlpAccelerationOffset), JlpHardwareVersion.Jlp03, JlpFeatures.Tolerates)]
        [InlineData((LuigiFeatureFlags)((int)FeatureCompatibility.Enhances << LuigiFeatureFlagsHelpers.JlpAccelerationOffset), JlpHardwareVersion.Jlp03, JlpFeatures.Enhances)]
        [InlineData((LuigiFeatureFlags)((int)FeatureCompatibility.Requires << LuigiFeatureFlagsHelpers.JlpAccelerationOffset), JlpHardwareVersion.Jlp03, JlpFeatures.Requires)]
        public void LuigiFeatureFlags_LuigiFeatureFlagsForJlpToProgramFeatures_ProduceCorrectlyFormedProgramFeaturesJlpFields(LuigiFeatureFlags flagsToTest, JlpHardwareVersion expectedJlpHardwareVersion, JlpFeatures expectedJlpFeatures)
        {
            var expectedProgramFeatures = CreateProgramFeaturesWithoutCoreCompatibility(flagsToTest);
            expectedProgramFeatures.JlpHardwareVersion = expectedJlpHardwareVersion;
            expectedProgramFeatures.Jlp = expectedJlpFeatures;

            var programFeatures = flagsToTest.ToProgramFeatures();

            Assert.Equal(expectedProgramFeatures, programFeatures);
        }

        [Theory]
        [InlineData((ushort)0, LuigiFeatureFlags.None)]
        [InlineData((ushort)128, (LuigiFeatureFlags)(128 << LuigiFeatureFlagsHelpers.JlpFlashMinimumSaveDataSectorsCountOffset))]
        [InlineData((ushort)0x4FF, (LuigiFeatureFlags)(255 << LuigiFeatureFlagsHelpers.JlpFlashMinimumSaveDataSectorsCountOffset))]
        [InlineData((ushort)0x400, LuigiFeatureFlags.None)]
        public void LuigiFeatureFlags_JlpFlashSectorsToFlags_ProducesCorrectFlags(ushort sectorCount, LuigiFeatureFlags expectedFeatureFlags)
        {
            var featureFlags = sectorCount.MinimumFlashSaveDataSectorsCountToLuigiFeatureFlags();

            Assert.Equal(expectedFeatureFlags, featureFlags);
        }

        private static ProgramFeatures CreateProgramFeaturesWithoutCoreCompatibility(LuigiFeatureFlags featuresUnderTest)
        {
            var programFeaturesHavingNoLuigiFeatureFlags = new ProgramFeatures()
            {
                Ecs = EcsFeatures.Incompatible,
                IntellivisionII = FeatureCompatibility.Incompatible,
                Intellivoice = FeatureCompatibility.Incompatible,
                KeyboardComponent = KeyboardComponentFeatures.Incompatible,
            };
            if (featuresUnderTest.ExtendedPeripheralCompatabilityBitsVersion() > 0)
            {
                programFeaturesHavingNoLuigiFeatureFlags.Tutorvision = FeatureCompatibility.Incompatible;
            }
            programFeaturesHavingNoLuigiFeatureFlags.SetUnrecongizedRomFeatures();

            return programFeaturesHavingNoLuigiFeatureFlags;
        }
    }
}

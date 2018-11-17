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

using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiFeatureFlagsHelpersTests
    {
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

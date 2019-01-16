// <copyright file="ProgramFeaturesTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class ProgramFeaturesTests
    {
        private const LuigiFeatureFlags DefaultLuigiFeatureFlags =
            (LuigiFeatureFlags)((ulong)FeatureCompatibility.Tolerates << LuigiFeatureFlagsHelpers.IntellivoiceOffset)
            | (LuigiFeatureFlags)((ulong)EcsFeaturesHelpers.Default << LuigiFeatureFlagsHelpers.EcsOffset)
            | (LuigiFeatureFlags)((ulong)FeatureCompatibility.Tolerates << LuigiFeatureFlagsHelpers.IntellivisionIIOffset)
            | (LuigiFeatureFlags)((ulong)KeyboardComponentFeaturesHelpers.Default << LuigiFeatureFlagsHelpers.KeyboardComponentOffset);

        private static readonly IEnumerable<FeatureCategory> FeatureCategoriesToExclude = new[]
            {
                FeatureCategory.NumberOfCategories,
                FeatureCategory.EcsLegacy,
                FeatureCategory.IntellivoiceLegacy,
                FeatureCategory.IntellivisionIILegacy,
                FeatureCategory.JlpFlashCapacity,
                FeatureCategory.None
            };

        private static readonly IEnumerable<FeatureCategory> FeatureCategories = Enum.GetValues(typeof(FeatureCategory)).Cast<FeatureCategory>();

        [Fact]
        public void ProgramFeatures_DefaultConstructor_InitializesCorrectly()
        {
            var features = new ProgramFeatures();

            Assert.Equal(FeatureCompatibility.Tolerates, features.Ntsc);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Pal);
            Assert.Equal(GeneralFeatures.None, features.GeneralFeatures);
            Assert.Equal(KeyboardComponentFeaturesHelpers.Default, features.KeyboardComponent);
            Assert.Equal(FeatureCompatibility.Tolerates, features.SuperVideoArcade);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Intellivoice);
            Assert.Equal(FeatureCompatibility.Tolerates, features.IntellivisionII);
            Assert.Equal(EcsFeaturesHelpers.Default, features.Ecs);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Tutorvision);
            Assert.Equal(IntellicartCC3FeaturesHelpers.Default, features.Intellicart);
            Assert.Equal(CuttleCart3FeaturesHelpers.Default, features.CuttleCart3);
            Assert.Equal(JlpFeaturesHelpers.Default, features.Jlp);
            Assert.Equal(LtoFlashFeaturesHelpers.Default, features.LtoFlash);
            Assert.Equal(Bee3FeaturesHelpers.Default, features.Bee3);
            Assert.Equal(HiveFeaturesHelpers.Default, features.Hive);
            Assert.Equal((ushort)0, features.JlpFlashMinimumSaveSectors);
            Assert.Equal(JlpHardwareVersion.None, features.JlpHardwareVersion);
            Assert.Equal(DefaultLuigiFeatureFlags, features.LuigiFeaturesLo);
            Assert.Equal(LuigiFeatureFlags2.None, features.LuigiFeaturesHi);
        }

        [Fact]
        public void ProgramFeatures_EmptyFeatures_HasCorrectValues()
        {
            var features = ProgramFeatures.EmptyFeatures;

            Assert.Equal(FeatureCompatibility.Incompatible, features.Ntsc);
            Assert.Equal(FeatureCompatibility.Incompatible, features.Pal);
            Assert.Equal(GeneralFeatures.None, features.GeneralFeatures);
            Assert.Equal(KeyboardComponentFeatures.Incompatible, features.KeyboardComponent);
            Assert.Equal(FeatureCompatibility.Incompatible, features.SuperVideoArcade);
            Assert.Equal(FeatureCompatibility.Incompatible, features.Intellivoice);
            Assert.Equal(FeatureCompatibility.Incompatible, features.IntellivisionII);
            Assert.Equal(EcsFeatures.Incompatible, features.Ecs);
            Assert.Equal(FeatureCompatibility.Incompatible, features.Tutorvision);
            Assert.Equal(IntellicartCC3Features.Incompatible, features.Intellicart);
            Assert.Equal(CuttleCart3Features.Incompatible, features.CuttleCart3);
            Assert.Equal(JlpFeatures.Incompatible, features.Jlp);
            Assert.Equal(LtoFlashFeatures.Incompatible, features.LtoFlash);
            Assert.Equal(Bee3Features.Incompatible, features.Bee3);
            Assert.Equal(HiveFeatures.Incompatible, features.Hive);
            Assert.Equal((ushort)0, features.JlpFlashMinimumSaveSectors);
            Assert.Equal(JlpHardwareVersion.None, features.JlpHardwareVersion);
            Assert.Equal(LuigiFeatureFlags.None, features.LuigiFeaturesLo);
            Assert.Equal(LuigiFeatureFlags2.None, features.LuigiFeaturesHi);
        }

        [Fact]
        public void ProgramFeatures_DefaultFeatures_HasCorrectValues()
        {
            var features = ProgramFeatures.DefaultFeatures;

            Assert.Equal(FeatureCompatibility.Tolerates, features.Ntsc);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Pal);
            Assert.Equal(GeneralFeatures.None, features.GeneralFeatures);
            Assert.Equal(KeyboardComponentFeaturesHelpers.Default, features.KeyboardComponent);
            Assert.Equal(FeatureCompatibility.Tolerates, features.SuperVideoArcade);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Intellivoice);
            Assert.Equal(FeatureCompatibility.Tolerates, features.IntellivisionII);
            Assert.Equal(EcsFeaturesHelpers.Default, features.Ecs);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Tutorvision);
            Assert.Equal(IntellicartCC3FeaturesHelpers.Default, features.Intellicart);
            Assert.Equal(CuttleCart3FeaturesHelpers.Default, features.CuttleCart3);
            Assert.Equal(JlpFeaturesHelpers.Default, features.Jlp);
            Assert.Equal(LtoFlashFeaturesHelpers.Default, features.LtoFlash);
            Assert.Equal(Bee3FeaturesHelpers.Default, features.Bee3);
            Assert.Equal(HiveFeaturesHelpers.Default, features.Hive);
            Assert.Equal((ushort)0, features.JlpFlashMinimumSaveSectors);
            Assert.Equal(JlpHardwareVersion.None, features.JlpHardwareVersion);
            Assert.Equal(DefaultLuigiFeatureFlags, features.LuigiFeaturesLo);
            Assert.Equal(LuigiFeatureFlags2.None, features.LuigiFeaturesHi);
        }

        [Fact]
        public void ProgramFeatures_GetUnrecognizedRomFeatures_HasCorrectValues()
        {
            var features = ProgramFeatures.GetUnrecognizedRomFeatures();

            Assert.Equal(FeatureCompatibility.Enhances, features.Ntsc);
            Assert.Equal(FeatureCompatibility.Enhances, features.Pal);
            Assert.Equal(GeneralFeatures.UnrecognizedRom, features.GeneralFeatures);
            Assert.Equal(KeyboardComponentFeaturesHelpers.Default, features.KeyboardComponent);
            Assert.Equal(FeatureCompatibility.Tolerates, features.SuperVideoArcade);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Intellivoice);
            Assert.Equal(FeatureCompatibility.Tolerates, features.IntellivisionII);
            Assert.Equal(EcsFeaturesHelpers.Default, features.Ecs);
            Assert.Equal(FeatureCompatibility.Tolerates, features.Tutorvision);
            Assert.Equal(IntellicartCC3FeaturesHelpers.Default, features.Intellicart);
            Assert.Equal(CuttleCart3FeaturesHelpers.Default, features.CuttleCart3);
            Assert.Equal(JlpFeaturesHelpers.Default, features.Jlp);
            Assert.Equal(LtoFlashFeaturesHelpers.Default, features.LtoFlash);
            Assert.Equal(Bee3FeaturesHelpers.Default, features.Bee3);
            Assert.Equal(HiveFeaturesHelpers.Default, features.Hive);
            Assert.Equal((ushort)0, features.JlpFlashMinimumSaveSectors);
            Assert.Equal(JlpHardwareVersion.None, features.JlpHardwareVersion);
            Assert.Equal(DefaultLuigiFeatureFlags, features.LuigiFeaturesLo);
            Assert.Equal(LuigiFeatureFlags2.None, features.LuigiFeaturesHi);
        }

        [Fact]
        public void ProgramFeatures_ModifyEmptyFeatures_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Ntsc = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Pal = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.GeneralFeatures = GeneralFeatures.SystemRom);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.SuperVideoArcade = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Intellivoice = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.IntellivisionII = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Ecs = EcsFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.KeyboardComponent = KeyboardComponentFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Tutorvision = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Intellicart = IntellicartCC3Features.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.CuttleCart3 = CuttleCart3Features.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Jlp = JlpFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp05);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.JlpFlashMinimumSaveSectors = 8);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.LtoFlash = LtoFlashFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Bee3 = Bee3Features.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.EmptyFeatures.Hive = HiveFeatures.Requires);
        }

        [Fact]
        public void ProgramFeatures_ModifyDefaultFeatures_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Ntsc = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Pal = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.GeneralFeatures = GeneralFeatures.SystemRom);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.SuperVideoArcade = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Intellivoice = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.IntellivisionII = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Ecs = EcsFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.KeyboardComponent = KeyboardComponentFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Tutorvision = FeatureCompatibility.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Intellicart = IntellicartCC3Features.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.CuttleCart3 = CuttleCart3Features.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Jlp = JlpFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.JlpHardwareVersion = JlpHardwareVersion.Jlp05);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.JlpFlashMinimumSaveSectors = 8);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.LtoFlash = LtoFlashFeatures.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Bee3 = Bee3Features.Requires);
            Assert.Throws<InvalidOperationException>(() => ProgramFeatures.DefaultFeatures.Hive = HiveFeatures.Requires);
        }

        [Fact]
        public void ProgramFeatures_OperatorEqualsTwoNullInstances_ReturnsTrue()
        {
            ProgramFeatures lhs = null;
            ProgramFeatures rhs = null;

            Assert.True(lhs == rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorEqualsRightHandSideNull_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = null;

            Assert.False(lhs == rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorEqualsLeftHandSideNull_ReturnsFalse()
        {
            ProgramFeatures lhs = null;
            ProgramFeatures rhs = ProgramFeatures.DefaultFeatures;

            Assert.False(lhs == rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorEqualsSameInstance_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = lhs;

            Assert.True(lhs == rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorEqualsTwoSameInstances_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = ProgramFeatures.DefaultFeatures.Clone();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.True(lhs == rhs);
            Assert.True(rhs == lhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorEqualsTwoDifferentInstances_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = ProgramFeatures.EmptyFeatures;

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.False(lhs == rhs);
            Assert.False(rhs == lhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorNotEqualsTwoNullInstances_ReturnsFalse()
        {
            ProgramFeatures lhs = null;
            ProgramFeatures rhs = null;

            Assert.False(lhs != rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorNotEqualsRightHandSideNull_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = null;

            Assert.True(lhs != rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorNotEqualsLeftHandSideNull_ReturnsTrue()
        {
            ProgramFeatures lhs = null;
            ProgramFeatures rhs = ProgramFeatures.DefaultFeatures;

            Assert.True(lhs != rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorNotEqualsSameInstance_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = lhs;

            Assert.False(lhs != rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorNotEqualsTwoSameInstances_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = ProgramFeatures.DefaultFeatures.Clone();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.False(lhs != rhs);
        }

        [Fact]
        public void ProgramFeatures_OperatorNotEqualsTwoDifferentInstances_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = ProgramFeatures.EmptyFeatures;

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.True(lhs != rhs);
        }

        [Fact]
        public void ProgramFeatures_EqualsNullobject_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            object rhs = null;

            Assert.False(lhs.Equals(rhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsNullProgramFeatures_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = null;

            Assert.False(lhs.Equals(rhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsNonProgramFeatures_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            var rhs = new object();

            Assert.False(lhs.Equals(rhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsSameInstance_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = lhs;

            Assert.True(lhs.Equals(rhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsTwoSameInstances_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = ProgramFeatures.DefaultFeatures.Clone();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.True(lhs.Equals(rhs));
            Assert.True(rhs.Equals(lhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsTwoSameInstancesAsObject_ReturnsTrue()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            object rhs = ProgramFeatures.DefaultFeatures.Clone();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.True(lhs.Equals(rhs));
            Assert.True(rhs.Equals(lhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsTwoDifferentInstances_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            ProgramFeatures rhs = ProgramFeatures.EmptyFeatures;

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.False(lhs.Equals(rhs));
            Assert.False(rhs.Equals(lhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsTwoDifferentInstancesAsObject_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            object rhs = ProgramFeatures.EmptyFeatures;

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.False(lhs.Equals(rhs));
            Assert.False(rhs.Equals(lhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsInstanceOfAnotherImplementation_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            var rhs = new TestProgramFeatures();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.False(lhs.Equals(rhs));
            Assert.False(rhs.Equals(lhs));
        }

        [Fact]
        public void ProgramFeatures_EqualsInstanceOfAnotherImplementationAsObject_ReturnsFalse()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            object rhs = new TestProgramFeatures();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.False(lhs.Equals(rhs));
            Assert.False(rhs.Equals(lhs));
        }

        [Fact]
        public void ProgramFeatures_CompareToObjectNull_ReturnsNonZero()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            object rhs = null;

            Assert.NotEqual(0, lhs.CompareTo(rhs));
        }

        [Fact]
        public void ProgramFeatures_CompareToNonIProgramFeaturesObject_ThrowsArgumentException()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            var rhs = new object();

            Assert.Throws<ArgumentException>(() => lhs.CompareTo(rhs));
        }

        [Fact]
        public void ProgramFeatures_CompareToEquivalentProgramFeaturesAsObject_ReturnsZero()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            object rhs = ProgramFeatures.DefaultFeatures.Clone();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.Equal(0, lhs.CompareTo(rhs));
        }

        [Fact]
        public void ProgramFeatures_CompareToDifferentProgramFeaturesAsObject_ReturnsNonzero()
        {
            ProgramFeatures lhs = ProgramFeatures.DefaultFeatures;
            var rhs = new TestProgramFeatures();

            Assert.False(object.ReferenceEquals(lhs, rhs));
            Assert.NotEqual(0, lhs.CompareTo(rhs));
        }

        [Fact]
        public void ProgramFeatures_CombineTwoNullFeatures_ReturnsUnrecognizedRomFeatures()
        {
            var combinedFeatures = ProgramFeatures.Combine(null, null);

            Assert.True(ProgramFeatures.GetUnrecognizedRomFeatures() == combinedFeatures);
        }

        [Fact]
        public void ProgramFeatures_CombineWithNullFeatures_ReturnsCloneOfNonNullFeatures()
        {
            var features = new ProgramFeatures() { KeyboardComponent = KeyboardComponentFeatures.Enhances };
            var combinedFeatures = ProgramFeatures.Combine(features, null);

            Assert.False(object.ReferenceEquals(features, combinedFeatures));
            Assert.True(features == combinedFeatures);
        }

        [Fact]
        public void ProgramFeatures_CombineNullWithFeatures_ReturnsCloneOfNonNullFeatures()
        {
            var features = new ProgramFeatures() { KeyboardComponent = KeyboardComponentFeatures.Enhances };
            var combinedFeatures = ProgramFeatures.Combine(null, features);

            Assert.False(object.ReferenceEquals(features, combinedFeatures));
            Assert.True(features == combinedFeatures);
        }

        [Fact]
        public void ProgramFeatures_CombineNtscFeatures_ReturnsExpectedFeature()
        {
            var features0 = new ProgramFeatures() { Ntsc = FeatureCompatibility.Tolerates };
            var features1 = new ProgramFeatures() { Ntsc = FeatureCompatibility.Enhances };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(FeatureCompatibility.Enhances, combinedFeatures.Ntsc);
        }

        [Fact]
        public void ProgramFeatures_CombinePalFeatures_ReturnsExpectedFeature()
        {
            var features0 = new ProgramFeatures() { Pal = FeatureCompatibility.Tolerates };
            var features1 = new ProgramFeatures() { Pal = FeatureCompatibility.Enhances };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(FeatureCompatibility.Enhances, combinedFeatures.Pal);
        }

        [Fact]
        public void ProgramFeatures_CombineGeneralFeatures_ReturnsExpectedFeature()
        {
            var features0 = new ProgramFeatures() { GeneralFeatures = GeneralFeatures.OnboardRam };
            var features1 = new ProgramFeatures() { GeneralFeatures = GeneralFeatures.SystemRom };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(GeneralFeatures.OnboardRam | GeneralFeatures.SystemRom, combinedFeatures.GeneralFeatures);
        }

        [Theory]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances, FeatureCompatibility.Enhances)]
        [InlineData(FeatureCompatibility.Enhances, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        public void ProgramFeatures_CombineSuperVideoArcadeFeatures_ReturnsExpectedFeature(FeatureCompatibility value0, FeatureCompatibility value1, FeatureCompatibility expectedValue)
        {
            var features0 = new ProgramFeatures() { SuperVideoArcade = value0 };
            var features1 = new ProgramFeatures() { SuperVideoArcade = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.SuperVideoArcade);
        }

        [Theory]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances, FeatureCompatibility.Enhances)]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        [InlineData(FeatureCompatibility.Incompatible, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        public void ProgramFeatures_CombineIntellivoiceFeatures_ReturnsExpectedFeature(FeatureCompatibility value0, FeatureCompatibility value1, FeatureCompatibility expectedValue)
        {
            var features0 = new ProgramFeatures() { Intellivoice = value0 };
            var features1 = new ProgramFeatures() { Intellivoice = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Intellivoice);
        }

        [Theory]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances, FeatureCompatibility.Enhances)]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        [InlineData(FeatureCompatibility.Incompatible, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        public void ProgramFeatures_CombineIntellivisionIIFeatures_ReturnsExpectedFeature(FeatureCompatibility value0, FeatureCompatibility value1, FeatureCompatibility expectedValue)
        {
            var features0 = new ProgramFeatures() { IntellivisionII = value0 };
            var features1 = new ProgramFeatures() { IntellivisionII = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.IntellivisionII);
        }

        [Theory]
        [InlineData(EcsFeatures.Tolerates, EcsFeatures.Enhances, EcsFeatures.Enhances)]
        [InlineData(EcsFeatures.Tolerates, EcsFeatures.Requires, EcsFeatures.Requires)]
        [InlineData(EcsFeatures.Incompatible, EcsFeatures.Requires, EcsFeatures.Requires)]
        [InlineData(EcsFeatures.Tolerates | EcsFeatures.Printer, EcsFeatures.Requires, EcsFeatures.Requires | EcsFeatures.Printer)]
        public void ProgramFeatures_CombineEcsFeatures_ReturnsExpectedFeature(EcsFeatures value0, EcsFeatures value1, EcsFeatures expectedValue)
        {
            var features0 = new ProgramFeatures() { Ecs = value0 };
            var features1 = new ProgramFeatures() { Ecs = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Ecs);
        }

        [Theory]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances, FeatureCompatibility.Enhances)]
        [InlineData(FeatureCompatibility.Tolerates, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        [InlineData(FeatureCompatibility.Incompatible, FeatureCompatibility.Requires, FeatureCompatibility.Requires)]
        public void ProgramFeatures_CombineTutorvisionFeatures_ReturnsExpectedFeature(FeatureCompatibility value0, FeatureCompatibility value1, FeatureCompatibility expectedValue)
        {
            var features0 = new ProgramFeatures() { Tutorvision = value0 };
            var features1 = new ProgramFeatures() { Tutorvision = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Tutorvision);
        }

        [Theory]
        [InlineData(KeyboardComponentFeatures.Tolerates, KeyboardComponentFeatures.Enhances, KeyboardComponentFeatures.Enhances)]
        [InlineData(KeyboardComponentFeatures.Tolerates, KeyboardComponentFeatures.Requires, KeyboardComponentFeatures.Requires)]
        [InlineData(KeyboardComponentFeatures.Incompatible, KeyboardComponentFeatures.Requires, KeyboardComponentFeatures.Requires)]
        [InlineData(KeyboardComponentFeatures.Tolerates | KeyboardComponentFeatures.Microphone, KeyboardComponentFeatures.Requires, KeyboardComponentFeatures.Requires | KeyboardComponentFeatures.Microphone)]
        public void ProgramFeatures_CombineKeyboardComponentFeatures_ReturnsExpectedFeature(KeyboardComponentFeatures value0, KeyboardComponentFeatures value1, KeyboardComponentFeatures expectedValue)
        {
            var features0 = new ProgramFeatures() { KeyboardComponent = value0 };
            var features1 = new ProgramFeatures() { KeyboardComponent = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.KeyboardComponent);
        }

        [Theory]
        [InlineData(IntellicartCC3Features.Tolerates, IntellicartCC3Features.Enhances, IntellicartCC3Features.Enhances)]
        [InlineData(IntellicartCC3Features.Tolerates, IntellicartCC3Features.Requires, IntellicartCC3Features.Requires)]
        [InlineData(IntellicartCC3Features.Incompatible, IntellicartCC3Features.Requires, IntellicartCC3Features.Requires)]
        [InlineData(IntellicartCC3Features.Tolerates | IntellicartCC3Features.SixteenBitRAM, IntellicartCC3Features.Requires, IntellicartCC3Features.Requires | IntellicartCC3Features.SixteenBitRAM)]
        public void ProgramFeatures_CombineIntellicartFeatures_ReturnsExpectedFeature(IntellicartCC3Features value0, IntellicartCC3Features value1, IntellicartCC3Features expectedValue)
        {
            var features0 = new ProgramFeatures() { Intellicart = value0 };
            var features1 = new ProgramFeatures() { Intellicart = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Intellicart);
        }

        [Theory]
        [InlineData(CuttleCart3Features.Tolerates, CuttleCart3Features.Enhances, CuttleCart3Features.Enhances)]
        [InlineData(CuttleCart3Features.Tolerates, CuttleCart3Features.Requires, CuttleCart3Features.Requires)]
        [InlineData(CuttleCart3Features.Incompatible, CuttleCart3Features.Requires, CuttleCart3Features.Requires)]
        [InlineData(CuttleCart3Features.Tolerates | CuttleCart3Features.MattelBankswitching, CuttleCart3Features.Requires, CuttleCart3Features.Requires | CuttleCart3Features.MattelBankswitching)]
        public void ProgramFeatures_CombineCuttleCart3Features_ReturnsExpectedFeature(CuttleCart3Features value0, CuttleCart3Features value1, CuttleCart3Features expectedValue)
        {
            var features0 = new ProgramFeatures() { CuttleCart3 = value0 };
            var features1 = new ProgramFeatures() { CuttleCart3 = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.CuttleCart3);
        }

        [Theory]
        [InlineData(JlpFeatures.Tolerates, JlpFeatures.Enhances, JlpFeatures.Enhances)]
        [InlineData(JlpFeatures.Tolerates, JlpFeatures.Requires, JlpFeatures.Requires)]
        [InlineData(JlpFeatures.Incompatible, JlpFeatures.Requires, JlpFeatures.Requires)]
        [InlineData(JlpFeatures.Tolerates | JlpFeatures.SaveDataRequired, JlpFeatures.Requires, JlpFeatures.Requires | JlpFeatures.SaveDataRequired)]
        public void ProgramFeatures_CombineJlpFeatures_ReturnsExpectedFeature(JlpFeatures value0, JlpFeatures value1, JlpFeatures expectedValue)
        {
            var features0 = new ProgramFeatures() { Jlp = value0 };
            var features1 = new ProgramFeatures() { Jlp = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Jlp);
        }

        [Fact]
        public void ProgramFeatures_CombineJlpHardwareVersionFeatures_ReturnsExpectedFeature()
        {
            var features0 = new ProgramFeatures() { JlpHardwareVersion = JlpHardwareVersion.Jlp03 };
            var features1 = new ProgramFeatures() { JlpHardwareVersion = JlpHardwareVersion.Jlp04 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(JlpHardwareVersion.Jlp04, combinedFeatures.JlpHardwareVersion);
        }

        [Theory]
        [InlineData(128, 512, 640)]
        [InlineData(8, 1024, 8)]
        [InlineData(7, 3, 7)]
        [InlineData(2048, 0, 0)]
        public void ProgramFeatures_CombineJlpFlashSectorsFeatures_ReturnsExpectedFeature(ushort value0, ushort value1, ushort expectedValue)
        {
            var features0 = new ProgramFeatures() { JlpFlashMinimumSaveSectors = value0 };
            var features1 = new ProgramFeatures() { JlpFlashMinimumSaveSectors = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.JlpFlashMinimumSaveSectors);
        }

        [Theory]
        [InlineData(LtoFlashFeatures.Tolerates, LtoFlashFeatures.Enhances, LtoFlashFeatures.Enhances)]
        [InlineData(LtoFlashFeatures.Tolerates, JlpFeatures.Requires, LtoFlashFeatures.Requires)]
        [InlineData(LtoFlashFeatures.Incompatible, LtoFlashFeatures.Requires, LtoFlashFeatures.Requires)]
        [InlineData(LtoFlashFeatures.Tolerates | LtoFlashFeatures.LtoFlashMemoryMapped, LtoFlashFeatures.Requires, LtoFlashFeatures.Requires | LtoFlashFeatures.LtoFlashMemoryMapped)]
        public void ProgramFeatures_CombineLtoFlashFeatures_ReturnsExpectedFeature(LtoFlashFeatures value0, LtoFlashFeatures value1, LtoFlashFeatures expectedValue)
        {
            var features0 = new ProgramFeatures() { LtoFlash = value0 };
            var features1 = new ProgramFeatures() { LtoFlash = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.LtoFlash);
        }

        [Theory]
        [InlineData(Bee3Features.Tolerates, Bee3Features.Enhances, Bee3Features.Enhances)]
        [InlineData(Bee3Features.Tolerates, Bee3Features.Requires, Bee3Features.Requires)]
        [InlineData(Bee3Features.Incompatible, Bee3Features.Requires, Bee3Features.Requires)]
        [InlineData(Bee3Features.Tolerates | Bee3Features.SaveDataOptional, Bee3Features.Requires, Bee3Features.Requires | Bee3Features.SaveDataOptional)]
        public void ProgramFeatures_CombineLtoFlashFeatures_ReturnsExpectedFeature(Bee3Features value0, Bee3Features value1, Bee3Features expectedValue)
        {
            var features0 = new ProgramFeatures() { Bee3 = value0 };
            var features1 = new ProgramFeatures() { Bee3 = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Bee3);
        }

        [Theory]
        [InlineData(HiveFeatures.Tolerates, HiveFeatures.Enhances, HiveFeatures.Enhances)]
        [InlineData(HiveFeatures.Tolerates, HiveFeatures.Requires, HiveFeatures.Requires)]
        [InlineData(HiveFeatures.Incompatible, HiveFeatures.Requires, HiveFeatures.Requires)]
        [InlineData(HiveFeatures.Tolerates | HiveFeatures.SixteenBitRAM, HiveFeatures.Requires, HiveFeatures.Requires | HiveFeatures.SixteenBitRAM)]
        public void ProgramFeatures_CombineLtoFlashFeatures_ReturnsExpectedFeature(HiveFeatures value0, HiveFeatures value1, HiveFeatures expectedValue)
        {
            var features0 = new ProgramFeatures() { Hive = value0 };
            var features1 = new ProgramFeatures() { Hive = value1 };

            var combinedFeatures = ProgramFeatures.Combine(features0, features1);

            Assert.Equal(expectedValue, combinedFeatures.Hive);
        }

        public static IEnumerable<object[]> UpdateFeatureBitsTestData
        {
            get
            {
                foreach (var featureCategory in FeatureCategories.Except(FeatureCategoriesToExclude))
                {
                    var featuresBuilder = new ProgramFeaturesBuilder();
                    var featureBitsToAdd = 0u;

                    switch (featureCategory)
                    {
                        case FeatureCategory.Ntsc:
                            featuresBuilder.WithNtscCompatibility(FeatureCompatibility.Incompatible);
                            featureBitsToAdd = (uint)FeatureCompatibility.Requires;
                            break;
                        case FeatureCategory.Pal:
                            featuresBuilder.WithPalCompatibility(FeatureCompatibility.Incompatible);
                            featureBitsToAdd = (uint)FeatureCompatibility.Requires;
                            break;
                        case FeatureCategory.General:
                            featuresBuilder.WithGeneralFeatures(GeneralFeatures.None);
                            featureBitsToAdd = (uint)(GeneralFeatures.OnboardRam | GeneralFeatures.SystemRom | GeneralFeatures.PageFlipping);
                            break;
                        case FeatureCategory.KeyboardComponent:
                            featuresBuilder.WithKeyboardComponentFeatures(KeyboardComponentFeatures.Incompatible);
                            featureBitsToAdd = (uint)(KeyboardComponentFeatures.Requires | KeyboardComponentFeatures.BasicRequired | KeyboardComponentFeatures.Microphone | KeyboardComponentFeatures.TapeRequired);
                            break;
                        case FeatureCategory.SuperVideoArcade:
                            featuresBuilder.WithSuperVideoArcadeCompatibility(FeatureCompatibility.Incompatible);
                            featureBitsToAdd = (uint)FeatureCompatibility.Requires;
                            break;
                        case FeatureCategory.Intellivoice:
                            featuresBuilder.WithIntellivoiceCompatibility(FeatureCompatibility.Incompatible);
                            featureBitsToAdd = (uint)FeatureCompatibility.Requires;
                            break;
                        case FeatureCategory.IntellivisionII:
                            featuresBuilder.WithIntellivisionIICompatibility(FeatureCompatibility.Incompatible);
                            featureBitsToAdd = (uint)FeatureCompatibility.Requires;
                            break;
                        case FeatureCategory.Ecs:
                            featuresBuilder.WithEcsFeatures(EcsFeatures.Incompatible);
                            featureBitsToAdd = (uint)(EcsFeatures.Requires | EcsFeatures.Tape | EcsFeatures.Synthesizer);
                            break;
                        case FeatureCategory.Tutorvision:
                            featuresBuilder.WithTutorvisionCompatibility(FeatureCompatibility.Incompatible);
                            featureBitsToAdd = (uint)FeatureCompatibility.Requires;
                            break;
                        case FeatureCategory.Intellicart:
                            featuresBuilder.WithIntellicartFeatures(IntellicartCC3Features.Incompatible);
                            featureBitsToAdd = (uint)(IntellicartCC3Features.Requires | IntellicartCC3Features.Bankswitching);
                            break;
                        case FeatureCategory.CuttleCart3:
                            featuresBuilder.WithCuttleCart3Features(CuttleCart3Features.Incompatible);
                            featureBitsToAdd = (uint)(CuttleCart3Features.Requires | CuttleCart3Features.SerialPortRequired);
                            break;
                        case FeatureCategory.Jlp:
                            featuresBuilder.WithJlpFeatures(JlpFeatures.Incompatible);
                            featureBitsToAdd = (uint)(JlpFeatures.Requires | JlpFeatures.FsdBit0 | JlpFeatures.SaveDataRequired);
                            break;
                        case FeatureCategory.LtoFlash:
                            featuresBuilder.WithLtoFlashFeatures(LtoFlashFeatures.Incompatible);
                            featureBitsToAdd = (uint)(LtoFlashFeatures.LtoFlashMemoryMapped | LtoFlashFeatures.Requires);
                            break;
                        case FeatureCategory.Bee3:
                            featuresBuilder.WithBee3Features(Bee3Features.Incompatible);
                            featureBitsToAdd = (uint)(Bee3Features.Requires | Bee3Features.SixteenBitRAM);
                            break;
                        case FeatureCategory.Hive:
                            featuresBuilder.WithHiveFeatures(HiveFeatures.Incompatible);
                            featureBitsToAdd = (uint)(HiveFeatures.Requires | HiveFeatures.SixteenBitRAM | HiveFeatures.SaveDataRequired);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    yield return new object[] { (ProgramFeatures)featuresBuilder.Build(), featureCategory, featureBitsToAdd, featureBitsToAdd };
                }
            }
        }

        [Theory]
        [MemberData("UpdateFeatureBitsTestData")]
        public void ProgramFeatures_UpdateFeatureBits_UpdatesFeaturesCorrectly(ProgramFeatures features, FeatureCategory featureCategory, uint featureBitsToAdd, uint expectedFeatureBits)
        {
            features.UpdateFeatureBits(featureCategory, featureBitsToAdd, addFeatures: true);

            Assert.Equal(expectedFeatureBits, GetFeatureBits(features, featureCategory));
        }

        public static IEnumerable<object[]> UpdateFeatureBitsToZeroTestData
        {
            get
            {
                foreach (var featureCategory in FeatureCategories.Except(FeatureCategoriesToExclude))
                {
                    var featuresBuilder = new ProgramFeaturesBuilder();
                    var expectedFeatureBits = 0u;

                    switch (featureCategory)
                    {
                        case FeatureCategory.Ntsc:
                            featuresBuilder.WithNtscCompatibility(FeatureCompatibility.Enhances);
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.Pal:
                            featuresBuilder.WithPalCompatibility(FeatureCompatibility.Enhances);
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.General:
                            featuresBuilder.WithGeneralFeatures(GeneralFeatures.SystemRom | GeneralFeatures.UnrecognizedRom);
                            expectedFeatureBits = (uint)(GeneralFeatures.SystemRom | GeneralFeatures.UnrecognizedRom);
                            break;
                        case FeatureCategory.KeyboardComponent:
                            featuresBuilder.WithKeyboardComponentFeatures(KeyboardComponentFeatures.Requires | KeyboardComponentFeatures.BasicRequired | KeyboardComponentFeatures.Microphone | KeyboardComponentFeatures.TapeRequired);
                            expectedFeatureBits = (uint)KeyboardComponentFeatures.Incompatible;
                            break;
                        case FeatureCategory.SuperVideoArcade:
                            featuresBuilder.WithSuperVideoArcadeCompatibility(FeatureCompatibility.Requires);
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.Intellivoice:
                            featuresBuilder.WithIntellivoiceCompatibility(FeatureCompatibility.Requires);
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.IntellivisionII:
                            featuresBuilder.WithIntellivisionIICompatibility(FeatureCompatibility.Requires);
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.Ecs:
                            featuresBuilder.WithEcsFeatures(EcsFeatures.Requires | EcsFeatures.Tape | EcsFeatures.Synthesizer);
                            expectedFeatureBits = (uint)EcsFeatures.Incompatible;
                            break;
                        case FeatureCategory.Tutorvision:
                            featuresBuilder.WithTutorvisionCompatibility(FeatureCompatibility.Requires);
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.Intellicart:
                            featuresBuilder.WithIntellicartFeatures(IntellicartCC3Features.Requires | IntellicartCC3Features.Bankswitching);
                            expectedFeatureBits = (uint)IntellicartCC3Features.Incompatible;
                            break;
                        case FeatureCategory.CuttleCart3:
                            featuresBuilder.WithCuttleCart3Features(CuttleCart3Features.Requires | CuttleCart3Features.SerialPortRequired);
                            expectedFeatureBits = (uint)CuttleCart3Features.Incompatible;
                            break;
                        case FeatureCategory.Jlp:
                            featuresBuilder.WithJlpFeatures(JlpFeatures.Requires | JlpFeatures.FsdBit0 | JlpFeatures.SaveDataRequired);
                            expectedFeatureBits = (uint)(JlpFeatures.Requires | JlpFeatures.FsdBit0 | JlpFeatures.SaveDataRequired);
                            break;
                        case FeatureCategory.LtoFlash:
                            featuresBuilder.WithLtoFlashFeatures(LtoFlashFeatures.LtoFlashMemoryMapped | LtoFlashFeatures.Requires);
                            expectedFeatureBits = (uint)(LtoFlashFeatures.LtoFlashMemoryMapped | LtoFlashFeatures.Requires);
                            break;
                        case FeatureCategory.Bee3:
                            featuresBuilder.WithBee3Features(Bee3Features.Requires | Bee3Features.SixteenBitRAM);
                            expectedFeatureBits = (uint)(Bee3Features.Requires | Bee3Features.SixteenBitRAM);
                            break;
                        case FeatureCategory.Hive:
                            featuresBuilder.WithHiveFeatures(HiveFeatures.Requires | HiveFeatures.SixteenBitRAM | HiveFeatures.SaveDataRequired);
                            expectedFeatureBits = (uint)(HiveFeatures.Requires | HiveFeatures.SixteenBitRAM | HiveFeatures.SaveDataRequired);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    yield return new object[] { (ProgramFeatures)featuresBuilder.Build(), featureCategory, expectedFeatureBits };
                }
            }
        }

        [Theory]
        [MemberData("UpdateFeatureBitsToZeroTestData")]
        public void ProgramFeatures_UpdateFeatureBitsToZero_UpdatesFeaturesCorrectly(ProgramFeatures features, FeatureCategory featureCategory, uint expectedFeatureBits)
        {
            features.UpdateFeatureBits(featureCategory, 0u, addFeatures: true);

            Assert.Equal(expectedFeatureBits, GetFeatureBits(features, featureCategory));
        }

        public static IEnumerable<object[]> RemoveFeatureBitsTestData
        {
            get
            {
                foreach (var featureCategory in FeatureCategories.Except(FeatureCategoriesToExclude))
                {
                    var featuresBuilder = new ProgramFeaturesBuilder();
                    var expectedFeatureBits = 0u;
                    var featureBitsToRemove = 0u;

                    switch (featureCategory)
                    {
                        case FeatureCategory.Ntsc:
                            featuresBuilder.WithNtscCompatibility(FeatureCompatibility.Enhances);
                            featureBitsToRemove = (uint)FeatureCompatibility.Enhances;
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.Pal:
                            featuresBuilder.WithPalCompatibility(FeatureCompatibility.Enhances);
                            featureBitsToRemove = (uint)FeatureCompatibility.Enhances;
                            expectedFeatureBits = (uint)FeatureCompatibility.Incompatible;
                            break;
                        case FeatureCategory.General:
                            featuresBuilder.WithGeneralFeatures(GeneralFeatures.SystemRom | GeneralFeatures.UnrecognizedRom);
                            featureBitsToRemove = (uint)GeneralFeatures.SystemRom;
                            expectedFeatureBits = (uint)GeneralFeatures.UnrecognizedRom;
                            break;
                        case FeatureCategory.KeyboardComponent:
                            featuresBuilder.WithKeyboardComponentFeatures(KeyboardComponentFeatures.Requires | KeyboardComponentFeatures.BasicRequired | KeyboardComponentFeatures.Microphone | KeyboardComponentFeatures.TapeRequired);
                            featureBitsToRemove = (uint)KeyboardComponentFeatures.BasicRequired;
                            expectedFeatureBits = (uint)(KeyboardComponentFeatures.Requires | KeyboardComponentFeatures.Microphone | KeyboardComponentFeatures.TapeRequired);
                            break;
                        case FeatureCategory.SuperVideoArcade:
                            featuresBuilder.WithSuperVideoArcadeCompatibility(FeatureCompatibility.Requires);
                            featureBitsToRemove = (uint)FeatureCompatibility.Enhances;
                            expectedFeatureBits = (uint)FeatureCompatibility.Tolerates;
                            break;
                        case FeatureCategory.Intellivoice:
                            featuresBuilder.WithIntellivoiceCompatibility(FeatureCompatibility.Requires);
                            featureBitsToRemove = (uint)FeatureCompatibility.Enhances;
                            expectedFeatureBits = (uint)FeatureCompatibility.Tolerates;
                            break;
                        case FeatureCategory.IntellivisionII:
                            featuresBuilder.WithIntellivisionIICompatibility(FeatureCompatibility.Requires);
                            featureBitsToRemove = (uint)FeatureCompatibility.Enhances;
                            expectedFeatureBits = (uint)FeatureCompatibility.Tolerates;
                            break;
                        case FeatureCategory.Ecs:
                            featuresBuilder.WithEcsFeatures(EcsFeatures.Requires | EcsFeatures.Tape | EcsFeatures.Synthesizer);
                            featureBitsToRemove = (uint)EcsFeatures.Tape;
                            expectedFeatureBits = (uint)(EcsFeatures.Requires | EcsFeatures.Synthesizer);
                            break;
                        case FeatureCategory.Tutorvision:
                            featuresBuilder.WithTutorvisionCompatibility(FeatureCompatibility.Requires);
                            featureBitsToRemove = (uint)FeatureCompatibility.Enhances;
                            expectedFeatureBits = (uint)FeatureCompatibility.Tolerates;
                            break;
                        case FeatureCategory.Intellicart:
                            featuresBuilder.WithIntellicartFeatures(IntellicartCC3Features.Requires | IntellicartCC3Features.Bankswitching);
                            featureBitsToRemove = (uint)IntellicartCC3Features.Bankswitching;
                            expectedFeatureBits = (uint)IntellicartCC3Features.Requires;
                            break;
                        case FeatureCategory.CuttleCart3:
                            featuresBuilder.WithCuttleCart3Features(CuttleCart3Features.Requires | CuttleCart3Features.SerialPortRequired);
                            featureBitsToRemove = (uint)CuttleCart3Features.SerialPortRequired;
                            expectedFeatureBits = (uint)CuttleCart3Features.Requires;
                            break;
                        case FeatureCategory.Jlp:
                            featuresBuilder.WithJlpFeatures(JlpFeatures.Requires | JlpFeatures.FsdBit0 | JlpFeatures.FsdBit1 | JlpFeatures.SaveDataRequired);
                            featureBitsToRemove = (uint)JlpFeatures.FsdBit1;
                            expectedFeatureBits = (uint)(JlpFeatures.Requires | JlpFeatures.FsdBit0 | JlpFeatures.SaveDataRequired);
                            break;
                        case FeatureCategory.LtoFlash:
                            featuresBuilder.WithLtoFlashFeatures(LtoFlashFeatures.LtoFlashMemoryMapped | LtoFlashFeatures.Requires);
                            featureBitsToRemove = (uint)LtoFlashFeatures.LtoFlashMemoryMapped;
                            expectedFeatureBits = (uint)LtoFlashFeatures.Requires;
                            break;
                        case FeatureCategory.Bee3:
                            featuresBuilder.WithBee3Features(Bee3Features.Requires | Bee3Features.SixteenBitRAM);
                            featureBitsToRemove = (uint)Bee3Features.SixteenBitRAM;
                            expectedFeatureBits = (uint)Bee3Features.Requires;
                            break;
                        case FeatureCategory.Hive:
                            featuresBuilder.WithHiveFeatures(HiveFeatures.Requires | HiveFeatures.SixteenBitRAM | HiveFeatures.SaveDataRequired);
                            featureBitsToRemove = (uint)HiveFeatures.SixteenBitRAM;
                            expectedFeatureBits = (uint)(HiveFeatures.Requires | HiveFeatures.SaveDataRequired);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    yield return new object[] { (ProgramFeatures)featuresBuilder.Build(), featureCategory, featureBitsToRemove, expectedFeatureBits };
                }
            }
        }

        [Theory]
        [MemberData("RemoveFeatureBitsTestData")]
        public void ProgramFeatures_RmoveFeatureBits_UpdatesFeaturesCorrectly(ProgramFeatures features, FeatureCategory featureCategory, uint featureBitsToRemove, uint expectedFeatureBits)
        {
            features.UpdateFeatureBits(featureCategory, featureBitsToRemove, addFeatures: false);

            Assert.Equal(expectedFeatureBits, GetFeatureBits(features, featureCategory));
        }

        public static IEnumerable<object[]> GetHashCodeTestData
        {
            get
            {
                foreach (var featureCategory in FeatureCategories.Except(FeatureCategoriesToExclude))
                {
                    var featuresBuilder = new ProgramFeaturesBuilder();

                    switch (featureCategory)
                    {
                        case FeatureCategory.Ntsc:
                            featuresBuilder.WithNtscCompatibility(FeatureCompatibility.Enhances);
                            break;
                        case FeatureCategory.Pal:
                            featuresBuilder.WithPalCompatibility(FeatureCompatibility.Enhances);
                            break;
                        case FeatureCategory.General:
                            featuresBuilder.WithGeneralFeatures(GeneralFeatures.SystemRom | GeneralFeatures.UnrecognizedRom);
                            break;
                        case FeatureCategory.KeyboardComponent:
                            featuresBuilder.WithKeyboardComponentFeatures(KeyboardComponentFeatures.Requires | KeyboardComponentFeatures.BasicRequired | KeyboardComponentFeatures.Microphone | KeyboardComponentFeatures.TapeRequired);
                            break;
                        case FeatureCategory.SuperVideoArcade:
                            featuresBuilder.WithSuperVideoArcadeCompatibility(FeatureCompatibility.Requires);
                            break;
                        case FeatureCategory.Intellivoice:
                            featuresBuilder.WithIntellivoiceCompatibility(FeatureCompatibility.Requires);
                            break;
                        case FeatureCategory.IntellivisionII:
                            featuresBuilder.WithIntellivisionIICompatibility(FeatureCompatibility.Requires);
                            break;
                        case FeatureCategory.Ecs:
                            featuresBuilder.WithEcsFeatures(EcsFeatures.Requires | EcsFeatures.Tape | EcsFeatures.Synthesizer);
                            break;
                        case FeatureCategory.Tutorvision:
                            featuresBuilder.WithTutorvisionCompatibility(FeatureCompatibility.Requires);
                            break;
                        case FeatureCategory.Intellicart:
                            featuresBuilder.WithIntellicartFeatures(IntellicartCC3Features.Requires | IntellicartCC3Features.Bankswitching);
                            break;
                        case FeatureCategory.CuttleCart3:
                            featuresBuilder.WithCuttleCart3Features(CuttleCart3Features.Requires | CuttleCart3Features.SerialPortRequired);
                            break;
                        case FeatureCategory.Jlp:
                            featuresBuilder.WithJlpFeatures(JlpFeatures.Requires | JlpFeatures.FsdBit0 | JlpFeatures.FsdBit1 | JlpFeatures.SaveDataRequired);
                            break;
                        case FeatureCategory.LtoFlash:
                            featuresBuilder.WithLtoFlashFeatures(LtoFlashFeatures.LtoFlashMemoryMapped | LtoFlashFeatures.Requires);
                            break;
                        case FeatureCategory.Bee3:
                            featuresBuilder.WithBee3Features(Bee3Features.Requires | Bee3Features.SixteenBitRAM);
                            break;
                        case FeatureCategory.Hive:
                            featuresBuilder.WithHiveFeatures(HiveFeatures.Requires | HiveFeatures.SixteenBitRAM | HiveFeatures.SaveDataRequired);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    yield return new object[] { (ProgramFeatures)featuresBuilder.Build() };
                }
            }
        }

        [Theory]
        [MemberData("GetHashCodeTestData")]
        public void ProgramFeatures_GetHashCode_ComputesExpectedValue(ProgramFeatures features)
        {
            var hashCode = features.GetHashCode();

            var expectedHashCode = 0;
            foreach (var featureCategory in FeatureCategories.Except(FeatureCategoriesToExclude))
            {
                expectedHashCode = CombineHashCodes(expectedHashCode, featureCategory.GetHashCode());
                expectedHashCode = CombineHashCodes(expectedHashCode, GetFeatureBits(features, featureCategory).GetHashCode());
            }
            Assert.Equal(expectedHashCode, hashCode);
        }

        private static uint GetFeatureBits(IProgramFeatures features, FeatureCategory category)
        {
            var featureBits = 0u;
            switch (category)
            {
                case FeatureCategory.Ntsc:
                    featureBits = (uint)features.Ntsc;
                    break;
                case FeatureCategory.Pal:
                    featureBits = (uint)features.Pal;
                    break;
                case FeatureCategory.General:
                    featureBits = (uint)features.GeneralFeatures;
                    break;
                case FeatureCategory.KeyboardComponent:
                    featureBits = (uint)features.KeyboardComponent;
                    break;
                case FeatureCategory.SuperVideoArcade:
                    featureBits = (uint)features.SuperVideoArcade;
                    break;
                case FeatureCategory.Intellivoice:
                    featureBits = (uint)features.Intellivoice;
                    break;
                case FeatureCategory.IntellivisionII:
                    featureBits = (uint)features.IntellivisionII;
                    break;
                case FeatureCategory.Ecs:
                    featureBits = (uint)features.Ecs;
                    break;
                case FeatureCategory.Tutorvision:
                    featureBits = (uint)features.Tutorvision;
                    break;
                case FeatureCategory.Intellicart:
                    featureBits = (uint)features.Intellicart;
                    break;
                case FeatureCategory.CuttleCart3:
                    featureBits = (uint)features.CuttleCart3;
                    break;
                case FeatureCategory.Jlp:
                    featureBits = (uint)features.Jlp;
                    break;
                case FeatureCategory.LtoFlash:
                    featureBits = (uint)features.LtoFlash;
                    break;
                case FeatureCategory.Bee3:
                    featureBits = (uint)features.Bee3;
                    break;
                case FeatureCategory.Hive:
                    featureBits = (uint)features.Hive;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return featureBits;
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private class TestProgramFeatures : IProgramFeatures
        {
            public FeatureCompatibility Ntsc
            {
                get { return FeatureCompatibility.Tolerates; }
            }

            public FeatureCompatibility Pal
            {
                get { return FeatureCompatibility.Tolerates; }
            }

            public GeneralFeatures GeneralFeatures
            {
                get { return GeneralFeatures.None; }
            }

            public KeyboardComponentFeatures KeyboardComponent
            {
                get { return KeyboardComponentFeatures.Tolerates; }
            }

            public FeatureCompatibility SuperVideoArcade
            {
                get { return FeatureCompatibility.Tolerates; }
            }

            public FeatureCompatibility Intellivoice
            {
                get { return FeatureCompatibility.Tolerates; }
            }

            public FeatureCompatibility IntellivisionII
            {
                get { return FeatureCompatibility.Tolerates; }
            }

            public EcsFeatures Ecs
            {
                get { return EcsFeatures.Tolerates; }
            }

            public FeatureCompatibility Tutorvision
            {
                get { return FeatureCompatibility.Tolerates; }
            }

            public IntellicartCC3Features Intellicart
            {
                get { return IntellicartCC3Features.Tolerates; }
            }

            public CuttleCart3Features CuttleCart3
            {
                get { return CuttleCart3Features.Tolerates; }
            }

            public JlpFeatures Jlp
            {
                get { return JlpFeatures.Tolerates; }
            }

            public JlpHardwareVersion JlpHardwareVersion
            {
                get { return Core.Model.JlpHardwareVersion.None; }
            }

            public ushort JlpFlashMinimumSaveSectors
            {
                get { return 0; }
            }

            public LtoFlashFeatures LtoFlash
            {
                get { return LtoFlashFeatures.Tolerates; }
            }

            public Bee3Features Bee3
            {
                get { return Bee3Features.Incompatible; }
            }

            public HiveFeatures Hive
            {
                get { return HiveFeatures.Incompatible; }
            }

            public LuigiFeatureFlags LuigiFeaturesLo
            {
                get { return LuigiFeatureFlags.None; }
            }

            public LuigiFeatureFlags2 LuigiFeaturesHi
            {
                get { return LuigiFeatureFlags2.None; }
            }

            public int CompareTo(object obj)
            {
                return -1;
            }

            public int CompareTo(IProgramFeatures other)
            {
                return 1;
            }
        }
    }
}

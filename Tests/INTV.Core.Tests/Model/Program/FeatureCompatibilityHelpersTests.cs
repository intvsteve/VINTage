// <copyright file="FeatureCompatibilityHelpersTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class FeatureCompatibilityHelpersTests
    {
        [Theory]
        [InlineData(FeatureCategory.None)]
        [InlineData(FeatureCategory.Ntsc)]
        [InlineData(FeatureCategory.Pal)]
        [InlineData(FeatureCategory.General)]
        [InlineData(FeatureCategory.SuperVideoArcade)]
        [InlineData(FeatureCategory.Intellicart)]
        [InlineData(FeatureCategory.CuttleCart3)]
        [InlineData(FeatureCategory.Bee3)]
        [InlineData(FeatureCategory.Hive)]
        [InlineData(FeatureCategory.NumberOfCategories)]
        [InlineData(FeatureCategory.EcsLegacy)]
        [InlineData(FeatureCategory.IntellivisionIILegacy)]
        [InlineData(FeatureCategory.IntellivoiceLegacy)]
        [InlineData(FeatureCategory.JlpFlashCapacity)]
        public void FeatureCompatibility_UnsupportedFeatureCategoryToLuigiFeatureFlags_ThrowsInvalidOperationException(FeatureCategory category)
        {
            var featureCompatibility = FeatureCompatibility.Incompatible;

            Assert.Throws<InvalidOperationException>(() => featureCompatibility.ToLuigiFeatureFlags(category));
        }

        [Theory]
        [InlineData(FeatureCompatibility.Incompatible, LuigiFeatureFlags.None)]
        [InlineData(FeatureCompatibility.Tolerates, (LuigiFeatureFlags)(1u << 6))]
        [InlineData(FeatureCompatibility.Enhances, (LuigiFeatureFlags)(2u << 6))]
        [InlineData(FeatureCompatibility.Requires, (LuigiFeatureFlags)(3u << 6))]
        public void FeatureCompatibility_KeyboardComponentFeaturesCompatibilityToLuigiFeatureFlags_ProducesCorrectLuigiFeatureFlags(FeatureCompatibility compatibility, LuigiFeatureFlags expectedFeatureFlags)
        {
            Assert.Equal(expectedFeatureFlags, compatibility.ToLuigiFeatureFlags(FeatureCategory.KeyboardComponent));
        }

        [Theory]
        [InlineData(FeatureCompatibility.Incompatible, (LuigiFeatureFlags)(1u << 8))]
        [InlineData(FeatureCompatibility.Tolerates, (LuigiFeatureFlags)((1u << 10) | (1u << 8)))]
        [InlineData(FeatureCompatibility.Enhances, (LuigiFeatureFlags)((2u << 10) | (1u << 8)))]
        [InlineData(FeatureCompatibility.Requires, (LuigiFeatureFlags)((3u << 10) | (1u << 8)))]
        public void FeatureCompatibility_TutorvisionFeaturesCompatibilityToLuigiFeatureFlags_ProducesCorrectLuigiFeatureFlags(FeatureCompatibility compatibility, LuigiFeatureFlags expectedFeatureFlags)
        {
            Assert.Equal(expectedFeatureFlags, compatibility.ToLuigiFeatureFlags(FeatureCategory.Tutorvision));
        }
    }
}

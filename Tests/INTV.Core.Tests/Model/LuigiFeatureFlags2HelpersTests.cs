﻿// <copyright file="LuigiFeatureFlags2HelpersTests.cs" company="INTV Funhouse">
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
    public class LuigiFeatureFlags2HelpersTests
    {
        public static IEnumerable<object[]> LuigiFeatureFlags2ToProgramFeaturesTestData
        {
            get
            {
                yield return new object[] { LuigiFeatureFlags2.None };
                for (var i = 0; i < sizeof(LuigiFeatureFlags2) * 8; ++i)
                {
                    yield return new object[] { (LuigiFeatureFlags2)(1ul << i) };
                }
                yield return new object[] { LuigiFeatureFlags2.UnusedMask };
            }
        }

        [Theory]
        [MemberData("LuigiFeatureFlags2ToProgramFeaturesTestData")]
        public void LuigiFeatureFlags2_ToProgramFeatures_ProducesCorrectFeatures(LuigiFeatureFlags2 featureBits)
        {
            Assert.Equal(ProgramFeatures.GetUnrecognizedRomFeatures(), featureBits.ToProgramFeatures());
        }
    }
}

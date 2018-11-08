// <copyright file="IRomHelpersTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class IRomHelpersTests
    {
        [Fact]
        public void IRomHelpers_GetProgramFeaturesWithNullRom_ThrowsNullReferenceException()
        {
            IRom rom = null;

            Assert.Throws<NullReferenceException>(() => rom.GetProgramFeatures());
        }

        [Fact]
        public void IRomHelpers_GetProgramFeaturesForNonLuigiFormatRom_ReturnsUnrecognizedProgramFeatures()
        {
            IRomHelpersTestStorageAccess.Initialize(TestRomResources.TestCc3Path);
            var rom = Rom.Create(TestRomResources.TestCc3Path, null);
            Assert.NotNull(rom);

            var features = rom.GetProgramFeatures();

            Assert.Equal(ProgramFeatures.GetUnrecognizedRomFeatures(), features);
        }

        [Theory]
        [InlineData(TestRomResources.TestLuigiWithMetadataPath)]
        [InlineData(TestRomResources.TestLuigiWithMetadatdaScrambledForAnyDevicePath)]
        public void IRomHelpers_GetProgramFeaturesForLuigiFormatRom_ReturnsExpectedProgramFeatures(string testLuigiRomPath)
        {
            IRomHelpersTestStorageAccess.Initialize(testLuigiRomPath);
            var rom = Rom.Create(testLuigiRomPath, null);
            Assert.NotNull(rom);

            var features = rom.GetProgramFeatures();

            Assert.NotEqual(ProgramFeatures.GetUnrecognizedRomFeatures(), features);
        }

        private class IRomHelpersTestStorageAccess : CachedResourceStorageAccess<IRomHelpersTestStorageAccess>
        {
        }
    }
}

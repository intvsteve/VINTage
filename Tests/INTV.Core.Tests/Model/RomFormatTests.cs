// <copyright file="RomFormatTests.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomFormatTests
    {
        [Theory]
        [InlineData(RomFormat.None, "")]
        [InlineData(RomFormat.Bin, ".bin")]
        [InlineData(RomFormat.Intellicart, ".rom")]
        [InlineData(RomFormat.Rom, ".rom")]
        [InlineData(RomFormat.CuttleCart3, ".cc3")]
        [InlineData(RomFormat.CuttleCart3Advanced, ".cc3")]
        [InlineData(RomFormat.Luigi, ".luigi")]
        public void RomFormat_GetFileExtension_ReturnsExpectedFileExtension(RomFormat romFormat, string expectedFileExtension)
        {
            var actualFileExtension = romFormat.FileExtension();

            Assert.Equal(expectedFileExtension, actualFileExtension);
        }

        public static IEnumerable<object[]> CompatibleRomFormatsData
        {
            get
            {
                var romFormats = Enum.GetValues(typeof(RomFormat)).Cast<RomFormat>();
                foreach (var firstRomFormat in romFormats)
                {
                    foreach (var secondRomFormat in romFormats)
                    {
                        var consideredCompatible = firstRomFormat == secondRomFormat;
                        if (!consideredCompatible)
                        {
                            var compatibleRange = new Range<int>((int)RomFormat.Intellicart, (int)RomFormat.CuttleCart3Advanced);
                            consideredCompatible = compatibleRange.IsValueInRange((int)firstRomFormat) && compatibleRange.IsValueInRange((int)secondRomFormat);
                        }
                        yield return new object[] { firstRomFormat, secondRomFormat, consideredCompatible };
                    }
                }
            }
        }

        [Theory]
        [MemberData("CompatibleRomFormatsData")]
        public void RomFormat_CheckRomFormatCompatibility_ExpectedCompatibilityDetermined(RomFormat firstRomFormat, RomFormat secondRomFormat, bool expectedCompatibility)
        {
            var actualCompatibility = firstRomFormat.IsCompatibleWithRomFormat(secondRomFormat);

            Assert.Equal(expectedCompatibility, actualCompatibility);
        }
    }
}

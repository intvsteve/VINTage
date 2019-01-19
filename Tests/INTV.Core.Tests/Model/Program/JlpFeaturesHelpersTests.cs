// <copyright file="JlpFeaturesHelpersTests.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class JlpFeaturesHelpersTests
    {
        [Theory]
        [InlineData((ushort)0, 0u)]
        [InlineData((ushort)5, 7680u)] // magic number (see JlpFeaturesHelpers) is 8 * 96 * 2 = 1536
        [InlineData((ushort)100, 153600u)]
        [InlineData(ushort.MaxValue, 100661760u)]
        public void JlpFeaturesHelpers_JlpFlashSectorsToBytes_ConvertsToBytesCorrectly(ushort jlpFlashSectors, uint expectedSizeInBytes)
        {
            var jlpFlashSectorsAsSizeInBytes = jlpFlashSectors.JlpFlashSectorsToBytes();
        }

        [Theory]
        [InlineData((ushort)0, 0f)]
        [InlineData((ushort)5, 7.5f)]
        [InlineData((ushort)100, 150f)]
        [InlineData(ushort.MaxValue, 98302.5f)]
        public void JlpFeaturesHelpers_JlpFlashSectorsToKBytes_ConvertsToBytesCorrectly(ushort jlpFlashSectors, float expectedSizeInKilobytes)
        {
            var jlpFlashSectorsAsSizeInKilobytes = jlpFlashSectors.JlpFlashSectorsToKBytes();

            Assert.Equal(expectedSizeInKilobytes, jlpFlashSectorsAsSizeInKilobytes, 2);
        }
    }
}

// <copyright file="DeviceStatusFlagsHiTests.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

using INTV.LtoFlash.Model;
using Xunit;

namespace INTV.LtoFlash.Tests.Model
{
    public class DeviceStatusFlagsHiTests
    {
        [Fact]
        public void DeviceStatusFlagsHi_GetHighBits_GetsExpectedHighBits()
        {
            var flags = DeviceStatusFlagsHi.Default;

            var expectedHighBits = (uint)(((ulong)0xFFFFFFFF00000000 & (ulong)flags) >> 32);

            Assert.Equal(expectedHighBits, flags.GetHighBits());
        }

        [Fact]
        public void DeviceStatusFlagsHi_GetLowBits_GetsExpectedLowBits()
        {
            var flags = DeviceStatusFlagsHi.Default;

            var expectedLowBits = (uint)((ulong)0x00000000FFFFFFFF & (ulong)flags);

            Assert.Equal(expectedLowBits, flags.GetLowBits());
        }
    }
}

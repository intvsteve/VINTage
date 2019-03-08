// <copyright file="DeviceStatusFlagsLoTests.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.LtoFlash.Model;
using Xunit;

namespace INTV.LtoFlash.Tests.Model
{
    public class DeviceStatusFlagsLoTests
    {
        public static IEnumerable<object[]> DeviceStatusFlagsLoToHardwareStatusFlagsTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)HardwareStatusFlags.None };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedHardwareFlags = HardwareStatusFlags.None;
                    if (i < sizeof(HardwareStatusFlags) * 8)
                    {
                        expectedHardwareFlags = (HardwareStatusFlags)(1u << i);
                    }
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedHardwareFlags };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToHardwareStatusFlagsTestData")]
        public void DeviceStatusFlagsLo_ToHardwareStatusFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, HardwareStatusFlags expectedHardwareStatusFlags)
        {
            var actualHardwareStatusFlags = statusFlagsLo.ToHardwareStatusFlags();

            Assert.Equal(expectedHardwareStatusFlags, actualHardwareStatusFlags);
        }

        public static IEnumerable<object[]> DeviceStatusFlagsLoToIntellivisionIIStatusFlagsTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)IntellivisionIIStatusFlags.None };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedIntellivisionIIFlags = IntellivisionIIStatusFlags.None;
                    if ((i >= DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset) && (i < DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset + (sizeof(IntellivisionIIStatusFlags) * 8)))
                    {
                        expectedIntellivisionIIFlags = (IntellivisionIIStatusFlags)(1u << (i - DeviceStatusFlagsLoHelpers.IntellivisionIIStatusBitsOffset));
                    }
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedIntellivisionIIFlags };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToIntellivisionIIStatusFlagsTestData")]
        public void DeviceStatusFlagsLo_ToIntellivisionIICompatibilityFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, IntellivisionIIStatusFlags expectedIntellivisionIIStatusFlags)
        {
            var actualIntellivisionIIStatusFlags = statusFlagsLo.ToIntellivisionIICompatibilityFlags();

            // NOTE: Implementation currently suppresses the reserved bits. Replicate this here.
            expectedIntellivisionIIStatusFlags &= ~IntellivisionIIStatusFlags.ReservedMask;
            Assert.Equal(expectedIntellivisionIIStatusFlags, actualIntellivisionIIStatusFlags);
        }
    }
}

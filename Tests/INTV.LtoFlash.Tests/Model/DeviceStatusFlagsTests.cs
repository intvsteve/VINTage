// <copyright file="DeviceStatusFlagsTests.cs" company="INTV Funhouse">
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
using INTV.LtoFlash.Model;
using Xunit;

namespace INTV.LtoFlash.Tests.Model
{
    public class DeviceStatusFlagsTests
    {
        public void DeviceStatusFlags_Constructor_CreatesAsExpected()
        {
            var flags = new DeviceStatusFlags();

            Assert.Equal(DeviceStatusFlagsLo.None, flags.Lo);
            Assert.Equal(DeviceStatusFlagsHi.None, flags.Hi);
        }

        public void DeviceStatusFlags_ConstructorFromDeviceStatusLo_CreatesAsExpected()
        {
            var lowTestFlags = DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive | DeviceStatusFlagsLo.EnableCartConfig;
            var flags = new DeviceStatusFlags(lowTestFlags);

            Assert.Equal(lowTestFlags, flags.Lo);
            Assert.Equal(DeviceStatusFlagsHi.None, flags.Hi);
        }

        public void DeviceStatusFlags_ConstructorFromDeviceStatusHi_CreatesAsExpected()
        {
            var highTestFlags = DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet;
            var flags = new DeviceStatusFlags(highTestFlags);

            Assert.Equal(DeviceStatusFlagsLo.None, flags.Lo);
            Assert.Equal(highTestFlags, flags.Hi);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, true)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        public void DeviceStatusFlags_HasFlagUsingFullDeviceFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lowFlags, DeviceStatusFlagsHi highFlags, DeviceStatusFlagsLo lowFlagsToTest, DeviceStatusFlagsHi highFlagsToTest, bool expectedHasFlagResult)
        {
            var flags = new DeviceStatusFlags(lowFlags, highFlags);
            var flagsToCheck = new DeviceStatusFlags(lowFlagsToTest, highFlagsToTest);

            var hasFlag = flags.HasFlag(flagsToCheck);

            Assert.Equal(expectedHasFlagResult, hasFlag);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.None)]
        public void DeviceStatusFlags_And_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsHi rhsHiFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);
            var rhsFlags = new DeviceStatusFlags(rhsLoFlags, rhsHiFlags);

            var resultFlags = lhsFlags & rhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None,  DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.None)]
        public void DeviceStatusFlags_AndLoFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var resultFlags0 = lhsFlags & rhsLoFlags;
            var resultFlags1 = rhsLoFlags & lhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags0);
            Assert.Equal(expectedFlags, resultFlags1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None,  DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        public void DeviceStatusFlags_AndHiFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsHi rhsHiFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var resultFlags0 = lhsFlags & rhsHiFlags;
            var resultFlags1 = rhsHiFlags & lhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags0);
            Assert.Equal(expectedFlags, resultFlags1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig | DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory)]
        public void DeviceStatusFlags_Or_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsHi rhsHiFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);
            var rhsFlags = new DeviceStatusFlags(rhsLoFlags, rhsHiFlags);

            var resultFlags = lhsFlags | rhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsLo.EcsStatusDisabled | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsLo.EnableCartConfig | DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory)]
        public void DeviceStatusFlags_OrLoFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var resultFlags0 = lhsFlags | rhsLoFlags;
            var resultFlags1 = rhsLoFlags | lhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags0);
            Assert.Equal(expectedFlags, resultFlags1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory)]
        public void DeviceStatusFlags_OrHiFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsHi rhsHiFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var resultFlags0 = lhsFlags | rhsHiFlags;
            var resultFlags1 = rhsHiFlags | lhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags0);
            Assert.Equal(expectedFlags, resultFlags1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig | DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory)]
        public void DeviceStatusFlags_Xor_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsHi rhsHiFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);
            var rhsFlags = new DeviceStatusFlags(rhsLoFlags, rhsHiFlags);

            var resultFlags = lhsFlags ^ rhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsLo.EcsStatusDisabled | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsLo.EnableCartConfig | DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory)]
        public void DeviceStatusFlags_XorLoFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var resultFlags0 = lhsFlags ^ rhsLoFlags;
            var resultFlags1 = rhsLoFlags ^ lhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags0);
            Assert.Equal(expectedFlags, resultFlags1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory)]
        public void DeviceStatusFlags_XorHiFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsHi rhsHiFlags, DeviceStatusFlagsLo resultLoFlags, DeviceStatusFlagsHi resultHiFlags)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var resultFlags0 = lhsFlags ^ rhsHiFlags;
            var resultFlags1 = rhsHiFlags ^ lhsFlags;

            var expectedFlags = new DeviceStatusFlags(resultLoFlags, resultHiFlags);
            Assert.Equal(expectedFlags, resultFlags0);
            Assert.Equal(expectedFlags, resultFlags1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, true)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, true)]
        public void DeviceStatusFlags_Equality_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsHi rhsHiFlags, bool expectedResult)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);
            var rhsFlags = new DeviceStatusFlags(rhsLoFlags, rhsHiFlags);

            var result0 = lhsFlags == rhsFlags;
            var result1 = rhsFlags == lhsFlags;

            Assert.Equal(expectedResult, result0);
            Assert.Equal(expectedResult, result1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, true)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, false)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, false)]
        public void DeviceStatusFlags_EqualityLoFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, bool expectedResult)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var result0 = lhsFlags == rhsLoFlags;
            var result1 = rhsLoFlags == lhsFlags;

            Assert.Equal(expectedResult, result0);
            Assert.Equal(expectedResult, result1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsHi.ReservedMask, true)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.ResetMenuHistory, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, false)]
        public void DeviceStatusFlags_EqualityHiFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsHi rhsHiFlags, bool expectedResult)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var result0 = lhsFlags == rhsHiFlags;
            var result1 = rhsHiFlags == lhsFlags;

            Assert.Equal(expectedResult, result0);
            Assert.Equal(expectedResult, result1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, false)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, true)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, false)]
        public void DeviceStatusFlags_Inequality_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, DeviceStatusFlagsHi rhsHiFlags, bool expectedResult)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);
            var rhsFlags = new DeviceStatusFlags(rhsLoFlags, rhsHiFlags);

            var result0 = lhsFlags != rhsFlags;
            var result1 = rhsFlags != lhsFlags;

            Assert.Equal(expectedResult, result0);
            Assert.Equal(expectedResult, result1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, false)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, true)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, true)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, true)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, true)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, true)]
        public void DeviceStatusFlags_InequalityLoFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsLo rhsLoFlags, bool expectedResult)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var result0 = lhsFlags != rhsLoFlags;
            var result1 = rhsLoFlags != lhsFlags;

            Assert.Equal(expectedResult, result0);
            Assert.Equal(expectedResult, result1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsHi.ReservedMask, false)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.ResetMenuHistory, true)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, true)]
        public void DeviceStatusFlags_InequalityHiFlags_ReturnsExpectedResult(DeviceStatusFlagsLo lhsLoFlags, DeviceStatusFlagsHi lhsHiFlags, DeviceStatusFlagsHi rhsHiFlags, bool expectedResult)
        {
            var lhsFlags = new DeviceStatusFlags(lhsLoFlags, lhsHiFlags);

            var result0 = lhsFlags != rhsHiFlags;
            var result1 = rhsHiFlags != lhsFlags;

            Assert.Equal(expectedResult, result0);
            Assert.Equal(expectedResult, result1);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, ~DeviceStatusFlagsLo.None, ~DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, ~DeviceStatusFlagsLo.HardwareStatusFlagsMask, ~DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, ~DeviceStatusFlagsLo.None, ~DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, ~DeviceStatusFlagsLo.EcsStatusDisabled, ~DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, ~(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad), ~(DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory))]
        public void DeviceStatusFlags_Complement_ReturnsExpectedResult(DeviceStatusFlagsLo lowFlags, DeviceStatusFlagsHi highFlags, DeviceStatusFlagsLo resultLowFlags, DeviceStatusFlagsHi resultHighFlags)
        {
            var flags = new DeviceStatusFlags(lowFlags, highFlags);

            var resultFlags = ~flags;

            var expectedFlags = new DeviceStatusFlags(resultLowFlags, resultHighFlags);
            Assert.Equal(expectedFlags, resultFlags);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, "Lo: None; Hi: None")]
        [InlineData(DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.None, "Lo: Keyclicks; Hi: None")]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, "Lo: None; Hi: ResetMenuHistory")]
        [InlineData(DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet, "Lo: ZeroRamBeforeLoad; Hi: FlagsHaveBeenSet")]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, "Lo: Keyclicks, ZeroRamBeforeLoad; Hi: ResetMenuHistory, FlagsHaveBeenSet")]
        public void DeviceStatusFlags_ToString_ReturnsExpectedString(DeviceStatusFlagsLo lowFlags, DeviceStatusFlagsHi highFlags, string expectedFlagsString)
        {
            var flags = new DeviceStatusFlags(lowFlags, highFlags);

            var flagsString = flags.ToString();

            Assert.Equal(expectedFlagsString, flagsString);
        }

        [Fact]
        public void DeviceStatusFlags_ParseNullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceStatusFlags.Parse(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t \n")]
        [InlineData(";")]
        [InlineData("Lo")]
        [InlineData("Hi")]
        [InlineData("Lo:;Hi:")]
        [InlineData("Lo:; Hi:")]
        [InlineData("Lo: ; Hi: ")]
        [InlineData("Lo: ; Hi:\n ")]
        [InlineData("llow: None; hi: none")]
        [InlineData("Lo: None; hhI: None")]
        [InlineData("Lo: None")]
        [InlineData("Hi: None")]
        [InlineData("Lo: None; Lo: None")]
        [InlineData("Hi: None; Hi: None")]
        [InlineData("Lo: Keyclicks, goo; Hi: None")]
        [InlineData("Lo: None; Hi: FlagsHaveBeenSet, biff")]
        public void DeviceStatusFlags_ParseBadString_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => DeviceStatusFlags.Parse(value));
        }

        [Theory]
        [InlineData("Lo: None; Hi: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData("Hi: None; Lo: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData("Lo: Keyclicks; Hi: None", DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.None)]
        [InlineData("Hi: None; Lo: ConsolePowerOn", DeviceStatusFlagsLo.ConsolePowerOn, DeviceStatusFlagsHi.None)]
        [InlineData("Lo: None; Hi: FlagsHaveBeenSet", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData("Hi:FlagsHaveBeenSet ; Lo:None ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData("lo: EnableCartConfig,ConsolePowerOn; hi:None ", DeviceStatusFlagsLo.ConsolePowerOn | DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None)]
        [InlineData("hi: None; lo: ZeroRamBeforeLoad , Keyclicks", DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.None)]
        [InlineData("LO: None; HI: FlagsHaveBeenSet,ResetMenuHistory", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData("HI:ResetMenuHistory , FlagsHaveBeenSet ; LO: None ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        [InlineData("lO: Keyclicks,ZeroRamBeforeLoad ; hI:ResetMenuHistory, FlagsHaveBeenSet ", DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData("hI: FlagsHaveBeenSet,ResetMenuHistory ; lO:BackgroundGC, ShowTitleScreenOnPowerUp ", DeviceStatusFlagsLo.ShowTitleScreenOnPowerUp | DeviceStatusFlagsLo.BackgroundGC, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        public void DeviceStatusFlags_Parse_ProducesCorrectResult(string value, DeviceStatusFlagsLo expectedLowFlags, DeviceStatusFlagsHi expectedHighFlags)
        {
            var flags = DeviceStatusFlags.Parse(value);

            var expectedFlags = new DeviceStatusFlags(expectedLowFlags, expectedHighFlags);
            Assert.Equal(expectedFlags, flags);
        }

        [Theory]
        [InlineData("", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(" ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("\t \n", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(";", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Hi", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo:;Hi:", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo:; Hi:", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: ; Hi: ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: ; Hi:\n ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("llow: None; hi: none", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: None; hhI: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Hi: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: None; Lo: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Hi: None; Hi: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: Keyclicks, goo; Hi: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData("Lo: None; Hi: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData("Hi: None; Lo: None", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData("Lo: Keyclicks; Hi: None", DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.None, true)]
        [InlineData("Hi: None; Lo: ConsolePowerOn", DeviceStatusFlagsLo.ConsolePowerOn, DeviceStatusFlagsHi.None, true)]
        [InlineData("Lo: None; Hi: FlagsHaveBeenSet", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData("Hi:FlagsHaveBeenSet ; Lo:None ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData("lo: EnableCartConfig,ConsolePowerOn; hi:None ", DeviceStatusFlagsLo.ConsolePowerOn | DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, true)]
        [InlineData("hi: None; lo: ZeroRamBeforeLoad , Keyclicks", DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.None, true)]
        [InlineData("LO: None; HI: FlagsHaveBeenSet,ResetMenuHistory", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData("HI:ResetMenuHistory , FlagsHaveBeenSet ; LO: None ", DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        [InlineData("lO: Keyclicks,ZeroRamBeforeLoad ; hI:ResetMenuHistory, FlagsHaveBeenSet ", DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, true)]
        [InlineData("hI: FlagsHaveBeenSet,ResetMenuHistory ; lO:BackgroundGC, ShowTitleScreenOnPowerUp ", DeviceStatusFlagsLo.ShowTitleScreenOnPowerUp | DeviceStatusFlagsLo.BackgroundGC, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, true)]
        public void DeviceStatusFlags_TryParse_ProducesCorrectResult(string value, DeviceStatusFlagsLo expectedLowFlags, DeviceStatusFlagsHi expectedHighFlags, bool expectedSuccess)
        {
            DeviceStatusFlags flags;
            var success = DeviceStatusFlags.TryParse(value, out flags);

            Assert.Equal(expectedSuccess, success);
            var expectedFlags = new DeviceStatusFlags(expectedLowFlags, expectedHighFlags);
            Assert.Equal(expectedFlags, flags);
        }

        [Fact]
        public void DeviceStatusFlags_EqualsNull_ReturnsFalse()
        {
            var flags = new DeviceStatusFlags();

            var equal = flags.Equals(null);

            Assert.False(equal);
        }

        [Fact]
        public void DeviceStatusFlags_EqualsNonDeviceStatusFlagsObject_ReturnsFalse()
        {
            var flags = new DeviceStatusFlags();

            var equal = flags.Equals(this);

            Assert.False(equal);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None, true)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask, true)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ResetMenuHistory, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.IntellivisionIIStatusAggressive, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory, DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.None, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.None, DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks, DeviceStatusFlagsHi.FlagsHaveBeenSet, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet, DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.FlagsHaveBeenSet | DeviceStatusFlagsHi.ResetMenuHistory, true)]
        public void DeviceStatusFlags_EqualsDeviceStatusFlagsAsObject_ReturnsExpectedResult(DeviceStatusFlagsLo lowFlags, DeviceStatusFlagsHi highFlags, DeviceStatusFlagsLo objectLowFlags, DeviceStatusFlagsHi objectHighFlags, bool expectedResult)
        {
            var flags = new DeviceStatusFlags(lowFlags, highFlags);
            object objectFlags = new DeviceStatusFlags(objectLowFlags, objectHighFlags);

            var result = flags.Equals(objectFlags);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, DeviceStatusFlagsHi.None)]
        [InlineData(DeviceStatusFlagsLo.None, DeviceStatusFlagsHi.ReservedMask)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled, DeviceStatusFlagsHi.ResetMenuHistory)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.ZeroRamBeforeLoad, DeviceStatusFlagsHi.ResetMenuHistory | DeviceStatusFlagsHi.FlagsHaveBeenSet)]
        public void DeviceStatusFlags_GetHashCode_ProducesExpectedResult(DeviceStatusFlagsLo lowFlags, DeviceStatusFlagsHi highFlags)
        {
            var flags = new DeviceStatusFlags(lowFlags, highFlags);
            var hash = flags.GetHashCode();

            var lowHash = lowFlags.GetHashCode();
            var highHash = highFlags.GetHashCode();
            var expectedHash = ((lowHash << 5) + lowHash) ^ highHash;

            Assert.Equal(expectedHash, hash);
        }
    }
}

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

using System.Collections.Generic;
using INTV.LtoFlash.Model;
using Xunit;

namespace INTV.LtoFlash.Tests.Model
{
    public class DeviceStatusFlagsLoTests
    {
        #region Individual Hardware Status Flag Tests

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

        #endregion // Individual Hardware Status Flag Tests

        #region Individual Intellivision II Compatibility Flag Tests

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

        [Theory]
        [InlineData((DeviceStatusFlagsLo)0xFFFFFFFFFFFFFFFF, IntellivisionIIStatusFlags.Default)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusMask, IntellivisionIIStatusFlags.Default)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusAggressive | DeviceStatusFlagsLo.IntellivisionIIStatusConservative, IntellivisionIIStatusFlags.Default)]
        public void DeviceStatusFlagsLo_MultipleFlagsSetToIntellivisionIICompatibiltyFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, IntellivisionIIStatusFlags expectedIntellivisionIIStatusFlags)
        {
            var actualIntellivisionIIStatusFlags = statusFlagsLo.ToIntellivisionIICompatibilityFlags();

            Assert.Equal(expectedIntellivisionIIStatusFlags, actualIntellivisionIIStatusFlags);
        }

        #endregion // Individual Intellivision II Compatibility Flag Tests

        #region Individual ECS Compatibility Flag Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToEcsStatusFlagsTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)EcsStatusFlags.None };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedEcsFlags = EcsStatusFlags.None;
                    if ((i >= DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset) && (i < DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset + (sizeof(EcsStatusFlags) * 8)))
                    {
                        expectedEcsFlags = (EcsStatusFlags)(1u << (i - DeviceStatusFlagsLoHelpers.EcsStatusBitsOffset));
                    }
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedEcsFlags };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToEcsStatusFlagsTestData")]
        public void DeviceStatusFlagsLo_ToEcsCompatibilityFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, EcsStatusFlags expectedEcsStatusFlags)
        {
            var actualEcsStatusFlags = statusFlagsLo.ToEcsCompatibilityFlags();

            // NOTE: Implementation currently suppresses the reserved bits. Replicate this here.
            expectedEcsStatusFlags &= ~EcsStatusFlags.ReservedMask;
            Assert.Equal(expectedEcsStatusFlags, actualEcsStatusFlags);
        }

        [Theory]
        [InlineData((DeviceStatusFlagsLo)0xFFFFFFFFFFFFFFFF, EcsStatusFlags.Default)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusMask, EcsStatusFlags.Default)]
        public void DeviceStatusFlagsLo_AllEcsFlagsToEcsCompatibiltyFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, EcsStatusFlags expectedEcsStatusFlags)
        {
            var actualEcsStatusFlags = statusFlagsLo.ToEcsCompatibilityFlags();

            Assert.Equal(expectedEcsStatusFlags, actualEcsStatusFlags);
        }

        #endregion // Individual ECS Compatibility Flag Tests

        #region Show Title Screen Flags Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToShowTitleScreenFlagsTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)ShowTitleScreenFlags.Never };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedShowTitleScreenFlags = ShowTitleScreenFlags.Never;
                    if ((i >= DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset) && (i < DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset + 2))
                    {
                        expectedShowTitleScreenFlags = (ShowTitleScreenFlags)(1u << (i - DeviceStatusFlagsLoHelpers.ShowTitleScreenBitsOffset));
                    }
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedShowTitleScreenFlags };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToShowTitleScreenFlagsTestData")]
        public void DeviceStatusFlagsLo_ToShowTitleScreenFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, ShowTitleScreenFlags expectedShowTitleScreenFlags)
        {
            var actualShowTitleScreenFlags = statusFlagsLo.ToShowTitleScreenFlags();

            // NOTE: Implementation coerces 'reserved' to 'on power up'. Replicate this here.
            if (expectedShowTitleScreenFlags == ShowTitleScreenFlags.Reserved)
            {
                expectedShowTitleScreenFlags = ShowTitleScreenFlags.OnPowerUp;
            }
            Assert.Equal(expectedShowTitleScreenFlags, actualShowTitleScreenFlags);
        }

        #endregion // Show Title Screen Flags Tests

        #region Save Menu Position Flags Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToSaveMenuPositionFlagsTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)SaveMenuPositionFlags.Never };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedSaveMenuPositionFlags = SaveMenuPositionFlags.Never;
                    if ((i >= DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset) && (i < DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset + 2))
                    {
                        expectedSaveMenuPositionFlags = (SaveMenuPositionFlags)(1u << (i - DeviceStatusFlagsLoHelpers.SaveMenuPositionBitsOffset));
                    }
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedSaveMenuPositionFlags };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToSaveMenuPositionFlagsTestData")]
        public void DeviceStatusFlagsLo_ToSaveMenuPositionFlags_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, SaveMenuPositionFlags expectedSaveMenuPositionFlags)
        {
            var actualSaveMenuPositionFlags = statusFlagsLo.ToSaveMenuPositionFlags();

            // NOTE: Implementation coerces 'reserved' to during session only'. Replicate this here.
            if (expectedSaveMenuPositionFlags == SaveMenuPositionFlags.Reserved)
            {
                expectedSaveMenuPositionFlags = SaveMenuPositionFlags.DuringSessionOnly;
            }
            Assert.Equal(expectedSaveMenuPositionFlags, actualSaveMenuPositionFlags);
        }

        #endregion // Save Menu Position Flags Tests

        #region Background Garbage Collect Flag Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToBackgroundGCTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)false };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedBackgroundGC = i == DeviceStatusFlagsLoHelpers.BackgroundGCBitsOffset;
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedBackgroundGC };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToBackgroundGCTestData")]
        public void DeviceStatusFlagsLo_ToBackgroundGC_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, bool expectedBackgroundGC)
        {
            var actualBackgroundGC = statusFlagsLo.ToBackgroundGC();

            Assert.Equal(expectedBackgroundGC, actualBackgroundGC);
        }

        #endregion // Background Garbage Collect Flag Tests

        #region Keyclicks Flag Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToKeyclicksTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)false };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedKeyclicks = i == DeviceStatusFlagsLoHelpers.KeyclicksBitsOffset;
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedKeyclicks };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToKeyclicksTestData")]
        public void DeviceStatusFlagsLo_ToKeyclicks_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, bool expectedKeyclicks)
        {
            var actualKeyclicks = statusFlagsLo.ToKeyclicks();

            Assert.Equal(expectedKeyclicks, actualKeyclicks);
        }

        #endregion // Keyclicks Flag Tests

        #region Enable Onboard Configuration Menu Flag Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToEnableOnboardConfigMenuTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)false };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedEnableOnboardConfigMenu = i == DeviceStatusFlagsLoHelpers.EnableCartConfigBitsOffset;
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedEnableOnboardConfigMenu };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToEnableOnboardConfigMenuTestData")]
        public void DeviceStatusFlagsLo_ToEnableOnboardConfigMenu_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, bool expectedEnableOnboardConfigMenu)
        {
            var actualEnableOnboardConfigMenu = statusFlagsLo.ToEnableOnboardConfigMenu();

            Assert.Equal(expectedEnableOnboardConfigMenu, actualEnableOnboardConfigMenu);
        }

        #endregion // Enable Onboard Configuration Menu Flag Tests

        #region Enable RAM Zeroing Flag Tests

        public static IEnumerable<object[]> DeviceStatusFlagsLoToZeroLtoFlashRamTestData
        {
            get
            {
                yield return new object[] { (object)DeviceStatusFlagsLo.None, (object)false };
                for (var i = 0; i < sizeof(DeviceStatusFlagsLo) * 8; ++i)
                {
                    var deviceStatusFlagsLo = (DeviceStatusFlagsLo)(1ul << i);
                    var expectedZeroLtoFlashRam = i == DeviceStatusFlagsLoHelpers.ZeroRamBeforeLoadBitsOffset;
                    yield return new object[] { (object)deviceStatusFlagsLo, (object)expectedZeroLtoFlashRam };
                }
            }
        }

        [Theory]
        [MemberData("DeviceStatusFlagsLoToZeroLtoFlashRamTestData")]
        public void DeviceStatusFlagsLo_ToZeroLtoFlashRam_ReturnsExpectedFlags(DeviceStatusFlagsLo statusFlagsLo, bool expectedZeroLtoFlashRam)
        {
            var actualZeroLtoFlashRam = statusFlagsLo.ToZeroLtoFlashRam();

            Assert.Equal(expectedZeroLtoFlashRam, actualZeroLtoFlashRam);
        }

        #endregion // Enable RAM Zeroing Flag Tests

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, 0u)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask, 0u)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive | DeviceStatusFlagsLo.ConsolePowerOn, 0x00200002u)]
        [InlineData(DeviceStatusFlagsLo.ZeroRamBeforeLoad | (DeviceStatusFlagsLo)0x8000000000000000ul | DeviceStatusFlagsLo.SaveMenuPositionMask | DeviceStatusFlagsLo.EcsStatusDisabled, 0x808C0300u)]
        public void DeviceStatusFlagsLo_ToConfigurationFlags_ReturnsExpectedBits(DeviceStatusFlagsLo statusFlagsLo, uint expectedConfigurationBits)
        {
            var actualConfigurationBits = statusFlagsLo.ToConfigurationFlags();

            Assert.Equal(expectedConfigurationBits, actualConfigurationBits);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.None, 0, false)]
        [InlineData(DeviceStatusFlagsLo.None, 1, true)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks, 0, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks, -1, false)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks, 1000000, true)]
        [InlineData(DeviceStatusFlagsLo.IntellivisionIIStatusMask, 1, true)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusMask, 10, true)]
        [InlineData(DeviceStatusFlagsLo.ShowTitleScreenMask, 369, true)]
        [InlineData(DeviceStatusFlagsLo.SaveMenuPositionMask, 2058, true)]
        [InlineData(DeviceStatusFlagsLo.BackgroundGC, 698, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, 3993, false)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, 3994, true)]
        [InlineData(DeviceStatusFlagsLo.EnableCartConfig, 3995, true)]
        [InlineData(DeviceStatusFlagsLo.ZeroRamBeforeLoad, 4416, false)]
        [InlineData(DeviceStatusFlagsLo.ZeroRamBeforeLoad, 4600, false)] // TODO: UPDATE when released
        [InlineData(DeviceStatusFlagsLo.ZeroRamBeforeLoad, 10000, true)]
        public void DeviceStatusFlagsLo_IsConfigurableFeatureAvailableForFirmwareVersion_ReturnsExpectedResult(DeviceStatusFlagsLo feature, int firmwareVersion, bool expectedAvailability)
        {
            var isAvailable = feature.IsConfigurableFeatureAvailable(firmwareVersion);

            Assert.Equal(expectedAvailability, isAvailable);
        }

        [Theory]
        [InlineData(DeviceStatusFlagsLo.ConsolePowerOn)]
        [InlineData(DeviceStatusFlagsLo.HardwareStatusFlagsMask)]
        [InlineData(DeviceStatusFlagsLo.Keyclicks | DeviceStatusFlagsLo.BackgroundGC)]
        [InlineData(DeviceStatusFlagsLo.EcsStatusDisabled | DeviceStatusFlagsLo.IntellivisionIIStatusAggressive)]
        public void DeviceStatusFlagsLo_IsConfigurableFeatureAvailableForFirmwareVersion_ThrowsArgumentOutOfRangeException(DeviceStatusFlagsLo features)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => features.IsConfigurableFeatureAvailable(0));
        }
    }
}

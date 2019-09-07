// <copyright file="ConfigurableLtoFlashFeatureTests.cs" company="INTV Funhouse">
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
using INTV.LtoFlash.Model;
using Xunit;

namespace INTV.LtoFlash.Tests.Model
{
    public class ConfigurableLtoFlashFeatureTests
    {
        [Fact]
        public void ConfigurableLtoFLashFeature_CreateFeature_HasCorrectDefaultValue()
        {
            var defaultValue = -4;

            var feature = new ConfigurableTestLtoFlashIntFeature("CreateCheckDefaultValue", defaultValue);

            Assert.Equal(defaultValue, feature.FactoryDefaultValue);
        }

        [Fact]
        public void ConfigurableLtoFLashFeature_CreateFeature_HasCorrectCurrentValue()
        {
            var defaultValue = -3;

            var feature = new ConfigurableTestLtoFlashIntFeature("CreateCheckCurrentValue", defaultValue);

            Assert.Equal(defaultValue, feature.CurrentValue);
        }

        [Fact]
        public void ConfigurableLtoFLashFeature_CreateFeature_HasExpectedFlagsMask()
        {
            var defaultValue = 0x0C0C0C0C;

            var feature = new ConfigurableTestLtoFlashIntFeature("CreateCheckFlagsMask", defaultValue);

            var expectedFeatureFlagsMask = ConfigurableTestLtoFlashIntFeature.CreateFlagsMask(defaultValue);

            Assert.Equal(expectedFeatureFlagsMask, feature.FeatureFlagsMask);
        }

        [Fact]
        public void ConfigurableLtoFlashFeature_GetValueFromDeviceWithNullDevice_ThrowsNullReferenceException()
        {
            var feature = new ConfigurableTestLtoFlashIntFeature("GetValueFromDeviceWithNullDevice", -1);

            Assert.Throws<NullReferenceException>(() => feature.GetValueFromDevice(null));
        }

        [Fact]
        public void ConfigurableLtoFlashFeature_SetValueOnDeviceWithNullDevice_ThrowsNullReferenceException()
        {
            var feature = new ConfigurableTestLtoFlashIntFeature("SetValueOnDeviceWithNullDevice", 1);

            Assert.Throws<NullReferenceException>(() => feature.SetValueOnDevice(null, 3));
        }

        public static IEnumerable<object[]> ConfigurableLtoFlashFeatures
        {
            get
            {
                foreach (var configurableLtoFlashFeature in LtoFlash.Model.ConfigurableLtoFlashFeatures.Default.Features)
                {
                    yield return new object[] { configurableLtoFlashFeature };
                }
            }
        }

        [Theory]
        [MemberData("ConfigurableLtoFlashFeatures")]
        public void ConfigurableLtoFlashFeature_GetValueFromDeviceWithDummyDevice_ThrowsInvalidOperationException(IConfigurableLtoFlashFeature feature)
        {
            var dummyDevice = new Device("GetValueFromDeviceWithDummyDevice");

            Assert.Throws<InvalidOperationException>(() => feature.GetValueFromDevice(dummyDevice));
        }

        public static IEnumerable<object[]> ConfigurableLtoFlashFeaturesSetValueData
        {
            get
            {
                foreach (var configurableLtoFlashFeature in LtoFlash.Model.ConfigurableLtoFlashFeatures.Default.Features)
                {
                    yield return new object[] { configurableLtoFlashFeature, 0 };
                }
            }
        }

        [Theory]
        [MemberData("ConfigurableLtoFlashFeaturesSetValueData")]
        public void ConfigurableLtoFlashFeature_SetValueOnDeviceWithDummyDevice_ThrowsInvalidOperationException(IConfigurableLtoFlashFeature feature, object value)
        {
            var dummyDevice = new Device("SetValueOnDeviceWithDummyDevice");

            Assert.Throws<InvalidOperationException>(() => feature.SetValueOnDevice(dummyDevice, value));
        }

        [Theory]
        [InlineData(-1, 3, true)]
        [InlineData(-1, -1, false)]
        public void ConfigurableLtoFlashFeature_UpdateCurrentValue_UpdatesValueWithExpectedValueChangedResult(int defaultValue, int newFlags, bool expectedValueChanged)
        {
            var feature = new ConfigurableTestLtoFlashIntFeature("UpdateCurrentValue", defaultValue);

            var newStatusFlags = ConfigurableTestLtoFlashIntFeature.CreateFlagsMask(newFlags);
            var valueChanged = feature.UpdateCurrentValue(newStatusFlags);

            var expectedNewValue = (int)(newStatusFlags & feature.FeatureFlagsMask).Lo;
            Assert.Equal(expectedNewValue, feature.CurrentValue);
            Assert.Equal(expectedValueChanged, valueChanged);
        }

        [Fact]
        public void ReadOnlyConfigurableLtoFlashFeature_SetValueOnDevice_ThrowsInvalidOperationException()
        {
            var feature = ConfigurableTestLtoFlashIntFeature.CreateReadOnly();

            Assert.Throws<InvalidOperationException>(() => feature.SetValueOnDevice(null, 6));
        }

        [Fact]
        public void ReadOnlyConfigurableLtoFlashFeature_UpdateCurrentValue_ThrowsInvalidOperationException()
        {
            var feature = ConfigurableTestLtoFlashIntFeature.CreateReadOnly();

            var newStatusFlags = ConfigurableTestLtoFlashIntFeature.CreateFlagsMask(987);
            Assert.Throws<InvalidOperationException>(() => feature.UpdateCurrentValue(newStatusFlags));
        }

        private class ConfigurableTestLtoFlashIntFeature : ConfigurableLtoFlashFeature<int>
        {
            public ConfigurableTestLtoFlashIntFeature(string name, int defaultValue)
                : base(name, name, defaultValue, CreateFlagsMask(defaultValue))
            {
            }

            public static DeviceStatusFlags CreateFlagsMask(int forFlags)
            {
                var flagsMask = new DeviceStatusFlags((DeviceStatusFlagsLo)forFlags, (DeviceStatusFlagsHi)forFlags);
                return flagsMask;
            }

            public static IConfigurableLtoFlashFeature CreateReadOnly()
            {
                var name = "ReadOnlyTestLtoFlashIntFeature";
                var defaultValue = 0x13579;
                return new ReadOnlyConfigurableLtoFlashFeature(name, name, defaultValue, CreateFlagsMask(defaultValue));
            }

            protected override DeviceStatusFlags ConvertValueToDeviceStatusFlags(int newValue)
            {
                var flags = CreateFlagsMask(newValue) & FeatureFlagsMask;
                return flags;
            }

            protected override int ConvertDeviceStatusFlagsToValue(DeviceStatusFlags currentConfiguration)
            {
                var value = (int)(currentConfiguration & FeatureFlagsMask).Lo;
                return value;
            }
        }
    }
}

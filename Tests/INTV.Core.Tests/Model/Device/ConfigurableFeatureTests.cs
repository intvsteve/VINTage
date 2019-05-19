// <copyright file="ConfigurableFeatureTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Device;
using Xunit;

namespace INTV.Core.Tests.Model.Device
{
    public class ConfigurableFeatureTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("{0789B590-7857-4D77-BA76-156B7395FDD8}", null)]
        [InlineData("{C57CFC64-700F-41D0-B1FD-6D305C926D53}", "")]
        [InlineData(null, "That's")]
        [InlineData("", "Kinda silly")]
        public void ConfigurableFeature_CreateWithInvalidArgument_ThrowsArgumentOutOfRangeException(string uniqueId, string displayName)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConfigurableTestFeature(uniqueId, displayName));
        }

        [Fact]
        public void ConfigurableFeature_GetFactoryDefaultValueAfterCreating_ReturnsFactoryDefaultValue()
        {
            var feature = new ConfigurableTestFeature("GetFactoryDefaultTest");

            Assert.Equal(ConfigurableTestFeature.FactoryDefault, feature.FactoryDefaultValue);
            Assert.Equal(ConfigurableTestFeature.FactoryDefault, ((IConfigurableFeature)feature).FactoryDefaultValue);
        }

        [Fact]
        public void ConfigurableFeature_GetCurrentValueAfterCreating_ReturnsDefaultValue()
        {
            var feature = new ConfigurableTestFeature("GetCurrentValueAfterCreateTest");

            Assert.Equal(ConfigurableTestFeature.FactoryDefault, feature.CurrentValue);
        }

        [Fact]
        public void ConfigurableFeature_GetValueFromDevice_ReturnsDefaultValueIfNeverWrittenTo()
        {
            var feature = new ConfigurableTestFeature("GetValueFromDeviceAfterCreateTest");

            Assert.Equal(ConfigurableTestFeature.FactoryDefault, ((IConfigurableFeature)feature).GetValueFromDevice(null));
        }

        [Fact]
        public void ConfigurableFeature_SetValueOnDevice_ChangesCurrentValue()
        {
            var feature = new ConfigurableTestFeature("SetValueOnDeviceTest");

            var newValue = 0x12345;
            ((IConfigurableFeature)feature).SetValueOnDevice(null, newValue);

            Assert.Equal(newValue, feature.CurrentValue);
            Assert.Equal(newValue, ((IConfigurableFeature)feature).CurrentValue);
        }

        private class ConfigurableTestFeature : ConfigurableFeature<int>
        {
            public const int FactoryDefault = -1;

            public ConfigurableTestFeature(string name)
                : this(name, name)
            {
            }

            public ConfigurableTestFeature(string uniqueId, string displayName)
                : base(uniqueId, displayName, FactoryDefault)
            {
            }

            public override int GetValueFromDevice(IPeripheral device)
            {
                return CurrentValue;
            }

            public override void SetValueOnDevice(IPeripheral device, int newValue)
            {
                CurrentValue = newValue;
            }
        }
    }
}

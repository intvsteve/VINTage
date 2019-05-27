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
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConfigurableIntTestFeature(uniqueId, displayName));
        }

        [Fact]
        public void ConfigurableFeature_GetFactoryDefaultValueAfterCreating_ReturnsFactoryDefaultValue()
        {
            var feature = new ConfigurableIntTestFeature("GetFactoryDefaultTest");

            Assert.Equal(ConfigurableIntTestFeature.FactoryDefault, feature.FactoryDefaultValue);
            Assert.Equal(ConfigurableIntTestFeature.FactoryDefault, ((IConfigurableFeature)feature).FactoryDefaultValue);
        }

        [Fact]
        public void ConfigurableFeature_GetCurrentValueAfterCreating_ReturnsDefaultValue()
        {
            var feature = new ConfigurableIntTestFeature("GetCurrentValueAfterCreateTest");

            Assert.Equal(ConfigurableIntTestFeature.FactoryDefault, feature.CurrentValue);
        }

        [Fact]
        public void ConfigurableFeature_GetValueFromDevice_ReturnsDefaultValueIfNeverWrittenTo()
        {
            var feature = new ConfigurableIntTestFeature("GetValueFromDeviceAfterCreateTest");

            Assert.Equal(ConfigurableIntTestFeature.FactoryDefault, ((IConfigurableFeature)feature).GetValueFromDevice(null));
        }

        [Fact]
        public void ConfigurableFeature_SetValueOnDevice_ChangesCurrentValue()
        {
            var feature = new ConfigurableIntTestFeature("SetValueOnDeviceTest");

            var newValue = 0x12345;
            ((IConfigurableFeature)feature).SetValueOnDevice(null, newValue);

            Assert.Equal(newValue, feature.CurrentValue);
            Assert.Equal(newValue, ((IConfigurableFeature)feature).CurrentValue);
        }

        [Fact]
        public void ConfigurableFeature_SetObjectValueOnDevice_ChangesCurrentValue()
        {
            var feature = new ConfigurableTestFeature("SetObjectValueOnDeviceTest");

            var newValue = new object();
            feature.SetValueOnDevice(null, newValue);

            Assert.Equal(newValue, feature.CurrentValue);
        }

        [Fact]
        public void ReadOnlyConfigurableFeature_SetValueOnDevice_ThrowsInvalidOperationException()
        {
            var feature = new ReadOnlyConfigurableTestFeature("ReadOnlyConfigurableFeature");

            Assert.Throws<InvalidOperationException>(() => feature.SetValueOnDevice(null, 0x54321));
        }

        [Fact]
        public void GenericIntReadOnlyConfigurableIntFeature_SetValueOnDevice_ThrowsInvalidOperationException()
        {
            IConfigurableFeature feature = new ReadOnlyConfigurableIntTestFeature();

            Assert.Throws<InvalidOperationException>(() => feature.SetValueOnDevice(null, 0x54321));
        }

        [Fact]
        public void ReadOnlyConfigurableGenericIntFeature_SetValueOnDevice_ThrowsInvalidOperationException()
        {
            IConfigurableFeature<int> feature = new ReadOnlyConfigurableGenericIntTestFeature();

            Assert.Throws<InvalidOperationException>(() => feature.SetValueOnDevice(null, 0x54321));
        }

        private class ConfigurableTestFeature : IConfigurableFeature
        {
            public ConfigurableTestFeature(string name)
            {
                DisplayName = name;
                UniqueId = name;
                FactoryDefaultValue = name;
                CurrentValue = name;
            }

            public string DisplayName { get; private set; }

            public string UniqueId { get; private set; }

            public object FactoryDefaultValue { get; private set; }

            public object CurrentValue { get; private set; }

            public object GetValueFromDevice(IPeripheral device)
            {
                return CurrentValue;
            }

            public void SetValueOnDevice(IPeripheral device, object newValue)
            {
                this.VerifyWriteAccess();
                CurrentValue = newValue;
            }
        }

        private class ReadOnlyConfigurableTestFeature : ConfigurableTestFeature, IReadOnlyConfigurableFeature
        {
            public ReadOnlyConfigurableTestFeature(string name)
                : base("ReadOnly" + name)
            {
            }
        }

        private class ConfigurableIntTestFeature : ConfigurableFeature<int>
        {
            public const int FactoryDefault = -1;

            public ConfigurableIntTestFeature(string name)
                : this(name, name)
            {
            }

            public ConfigurableIntTestFeature(string uniqueId, string displayName)
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

        private class ReadOnlyConfigurableGenericIntTestFeature : ConfigurableIntTestFeature, IReadOnlyConfigurableFeature<int>
        {
            public ReadOnlyConfigurableGenericIntTestFeature()
                : base("ReadOnly<int>")
            {
            }
        }

        private class ReadOnlyConfigurableIntTestFeature : ConfigurableIntTestFeature, IReadOnlyConfigurableFeature
        {
            public ReadOnlyConfigurableIntTestFeature()
                : base("ReadOnly")
            {
            }

            object IReadOnlyConfigurableFeature.FactoryDefaultValue
            {
                get { throw new NotImplementedException(); }
            }

            object IReadOnlyConfigurableFeature.CurrentValue
            {
                get { throw new NotImplementedException(); }
            }

            object IReadOnlyConfigurableFeature.GetValueFromDevice(IPeripheral device)
            {
                throw new NotImplementedException();
            }
        }
    }
}

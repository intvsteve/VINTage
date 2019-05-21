﻿// <copyright file="ConfigurableLtoFlashFeature`T.cs" company="INTV Funhouse">
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

using INTV.Core.ComponentModel;
using INTV.Core.Model.Device;
using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Common implementation for configurable Locutus features.
    /// </summary>
    /// <typeparam name="T">The data type of the configurable feature.</typeparam>
    public abstract class ConfigurableLtoFlashFeature<T> : ConfigurableFeature<T>, IConfigurableLtoFlashFeature
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ConfigurableLtoFlashFeature{T}"/>.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the configurable feature.</param>
        /// <param name="displayName">The user-friendly display name of the feature.</param>
        /// <param name="defaultValue">The factory default value for the feature.</param>
        /// <param name="featureFlagsMask">The flags mask for the feature.</param>
        protected ConfigurableLtoFlashFeature(string uniqueId, string displayName, T defaultValue, DeviceStatusFlags featureFlagsMask)
            : base(uniqueId, displayName, defaultValue)
        {
            FeatureFlagsMask = featureFlagsMask;
        }

        #region IConfigurableLtoFlashFeature Properties

        /// <summary>
        /// Gets the flags mask for the feature's value as a <see cref="DeviceStatusFlags"/> value.
        /// </summary>
        public DeviceStatusFlags FeatureFlagsMask { get; private set; }

        #endregion // IConfigurableLtoFlashFeature Properties

        #region IConfigurableFeature<T> Overrides

        /// <inheritdoc />
        public override T GetValueFromDevice(IPeripheral device)
        {
            var locutus = (Device)device;
            var currentConfiguration = locutus.DeviceStatusFlags;
            return ConvertDeviceStatusFlagsToValue(currentConfiguration);
        }

        /// <inheritdoc />
        public override void SetValueOnDevice(IPeripheral device, T newValue)
        {
            var locutus = (Device)device;
            var newConfiguration = GetUpdatedConfigurationFlags(locutus, newValue);
            locutus.SetConfiguration(newConfiguration, (m, e) => locutus.ErrorHandler(DeviceStatusFlags.ZeroRamBeforeLoad, ProtocolCommandId.SetConfiguration, m, e));
        }

        #endregion // IConfigurableFeature<T> Overrides

        #region IConfigurableLtoFlashFeature Methods

        /// <inheritdoc />
        public bool UpdateCurrentValue(DeviceStatusFlags currentConfiguration)
        {
            var newValue = ConvertDeviceStatusFlagsToValue(currentConfiguration);
            var valueChanged = INotifyPropertyChangedHelpers.SafeDidValueChangeCompare(newValue, CurrentValue);
            CurrentValue = newValue;
            return valueChanged;
        }

        #endregion // IConfigurableLtoFlashFeature Methods

        /// <summary>
        /// Directly modify the current value of the feature.
        /// </summary>
        /// <param name="newValue">The new value to unconditionally assign to <see cref="CurrentValue"/>.</param>
        public void SetCurrentValue(T newValue)
        {
            CurrentValue = newValue;
        }

        /// <summary>
        /// Converts the given new value to its corresponding device status flags.
        /// </summary>
        /// <param name="newValue">The value to convert to <see cref="DeviceStatusFlags"/>.</param>
        /// <returns><paramref name="newValue"/> represented as <see cref="DeviceStatusFlags"/>.</returns>
        protected abstract DeviceStatusFlags ConvertValueToDeviceStatusFlags(T newValue);

        /// <summary>
        /// Converts the given current configuration as <see cref="DeviceStatusFlags"/> to a concrete type.
        /// </summary>
        /// <param name="currentConfiguration">The device's current status.</param>
        /// <returns>The configurable feature as a concretely typed value.</returns>
        protected abstract T ConvertDeviceStatusFlagsToValue(DeviceStatusFlags currentConfiguration);

        private DeviceStatusFlags GetUpdatedConfigurationFlags(Device device, T newValue)
        {
            var configuration = device.DeviceStatusFlags & ~FeatureFlagsMask;
            configuration |= ConvertValueToDeviceStatusFlags(newValue);
            return configuration;
        }
    }
}
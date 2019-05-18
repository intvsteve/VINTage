// <copyright file="IConfigurableFeature.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Use this interface to describe a device-specific configurable feature.
    /// </summary>
    public interface IConfigurableFeature
    {
        /// <summary>
        /// Gets a localized user-readable display name for the feature.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets a unique name for the feature.
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// Gets the 'factory default' value for the feature.
        /// </summary>
        object FactoryDefaultValue { get; }

        /// <summary>
        /// Gets the current value of the feature.
        /// </summary>
        object CurrentValue { get; }

        /// <summary>
        /// Gets the current value of the feature directly from the device.
        /// </summary>
        /// <param name="device">The device to get the configured value from.</param>
        /// <returns>The value on the device.</returns>
        object GetFeatureValueFromDevice(IPeripheral device);

        /// <summary>
        /// Set the value of the feature on the device itself.
        /// </summary>
        /// <param name="device">The device to set the configured value on.</param>
        /// <param name="newValue">The new value to set on the device.</param>
        void SetFeatureValueOnDevice(IPeripheral device, object newValue);
    }
}

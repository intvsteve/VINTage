// <copyright file="IConfigurableLtoFlashFeature.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Device;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Extends <see cref="IConfigurableFeature"/> for use with Locutus devices.
    /// </summary>
    public interface IConfigurableLtoFlashFeature : IConfigurableFeature
    {
        /// <summary>
        /// Gets the flags mask for the feature's value as a <see cref="DeviceStatusFlags"/> value.
        /// </summary>
        DeviceStatusFlags FeatureFlagsMask { get; }

        /// <summary>
        /// Update the configurable feature's current value based on current configuration report by a Locutus device.
        /// </summary>
        /// <param name="currentConfiguration">The current device configuration data.</param>
        /// <returns><c>true</c> if the configurable feature's value described by <paramref name="currentConfiguration"/> is different than <see cref="CurrentValue"/>.</returns>
        /// <remarks>The value of <paramref name="currentConfiguration"/> is typically from the status response of a ping or background garbage collect command.</remarks>
        bool UpdateCurrentValue(DeviceStatusFlags currentConfiguration);
    }
}

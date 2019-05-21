// <copyright file="ConfigurableLtoFlashBooleanFeature.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implements <see cref="ConfigurableLtoFlashFeature{bool}"/>. Used for simple on / off configurable properties.
    /// </summary>
    public class ConfigurableLtoFlashBooleanFeature : ConfigurableLtoFlashFeature<bool>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurableLtoFlashBooleanFeature"/>.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the configurable feature.</param>
        /// <param name="displayName">The user-friendly display name of the feature.</param>
        /// <param name="defaultValue">The factory default value for the feature.</param>
        /// <param name="featureFlagsMask">The flags mask for the feature.</param>
        public ConfigurableLtoFlashBooleanFeature(string uniqueId, string displayName, bool defaultValue, DeviceStatusFlags featureFlagsMask)
            : base(uniqueId, displayName, defaultValue, featureFlagsMask)
        {
        }

        /// <inheritdoc />
        protected override DeviceStatusFlags ConvertValueToDeviceStatusFlags(bool newValue)
        {
            var configurationFlags = newValue ? FeatureFlagsMask : DeviceStatusFlags.None;
            return configurationFlags;
        }

        /// <inheritdoc />
        protected override bool ConvertDeviceStatusFlagsToValue(DeviceStatusFlags currentConfiguration)
        {
            var value = currentConfiguration.HasFlag(FeatureFlagsMask);
            return value;
        }
    }
}

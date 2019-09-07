// <copyright file="ConfigurableLtoFlashIntellivisionIICompatibilityFeature.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implements a configurable feature for Locutus's Intellivision II compatibility options.
    /// </summary>
    public class ConfigurableLtoFlashIntellivisionIICompatibilityFeature : ConfigurableLtoFlashFeature<IntellivisionIIStatusFlags>
    {
        private static readonly Lazy<IConfigurableLtoFlashFeature> Default = new Lazy<IConfigurableLtoFlashFeature>(() => new ReadOnlyConfigurableLtoFlashFeature(Device.IntvIICompatibilityPropertyName, Resources.Strings.SetIntellivisionIICompatibilityCommand_Name, IntellivisionIIStatusFlags.Default, DeviceStatusFlags.IntellivisionIIStatusMask));

        /// <summary>
        /// Initialize a new instance of <see cref="ConfigurableLtoFlashIntellivisionIICompatibilityFeature"/>.
        /// </summary>
        protected ConfigurableLtoFlashIntellivisionIICompatibilityFeature()
            : base(Device.IntvIICompatibilityPropertyName, Resources.Strings.SetIntellivisionIICompatibilityCommand_Name, IntellivisionIIStatusFlags.Default, DeviceStatusFlags.IntellivisionIIStatusMask)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="ConfigurableLtoFlashIntellivisionIICompatibilityFeature"/>.
        /// </summary>
        /// <param name="readOnly">If <c>true</c>, the feature is read-only and will throw a <see cref="System.InvalidOperationException"/> if modified.</param>
        /// <returns>A new configurable LTO Flash! feature.</returns>
        public static IConfigurableLtoFlashFeature Create(bool readOnly)
        {
            IConfigurableLtoFlashFeature configurableFeature;
            if (readOnly)
            {
                configurableFeature = Default.Value;
            }
            else
            {
                configurableFeature = new ConfigurableLtoFlashIntellivisionIICompatibilityFeature();
            }
            return configurableFeature;
        }

        /// <inheritdoc />
        protected override DeviceStatusFlags ConvertValueToDeviceStatusFlags(IntellivisionIIStatusFlags newValue)
        {
            var configurationFlags = new DeviceStatusFlags(newValue.ToDeviceStatusFlagsLo());
            return configurationFlags;
        }

        /// <inheritdoc />
        protected override IntellivisionIIStatusFlags ConvertDeviceStatusFlagsToValue(DeviceStatusFlags currentConfiguration)
        {
            var value = currentConfiguration.Lo.ToIntellivisionIICompatibilityFlags();
            return value;
        }
    }
}

// <copyright file="ConfigurableLtoFlashFeatures.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Provides restricted access to a collection of <see cref="IConfigurableLtoFlashFeature"/>s.
    /// </summary>
    public class ConfigurableLtoFlashFeatures
    {
        private readonly Dictionary<string, IConfigurableLtoFlashFeature> _configurableFeatures;

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurableLtoFlashFeatures"/>
        /// </summary>
        public ConfigurableLtoFlashFeatures()
        {
            _configurableFeatures = InitializeConfigurableFeatures();
        }

        /// <summary>
        /// Gets the unique identifiers of the features.
        /// </summary>
        public IEnumerable<string> FeatureIds
        {
            get { return _configurableFeatures.Keys; }
        }

        /// <summary>
        /// Gets the configurable features.
        /// </summary>
        public IEnumerable<IConfigurableLtoFlashFeature> Features
        {
            get { return _configurableFeatures.Values; }
        }

        /// <summary>
        /// Gets the <see cref="IConfigurableLtoFlashFeature"/> with the associated unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the feature.</param>
        /// <returns>The configurable feature with the unique identifier <paramref name="uniqueId"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="uniqueId"/> is <c>null</c>.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">There is no configurable feature with the unique identifier <paramref name="uniqueId"/>.</exception>
        public IConfigurableLtoFlashFeature this[string uniqueId]
        {
            get { return _configurableFeatures[uniqueId]; }
        }

        /// <summary>
        /// Gets the current value of the feature with the given unique identifier.
        /// </summary>
        /// <typeparam name="T">The data type of the feature.</typeparam>
        /// <param name="feature">The unique identifier of the requested feature.</param>
        /// <returns>The feature's current value.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="feature"/> is <c>null</c>.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">There is no configurable feature with the unique identifier <paramref name="feature"/>.</exception>
        public T GetCurrentValue<T>(string feature)
        {
            var configurableFeature = this[feature] as ConfigurableLtoFlashFeature<T>;
            var currentValue = configurableFeature.CurrentValue;
            return currentValue;
        }

        /// <summary>
        /// Update all applicable features whose values can be determined from <paramref name="newDeviceStatus"/>.
        /// </summary>
        /// <param name="newDeviceStatus">Status flags from a Locutus device that provide configurable feature values.</param>
        /// <returns>The unique identifiers of the configurable features whose values were changed via the new status.</returns>
        internal IEnumerable<string> UpdateConfigurablePropertiesFromDeviceStatus(DeviceStatusResponse newDeviceStatus)
        {
            var changedFeatures = new List<string>();
            if (newDeviceStatus != null)
            {
                foreach (var configurableFeature in _configurableFeatures.Values)
                {
                    if (configurableFeature.UpdateCurrentValue(newDeviceStatus.DeviceStatusFlags))
                    {
                        changedFeatures.Add(configurableFeature.UniqueId);
                    }
                }
            }
            return changedFeatures;
        }

        private static Dictionary<string, IConfigurableLtoFlashFeature> InitializeConfigurableFeatures()
        {
            var configurableFeatures = new IConfigurableLtoFlashFeature[]
            {
                new ConfigurableLtoFlashEcsCompatibilityFeature(),
                new ConfigurableLtoFlashIntellivisionIICompatibilityFeature(),
                new ConfigurableLtoFlashShowTitleScreenFeature(),
                new ConfigurableLtoFlashBooleanFeature(Device.BackgroundGCPropertyName, Resources.Strings.SetBackgroundGarbageCollectCommand_Name, true, DeviceStatusFlags.BackgroundGC),
                new ConfigurableLtoFlashBooleanFeature(Device.KeyclicksPropertyName, Resources.Strings.SetKeyclicksCommand_Name, false, DeviceStatusFlags.Keyclicks),
                new ConfigurableLtoFlashBooleanFeature(Device.EnableConfigMenuOnCartPropertyName, Resources.Strings.SetEnableConfigMenuOnCartCommand_Name, true, DeviceStatusFlags.EnableCartConfig),
                new ConfigurableLtoFlashBooleanFeature(Device.ZeroLtoFlashRamPropertyName, Resources.Strings.SetRandomizeLtoFlashRamCommand_Name, true, DeviceStatusFlags.ZeroRamBeforeLoad),
            };
            var configurableFeaturesDictionary = configurableFeatures.ToDictionary(f => f.UniqueId);
            return configurableFeaturesDictionary;
        }
    }
}

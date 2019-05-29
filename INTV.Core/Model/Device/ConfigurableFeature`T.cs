// <copyright file="ConfigurableFeature`T.cs" company="INTV Funhouse">
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
    /// Provides a basic abstract class for implementing IConfigurableFeature{T}.
    /// </summary>
    /// <typeparam name="T">The data type of the value for the feature.</typeparam>
    public abstract class ConfigurableFeature<T> : IConfigurableFeature, IConfigurableFeature<T>
    {
        /// <summary>
        /// Create a new instance of <see cref="ConfigurableFeature{T}"/>
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the configurable feature.</param>
        /// <param name="displayName">The localized display name of the feature.</param>
        /// <param name="factoryDefaultValue">The 'factory default' value of the feature.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="uniqueId"/> or <paramref name="displayName"/> are <c>null</c> or empty.</exception>
        protected ConfigurableFeature(string uniqueId, string displayName, T factoryDefaultValue)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                throw new System.ArgumentOutOfRangeException("uniqueId");
            }
            if (string.IsNullOrEmpty(displayName))
            {
                throw new System.ArgumentOutOfRangeException("displayName");
            }
            UniqueId = uniqueId;
            DisplayName = displayName;
            FactoryDefaultValue = factoryDefaultValue;
            _currentValue = factoryDefaultValue;
        }

        #region Properties

        #region IConfigurableFeature Properties

        /// <inheritdoc />
        object IConfigurableFeature.FactoryDefaultValue
        {
            get { return FactoryDefaultValue; }
        }

        /// <inheritdoc />
        object IConfigurableFeature.CurrentValue
        {
            get { return CurrentValue; }
        }

        #endregion // IConfigurableFeature Properties

        #region IConfigurableFeature<T> Properties

        /// <inheritdoc />
        public string DisplayName { get; private set; }

        /// <inheritdoc />
        public string UniqueId { get; private set; }

        /// <inheritdoc />
        public T FactoryDefaultValue { get; private set; }

        /// <inheritdoc />
        public virtual T CurrentValue
        {
            get
            {
                return _currentValue;
            }

            protected set
            {
                this.VerifyWriteAccess<T>();
                _currentValue = value;
            }
        }
        private T _currentValue;

        #endregion // IConfigurableFeature<T> Properties

        #endregion // Properties

        #region IConfigurableFeature Methods

        /// <inheritdoc />
        object IConfigurableFeature.GetValueFromDevice(IPeripheral device)
        {
            return GetValueFromDevice(device);
        }

        /// <inheritdoc />
        void IConfigurableFeature.SetValueOnDevice(IPeripheral device, object newValue)
        {
            this.VerifyWriteAccess<T>(); // throws if read-only
            SetValueOnDevice(device, (T)newValue);
        }

        #endregion // IConfigurableFeature Methods

        #region IConfigurableFeature<T> Methods

        /// <inheritdoc />
        public abstract T GetValueFromDevice(IPeripheral device);

        /// <inheritdoc />
        /// <remarks>Implementations should be sure to verify write access for read-only properties.</remarks>
        public abstract void SetValueOnDevice(IPeripheral device, T newValue);

        #endregion // IConfigurableFeature<T> Methods
    }
}

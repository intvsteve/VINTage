// <copyright file="IConfigurableFeature`T.cs" company="INTV Funhouse">
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
    /// Strongly type interface for <see cref="IConfigurableFeature"/>.
    /// </summary>
    /// <typeparam name="T">The data type of the value on the device.</typeparam>
    public interface IConfigurableFeature<T>
    {
        /// <inheritdoc cref="IConfigurableFeature.DisplayName"/>
        string DisplayName { get; }

        /// <inheritdoc cref="IConfigurableFeature.UniqueId"/>
        string UniqueId { get; }

        /// <inheritdoc cref="IConfigurableFeature.FactoryDefaultValue"/>
        T FactoryDefaultValue { get; }

        /// <inheritdoc cref="IConfigurableFeature.CurrentValue"/>
        T CurrentValue { get; }

        /// <inheritdoc cref="IConfigurableFeature.GetValueFromDevice(IPeripheral)"/>
        T GetValueFromDevice(IPeripheral device);

        /// <inheritdoc cref="IConfigurableFeature.SetValueOnDevice(IPeripheral, object)"/>
        void SetValueOnDevice(IPeripheral device, T newValue);
    }
}

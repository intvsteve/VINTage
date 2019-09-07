// <copyright file="Peripheral.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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
using INTV.Core.ComponentModel;
using INTV.Core.Model.Program;

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Provides a partial implementation of the IPeripheral interface.
    /// </summary>
    public abstract class Peripheral : ModelBase, IPeripheral
    {
        /// <summary>
        /// Name for the 'Name' property. Useful for those who wish to detect property changes.
        /// </summary>
        public const string NamePropertyName = "Name";

        /// <summary>
        /// Name for the 'Connections' property. Useful for those who wish to detect property changes.
        /// </summary>
        public const string ConnectionsPropertyName = "Connections";

        private string _name;

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            protected set { AssignAndUpdateProperty(NamePropertyName, value, ref _name); }
        }

        /// <inheritdoc />
        public abstract IEnumerable<IConnection> Connections { get; protected set; }

        /// <inheritdoc />
        public abstract IEnumerable<IConfigurableFeature> ConfigurableFeatures { get; }

        /// <inheritdoc />
        public abstract bool IsRomCompatible(IProgramDescription programDescription);

        /// <summary>
        /// This event is raised when a peripheral is attached.
        /// </summary>
        public static event System.EventHandler<PeripheralEventArgs> PeripheralAttached;

        /// <summary>
        /// This event is raised when a peripheral is detached.
        /// </summary>
        public static event System.EventHandler<PeripheralEventArgs> PeripheralDetached;

        /// <summary>
        /// Raises the PeripheralAttached event.
        /// </summary>
        /// <param name="peripheral">The peripheral that attached.</param>
        /// <param name="uniqueId">The unique identifier of the peripheral.</param>
        protected static void RaisePeripheralAttached(IPeripheral peripheral, string uniqueId)
        {
            var peripheralAttached = PeripheralAttached;
            if (peripheralAttached != null)
            {
                peripheralAttached(peripheral, new PeripheralEventArgs(uniqueId));
            }
        }

        /// <summary>
        /// Raises the PeripheralDetached event.
        /// </summary>
        /// <param name="peripheral">The peripheral that detached.</param>
        /// <param name="uniqueId">The unique identifier of the peripheral.</param>
        protected static void RaisePeripheralDetached(IPeripheral peripheral, string uniqueId)
        {
            var peripheralDetached = PeripheralDetached;
            if (peripheralDetached != null)
            {
                peripheralDetached(peripheral, new PeripheralEventArgs(uniqueId));
            }
        }
    }
}

// <copyright file="PeripheralEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Base event argument type for IPeripheral-related events.
    /// </summary>
    public class PeripheralEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.Device.PeripheralEventArgs"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the peripheral.</param>
        public PeripheralEventArgs(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        /// <summary>
        /// Gets the unique identifier of the peripheral that raised the event.
        /// </summary>
        public string UniqueId { get; private set; }
    }
}

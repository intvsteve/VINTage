// <copyright file="DeviceChangeEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using System.Collections.Generic;
using INTV.Core.Model.Device;

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// Data for DeviceChange events.
    /// </summary>
    public class DeviceChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the device to which the event applies.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of connection.
        /// </summary>
        public ConnectionType Type { get; private set; }

        /// <summary>
        /// Gets connection-specific information.
        /// </summary>
        public IDictionary<string, object> State { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DeviceChangeEventArgs class.
        /// </summary>
        /// <param name="name">The name of the device.</param>
        /// <param name="type">The type of the connection.</param>
        /// <param name="state">State information for use by the connection.</param>
        internal DeviceChangeEventArgs(string name, ConnectionType type, IDictionary<string, object> state)
        {
            Name = name;
            Type = type;
            State = state;
        }
    }
}

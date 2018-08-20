// <copyright file="IConfiguration.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using INTV.Core.Model.Device;

namespace INTV.Core.ComponentModel
{
    /// <summary>
    /// Component-specific configuration implements this interface.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets persistent settings.
        /// </summary>
        object Settings { get; }

        /// <summary>
        /// Gets the peripherals that have been historically connected to the system, regardless of whether currently connected.
        /// </summary>
        IEnumerable<IPeripheral> ConnectedPeripheralsHistory { get; }

        /// <summary>
        /// Perform final initialization before use.
        /// </summary>
        void Initialize();
    }
}

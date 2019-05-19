// <copyright file="IPeripheral.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Defines a peripheral that works with an Intellivision console.
    /// </summary>
    public interface IPeripheral
    {
        /// <summary>
        /// Gets the name of the peripheral.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the connections the peripheral uses or offers.
        /// </summary>
        IEnumerable<IConnection> Connections { get; }

        /// <summary>
        /// Gets the configurable features the peripheral has.
        /// </summary>
        IEnumerable<IConfigurableFeature> ConfigurableFeatures { get; }

        /// <summary>
        /// Determines whether this peripheral compatible with the specified rom.
        /// </summary>
        /// <param name="programDescription">The description of the program (ROM) whose compatibility is being checked.</param>
        /// <returns><c>true</c> if this peripheral is compatible with the specified ROM; otherwise, <c>false</c>.</returns>
        bool IsRomCompatible(IProgramDescription programDescription);
    }
}

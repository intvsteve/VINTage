// <copyright file="MemoryMap`T.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Provides a simple way to describe a memory-mapped connection using registers as keys to the memory map.
    /// </summary>
    /// <typeparam name="T">The register type, typically an enum.</typeparam>
    public abstract class MemoryMap<T> : MemoryMap
    {
        private readonly Dictionary<T, ushort> _registerMemoryMap = new Dictionary<T, ushort>();

        /// <summary>
        /// Initializes a new instance of MemoryMap.
        /// </summary>
        /// <param name="baseAddress">The address of the memory map connection.</param>
        /// <param name="size">The number of contiguous bytes mapped to a device using this connection.</param>
        /// <param name="aliases">Any aliases that may be also access the device.</param>
        protected MemoryMap(ushort baseAddress, ushort size, List<ushort> aliases)
            : base(baseAddress, size, aliases)
        {
        }

        /// <summary>
        /// Gets the registers defined in the memory map.
        /// </summary>
        public IEnumerable<T> Registers
        {
            get { return _registerMemoryMap.Keys; }
        }

        /// <summary>
        /// Given a register, return its memory-mapped location.
        /// </summary>
        /// <param name="register">The register for which to retrive the memory location.</param>
        /// <returns>The memory address that corresponds to the register.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no address is availble for the given register.</exception>
        public ushort GetMemoryLocationForRegister(T register)
        {
            return _registerMemoryMap[register];
        }
    }
}

// <copyright file="MemoryMap.cs" company="INTV Funhouse">
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
    /// Provides a simple way to describe a memory-mapped connection.
    /// </summary>
    public class MemoryMap : Connection
    {
        private readonly List<ushort> _aliases;
        private ushort _baseAddress;
        private ushort _size;

        /// <summary>
        /// Initializes a new instance of MemoryMap.
        /// </summary>
        /// <param name="baseAddress">The address of the memory map connection.</param>
        /// <param name="size">The number of contiguous bytes mapped to a device using this connection.</param>
        /// <param name="aliases">Any aliases that may be also access the device.</param>
        public MemoryMap(ushort baseAddress, ushort size, List<ushort> aliases)
            : base("Memory Map", ConnectionType.MemoryMap)
        {
            _baseAddress = baseAddress;
            _size = size;
            _aliases = aliases;
        }

        /// <summary>
        /// Gets the base address used to access a device mapped into memory.
        /// </summary>
        public ushort BaseAddress
        {
            get { return _baseAddress; }
        }

        /// <summary>
        /// Gets he number of contiguous bytes of memory mapped to the device.
        /// </summary>
        public ushort Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Gets an enumerable of other addresses that also access the device.
        /// </summary>
        public IEnumerable<ushort> Aliases
        {
            get { return _aliases; }
        }
    }
}

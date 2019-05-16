// <copyright file="ContiguousFileSystemEntries`T.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// This data structure describes an aggregation of contiguous file system entries that can be
    /// uploaded to a specific location in the RAM on a Locutus device.
    /// </summary>
    /// <typeparam name="T">The data type of the contiguous entries.</typeparam>
    internal sealed class ContiguousFileSystemEntries<T> : Tuple<uint, IList<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.Commands.ContiguousFileSystemEntries{T}"/> class.
        /// </summary>
        /// <param name="addressInRam">Address in RAM to start the contiguous entries.</param>
        /// <param name="contiguousEntries">The contiguous entries.</param>
        public ContiguousFileSystemEntries(uint addressInRam, IList<T> contiguousEntries)
            : base(addressInRam, contiguousEntries)
        {
        }

        /// <summary>
        /// Gets the address at which the contiguous block should be uploaded to RAM.
        /// </summary>
        public uint Address
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the contiguous entries.
        /// </summary>
        public IList<T> Entries
        {
            get { return Item2; }
        }
    }
}

// <copyright file="IProgramInformationTable.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Interface to a set of program information. Each entry in the table is a unique program.
    /// </summary>
    public interface IProgramInformationTable
    {
        /// <summary>
        /// Gets an enumerable of the programs in the table.
        /// </summary>
        IEnumerable<IProgramInformation> Programs { get; }

        /// <summary>
        /// Locates a program in the table given a CRC of its ROM.
        /// </summary>
        /// <param name="crc">The 32-bit CRC of the program ROM to be located.</param>
        /// <returns>The IProgramInformation if an entry with a matching CRC value is found in the table, otherwise <c>null</c>.</returns>
        IProgramInformation FindProgram(uint crc);
    }
}

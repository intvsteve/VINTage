// <copyright file="IProgramInformation.cs" company="INTV Funhouse">
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
    /// Defines an interface to describe an Intellivision program.
    /// </summary>
    public interface IProgramInformation
    {
        /// <summary>
        /// Gets a value indicating the origin of the instance.
        /// </summary>
        ProgramInformationOrigin DataOrigin { get; }

        /// <summary>
        /// Gets or sets the title of the program, independent of any ROM variations. Think of this as the base name of the program.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the vendor or other entity who produced the program.
        /// </summary>
        string Vendor { get; set; }

        /// <summary>
        /// Gets or sets the year the program was copyrighted (or released).
        /// </summary>
        string Year { get; set; }

        /// <summary>
        /// Gets or sets the features specific to the program.
        /// </summary>
        ProgramFeatures Features { get; set; }

        /// <summary>
        /// Gets or sets a short name for the program, maximum 18 characters.
        /// </summary>
        string ShortName { get; set; }

        /// <summary>
        /// Gets an enumerable of known versions of the program ROM.
        /// </summary>
        IEnumerable<CrcData> Crcs { get; }

        /// <summary>
        /// Add a new CRC value to the program information.
        /// </summary>
        /// <param name="newCrc">The new CRC by which the program may be identified.</param>
        /// <param name="crcDescription">A brief (one or two word) description, if applicable.</param>
        /// <param name="incompatibilityFlags">Known incompatibilities this ROM may have with certain hardware.</param>
        /// <returns><c>true</c> if the new CRC value was added, <c>false</c> if the CRC was already present.</returns>
        bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilityFlags);

        /// <summary>
        /// Modify an existing CRC value in the program information.
        /// </summary>
        /// <param name="crc">The CRC whose description is to be modified.</param>
        /// <param name="newCrcDescription">The new description of the CRC. If <c>null</c> the description is left unchanged.</param>
        /// <param name="newIncompatibilityFlags">The new incompatibility flags to assign.</param>
        /// <returns><c>true</c> if the new settings were applied, <c>false</c> if the CRC was not found.</returns>
        bool ModifyCrc(uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags);
    }
}

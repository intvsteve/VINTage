// <copyright file="IProgramDescription.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Defines the interface for an Intellivision program description.
    /// </summary>
    public interface IProgramDescription
    {
        /// <summary>
        /// Gets the 32-bit CRC of the program's ROM file.
        /// </summary>
        uint Crc { get; }

        /// <summary>
        /// Gets the ROM file this object describes.
        /// </summary>
        IRom Rom { get; }

        /// <summary>
        /// Gets the fundamental program information associated with the program.
        /// </summary>
        IProgramInformation ProgramInformation { get; }

        /// <summary>
        /// Gets or sets the descriptive name of the program.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a short name for the program, maximum 18 characters.
        /// </summary>
        /// <value>The short name.</value>
        string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the vendor or other entity who produced the program.
        /// </summary>
        string Vendor { get; set; }

        /// <summary>
        /// Gets or sets the year the program was copyrighted (or released).
        /// </summary>
        string Year { get; set; }

        /// <summary>
        /// Gets or sets the compatibility and features used by the program.
        /// </summary>
        ProgramFeatures Features { get; set; }
    }
}

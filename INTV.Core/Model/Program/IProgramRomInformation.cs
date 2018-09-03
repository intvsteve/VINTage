// <copyright file="IProgramRomInformation.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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
    /// Defines an interface to describe an Intellivision program ROM.
    /// </summary>
    /// <remarks>This interface is intended to eventually supersede the existing combined <see cref="IProgramDescription"/> and <see cref="IProgramInformation"/> interfaces.
    /// It is most closely aligned with the ROM information database developed for the INTV Funhouse website.</remarks>
    public interface IProgramRomInformation
    {
        /// <summary>
        /// Gets the title of the program, independent of any ROM variations. Think of this as the name on the box or cartridge label for the program.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the vendor or other entity who produced the program.
        /// </summary>
        string Vendor { get; }

        /// <summary>
        /// Gets the year the program was copyrighted (or released).
        /// </summary>
        string Year { get; }

        /// <summary>
        /// Gets the long name for the program, maximum 60 characters.
        /// </summary>
        string LongName { get; }

        /// <summary>
        /// Gets the short name for the program, maximum 18 characters.
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// Gets a short name describing the ROM variation, if applicable.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// Gets the format of the program ROM.
        /// </summary>
        RomFormat Format { get; }

        /// <summary>
        /// Gets the unique identifier of the program ROM.
        /// </summary>
        ProgramIdentifier Id { get; }

        /// <summary>
        /// Gets the features specific to the program.
        /// </summary>
        IProgramFeatures Features { get; }

        /// <summary>
        /// Gets the metadata about the program.
        /// </summary>
        IProgramMetadata Metadata { get; }
    }
}

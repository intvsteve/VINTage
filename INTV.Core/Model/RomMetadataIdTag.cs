// <copyright file="RomMetadataIdTag.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// These values identify the kind of ROM file metadata that was found in a .ROM file.
    /// The values are taken from the id_tag.txt file in the jzIntv/doc/rom_fmt directory.
    /// </summary>
    public enum RomMetadataIdTag : byte
    {
        /// <summary>Not a valid ID tag.</summary>
        None = 0x00,

        /// <summary>Pseudonym for None.</summary>
        Ignore = None,

        #region General Game Information (0x01 - 0x1F)

        /// <summary>Game title (a.k.a. Long Name).</summary>
        Title = 0x01,

        /// <summary>Publisher of the ROM.</summary>
        Publisher = 0x02,

        /// <summary>Development credits.</summary>
        Credits = 0x03,

        /// <summary>Site URL or other contact information.</summary>
        UrlContactInfo = 0x04,

        /// <summary>The release date.</summary>
        ReleaseDate = 0x05,

        /// <summary>Feature and compatibility flags.</summary>
        Features = 0x06,

        /// <summary>The controller binding information useful for emulators.</summary>
        ControllerBindings = 0x07,

        /// <summary>
        /// The "short" title for a game. Typically limited to 18 characters,
        /// which will fit using the standard GROM font on an Intellivision console.
        /// </summary>
        /// <remarks>This is an extension to the original format from 2001.</remarks>
        ShortTitle = 0x08,

        /// <summary>The release license for the ROM.</summary>
        License = 0x09,

        //// IDs 0x10 - 0x1F are reserved for future use.

        #endregion // General Game Information (0x01 - 0x1F)

        #region Debugging / Development Related Information (0x20 - 0x3F)

        /// <summary>Symbol table for the ROM image.</summary>
        SymbolTable = 0x20,

        /// <summary>The fine-grain memory attribute table.</summary>
        FineGrainMemoryAttributeTable = 0x21,

        /// <summary>The line number mapping table.</summary>
        LineNumberMappingTable = 0x22,

        //// IDs 0x23 - 0x3F are reserved for future use.

        #endregion // Debugging / Development Related Information (0x20 - 0x3F)

        #region Reserved (0x40 - 0xDF)

        //// IDs 0x40 - 0xDF are reserved for future use.

        #endregion // Reserved (0x40 - 0xDF)

        #region Extended Tags (0xF0 - 0xFF)

        //// IDs 0xF0 - 0xFF are for developer use, with custom content specified by the developer.

        #endregion // Extended Tags (0xF0 - 0xFF)
    }
}

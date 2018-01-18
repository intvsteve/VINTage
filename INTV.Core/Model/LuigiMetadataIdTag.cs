// <copyright file="LuigiMetadataIdTag.cs" company="INTV Funhouse">
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

using System;

namespace INTV.Core.Model
{
    /// <summary>
    /// ID tags for LUIGI metadata sub-blocks.
    /// </summary>
    public enum LuigiMetadataIdTag : byte
    {
        /// <summary>Name field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Name = 0x00,

        /// <summary>Short name field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        ShortName = 0x01,

        /// <summary>Author field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Author = 0x02,

        /// <summary>Publisher (vendor) field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Publisher = 0x03,

        /// <summary>Date field.</summary>
        Date = 0x04,

        /// <summary>License field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        License = 0x05,

        /// <summary>Description field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Description = 0x06,

        /// <summary>Miscellaneous data field.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Miscellaneous = 0x07,

        /// <summary>Indicates contributions to program art / graphics.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Graphics = 0x08,

        /// <summary>Indicates contributions to music.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Music = 0x09,

        /// <summary>Indicates contributions to sound effects.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        SoundEffects = 0x0A,

        /// <summary>Indicates contributions to voice acting.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        VoiceActing = 0x0B,

        /// <summary>Indicates documentation authorship.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        Documentation = 0x0C,

        /// <summary>The program concept or design.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        ConceptDesign = 0x0D,

        /// <summary>Indicates contributions to box, overlay, manual or other artwork.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        BoxOrOtherArtwork = 0x0E,

        /// <summary>Site URL or other contact information.</summary>
        /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
        UrlContactInfo = 0x0F,

        /// <summary>Unknown or unrecognized field.</summary>
        Unknown = 0xFF
    }
}

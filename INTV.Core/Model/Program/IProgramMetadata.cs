// <copyright file="IProgramMetadata.cs" company="INTV Funhouse">
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

using System.Collections.Generic;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This interface describes metadata that a program may provide. These fields are supported across
    /// the various known Intellivision ROM formats to varying degrees.
    /// </summary>
    public interface IProgramMetadata
    {
        /// <summary>
        /// Gets the long names for the program.
        /// </summary>
        /// <remarks>A practical limit to this is sixty (60) characters.</remarks>
        IEnumerable<string> LongNames { get; }

        /// <summary>
        /// Gets the short names for the program.
        /// </summary>
        /// <remarks>A practical limit to this is eighteen (18) characters.</remarks>
        IEnumerable<string> ShortNames { get; }

        /// <summary>
        /// Gets the descriptions of the program.
        /// </summary>
        IEnumerable<string> Descriptions { get; }

        /// <summary>
        /// Gets the publishers of the program.
        /// </summary>
        IEnumerable<string> Publishers { get; }

        /// <summary>
        /// Gets the developers who contributed code to the program.
        /// </summary>
        IEnumerable<string> Programmers { get; }

        /// <summary>
        /// Gets the program concept creators / designers.
        /// </summary>
        IEnumerable<string> Designers { get; }

        /// <summary>
        /// Gets the pixel artists.
        /// </summary>
        IEnumerable<string> Graphics { get; }

        /// <summary>
        /// Gets the music composers / arrangers.
        /// </summary>
        IEnumerable<string> Music { get; }

        /// <summary>
        /// Gets the sound effects designers / creators.
        /// </summary>
        IEnumerable<string> SoundEffects { get; }

        /// <summary>
        /// Gets the voice actors.
        /// </summary>
        IEnumerable<string> Voices { get; }

        /// <summary>
        /// Gets the program documentation authors.
        /// </summary>
        IEnumerable<string> Documentation { get; }

        /// <summary>
        /// Gets the program box art and other non-in-application artwork.
        /// </summary>
        IEnumerable<string> Artwork { get; }

        /// <summary>
        /// Gets the release dates of the program.
        /// </summary>
        IEnumerable<MetadataDateTime> ReleaseDates { get; }

        /// <summary>
        /// Gets the licenses under which the program and possibly its source code are released.
        /// </summary>
        IEnumerable<string> Licenses { get; }

        /// <summary>
        /// Gets the contact or other information regarding the program.
        /// </summary>
        IEnumerable<string> ContactInformation { get; }

        /// <summary>
        /// Gets the short version descriptions, e.g. 1.0.1 or alpha 0, et. al.
        /// </summary>
        IEnumerable<string> Versions { get; }

        /// <summary>
        /// Gets the build dates for the program.
        /// </summary>
        IEnumerable<MetadataDateTime> BuildDates { get; }

        /// <summary>
        /// Gets other miscellaneous additional information.
        /// </summary>
        IEnumerable<string> AdditionalInformation { get; }
    }
}

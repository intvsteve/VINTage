// <copyright file="IProgramMetadataFieldId.cs" company="INTV Funhouse">
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
    /// These identifiers are a simple way to map to the corresponding data in an instance of <see cref="IProgramMetadata"/>.
    /// </summary>
    public enum IProgramMetadataFieldId : byte
    {
        /// <summary>An invalid metadata tag.</summary>
        None = 0,

        /// <summary>Refers to <see cref="IProgramMetadata.LongNames"/></summary>
        LongNames,

        /// <summary>Refers to <see cref="IProgramMetadata.ShortNames"/></summary>
        ShortNames,

        /// <summary>Refers to <see cref="IProgramMetadata.Descriptions"/></summary>
        Descriptions,

        /// <summary>Refers to <see cref="IProgramMetadata.Publishers"/></summary>
        Publishers,

        /// <summary>Refers to <see cref="IProgramMetadata.Programmers"/></summary>
        Programmers,

        /// <summary>Refers to <see cref="IProgramMetadata.Designers"/></summary>
        Designers,

        /// <summary>Refers to <see cref="IProgramMetadata.Graphics"/></summary>
        Graphics,

        /// <summary>Refers to <see cref="IProgramMetadata.Music"/></summary>
        Music,

        /// <summary>Refers to <see cref="IProgramMetadata.SoundEffects"/></summary>
        SoundEffects,

        /// <summary>Refers to <see cref="IProgramMetadata.Voices"/></summary>
        Voices,

        /// <summary>Refers to <see cref="IProgramMetadata.Documentation"/></summary>
        Documentation,

        /// <summary>Refers to <see cref="IProgramMetadata.Artwork"/></summary>
        Artwork,

        /// <summary>Refers to <see cref="IProgramMetadata.ReleaseDates"/></summary>
        ReleaseDates,

        /// <summary>Refers to <see cref="IProgramMetadata.Licenses"/></summary>
        Licenses,

        /// <summary>Refers to <see cref="IProgramMetadata.ContactInformation"/></summary>
        ContactInformation,

        /// <summary>Refers to <see cref="IProgramMetadata.Versions"/></summary>
        Versions,

        /// <summary>Refers to <see cref="IProgramMetadata.BuildDates"/></summary>
        BuildDates,

        /// <summary>Refers to <see cref="IProgramMetadata.AdditionalInformation"/></summary>
        AdditionalInformation,
    }
}

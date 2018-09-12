// <copyright file="IProgramMetadataBuilder.cs" company="INTV Funhouse">
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
    /// This interface specifies a builder pattern for creating an instance of <see cref="IProgramMetadata"/>.
    /// </summary>
    public interface IProgramMetadataBuilder
    {
        /// <summary>
        /// Adds an enumerable of long program names to the metadata.
        /// </summary>
        /// <param name="longNames">The long program names to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithLongNames(IEnumerable<string> longNames);

        /// <summary>
        /// Adds an enumerable of short program names to the metadata.
        /// </summary>
        /// <param name="shortNames">The short program names to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithShortNames(IEnumerable<string> shortNames);

        /// <summary>
        /// Adds an enumerable of descriptions to the metadata.
        /// </summary>
        /// <param name="descriptions">The descriptions to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithDescriptions(IEnumerable<string> descriptions);

        /// <summary>
        /// Adds an enumerable of publishers to the metadata.
        /// </summary>
        /// <param name="publishers">The publishers to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithPublishers(IEnumerable<string> publishers);

        /// <summary>
        /// Adds an enumerable of programmer names to the metadata.
        /// </summary>
        /// <param name="programmers">The programmer names to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithProgrammers(IEnumerable<string> programmers);

        /// <summary>
        /// Adds an enumerable of designers to the metadata.
        /// </summary>
        /// <param name="designers">The designer names to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithDesigners(IEnumerable<string> designers);

        /// <summary>
        /// Adds an enumerable of graphics artists to the metadata.
        /// </summary>
        /// <param name="graphicsArtists">The graphics artists to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithGraphics(IEnumerable<string> graphicsArtists);

        /// <summary>
        /// Adds an enumerable of music contributors to the metadata.
        /// </summary>
        /// <param name="musicContributors">The music contributors to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithMusic(IEnumerable<string> musicContributors);

        /// <summary>
        /// Adds an enumerable of sound effects artists to the metadata.
        /// </summary>
        /// <param name="soundEffectsArtists">The sound effects artists to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithSoundEffects(IEnumerable<string> soundEffectsArtists);

        /// <summary>
        /// Adds an enumerable of voice actors to the metadata.
        /// </summary>
        /// <param name="voiceActors">The voice actors to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithVoices(IEnumerable<string> voiceActors);

        /// <summary>
        /// Adds an enumerable of documentation authors to the metadata.
        /// </summary>
        /// <param name="documentationAuthors">The documentation authors to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithDocumentation(IEnumerable<string> documentationAuthors);

        /// <summary>
        /// Adds an enumerable of artwork contributors to the metadata.
        /// </summary>
        /// <param name="artworkContributors">The artwork contributors to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithArtwork(IEnumerable<string> artworkContributors);

        /// <summary>
        /// Adds an enumerable of release dates to the metadata.
        /// </summary>
        /// <param name="releaseDates">The release dates to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithReleaseDates(IEnumerable<MetadataDateTime> releaseDates);

        /// <summary>
        /// Adds an enumerable of licenses to the metadata.
        /// </summary>
        /// <param name="licenses">The licenses to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithLicenses(IEnumerable<string> licenses);

        /// <summary>
        /// Adds an enumerable of contact information entries to the metadata.
        /// </summary>
        /// <param name="contactInformation">The contact information data to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithContactInformation(IEnumerable<string> contactInformation);

        /// <summary>
        /// Adds an enumerable of version names to the metadata.
        /// </summary>
        /// <param name="versions">The version names to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithVersions(IEnumerable<string> versions);

        /// <summary>
        /// Adds an enumerable of build dates to the metadata.
        /// </summary>
        /// <param name="buildDates">The build dates to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithBuildDates(IEnumerable<MetadataDateTime> buildDates);

        /// <summary>
        /// Adds an enumerable of additional information entries to the metadata.
        /// </summary>
        /// <param name="additionalInformation">The additional information data to add.</param>
        /// <returns>The builder.</returns>
        IProgramMetadataBuilder WithAdditionalInformation(IEnumerable<string> additionalInformation);

        /// <summary>
        /// Creates the concrete instance of program metadata.
        /// </summary>
        /// <returns>The <see cref="IProgramMetadata"/> containing the information provided during the build process.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the metadata's configuration is invalid. Consult the message in
        /// the exception for detailed information.</exception>
        IProgramMetadata Build();
    }
}

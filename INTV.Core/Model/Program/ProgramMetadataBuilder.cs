// <copyright file="ProgramMetadataBuilder.cs" company="INTV Funhouse">
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
    public class ProgramMetadataBuilder : IProgramMetadataBuilder
    {
        private ProgramMetadata _programMetadata = new ProgramMetadata();

        public IProgramMetadataBuilder WithInitialMetadata(IProgramMetadata initialMetadata)
        {
            if (initialMetadata != null)
            {
                _programMetadata = new ProgramMetadata();
                WithLongNames(initialMetadata.LongNames);
                WithShortNames(initialMetadata.ShortNames);
                WithDescriptions(initialMetadata.Descriptions);
                WithPublishers(initialMetadata.Publishers);
                WithProgrammers(initialMetadata.Programmers);
                WithDesigners(initialMetadata.Designers);
                WithGraphics(initialMetadata.Graphics);
                WithMusic(initialMetadata.Music);
                WithSoundEffects(initialMetadata.SoundEffects);
                WithVoices(initialMetadata.Voices);
                WithDocumentation(initialMetadata.Documentation);
                WithArtwork(initialMetadata.Artwork);
                WithReleaseDates(initialMetadata.ReleaseDates);
                WithLicenses(initialMetadata.Licenses);
                WithContactInformation(initialMetadata.ContactInformation);
                WithVersions(initialMetadata.Versions);
                WithBuildDates(initialMetadata.BuildDates);
                WithAdditionalInformation(initialMetadata.AdditionalInformation);
            }
            return this;
        }

        #region IProgramMetadataBuilder

        /// <inheritdoc />
        public IProgramMetadataBuilder WithLongNames(IEnumerable<string> longNames)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.LongNames, longNames);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithShortNames(IEnumerable<string> shortNames)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.ShortNames, shortNames);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithDescriptions(IEnumerable<string> descriptions)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Descriptions, descriptions);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithPublishers(IEnumerable<string> publishers)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Publishers, publishers);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithProgrammers(IEnumerable<string> programmers)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Programmers, programmers);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithDesigners(IEnumerable<string> designers)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Designers, designers);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithGraphics(IEnumerable<string> graphicsArtists)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Graphics, graphicsArtists);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithMusic(IEnumerable<string> musicContributors)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Music, musicContributors);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithSoundEffects(IEnumerable<string> soundEffectsArtists)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.SoundEffects, soundEffectsArtists);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithVoices(IEnumerable<string> voiceActors)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Voices, voiceActors);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithDocumentation(IEnumerable<string> documentationAuthors)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Documentation, documentationAuthors);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithArtwork(IEnumerable<string> artworkContributors)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Artwork, artworkContributors);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithReleaseDates(IEnumerable<MetadataDateTime> releaseDates)
        {
            _programMetadata.ReplaceReleaseDates(releaseDates);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithLicenses(IEnumerable<string> licenses)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Licenses, licenses);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithContactInformation(IEnumerable<string> contactInformation)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.ContactInformation, contactInformation);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithVersions(IEnumerable<string> versions)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.Versions, versions);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithBuildDates(IEnumerable<MetadataDateTime> buildDates)
        {
            _programMetadata.ReplaceBuildDates(buildDates);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadataBuilder WithAdditionalInformation(IEnumerable<string> additionalInformation)
        {
            _programMetadata.ReplaceValue(IProgramMetadataFieldId.AdditionalInformation, additionalInformation);
            return this;
        }

        /// <inheritdoc />
        public IProgramMetadata Build()
        {
            return _programMetadata;
        }

        #endregion // IProgramMetadataBuilder
    }
}

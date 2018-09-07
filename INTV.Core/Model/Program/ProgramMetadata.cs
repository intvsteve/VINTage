// <copyright file="ProgramMetadata.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Implements <see cref="IProgramMetadata"/>.
    /// </summary>
    internal class ProgramMetadata : IProgramMetadata
    {
        #region IProgramMetadata

        /// <inheritdoc />
        public IEnumerable<string> LongNames
        {
            get { return _longNames; }
        }
        private HashSet<string> _longNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> ShortNames
        {
            get { return _shortNames; }
        }
        private HashSet<string> _shortNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Descriptions
        {
            get { return _descriptions; }
        }
        private HashSet<string> _descriptions = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Publishers
        {
            get { return _publishers; }
        }
        private HashSet<string> _publishers = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Programmers
        {
            get { return _programmers; }
        }
        private HashSet<string> _programmers = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Designers
        {
            get { return _designers; }
        }
        private HashSet<string> _designers = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Graphics
        {
            get { return _graphics; }
        }
        private HashSet<string> _graphics = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Music
        {
            get { return _music; }
        }
        private HashSet<string> _music = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> SoundEffects
        {
            get { return _soundEffects; }
        }
        private HashSet<string> _soundEffects = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Voices
        {
            get { return _voices; }
        }
        private HashSet<string> _voices = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Documentation
        {
            get { return _documentation; }
        }
        private HashSet<string> _documentation = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Artwork
        {
            get { return _artwork; }
        }
        private HashSet<string> _artwork = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return _releaseDates; }
        }
        private HashSet<MetadataDateTime> _releaseDates = new HashSet<MetadataDateTime>();

        /// <inheritdoc />
        public IEnumerable<string> Licenses
        {
            get { return _licenses; }
        }
        private HashSet<string> _licenses = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> ContactInformation
        {
            get { return _contactInformation; }
        }
        private HashSet<string> _contactInformation = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<string> Versions
        {
            get { return _versions; }
        }
        private HashSet<string> _versions = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <inheritdoc />
        public IEnumerable<MetadataDateTime> BuildDates
        {
            get { return _buildDates; }
        }
        private HashSet<MetadataDateTime> _buildDates = new HashSet<MetadataDateTime>();

        /// <inheritdoc />
        public IEnumerable<string> AdditionalInformation
        {
            get { return _additionalInformation; }
        }
        private HashSet<string> _additionalInformation = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        #endregion // IProgramMetadata

        /// <summary>
        /// General-purpose method to replace in-toto a particular metadata value that is represented as <see cref="IEnumerable{string}"/>.
        /// </summary>
        /// <param name="field">Identifier of the metadata field to replace.</param>
        /// <param name="newValue">The replacement data for the field.</param>
        internal void ReplaceValue(IProgramMetadataFieldId field, IEnumerable<string> newValue)
        {
            if (newValue == null)
            {
                newValue = Enumerable.Empty<string>();
            }
            var value = new HashSet<string>(newValue, StringComparer.CurrentCultureIgnoreCase);
            switch (field)
            {
                case IProgramMetadataFieldId.LongNames:
                    _longNames = value;
                    break;
                case IProgramMetadataFieldId.ShortNames:
                    _shortNames = value;
                    break;
                case IProgramMetadataFieldId.Descriptions:
                    _descriptions = value;
                    break;
                case IProgramMetadataFieldId.Publishers:
                    _publishers = value;
                    break;
                case IProgramMetadataFieldId.Programmers:
                   _programmers  = value;
                    break;
                case IProgramMetadataFieldId.Designers:
                    _designers = value;
                    break;
                case IProgramMetadataFieldId.Graphics:
                    _graphics = value;
                    break;
                case IProgramMetadataFieldId.Music:
                    _music = value;
                    break;
                case IProgramMetadataFieldId.SoundEffects:
                    _soundEffects = value;
                    break;
                case IProgramMetadataFieldId.Voices:
                    _voices = value;
                    break;
                case IProgramMetadataFieldId.Documentation:
                    _documentation = value;
                    break;
                case IProgramMetadataFieldId.Artwork:
                    _artwork = value;
                    break;
                case IProgramMetadataFieldId.Licenses:
                    _licenses = value;
                    break;
                case IProgramMetadataFieldId.ContactInformation:
                    _contactInformation = value;
                    break;
                case IProgramMetadataFieldId.Versions:
                    _versions = value;
                    break;
                case IProgramMetadataFieldId.AdditionalInformation:
                    _additionalInformation = value;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Replace the release dates metadata.
        /// </summary>
        /// <param name="releaseDates">The new values for the release dates metadata.</param>
        internal void ReplaceReleaseDates(IEnumerable<MetadataDateTime> releaseDates)
        {
            if (releaseDates == null)
            {
                releaseDates = Enumerable.Empty<MetadataDateTime>();
            }
            _releaseDates = new HashSet<MetadataDateTime>(releaseDates);
        }

        /// <summary>
        /// Replace the build dates metadata.
        /// </summary>
        /// <param name="buildDates">The new values for the build dates metadata.</param>
        internal void ReplaceBuildDates(IEnumerable<MetadataDateTime> buildDates)
        {
            if (buildDates == null)
            {
                buildDates = Enumerable.Empty<MetadataDateTime>();
            }
            _buildDates = new HashSet<MetadataDateTime>(buildDates);
        }
    }
}

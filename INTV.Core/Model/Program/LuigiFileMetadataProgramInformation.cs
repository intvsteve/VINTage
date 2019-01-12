// <copyright file="LuigiFileMetadataProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2016-2019 All Rights Reserved
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
    /// This class implements IProgramInformation based on data from a LUIGI ROM.
    /// </summary>
    public class LuigiFileMetadataProgramInformation : ProgramInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.Program.LuigiFileMetadataProgramInformation"/> class.
        /// </summary>
        /// <param name="header">A LUIGI file header that describes the ROM's features.</param>
        /// <param name="metadata">Additional ROM metadata, if any.</param>
        public LuigiFileMetadataProgramInformation(LuigiFileHeader header, LuigiMetadataBlock metadata)
        {
            _features = ProgramFeatures.Combine(header.Features.ToProgramFeatures(), header.Features2.ToProgramFeatures());
            _crc = new CrcData(header.OriginalRomCrc32, string.Empty, _features.ToIncompatibilityFlags());
            Metadata = metadata;
            if (metadata != null)
            {
                _title = metadata.LongNames.FirstOrDefault();
                _vendor = metadata.Publishers.FirstOrDefault();
                if (metadata.ReleaseDates.Any())
                {
                    _year = metadata.ReleaseDates.First().Date.Year.ToString();
                }
                _shortName = metadata.ShortNames.FirstOrDefault();
            }
        }

        #region Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return ProgramInformationOrigin.LuigiMetadataBlock; }
        }

        /// <inheritdoc />
        public override string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private string _title;

        /// <inheritdoc />
        public override string Vendor
        {
            get { return _vendor; }
            set { _vendor = value; }
        }
        private string _vendor;

        /// <inheritdoc />
        public override string Year
        {
            get { return _year; }
            set { _year = value; }
        }
        private string _year;

        /// <inheritdoc />
        public override ProgramFeatures Features
        {
            get { return _features; }
            set { _features = value; }
        }
        private ProgramFeatures _features;

        /// <inheritdoc />
        public override string ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }
        private string _shortName;

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get { yield return _crc; }
        }
        private CrcData _crc;

        #endregion // IProgramInformation

        #region IProgramMetadata

        /// <inheritdoc />
        public override IEnumerable<string> LongNames
        {
            get { return Metadata.LongNames; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ShortNames
        {
            get { return Metadata.ShortNames; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Descriptions
        {
            get { return Metadata.Descriptions; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Publishers
        {
            get { return Metadata.Publishers; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Programmers
        {
            get { return Metadata.Authors; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Designers
        {
            get { return Metadata.ConceptAndDesignCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Graphics
        {
            get { return Metadata.GraphicsCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Music
        {
            get { return Metadata.MusicCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> SoundEffects
        {
            get { return Metadata.SoundEffectsCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Voices
        {
            get { return Metadata.VoiceActingCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Documentation
        {
            get { return Metadata.DocumentationCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Artwork
        {
            get { return Metadata.BoxOrOtherArtworkCredits; }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return Metadata.ReleaseDates; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Licenses
        {
            get { return Metadata.Licenses; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ContactInformation
        {
            get { return Metadata.UrlContactInfos; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Versions
        {
            get { return Metadata.Versions; }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> BuildDates
        {
            get { return Metadata.BuildDates; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> AdditionalInformation
        {
            get { yield break; }
        }

        #endregion // IProgramMetadata

        private LuigiMetadataBlock Metadata { get; set; }

        #endregion // Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new InvalidOperationException();
        }

        #endregion // IProgramInformation
    }
}

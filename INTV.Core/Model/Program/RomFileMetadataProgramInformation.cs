// <copyright file="RomFileMetadataProgramInformation.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This class implements IProgramInformation based on metadata from a .ROM-format ROM.
    /// </summary>
    public class RomFileMetadataProgramInformation : ProgramInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.Program.RomFileMetadataProgramInformation"/> class.
        /// </summary>
        /// <param name="rom">The ROM whose metadata program information is desired. If not a RomFormatRom, the
        /// metadata will be empty, and the features will be generic unrecognized ROM features.</param>
        public RomFileMetadataProgramInformation(IRom rom)
        {
            _features = ProgramFeatures.GetUnrecognizedRomFeatures();
            Metadata = Enumerable.Empty<RomMetadataBlock>();
            var romFormatRom = Rom.AsSpecificRomType<RomFormatRom>(rom);
            if (romFormatRom != null)
            {
                Metadata = romFormatRom.Metadata;
                var stringMetaData = Metadata.FirstOrDefault(m => m.Type == RomMetadataIdTag.Title) as RomMetadataString;
                if ((stringMetaData != null) && !string.IsNullOrEmpty(stringMetaData.StringValue))
                {
                    _title = stringMetaData.StringValue;
                }
                stringMetaData = Metadata.FirstOrDefault(m => m.Type == RomMetadataIdTag.ShortTitle) as RomMetadataString;
                if ((stringMetaData != null) && !string.IsNullOrEmpty(stringMetaData.StringValue))
                {
                    _shortName = stringMetaData.StringValue;
                }
                var date = Metadata.OfType<RomMetadataDate>().FirstOrDefault(d => d.Type == RomMetadataIdTag.ReleaseDate);
                if ((date != null) && date.Date.Flags.HasFlag(MetadataDateTimeFlags.Year))
                {
                    _year = date.Date.Date.Year.ToString();
                }
                var vendor = Metadata.OfType<RomMetadataPublisher>().FirstOrDefault();
                if (vendor != null)
                {
                    _vendor = vendor.Publisher;
                }
                var features = Metadata.OfType<RomMetadataFeatures>().FirstOrDefault();
                if (features != null)
                {
                    _features = features.Features;
                }
                _crc = new CrcData(romFormatRom.Crc, string.Empty, _features.ToIncompatibilityFlags());
            }
        }

        #region Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return ProgramInformationOrigin.RomMetadataBlock; }
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

        #endregion IProgramInformation

        #region IProgramMetadata

        /// <inheritdoc />
        public override IEnumerable<string> LongNames
        {
            get { return Metadata.OfType<RomMetadataString>().Where(m => m.Type == RomMetadataIdTag.Title).Select(m => m.StringValue); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ShortNames
        {
            get { return Metadata.OfType<RomMetadataString>().Where(m => m.Type == RomMetadataIdTag.ShortTitle).Select(m => m.StringValue); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Descriptions
        {
            get { return Metadata.OfType<RomMetadataString>().Where(m => m.Type == RomMetadataIdTag.Description).Select(m => m.StringValue); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Publishers
        {
            get { return Metadata.OfType<RomMetadataPublisher>().Where(m => m.Type == RomMetadataIdTag.Publisher).Select(m => m.Publisher); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Programmers
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.Programming); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Designers
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.GameConceptDesign); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Graphics
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.Graphics); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Music
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.Music); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> SoundEffects
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.SoundEffects); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Voices
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.VoiceActing); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Documentation
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.Documentation); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Artwork
        {
            get { return Metadata.OfType<RomMetadataCredits>().SelectMany(c => c.BoxOrOtherArtwork); }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return Metadata.OfType<RomMetadataDate>().Where(m => m.Type == RomMetadataIdTag.ReleaseDate).Select(m => m.Date); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Licenses
        {
            get { return Metadata.OfType<RomMetadataString>().Where(m => m.Type == RomMetadataIdTag.License).Select(m => m.StringValue); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ContactInformation
        {
            get { return Metadata.OfType<RomMetadataString>().Where(m => m.Type == RomMetadataIdTag.UrlContactInfo).Select(m => m.StringValue); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Versions
        {
            get { return Metadata.OfType<RomMetadataString>().Where(m => m.Type == RomMetadataIdTag.Version).Select(m => m.StringValue); }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> BuildDates
        {
            get { return Metadata.OfType<RomMetadataDate>().Where(m => m.Type == RomMetadataIdTag.BuildDate).Select(m => m.Date); }
        }

        /// <inheritdoc />
        public override IEnumerable<string> AdditionalInformation
        {
            get { yield break; }
        }

        #endregion // IProgramMetadata

        /// <summary>
        /// Gets all the metadata in its originally parsed form.
        /// </summary>
        internal IEnumerable<RomMetadataBlock> Metadata { get; private set; }

        #endregion Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new System.NotImplementedException();
        }

        #endregion // IProgramInformation
    }
}

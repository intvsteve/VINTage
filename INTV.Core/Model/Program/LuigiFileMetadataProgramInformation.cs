// <copyright file="LuigiFileMetadataProgramInformation.cs" company="INTV Funhouse">
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
        private string _title;
        private string _vendor;
        private string _year;
        private CrcData _crc;
        private ProgramFeatures _features;

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
                if (metadata.Dates.Any())
                {
                    _year = metadata.Dates.First().Date.Year.ToString();
                }
                ShortName = metadata.ShortNames.FirstOrDefault();
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

        /// <inheritdoc />
        public override string Vendor
        {
            get { return _vendor; }
            set { _vendor = value; }
        }

        /// <inheritdoc />
        public override string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        /// <inheritdoc />
        public override ProgramFeatures Features
        {
            get { return _features; }
            set { _features = value; }
        }

        /// <inheritdoc />
        public override IEnumerable<CrcData> Crcs
        {
            get { yield return _crc; }
        }

        #endregion // IProgramInformation

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public IEnumerable<string> Authors
        {
            get { return Metadata.Authors; }
        }

        /// <summary>
        /// Gets the graphics artists.
        /// </summary>
        public IEnumerable<string> Graphics
        {
            get { return Metadata.GraphicsCredits; }
        }

        /// <summary>
        /// Gets the music credits.
        /// </summary>
        public IEnumerable<string> Music
        {
            get { return Metadata.MusicCredits; }
        }

        /// <summary>
        /// Gets the sound effects credits.
        /// </summary>
        public IEnumerable<string> SoundEffects
        {
            get { return Metadata.SoundEffectsCredits; }
        }

        /// <summary>
        /// Gets the voice acting credits.
        /// </summary>
        public IEnumerable<string> Voices
        {
            get { return Metadata.VoiceActingCredits; }
        }

        /// <summary>
        /// Gets the documentation credits.
        /// </summary>
        public IEnumerable<string> Documentation
        {
            get { return Metadata.DocumentationCredits; }
        }

        /// <summary>
        /// Gets the artwork credits for boxes, et. al.
        /// </summary>
        public IEnumerable<string> Artwork
        {
            get { return Metadata.BoxOrOtherArtworkCredits; }
        }

        /// <summary>
        /// Gets the program concept credits.
        /// </summary>
        public IEnumerable<string> Concept
        {
            get { return Metadata.ConceptAndDesignCredits; }
        }

        /// <summary>
        /// Gets the 'more info' values.
        /// </summary>
        public IEnumerable<string> MoreInfo
        {
            get { return Metadata.UrlContactInfos; }
        }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        public IEnumerable<string> Publishers
        {
            get { return Metadata.Publishers; }
        }

        /// <summary>
        /// Gets the release licenses.
        /// </summary>
        public IEnumerable<string> Licenses
        {
            get { return Metadata.Licenses; }
        }

        /// <summary>
        /// Gets the release dates.
        /// </summary>
        public IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return Metadata.Dates; }
        }

        /// <summary>
        /// Gets the build dates.
        /// </summary>
        public IEnumerable<MetadataDateTime> BuildDates
        {
            get { return Metadata.BuildDates; }
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        public IEnumerable<string> Versions
        {
            get { return Metadata.Versions; }
        }

        private LuigiMetadataBlock Metadata { get; set; }

        #endregion // Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new NotImplementedException();
        }

        #endregion // IProgramInformation
    }
}

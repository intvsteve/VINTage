// <copyright file="RomFileMetadataProgramInformation.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This class implements IProgramInformation based on metadata from a .ROM-format ROM.
    /// </summary>
    public class RomFileMetadataProgramInformation : ProgramInformation
    {
        private string _title;
        private string _vendor;
        private string _year;
        private CrcData _crc;
        private ProgramFeatures _features;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.Program.RomFileMetadataProgramInformation"/> class.
        /// </summary>
        /// <param name="rom">The ROM from which program information is collected.</param>
        public RomFileMetadataProgramInformation(IRom rom)
        {
            _features = ProgramFeatures.EmptyFeatures.Clone();
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
                    ShortName = stringMetaData.StringValue;
                }
                var date = Metadata.OfType<RomMetadataPublicationDate>().FirstOrDefault();
                if (date != null)
                {
                    _year = date.Year.ToString();
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

        #endregion IProgramInformation

        /// <summary>
        /// Gets all the metadata in its originally parsed form.
        /// </summary>
        public IEnumerable<RomMetadataBlock> Metadata { get; private set; }

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

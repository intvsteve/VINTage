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
                if (metadata.Years.Any())
                {
                    _year = metadata.Years.First().ToString();
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

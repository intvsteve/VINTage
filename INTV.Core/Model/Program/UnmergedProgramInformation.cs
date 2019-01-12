// <copyright file="UnmergedProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Provides an implementation of IProgramInformation for programs that are not yet part of the INTV Funhouse database.
    /// </summary>
    internal class UnmergedProgramInformation : ProgramInformation
    {
        /// <summary>
        /// Creates a new instance of the UnmergedProgramInformation class.
        /// </summary>
        /// <param name="code">INTV Funhouse database code of the program.</param>
        /// <param name="title">Title of the program.</param>
        /// <param name="vendor">Program vendor.</param>
        /// <param name="year">The year the program was copyrighted (or released).</param>
        /// <param name="crcData">Known CRC values for the program ROM variants.</param>
        /// <param name="crcDescriptions">Descriptions of the ROM variations.</param>
        /// <param name="crcCfgs">The default .cfg file to use if one is not provided.</param>
        /// <param name="features">The program's features.</param>
        internal UnmergedProgramInformation(string code, string title, string vendor, string year, uint[] crcData, string[] crcDescriptions, int[] crcCfgs, ProgramFeatures features)
        {
            Code = code;
            _title = title;
            _vendor = vendor;
            _year = year;
            _crc = new List<CrcData>();
            for (int i = 0; i < crcData.Length; ++i)
            {
                var crcDescription = string.Empty;
                if (i < crcDescriptions.Length)
                {
                    crcDescription = crcDescriptions[i];
                }
                _crc.Add(new CrcData(crcData[i], crcDescription, IncompatibilityFlags.None, crcCfgs[i]));
            }
            _features = features;
        }

        #region Properties

        #region IProgramInformation

        /// <inheritdoc />
        public override ProgramInformationOrigin DataOrigin
        {
            get { return ProgramInformationOrigin.JzIntv; }
        }

        /// <inheritdoc />
        public override string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private string _title;

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
            get { return _crc; }
        }
        private List<CrcData> _crc;

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

        #endregion // IProgramInformation

        #region IProgramMetadata

        /// <inheritdoc />
        public override IEnumerable<string> LongNames
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ShortNames
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Descriptions
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Publishers
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Programmers
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Designers
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Graphics
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Music
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> SoundEffects
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Voices
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Documentation
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Artwork
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Licenses
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> ContactInformation
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> Versions
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<MetadataDateTime> BuildDates
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override IEnumerable<string> AdditionalInformation
        {
            get { yield break; }
        }

        #endregion // IProgramMetadata

        /// <summary>
        /// Gets the proposed INTV Funhouse database code for the program.
        /// </summary>
        public string Code { get; private set; }

        #endregion // Properties

        #region IProgramInformation Methods

        /// <inheritdoc />
        public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
        {
            throw new NotImplementedException();
        }

        #endregion // IProgramInformation Methods
    }
}

// <copyright file="ProgramInformation.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
    /// Provides partial implementation of IProgramInformation with some useful additions to it.
    /// </summary>
    public abstract class ProgramInformation : IProgramInformation, IProgramMetadata
    {
        /// <summary>
        /// Default name to use for an unrecognized program.
        /// </summary>
        public static readonly string UnknownProgramTitle = Resources.Strings.ProgramInformation_DefaultTitle;

        #region Properties

        #region IProgramInformation Properties

        /// <inheritdoc />
        public abstract ProgramInformationOrigin DataOrigin { get; }

        /// <inheritdoc />
        public abstract string Title { get; set; }

        /// <inheritdoc />
        public virtual string Vendor { get; set; }

        /// <inheritdoc />
        public abstract string Year { get; set; }

        /// <inheritdoc />
        public abstract ProgramFeatures Features { get; set; }

        /// <inheritdoc />
        public string ShortName { get; set; }

        /// <inheritdoc />
        public abstract IEnumerable<CrcData> Crcs { get; }

        #endregion // IProgramInformation Properties

        #region IProgramMetadata Properties

        /// <inheritdoc />
        public abstract IEnumerable<string> LongNames { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> ShortNames { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Descriptions { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Publishers { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Programmers { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Designers { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Graphics { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Music { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> SoundEffects { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Voices { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Documentation { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Artwork { get; }

        /// <inheritdoc />
        public abstract IEnumerable<MetadataDateTime> ReleaseDates { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Licenses { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> ContactInformation { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> Versions { get; }

        /// <inheritdoc />
        public abstract IEnumerable<MetadataDateTime> BuildDates { get; }

        /// <inheritdoc />
        public abstract IEnumerable<string> AdditionalInformation { get; }

        #endregion // IProgramMetadata Properties

        #endregion // Properties

        #region IProgramInformation Methods

        /// <inheritdoc />
        public abstract bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities);

        /// <inheritdoc />
        public virtual bool ModifyCrc(uint crc, string newCrcDescription, IncompatibilityFlags newIncompatibilityFlags)
        {
            return IProgramInformationHelpers.ModifyCrc(this, crc, newCrcDescription, newIncompatibilityFlags);
        }

        #endregion // IProgramInformation Methods
    }
}

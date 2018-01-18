// <copyright file="RomMetadataDate.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for general date metadata in a .ROM-format ROM.
    /// </summary>
    public class RomMetadataDate : RomMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataDate"/> class.
        /// </summary>
        /// <param name="length">Length of the payload in bytes.</param>
        /// <param name="type">The specific kind of metadata in the block.</param>
        public RomMetadataDate(uint length, RomMetadataIdTag type)
            : base(length, type)
        {
            switch (type)
            {
                case RomMetadataIdTag.BuildDate:
                case RomMetadataIdTag.ReleaseDate:
                    break;
                default:
                    throw new InvalidOperationException(string.Format(Resources.Strings.RomMetadataDate_InvalidMetadataIdFormat, type));
            }
            Date = MetadataDateTime.MinValue;
        }

        #region Properties

        /// <summary>
        /// Gets the date that was encoded in the metadata.
        /// </summary>
        public MetadataDateTime Date { get; private set; }

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var parsedDateMetadata = reader.ParseDateTimeFromMetadata(Length);
            Date = parsedDateMetadata;
            return Length;
        }

        #endregion // RomMetadataBlock
    }
}

// <copyright file="RomMetadataPublicationDate.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for publication data metadata in a .ROM-format ROM.
    /// </summary>
    public class RomMetadataPublicationDate : RomMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataPublicationDate"/> class.
        /// </summary>
        /// <param name="length">Length of the payload in bytes.</param>
        public RomMetadataPublicationDate(uint length)
            : base(length, RomMetadataIdTag.ReleaseDate)
        {
            Date = DateTime.MinValue;
        }

        #region Properties

        /// <summary>
        /// Gets the publication date.
        /// </summary>
        /// <remarks>If month and date were not defined, June 15 is the magical date. If we somehow encountered this metadata
        /// and were unable to retrieve anything useful, then the value will be System.DateTime.MinValue.</remarks>
        public DateTime Date { get; private set; }

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var year = 0;
            var remainingPayload = Length;
            if (remainingPayload > 0)
            {
                year = (ushort)(1900 + reader.ReadByte());
                --remainingPayload;
            }

            var month = 6;
            if (remainingPayload > 0)
            {
                month = reader.ReadByte();
                --remainingPayload;
                if ((month < 1) || (month > 12))
                {
                    month = 6;
                }
            }

            var day = 15;
            if (remainingPayload > 0)
            {
                day = reader.ReadByte();
                --remainingPayload;
                if ((day < 1) || (day > DateTime.DaysInMonth(year, month)))
                {
                    day = 15;
                }
            }
            if (remainingPayload > 0)
            {
                System.Diagnostics.Debug.WriteLine("Too many bytes left! Draining...");
                reader.BaseStream.Seek(remainingPayload, System.IO.SeekOrigin.Current);
            }

            if (year > 0)
            {
                Date = new DateTime(year, month, day);
            }
            return Length;
        }

        #endregion // RomMetadataBlock
    }
}

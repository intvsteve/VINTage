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
        }

        #region Properties

        /// <summary>
        /// Gets the publication year.
        /// </summary>
        /// <remarks>If this value is zero, year was not defined.</remarks>
        public ushort Year { get; private set; }

        /// <summary>
        /// Gets the publication month.
        /// </summary>
        /// <remarks>If this value is zero, month was not defined.</remarks>
        public byte Month { get; private set; }

        /// <summary>
        /// Gets the publication day.
        /// </summary>
        /// <remarks>If this value is zero, day was not defined.</remarks>
        public byte Day { get; private set; }

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var remainingPayload = Length;
            if (remainingPayload > 0)
            {
                Year = (ushort)(1900 + reader.ReadByte());
                --remainingPayload;
            }
            if (remainingPayload > 0)
            {
                Month = reader.ReadByte();
                --remainingPayload;
            }
            if (remainingPayload > 0)
            {
                Day = reader.ReadByte();
                --remainingPayload;
            }
            if (remainingPayload > 0)
            {
                System.Diagnostics.Debug.WriteLine("Too many bytes left! Draining...");
            }
            if (remainingPayload > 0)
            {
                reader.BaseStream.Seek(remainingPayload, System.IO.SeekOrigin.Current);
            }
            return Length;
        }

        #endregion // RomMetadataBlock
    }
}

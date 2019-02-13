// <copyright file="RomMetadataString.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for simple string metadata blocks in a .ROM file.
    /// </summary>
    public class RomMetadataString : RomMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataString"/> class.
        /// </summary>
        /// <param name="length">Length of the string in the metadata block.</param>
        /// <param name="type">The specific kind of string metadata.</param>
        public RomMetadataString(uint length, RomMetadataIdTag type)
            : base(length, type)
        {
        }

        #region Properties

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string StringValue { get; private set; }

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var allowLineBreaks = false;
            switch (Type)
            {
                case RomMetadataIdTag.Description:
                case RomMetadataIdTag.License:
                    allowLineBreaks = true;
                    break;
                default:
                    break;
            }
            StringValue = reader.ParseStringFromMetadata(Length, allowLineBreaks);
            return Length;
        }

        #endregion // RomMetadataBlock
    }
}

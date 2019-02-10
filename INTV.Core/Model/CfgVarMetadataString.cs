// <copyright file="CfgVarMetadataString.cs" company="INTV Funhouse">
// Copyright (c) 2018-2019 All Rights Reserved
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
    /// Class for simple metadata strings in a .CFG file.
    /// </summary>
    public class CfgVarMetadataString : CfgVarMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.CfgVarMetadataString"/> class.
        /// </summary>
        /// <param name="type">The specific kind of string metadata.</param>
        public CfgVarMetadataString(CfgVarMetadataIdTag type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string StringValue { get; private set; }

        /// <inheritdoc />
        protected override string ConvertPayloadToString(byte[] payload)
        {
            var quoteIndexes = payload.GetEnclosingQuoteCharacterIndexesFromBytePayload();
            var payloadString = payload.UnescapeBytePayload(quoteIndexes);
            return payloadString;
        }

        /// <inheritdoc/>
        protected override void Parse(string payload)
        {
            StringValue = payload;
        }
    }
}

// <copyright file="LuigiMetadataBlock.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// LUIGI metadata block, which contains optional supplemental information about a ROM.
    /// </summary>
    public class LuigiMetadataBlock : LuigiDataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.LuigiMetadataBlock"/> class.
        /// </summary>
        internal LuigiMetadataBlock()
            : base(LuigiDataBlockType.Metadata)
        {
            _longNames = new List<string>();
            _shortNames = new List<string>();
            _authors = new List<string>();
            _publishers = new List<string>();
            _years = new List<ushort>();
            _licenses = new List<string>();
            _descriptions = new List<string>();
            _additionalInformation = new List<string>();
        }

        /// <summary>
        /// Gets the long names.
        /// </summary>
        public IEnumerable<string> LongNames
        {
            get { return _longNames; }
        }
        private List<string> _longNames;

        /// <summary>
        /// Gets the short names.
        /// </summary>
        public IEnumerable<string> ShortNames
        {
            get { return _shortNames; }
        }
        private List<string> _shortNames;

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public IEnumerable<string> Authors
        {
            get { return _authors; }
        }
        private List<string> _authors;

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        public IEnumerable<string> Publishers
        {
            get { return _publishers; }
        }
        private List<string> _publishers;

        /// <summary>
        /// Gets the publication dates.
        /// </summary>
        public IEnumerable<ushort> Years
        {
            get { return _years; }
        }
        private List<ushort> _years;

        /// <summary>
        /// Gets the licenses.
        /// </summary>
        public IEnumerable<string> Licenses
        {
            get { return _licenses; }
        }
        private List<string> _licenses;

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        public IEnumerable<string> Descriptions
        {
            get { return _descriptions; }
        }
        private List<string> _descriptions;

        /// <summary>
        /// Gets any additional information.
        /// </summary>
        public IEnumerable<string> AdditionalInformation
        {
            get { return _additionalInformation; }
        }
        private List<string> _additionalInformation;

        /// <inheritdoc/>
        protected override int DeserializePayload(Core.Utility.BinaryReader reader)
        {
            var runningBytesRead = 0;
            while (runningBytesRead < Length)
            {
                var tag = (Tag)reader.ReadByte();
                ++runningBytesRead;
                var dataLength = reader.ReadByte();
                ++runningBytesRead;
                string stringResult = null;
                switch (tag)
                {
                    case Tag.Year:
                        System.Diagnostics.Debug.Assert(dataLength == 1, "LUIGI Year metadata should only contain one byte.");
                        var year = (ushort)(reader.ReadByte() + 1900);
                        _years.Add(year);
                        break;
                    default:
                        // Documentation indicates this could be ASCII or UTF-8...
                        stringResult = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(dataLength), 0, dataLength).Trim('\0');
                        break;
                }
                runningBytesRead += dataLength;
                if (stringResult != null)
                {
                    switch (tag)
                    {
                        case Tag.Name:
                            _longNames.Add(stringResult);
                            break;
                        case Tag.ShortName:
                            _shortNames.Add(stringResult);
                            break;
                        case Tag.Author:
                            _authors.Add(stringResult);
                            break;
                        case Tag.Publisher:
                            _publishers.Add(stringResult);
                            break;
                        case Tag.Year:
                            break;
                        case Tag.License:
                            _licenses.Add(stringResult);
                            break;
                        case Tag.Description:
                            _descriptions.Add(stringResult);
                            break;
                        case Tag.Miscellaneous:
                            _additionalInformation.Add(stringResult);
                            break;
                        default:
                            break;
                    }
                }
            }
            return Length;
        }

        private enum Tag : byte
        {
            /// <summary>Name field.</summary>
            Name,

            /// <summary>Short name field.</summary>
            ShortName,

            /// <summary>Author field.</summary>
            Author,

            /// <summary>Publisher (vendor) field.</summary>
            Publisher,

            /// <summary>Year field.</summary>
            Year,

            /// <summary>License field.</summary>
            License,

            /// <summary>Description field.</summary>
            Description,

            /// <summary>Miscellaneous data field.</summary>
            Miscellaneous,

            /// <summary>Unknown or unrecognized field.</summary>
            Unknown = 0xFF
        }
    }
}

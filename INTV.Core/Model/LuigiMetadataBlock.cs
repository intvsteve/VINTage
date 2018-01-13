﻿// <copyright file="LuigiMetadataBlock.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// LUIGI metadata block, which contains optional supplemental information about a ROM.
    /// </summary>
    public class LuigiMetadataBlock : LuigiDataBlock
    {
        private static readonly Dictionary<LuigiMetadataIdTag, MetadataParser> MetadataParsers = new Dictionary<LuigiMetadataIdTag, MetadataParser>()
        {
            { LuigiMetadataIdTag.Name, StringMetadataParser },
            { LuigiMetadataIdTag.ShortName, StringMetadataParser },
            { LuigiMetadataIdTag.Author, StringMetadataParser },
            { LuigiMetadataIdTag.Publisher, StringMetadataParser },
            { LuigiMetadataIdTag.Date, DateMetadataParser },
            { LuigiMetadataIdTag.License, StringMetadataParser },
            { LuigiMetadataIdTag.Description, StringMetadataParser },
            { LuigiMetadataIdTag.Miscellaneous, StringMetadataParser },
            { LuigiMetadataIdTag.Graphics, StringMetadataParser },
            { LuigiMetadataIdTag.Music, StringMetadataParser },
            { LuigiMetadataIdTag.SoundEffects, StringMetadataParser },
            { LuigiMetadataIdTag.VoiceActing, StringMetadataParser },
            { LuigiMetadataIdTag.Documentation, StringMetadataParser },
            { LuigiMetadataIdTag.ConceptDesign, StringMetadataParser },
            { LuigiMetadataIdTag.BoxOrOtherArtwork, StringMetadataParser },
            { LuigiMetadataIdTag.UrlContactInfo, StringMetadataParser },
            { LuigiMetadataIdTag.Unknown, DefaultMetadataParser }
        };

        private static readonly LuigiMetadataIdTag[] StringMetadataTypes =
        {
            LuigiMetadataIdTag.Name,
            LuigiMetadataIdTag.ShortName,
            LuigiMetadataIdTag.Author,
            LuigiMetadataIdTag.Publisher,
            LuigiMetadataIdTag.License,
            LuigiMetadataIdTag.Description,
            LuigiMetadataIdTag.Miscellaneous,
            LuigiMetadataIdTag.Graphics,
            LuigiMetadataIdTag.Music,
            LuigiMetadataIdTag.SoundEffects,
            LuigiMetadataIdTag.VoiceActing,
            LuigiMetadataIdTag.Documentation,
            LuigiMetadataIdTag.ConceptDesign,
            LuigiMetadataIdTag.BoxOrOtherArtwork,
            LuigiMetadataIdTag.UrlContactInfo,
        };

        private Dictionary<LuigiMetadataIdTag, List<string>> _stringMetadataEntries = new Dictionary<LuigiMetadataIdTag, List<string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.LuigiMetadataBlock"/> class.
        /// </summary>
        internal LuigiMetadataBlock()
            : base(LuigiDataBlockType.Metadata)
        {
            foreach (var stringMetadataType in StringMetadataTypes)
            {
                _stringMetadataEntries[stringMetadataType] = new List<string>();
            }
            _dates = new List<DateTime>();
        }

        /// <summary>
        /// Gets the long names.
        /// </summary>
        public IEnumerable<string> LongNames
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Name]; }
        }

        /// <summary>
        /// Gets the short names.
        /// </summary>
        public IEnumerable<string> ShortNames
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.ShortName]; }
        }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public IEnumerable<string> Authors
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Author]; }
        }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        public IEnumerable<string> Publishers
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Publisher]; }
        }

        /// <summary>
        /// Gets the publication dates.
        /// </summary>
        public IEnumerable<DateTime> Dates
        {
            get { return _dates; }
        }
        private List<DateTime> _dates;

        /// <summary>
        /// Gets the licenses.
        /// </summary>
        public IEnumerable<string> Licenses
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.License]; }
        }

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        public IEnumerable<string> Descriptions
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Description]; }
        }

        /// <summary>
        /// Gets any additional information.
        /// </summary>
        public IEnumerable<string> AdditionalInformation
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Miscellaneous]; }
        }

        /// <summary>Gets program graphics credits.</summary>
        public IEnumerable<string> GraphicsCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Graphics]; }
        }

        /// <summary>Gets program music credits.</summary>
        public IEnumerable<string> MusicCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Music]; }
        }

        /// <summary>Gets program sound effects credits.</summary>
        public IEnumerable<string> SoundEffectsCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.SoundEffects]; }
        }

        /// <summary>Gets program voice acting credits.</summary>
        public IEnumerable<string> VoiceActingCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.VoiceActing]; }
        }

        /// <summary>Gets program documentation credits.</summary>
        public IEnumerable<string> DocumentationCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.Documentation]; }
        }

        /// <summary>Gets concept and design credits.</summary>
        public IEnumerable<string> ConceptAndDesignCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.ConceptDesign]; }
        }

        /// <summary>Gets box, overlay, manual, and other artwork credits.</summary>
        public IEnumerable<string> BoxOrOtherArtworkCredits
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.BoxOrOtherArtwork]; }
        }

        /// <summary>Gets URL contact info data.</summary>
        public IEnumerable<string> UrlContactInfos
        {
            get { return _stringMetadataEntries[LuigiMetadataIdTag.UrlContactInfo]; }
        }

        /// <inheritdoc/>
        protected override int DeserializePayload(Core.Utility.BinaryReader reader)
        {
            var runningBytesRead = 0;
            while (runningBytesRead < Length)
            {
                var tag = (LuigiMetadataIdTag)reader.ReadByte();
                ++runningBytesRead;
                var dataLength = reader.ReadByte();
                ++runningBytesRead;

                MetadataParser parser;
                if (!MetadataParsers.TryGetValue(tag, out parser))
                {
                    parser = DefaultMetadataParser;
                }

                var metadata = parser(reader, dataLength);
                runningBytesRead += dataLength;

                switch (tag)
                {
                    case LuigiMetadataIdTag.Date:
                        var date = (DateTime)metadata;
                        _dates.Add(date);
                        break;
                    default:
                        List<string> metadataStringStorage;
                        if (_stringMetadataEntries.TryGetValue(tag, out metadataStringStorage))
                        {
                            var stringResult = (string)metadata;
                            metadataStringStorage.Add(stringResult);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Either failed to parse or could not find storage for tag: " + tag);
                        }
                        break;
                }
            }
            return Length;
        }

        private static object StringMetadataParser(Core.Utility.BinaryReader reader, byte payloadLength)
        {
            // Documentation indicates this could be ASCII or UTF-8...
            var stringResult = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(payloadLength), 0, payloadLength).Trim('\0');
            return stringResult;
        }

        private static object DateMetadataParser(Core.Utility.BinaryReader reader, byte payloadLength)
        {
            System.Diagnostics.Debug.Assert(payloadLength >= 1, "LUIGI Year metadata should contain at least one byte.");
            var year = reader.ReadByte() + 1900; // (yes, assuming Gregorian calendar :P

            var month = 6; // if not present, assume "middle" month of the year (yes, assuming Gregorian calendar :P )
            if (payloadLength > 1)
            {
                month = reader.ReadByte();
                if ((month < 1) || (month > 12))
                {
                    month = 6;
                }
            }

            var day = 15; // if not present, assume "middle" day of the month (yes, assuming Gregorian calendar :P )
            if (payloadLength > 2)
            {
                day = reader.ReadByte();
                if ((day < 1) || (day > DateTime.DaysInMonth(year, month)))
                {
                    day = 15;
                }
            }

            var date = new DateTime(year, month, day);
            return date;
        }

        private static object DefaultMetadataParser(Core.Utility.BinaryReader reader, byte payloadLength)
        {
            reader.BaseStream.Seek(payloadLength, System.IO.SeekOrigin.Current);
            return null;
        }

        private delegate object MetadataParser(Core.Utility.BinaryReader reader, byte payloadLength);
    }
}

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

using System;
using System.Collections.Generic;

namespace INTV.Core.Model
{
    /// <summary>
    /// LUIGI metadata block, which contains optional supplemental information about a ROM.
    /// </summary>
    public class LuigiMetadataBlock : LuigiDataBlock
    {
        private static readonly Dictionary<Tag, MetadataParser> MetadataParsers = new Dictionary<Tag, MetadataParser>()
        {
            { Tag.Name, StringMetadataParser },
            { Tag.ShortName, StringMetadataParser },
            { Tag.Author, StringMetadataParser },
            { Tag.Publisher, StringMetadataParser },
            { Tag.Date, DateMetadataParser },
            { Tag.License, StringMetadataParser },
            { Tag.Description, StringMetadataParser },
            { Tag.Miscellaneous, StringMetadataParser },
            { Tag.Graphics, StringMetadataParser },
            { Tag.Music, StringMetadataParser },
            { Tag.SoundEffects, StringMetadataParser },
            { Tag.VoiceActing, StringMetadataParser },
            { Tag.Documentation, StringMetadataParser },
            { Tag.ConceptDesign, StringMetadataParser },
            { Tag.BoxOrOtherArtwork, StringMetadataParser },
            { Tag.UrlContactInfo, StringMetadataParser },
            { Tag.Unknown, DefaultMetadataParser }
        };

        private static readonly Tag[] StringMetadataTypes =
        {
            Tag.Name,
            Tag.ShortName,
            Tag.Author,
            Tag.Publisher,
            Tag.License,
            Tag.Description,
            Tag.Miscellaneous,
            Tag.Graphics,
            Tag.Music,
            Tag.SoundEffects,
            Tag.VoiceActing,
            Tag.Documentation,
            Tag.ConceptDesign,
            Tag.BoxOrOtherArtwork,
            Tag.UrlContactInfo,
        };

        private Dictionary<Tag, List<string>> _stringMetadataEntries = new Dictionary<Tag, List<string>>();

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
            get { return _stringMetadataEntries[Tag.Name]; }
        }

        /// <summary>
        /// Gets the short names.
        /// </summary>
        public IEnumerable<string> ShortNames
        {
            get { return _stringMetadataEntries[Tag.ShortName]; }
        }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public IEnumerable<string> Authors
        {
            get { return _stringMetadataEntries[Tag.Author]; }
        }

        /// <summary>
        /// Gets the publishers.
        /// </summary>
        public IEnumerable<string> Publishers
        {
            get { return _stringMetadataEntries[Tag.Publisher]; }
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
            get { return _stringMetadataEntries[Tag.License]; }
        }

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        public IEnumerable<string> Descriptions
        {
            get { return _stringMetadataEntries[Tag.Description]; }
        }

        /// <summary>
        /// Gets any additional information.
        /// </summary>
        public IEnumerable<string> AdditionalInformation
        {
            get { return _stringMetadataEntries[Tag.Miscellaneous]; }
        }

        /// <summary>Gets program graphics credits.</summary>
        public IEnumerable<string> GraphicsCredits
        {
            get { return _stringMetadataEntries[Tag.Graphics]; }
        }

        /// <summary>Gets program music credits.</summary>
        public IEnumerable<string> MusicCredits
        {
            get { return _stringMetadataEntries[Tag.Music]; }
        }

        /// <summary>Gets program sound effects credits.</summary>
        public IEnumerable<string> SoundEffectsCredits
        {
            get { return _stringMetadataEntries[Tag.SoundEffects]; }
        }

        /// <summary>Gets program voice acting credits.</summary>
        public IEnumerable<string> VoiceActingCredits
        {
            get { return _stringMetadataEntries[Tag.VoiceActing]; }
        }

        /// <summary>Gets program documentation credits.</summary>
        public IEnumerable<string> DocumentationCredits
        {
            get { return _stringMetadataEntries[Tag.Documentation]; }
        }

        /// <summary>Gets concept and design credits.</summary>
        public IEnumerable<string> ConceptAndDesignCredits
        {
            get { return _stringMetadataEntries[Tag.ConceptDesign]; }
        }

        /// <summary>Gets box, overlay, manual, and other artwork credits.</summary>
        public IEnumerable<string> BoxOrOtherArtworkCredits
        {
            get { return _stringMetadataEntries[Tag.BoxOrOtherArtwork]; }
        }

        /// <summary>Gets URL contact info data.</summary>
        public IEnumerable<string> UrlContactInfos
        {
            get { return _stringMetadataEntries[Tag.UrlContactInfo]; }
        }

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

                MetadataParser parser;
                if (!MetadataParsers.TryGetValue(tag, out parser))
                {
                    parser = DefaultMetadataParser;
                }

                var metadata = parser(reader, dataLength);
                runningBytesRead += dataLength;

                switch (tag)
                {
                    case Tag.Date:
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

        /// <summary>ID tags for LUIGI metadata sub-blocks.</summary>
        private enum Tag : byte
        {
            /// <summary>Name field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Name = 0x00,

            /// <summary>Short name field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            ShortName = 0x01,

            /// <summary>Author field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Author = 0x02,

            /// <summary>Publisher (vendor) field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Publisher = 0x03,

            /// <summary>Date field.</summary>
            Date = 0x04,

            /// <summary>License field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            License = 0x05,

            /// <summary>Description field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Description = 0x06,

            /// <summary>Miscellaneous data field.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Miscellaneous = 0x07,

            /// <summary>Indicates contributions to program art / graphics.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Graphics = 0x08,

            /// <summary>Indicates contributions to music.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Music = 0x09,

            /// <summary>Indicates contributions to sound effects.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            SoundEffects = 0x0A,

            /// <summary>Indicates contributions to voice acting.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            VoiceActing = 0x0B,

            /// <summary>Indicates documentation authorship.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            Documentation = 0x0C,

            /// <summary>The program concept or design.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            ConceptDesign = 0x0D,

            /// <summary>Indicates contributions to box, overlay, manual or other artwork.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            BoxOrOtherArtwork = 0x0E,

            /// <summary>Site URL or other contact information.</summary>
            /// <remarks>Data for this field is ASCII/UTF-8 string.</remarks>
            UrlContactInfo = 0x0F,

            /// <summary>Unknown or unrecognized field.</summary>
            Unknown = 0xFF
        }
    }
}

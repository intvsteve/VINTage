// <copyright file="LuigiMetadataBlock.cs" company="INTV Funhouse">
// Copyright (c) 2016-2018 All Rights Reserved
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
using System.Linq;

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

        private List<CfgVarMetadataBlock> _miscellaneousCfgVarMetadata = null;

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
            _releaseDates = new List<MetadataDateTime>();
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

        /// <summary>Gets the release dates.</summary>
        public IEnumerable<MetadataDateTime> ReleaseDates
        {
            get { return _releaseDates; }
        }
        private List<MetadataDateTime> _releaseDates;

        /// <summary>Gets the build dates.</summary>
        public IEnumerable<MetadataDateTime> BuildDates
        {
            get { return MiscellaneousCfgVarMetadata.OfType<CfgVarMetadataDate>().Where(m => m.Type == CfgVarMetadataIdTag.BuildDate).Select(m => m.Date); }
        }

        /// <summary>Gets the versions.</summary>
        public IEnumerable<string> Versions
        {
            get { return MiscellaneousCfgVarMetadata.OfType<CfgVarMetadataString>().Where(m => m.Type == CfgVarMetadataIdTag.Version).Select(m => m.StringValue); }
        }

        private IEnumerable<CfgVarMetadataBlock> MiscellaneousCfgVarMetadata
        {
            get
            {
                if (_miscellaneousCfgVarMetadata == null)
                {
                    _miscellaneousCfgVarMetadata = new List<CfgVarMetadataBlock>();
                    foreach (var entry in AdditionalInformation)
                    {
                        // Square peg, meet round hole.
                        var cfgFileLineBytes = System.Text.Encoding.UTF8.GetBytes(entry);
                        using (var cfgFileLineStream = new System.IO.MemoryStream(cfgFileLineBytes))
                        {
                            var cfgVarMetadataBlock = CfgVarMetadataBlock.Inflate(cfgFileLineStream);
                            if (cfgVarMetadataBlock != null)
                            {
                                _miscellaneousCfgVarMetadata.Add(cfgVarMetadataBlock);
                            }
                        }
                    }
                }
                return _miscellaneousCfgVarMetadata;
            }
        }

        /// <inheritdoc/>
        protected override int DeserializePayload(Core.Utility.BinaryReader reader)
        {
            // This is inefficient, but simple. Read and walk the payload twice. First time validates its checksum. Second pass actually parses it.
            base.DeserializePayload(reader);
            reader.BaseStream.Seek(-Length, System.IO.SeekOrigin.Current);

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
                        var date = (MetadataDateTime)metadata;
                        _releaseDates.Add(date);
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
            var date = reader.ParseDateTimeFromMetadata(payloadLength);
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

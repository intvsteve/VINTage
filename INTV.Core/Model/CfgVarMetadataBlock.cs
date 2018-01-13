// <copyright file="CfgVarMetadataBlock.cs" company="INTV Funhouse">
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
using System.Collections.Generic;

namespace INTV.Core.Model
{
    /// <summary>
    /// Base class for a CFGVAR-based metadata block, which contains additional information about a ROM
    /// as described in the .cfg file accompanying a .BIN-format ROM.
    /// </summary>
    public abstract class CfgVarMetadataBlock : INTV.Core.Utility.ByteSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.CfgVarMetadataBlock"/> class.
        /// </summary>
        /// <param name="type">he specific kind of metadata.</param>
        protected CfgVarMetadataBlock(CfgVarMetadataIdTag type)
        {
            Type = type;
        }

        #region Properties

        #region ByteSerializer

        /// <inheritdoc/>
        public override int SerializeByteCount
        {
            get { return -1; }
        }

        /// <inheritdoc/>
        public override int DeserializeByteCount
        {
            get { return _deserializeByteCount; }
        }

        private int _deserializeByteCount;

        #endregion // ByteSerializer

        /// <summary>
        /// Gets the type of the metadata.
        /// </summary>
        public CfgVarMetadataIdTag Type { get; private set; }

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a CfgVarMetadataBlock by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a CfgVarMetadataBlock.</returns>
        public static CfgVarMetadataBlock Inflate(System.IO.Stream stream)
        {
            CfgVarMetadataBlock metadataBlock = null;
            using (var reader = new INTV.Core.Utility.BinaryReader(stream))
            {
                metadataBlock = Inflate(reader);
            }
            return metadataBlock;
        }

        /// <summary>
        /// Creates a new instance of a CfgVarMetadataBlock by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a CfgVarMetadataBlock.</returns>
        /// <remarks>It is assumed that the reader is currently positioned at the beginning of a serialized CFGVAR metadata entry.</remarks>
        public static CfgVarMetadataBlock Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            CfgVarMetadataBlock metadataBlock = null;
            var metadataBlockType = DecodeBlockType(reader);
            switch (metadataBlockType)
            {
                case CfgVarMetadataIdTag.Name:
                case CfgVarMetadataIdTag.ShortName:
                case CfgVarMetadataIdTag.Author:
                case CfgVarMetadataIdTag.GameArt:
                case CfgVarMetadataIdTag.Music:
                case CfgVarMetadataIdTag.SoundEffects:
                case CfgVarMetadataIdTag.VoiceActing:
                case CfgVarMetadataIdTag.Description:
                case CfgVarMetadataIdTag.Documentation:
                case CfgVarMetadataIdTag.BoxOrOtherArtwork:
                case CfgVarMetadataIdTag.ConceptDesign:
                case CfgVarMetadataIdTag.MoreInfo:
                case CfgVarMetadataIdTag.Publisher:
                case CfgVarMetadataIdTag.License:
                case CfgVarMetadataIdTag.Version:
                    metadataBlock = new CfgVarMetadataString(metadataBlockType);
                    break;
                case CfgVarMetadataIdTag.ReleaseDate:
                case CfgVarMetadataIdTag.Year:
                case CfgVarMetadataIdTag.BuildDate:
                    metadataBlock = new CfgVarMetadataDate(metadataBlockType);
                    break;
                case CfgVarMetadataIdTag.EcsCompatibility:
                case CfgVarMetadataIdTag.IntellivoiceCompatibility:
                case CfgVarMetadataIdTag.IntellivisionIICompatibility:
                case CfgVarMetadataIdTag.KeyboardComponentCompatibility:
                case CfgVarMetadataIdTag.TutorvisionCompatibility:
                case CfgVarMetadataIdTag.Ecs:
                case CfgVarMetadataIdTag.Voice:
                case CfgVarMetadataIdTag.IntellivisionII:
                case CfgVarMetadataIdTag.JlpAccelerators:
                case CfgVarMetadataIdTag.Jlp:
                    metadataBlock = new CfgVarMetadataFeatureCompatibility(metadataBlockType);
                    break;
                case CfgVarMetadataIdTag.LtoFlashMapper:
                    metadataBlock = new CfgVarMetadataBoolean(metadataBlockType);
                    break;
                case CfgVarMetadataIdTag.JlpFlash:
                    metadataBlock = new CfgVarMetadataInteger(metadataBlockType);
                    break;
                default:
                    break;
            }
            if (metadataBlock != null)
            {
                metadataBlock._deserializeByteCount = (int)reader.BaseStream.Length;
                metadataBlock.Deserialize(reader);
            }
            return metadataBlock;
        }

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        /// <remarks>The precondition here is that the reader is positioned immediately after the ROM metadata block type.</remarks>
        protected override int Deserialize(Core.Utility.BinaryReader reader)
        {
            int deserializedPayloadLength = 0;
            var payloadLength = reader.BaseStream.Length;
            var data = new byte[payloadLength];
            try
            {
                while (deserializedPayloadLength < payloadLength)
                {
                    data[deserializedPayloadLength++] = reader.ReadByte();
                }
            }
            catch (System.IO.EndOfStreamException)
            {
            }
            var payload = System.Text.Encoding.UTF8.GetString(data, 0, data.Length).Trim('\0');
            Parse(payload);
            return deserializedPayloadLength;
        }

        #endregion // ByteSerializer

        /// <summary>
        /// Parse the specified payload.
        /// </summary>
        /// <param name="payload">The string data for a CFGVAR.</param>
        protected abstract void Parse(string payload);

        protected string GetCleanPayloadString(string payload)
        {
            if (!string.IsNullOrEmpty(payload))
            {
                payload = payload.Trim().Trim(new[] { '"' });
            }
            return payload;
        }

        private static CfgVarMetadataIdTag DecodeBlockType(Core.Utility.BinaryReader reader)
        {
            var type = CfgVarMetadataIdTag.Invalid;
            var payloadLength = reader.BaseStream.Length;
            var data = new byte[payloadLength];
            var bytesRead = 0;
            var foundEquals = false;
            while (!foundEquals && (bytesRead < payloadLength))
            {
                var byteChar = reader.ReadByte();
                var character = Convert.ToChar(byteChar);
                foundEquals = character == '=';
                if (!foundEquals)
                {
                    data[bytesRead++] = byteChar;
                }
            }
            if (foundEquals)
            {
                var tagString = System.Text.Encoding.UTF8.GetString(data, 0, data.Length).Trim('\0').Trim();
                type = tagString.ToCfgVarMetadataIdTag();
            }
            return type;
        }

#if false
        private static string DeserializePayload(Core.Utility.BinaryReader reader)
        {
            var payloadSize = (int)reader.BaseStream.Length;
            var bytes = reader.ReadBytes(payloadSize);
            var payload = System.Text.Encoding.UTF8.GetString(bytes);
            return payload;
        }

        private static KeyValuePair<CfgVarMetadataIdTag, string> Decode(string payload)
        {
            var decodedData = new KeyValuePair<CfgVarMetadataIdTag, string>(CfgVarMetadataIdTag.Invalid, string.Empty);
            if (!string.IsNullOrEmpty(payload))
            {
                var assignmentIndex = payload.IndexOf('=');
                if ((assignmentIndex > 0) && ((assignmentIndex + 1) < payload.Length))
                {
                    var tag = payload.Substring(0, assignmentIndex).Trim().ToCfgVarMetadataIdTag();
                    var value = payload.Substring(assignmentIndex + 1).Trim();
                    decodedData = new KeyValuePair<CfgVarMetadataIdTag, string>(tag, value);
                }
            }
            return decodedData;
        }
        #endif
    }
}

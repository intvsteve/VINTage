// <copyright file="RomMetadataBlock.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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
    /// Base class for for metadata (ID tag) attached to .ROM-format ROMs.
    /// </summary>
    public class RomMetadataBlock : INTV.Core.Utility.ByteSerializer
    {
        private const byte PayloadLengthMask = 0xC0;
        private const int PayloadLengthBitShift = 6;
        private const int CrcByteCount = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataBlock"/> class.
        /// </summary>
        /// <param name="length">The length (in bytes) of the payload.</param>
        /// <param name="type">The specific kind of metadata in the block.</param>
        protected RomMetadataBlock(uint length, RomMetadataIdTag type)
        {
            Length = length;
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
        /// Gets the ROM metadata block type.
        /// </summary>
        public RomMetadataIdTag Type { get; private set; }

        /// <summary>
        /// Gets the length of the block's payload, in bytes.
        /// </summary>
        public uint Length { get; private set; }

        public ushort Crc { get; private set; }

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a RomMetadataBlock by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a RomMetadataBlock.</returns>
        public static RomMetadataBlock Inflate(System.IO.Stream stream)
        {
            RomMetadataBlock metadataBlock = null;
            using (var reader = new INTV.Core.Utility.BinaryReader(stream))
            {
                metadataBlock = Inflate(reader);
            }
            return metadataBlock;
        }

        /// <summary>
        /// Creates a new instance of a RomMetadataBlock by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a RomMetadataBlock.</returns>
        /// <remarks>It is assumed that the reader is currently positioned at the beginning of a serialized ROM metadata block.</remarks>
        public static RomMetadataBlock Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            RomMetadataBlock metadataBlock = null;
            int additionalBytesInPayloadLength;
            var payloadLength = DecodeLength(reader, out additionalBytesInPayloadLength);
            var metadataBlockType = RomMetadataIdTag.Ignore;
            if (payloadLength > 0)
            {
                metadataBlockType = (RomMetadataIdTag)reader.ReadByte();
                switch (metadataBlockType)
                {
                    case RomMetadataIdTag.Title:
                    case RomMetadataIdTag.ShortTitle:
                    case RomMetadataIdTag.License:
                    case RomMetadataIdTag.Description:
                        metadataBlock = new RomMetadataString(payloadLength, metadataBlockType);
                        break;
                    case RomMetadataIdTag.ReleaseDate:
                        metadataBlock = new RomMetadataPublicationDate(payloadLength);
                        break;
                    case RomMetadataIdTag.Features:
                        metadataBlock = new RomMetadataFeatures(payloadLength);
                        break;
                    case RomMetadataIdTag.Publisher:
                        metadataBlock = new RomMetadataPublisher(payloadLength);
                        break;
                    case RomMetadataIdTag.Credits:
                        metadataBlock = new RomMetadataCredits(payloadLength);
                        break;
                    case RomMetadataIdTag.ControllerBindings:
                        metadataBlock = new RomMetadataControllerBindings(payloadLength);
                        break;
                    case RomMetadataIdTag.UrlContactInfo:
                    default:
                        metadataBlock = new RomMetadataBlock(payloadLength, metadataBlockType);
                        break;
                }
                metadataBlock._deserializeByteCount = (int)payloadLength + additionalBytesInPayloadLength + 1 + CrcByteCount + sizeof(RomMetadataIdTag);
                metadataBlock.Deserialize(reader);

                // Re-reading the block is more expensive than having a running CRC16 but it's easier to implement. :P
                var numBytesInForCrc = metadataBlock._deserializeByteCount - CrcByteCount;
                reader.BaseStream.Seek(-numBytesInForCrc, System.IO.SeekOrigin.Current);
                var crc16 = INTV.Core.Utility.Crc16.OfBlock(reader.ReadBytes(numBytesInForCrc), INTV.Core.Utility.Crc16.InitialValue);
                metadataBlock.Crc = (ushort)(((int)reader.ReadByte() << 8) | reader.ReadByte());

                // TODO: Report error if CRCs do not match.
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
            var deserializedPayloadLength = DeserializePayload(reader);
            if (deserializedPayloadLength != Length)
            {
                throw new System.InvalidOperationException("Failed to deserialize ROM metadata payload!");
            }
            return (int)deserializedPayloadLength;
        }

        #endregion // ByteSerializer

        /// <summary>
        /// Deserializes the payload.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>The number of bytes deserialized.</returns>
        /// <remarks>The default implementation merely advances the reader without parsing or even reading
        /// any of the payload into memory. Specific subclasses may override this implementation if
        /// any specific data from the payload is desired.</remarks>
        protected virtual uint DeserializePayload(Core.Utility.BinaryReader reader)
        {
            reader.BaseStream.Seek(Length, System.IO.SeekOrigin.Current);
            return Length;
        }

        private static uint DecodeLength(Core.Utility.BinaryReader reader, out int additionalBytesInPayloadLength)
        {
            var data = reader.ReadByte();
            uint payloadLength = (uint)(data & ~PayloadLengthMask); // low 6 bits are the low 6 bits of payload length
            additionalBytesInPayloadLength = (data & PayloadLengthMask) >> PayloadLengthBitShift;
            for (var i = 0; i < additionalBytesInPayloadLength; ++i)
            {
                uint partialLengthBits = reader.ReadByte();
                partialLengthBits = partialLengthBits << ((i * 8) + PayloadLengthBitShift);
                payloadLength |= partialLengthBits;
            }
            return payloadLength;
        }
    }
}

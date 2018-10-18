using INTV.Core.Model;
using INTV.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomMetadataBlockTests
    {
        [Fact]
        public void RomMetadataBlock_Serialize_ThrowsNotImplementedException()
        {
            var metadataBlock = new RomMetadataTestBlock(0, RomMetadataIdTag.Ignore);

            Assert.Throws<NotImplementedException>(() => metadataBlock.Serialize(null));
        }

        [Fact]
        public void RomMetadataBlock_GetSerializeByteCount_ReturnsNegativeOne()
        {
            var metadataBlock = new RomMetadataTestBlock(4, RomMetadataIdTag.Ignore);

            Assert.Equal(-1, metadataBlock.SerializeByteCount);
        }

        [Fact]
        public void RomMetadataBlock_GetDeserializeByteCount_ReturnsZero()
        {
            var metadataBlock = new RomMetadataTestBlock(1, RomMetadataIdTag.Ignore);

            Assert.Equal(0, metadataBlock.DeserializeByteCount);
        }

        [Fact]
        public void RomMetadataBlock_DeserializeWithNullBinaryReader_ThrowsNullReferenceException()
        {
            var metadataBlock = new RomMetadataTestBlock(0, RomMetadataIdTag.Ignore) { BytesToRead = 1 };

            Assert.Throws<NullReferenceException>(() => metadataBlock.Deserialize(null));
        }

        [Fact]
        public void RomMetadataBlock_DeserializeWithBadLength_ThrowsInvalidOperationException()
        {
            using (var stream = RomMetadataTestBlock.CreatePseudoMetadata(0, RomMetadataIdTag.None, null))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var metadataBlock = new RomMetadataTestBlock(4, RomMetadataIdTag.Ignore);

                    Assert.Throws<InvalidOperationException>(() => metadataBlock.Deserialize(reader));
                }
            }
        }

        [Fact]
        public void RomMetadataBlock_InflateBlockTypeIgnore_ProducesCorrectMetadataBlockType()
        {
            using (var stream = RomMetadataTestBlock.CreatePseudoMetadata(0, RomMetadataIdTag.None, null))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var metadataBlock = RomMetadataBlock.Inflate(reader);

                    Assert.NotNull(metadataBlock);
                    Assert.Equal(RomMetadataIdTag.None, metadataBlock.Type);
                }
            }
        }

        /// <summary>
        /// Test to get code coverage of RomMetadataBlock only. See separate tests for RomMetadataControllerBindings.
        /// </summary>
        [Fact]
        public void RomMetadataBlock_InflateEmptyControllerMetadata_ProducesEmptyControllerMetadata()
        {
            using (var stream = RomMetadataTestBlock.CreatePseudoMetadata(0, RomMetadataIdTag.ControllerBindings, null))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var metadataBlock = RomMetadataBlock.Inflate(reader);

                    Assert.NotNull(metadataBlock);
                    Assert.Equal(RomMetadataIdTag.ControllerBindings, metadataBlock.Type);
                }
            }
        }

        private class RomMetadataTestBlock : RomMetadataBlock
        {
            public RomMetadataTestBlock(uint length, RomMetadataIdTag type)
                : base(length, type)
            {
            }

            public uint FakeDeserializeLength { get; set; }

            public int BytesToRead { get; set; }

            public static System.IO.MemoryStream CreatePseudoMetadata(byte payloadLength, RomMetadataIdTag payloadType, byte[] payloadData)
            {
                var stream = new System.IO.MemoryStream();
                var data = new List<byte>() { payloadLength, (byte)payloadType};
                if (payloadData != null)
                {
                    data.AddRange(payloadData);
                }

                var crc16 = Crc16.OfBlock(data.ToArray(), Crc16.InitialValue);
                var crcByte = (byte)((crc16 & (ushort)0xFF00) >> 8);
                data.Add(crcByte);
                crcByte = (byte)(crc16 & 0xFF);
                data.Add(crcByte);
                stream.Write(data.ToArray(), 0, data.Count);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return stream;
            }

            protected override uint DeserializePayload(BinaryReader reader)
            {
                if (BytesToRead > 0)
                {
                    base.DeserializePayload(reader);
                }
                return FakeDeserializeLength;
            }
        }
    }
}

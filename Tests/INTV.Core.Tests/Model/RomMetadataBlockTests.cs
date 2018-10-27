// <copyright file="RomMetadataBlockTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Utility;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in BinaryWriter' works correctly.")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in BinaryWriter' works correctly.")]
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
                var data = new List<byte>() { payloadLength, (byte)payloadType };
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

// <copyright file="RomMetadataBlockTests.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in 'BinaryReader' works correctly.")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in 'BinaryReader' works correctly.")]
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

        [Fact]
        public void RomMetadataBlock_InflateUtf8TestMetadata_ProducesExpectedMetadata()
        {
            var rawData = new byte[]
            {
                0x14, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x30, 0x30, 0x20, 0xF0, 0x90,
                0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x9B, 0xBA, 0x15, 0x02, 0xFF, 0xF0, 0x90, 0x80, 0x80, 0xF0,
                0x90, 0x80, 0x81, 0x20, 0x30, 0x36, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x1C,
                0xCC, 0x4F, 0x03, 0x03, 0x01, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x31,
                0x30, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x00, 0xFF, 0x01, 0xF0, 0x90, 0x80,
                0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x32, 0x30, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF,
                0xBF, 0x00, 0x02, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x31, 0x31, 0x20,
                0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x00, 0x04, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0,
                0x90, 0x80, 0x81, 0x20, 0x31, 0x32, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x00,
                0x08, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x31, 0x33, 0x20, 0xF0, 0x90,
                0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x00, 0x10, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80,
                0x81, 0x20, 0x31, 0x34, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x00, 0x20, 0x01,
                0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x31, 0x35, 0x20, 0xF0, 0x90, 0x80, 0x83,
                0xF4, 0x8F, 0xBF, 0xBF, 0x00, 0x40, 0x01, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20,
                0x31, 0x36, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x00, 0x80, 0x01, 0xF0, 0x90,
                0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x31, 0x37, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F,
                0xBF, 0xBF, 0x00, 0x3C, 0x36, 0x14, 0x04, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20,
                0x30, 0x35, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0xEB, 0x87, 0x14, 0x08, 0xF0,
                0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x30, 0x31, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4,
                0x8F, 0xBF, 0xBF, 0xEE, 0x7F, 0x14, 0x09, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20,
                0x30, 0x32, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x8A, 0x46, 0x14, 0x0A, 0xF0,
                0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20, 0x30, 0x33, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4,
                0x8F, 0xBF, 0xBF, 0x8B, 0x38, 0x14, 0x0C, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x81, 0x20,
                0x30, 0x34, 0x20, 0xF0, 0x90, 0x80, 0x83, 0xF4, 0x8F, 0xBF, 0xBF, 0x4B, 0xB4
            };

            using (var dataStream = new System.IO.MemoryStream(rawData, writable: false))
            {
                while (dataStream.Position < dataStream.Length)
                {
                    var metadataBlock = RomMetadataBlock.Inflate(dataStream);

                    Assert.NotNull(metadataBlock);
                    var stringMetadata = metadataBlock as RomMetadataString;
                    switch (metadataBlock.Type)
                    {
                        case RomMetadataIdTag.Title:
                            Assert.True(stringMetadata.StringValue.Contains(" 00 "));
                            break;
                        case RomMetadataIdTag.Publisher:
                            var publisherMetadata = metadataBlock as RomMetadataPublisher;
                            Assert.True(publisherMetadata.Publisher.Contains(" 06 "));
                            break;
                        case RomMetadataIdTag.Credits:
                            var creditsMetadata = metadataBlock as RomMetadataCredits;
                            Assert.NotNull(creditsMetadata.Programming.First(s => s.Contains(" 10 ")));
                            Assert.NotNull(creditsMetadata.Graphics.First(s => s.Contains(" 11 ")));
                            Assert.NotNull(creditsMetadata.Music.First(s => s.Contains(" 12 ")));
                            Assert.NotNull(creditsMetadata.SoundEffects.First(s => s.Contains(" 13 ")));
                            Assert.NotNull(creditsMetadata.VoiceActing.First(s => s.Contains(" 14 ")));
                            Assert.NotNull(creditsMetadata.Documentation.First(s => s.Contains(" 15 ")));
                            Assert.NotNull(creditsMetadata.GameConceptDesign.First(s => s.Contains(" 16 ")));
                            Assert.NotNull(creditsMetadata.BoxOrOtherArtwork.First(s => s.Contains(" 17 ")));
                            Assert.NotNull(creditsMetadata.Programming.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.Graphics.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.Music.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.SoundEffects.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.VoiceActing.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.Documentation.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.GameConceptDesign.First(s => s.Contains(" 20 ")));
                            Assert.NotNull(creditsMetadata.BoxOrOtherArtwork.First(s => s.Contains(" 20 ")));
                            break;
                        case RomMetadataIdTag.UrlContactInfo:
                            Assert.True(stringMetadata.StringValue.Contains(" 05 "));
                            break;
                        case RomMetadataIdTag.ShortTitle:
                            Assert.True(stringMetadata.StringValue.Contains(" 01 "));
                            break;
                        case RomMetadataIdTag.License:
                            Assert.True(stringMetadata.StringValue.Contains(" 02 "));
                            break;
                        case RomMetadataIdTag.Description:
                            Assert.True(stringMetadata.StringValue.Contains(" 03 "));
                            break;
                        case RomMetadataIdTag.Version:
                            Assert.True(stringMetadata.StringValue.Contains(" 04 "));
                            break;
                        case RomMetadataIdTag.ReleaseDate:
                        case RomMetadataIdTag.BuildDate:
                        case RomMetadataIdTag.Features:
                        case RomMetadataIdTag.ControllerBindings:
                        default:
                            throw new InvalidOperationException("Invalid metadata in test");
                    }
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

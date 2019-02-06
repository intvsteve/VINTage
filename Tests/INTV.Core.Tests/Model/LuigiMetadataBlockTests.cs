// <copyright file="LuigiMetadataBlockTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiMetadataBlockTests
    {
        [Fact]
        public void LuigiMetadataBlock_InflateStringWithBadCharacters_ProducesEmptyStringMetadata()
        {
            using (var metadataStream = new System.IO.MemoryStream())
            {
                var stringPayload = new byte[255]; // put the values 1..255 as a "string" in a name metadata
                for (var i = 0; i < 255; ++i)
                {
                    stringPayload[i] = (byte)(256 - i);
                }
                var metadataPayload = new System.IO.MemoryStream();
                metadataPayload.WriteByte((byte)LuigiMetadataIdTag.Name);
                metadataPayload.WriteByte((byte)stringPayload.Length);
                metadataPayload.Write(stringPayload, 0, stringPayload.Length);
                var payload = metadataPayload.ToArray();
                metadataStream.WriteByte((byte)LuigiDataBlockType.Metadata);
                var payloadLengthBytes = BitConverter.GetBytes((ushort)payload.Length);
                metadataStream.Write(payloadLengthBytes, 0, payloadLengthBytes.Length);
                var headerCrc = Crc8.OfBlock(metadataStream.ToArray());
                metadataStream.WriteByte(headerCrc);
                var payloadCrc = Crc32.OfBlock(payload, Crc32Polynomial.Castagnoli);
                var payloadCrcBytes = BitConverter.GetBytes(payloadCrc);
                metadataStream.Write(payloadCrcBytes, 0, payloadCrcBytes.Length);
                metadataStream.Write(payload, 0, payload.Length);
                metadataStream.Seek(0, System.IO.SeekOrigin.Begin);

                var metadataBlock = LuigiMetadataBlock.Inflate(metadataStream) as LuigiMetadataBlock;

                Assert.True(string.IsNullOrEmpty(metadataBlock.LongNames.First()));
            }
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in 'BinaryReader' works correctly.")]
        public void LuigiMetadataBlock_InflateUnknownBlock_ProducesBaseMetadataBlock()
        {
            using (var rawMetadataBlock = new System.IO.MemoryStream())
            {
                rawMetadataBlock.WriteByte((byte)LuigiDataBlockType.Metadata);
                rawMetadataBlock.WriteByte(2);
                rawMetadataBlock.WriteByte(0); // two byte payload length
                var headerCrc = Crc8.OfBlock(rawMetadataBlock.ToArray());
                rawMetadataBlock.WriteByte(headerCrc);
                var payload = new byte[] { 0xED, 2 };
                var payloadCrc = Crc32.OfBlock(payload, Crc32Polynomial.Castagnoli);
                var payloadCrcBytes = BitConverter.GetBytes(payloadCrc);
                rawMetadataBlock.Write(payloadCrcBytes, 0, payloadCrcBytes.Length);
                rawMetadataBlock.Write(payload, 0, payload.Length);
                rawMetadataBlock.Seek(0, System.IO.SeekOrigin.Begin);

                var data = rawMetadataBlock.ToArray();
                using (var reader = new BinaryReader(rawMetadataBlock))
                {
                    var metadataBlock = LuigiMetadataBlock.Inflate(reader);

                    Assert.NotNull(metadataBlock);
                    Assert.Equal(headerCrc, metadataBlock.HeaderCrc);
                    Assert.Equal(payloadCrc, metadataBlock.PayloadCrc);
                    Assert.True(metadataBlock is LuigiMetadataBlock);
                }
            }
        }
    }
}

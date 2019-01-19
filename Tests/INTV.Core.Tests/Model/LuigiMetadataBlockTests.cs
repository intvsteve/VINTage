// <copyright file="LuigiMetadataBlockTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiMetadataBlockTests
    {
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
                var payloadCrc = Crc32.OfBlock(payload);
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

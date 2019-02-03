// <copyright file="LuigiDataBlockTests.cs" company="INTV Funhouse">
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
using System.IO;
using INTV.Core.Model;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiDataBlockTests
    {
        [Fact]
        public void LuigiDataBlock_SerializeByteCount_EqualsNegativeOne()
        {
            var block = new LuigiTestDataBlock();

            Assert.Equal(-1, block.SerializeByteCount);
        }

        [Fact]
        public void LuigiDataBlock_Serialize_ThrowsNotImplementedException()
        {
            var block = new LuigiTestDataBlock();

            Assert.Throws<NotImplementedException>(() => block.Serialize(null));
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiScrambleKeyBlock_ReturnsSetScrambleKey()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiScrambleKeyBlock>();

            Assert.Equal(LuigiDataBlockType.SetScrambleKey, blockType);
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiMemoryMapAndPermissionsTableBlock_ReturnsMemoryMapAndPermissionsTable()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiMemoryMapAndPermissionsTableBlock>();

            Assert.Equal(LuigiDataBlockType.MemoryMapAndPermissionsTable, blockType);
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiDataHunkBlock_ReturnsDataHunk()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiDataHunkBlock>();

            Assert.Equal(LuigiDataBlockType.DataHunk, blockType);
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiMetadataBlock_ReturnsMetadata()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiMetadataBlock>();

            Assert.Equal(LuigiDataBlockType.Metadata, blockType);
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiEndOfFileBlock_ReturnsEndOfFile()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiEndOfFileBlock>();

            Assert.Equal(LuigiDataBlockType.EndOfFile, blockType);
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiDataBlock_ReturnsUnknownBlockType()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiDataBlock>();

            Assert.Equal(LuigiDataBlockType.UnknownBlockType, blockType);
        }

        [Fact]
        public void LuigiDataBlock_GetBlockTypeFromLuigiTestDataBlock_ReturnsUnknownBlockType()
        {
            var blockType = LuigiDataBlock.GetBlockType<LuigiTestDataBlock>();

            Assert.Equal(LuigiDataBlockType.UnknownBlockType, blockType);
        }

        [Fact]
        public void LuigiDataBlock_DeserializeEmptyUnknownBlockType_ReturnsCorrectNumberOfBytes()
        {
            using (var fakeLuigiBlockData = new MemoryStream())
            {
                fakeLuigiBlockData.WriteByte((byte)LuigiTestDataBlock.BlockType); // bogus block type
                fakeLuigiBlockData.WriteByte(0);
                fakeLuigiBlockData.WriteByte(0); // zero payload length
                var crc = Crc8.OfBlock(fakeLuigiBlockData.ToArray());
                fakeLuigiBlockData.WriteByte(crc);
                fakeLuigiBlockData.Seek(0, SeekOrigin.Begin);

                var luigiDataBlock = LuigiDataBlock.Inflate(fakeLuigiBlockData);

                Assert.Equal(LuigiTestDataBlock.BlockType, luigiDataBlock.Type);
                Assert.Equal(crc, luigiDataBlock.HeaderCrc);
                Assert.Equal(0u, luigiDataBlock.PayloadCrc);
            }
        }

        private class LuigiTestDataBlock : LuigiDataBlock
        {
            public const LuigiDataBlockType BlockType = (LuigiDataBlockType)0xFD;

            public LuigiTestDataBlock()
                : base(BlockType)
            {
            }
        }
    }
}

// <copyright file="ByteSerializerTests.cs" company="INTV Funhouse">
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

using System.Text;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class ByteSerializerTests
    {
        [Fact]
        public void ByteSerializer_SerializeToStream_SerializesExpectedNumberOfBytes()
        {
            using (var memory = new System.IO.MemoryStream())
            {
                var data = new SerializeToBytesTester(69);
                Assert.Equal(sizeof(int), data.Serialize(memory, Encoding.ASCII));
                Assert.Equal(sizeof(int), memory.Length);
            }
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in BinaryWriter' works correctly.")]
        public void ByteSerializer_SerializeUsingBinaryWriter_SerializesExpectedNumberOfBytes()
        {
            using (var memory = new System.IO.MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    var data = new SerializeToBytesTester(42);
                    Assert.Equal(sizeof(int), data.Serialize(writer));
                }
                Assert.Equal(sizeof(int), memory.Length);
            }
        }

        [Fact]
        public void ByteSerializer_SerializeThenDeserialzeViaStream_CorrectlyInflatesData()
        {
            var data = new SerializeToBytesTester(69);
            SerializeToBytesTester deserializedData = null;

            using (var memory = new System.IO.MemoryStream())
            {
                data.Serialize(memory, Encoding.ASCII);
                memory.Seek(0, System.IO.SeekOrigin.Begin);
                deserializedData = SerializeToBytesTester.Inflate<SerializeToBytesTester>(memory);
            }

            Assert.Equal(data.MuhDater, deserializedData.MuhDater);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in BinaryReader / BinaryWriter' works correctly.")]
        public void ByteSerializer_SerializeThenDeserialzeViaBinaryReaderWriter_CorrectlyInflatesData()
        {
            var data = new SerializeToBytesTester(42);
            SerializeToBytesTester deserializedData = null;

            using (var memory = new System.IO.MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    data.Serialize(writer);
                }
                using (var reader = new BinaryReader(memory))
                {
                    memory.Seek(0, System.IO.SeekOrigin.Begin);
                    deserializedData = SerializeToBytesTester.Inflate<SerializeToBytesTester>(reader);
                }
            }

            Assert.Equal(data.MuhDater, deserializedData.MuhDater);
        }

        private class SerializeToBytesTester : ByteSerializer
        {
            public SerializeToBytesTester()
            {
                MuhDater = -1;
            }

            public SerializeToBytesTester(int muhDater)
            {
                MuhDater = muhDater;
            }

            public int MuhDater { get; private set; }

            public override int SerializeByteCount
            {
                get { return sizeof(int); }
            }

            public override int DeserializeByteCount
            {
                get { return sizeof(int); }
            }

            public override int Serialize(BinaryWriter writer)
            {
                writer.Write(MuhDater);
                return sizeof(int);
            }

            public override int Deserialize(BinaryReader reader)
            {
                MuhDater = reader.ReadInt32();
                return sizeof(int);
            }
        }
    }
}

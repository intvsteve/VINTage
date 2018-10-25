// <copyright file="RomMetadataFeaturesTests.cs" company="INTV Funhouse">
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

using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomMetadataFeaturesTests
    {
        [Fact]
        public void RomMetadataFeatures_CreateWithZeroDataLength_DeserializeDoesNotChangeFeatures()
        {
            var featuresMetadata = new RomMetadataFeatures(0);
            var expectedFeatures = featuresMetadata.Features;

            using (var stream = new System.IO.MemoryStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    featuresMetadata.Deserialize(reader);
                }
            }

            Assert.Equal(expectedFeatures, featuresMetadata.Features);
        }

        [Fact]
        public void RomMetadataFeatures_CreateWithDataLengthButInsufficientDataBufferAndDeserialize_ThrowsEndOfStreamException()
        {
            var featuresMetadata = new RomMetadataFeatures(1);
            var expectedFeatures = featuresMetadata.Features;

            using (var stream = new System.IO.MemoryStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => featuresMetadata.Deserialize(reader));
                }
            }
        }

        [Fact]
        public void RomMetadataFeatures_CreateWithAdditionalDataAndDeserialize_MovesToEndOfBuffer()
        {
            var dataSize = 10;
            var featuresMetadata = new RomMetadataFeatures((uint)dataSize);
            var expectedFeatures = ProgramFeatures.DefaultFeatures;

            using (var stream = new System.IO.MemoryStream())
            {
                var data = Enumerable.Repeat((byte)0, dataSize);
                stream.Write(data.ToArray(), 0, dataSize);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                using (var reader = new BinaryReader(stream))
                {
                    featuresMetadata.Deserialize(reader);
                }
            }

            Assert.Equal(expectedFeatures, featuresMetadata.Features);
        }
    }
}

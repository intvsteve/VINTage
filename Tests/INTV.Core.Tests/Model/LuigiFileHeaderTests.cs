// <copyright file="LuigiFileHeaderTests.cs" company="INTV Funhouse">
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
using System.Text;
using INTV.Core.Model;
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class LuigiFileHeaderTests
    {
        [Fact]
        public void LuigiFileHeader_SerializeByteCount_IsNegativeOne()
        {
            var header = new LuigiFileHeader();

            Assert.Equal(-1, header.SerializeByteCount);
        }

        [Theory]
        [InlineData(TestRomResources.TestLuigiFromBinPath, null, true)]
        [InlineData(TestRomResources.TestRomPath, null, false)]
        [InlineData(TestRomResources.TestBinPath, "/Really/not/a/valid.luigi", false)]
        public void LuigiFileHeader_IsPotentialLuigiFile_ProvidesCorrectResult(string resourceToRegister, string potentialLuigiFilePath, bool expectedIsPotentialLuigiFile)
        {
            IReadOnlyList<StorageLocation> paths;
            var storage = LuigiFileHeaderTestStorageAccess.Initialize(out paths, resourceToRegister);
            var romFilePath = paths.First();
            if (string.IsNullOrEmpty(potentialLuigiFilePath))
            {
                potentialLuigiFilePath = romFilePath.Path;
            }

            Assert.Equal(expectedIsPotentialLuigiFile, LuigiFileHeader.PotentialLuigiFile(storage.CreateLocation(potentialLuigiFilePath)));
        }

        [Fact]
        public void LuigiFileHeader_IsPotentialLuigiFileWithInvalidLocation_ReturnsFalse()
        {
            LuigiFileHeaderTestStorageAccess.Initialize();

            Assert.False(LuigiFileHeader.PotentialLuigiFile(StorageLocation.InvalidLocation));
        }

        [Fact]
        public void LuigiFileHeader_IsPotentialLuigiFileWithNullLocation_ReturnsFalse()
        {
            var storage = LuigiFileHeaderTestStorageAccess.Initialize();

            Assert.False(LuigiFileHeader.PotentialLuigiFile(storage.NullLocation));
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithBadMagicKey_ThrowsUnexpectedFileTypeException()
        {
            var bogusHeaderMagic = "LT0";
            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(bogusHeaderMagic)))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<UnexpectedFileTypeException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedMagicKey_ThrowsUnexpectedFileTypeException()
        {
            var bogusHeaderMagic = "LT";
            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(bogusHeaderMagic)))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<UnexpectedFileTypeException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithFutureVersion_ThrowsInvalidOperationException()
        {
            var futureHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0xD0, // LTO, version -- bogus instead of 0x01
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1)
                0x81, 0x93, 0x43, 0xB0, 0x0C, 0x47, 0x3F, 0x12, // UID
                0x00, 0x00, 0x00, // reserved
                0x8B // CRC
            };
            using (var stream = new System.IO.MemoryStream(futureHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<InvalidOperationException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithBogusCrc_ThrowsInvalidDataException()
        {
            var badCrcHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1)
                0x81, 0x93, 0x43, 0xB0, 0x0C, 0x47, 0x3F, 0x12, // UID
                0x00, 0x00, 0x00, // reserved
                0x22 // CRC - should be 0x8B
            };
            using (var stream = new System.IO.MemoryStream(badCrcHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var exception = Assert.Throws<System.IO.InvalidDataException>(() => LuigiFileHeader.Inflate(reader));
                    var expectedMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.InvalidDataBlockChecksumFormat, 0x8B, 0x22);
                    Assert.Equal(expectedMessage, exception.Message);
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithCorruptedFeatures_ThrowsInvalidDataException()
        {
            var badCrcHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1) (corrupted)
                0x81, 0x93, 0x43, 0xB0, 0x0C, 0x47, 0x3F, 0x12, // UID
                0x00, 0x00, 0x00, // reserved
                0x8B // CRC
            };
            using (var stream = new System.IO.MemoryStream(badCrcHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var exception = Assert.Throws<System.IO.InvalidDataException>(() => LuigiFileHeader.Inflate(reader));
                    var expectedMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.InvalidDataBlockChecksumFormat, 0x61, 0x8B);
                    Assert.Equal(expectedMessage, exception.Message);
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedHeaderAfterKey_ThrowsEndOfStreamException()
        {
            var truncatedHeader = new byte[]
            {
                0x4C, 0x54, 0x4F // LTO
            };
            using (var stream = new System.IO.MemoryStream(truncatedHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedHeaderInFeatures_ThrowsEndOfStreamException()
        {
            var truncatedHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00 // features (v1)
            };
            using (var stream = new System.IO.MemoryStream(truncatedHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedHeaderInFeatures2_ThrowsEndOfStreamException()
        {
            var truncatedHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1)
            };
            using (var stream = new System.IO.MemoryStream(truncatedHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedHeaderInUid_ThrowsEndOfStreamException()
        {
            var truncatedHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1)
            };
            using (var stream = new System.IO.MemoryStream(truncatedHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedHeaderInReservedData_ThrowsEndOfStreamException()
        {
            var truncatedHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1)
                0x81, 0x93, 0x43, 0xB0, 0x0C, 0x47, 0x3F, 0x12, // UID
                0x00, 0x00, // reserved
            };
            using (var stream = new System.IO.MemoryStream(truncatedHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_DeserializeWithTruncatedHeader_ThrowsEndOfStreamException()
        {
            var truncatedHeader = new byte[]
            {
                0x4C, 0x54, 0x4F, 0x01, // LTO, version
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // features (v1)
                0x81, 0x93, 0x43, 0xB0, 0x0C, 0x47, 0x3F, 0x12, // UID
                0x00, 0x00, 0x00 // reserved
            };
            using (var stream = new System.IO.MemoryStream(truncatedHeader))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Assert.Throws<System.IO.EndOfStreamException>(() => LuigiFileHeader.Inflate(reader));
                }
            }
        }

        [Fact]
        public void LuigiFileHeader_GetFromValidLuigiRom_DoesNotThrowAndReportsExpectedVersion()
        {
            IReadOnlyList<StorageLocation> paths;
            var storage = LuigiFileHeaderTestStorageAccess.Initialize(out paths, TestRomResources.TestLuigiFromRomPath);

            using (var reader = new BinaryReader(storage.Open(paths.First().Path)))
            {
                var header = LuigiFileHeader.Inflate(reader);

                Assert.Equal(1, header.Version);
                Assert.Equal(32, header.DeserializeByteCount);
            }
        }

        [Fact]
        public void LuigiFileHeader_ReadVersionZeroHeader_DoesNotThrowAndReportsVersionZero()
        {
            var romPath = LuigiFileHeaderTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinWithVersionZeroHeaderPath).First();

            var header = LuigiFileHeader.GetHeader(romPath);

            Assert.Equal(0, header.Version);
        }

        [Theory]
        [InlineData(LuigiFeatureFlags.None, false, false)]
        [InlineData(LuigiFeatureFlags.FeatureFlagsExplicitlySet, false, false)]
        [InlineData(LuigiFeatureFlags.TutorVisionMask, false, true)]
        [InlineData(LuigiFeatureFlags.None, true, false)]
        [InlineData(LuigiFeatureFlags.EcsMask, true, true)]
        [InlineData(LuigiFeatureFlags.FeatureFlagsExplicitlySet, true, false)]
        [InlineData(LuigiFeatureFlags.FeatureFlagsExplicitlySet | LuigiFeatureFlags.KeyboardComponentMask, true, true)]
        public void LuigiFileHeader_WouldModifyFeatures_ProducesCorrectResult(LuigiFeatureFlags newFeatures, bool forceFeatureUpdate, bool expectedWouldModify)
        {
            var header = new LuigiFileHeader();

            Assert.Equal(expectedWouldModify, header.WouldModifyFeatures(newFeatures, forceFeatureUpdate));
        }

        [Theory]
        [InlineData(LuigiFeatureFlags.EcsMask, false)]
        [InlineData((LuigiFeatureFlags)0x55, true)]
        public void LuigiFileHeader_UpdateFeaturesOnLuigiWithMetadata_UpdatesFeatures(LuigiFeatureFlags newFeatures, bool forceFeatureUpdate)
        {
            var romPath = LuigiFileHeaderTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForDevice1Path).First();

            var header = LuigiFileHeader.GetHeader(romPath);
            var originalCrc = header.Crc;
            var expectedFeatures = header.WouldModifyFeatures(newFeatures, forceFeatureUpdate) ? newFeatures : header.Features;
            Assert.Equal(0x8B, originalCrc);

            var crcShouldChange = header.Features != expectedFeatures;
            header.UpdateFeatures(newFeatures, forceFeatureUpdate);

            Assert.Equal(expectedFeatures, header.Features);
            Assert.Equal(crcShouldChange, header.Crc != originalCrc);
        }

        [Theory]
        [InlineData(LuigiFeatureFlags.EcsMask, false)]
        [InlineData((LuigiFeatureFlags)0x55, true)]
        public void LuigiFileHeader_UpdateFeaturesOnScrambledLuigiRom_UpdatesFeatures(LuigiFeatureFlags newFeatures, bool forceFeatureUpdate)
        {
            var romPath = LuigiFileHeaderTestStorageAccess.Initialize(TestRomResources.TestLuigiScrambledForDevice0Path).First();

            var header = LuigiFileHeader.GetHeader(romPath);
            var originalCrc = header.Crc;
            Assert.Equal(0xEC, header.Crc);

            var crcShouldChange = header.Features != newFeatures;
            header.UpdateFeatures(newFeatures, forceFeatureUpdate);

            Assert.Equal(newFeatures, header.Features);
            Assert.Equal(crcShouldChange, header.Crc != originalCrc);
        }

        [Fact]
        public void LuigiFileHeader_SerializeDefaultVersionZeroHeader_SerializesCorrectNumberOfBytes()
        {
            var header = new LuigiFileHeader() { Reserved = new byte[LuigiFileHeader.ReservedHeaderBytesSize] };

            using (var writer = new BinaryWriter(new System.IO.MemoryStream()))
            {
                var bytesSerialized = header.Serialize(writer);

                Assert.Equal(16, bytesSerialized);
            }
        }

        [Fact]
        public void LuigiFileHeader_SerializeHeaderFromTestRom_SerializesCorrectNumberOfBytes()
        {
            var romPath = LuigiFileHeaderTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var header = LuigiFileHeader.GetHeader(romPath);

            using (var writer = new BinaryWriter(new System.IO.MemoryStream()))
            {
                var bytesSerialized = header.Serialize(writer);

                Assert.Equal(32, bytesSerialized);
            }
        }

        private class LuigiFileHeaderTestStorageAccess : CachedResourceStorageAccess<LuigiFileHeaderTestStorageAccess>
        {
        }
    }
}

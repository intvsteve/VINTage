// <copyright file="RomComparerStrictCrcOnlyTests.cs" company="INTV Funhouse">
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

using System.Linq;
using INTV.Core.Model;
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomComparerStrictCrcOnlyTests
    {
        [Theory]
        [InlineData(TestRomResources.TestRomPath, TestRomResources.TestRomPath, 0)]
        [InlineData(TestRomResources.TestRomPath, TestRomResources.TestRomMetadataPath, 0x3EB551A3)]
        public void RomComparerStrictCrcOnlyTests_CompareRoms_CompareCorrectly(string firstRomPath, string secondRomPath, int expectedCompareResult)
        {
            var paths = RomComparerStrictCrcOnlyTestStorageAccess.Initialize(firstRomPath, secondRomPath);
            var rom0 = Rom.Create(paths[0], StorageLocation.InvalidLocation);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], StorageLocation.InvalidLocation);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom0, rom1);

            Assert.Equal(expectedCompareResult, compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareMissingRoms_ReturnPathCompareResult()
        {
            var storage = RomComparerStrictCrcOnlyTestStorageAccess.Initialize();
            var rom0 = new XmlRom();
            rom0.UpdateRomPath(storage.CreateLocation("/where/is/waldo.bin"));
            var rom1 = new XmlRom(); 
            rom1.UpdateRomPath(storage.CreateLocation("where/in/the/world/is/carmen/sandiego.rom"));

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom0, rom1);

            Assert.Equal(rom0.RomPath.CompareTo(rom1.RomPath), compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareCorruptAndMissingRoms_ReturnPathCompareResult()
        {
            var storage = RomComparerStrictCrcOnlyTestStorageAccess.Initialize();
            var rom0 = new CorruptedTestRom(storage.CreateLocation("whos/on/first.rom"));
            var rom1 = new XmlRom(); 
            rom1.UpdateRomPath(storage.CreateLocation("/what/is/on/second.bin"));

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom0, rom1);

            Assert.Equal(rom0.RomPath.CompareTo(rom1.RomPath), compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareRomToItself_ReturnsZero()
        {
            var romPath = RomComparerStrictCrcOnlyTestStorageAccess.Initialize(TestRomResources.TestRomPath).First();
            var rom = Rom.Create(romPath, StorageLocation.InvalidLocation);
            Assert.NotNull(rom);

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom, rom);

            Assert.Equal(0, compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareTwoNullRoms_ReturnsZero()
        {
            var compareResult = RomComparerStrictCrcOnly.Default.Compare(null, null);

            Assert.Equal(0, compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareRomToNull_ReturnsOne()
        {
            var romPath = RomComparerStrictCrcOnlyTestStorageAccess.Initialize(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.Create(romPath, StorageLocation.InvalidLocation);
            Assert.NotNull(rom);

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom, null);

            Assert.Equal(1, compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareNullToRom_ReturnsNegativeOne()
        {
            var paths = RomComparerStrictCrcOnlyTestStorageAccess.Initialize(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
            var rom = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom);

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(null, rom);

            Assert.Equal(-1, compareResult);
        }

        private class RomComparerStrictCrcOnlyTestStorageAccess : CachedResourceStorageAccess<RomComparerStrictCrcOnlyTestStorageAccess>
        {
        }

        private class CorruptedTestRom : IRom
        {
            public CorruptedTestRom(StorageLocation path)
            {
                RomPath = path;
            }

            public RomFormat Format
            {
                get { return RomFormat.CuttleCart3Advanced; }
            }

            public StorageLocation RomPath { get; private set; }

            public StorageLocation ConfigPath
            {
                get { return StorageLocation.InvalidLocation; }
            }

            public bool IsValid
            {
                get { return false; }
            }

            public uint Crc
            {
                get { return 0; }
            }

            public uint CfgCrc
            {
                get { return 0; }
            }

            public bool Validate()
            {
                return false;
            }

            public uint RefreshCrc(out bool changed)
            {
                changed = false;
                return Crc;
            }

            public uint RefreshCfgCrc(out bool changed)
            {
                changed = false;
                return CfgCrc;
            }
        }
    }
}

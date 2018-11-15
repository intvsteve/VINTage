// <copyright file="RomComparerStrictCrcOnlyTests.cs" company="INTV Funhouse">
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
            var paths = RomComparerStrictCrcOnlyTestStorageAccess.InitializeStorageWithCopiesOfResources(firstRomPath, secondRomPath);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom0, rom1);

            Assert.Equal(expectedCompareResult, compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareMissingRoms_ReturnPathCompareResult()
        {
            RomComparerStrictCrcOnlyTestStorageAccess.Initialize(null);
            var rom0 = new XmlRom();
            rom0.UpdateRomPath("/where/is/waldo.bin");
            var rom1 = new XmlRom(); 
            rom1.UpdateRomPath("where/in/the/world/is/carmen/sandiego.rom");

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom0, rom1);

            Assert.Equal(string.Compare(rom0.RomPath, rom1.RomPath), compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareCorruptAndMissingRoms_ReturnPathCompareResult()
        {
            RomComparerStrictCrcOnlyTestStorageAccess.Initialize(null);
            var rom0 = new CorruptedTestRom("whos/on/first.rom");
            var rom1 = new XmlRom(); 
            rom1.UpdateRomPath("/what/is/on/second.bin");

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom0, rom1);

            Assert.Equal(string.Compare(rom0.RomPath, rom1.RomPath), compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareRomToItself_ReturnsZero()
        {
            var romPath = RomComparerStrictCrcOnlyTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomPath).First();
            var rom = Rom.Create(romPath, null);
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
            var romPath = RomComparerStrictCrcOnlyTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromBinPath).First();
            var rom = Rom.Create(romPath, null);
            Assert.NotNull(rom);

            var compareResult = RomComparerStrictCrcOnly.Default.Compare(rom, null);

            Assert.Equal(1, compareResult);
        }

        [Fact]
        public void RomComparerStrictCrcOnlyTests_CompareNullToRom_ReturnsNegativeOne()
        {
            var paths = RomComparerStrictCrcOnlyTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgPath);
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
            public CorruptedTestRom(string path)
            {
                RomPath = path;
            }

            public RomFormat Format
            {
                get { return RomFormat.CuttleCart3Advanced; }
            }

            public string RomPath { get; private set; }

            public string ConfigPath
            {
                get { return null; }
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

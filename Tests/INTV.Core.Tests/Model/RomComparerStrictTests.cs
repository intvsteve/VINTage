// <copyright file="RomComparerStrictTests.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Model;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomComparerStrictTests
    {
        [Fact]
        public void RomComparerStrict_CompareTwoBinFormatRomsWithDifferentCfgFile_CompareAsDifferent()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestBinMetadataPath, TestRomResources.TestCfgMetadataPath);
            var rom0 = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[2], paths[3]);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.NotEqual(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareTwoBinFormatRomsFirstWithoutCfgFile_CompareAsDifferent()
        {
            IReadOnlyList<string> paths;
            RomComparerStrictTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestBinMetadataPath)
                .WithStockCfgResources();
            var rom0 = Rom.Create(paths[2], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom1);

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(rom0.RomPath))
            using (IRomHelpersSupport.AddSelfCleaningRomInfo(rom1.RomPath))
            {
                var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

                Assert.NotEqual(0, compareResult);
            }
        }

        [Fact]
        public void RomComparerStrict_CompareTwoBinFormatRomsSecondWithoutCfgFile_CompareAsDifferent()
        {
            IReadOnlyList<string> paths;
            RomComparerStrictTestStorageAccess.Initialize(out paths, TestRomResources.TestBinPath, TestRomResources.TestCfgPath, TestRomResources.TestBinMetadataPath)
                .WithStockCfgResources();
            var rom0 = Rom.Create(paths[0], paths[1]);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[2], null);
            Assert.NotNull(rom1);

            using (IRomHelpersSupport.AddSelfCleaningRomInfo(rom0.RomPath))
            using (IRomHelpersSupport.AddSelfCleaningRomInfo(rom1.RomPath))
            {
                var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

                Assert.NotEqual(0, compareResult);
            }
        }

        [Fact]
        public void RomComparerStrict_CompareIdenticalRomFormatRoms_CompareAsSame()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomPath, TestRomResources.TestRomPath);
            Assert.NotEqual(paths[0], paths[1]);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var duplicateRomPath = paths[1];
            var rom1 = Rom.Create(duplicateRomPath, null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.Equal(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareDifferentRomFormatRoms_CompareAsDifferent()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestRomPath, TestRomResources.TestRomMetadataPath);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.NotEqual(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareTwoIdenticalLuigiFormatRoms_CompareAsSame()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromBinPath, TestRomResources.TestLuigiFromBinPath);
            Assert.NotEqual(paths[0], paths[1]);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var duplicateLuigiPath = paths[1];
            var rom1 = Rom.Create(duplicateLuigiPath, null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.Equal(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareTwoAlmostIdenticalLuigiFormatRoms_CompareAsDifferent()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromBinPath, TestRomResources.TestLuigiWithExtraNullBytePath);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.NotEqual(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareTwoDifferentLuigiFormatRoms_CompareAsDifferent()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromBinPath, TestRomResources.TestLuigiWithMetadataPath);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.NotEqual(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareLuigiFromBinToLuigiFromRom_CompareAsDifferent()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromBinPath, TestRomResources.TestLuigiFromRomPath);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.NotEqual(0, compareResult);
        }

        [Fact]
        public void RomComparerStrict_CompareLuigiFormatRomToRomFormatRom_CompareAsDifferent()
        {
            var paths = RomComparerStrictTestStorageAccess.InitializeStorageWithCopiesOfResources(TestRomResources.TestLuigiFromBinPath, TestRomResources.TestRomPath);
            var rom0 = Rom.Create(paths[0], null);
            Assert.NotNull(rom0);
            var rom1 = Rom.Create(paths[1], null);
            Assert.NotNull(rom1);

            var compareResult = RomComparerStrict.Default.Compare(rom0, rom1);

            Assert.NotEqual(0, compareResult);
        }

        private class RomComparerStrictTestStorageAccess : CachedResourceStorageAccess<RomComparerStrictTestStorageAccess>
        {
        }
    }
}

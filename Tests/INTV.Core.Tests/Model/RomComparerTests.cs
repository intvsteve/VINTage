// <copyright file="RomComparerTests.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomComparerTests
    {
        private const RomComparison BogusComparison = (RomComparison)12345;

        public static IEnumerable<object[]> RomComparisonModeData
        {
            get
            {
                var romComparisonModes = Enum.GetValues(typeof(RomComparison)).Cast<RomComparison>();
                foreach (var romComparisonMode in romComparisonModes)
                {
                    Type expectedComparerType = null;
                    var shouldThrowArgumentException = false;
                    switch (romComparisonMode)
                    {
                        case RomComparison.Strict:
                            expectedComparerType = typeof(RomComparerStrict);
                            break;
                        case RomComparison.StrictRomCrcOnly:
                            expectedComparerType = typeof(RomComparerStrictCrcOnly);
                            break;
                        case RomComparison.CanonicalStrict:
#if CANONICAL_COMPARE_SUPPORTED
                            expectedComparerType = typeof(CanonicalRomComparerStrict);
#else
                            shouldThrowArgumentException = true;
#endif // CANONICAL_COMPARE_SUPPORTED
                            break;
                        case RomComparison.CanonicalRomCrcOnly:
#if CANONICAL_COMPARE_SUPPORTED
                            expectedComparerType = typeof(CanonicalRomComparer);
#else
                            shouldThrowArgumentException = true;
#endif // CANONICAL_COMPARE_SUPPORTED
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    yield return new object[] { romComparisonMode, expectedComparerType, shouldThrowArgumentException };
                }
                yield return new object[] { BogusComparison, null, true };
            }
        }

        [Theory]
        [MemberData("RomComparisonModeData")]
        public void RomComparer_GetComparerForMode_BehavesAsExpected(RomComparison comparisonMode, Type expectedComparisonType, bool shouldThrowArgumentException)
        {
            if (shouldThrowArgumentException)
            {
                Assert.Throws<ArgumentException>(() => RomComparer.GetComparer(comparisonMode));
            }
            else
            {
                var comparer = RomComparer.GetComparer(comparisonMode);

                Assert.NotNull(comparer);
                Assert.True(expectedComparisonType.IsInstanceOfType(comparer));
            }
        }

        public static IEnumerable<object[]> DefaultRomComparisonModeData
        {
            get
            {
                var romComparisonModes = Enum.GetValues(typeof(RomComparison)).Cast<RomComparison>();
                foreach (var romComparisonMode in romComparisonModes)
                {
                    RomComparer expectedComparer = null;
                    var shouldThrowArgumentException = false;
                    switch (romComparisonMode)
                    {
                        case RomComparison.Strict:
                            expectedComparer = RomComparerStrict.Default;
                            break;
                        case RomComparison.StrictRomCrcOnly:
                            expectedComparer = RomComparerStrictCrcOnly.Default;
                            break;
                        case RomComparison.CanonicalStrict:
#if CANONICAL_COMPARE_SUPPORTED
                            expectedComparer = CanonicalRomComparerStrict.Default;
#else
                            shouldThrowArgumentException = true;
#endif // CANONICAL_COMPARE_SUPPORTED
                            break;
                        case RomComparison.CanonicalRomCrcOnly:
#if CANONICAL_COMPARE_SUPPORTED
                            expectedComparer = CanonicalRomComparer.Default;
#else
                            shouldThrowArgumentException = true;
#endif // CANONICAL_COMPARE_SUPPORTED
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    yield return new object[] { romComparisonMode, expectedComparer, shouldThrowArgumentException };
                }
                yield return new object[] { BogusComparison, null, true };
            }
        }

        [Theory]
        [MemberData("DefaultRomComparisonModeData")]
        public void RomComparer_GetDefaultComparerForMode_BehavesAsExpected(RomComparison comparisonMode, RomComparer expectedComparer, bool shouldThrowArgumentException)
        {
            if (shouldThrowArgumentException)
            {
                Assert.Throws<ArgumentException>(() => RomComparer.GetDefaultComparerForMode(comparisonMode));
            }
            else
            {
                var comparer = RomComparer.GetDefaultComparerForMode(comparisonMode);

                Assert.NotNull(comparer);
                Assert.True(object.ReferenceEquals(expectedComparer, comparer));
            }
        }

        [Fact]
        public void RomComparer_CompareTwoNullObjects_ReturnsEqual()
        {
            object x = null;
            object y = null;

            Assert.Equal(0, RomComparer.GetDefaultComparerForMode(RomComparer.DefaultCompareMode).Compare(x, y));
        }

        [Fact]
        public void RomComparer_CompareTwoNonRoms_ThrowsArgumentException()
        {
            var x = new object();
            var y = Array.CreateInstance(typeof(int), 1);

            Assert.Throws<ArgumentException>(() => RomComparer.GetDefaultComparerForMode(RomComparer.DefaultCompareMode).Compare(x, y));
        }

        [Fact]
        public void RomComparer_FirstNotRomSecondNull_ThrowsArgumentException()
        {
            var x = new object();

            Assert.Throws<ArgumentException>(() => RomComparer.GetDefaultComparerForMode(RomComparer.DefaultCompareMode).Compare(x, null));
        }

        [Fact]
        public void RomComparer_FirstNullRomSecondNotRom_ThrowsArgumentException()
        {
            var y = new object();

            Assert.Throws<ArgumentException>(() => RomComparer.GetDefaultComparerForMode(RomComparer.DefaultCompareMode).Compare(null, y));
        }

        [Fact]
        public void RomComparer_FirstNotRomSecondIsRom_ThrowsArgumentException()
        {
            RomComparerTestStorageAccess.Initialize(TestRomResources.TestRomPath);
            var rom = Rom.Create(TestRomResources.TestRomPath, null);
            Assert.NotNull(rom);
            var x = new object();

            using (var comparer = RomComparer.GetComparer(RomComparison.Strict))
            {
                Assert.Throws<ArgumentException>(() => comparer.Compare(x, rom));
            }
        }

        [Fact]
        public void RomComparer_FirstIsRomSecondNotRom_ThrowsArgumentException()
        {
            RomComparerTestStorageAccess.Initialize(TestRomResources.TestBinPath);
            var rom = Rom.Create(TestRomResources.TestBinPath, null);
            Assert.NotNull(rom);
            var y = new object();

            using (var comparer = RomComparer.GetComparer(RomComparison.StrictRomCrcOnly))
            {
                Assert.Throws<ArgumentException>(() => comparer.Compare(rom, y));
            }
        }

        [Theory]
        [InlineData(RomComparison.Strict)]
        [InlineData(RomComparison.StrictRomCrcOnly)]
        public void RomComparer_CompareTwoDifferentRomsAsObjects_ProducesCorrectResult(RomComparison comparisonMode)
        {
            RomComparerTestStorageAccess.Initialize(TestRomResources.TestRomPath, TestRomResources.TestBinPath);
            object rom0 = Rom.Create(TestRomResources.TestRomPath, null);
            object rom1 = Rom.Create(TestRomResources.TestBinPath, null);

            using (var comparer = RomComparer.GetComparer(comparisonMode))
            {
                Assert.NotEqual(0, comparer.Compare(rom0, rom1));
            }
        }

        [Theory]
        [InlineData(RomComparison.Strict)]
        [InlineData(RomComparison.StrictRomCrcOnly)]
        public void RomComparer_CompareTwoSameRomsAsObjects_ProducesCorrectResult(RomComparison comparisonMode)
        {
            var storageAccess = RomComparerTestStorageAccess.Initialize(TestRomResources.TestRomPath);
            string romCopy = "/Resources/RomComparer_CompareTwoSameRomsAsObjects_ProducesCorrectResult/test_rom.rom";
            storageAccess.CreateCopyOfResource(TestRomResources.TestRomPath, romCopy);
            object rom0 = Rom.Create(TestRomResources.TestRomPath, null);
            object rom1 = Rom.Create(romCopy, null);

            using (var comparer = RomComparer.GetComparer(comparisonMode))
            {
                Assert.Equal(0, comparer.Compare(rom0, rom1));
            }
        }

        [Theory]
        [InlineData(RomComparison.Strict)]
        [InlineData(RomComparison.StrictRomCrcOnly)]
        public void RomComparer_CompareRomToNullsAsObjects_ProducesCorrectResult(RomComparison comparisonMode)
        {
            var storageAccess = RomComparerTestStorageAccess.Initialize(TestRomResources.TestRomPath);
            object rom0 = Rom.Create(TestRomResources.TestRomPath, null);
            object rom1 = null;

            using (var comparer = RomComparer.GetComparer(comparisonMode))
            {
                Assert.Equal(1, comparer.Compare(rom0, rom1));
            }
        }

        [Theory]
        [InlineData(RomComparison.Strict)]
        [InlineData(RomComparison.StrictRomCrcOnly)]
        public void RomComparer_CompareNullToRomsAsObjects_ProducesCorrectResult(RomComparison comparisonMode)
        {
            var storageAccess = RomComparerTestStorageAccess.Initialize(TestRomResources.TestRomPath);
            object rom0 = null;
            object rom1 = Rom.Create(TestRomResources.TestRomPath, null);

            using (var comparer = RomComparer.GetComparer(comparisonMode))
            {
                Assert.Equal(-1, comparer.Compare(rom0, rom1));
            }
        }

        [Theory]
        [InlineData(RomComparison.Strict)]
        [InlineData(RomComparison.StrictRomCrcOnly)]
        public void RomComparer_CompareNullToNullAsIRoms_ProducesCorrectResult(RomComparison comparisonMode)
        {
            IRom rom0 = null;
            IRom rom1 = null;

            using (var comparer = RomComparer.GetComparer(comparisonMode))
            {
                Assert.Equal(0, comparer.Compare(rom0, rom1));
            }
        }

        private class RomComparerTestStorageAccess : CachedResourceStorageAccess<RomComparerTestStorageAccess>
        {
        }
    }
}

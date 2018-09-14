// <copyright file="RangeTests.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Utility
{
    public class RangeTests
    {
        [Fact]
        public void RangeUnspecified_IsNotValid()
        {
            var range = new Range<int>();
            Assert.Equal(0, range.Minimum);
            Assert.Equal(0, range.Maximum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeIncorrectlySpecified_IsNotValid()
        {
            var range = new Range<int>(3, -1);
            Assert.Equal(3, range.Minimum);
            Assert.Equal(-1, range.Maximum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeIncorrectlySpecified_Copy_CopyIsNotValidAndUsesDefaultMinAndMax()
        {
            var badRange = new Range<long>(4, -10);
            var rangeCopy = new Range<long>(badRange);
            Assert.False(rangeCopy.IsValid);
            Assert.Equal(0, rangeCopy.Minimum);
            Assert.Equal(0, rangeCopy.Maximum);
        }

        [Fact]
        public void RangeIncorrectlySpecified_SetMin_IsValid()
        {
            var range = new Range<sbyte>(4, -4);
            Assert.False(range.IsValid);
            range.Minimum = -8;
            Assert.Equal(-8, range.Minimum);
            Assert.Equal(-4, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMinToZero_RemainsInvalid()
        {
            var range = new Range<byte>();
            range.Minimum = 0;
            Assert.Equal(0, range.Minimum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMaxToZero_RemainsInvalid()
        {
            var range = new Range<byte>();
            range.Maximum = 0;
            Assert.Equal(0, range.Maximum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMinToNonZero_RemainsInvalid()
        {
            var range = new Range<short>();
            range.Minimum = -2;
            Assert.Equal(-2, range.Minimum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMaxToNonZero_RemainsInvalid()
        {
            var range = new Range<ushort>();
            range.Maximum = 8;
            Assert.Equal(8, range.Maximum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMinAndMaxWithMaxToSameValie_BecomesValid()
        {
            var range = new Range<long>();
            range.Minimum = 2609;
            range.Maximum = 2609;
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMinAndMaxWithMaxGreaterThanMin_BecomesValid()
        {
            var range = new Range<byte>();
            range.Minimum = 2;
            range.Maximum = 3;
            Assert.Equal(2, range.Minimum);
            Assert.Equal(3, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Theory]
        [InlineData(2609, true)]
        [InlineData(1152, true)]
        [InlineData(5872, true)]
        [InlineData(1001, false)]
        [InlineData(9181, false)]
        public void RangeSpecified_ValueInRange_ReportsInRangeCorrectly(int valueToTest, bool isInRange)
        {
            var range = new Range<int>(1152, 5872);
            Assert.Equal(isInRange, range.IsValueInRange(valueToTest));
        }

        [Fact]
        public void RangeUnspecified_UpdateMinimum_MinimumUpdatedRemainsInvalid()
        {
            var range = new Range<byte>();
            range.UpdateMinimum(1);
            Assert.Equal(1, range.Minimum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_UpdateMaximum_MaximumUpdatedRemainsInvalid()
        {
            var range = new Range<byte>();
            range.UpdateMaximum(1);
            Assert.Equal(1, range.Maximum);
            Assert.False(range.IsValid);
        }

        [Fact]
        public void RangeSpecified_UpdateMinimumToBeLess_MinimumUpdated()
        {
            var range = new Range<byte>(4, 8);
            range.UpdateMinimum(1);
            Assert.Equal(1, range.Minimum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeSpecified_UpdateMaximumToBeGreater_MaximumUpdated()
        {
            var range = new Range<byte>(4, 8);
            range.UpdateMaximum(10);
            Assert.Equal(10, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeSpecified_UpdateMinimumToBeLarger_MinimumNotUpdated()
        {
            var range = new Range<byte>(4, 8);
            range.UpdateMinimum(6);
            Assert.Equal(4, range.Minimum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeSpecified_UpdateMaximumToBeSmaller_MaximumNotUpdated()
        {
            var range = new Range<byte>(4, 8);
            range.UpdateMaximum(6);
            Assert.Equal(8, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_AddValue_BecomesValidWithMatchingMinAndMax()
        {
            var range = new Range<int>();
            range.Add(4);
            Assert.Equal(4, range.Minimum);
            Assert.Equal(4, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMinAddValue_BecomesValidWithMatchingMinAndMax()
        {
            var range = new Range<int>();
            range.Minimum = 5;
            range.Add(4);
            Assert.Equal(4, range.Minimum);
            Assert.Equal(4, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Fact]
        public void RangeUnspecified_SetMaxAddValue_BecomesValidWithMatchingMinAndMax()
        {
            var range = new Range<int>();
            range.Maximum = 2;
            range.Add(4);
            Assert.Equal(4, range.Minimum);
            Assert.Equal(4, range.Maximum);
            Assert.True(range.IsValid);
        }

        [Theory]
        [InlineData(0, -1, 1)]
        [InlineData(-1, -1, 1)]
        [InlineData(-4, -4, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(5, -1, 5)]
        public void RangeSpecified_AddValue_ExpandsAsExpectedRemainsValid(int valueToAdd, int expectedMin, int expectedMax)
        {
            var range = new Range<int>(-1, 1);
            range.Add(valueToAdd);
            Assert.Equal(expectedMin, range.Minimum);
            Assert.Equal(expectedMax, range.Maximum);
            Assert.True(range.IsValid);
        }

        public static IEnumerable<object[]> MergeRangesTestData
        {
            get
            {
                yield return new object[] { new Range<int>(), new Range<int>(), 0, 0, false };
                yield return new object[] { new Range<int>(), new Range<int>(2, 1), 0, 0, false };
                yield return new object[] { new Range<int>(), new Range<int>(1, 2), 1, 2, true };
                yield return new object[] { new Range<int>(2, 1), new Range<int>(), 0, 0, false };
                yield return new object[] { new Range<int>(1, 2), new Range<int>(), 1, 2, true };
                yield return new object[] { new Range<int>(1, 2), new Range<int>(6, 3), 1, 2, true };
                yield return new object[] { new Range<int>(-4, 4), new Range<int>(-1, 1), -4, 4, true };
                yield return new object[] { new Range<int>(-4, 4), new Range<int>(-10, 1), -10, 4, true };
                yield return new object[] { new Range<int>(-4, 4), new Range<int>(-1, 10), -4, 10, true };
                yield return new object[] { new Range<int>(-4, 4), new Range<int>(-10, 6), -10, 6, true };
            }
        }

        [Theory]
        [MemberData("MergeRangesTestData")]
        public void Range_MergeWithRange_ResultIsExpected(Range<int> baseRange, Range<int> otherRange, int expectedMin, int expectedMax, bool expectedRangeIsValid)
        {
            var mergedRange = baseRange.Merge(otherRange);
            Assert.Equal(expectedMin, mergedRange.Minimum);
            Assert.Equal(expectedMax, mergedRange.Maximum);
            Assert.Equal(expectedRangeIsValid, mergedRange.IsValid);
        }
    }
}

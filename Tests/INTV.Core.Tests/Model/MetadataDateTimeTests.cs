// <copyright file="MetadataDateTimeTests.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.Linq;
using INTV.Core.Model;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class MetadataDateTimeTests
    {
        private const MetadataDateTimeFlags Date = MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Month | MetadataDateTimeFlags.Day;
        private const MetadataDateTimeFlags UtcDate = Date | MetadataDateTimeFlags.UtcOffset;
        private const MetadataDateTimeFlags Time = MetadataDateTimeFlags.Hour | MetadataDateTimeFlags.Minute | MetadataDateTimeFlags.Second;
        private const MetadataDateTimeFlags UtcTime = Time | MetadataDateTimeFlags.UtcOffset;
        private const MetadataDateTimeFlags DateTime = Date | Time;
        private const MetadataDateTimeFlags UtcDateTime = DateTime | MetadataDateTimeFlags.UtcOffset;

        [Fact]
        public void MetadataDateTime_CompareToNull_ThrowsArgumentException()
        {
            var dateTime = new MetadataDateTime(DateTimeOffset.UtcNow, UtcDateTime);

            Assert.Throws<ArgumentException>(() => dateTime.CompareTo(null));
        }

        [Fact]
        public void MetadataDateTime_EqualsNull_ThrowsArgumentException()
        {
            var dateTime = new MetadataDateTime(DateTimeOffset.UtcNow, UtcDateTime);

            Assert.Throws<ArgumentException>(() => dateTime.Equals(null));
        }

        [Fact]
        public void MetadataDateTime_CompareToIncompatibleType_ThrowsArgumentException()
        {
            var dateTime = new MetadataDateTime(DateTimeOffset.UtcNow, UtcDateTime);

            Assert.Throws<ArgumentException>(() => dateTime.CompareTo(DateTimeOffset.UtcNow));
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherWhenFullyEqual_ComparesEqual()
        {
            var dateTime0 = new MetadataDateTime(DateTimeOffset.UtcNow, UtcDateTime);
            var dateTime1 = new MetadataDateTime(dateTime0.Date, UtcDateTime);

            Assert.Equal(0, dateTime0.CompareTo(dateTime1));
            Assert.Equal(0, dateTime0.CompareTo((object)dateTime1));
            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: false));
            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: false));
            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: true));
            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: true, compareOnlyCommonValidFields: false));
            Assert.True(dateTime1.Equals(dateTime0));
            Assert.True(dateTime1.Equals((object)dateTime0));
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherStrictWithDifferentFlags_IsNonzero()
        {
            var dateTime0 = new MetadataDateTime(DateTimeOffset.UtcNow, UtcDateTime);
            var dateTime1 = new MetadataDateTime(dateTime0.Date, DateTime);

            Assert.True(dateTime0.CompareTo(dateTime1, strict: true) > 0);
            Assert.True(dateTime1.CompareTo(dateTime0, strict: true) < 0);
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherNonStrictWithDifferentFlags_CommonFieldsCompareAsEqual()
        {
            var dateTime0 = new MetadataDateTime(DateTimeOffset.Now, DateTime | MetadataDateTimeFlags.LeapSecond);
            var rawDateTime = dateTime0.Date.DateTime;
            var dateTime1 = new MetadataDateTime(new DateTimeOffset(new DateTime(rawDateTime.Year, rawDateTime.Month, rawDateTime.Day)), Date);

            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: true));
            Assert.True(dateTime1.Equals(dateTime0));
            Assert.False(dateTime0.GetHashCode() == dateTime1.GetHashCode());
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherNonStrictWithDifferentFlagsAndNoDifferenceInCommonField_CommonFieldsCompareAsEqual()
        {
            var dateTime0 = new MetadataDateTime(DateTimeOffset.Now, Date | MetadataDateTimeFlags.LeapSecond);
            var dateTime1 = new MetadataDateTime(dateTime0.Date, DateTime | MetadataDateTimeFlags.LeapSecond);

            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: true));
            Assert.True(dateTime1.Equals(dateTime0));
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherNonStrictWithDifferentFlagsAndDifferenceInCommonField_CommonFieldsCompareAsNotEqual()
        {
            var dateTime0 = new MetadataDateTime(DateTimeOffset.Now, Date | MetadataDateTimeFlags.LeapSecond);
            var dateTime1 = new MetadataDateTime(dateTime0.Date.AddDays(1), DateTime | MetadataDateTimeFlags.LeapSecond);

            Assert.True(dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: true) != 0);
            Assert.False(dateTime1.Equals(dateTime0));
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherNonStrictWithDifferentFlagsAndDifferenceInNotCommonField_CommonFieldsCompareAsEqual()
        {
            // NOTE: It is possible for this test to fail if, by adding one second, it causes the date to roll over to a new year.
            // So don't run this test on New Year's Eve at 23:59:59+.
            var dateTime0 = new MetadataDateTime(DateTimeOffset.Now, MetadataDateTimeFlags.Year);
            var dateTime1 = new MetadataDateTime(dateTime0.Date.AddSeconds(1), DateTime);

            Assert.Equal(0, dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: true));
        }

        [Fact]
        public void MetadataDateTime_CompareToOtherNonStrictWithNoCommonFlags_ComparesUnderlyingDateTimeOnly()
        {
            var dateTime0 = new MetadataDateTime(DateTimeOffset.Now, Date);
            var dateTime1 = new MetadataDateTime(dateTime0.Date.AddSeconds(1.2), UtcTime);
            Assert.True((dateTime0.Flags & dateTime1.Flags) == MetadataDateTimeFlags.None);

            var dateTimeOffsetCompareToResult = dateTime0.Date.CompareTo(dateTime1.Date);
            var metadataDateTimeCompareToResult = dateTime0.CompareTo(dateTime1, strict: false, compareOnlyCommonValidFields: true);

            Assert.Equal(dateTimeOffsetCompareToResult, metadataDateTimeCompareToResult);
        }

        [Fact]
        public void MetadataDateTime_ToString_ProducesCorrectResult()
        {
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var metadataDateTime = new MetadataDateTime(dateTimeOffsetNow, Date);

            var expectedToStringResult = dateTimeOffsetNow.ToString() + " {" + metadataDateTime.Flags.ToString() + "}";
            var actualToStringResult = metadataDateTime.ToString();

            Assert.Equal(expectedToStringResult, actualToStringResult);
        }

        [Fact]
        public void MetadataDateTime_ToStringWithFormat_ProducesCorrectResult()
        {
            var dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var metadataDateTime = new MetadataDateTime(dateTimeOffsetNow, Date);

            var expectedToStringResult = dateTimeOffsetNow.ToString(dateTimeFormat) + " {" + metadataDateTime.Flags.ToString() + "}";
            var actualToStringResult = metadataDateTime.ToString(dateTimeFormat);

            Assert.Equal(expectedToStringResult, actualToStringResult);
        }

        [Fact]
        public void MetadataDateTime_ToStringWithFormatPovider_ProducesCorrectResult()
        {
            var formatProvider = CultureInfo.GetCultureInfo("de-DE");
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var metadataDateTime = new MetadataDateTime(dateTimeOffsetNow, Date);

            var expectedToStringResult = dateTimeOffsetNow.ToString(formatProvider) + " {" + metadataDateTime.Flags.ToString() + "}";
            var actualToStringResult = metadataDateTime.ToString(formatProvider);

            Assert.Equal(expectedToStringResult, actualToStringResult);
        }

        [Fact]
        public void MetadataDateTime_ToStringWithFormatAndFormatPovider_ProducesCorrectResult()
        {
            var dateTimeFormat = "dddd, MMM dd yyyy HH:mm:ss zzz";
            var formatProvider = CultureInfo.GetCultureInfo("de-DE");
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var metadataDateTime = new MetadataDateTime(dateTimeOffsetNow, Date);

            var expectedToStringResult = dateTimeOffsetNow.ToString(dateTimeFormat, formatProvider) + " {" + metadataDateTime.Flags.ToString() + "}";
            var actualToStringResult = metadataDateTime.ToString(dateTimeFormat, formatProvider);

            Assert.Equal(expectedToStringResult, actualToStringResult);
        }
    }
}

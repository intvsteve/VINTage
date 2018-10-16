// <copyright file="MetadataDateTimeBuilderTests.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class MetadataDateTimeBuilderTests
    {
        [Theory]
        [InlineData(-123)]
        [InlineData(1000000)]
        public void MetadataDateTimeBuilder_WithYearOutOfRange_ThrowsArgumentOutOfRangeException(int year)
        {
            var builder = new MetadataDateTimeBuilder(year);

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Build());
        }

        [Fact]
        public void MetadataDateTimeBuilder_WithAllValidValues_CreatesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 10;
            var day = 15;
            var hour = 17;
            var minute = 34;
            var second = 48;
            var offsetHours = 1;
            var offsetMinutes = 23;
            var flags = Enum.GetValues(typeof(MetadataDateTimeFlags)).Cast<MetadataDateTimeFlags>().Aggregate((all, flag) => all | flag) & ~MetadataDateTimeFlags.LeapSecond;
            var offset = new TimeSpan(offsetHours, offsetMinutes, 0);
            var date = new System.DateTimeOffset(year, month, day, hour, minute, second, offset);
            var expectedMetadataDateTime = new MetadataDateTime(date, flags);

            var builder = new MetadataDateTimeBuilder(year);
            builder.WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).WithOffset(offsetHours, offsetMinutes);
            var actualMetadataDateTime = builder.Build();

            Assert.Equal(0, expectedMetadataDateTime.CompareTo(actualMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataDateTimeBuilder_WithLeapSecond_CreatesExpectedMetadataDateTime()
        {
            var year = 1999;
            var second = 60;
            var flags = MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Second | MetadataDateTimeFlags.LeapSecond;
            var date = new System.DateTimeOffset(year, 1, 1, 0, 0, 59, TimeSpan.Zero);
            var expectedMetadataDateTime = new MetadataDateTime(date, flags);

            var builder = new MetadataDateTimeBuilder(year).WithSecond(second);
            var actualMetadataDateTime = builder.Build();

            Assert.Equal(0, expectedMetadataDateTime.CompareTo(actualMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataDateTimeBuilder_WithInvalidValues_CreatesExpectedMetadataDateTime()
        {
            var year = 1979;
            var month = 69;
            var day = 32;
            var hour = 44;
            var minute = 88;
            var second = -2;
            var flags = MetadataDateTimeFlags.Year;
            var dateTime = new DateTime(year, 1, 1);
            var date = new System.DateTimeOffset(dateTime, TimeSpan.Zero);
            var expectedMetadataDateTime = new MetadataDateTime(date, flags);

            var builder = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second);
            var actualMetadataDateTime = builder.Build();

            Assert.Equal(0, expectedMetadataDateTime.CompareTo(actualMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataDateTimeBuilder_WithUtcOffsetHoursOutOfRange_CreatesExpectedMetadataDateTime()
        {
            var year = 1991;
            var offsetHours = 123;
            var offsetMinutes = 22;
            var flags = MetadataDateTimeFlags.Year;
            var dateTime = new DateTime(year, 1, 1);
            var date = new System.DateTimeOffset(dateTime, TimeSpan.Zero);
            var expectedMetadataDateTime = new MetadataDateTime(date, flags);

            var builder = new MetadataDateTimeBuilder(year).WithOffset(offsetHours, offsetMinutes);
            var actualMetadataDateTime = builder.Build();

            Assert.Equal(0, expectedMetadataDateTime.CompareTo(actualMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataDateTimeBuilder_WithUtcOffsetMinutesOutOfRange_CreatesExpectedMetadataDateTime()
        {
            var year = 1984;
            var offsetHours = 3;
            var offsetMinutes = -9864;
            var flags = MetadataDateTimeFlags.Year;
            var dateTime = new DateTime(year, 1, 1);
            var date = new System.DateTimeOffset(dateTime, TimeSpan.Zero);
            var expectedMetadataDateTime = new MetadataDateTime(date, flags);

            var builder = new MetadataDateTimeBuilder(year).WithOffset(offsetHours, offsetMinutes);
            var actualMetadataDateTime = builder.Build();

            Assert.Equal(0, expectedMetadataDateTime.CompareTo(actualMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }
    }
}

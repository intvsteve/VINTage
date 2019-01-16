// <copyright file="MetadataHelpersTests.cs" company="INTV Funhouse">
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

using INTV.Core.Model;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class MetadataHelpersTests
    {
        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingCompleteDateTimeWithOffset_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 10;
            var day = 9;
            var hour = 8;
            var minute = 7;
            var second = 6;
            var offsetHours = 5;
            var offsetMinutes = 4;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second, offsetHours, offsetMinutes)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).WithOffset(offsetHours, offsetMinutes).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingCompleteDateTimeWithOffsetAndAdditionalPayload_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 10;
            var day = 9;
            var hour = 8;
            var minute = 7;
            var second = 6;
            var offsetHours = 5;
            var offsetMinutes = 4;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second, offsetHours, offsetMinutes, 12)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).WithOffset(offsetHours, offsetMinutes).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeMonth_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 16;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeDay_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 10;
            var day = 72;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeHour_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 9;
            var day = 8;
            var hour = 63;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeMinute_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 8;
            var day = 7;
            var hour = 6;
            var minute = 99;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeSecond_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 7;
            var day = 6;
            var hour = 5;
            var minute = 4;
            var second = 120;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithLeapSecond_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 7;
            var day = 6;
            var hour = 5;
            var minute = 4;
            var second = 60;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeOffsetHours_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 6;
            var day = 5;
            var hour = 4;
            var minute = 3;
            var second = 2;
            var offsetHours = 16;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second, offsetHours)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            // Note that we don't bother with sending the offset data to the builder here. This is because of subtle differences with how the builder
            // processes offset vs. how the parser does. In this test, the parser stops after the offset hours check. The Builder takes offset as hours
            // and minutes, and would ignore the hours, but honor the minutes. So just skip defining offset at all with the builder.
            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeOffsetMinutes_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 5;
            var day = 4;
            var hour = 3;
            var minute = 2;
            var second = 1;
            var offsetHours = 0;
            var offsetMinutes = 88;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second, offsetHours, offsetMinutes)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            // NOTE: The builder allows specifying offset entirely in minutes, whereas the above parser operates on bytes.
            // As a result, the above produces a timestamp with UtcOffset flag specified, and it just ignores the minutes setting.
            // This is correct behavior, but subtly different than the results from using the Builder. Therefore, alter the minutes offset
            // passed to the builder to create the expected result.
            offsetMinutes = 0;
            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).WithOffset(offsetHours, offsetMinutes).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeMonthAndDay_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 16;
            var day = 72;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeDayAndHour_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 10;
            var day = 72;
            var hour = 63;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeHourAndMinute_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 9;
            var day = 8;
            var hour = 63;
            var minute = 99;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeMinuteAndSecond_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 8;
            var day = 7;
            var hour = 6;
            var minute = 99;
            var second = 120;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeSecondAndOffsetHours_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 7;
            var day = 6;
            var hour = 5;
            var minute = 4;
            var second = 98;
            var offsetHours = 16;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second, offsetHours)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).WithOffset(offsetHours, 0).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        [Fact]
        public void MetadataHelpers_DeserializeBufferContainingDateTimeWithOutOfRangeOffsetHoursAndOffsetMinutes_ProducesExpectedMetadataDateTime()
        {
            var year = 2018;
            var month = 6;
            var day = 5;
            var hour = 4;
            var minute = 3;
            var second = 2;
            var offsetHours = 16;
            var offsetMinutes = 123;

            uint payloadLength;
            var parsedMetadataDateTime = MetadataDateTime.MinValue;
            using (var reader = new BinaryReader(InitializeMetadataDateTimeBuffer(out payloadLength, year, month, day, hour, minute, second, offsetHours, offsetMinutes)))
            {
                parsedMetadataDateTime = reader.ParseDateTimeFromMetadata(payloadLength);
            }

            var expectedMetadataDateTime = new MetadataDateTimeBuilder(year).WithMonth(month).WithDay(day).WithHour(hour).WithMinute(minute).WithSecond(second).WithOffset(offsetHours, offsetMinutes).Build();
            Assert.Equal(0, expectedMetadataDateTime.CompareTo(parsedMetadataDateTime, strict: true, compareOnlyCommonValidFields: false));
        }

        private System.IO.Stream InitializeMetadataDateTimeBuffer(out uint payloadLength, int year = -1, int month = -1, int day = -1, int hour = -1, int minute = -1, int second = -1, int? offsetHours = null, int offsetMinutes = -1, int additionalPayload = -1)
        {
            var stream = new System.IO.MemoryStream();
            if (year >= 1900)
            {
                stream.WriteByte((byte)(year - 1900));
            }

            if (month >= 0)
            {
                stream.WriteByte((byte)month);
            }

            if (day >= 0)
            {
                stream.WriteByte((byte)day);
            }

            if (hour >= 0)
            {
                stream.WriteByte((byte)hour);
            }

            if (minute >= 0)
            {
                stream.WriteByte((byte)minute);
            }

            if (second >= 0)
            {
                stream.WriteByte((byte)second);
            }

            if (offsetHours.HasValue)
            {
                stream.WriteByte((byte)offsetHours.Value);
            }

            if (offsetMinutes >= 0)
            {
                stream.WriteByte((byte)offsetMinutes);
            }

            if (additionalPayload > 0)
            {
                var buffer = new byte[additionalPayload];
                stream.Write(buffer, 0, additionalPayload);
            }

            payloadLength = (uint)stream.Length;
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return stream;
        }
    }
}

// <copyright file="MetadataHelpersTests.cs" company="INTV Funhouse">
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
using System.Text;
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

        [Fact]
        public void MetadataHelpers_GetEnclosingQuoteCharactersFromNullPayload_ThrowsNullReferenceException()
        {
            byte[] nullData = null;

            Assert.Throws<NullReferenceException>(() => nullData.GetEnclosingQuoteCharacterIndexesFromBytePayload());
        }

        [Theory]
        [InlineData("", -1, -1)]
        [InlineData("\"", 0, 0)]
        [InlineData("\"\"", 0, 1)]
        [InlineData("\"\"\"", 0, 2)]
        [InlineData(" \t\" a\"b ", 2, -1)]
        [InlineData("\" a\"b \"", 0, 6)]
        [InlineData("Kröte", -1, -1)]
        [InlineData(" \" Kröte \" ", 1, 10)]
        [InlineData("Kr\"öte", -1, -1)]
        [InlineData("\"\"Kröte", 0, -1)]
        [InlineData("\n\"\"Kröte\t\n \" ", -1, -1)]
        [InlineData("\"子豚\"", 0, 7)]
        [InlineData("子\"豚", -1, -1)]
        [InlineData("子豚", -1, -1)]
        [InlineData("\"ハウディドゥーディー\"", 0, 31)]
        [InlineData("ハウディドゥー\"ディー", -1, -1)]
        [InlineData("ハウディドゥー\"ディー\" ", -1, -1)]
        [InlineData("ハウディドゥーディー", -1, -1)]
        public void MetadataHelpers_GetEnclosingQuoteCharacterIndexesFromBytePayload_ReturnsExpectedIndexes(string stringToTest, int expectedFirstQuoteIndex, int expectedLastQuoteIndex)
        {
            // We're going to use raw bytes, so convert using UTF-8.
            var payloadBytes = Encoding.UTF8.GetBytes(stringToTest);

            var quoteIndexes = payloadBytes.GetEnclosingQuoteCharacterIndexesFromBytePayload();

            Assert.Equal(expectedFirstQuoteIndex, quoteIndexes.Minimum);
            Assert.Equal(expectedLastQuoteIndex, quoteIndexes.Maximum);
        }

        [Fact]
        public void MetadataHelpers_EscapeToBytePayloadWithNullString_ThrowsArgumentNullException()
        {
            string nullStringToEscape = null;

            Assert.Throws<ArgumentNullException>(() => nullStringToEscape.EscapeToBytePayload());
        }

        public static IEnumerable<object> EscapeToBytePayloadTestData
        {
            get
            {
                yield return new object[]
                {
                    string.Empty,
                    new byte[] { },
                    string.Empty
                };
                yield return new object[]
                {
                    "\"",
                    new byte[] { 0x5C, 0x22 },
                    "\\\""
                };
                yield return new object[]
                {
                    "\"\"",
                    new byte[] { 0x5C, 0x22, 0x5C, 0x22 },
                    "\\\"\\\""
                };
                yield return new object[]
                {
                    "\"\"\"",
                    new byte[] { 0x5C, 0x22, 0x5C, 0x22, 0x5C, 0x22 },
                    "\\\"\\\"\\\""
                };
                yield return new object[]
                {
                    " \t\" a\"b ",
                    new byte[] { 0x20, 0x5C, 0x74, 0x5C, 0x22, 0x20, 0x61, 0x5C, 0x22, 0x62, 0x20 },
                    " \\t\\\" a\\\"b "
                };
                yield return new object[]
                {
                    "\" a\"b \"",
                    new byte[] { 0x5C, 0x22, 0x20, 0x61, 0x5C, 0x22, 0x62, 0x20, 0x5C, 0x22 },
                    "\\\" a\\\"b \\\""
                };
                yield return new object[]
                {
                    "\" a\"b \r\b\"",
                    new byte[] { 0x5C, 0x22, 0x20, 0x61, 0x5C, 0x22, 0x62, 0x20, 0x5C, 0x72, 0x5C, 0x78, 0x30, 0x38, 0x5C, 0x22 },
                    "\\\" a\\\"b \\r\\x08\\\""
                };
                yield return new object[]
                {
                    "Kröte",
                    new byte[] { 0X4B, 0x72, 0x5C, 0x78, 0x43, 0x33, 0x5C, 0x78, 0x42, 0x36, 0x74, 0x65 },
                    "Kr\\xC3\\xB6te"
                };
                yield return new object[]
                {
                    " \" Kröte \" ",
                    new byte[] { 0x20, 0x5C, 0x22, 0x20, 0X4B, 0x72, 0x5C, 0x78, 0x43, 0x33, 0x5C, 0x78, 0x42, 0x36, 0x74, 0x65, 0x20, 0x5C, 0x22, 0x20 },
                    " \\\" Kr\\xC3\\xB6te \\\" "
                };
                yield return new object[]
                {
                    "Kr\"öte",
                    new byte[] { 0X4B, 0x72, 0x5C, 0x22, 0x5C, 0x78, 0x43, 0x33, 0x5C, 0x78, 0x42, 0x36, 0x74, 0x65 },
                    "Kr\\\"\\xC3\\xB6te"
                };
                yield return new object[]
                {
                    "\"\"Kröte",
                    new byte[] { 0x5C, 0x22, 0x5C, 0x22, 0X4B, 0x72, 0x5C, 0x78, 0x43, 0x33, 0x5C, 0x78, 0x42, 0x36, 0x74, 0x65 },
                    "\\\"\\\"Kr\\xC3\\xB6te"
                };
                yield return new object[]
                {
                    "\n\"\"Kröte\t\n \" ",
                    new byte[] { 0x5C, 0x6E, 0x5C, 0x22, 0x5C, 0x22, 0X4B, 0x72, 0x5C, 0x78, 0x43, 0x33, 0x5C, 0x78, 0x42, 0x36, 0x74, 0x65, 0x5C, 0x74, 0x5C, 0x6E, 0x20, 0x5C, 0x22, 0x20 },
                    "\\n\\\"\\\"Kr\\xC3\\xB6te\\t\\n \\\" "
                };
                yield return new object[]
                {
                    "\"子豚\"",
                    new byte[] { 0x5C, 0x22, 0x5C, 0x78, 0x45, 0x35, 0x5C, 0x78, 0x41, 0x44, 0x5C, 0x78, 0x39, 0x30, 0x5C, 0x78, 0x45, 0x38, 0x5C, 0x78, 0x42, 0x31, 0x5C, 0x78, 0x39, 0x41, 0x5C, 0x22 },
                    "\\\"\\xE5\\xAD\\x90\\xE8\\xB1\\x9A\\\""
                };
                yield return new object[]
                {
                    "子\"豚",
                    new byte[] { 0x5C, 0x78, 0x45, 0x35, 0x5C, 0x78, 0x41, 0x44, 0x5C, 0x78, 0x39, 0x30, 0x5C, 0x22, 0x5C, 0x78, 0x45, 0x38, 0x5C, 0x78, 0x42, 0x31, 0x5C, 0x78, 0x39, 0x41 },
                    "\\xE5\\xAD\\x90\\\"\\xE8\\xB1\\x9A"
                };
                yield return new object[]
                {
                    "子豚",
                    new byte[] { 0x5C, 0x78, 0x45, 0x35, 0x5C, 0x78, 0x41, 0x44, 0x5C, 0x78, 0x39, 0x30, 0x5C, 0x78, 0x45, 0x38, 0x5C, 0x78, 0x42, 0x31, 0x5C, 0x78, 0x39, 0x41 },
                    "\\xE5\\xAD\\x90\\xE8\\xB1\\x9A"
                };
                yield return new object[]
                {
                    "\"ハウディドゥーディー\"",
                    new byte[] { 0x5C, 0x22, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x46, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x36, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x39, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x35, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43, 0x5C, 0x22 },
                    "\\\"\\xE3\\x83\\x8F\\xE3\\x82\\xA6\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\x89\\xE3\\x82\\xA5\\xE3\\x83\\xBC\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\xBC\\\""
                };
                yield return new object[]
                {
                    "ハウディドゥー\"ディー",
                    new byte[] { 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x46, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x36, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x39, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x35, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43, 0x5C, 0x22, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43 },
                    "\\xE3\\x83\\x8F\\xE3\\x82\\xA6\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\x89\\xE3\\x82\\xA5\\xE3\\x83\\xBC\\\"\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\xBC"
                };
                yield return new object[]
                {
                    "ハウディドゥー\"ディー\" ",
                    new byte[] { 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x46, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x36, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x39, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x35, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43, 0x5C, 0x22, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43, 0x5C, 0x22, 0x20 },
                    "\\xE3\\x83\\x8F\\xE3\\x82\\xA6\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\x89\\xE3\\x82\\xA5\\xE3\\x83\\xBC\\\"\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\xBC\\\" "
                };
                yield return new object[]
                {
                    "ハウディドゥーディー",
                    new byte[] { 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x46, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x36, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x39, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x35, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x38, 0x37, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x32, 0x5C, 0x78, 0x41, 0x33, 0x5C, 0x78, 0x45, 0x33, 0x5C, 0x78, 0x38, 0x33, 0x5C, 0x78, 0x42, 0x43 },
                    "\\xE3\\x83\\x8F\\xE3\\x82\\xA6\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\x89\\xE3\\x82\\xA5\\xE3\\x83\\xBC\\xE3\\x83\\x87\\xE3\\x82\\xA3\\xE3\\x83\\xBC"
                };
            }
        }

        [Theory]
        [MemberData("EscapeToBytePayloadTestData")]
        public void MetadataHelpers_EscapeToBytePayload_EscapesStringAsExpected(string stringToEscape, byte[] expectedPayloadData, string expectedPayloadAsString)
        {
            var payloadData = stringToEscape.EscapeToBytePayload();
            var payloadAsString = Encoding.UTF8.GetString(payloadData);

            Assert.Equal(expectedPayloadData, payloadData);
            Assert.Equal(expectedPayloadAsString, payloadAsString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadWithNullDataAndNullRange_ThrowsNullReferenceException()
        {
            byte[] nullPayload = null;

            Assert.Throws<NullReferenceException>(() => nullPayload.UnescapeFromBytePayload(null));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadWithNullDataAndOneCharacterRange_ReturnsEmptyString()
        {
            byte[] nullPayload = null;

            var unescapedString = nullPayload.UnescapeFromBytePayload(new Range<int>(0, 1));

            Assert.Equal(0, unescapedString.Length);
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadWithNullDataAndInvalidRange_ThrowsNullReferenceException()
        {
            byte[] nullPayload = null;

            Assert.Throws<NullReferenceException>(() => nullPayload.UnescapeFromBytePayload(new Range<int>(10, 2)));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadWithNullDataAndSpecifiedRange_ThrowsNullReferenceException()
        {
            byte[] nullPayload = null;

            Assert.Throws<NullReferenceException>(() => nullPayload.UnescapeFromBytePayload(new Range<int>(0, 2)));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadWithNullDataAndEmptyRange_ThrowsNullReferenceException()
        {
            byte[] nullPayload = null;

            Assert.Throws<NullReferenceException>(() => nullPayload.UnescapeFromBytePayload(new Range<int>(1, 1)));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadRangeFirstIndexTooLow_ThrowsIndexOutOfRangeException()
        {
            var payload = new byte[] { 0x31, 0x32 };

            Assert.Throws<IndexOutOfRangeException>(() => payload.UnescapeFromBytePayload(new Range<int>(-2, 1)));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadRangeLastIndexTooHigh_ThrowsIndexOutOfRangeException()
        {
            var payload = new byte[] { 0x31, 0x32 };

            Assert.Throws<IndexOutOfRangeException>(() => payload.UnescapeFromBytePayload(new Range<int>(0, 4)));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadRangeIndexesOutOfRangeLow_ThrowsIndexOutOfRangeException()
        {
            var payload = new byte[] { 0x31, 0x32 };

            Assert.Throws<IndexOutOfRangeException>(() => payload.UnescapeFromBytePayload(new Range<int>(-10, -2)));
        }

        [Fact]
        public void MetadataHelpers_UnescapeFromBytePayloadRangeIndexesOutOfRangeHigh_ThrowsIndexOutOfRangeException()
        {
            var payload = new byte[] { 0x31, 0x32 };

            Assert.Throws<IndexOutOfRangeException>(() => payload.UnescapeFromBytePayload(new Range<int>(10, 20)));
        }

        public static IEnumerable<object> EscapeUnescapeRoundTripTestData
        {
            get
            {
                foreach (var entry in EscapeToBytePayloadTestData)
                {
                    var entryDataArray = entry as object[];
                    yield return new object[] { entryDataArray[0] };
                }
            }
        }

        [Theory]
        [MemberData("EscapeUnescapeRoundTripTestData")]
        public void MetadataHelpers_EscapeToBytePayloadUnescapeBytePayloadRoundTrip_ProducesOriginalString(string stringToRoundTrip)
        {
            var escapedPayload = stringToRoundTrip.EscapeToBytePayload();

            var unescapedString = escapedPayload.UnescapeFromBytePayload(null);

            Assert.Equal(stringToRoundTrip, unescapedString);
        }

        public static IEnumerable<object> OctalEncodedTestData
        {
            get
            {
                yield return new object[]
                {
                    new byte[] { },
                    string.Empty
                };
                yield return new object[]
                {
                    new byte[] { 0x5C, 0x31, 0x32, 0x37, 0x5C, 0x31, 0x35, 0x30, 0x5C, 0x31, 0x34, 0x31, 0x5C, 0x31, 0x36, 0x34, 0x5C, 0x30, 0x34, 0x37, 0x5C, 0x31, 0x36, 0x33, 0x5C, 0x30, 0x34, 0x30, 0x5C, 0x31, 0x32, 0x34, 0x5C, 0x31, 0x35, 0x30, 0x5C, 0x31, 0x35, 0x31, 0x5C, 0x31, 0x36, 0x33, 0x5C, 0x30, 0x37, 0x37 },
                    "What's This?"
                };
                yield return new object[]
                {
                    new byte[] { 0x20, 0x5C, 0x30, 0x31, 0x32, 0x77, 0x5C, 0x31, 0x35, 0x30, 0x61, 0x5C, 0x31, 0x36, 0x34, 0x5C, 0x30, 0x31, 0x35, 0x5C, 0x30, 0x31, 0x32, 0x5C, 0x31, 0x35, 0x31, 0x5C, 0x31, 0x36, 0x33, 0x5C, 0x30, 0x31, 0x32, 0x5C, 0x31, 0x32, 0x34, 0x5C, 0x31, 0x35, 0x30, 0x5C, 0x31, 0x30, 0x31, 0x5C, 0x31, 0x36, 0x34, 0x5C, 0x30, 0x37, 0x37, 0x20, 0x5C, 0x30, 0x31, 0x32, 0x5C, 0x30, 0x31, 0x31 },
                    " \nwhat\r\nis\nThAt? \n\t"
                };
                yield return new object[]
                {
                    new byte[] { 0x5C, 0x30, 0x34, 0x32, 0x5C, 0x33, 0x34, 0x35, 0x5C, 0x32, 0x35, 0x35, 0x5C, 0x32, 0x32, 0x30, 0x5C, 0x33, 0x35, 0x30, 0x5C, 0x32, 0x36, 0x31, 0x5C, 0x32, 0x33, 0x32, 0x5C, 0x30, 0x34, 0x32 },
                    "\"子豚\""
                };
                yield return new object[]
                {
                    new byte[] { 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x33, 0x5C, 0x32, 0x31, 0x37, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x32, 0x5C, 0x32, 0x34, 0x36, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x33, 0x5C, 0x32, 0x30, 0x37, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x32, 0x5C, 0x32, 0x34, 0x33, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x33, 0x5C, 0x32, 0x31, 0x31, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x32, 0x5C, 0x32, 0x34, 0x35, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x33, 0x5C, 0x32, 0x37, 0x34, 0x5C, 0x30, 0x34, 0x32, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x33, 0x5C, 0x32, 0x30, 0x37, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x32, 0x5C, 0x32, 0x34, 0x33, 0x5C, 0x33, 0x34, 0x33, 0x5C, 0x32, 0x30, 0x33, 0x5C, 0x32, 0x37, 0x34, 0x5C, 0x30, 0x34, 0x32,  0x5C, 0x30, 0x34, 0x30 },
                    "ハウディドゥー\"ディー\" "
                };
            }
        }

        [Theory]
        [MemberData("OctalEncodedTestData")]
        public void MetadataHelpers_UnescapeOctalEncodedStrings_ProducesExpectedString(byte[] octalEncodedString, string expectedString)
        {
            var unescapedString = octalEncodedString.UnescapeFromBytePayload(null);

            Assert.Equal(expectedString, unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringHexNull_ProducesValidString()
        {
            var escaped = new byte[] { 0x5C, 0x78, 0x30, 0x30 };

            var unescapedString = escaped.UnescapeFromBytePayload(null);

            Assert.Equal(string.Empty, unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringHexTruncatedTwoCharacters_ProducesValidString()
        {
            var escaped = new byte[] { 0x5C, 0x78 };

            var unescapedString = escaped.UnescapeFromBytePayload(null);

            Assert.Equal("x", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringHexTruncatedOneCharacter_ProducesValidString()
        {
            var escaped = new byte[] { 0x5C, 0x78, 0x31 };

            var unescapedString = escaped.UnescapeFromBytePayload(null);

            Assert.Equal("x", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringLowerCaseHexEscape_ProducesValidString()
        {
            var escaped = new byte[] { 0X4B, 0x72, 0x5C, 0x78, 0x63, 0x33, 0x5C, 0x78, 0x62, 0x36, 0x74, 0x65 };

            var unescapedString = escaped.UnescapeFromBytePayload(null);

            Assert.Equal("Kröte", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithInvalidEscape_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x79, 0x75, 0x70 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("yup", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithInvalidHexHighNybble_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x78, 0x72, 0x38 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("x", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithInvalidHexLowNybble_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x78, 0x38, 0x72 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("x", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringOctalNull_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x30, 0x30, 0x30 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal(string.Empty, unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringOctalTruncatedOneCharacter_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x31, 0x32 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("1", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringOctalTruncatedTwoCharacters_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x31 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("1", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithInvalidOctalHighTrybble_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x38, 0x31, 0x32 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("812", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithInvalidOctalMidNybble_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x30, 0x38, 0x31, };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("0", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithInvalidOctalLowNybble_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x30, 0x31, 0x38 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal("0", unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeStringWithEscapedNull_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c, 0x00 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal(string.Empty, unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeNullString_ProducesValidString()
        {
            var badEscape = new byte[] { 0x00 };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal(string.Empty, unescapedString);
        }

        [Fact]
        public void MetadataHelpers_UnescapeSolitaryBackslash_ProducesValidString()
        {
            var badEscape = new byte[] { 0x5c };

            var unescapedString = badEscape.UnescapeFromBytePayload(null);

            Assert.Equal(string.Empty, unescapedString);
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

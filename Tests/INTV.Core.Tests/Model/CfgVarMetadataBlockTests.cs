// <copyright file="CfgVarMetadataBlockTests.cs" company="INTV Funhouse">
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
using System.Text;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using Xunit;
using Xunit.Abstractions;

namespace INTV.Core.Tests.Model
{
    public class CfgVarMetadataBlockTests
    {
        private const MetadataDateTimeFlags Date = MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Month | MetadataDateTimeFlags.Day;
        private const MetadataDateTimeFlags UtcDate = Date | MetadataDateTimeFlags.UtcOffset;
        private const MetadataDateTimeFlags Time = MetadataDateTimeFlags.Hour | MetadataDateTimeFlags.Minute | MetadataDateTimeFlags.Second;
        private const MetadataDateTimeFlags UtcTime = Time | MetadataDateTimeFlags.UtcOffset;
        private const MetadataDateTimeFlags DateTime = Date | Time;
        private const MetadataDateTimeFlags UtcDateTime = DateTime | MetadataDateTimeFlags.UtcOffset;

        public CfgVarMetadataBlockTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private ITestOutputHelper Output { get; set; }

        [Fact]
        public void CfgVarMetadata_InflateAllNullStream_ThrowsNullReferenceExcetpion()
        {
            Assert.Throws<NullReferenceException>(() => CfgVarMetadataBlock.InflateAll(null));
        }

        [Fact]
        public void CfgVarMetadata_InflateNullStream_ThrowsArgumentNullExcetpion()
        {
            Assert.Throws<ArgumentNullException>(() => CfgVarMetadataBlock.Inflate((System.IO.Stream)null));
        }

        [Fact]
        public void CfgVarMetadata_InflateNullBinaryReader_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => CfgVarMetadataBlock.Inflate((BinaryReader)null));
        }

        [Fact]
        public void CfgVarMetadata_InflateAllAsciiStreamWithNoValidContent_ReturnsEmptyEnumerable()
        {
            var meaninglessCfg =
@"[Howdy] 
pard = ner";

            using (var stream = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(meaninglessCfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.InflateAll(stream);

                Assert.Empty(cfgVarMetadata);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateAllUtf8StreamWithNoValidContent_ReturnsEmptyEnumerable()
        {
            var meaninglessCfg =
@"[Doody]
dooh";

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(meaninglessCfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.InflateAll(stream);

                Assert.Empty(cfgVarMetadata);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateCfgVarMetadataString()
        {
            var expectedValue = "Whee!";
            var cfg = "name = " + expectedValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.Inflate(stream);

                Assert.IsType<CfgVarMetadataString>(cfgVarMetadata);
                Assert.True(cfgVarMetadata.DeserializeByteCount > 0);
                Assert.Equal(expectedValue, ((CfgVarMetadataString)cfgVarMetadata).StringValue);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateCfgVarMetadataInteger()
        {
            var expectedValue = 128;
            var cfg = "jlp_flash = " + expectedValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.Inflate(stream);

                Assert.IsType<CfgVarMetadataInteger>(cfgVarMetadata);
                Assert.True(cfgVarMetadata.DeserializeByteCount > 0);
                Assert.Equal(expectedValue, ((CfgVarMetadataInteger)cfgVarMetadata).IntegerValue);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateCfgVarMetadataBoolean()
        {
            var expectedValue = true;
            var cfg = "lto_mapper = 1";

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.Inflate(stream);

                Assert.IsType<CfgVarMetadataBoolean>(cfgVarMetadata);
                Assert.True(cfgVarMetadata.DeserializeByteCount > 0);
                Assert.Equal(expectedValue, ((CfgVarMetadataBoolean)cfgVarMetadata).BooleanValue);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateCfgVarMetadataDate()
        {
            // YYYY-MM-DD HH:MM:SS +hhmm
            var dateTime = new DateTimeOffset(2018, 9, 25, 22, 33, 44, new TimeSpan(6, 30, 0));
            var expectedValue = new MetadataDateTime(dateTime, UtcDateTime);
            var cfg = "release_date = 2018-09-25 22:33:44 +0630";

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.Inflate(stream);

                Assert.IsType<CfgVarMetadataDate>(cfgVarMetadata);
                Assert.True(cfgVarMetadata.DeserializeByteCount > 0);
                Assert.Equal(expectedValue, ((CfgVarMetadataDate)cfgVarMetadata).Date);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateCfgVarMetadataFeatureCompatibility()
        {
            var expectedValue = FeatureCompatibility.Requires;
            var cfg = "ecs_compat = 3";

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfg)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.Inflate(stream);

                Assert.IsType<CfgVarMetadataFeatureCompatibility>(cfgVarMetadata);
                Assert.True(cfgVarMetadata.DeserializeByteCount > 0);
                Assert.Equal(expectedValue, ((CfgVarMetadataFeatureCompatibility)cfgVarMetadata).Compatibility);
            }
        }

        [Theory]
        [InlineData("name = Bargle", typeof(CfgVarMetadataString))]
        [InlineData("jlp_flash = 256", typeof(CfgVarMetadataInteger))]
        [InlineData("lto_mapper = 0", typeof(CfgVarMetadataBoolean))]
        [InlineData("build_date = 2000/01/01", typeof(CfgVarMetadataDate))]
        [InlineData("intv2_compat = 2", typeof(CfgVarMetadataFeatureCompatibility))]
        public void CfgVarMetadata_InflateThenSerialize_ThrowsNotImplementedException(string stringToParse, Type expectedType)
        {
            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(stringToParse)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.Inflate(stream);

                Assert.IsType(expectedType, cfgVarMetadata);
                Assert.Equal(-1, cfgVarMetadata.SerializeByteCount);
                using (var writeStream = new System.IO.MemoryStream())
                {
                    using (var writer = new BinaryWriter(writeStream))
                    {
                        Assert.Throws<NotImplementedException>(() => cfgVarMetadata.Serialize(writer));
                    }
                }
            }
        }

        [Fact]
        public void CfgVarMetadata_InflatAllWithVarsSectionFirstAndNoValidVars_FindsNoCfgVars()
        {
            var cfgWithVarsFirstAndNoneValid =
@"
[vars]
biff
Kipper = the dog
That's a toad, not a frog!
";

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgWithVarsFirstAndNoneValid)))
            {
                var cfgVarMetadata = CfgVarMetadataBlock.InflateAll(stream);

                Assert.Empty(cfgVarMetadata);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflatAllWithMultipleVarsSections_FindsAllCfgVars()
        {
            var cfgWithTwoVarsSections =
@"
[mapping]
$0000 - $0FFF = $5000

[vars]
name = wiglet

[preload]
whatevs man

[vars]
short_name = wig
";

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgWithTwoVarsSections)))
            {
                var cfgVarMetadataValues = CfgVarMetadataBlock.InflateAll(stream).Cast<CfgVarMetadataString>().Select(c => c.StringValue);

                Assert.Equal(2, cfgVarMetadataValues.Count());
                Assert.Contains("wiglet", cfgVarMetadataValues);
                Assert.Contains("wig", cfgVarMetadataValues);
            }
        }

        [Theory]
        [InlineData("dunno", "dunno")]
        [InlineData("\"Howdy doody!\"", "Howdy doody!")]
        [InlineData("\"", "")]
        [InlineData("\"\"", "")]
        [InlineData("\" \"", " ")]
        [InlineData("\"Mattel\" Electronics", "Mattel\" Electronics")]
        public void CfgVarMetadata_InflateStringValue_GetsAppropriateString(string cfgFileValue, string expectedCfgVarMetadataStringValue)
        {
            var cfgStringValueEntry = "publisher=" + cfgFileValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgStringValueEntry)))
            {
                var cfgVarMetadataString = CfgVarMetadataBlock.Inflate(stream) as CfgVarMetadataString;

                // The parser strips from beginning and end, so this is correct, despite being odd.
                Assert.Equal(expectedCfgVarMetadataStringValue, cfgVarMetadataString.StringValue);
            }
        }

        [Theory]
        [InlineData("+23", 23)]
        [InlineData("-99", -99)]
        [InlineData("a", 0)]
        [InlineData("1 2", 0)]
        [InlineData("", 0)]
        [InlineData("99887766554433221100112233445566778899", 0)]
        public void CfgVarMetadata_InflateIntegralValue_GetsProperValue(string cfgFileValue, int expectedCfgVarMetadataIntegerValue)
        {
            var cfgIntegerValueEntry = "jlp_flash = " + cfgFileValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgIntegerValueEntry)))
            {
                var cfgVarMetadataInteger = CfgVarMetadataBlock.Inflate(stream) as CfgVarMetadataInteger;

                Assert.Equal(expectedCfgVarMetadataIntegerValue, cfgVarMetadataInteger.IntegerValue);
            }
        }

        [Theory]
        [InlineData("0", FeatureCompatibility.Incompatible)]
        [InlineData(" 1 ", FeatureCompatibility.Tolerates)]
        [InlineData("2", FeatureCompatibility.Enhances)]
        [InlineData("3 ", FeatureCompatibility.Requires)]
        [InlineData("+3 ", FeatureCompatibility.Requires)]
        [InlineData("", FeatureCompatibility.Tolerates)]
        [InlineData("-", FeatureCompatibility.Tolerates)]
        [InlineData(" ", FeatureCompatibility.Tolerates)]
        [InlineData("\"\"", FeatureCompatibility.Tolerates)]
        [InlineData("\" \"", FeatureCompatibility.Tolerates)]
        [InlineData("\"2\"", FeatureCompatibility.Enhances)]
        [InlineData("\" 2 \"", FeatureCompatibility.Enhances)]
        [InlineData("4", FeatureCompatibility.Tolerates)]
        [InlineData("-4", FeatureCompatibility.Tolerates)]
        [InlineData("444", FeatureCompatibility.Tolerates)]
        [InlineData("who's on first?", FeatureCompatibility.Tolerates)]
        public void CfgVarMetadata_InflateFeatureCompatibility_GetsExpectedFeatureCompatibilty(string cfgFileValue, FeatureCompatibility expectedCfgVarMetadataFeatureCompatibilityValue)
        {
            var cfgFeatureCompatibilityEntry = "tv_compat = " + cfgFileValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgFeatureCompatibilityEntry)))
            {
                var cfgVarMetadataFeatureCompatibility = CfgVarMetadataBlock.Inflate(stream) as CfgVarMetadataFeatureCompatibility;

                Assert.Equal(expectedCfgVarMetadataFeatureCompatibilityValue, cfgVarMetadataFeatureCompatibility.Compatibility);
            }
        }

        [Theory]
        [InlineData(CfgVarMetadataIdTag.Ecs, "", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Ecs, "0", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Ecs, "1", FeatureCompatibility.Requires)]
        [InlineData(CfgVarMetadataIdTag.Ecs, "2", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Ecs, "3", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Ecs, "4", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "0", FeatureCompatibility.Incompatible)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "1", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "2", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "3", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "4", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Voice, "", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Voice, "0", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Voice, "1", FeatureCompatibility.Enhances)]
        [InlineData(CfgVarMetadataIdTag.Voice, "2", FeatureCompatibility.Tolerates)]
        [InlineData(CfgVarMetadataIdTag.Voice, "3", FeatureCompatibility.Tolerates)]
        public void CfgVarMetadata_InflateLegacyCompatibility_GetsExpectedFeatureCompatibility(CfgVarMetadataIdTag legacyTag, string cfgFileValue, FeatureCompatibility expectedCfgVarMetadataFeatureCompatibilityValue)
        {
            var cfgLegacyFeatureCompatibilityEntry = legacyTag.ToCfgVarString() + " = " + cfgFileValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgLegacyFeatureCompatibilityEntry)))
            {
                var cfgVarMetadataFeatureCompatibility = CfgVarMetadataBlock.Inflate(stream) as CfgVarMetadataFeatureCompatibility;

                Assert.Equal(expectedCfgVarMetadataFeatureCompatibilityValue, cfgVarMetadataFeatureCompatibility.Compatibility);
            }
        }

        [Fact]
        public void CfgVarMetadata_InflateBogusCompatibilty_ThrowsInvalidOperationException()
        {
            var makeItCrash = "1";
            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(makeItCrash)))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var cfgVarMetadataFeatureCompatibility = new CfgVarMetadataFeatureCompatibility(CfgVarMetadataIdTag.Name);

                    Assert.Throws<InvalidOperationException>(() => cfgVarMetadataFeatureCompatibility.Deserialize(reader));
                }
            }
        }

        [Theory]
        [InlineData("+23", true)]
        [InlineData("-99", true)]
        [InlineData("a", false)]
        [InlineData("1 2", false)]
        [InlineData("", false)]
        [InlineData("99887766554433221100112233445566778899", false)]
        [InlineData("true", false)]
        [InlineData("false", false)]
        public void CfgVarMetadata_InflateBoolean_GetsExpectedBooleanValue(string cfgFileValue, bool expectedCfgVarMetadataBooleanValue)
        {
            var cfgBooleanEntry = "  lto_mapper           =" + cfgFileValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgBooleanEntry)))
            {
                var cfgVarMetadataBoolean = CfgVarMetadataBlock.Inflate(stream) as CfgVarMetadataBoolean;

                Assert.Equal(expectedCfgVarMetadataBooleanValue, cfgVarMetadataBoolean.BooleanValue);
            }
        }

        /* Date format information snapshot from as1600.txt
         * ------------------------------------------------------------------------------
         * Dates are variable-precision quantities.  They can specify as little as just
         * the year, or they can specify all the way down to seconds, including timezone.
         * The full date format is one of the following four patterns:
         * 
         * YYYY-MM-DD HH:MM:SS +hhmm
         * YYYY-MM-DD HH:MM:SS +hh:mm
         * YYYY/MM/DD HH:MM:SS +hhmm
         * YYYY/MM/DD HH:MM:SS +hh:mm
         *
         * That is:
         * 
         * -- Year: 4 digits, 1900 - 2155
         * -- Month: 2 digits, 01 - 12
         * -- Day: 2 digits, 01 - 31
         * -- Hour: 2 digits, 00 - 23
         * -- Minutes: 2 digits, 00 - 59
         * -- Seconds: 2 digits, 00 - 60  (leap second permitted)
         * -- + or - to indicate east/west of UTC
         * -- Hours ahead/behind UTC (0 - 12)
         * -- Minutes ahead/behind UTC
         * 
         * Either slashes or dashes are permitted between the YYYY-MM-DD; however, you
         * must be consistent.  You can specify lower precision dates by leaving off
         * later fields.  For example:
         * 
         * YYYY                Year only
         * YYYY-MM             Year and month only
         * YYYY-MM-DD HH:MM    Year, month, day, hours, and minute
         * 
         * For dates, years below 100 are interpreted as 19xx.  So 80 means 1980, while
         * 17 means 1917.   Be mindful of Y2K.  The valid range of years is 1901 to 2155,
         * with 1 through 99 serving as aliased for 1901 to 1999.  A year of 0 or any
         * other out-of-range value is treated as an invalid/missing date.
         */
        public static IEnumerable<object[]> MetadataDateTimeTestData
        {
            get
            {
                yield return new object[] { "2009-11-27 12:13:14 +0400", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, 4, 0) };
                yield return new object[] { "2009-11-27 12:13:14 -1138", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, -11, 38) };
                yield return new object[] { "2009-11-27 12:13:14 -04:56", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, -4, -56) };
                yield return new object[] { "2009-11-27 12:13:14 +01:23", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, 1, 23) };
                yield return new object[] { "2009-11-27 12:13:14", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14) };
                yield return new object[] { "2009-11-27 12:13", CreateMetadataDateTime(2009, 11, 27, 12, 13) };
                yield return new object[] { "2009-11-27 12", CreateMetadataDateTime(2009, 11, 27, 12) };
                yield return new object[] { "2009-11-27", CreateMetadataDateTime(2009, 11, 27) };
                yield return new object[] { "2009-11", CreateMetadataDateTime(2009, 11) };
                yield return new object[] { "2009", CreateMetadataDateTime(2009) };
                yield return new object[] { "20", CreateMetadataDateTime(1920) };
                yield return new object[] { "2009/11/27 12:13:14 +0400", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, 4, 0) };
                yield return new object[] { "2009/11/27 12:13:14 -1016", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, -10, -16) };
                yield return new object[] { "2009/11/27 12:13:14 -04:56", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, -4, -56) };
                yield return new object[] { "2009/11/27 12:13:14 +01:23", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14, 1, 23) };
                yield return new object[] { "2009/11/27 12:13:14", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14) };
                yield return new object[] { "2009/11/27 12:13", CreateMetadataDateTime(2009, 11, 27, 12, 13) };
                yield return new object[] { "2009/11/27 12", CreateMetadataDateTime(2009, 11, 27, 12) };
                yield return new object[] { "2009/11/27", CreateMetadataDateTime(2009, 11, 27) };
                yield return new object[] { "2009/11", CreateMetadataDateTime(2009, 11) };
                yield return new object[] { "2009-11-27 12:13:60", CreateMetadataDateTime(2009, 11, 27, 12, 13, 60) };
                yield return new object[] { "256", MetadataDateTime.MinValue };
                yield return new object[] { "-1", MetadataDateTime.MinValue };
                yield return new object[] { "1900", MetadataDateTime.MinValue };
                yield return new object[] { "2156", MetadataDateTime.MinValue };
                yield return new object[] { "1980-13", CreateMetadataDateTime(1980) };
                yield return new object[] { "81-0", CreateMetadataDateTime(1981) };
                yield return new object[] { "1982/-3", CreateMetadataDateTime(1982) };
                yield return new object[] { "-99/44/3", MetadataDateTime.MinValue };
                yield return new object[] { "/45/12/25", MetadataDateTime.MinValue };
                yield return new object[] { "/-05/12/25", MetadataDateTime.MinValue };
                yield return new object[] { "2 3 4", CreateMetadataDateTime(1902) };
                yield return new object[] { "1980-13-14", CreateMetadataDateTime(1980) };
                yield return new object[] { "1983-12/-14", MetadataDateTime.MinValue };
                yield return new object[] { "1983/12/-14", CreateMetadataDateTime(1983, 12) }; // This is due to parser preferring '/' over '-' separator.
                yield return new object[] { "78-12-6 26:98", CreateMetadataDateTime(1978, 12, 6) };
                yield return new object[] { "86-6-6 14:684", CreateMetadataDateTime(1986, 6, 6, 14) };
                yield return new object[] { "1989/2/23 4:18:61", CreateMetadataDateTime(1989, 2, 23, 4, 18) };
                yield return new object[] { "1988/3/16 -5:32:12", CreateMetadataDateTime(1988, 3, 16) };
                yield return new object[] { "066/0005/009 019:008:060 -2", CreateMetadataDateTime(1966, 5, 9, 19, 8, 60) };
                yield return new object[] { "066/0005/009 019:008:060 -00", CreateMetadataDateTime(1966, 5, 9, 19, 8, 60, 0, null) };
                yield return new object[] { "1985-8-4 12:34:56 +02:", CreateMetadataDateTime(1985, 8, 4, 12, 34, 56, 2, null) };
                yield return new object[] { "1985-8-4 12:34:56 :", CreateMetadataDateTime(1985, 8, 4, 12, 34, 56) };
                yield return new object[] { "1985-8-4 12:34:56 +:", CreateMetadataDateTime(1985, 8, 4, 12, 34, 56) };
                yield return new object[] { "1985-8-4 12:34:56 -2:3", CreateMetadataDateTime(1985, 8, 4, 12, 34, 56, -2, 3) };
                yield return new object[] { "1985-8-4 12:34:56 -2:-3", CreateMetadataDateTime(1985, 8, 4, 12, 34, 56, -2, 0) }; // Value after ':' in offset is parsed as unsigned byte.
                yield return new object[] { "2009-11-27 12:13:14 -1234", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14) }; // UTC offset range is -12 to +12 hours -- this specifies -12:34, which is invalid, and therefore ignored.
                yield return new object[] { "2009-11-27 12:13:14 +12:01", CreateMetadataDateTime(2009, 11, 27, 12, 13, 14) }; // UTC offset range is -12 to +12 hours -- this specifies +12:01, which is invalid, and therefore ignored.
                yield return new object[] { string.Empty, MetadataDateTime.MinValue };
                yield return new object[] { " ", MetadataDateTime.MinValue };
                yield return new object[] { " \" ", MetadataDateTime.MinValue };
                yield return new object[] { "\"\"", MetadataDateTime.MinValue };
                yield return new object[] { "\" \" ", MetadataDateTime.MinValue };
                yield return new object[] { "\n", MetadataDateTime.MinValue };
                yield return new object[] { "\r", MetadataDateTime.MinValue };
                yield return new object[] { "34-well/that's just--AWFUL!", MetadataDateTime.MinValue }; // Again -- code favors '/' over '-'.
                yield return new object[] { "34/well/that's just--AWFUL!", CreateMetadataDateTime(1934) };
            }
        }

        [Theory]
        [MemberData("MetadataDateTimeTestData")]
        public void CfgVarMetadata_InflateMetadataDateTime_GetsExpectedMetadataDateTimeValue(string cfgFileValue, MetadataDateTime expectedCfgVarMetadataDateValue)
        {
            var cfgBooleanEntry = "build_date =" + cfgFileValue;

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cfgBooleanEntry)))
            {
                var cfgVarMetadataBoolean = CfgVarMetadataBlock.Inflate(stream) as CfgVarMetadataDate;

                var compareResult = expectedCfgVarMetadataDateValue.CompareTo(cfgVarMetadataBoolean.Date, strict: true, compareOnlyCommonValidFields: false);
                if (compareResult != 0)
                {
                    Output.WriteLine("Input:    [" + cfgFileValue + "]\nExpected: " + expectedCfgVarMetadataDateValue + "\nActual:   " + cfgVarMetadataBoolean.Date);
                }
                Assert.Equal(0, compareResult);
            }
        }

        private static MetadataDateTime CreateMetadataDateTime(int year)
        {
            return CreateMetadataDateTime(year, 0);
        }

        private static MetadataDateTime CreateMetadataDateTime(int year, int month)
        {
            return CreateMetadataDateTime(year, month, 0);
        }

        private static MetadataDateTime CreateMetadataDateTime(int year, int month, int day)
        {
            return CreateMetadataDateTime(year, month, day, -1);
        }

        private static MetadataDateTime CreateMetadataDateTime(int year, int month, int day, int hour)
        {
            return CreateMetadataDateTime(year, month, day, hour, -1);
        }

        private static MetadataDateTime CreateMetadataDateTime(int year, int month, int day, int hour, int minute)
        {
            return CreateMetadataDateTime(year, month, day, hour, minute, -1);
        }

        private static MetadataDateTime CreateMetadataDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            return CreateMetadataDateTime(year, month, day, hour, minute, second, null, null);
        }

        private static MetadataDateTime CreateMetadataDateTime(int year, int month, int day, int hour, int minute, int second, int? offsetHours, int? offsetMinutes)
        {
            var dateTime = MetadataDateTime.MinValue;

            var flags = MetadataDateTimeFlags.None;
            if (year > 0)
            {
                flags |= MetadataDateTimeFlags.Year;
            }

            if (month > 0)
            {
                flags |= MetadataDateTimeFlags.Month;
            }
            else
            {
                month = 1;
            }

            if (day > 0)
            {
                flags |= MetadataDateTimeFlags.Day;
            }
            else
            {
                day = 1;
            }

            if (hour >= 0)
            {
                flags |= MetadataDateTimeFlags.Hour;
            }
            else
            {
                hour = 0;
            }

            if (minute >= 0)
            {
                flags |= MetadataDateTimeFlags.Minute;
            }
            else
            {
                minute = 0;
            }

            if (second >= 0)
            {
                flags |= MetadataDateTimeFlags.Second;
            }
            else
            {
                second = 0;
            }
            if (second == 60)
            {
                second = 59;
                flags |= MetadataDateTimeFlags.LeapSecond;
            }

            if (offsetHours.HasValue)
            {
                flags |= MetadataDateTimeFlags.UtcOffset;
            }
            if (offsetMinutes.HasValue)
            {
                flags |= MetadataDateTimeFlags.UtcOffset;
            }

            if (flags != MetadataDateTimeFlags.None)
            {
                var offset = TimeSpan.Zero;
                if (flags.HasFlag(MetadataDateTimeFlags.UtcOffset))
                {
                    var hours = offsetHours.HasValue ? offsetHours.Value : 0;
                    var minutes = offsetMinutes.HasValue ? offsetMinutes.Value : 0;
                    if ((hours < 0) && (minutes > 0))
                    {
                        minutes = -minutes;
                    }
                    offset = new TimeSpan(hours, minutes, 0);
                }
                var dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, second, offset);
                dateTime = new MetadataDateTime(dateTimeOffset, flags);
            }

            return dateTime;
        }
    }
}

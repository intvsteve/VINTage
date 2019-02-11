// <copyright file="StringUtilitiesTests.cs" company="INTV Funhouse">
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
using System.Text.RegularExpressions;
using System.Web;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests
{
    public class StringUtilitiesTests
    {
        #region EnforceNameLength Tests

        [Theory]
        [InlineData(null, 2, true, null)]
        [InlineData("", 1, true, "")]
        [InlineData("TestString", 1, true, "T")]
        [InlineData("TestString", 100, true, "TestString")]
        [InlineData("TestString", 10, true, "TestString")]
        [InlineData("TestString", 0, true, "")]
        [InlineData("üben!", 2, true, "~b")]
        [InlineData("üben!", 2, false, "üb")]
        [InlineData("üben!", 20, true, "~ben!")]
        [InlineData("üben!", 20, false, "üben!")]
        [InlineData("üben!", 0, true, "")]
        [InlineData("üben!", 0, false, "")]
        public void EnforceNameLength_EnforcesExpectedLengthAndSubstitutesCharacters(string stringToFilter, int maxLength, bool restrictToGromCharacters, string expectedResult)
        {
            var actualResult = stringToFilter.EnforceNameLength(maxLength, restrictToGromCharacters);
            Assert.Equal(expectedResult, actualResult);
        }

        #endregion // EnforceNameLength Tests

        #region HTML Decoder Tests

        [Theory]
        [InlineData(null)]
        [InlineData("TestString without HTML to decode")]
        [InlineData("Test string with &quot;encoded&quot; HTML characters")]
        [InlineData("Test string with <b>&quot;encoded&quot;</b> HTML characters")]
        public void RegisterNullHtmlDecoder_CallDecodeHtmlString_DoesNotCrash(string testString)
        {
            StringUtilities.RegisterHtmlDecoder(null);
            Assert.Equal(testString, testString.DecodeHtmlString());
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("TestString without HTML to decode", "TestString without HTML to decode")]
        [InlineData("Test string with &quot;encoded&quot; HTML characters", "Test string with \"encoded\" HTML characters")]
        [InlineData("Test string with <b>&quot;encoded&quot;</b> HTML characters", "Test string with <b>\"encoded\"</b> HTML characters")]
        public void RegisterSimpleHtmlDecoder_CallDecodeHtmlString_DecodesHtml(string testString, string expectedResult)
        {
            StringUtilities.RegisterHtmlDecoder(HtmlDecode);
            Assert.Equal(expectedResult, testString.DecodeHtmlString());
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("TestString without HTML to decode", "TestString without HTML to decode")]
        [InlineData("Test string with &quot;encoded&quot; HTML characters", "Test string with \"encoded\" HTML characters")]
        [InlineData("Test string with <b>&quot;encoded&quot;</b> HTML characters", "Test string with \"encoded\" HTML characters")]
        public void RegisterBasicHtmlDecoder_CallDecodeHtmlString_DecodesHtmlAndRemovesTags(string testString, string expectedResult)
        {
            StringUtilities.RegisterHtmlDecoder(HtmlStripTagsAndDecode);
            Assert.Equal(expectedResult, testString.DecodeHtmlString());
        }

        #endregion // HTML Decoder Tests

        #region HTML Encoder Tests

        [Theory]
        [InlineData(null)]
        [InlineData("TestString without HTML to encode")]
        [InlineData("Test string with \"non-encoded\" HTML characters")]
        [InlineData("Test string with non-encoded \"Dungeons & Dragons©®\" HTML characters")]
        public void RegisterNullHtmlEncoder_CallEncodeHtmlString_DoesNotCrash(string testString)
        {
            StringUtilities.RegisterHtmlEncoder(null);
            Assert.Equal(testString, testString.EncodeHtmlString());
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("TestString without HTML to encode", "TestString without HTML to encode")]
        [InlineData("Test string with \"non-encoded\" HTML characters", "Test string with &quot;non-encoded&quot; HTML characters")]
        [InlineData("Test string with non-encoded \"Dungeons & Dragons©®\" HTML characters", "Test string with non-encoded &quot;Dungeons &amp; Dragons&#169;&#174;&quot; HTML characters")]
        ////[InlineData("Test string with non-encoded \"Dungeons & Dragons©®\" HTML characters", "Test string with non-encoded &quot;Dungeons &amp; Dragons&copy;&reg;&quot; HTML characters")]
        public void RegisterHtmlEncoder_CallEncodeHtmlString_EncodesHtml(string testString, string expectedResult)
        {
            StringUtilities.RegisterHtmlEncoder(HtmlEncode);
            Assert.Equal(expectedResult, testString.EncodeHtmlString());
        }

        #endregion // HTML Encoder Tests

        #region ContainsInvalidCharacters Tests

        [Theory]
        [InlineData(null, false, false)]
        [InlineData("", true, false)]
        [InlineData(" \t ", true, false)]
        [InlineData("\01\02\03whoa", false, true)]
        [InlineData("\u2400", false, false)] // other symbol
        [InlineData("\uf000", false, true)] // private
        [InlineData("\t", false, false)]
        [InlineData("\r", false, true)]
        [InlineData("\n", false, true)]
        [InlineData("\f", false, true)]
        [InlineData("\r\n", false, true)]
        [InlineData("\u2028", false, true)] // line separator
        [InlineData("\u2029", false, true)] // paragraph separator
        [InlineData("\t", true, false)]
        [InlineData("\r", true, false)]
        [InlineData("\n", true, false)]
        [InlineData("\f", true, true)]
        [InlineData("\r\n", true, false)]
        [InlineData("a\rb", false, true)]
        [InlineData("a\nb", false, true)]
        [InlineData("a\r\nb", true, false)]
        [InlineData("a\r\nb", true, false)]
        [InlineData("\u2028", true, false)] // line separator
        [InlineData("\u2029", true, false)] // paragraph separator
        [InlineData("\ufffd", true, false)] // replacement character (other symbol)
        [InlineData("\ufffe", true, true)] // not-a-character
        [InlineData("\uffff", true, true)] // not-a-character
        [InlineData("∞", false, false)]
        [InlineData("∞", true, false)]
        [InlineData("A bòöger!{{}}])(_*@!&#)(&%)(*&%!#*+_|", false, false)]
        [InlineData("A bòöger!{{}}])(_*@!&#)(&%)(*&%!#*+_|", true, false)]
        public void ContainsInvalidCharacters_TestGivenString_DetectsInvalidCharacters(string stringToCheck, bool allowLineBreaks, bool expectedResult)
        {
            var actualResult = stringToCheck.ContainsInvalidCharacters(allowLineBreaks);
            Assert.Equal(expectedResult, actualResult);
        }

        #endregion // ContainsInvalidCharacters Tests

        #region GetEnclosingQuoteCharacterIndexes Tests

        [Fact]
        public void GetEnclosingQuoteCharacterIndexes_NullString_ThrowsArgumentNullException()
        {
            string nullString = null;

            Assert.Throws<ArgumentNullException>(() => nullString.GetEnclosingQuoteCharacterIndexes());
        }

        [Theory]
        [InlineData("", -1, -1)]
        [InlineData(" \" ", 1, 1)]
        [InlineData("\"\"\"", 0, 2)]
        [InlineData("asd\"f\"gh", -1, -1)]
        public void GetEnclosingQuoteCharacterIndexes_FromString_ReturnsExpectedRange(string stringToCheck, int expectedFirstQuoteIndex, int expactedLastQuoteIndex)
        {
            var indexes = stringToCheck.GetEnclosingQuoteCharacterIndexes();

            Assert.Equal(expectedFirstQuoteIndex, indexes.Minimum);
            Assert.Equal(expactedLastQuoteIndex, indexes.Maximum);
        }

        #endregion // GetEnclosingQuoteCharacterIndexes Tests

        #region C-Style Formatting Tests

        /// <summary>
        /// Tests for the SPrintf utility.
        /// </summary>
        /// <param name="value">The value to send to SPrintf.</param>
        /// <param name="format">The C-style format specifier to use.</param>
        /// <param name="expectedResult">The expected string.</param>
        [Theory]
        [InlineData(48879, "%.4X", "BEEF")]
        [InlineData(3565, "%.3X", "DED")]
        [InlineData(112, "%.2X", "70")]
        [InlineData(48879, "%.4x", "beef")]
        [InlineData(3565, "%.3x", "ded")]
        [InlineData(112, "%.2x", "70")]
        [InlineData(60, "%d", "60")]
        [InlineData(-60, "%d", "-60")]
        [InlineData(10, "%o", "12")]
        public void CStyleFormatSpecifier_CallSPrintfWithOneIntArgument_GetExpectedString(int value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(48879L, "%.4X", "BEEF")]
        [InlineData(3565L, "%.3X", "DED")]
        [InlineData(112L, "%.2X", "70")]
        [InlineData(48879L, "%.4x", "beef")]
        [InlineData(3565L, "%.3x", "ded")]
        [InlineData(112L, "%.2x", "70")]
        [InlineData(60L, "%d", "60")]
        [InlineData(-60L, "%d", "-60")]
        [InlineData(10L, "%o", "12")]
        public void CStyleFormatSpecifier_CallSPrintfWithOneLongArgument_GetExpectedString(long value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        /// <summary>
        /// Tests for the SPrintf utility.
        /// </summary>
        /// <param name="firstValue">The first value to send to SPrintf.</param>
        /// <param name="secondValue">The second value to send to SPrintf.</param>
        /// <param name="format">The C-style format specifier to use.</param>
        /// <param name="expectedResult">The expected string.</param>
        /// <remarks>Note that only those aspects of C-style formatting are tested that are useful in VINTage. This restricts
        /// the tests to a narrow subset of the gamut of possibilities in C-style formatting. In practicality, this means
        /// the format strings used in error_db.yaml in the INTV.LtoFlash plugin.</remarks>
        [Theory]
        [InlineData(48879, 3565, "This %.4X is %.3X", "This BEEF is DED")]
        [InlineData(112, 60, "%.2X is the new %d", "70 is the new 60")]
        public void CStyleFormatSpecifier_CallSPrintfWithTwoIntArguments_GetExpectedString(int firstValue, int secondValue, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, firstValue, secondValue);
            Assert.Equal(expectedResult, actualResult);
        }

        /// <summary>
        /// Tests for the SPrintf utility based on using the decimal integer format specifier.
        /// </summary>
        /// <param name="value">The integer value to use.</param>
        /// <param name="format">The C-style format specifier to use.</param>
        /// <param name="expectedResult">The expected string result of the formatting.</param>
        /// <remarks>Tests that use group separators are assumed to use the comma. Some may fail unless / until this is
        /// rewritten using the member data feature</remarks>
        [Theory]
        [InlineData(69, "%d", "69")]
        [InlineData(69, "%10d", "        69")]
        [InlineData(69, "%-10d", "69        ")]
        [InlineData(69, "%010d", "0000000069")]
        [InlineData(69, "%-010d", "69        ")]
        [InlineData(69, "%+d", "+69")]
        [InlineData(69, "%+10d", "       +69")]
        [InlineData(69, "%-+10d", "+69       ")]
        [InlineData(69, "%+010d", "+000000069")]
        [InlineData(69, "%-+010d", "+69       ")]
        [InlineData(65537, "%d", "65537")]
        [InlineData(65537, "%'d", "65,537")]
        [InlineData(10065537, "%'d", "10,065,537")]
        [InlineData(-69, "%d", "-69")]
        [InlineData(-69, "%10d", "       -69")]
        [InlineData(-69, "%-10d", "-69       ")]
        [InlineData(-69, "%010d", "-000000069")]
        [InlineData(-69, "%-010d", "-69       ")]
        [InlineData(-69, "%+d", "-69")]
        [InlineData(-69, "%+10d", "       -69")]
        [InlineData(-69, "%-+10d", "-69       ")]
        [InlineData(-69, "%+010d", "-000000069")]
        [InlineData(-69, "%-+010d", "-69       ")]
        [InlineData(-65537, "%d", "-65537")]
        [InlineData(-65537, "%'d", "-65,537")]
        [InlineData(-10065537, "%'d", "-10,065,537")]
        [InlineData(69, "%hd", "69")]
        [InlineData(69, "%ld", "69")]
        public void CStyleDecimalFormatSpecifier_CallSPrintfWithOneInt_GetExpectedString(int value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(69, "%d", "69")]
        [InlineData(69, "%10d", "        69")]
        [InlineData(69, "%-10d", "69        ")]
        [InlineData(69, "%010d", "0000000069")]
        [InlineData(69, "%-010d", "69        ")]
        [InlineData(69, "%+d", "+69")]
        [InlineData(69, "%+10d", "       +69")]
        [InlineData(69, "%-+10d", "+69       ")]
        [InlineData(69, "%+010d", "+000000069")]
        [InlineData(69, "%-+010d", "+69       ")]
        [InlineData(-69, "%d", "-69")]
        [InlineData(-69, "%10d", "       -69")]
        [InlineData(-69, "%-10d", "-69       ")]
        [InlineData(-69, "%010d", "-000000069")]
        [InlineData(-69, "%-010d", "-69       ")]
        [InlineData(-69, "%+d", "-69")]
        [InlineData(-69, "%+10d", "       -69")]
        [InlineData(-69, "%-+10d", "-69       ")]
        [InlineData(-69, "%+010d", "-000000069")]
        [InlineData(-69, "%-+010d", "-69       ")]
        public void CStyleDecimalFormatSpecifier_CallSPrintfWithOneSByte_GetExpectedString(sbyte value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void CStyleDecimalFormatSpecifier_CallSPrintfWithOneDecimal_ThrowsFormatException()
        {
            decimal value = 42;
            Assert.Throws<FormatException>(() => StringUtilities.SPrintf("%d", value));
        }

        [Theory]
        [InlineData(69, "[%i]", "[69]")]
        [InlineData(69, "[% i]", "[ 69]")]
        [InlineData(69, "[% -10i]", "[ 69       ]")]
        [InlineData(69, "[% +10i]", "[       +69]")]
        [InlineData(69, "[% 010i]", "[ 000000069]")]
        [InlineData(69, "[%10i]", "[        69]")]
        [InlineData(69, "%-10i", "69        ")]
        [InlineData(69, "%010i", "0000000069")]
        [InlineData(69, "%-010i", "69        ")]
        [InlineData(69, "%+i", "+69")]
        [InlineData(69, "%+10i", "       +69")]
        [InlineData(69, "%-+10i", "+69       ")]
        [InlineData(69, "%+010i", "+000000069")]
        [InlineData(69, "%-+010i", "+69       ")]
        [InlineData(65537, "%i", "65537")]
        [InlineData(65537, "%'i", "65,537")]
        [InlineData(10065537, "%'i", "10,065,537")]
        [InlineData(-69, "%i", "-69")]
        [InlineData(-69, "%10i", "       -69")]
        [InlineData(-69, "%-10i", "-69       ")]
        [InlineData(-69, "%010i", "-000000069")]
        [InlineData(-69, "%-010i", "-69       ")]
        [InlineData(-69, "%+i", "-69")]
        [InlineData(-69, "%+10i", "       -69")]
        [InlineData(-69, "%-+10i", "-69       ")]
        [InlineData(-69, "%+010i", "-000000069")]
        [InlineData(-69, "%-+010i", "-69       ")]
        [InlineData(-65537, "%i", "-65537")]
        [InlineData(-65537, "%'i", "-65,537")]
        [InlineData(-10065537, "%'i", "-10,065,537")]
        [InlineData(69, "%hi", "69")]
        [InlineData(69, "%li", "69")]
        public void CStyleIntegerFormatSpecifier_CallSPrintfWithOneInt_GetExpectedString(int value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "214")]
        [InlineData(-42, "%20u", "                 214")]
        [InlineData(-42, "%-20u", "214                 ")]
        [InlineData(-42, "%020u", "00000000000000000214")]
        [InlineData(-42, "%-020u", "214                 ")]
        [InlineData(127, "%u", "127")]
        [InlineData(127, "%'u", "127")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneSByte_GetExpectedString(sbyte value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(127, "%u", "127")]
        [InlineData(127, "%'u", "127")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneByte_GetExpectedString(byte value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "65494")]
        [InlineData(-42, "%20u", "               65494")]
        [InlineData(-42, "%-20u", "65494               ")]
        [InlineData(-42, "%020u", "00000000000000065494")]
        [InlineData(-42, "%-020u", "65494               ")]
        [InlineData(32767, "%u", "32767")]
        [InlineData(32767, "%'u", "32,767")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneShort_GetExpectedString(short value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(32767, "%u", "32767")]
        [InlineData(32767, "%'u", "32,767")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneUnsignedShort_GetExpectedString(ushort value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "4294967254")]
        [InlineData(-42, "%20u", "          4294967254")]
        [InlineData(-42, "%-20u", "4294967254          ")]
        [InlineData(-42, "%020u", "00000000004294967254")]
        [InlineData(-42, "%-020u", "4294967254          ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneInt_GetExpectedString(int value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneUnsignedInt_GetExpectedString(uint value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "18446744073709551574")]
        [InlineData(-42, "%21u", " 18446744073709551574")]
        [InlineData(-42, "%-21u", "18446744073709551574 ")]
        [InlineData(-42, "%021u", "018446744073709551574")]
        [InlineData(-42, "%-021u", "18446744073709551574 ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneLong_GetExpectedString(long value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        [InlineData(42, "%hu", "42")]
        [InlineData(42, "%lu", "42")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithUnsignedOneLong_GetExpectedString(ulong value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "4294967254")]
        [InlineData(-42, "%20u", "          4294967254")]
        [InlineData(-42, "%-20u", "4294967254          ")]
        [InlineData(-42, "%020u", "00000000004294967254")]
        [InlineData(-42, "%-020u", "4294967254          ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneFloat_GetExpectedString(float value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "18446744073709551574")]
        [InlineData(-42, "%21u", " 18446744073709551574")]
        [InlineData(-42, "%-21u", "18446744073709551574 ")]
        [InlineData(-42, "%021u", "018446744073709551574")]
        [InlineData(-42, "%-021u", "18446744073709551574 ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneDouble_GetExpectedString(double value, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(42, "%u", "42")]
        [InlineData(42, "%10u", "        42")]
        [InlineData(42, "%-10u", "42        ")]
        [InlineData(42, "%010u", "0000000042")]
        [InlineData(42, "%-010u", "42        ")]
        [InlineData(-42, "%u", "18446744073709551574")]
        [InlineData(-42, "%21u", " 18446744073709551574")]
        [InlineData(-42, "%-21u", "18446744073709551574 ")]
        [InlineData(-42, "%021u", "018446744073709551574")]
        [InlineData(-42, "%-021u", "18446744073709551574 ")]
        [InlineData(65537, "%u", "65537")]
        [InlineData(65537, "%'u", "65,537")]
        [InlineData(10065537, "%'u", "10,065,537")]
        public void CStyleUnsignedDecimalFormatSpecifier_CallSPrintfWithOneDecimal_GetExpectedBehavior(decimal value, string format, string expectedResult)
        {
            if (value < 0)
            {
                Assert.Throws<OverflowException>(() => StringUtilities.SPrintf(format, value));
            }
            else
            {
                var actualResult = StringUtilities.SPrintf(format, value);
                Assert.Equal(expectedResult, actualResult);
            }
        }

        [Theory]
        [InlineData("%d")]
        [InlineData("%i")]
        [InlineData("%u")]
        [InlineData("%x")]
        [InlineData("%X")]
        [InlineData("%o")]
        [InlineData("%e")]
        [InlineData("%E")]
        [InlineData("%g")]
        [InlineData("%G")]
        [InlineData("%p")]
        [InlineData("%c")]
        [InlineData("%s")]
        public void CStyleFormatSpecifier_CallSPrintfWithNull_GetExpectedString(string format)
        {
            var actualResult = StringUtilities.SPrintf(format, null);
            Assert.Empty(actualResult);
        }

        [Theory]
        [InlineData("%d", "")]
        [InlineData("%i", "")]
        [InlineData("%u", "")]
        [InlineData("%x", "")]
        [InlineData("%X", "")]
        [InlineData("%o", "")]
        [InlineData("%e", "")]
        [InlineData("%E", "")]
        [InlineData("%g", "")]
        [InlineData("%G", "")]
        [InlineData("%p", "")]
        [InlineData("%c", "")]
        [InlineData("%s", "System.Object")]
        public void CStyleFormatSpecifier_CallSPrintfWithObject_GetExpectedString(string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, new object());
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%x]", "[2a]", 42)]
        [InlineData("[%X]", "[2A]", 42)]
        [InlineData("[%5x]", "[   2a]", 42)]
        [InlineData("[%5X]", "[   2A]", 42)]
        [InlineData("[%05x]", "[0002a]", 42)]
        [InlineData("[%05X]", "[0002A]", 42)]
        [InlineData("[%-05x]", "[2a   ]", 42)]
        [InlineData("[%-05X]", "[2A   ]", 42)]
        [InlineData("[%#x]", "[0x2a]", 42)]
        [InlineData("[%#X]", "[0X2A]", 42)]
        [InlineData("[%#5x]", "[ 0x2a]", 42)]
        [InlineData("[%#5X]", "[ 0X2A]", 42)]
        [InlineData("[%#05x]", "[0x02a]", 42)]
        [InlineData("[%#05X]", "[0X02A]", 42)]
        [InlineData("[%#-05x]", "[0x2a ]", 42)]
        [InlineData("[%#-05X]", "[0X2A ]", 42)]
        [InlineData("[%.2x]", "[05]", 5)]
        public void CStyleHexadecimalFormatSpecifier_CallSPrintfWithOneInt_GetExpectedString(string format, string expectedResult, int value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneLong_GetExpectedString(string format, string expectedResult, long value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneUnsignedLong_GetExpectedString(string format, string expectedResult, ulong value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneInt_GetExpectedString(string format, string expectedResult, int value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneUnsignedInt_GetExpectedString(string format, string expectedResult, uint value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneShort_GetExpectedString(string format, string expectedResult, short value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneUnsignedShort_GetExpectedString(string format, string expectedResult, ushort value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneSByte_GetExpectedString(string format, string expectedResult, sbyte value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneByte_GetExpectedString(string format, string expectedResult, byte value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneFloat_GetExpectedString(string format, string expectedResult, float value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneDouble_GetExpectedString(string format, string expectedResult, double value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%o]", "[52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%5o]", "[   52]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%05o]", "[00052]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%-05o]", "[52   ]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#o]", "[052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#5o]", "[  052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#05o]", "[00052]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        [InlineData("[%#-05o]", "[052  ]", 42)]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithOneDecimal_GetExpectedString(string format, string expectedResult, decimal value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void CStyleOctalFormatSpecifier_CallSPrintfWithObject_GetExpectedString()
        {
            var actualResult = StringUtilities.SPrintf("%o", new object());
            Assert.Equal(string.Empty, actualResult);
        }

        [Fact]
        public void CStylePointerFormatSpecifier_CallSPrintfWithIntPtr_GetExpectedString()
        {
            // NOTE: The pointer size will be determined by architecture used to run the tests.
            var actualResult = StringUtilities.SPrintf("[%p]", (IntPtr)987654);
            Assert.Equal("[0xf1206]", actualResult);
        }

        [Fact]
        public void CStylePointerFormatSpecifier_CallSPrintfWithInt_GetExpectedString()
        {
            var actualResult = StringUtilities.SPrintf("[%p]", 987654);
            Assert.Equal("[]", actualResult);
        }

        [Fact]
        public void CStylePointerFormatSpecifier_CallSPrintfWithLong_GetExpectedString()
        {
            var actualResult = StringUtilities.SPrintf("[%p]", 9876543210L);
            Assert.Equal("[]", actualResult);
        }

        [Theory]
        [InlineData("[%f]", "[42.000000]", 42)]
        [InlineData("[%f]", "[42.500000]", 42.5)]
        [InlineData("[%10f]", "[ 42.000000]", 42)]
        [InlineData("[%10f]", "[ 42.500000]", 42.5)]
        [InlineData("[%-10f]", "[42.000000 ]", 42)]
        [InlineData("[%-10f]", "[42.500000 ]", 42.5)]
        [InlineData("[%010f]", "[042.000000]", 42)]
        [InlineData("[%010f]", "[042.500000]", 42.5)]
        [InlineData("[%-010f]", "[42.000000 ]", 42)]
        [InlineData("[%-010f]", "[42.500000 ]", 42.5)]
        [InlineData("[%+f]", "[+42.000000]", 42)]
        [InlineData("[%+f]", "[+42.500000]", 42.5)]
        [InlineData("[%+10f]", "[+42.000000]", 42)]
        [InlineData("[%+10f]", "[+42.500000]", 42.5)]
        [InlineData("[%+-10f]", "[+42.000000]", 42)]
        [InlineData("[%+-10f]", "[+42.500000]", 42.5)]
        [InlineData("[%+010f]", "[+42.000000]", 42)]
        [InlineData("[%+010f]", "[+42.500000]", 42.5)]
        [InlineData("[%+-010f]", "[+42.000000]", 42)]
        [InlineData("[%+-010f]", "[+42.500000]", 42.5)]
        [InlineData("[%f]", "[-42.000000]", -42)]
        [InlineData("[%f]", "[-42.500000]", -42.5)]
        [InlineData("[%10f]", "[-42.000000]", -42)]
        [InlineData("[%10f]", "[-42.500000]", -42.5)]
        [InlineData("[%-10f]", "[-42.000000]", -42)]
        [InlineData("[%-10f]", "[-42.500000]", -42.5)]
        [InlineData("[%010f]", "[-42.000000]", -42)]
        [InlineData("[%010f]", "[-42.500000]", -42.5)]
        [InlineData("[%-010f]", "[-42.000000]", -42)]
        [InlineData("[%-010f]", "[-42.500000]", -42.5)]
        [InlineData("[%+f]", "[-42.000000]", -42)]
        [InlineData("[%+f]", "[-42.500000]", -42.5)]
        [InlineData("[%+10f]", "[-42.000000]", -42)]
        [InlineData("[%+10f]", "[-42.500000]", -42.5)]
        [InlineData("[%+-10f]", "[-42.000000]", -42)]
        [InlineData("[%+-10f]", "[-42.500000]", -42.5)]
        [InlineData("[%+010f]", "[-42.000000]", -42)]
        [InlineData("[%+010f]", "[-42.500000]", -42.5)]
        [InlineData("[%+-010f]", "[-42.000000]", -42)]
        [InlineData("[%+-010f]", "[-42.500000]", -42.5)]
        [InlineData("[%.2f]", "[42.00]", 42)]
        [InlineData("[%.2f]", "[42.50]", 42.5)]
        [InlineData("[%10.2f]", "[     42.00]", 42)]
        [InlineData("[%10.2f]", "[     42.50]", 42.5)]
        [InlineData("[%-10.2f]", "[42.00     ]", 42)]
        [InlineData("[%-10.2f]", "[42.50     ]", 42.5)]
        [InlineData("[%010.2f]", "[0000042.00]", 42)]
        [InlineData("[%010.2f]", "[0000042.50]", 42.5)]
        [InlineData("[%-010.2f]", "[42.00     ]", 42)]
        [InlineData("[%-010.2f]", "[42.50     ]", 42.5)]
        [InlineData("[%+.2f]", "[+42.00]", 42)]
        [InlineData("[%+.2f]", "[+42.50]", 42.5)]
        [InlineData("[%+10.2f]", "[    +42.00]", 42)]
        [InlineData("[%+10.2f]", "[    +42.50]", 42.5)]
        [InlineData("[%+-10.2f]", "[+42.00    ]", 42)]
        [InlineData("[%+-10.2f]", "[+42.50    ]", 42.5)]
        [InlineData("[%+010.2f]", "[+000042.00]", 42)]
        [InlineData("[%+010.2f]", "[+000042.50]", 42.5)]
        [InlineData("[%+-010.2f]", "[+42.00    ]", 42)]
        [InlineData("[%+-010.2f]", "[+42.50    ]", 42.5)]
        [InlineData("[%.2f]", "[-42.00]", -42)]
        [InlineData("[%.2f]", "[-42.50]", -42.5)]
        [InlineData("[%10.2f]", "[    -42.00]", -42)]
        [InlineData("[%10.2f]", "[    -42.50]", -42.5)]
        [InlineData("[%-10.2f]", "[-42.00    ]", -42)]
        [InlineData("[%-10.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%010.2f]", "[-000042.00]", -42)]
        [InlineData("[%010.2f]", "[-000042.50]", -42.5)]
        [InlineData("[%-010.2f]", "[-42.00    ]", -42)]
        [InlineData("[%-010.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%+.2f]", "[-42.00]", -42)]
        [InlineData("[%+.2f]", "[-42.50]", -42.5)]
        [InlineData("[%+10.2f]", "[    -42.00]", -42)]
        [InlineData("[%+10.2f]", "[    -42.50]", -42.5)]
        [InlineData("[%+-10.2f]", "[-42.00    ]", -42)]
        [InlineData("[%+-10.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%+010.2f]", "[-000042.00]", -42)]
        [InlineData("[%+010.2f]", "[-000042.50]", -42.5)]
        [InlineData("[%+-010.2f]", "[-42.00    ]", -42)]
        [InlineData("[%+-010.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%'.2f]", "[1,234.57]", 1234.567)]
        public void CStyleFloatFormatSpecifier_CallSPrintfWithOneDouble_GetExpectedString(string format, string expectedResult, double value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%f]", "[42.000000]", 42)]
        [InlineData("[%f]", "[42.500000]", 42.5)]
        [InlineData("[%10f]", "[ 42.000000]", 42)]
        [InlineData("[%10f]", "[ 42.500000]", 42.5)]
        [InlineData("[%-10f]", "[42.000000 ]", 42)]
        [InlineData("[%-10f]", "[42.500000 ]", 42.5)]
        [InlineData("[%010f]", "[042.000000]", 42)]
        [InlineData("[%010f]", "[042.500000]", 42.5)]
        [InlineData("[%-010f]", "[42.000000 ]", 42)]
        [InlineData("[%-010f]", "[42.500000 ]", 42.5)]
        [InlineData("[%+f]", "[+42.000000]", 42)]
        [InlineData("[%+f]", "[+42.500000]", 42.5)]
        [InlineData("[%+10f]", "[+42.000000]", 42)]
        [InlineData("[%+10f]", "[+42.500000]", 42.5)]
        [InlineData("[%+-10f]", "[+42.000000]", 42)]
        [InlineData("[%+-10f]", "[+42.500000]", 42.5)]
        [InlineData("[%+010f]", "[+42.000000]", 42)]
        [InlineData("[%+010f]", "[+42.500000]", 42.5)]
        [InlineData("[%+-010f]", "[+42.000000]", 42)]
        [InlineData("[%+-010f]", "[+42.500000]", 42.5)]
        [InlineData("[%f]", "[-42.000000]", -42)]
        [InlineData("[%f]", "[-42.500000]", -42.5)]
        [InlineData("[%10f]", "[-42.000000]", -42)]
        [InlineData("[%10f]", "[-42.500000]", -42.5)]
        [InlineData("[%-10f]", "[-42.000000]", -42)]
        [InlineData("[%-10f]", "[-42.500000]", -42.5)]
        [InlineData("[%010f]", "[-42.000000]", -42)]
        [InlineData("[%010f]", "[-42.500000]", -42.5)]
        [InlineData("[%-010f]", "[-42.000000]", -42)]
        [InlineData("[%-010f]", "[-42.500000]", -42.5)]
        [InlineData("[%+f]", "[-42.000000]", -42)]
        [InlineData("[%+f]", "[-42.500000]", -42.5)]
        [InlineData("[%+10f]", "[-42.000000]", -42)]
        [InlineData("[%+10f]", "[-42.500000]", -42.5)]
        [InlineData("[%+-10f]", "[-42.000000]", -42)]
        [InlineData("[%+-10f]", "[-42.500000]", -42.5)]
        [InlineData("[%+010f]", "[-42.000000]", -42)]
        [InlineData("[%+010f]", "[-42.500000]", -42.5)]
        [InlineData("[%+-010f]", "[-42.000000]", -42)]
        [InlineData("[%+-010f]", "[-42.500000]", -42.5)]
        [InlineData("[%.2f]", "[42.00]", 42)]
        [InlineData("[%.2f]", "[42.50]", 42.5)]
        [InlineData("[%10.2f]", "[     42.00]", 42)]
        [InlineData("[%10.2f]", "[     42.50]", 42.5)]
        [InlineData("[%-10.2f]", "[42.00     ]", 42)]
        [InlineData("[%-10.2f]", "[42.50     ]", 42.5)]
        [InlineData("[%010.2f]", "[0000042.00]", 42)]
        [InlineData("[%010.2f]", "[0000042.50]", 42.5)]
        [InlineData("[%-010.2f]", "[42.00     ]", 42)]
        [InlineData("[%-010.2f]", "[42.50     ]", 42.5)]
        [InlineData("[%+.2f]", "[+42.00]", 42)]
        [InlineData("[%+.2f]", "[+42.50]", 42.5)]
        [InlineData("[%+10.2f]", "[    +42.00]", 42)]
        [InlineData("[%+10.2f]", "[    +42.50]", 42.5)]
        [InlineData("[%+-10.2f]", "[+42.00    ]", 42)]
        [InlineData("[%+-10.2f]", "[+42.50    ]", 42.5)]
        [InlineData("[%+010.2f]", "[+000042.00]", 42)]
        [InlineData("[%+010.2f]", "[+000042.50]", 42.5)]
        [InlineData("[%+-010.2f]", "[+42.00    ]", 42)]
        [InlineData("[%+-010.2f]", "[+42.50    ]", 42.5)]
        [InlineData("[%.2f]", "[-42.00]", -42)]
        [InlineData("[%.2f]", "[-42.50]", -42.5)]
        [InlineData("[%10.2f]", "[    -42.00]", -42)]
        [InlineData("[%10.2f]", "[    -42.50]", -42.5)]
        [InlineData("[%-10.2f]", "[-42.00    ]", -42)]
        [InlineData("[%-10.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%010.2f]", "[-000042.00]", -42)]
        [InlineData("[%010.2f]", "[-000042.50]", -42.5)]
        [InlineData("[%-010.2f]", "[-42.00    ]", -42)]
        [InlineData("[%-010.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%+.2f]", "[-42.00]", -42)]
        [InlineData("[%+.2f]", "[-42.50]", -42.5)]
        [InlineData("[%+10.2f]", "[    -42.00]", -42)]
        [InlineData("[%+10.2f]", "[    -42.50]", -42.5)]
        [InlineData("[%+-10.2f]", "[-42.00    ]", -42)]
        [InlineData("[%+-10.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%+010.2f]", "[-000042.00]", -42)]
        [InlineData("[%+010.2f]", "[-000042.50]", -42.5)]
        [InlineData("[%+-010.2f]", "[-42.00    ]", -42)]
        [InlineData("[%+-010.2f]", "[-42.50    ]", -42.5)]
        public void CStyleFloatFormatSpecifier_CallSPrintfWithOneFloat_GetExpectedString(string format, string expectedResult, float value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%f]", "[42.000000]", 42)]
        [InlineData("[%f]", "[42.500000]", 42.5)]
        [InlineData("[%10f]", "[ 42.000000]", 42)]
        [InlineData("[%10f]", "[ 42.500000]", 42.5)]
        [InlineData("[%-10f]", "[42.000000 ]", 42)]
        [InlineData("[%-10f]", "[42.500000 ]", 42.5)]
        [InlineData("[%010f]", "[042.000000]", 42)]
        [InlineData("[%010f]", "[042.500000]", 42.5)]
        [InlineData("[%-010f]", "[42.000000 ]", 42)]
        [InlineData("[%-010f]", "[42.500000 ]", 42.5)]
        [InlineData("[%+f]", "[+42.000000]", 42)]
        [InlineData("[%+f]", "[+42.500000]", 42.5)]
        [InlineData("[%+10f]", "[+42.000000]", 42)]
        [InlineData("[%+10f]", "[+42.500000]", 42.5)]
        [InlineData("[%+-10f]", "[+42.000000]", 42)]
        [InlineData("[%+-10f]", "[+42.500000]", 42.5)]
        [InlineData("[%+010f]", "[+42.000000]", 42)]
        [InlineData("[%+010f]", "[+42.500000]", 42.5)]
        [InlineData("[%+-010f]", "[+42.000000]", 42)]
        [InlineData("[%+-010f]", "[+42.500000]", 42.5)]
        [InlineData("[%f]", "[-42.000000]", -42)]
        [InlineData("[%f]", "[-42.500000]", -42.5)]
        [InlineData("[%10f]", "[-42.000000]", -42)]
        [InlineData("[%10f]", "[-42.500000]", -42.5)]
        [InlineData("[%-10f]", "[-42.000000]", -42)]
        [InlineData("[%-10f]", "[-42.500000]", -42.5)]
        [InlineData("[%010f]", "[-42.000000]", -42)]
        [InlineData("[%010f]", "[-42.500000]", -42.5)]
        [InlineData("[%-010f]", "[-42.000000]", -42)]
        [InlineData("[%-010f]", "[-42.500000]", -42.5)]
        [InlineData("[%+f]", "[-42.000000]", -42)]
        [InlineData("[%+f]", "[-42.500000]", -42.5)]
        [InlineData("[%+10f]", "[-42.000000]", -42)]
        [InlineData("[%+10f]", "[-42.500000]", -42.5)]
        [InlineData("[%+-10f]", "[-42.000000]", -42)]
        [InlineData("[%+-10f]", "[-42.500000]", -42.5)]
        [InlineData("[%+010f]", "[-42.000000]", -42)]
        [InlineData("[%+010f]", "[-42.500000]", -42.5)]
        [InlineData("[%+-010f]", "[-42.000000]", -42)]
        [InlineData("[%+-010f]", "[-42.500000]", -42.5)]
        [InlineData("[%.2f]", "[42.00]", 42)]
        [InlineData("[%.2f]", "[42.50]", 42.5)]
        [InlineData("[%10.2f]", "[     42.00]", 42)]
        [InlineData("[%10.2f]", "[     42.50]", 42.5)]
        [InlineData("[%-10.2f]", "[42.00     ]", 42)]
        [InlineData("[%-10.2f]", "[42.50     ]", 42.5)]
        [InlineData("[%010.2f]", "[0000042.00]", 42)]
        [InlineData("[%010.2f]", "[0000042.50]", 42.5)]
        [InlineData("[%-010.2f]", "[42.00     ]", 42)]
        [InlineData("[%-010.2f]", "[42.50     ]", 42.5)]
        [InlineData("[%+.2f]", "[+42.00]", 42)]
        [InlineData("[%+.2f]", "[+42.50]", 42.5)]
        [InlineData("[%+10.2f]", "[    +42.00]", 42)]
        [InlineData("[%+10.2f]", "[    +42.50]", 42.5)]
        [InlineData("[%+-10.2f]", "[+42.00    ]", 42)]
        [InlineData("[%+-10.2f]", "[+42.50    ]", 42.5)]
        [InlineData("[%+010.2f]", "[+000042.00]", 42)]
        [InlineData("[%+010.2f]", "[+000042.50]", 42.5)]
        [InlineData("[%+-010.2f]", "[+42.00    ]", 42)]
        [InlineData("[%+-010.2f]", "[+42.50    ]", 42.5)]
        [InlineData("[%.2f]", "[-42.00]", -42)]
        [InlineData("[%.2f]", "[-42.50]", -42.5)]
        [InlineData("[%10.2f]", "[    -42.00]", -42)]
        [InlineData("[%10.2f]", "[    -42.50]", -42.5)]
        [InlineData("[%-10.2f]", "[-42.00    ]", -42)]
        [InlineData("[%-10.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%010.2f]", "[-000042.00]", -42)]
        [InlineData("[%010.2f]", "[-000042.50]", -42.5)]
        [InlineData("[%-010.2f]", "[-42.00    ]", -42)]
        [InlineData("[%-010.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%+.2f]", "[-42.00]", -42)]
        [InlineData("[%+.2f]", "[-42.50]", -42.5)]
        [InlineData("[%+10.2f]", "[    -42.00]", -42)]
        [InlineData("[%+10.2f]", "[    -42.50]", -42.5)]
        [InlineData("[%+-10.2f]", "[-42.00    ]", -42)]
        [InlineData("[%+-10.2f]", "[-42.50    ]", -42.5)]
        [InlineData("[%+010.2f]", "[-000042.00]", -42)]
        [InlineData("[%+010.2f]", "[-000042.50]", -42.5)]
        [InlineData("[%+-010.2f]", "[-42.00    ]", -42)]
        [InlineData("[%+-010.2f]", "[-42.50    ]", -42.5)]
        public void CStyleFloatFormatSpecifier_CallSPrintfWithOneDecimal_GetExpectedString(string format, string expectedResult, decimal value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%e]", "[4.200000e+001]", 42)]
        [InlineData("[%e]", "[4.250000e+001]", 42.5)]
        [InlineData("[%20e]", "[       4.200000e+001]", 42)]
        [InlineData("[%20e]", "[       4.250000e+001]", 42.5)]
        [InlineData("[%-20e]", "[4.200000e+001       ]", 42)]
        [InlineData("[%-20e]", "[4.250000e+001       ]", 42.5)]
        [InlineData("[%020e]", "[00000004.200000e+001]", 42)]
        [InlineData("[%020e]", "[00000004.250000e+001]", 42.5)]
        [InlineData("[%-020e]", "[4.200000e+001       ]", 42)]
        [InlineData("[%-020e]", "[4.250000e+001       ]", 42.5)]
        [InlineData("[%+E]", "[+4.200000E+001]", 42)]
        [InlineData("[%+E]", "[+4.250000E+001]", 42.5)]
        [InlineData("[%+20E]", "[      +4.200000E+001]", 42)]
        [InlineData("[%+20E]", "[      +4.250000E+001]", 42.5)]
        [InlineData("[%+-20E]", "[+4.200000E+001      ]", 42)]
        [InlineData("[%+-20E]", "[+4.250000E+001      ]", 42.5)]
        [InlineData("[%+020E]", "[+0000004.200000E+001]", 42)]
        [InlineData("[%+020E]", "[+0000004.250000E+001]", 42.5)]
        [InlineData("[%+-020E]", "[+4.200000E+001      ]", 42)]
        [InlineData("[%+-020E]", "[+4.250000E+001      ]", 42.5)]
        [InlineData("[%e]", "[-4.200000e+001]", -42)]
        [InlineData("[%e]", "[-4.250000e+001]", -42.5)]
        [InlineData("[%20e]", "[      -4.200000e+001]", -42)]
        [InlineData("[%20e]", "[      -4.250000e+001]", -42.5)]
        [InlineData("[%-20e]", "[-4.200000e+001      ]", -42)]
        [InlineData("[%-20e]", "[-4.250000e+001      ]", -42.5)]
        [InlineData("[%020e]", "[-0000004.200000e+001]", -42)]
        [InlineData("[%020e]", "[-0000004.250000e+001]", -42.5)]
        [InlineData("[%-020e]", "[-4.200000e+001      ]", -42)]
        [InlineData("[%-020e]", "[-4.250000e+001      ]", -42.5)]
        [InlineData("[%+e]", "[-4.200000e+001]", -42)]
        [InlineData("[%+e]", "[-4.250000e+001]", -42.5)]
        [InlineData("[%+20e]", "[      -4.200000e+001]", -42)]
        [InlineData("[%+20e]", "[      -4.250000e+001]", -42.5)]
        [InlineData("[%+-20e]", "[-4.200000e+001      ]", -42)]
        [InlineData("[%+-20e]", "[-4.250000e+001      ]", -42.5)]
        [InlineData("[%+020e]", "[-0000004.200000e+001]", -42)]
        [InlineData("[%+020e]", "[-0000004.250000e+001]", -42.5)]
        [InlineData("[%+-020e]", "[-4.200000e+001      ]", -42)]
        [InlineData("[%+-020e]", "[-4.250000e+001      ]", -42.5)]
        [InlineData("[%.2e]", "[4.20e+001]", 42)]
        [InlineData("[%.2e]", "[4.25e+001]", 42.5)]
        [InlineData("[%20.2e]", "[           4.20e+001]", 42)]
        [InlineData("[%20.2e]", "[           4.25e+001]", 42.5)]
        [InlineData("[%-20.2e]", "[4.20e+001           ]", 42)]
        [InlineData("[%-20.2e]", "[4.25e+001           ]", 42.5)]
        [InlineData("[%020.2e]", "[000000000004.20e+001]", 42)]
        [InlineData("[%020.2e]", "[000000000004.25e+001]", 42.5)]
        [InlineData("[%-020.2e]", "[4.20e+001           ]", 42)]
        [InlineData("[%-020.2e]", "[4.25e+001           ]", 42.5)]
        [InlineData("[%+.2E]", "[+4.20E+001]", 42)]
        [InlineData("[%+.2E]", "[+4.25E+001]", 42.5)]
        [InlineData("[%+20.2E]", "[          +4.20E+001]", 42)]
        [InlineData("[%+20.2E]", "[          +4.25E+001]", 42.5)]
        [InlineData("[%+-20.2E]", "[+4.20E+001          ]", 42)]
        [InlineData("[%+-20.2E]", "[+4.25E+001          ]", 42.5)]
        [InlineData("[%+020.2E]", "[+00000000004.20E+001]", 42)]
        [InlineData("[%+020.2E]", "[+00000000004.25E+001]", 42.5)]
        [InlineData("[%+-020.2E]", "[+4.20E+001          ]", 42)]
        [InlineData("[%+-020.2E]", "[+4.25E+001          ]", 42.5)]
        [InlineData("[%.2e]", "[-4.20e+001]", -42)]
        [InlineData("[%.2e]", "[-4.25e+001]", -42.5)]
        [InlineData("[%20.2e]", "[          -4.20e+001]", -42)]
        [InlineData("[%20.2e]", "[          -4.25e+001]", -42.5)]
        [InlineData("[%-20.2e]", "[-4.20e+001          ]", -42)]
        [InlineData("[%-20.2e]", "[-4.25e+001          ]", -42.5)]
        [InlineData("[%020.2e]", "[-00000000004.20e+001]", -42)]
        [InlineData("[%020.2e]", "[-00000000004.25e+001]", -42.5)]
        [InlineData("[%-020.2e]", "[-4.20e+001          ]", -42)]
        [InlineData("[%-020.2e]", "[-4.25e+001          ]", -42.5)]
        [InlineData("[%+.2e]", "[-4.20e+001]", -42)]
        [InlineData("[%+.2e]", "[-4.25e+001]", -42.5)]
        [InlineData("[%+20.2e]", "[          -4.20e+001]", -42)]
        [InlineData("[%+20.2e]", "[          -4.25e+001]", -42.5)]
        [InlineData("[%+-20.2e]", "[-4.20e+001          ]", -42)]
        [InlineData("[%+-20.2e]", "[-4.25e+001          ]", -42.5)]
        [InlineData("[%+020.2e]", "[-00000000004.20e+001]", -42)]
        [InlineData("[%+020.2e]", "[-00000000004.25e+001]", -42.545)]
        [InlineData("[%+-020.2e]", "[-4.20e+001          ]", -42)]
        [InlineData("[%+-020.2e]", "[-4.26e+001          ]", -42.555)]
        public void CStyleScientificNotationFormatSpecifier_CallSprintfWithOneDouble_GetExpectedString(string format, string expectedResult, double value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%c]", "[A]", 'A')]
        [InlineData("[%c]", "[A]", (char)65)]
        public void CStyleCharacterFormatSpecifier_CallSPrintfWithCharacter_GetExpectedString(string format, string expectedResult, char character)
        {
            var actualResult = StringUtilities.SPrintf(format, character);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void CStyleCharacterFormatSpecifier_CallSPrintfWithString_GetExpectedString()
        {
            var actualResult = StringUtilities.SPrintf("[%c]", "A Test");
            Assert.Equal("[A]", actualResult);
        }

        [Fact]
        public void CStyleCharacterFormatSpecifier_CallSPrintfWithInt_GetExpectedString()
        {
            var actualResult = StringUtilities.SPrintf("[%c]", 65);
            Assert.Equal("[A]", actualResult);
        }

        [Theory]
        [InlineData("[%s]", "[This is a test]", "This is a test")]
        [InlineData("[%s]", "[A test with %]", "A test with %")]
        [InlineData("[%s]", "[A test with %s inside]", "A test with %s inside")]
        [InlineData("[%% %s %%]", "[% % Another test % %]", "% Another test %")]
        [InlineData("[%20s]", "[       a long string]", "a long string")]
        [InlineData("[%-20s]", "[a long string       ]", "a long string")]
        [InlineData("[%020s]", "[0000000a long string]", "a long string")]
        [InlineData("[%-020s]", "[a long string       ]", "a long string")]
        [InlineData("[%.10s]", "[This is a ]", "This is a shortened string")]
        [InlineData("[%20.10s]", "[          This is a ]", "This is a shortened string")]
        [InlineData("[%-20.10s]", "[This is a           ]", "This is a shortened string")]
        [InlineData("[%020.10s]", "[0000000000This is a ]", "This is a shortened string")]
        [InlineData("[%-020.10s]", "[This is a           ]", "This is a shortened string")]
        public void CStyleStringFormatSpecifier_CallSPrintfWithOneString_GetExpectedString(string format, string expectedResult, string value)
        {
            var actualResult = StringUtilities.SPrintf(format, value);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("[%%]", "[%]")]
        [InlineData("[%n]", "[1]")]
        [InlineData("[%%n shows the number of processed chars so far (%010n)]", "[%n shows the number of processed chars so far (0000000048)]")]
        public void CStyleSpecialFormatters_CallSPrintf_GetExpectedString(string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void CStylePositionIndexFormatSpecifier_CallSPrintfWithTwoInts_GetExpectedString()
        {
            var actualResult = StringUtilities.SPrintf("[%2$d %1$#x %1$d]", 49, 11);
            Assert.Equal("[11 0x31 49]", actualResult);
        }

        #endregion // C-Style Formatting Tests

        private static string HtmlDecode(string encodedHtmlString)
        {
            var decodedString = string.Empty;
            if (!string.IsNullOrEmpty(encodedHtmlString))
            {
                decodedString = HttpUtility.HtmlDecode(encodedHtmlString);
            }
            return decodedString;
        }

        private static string HtmlStripTagsAndDecode(string encodedHtmlString)
        {
            var decodedString = string.Empty;
            if (!string.IsNullOrEmpty(encodedHtmlString))
            {
                decodedString = Regex.Replace(encodedHtmlString, "<.*?>", string.Empty);
                decodedString = HttpUtility.HtmlDecode(decodedString);
            }
            return decodedString;
        }

        private static string HtmlEncode(string stringToEncode)
        {
            var decodedString = string.Empty;
            if (!string.IsNullOrEmpty(stringToEncode))
            {
                decodedString = HttpUtility.HtmlEncode(stringToEncode);
            }
            return decodedString;
        }
    }
}

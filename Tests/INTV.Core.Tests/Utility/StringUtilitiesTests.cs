// <copyright file="StringUtilitiesTests.cs" company="INTV Funhouse">
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

        #region C-Style Formatting Tests

        /// <summary>
        /// Tests for the SPrintf utility.
        /// </summary>
        /// <param name="value">The value to send to SPrintf.</param>
        /// <param name="format">The C-style format specifier to use.</param>
        /// <param name="expectedResult">The expected string.</param>
        /// <remarks>Note that only those aspects of C-style formatting are tested that are useful in VINTage. This restricts
        /// the tests to a narrow subset of the gamut of possibilities in C-style formatting. In practicality, this means
        /// the format strings used in error_db.yaml in the INTV.LtoFlash plugin.</remarks>
        [Theory]
        [InlineData(48879, "%.4X", "BEEF")]
        [InlineData(3565, "%.3X", "DED")]
        [InlineData(112, "%.2X", "70")]
        [InlineData(60, "%d", "60")]
        public void CStyleFormatSpecifier_CallSPrintfWithOneArgument_GetExpectedString(int value, string format, string expectedResult)
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
        public void CStyleFormatSpecifier_CallSPrintfWithTwoArguments_GetExpectedString(int firstValue, int secondValue, string format, string expectedResult)
        {
            var actualResult = StringUtilities.SPrintf(format, firstValue, secondValue);
            Assert.Equal(expectedResult, actualResult);
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

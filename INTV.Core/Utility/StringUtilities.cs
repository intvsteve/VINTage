// <copyright file="StringUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using System.Text;
using System.Text.RegularExpressions;

namespace INTV.Core.Utility
{
    /// <summary>
    /// Useful functions for working with strings.
    /// </summary>
    public static class StringUtilities
    {
        private static Func<string, string> _htmlDecoder;
        private static Func<string, string> _htmlEncoder;

        /// <summary>
        /// The replacement character to use for characters not in the GROM character set.
        /// </summary>
        public const char ReplacementCharacter = '~';

        /// <summary>
        /// Given an input string, truncates to a maximum length.
        /// </summary>
        /// <param name="newName">The string to potentially truncate.</param>
        /// <param name="maxLength">The maximum allowed length of the string.</param>
        /// <param name="restrictToGromCharacters">If <c>true</c> then characters that are not part of the GROM character set will be replaced.</param>
        /// <returns>The string, with a maximum length of maxLength.</returns>
        /// <remarks>In the future, perhaps this can have more logic, such as trimming words like 'the' from the beginning, etc.</remarks>
        public static string EnforceNameLength(this string newName, int maxLength, bool restrictToGromCharacters)
        {
            var result = newName;
            if (!string.IsNullOrEmpty(newName))
            {
                if (newName.Length > maxLength)
                {
                    result = newName.Substring(0, maxLength);
                }
                if (restrictToGromCharacters)
                {
                    var characters = result.ToCharArray();
                    for (var i = 0; i < characters.Length; ++i)
                    {
                        if (!INTV.Core.Model.Grom.Characters.Contains(characters[i]))
                        {
                            characters[i] = ReplacementCharacter;
                        }
                    }
                    result = new string(characters);
                }
            }
            return result;
        }

        /// <summary>
        /// Registers a delegate to decode a string that was HTML-encoded.
        /// </summary>
        /// <param name="decoder">The HTML decoder delegate to use.</param>
        public static void RegisterHtmlDecoder(Func<string, string> decoder)
        {
            _htmlDecoder = decoder;
        }

        /// <summary>
        /// Registers a delegate to encode a string for use in HTML.
        /// </summary>
        /// <param name="encoder">The HTML decoder delegate to use.</param>
        public static void RegisterHtmlEncoder(Func<string, string> encoder)
        {
            _htmlEncoder = encoder;
        }

        /// <summary>
        /// Decodes a string that was HTML-encoded.
        /// </summary>
        /// <param name="encodedHtmlString">A string that was HTML-encoded.</param>
        /// <returns>The decoded string.</returns>
        public static string DecodeHtmlString(this string encodedHtmlString)
        {
            var decodedHtmlString = encodedHtmlString;
            if ((_htmlDecoder != null) && !string.IsNullOrEmpty(encodedHtmlString))
            {
                decodedHtmlString = _htmlDecoder(encodedHtmlString);
            }
            return decodedHtmlString;
        }

        /// <summary>
        /// Encodes a string for use in HTML.
        /// </summary>
        /// <param name="rawString">A string that must be HTML-encoded.</param>
        /// <returns>The encoded string.</returns>
        public static string EncodeHtmlString(this string rawString)
        {
            var encodedHtmlString = rawString;
            if ((_htmlEncoder != null) && !string.IsNullOrEmpty(rawString))
            {
                encodedHtmlString = _htmlEncoder(rawString);
            }
            return encodedHtmlString;
        }

        #region C-style Format Specifier Support

        // The code in this region is from http://www.codeproject.com/Articles/19274/A-printf-implementation-in-C
        // It was released under the MIT License:
        // The MIT License (MIT)
        // Copyright (c) 2015 Richard Prinz

        // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
        // documentation files (the "Software"), to deal in the Software without restriction, including without limitation
        // the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
        // and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

        // The above copyright notice and this permission notice shall be included in all copies or substantial portions
        // of the Software.

        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
        // TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
        // THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
        // CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
        // DEALINGS IN THE SOFTWARE.

        /// <summary>
        /// Adapted, limited version of sprintf from Stack Overflow.
        /// </summary>
        /// <param name="format">A C-style format specifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The formatted output string.</returns>
        /// <remarks>Adapted from: http://www.codeproject.com/Articles/19274/A-printf-implementation-in-C</remarks>
        public static string SPrintf(string format, params object[] parameters)
        {
            var formattedStringBuilder = new StringBuilder();
            var regEx = new Regex(@"\%(\d*\$)?([\'\#\-\+ ]*)(\d*)(?:\.(\d+))?([hl])?([dioxXucsfeEgGpn%])");
            ////"%[parameter][flags][width][.precision][length]type"

            var defaultParamIndex = 0;

            // find all format parameters in format string
            formattedStringBuilder.Append(format);
            var match = regEx.Match(formattedStringBuilder.ToString());
            while (match.Success)
            {
                // get parameter index
                var paramIndex = defaultParamIndex;
                if (match.Groups[1] != null && match.Groups[1].Value.Length > 0)
                {
                    string val = match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1);
                    paramIndex = Convert.ToInt32(val) - 1;
                }

                // extract format flags
                var flagAlternate = false;
                var flagLeftToRight = false;
                var flagPositiveSign = false;
                var flagPositiveSpace = false;
                var flagZeroPadding = false;
                var flagGroupThousands = false;
                if (match.Groups[2] != null && match.Groups[2].Value.Length > 0)
                {
                    var flags = match.Groups[2].Value;

                    flagAlternate = flags.IndexOf('#') >= 0;
                    flagLeftToRight = flags.IndexOf('-') >= 0;
                    flagPositiveSign = flags.IndexOf('+') >= 0;
                    flagPositiveSpace = flags.IndexOf(' ') >= 0;
                    flagGroupThousands = flags.IndexOf('\'') >= 0;

                    // positive + indicator overrides a positive space character
                    if (flagPositiveSign && flagPositiveSpace)
                    {
                        flagPositiveSpace = false;
                    }
                }

                // extract field length and padding character
                var paddingCharacter = ' ';
                var fieldLength = int.MinValue;
                if (match.Groups[3] != null && match.Groups[3].Value.Length > 0)
                {
                    fieldLength = Convert.ToInt32(match.Groups[3].Value);
                    flagZeroPadding = match.Groups[3].Value[0] == '0';
                }

                if (flagZeroPadding)
                {
                    paddingCharacter = '0';
                }

                // leftToRight alignment overrides zero padding
                if (flagLeftToRight && flagZeroPadding)
                {
                    flagZeroPadding = false;
                    paddingCharacter = ' ';
                }

                // extract field precision
                var fieldPrecision = int.MinValue;
                if (match.Groups[4] != null && match.Groups[4].Value.Length > 0)
                {
                    fieldPrecision = Convert.ToInt32(match.Groups[4].Value);
                }

                // extract short / long indicator
                var shortLongIndicator = char.MinValue;
                if (match.Groups[5] != null && match.Groups[5].Value.Length > 0)
                {
                    shortLongIndicator = match.Groups[5].Value[0];
                }

                // extract format specifier
                var formatSpecifier = char.MinValue;
                if (match.Groups[6] != null && match.Groups[6].Value.Length > 0)
                {
                    formatSpecifier = match.Groups[6].Value[0];
                }

                // default precision is 6 digits if none is specified except
                if (fieldPrecision == int.MinValue &&
                    formatSpecifier != 's' &&
                    formatSpecifier != 'c' &&
                    char.ToUpper(formatSpecifier) != 'X' &&
                    formatSpecifier != 'o')
                {
                    fieldPrecision = 6;
                }

                // get next value parameter and convert value parameter depending on short / long indicator
                object value = null;
                if ((parameters == null) || (paramIndex >= parameters.Length))
                {
                    value = null;
                }
                else
                {
                    value = parameters[paramIndex];

                    if (shortLongIndicator == 'h')
                    {
                        if (value is int)
                        {
                            value = (short)((int)value);
                        }
                        else if (value is long)
                        {
                            value = (short)((long)value);
                        }
                        else if (value is uint)
                        {
                            value = (ushort)((uint)value);
                        }
                        else if (value is ulong)
                        {
                            value = (ushort)((ulong)value);
                        }
                    }
                    else if (shortLongIndicator == 'l')
                    {
                        if (value is short)
                        {
                            value = (long)((short)value);
                        }
                        else if (value is int)
                        {
                            value = (long)((int)value);
                        }
                        else if (value is ushort)
                        {
                            value = (ulong)((ushort)value);
                        }
                        else if (value is uint)
                        {
                            value = (ulong)((uint)value);
                        }
                    }
                }

                // convert value parameters to a string depending on the formatSpecifier
                var formattedString = string.Empty;
                switch (formatSpecifier)
                {
                    case '%':   // % character
                        formattedString = "%";
                        break;

                    case 'i':   // integer
                    case 'd':   // integer
                        formattedString = FormatNumber(flagGroupThousands ? "n" : "d", flagAlternate, fieldLength, int.MinValue, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'o':   // octal integer - no leading zero
                        formattedString = FormatOct("o", flagAlternate, fieldLength, int.MinValue, flagLeftToRight, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'x':   // hex integer - no leading zero
                        formattedString = FormatHex("x", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'X':   // same as x but with capital hex characters
                        formattedString = FormatHex("X", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'u':   // unsigned integer
                        formattedString = FormatNumber(flagGroupThousands ? "n" : "d", flagAlternate, fieldLength, int.MinValue, flagLeftToRight, false, false, paddingCharacter, ToUnsigned(value));
                        ++defaultParamIndex;
                        break;

                    case 'c':   // character
                        if (IsNumericType(value))
                        {
                            formattedString = Convert.ToChar(value).ToString();
                        }
                        else if (value is char)
                        {
                            formattedString = ((char)value).ToString();
                        }
                        else if (value is string && ((string)value).Length > 0)
                        {
                            formattedString = ((string)value)[0].ToString();
                        }
                        ++defaultParamIndex;
                        break;

                    case 's':   // string
                        formattedString = value.ToString();
                        if (fieldPrecision >= 0)
                        {
                            formattedString = formattedString.Substring(0, fieldPrecision);
                        }
                        if (fieldLength != int.MinValue)
                        {
                            if (flagLeftToRight)
                            {
                                formattedString = formattedString.PadRight(fieldLength, paddingCharacter);
                            }
                            else
                            {
                                formattedString = formattedString.PadLeft(fieldLength, paddingCharacter);
                            }
                        }
                        ++defaultParamIndex;
                        break;
                    case 'f':   // double
                        formattedString = FormatNumber(flagGroupThousands ? "n" : "f", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'e':   // double / exponent
                        formattedString = FormatNumber("e", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'E':   // double / exponent
                        formattedString = FormatNumber("E", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'g':   // double / exponent
                        formattedString = FormatNumber("g", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'G':   // double / exponent
                        formattedString = FormatNumber("G", flagAlternate, fieldLength, fieldPrecision, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, value);
                        ++defaultParamIndex;
                        break;

                    case 'p':   // pointer
                        if (value is IntPtr)
                        {
                            if (IntPtr.Size == sizeof(long))
                            {
                                formattedString = "0x" + ((IntPtr)value).ToInt64().ToString("x");
                            }
                            else
                            {
                                formattedString = "0x" + ((IntPtr)value).ToInt32().ToString("x");
                            }
                        }
                        ++defaultParamIndex;
                        break;

                    case 'n':   // number of characters so far
                        formattedString = FormatNumber("d", flagAlternate, fieldLength, int.MinValue, flagLeftToRight, flagPositiveSign, flagPositiveSpace, paddingCharacter, match.Index);
                        break;

                    default:
                        formattedString = string.Empty;
                        ++defaultParamIndex;
                        break;
                }

                // Replace format parameter with parameter value and start searching for the next format parameter
                // AFTER the position of the current inserted value to prohibit recursive matches if the value also
                // includes a format specifier
                formattedStringBuilder.Remove(match.Index, match.Length);
                formattedStringBuilder.Insert(match.Index, formattedString);
                match = regEx.Match(formattedStringBuilder.ToString(), match.Index + formattedString.Length);
            }

            return formattedStringBuilder.ToString();
        }

        private static string FormatOct(string nativeFormat, bool alternate, int fieldLength, int fieldPrecision, bool leftToRight, char padChar, object value)
        {
            string lengthFormat = "{0" + (fieldLength != int.MinValue ? "," + (leftToRight ? "-" : string.Empty) + fieldLength.ToString() : string.Empty) + "}";

            string formattedNumberString = string.Empty;
            if (IsNumericType(value))
            {
                formattedNumberString = Convert.ToString(UnboxToLong(value, true), 8);

                if (leftToRight || padChar == ' ')
                {
                    if (alternate && formattedNumberString != "0")
                    {
                        formattedNumberString = "0" + formattedNumberString;
                    }
                    formattedNumberString = string.Format(lengthFormat, formattedNumberString);
                }
                else
                {
                    if (fieldLength != int.MinValue)
                    {
                        formattedNumberString = formattedNumberString.PadLeft(fieldLength - ((alternate && (formattedNumberString != "0")) ? 1 : 0), padChar);
                    }
                    if (alternate && formattedNumberString != "0")
                    {
                        formattedNumberString = "0" + formattedNumberString;
                    }
                }
            }

            return formattedNumberString;
        }

        private static string FormatHex(string nativeFormat, bool alternate, int fieldLength, int fieldPrecision, bool leftToRight, char padChar, object value)
        {
            string lengthFormat = "{0" + (fieldLength != int.MinValue ? "," + (leftToRight ? "-" : string.Empty) + fieldLength.ToString() : string.Empty) + "}";
            string numberFormat = "{0:" + nativeFormat + (fieldPrecision != int.MinValue ? fieldPrecision.ToString() : string.Empty) + "}";

            string formattedNumberString = string.Empty;
            if (IsNumericType(value))
            {
                formattedNumberString = string.Format(numberFormat, value);

                if (leftToRight || padChar == ' ')
                {
                    if (alternate)
                    {
                        formattedNumberString = (nativeFormat == "x" ? "0x" : "0X") + formattedNumberString;
                    }
                    formattedNumberString = string.Format(lengthFormat, formattedNumberString);
                }
                else
                {
                    if (fieldLength != int.MinValue)
                    {
                        formattedNumberString = formattedNumberString.PadLeft(fieldLength - (alternate ? 2 : 0), padChar);
                    }
                    if (alternate)
                    {
                        formattedNumberString = (nativeFormat == "x" ? "0x" : "0X") + formattedNumberString;
                    }
                }
            }

            return formattedNumberString;
        }

        private static string FormatNumber(string nativeFormat, bool alternate, int fieldLength, int fieldPrecision, bool leftToRight, bool positiveSign, bool positiveSpace, char padChar, object value)
        {
            string lengthFormat = "{0" + (fieldLength != int.MinValue ? "," + (leftToRight ? "-" : string.Empty) + fieldLength.ToString() : string.Empty) + "}";
            string numberFormat = "{0:" + nativeFormat + (fieldPrecision != int.MinValue ? fieldPrecision.ToString() : "0") + "}";

            string formattedNumberString = string.Empty;
            if (IsNumericType(value))
            {
                formattedNumberString = string.Format(numberFormat, value);

                if (leftToRight || padChar == ' ')
                {
                    if (IsPositive(value, true))
                    {
                        formattedNumberString = (positiveSign ? "+" : (positiveSpace ? " " : string.Empty)) + formattedNumberString;
                    }
                    formattedNumberString = string.Format(lengthFormat, formattedNumberString);
                }
                else
                {
                    if (formattedNumberString.StartsWith("-"))
                    {
                        formattedNumberString = formattedNumberString.Substring(1);
                    }
                    if (fieldLength != int.MinValue)
                    {
                        formattedNumberString = formattedNumberString.PadLeft(fieldLength - 1, padChar);
                    }
                    if (IsPositive(value, true))
                    {
                        formattedNumberString = (positiveSign ? "+" : (positiveSpace ? " " : (fieldLength != int.MinValue ? padChar.ToString() : string.Empty))) + formattedNumberString;
                    }
                    else
                    {
                        formattedNumberString = "-" + formattedNumberString;
                    }
                }
            }

            return formattedNumberString;
        }

        /// <summary>
        /// Determines whether the specified value is of numeric type.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <returns><c>true</c> if o is a numeric type; otherwise, <c>false</c>.</returns>
        private static bool IsNumericType(object o)
        {
            return o is byte ||
                o is sbyte ||
                o is short ||
                o is ushort ||
                o is int ||
                o is uint ||
                o is long ||
                o is ulong ||
                o is float ||
                o is double ||
                o is decimal;
        }

        /// <summary>
        /// Determines whether the specified value is positive.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="zeroIsPositive">if set to <c>true</c> treats 0 as positive.</param>
        /// <returns><c>true</c> if the specified value is positive; otherwise, <c>false</c>.</returns>
        private static bool IsPositive(object value, bool zeroIsPositive)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                    return zeroIsPositive ? (sbyte)value >= 0 : (sbyte)value > 0;
                case TypeCode.Int16:
                    return zeroIsPositive ? (short)value >= 0 : (short)value > 0;
                case TypeCode.Int32:
                    return zeroIsPositive ? (int)value >= 0 : (int)value > 0;
                case TypeCode.Int64:
                    return zeroIsPositive ? (long)value >= 0 : (long)value > 0;
                case TypeCode.Single:
                    return zeroIsPositive ? (float)value >= 0 : (float)value > 0;
                case TypeCode.Double:
                    return zeroIsPositive ? (double)value >= 0 : (double)value > 0;
                case TypeCode.Decimal:
                    return zeroIsPositive ? (decimal)value >= 0 : (decimal)value > 0;
                case TypeCode.Byte:
                    return zeroIsPositive ? true : (byte)value > 0;
                case TypeCode.UInt16:
                    return zeroIsPositive ? true : (ushort)value > 0;
                case TypeCode.UInt32:
                    return zeroIsPositive ? true : (uint)value > 0;
                case TypeCode.UInt64:
                    return zeroIsPositive ? true : (ulong)value > 0;
                case TypeCode.Char:
                    return zeroIsPositive ? true : (char)value != '\0';
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts the specified values boxed type to its corresponding unsigned type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A boxed numeric object whose type is unsigned.</returns>
        private static object ToUnsigned(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                    return (byte)((sbyte)value);
                case TypeCode.Int16:
                    return (ushort)((short)value);
                case TypeCode.Int32:
                    return (uint)((int)value);
                case TypeCode.Int64:
                    return (ulong)((long)value);
                case TypeCode.Byte:
                    return value;
                case TypeCode.UInt16:
                    return value;
                case TypeCode.UInt32:
                    return value;
                case TypeCode.UInt64:
                    return value;
                case TypeCode.Single:
                    return (uint)((float)value);
                case TypeCode.Double:
                    return (ulong)((double)value);
                case TypeCode.Decimal:
                    return (ulong)((decimal)value);
                default:
                    return null;
            }
        }

        private static long UnboxToLong(object value, bool round)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                    return (long)((sbyte)value);
                case TypeCode.Int16:
                    return (long)((short)value);
                case TypeCode.Int32:
                    return (long)((int)value);
                case TypeCode.Int64:
                    return (long)value;
                case TypeCode.Byte:
                    return (long)((byte)value);
                case TypeCode.UInt16:
                    return (long)((ushort)value);
                case TypeCode.UInt32:
                    return (long)((uint)value);
                case TypeCode.UInt64:
                    return (long)((ulong)value);
                case TypeCode.Single:
                    return round ? (long)Math.Round((float)value) : (long)((float)value);
                case TypeCode.Double:
                    return round ? (long)Math.Round((double)value) : (long)((double)value);
                case TypeCode.Decimal:
                    return round ? (long)Math.Round((decimal)value) : (long)((decimal)value);
                default:
                    return 0;
            }
        }

        #endregion // C-style Format Specifier Support
    }
}

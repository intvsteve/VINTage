// <copyright file="MetadataHelpers.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Helper methods for working with ROM metadata.
    /// </summary>
    public static class MetadataHelpers
    {
        /// <summary>
        /// Parses a date / time value from ROM metadata.
        /// </summary>
        /// <param name="reader">A binary reader to use to get the data.</param>
        /// <param name="payloadLength">Payload length of the metadata to parse, in bytes.</param>
        /// <returns>A MetadataDateTime that contains the date and time parsed from metadata, as well as flags indicating date and time field validity and specifics.</returns>
        public static MetadataDateTime ParseDateTimeFromMetadata(this INTV.Core.Utility.BinaryReader reader, uint payloadLength)
        {
            var remainingPayload = (int)payloadLength;
            var dateTimeFlags = MetadataDateTimeFlags.None;

            var year = MetadataDateTime.DefaultYear;
            if (remainingPayload > 0)
            {
                dateTimeFlags |= MetadataDateTimeFlags.Year;
                year = 1900 + reader.ReadByte();
                --remainingPayload;
            }

            var month = MetadataDateTime.DefaultMonth;
            if (remainingPayload > 0)
            {
                month = reader.ReadByte();
                --remainingPayload;
                var monthRange = new Range<int>(1, 12);
                if (!monthRange.IsValueInRange(month))
                {
                    month = MetadataDateTime.DefaultMonth;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.Month;
                }
            }

            var day = MetadataDateTime.DefaultDay;
            if (remainingPayload > 0)
            {
                day = reader.ReadByte();
                --remainingPayload;
                var dayRange = new Range<int>(1, DateTime.DaysInMonth(year, month));
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Month) || !dayRange.IsValueInRange(day))
                {
                    day = MetadataDateTime.DefaultDay;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.Day;
                }
            }

            var hour = MetadataDateTime.DefaultHour;
            if (remainingPayload > 0)
            {
                hour = reader.ReadByte();
                --remainingPayload;
                var hourRange = new Range<int>(0, 23);
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Day) || !hourRange.IsValueInRange(hour))
                {
                    hour = MetadataDateTime.DefaultHour;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.Hour;
                }
            }

            var minute = MetadataDateTime.DefaultMinute;
            if (remainingPayload > 0)
            {
                minute = reader.ReadByte();
                --remainingPayload;
                var minuteRange = new Range<int>(0, 59);
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Hour) || !minuteRange.IsValueInRange(minute))
                {
                    minute = MetadataDateTime.DefaultMinute;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.Minute;
                }
            }

            var second = MetadataDateTime.DefaultSecond;
            if (remainingPayload > 0)
            {
                second = reader.ReadByte();
                --remainingPayload;
                if (dateTimeFlags.HasFlag(MetadataDateTimeFlags.Minute) && (second == 60))
                {
                    dateTimeFlags |= MetadataDateTimeFlags.LeapSecond | MetadataDateTimeFlags.Second;
                    second = 59;
                }
                else if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Minute) || (second > 59))
                {
                    second = MetadataDateTime.DefaultSecond;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.Second;
                }
            }

            var utcOffsetHours = MetadataDateTime.DefaultUtcOffsetHours;
            if (remainingPayload > 0)
            {
                utcOffsetHours = reader.ReadSByte();
                --remainingPayload;
                var offsetHoursRange = new Range<int>(-12, 12);
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Second) || !offsetHoursRange.IsValueInRange(utcOffsetHours))
                {
                    utcOffsetHours = MetadataDateTime.DefaultUtcOffsetHours;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.UtcOffset;
                }
            }

            var utcOffsetMinutes = MetadataDateTime.DefaultUtcOffsetMinutes;
            if (remainingPayload > 0)
            {
                utcOffsetMinutes = reader.ReadByte();
                --remainingPayload;
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.UtcOffset) || (utcOffsetMinutes > 59))
                {
                    utcOffsetMinutes = MetadataDateTime.DefaultUtcOffsetMinutes;
                }
                else
                {
                    dateTimeFlags |= MetadataDateTimeFlags.UtcOffset;
                }
            }

            if (remainingPayload > 0)
            {
                System.Diagnostics.Debug.WriteLine("Too many bytes left! Draining...");
                reader.BaseStream.Seek(remainingPayload, System.IO.SeekOrigin.Current);
            }

            var date = DateTimeOffset.MinValue;
            if (dateTimeFlags != MetadataDateTimeFlags.None)
            {
                var offset = dateTimeFlags.HasFlag(MetadataDateTimeFlags.UtcOffset) ? new TimeSpan(utcOffsetHours, utcOffsetMinutes, 0) : TimeSpan.Zero;
                date = new DateTimeOffset(year, month, day, hour, minute, second, offset);
            }

            return new MetadataDateTime(date, dateTimeFlags);
        }

        /// <summary>
        /// Parses string metadata, checking for invalid content.
        /// </summary>
        /// <param name="reader">A binary reader to use to get the data.</param>
        /// <param name="payloadLength">Payload length of the metadata to parse, in bytes.</param>
        /// <param name="allowLineBreaks">If <c>true</c>, line breaks are allowed in the string and preserved; otherwise, line breaks are considered invalid and a string containing one is rejected.</param>
        /// <returns>The string as parsed. If invalid characters are found, an empty string is returned.</returns>
        public static string ParseStringFromMetadata(this INTV.Core.Utility.BinaryReader reader, uint payloadLength, bool allowLineBreaks)
        {
            // PCLs only support UTF8...
            // LUIGI documentation indicates this could be ASCII or UTF-8 (LUIGI)...
            // ROM metadata spec says ASCII. Let's hope we don't run into anything *too* weird.
            var bytes = reader.ReadBytes((int)payloadLength);
            var stringResult = System.Text.Encoding.UTF8.GetString(bytes, 0, (int)payloadLength).Trim('\0');
            if (stringResult.ContainsInvalidCharacters(allowLineBreaks))
            {
                stringResult = string.Empty;
            }
            return stringResult;
        }

        /// <summary>
        /// Gets the indexes if the first and last quotation mark character in the given array of bytes.
        /// </summary>
        /// <param name="rawCharacterPayload">The array of bytes to inspect.</param>
        /// <returns>A <see cref="Range{int}"/> instance which includes the index values if the first and last instance of the
        /// ASCII quotation marks character (0x22).</returns>
        public static Range<int> GetEnclosingQuoteCharacterIndexesFromBytePayload(this byte[] rawCharacterPayload)
        {
            var firstQuoteIndex = -1;
            var lastQuoteIndex = -1;
            var firstNonWhitespaceCharacterIndex = -1;
            var lastNonWhitespaceCharacterIndex = -1;
            for (int i = 0; i < rawCharacterPayload.Length; ++i)
            {
                char character = (char)rawCharacterPayload[i];
                switch (character)
                {
                    case ' ':
                    case '\t':
                    case '\0':
                        break;
                    case '"':
                        if (firstQuoteIndex < 0)
                        {
                            firstQuoteIndex = i;
                        }
                        lastQuoteIndex = i;
                        if (firstNonWhitespaceCharacterIndex < 0)
                        {
                            firstNonWhitespaceCharacterIndex = i;
                        }
                        lastNonWhitespaceCharacterIndex = i;
                        break;
                    default:
                        if (firstNonWhitespaceCharacterIndex < 0)
                        {
                            firstNonWhitespaceCharacterIndex = i;
                        }
                        lastNonWhitespaceCharacterIndex = i;
                        break;
                }
            }
            if (firstNonWhitespaceCharacterIndex != firstQuoteIndex)
            {
                firstQuoteIndex = -1;
                lastQuoteIndex = -1;
            }
            if (lastNonWhitespaceCharacterIndex != lastQuoteIndex)
            {
                lastQuoteIndex = -1;
            }
            var indexes = new Range<int>(firstQuoteIndex, lastQuoteIndex);
            return indexes;
        }

        /// <summary>
        /// Escapes the given string following the rules defined in the jzintv / SDK-1600 software stack.
        /// </summary>
        /// <param name="stringToEscape">The string to apply the escaping rules to.</param>
        /// <returns>An array of bytes suitably encoded for exchange with SDK-1600.</returns>
        /// <remarks>The details regarding how this encoding works are commented in the body of the function. This method first
        /// produces an array of bytes using UTF-8 encoding, and then treats any bytes in the result that appear either as ASCII
        /// control characters (having a value less than 0x20) or above 0x7E as requiring encoding as ASCII hexadecimal strings.
        /// This function does not wrap the resulting byte array in quotation marks. If this array of bytes is re-encoded using
        /// UTF-8, the resulting string will be the encoded version of the original string. The result is delivered as an array
        /// of bytes for ease of use with <see cref="BinaryWriter"/> and <see cref="BinaryReader"/> implementations as well as
        /// the underlying <see cref="System.IO.Stream"/> those classes use..</remarks>
        public static byte[] EscapeString(this string stringToEscape)
        {
            /* ------------------------------------------------------------------------ */
            /*  String quoting/escaping rules:                                          */
            /*                                                                          */
            /*  1.  If a string contains any of these characters: ; [ ] $ = - ,         */
            /*      or a space character, it must be quoted.                            */
            /*                                                                          */
            /*  2.  If a string contains a lone double quote, it must be quoted.        */
            /*      The double quote must be escaped with a backslash.                  */
            /*                                                                          */
            /*  3.  If a string contains characters with the  values 0x09, 0x0A, or     */
            /*      0x0D, the string must be quoted and the character must be escaped.  */
            /*      These three  characters map strictly as follows:                    */
            /*                                                                          */
            /*          0x09 => \t, 0x0A => \n, 0x0D => \r.                             */
            /*                                                                          */
            /*  4.  If a string contains any other character with a value below         */
            /*      0x20 or a value above 0x7E, the string must be quoted, and the      */
            /*      character must be escaped.  The character will be escaped with      */
            /*      a hexadecimal escape.  0x00 => \x00.  0x7E => \x7E.                 */
            /*                                                                          */
            /*  5.  If the string gets quoted, any backslashes must be escaped with     */
            /*      a backslash.  e.g.  foo-bar\baz => "foo-bar\\baz".                  */
            /* ------------------------------------------------------------------------ */
            var bytePayload = new List<byte>();
            var bytesForString = System.Text.Encoding.UTF8.GetBytes(stringToEscape);
            foreach (var byteCharacter in bytesForString)
            {
                switch ((char)byteCharacter)
                {
                    case '\\':
                    case '"':
                        bytePayload.Add((byte)'\\');
                        bytePayload.Add(byteCharacter);
                        break;
                    case '\t':
                        bytePayload.Add((byte)'\\');
                        bytePayload.Add((byte)'t');
                        break;
                    case '\r':
                        bytePayload.Add((byte)'\\');
                        bytePayload.Add((byte)'r');
                        break;
                    case '\n':
                        bytePayload.Add((byte)'\\');
                        bytePayload.Add((byte)'n');
                        break;
                    default:
                        if ((byteCharacter < 0x20) || (byteCharacter > 0x7E))
                        {
                            var hexDigitCharacters = new[]
                            {
                                (byte)'0',
                                (byte)'1',
                                (byte)'2',
                                (byte)'3',
                                (byte)'4',
                                (byte)'5',
                                (byte)'6',
                                (byte)'7',
                                (byte)'8',
                                (byte)'9',
                                (byte)'A',
                                (byte)'B',
                                (byte)'C',
                                (byte)'D',
                                (byte)'E',
                                (byte)'F'
                            };
                            var highNybbleCharacter = hexDigitCharacters[((0xF0 & byteCharacter) >> 4)];
                            var lowNybbleCharactger = hexDigitCharacters[0x0F & byteCharacter];

                            bytePayload.Add((byte)'\\');
                            bytePayload.Add((byte)'x');
                            bytePayload.Add(highNybbleCharacter);
                            bytePayload.Add(lowNybbleCharactger);
                        }
                        else
                        {
                            bytePayload.Add(byteCharacter);
                        }
                        break;
                }
            }
            return bytePayload.ToArray();
        }
    }
}

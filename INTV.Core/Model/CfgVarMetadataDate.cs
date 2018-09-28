// <copyright file="CfgVarMetadataDate.cs" company="INTV Funhouse">
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

using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// A simple class for date metadata from a .CFG file.
    /// </summary>
    public class CfgVarMetadataDate : CfgVarMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.CfgVarMetadataDate"/> class.
        /// </summary>
        /// <param name="type">The specific kind of date metadata.</param>
        public CfgVarMetadataDate(CfgVarMetadataIdTag type)
            : base(type)
        {
            Date = MetadataDateTime.MinValue;
        }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public MetadataDateTime Date { get; private set; }

        /// <inheritdoc/>
        protected override void Parse(string payload)
        {
            var dateTimeString = GetCleanPayloadString(payload);
            if (!string.IsNullOrEmpty(dateTimeString))
            {
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

                // With the above in mind, we then have up to three parts in the string, separated by whitespace:
                // The date, the time, and UTC offset. We'll take these values and do the byte parsing.
                // We'll need to take care though for four-digit years, as the byte parser expects offset
                // from 1900.
                var dateTimeParts = dateTimeString.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                var numberOfParts = dateTimeParts.Length;
                if (numberOfParts > 0)
                {
                    var flags = MetadataDateTimeFlags.None;
                    var year = MetadataDateTime.DefaultYear;
                    var month = MetadataDateTime.DefaultMonth;
                    var day = MetadataDateTime.DefaultDay;
                    var dateString = dateTimeParts[0];
                    string[] parts;

                    if (dateString.IndexOf('/') > 0)
                    {
                        parts = dateString.Split(new[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
                    }
                    else if (dateString.IndexOf('-') > 0)
                    {
                        parts = dateString.Split(new[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        parts = new[] { dateString };
                    }
                    if (parts.Length > 0)
                    {
                        ushort shortYear;
                        if (ushort.TryParse(parts[0], out shortYear))
                        {
                            var shortYearRange = new Range<int>(1, 99);
                            var fullYearRange = new Range<int>(1901, 1900 + 255);
                            if (shortYearRange.IsValueInRange(shortYear))
                            {
                                year = 1900 + shortYear;
                                flags |= MetadataDateTimeFlags.Year;
                            }
                            else if (fullYearRange.IsValueInRange(shortYear))
                            {
                                year = shortYear;
                                flags |= MetadataDateTimeFlags.Year;
                            }
                        }
                    }
                    if (flags.HasFlag(MetadataDateTimeFlags.Year))
                    {
                        if (parts.Length > 1)
                        {
                            byte byteMonth;
                            if (byte.TryParse(parts[1], out byteMonth))
                            {
                                var monthRange = new Range<byte>(1, 12);
                                if (monthRange.IsValueInRange(byteMonth))
                                {
                                    month = byteMonth;
                                    flags |= MetadataDateTimeFlags.Month;
                                }
                            }
                        }
                    }
                    if (flags.HasFlag(MetadataDateTimeFlags.Month) && (parts.Length > 2))
                    {
                        byte byteDay;
                        if (byte.TryParse(parts[2], out byteDay))
                        {
                            var dayRange = new Range<byte>(1, (byte)System.DateTime.DaysInMonth(year, month));
                            if (dayRange.IsValueInRange(byteDay))
                            {
                                day = byteDay;
                                flags |= MetadataDateTimeFlags.Day;
                            }
                        }
                    }

                    var hour = MetadataDateTime.DefaultHour;
                    var minute = MetadataDateTime.DefaultMinute;
                    var second = MetadataDateTime.DefaultMinute;
                    var timeString = numberOfParts > 1 ? dateTimeParts[1] : string.Empty;
                    parts = timeString.Split(new[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (flags.HasFlag(MetadataDateTimeFlags.Day) && (parts.Length > 0))
                    {
                        byte byteHour;
                        if (byte.TryParse(parts[0], out byteHour))
                        {
                            var hourRange = new Range<byte>(0, 23);
                            if (hourRange.IsValueInRange(byteHour))
                            {
                                hour = byteHour;
                                flags |= MetadataDateTimeFlags.Hour;
                            }
                        }
                    }
                    if (flags.HasFlag(MetadataDateTimeFlags.Hour) && (parts.Length > 1))
                    {
                        byte byteMinute;
                        if (byte.TryParse(parts[1], out byteMinute))
                        {
                            var minuteRange = new Range<byte>(0, 59);
                            if (minuteRange.IsValueInRange(byteMinute))
                            {
                                minute = byteMinute;
                                flags |= MetadataDateTimeFlags.Minute;
                            }
                        }
                    }
                    if (flags.HasFlag(MetadataDateTimeFlags.Minute) && (parts.Length > 2))
                    {
                        byte byteSecond;
                        if (byte.TryParse(parts[2], out byteSecond))
                        {
                            var secondRange = new Range<byte>(0, 60);
                            if (secondRange.IsValueInRange(byteSecond))
                            {
                                second = byteSecond;
                                flags |= MetadataDateTimeFlags.Second;
                                if (byteSecond == 60)
                                {
                                    // For leap second, set flag and set to 59, since C# does not support it.
                                    --second;
                                    flags |= MetadataDateTimeFlags.LeapSecond;
                                }
                            }
                        }
                    }

                    var utcOffsetString = numberOfParts > 2 ? dateTimeParts[2] : string.Empty;
                    var utcOffsetHours = MetadataDateTime.DefaultUtcOffsetHours;
                    var utcOffsetMinutes = MetadataDateTime.DefaultUtcOffsetMinutes;
                    if (flags.HasFlag(MetadataDateTimeFlags.Second) && (utcOffsetString.Length > 2) && ((utcOffsetString[0] == '-') || (utcOffsetString[0] == '+')))
                    {
                        if (utcOffsetString.IndexOf(':') > 0)
                        {
                            parts = utcOffsetString.Split(new[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 0)
                            {
                                sbyte byteUtcOffsetHours;
                                if (sbyte.TryParse(parts[0], out byteUtcOffsetHours))
                                {
                                    var offsetHoursRange = new Range<sbyte>(-12, 12);
                                    if (offsetHoursRange.IsValueInRange(byteUtcOffsetHours))
                                    {
                                        utcOffsetHours = byteUtcOffsetHours;
                                        flags |= MetadataDateTimeFlags.UtcOffset;
                                    }
                                }
                            }
                            if (flags.HasFlag(MetadataDateTimeFlags.UtcOffset))
                            {
                                if (parts.Length > 1)
                                {
                                    byte byteUtcOffsetMinutes;
                                    if (byte.TryParse(parts[1], out byteUtcOffsetMinutes))
                                    {
                                        var offsetMinutesRange = new Range<byte>(0, 59);
                                        if (offsetMinutesRange.IsValueInRange(byteUtcOffsetMinutes))
                                        {
                                            utcOffsetMinutes = byteUtcOffsetMinutes;
                                            if ((utcOffsetHours < 0) || (utcOffsetString[0] == '-'))
                                            {
                                                utcOffsetMinutes = -utcOffsetMinutes;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Treat as a single number, with HHMM.
                            short shortUtcOffset;
                            if (short.TryParse(utcOffsetString, out shortUtcOffset))
                            {
                                const short MaxUtcOffset = 12 * 60;
                                var offsetMinutes = shortUtcOffset < 0 ? shortUtcOffset % -100 : shortUtcOffset % 100;
                                var offsetHours = shortUtcOffset / 100;
                                var utcTotalDeltaInMinutes = (offsetHours * 60) + offsetMinutes;
                                if (System.Math.Abs(utcTotalDeltaInMinutes) <= MaxUtcOffset)
                                {
                                    utcOffsetMinutes = utcTotalDeltaInMinutes;
                                    flags |= MetadataDateTimeFlags.UtcOffset;
                                }
                            }
                        }
                    }

                    if (flags != MetadataDateTimeFlags.None)
                    {
                        var offset = flags.HasFlag(MetadataDateTimeFlags.UtcOffset) ? new System.TimeSpan(utcOffsetHours, utcOffsetMinutes, 0) : System.TimeSpan.Zero;
                        var date = new System.DateTimeOffset(year, month, day, hour, minute, second, offset);
                        Date = new MetadataDateTime(date, flags);
                    }
                }
            }
        }
    }
}

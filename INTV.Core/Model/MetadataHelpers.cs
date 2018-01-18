// <copyright file="MetadataHelpers.cs" company="INTV Funhouse">
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
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Year) || (month < 1) || (month > 12))
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
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Month) || (day < 1) || (day > DateTime.DaysInMonth(year, month)))
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
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Day) || (hour > 23))
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
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Hour) || (minute > 59))
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
                if (!dateTimeFlags.HasFlag(MetadataDateTimeFlags.Second) || (utcOffsetHours < -12) || (utcOffsetHours > 12))
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
    }
}

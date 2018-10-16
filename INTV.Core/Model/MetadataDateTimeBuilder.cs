// <copyright file="MetadataDateTimeBuilder.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// A builder for creating <see cref="MetadataDateTime"/> values.
    /// </summary>
    public class MetadataDateTimeBuilder
    {
        private int _year = -1;
        private int _month = -1;
        private int _day = -1;
        private int _hour = -1;
        private int _minute = -1;
        private int _second = -1;
        private int _offsetHours = int.MaxValue;
        private int _offsetMinutes = int.MaxValue;
        private MetadataDateTimeFlags _flags = MetadataDateTimeFlags.None;

        /// <summary>
        /// Initialize a new instance of the builder with the given year.
        /// </summary>
        /// <param name="year">The year for the date structure.</param>
        public MetadataDateTimeBuilder(int year)
        {
            WithYear(year);
        }

        /// <summary>
        /// Sets the year to use.
        /// </summary>
        /// <param name="year">The year for the date structure.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithYear(int year)
        {
            _year = year;
            return this;
        }

        /// <summary>
        /// Sets the month to use.
        /// </summary>
        /// <param name="month">The month of the year for the date structure.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithMonth(int month)
        {
            _month = month;
            return this;
        }

        /// <summary>
        /// Sets the day to use.
        /// </summary>
        /// <param name="day">The day of the month for the date structure.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithDay(int day)
        {
            _day = day;
            return this;
        }

        /// <summary>
        /// Sets the hour to use.
        /// </summary>
        /// <param name="hour">The hour of the day for the date structure.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithHour(int hour)
        {
            _hour = hour;
            return this;
        }

        /// <summary>
        /// Sets the minute to use.
        /// </summary>
        /// <param name="minute">The minute of the hour for the date structure.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithMinute(int minute)
        {
            _minute = minute;
            return this;
        }

        /// <summary>
        /// Sets the second to use.
        /// </summary>
        /// <param name="second">The second of the minute for the date structure.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithSecond(int second)
        {
            _second = second;
            return this;
        }

        /// <summary>
        /// Sets the offset from UTC time to use.
        /// </summary>
        /// <param name="hours">The hours portion of the offset from UTC.</param>
        /// <param name="minutes">The minutes portion of the offset from UTC.</param>
        /// <returns>The builder.</returns>
        public MetadataDateTimeBuilder WithOffset(int hours, int minutes)
        {
            _offsetHours = hours;
            _offsetMinutes = minutes;
            return this;
        }

        /// <summary>
        /// Creates the final <see cref="MetadataDateTime"/> instance.
        /// </summary>
        /// <returns>A valid instance of a <see cref="MetadataDateTime"/>. See remarks.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if a year is specified that cannot be supported.</exception>
        /// <remarks>Other than the value provided via the year, out-of-range values for other portions of the date time will be
        /// ignored, replaced using the appropriate default value defined by the <see cref="MetadataDateTime"/> type.</remarks>
        public MetadataDateTime Build()
        {
            ValidateYear();
            ValidateMonth();
            ValidateDay();
            ValidateHour();
            ValidateMinute();
            ValidateSecond();
            ValidateOffset();

            var offset = _flags.HasFlag(MetadataDateTimeFlags.UtcOffset) ? new System.TimeSpan(_offsetHours, _offsetMinutes, 0) : System.TimeSpan.Zero;
            var date = new System.DateTimeOffset(_year, _month, _day, _hour, _minute, _second, offset);
            var metadataDateTime = new MetadataDateTime(date, _flags);

            return metadataDateTime;
        }

        private void ValidateYear()
        {
            var yearRange = new Range<int>(DateTime.MinValue.Year, DateTime.MaxValue.Year);
            if (yearRange.IsValueInRange(_year))
            {
                _flags |= MetadataDateTimeFlags.Year;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void ValidateMonth()
        {
            var monthRange = new Range<int>(1, 12);
            if (monthRange.IsValueInRange(_month))
            {
                _flags |= MetadataDateTimeFlags.Month;
            }
            else
            {
                _month = MetadataDateTime.DefaultMonth;
            }
        }

        private void ValidateDay()
        {
            var dayRange = new Range<int>(1, (byte)System.DateTime.DaysInMonth(_year, _month));
            if (dayRange.IsValueInRange(_day))
            {
                _flags |= MetadataDateTimeFlags.Day;
            }
            else
            {
                _day = MetadataDateTime.DefaultDay;
            }
        }

        private void ValidateHour()
        {
            var hourRange = new Range<int>(0, 23);
            if (hourRange.IsValueInRange(_hour))
            {
                _flags |= MetadataDateTimeFlags.Hour;
            }
            else
            {
                _hour = MetadataDateTime.DefaultHour;
            }
        }

        private void ValidateMinute()
        {
            var minuteRange = new Range<int>(0, 59);
            if (minuteRange.IsValueInRange(_minute))
            {
                _flags |= MetadataDateTimeFlags.Minute;
            }
            else
            {
                _minute = MetadataDateTime.DefaultMinute;
            }
        }

        private void ValidateSecond()
        {
            var secondRange = new Range<int>(0, 60);
            if (secondRange.IsValueInRange(_second))
            {
                _flags |= MetadataDateTimeFlags.Second;
                if (_second == 60)
                {
                    _flags |= MetadataDateTimeFlags.LeapSecond;
                    _second = 59; // One day, a future version of .NET / C# will support leap seconds!
                }
            }
            else
            {
                _second = MetadataDateTime.DefaultSecond;
            }
        }

        private void ValidateOffset()
        {
            var offsetHoursRange = new Range<int>(-12, 12);
            if (offsetHoursRange.IsValueInRange(_offsetHours))
            {
                _flags |= MetadataDateTimeFlags.UtcOffset;
            }
            else
            {
                _offsetHours = MetadataDateTime.DefaultUtcOffsetHours;
            }

            if (_flags.HasFlag(MetadataDateTimeFlags.UtcOffset))
            {
                var offsetMinutesRange = new Range<int>(0, 59);
                if (offsetMinutesRange.IsValueInRange(_offsetMinutes))
                {
                    _flags |= MetadataDateTimeFlags.UtcOffset;
                }
            }

            // Range check the total offset, so things like -12:40 are rejected.
            const int MaxUtcOffset = 12 * 60;
            var utcTotalDeltaInMinutes = (_offsetHours * 60) + _offsetMinutes;
            if (System.Math.Abs(utcTotalDeltaInMinutes) > MaxUtcOffset)
            {
                _offsetHours = 0;
                _offsetMinutes = 0;
                _flags &= ~MetadataDateTimeFlags.UtcOffset;
            }
        }
    }
}

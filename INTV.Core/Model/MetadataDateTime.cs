// <copyright file="MetadataDateTime.cs" company="INTV Funhouse">
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

////#define USER_FRIENDLY_DEFAULT_DATETIME

using System;

namespace INTV.Core.Model
{
    /// <summary>
    /// This type wraps <see cref="System.DateTimeOffset"/> in order to support leap second and indicate
    /// whether certain fields within the wrapped <see cref="System.DateTimeOffset"/> should be considered valid.
    /// </summary>
    public struct MetadataDateTime : IComparable, IComparable<MetadataDateTime>, IEquatable<MetadataDateTime>, IFormattable
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        public static MetadataDateTime MinValue = new MetadataDateTime(DateTimeOffset.MinValue, MetadataDateTimeFlags.None);

#if USER_FRIENDLY_DEFAULT_DATETIME
        // Use Approximate "birthday" of the Intellivision console for default date.
        internal const int DefaultYear = 79;
        internal const int DefaultMonth = 12;
        internal const int DefaultDay = 3;
        internal const int DefaultHour = 12;
        internal const int DefaultMinute = 0;
        internal const int DefaultSecond = 0;
        internal const int DefaultUtcOffsetHours = -8;
        internal const int DefaultUtcOffsetMinutes = 0;
#else
        internal const int DefaultYear = 0;
        internal const int DefaultMonth = 1;
        internal const int DefaultDay = 1;
        internal const int DefaultHour = 0;
        internal const int DefaultMinute = 0;
        internal const int DefaultSecond = 0;
        internal const int DefaultUtcOffsetHours = 0;
        internal const int DefaultUtcOffsetMinutes = 0;
#endif // USER_FRIENDLY_DEFAULT_DATETIME

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.MetadataDateTime"/> struct.
        /// </summary>
        /// <param name="date">The <see cref="System.DateTimeOffset"/> to wrap.</param>
        /// <param name="flags">The field validation flags.</param>
        public MetadataDateTime(DateTimeOffset date, MetadataDateTimeFlags flags)
            : this()
        {
            Date = date;
            Flags = flags;
        }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTimeOffset Date { get; private set; }

        /// <summary>
        /// Gets the field validation flags.
        /// </summary>
        public MetadataDateTimeFlags Flags { get; private set; }

        /// <summary>
        /// Compares the given <see cref="MetadataDateTime"/> to this instance.
        /// </summary>
        /// <param name="other">The other value to compare to this instance.</param>
        /// <param name="compareFlags">If <c>true</c>, include <see cref="Flags"/> as part of the comparison if the <see cref="Date"/> values are considered equal.</param>
        /// <returns>An integer value indicating less than, equal, or greater than.</returns>
        public int CompareTo(MetadataDateTime other, bool compareFlags)
        {
            var result = Date.CompareTo(other.Date);
            if ((result == 0) && compareFlags)
            {
                result = (int)Flags - (int)other.Flags;
            }
            return result;
        }

        #region IComparable

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            var result = -1;
            if (obj is MetadataDateTime)
            {
                result = CompareTo((MetadataDateTime)obj);
            }

            return result;
        }

        #endregion // IComparable

        #region IComparable<MetadataDateTime>

        /// <inheritdoc />
        public int CompareTo(MetadataDateTime other)
        {
            return CompareTo(other, compareFlags: true);
        }

        #endregion // IComparable<MetadataDateTime>

        #region IEquatable<MetadataDateTime>

        /// <inheritdoc />
        public bool Equals(MetadataDateTime other)
        {
            return CompareTo(other) == 0;
        }

        #endregion // IEquatable<MetadataDateTime>

        #region IFormattable

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var toString = Date.ToString(formatProvider) + " {" + Flags.ToString() + "}";
            return toString;
        }

        #endregion // IFormattable
    }
}

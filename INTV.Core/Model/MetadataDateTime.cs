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
using System.Linq;

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
        /// Tests two instances of <see cref="MetadataDateTime"/> for equality.
        /// </summary>
        /// <param name="lhs">Value on left hand side of operator.</param>
        /// <param name="rhs">Value on right hand side of operator.</param>
        /// <returns><c>true</c> if the two values are considered equal. See remarks.</returns>
        /// <remarks>This is the same as calling <see cref="CompareTo(MetadataDateTime other, bool strict, bool compareOnlyCommonValidFields)"/> with strict set to <c>false</c> and compareOnlyCommonValidFields set to <c>true</c>.</remarks>
        public static bool operator ==(MetadataDateTime lhs, MetadataDateTime rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests two instances of <see cref="MetadataDateTime"/> for inequality.
        /// </summary>
        /// <param name="lhs">Value on left hand side of operator.</param>
        /// <param name="rhs">Value on right hand side of operator.</param>
        /// <returns><c>true</c> if the two values are considered not equal. See remarks.</returns>
        /// <remarks>This is the same as calling <see cref="CompareTo(MetadataDateTime other, bool strict, bool compareOnlyCommonValidFields)"/> with strict set to <c>false</c> and compareOnlyCommonValidFields set to <c>true</c>.</remarks>
        public static bool operator !=(MetadataDateTime lhs, MetadataDateTime rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Converts the value of the underlying System.DateTimeOffset object to its equivalent string representation using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string as supported by <see cref="System.DateTimeOffset"/>.</param>
        /// <returns>A string representation of the value of the underlying System.DateTimeOffset object, as specified by <paramref name="formatProvider"/>.</returns>
        /// <remarks>Includes the flags as part of the output.</remarks>
        /// <exception cref="System.FormatException">The length of format is one, and it is not one of the standard format specifier characters defined for
        /// <see cref="System.Globalization.DateTimeFormatInfo"/>. -or-format does not contain a valid custom format pattern.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The date and time is outside the range of dates supported by the calendar used by formatProvider.</exception>
        public string ToString(string format)
        {
            return ToString(format, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Converts the value of the underlying System.DateTimeOffset object to its equivalent string representation using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of the value of the underlying System.DateTimeOffset object, as specified by <paramref name="formatProvider"/>.</returns>
        /// <remarks>Includes the flags as part of the output.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">The date and time is outside the range of dates supported by the calendar used by formatProvider.</exception>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        /// Compares the given <see cref="MetadataDateTime"/> to this instance.
        /// </summary>
        /// <param name="other">The other value to compare to this instance.</param>
        /// <param name="strict">If <c>true</c>, include <see cref="Flags"/> as part of the comparison if the <see cref="Date"/> values are considered equal.</param>
        /// <returns>An integer value indicating less than, equal, or greater than.</returns>
        /// <remarks>This comparison will only use commonly valid fields. It is the same as calling <see cref="CompareTo(MetadataDateTime other, bool strict, bool compareOnlyCommonValidFields)"/> with compareOnlyCommonValidFields set to true.</remarks>
        public int CompareTo(MetadataDateTime other, bool strict)
        {
            return CompareTo(other, strict, compareOnlyCommonValidFields: true);
        }

        /// <summary>
        /// Compares the given <see cref="MetadataDateTime"/> to this instance.
        /// </summary>
        /// <param name="other">The other value to compare to this instance.</param>
        /// <param name="strict">If <c>true</c>, the <see cref="Flags"/> must be the same before the date comparison will be done.</param>
        /// <param name="compareOnlyCommonValidFields">If <c>true</c>, only compare fields of each item's <see cref="Date"/> for which the corresponding flag is valid.</param>
        /// <returns>An integer value indicating less than, equal, or greater than.</returns>
        /// <remarks>When <paramref name="strict"/> is <c>true</c>, the flag comparison of flags is done first. Next, if <paramref name="compareOnlyValidFields"/>
        /// is <c>true</c>, only portions of the date will be compared for which a flag is present in both objects. If there are no intersecting flags at
        /// when <paramref name="compareOnlyValidFields"/> is <c>true</c>, the underlying <see cref="System.DateTimeOffset"/> produces the result.
        /// If both <paramref name="strict"/> and <paramref name="compareOnlyValidFields"/> are <c>false</c>, then the comparison is equivalent to a simple
        /// <see cref="System.DateTimeOffset.CompareTo(DateTimeOffset)"/> operation.</remarks>
        public int CompareTo(MetadataDateTime other, bool strict, bool compareOnlyCommonValidFields)
        {
            var result = 0;
            if (strict)
            {
                result = (int)Flags - (int)other.Flags;
            }
            if (result == 0)
            {
                var doDateTimeCompare = true;
                if (compareOnlyCommonValidFields)
                {
                    var commonFlags = Flags & other.Flags;
                    doDateTimeCompare = commonFlags == MetadataDateTimeFlags.None;
                    if (!doDateTimeCompare)
                    {
                        foreach (var fieldToCheck in Enum.GetValues(typeof(MetadataDateTimeFlags)).Cast<MetadataDateTimeFlags>())
                        {
                            if (commonFlags.HasFlag(fieldToCheck))
                            {
                                switch (fieldToCheck)
                                {
                                    case MetadataDateTimeFlags.Year:
                                        result = Date.Year - other.Date.Year;
                                        break;
                                    case MetadataDateTimeFlags.Month:
                                        result = Date.Month - other.Date.Month;
                                        break;
                                    case MetadataDateTimeFlags.Day:
                                        result = Date.Day - other.Date.Day;
                                        break;
                                    case MetadataDateTimeFlags.Hour:
                                        result = Date.Hour - other.Date.Hour;
                                        break;
                                    case MetadataDateTimeFlags.Minute:
                                        result = Date.Minute - other.Date.Minute;
                                        break;
                                    case MetadataDateTimeFlags.Second:
                                        result = Date.Second - other.Date.Second;
                                        break;
                                    case MetadataDateTimeFlags.LeapSecond:
                                        // This is just a bookkeeping flag, as leap second support is only available in future release of Windows:
                                        // https://mspoweruser.com/microsoft-is-adding-support-for-leap-seconds-in-windows/
                                        // Presumably, some future release of C# will provide support for the leap second, and the need for this flag will
                                        // likely go away. That strongly implies dropping all Windows platforms prior to the future release of Windows 10.
                                        break;
                                    case MetadataDateTimeFlags.UtcOffset:
                                        result = Date.Offset.CompareTo(other.Date.Offset);
                                        break;
                                    default:
                                        break;
                                }
                                if (result != 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (doDateTimeCompare && (result == 0))
                {
                    result = Date.CompareTo(other.Date);
                }
            }
            return result;
        }

        #region IComparable

        /// <inheritdoc />
        /// <exception cref="System.ArgumentException">Thrown if <param name="obj"/> is not a <see cref="MetadataDateTime"/>.</exception>
        public int CompareTo(object obj)
        {
            if (obj is MetadataDateTime)
            {
                return CompareTo((MetadataDateTime)obj);
            }
            throw new ArgumentException();
        }

        #endregion // IComparable

        #region IComparable<MetadataDateTime>

        /// <inheritdoc />
        public int CompareTo(MetadataDateTime other)
        {
            return CompareTo(other, strict: true);
        }

        #endregion // IComparable<MetadataDateTime>

        #region IEquatable<MetadataDateTime>

        /// <inheritdoc />
        /// <remarks>This is the same as calling <see cref="CompareTo(MetadataDateTime other, bool strict, bool compareOnlyCommonValidFields)"/> with strict set to <c>false</c> and compareOnlyCommonValidFields set to <c>true</c>.</remarks>
        public bool Equals(MetadataDateTime other)
        {
            return CompareTo(other, strict: false, compareOnlyCommonValidFields: true) == 0;
        }

        #endregion // IEquatable<MetadataDateTime>

        #region IFormattable

        /// <inheritdoc />
        /// <remarks>Includes the flags as part of the output.</remarks>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var toString = Date.ToString(format, formatProvider) + " {" + Flags.ToString() + "}";
            return toString;
        }

        #endregion // IFormattable

        #region System.Object

        /// <inheritdoc />
        /// <remarks>This is the same as calling <see cref="CompareTo(MetadataDateTime other, bool strict, bool compareOnlyCommonValidFields)"/> with strict set to <c>false</c> and compareOnlyCommonValidFields set to <c>true</c>.</remarks>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="obj"/> is not a <see cref="MetadataDateTime"/>.</exception>
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Date.GetHashCode() * 31) + Flags.GetHashCode();
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentException">Thrown if <param name="obj"/> is not a <see cref="MetadataDateTime"/>.</exception>
        public override string ToString()
        {
            return ToString(null, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        #endregion // System.Object
    }
}

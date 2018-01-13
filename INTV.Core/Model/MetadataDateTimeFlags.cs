// <copyright file="MetadataDateTimeFlags.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Metadata date time field element validity flags.
    /// </summary>
    [System.Flags]
    public enum MetadataDateTimeFlags
    {
        /// <summary>None of the fields in a <see cref="System.DateTime"/> or <see cref="System.DateTimeOffset"/> structure should be considered valid.</summary>
        None = 0,

        /// <summary>The year field is valid.</summary>
        Year = 1 << 0,

        /// <summary>The month field is valid.</summary>
        Month = 1 << 1,

        /// <summary>The day field is valid.</summary>
        Day = 1 << 2,

        /// <summary>The hour field is valid.</summary>
        Hour = 1 << 3,

        /// <summary>The minute field is valid.</summary>
        Minute = 1 << 4,

        /// <summary>The second field is valid.</summary>
        Second = 1 << 5,

        /// <summary>The date/time indicates a leap second.</summary>
        LeapSecond = 1 << 6,

        /// <summary>The date/time contains a valid UTC offset.</summary>
        UtcOffset = 1 << 7
    }
}

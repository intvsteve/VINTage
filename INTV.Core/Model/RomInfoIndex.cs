// <copyright file="RomInfoIndex.cs" company="INTV Funhouse">
// Copyright (c) 2016-2019 All Rights Reserved
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
    /// Convenient names to get results from an enumerable of string results.
    /// The enumerable is required to contain results in this order.
    /// </summary>
    public enum RomInfoIndex
    {
        /// <summary>
        /// A 'not a valid value' index.
        /// </summary>
        None = -1,

        /// <summary>
        /// Index to the (long) name of a ROM.
        /// </summary>
        Name = 0,

        /// <summary>
        /// Index to the copyright date string for a ROM.
        /// </summary>
        Copyright,

        /// <summary>
        /// Index to the short name of a ROM.
        /// </summary>
        ShortName,

        /// <summary>
        /// Sentinel value for maximum number of entries that are valid.
        /// </summary>
        NumEntries
    }

    /// <summary>
    /// Helper methods for the RomInfoIndex enumeration.
    /// </summary>
    public static class RomInfoIndexHelpers
    {
        /// <summary>
        /// The maximum supported length for a 'short' name.
        /// </summary>
        public const int MaxShortNameLength = 18;

        /// <summary>
        /// Gets a specific value from an enumerable of strings.
        /// </summary>
        /// <param name="results">The results from which to extract a result.</param>
        /// <param name="index">The desired result to extract.</param>
        /// <returns>Desired value extracted from the enumerable, e.g. output from the intvname utility from the SDK-1600.</returns>
        public static string GetRomInfoString(this IEnumerable<string> results, RomInfoIndex index)
        {
            var result = string.Empty;
            if (index > RomInfoIndex.None)
            {
                var resultsCount = results.Count();
                if (resultsCount > (int)index)
                {
                    result = results.ElementAt((int)index);
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                if (result.ContainsInvalidCharacters(allowLineBreaks: false))
                {
                    result = string.Empty;
                }
                else
                {
                    result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ").Trim();
                }
            }
            return result;
        }
    }
}

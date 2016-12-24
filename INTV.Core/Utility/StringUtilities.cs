// <copyright file="StringUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

using System.Linq;

namespace INTV.Core.Utility
{
    /// <summary>
    /// Useful functions for working with strings.
    /// </summary>
    public static class StringUtilities
    {
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
    }
}

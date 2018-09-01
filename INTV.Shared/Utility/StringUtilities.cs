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

using System.Net;
using System.Text.RegularExpressions;

namespace INTV.Shared.Utility
{
    public static class StringUtilities
    {
        /// <summary>
        /// Decodes a string that may contain HTML-encoded characters and strip HTML tags.
        /// </summary>
        /// <param name="encodedHtmlString">A string potentially containing HTML.</param>
        /// <returns>The stripped and decoded string.</returns>
        public static string HtmlDecode(this string encodedHtmlString)
        {
            return encodedHtmlString.HtmlStripTagsAndDecode(stripHtmlTags: true);
        }

        /// <summary>
        /// Decodes a string that may contain HTML-encoded characters and tags. Tags are optionally stripped.
        /// </summary>
        /// <param name="encodedHtmlString">A string potentially containing HTML.</param>
        /// <param name="stripHtmlTags">If <c>true</c>, strip HTML tags, then decode the string. Otherwise, any HTML tags in the string will remain.</param>
        /// <returns>The stripped and decoded string.</returns>
        /// <remarks>If someone's up to integrating the HTML Agility Pack, that would be nice.</remarks>
        public static string HtmlStripTagsAndDecode(this string encodedHtmlString, bool stripHtmlTags)
        {
            var decodedString = string.Empty;
            if (!string.IsNullOrEmpty(encodedHtmlString))
            {
                // NOTE: Not using HTML Agility Pack. Would be a nice upgrade, though. So, instead, do a dubious thing and just
                // use a regular expression to slice out things that might be HTML tags. DANGER!
                // This function assumes the HTML is from a trusted source, and properly escapes anything that it should.
                // See: https://stackoverflow.com/questions/18153998/how-do-i-remove-all-html-tags-from-a-string-without-knowing-which-tags-are-in-it
                decodedString = stripHtmlTags ? Regex.Replace(encodedHtmlString, "<.*?>", string.Empty) : encodedHtmlString;
                decodedString = WebUtility.HtmlDecode(decodedString);
            }
            return decodedString;
        }
    }
}

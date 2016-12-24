// <copyright file="ValidEmailAddressBehavior.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Common part of the email text validation behavior.
    /// </summary>
    public static partial class ValidEmailAddressBehavior
    {
        /// <summary>
        /// Determines if the given text forms a valid email address.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="text"/> is a legal email address snytactically; otherwise, <c>false</c>.</returns>
        /// <param name="text">The text to test to determine if it forms a syntactically valid email address.</param>
        public static bool IsValidEmailAddress(string text)
        {
            var isValid = string.IsNullOrEmpty(text); // allow null / empty
            if (!isValid)
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(text);
                    isValid = email.Address.Length > 0;
                }
                catch (System.Exception)
                {
                }
            }
            return isValid;
        }
    }
}

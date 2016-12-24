// <copyright file="ViewModelHelpers.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Useful methods for ViewModel implementations.
    /// </summary>
    public static class ViewModelHelpers
    {
        /// <summary>
        /// Returns either the string or, if string is null, returns string.Empty.
        /// </summary>
        /// <param name="possiblyNullString">The string to return; if <c>null</c> will return string.Empty.</param>
        /// <returns>A non-null string.</returns>
        public static string SafeString(this string possiblyNullString)
        {
            return (possiblyNullString == null) ? string.Empty : possiblyNullString;
        }
    }
}

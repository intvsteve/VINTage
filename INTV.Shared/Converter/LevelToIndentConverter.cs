// <copyright file="LevelToIndentConverter.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Converts a tree item level to left margin.
    /// </summary>
    /// <remarks>TODO: Pass a parameter if you want a unit length other than 16.0 per level of indention.
    /// TODO: Split into separate file.</remarks>
    public class LevelToIndentConverter : System.Windows.Data.IValueConverter
    {
        /// <summary>
        /// Size of the indent to use.
        /// </summary>
        public const double IndentSize = 16.0;

        /// <inheritdoc />
        public object Convert(object o, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return new System.Windows.Thickness((int)o * IndentSize, 0, 0, 0);
        }

        /// <inheritdoc />
        public object ConvertBack(object o, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

// <copyright file="PlaceholderTextVisibilityConverter.cs" company="INTV Funhouse">
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

using System;
using System.Windows;
using System.Windows.Data;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Given a text value that is null or empty, determine whether an entity should be visible or not. If value is null or empty, then visible, otherwise hidden.
    /// </summary>
    public class PlaceholderTextVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var textToTest = value as string;
            return string.IsNullOrEmpty(textToTest) ? Visibility.Visible : Visibility.Hidden;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion // IValueConverter Members
    }
}

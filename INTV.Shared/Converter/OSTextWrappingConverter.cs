// <copyright file="OSTextWrappingConverter.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Simple converter between platform-independent and platform-specific text wrapping enum. The binding is type-sensitive, hence this converter.
    /// </summary>
    public class OSTextWrappingConverter : System.Windows.Data.IValueConverter
    {
        #region IValueConverter Members

        /// <inheritdoc />
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (System.Windows.TextWrapping)value;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (INTV.Shared.View.OSTextWrapping)value;
        }

        #endregion
    }
}

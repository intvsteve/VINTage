// <copyright file="EnumConverter.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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
using System.Windows.Data;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Simple bidirectional enum converter.
    /// </summary>
    /// <remarks>Adapted from this handy Stack Overflow article:
    /// http://stackoverflow.com/questions/20707160/data-binding-int-property-to-enum-in-wpf
    /// </remarks>
    public class EnumConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = null;
            if (value != null)
            {
                if ((Nullable.GetUnderlyingType(targetType) == typeof(bool)) && (parameter != null) && parameter.GetType().IsEnum)
                {
                    // Converting to a nullable Boolean value.
                    result = value.Equals(parameter);
                }
                else if (targetType.IsEnum)
                {
                    // Converting to an enum value.
                    if (value is bool)
                    {
                        if ((bool)value)
                        {
                            result = parameter;
                        }
                        else
                        {
                            result = Binding.DoNothing;
                        }
                    }
                    else
                    {
                        result = Enum.ToObject(targetType, value);
                    }
                }
                else if (value.GetType().IsEnum)
                {
                    // Converting from enum.
                    result = System.Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
                }
            }
            return result;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }

        #endregion
    }
}

// <copyright file="EnumStringConverter.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using System.Windows.Data;
using INTV.Shared.Utility;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Converter for moving between an enum-as-string and user-facing-string values.
    /// </summary>
    /// <remarks>This specialized converter requires that the converter parameter is the full type name, followed by
    /// the assembly name containing the type in the following format:
    /// Full.Name.Space.To.EnumType-Assembly.Name
    /// This string information is used to get the actual Type of the Enum. From that information, the next requirement
    /// is that the implementing assembly provide a string resource (in a resource named 'Strings') with the following key:
    /// EnumType_ValueName
    /// That resource is presumed to be the user-facing string for the enum value.</remarks>
    public class EnumStringConverter : IValueConverter
    {
        private Dictionary<string, string> _converterData = new Dictionary<string, string>();

        #region IValueConverter

        /// <inheritdoc />
        /// <remarks>This form of the converter expects to convert FROM the direct Enum string TO a user-facing string.</remarks>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Type enumType = GetEnumTypeFromParameter(parameter);
            EnsureConverterData(enumType);
            string userFacingString = null;
            if (string.IsNullOrEmpty(value as string) || !_converterData.TryGetValue(value as string, out userFacingString))
            {
                // assume that the first value in the enum is the 'default'
                var values = Enum.GetValues(enumType);
                _converterData.TryGetValue(values.GetValue(0).ToString(), out userFacingString);
            }
            return userFacingString;
        }

        /// <inheritdoc />
        /// <remarks>This form of the converter expects to convert FROM the user-facing string TO a direct Enum string (i.e. enumValue.ToString()).</remarks>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException(Resources.Strings.EnumToStringConverterDataTypeErrorMessage);
            }
            EnsureConverterData(GetEnumTypeFromParameter(parameter));
            var enumString = _converterData.FirstOrDefault(e => e.Value == (value as string)).Key;
            return enumString;
        }

        #endregion // IValueConverter

        private Type GetEnumTypeFromParameter(object parameter)
        {
            Type enumType = null;
            var enumTypeString = parameter as string;
            if (!string.IsNullOrEmpty(enumTypeString))
            {
                enumType = Type.GetType(enumTypeString.Replace("-", ", "));
            }
            if ((enumType == null) || !enumType.IsEnum)
            {
                throw new InvalidOperationException(Resources.Strings.EnumToStringConverterErrorMessage);
            }
            return enumType;
        }

        private void EnsureConverterData(Type enumType)
        {
            if (!_converterData.Any())
            {
                var enumTypeName = enumType.Name;
                var values = Enum.GetValues(enumType);
                var i = 0;
                foreach (var value in values)
                {
                    var valueName = value.ToString();
                    var key = MakeResourceKey(enumTypeName, valueName);
                    var userFacingString = enumType.GetResourceString(key);
                    _converterData[valueName] = userFacingString;
                    ++i;
                }
                var underlyingType = Enum.GetUnderlyingType(enumType);
            }
        }

        private string MakeResourceKey(string enumTypeName, object value)
        {
            var valueName = value.ToString();
            var key = enumTypeName + '_' + valueName;
            return key;
        }
    }
}

// <copyright file="BooleanToDisplayModeConverter.cs" company="INTV Funhouse">
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
using System.Windows.Data;
using INTV.JzIntv.Model;

namespace INTV.JzIntvUI.Converter
{
    /// <summary>
    /// Converts between a Boolean value and the DisplayMode enumeration. Used for Ribbon UI for easy access to jzIntv settings.
    /// </summary>
    public class BooleanToDisplayModeConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fullscreen = false;
            var setting = value as string;
            if (!string.IsNullOrEmpty(setting))
            {
                DisplayMode displayMode;
                if (Enum.TryParse<DisplayMode>(setting, out displayMode))
                {
                    fullscreen = displayMode == DisplayMode.Fullscreen;
                }
            }
            return fullscreen;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var displayMode = DisplayMode.Default;
            if (value != null)
            {
                displayMode = (bool)value ? DisplayMode.Fullscreen : DisplayMode.Windowed;
            }
           return displayMode.ToString();
        }
    }
}

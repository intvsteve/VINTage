// <copyright file="RibbonComboBoxToEnumConverter`T.cs" company="INTV Funhouse">
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
using INTV.LtoFlash.ViewModel;

namespace INTV.LtoFlash.Converter
{
    /// <summary>
    /// Implements a converter to avoid an annoying behavior with <see cref="RibbonComboBox"/>.
    /// </summary>
    /// <typeparam name="T">The enum type use for the values in the combo box.</typeparam>
    public abstract class RibbonComboBoxToEnumConverter<T> : System.Windows.Data.IValueConverter where T : struct
    {
        /// <summary>
        /// Gets or sets the default value to use.
        /// </summary>
        protected static T DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the function to use to convert the value to a display string.
        /// </summary>
        protected static Func<T, string> ToDisplayString { get; set; }

        /// <summary>
        /// Gets or sets the function to use to convert from a display string to a value of the type.
        /// </summary>
        protected static Func<string, T> FromDisplayString { get; set; }

        /// <summary>
        /// Gets or sets the function to use to convert from a ViewModel to a value of the type.
        /// </summary>
        protected static Func<DeviceViewModel, T> FromViewModel { get; set; }

        #region IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var displayVal = ToDisplayString((T)value);
            return displayVal;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var theValue = DefaultValue;

            if (value == null)
            {
                // HACK to deal with RibbonComboBox. When you click to open the box, a null value is sent, which results in a value
                // change being sent off to the view model. This HACK avoids this by just using the current value. An alternative
                // approach would be to use a multi-value converter, or perhaps an attached behavior. Maybe creating a full-blown
                // ViewModel for each enum value, or writing a class for the combo box item visuals would work. Too much work.
                // RibbonComboBox is a pain.
                var ltoFlash = INTV.Shared.ComponentModel.CompositionHelpers.Container.GetExportedValue<INTV.LtoFlash.ViewModel.LtoFlashViewModel>();
                theValue = FromViewModel(ltoFlash.ActiveLtoFlashDevice);
            }
            else
            {
                theValue = FromDisplayString((string)value);
            }
            return theValue;
        }

        #endregion // IValueConverter
    }
}

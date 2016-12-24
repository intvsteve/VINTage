// <copyright file="JlpFlashStorageUsageLevelToBrushConverter.WPF.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Converts the JLP flash usage level to a color for display purposes.
    /// </summary>
    public class JlpFlashStorageUsageLevelToBrushConverter : System.Windows.Data.IValueConverter
    {
        #region IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var textColor = System.Windows.SystemColors.WindowTextColor;
            var flashStorageUsageLevel = (JlpFlashStorageUsageLevel)value;
            switch (flashStorageUsageLevel)
            {
                case JlpFlashStorageUsageLevel.None:
                case JlpFlashStorageUsageLevel.Normal:
                    break;
                case JlpFlashStorageUsageLevel.High:
                    textColor = INTV.Core.Model.Stic.Color.Orange.ToColor();
                    break;
                case JlpFlashStorageUsageLevel.LtoFlashOnly:
                    textColor = INTV.Core.Model.Stic.Color.Red.ToColor();
                    break;
            }
            var brush = new System.Windows.Media.SolidColorBrush(textColor);
            return brush;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion // IValueConverter
    }
}

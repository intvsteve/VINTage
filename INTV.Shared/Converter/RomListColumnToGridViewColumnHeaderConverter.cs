// <copyright file="RomListColumnToGridViewColumnHeaderConverter.cs" company="INTV Funhouse">
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

using System;
using System.Linq;
using System.Windows.Controls;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// This converter requires a RomListColumn and a GridView as its first and second value. It will convert this
    /// information into a GridViewColumnHeader for use with sorting the contents of a GridView.
    /// </summary>
    public class RomListColumnToGridViewColumnHeaderConverter : System.Windows.Data.IMultiValueConverter
    {
        /// <inheritdoc />
        /// <remarks>The first value of the values array must be a RomListColumn value. The second must be a GridView. The converter
        /// returns the GridViewColumnHeader that has a Tag value matching the first element in the values array.</remarks>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object sortColumnHeader = null;
            var settingsSortColumn = (RomListColumn)values[0];
            if (settingsSortColumn != RomListColumn.None)
            {
                var gridView = values[1] as GridView;
                if (gridView != null)
                {
                    var sortColumn = gridView.Columns.FirstOrDefault(c => (RomListColumn)((GridViewColumnHeader)((GridViewColumn)c).Header).Tag == settingsSortColumn);
                    sortColumnHeader = sortColumn.Header;
                }
            }
            return sortColumnHeader;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

// <copyright file="ProgramToProgramDescriptionConverter.cs" company="INTV Funhouse">
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
using System.Windows.Data;
using INTV.Core.Model.Program;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.Converter
{
    /// <summary>
    /// Given a collection of ProgramDescriptionViewModels and a ProgramViewModel, locate the element in the collection corresponding to the program.
    /// </summary>
    public class ProgramToProgramDescriptionConverter : IMultiValueConverter
    {
        /// <inheritdoc />
        /// <remarks>This converter requires values[0] to be an observable collection of ProgramDescriptionViewModel objects, and the second to be
        /// a ProgramViewModel. If values[2] is a ListView, the converter also has a side effect of scrolling the selected item into view.</remarks>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object selectedItem = null;
            if (values.Length >= 2)
            {
                var listViewItems = values[0] as ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescription>;
                var selectedMenuItem = values[1] as INTV.LtoFlash.ViewModel.ProgramViewModel;
                if ((listViewItems != null) && listViewItems.Any() && (selectedMenuItem != null))
                {
                    selectedItem = listViewItems.FirstOrDefault(p => selectedMenuItem.ProgramDescription == p.Model);
                }
                if ((selectedItem != null) && (values.Length >= 3))
                {
                    var view = values[2] as System.Windows.Controls.ListView;
                    if (view != null)
                    {
                        view.ScrollIntoView(selectedItem);
                    }
                }
            }
            return selectedItem;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

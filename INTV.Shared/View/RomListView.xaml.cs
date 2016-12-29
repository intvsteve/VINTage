// <copyright file="RomListView.xaml.cs" company="INTV Funhouse">
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

using System.Windows;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for RomListView.xaml
    /// </summary>
    public partial class RomListView : System.Windows.Controls.ListView
    {
        #region Commands

        public static RelayCommand DoubleClickItemCommand = new RelayCommand(OnDoubleClick);

        #endregion // Commands

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RomListView type.
        /// </summary>
        public RomListView()
        {
            InitializeComponent();
        }

        #endregion // Constructors

        /// <summary>
        /// Mechanism to explicitly edit a specific column in the ROMs view.
        /// </summary>
        /// <param name="column">The column to edit.</param>
        internal void EditSelectedItemColumn(RomListColumn column)
        {
            if (SelectedItem != null)
            {
                var container = ItemContainerGenerator.ContainerFromItem(SelectedItem);
                if (container != null)
                {
                    var visualForEdit = FindElementForColumn(column);
                    INTV.Shared.Behavior.InPlaceEditBehavior.SetLastClickedElement(this, visualForEdit);
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>In this override, we ensure that on selection change, if the user uses the keyboard to navigate and then uses F2
        /// to invoke in-place edit, the ROM's title (name) will begin editing. The generic InPlaceEditBehavior's selection changed handler
        /// plays it safe and resets the LastClickedElement because it doesn't know what to assume.</remarks>
        protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            var defaultVisualForEdit = FindElementForColumn(RomListColumn.Title);
            INTV.Shared.Behavior.InPlaceEditBehavior.SetLastClickedElement(this, defaultVisualForEdit);
        }

        private static void OnDoubleClick(object parameter)
        {
            var doubleClickedProgram = parameter as ProgramDescriptionViewModel;
            if (doubleClickedProgram != null)
            {
                // This is a bit icky, since the parameter doesn't have a way to crawl back up to our ROM list...
                // So we use the global ROM list to invoke the handler.
                ProgramCollection.Roms.InvokeProgramFromDescription(doubleClickedProgram.Model);
            }
        }

        private FrameworkElement FindElementForColumn(RomListColumn column)
        {
            FrameworkElement elementForColumn = null;
            if (SelectedItem != null)
            {
                var container = ItemContainerGenerator.ContainerFromItem(SelectedItem);
                if (container != null)
                {
                    elementForColumn = container.FindChild<FrameworkElement>(e => (e != null) && (e.Tag != null) && ((RomListColumn)e.Tag == column));
                }
            }
            return elementForColumn;
        }
    }
}

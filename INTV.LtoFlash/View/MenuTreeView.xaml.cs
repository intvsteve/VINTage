// <copyright file="MenuTreeView.xaml.cs" company="INTV Funhouse">
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
using System.Windows.Controls;
using System.Windows.Input;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Interaction logic for MenuTreeView.xaml
    /// </summary>
    public partial class MenuTreeView : TreeListView
    {
        /// <summary>
        /// Initializes a new instance of the MenuTreeView type.
        /// </summary>
        public MenuTreeView()
        {
            InitializeComponent();
            MouseRightButtonDown += HandleMouseRightButtonDown;
        }

        /// <summary>
        /// Mechanism to explicitly edit a specific column in the menu layout view.
        /// </summary>
        /// <param name="column">The selected column.</param>
        internal void EditSelectedItemColumn(MenuLayoutColumn column)
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

        private FrameworkElement FindElementForColumn(MenuLayoutColumn column)
        {
            FrameworkElement elementForColumn = null;
            if (SelectedItem != null)
            {
                var container = ItemContainerGenerator.ContainerFromItem(SelectedItem);
                if (container != null)
                {
                    elementForColumn = container.FindChild<FrameworkElement>(e => (e != null) && (e.Tag != null) && ((MenuLayoutColumn)e.Tag == column));
                }
            }
            return elementForColumn;
        }

        /// <summary>
        /// Select item when right clicked.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Mouse button event data.</param>
        private void HandleMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedVisual = e.OriginalSource as FrameworkElement;
            var clickedItem = clickedVisual.GetParent<TreeListViewItem>();
            if (clickedItem != null)
            {
                clickedItem.IsSelected = true;
                clickedItem.Focus();
                e.Handled = true;
            }
        }
    }
}

// <copyright file="SelectedItemObserver.cs" company="INTV Funhouse">
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
using System.Windows.Controls.Primitives;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements a value observer for SelectedItem on ItemsControl-based visuals. These include Selector, TreeView and ListBox.
    /// </summary>
    public static class SelectedItemObserver
    {
        /// <summary>
        /// Setting this attached property to <c>true</c> enables the observers.
        /// </summary>
        public static readonly DependencyProperty SelectedItemObserverProperty = DependencyProperty.RegisterAttached("SelectedItemObserver", typeof(bool), typeof(SelectedItemObserver), new PropertyMetadata(SelectedItemObserverChangedCallBack));

        /// <summary>
        /// This attached property is updated when the selected item changes.
        /// </summary>
        public static readonly DependencyProperty ObservedItemSelectionProperty = DependencyProperty.RegisterAttached("ObservedItemSelection", typeof(object), typeof(SelectedItemObserver));

        /// <summary>
        /// Get the value of the SelectedItemObserver attached property.
        /// </summary>
        /// <param name="itemsControl">he visual from which to read the SelectedItemObserver property.</param>
        /// <returns>><c>true</c> if the observer behavior has been enabled.</returns>
        public static bool GetSelectedItemObserver(this ItemsControl itemsControl)
        {
            return (bool)itemsControl.GetValue(SelectedItemObserverProperty);
        }

        /// <summary>
        /// Sets the value of the SelectedItemObserver attached property.
        /// </summary>
        /// <param name="itemsControl">The visual upon which to set the SelectedItemObserver property.</param>
        /// <param name="observe"><c>true</c> if the observer behavior should be enabled.</param>
        public static void SetSelectedItemObserver(this ItemsControl itemsControl, bool observe)
        {
            itemsControl.SetValue(SelectedItemObserverProperty, observe);
        }

        /// <summary>
        /// Gets the value of the ObservedItemSelection attached property.
        /// </summary>
        /// <param name="itemsControl">The visual from which to read the value of the ObservedItemSelection attached property.</param>
        /// <returns>The most recent selected item of the visual.</returns>
        public static object GetObservedItemSelection(this ItemsControl itemsControl)
        {
            return itemsControl.GetValue(ObservedItemSelectionProperty);
        }

        /// <summary>
        /// Set the value of the ObservedItemSelection attached property.
        /// </summary>
        /// <param name="itemsControl">The visual upon which to set the value of the ObservedItemSelection attached property.</param>
        /// <param name="selectedItem">The new value for the ObservedItemSelection property.</param>
        public static void SetObservedItemSelection(this ItemsControl itemsControl, object selectedItem)
        {
            itemsControl.SetValue(ObservedItemSelectionProperty, selectedItem);
        }

        /// <summary>
        /// Gets the currently selected item for the given control. See remarks.
        /// </summary>
        /// <param name="control">The control from which to retrieve the selected item. Must be an ItemSelector.</param>
        /// <returns>The selected item.</returns>
        /// <remarks>If the control is a ListBox, it this function returns <c>null</c> if multiple items, or no items, are selected. For other
        /// multi-select controls, the first selected element is returned.</remarks>
        internal static object GetSelectedItem(object control)
        {
            object selectedItem = null;
            var treeView = control as TreeView;
            var listBox = control as ListBox;
            var selector = control as Selector;
            if (treeView != null)
            {
                selectedItem = treeView.SelectedItem;
            }
            else if ((listBox != null) && (listBox.SelectedItems.Count == 1))
            {
                selectedItem = listBox.SelectedItem;
            }
            else if (selector != null)
            {
                selectedItem = selector.SelectedItem;
            }
            return selectedItem;
        }

        private static void SelectedItemObserverChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = o as FrameworkElement;
            var treeView = element as TreeView;
            var selector = element as Selector;
            if ((bool)e.NewValue)
            {
                if (selector != null)
                {
                    selector.SelectionChanged += OnSelectionChanged;
                }
                else if (treeView != null)
                {
                    treeView.SelectedItemChanged += OnSelectedItemChanged;
                }
                UpdateObservedSelectedItem(element, GetSelectedItem(o));
            }
            else
            {
                if (selector != null)
                {
                    selector.SelectionChanged -= OnSelectionChanged;
                }
            }
        }

        private static void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            UpdateObservedSelectedItem(treeView, GetSelectedItem(treeView));
        }

        private static void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;
            UpdateObservedSelectedItem(selector, GetSelectedItem(selector));
        }

        private static void UpdateObservedSelectedItem(FrameworkElement element, object newSelectedItem)
        {
            element.SetCurrentValue(ObservedItemSelectionProperty, newSelectedItem);
        }
    }
}

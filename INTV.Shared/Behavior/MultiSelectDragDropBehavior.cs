// <copyright file="MultiSelectDragDropBehavior.cs" company="INTV Funhouse">
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
using System.Windows;
using System.Windows.Controls;
using INTV.Shared.Utility;

using Key = System.Windows.Input.Key;
using Keyboard = System.Windows.Input.Keyboard;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// This class provides features to assist with multi-select drag and drop.
    /// </summary>
    public static class MultiSelectDragDropBehavior
    {
        /// <summary>
        /// This attached property stores whether or not an object supports multi-select drag and drop.
        /// </summary>
        public static readonly DependencyProperty AllowMultiSelectDragDropProperty = DependencyProperty.RegisterAttached("AllowMultiSelectDragDrop", typeof(bool), typeof(MultiSelectDragDropBehavior), new PropertyMetadata(AllowMultiSelectDragDropPropertyChangedCallBack));

        /// <summary>
        /// This attached property stores the initially clicked item for a multi-select drag-drop.
        /// </summary>
        private static readonly DependencyProperty MultiSelectDragDropDraggedItemProperty = DependencyProperty.RegisterAttached("MultiSelectDragDropDraggedItem", typeof(WeakReference), typeof(MultiSelectDragDropBehavior), new PropertyMetadata(AllowMultiSelectDragDropPropertyChangedCallBack));

        /// <summary>
        /// Property setter for AllowMultiSelectDragDrop.
        /// </summary>
        /// <param name="element">The visual to which to attach the AllowMultiSelectDragDrop property and its value.</param>
        /// <param name="allow">The value for the property.</param>
        public static void SetAllowMultiSelectDragDrop(this ItemsControl element, bool allow)
        {
            element.SetValue(AllowMultiSelectDragDropProperty, allow);
        }

        /// <summary>
        /// Property getter for AllowMultiSelectDragDrop.
        /// </summary>
        /// <param name="element">The visual from which the property is to be read.</param>
        /// <returns>The value of the property.</returns>
        public static bool GetAllowMultiSelectDragDrop(ItemsControl element)
        {
            return (bool)element.GetValue(AllowMultiSelectDragDropProperty);
        }

        private static void SetMultiSelectDragDropDraggedItem(this ItemsControl element, WeakReference clickedElement)
        {
            element.SetValue(MultiSelectDragDropDraggedItemProperty, clickedElement);
        }

        private static WeakReference GetMultiSelectDragDropDraggedItem(this ItemsControl element)
        {
            return element.GetValue(MultiSelectDragDropDraggedItemProperty) as WeakReference;
        }

        private static void AllowMultiSelectDragDropPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var visual = d as UIElement;
            if (visual != null)
            {
                visual.LostMouseCapture += SelectorLostMouseCapture;
                visual.PreviewMouseLeftButtonUp += ItemsControlPreviewMouseLeftButtonUp;
            }
        }

        /// <summary>
        /// This function implements additional bookkeeping necessary for multiple selection drag and drop to work. It is called from the DragStartBehavior.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Mouse button event argument data.</param>
        internal static void ItemsControlPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl != null)
            {
                var hitElement = itemsControl.InputHitTest(e.GetPosition(itemsControl)) as UIElement;
                var hitIsInAdorner = (hitElement != null) && (hitElement.GetParent<System.Windows.Documents.Adorner>() != null);
                if (!hitIsInAdorner)
                {
                    var clickedItem = GetClickedItem(itemsControl, e.OriginalSource as DependencyObject); // = (e.OriginalSource as DependencyObject).GetParent<System.Windows.Controls.ListViewItem>();
                    WeakReference clickedItemReference = null;
                    if (clickedItem != null)
                    {
                        if (clickedItem.IsSelected() && itemsControl.CaptureMouse())
                        {
                            clickedItemReference = new WeakReference(clickedItem);
                            ////e.Handled = true; // do NOT mark as handled -- otherwise double-click won't work properly
                        }
                        else
                        {
                            clickedItem = null;
                        }
                    }
                    itemsControl.SetMultiSelectDragDropDraggedItem(clickedItemReference);
                }
            }
        }

        private static void ItemsControlPreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl != null)
            {
                var clickedItemReference = itemsControl.GetMultiSelectDragDropDraggedItem();
                itemsControl.SetMultiSelectDragDropDraggedItem(null);
                var item = clickedItemReference.GetClickedItem();
                if ((item != null) && itemsControl.IsMouseCaptured)
                {
                    e.Handled = true;
                    itemsControl.ReleaseMouseCapture();

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        item.ToggleIsSelected();
                    }
                    else
                    {
                        var listView = itemsControl as ListView;
                        var indexOfItem = listView.SelectedItems.IndexOf(item.DataContext);
                        if ((listView != null) && ((indexOfItem < 0) || (listView.SelectedItems.Count > 1)))
                        {
                            var numItems = listView.SelectedItems.Count;
                            if (indexOfItem < 0)
                            {
                                for (var i = 0; i < numItems; ++i)
                                {
                                    listView.SelectedItems.RemoveAt(0);
                                }
                            }
                            else
                            {
                                for (var i = indexOfItem + 1; i < numItems; ++i)
                                {
                                    listView.SelectedItems.RemoveAt(indexOfItem + 1);
                                }
                                for (var i = 0; i < indexOfItem; ++i)
                                {
                                    listView.SelectedItems.RemoveAt(0);
                                }
                            }
                        }
                        item.SetIsSelected(true);
                    }

                    if (!item.IsKeyboardFocused)
                    {
                        item.Focus();
                    }
                }
            }
        }

        private static void SelectorLostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.SetMultiSelectDragDropDraggedItem(null);
            }
        }

        private static Control GetClickedItem(UIElement container, DependencyObject originalSource)
        {
            Control clickedItem = null;
            if (container is ListView)
            {
                clickedItem = originalSource.GetParent<ListViewItem>();
            }
            else if (container is TreeView)
            {
                clickedItem = originalSource.GetParent<TreeViewItem>();
            }
            return clickedItem;
        }

        private static Control GetClickedItem(this WeakReference clickedItemReference)
        {
            Control clickedItem = null;
            if ((clickedItemReference != null) && clickedItemReference.IsAlive)
            {
                clickedItem = clickedItemReference.Target as Control;
            }
            return clickedItem;
        }

        private static bool IsSelected(this Control item)
        {
            bool isSelected = false;
            if (item is ListViewItem)
            {
                isSelected = ((ListViewItem)item).IsSelected;
            }
            else if (item is TreeViewItem)
            {
                isSelected = ((TreeViewItem)item).IsSelected;
            }
            return isSelected;
        }

        private static void SetIsSelected(this Control item, bool isSelected)
        {
            if (item is ListViewItem)
            {
                ((ListViewItem)item).IsSelected = isSelected;
            }
            else if (item is TreeViewItem)
            {
                ((TreeViewItem)item).IsSelected = isSelected;
            }
        }

        private static void ToggleIsSelected(this Control item)
        {
            item.SetIsSelected(!item.IsSelected());
        }
    }
}

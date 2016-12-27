// <copyright file="DragScrollBehavior.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Attached behavior to assist with adding 'drag scroll' to a control.
    /// </summary>
    /// <remarks>Adapted from this article: http://www.codeproject.com/Tips/635510/WPF-Drag-Drop-Auto-scroll
    /// which adapted from a Stack Overflow version. It is released under CPOL:
    /// http://www.codeproject.com/info/cpol10.aspx 
    /// This version is has only been tested with TreeView controls, and is tuned to work with them.
    /// Furthermore, only non-virtualized display has been tested. I.e. the has only been tested when
    /// the CanContentScroll property, which is somewhat deceptively named, has been <c>false</c>.</remarks>
    public static class DragScrollBehavior
    {
        /// <summary>
        /// This attached property is set in order to declare whether or not control will scroll vertically when dragging something.
        /// </summary>
        public static readonly DependencyProperty AllowsDragScrollProperty = DependencyProperty.RegisterAttached("AllowsDragScroll", typeof(bool), typeof(DragScrollBehavior), new PropertyMetadata(false, OnAllowsDragScrollChanged));

        private static readonly DependencyProperty LastScrollTimeProperty = DependencyProperty.RegisterAttached("LastScrollTime", typeof(DateTime), typeof(DragScrollBehavior));

        /// <summary>
        /// Property setter for the AllowsDragScroll attached property.
        /// </summary>
        /// <param name="element">The visual that cares about the property.</param>
        /// <param name="allowsDragScroll">If <c>true</c>, enable scrolling when dragging something.</param>
        public static void SetAllowsDragScroll(this FrameworkElement element, bool allowsDragScroll)
        {
            element.SetValue(AllowsDragScrollProperty, allowsDragScroll);
        }

        /// <summary>
        /// Property getter for the AllowsDragScroll attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        public static bool GetAllowsDragScroll(this FrameworkElement element)
        {
            return (bool)element.GetValue(AllowsDragScrollProperty);
        }

        private static void SetLastScrollTime(this FrameworkElement element, DateTime lastScrollTime)
        {
            element.SetValue(LastScrollTimeProperty, lastScrollTime);
        }

        private static DateTime GetLastScrollTime(this FrameworkElement element)
        {
            return (DateTime)element.GetValue(LastScrollTimeProperty);
        }

        private static void OnAllowsDragScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                element.PreviewDragOver -= OnPreviewDragOver;
                if ((bool)e.NewValue)
                {
                    element.PreviewDragOver += OnPreviewDragOver;
                }
            }
        }

        private static void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var scrollViewer = element.FindChild<ScrollViewer>(v => IsDesiredScrollViewer(element, v));
                if (scrollViewer == null)
                {
                    return;
                }

                var lastScrollTime = element.GetLastScrollTime();
                var now = DateTime.UtcNow;
                var millisecondsElapsedSinceLastScroll = (now - lastScrollTime).Milliseconds;

                const int ItemBasedScrollingMillisecondsElapsedTime = 300; // slower scrolling for item-based scrolling
                const int ContentBasedScrollingMillisecondsElapsedTime = 20; // faster scrolling for generic content scrolling

                // The remarks regarding CanContentScroll property describe what it means when this value is true.
                // https://msdn.microsoft.com/en-us/library/system.windows.controls.scrollviewer.cancontentscroll(v=vs.110).aspx
                // When an ItemsControl is virtualized, this value must be true so that scrolling and presentation is done to show only entire
                // items in an ItemsControl. That scenario *HAS NOT BEEN TESTED* with this code.
                var useItemBasedScrolling = ScrollViewer.GetCanContentScroll(element);

                var requiredElapsedTime = useItemBasedScrolling ? ItemBasedScrollingMillisecondsElapsedTime : ContentBasedScrollingMillisecondsElapsedTime;
                if (millisecondsElapsedSinceLastScroll < requiredElapsedTime)
                {
                    return;
                }

                var scrollViewerHeight = scrollViewer.ActualHeight;
                var estimatedItemHeight = EstimateItemHeight(element, e);
                if (estimatedItemHeight > 0)
                {
                    // We've hit an item! Now let's sort out if it's at the top or bottom, and whether we should
                    // even bother trying to scroll.
                    var autoscrollItemHitSize = estimatedItemHeight * 0.75; // keep it a small region
                    var scrollAcceleration = useItemBasedScrolling ? 1.0 : 1.0; // scroll just a smidge faster?
                    var verticalPositionInScroller = e.GetPosition(scrollViewer).Y; // vertical location within scroll viewer
                    if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                    {
                        // Adjust for scrollbar height. Because we're using item height as the scroll amount, and hit testing for
                        // items to get it, scrollbar along the bottom of the viewer won't count. When scrollbar is visible, it consumes
                        // space at the bottom of the viewer.
                        var scrollbar = scrollViewer.FindChild<System.Windows.Controls.Primitives.ScrollBar>(s => s.Orientation == Orientation.Horizontal);
                        if (scrollbar != null)
                        {
                            scrollViewerHeight -= scrollbar.ActualHeight;
                        }
                    }

                    var scrollOffset = double.NaN; // set to valid number if we should actually scroll
                    if (verticalPositionInScroller <= autoscrollItemHitSize)
                    {
                        // scroll 'item height'-ish pixels up
                        scrollOffset = scrollViewer.VerticalOffset - (estimatedItemHeight * scrollAcceleration);
                    }
                    else if (verticalPositionInScroller >= (scrollViewerHeight - autoscrollItemHitSize))
                    {
                        // scroll 'item height'-ish pixels down
                        scrollOffset = scrollViewer.VerticalOffset + (estimatedItemHeight * scrollAcceleration);
                    }

                    if (!double.IsNaN(scrollOffset))
                    {
                        scrollViewer.ScrollToVerticalOffset(scrollOffset);
                        element.SetLastScrollTime(now);
                    }
                }
            }
        }

        private static bool IsDesiredScrollViewer(FrameworkElement parent, ScrollViewer scrollViewer)
        {
            var isTreeListView = parent is TreeListView;
            var isDesiredScrollViewer = !isTreeListView;
            if (isTreeListView)
            {
                isDesiredScrollViewer = scrollViewer.Name == "_tv_scrollviewer" || scrollViewer.Name == "_tv_scrollviewer_";
            }
            return isDesiredScrollViewer;
        }

        private static double EstimateItemHeight(FrameworkElement element, DragEventArgs args)
        {
            var itemHeight = 0.0;
            var itemsControl = element as ItemsControl;
            if (itemsControl != null)
            {
                var hitElement = itemsControl.InputHitTest(args.GetPosition(itemsControl)) as UIElement;
                var hitIsInAdorner = (hitElement != null) && (hitElement.GetParent<System.Windows.Documents.Adorner>() != null);
                if (!hitIsInAdorner)
                {
                    var treeViewItem = itemsControl.GetContainerAtPoint<TreeViewItem>(hitElement);
                    if (treeViewItem != null)
                    {
                        itemHeight = treeViewItem.ActualHeight;
                    }
                }
            }
            return itemHeight;
        }

        private static ItemVisual GetContainerAtPoint<ItemVisual>(this ItemsControl control, DependencyObject hitVisual) where ItemVisual : DependencyObject
        {
            while ((System.Windows.Media.VisualTreeHelper.GetParent(hitVisual) != null) && !(hitVisual is ItemVisual))
            {
                hitVisual = System.Windows.Media.VisualTreeHelper.GetParent(hitVisual);
            }
            return hitVisual as ItemVisual;
        }
    }
}

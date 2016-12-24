// <copyright file="SynchronizedScrollBehavior.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements a behavior to synchronize scrolling.
    /// </summary>
    /// <remarks>From CodeProject: http://www.codeproject.com/Articles/39244/Scroll-Synchronization </remarks>
    public static class SynchronizedScrollBehavior
    {
        /// <summary>
        /// This attached property is set in order to specify which synchronized scrolling group a ScrollViewer belongs to.
        /// </summary>
        public static readonly DependencyProperty ScrollGroupProperty = DependencyProperty.RegisterAttached("ScrollGroup", typeof(string), typeof(ScrollSynchronizer), new PropertyMetadata(ScrollSynchronizer.OnScrollGroupChanged));

        /// <summary>
        /// Property setter for the ScrollGroup attached property.
        /// </summary>
        /// <param name="scrollViewer">The ScrollViewer whose scroll group is being set.</param>
        /// <param name="scrollGroup">The name of the scroll group.</param>
        public static void SetScrollGroup(DependencyObject scrollViewer, string scrollGroup)
        {
            scrollViewer.SetValue(ScrollGroupProperty, scrollGroup);
        }

        /// <summary>
        /// Property getter for the ScrollGroup attached property.
        /// </summary>
        /// <param name="scrollViewer">The ScrollViewer whose scroll group is being retrieved.</param>
        /// <returns>The name of the scroll group, or <c>null</c> if it not a member of one.</returns>
        public static string GetScrollGroup(DependencyObject scrollViewer)
        {
            return (string)scrollViewer.GetValue(ScrollGroupProperty);
        }

        private class ScrollSynchronizer : DependencyObject
        {
            private static Dictionary<ScrollViewer, string> scrollViewers = new Dictionary<ScrollViewer, string>();
            private static Dictionary<string, double> horizontalScrollOffsets = new Dictionary<string, double>();
            private static Dictionary<string, double> verticalScrollOffsets = new Dictionary<string, double>();

            /// <summary>
            /// This is the value change handler for when the ScrollGroup attached property changes.
            /// </summary>
            /// <param name="d">The ScrollViewer whose ScrollGroup property is changing.</param>
            /// <param name="e">The dependency property changed arguments.</param>
            internal static void OnScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var scrollViewer = d as ScrollViewer;
                if (scrollViewer != null)
                {
                    if (!string.IsNullOrEmpty((string)e.OldValue))
                    {
                        // Remove scroll viewer
                        if (scrollViewers.ContainsKey(scrollViewer))
                        {
                            scrollViewer.ScrollChanged -= new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
                            scrollViewers.Remove(scrollViewer);
                        }
                    }

                    if (!string.IsNullOrEmpty((string)e.NewValue))
                    {
                        // If group already exists, set scroll position of the new scroll viewer to the scroll position of the group.
                        if (horizontalScrollOffsets.Keys.Contains((string)e.NewValue))
                        {
                            scrollViewer.ScrollToHorizontalOffset(horizontalScrollOffsets[(string)e.NewValue]);
                        }
                        else
                        {
                            horizontalScrollOffsets.Add((string)e.NewValue, scrollViewer.HorizontalOffset);
                        }

                        if (verticalScrollOffsets.Keys.Contains((string)e.NewValue))
                        {
                            scrollViewer.ScrollToVerticalOffset(verticalScrollOffsets[(string)e.NewValue]);
                        }
                        else
                        {
                            verticalScrollOffsets.Add((string)e.NewValue, scrollViewer.VerticalOffset);
                        }

                        // Add scroll viewer.
                        scrollViewers.Add(scrollViewer, (string)e.NewValue);
                        scrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
                    }
                }
            }

            private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                if (e.VerticalChange != 0 || e.HorizontalChange != 0)
                {
                    var changedScrollViewer = sender as ScrollViewer;
                    Scroll(changedScrollViewer);
                }
            }

            private static void Scroll(ScrollViewer changedScrollViewer)
            {
                var group = scrollViewers[changedScrollViewer];
                verticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;
                horizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;

                foreach (var scrollViewer in scrollViewers.Where((s) => s.Value == group && s.Key != changedScrollViewer))
                {
                    if (scrollViewer.Key.VerticalOffset != changedScrollViewer.VerticalOffset)
                    {
                        scrollViewer.Key.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
                    }

                    if (scrollViewer.Key.HorizontalOffset != changedScrollViewer.HorizontalOffset)
                    {
                        scrollViewer.Key.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
                    }
                }
            }
        }
    }
}

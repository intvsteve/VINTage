// <copyright file="ShowHideGridViewColumnBehavior.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using INTV.Shared.Utility;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements an attached behavior to show or hide columns in a GridView.
    /// </summary>
    public static class ShowHideGridViewColumnBehavior
    {
        private const int IgnoreVisibilityChange = -2;
        private const double DefaultInitialWidth = 128;

        /// <summary>
        /// This attached property is set to <c>true</c> to indicate that the attached behavior to show or hide columns in a GridView should be enabled.
        /// </summary>
        public static readonly DependencyProperty AllowHideColumnsProperty = DependencyProperty.RegisterAttached("AllowHideColumns", typeof(bool), typeof(ShowHideGridViewColumnBehavior), new PropertyMetadata(AllowHideColumnsPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the AllowHideColumns attached property.
        /// </summary>
        /// <param name="control">The visual upon which to set the property.</param>
        /// <param name="allowHideColumns">The value of the property.</param>
        public static void SetAllowHideColumns(ItemsControl control, bool allowHideColumns)
        {
            control.SetValue(AllowHideColumnsProperty, allowHideColumns);
        }

        /// <summary>
        /// Property getter for the AllowHideColumns attached property.
        /// </summary>
        /// <param name="control">The visual from which to get the property.</param>
        /// <returns>The value of the property.</returns>
        private static bool GetAllowHideColumns(ItemsControl control)
        {
            return (bool)control.GetValue(AllowHideColumnsProperty);
        }

        /// <summary>
        /// This attached property is used to show or hide a column in a GridView control.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(ShowHideGridViewColumnBehavior), new PropertyMetadata(true, IsVisiblePropertyChangedCallBack));

        /// <summary>
        /// Property setter for the IsVisible attached property.
        /// </summary>
        /// <param name="column">The visual upon which to set the property.</param>
        /// <param name="isVisible">The value of the property.</param>
        public static void SetIsVisible(this GridViewColumn column, bool isVisible)
        {
            column.SetValue(IsVisibleProperty, isVisible);
        }

        /// <summary>
        /// Property getter for the IsVisible attached property.
        /// </summary>
        /// <param name="column">The visual from which to get the property.</param>
        /// <returns>The value of the property.</returns>
        public static bool GetIsVisible(this GridViewColumn column)
        {
            return (bool)column.GetValue(IsVisibleProperty);
        }

        /// <summary>
        /// This attached property is used to set the minimum width for a column in a GridView.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(ShowHideGridViewColumnBehavior));

        /// <summary>
        /// Property setter for the MinWidth attached property.
        /// </summary>
        /// <param name="column">The visual upon which to set the property.</param>
        /// <param name="minWidth">The value of the property.</param>
        public static void SetMinWidth(this GridViewColumn column, double minWidth)
        {
            column.SetValue(IsVisibleProperty, minWidth);
        }

        /// <summary>
        /// Property getter for the MinWidth attached property.
        /// </summary>
        /// <param name="column">The visual from which to get the property.</param>
        /// <returns>The value of the property.</returns>
        public static double GetMinWidth(this GridViewColumn column)
        {
            return (double)column.GetValue(MinWidthProperty);
        }

        private static readonly DependencyProperty OriginalColumnsProperty = DependencyProperty.RegisterAttached("OriginalColumns", typeof(List<GridViewColumn>), typeof(ShowHideGridViewColumnBehavior));

        private static void SetOriginalColumns(this System.Windows.Controls.Primitives.GridViewRowPresenterBase presenter, List<GridViewColumn> columns)
        {
            presenter.SetValue(OriginalColumnsProperty, columns);
        }

        private static List<GridViewColumn> GetOriginalColumns(this GridViewRowPresenter presenter)
        {
            return presenter.GetValue(OriginalColumnsProperty) as List<GridViewColumn>;
        }

        private static readonly DependencyProperty RowPresenterProperty = DependencyProperty.RegisterAttached("RowPresenter", typeof(WeakReference), typeof(ShowHideGridViewColumnBehavior));

        private static void SetRowPresenter(this GridViewColumn column, WeakReference rowPresenterReference)
        {
            column.SetValue(RowPresenterProperty, rowPresenterReference);
        }

        private static WeakReference GetRowPresenter(this GridViewColumn column)
        {
            return column.GetValue(RowPresenterProperty) as WeakReference;
        }

        private static void IsVisiblePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = (GridViewColumn)d;
            var rowPresenterReference = GetRowPresenter(column);
            if ((rowPresenterReference != null) && !rowPresenterReference.IsAlive)
            {
                // If the presenter is gone, try to get it again -- this can happen if collections are replaced or undergo massive changes
                System.Diagnostics.Debug.WriteLine("Where did that RowPresenter go? Ugh.");
            }

            // will be null until the full control is loaded
            if ((rowPresenterReference != null) && rowPresenterReference.IsAlive) 
            {
                var rowPresenter = (GridViewRowPresenter)rowPresenterReference.Target;
                bool makeVisible = (bool)e.NewValue;
                if (makeVisible)
                {
                    var originalColumns = rowPresenter.GetOriginalColumns();
                    var insertLocation = FindInsertLocation(rowPresenter.Columns, originalColumns, column);
                    if (insertLocation != IgnoreVisibilityChange)
                    {
                        if (insertLocation < 0)
                        {
                            rowPresenter.Columns.Add(column);
                        }
                        else
                        {
                            rowPresenter.Columns.Insert(insertLocation, column);
                        }
                        if (column.Width == 0)
                        {
                            var minWidth = column.GetMinWidth();
                            if ((minWidth > 0) && (column.ActualWidth < minWidth))
                            {
                                column.Width = minWidth;
                            }
                            else if (column.ActualWidth <= 0)
                            {
                                column.Width = DefaultInitialWidth;
                            }
                        }
                    }
                }
                else
                {
                    rowPresenter.Columns.Remove(column);
                }
            }
        }

        private static void AllowHideColumnsPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = d as ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.Loaded += ControlLoaded;
            }
        }

        private static void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var itemsControl = (ItemsControl)sender;
            itemsControl.Loaded -= ControlLoaded;
            System.Windows.Controls.Primitives.GridViewRowPresenterBase gridViewRowPresenter = itemsControl.FindChild<GridViewRowPresenter>();
            if (gridViewRowPresenter == null)
            {
                gridViewRowPresenter = itemsControl.FindChild<GridViewHeaderRowPresenter>();
            }
            if (gridViewRowPresenter != null)
            {
                var columns = gridViewRowPresenter.Columns;
                SetOriginalColumns(gridViewRowPresenter, columns.ToList());
                var columnsToHide = new List<GridViewColumn>();
                foreach (var column in columns)
                {
                    SetRowPresenter(column, new WeakReference(gridViewRowPresenter));
                    if (!column.GetIsVisible())
                    {
                        columnsToHide.Add(column);
                    }
                }
                foreach (var column in columnsToHide)
                {
                    columns.Remove(column);
                }
            }
        }

        private static void InitializeColumns(ItemsControl itemsControl)
        {
        }

        private static int FindInsertLocation(GridViewColumnCollection currentColumns, List<GridViewColumn> originalColumns, GridViewColumn column)
        {
            int insertLocation = IgnoreVisibilityChange;
            var currentLocation = currentColumns.IndexOf(column);
            if (currentLocation < 0)
            {
                var originalLocation = originalColumns.IndexOf(column);
                if (originalLocation > 0)
                {
                    // try to find where to insert based on neighbors
                    if (originalLocation >= (originalColumns.Count - 1))
                    {
                        // we were the last column, so stay there.
                        insertLocation = -1;
                    }
                    else
                    {
                        insertLocation = originalLocation;
                    }
                }
                else if (originalLocation < 0)
                {
                    throw new InvalidOperationException();
                }
                ////else we were the first column, so stay there
            }
            return insertLocation;
        }
    }
}

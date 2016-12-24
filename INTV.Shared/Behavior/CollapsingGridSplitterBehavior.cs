// <copyright file="CollapsingGridSplitterBehavior.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements a way to get around the problem that if you collapse a GridSplitter and the
    /// visual associated with one side of the it, the containing Grid will not reclaim the emptied area.
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/12483017/wpf-collapse-gridsplitter"/>
    public static class CollapsingGridSplitterBehavior
    {
        #region Attached Properties

        #region CollapsibleGridSplitter

        /// <summary>
        /// This attached property is used to enable the collapsible GridSplitter feature on a control.
        /// </summary>
        /// <remarks>The typical usage is to have a <see cref="Grid"/> attach a <see cref="GridSplitter"/> value to itself to
        /// indicate the splitter to maintain. NOTE: Only ONE GridSplitter per Grid is maintained in this fashion!</remarks>
        public static readonly DependencyProperty CollapsibleGridSplitterProperty = DependencyProperty.RegisterAttached("CollapsibleGridSplitter", typeof(GridSplitter), typeof(CollapsingGridSplitterBehavior), new PropertyMetadata(OnCollapsibleGridSplitterChanged));

        /// <summary>
        /// Gets the value of the CollapsibleGridSplitterProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>The GridSplitter needing collapsible behavior.</returns>
        public static GridSplitter GetCollapsibleGridSplitter(DependencyObject control)
        {
            return (GridSplitter)control.GetValue(CollapsibleGridSplitterProperty);
        }

        /// <summary>
        /// Sets the value of the CollapsibleGridSplitterProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="gridSplitter">The GridSplitter desiring the collapsible behavior.</param>
        public static void SetCollapsibleGridSplitter(DependencyObject control, GridSplitter gridSplitter)
        {
            control.SetValue(CollapsibleGridSplitterProperty, gridSplitter);
        }

        #endregion // CollapsibleGridSplitter

        #region GridSplitterLengthsCache

        /// <summary>
        /// This attached property is used to cache the sizes of collapsed elements when hiding a GridSplitter.
        /// </summary>
        private static readonly DependencyProperty GridSplitterLengthsCacheProperty = DependencyProperty.RegisterAttached("GridSplitterLengthsCache", typeof(WeakKeyDictionary<DefinitionBase, GridLength>), typeof(CollapsingGridSplitterBehavior));

        /// <summary>
        /// Gets the value of the GridSplitterLengthsCacheProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>A dictionary containing the cached GridLength values of the splitter and its neighbors..</returns>
        private static WeakKeyDictionary<DefinitionBase, GridLength> GetGridSplitterLengthsCache(DependencyObject control)
        {
            return control.GetValue(GridSplitterLengthsCacheProperty) as WeakKeyDictionary<DefinitionBase, GridLength>;
        }

        /// <summary>
        /// Sets the value of the GridSplitterLengthsCacheProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="gridLengthsCache">A dictionary of cached GridLength values.</param>
        private static void SetGridSplitterLengthsCache(DependencyObject control, WeakKeyDictionary<DefinitionBase, GridLength> gridLengthsCache)
        {
            control.SetValue(GridSplitterLengthsCacheProperty, gridLengthsCache);
        }

        #endregion // GridSplitterLengthsCache

        #endregion // Attached Properties

        #region Event Handlers

        private static void OnCollapsibleGridSplitterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if (grid != null)
            {
                var gridSplitter = e.NewValue as GridSplitter;
                if (gridSplitter != null)
                {
                    UpdateGrid(grid, gridSplitter, gridSplitter.IsVisible); // initialize values in the cache
                    gridSplitter.IsVisibleChanged += (sender, args) => UpdateGrid(grid, (GridSplitter)sender, (bool)args.NewValue);
                }
            }
        }

        private static void UpdateGrid(Grid grid, GridSplitter gridSplitter, bool isVisible)
        {
            // Get the definitions of interest. These include the splitter's and its neighbors.
            var index = gridSplitter.GetIndex();
            var definitions = grid.GetDefinitions(gridSplitter);
            var splitterDefinition = definitions.ElementAt(index);
            var previousDefinition = definitions.ElementAt(gridSplitter.GetPreviousIndex(index));
            var nextDefinition = definitions.ElementAt(gridSplitter.GetNextIndex(index));

            var gridLengthsCache = GetGridSplitterLengthsCache(grid);
            if (gridLengthsCache == null)
            {
                // No cache, so create and store one.
                gridLengthsCache = new WeakKeyDictionary<DefinitionBase, GridLength>();
                SetGridSplitterLengthsCache(grid, gridLengthsCache);
            }

            if (gridLengthsCache.ContainsKey(splitterDefinition) && gridLengthsCache.ContainsKey(previousDefinition) && gridLengthsCache.ContainsKey(nextDefinition))
            {
                if (isVisible)
                {
                    // Restore to cached size.
                    splitterDefinition.SetLength(gridLengthsCache[splitterDefinition]);
                    previousDefinition.SetLength(gridLengthsCache[previousDefinition]);
                    nextDefinition.SetLength(gridLengthsCache[nextDefinition]);
                }
                else
                {
                    // Ensure collapsed. Update the cached sizes if necessary.
                    splitterDefinition.HideDefinitionAndCacheLength(gridLengthsCache);
                    previousDefinition.HideDefinitionAndCacheLength(gridLengthsCache);
                    nextDefinition.HideDefinitionAndCacheLength(gridLengthsCache);
                }
            }
            else
            {
                // We haven't cached the GridLength values, or at least one of the cached definitions somehow went bad. Store the current values.
                // Since the cache is out-of-date, leave everything alone.
                gridLengthsCache[splitterDefinition] = splitterDefinition.GetLength();
                gridLengthsCache[previousDefinition] = previousDefinition.GetLength();
                gridLengthsCache[nextDefinition] = nextDefinition.GetLength();
            }
        }

        #endregion // Event Handlers

        #region Grid Extension Methods

        private static IEnumerable<DefinitionBase> GetDefinitions(this Grid grid, GridSplitter gridSplitter)
        {
            var definitions = Enumerable.Empty<DefinitionBase>();
            switch (gridSplitter.GetResizeDirection())
            {
                case GridResizeDirection.Columns:
                    definitions = grid.ColumnDefinitions;
                    break;
                case GridResizeDirection.Rows:
                    definitions = grid.RowDefinitions;
                    break;
            }
            return definitions;
        }

        #endregion // Grid Extension Methods

        #region GridSplitter Extension Methods

        private static int GetIndex(this GridSplitter gridSplitter)
        {
            var index = -1;
            switch (gridSplitter.GetResizeDirection())
            {
                case GridResizeDirection.Columns:
                    index = (int)gridSplitter.GetValue(Grid.ColumnProperty);
                    break;
                case GridResizeDirection.Rows:
                    index = (int)gridSplitter.GetValue(Grid.RowProperty);
                    break;
            }
            return index;
        }

        private static int GetPreviousIndex(this GridSplitter gridSplitter, int index)
        {
            switch (gridSplitter.GetResizeBehavior())
            {
                case GridResizeBehavior.PreviousAndNext:
                case GridResizeBehavior.PreviousAndCurrent:
                    return index - 1;
                case GridResizeBehavior.CurrentAndNext:
                    return index;
                case GridResizeBehavior.BasedOnAlignment:
                default:
                    throw new NotSupportedException();
            }
        }

        private static int GetNextIndex(this GridSplitter gridSplitter, int index)
        {
            switch (gridSplitter.GetResizeBehavior())
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    return index;
                case GridResizeBehavior.PreviousAndNext:
                case GridResizeBehavior.CurrentAndNext:
                    return index + 1;
                case GridResizeBehavior.BasedOnAlignment:
                default:
                    throw new NotSupportedException();
            }
        }

        private static GridResizeDirection GetResizeDirection(this GridSplitter gridSplitter)
        {
            // The following logic is based on documentation at: https://msdn.microsoft.com/en-us/library/system.windows.controls.gridresizedirection(v=vs.110).aspx
            var resizeDirection = gridSplitter.ResizeDirection;
            if (gridSplitter.ResizeDirection == GridResizeDirection.Auto)
            {
                if (gridSplitter.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    if (gridSplitter.VerticalAlignment != VerticalAlignment.Stretch)
                    {
                        resizeDirection = GridResizeDirection.Rows;
                    }
                    else
                    {
                        resizeDirection = gridSplitter.ActualWidth <= gridSplitter.ActualHeight ? GridResizeDirection.Columns : GridResizeDirection.Rows;
                    }
                }
                else
                {
                    resizeDirection = GridResizeDirection.Columns;
                }
            }
            return resizeDirection;
        }

        private static GridResizeBehavior GetResizeBehavior(this GridSplitter gridSplitter)
        {
            // The following logic is based on documentation at: https://msdn.microsoft.com/en-us/library/system.windows.controls.gridresizebehavior(v=vs.110).aspx
            var resizeBehavior = gridSplitter.ResizeBehavior;
            if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
            {
                if (gridSplitter.GetResizeDirection() == GridResizeDirection.Rows)
                {
                    switch (gridSplitter.VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case VerticalAlignment.Bottom:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        case VerticalAlignment.Center:
                        case VerticalAlignment.Stretch:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
                else
                {
                    switch (gridSplitter.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case HorizontalAlignment.Right:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        case HorizontalAlignment.Center:
                        case HorizontalAlignment.Stretch:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
            }
            return resizeBehavior;
        }

        #endregion // GridSplitter Extension Methods

        #region DefinitionBase Extension Methods

        private static GridLength GetLength(this DefinitionBase definition)
        {
            var rowDefinition = definition as RowDefinition;
            if (rowDefinition != null)
            {
                return rowDefinition.Height;
            }
            else
            {
                return ((ColumnDefinition)definition).Width;
            }
        }

        private static void SetLength(this DefinitionBase definition, GridLength length)
        {
            var rowDefinition = definition as RowDefinition;
            if (rowDefinition != null)
            {
                rowDefinition.Height = length;
            }
            else
            {
                ((ColumnDefinition)definition).Width = length;
            }
        }

        private static void HideDefinitionAndCacheLength(this DefinitionBase definition, WeakKeyDictionary<DefinitionBase, GridLength> gridLengthsCache)
        {
            var currentLength = definition.GetLength();
            switch (currentLength.GridUnitType)
            {
                case GridUnitType.Auto:
                case GridUnitType.Star:
                    // If the current unit is Auto or Star, we don't need to update the current or cached value.
                    break;
                case GridUnitType.Pixel:
                    // If current unit is Pixel, we need to set length to zero to hide the definition and update the cached value.
                    gridLengthsCache[definition] = currentLength;
                    definition.SetLength(new GridLength(0));
                    break;
            }
        }

        #endregion // DefinitionBase Extension Methods
    }
}

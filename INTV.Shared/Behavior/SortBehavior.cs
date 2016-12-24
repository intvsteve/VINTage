// <copyright file="SortBehavior.cs" company="INTV Funhouse">
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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Defines attached behaviors to make custom sort behaviors easier to accomplish in XAML, and with very little code-behind (only the actual sort command).
    /// </summary>
    /// <remarks>From a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class SortBehavior
    {
        /// <summary>
        /// Setting this property will cause a command to execute when the visual to which it is bound is clicked.
        /// </summary>
        public static readonly DependencyProperty SortCommandProperty = DependencyProperty.RegisterAttached("SortCommand", typeof(ICommand), typeof(SortBehavior), new PropertyMetadata(SortCommandPropertyChangedCallback));

        /// <summary>
        /// Property setter for the SortCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the sort command.</param>
        /// <param name="command">The command to execute to handle the sort.</param>
        public static void SetSortCommand(this FrameworkElement element, ICommand command)
        {
            element.SetValue(SortCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the SortCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static ICommand GetSortCommand(FrameworkElement element)
        {
            return element.GetValue(SortCommandProperty) as ICommand;
        }

        /// <summary>
        /// This attached property is used to initialize or track whether a sort should be in ascending or descending order.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.RegisterAttached("SortDirection", typeof(ListSortDirection?), typeof(SortBehavior), new PropertyMetadata(SortColumnOrDirectionChanged));

        /// <summary>
        /// Property setter for the SortDirection attached property.
        /// </summary>
        /// <param name="element">The visual that displays the data to sort.</param>
        /// <param name="direction">The order in which to sort the data.</param>
        public static void SetSortDirection(this FrameworkElement element, ListSortDirection direction)
        {
            element.SetValue(SortDirectionProperty, direction);
        }

        /// <summary>
        /// Property getter for the SortDirection attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static ListSortDirection? GetSortDirection(FrameworkElement element)
        {
            return (ListSortDirection?)element.GetValue(SortDirectionProperty);
        }

        /// <summary>
        /// This attached property is used to initialize or track which column of data in a table is the one upon which to perform the sorting command.
        /// </summary>
        public static readonly DependencyProperty SortColumnProperty = DependencyProperty.RegisterAttached("SortColumn", typeof(object), typeof(SortBehavior), new PropertyMetadata(SortColumnOrDirectionChanged));

        /// <summary>
        /// Property setter for the SortColumn attached property.
        /// </summary>
        /// <param name="element">The visual that displays the data to sort.</param>
        /// <param name="sortColumn">The column to act as the primary sort criteria.</param>
        public static void SetSortColumn(this FrameworkElement element, object sortColumn)
        {
            element.SetValue(SortColumnProperty, sortColumn);
        }

        /// <summary>
        /// Property getter for the SortColumn attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static object GetSortColumn(FrameworkElement element)
        {
            return element.GetValue(SortColumnProperty);
        }

        /// <summary>
        /// This attached property is used to display a glyph indicating an ascending sort.
        /// </summary>
        public static readonly DependencyProperty SortAscendingGlyphResourceNameProperty = DependencyProperty.RegisterAttached("SortAscendingGlyphResourceName", typeof(string), typeof(SortBehavior));

        /// <summary>
        /// Property setter for the SortAscendingGlyphResourceName attached property.
        /// </summary>
        /// <param name="element">The visual that will be displaying the glyph.</param>
        /// <param name="resourceName">The name of the resource to use to display the sorting glyph.</param>
        public static void SetSortAscendingGlyphResourceName(this FrameworkElement element, string resourceName)
        {
            element.SetValue(SortAscendingGlyphResourceNameProperty, resourceName);
        }

        /// <summary>
        /// Property getter for the SortAscendingGlyphResourceName attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static string GetSortAscendingGlyphResourceName(FrameworkElement element)
        {
            return element.GetValue(SortAscendingGlyphResourceNameProperty) as string;
        }

        /// <summary>
        /// This attached property is used to display a glyph indicating a descending sort.
        /// </summary>
        public static readonly DependencyProperty SortDescendingGlyphResourceNameProperty = DependencyProperty.RegisterAttached("SortDescendingGlyphResourceName", typeof(string), typeof(SortBehavior));

        /// <summary>
        /// Property setter for the attached SortDescendingGlyphResourceName attached property.
        /// </summary>
        /// <param name="element">The visual that will be displaying the glyph.</param>
        /// <param name="resourceName">The name of the resource to use to display the sorting glyph.</param>
        public static void SetSortDescendingGlyphResourceName(this FrameworkElement element, string resourceName)
        {
            element.SetValue(SortDescendingGlyphResourceNameProperty, resourceName);
        }

        /// <summary>
        /// Property getter for the attached SortDescendingGlyphResourceName attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static string GetSortDescendingGlyphResourceName(FrameworkElement element)
        {
            return element.GetValue(SortDescendingGlyphResourceNameProperty) as string;
        }

        private static void SortColumnOrDirectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = (FrameworkElement)o;
            GridViewColumnHeader newSortColumn = null;
            GridViewColumnHeader oldSortColumn = null;
            ListSortDirection? sortDirection = null;
            switch (e.Property.Name)
            {
                case "SortColumn":
                    newSortColumn = e.NewValue as GridViewColumnHeader;
                    oldSortColumn = e.OldValue as GridViewColumnHeader;
                    sortDirection = GetSortDirection((FrameworkElement)o);
                    break;
                case "SortDirection":
                    newSortColumn = GetSortColumn((FrameworkElement)o) as GridViewColumnHeader;
                    if (e.NewValue != null)
                    {
                        sortDirection = (ListSortDirection)e.NewValue;
                    }
                    break;
            }
            if ((newSortColumn != null) && sortDirection.HasValue)
            {
                var headerTemplateArrowName = (sortDirection == ListSortDirection.Ascending) ? GetSortAscendingGlyphResourceName(visual) : GetSortDescendingGlyphResourceName(visual);
                if (!string.IsNullOrWhiteSpace(headerTemplateArrowName))
                {
                    // Update the sorting glyph after load. We need to ensure that, if this executes during initialization code, that
                    // the visual tree is in a suitable state to set the glyph.
                    newSortColumn.Dispatcher.BeginInvoke(
                        new System.Action(() =>
                        {
                            newSortColumn.Column.HeaderTemplate = newSortColumn.TryFindResource(headerTemplateArrowName) as DataTemplate;

                            // Remove glyph from previous sort column.
                            if ((oldSortColumn != null) && (oldSortColumn != newSortColumn))
                            {
                                oldSortColumn.Column.HeaderTemplate = null;
                            }
                        }),
                        System.Windows.Threading.DispatcherPriority.Loaded);
                }
            }
        }

        private static void SortCommandPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as FrameworkElement;
            if (visual != null)
            {
                visual.AddHandler(
                    ButtonBase.ClickEvent,
                    new RoutedEventHandler((sender, args) =>
                    {
                        var columnClicked = args.OriginalSource as ButtonBase;
                        if (columnClicked != null)
                        {
                            bool toggleSortDirection = true;
                            var sortDirection = ListSortDirection.Ascending;
                            var headerClicked = columnClicked as GridViewColumnHeader;
                            bool doTheSort = headerClicked != null;
                            var previousHeader = GetSortColumn(visual);
                            if (doTheSort)
                            {
                                doTheSort = headerClicked.Role != GridViewColumnHeaderRole.Padding;
                                if (doTheSort)
                                {
                                    previousHeader = GetSortColumn(visual) as GridViewColumnHeader;
                                    toggleSortDirection = headerClicked == previousHeader;
                                }
                            }
                            if (doTheSort)
                            {
                                if (toggleSortDirection)
                                {
                                    var previousSortDirection = GetSortDirection(visual);
                                    if (previousSortDirection.HasValue)
                                    {
                                        if (previousSortDirection.Value == ListSortDirection.Ascending)
                                        {
                                            sortDirection = ListSortDirection.Descending;
                                        }
                                        else
                                        {
                                            sortDirection = ListSortDirection.Ascending;
                                        }
                                    }
                                }
                                var sortColumn = columnClicked.Tag;
                                var sortData = new SortCommandData(sortDirection, sortColumn);

                                var command = GetSortCommand(visual);
                                if (command.CanExecute(sortData))
                                {
                                    command.Execute(sortData);
                                    SetSortColumn(visual, columnClicked);
                                    SetSortDirection(visual, sortDirection);
                                }
                            }
                            args.Handled = doTheSort;
                        }
                    }));
            }
        }
    }
}

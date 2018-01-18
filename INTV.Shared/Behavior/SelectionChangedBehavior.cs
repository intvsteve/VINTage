// <copyright file="SelectionChangedBehavior.cs" company="INTV Funhouse">
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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Defines an attached behavior so you can more easily be notified of selection changed behaviors on Selector objects - specifically MultiSelect cases.
    /// </summary>
    /// <remarks>Inspired by a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class SelectionChangedBehavior
    {
        /// <summary>
        /// This attached property provides a property to bind to for the selected items in a multi-select control.
        /// </summary>
        public static readonly DependencyProperty BindableSelectedItemsProperty = DependencyProperty.RegisterAttached("BindableSelectedItems", typeof(IList), typeof(SelectionChangedBehavior), new PropertyMetadata(BindableSelectedItemsPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the BindableSelectedItems attached property.
        /// </summary>
        /// <param name="element">The visual upon which to set the property value.</param>
        /// <param name="selectedItems">The value of the property.</param>
        public static void SetBindableSelectedItems(this Selector element, IList selectedItems)
        {
            element.SetValue(BindableSelectedItemsProperty, selectedItems);
        }

        /// <summary>
        /// Property getter for the BindableSelectedItems attached property.
        /// </summary>
        /// <param name="element">The visual from which to get the property value.</param>
        /// <returns>The value of the property.</returns>
        private static IList GetBindableSelectedItems(Selector element)
        {
            return element.GetValue(BindableSelectedItemsProperty) as IList;
        }

        private static void BindableSelectedItemsPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as Selector;
            if (visual != null)
            {
                var targetCollection = e.NewValue as INTV.Shared.ViewModel.IObservableViewModelCollection;
                if (targetCollection != null)
                {
                    targetCollection.SourceCollection = GetSelectedItemsCollection(visual);
                }
            }
        }

        private static IList GetSelectedItemsCollection(Selector selector)
        {
            if (selector is MultiSelector)
            {
                return (selector as MultiSelector).SelectedItems;
            }
            else if (selector is ListBox)
            {
                return (selector as ListBox).SelectedItems;
            }
            else
            {
                throw new InvalidOperationException(Resources.Strings.SelectionChangedBehavior_NoSelectedItemsProperty);
            }
        }
    }
}

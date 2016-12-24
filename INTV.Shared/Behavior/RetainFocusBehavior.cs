// <copyright file="RetainFocusBehavior.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements an attached behavior that will restore focus to a control.
    /// </summary>
    /// <remarks>Adapted from Avanka's post, and Zamotic's extension to it, on stackoverflow.com:
    /// http://stackoverflow.com/questions/1356045/set-focus-on-textbox-in-wpf-from-view-model-c-wpf </remarks>
    public static class RetainFocusBehavior
    {
        /// <summary>
        /// The attached property to set to force a control to set focus on itself.
        /// </summary>
        /// <remarks>The value of this property does not matter. Whenever it changes, the control will attempt to set focus to itself if the new value is not
        /// zero, and is different than the previous value. If the control to which this property is attached is an ItemsControl, the focus will be retained only
        /// if the ItemsControl has at least one item in it.</remarks>
        public static readonly DependencyProperty RetainFocusProperty = DependencyProperty.RegisterAttached("RetainFocus", typeof(int), typeof(RetainFocusBehavior), new PropertyMetadata(OnRetainFocusPropertyChanged));

        /// <summary>
        /// Property setter for the RetainFocus attached property.
        /// </summary>
        /// <param name="element">The visual whose RetainFocus property is being set.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetRetainFocus(DependencyObject element, int value)
        {
            element.SetValue(RetainFocusProperty, value);
        }

        /// <summary>
        /// Property getter for the RetainFocus attached property.
        /// </summary>
        /// <param name="element">The visual whose RetainFocus property is being read.</param>
        /// <returns>The value of the property.</returns>
        public static int GetRetainFocus(DependencyObject element)
        {
            return (int)element.GetValue(RetainFocusProperty);
        }

        private static void OnRetainFocusPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)o;
            var itemsControl = element as System.Windows.Controls.ItemsControl;

            // Focus is retained only if the new value is different, and, in the case of an ItemsControl the ItemsControl has any items.
            if (((int)e.NewValue != 0) && (e.NewValue != e.OldValue) && ((itemsControl == null) || itemsControl.HasItems))
            {
                element.Focus();
            }
        }
    }
}

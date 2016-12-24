// <copyright file="DragDropRearrangeBehavior.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Behavior
{
    public static class DragDropRearrangeBehavior
    {
        /// <summary>
        /// This attached property is set in order to declare whether or not an ItemsControl allows drag/drop rearrangement of its items.
        /// </summary>
        public static readonly DependencyProperty AllowsDragDropRearrangeProperty = DependencyProperty.RegisterAttached("AllowsDragDropRearrange", typeof(bool), typeof(DragDropRearrangeBehavior), new FrameworkPropertyMetadata() { Inherits = true });

        /// <summary>
        /// Property setter for the AllowsDragDropRearrange attached property.
        /// </summary>
        /// <param name="element">The visual that cares about the property.</param>
        /// <param name="allowsDragDropRearrange">If <c>true</c>, use a drag/drop adorner to indicate item rearrangement.</param>
        public static void SetAllowsDragDropRearrange(this ItemsControl element, bool allowsDragDropRearrange)
        {
            element.SetValue(AllowsDragDropRearrangeProperty, allowsDragDropRearrange);
        }

        /// <summary>
        /// Property getter for the AllowsDragDropRearrange attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        public static bool GetAllowsDragDropRearrange(ItemsControl element)
        {
            return (bool)element.GetValue(AllowsDragDropRearrangeProperty);
        }
    }
}

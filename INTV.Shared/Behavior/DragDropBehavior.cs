// <copyright file="DragDropBehavior.cs" company="INTV Funhouse">
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
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Defines an attached behavior to make custom drop behaviors easier to accomplish in XAML.
    /// </summary>
    /// <remarks>From a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class DragDropBehavior
    {
        /// <summary>
        /// This attached property is set in order to define the command to execute for a Drop event.
        /// </summary>
        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached("DropCommand", typeof(ICommand), typeof(DragDropBehavior), new PropertyMetadata(DropCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the DropCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the drop.</param>
        /// <param name="command">The command to execute to handle the drop.</param>
        public static void SetDropCommand(this UIElement element, ICommand command)
        {
            element.SetValue(DropCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the DropCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static ICommand GetDropCommand(UIElement element)
        {
            return element.GetValue(DropCommandProperty) as ICommand;
        }

        private static void DropCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as UIElement;
            if (visual != null)
            {
                visual.Drop += (sender, args) =>
                    {
                        GetDropCommand(visual).Execute(args);
                        var itemsControl = visual as System.Windows.Controls.ItemsControl;
                        if ((itemsControl != null) && DragDropRearrangeBehavior.GetAllowsDragDropRearrange(itemsControl))
                        {
                            var dragDropFeedback = args.Data.GetData(DragDropHelpers.DragDropFeedbackDataFormat) as IDragDropFeedback;
                            if (dragDropFeedback != null)
                            {
                                dragDropFeedback.DropLocation = DropOnItemLocation.None;
                            }
                            INTV.Shared.View.DropItemLocationAdorner.Update(itemsControl, dragDropFeedback, true);
                        }
                        args.Handled = true;
                    };
            }
        }
    }
}

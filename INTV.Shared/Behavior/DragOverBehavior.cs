// <copyright file="DragOverBehavior.cs" company="INTV Funhouse">
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
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Defines an attached behavior to make custom drag over behaviors easier to accomplish in XAML.
    /// </summary>
    /// <remarks>From a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class DragOverBehavior
    {
        /// <summary>
        /// Setting this property will cause a command to execute when the visual receives a DragOver event.
        /// </summary>
        public static readonly DependencyProperty DragOverCommandProperty = DependencyProperty.RegisterAttached("DragOverCommand", typeof(ICommand), typeof(DragOverBehavior), new PropertyMetadata(DragOverCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the DragOverCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the drag over.</param>
        /// <param name="command">The command to execute to handle the drag over.</param>
        public static void SetDragOverCommand(this UIElement element, ICommand command)
        {
            element.SetValue(DragOverCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the DragOverCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        public static ICommand GetDragOverCommand(UIElement element)
        {
            return element.GetValue(DragOverCommandProperty) as ICommand;
        }

        /// <summary>
        /// Gets the actual height of an item that is being dragged over in a Drag and Drop operation.
        /// </summary>
        /// <param name="visual">The visual whose height is desired.</param>
        /// <returns>The actual height of the visual.</returns>
        /// <remarks>If the visual is a TreeViewItem, the height is determined from the PART_Header template part, rather than
        /// the entire item. This is because an expanded TreeViewItem's height includes the height of all its children.</remarks>
        public static double GetDragOverVisualHeight(FrameworkElement visual)
        {
            var height = visual.ActualHeight;
            var treeViewItem = visual as TreeViewItem;
            if (treeViewItem != null)
            {
                var header = (FrameworkElement)treeViewItem.Template.FindName("PART_Header", treeViewItem);
                height = header.ActualHeight;
            }
            return height;
        }

        private static void DragOverCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as FrameworkElement;
            if (visual != null)
            {
                visual.DragOver += (sender, args) =>
                {
                    IDragDropFeedback dragDropFeedback = null;
                    if (args.Data.GetDataPresent(DragDropHelpers.DragDropFeedbackDataFormat))
                    {
                        dragDropFeedback = args.Data.GetData(DragDropHelpers.DragDropFeedbackDataFormat) as IDragDropFeedback;
                    }
                    else
                    {
                        var itemsControl = visual as System.Windows.Controls.ItemsControl;
                        var allowsRearrange = (itemsControl != null) && DragDropRearrangeBehavior.GetAllowsDragDropRearrange(itemsControl);
                        if (allowsRearrange)
                        {
                            dragDropFeedback = new DragDropFeedback();
                            args.Data.SetData(DragDropHelpers.DragDropFeedbackDataFormat, dragDropFeedback);
                        }
                    }
                    if (dragDropFeedback != null)
                    {
                        var height = visual.ActualHeight;
                        var location = args.GetPosition(visual);
                        var dropLocation = GetDropLocation(visual, location);
                        dragDropFeedback.DropLocation = dropLocation;
                    }
                    GetDragOverCommand(visual).Execute(args);
                    if (dragDropFeedback != null)
                    {
                        if (args.Effects == DragDropEffects.None)
                        {
                            dragDropFeedback.DropLocation = DropOnItemLocation.None;
                        }
                        DropItemLocationAdorner.Update(visual, dragDropFeedback, false);
                    }
                    args.Handled = true;
                };
            }
        }

        private static DropOnItemLocation GetDropLocation(FrameworkElement visual, Point positionOverItem)
        {
            var dropLocation = DropOnItemLocation.None;
            var height = GetDragOverVisualHeight(visual);
            var ratioFromTop = positionOverItem.Y / height;
            if ((ratioFromTop >= 0) && (ratioFromTop < 0.25))
            {
                dropLocation = DropOnItemLocation.TopQuarter;
            }
            else if ((ratioFromTop >= 0.25) && (ratioFromTop < 0.5))
            {
                dropLocation = DropOnItemLocation.TopMiddleQuarter;
            }
            else if ((ratioFromTop >= 0.5) && (ratioFromTop < 0.75))
            {
                dropLocation = DropOnItemLocation.BottomMiddleQuarter;
            }
            else if ((ratioFromTop >= 0.75) && (ratioFromTop <= 1))
            {
                dropLocation = DropOnItemLocation.BottomQuarter;
            }
            return dropLocation;
        }
    }
}

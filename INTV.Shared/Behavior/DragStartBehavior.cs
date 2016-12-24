// <copyright file="DragStartBehavior.cs" company="INTV Funhouse">
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
    /// Defines attached behaviors to make custom drag start behaviors easier to accomplish in XAML.
    /// </summary>
    /// <remarks>From a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class DragStartBehavior
    {
        /// <summary>
        /// This attached property stores a command to execute to preview the start of a drag-drop operation.
        /// </summary>
        public static readonly DependencyProperty PreviewDragStartCommandProperty = DependencyProperty.RegisterAttached("PreviewDragStartCommand", typeof(IDragStartCommand), typeof(DragStartBehavior), new PropertyMetadata(PreviewDragStartCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the PreviewDragStartCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the drag start.</param>
        /// <param name="command">The command to execute to handle the drag start.</param>
        public static void SetPreviewDragStartCommand(this FrameworkElement element, IDragStartCommand command)
        {
            element.SetValue(PreviewDragStartCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the PreviewDragStartCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the property.</param>
        /// <returns>The value of the property.</returns>
        private static IDragStartCommand GetPreviewDragStartCommand(FrameworkElement element)
        {
            return element.GetValue(PreviewDragStartCommandProperty) as IDragStartCommand;
        }

        private static void PreviewDragStartCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as FrameworkElement;
            if (visual != null)
            {
                visual.PreviewMouseLeftButtonDown += (sender, args) =>
                    {
                        DragDropHelpers.LastMouseDownVisual = visual;
                        DragDropHelpers.LastMouseDown = args.GetPosition(null);
                        var itemsControl = visual as System.Windows.Controls.ItemsControl;
                        if (itemsControl != null)
                        {
                            if (MultiSelectDragDropBehavior.GetAllowMultiSelectDragDrop(itemsControl))
                            {
                                MultiSelectDragDropBehavior.ItemsControlPreviewMouseLeftButtonDown(itemsControl, args);
                            }
                        }
                    };

                visual.MouseMove += (sender, args) =>
                    {
                        if (!DragDropHelpers.InDragOperation && (DragDropHelpers.LastMouseDownVisual == visual) && (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed))
                        {
                            try
                            {
                                DragDropHelpers.InDragOperation = DragDropHelpers.DragStarted(args.GetPosition(null));
                                if (DragDropHelpers.InDragOperation)
                                {
                                    var command = GetPreviewDragStartCommand(visual);
                                    if (command.CanExecute(args))
                                    {
                                        command.Execute(args);
                                        var dragData = new DataObject(command.Format, command.Data);
                                        var itemsControl = visual as System.Windows.Controls.ItemsControl;
                                        if ((itemsControl != null) && DragDropRearrangeBehavior.GetAllowsDragDropRearrange(itemsControl))
                                        {
                                            dragData.SetData(DragDropHelpers.DragDropFeedbackDataFormat, new DragDropFeedback());
                                        }
                                        var effects = DragDrop.DoDragDrop(visual, dragData, command.Effects);
                                        if (dragData.GetDataPresent(DragDropHelpers.DragDropFeedbackDataFormat))
                                        {
                                            var dragDropFeedback = dragData.GetData(DragDropHelpers.DragDropFeedbackDataFormat) as IDragDropFeedback;
                                            if (dragDropFeedback != null)
                                            {
                                                dragDropFeedback.DropLocation = DropOnItemLocation.None;
                                            }
                                            INTV.Shared.View.DropItemLocationAdorner.Update(visual, dragDropFeedback, true);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                DragDropHelpers.InDragOperation = false;
                            }
                        }
                    };
            }
        }
    }
}

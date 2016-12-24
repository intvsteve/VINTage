// <copyright file="DoubleClickBehavior.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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
using System.Windows.Input;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements a Behavior for double-click commands.
    /// </summary>
    public static class DoubleClickBehavior
    {
        #region DoubleClickCommand

        /// <summary>
        /// Setting this property will cause a command to execute when the visual to which it is bound is clicked.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(DoubleClickBehavior), new PropertyMetadata(DoubleClickCommandPropertyChangedCallback));

        /// <summary>
        /// Property setter for the DoubleClickCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the double click.</param>
        /// <param name="command">The command to execute to handle the drag enter.</param>
        public static void SetDoubleClickCommand(this Control element, ICommand command)
        {
            element.SetValue(DoubleClickCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the DoubleClickCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to retrieve the property.</param>
        /// <returns>The value of the property.</returns>
        private static ICommand GetDoubleClickCommand(this Control element)
        {
            return element.GetValue(DoubleClickCommandProperty) as ICommand;
        }

        private static void DoubleClickCommandPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as Control;
            if (visual != null)
            {
                visual.MouseDoubleClick += (sender, args) =>
                {
                    var command = GetDoubleClickCommand(visual);
                    var commandParameter = visual.GetDoubleClickCommandParameter();
                    if (commandParameter == null)
                    {
                        var itemsControl = sender as ItemsControl;
                        if (itemsControl != null)
                        {
                            FrameworkElement hitItem = null;
                            if (sender is ListView)
                            {
                                hitItem = itemsControl.GetContainerAtPoint<ListViewItem>(args.GetPosition(itemsControl));
                            }
                            else if (sender is TreeView)
                            {
                                hitItem = itemsControl.GetContainerAtPoint<TreeViewItem>(args.GetPosition(itemsControl));
                            }
                            if (hitItem != null)
                            {
                                commandParameter = hitItem.DataContext;
                            }
                        }
                    }
                    if (commandParameter == null)
                    {
                        commandParameter = visual.DataContext;
                    }
                    if ((command != null) && command.CanExecute(commandParameter))
                    {
                        command.Execute(commandParameter);
                        args.Handled = true;
                    }
                };
            }
        }

        #endregion DoubleClickCommand

        #region DoubleClickCommandParameter

        /// <summary>
        /// Setting this property will store a parameter for the command.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandParameterProperty = DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(DoubleClickBehavior), new PropertyMetadata(DoubleClickCommandParameterPropertyChangedCallback));

        /// <summary>
        /// Property setter for the DoubleClickCommandParameter attached property.
        /// </summary>
        /// <param name="element">The control whose command parameter data is being set.</param>
        /// <param name="commandParameter">The parameter for the double-click command.</param>
        public static void SetDoubleClickCommandParameter(this Control element, object commandParameter)
        {
            element.SetValue(DoubleClickCommandParameterProperty, commandParameter);
        }

        private static object GetDoubleClickCommandParameter(this Control element)
        {
            return element.GetValue(DoubleClickCommandParameterProperty);
        }

        private static void DoubleClickCommandParameterPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as Control;
            if (visual != null)
            {
                visual.SetDoubleClickCommandParameter(e.NewValue);
            }
        }

        #endregion // DoubleClickCommandParameter

        private static ItemVisual GetContainerAtPoint<ItemVisual>(this ItemsControl control, Point p) where ItemVisual : DependencyObject
        {
            var result = System.Windows.Media.VisualTreeHelper.HitTest(control, p);
            var hitVisual = result.VisualHit;
            while ((System.Windows.Media.VisualTreeHelper.GetParent(hitVisual) != null) && !(hitVisual is ItemVisual))
            {
                hitVisual = System.Windows.Media.VisualTreeHelper.GetParent(hitVisual);
            }
            return hitVisual as ItemVisual;
        }
    }
}

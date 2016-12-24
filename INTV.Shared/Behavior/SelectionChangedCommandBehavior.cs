// <copyright file="SelectionChangedCommandBehavior.cs" company="INTV Funhouse">
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
using System.Windows.Controls.Primitives;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements an attached behavior to execute a command when the selected item in a Selector changes.
    /// </summary>
    public static class SelectionChangedCommandBehavior
    {
        /// <summary>
        /// This attached property is the command to execute when the selection changes on a Selector control.
        /// </summary>
        /// <remarks>The parameter sent to the command's CanExecute and Execute handlers is the currently selected item.</remarks>
        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.RegisterAttached("SelectionChangedCommand", typeof(ICommand), typeof(SelectionChangedCommandBehavior), new PropertyMetadata(SelectionChangedCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the SelectionChangedCommand attached property.
        /// </summary>
        /// <param name="selector">The Selector control that should execute the command.</param>
        /// <param name="command">The command to associate with the Selector control.</param>
        public static void SetSelectionChangedCommand(this Selector selector, ICommand command)
        {
            selector.SetValue(SelectionChangedCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the SelectionChangedCommand attached property.
        /// </summary>
        /// <param name="selector">The Selector control from which to retrieve the command.</param>
        /// <returns>The command associated with the Selector control.</returns>
        private static ICommand GetSelectionChangedCommand(Selector selector)
        {
            return selector.GetValue(SelectionChangedCommandProperty) as ICommand;
        }

        private static void SelectionChangedCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var selector = o as Selector;
            if (selector != null)
            {
                selector.SelectionChanged += (sender, args) =>
                {
                    var command = GetSelectionChangedCommand(selector);
                    if (command.CanExecute(selector.SelectedItem))
                    {
                        command.Execute(selector.SelectedItem);
                    }
                };
            }
        }
    }
}

// <copyright file="WindowWillCloseBehavior.cs" company="INTV Funhouse">
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
    /// Implements an attached behavior to execute a command when a window will be closing.
    /// </summary>
    public static class WindowWillCloseBehavior
    {
        /// <summary>
        /// This attached property stores the command to execute prior to a window closing. If the CanExecute handler of the command returns
        /// false, the window will not close.
        /// </summary>
        public static readonly DependencyProperty WindowWillCloseCommandProperty = DependencyProperty.RegisterAttached("WindowWillCloseCommand", typeof(ICommand), typeof(WindowWillCloseBehavior), new PropertyMetadata(WindowWillCloseCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the WindowWillCloseCommand attached property.
        /// </summary>
        /// <param name="window">The visual that receives the sort command.</param>
        /// <param name="command">The command to execute to handle the sort.</param>
        public static void SetWindowWillCloseCommand(this Window window, ICommand command)
        {
            window.SetValue(WindowWillCloseCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the WindowWillCloseCommand attached property.
        /// </summary>
        /// <param name="window">The visual from which to retrieve the property.</param>
        /// <returns>The value of the property.</returns>
        private static ICommand GetWindowWillCloseCommand(Window window)
        {
            return window.GetValue(WindowWillCloseCommandProperty) as ICommand;
        }

        private static void WindowWillCloseCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var window = o as Window;
            if (window != null)
            {
                window.Closing += (sender, args) =>
                    {
                        var willCloseCommand = GetWindowWillCloseCommand(window);
                        if (willCloseCommand.CanExecute(window))
                        {
                            willCloseCommand.Execute(window);
                        }
                        else
                        {
                            args.Cancel = true;
                        }
                    };
            }
        }
    }
}

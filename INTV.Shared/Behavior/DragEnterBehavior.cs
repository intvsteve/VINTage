// <copyright file="DragEnterBehavior.cs" company="INTV Funhouse">
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
    /// Defines an attached behavior to make custom drag enter behaviors easier to accomplish in XAML.
    /// </summary>
    /// <remarks>From a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class DragEnterBehavior
    {
        /// <summary>
        /// This attached property stores the command to execute when a drag-drop operation enters a specific visual's bounds.
        /// </summary>
        public static readonly DependencyProperty DragEnterCommandProperty = DependencyProperty.RegisterAttached("DragEnterCommand", typeof(ICommand), typeof(DragEnterBehavior), new PropertyMetadata(DragEnterCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the DragEnterCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the drag enter.</param>
        /// <param name="command">The command to execute to handle the drag enter.</param>
        public static void SetDragEnterCommand(this UIElement element, ICommand command)
        {
            element.SetValue(DragEnterCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the DragEnterCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to retrieve the property.</param>
        /// <returns>The value of the property.</returns>
        private static ICommand GetDragEnterCommand(UIElement element)
        {
            return element.GetValue(DragEnterCommandProperty) as ICommand;
        }

        private static void DragEnterCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as UIElement;
            if (visual != null)
            {
                visual.DragEnter += (sender, args) =>
                {
                    GetDragEnterCommand(visual).Execute(args);
                    args.Handled = true;
                };
            }
        }
    }
}

// <copyright file="PreviewDragEnterBehavior.cs" company="INTV Funhouse">
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
    /// Defines an attached behavior to make custom preview drag enter behaviors easier to accomplish in XAML.
    /// </summary>
    /// <remarks>From a most helpful article at wpfsharp.com.</remarks>
    /// <see cref="http://www.wpfsharp.com/2012/03/22/mvvm-and-drag-and-drop-command-binding-with-an-attached-behavior/"/>
    public static class PreviewDragEnterBehavior
    {
        /// <summary>
        /// This attached property is set in order to define the command to execute for a PreviewDragEnter event.
        /// </summary>
        public static readonly DependencyProperty PreviewDragEnterCommandProperty = DependencyProperty.RegisterAttached("PreviewDragEnterCommand", typeof(ICommand), typeof(PreviewDragEnterBehavior), new PropertyMetadata(PreviewDragEnterCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the PreviewDragEnterCommand attached property.
        /// </summary>
        /// <param name="element">The visual that receives the preview drag enter.</param>
        /// <param name="command">The command to execute to handle the drag enter.</param>
        public static void SetPreviewDragEnterCommand(this UIElement element, ICommand command)
        {
            element.SetValue(PreviewDragEnterCommandProperty, command);
        }

        /// <summary>
        /// Property getter for the PreviewDragEnterCommand attached property.
        /// </summary>
        /// <param name="element">The visual from which to retrieve the property.</param>
        /// <returns>The value of the property.</returns>
        private static ICommand GetPreviewDragEnterCommand(UIElement element)
        {
            return element.GetValue(PreviewDragEnterCommandProperty) as ICommand;
        }

        private static void PreviewDragEnterCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var visual = o as UIElement;
            if (visual != null)
            {
                visual.PreviewDragEnter += (sender, args) =>
                {
                    GetPreviewDragEnterCommand(visual).Execute(args);
                    args.Handled = true;
                };
            }
        }
    }
}

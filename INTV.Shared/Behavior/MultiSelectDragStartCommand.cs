// <copyright file="MultiSelectDragStartCommand.cs" company="INTV Funhouse">
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

using System;
using System.Windows;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// This class provides support for multiple selection drag command implementation.
    /// </summary>
    public abstract class MultiSelectDragStartCommand : RelayCommand, IDragStartCommand
    {
        /// <summary>
        /// Initializes a new instance of MultiSelectDragStartCommand.
        /// </summary>
        /// <param name="onExecute">The function to call when the command is executed.</param>
        protected MultiSelectDragStartCommand(Action<object> onExecute)
            : base(onExecute, CanDragStart)
        {
        }

        /// <inheritdoc />
        public abstract string Format { get; }

        /// <inheritdoc />
        public abstract object Data { get; }

        /// <inheritdoc />
        public abstract DragDropEffects Effects { get; }

        /// <inheritdoc />
        public override void Execute(object parameter)
        {
#if WIN
            var mouseEventArgs = (System.Windows.Input.MouseEventArgs)parameter;
            var dragSource = mouseEventArgs.Source as System.Windows.Controls.ItemsControl;
            if (dragSource != null)
            {
                dragSource.ReleaseMouseCapture();
            }
#elif MAC
            ErrorReporting.ReportNotImplementedError("MultiSelectDragStartCommand.Execute");
#endif // WIN
            base.Execute(parameter);
        }

        private static bool CanDragStart(object parameter)
        {
            var canStartDrag = false;
#if WIN
            var mouseEventArgs = (System.Windows.Input.MouseEventArgs)parameter;
            var dragSource = mouseEventArgs.Source as System.Windows.Controls.ItemsControl;
            if ((dragSource != null) && dragSource.IsMouseCaptured)
            {
                if (dragSource is System.Windows.Controls.ListView)
                {
                    var items = ((System.Windows.Controls.ListView)dragSource).SelectedItems;
                    canStartDrag = (items != null) && (items.Count > 0);
                }
                else if (dragSource is System.Windows.Controls.TreeView)
                {
                }
            }
#elif MAC
            ErrorReporting.ReportNotImplementedError("MultiSelectDragStartCommand.CanDragStart");
#endif // WIN
            return canStartDrag;
        }
    }
}

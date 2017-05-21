// <copyright file="StartDragFileNodeCommand.cs" company="INTV Funhouse">
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
using INTV.Shared.ComponentModel;

#if WIN
using DragDropEffects = System.Windows.DragDropEffects;
#elif MAC
using DragDropEffects = INTV.Shared.Behavior.DragDropEffects;
#endif // WIN

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Specialization of the RelayCommand class to define a command used to indicate the start of a drag operation.
    /// </summary>
    public class StartDragFileNodeCommand : RelayCommand, INTV.Shared.Behavior.IDragStartCommand
    {
        private FileNodeViewModel _draggedItem;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the StartDragFileNodeCommand class.
        /// </summary>
        /// <param name="draggedItem">The item that's being dragged.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        public StartDragFileNodeCommand(FileNodeViewModel draggedItem, Func<object, bool> canExecute)
            : base(NoOp, canExecute, null)
        {
            _draggedItem = draggedItem;
        }

        #endregion // Constructors

        #region Properties

        #region IDragStartCommand Properties

        /// <inheritdoc />
        public string Format
        {
            get { return MenuLayoutViewModel.DragDataFormat; }
        }

        /// <inheritdoc />
        public object Data
        {
            get { return _draggedItem.Model; }
        }

        /// <inheritdoc />
        public DragDropEffects Effects
        {
            get { return DragDropEffects.Move; }
        }

        #endregion // IDragStartCommand Properties

        #endregion // Properties
    }
}

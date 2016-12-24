// <copyright file="RelayCommand.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Implements the ICommand interface without relying upon a visual to route the command.
    /// </summary>
    public partial class RelayCommand : INTV.Core.ComponentModel.RelayCommandBase, ICommand
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        public RelayCommand(Action<object> onExecute)
            : this(onExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        public RelayCommand(Action<object> onExecute, Func<object, bool> canExecute)
            : this(onExecute, canExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        /// <param name="parameter">Data to pass to the Execute and CanExecute methods.</param>
        public RelayCommand(Action<object> onExecute, Func<object, bool> canExecute, object parameter)
            : base(onExecute, canExecute, parameter)
        {
            BlockWhenAppIsBusy = true;
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="toClone">The command to copy.</param>
        protected RelayCommand(RelayCommand toClone)
            : base(toClone)
        {
            BlockWhenAppIsBusy = toClone.BlockWhenAppIsBusy;
        }

        #endregion // Constructors

        /// <summary>
        /// Gets or sets a value indicating whether the command should be disabled when the application is in the 'busy' state.
        /// </summary>
        public bool BlockWhenAppIsBusy { get; set; }

        /// <summary>
        /// A no-op to use as a command execute handler.
        /// </summary>
        /// <param name="parameter">Data for executing the command. (Unused)</param>
        public static void NoOp(object parameter)
        {
        }

        /// <inheritdoc/>
        public virtual bool CanExecute(object parameter)
        {
            bool blockIfBusy = false;
            if (BlockWhenAppIsBusy)
            {
                var application = INTV.Shared.Utility.SingleInstanceApplication.Instance;
                if (application != null)
                {
                    blockIfBusy = application.IsBusy;
                }
            }
            return !blockIfBusy && OnCanExecute(parameter);
        }

        /// <inheritdoc/>
        public virtual void Execute(object parameter)
        {
            OnExecute(parameter);
        }
    }
}

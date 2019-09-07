// <copyright file="RelayCommandBase.cs" company="INTV Funhouse">
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

namespace INTV.Core.ComponentModel
{
    /// <summary>
    /// Defines a base command-like interface which does not rely upon a visual to route the command.
    /// </summary>
    public partial class RelayCommandBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RelayCommandBase class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        protected RelayCommandBase(Action<object> onExecute)
            : this(onExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommandBase class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        protected RelayCommandBase(Action<object> onExecute, Func<object, bool> canExecute)
            : this(onExecute, canExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommandBase class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        /// <param name="parameter">Data to pass to the Execute and CanExecute methods.</param>
        protected RelayCommandBase(Action<object> onExecute, Func<object, bool> canExecute, object parameter)
        {
            ExecuteAction = onExecute;
            CanExecuteAction = canExecute;
            Parameter = parameter;
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommandBase class from another instance.
        /// </summary>
        /// <param name="toClone">The command to clone.</param>
        protected RelayCommandBase(RelayCommandBase toClone)
            : this(toClone.ExecuteAction, toClone.CanExecuteAction, toClone.Parameter)
        {
            UniqueId = toClone.UniqueId;
            PreferredParameterType = toClone.PreferredParameterType;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the preferred data type of the command's parameter.
        /// </summary>
        public Type PreferredParameterType { get; set; }

        private Action<object> ExecuteAction { get; set; }

        private Func<object, bool> CanExecuteAction { get; set; }

        private object Parameter { get; set; }

        #endregion // Properties

        /// <summary>
        /// Executes the CanExecute handler if defined.
        /// </summary>
        /// <param name="parameter">Data for the can execute handler.</param>
        /// <returns><c>true</c> if the command is allowed to execute.</returns>
        /// <remarks>If the supplied parameter is null, the parameter passed to the constructor will be used.</remarks>
        protected bool OnCanExecute(object parameter)
        {
            return (CanExecuteAction == null) || CanExecuteAction((parameter == null) ? Parameter : parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Data for the execute handler.</param>
        /// <remarks>If the supplied parameter is null, the parameter passed to the constructor will be used.</remarks>
        protected void OnExecute(object parameter)
        {
            ExecuteAction((parameter == null) ? Parameter : parameter);
        }
    }
}

// <copyright file="VisualDeviceCommand.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Specializes VisualRelayCommand to work better with commands targeting a Locutus device.
    /// </summary>
    public class VisualDeviceCommand : INTV.Shared.ComponentModel.VisualRelayCommand
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the VisualDeviceCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        public VisualDeviceCommand(Action<object> onExecute)
            : this(onExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the VisualDeviceCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        public VisualDeviceCommand(Action<object> onExecute, Func<object, bool> canExecute)
            : base(onExecute, canExecute)
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets or sets the underlying device protocol commands required to carry out this user-initiated command.
        /// </summary>
        internal IEnumerable<ProtocolCommandId> RequiredProtocolCommands { get; set; }
    }
}

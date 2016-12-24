// <copyright file="CommandHelpers.cs" company="INTV Funhouse">
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

using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;

namespace INTV.LtoFlash.Commands
{
    public static class CommandHelpers
    {
        /// <summary>
        /// Helper method to check if a command can execute.
        /// </summary>
        /// <param name="ltoFlash">The top-level view model whose active device will be used to test command availability.</param>
        /// <param name="command">The command whose availability is being tested.</param>
        /// <returns><c>true</c> if the command can execute, otherwise <c>false</c>.</returns>
        internal static bool CanExecuteCommand(this LtoFlashViewModel ltoFlash, VisualDeviceCommand command)
        {
            bool canExecute = (ltoFlash != null) && (ltoFlash.ActiveLtoFlashDevice != null) && (ltoFlash.ActiveLtoFlashDevice.Device != null);
            if (canExecute)
            {
                canExecute = ltoFlash.ActiveLtoFlashDevice.Device.CanExecuteCommand(command);
            }
            return canExecute;
        }

        /// <summary>
        /// Helper method to check if a command can execute.
        /// </summary>
        /// <param name="device">The target device for the given command.</param>
        /// <param name="command">The command whose availability is being tested.</param>
        /// <returns><c>true</c> if the command can execute, otherwise <c>false</c>.</returns>
        internal static bool CanExecuteCommand(this Device device, VisualDeviceCommand command)
        {
            var canExecute = (device != null) && device.IsValid && !device.IsCommandInProgress && device.AllCommandsAvailable(command.RequiredProtocolCommands);
            return canExecute;
        }
    }
}

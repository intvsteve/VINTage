// <copyright file="CommandAvailable.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Enumerates if or when a command may be available or accessible for the user to execute.
    /// </summary>
    public enum CommandAvailable
    {
        /// <summary>
        /// The command is never available. For example, the Locutus device simulator has commands that
        /// are not possible on actual hardware, such as setting the device state. So when a Device
        /// is connected to actual hardware, the command will never be available.
        /// </summary>
        Never,

        /// <summary>
        /// Command is not available for execution at this time.
        /// </summary>
        No,

        /// <summary>
        /// Command is available for execution.
        /// </summary>
        Yes,

        /// <summary>
        /// Command should always be available for execution. For example, Ping and Garbage Collect commands
        /// are always allowed to execute. Firmware updates may allow other commands to be available in the future.
        /// </summary>
        Always,
    }
}

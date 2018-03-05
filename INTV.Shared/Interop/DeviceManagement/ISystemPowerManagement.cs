// <copyright file="ISystemPowerManagement.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// This interface provides a way to respond to system power events, potentially
    /// cancelling certain actions.
    /// </summary>
    /// <remarks>Note that no assumptions should be made by users of this interface regarding
    /// the threads upon which these events will be raised.</remarks>
    public interface ISystemPowerManagement
    {
        /// <summary>
        /// Occurs when system will enter a lower-power mode -- e.g. sleep.
        /// </summary>
        /// <remarks>See <see cref="SystemWillSleepEventArgs"/> for more details regarding
        /// how to process this event.</remarks>
        event EventHandler<SystemWillSleepEventArgs> SystemWillSleep;

        /// <summary>
        /// Occurs when system will power off.
        /// </summary>
        event EventHandler<EventArgs> SystemWillPowerOff;

        /// <summary>
        /// Occurs when system did power on.
        /// </summary>
        event EventHandler<EventArgs> SystemDidPowerOn;
    }
}

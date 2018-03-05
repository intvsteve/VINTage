// <copyright file="SystemWillSleepEventArgs.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// Event data for the <see cref="ISystemPowerManagement.SystemWillSleep"/> event.
    /// </summary>
    public class SystemWillSleepEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="INTV.Shared.Interop.DeviceManagement.SystemWillSleepEventArgs"/> class.
        /// </summary>
        /// <param name="canCancel">If set to <c>true</c>, the recipient can cancel the entry to low-power (sleep) mode.
        /// If set to <c>false</c>, any attempt to cancel will be ignored - or at best delay the entry to low-power
        /// mode by some relatively brief amount of time.</param>
        public SystemWillSleepEventArgs(bool canCancel)
        {
            CanCancel = canCancel;
        }

        /// <summary>
        /// Gets a value indicating whether cancelling the entry to low-power mode is supported.
        /// </summary>
        /// <remarks>If this value is <c>false</c>, the value of <see cref="Cancel"/> will at most delay entering
        /// lower power mode for a brief period of time.</remarks>
        public bool CanCancel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the system's entry to sleep (low power) mode.
        /// </summary>
        /// <remarks>This value may be ignored, depending on the value of <see cref="CanCancel"/>.</remarks>
        public bool Cancel { get; set; }
    }
}

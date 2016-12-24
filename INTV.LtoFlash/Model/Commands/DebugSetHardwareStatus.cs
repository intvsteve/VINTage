// <copyright file="DebugSetHardwareStatus.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

#if DEBUG

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Implements the command to set the hardware status flags on the Locutus simulator.
    /// </summary>
    internal class DebugSetHardwareStatus : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 5000;

        private DebugSetHardwareStatus(HardwareStatusFlags hardwareStatusFlags)
            : base(ProtocolCommandId.DebugSetHardwareStatus, DefaultResponseTimeout, (uint)hardwareStatusFlags)
        {
        }

        /// <summary>
        /// Creates an instance of the DebugSetHardwareStatus command.
        /// </summary>
        /// <param name="hardwareStatusFlags">The hardware status flags to set on the simulator.</param>
        /// <returns>A new instance of the command.</returns>
        public static DebugSetHardwareStatus Create(HardwareStatusFlags hardwareStatusFlags)
        {
            return new DebugSetHardwareStatus(hardwareStatusFlags);
        }
    }
}

#endif // DEBUG

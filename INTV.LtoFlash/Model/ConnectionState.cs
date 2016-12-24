// <copyright file="ConnectionState.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Fine-grained connection state information. Depending on various conditions, communication with
    /// Locutus may need to behave differently.
    /// </summary>
    internal enum ConnectionState
    {
        /// <summary>
        /// Device isn't actually connected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Port is open, and periodic ping command is issued to follow device state.
        /// </summary>
        Ping,

        /// <summary>
        /// Port is open, and periodic garbage collect command is issued to perform background file system maintenance.
        /// </summary>
        GarbageCollect,

        /// <summary>
        /// Port is open, but device is in game mode, or otherwise indisposed. If a beacon is received, the
        /// device will transition to one of Ping or GarbageCollect states.
        /// </summary>
        WaitForBeacon,

        /// <summary>
        /// Port is open, but all activity should be suspended until the device is explicitly placed into another state.
        /// </summary>
        Idle
    }
}

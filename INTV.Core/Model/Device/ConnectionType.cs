// <copyright file="ConnectionType.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Types of connections a device may have.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// No connection.
        /// </summary>
        None,

        /// <summary>
        /// Intellivision cartridge port connection.
        /// </summary>
        CartridgePort,

        /// <summary>
        /// Memory map connection.
        /// </summary>
        MemoryMap,

        /// <summary>
        /// Standard RS-232 serial port connection.
        /// </summary>
        Serial,

        /// <summary>
        /// A named pipe connection.
        /// </summary>
        NamedPipe,

#if CRAZY_IDEAS
        Usb,
        Ethernet,
        Infrared,
        Rf
#endif // CRAZY_IDEAS
    }
}

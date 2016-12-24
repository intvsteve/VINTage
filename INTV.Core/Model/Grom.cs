// <copyright file="Grom.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

using System.Collections.Generic;
using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// Models the Intellivision Graphics ROM.
    /// </summary>
    public class Grom : INTV.Core.Model.Device.IPeripheral
    {
        /// <summary>
        /// The characters supported by GROM. Note that these are ASCII, so some, such as the arrow characters, are mapped.
        /// </summary>
        public static readonly char[] Characters = new char[]
        {
            ' ', '!', '"', '#', '$', '%', '&', '\'', ',', '(', ')', '*', '+', ',', '-', '.', '/',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',
            '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
            '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~'
        };

        private static readonly INTV.Core.Model.Device.MemoryMap Connection = new Device.MemoryMap(0x3000, 0x7FF, null);

        /// <inheritdoc />
        public string Name
        {
            get { return "GROM"; }
        }

        /// <inheritdoc />
        public IEnumerable<Device.IConnection> Connections
        {
            get { yield return Connection; }
        }

        /// <inheritdoc />
        public bool IsRomCompatible(IProgramDescription programDescription)
        {
            return true;
        }
    }
}

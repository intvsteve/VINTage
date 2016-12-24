// <copyright file="ReadForkToRam.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Implements the command to instruct the file system on a Locutus device to copy the contents of a fork into RAM on the device.
    /// </summary>
    internal sealed class ReadForkToRam : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 10000;

        private ReadForkToRam(uint address, ushort globalForkNumber, uint offset, int length)
            : base(ProtocolCommandId.LfsCopyForkToRam, DefaultResponseTimeout, globalForkNumber, address, offset, (uint)length)
        {
            ProtocolCommandHelpers.ValidateDataBlockSizeAndAddress(address, (int)length);
        }

        /// <summary>
        /// Creates an instance of the ReadForkToRam command.
        /// </summary>
        /// <param name="address">The address in RAM to which to begin copying the fork's data.</param>
        /// <param name="globalForkNumber">The global fork number of the fork to read into RAM.</param>
        /// <param name="offset">The offset (in bytes) into the fork at which to begin reading data to place into RAM.</param>
        /// <param name="length">The number of bytes to read into RAM.</param>
        /// <returns>A new instance of the command.</returns>
        public static ReadForkToRam Create(uint address, ushort globalForkNumber, uint offset, int length)
        {
            return new ReadForkToRam(address, globalForkNumber, offset, length);
        }
    }
}

// <copyright file="CreateForkFromRam.cs" company="INTV Funhouse">
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
    /// Implements the command to instruct the file system on a Locutus device create a file system fork from data in RAM on the device.
    /// </summary>
    internal sealed class CreateForkFromRam : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 5 * 60 * 1000; // There have been cases of create fork taking 4 minutes on heavily loaded devices!

        private CreateForkFromRam(uint address, Fork fork)
            : base(ProtocolCommandId.LfsCreateForkFromRam, DefaultResponseTimeout, fork.GlobalForkNumber, address, fork.Size, fork.Crc24)
        {
            ProtocolCommandHelpers.ValidateDataBlockSizeAndAddress(address, (int)fork.Size);
        }

        /// <summary>
        /// Creates an instance of the CreateForkFromRam command.
        /// </summary>
        /// <param name="address">The address in RAM from which to begin reading data for the fork.</param>
        /// <param name="fork">The fork that was uploaded to RAM whose data is used to create from RAM on the device.</param>
        /// <returns>A new instance of the command.</returns>
        public static CreateForkFromRam Create(uint address, Fork fork)
        {
            return new CreateForkFromRam(address, fork);
        }
    }
}

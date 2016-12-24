// <copyright file="ChecksumDataBlockInRam.cs" company="INTV Funhouse">
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
    /// Implements the command to instruct a Locutus device to perform a CRC32 checksum on a data block of a certain size at a certain location in RAM on the device.
    /// </summary>
    internal sealed class ChecksumDataBlockInRam : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 10000;

        private ChecksumDataBlockInRam(uint address, int blockLength)
            : base(ProtocolCommandId.LfsChecksumDataBlockInRam, DefaultResponseTimeout, address, (uint)blockLength)
        {
            ProtocolCommandHelpers.ValidateDataBlockSizeAndAddress(address, blockLength);
        }

        /// <summary>
        /// Creates an instance of the ChecksumDataBlockInRam command.
        /// </summary>
        /// <param name="address">The address in device RAM at which to start the checksum calculation.</param>
        /// <param name="blockLength">The number of bytes in RAM with which to compute the checksum</param>
        /// <returns>A new instance of the command.</returns>
        public static ChecksumDataBlockInRam Create(uint address, int blockLength)
        {
            return new ChecksumDataBlockInRam(address, blockLength);
        }

        /// <inheritdoc />
        public override object Execute(INTV.Shared.Model.IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            return ExecuteWithResponse(target, taskData, Inflate, out succeeded);
        }

        /// <inheritdoc />
        protected override byte[] ReadResponseData(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            return reader.ReadBytes(sizeof(uint));
        }

        private static uint Inflate(System.IO.Stream stream)
        {
            uint crc = 0;
            using (var reader = new INTV.Shared.Utility.ASCIIBinaryReader(stream))
            {
                crc = reader.ReadUInt32();
            }
            return crc;
        }
    }
}

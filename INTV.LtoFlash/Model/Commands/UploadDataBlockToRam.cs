// <copyright file="UploadDataBlockToRam.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
    /// Implements the command to upload a block of data to RAM on a Locutus device.
    /// </summary>
    internal sealed class UploadDataBlockToRam : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 10000;

        private UploadDataBlockToRam(uint address, INTV.Core.Utility.ByteSerializer data, uint runningCrc24)
            : base(ProtocolCommandId.LfsUploadDataBlockToRam, DefaultResponseTimeout, address, (uint)data.SerializeByteCount)
        {
            ProtocolCommandHelpers.ValidateDataBlockSizeAndAddress(address, (int)data.SerializeByteCount);
            Data = data;
            RunningCrc24 = runningCrc24;
        }

        /// <summary>
        /// Gets a CRC24 value that is updated when the data is serialized to send to the device.
        /// </summary>
        public uint RunningCrc24 { get; private set; }

        private INTV.Core.Utility.ByteSerializer Data { get; set; }

        /// <summary>
        /// Creates an instance of the UploadDataBlockToRam command.
        /// </summary>
        /// <param name="address">The address in RAM at which to upload a data block.</param>
        /// <param name="data">The data to be flattened and uploaded to RAM.</param>
        /// <param name="runningCrc24">The base CRC24 value to use when updating.</param>
        /// <returns>A new instance of the command.</returns>
        public static UploadDataBlockToRam Create(uint address, INTV.Core.Utility.ByteSerializer data, uint runningCrc24)
        {
            return new UploadDataBlockToRam(address, data, runningCrc24);
        }

        /// <inheritdoc />
        public override object Execute(INTV.Shared.Model.IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            using (var data = new System.IO.MemoryStream())
            {
                Data.Serialize(new INTV.Shared.Utility.ASCIIBinaryWriter(data));
                data.Seek(0, System.IO.SeekOrigin.Begin);
                RunningCrc24 = INTV.Core.Utility.Crc24.OfStream(data, RunningCrc24);
                data.Seek(0, System.IO.SeekOrigin.Begin);
                succeeded = ExecuteCommandWithData(target, taskData, data, null);
            }
            return succeeded;
        }
    }
}

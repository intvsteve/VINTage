// <copyright file="DownloadDataBlockFromRam.cs" company="INTV Funhouse">
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
    /// Implements the command to download a block of data in RAM from a Locutus device.
    /// </summary>
    internal sealed class DownloadDataBlockFromRam : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 20000;

        private DownloadDataBlockFromRam(uint address, int expectedDataLength)
            : base(ProtocolCommandId.LfsDownloadDataBlockFromRam, DefaultResponseTimeout, address, (uint)expectedDataLength)
        {
            ProtocolCommandHelpers.ValidateDataBlockSizeAndAddress(address, expectedDataLength);
        }

        private int ExpectedDataLength
        {
            get { return (int)Arg1; }
        }

        /// <summary>
        /// Creates an instance of the DownloadDataBlockFromRam command.
        /// </summary>
        /// <param name="address">The address in RAM from which to download the data.</param>
        /// <param name="expectedDataLength">The number of bytes to download from RAM.</param>
        /// <returns>A new instance of the command.</returns>
        public static DownloadDataBlockFromRam Create(uint address, int expectedDataLength)
        {
            return new DownloadDataBlockFromRam(address, expectedDataLength);
        }

        /// <inheritdoc />
        public override object Execute(INTV.Shared.Model.IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            return ExecuteWithResponse<byte[]>(target, taskData, (s) => ((System.IO.MemoryStream)s).ToArray(), out succeeded);
        }

        /// <inheritdoc />
        protected override byte[] ReadResponseData(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            return reader.ReadBytes(ExpectedDataLength);
        }
    }
}

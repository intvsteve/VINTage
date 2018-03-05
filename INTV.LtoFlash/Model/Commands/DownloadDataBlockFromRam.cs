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

////#define ENABLE_DIAGNOSTIC_OUTPUT

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

        /// <summary>
        /// The default size of the read chunk.
        /// </summary>
        /// <remarks>The default value of zero means to read the entire requested chunk as one large read. This can be
        /// overridden at any time. Most specifically, on some newer versions of macOS, this needs to be changed.
        /// Reading chunks up to 768 bytes has been confirmed working in High Sierra. Larger (1024 bytes or more)
        /// will time out partway through the read. Users can configure the read chunk size to be smaller if needed. It turns
        /// out the driver uses a 512 byte buffer, so we'll just happen to choose that as our default chunk size.</remarks>
        private const int DefaultReadChunkSize = 0;

        /// <summary>
        /// Gets or sets the size of the chunk to read from the port, in bytes.
        /// </summary>
        internal static int ReadChunkSize
        {
            get { return _readChunkSize; }
            set { _readChunkSize = value; }
        }
        private static int _readChunkSize = DefaultReadChunkSize;

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
            byte[] responseData = new byte[ExpectedDataLength];
            var bytesRemaining = ExpectedDataLength;
            var chunkSize = ReadChunkSize == 0 ? ExpectedDataLength : ReadChunkSize;

            DebugOutput("----------BEGINREAD buffer, expected: " + ExpectedDataLength + ", ChunkSize: " + chunkSize);

            var chunkNumber = 0;
            using (var memory = new System.IO.MemoryStream())
            {
                do
                {
                    var bytesToRead = (bytesRemaining / chunkSize) > 0 ? chunkSize : bytesRemaining;

                    DebugOutput("READING CHUNK # " + chunkNumber++ + ", numToRead: " + bytesToRead);

                    var dataRead = reader.ReadBytes(bytesToRead);

                    DebugOutput("WRITING to memory: " + bytesToRead + ", buff: " + dataRead.Length);

                    memory.Write(dataRead, 0, bytesToRead);
                    bytesRemaining -= bytesToRead;
                }
                while (bytesRemaining > 0);
                memory.Seek(0, System.IO.SeekOrigin.Begin);
                responseData = memory.ToArray();
            }

            DebugOutput("----------RETURNING buffer: " + responseData.Length + ", expected: " + ExpectedDataLength);

            return responseData;
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

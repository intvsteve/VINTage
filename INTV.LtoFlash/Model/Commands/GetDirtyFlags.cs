// <copyright file="GetDirtyFlags.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Implements the command to upload get file system status flags from a Locutus device.
    /// </summary>
    internal sealed class GetDirtyFlags : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 1000;

        private GetDirtyFlags()
            : base(ProtocolCommandId.LfsGetFileSystemStatusFlags, DefaultResponseTimeout)
        {
        }

        /// <summary>
        /// Gets the instance of the GetDirtyFlags command.
        /// </summary>
        public static readonly GetDirtyFlags Instance = new GetDirtyFlags();

        /// <inheritdoc />
        public override object Execute(INTV.Shared.Model.IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            return ExecuteWithResponse(target, taskData, Inflate, out succeeded);
        }

        /// <inheritdoc />
        protected override byte[] ReadResponseData(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            return reader.ReadBytes(sizeof(LfsDirtyFlags));
        }

        private static LfsDirtyFlags Inflate(System.IO.Stream stream)
        {
            var dirtyFlags = LfsDirtyFlags.None;
            using (var reader = new INTV.Shared.Utility.ASCIIBinaryReader(stream))
            {
                var rawFlags = reader.ReadUInt32();

                // On a brand new device, these flags come back as all set, so ignore those.
                if (rawFlags != 0xFFFFFFFF)
                {
                    dirtyFlags = (LfsDirtyFlags)rawFlags;
                }
            }
            return dirtyFlags;
        }
    }
}

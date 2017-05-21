// <copyright file="DownloadAndLaunch.cs" company="INTV Funhouse">
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
    /// Implements the command to load a LUIGI formatted ROM to RAM on a Locutus device and start the program running.
    /// </summary>
    internal sealed class DownloadAndLaunch : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 30000;

        private DownloadAndLaunch(FileSystemFile luigiFile)
            : base(ProtocolCommandId.DownloadAndPlay, DefaultResponseTimeout, (uint)luigiFile.SerializeByteCount)
        {
            File = luigiFile;
        }

        private FileSystemFile File { get; set; }

        /// <summary>
        /// Creates an instance of the DownloadAndLaunch command.
        /// </summary>
        /// <param name="luigiFile">The program to load and launch.</param>
        /// <returns>A new instance of the command.</returns>
        public static DownloadAndLaunch Create(FileSystemFile luigiFile)
        {
            return new DownloadAndLaunch(luigiFile);
        }

        /// <inheritdoc />
        public override object Execute(INTV.Shared.Model.IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            using (var data = new System.IO.FileStream(File.FileInfo.FullName, System.IO.FileMode.Open))
            {
                succeeded = ExecuteCommandWithData(target, taskData, data, () => taskData.Device.ConnectionState = ConnectionState.WaitForBeacon);
            }
            return succeeded;
        }
    }
}

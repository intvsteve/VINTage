// <copyright file="DownloadErrorLog.cs" company="INTV Funhouse">
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
    /// Implements the command to instruct a Locutus device to download its error log.
    /// </summary>
    internal sealed class DownloadErrorLog : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 20000;

        private DownloadErrorLog()
            : base(ProtocolCommandId.DownloadErrorLog, DefaultResponseTimeout)
        {
        }

        /// <summary>
        /// Gets the instance of the DownloadErrorLog command.
        /// </summary>
        public static readonly DownloadErrorLog Instance = new DownloadErrorLog();

        /// <inheritdoc />
        public override object Execute(INTV.Shared.Model.IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            var errorLog = ExecuteWithResponse<ErrorLog>(target, taskData, ErrorLog.Inflate, out succeeded);
            target.LogPortMessage("ERROR LOG: '" + (errorLog == null ? "<null>" : errorLog.Text) + "'");
            return errorLog;
        }

        /// <inheritdoc />
        protected override byte[] ReadResponseData(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            return reader.ReadBytes(ErrorLog.FlatSizeInBytes);
        }
    }
}

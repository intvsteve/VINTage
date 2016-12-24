// <copyright file="UpdateGlobalFileTable.cs" company="INTV Funhouse">
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
    /// Implements the command to update the global file table on a Locutus device.
    /// </summary>
    internal sealed class UpdateGlobalFileTable : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 2 * 60 * 1000; // may take a long time on nearly full devices

        private UpdateGlobalFileTable(uint address, ushort firstGlobalFileNumber, int numberOfEntries)
            : base(ProtocolCommandId.LfsUpdateGftFromRam, DefaultResponseTimeout, firstGlobalFileNumber, (uint)numberOfEntries, address)
        {
        }

        /// <summary>
        /// Creates an instance of the UpdateGlobalFileTable command.
        /// </summary>
        /// <param name="address">The address in RAM to which to upload the modified file entries.</param>
        /// <param name="files">The files to upload.</param>
        /// <returns>A new instance of the command.</returns>
        public static UpdateGlobalFileTable Create(uint address, System.Collections.Generic.IList<ILfsFileInfo> files)
        {
            return new UpdateGlobalFileTable(address, files[0].GlobalFileNumber, files.Count);
        }

        /// <summary>
        /// Creates an instance of the UpdateGlobalFileTable command.
        /// </summary>
        /// <param name="address">The address in RAM to which to upload the modified file entries.</param>
        /// <param name="updateRange">The range (inclusive) of Global File Table entries included in the update.</param>
        /// <returns>A new instance of the command.</returns>
        public static UpdateGlobalFileTable Create(uint address, INTV.Core.Utility.Range<ushort> updateRange)
        {
            return new UpdateGlobalFileTable(address, updateRange.Minimum, updateRange.Maximum - updateRange.Minimum + 1);
        }
    }
}

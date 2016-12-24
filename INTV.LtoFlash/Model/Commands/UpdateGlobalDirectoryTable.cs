// <copyright file="UpdateGlobalDirectoryTable.cs" company="INTV Funhouse">
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
    /// Implements the command to update the global directory table on a Locutus device.
    /// </summary>
    internal sealed class UpdateGlobalDirectoryTable : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 2 * 60 * 1000; // may take a long time on nearly full devices

        private UpdateGlobalDirectoryTable(uint address, byte firstGlobalDirectoryNumber, int numberOfEntries)
            : base(ProtocolCommandId.LfsUpdateGdtFromRam, DefaultResponseTimeout, firstGlobalDirectoryNumber, (uint)numberOfEntries, address)
        {
        }

        /// <summary>
        /// Creates an instance of the UpdateGlobalDirectoryTable command.
        /// </summary>
        /// <param name="address">The address in RAM to which to upload the modified directory entries.</param>
        /// <param name="directories">The directories to upload</param>
        /// <returns>A new instance of the command.</returns>
        public static UpdateGlobalDirectoryTable Create(uint address, System.Collections.Generic.IList<IDirectory> directories)
        {
            return new UpdateGlobalDirectoryTable(address, directories[0].GlobalDirectoryNumber, directories.Count);
        }

        /// <summary>
        /// Creates an instance of the UpdateGlobalDirectoryTable command.
        /// </summary>
        /// <param name="address">The address in RAM to which to upload the modified directory entries.</param>
        /// <param name="updateRange">The range (inclusive) of Global Directory Table entries included in the update.</param>
        /// <returns>A new instance of the command.</returns>
        public static UpdateGlobalDirectoryTable Create(uint address, INTV.Core.Utility.Range<byte> updateRange)
        {
            return new UpdateGlobalDirectoryTable(address, updateRange.Minimum, updateRange.Maximum - updateRange.Minimum + 1);
        }
    }
}

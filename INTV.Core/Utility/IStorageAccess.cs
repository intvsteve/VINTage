// <copyright file="IStorageAccess.cs" company="INTV Funhouse">
// Copyright (c) 2017-2018 All Rights Reserved
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

using System;
using System.IO;

namespace INTV.Core.Utility
{
    /// <summary>
    /// Clients supply this interface to provide access to a storage, such as a file system.
    /// </summary>
    public interface IStorageAccess
    {
        /// <summary>
        /// Open a stream to data at the given storage location.
        /// </summary>
        /// <param name="storageLocation">Location of the stream.</param>
        /// <returns>A stream to be used for reading, writing, et. al.</returns>
        Stream Open(string storageLocation);

        /// <summary>
        /// Check whether the given storage location is valid.
        /// </summary>
        /// <param name="storageLocation">Location to check.</param>
        /// <returns><c>true</c> if <paramref name="storageLocation"/> is valid, <c>false</c> otherwise.</returns>
        bool Exists(string storageLocation);

        /// <summary>
        /// Get the size, in bytes, of the data at the given storage location.
        /// </summary>
        /// <param name="storageLocation">Location whose length is desired.</param>
        /// <returns>Length, in bytes, of the data at the given storage location.</returns>
        long Size(string storageLocation);

        /// <summary>
        /// Get the last modification time of the data at the given storage location, in UTC.
        /// </summary>
        /// <param name="storageLocation">Storage location whose modification time is desired.</param>
        /// <returns>The last modification time, in UTC.</returns>
        DateTime LastWriteTimeUtc(string storageLocation);
    }
}

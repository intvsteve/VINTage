// <copyright file="StorageAccess.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Implements INTV.Core.Utility.IStorageAccess for a standard file system.
    /// </summary>
    public class StorageAccess : INTV.Core.Utility.IStorageAccess
    {
        /// <inheritdoc />
        public System.IO.Stream Open(string storageLocation)
        {
            return FileUtilities.OpenFileStream(storageLocation);
        }

        /// <inheritdoc />
        public bool Exists(string storageLocation)
        {
            return System.IO.File.Exists(storageLocation);
        }

        /// <inheritdoc />
        public long Size(string storageLocation)
        {
            return new System.IO.FileInfo(storageLocation).Length;
        }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc(string storageLocation)
        {
            return System.IO.File.GetLastWriteTimeUtc(storageLocation);
        }

        /// <inheritdoc/>
        public bool IsLocationAContainer(string storageLocation)
        {
            var lastCharacter = storageLocation.Last();
            var isContainer = lastCharacter == Path.DirectorySeparatorChar || lastCharacter == Path.AltDirectorySeparatorChar || System.IO.Directory.Exists(storageLocation);
            return isContainer;
        }
    }
}

// <copyright file="ICompressedArchiveEntry.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// This interface describes a basic archive entry.
    /// </summary>
    public interface ICompressedArchiveEntry
    {
        /// <summary>
        /// Gets the name of the entry within the archive.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the size of the uncompressed archive entry.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the last modification date and time of the archive entry.
        /// </summary>
        DateTime LastModificationType { get; }

        /// <summary>
        /// Gets a value indicating whether or not the entry describes a directory.
        /// </summary>
        bool IsDirectory { get; }
    }
}

// <copyright file="FileSystemConstants.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Defines limitations defined by the Locutus file system.
    /// </summary>
    public static class FileSystemConstants
    {
        /// <summary>
        /// The maximum allowed length for a short file name.
        /// </summary>
        public const int MaxShortNameLength = 18;

        /// <summary>
        /// The maximum allowed length for a long file name.
        /// </summary>
        public const int MaxLongNameLength = 60;

        /// <summary>
        /// The maximum number of items a directory may contain.
        /// </summary>
        public const int MaxItemCount = 255;

        /// <summary>
        /// The number of elements allowed in the global directory table.
        /// </summary>
        public const int GlobalDirectoryTableSize = 128;

        /// <summary>
        /// The number of elements allowed in the global file table.
        /// </summary>
        public const int GlobalFileTableSize = 1536;

        /// <summary>
        /// The number of elements allowed in the global fork table.
        /// </summary>
        public const int GlobalForkTableSize = 3072;
    }
}

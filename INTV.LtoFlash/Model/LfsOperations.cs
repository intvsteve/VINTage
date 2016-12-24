// <copyright file="LfsOperations.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// These values enumerate the types of operations executed on a Locutus file system.
    /// </summary>
    [System.Flags]
    public enum LfsOperations
    {
        /// <summary>
        /// No operation.
        /// </summary>
        None = 0,

        /// <summary>
        /// A file system object is to be added.
        /// </summary>
        Add = 1 << 0,

        /// <summary>
        /// A file system object is to be deleted.
        /// </summary>
        Remove = 1 << 1,

        /// <summary>
        /// A file system object is to be updated.
        /// </summary>
        Update = 1 << 2,

        /// <summary>
        /// The file system is being reformatted.
        /// </summary>
        Reformat = 1 << 3,

        /// <summary>
        /// The file system flags are being updated.
        /// </summary>
        UpdateFlags = 1 << 4,

        /// <summary>
        /// Indicates that the operation failed.
        /// </summary>
        FailedOperation = 1 << 31
    }
}

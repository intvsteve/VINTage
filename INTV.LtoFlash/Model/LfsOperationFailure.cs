// <copyright file="LfsOperationFailure.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Describes a filed update operation when sending contents to or receiving contents from a Locutus device.
    /// </summary>
    public class LfsOperationFailure : Tuple<LfsEntityType, ushort, LfsOperations, string, FailedOperationException>
    {
        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="entryType">The type of LFS entity involved with the error.</param>
        /// <param name="id">The unique identifier of the LFS entity (GDN, GFN, GKN).</param>
        /// <param name="operation">The type of operation (add, update, delete, et. al.).</param>
        /// <param name="name">A user-friendly name of the entry involved in the failed operation.</param>
        /// <param name="exception">The exception that occurred, if any.</param>
        public LfsOperationFailure(LfsEntityType entryType, ushort id, LfsOperations operation, string name, FailedOperationException exception)
            : base(entryType, id, operation, name, exception)
        {
        }

        /// <summary>
        /// Gets the type of entry involved with the failed operation.
        /// </summary>
        public LfsEntityType EntryType
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the unique identifier of the entry (GDN, GFN, GKN).
        /// </summary>
        public ushort Id
        {
            get { return Item2; }
        }

        /// <summary>
        /// Gets the kind of operation that failed.
        /// </summary>
        public LfsOperations Operation
        {
            get { return Item3; }
        }

        /// <summary>
        /// Gets the name of the file system entity that a user could recognized (directory, file, or fork name).
        /// </summary>
        public string Name
        {
            get { return Item4; }
        }

        /// <summary>
        /// Gets the exception that occurred, if any.
        /// </summary>
        public FailedOperationException Exception
        {
            get { return Item5; }
        }

        /// <summary>
        /// Gets a brief, user-readable summary of the failure.
        /// </summary>
        public string Description
        {
            get { return string.Format("{0}:{1}:{2} {3}", EntryType, Id, Operation & ~LfsOperations.FailedOperation, Name); }
        }
    }
}

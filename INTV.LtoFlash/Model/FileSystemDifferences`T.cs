// <copyright file="FileSystemDifferences`T.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using System.Collections.Generic;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class stores file system differences.
    /// </summary>
    /// <typeparam name="T">The type of file system entries.</typeparam>
    internal sealed class FileSystemDifferences<T> : Tuple<IList<T>, IList<uint>, IList<T>, IDictionary<string, FailedOperationException>> where T : class, IGlobalFileSystemEntry
    {
        /// <summary>
        /// Initializes a new instance of FileSystemDifferences.
        /// </summary>
        public FileSystemDifferences()
            : base(new List<T>(), new List<uint>(), new List<T>(), new Dictionary<string, FailedOperationException>())
        {
        }

        /// <summary>
        /// Gets the list of items to add.
        /// </summary>
        public IList<T> ToAdd
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the list of items to delete.
        /// </summary>
        public IList<uint> ToDelete
        {
            get { return Item2; }
        }

        /// <summary>
        /// Gets the list of items to update.
        /// </summary>
        public IList<T> ToUpdate
        {
            get { return Item3; }
        }

        /// <summary>
        /// Gets the list of item operations that failed, typically due to missing files on local machine.
        /// </summary>
        public IDictionary<string, FailedOperationException> FailedOperations
        {
            get { return Item4; }
        }

        /// <summary>
        /// Reports whether there are any differences of any kind.
        /// </summary>
        /// <returns><c>true</c> if any differences of any kind are present.</returns>
        public bool Any()
        {
            return (ToAdd.Count > 0) || (ToDelete.Count > 0) || (ToUpdate.Count > 0);
        }
    }
}

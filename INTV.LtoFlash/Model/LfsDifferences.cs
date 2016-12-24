// <copyright file="LfsDifferences.cs" company="INTV Funhouse">
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
using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class provides a convenience wrapper to contain the totality of file system differences between two different Locutus File Systems.
    /// </summary>
    internal sealed class LfsDifferences
    {
        /// <summary>
        /// Initializes a new instance of the LfsDifferences class.
        /// </summary>
        /// <param name="directoryDifferences">The differences between the directory tables of the source and target file systems.</param>
        /// <param name="fileDifferences">The differences between the file tables of the source and target file systems.</param>
        /// <param name="forkDifferences">The differences between the fork tables of the source and target file systems.</param>
        public LfsDifferences(FileSystemDifferences<IDirectory> directoryDifferences, FileSystemDifferences<ILfsFileInfo> fileDifferences, FileSystemDifferences<Fork> forkDifferences)
        {
            DirectoryDifferences = directoryDifferences;
            FileDifferences = fileDifferences;
            ForkDifferences = forkDifferences;
        }

        /// <summary>
        /// Gets the directory differences.
        /// </summary>
        public FileSystemDifferences<IDirectory> DirectoryDifferences { get; private set; }

        /// <summary>
        /// Gets the file differences.
        /// </summary>
        public FileSystemDifferences<ILfsFileInfo> FileDifferences { get; private set; }

        /// <summary>
        /// Gets the fork differences.
        /// </summary>
        public FileSystemDifferences<Fork> ForkDifferences { get; private set; }

        /// <summary>
        /// Reports whether there are any differences of any kind.
        /// </summary>
        /// <returns><c>true</c> if any differences of any kind are present.</returns>
        public bool Any()
        {
            return ForkDifferences.Any() || FileDifferences.Any() || DirectoryDifferences.Any();
        }

        /// <summary>
        /// Determines whether the given directory entry is contained in the directory add, update or delete differences.
        /// </summary>
        /// <param name="directory">The directory to search for.</param>
        /// <param name="operation">Which kind of operations to search for.</param>
        /// <returns><c>true</c> if the given directory participates in any of the given operations.</returns>
        public bool Contains(IDirectory directory, LfsOperations operation)
        {
            var isInDiff = false;
            if (operation.HasFlag(LfsOperations.Add))
            {
                isInDiff |= DirectoryDifferences.ToAdd.Any(d => d.GlobalDirectoryNumber == directory.GlobalDirectoryNumber);
            }
            if (operation.HasFlag(LfsOperations.Update))
            {
                isInDiff |= DirectoryDifferences.ToUpdate.Any(d => d.GlobalDirectoryNumber == directory.GlobalDirectoryNumber);
            }
            if (operation.HasFlag(LfsOperations.Remove))
            {
                isInDiff |= DirectoryDifferences.ToDelete.Contains(directory.GlobalDirectoryNumber);
            }
            return isInDiff;
        }

        /// <summary>
        /// Determines whether the given file entry is contained in the file add, update or delete differences.
        /// </summary>
        /// <param name="file">The file to search for.</param>
        /// <param name="operation">Which kind of operations to search for.</param>
        /// <returns><c>true</c> if the given file participates in any of the given operations.</returns>
        public bool Contains(ILfsFileInfo file, LfsOperations operation)
        {
            var isInDiff = false;
            if (operation.HasFlag(LfsOperations.Add))
            {
                isInDiff |= FileDifferences.ToAdd.Any(f => f.GlobalFileNumber == file.GlobalFileNumber);
            }
            if (operation.HasFlag(LfsOperations.Update))
            {
                isInDiff |= FileDifferences.ToUpdate.Any(f => f.GlobalFileNumber == file.GlobalFileNumber);
            }
            if (operation.HasFlag(LfsOperations.Remove))
            {
                isInDiff |= FileDifferences.ToDelete.Contains(file.GlobalFileNumber);
            }
            return isInDiff;
        }

        /// <summary>
        /// Determines whether the given fork entry is contained in the fork add, update or delete differences.
        /// </summary>
        /// <param name="fork">The fork to search for.</param>
        /// <param name="operation">Which kind of operations to search for.</param>
        /// <returns><c>true</c> if the given fork participates in any of the given operations.</returns>
        public bool Contains(Fork fork, LfsOperations operation)
        {
            var isInDiff = false;
            if (operation.HasFlag(LfsOperations.Add))
            {
                isInDiff |= ForkDifferences.ToAdd.Any(f => f.GlobalForkNumber == fork.GlobalForkNumber);
            }
            if (operation.HasFlag(LfsOperations.Update))
            {
                isInDiff |= ForkDifferences.ToUpdate.Any(f => f.GlobalForkNumber == fork.GlobalForkNumber);
            }
            if (operation.HasFlag(LfsOperations.Remove))
            {
                isInDiff |= ForkDifferences.ToDelete.Contains(fork.GlobalForkNumber) || ForkDifferences.ToUpdate.Any(f => f.GlobalForkNumber == fork.GlobalForkNumber);
            }
            return isInDiff;
        }

        /// <summary>
        /// Accumulates all errors in the differences into a single result.
        /// </summary>
        /// <param name="errorFilter">Optional filter to exclude certain types of errors.</param>
        /// <returns>A dictionary containing all of the failures in this differences set.</returns>
        public IDictionary<string, FailedOperationException> GetAllFailures(Predicate<Exception> errorFilter)
        {
            var allFailures = new Dictionary<string, FailedOperationException>();
            foreach (var entry in DirectoryDifferences.FailedOperations.Where(f => (errorFilter == null) || errorFilter(f.Value)))
            {
                allFailures[entry.Key] = entry.Value;
            }
            foreach (var entry in FileDifferences.FailedOperations.Where(f => (errorFilter == null) || errorFilter(f.Value)))
            {
                allFailures[entry.Key] = entry.Value;
            }
            foreach (var entry in ForkDifferences.FailedOperations.Where(f => (errorFilter == null) || errorFilter(f.Value)))
            {
                allFailures[entry.Key] = entry.Value;
            }
            return allFailures;
        }

        /// <summary>
        /// Accumulates all errors in the differences into a single result, along with pre-existing ones.
        /// </summary>
        /// <param name="otherFailures">Pre-existing errors to retain.</param>
        /// <param name="errorFilter">Optional filter to exclude certain types of errors.</param>
        /// <returns>A dictionary containing all of the failures in this differences set along with those provided.</returns>
        public IDictionary<string, FailedOperationException> CombineAllFailures(IDictionary<string, FailedOperationException> otherFailures, Predicate<Exception> errorFilter)
        {
            var allFailures = GetAllFailures(errorFilter);
            foreach (var entry in otherFailures.Where(f => (errorFilter == null) || errorFilter(f.Value)))
            {
                allFailures[entry.Key] = entry.Value;
            }
            return allFailures;
        }
    }
}

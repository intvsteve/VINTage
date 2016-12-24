// <copyright file="LfsUpdateOperation.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;
using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Describes an update operation that is intended to fit entirely into the available RAM on a Locutus device.
    /// </summary>
    internal class LfsUpdateOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.LfsUpdateOperation"/> class.
        /// </summary>
        /// <param name="updateOperation">The nature of the update operation.</param>
        public LfsUpdateOperation(LfsUpdateOperation updateOperation)
        {
            FileSystem = updateOperation.FileSystem;
            Operation = updateOperation.Operation;
            Directories = new HashSet<byte>(updateOperation.Directories);
            Files = new HashSet<ushort>(updateOperation.Files);
            Forks = new HashSet<ushort>(updateOperation.Forks);
            Failures = new HashSet<LfsOperationFailure>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.LfsUpdateOperation"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="operation">The nature of the operation.</param>
        /// <param name="entityType">The kind of file system entity involved on the operation.</param>
        /// <param name="entityId">The identifier (GDN, GFN, GKN) of the entity involved inthe operation.</param>
        public LfsUpdateOperation(FileSystem fileSystem, LfsOperations operation, LfsEntityType entityType, ushort entityId)
        {
            FileSystem = fileSystem;
            Operation = operation;
            Directories = new HashSet<byte>();
            Files = new HashSet<ushort>();
            Forks = new HashSet<ushort>();
            Failures = new HashSet<LfsOperationFailure>();
        }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        public FileSystem FileSystem { get; private set; }

        /// <summary>
        /// Gets the kind of update operation.
        /// </summary>
        public LfsOperations Operation { get; private set; }

        /// <summary>
        /// Gets the text to display in the progress bar title when executing the operation.
        /// </summary>
        public string ProgressTitle
        {
            get { return GetOperationName(); }
        }

        /// <summary>
        /// Gets the total size of the update in bytes (for data transfer operations).
        /// </summary>
        public uint TotalUpdateSize
        {
            get { return ComputeTotalSize(); }
        }

        /// <summary>
        /// Gets the directories involved in the operation, identified by GDN.
        /// </summary>
        public HashSet<byte> Directories { get; private set; }

        /// <summary>
        /// Gets the range of Global Directory Numbers (GNDs) involved in the operation.
        /// </summary>
        public Range<byte> GdnRange
        {
            get
            {
                var gdnRange = new Range<byte>(Directories.Min(), Directories.Max());
                return gdnRange;
            }
        }

        /// <summary>
        /// Gets the files involved in the operation, identified by GFN.
        /// </summary>
        public HashSet<ushort> Files { get; private set; }

        /// <summary>
        /// Gets the range of Global File Numbers (GFNs) involved in the operation
        /// </summary>
        public Range<ushort> GfnRange
        {
            get
            {
                var gfnRange = new Range<ushort>(Files.Min(), Files.Max());
                return gfnRange;
            }
        }

        /// <summary>
        /// Gets the forks involved in the operation, identified by GKN.
        /// </summary>
        public HashSet<ushort> Forks { get; private set; }

        /// <summary>
        /// Gets the collection of failures that have occurred.
        /// </summary>
        public HashSet<LfsOperationFailure> Failures { get; private set; }

        /// <summary>
        /// Record a failed update operation.
        /// </summary>
        /// <param name="entryType">The kind of entry whose update operation failed.</param>
        /// <param name="id">The LFS identifier of the entity.</param>
        /// <param name="name">User-facing name of the entity.</param>
        /// <param name="exception">The error that occurred.</param>
        /// <param name="targetDeviceId">The unique ID of the target device involved in the operation.</param>
        public void RecordFailure(LfsEntityType entryType, ushort id, string name, Exception exception, string targetDeviceId)
        {
            var error = FailedOperationException.WrapIfNeeded(exception, entryType, id, targetDeviceId);
            RecordFailure(name, error);
        }

        /// <summary>
        /// Record a failed update operation.
        /// </summary>
        /// <param name="name">User-facing name of the entity.</param>
        /// <param name="exception">The error that occurred.</param>
        public void RecordFailure(string name, FailedOperationException exception)
        {
            Operation |= LfsOperations.FailedOperation;
            Failures.Add(new LfsOperationFailure(exception.EntityType, (ushort)exception.GlobalFileSystemNumber, Operation, name, exception));
        }

        /// <summary>
        /// Removes elements from this operation that are included in the given operation.
        /// </summary>
        /// <param name="update">An operation whose entries are to be removed from this instance's entries.</param>
        public void Prune(LfsUpdateOperation update)
        {
            if (!object.ReferenceEquals(this, update))
            {
                Forks.ExceptWith(update.Forks);
                Files.ExceptWith(update.Files);
                Directories.ExceptWith(update.Directories);
                if (!Forks.Any() && !Files.Any() && !Directories.Any())
                {
                    Operation = LfsOperations.None;
                }
            }
        }

        /// <summary>
        /// Merge the specified operation with this one.
        /// </summary>
        /// <param name="operation">The operation whose entries are to be merged into this one.</param>
        /// <returns>The result of the merge.</returns>
        public LfsUpdateOperation Merge(LfsUpdateOperation operation)
        {
            var mergedOperation = new LfsUpdateOperation(this);
            mergedOperation.Operation |= operation.Operation;
            mergedOperation.Forks.UnionWith(operation.Forks);
            mergedOperation.Files.UnionWith(operation.Files);
            mergedOperation.Directories.UnionWith(operation.Directories);
            mergedOperation.Failures.UnionWith(operation.Failures);
            return mergedOperation;
        }

        /// <summary>
        /// Clear all the entries from this operation.
        /// </summary>
        public void Clear()
        {
            Operation = LfsOperations.None;
            Forks.Clear();
            Files.Clear();
            Directories.Clear();
            Failures.Clear();
        }

        private uint ComputeTotalSize()
        {
            var totalGdtUpdateSize = 0u;
            if (Directories.Any())
            {
                var span = GdnRange;
                totalGdtUpdateSize = (uint)((span.Maximum - span.Minimum) + 1) * Directory.FlatSizeInBytes;
            }

            var totalGftUpdateSize = 0u;
            if (Files.Any())
            {
                var span = GfnRange;
                totalGftUpdateSize = (uint)((span.Maximum - span.Minimum) + 1) * LfsFileInfo.FlatSizeInBytes;
            }

            var totalForkSize = 0u;
            foreach (var fork in Forks)
            {
                totalForkSize = totalForkSize.Align(); // accounts for alignment during upload
                totalForkSize += (uint)FileSystem.Forks[fork].SerializeByteCount;
            }
            totalForkSize = totalForkSize.Align(); // we may overestimate by one byte

            var size = totalGdtUpdateSize + totalGftUpdateSize + totalForkSize;
            return size;
        }

        private static readonly Dictionary<LfsOperations, string> OperationNames = new Dictionary<LfsOperations, string>()
            {
                { LfsOperations.Add, Resources.Strings.LfsOperation_Add },
                { LfsOperations.Remove, Resources.Strings.LfsOperation_Remove },
                { LfsOperations.Update, Resources.Strings.LfsOperation_Update },
                { LfsOperations.Add | LfsOperations.Update, Resources.Strings.LfsOperation_Synchronize }
            };

        private string GetOperationName()
        {
            var name = string.Empty;
            if (!OperationNames.TryGetValue(Operation, out name))
            {
                name = Resources.Strings.LfsOperation_Synchronize;
            }
            return name;
        }
    }
}

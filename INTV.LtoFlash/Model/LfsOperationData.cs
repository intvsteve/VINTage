// <copyright file="LfsOperationData.cs" company="INTV Funhouse">
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
    /// This class describes an operation to be applied to the file system on a Locutus device.
    /// </summary>
    public sealed class LfsOperationData
    {
        private const uint InvalidUid = 0xFFFFFFFF;
        private const uint InvalidFileSystemNumber = 0xFFFFFFFF;

        #region Constructors

        private LfsOperationData()
            : this(LfsOperations.None, LfsEntityType.Unknown)
        {
            FileSystemNumber = InvalidFileSystemNumber;
            Uid = InvalidUid;
        }

        private LfsOperationData(LfsOperations operation)
            : this(operation, LfsEntityType.Unknown)
        {
        }

        private LfsOperationData(LfsOperations operation, LfsEntityType type)
        {
            Operation = operation;
            Type = type;
            FileSystemNumber = InvalidFileSystemNumber;
            Uid = InvalidUid;
        }

        private LfsOperationData(LfsOperations operation, LfsEntityType type, IGlobalFileSystemEntry fileSystemEntry)
            : this(operation, type)
        {
            Uid = fileSystemEntry.Uid;
            FileSystemNumber = InvalidFileSystemNumber;
        }

        private LfsOperationData(LfsOperations operation, Fork fork)
            : this(operation, LfsEntityType.Fork, fork)
        {
            FileSystemNumber = fork.GlobalForkNumber;
        }

        private LfsOperationData(LfsOperations operation, ILfsFileInfo file)
            : this(operation, LfsEntityType.File, file)
        {
            FileSystemNumber = file.GlobalFileNumber;
        }

        private LfsOperationData(LfsOperations operation, IDirectory folder)
            : this(operation, LfsEntityType.Directory, folder)
        {
            FileSystemNumber = folder.GlobalDirectoryNumber;
        }

        #endregion // Constructors

        /// <summary>
        /// Gets or sets the kind of operation being described.
        /// </summary>
        /// <remarks>This property is settable because it is deserialized via XML.</remarks>
        public LfsOperations Operation { get; set; }

        /// <summary>
        /// Gets or sets the type of file system object to which the operation applies.
        /// </summary>
        /// <remarks>This property is settable because it is deserialized via XML.</remarks>
        public LfsEntityType Type { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the entity involved in the operation.
        /// </summary>
        /// <remarks>This property is settable because it is deserialized via XML.</remarks>
        public uint Uid { get; set; }

        /// <summary>
        /// Gets or sets the global file system number to which the operation applies.
        /// </summary>
        /// <remarks>This property is settable because it is deserialized via XML.</remarks>
        public uint FileSystemNumber { get; set; }

        /// <summary>
        /// Creates a new LfsOperationData to describe a file system operation.
        /// </summary>
        /// <param name="operation">The kind of operation to create.</param>
        /// <param name="entry">The global file system entry involved in the operation.</param>
        /// <param name="targetType">The type of entity to which the operation applies.</param>
        /// <returns>A new instance of LfsOperationData.</returns>
        public static LfsOperationData Create(LfsOperations operation, IGlobalFileSystemEntry entry, System.Type targetType)
        {
            LfsOperationData operationData = null;
            switch (operation)
            {
                case LfsOperations.Add:
                case LfsOperations.Remove:
                case LfsOperations.Update:
                    if ((entry is IDirectory) && (targetType == typeof(IDirectory)))
                    {
                        operationData = new LfsOperationData(operation, (IDirectory)entry);
                    }
                    else if ((entry is ILfsFileInfo) && (targetType == typeof(ILfsFileInfo)))
                    {
                        operationData = new LfsOperationData(operation, (ILfsFileInfo)entry);
                    }
                    else if (entry is Fork)
                    {
                        operationData = new LfsOperationData(operation, (Fork)entry);
                    }
                    else
                    {
                        throw new System.InvalidOperationException();
                    }
                    break;
                case LfsOperations.UpdateFlags:
                    operationData = new LfsOperationData(operation);
                    break;
                case LfsOperations.Reformat:
                    throw new System.InvalidOperationException(Resources.Strings.Reformat_ArgError);
            }
            return operationData;
        }
    }
}

// <copyright file="GlobalForkTable.cs" company="INTV Funhouse">
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

using System;
using System.Linq;
using INTV.Core.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class models the storage of data forks in the Locutus file system (LFS).
    /// In LFS, each fork is assigned a global fork number (GKN), which is simply an index into this array.
    /// </summary>
    public class GlobalForkTable : INTV.Shared.Utility.FixedSizeCollection<Fork>
    {
        #region Constants

        /// <summary>
        /// The number of elements allowed in the global fork table.
        /// </summary>
        public const int TableSize = FileSystemConstants.GlobalForkTableSize;

        /// <summary>
        /// The value indicating an invalid global fork number.
        /// </summary>
        public const ushort InvalidForkNumber = ushort.MaxValue;

        /// <summary>
        /// The header identifying a GlobalForkTable in a data stream.
        /// </summary>
        public static readonly byte[] Header = new byte[3] { (byte)'G', (byte)'K', (byte)'T' };

        #endregion // Constants

        private FileSystem _fileSystem;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a GlobalForkTable for a specific file system.
        /// </summary>
        /// <param name="fileSystem">The specific file system to which the table belongs.</param>
        public GlobalForkTable(FileSystem fileSystem)
            : base(TableSize)
        {
            _fileSystem = fileSystem;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets an extremely simple estimate of space used in bytes.
        /// </summary>
        public uint EstimatedStorageRequired
        {
            get
            {
                lock (_fileSystem)
                {
                    var estimatedStorageRequired = 0u;
                    foreach (var fork in this)
                    {
                        if (fork != null)
                        {
                            // file system blocks are 8K, so even a 1 byte file takes up 8K of flash storage. Round up to account for that.
                            var forkSize = (fork.Size + 8191u) & ~8191u;
                            estimatedStorageRequired += forkSize;
                        }
                    }
                    return estimatedStorageRequired;
                }
            }
        }

        #endregion // Properties

        /// <summary>
        /// Decompresses data describing a GlobalForkTable.
        /// </summary>
        /// <param name="reader">A binary data reader from which to extract the flattened table.</param>
        /// <param name="rawData">The data stream.</param>
        public static void DecompressData(INTV.Shared.Utility.ASCIIBinaryReader reader, System.IO.Stream rawData)
        {
            FileSystemHelpers.DecompressedFixedSizeCollection(reader, rawData, Header, Fork.FlatSizeInBytes, TableSize);
        }

        /// <summary>
        /// Populates the contents of the table from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader used to read the data.</param>
        /// <returns>The number of bytes read.</returns>
        public int Deserialize(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            GlobalForkTable forkTable = this;
            byte[] header = reader.ReadBytes(Header.Length);
            var bytesRead = header.Length;

            ushort globalForkNumber = 0;
            ushort numEntries = reader.ReadUInt16();
            bytesRead += sizeof(ushort);

            int numChunksToRead = (int)Math.Ceiling((double)numEntries / 8);
            for (int i = 0; i < numChunksToRead; ++i)
            {
                var validEntriesMask = reader.ReadByte();
                ++bytesRead;
                for (int e = 0; e < 8; ++e)
                {
                    byte mask = (byte)(1 << e);
                    if ((mask & validEntriesMask) == mask)
                    {
                        var fork = Fork.Inflate(reader);
                        fork.FileSystem = _fileSystem;
                        bytesRead += fork.DeserializeByteCount;
                        fork.GlobalForkNumber = globalForkNumber;
                        forkTable[globalForkNumber] = fork;
                    }
                    ++globalForkNumber;
                }
            }
            return bytesRead;
        }

        /// <summary>
        /// Adds a fork based upon a file in the local file system.
        /// </summary>
        /// <param name="filePath">The fully qualified path for which to create a Fork.</param>
        /// <returns>A Fork object referring to the file.</returns>
        /// <remarks>Note that the function may return a preexisting Fork already present in the table.</remarks>
        public Fork AddFork(string filePath)
        {
            lock (_fileSystem)
            {
                var fork = new Fork(filePath);
                Add(fork); // we may end up discarding the newly created fork and return an existing one (dedup)
                return this[fork.GlobalForkNumber];
            }
        }

        /// <summary>
        /// Adds a fork based upon a ROM in the local file system.
        /// </summary>
        /// <param name="rom">A ROM in the local file system.</param>
        /// <returns>A Fork object referring to the ROM.</returns>
        /// <remarks>Note that the function may return a preexisting Fork already present in the table.</remarks>
        public Fork AddFork(IRom rom)
        {
            lock (_fileSystem)
            {
                var fork = new Fork(rom);
                Add(fork); // we may end up discarding the newly created fork and return an existing one (dedup)
                return this[fork.GlobalForkNumber];
            }
        }

        /// <summary>
        /// Adds a fork at a predefined location, relocating a fork that may already exist at that location.
        /// </summary>
        /// <param name="fork">The fork to add.</param>
        public void AddAndRelocate(Fork fork)
        {
            lock (_fileSystem)
            {
                var collidingFork = this[fork.GlobalForkNumber];
                if (!ReferenceEquals(collidingFork, fork))
                {
                    if (collidingFork != null)
                    {
                        collidingFork.GlobalForkNumber = InvalidForkNumber;
                        this[fork.GlobalForkNumber] = null;
                    }
                    this.Add(fork);
                    if (collidingFork != null)
                    {
                        this.Add(collidingFork);
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>If the provided Fork is already in the table based on equality check, the one passed in will not
        /// be stored in the table. Therefore, direct calls to this function should be treated with caution.
        /// The preferred way to add elements is via the AddFork method.</remarks>
        public override void Add(Fork item)
        {
            lock (_fileSystem)
            {
                var currentGlobalForkNumber = item.GlobalForkNumber;
                if (currentGlobalForkNumber != InvalidForkNumber)
                {
                    var itemAtCurrentLocation = this[currentGlobalForkNumber];
                    if (itemAtCurrentLocation == null)
                    {
                        item.FileSystem = _fileSystem;
                        this[currentGlobalForkNumber] = item;
                        OnCollectionChanged(item, null, currentGlobalForkNumber);
                        return;
                    }
                    else if (itemAtCurrentLocation == item)
                    {
                        return;
                    }
                    else
                    {
                        ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "Attempted to add item to location already occupied in the Global Fork Table.", "GlobalForkTable");
                    }
                }
                var index = IndexOf(item);
                if (index >= 0)
                {
                    item.GlobalForkNumber = (ushort)index;
                    var itemAtCurrentLocation = this[index];
                    var replaceFork = itemAtCurrentLocation == null;
                    if (!replaceFork)
                    {
                        if (string.IsNullOrEmpty(itemAtCurrentLocation.FilePath))
                        {
                            // Fork's file path is no good, so if the new fork's path is, use it instead.
                            replaceFork = !string.IsNullOrEmpty(item.FilePath);
                        }
                    }
                    if (replaceFork)
                    {
                        item.FileSystem = _fileSystem;
                        this[index] = item;
                        OnCollectionChanged(item, null, index);
                    }
                }
                else
                {
                    item.FileSystem = _fileSystem;
                    index = AddItem(item);
                    if ((index >= 0) && (item.GlobalForkNumber == InvalidForkNumber))
                    {
                        item.GlobalForkNumber = (ushort)index;
                    }
                    if (item.GlobalForkNumber != index)
                    {
                        ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "A fork was added to the GlobalForkTable, but its private GKN does not match the assigned location.", "GlobalForkTable");
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>Removing an item from a GlobalForkTable does not necessarily eliminate it from the table. If other elements in the
        /// owning FileSystem refer to the fork, it remains in the table.</remarks>
        public override void RemoveAt(int index)
        {
            lock (_fileSystem)
            {
                var fork = this[index];
                bool purge = true;
                if (fork != null)
                {
                    purge = !_fileSystem.Files.Any(f => f.UsesFork(fork));
                }
                if (purge)
                {
                    base.RemoveAt(index);
                }
            }
        }
    }
}

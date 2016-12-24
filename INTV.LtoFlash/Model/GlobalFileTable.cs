// <copyright file="GlobalFileTable.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class models the storage of files in the Locutus file system (LFS).
    /// In LFS, each file is assigned a global file number (GFN), which is simply an index into this array.
    /// </summary>
    public class GlobalFileTable : INTV.Shared.Utility.FixedSizeCollection<ILfsFileInfo>
    {
        #region Constants

        /// <summary>
        /// The number of elements allowed in the global file table.
        /// </summary>
        public const int TableSize = FileSystemConstants.GlobalFileTableSize;

        /// <summary>
        /// The value indicating an invalid global file number.
        /// </summary>
        public const ushort InvalidFileNumber = ushort.MaxValue;

        /// <summary>
        /// The root directory's parent global file number.
        /// </summary>
        public const ushort RootDirectoryFileNumber = 0;

        /// <summary>
        /// The header identifying a GlobalFileTable in a data stream.
        /// </summary>
        public static readonly byte[] Header = new byte[3] { (byte)'G', (byte)'F', (byte)'T' };

        #endregion // Constants

        private FileSystem _fileSystem;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a GlobalFileTable for a specific file system.
        /// </summary>
        /// <param name="fileSystem">The specific file system to which the table belongs.</param>
        public GlobalFileTable(FileSystem fileSystem)
            : base(TableSize)
        {
            _fileSystem = fileSystem;
        }

        #endregion // Constructors

        /// <summary>
        /// Decompresses data describing a GlobalFileTable.
        /// </summary>
        /// <param name="reader">A binary data reader from which to extract the flattened table.</param>
        /// <param name="destination">Receives the decompressed data.</param>
        public static void DecompressData(INTV.Shared.Utility.ASCIIBinaryReader reader, System.IO.Stream destination)
        {
            FileSystemHelpers.DecompressedFixedSizeCollection(reader, destination, Header, LfsFileInfo.FlatSizeInBytes, TableSize);
        }

        /// <summary>
        /// Populates the contents of the table from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader used to read the data.</param>
        /// <returns>The number of bytes read.</returns>
        public int Deserialize(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            GlobalFileTable fileTable = this;
            byte[] header = reader.ReadBytes(Header.Length);
            var bytesRead = header.Length;

            ushort globalFileNumber = 0;
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
                        var file = LfsFileInfo.Inflate(reader);
                        file.FileSystem = _fileSystem;
                        bytesRead += file.DeserializeByteCount;
                        file.GlobalFileNumber = globalFileNumber;
                        fileTable[globalFileNumber] = file;
                    }
                    ++globalFileNumber;
                }
            }
            return bytesRead;
        }

        /// <summary>
        /// Adds a file at a predefined location, relocating a file that may already exist at that location.
        /// </summary>
        /// <param name="item">The file to add.</param>
        public void AddAndRelocate(ILfsFileInfo item)
        {
            lock (_fileSystem)
            {
                var collidingItem = this[item.GlobalFileNumber];
                if (collidingItem != null)
                {
                    collidingItem.GlobalFileNumber = InvalidFileNumber;
                    this[item.GlobalFileNumber] = null;
                }
                this.Add(item);
                if (collidingItem != null)
                {
                    this.Add(collidingItem);
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>In addition to placing the entry into the table, this function will also assign a
        /// Global File Number to the item if it has not already been assigned one. Furthermore, if the
        /// entry describes a directory, it will also be assigned an entry in the Global Directory Table.
        /// Furthermore, adding a directory also results in the recursive addition of the directory's
        /// contents to the GlobalFileTable and GlobalDirectoryTable as appropriate.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if an item is added to the table but its
        /// GFN does not properly match its actual location in the table.</exception>
        public override void Add(ILfsFileInfo item)
        {
            lock (_fileSystem)
            {
                var currentGlobalFileNumber = item.GlobalFileNumber;
                if (currentGlobalFileNumber != InvalidFileNumber)
                {
                    var itemAtCurrentLocation = this[currentGlobalFileNumber];
                    if (itemAtCurrentLocation == null)
                    {
                        this[currentGlobalFileNumber] = item;
                        OnCollectionChanged(item, null, currentGlobalFileNumber);
                        return;
                    }
                    else if (itemAtCurrentLocation == item)
                    {
                        return;
                    }
                    else
                    {
                        ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "Attempted to add item to location already occupied in the Global File Table.", "GlobalFileTable");
                    }
                }
                var index = AddItem(item);
                if ((index >= 0) && (item.GlobalFileNumber == InvalidFileNumber))
                {
                    item.GlobalFileNumber = (ushort)index;
                }
                if (item.GlobalFileNumber != index)
                {
                    ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "A file was added to the GlobalFileTable, but its private GFN does not match the assigned location.", "GlobalFileTable");
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>If the location at the given index represents a directory, its contents will be recursively
        /// removed from the GlobalFileTable and GlobalDirectoryTable as appropriate.</remarks>
        public override void RemoveAt(int index)
        {
            lock (_fileSystem)
            {
                var element = this[index];
                base.RemoveAt(index);
                if (element != null)
                {
                    element.RemoveForks();
                }
                var directory = element as IDirectory;
                if (directory != null)
                {
                    _fileSystem.Directories.RemoveAt(element.GlobalDirectoryNumber);
                    var folder = (Folder)element;
                    foreach (var entry in folder.Files)
                    {
                        RemoveAt(entry.GlobalFileNumber);
                    }
                }
            }
        }
    }
}

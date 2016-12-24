// <copyright file="GlobalDirectoryTable.cs" company="INTV Funhouse">
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
    /// This class models the storage of directories in the Locutus file system (LFS).
    /// In LFS, each directory is assigned a global directory number (GDN), which is simply an index into this array.
    /// </summary>
    public class GlobalDirectoryTable : INTV.Shared.Utility.FixedSizeCollection<IDirectory>
    {
        #region Constants

        /// <summary>
        /// The number of elements allowed in the global directory table.
        /// </summary>
        public const int TableSize = FileSystemConstants.GlobalDirectoryTableSize;

        /// <summary>
        /// The value indicating an invalid global directory number.
        /// </summary>
        public const byte InvalidDirectoryNumber = byte.MaxValue;

        /// <summary>
        /// The global directory number reserved as the number for the root directory of the file system.
        /// </summary>
        public const byte RootDirectoryNumber = 0;

        /// <summary>
        /// The header identifying a GlobalDirectoryTable in a data stream.
        /// </summary>
        public static readonly byte[] Header = new byte[3] { (byte)'G', (byte)'D', (byte)'T' };

        #endregion // Constants

        private FileSystem _fileSystem;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a GlobalDirectoryTable for a specific file system.
        /// </summary>
        /// <param name="fileSystem">The specific file system to which the table belongs.</param>
        public GlobalDirectoryTable(FileSystem fileSystem)
            : base(TableSize)
        {
            _fileSystem = fileSystem;
        }

        #endregion // Constructors

        /// <summary>
        /// Decompresses data describing a GlobalDirectoryTable.
        /// </summary>
        /// <param name="reader">A binary data reader from which to extract the flattened table.</param>
        /// <param name="rawData">The data stream.</param>
        public static void DecompressData(INTV.Shared.Utility.ASCIIBinaryReader reader, System.IO.Stream rawData)
        {
            FileSystemHelpers.DecompressedFixedSizeCollection(reader, rawData, Header, Directory.FlatSizeInBytes, TableSize);
        }

        /// <summary>
        /// Populates the contents of the table from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader used to read the data.</param>
        /// <returns>The number of bytes read.</returns>
        public int Deserialize(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            GlobalDirectoryTable directoryTable = this;
            byte[] header = reader.ReadBytes(Header.Length);
            var bytesRead = header.Length;

            byte globalDirectoryNumber = 0;
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
                        var directory = Directory.Inflate(reader);
                        directory.FileSystem = _fileSystem;
                        bytesRead += directory.DeserializeByteCount;
                        directory.GlobalDirectoryNumber = globalDirectoryNumber;
                        directoryTable[globalDirectoryNumber] = directory;
                    }
                    ++globalDirectoryNumber;
                }
            }
            return bytesRead;
        }

        /// <summary>
        /// Adds a directory at a predefined location, relocating a directory that may already exist at that location.
        /// </summary>
        /// <param name="directory">The directory to add.</param>
        public void AddAndRelocate(IDirectory directory)
        {
            lock (_fileSystem)
            {
                var collidingDirectory = this[directory.GlobalDirectoryNumber];
                if (collidingDirectory != null)
                {
                    collidingDirectory.GlobalDirectoryNumber = InvalidDirectoryNumber; // removes from parent
                }
                this.Add(directory);
                if (collidingDirectory != null)
                {
                    this.Add(collidingDirectory);
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>In addition to placing the directory into the table, this function will assign a
        /// Global Directory Number to the item if it has not already been assigned one.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if a directory is added to the table but its
        /// GDN does not properly match its actual location in the table.</exception>
        public override void Add(IDirectory item)
        {
            lock (_fileSystem)
            {
                var directory = (ILfsFileInfo)item;
                var currentGlobalDirectoryNumber = directory.GlobalDirectoryNumber;
                if (currentGlobalDirectoryNumber != InvalidDirectoryNumber)
                {
                    var itemAtCurrentLocation = this[currentGlobalDirectoryNumber];
                    if (itemAtCurrentLocation == null)
                    {
                        this[currentGlobalDirectoryNumber] = item;
                        OnCollectionChanged(item, null, currentGlobalDirectoryNumber);
                        return;
                    }
                    else if (itemAtCurrentLocation == item)
                    {
                        return;
                    }
                    else
                    {
                        ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "Attempted to add item to location already occupied in the Global Directory Table.", "GlobalDirectoryTable");
                    }
                }
                var index = AddItem(item);
                if ((index >= 0) && (directory.GlobalDirectoryNumber == InvalidDirectoryNumber))
                {
                    directory.GlobalDirectoryNumber = (byte)index;
                }
                if (directory.GlobalDirectoryNumber != index)
                {
                    ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "A directory was added to the GlobalDirectoryTable, but its private GDN does not match the assigned location.", "GlobalDirectoryTable");
                }
            }
        }
    }
}

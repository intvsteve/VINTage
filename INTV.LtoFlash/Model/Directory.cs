// <copyright file="Directory.cs" company="INTV Funhouse">
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

using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation of the LFS IDirectory interface for communicating with a Locutus device.
    /// </summary>
    public class Directory : INTV.Core.Utility.ByteSerializer, IDirectory
    {
        /// <summary>
        /// The size of the flattened binary structure, in bytes.
        /// </summary>
        public const int FlatSizeInBytes = PresentationOrder.FlatSizeInBytes + sizeof(ushort);

        /// <summary>
        /// Represents an invalid directory.
        /// </summary>
        public static readonly Directory InvalidDirectory;

        private ushort _parentDirectoryGlobalFileNumber;
        private PresentationOrder _presentationOrder;
        private byte _globalDirectoryNumber;

        #region Constructors

        /// <summary>
        /// Performs one-time initialization for the Directory class.
        /// </summary>
        static Directory()
        {
            ErrorReporting.ReportErrorIf<System.InvalidOperationException>(FlatSizeInBytes != 512, ReportMechanism.Default, "Directory flat size is incorrect.", "Directory Sanity Check");
            InvalidDirectory = new Directory();
        }

        /// <summary>
        /// Initializes a new instance of a Directory.
        /// </summary>
        public Directory()
        {
            _parentDirectoryGlobalFileNumber = GlobalFileTable.InvalidFileNumber;
            _globalDirectoryNumber = GlobalDirectoryTable.InvalidDirectoryNumber;
            _presentationOrder = new PresentationOrder();
        }

        /// <summary>
        /// Initializes a new instance of a Directory from an IFileContainer.
        /// </summary>
        /// <param name="folder">The file container from which to create a Directory object for serialization.</param>
        /// <remarks>This constructor is used to convert from a user interface representation of a folder in the
        /// menu layout to a LFS Directory representation.</remarks>
        internal Directory(IFileContainer folder)
        {
            if (folder is Folder)
            {
                var directory = (Folder)folder;
                GlobalDirectoryNumber = directory.GlobalDirectoryNumber;
                ParentDirectoryGlobalFileNumber = directory.ParentDirectoryGlobalFileNumber;
            }
            _presentationOrder = new PresentationOrder();
            int i = 0;
            foreach (var item in ((Folder)folder).Files)
            {
                if (item.GlobalFileNumber >= GlobalFileTable.TableSize)
                {
                    ErrorReporting.ReportError<System.InvalidOperationException>(ReportMechanism.Default, "Bad GFN", "Directory");
                }
                _presentationOrder[i] = item.GlobalFileNumber;
                ++i;
            }
        }

        #endregion // Constructors

        #region Properties

        #region IDirectory Properties

        /// <inheritdoc />
        public ushort ParentDirectoryGlobalFileNumber
        {
            get { return _parentDirectoryGlobalFileNumber; }
            set { _parentDirectoryGlobalFileNumber = value; }
        }

        /// <inheritdoc />
        public PresentationOrder PresentationOrder
        {
            get { return _presentationOrder; }
        }

        /// <inheritdoc />
        public byte GlobalDirectoryNumber
        {
            get { return _globalDirectoryNumber; }
            set { _globalDirectoryNumber = value; }
        }

        #endregion // IDirectory

        #region IGlobalFileSystemEntry

        /// <inheritdoc />
        public int EntryUpdateSize
        {
            get { return FlatSizeInBytes; }
        }

        /// <inheritdoc />
        /// <remarks>The unique ID is computed using the Directory's file descriptor UID as well as the UIDs of the files it contains.</remarks>
        public uint Uid
        {
            get
            {
                uint uid = FileSystem.Files[ParentDirectoryGlobalFileNumber].Uid;
                foreach (var entry in PresentationOrder)
                {
                    if (entry == GlobalFileTable.InvalidFileNumber)
                    {
                        break;
                    }
                    else
                    {
                        uid ^= FileSystem.Files[entry].Uid;
                    }
                }
                return uid;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return FileSystem.Files[ParentDirectoryGlobalFileNumber].Name; }
        }

        /// <inheritdoc />
        public FileSystem FileSystem
        {
            get;
            internal set;
        }

        #endregion // IGlobalFileSystemEntry

        #region ByteSerializer

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        #endregion // ByteSerializer

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a Directory by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a Directory.</returns>
        public static Directory Inflate(System.IO.Stream stream)
        {
            return Inflate<Directory>(stream);
        }

        /// <summary>
        /// Creates a new instance of a Directory by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a Directory.</returns>
        public static Directory Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<Directory>(reader);
        }

        #region IGlobalFileSystemEntry

        /// <inheritdoc />
        public IGlobalFileSystemEntry Clone(FileSystem fileSystem)
        {
            throw new System.NotImplementedException();
        }

        #endregion // IGlobalFileSystemEntry

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            writer.Write(ParentDirectoryGlobalFileNumber);
            _presentationOrder.Serialize(writer);
            return SerializeByteCount;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            _parentDirectoryGlobalFileNumber = reader.ReadUInt16();
            _presentationOrder = PresentationOrder.Inflate(reader);
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer

        #region object Overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return "Directory {ID: " + GlobalDirectoryNumber + " Contains " + _presentationOrder.ValidEntryCount + " Valid Entries}";
        }

        #endregion // object Overrides
    }
}

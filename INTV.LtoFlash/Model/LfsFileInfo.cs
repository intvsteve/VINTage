// <copyright file="LfsFileInfo.cs" company="INTV Funhouse">
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

using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation of LFS ILfsFileInfo interface for communicating with a Locutus device.
    /// </summary>
    public class LfsFileInfo : INTV.Core.Utility.ByteSerializer, ILfsFileInfo
    {
        #region Constants

        /// <summary>
        /// The size of the flattened binary structure, in bytes.
        /// </summary>
        public const int FlatSizeInBytes = sizeof(FileType) + sizeof(INTV.Core.Model.Stic.Color)
            + FileSystemConstants.MaxShortNameLength + FileSystemConstants.MaxLongNameLength + sizeof(byte) + sizeof(byte)
            + (MaxForks * sizeof(ushort));

        /// <summary>
        /// The maximum number of Fork entries allowed in a LfsFileInfo.
        /// </summary>
        public const int MaxForks = (int)ForkKind.NumberOfForkKinds;

        /// <summary>
        /// Represents an invalid Locutus File System file, e.g. "Not A File".
        /// </summary>
        public static readonly LfsFileInfo InvalidFile;

        #endregion // Constants

        private ushort[] _forks;

        #region Constructors

        /// <summary>
        /// Performs one-time initialization for the LfsFileInfo class.
        /// </summary>
        static LfsFileInfo()
        {
            INTV.Shared.Utility.ErrorReporting.ReportErrorIf<System.InvalidOperationException>(MaxForks != (int)ForkKind.NumberOfForkKinds, INTV.Shared.Utility.ReportMechanism.Default, "LfsFileInfo has incorrect number of forks.", "LfsFileInfo Sanity Check");
            InvalidFile = new LfsFileInfo() { Color = Core.Model.Stic.Color.NotAColor };
        }

        /// <summary>
        /// Initializes a new instance of LfsFileInfo.
        /// </summary>
        public LfsFileInfo()
        {
            FileType = FileType.None;
            Color = INTV.Core.Model.Stic.Color.White;
            GlobalDirectoryNumber = GlobalDirectoryTable.InvalidDirectoryNumber;
            GlobalFileNumber = GlobalFileTable.InvalidFileNumber;
            Reserved = 0xFF;
            _forks = new ushort[MaxForks];
            for (int i = 0; i < _forks.Length; ++i)
            {
                _forks[i] = GlobalForkTable.InvalidForkNumber;
            }
        }

        /// <summary>
        /// Initializes a new instance of a LfsFileInfo from an IFile.
        /// </summary>
        /// <param name="file">The file from which to create a LfsFileInfo object for serialization.</param>
        /// <remarks>This constructor is used to convert from a user interface representation of a file in the
        /// menu layout to a LFS LfsFileInfo representation.</remarks>
        public LfsFileInfo(IFile file)
        {
            FileType = file.FileType;
            Color = file.Color;
            ShortName = file.ShortName;
            LongName = file.LongName;
            if (string.IsNullOrEmpty(ShortName))
            {
                System.Diagnostics.Debug.WriteLine("Null or empty short name.");
            }
            if (string.IsNullOrEmpty(LongName))
            {
                System.Diagnostics.Debug.WriteLine("Null or empty long name.");
            }
            _forks = new ushort[MaxForks];
            if (file is FileNode)
            {
                var program = (FileNode)file;
                GlobalFileNumber = program.GlobalFileNumber;
                GlobalDirectoryNumber = program.GlobalDirectoryNumber;
                if (string.IsNullOrEmpty(ShortName) && !string.IsNullOrEmpty(LongName) && (GlobalFileNumber != GlobalFileTable.RootDirectoryFileNumber))
                {
                    ShortName = LongName.Substring(0, System.Math.Min(LongName.Length, FileSystemConstants.MaxShortNameLength));
                }
                Reserved = program.Reserved;
                for (int i = 0; i < _forks.Length; ++i)
                {
                    _forks[i] = program.ForkNumbers[i];
                }
            }
            else
            {
                for (int i = 0; i < _forks.Length; ++i)
                {
                    _forks[i] = GlobalForkTable.InvalidForkNumber;
                }
            }
        }

        #endregion // Constructors

        #region Properties

        #region ILfsFileInfo Properties

        /// <inheritdoc />
        public FileType FileType { get; private set; }

        /// <inheritdoc />
        public INTV.Core.Model.Stic.Color Color { get; set; }

        /// <inheritdoc />
        public string ShortName { get; set; }

        /// <inheritdoc />
        public string LongName { get; set; }

        /// <inheritdoc />
        public ushort GlobalFileNumber { get; set; }

        /// <inheritdoc />
        public byte GlobalDirectoryNumber { get; set; }

        /// <inheritdoc />
        public byte Reserved { get; private set; }

        public ushort[] ForkNumbers
        {
            get { return _forks; }
        }

        /// <inheritdoc />
        public Fork Rom
        {
            get { return GetFork(ForkKind.Program); }
            set { SetFork(ForkKind.Program, value); }
        }

        /// <inheritdoc />
        public Fork Manual
        {
            get { return GetFork(ForkKind.Manual); }
            set { SetFork(ForkKind.Manual, value); }
        }

        /// <inheritdoc />
        public Fork JlpFlash
        {
            get { return GetFork(ForkKind.JlpFlash); }
            set { SetFork(ForkKind.JlpFlash, value); }
        }

        /// <inheritdoc />
        public Fork Vignette
        {
            get { return GetFork(ForkKind.Vignette); }
            set { SetFork(ForkKind.Vignette, value); }
        }

        /// <inheritdoc />
        public Fork ReservedFork4
        {
            get { return GetFork(ForkKind.Reserved4); }
            set { SetFork(ForkKind.Reserved4, value); }
        }

        /// <inheritdoc />
        public Fork ReservedFork5
        {
            get { return GetFork(ForkKind.Reserved5); }
            set { SetFork(ForkKind.Reserved5, value); }
        }

        /// <inheritdoc />
        public Fork ReservedFork6
        {
            get { return GetFork(ForkKind.Reserved6); }
            set { SetFork(ForkKind.Reserved6, value); }
        }

        #endregion // ILfsFileInfo Properties

        #region IGlobalFileSystemEntry

        /// <inheritdoc />
        public int EntryUpdateSize
        {
            get { return FlatSizeInBytes; }
        }

        /// <inheritdoc />
        public uint Uid
        {
            get
            {
                const uint InvalidUid = 0xFFFFFFFF;
                uint uid = InvalidUid;
                for (int i = 0; i < (int)ForkKind.NumberOfForkKinds; ++i)
                {
                    var fork = GetFork((ForkKind)i);
                    if ((fork != null) && (fork.Uid != Fork.InvalidUid))
                    {
                        uid ^= fork.Uid;
                    }
                }
                return uid;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return LongName; }
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

        #endregion ByteSerializer

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a LfsFileInfo by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a LfsFileInfo.</returns>
        public static LfsFileInfo Inflate(System.IO.Stream stream)
        {
            return Inflate<LfsFileInfo>(stream);
        }

        /// <summary>
        /// Creates a new instance of a LfsFileInfo by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a LfsFileInfo.</returns>
        public static LfsFileInfo Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<LfsFileInfo>(reader);
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
            if (FileType != FileType.None)
            {
                writer.Write((byte)FileType);
                writer.Write((byte)Color);
                byte[] shortNameBuffer = new byte[FileSystemConstants.MaxShortNameLength];
                var bufferFillStart = 1;
                if (!string.IsNullOrEmpty(ShortName))
                {
                    bufferFillStart = ShortName.Length + 1;
                    System.Text.Encoding.ASCII.GetBytes(ShortName, 0, System.Math.Min(ShortName.Length, FileSystemConstants.MaxShortNameLength), shortNameBuffer, 0);
                }
                for (int i = bufferFillStart; i < FileSystemConstants.MaxShortNameLength; ++i)
                {
                    shortNameBuffer[i] = 0xFF;
                }
                writer.Write(shortNameBuffer, 0, FileSystemConstants.MaxShortNameLength);

                byte[] longNameBuffer = new byte[FileSystemConstants.MaxLongNameLength];
                bufferFillStart = 1;
                if (!string.IsNullOrEmpty(LongName))
                {
                    bufferFillStart = LongName.Length + 1;
                    System.Text.Encoding.ASCII.GetBytes(LongName, 0, System.Math.Min(LongName.Length, FileSystemConstants.MaxLongNameLength), longNameBuffer, 0);
                }
                for (int i = bufferFillStart; i < FileSystemConstants.MaxLongNameLength; ++i)
                {
                    longNameBuffer[i] = 0xFF;
                }

                writer.Write(longNameBuffer, 0, FileSystemConstants.MaxLongNameLength);
                writer.Write(GlobalDirectoryNumber);
                writer.Write(Reserved);
                foreach (var globalForkNumber in _forks)
                {
                    writer.Write(globalForkNumber);
                }
            }
            else
            {
                for (var i = 0; i < SerializeByteCount; ++i)
                {
                    writer.Write((byte)0xFF);
                }
            }
            return SerializeByteCount;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            FileType = (FileType)reader.ReadByte();
            Color = (INTV.Core.Model.Stic.Color)reader.ReadByte();

            var nameBuffer = reader.ReadBytes(FileSystemConstants.MaxShortNameLength);
            var nameLength = System.Array.IndexOf(nameBuffer, (byte)0);
            if (nameLength < 0)
            {
                nameLength = FileSystemConstants.MaxShortNameLength;
            }
            if (nameLength > 0)
            {
                ShortName = System.Text.Encoding.ASCII.GetString(nameBuffer, 0, nameLength);
            }

            nameBuffer = reader.ReadBytes(FileSystemConstants.MaxLongNameLength);
            nameLength = System.Array.IndexOf(nameBuffer, (byte)0);
            if (nameLength < 0)
            {
                nameLength = FileSystemConstants.MaxLongNameLength;
            }
            if (nameLength > 0)
            {
                LongName = System.Text.Encoding.ASCII.GetString(nameBuffer, 0, nameLength);
            }

            GlobalDirectoryNumber = reader.ReadByte();
            Reserved = reader.ReadByte();

            for (int i = (int)ForkKind.FirstKind; i < (int)ForkKind.NumberOfForkKinds; ++i)
            {
                _forks[i] = reader.ReadUInt16();
            }
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer

        #region object Overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return "LfsFileInfo {ID: " + GlobalFileNumber + " Long: " + LongName + ", Short: " + ShortName + "}";
        }

        #endregion // object Overrides

        private Fork GetFork(ForkKind which)
        {
            Fork fork = null;
            var globalForkNumber = _forks[(int)which];
            if (globalForkNumber < GlobalForkTable.TableSize)
            {
                fork = FileSystem.Forks[globalForkNumber];
            }
            return fork;
        }

        private void SetFork(ForkKind which, Fork fork)
        {
            _forks[(int)which] = (fork == null) ? GlobalForkTable.InvalidForkNumber : fork.GlobalForkNumber;
        }
    }
}

// <copyright file="Fork.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

////#define REPORT_PERFORMANCE

using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class describes a Locutus File System (LFS) fork. It consists of a starting
    /// virtual address block and a size. The specification states that the virtual
    /// blocks in a fork shall be contiguous.
    /// </summary>
    /// <remarks>From the specification, the flat byte layout of a Fork is:
    /// Start   End     Description
    /// 0       1       Starting virtual block.  0xFFFF means "not present"
    /// 2       4       Length of fork in bytes.
    /// Expressed as a C / C++ data structure, each GKT entry is laid out as follows:
    /// struct fork_info
    /// {
    ///     uint8_t     vblk[2];        // virtual block number (LSB first)
    ///     uint8_t     length[3];      // length in bytes      (LSB first)
    ///     uint8_t     uid[3];         // UID of the data fork (LSB first)
    /// };
    /// </remarks>
    public class Fork : INTV.Core.Utility.ByteSerializer, IGlobalFileSystemEntry
    {
#if REPORT_PERFORMANCE
        private static System.TimeSpan AccumulatedRefreshTime { get; set; }
        private static System.TimeSpan AccumulatedUpdateFilePathTime { get; set; }
        private static System.TimeSpan AccumulatedUpdateFilePathRawTime { get; set; }
        private static System.TimeSpan AccumulatedUpdateFilePathForcedTime { get; set; }
        private static System.TimeSpan AccumulatedUpdateFilePathForcedPrepareTime { get; set; }
#endif // REPORT_PERFORMANCE

        #region Constants

        /// <summary>
        /// The size of the flattened binary structure, in bytes.
        /// </summary>
        public const int FlatSizeInBytes = 8;

        /// <summary>
        /// This is the theoretical maximum Fork size (1MB).
        /// </summary>
        public const uint MaxForkSize = INTV.Core.Model.Program.LtoFlashFeaturesHelpers.MaxFileSize;

        /// <summary>
        /// Invalid unique ID.
        /// </summary>
        public const uint InvalidUid = INTV.Core.Utility.Crc24.InvalidCrc;

        /// <summary>
        /// The root file in the file system may store menu position data in a fork that is
        /// identified with this special UID. It should be the menu fork of the root file.
        /// </summary>
        public const uint MenuPositionForkUid = 0xC0FFEE;

        #endregion // Constants

        /// <summary>
        /// Represents an invalid, unused, or empty, fork.
        /// </summary>
        public static readonly Fork Empty = new Fork();

        /// <summary>
        /// Represents an invalid, unused, or empty, fork.
        /// </summary>
        public static readonly Fork InvalidFork = Empty;

        private string _filePath;
        private FileSystem _fileSystem;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a Fork.
        /// </summary>
        public Fork()
        {
            StartingVirtualBlock = 0xFFFF;
            Crc24 = INTV.Core.Utility.Crc24.InvalidCrc;
            GlobalForkNumber = GlobalForkTable.InvalidForkNumber;
        }

        /// <summary>
        /// Initializes a new instance of Fork with a known CRC24, size, and GKN.
        /// </summary>
        /// <param name="crc24">The 24-bit CRC of the fork.</param>
        /// <param name="size">The size of the fork, in bytes.</param>
        /// <param name="globalForkNumber">The Global ForK Number.</param>
        public Fork(uint crc24, uint size, ushort globalForkNumber)
            : this()
        {
            Crc24 = crc24;
            Size = size;
            GlobalForkNumber = globalForkNumber;

            // TODO The one caller of this constructor contains comments indicating it may not be possible to reach this code.
            System.Diagnostics.Debug.WriteLine("TODO TODO LUIGI LOOKUP!");
        }

        /// <summary>
        /// Initializes a new instance of fork associated with a host PC file system file.
        /// </summary>
        /// <param name="filePath">The fully qualified path to the file in the local PC's file system.</param>
        public Fork(string filePath)
            : this()
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of fork associated with a host PC file system file.
        /// </summary>
        /// <param name="rom">The ROM for the fork.</param>
        public Fork(IRom rom)
            : this()
        {
            Rom = rom;
            FilePath = rom.PrepareForDeployment(LuigiGenerationMode.Standard);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the starting virtual address block of the Fork object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public ushort StartingVirtualBlock { get; private set; }

        /// <summary>
        /// Gets or sets the size of the Fork, in bytes.
        /// </summary>
        /// <remarks>In reality, this is a three-byte integer. Size is settable only for XML.</remarks>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the 24-bit CRC value of the fork.
        /// </summary>
        public uint Crc24 { get; set; }

        /// <summary>
        /// Gets or sets the local file system path to send down to the device.
        /// </summary>
        public string FilePath
        {
            get
            {
                if ((FileSystem != null) && !FileSystem.Frozen)
                {
                    UpdateFilePath(_filePath, false);
                }
                return _filePath;
            }

            set
            {
                UpdateFilePath(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a file exists at the path stored in the fork.
        /// </summary>
        public bool FileExists
        {
            get
            {
                var exists = (_filePath != null) && System.IO.File.Exists(_filePath);
                return exists;
            }
        }

        /// <summary>
        /// Gets the ROM the fork represents. If this fork does not represent a ROM, it this value will be <c>null</c>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public IRom Rom { get; internal set; }

        /// <summary>
        /// Gets or sets the global fork number used locate the fork in the Global Fork Table.
        /// </summary>
        public ushort GlobalForkNumber { get; set; }

        /// <summary>
        /// Gets the full file path without performing any validation.
        /// </summary>
        public string FullName
        {
            get { return string.IsNullOrWhiteSpace(_filePath) ? Resources.Strings.Fork_NoFile : _filePath; }
        }

        #region IGlobalFileSystemEntry

        /// <inheritdoc />
        public int EntryUpdateSize
        {
            get { return SerializeByteCount; }
        }

        /// <inheritdoc />
        /// <remarks>A Fork will use the 24-bit CRC of the data it stores as its unique identifier.</remarks>
        public uint Uid
        {
            get { return Crc24; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return string.IsNullOrWhiteSpace(_filePath) ? Resources.Strings.Fork_NoFile : System.IO.Path.GetFileName(_filePath); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public FileSystem FileSystem
        {
            get
            {
                return _fileSystem;
            }

            internal set
            {
                // Ensure CRC, Size, et. al. are up-to-date. This got optimized such that during XML parse, setting FilePath directly
                // would no longer cause additional file system activity.
                var currentFileSystem = _fileSystem;
                _fileSystem = value;
                if ((currentFileSystem == null) && (value != null) && !string.IsNullOrEmpty(_filePath) && ((Crc24 == INTV.Core.Utility.Crc24.InvalidCrc) || (Size == 0)))
                {
                    UpdateFilePath(_filePath, true);
                }
            }
        }

        #endregion // IGlobalFileSystemEntry

        #region ByteSerializer

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return GetFlatSize(SerializationOperation.Serialize); }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return GetFlatSize(SerializationOperation.Deserialize); }
        }

        #endregion // ByteSerializer

        #endregion // Properties

        /// <summary>
        /// Reset internal performance timing data.
        /// </summary>
        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        internal static void ResetAccumulatedTimes()
        {
#if REPORT_PERFORMANCE
            AccumulatedRefreshTime = System.TimeSpan.Zero;
            AccumulatedUpdateFilePathTime = System.TimeSpan.Zero;
            AccumulatedUpdateFilePathRawTime = System.TimeSpan.Zero;
            AccumulatedUpdateFilePathForcedTime = System.TimeSpan.Zero;
            AccumulatedUpdateFilePathForcedPrepareTime = System.TimeSpan.Zero;
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Report performance data to a logger or debug output.
        /// </summary>
        /// <param name="logger">A logger to record into. May be <c>null</c>.</param>
        /// <param name="prefix">Prefix to include in each line of the output.</param>
        /// <remarks>If <param name="logger"/> is <c>null</c>, output is reported to debug output.</remarks>
        [System.Diagnostics.Conditional("REPORT_PERFORMANCE")]
        public static void ReportAccumulatedTimes(INTV.Shared.Utility.Logger logger, string prefix)
        {
            System.Action<string> logIt = (o) => System.Diagnostics.Debug.WriteLine(o.ToString());
            if (logger == null)
            {
                logIt = logger.Log;
            }
            prefix = prefix == null ? string.Empty : prefix + " ";
#if REPORT_PERFORMANCE
            logIt(prefix + "Total RefreshFork ----------------------: " + AccumulatedRefreshTime.ToString());
            logIt(prefix + "Total UpdateFilePath -------------------: " + AccumulatedUpdateFilePathTime.ToString());
            logIt(prefix + "Total UpdateFilePathRaw ----------------: " + AccumulatedUpdateFilePathRawTime.ToString());
            logIt(prefix + "Total UpdateFilePathForced -------------: " + AccumulatedUpdateFilePathForcedTime.ToString());
            logIt(prefix + "Total UpdateFilePathForcedPrepare ------: " + AccumulatedUpdateFilePathForcedPrepareTime.ToString());
#else
            logIt(prefix + "REPORT_PERFORMANCE has not been #defined in:" + typeof(Fork).FullName);
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Refreshes the data describing the instance.
        /// </summary>
        /// <param name="checkCrc">If <c>true</c>, force a re-check of the CRC.</param>
        /// <param name="error">Receives an error that may have occurred during the refresh operation.</param>
        /// <param name="fileFailedToUpdate">Receives a description of the error that occurred during the refresh operation.</param>
        /// <returns><c>true</c> if the size or Crc24/Uid changes or fails to update.</returns>
        internal bool Refresh(bool checkCrc, out System.Exception error, out string fileFailedToUpdate)
        {
#if REPORT_PERFORMANCE
            System.Diagnostics.Debug.WriteLine(">> ENTER Fork.Refresh(" + checkCrc + ", " + _filePath + ")");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            fileFailedToUpdate = null;
            error = null;
            var crc = Crc24;
            var size = Size;
            var filePath = _filePath;
            try
            {
                if (checkCrc)
                {
                    // NOTE: Reading FilePath may have side effects because it will call the PrepareForDeployment
                    // extension method in certain conditions.
                    filePath = FilePath;
                    _filePath = null;
                }
                error = UpdateFilePath(filePath, checkCrc);
            }
            catch (System.IO.IOException e)
            {
                _filePath = filePath;
                error = e;
            }
            catch (LuigiFileGenerationException e)
            {
                _filePath = filePath;
                error = e;
            }
            if (error != null)
            {
                var fileNotFound = error as System.IO.FileNotFoundException;
                if (fileNotFound != null)
                {
                    fileFailedToUpdate = fileNotFound.FileName;
                }
                else if (!string.IsNullOrEmpty(filePath))
                {
                    fileFailedToUpdate = filePath;
                }
                else if (!string.IsNullOrEmpty(_filePath))
                {
                    fileFailedToUpdate = _filePath;
                }
                if (string.IsNullOrEmpty(fileFailedToUpdate))
                {
                    fileFailedToUpdate = Resources.Strings.Fork_NoFile;
                }
            }
            return (error != null) || (filePath != FilePath) || (crc != Crc24) || (size != Size);
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">> EXIT  Fork.Refresh(" + checkCrc + ", " + _filePath + ")"); // took: + " + stopwatch.Elapsed.ToString());
                AccumulatedRefreshTime += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }

        #region IGlobalFileSystemEntry

        /// <inheritdoc />
        public IGlobalFileSystemEntry Clone(FileSystem fileSystem)
        {
            var fork = (Fork)this.MemberwiseClone();
            fork.FileSystem = fileSystem;
            return fork;
        }

        #endregion // IGlobalFileSystemEntry

        #region ByteSerializer

        /// <summary>
        /// Creates a new instance of a Fork by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a Fork.</returns>
        public static Fork Inflate(System.IO.Stream stream)
        {
            return Inflate<Fork>(stream);
        }

        /// <summary>
        /// Creates a new instance of a Fork by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a Fork.</returns>
        public static Fork Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<Fork>(reader);
        }

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            // NOTE: The StartingVirtualBlock, Size, and UID are already included in the command to create
            // a Fork from RAM, so all we need to include is the actual file data.
            ////writer.Write(StartingVirtualBlock);
            ////writer.Write(System.BitConverter.GetBytes(Size), 0, 3);
            ////writer.Write(System.BitConverter.GetBytes(Crc24), 0, 3);
            using (var file = FileUtilities.OpenFileStream(FilePath))
            {
                file.CopyTo(writer.BaseStream);
            }
            return SerializeByteCount;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            StartingVirtualBlock = reader.ReadUInt16();
            byte[] rawuInt = new byte[4];
            reader.ReadBytes(3).CopyTo(rawuInt, 0);
            Size = System.BitConverter.ToUInt32(rawuInt, 0);
            reader.ReadBytes(3).CopyTo(rawuInt, 0);
            Crc24 = System.BitConverter.ToUInt32(rawuInt, 0);

            // NOTE: The actual data in the fork, when downloading a GKT, is not included.
            // The CopyForkToRam command is used to upload the fork's data to RAM.
            // The DownloadDataBlockFromRam command will bring the data to the host PC.
            return DeserializeByteCount; 
        }

        #endregion // ByteSerializer

        #region object Overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            bool areEqual = obj is Fork;
            if (areEqual)
            {
                areEqual = this == (Fork)obj;
            }
            return areEqual;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return StartingVirtualBlock.GetHashCode() ^ Size.GetHashCode() ^ Crc24.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var forkString = "Fork {" + "ID: " + GlobalForkNumber + " UID: " + Crc24.ToString("X4");
            return forkString + " Base:" + StartingVirtualBlock.ToString("X4") + " Size: " + Size + "}";
        }

        #endregion // object Overrides

        #region Operators

        /// <summary>
        /// Determines equality of two Fork objects.
        /// </summary>
        /// <param name="lhs">Left-hand side of equality operator.</param>
        /// <param name="rhs">Right-hand side of equality operator.</param>
        /// <returns><c>true</c> if the two objects are equal.</returns>
        /// <remarks>Two forks are considered equal if their sizes and Crc24 values match. The StartingVirtualBlock is
        /// excluded from this comparison because it is assigned by the Locutus device itself, and may change during
        /// file system maintenance.</remarks>
        public static bool operator ==(Fork lhs, Fork rhs)
        {
            bool areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual && !object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null))
            {
                // Note that StartingVirtualBlock is excluded because, if comparing against changes
                // on host PC, the starting virtual block will be unassigned. Also, the starting
                // virtual block may change.
                // GKN is ignored in order to allow for dedup - the GKN is not assigned until a slot
                // is found for the fork. If a temporary fork is used for a search, we need the match
                // to succeed based on fork data other than the GKN.
                areEqual = (lhs.Crc24 == rhs.Crc24) && (rhs.Size == lhs.Size) /* && (lhs.StartingVirtualBlock == rhs.StartingVirtualBlock) */;
            }
            return areEqual;
        }

        /// <summary>
        /// Determines inequality of two Fork objects.
        /// </summary>
        /// <param name="lhs">Left-hand side of inequality operator.</param>
        /// <param name="rhs">Right-hand side of inequality operator.</param>
        /// <returns><c>true</c> if the two objects are not equal.</returns>
        public static bool operator !=(Fork lhs, Fork rhs)
        {
            return !(rhs == lhs);
        }

        #endregion // Operators

        private int GetFlatSize(SerializationOperation operation)
        {
            int flatSize = 0;
            switch (operation)
            {
                case SerializationOperation.Serialize:
                    if (!string.IsNullOrWhiteSpace(FilePath) && System.IO.File.Exists(FilePath))
                    {
                        var fileInfo = new System.IO.FileInfo(FilePath);
                        flatSize = (int)fileInfo.Length;
                    }
                    break;
                case SerializationOperation.Deserialize:
                    flatSize = FlatSizeInBytes;
                    break;
            }
            return flatSize;
        }

        private System.Exception UpdateFilePath(string filePath, bool forceUpdate)
        {
#if REPORT_PERFORMANCE
            System.Diagnostics.Debug.WriteLine(">>>> ENTER Fork.UpdateFilePath(" + filePath + "," + forceUpdate + ") + T:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            System.Exception error = null;
            if ((FileSystem != null) && FileSystem.Frozen)
            {
                // During save, do not attempt to regenerate contents.
                return error;
            }
            try
            {
                // Do some special work if this is a ROM fork.
                if ((FileSystem != null) && (FileSystem.GetForkKind(this) == ForkKind.Program))
                {
                    if (forceUpdate)
                    {
#if REPORT_PERFORMANCE
                        System.Diagnostics.Debug.WriteLine(">>>>>> ENTER Fork.UpdateFilePath(forceUpdate): GetRom");
                        var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();
                        try
                        {
#endif // REPORT_PERFORMANCE
                        var filesUsingFork = FileSystem.GetFilesUsingForks(new[] { this }).Values;
                        foreach (var program in filesUsingFork.OfType<Program>())
                        {
                            // Since "Program" entries using the same Fork will all point to this one,
                            // we don't need to re-do this work for every file that might point at the fork.
                            var rom = program.Description.GetRom();
                            if ((rom != null) && !string.IsNullOrEmpty(rom.RomPath))
                            {
                                var path = string.Empty;
#if REPORT_PERFORMANCE
                                var stopwatch3 = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
                                path = rom.PrepareForDeployment(LuigiGenerationMode.Standard);
#if REPORT_PERFORMANCE
                                stopwatch3.Stop();
                                AccumulatedUpdateFilePathForcedPrepareTime += stopwatch3.Elapsed;
#endif // REPORT_PERFORMANCE
                                UpdateFilePath(path);
                            }
                        }
#if REPORT_PERFORMANCE
                        }
                        finally
                        {
                            stopwatch2.Stop();
                            System.Diagnostics.Debug.WriteLine(">>>>>> EXIT  Fork.UpdateFilePath(forceUpdate): GetRom"); // took: + " + stopwatch2.Elapsed.ToString());
                            AccumulatedUpdateFilePathForcedTime += stopwatch2.Elapsed;
                        }
#endif // REPORT_PERFORMANCE
                    }
                    else
                    {
                        var rom = Rom;
                        var prepareForDeployment = string.IsNullOrEmpty(filePath) && (rom != null);
                        if (!prepareForDeployment && (rom != null))
                        {
                            // We think we don't have to prepare for deployment, but do have ROM set.
                            // Let's just make sure that we have that LUIGI file, shall we?
                            prepareForDeployment = string.IsNullOrEmpty(_filePath) || !System.IO.File.Exists(_filePath);
                            if (!prepareForDeployment && System.IO.File.Exists(_filePath))
                            {
                                // Check to see if it looks like a valid LUIGI file.
                                prepareForDeployment = LuigiFileHeader.GetHeader(_filePath) == null;
                            }
                        }
                        if (prepareForDeployment)
                        {
                            // No file path, but somehow we have a ROM. Force recreation of the LUIGI file. This can
                            // occur during sync from device.
                            UpdateFilePath(Rom.PrepareForDeployment(LuigiGenerationMode.Standard));
                        }
                        else if (rom == null)
                        {
                            if (!System.IO.File.Exists(filePath))
                            {
                                // No ROM, and the file we think we have does not exist.
                                ILfsFileInfo file;
                                rom = null;
                                if (FileSystem.GetFilesUsingForks(new[] { this }).TryGetValue(this, out file))
                                {
                                    switch (FileSystem.Origin)
                                    {
                                        case FileSystemOrigin.HostComputer:
                                            var description = ((Program)file).Description;
                                            rom = description == null ? null : description.GetRom();
                                            break;
                                        case FileSystemOrigin.LtoFlash:
                                            break;
                                    }
                                }
                                prepareForDeployment = (rom != null) && !System.IO.File.Exists(filePath) && !string.IsNullOrEmpty(rom.RomPath) && System.IO.File.Exists(rom.RomPath);
                                if (prepareForDeployment)
                                {
                                    _filePath = null;
                                    UpdateFilePath(rom.PrepareForDeployment(LuigiGenerationMode.Standard));
                                }
                            }

                            // We should *always* be a LUIGI file. If not, fix it!
                            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                            {
                                if (!LuigiFileHeader.PotentialLuigiFile(filePath))
                                {
                                    rom = INTV.Core.Model.Rom.Create(filePath, null);
                                    prepareForDeployment = rom != null;
                                    if (prepareForDeployment)
                                    {
                                        _filePath = null;
                                        UpdateFilePath(rom.PrepareForDeployment(LuigiGenerationMode.Standard));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (forceUpdate)
                    {
                        _filePath = null;
                    }
                    UpdateFilePath(filePath);
                    if (forceUpdate && !string.IsNullOrEmpty(_filePath) && !System.IO.File.Exists(_filePath))
                    {
                        error = new System.IO.FileNotFoundException("File for fork " + GlobalForkNumber + " not found.", filePath, null);
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                error = e;
                if (!forceUpdate)
                {
                    throw;
                }
            }
            return error;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>>> EXIT  Fork.UpdateFilePath(" + filePath + ",force)"); // took: + " + stopwatch.Elapsed.ToString());
                AccumulatedUpdateFilePathTime += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }

        private void UpdateFilePath(string filePath)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            if (_filePath != filePath)
            {
                _filePath = filePath;
                if ((FileSystem == null) && (Rom == null) && !Properties.Settings.Default.ValidateMenuAtStartup)
                {
                    // Not in a file system yet - assume XML load, or that CRC and Size will be updated later as needed.
                    return;
                }
                var originalUid = Uid;
                Crc24 = INTV.Core.Utility.Crc24.InvalidCrc;
                Size = 0;
                if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
                {
                    var fileInfo = new System.IO.FileInfo(filePath);
                    Crc24 = (originalUid == MenuPositionForkUid) ? MenuPositionForkUid : INTV.Core.Utility.Crc24.OfFile(filePath);
                    Size = (uint)fileInfo.Length;
                }
                if (originalUid == MenuPositionForkUid)
                {
                    // Preserve this special UID regardless of actual file contents.
                    Crc24 = MenuPositionForkUid;
                }
            }
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>Fork..Crc24.OfFile() took: + " + stopwatch.Elapsed.ToString());
                AccumulatedUpdateFilePathRawTime += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }

        private enum SerializationOperation
        {
            /// <summary>
            /// Identifies a serialize operation.
            /// </summary>
            Serialize,

            /// <summary>
            /// Identifies a deserialize operation.
            /// </summary>
            Deserialize
        }
    }
}

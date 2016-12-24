// <copyright file="FileSystem.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

////#define FORCE_JLPFLASH_FORKS
////#define MEASURE_CLONE_PERFORMANCE

using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Models the Locutus File System (v2).
    /// </summary>
    public class FileSystem : INTV.Core.Utility.ByteSerializer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Locutus File System v2.
        /// </summary>
        public FileSystem()
            : this(FileSystemOrigin.LtoFlash)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Locutus File System v2.
        /// </summary>
        /// <param name="origin">Origin of the file system.</param>
        public FileSystem(FileSystemOrigin origin)
        {
            Origin = origin;
            Directories = new GlobalDirectoryTable(this);
            Files = new GlobalFileTable(this);
            Forks = new GlobalForkTable(this);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the root of the file system.
        /// </summary>
        public MenuLayout Root
        {
            get { return Directories[GlobalDirectoryTable.RootDirectoryNumber] as MenuLayout; }
        }

        /// <summary>
        /// Gets a value indicating the origin of the file system.
        /// </summary>
        public FileSystemOrigin Origin { get; private set; }

        /// <summary>
        /// Gets or sets the status of the file system.
        /// </summary>
        public LfsDirtyFlags Status { get; set; }

        /// <summary>
        /// Gets the global directory table used by the file system.
        /// </summary>
        public GlobalDirectoryTable Directories { get; private set; }

        /// <summary>
        /// Gets the global file table used by the file system.
        /// </summary>
        public GlobalFileTable Files { get; private set; }

        /// <summary>
        /// Gets the global fork table used by the file system.
        /// </summary>
        public GlobalForkTable Forks { get; private set; }

        #region ByteSerializer Properties

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return -1; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return -1; }
        }

        #endregion // ByteSerializer Properties

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a FileSystem by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a FileSystem.</returns>
        public static FileSystem Inflate(System.IO.Stream stream)
        {
            FileSystem inflatedObject = null;
            using (var reader = new INTV.Shared.Utility.ASCIIBinaryReader(stream))
            {
                inflatedObject = Inflate(reader);
            }
            return inflatedObject;
        }

        /// <summary>
        /// Creates a new instance of a FileSystem by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a FileSystem.</returns>
        public static FileSystem Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<FileSystem>(reader);
        }

        /// <summary>
        /// Creates a copy (clone) of a FileSystem. The different element types of the tables specify their own rules about cloning.
        /// </summary>
        /// <returns>A copy of the file system.</returns>
        public FileSystem Clone()
        {
            System.Diagnostics.Debug.Assert(Origin == FileSystemOrigin.HostComputer, "FileSystem cloning is currently only implemented for Host Computer LFS, not for on-device LFS.");

            var fileSystem = new FileSystem(Origin);
#if MEASURE_CLONE_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // MEASURE_CLONE_PERFORMANCE
            lock (this)
            {
                // First, clone the forks.
                foreach (var fork in Forks)
                {
                    if (fork != null)
                    {
                        var clone = (Fork)fork.Clone(fileSystem);
                        clone.FileSystem = fileSystem;
                        fileSystem.Forks[clone.GlobalForkNumber] = clone;
                    }
                }

                // Then, clone the files.
                foreach (var file in Files)
                {
                    if ((file != null) && (file.FileType == FileType.File))
                    {
                        var clone = (FileNode)file.Clone(fileSystem);
                        fileSystem.Files[clone.GlobalFileNumber] = clone;

                        // Can't set parent yet, since directories haven't been created.
                    }
                }

                // Next, clone the directories.
                foreach (var directory in Directories)
                {
                    if (directory != null)
                    {
                        var clone = (Folder)directory.Clone(fileSystem);
                        fileSystem.Directories[clone.GlobalDirectoryNumber] = clone;
                        fileSystem.Files[clone.GlobalFileNumber] = clone;

                        // Can't set contents yet because some may be directories not yet created.
                    }
                }

                // Finally, we need to populate the items in the directories and assign parents.
                foreach (var directory in Directories)
                {
                    if (directory != null)
                    {
                        var items = new System.Collections.Generic.List<FileNode>();
                        foreach (var item in ((Folder)directory).Files)
                        {
                            FileNode itemToAdd = null;
                            switch (item.FileType)
                            {
                                case FileType.File:
                                    itemToAdd = (FileNode)fileSystem.Files[item.GlobalFileNumber];
                                    break;
                                case FileType.Folder:
                                    itemToAdd = (FileNode)fileSystem.Directories[item.GlobalDirectoryNumber];
                                    break;
                                default:
                                    throw new System.InvalidOperationException(Resources.Strings.FileSystem_InvalidFileType);
                            }
                            items.Add(itemToAdd);
                            itemToAdd.Parent = (Folder)fileSystem.Directories[directory.GlobalDirectoryNumber];
                        }
                        ((Folder)fileSystem.Directories[directory.GlobalDirectoryNumber]).Files = new System.Collections.ObjectModel.ObservableCollection<FileNode>(items);
                    }
                }
            }
#if MEASURE_CLONE_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                ////System.Diagnostics.Debug.WriteLine("## Clone took " + stopwatch.Elapsed.ToString());
                ////System.Console.WriteLine("## Clone took " + stopwatch.Elapsed.ToString());
            }
#endif // MEASURE_CLONE_PERFORMANCE

            return fileSystem;
        }

        /// <summary>
        /// Determines if the file system is in a consistent state.
        /// </summary>
        /// <returns><c>true</c> if the file system is consistent, with no orphaned objects.</returns>
        public bool Validate()
        {
            var orphanedEntries = GetOrphanedFileSystemEntries();
            var isValid = !orphanedEntries.OrphanedForks.Any() && !orphanedEntries.OrphanedFiles.Any() && !orphanedEntries.OrphanedDirectories.Any();
            return isValid;
        }

        public OrphanedFileSystemEntries GetOrphanedFileSystemEntries()
        {
            var orphanedForks = Forks.Where(k => (k != null) && (k.GlobalForkNumber != GlobalForkTable.InvalidForkNumber) && !Files.Any(f => (f != null) && f.ForkNumbers.Contains(k.GlobalForkNumber)));
            var orphanedFiles = Files.Where(f => (f != null) && (f.GlobalFileNumber != GlobalFileTable.InvalidFileNumber) && !Directories.Any(d => (d != null) && (d.PresentationOrder.Contains(f.GlobalFileNumber) || (d.ParentDirectoryGlobalFileNumber == f.GlobalFileNumber))));
            var orphanedDirectories = Directories.Where(d => (d != null) && (d.GlobalDirectoryNumber != GlobalDirectoryTable.RootDirectoryNumber) && (d.ParentDirectoryGlobalFileNumber != GlobalDirectoryTable.RootDirectoryNumber) && (!Files.Any(f => (f != null) && (f.GlobalFileNumber == d.ParentDirectoryGlobalFileNumber)) || !Directories.Any(dir => (dir != null) && dir.PresentationOrder.Contains(d.ParentDirectoryGlobalFileNumber))));
            var orphanedEntries = new OrphanedFileSystemEntries(orphanedForks, orphanedFiles, orphanedDirectories);
            return orphanedEntries;
        }

        #region ByteSerializer Implementation

        /// <inheritdoc/>
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            var asciiReader = (INTV.Shared.Utility.ASCIIBinaryReader)reader;
            var bytesRead = Directories.Deserialize(asciiReader);
            bytesRead += Files.Deserialize(asciiReader);
            bytesRead += Forks.Deserialize(asciiReader);

#if FORCE_JLPFLASH_FORKS
            foreach (var file in Files.Where(f => (f != null)))
            {
                var fakeDataFork = new Fork();
                fakeDataFork.Size = 0x0600;
                fakeDataFork.Crc24 = INTV.Core.Utility.RandomUtilities.Next24();
                Forks.Add(fakeDataFork);
                file.JlpFlash = fakeDataFork;
            }
#endif // FORCE_JLPFLASH_FORKS

            return bytesRead;
        }

        #endregion // ByteSerializer Implementation
    }
}

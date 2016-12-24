// <copyright file="FileNode.cs" company="INTV Funhouse">
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

////#define REPORT_PERFORMANCE

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Stic;
using INTV.Core.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Provides a partial implementation of the IFile interface as well as the LFS file type.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("MenuItem")]
    public abstract class FileNode : ModelBase, IFile, ILfsFileInfo
    {
        #region Property Names

        public const string LongNamePropertyName = "LongName";
        public const string ShortNamePropertyName = "ShortName";
        public const string ColorPropertyName = "Color";

        #endregion // Property Names

        #region XML Element Node Names

        protected const string RomForkXmlNodeName = "RomFork";
        protected const string ManualForkXmlNodeName = "ManualFork";
        protected const string SaveDataForkXmlNodeName = "SaveDataFork";
        protected const string VignetteForkXmlNodeName = "VignetteFork";
        protected const string ReservedFork4XmlNodeName = "ReservedFork4";
        protected const string ReservedFork5XmlNodeName = "ReservedFork5";
        protected const string ReservedFork6XmlNodeName = "ReservedFork6";

        #endregion // XML Element Node Names

        #region Constructors

        private FileNode()
        {
            _color = Color.White;
            _forks = new Fork[(int)ForkKind.NumberOfForkKinds];
        }

        /// <summary>
        /// Initializes a new instance of a FileNode.
        /// </summary>
        /// <param name="fileSystem">The file system to which the node belongs.</param>
        protected FileNode(FileSystem fileSystem)
            : this()
        {
            _fileSystem = fileSystem;
            _globalFileNumber = GlobalFileTable.InvalidFileNumber;
        }

        /// <summary>
        /// Initializes a new instance of a FileNode.
        /// </summary>
        /// <param name="fileInfo">A LfsFileInfo retrieved from an LTO Flash! device.</param>
        /// <remarks>This constructor is used to create a user interface to display the file system
        /// retrieved from a Locutus device.</remarks>
        protected FileNode(ILfsFileInfo fileInfo)
            : this()
        {
            _longName = fileInfo.LongName;
            if (string.Compare(fileInfo.LongName, fileInfo.ShortName, StringComparison.Ordinal) != 0)
            {
                _shortName = fileInfo.ShortName;
            }
            _color = fileInfo.Color;
            _globalFileNumber = fileInfo.GlobalFileNumber;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the collection of files are stored in the object.
        /// </summary>
        /// <remarks>Determine if this can be made abstract. Rely upon overrides in Program and Folder instead.</remarks>
        [System.Xml.Serialization.XmlArray("Items")]
        [System.Xml.Serialization.XmlArrayItem("MenuItem")]
        public virtual ObservableCollection<FileNode> Files
        {
            get { return null; }
            set { }
        }

        #region IFile Properties

        /// <inheritdoc />
        /// <remarks>Also implements property of the same name for ILfsFileInfo.</remarks>
        public abstract FileType FileType { get; }

        /// <inheritdoc />
        /// <remarks>Also implements property of the same name for ILfsFileInfo.</remarks>
        public Color Color
        {
            get { return _color; }
            set { AssignAndUpdateProperty(ColorPropertyName, value, ref _color); }
        }
        private Color _color;

        /// <inheritdoc />
        /// <remarks>Also implements property of the same name for ILfsFileInfo.</remarks>
        public virtual string ShortName
        {
            get
            {
                return _shortName;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    AssignAndUpdateProperty(ShortNamePropertyName, value.EnforceNameLength(FileSystemConstants.MaxShortNameLength, true), ref _shortName);
                }
            }
        }
        private string _shortName;

        /// <inheritdoc />
        /// <remarks>Also implements property of the same name for ILfsFileInfo.</remarks>
        public virtual string LongName
        {
            get
            {
                return _longName;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    AssignAndUpdateProperty(LongNamePropertyName, value.EnforceNameLength(FileSystemConstants.MaxLongNameLength, true), ref _longName);
                }
            }
        }
        private string _longName;

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual IFileContainer Parent
        {
            get { return _parent; }
            set { AssignAndUpdateProperty("Parent", value, ref _parent); }
        }
        private IFileContainer _parent;

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public abstract uint Crc32 { get; set; }

        #endregion // IFile Properties

        #region ILfsFileInfo Properties

        /// <inheritdoc />
        public virtual ushort GlobalFileNumber
        {
            get { return _globalFileNumber; }
            set { SetGlobalFileNumber(value); }
        }
        private ushort _globalFileNumber;

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public abstract byte GlobalDirectoryNumber { get; set; }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public abstract byte Reserved { get; }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual ushort[] ForkNumbers
        {
            get { return _forks.Select(f => (f == null) ? GlobalForkTable.InvalidForkNumber : f.GlobalForkNumber).ToArray(); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork Rom
        {
            get { return Forks[(int)ForkKind.Program]; }
            set { SetFork(ForkKind.Program, value); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork Manual
        {
            get { return Forks[(int)ForkKind.Manual]; }
            set { SetFork(ForkKind.Manual, value); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork JlpFlash
        {
            get { return Forks[(int)ForkKind.JlpFlash]; }
            set { SetFork(ForkKind.JlpFlash, value); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork Vignette
        {
            get { return Forks[(int)ForkKind.Vignette]; }
            set { SetFork(ForkKind.Vignette, value); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork ReservedFork4
        {
            get { return Forks[(int)ForkKind.Reserved4]; }
            set { SetFork(ForkKind.Reserved4, value); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork ReservedFork5
        {
            get { return Forks[(int)ForkKind.Reserved5]; }
            set { SetFork(ForkKind.Reserved5, value); }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public virtual Fork ReservedFork6
        {
            get { return Forks[(int)ForkKind.Reserved6]; }
            set { SetFork(ForkKind.Reserved6, value); }
        }

        #region IGlobalFileSystemEntry Properties

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public abstract int EntryUpdateSize { get; }

        /// <inheritdoc />
        public abstract uint Uid { get; }

        /// <inheritdoc />
        public string Name
        {
            get { return LongName; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public FileSystem FileSystem
        {
            get { return _fileSystem; }
            internal set { _fileSystem = value; }
        }
        private FileSystem _fileSystem;

        #endregion // IGlobalFileSystemEntry Properties

        #endregion // ILfsFileInfo Properties

        /// <summary>
        /// Gets or sets the fork numbers referred to by the file.
        /// </summary>
        protected Fork[] Forks
        {
            get { return _forks; }
            set { _forks = value; }
        }
        private Fork[] _forks;

        #endregion // Properties

        /// <summary>
        /// Creates a specific instance of a FileNode as appropriate for the given LfsFileInfo.
        /// </summary>
        /// <param name="fileInfo">A LfsFileInfo from which to create a FileNode.</param>
        /// <returns>A FileNode based upon the contents of the given directory entry.</returns>
        public static FileNode Create(LfsFileInfo fileInfo)
        {
            FileNode fileNode = null;
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
                if (fileInfo != null)
                {
                    switch (fileInfo.FileType)
                    {
                        case FileType.File:
                            fileNode = new Program(fileInfo);
                            break;
                        case FileType.Folder:
                            fileNode = new Folder(fileInfo);
                            break;
                        default:
                            throw new INTV.Core.UnexpectedFileTypeException(fileInfo.FileType.ToString());
                    }
                }
                else
                {
                    throw new InconsistentFileSystemException(LfsEntityType.Unknown, 0xFFFFFFFF, string.Empty);
                }
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileNode creation took: + " + stopwatch.Elapsed.ToString());
            }
#endif // REPORT_PERFORMANCE
            return fileNode;
        }

        #region IGlobalFileSystemEntry Methods

        /// <inheritdoc />
        public abstract IGlobalFileSystemEntry Clone(FileSystem fileSystem);

        #endregion // IGlobalFileSystemEntry Methods

        /// <summary>
        /// The file or folder should update its CRC data.
        /// </summary>
        public abstract void UpdateCrc();

        /// <summary>
        /// Creates a path-like representation describing where a file in the file system exists in the hierarchy.
        /// </summary>
        /// <returns>A string describing the location of the file node in the hierarchy.</returns>
        /// <remarks>The returned path is similar to a UNIX-style path, using a forward slash to indicate directories in a LFS. So, if the root folder
        /// is named 'LTO Flash!' and a file named 'Pumpkin Spice Patrol' is in a subdirectory 'LTO', the returned path is:
        /// /LTO Flash!/LTO/Pumpkin Spice Patrol
        /// Of course, this can become confusing when people name ROMs with slash (/) characters. These paths are strictly for presentation
        /// purposes, and should never be parsed in an attempt to locate a FileNode.</remarks>
        public string GetMenuPath()
        {
            return GetMenuPath(null);
        }

        /// <summary>
        /// Creates a path-like representation describing where a file in the file system exists in the hierarchy prefixed with a device ID.
        /// </summary>
        /// <param name="prefix">If not <c>null</c> or empty, this additional prefix is placed, followed by a colon (:) at the beginning of the returned path.</param>
        /// <returns>A string describing the location of the file node in the hierarchy with the optional prefix.</returns>
        /// <remarks>The returned path is similar to a UNIX-style path, using a forward slash to indicate directories in a LFS. The optional
        /// prefix can be used to provide additional context, such as a Locutus' DRUID. So, if the prefix is 'ABC123', the root folder
        /// named 'LTO Flash!' and a file named 'Pumpkin Spice Patrol' is in a subdirectory 'LTO', the returned path is:
        /// ABC123:/LTO Flash!/LTO/Pumpkin Spice Patrol
        /// Of course, this can become confusing when people name ROMs with slash (/) or colon (:) characters. These paths are strictly
        /// for presentation purposes, and should never be parsed in an attempt to locate a FileNode.</remarks>
        public string GetMenuPath(string prefix)
        {
            var pathElements = new Stack<string>();
            var node = this;
            do
            {
                var nodeName = node is MenuLayout ? node.ShortName : node.LongName;
                if ((nodeName == null) && (node is MenuLayout))
                {
                    nodeName = MenuLayout.RootName;
                }
                if (nodeName == null)
                {
                    nodeName = node.ShortName;
                }
                if (nodeName == null)
                {
                    nodeName = "<null>";
                }
                pathElements.Push(nodeName);
                node = node.Parent as FileNode;
            }
            while (node != null);
            prefix = string.IsNullOrEmpty(prefix) ? string.Empty : prefix + ":";
            var path = prefix + "/" + string.Join("/", pathElements.ToArray());
            return path;
        }

        /// <summary>
        /// Called after a MenuLayout has been loaded from XML to perform post-load fix-up for
        /// the file system and other values.
        /// </summary>
        /// <param name="fileSystem">The file system to which the file belongs.</param>
        /// <param name="updateRomList">If <c>true</c>, update ROM list.</param>
        internal virtual void LoadComplete(FileSystem fileSystem, bool updateRomList)
        {
            _fileSystem = fileSystem;
            SetGlobalFileNumber(_globalFileNumber);
            foreach (var fork in _forks)
            {
                if (fork != null)
                {
                    FileSystem.Forks.Add(fork);
                }
            }
        }

        /// <summary>
        /// Assigns forks using the file system and supplied fork numbers.
        /// </summary>
        /// <param name="forkNumbers">The fork numbers identifying which forks to use.</param>
        internal void SetForks(ushort[] forkNumbers)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
                for (int i = 0; i < (int)ForkKind.NumberOfForkKinds; ++i)
                {
                    var forkNumber = forkNumbers[i];
                    if (forkNumber != GlobalForkTable.InvalidForkNumber)
                    {
                        var fork = FileSystem.Forks[forkNumber];
                        SetFork((ForkKind)i, fork);
                    }
                }
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(" ## SetForks on '" + LongName + "' took " + stopwatch.Elapsed.ToString());
            }
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Assigns a fork.
        /// </summary>
        /// <param name="kind">The kind of fork to assign.</param>
        /// <param name="fork">The actual fork to assign.</param>
        protected void SetFork(ForkKind kind, Fork fork)
        {
            switch (kind)
            {
                case ForkKind.NumberOfForkKinds:
                case ForkKind.None:
                    break;
                default:
                    var previousFork = _forks[(int)kind];
                    _forks[(int)kind] = fork;
                    if (previousFork != null)
                    {
                        FileSystem.Forks.Remove(previousFork);
                    }
                    break;
            }
        }

        private void SetGlobalFileNumber(ushort globalFileNumber)
        {
            // FileSystem may not be set during creation from XML. Therefore, defer registering in the file table until the parent chain is established.
            var fileSystem = FileSystem;
            var fileTable = fileSystem == null ? null : fileSystem.Files;
            if ((fileTable != null) && (globalFileNumber != GlobalFileTable.InvalidFileNumber))
            {
                if ((_globalFileNumber < GlobalFileTable.TableSize) && (_globalFileNumber != globalFileNumber))
                {
                    System.Diagnostics.Debug.Assert((fileTable[_globalFileNumber] == null) || (fileTable[_globalFileNumber] == this), "Unexpected entry in GFT.");
                    fileTable.RemoveAt(_globalFileNumber);
                }
                if ((fileTable[globalFileNumber] != null) && fileTable[globalFileNumber] != this)
                {
                    // This can happen when the persisted menu layout somehow contains duplicate entries for file numbers.
                    _globalFileNumber = GlobalFileTable.InvalidFileNumber;
                    fileTable.Add(this);
                    globalFileNumber = _globalFileNumber;
                }
                else
                {
                    fileTable.Insert(globalFileNumber, this);
                }
            }
            _globalFileNumber = globalFileNumber;
        }

        private FileSystem GetFileSystem()
        {
            var fileSystem = _fileSystem;
            var parent = (FileNode)Parent;
            while ((fileSystem == null) && (parent != null))
            {
                fileSystem = parent.FileSystem;
                parent = (FileNode)parent.Parent;
            }
            return fileSystem;
        }
    }
}

// <copyright file="Folder.cs" company="INTV Funhouse">
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
////#define PERSIST_RESERVED_FORKS

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using INTV.Core.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implements IFileContainer for submenus in a menu layout to be deployed to a Locutus device.
    /// </summary>
    public class Folder : FileNode, IFileContainer, IDirectory
    {
        private const string RomForkXmlNodeOverrideName = "ExecutableFork";
        private const string ManualForkXmlNodeOverrideName = "DataFork";
        private const string SaveDataForkXmlNodeOverrideName = "DirectorySaveDataFork";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a Folder.
        /// </summary>
        public Folder()
            : base(default(FileSystem))
        {
            _globalDirectoryNumber = GlobalDirectoryTable.InvalidDirectoryNumber;
            Crc32 = INTV.Core.Utility.RandomUtilities.Next32();
        }

        /// <summary>
        /// Initializes a new instance of a Folder.
        /// </summary>
        /// <param name="fileSystem">The file system in which the folder exists.</param>
        /// <param name="globalDirectoryNumber">The directory number for the folder.</param>
        /// <param name="deviceId">If not <c>null</c> or empty, indicates the folder is intended for a specific device. Used in error reporting.</param>
        /// <remarks>If the directory number is invalid, one will be assigned.</remarks>
        internal Folder(FileSystem fileSystem, byte globalDirectoryNumber, string deviceId)
            : base(fileSystem)
        {
            _globalDirectoryNumber = globalDirectoryNumber;

            // When we create directly from Locutus, the directory table is already populated, so there's no need to fill out the parent file.
            if (fileSystem.Origin == FileSystemOrigin.LtoFlash)
            {
                // We need to "inflate" manually in this case via the FileSystem. That is, in the case
                // of a FileSystem retrieved from Locutus, we have the flat worldview, which we must
                // now tree-ify. In the case with a HostComputer origin, we're either manually populating
                // via the UI or inflating from XML, both of which will insert new elements as they are
                // manipulated by the user or created via the XmlParser.
                var directory = fileSystem.Directories[globalDirectoryNumber] as Directory;
                SetContents(directory, deviceId);
            }
            else
            {
                // We're either manually creating a folder via the UI, or inflating from XML. In either case, the
                // contents will be added later.
                fileSystem.Directories.Add(this);
                fileSystem.Files.Add(this);
                SetContents(new ObservableCollection<FileNode>());
            }
            Crc32 = INTV.Core.Utility.RandomUtilities.Next32();
        }

        /// <summary>
        /// Initializes a new instance of a Folder.
        /// </summary>
        /// <param name="directoryEntry">A directory entry from a Locutus device.</param>
        /// <remarks>This constructor is typically used to create a model for a user interface to operate on for
        /// the purpose of viewing the contents of a Locutus device.</remarks>
        public Folder(LfsFileInfo directoryEntry)
            : base(directoryEntry)
        {
            _globalDirectoryNumber = directoryEntry.GlobalDirectoryNumber;
            SetContents(new ObservableCollection<FileNode>());
        }

        #endregion // Constructors

        #region Events

        /// <inheritdoc />
        public event EventHandler<EventArgs> ContentsChanged;

        #endregion // Events

        #region Properties

        /// <inheritdoc />
        [System.Xml.Serialization.XmlArray("Items")]
        [System.Xml.Serialization.XmlArrayItem("MenuItem")]
        public override ObservableCollection<FileNode> Files
        {
            get { return _items; }
            set { SetContents(value); }
        }
        private ObservableCollection<FileNode> _items;

        #region IFile Properties

        /// <inheritdoc />
        public override FileType FileType
        {
            get { return FileType.Folder; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public sealed override uint Crc32 { get; set; }

        #endregion // IFile Properties

        #region IFileContainer Properties

        /// <inheritdoc />
        public IEnumerable<IFile> Items
        {
            get { return Files; }
        }

        /// <inheritdoc />
        public int Size
        {
            get { return Files.Count; }
        }

        #endregion // IFileContainer Properties

        #region ILfsFileInfo Properties

        /// <inheritdoc />
        /// <remarks>Also part of IDirectory.</remarks>
        public override byte GlobalDirectoryNumber
        {
            get { return _globalDirectoryNumber; }
            set { SetGlobalDirectoryNumber(value); }
        }
        private byte _globalDirectoryNumber;

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public override byte Reserved
        {
            get { return 0xFF; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(RomForkXmlNodeOverrideName)]
        public override Fork Rom
        {
            get { return base.Rom; }
            set { base.Rom = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ManualForkXmlNodeOverrideName)]
        public override Fork Manual
        {
            get { return base.Manual; }
            set { base.Manual = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(SaveDataForkXmlNodeOverrideName)]
        public override Fork JlpFlash
        {
            get { return base.JlpFlash; }
            set { base.JlpFlash = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(VignetteForkXmlNodeName)]
        public override Fork Vignette
        {
            get { return base.Vignette; }
            set { base.Vignette = value; }
        }

#if PERSIST_RESERVED_FORKS

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ReservedFork4XmlNodeName)]
        public override Fork ReservedFork4
        {
            get { return base.ReservedFork4; }
            set { base.ReservedFork4 = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ReservedFork5XmlNodeName)]
        public override Fork ReservedFork5
        {
            get { return base.ReservedFork5; }
            set { base.ReservedFork5 = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ReservedFork6XmlNodeName)]
        public override Fork ReservedFork6
        {
            get { return base.ReservedFork6; }
            set { base.ReservedFork6 = value; }
        }

#endif // PERSIST_RESERVED_FORKS

        #endregion ILfsFileInfo Properties

        #region IDirectory Properties

        /// <inheritdoc />
        public ushort ParentDirectoryGlobalFileNumber
        {
            get { return (Parent == null) ? GlobalFileTable.RootDirectoryFileNumber : ((Folder)Parent).GlobalFileNumber; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public PresentationOrder PresentationOrder
        {
            get { return GetPresentationOrder(); }
        }

        #endregion // IDirectory Properties

        #region IGlobalFileSystemEntry Properties

        /// <inheritdoc />
        /// <remarks>When serialized, looks the same as a Directory that has been serialized.</remarks>
        public override int EntryUpdateSize
        {
            get { return Directory.FlatSizeInBytes; }
        }

        /// <inheritdoc />
        public override uint Uid
        {
            get { return Crc32; }
        }

        #endregion // IGlobalFileSystemEntry Properties

        #endregion // Properties

        #region IGlobalFileSystemEntry Methods

        /// <inheritdoc />
        public override IGlobalFileSystemEntry Clone(FileSystem fileSystem)
        {
            var folder = (Folder)this.MemberwiseClone();
            folder.RemoveAllEventHandlers();
            folder.FileSystem = fileSystem;
            folder.Forks = new Fork[(int)ForkKind.NumberOfForkKinds];
            folder.SetForks(this.ForkNumbers);
            folder._items = null;
            folder.Parent = null;
            return folder;
        }

        #endregion // IGlobalFileSystemEntry Methods

        #region IFileContainer Methods

        /// <inheritdoc />
        public bool AddChild(IFile child, bool updateFileSystemTables)
        {
            bool added = Files.IndexOf((FileNode)child) < 0;
            if (added)
            {
                if (child.Parent != null)
                {
                    child.Parent.RemoveChild(child, updateFileSystemTables);
                }
                Files.Add((FileNode)child);
                UpdateCrc();
            }
            return added;
        }

        /// <inheritdoc />
        public void InsertChild(int index, IFile child, bool updateFileSystemTables)
        {
            if (child.Parent == this)
            {
                var currentIndex = IndexOfChild(child);
                if (index != currentIndex)
                {
                    Files.Move(currentIndex, index);
                    UpdateCrc();
                }
            }
            else
            {
                if (child.Parent != null)
                {
                    child.Parent.RemoveChild(child, updateFileSystemTables);
                }
                Files.Insert(index, (FileNode)child);
                UpdateCrc();
            }
        }

        /// <inheritdoc />
        public bool MoveChildToNewParent(IFile child, IFileContainer newParent, bool updateFileSystemTables)
        {
            return MoveChildToNewParent(child, newParent, -1, updateFileSystemTables);
        }

        /// <inheritdoc />
        public bool MoveChildToNewParent(IFile child, IFileContainer newParent, int locationInNewParent, bool updateFileSystemTables)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            bool moved = false;
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            if (newParent == null)
            {
                throw new ArgumentNullException("newParent");
            }
            if (IsParentOfChild(child))
            {
                var file = (FileNode)child;
                if (newParent == this)
                {
                    if (locationInNewParent < 0)
                    {
                        locationInNewParent = Files.Count - 1;
                    }
                    var currentIndex = Files.IndexOf(file);
                    Files.Move(currentIndex, locationInNewParent);
                    UpdateCrc();
                    moved = true;
                }
                else
                {
                    var newFolder = (Folder)newParent;
                    if (locationInNewParent < 0)
                    {
                        newFolder.Files.Add(file);
                    }
                    else
                    {
                        newFolder.Files.Insert(locationInNewParent, file);
                    }
                    moved = Files.Remove(file); // Remove nulls out the parent, so restore it.
                    file.Parent = newFolder;
                    UpdateCrc();
                    newFolder.UpdateCrc();
                }
            }
            else
            {
                throw new InvalidOperationException(Resources.Strings.Folder_NotParentError);
            }
#if REPORT_PERFORMANCE
            stopwatch.Stop();
            var prefix = "*";
            System.Diagnostics.Debug.WriteLine(prefix + "Folder.MoveToNewParent() for '" + LongName + "' took " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE
            return moved;
        }

        /// <inheritdoc />
        public bool RemoveChild(IFile child, bool updateFileSystemTables)
        {
            bool removed = Files.Remove((FileNode)child);
            if (removed && updateFileSystemTables)
            {
                FileSystem.Files.RemoveAt(((ILfsFileInfo)child).GlobalFileNumber);
            }
            if (removed)
            {
                UpdateCrc();
            }
            return removed;
        }

        /// <inheritdoc />
        public void RemoveChildFromHierarchy(Predicate<IFile> filter)
        {
            var itemsToRemove = new List<FileNode>(Files.Where(f => filter(f)));
            foreach (var item in itemsToRemove)
            {
                while (RemoveChild(item, true))
                {
                    UpdateCrc();
                    continue;
                }
            }
            foreach (var folder in Files.OfType<IFileContainer>())
            {
                folder.RemoveChildFromHierarchy(filter);
            }
        }

        /// <inheritdoc />
        public bool IsParentOfChild(IFile child)
        {
            return Files.Contains(child); // or just use child.Parent == this?
        }

        /// <inheritdoc />
        public bool ContainsChild(IFile child)
        {
            bool isParentOfChild = IsParentOfChild(child);
            if (!isParentOfChild)
            {
                foreach (var folder in Files.OfType<IFileContainer>())
                {
                    isParentOfChild = folder.ContainsChild(child);
                    if (isParentOfChild)
                    {
                        break;
                    }
                }
            }
            return isParentOfChild;
        }

        /// <inheritdoc />
        public int IndexOfChild(IFile child)
        {
            return Files.IndexOf((FileNode)child);
        }

        /// <inheritdoc />
        public IEnumerable<IFile> FindChildren(Predicate<IFile> filter, bool recurse)
        {
            var matches = new List<IFile>();
            FindChildren(filter, recurse, matches);
            return matches;
        }

        #endregion // IFileContainer Methods

        /// <inheritdoc />
        public override void UpdateCrc()
        {
            Crc32 = INTV.Core.Utility.RandomUtilities.Next32();
        }

        /// <summary>
        /// Sorts the items in the folder by long name.
        /// </summary>
        /// <param name="ascending">If set to <c>true</c> sorts in ascending order, non-case-sensitive; otherwise in descending order.</param>
        public void SortByName(bool ascending)
        {
            // Disconnect this while sorting so we cut down on noise -- this gives a HUGE performance boost.
            _items.CollectionChanged -= FilesCollectionChanged;
            var sortedItems = new ObservableCollection<FileNode>(ascending ? _items.OrderBy(f => f.LongName, StringComparer.OrdinalIgnoreCase).ToList() : _items.OrderByDescending(f => f.LongName, StringComparer.OrdinalIgnoreCase).ToList());
            foreach (var item in sortedItems)
            {
#if REPORT_PERFORMANCE
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
                var oldIndex = _items.IndexOf(item);
                var newIndex = sortedItems.IndexOf(item);
                if (oldIndex != newIndex)
                {
                    _items.Move(oldIndex, newIndex);
                }
#if REPORT_PERFORMANCE
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine("Folder.SortByName() moved " + oldIndex + "->" + newIndex + " which took: " + stopwatch.Elapsed);
#endif // REPORT_PERFORMANCE
            }
            _items.CollectionChanged += FilesCollectionChanged;
            OnItemsChanged(); // This ensures ViewModel sees the update.
        }

        #region object overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return "Folder " + LongName + " {Contains " + Items.Count() + " Entries}";
        }

        #endregion // object overrides

        /// <inheritdoc />
        internal override void LoadComplete(FileSystem fileSystem, bool updateRomList)
        {
            base.LoadComplete(fileSystem, updateRomList);
            SetGlobalDirectoryNumber(_globalDirectoryNumber);
            foreach (var file in Files)
            {
                file.LoadComplete(fileSystem, updateRomList);
            }
        }

        /// <summary>
        /// Recursively finds Program elements that meet the provided condition.
        /// </summary>
        /// <param name="condition">The predicate that must be met.</param>
        /// <returns>An enumerable of the programs that satisfy the predicate.</returns>
        internal IEnumerable<Program> FindPrograms(Predicate<Program> condition)
        {
            foreach (var item in Items)
            {
                var program = item as Program;
                if (program != null)
                {
                    if ((program.Description != null) && condition(program))
                    {
                        yield return program;
                    }
                }
                else
                {
                    var folder = item as Folder;
                    if (folder != null)
                    {
                        foreach (var p in folder.FindPrograms(condition))
                        {
                            yield return p;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event when the contents of the folder changes.
        /// </summary>
        protected virtual void OnItemsChanged()
        {
            RaisePropertyChanged("Items");
        }

        /// <summary>
        /// Raises the ContentsChanged event when something in the folder or a subfolder within it changes.
        /// </summary>
        /// <param name="sender">The entity that changed.</param>
        /// <param name="e">Default event arguments.</param>
        protected void RaiseContentsChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Items");
            var contentsChanged = ContentsChanged;
            if (contentsChanged != null)
            {
                contentsChanged(this, e);
            }
        }

        /// <summary>
        /// Assigns a Global Directory Number to the Folder.
        /// </summary>
        /// <param name="globalDirectoryNumber">The directory number to assign.</param>
        protected void SetGlobalDirectoryNumber(byte globalDirectoryNumber)
        {
            // FileSystem may not be set during creation from XML. Therefore, defer registering in the file table until the parent chain is established.
            var fileSystem = FileSystem;
            var directoryTable = fileSystem == null ? null : FileSystem.Directories;
            if (directoryTable != null)
            {
                if ((_globalDirectoryNumber < GlobalDirectoryTable.TableSize) && (_globalDirectoryNumber != globalDirectoryNumber))
                {
                    directoryTable.RemoveAt(_globalDirectoryNumber);
                }
                if (globalDirectoryNumber != GlobalDirectoryTable.InvalidDirectoryNumber)
                {
                    directoryTable.Insert(globalDirectoryNumber, this);
                }
            }
            _globalDirectoryNumber = globalDirectoryNumber;
        }

#if REPORT_PERFORMANCE
        private static int SetContentsNesting;
#endif // REPORT_PERFORMANCE

        private void FindChildren(Predicate<IFile> filter, bool recurse, List<IFile> matches)
        {
            matches.AddRange(Items.Where(i => filter(i)));
            if (recurse)
            {
                var folders = Items.OfType<Folder>();
                foreach (var folder in folders)
                {
                   folder.FindChildren(filter, recurse, matches);
                }
            }
        }

        /// <summary>
        /// Sets the contents of a folder based on a directory retrieved from a Locutus device.
        /// </summary>
        /// <param name="directory">The directory structure whose contents populate the folder.</param>
        /// <param name="deviceId">If not <c>null</c> or empty, indicates the folder is intended for a specific device. Used in error reporting.</param>
        private void SetContents(Directory directory, string deviceId)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            ++SetContentsNesting;
            try
            {
#endif // REPORT_PERFORMANCE
                if (directory != null)
                {
                    var fromDirtyFileSystem = directory.FileSystem.Status != LfsDirtyFlags.None;
                    var items = new List<FileNode>(Enumerable.Repeat<FileNode>(null, directory.PresentationOrder.ValidEntryCount));
                    for (int i = 0; i < directory.PresentationOrder.ValidEntryCount; ++i)
                    {
                        var globalFileNumber = directory.PresentationOrder[i];
                        var file = (LfsFileInfo)directory.FileSystem.Files[globalFileNumber];
                        var createFileNode = !fromDirtyFileSystem || (file != null);
                        if (createFileNode)
                        {
                            var item = FileNode.Create(file);
                            if (item != null)
                            {
                                item.FileSystem = directory.FileSystem;
                                if (item is Folder)
                                {
                                    var folder = (Folder)item;
                                    var folderContents = (Directory)directory.FileSystem.Directories[item.GlobalDirectoryNumber];
                                    try
                                    {
                                        folder.SetContents(folderContents, deviceId);
                                    }
                                    catch (InconsistentFileSystemException)
                                    {
                                        directory.FileSystem.Status |= LfsDirtyFlags.FileSystemUpdateInProgress; // flag the file system as inconsistent
                                    }
                                }
                                item.SetForks(file.ForkNumbers);
                            }
                            else
                            {
                                throw new InconsistentFileSystemException(LfsEntityType.Unknown, globalFileNumber, deviceId);
                            }
                            items[i] = item;
                        }
                    }
                    try
                    {
                        SetContents(new ObservableCollection<FileNode>(items.Where(item => item != null)));
                    }
                    catch (InconsistentFileSystemException)
                    {
                        directory.FileSystem.Status |= LfsDirtyFlags.FileSystemUpdateInProgress; // flag the file system as inconsistent
                    }
                }
                else
                {
                    throw new InconsistentFileSystemException(LfsEntityType.Directory, deviceId);
                }
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                --SetContentsNesting;
                var prefix = new string('*', SetContentsNesting + 1);
                System.Diagnostics.Debug.WriteLine(prefix + "Folder.SetContents(" + directory.FileSystem.Origin + ") for '" + LongName + "' took " + stopwatch.Elapsed.ToString());
            }
#endif // REPORT_PERFORMANCE
        }

        private void SetContents(ObservableCollection<FileNode> newContents)
        {
            if (_items != newContents)
            {
                if (_items != null)
                {
                    _items.CollectionChanged -= FilesCollectionChanged;
                }
                if (newContents != null)
                {
                    newContents.CollectionChanged += FilesCollectionChanged;
                    foreach (var file in newContents)
                    {
                        if (file != null)
                        {
                            file.Parent = this;
                        }
                        else
                        {
                            throw new InconsistentFileSystemException(LfsEntityType.Unknown, 0xFFFFFFFF, string.Empty);
                        }
                    }
                }
            }
            AssignAndUpdateProperty("Files", newContents, ref _items);
        }

        private PresentationOrder GetPresentationOrder()
        {
            var presentationOrder = new PresentationOrder(Files);
            return presentationOrder;
        }

        private void FilesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            var file = (FileNode)item;
                            file.Parent = this;
                            file.PropertyChanged += OnContentPropertyChanged;
                            var program = item as Program;
                            if (program != null)
                            {
                                if (program.Description == null)
                                {
                                    var rom = INTV.Core.Model.Rom.Create(program.Rom.FilePath, null);
                                    var programInfo = rom.GetProgramInformation();
                                    var programDescription = new INTV.Core.Model.Program.ProgramDescription(rom.Crc, rom, programInfo);
                                    program.Description = programDescription;
                                }
                                program.Description.Files.PropertyChanged += OnContentPropertyChanged;
                            }
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var file = (FileNode)item;
                            var program = item as Program;
                            if (program != null)
                            {
                                program.Description.Files.PropertyChanged -= OnContentPropertyChanged;
                            }
                            file.PropertyChanged -= OnContentPropertyChanged;
                            file.Parent = null;
                        }
                    }
                    break;
            }
            OnItemsChanged();
        }

        private void OnContentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseContentsChanged(this, EventArgs.Empty);
        }
    }
}

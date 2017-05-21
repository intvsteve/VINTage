// <copyright file="FileNodeViewModel.cs" company="INTV Funhouse">
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

//// VALIDATE_MANUAL_CONTENT

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Provides partial implementation of the IFile interface for use as a ViewModel.
    /// </summary>
    public abstract partial class FileNodeViewModel : System.ComponentModel.INotifyPropertyChanged, IFile
    {
        #region Property Names

        public const string IconPropertyName = "Icon";
        public const string IconTipStripPropertyName = "IconTipStrip";
        public const string ColorPropertyName = "Color";
        public const string ShortNamePropertyName = "ShortName";
        public const string LongNamePropertyName = "LongName";

        #endregion // Property Names

        #region INotifyPropertyChanged implementation

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the maximum length allowed for the LongName of an IFile.
        /// </summary>
        public static int MaxLongNameLength
        {
            get { return FileSystemConstants.MaxLongNameLength; }
        }

        /// <summary>
        /// Gets the maximum length allowed for the ShortName of an IFile.
        /// </summary>
        public static int MaxShortNameLength
        {
            get { return FileSystemConstants.MaxShortNameLength; }
        }

        /// <summary>
        /// Gets the maximum length allowed for the LongName of an IFile.
        /// </summary>
        public int MaxLongNameLength_xp
        {
            get { return FileSystemConstants.MaxLongNameLength; }
        }

        /// <summary>
        /// Gets the maximum length allowed for the ShortName of an IFile.
        /// </summary>
        public int MaxShortNameLength_xp
        {
            get { return FileSystemConstants.MaxShortNameLength; }
        }

        /// <summary>
        /// Gets or sets the icon to display for the item.
        /// </summary>
        [INTV.Shared.Utility.OSExport(IconPropertyName)]
        public abstract OSImage Icon { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether an IFile that is an IFileContainer is open or not.
        /// </summary>
        public abstract bool IsOpen { get; set; }

        /// <summary>
        /// Gets the tool tip to display if there are discrepancies between the menu and the ROM on disk.
        /// </summary>
        public abstract string IconTipStrip { get; }

        /// <summary>
        /// Gets or sets the display color of the file.
        /// </summary>
        [OSExport(ColorPropertyName)]
        public FileNodeColorViewModel Color
        {
            get
            {
                return FileNodeColorViewModel.GetColor(Model == null ? INTV.Core.Model.Stic.Color.White : Model.Color);
            }

            set
            {
                if (value != null)
                {
                    var currentColor = ((IFile)this).Color;
                    if (currentColor != value.IntvColor)
                    {
                        ((IFile)this).Color = value.IntvColor;
                        RaisePropertyChanged(ColorPropertyName, (s, c) => OnColorChanged(value.IntvColor), value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node has support files.
        /// </summary>
        public abstract bool HasSupportFiles { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is the selected item in its containing visual.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { AssignAndUpdateProperty("IsSelected", value, ref _isSelected); }
        }
        private bool _isSelected;

        /// <summary>
        /// Gets a value indicating whether or not the item can be edited.
        /// </summary>
        public bool IsEditable
        {
            get { return (Model != null) && (((FileNode)Model).FileSystem.Origin == FileSystemOrigin.HostComputer); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the short name of the file needs to be displayed.
        /// </summary>
        public bool ShowShortName
        {
            get { return _showShortNanme; }
            private set { AssignAndUpdateProperty("ShowShortName", value, ref _showShortNanme); }
        }
        private bool _showShortNanme;

        /// <summary>
        /// Gets a value indicating whether or not the short name of the item has been customized.
        /// </summary>
        public bool IsCustomShortName
        {
            get { return _isShortNameCustomized; }
            private set { AssignAndUpdateProperty("IsCustomShortName", value, ref _isShortNameCustomized); }
        }
        private bool _isShortNameCustomized;

        /// <summary>
        /// Gets the collection of files stored by this item. Null if the node is not a folder.
        /// </summary>
        public virtual ObservableViewModelCollection<FileNodeViewModel, IFile> Items
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public virtual IFile Model
        {
            get
            {
                return _fileModel;
            }

            set
            {
                var notifier = _fileModel as INotifyPropertyChanged;
                if (notifier != null)
                {
                    notifier.PropertyChanged -= OnModelPropertyChanged;
                }
                _fileModel = value;
                notifier = _fileModel as INotifyPropertyChanged;
                if (notifier != null)
                {
                    notifier.PropertyChanged += OnModelPropertyChanged;
                    OnModelChanged();
                }
            }
        }
        private IFile _fileModel;

        /// <summary>
        /// Gets or sets the manual associated with the file node.
        /// </summary>
        public abstract string Manual { get; set; }

        /// <summary>
        /// Gets or sets the saved data file associated with the file node.
        /// </summary>
        public abstract string SaveData { get; set; }

        /// <summary>
        /// Gets or sets the 'difference' state between source and destination.
        /// </summary>
        /// <remarks>The value for the state depends on the synchronization mode in LtoFlashViewModel.</remarks>
        internal ProgramSupportFileState State
        {
            get { return _state; }
            set { AssignAndUpdateProperty("State", value, ref _state, (p, v) => OnColorChanged(Color.IntvColor)); }
        }
        private ProgramSupportFileState _state;

        #region IFile Properties

        /// <inheritdoc />
        public FileType FileType
        {
            get { return Model.FileType; }
        }

        /// <inheritdoc />
        INTV.Core.Model.Stic.Color IFile.Color
        {
            get { return Model.Color; }
            set { Model.Color = value; }
        }

        /// <inheritdoc />
        [INTV.Shared.Utility.OSExport(ShortNamePropertyName)]
        public string ShortName
        {
            get { return IsCustomShortName ? Model.ShortName : GetDefaultShortName(); }
            set { Model.ShortName = value; }
        }

        /// <inheritdoc />
        [OSExport(LongNamePropertyName)]
        public string LongName
        {
            get { return (Model == null) ? null : Model.LongName; }
            set { Model.LongName = value; }
        }

        /// <inheritdoc />
        public IFileContainer Parent
        {
            get { return Model.Parent; }
            set { Model.Parent = value; }
        }

        /// <inheritdoc />
        public uint Crc32
        {
            get { return Model.Crc32; }
            set { throw new System.InvalidOperationException(); }
        }

        #endregion // IFile Properties

        #endregion // Properties

        /// <summary>
        /// Determines whether the manual file should be accepted.
        /// </summary>
        /// <param name="manualPath">The fully qualified path to the file to use as a manual.</param>
        /// <returns><c>true</c> if the file is acceptable as a manual.</returns>
        public static bool AcceptManual(string manualPath)
        {
            bool accept = true;
            var fileInfo = new System.IO.FileInfo(manualPath);
            accept = fileInfo.Exists && fileInfo.Length <= Device.TotalRAMSize;
#if VALIDATE_MANUAL_CONTENT
            if (accept)
            {
                var manualText = System.IO.File.ReadAllText(manualPath);
                var asciiManualText = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(manualText));
                accept = string.CompareOrdinal(asciiManualText, manualText) == 0;
            }
#endif // VALIDATE_MANUAL_CONTENT
            return accept;
        }

        /// <summary>
        /// Determines whether the save data file should be accepted.
        /// </summary>
        /// <param name="saveDataPath">The fully qualified path to the file to use as save data.</param>
        /// <returns><c>true</c> if the file is acceptable as a save data file.</returns>
        public static bool AcceptDataFile(string saveDataPath)
        {
            bool accept = true;
            var fileInfo = new System.IO.FileInfo(saveDataPath);
            accept = fileInfo.Exists && fileInfo.Length <= Device.TotalRAMSize;
            return accept;
        }

        #region object Overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return LongName;
        }

        #endregion // object Overrides

        /// <summary>
        /// Adds items to the given menu layout in which each items specifies its own destination directory.
        /// </summary>
        /// <param name="menuLayout">The menu layout to which items are to be added.</param>
        /// <param name="destinationDirectories">The destination directories to contain the added items.</param>
        /// <param name="items">The items to add.</param>
        internal static void AddItems(MenuLayoutViewModel menuLayout, IEnumerable<string> destinationDirectories, IEnumerable<ProgramDescription> items)
        {
            var taskData = new AddRomsToMenuData(menuLayout, destinationDirectories, items);
            var addRomsTask = new AsyncTaskWithProgress(Resources.Strings.AddItems_ProgressTitle, true, false);
            addRomsTask.RunTask(taskData, AddItems, AddItemsComplete);
        }

        /// <summary>
        /// Verifies that the programs in the given collection can be added to the given target folder.
        /// </summary>
        /// <param name="destination">The new home for the programs.</param>
        /// <param name="programs">The programs to be placed into the target.</param>
        /// <param name="rejectedRoms">Accumulates ROMs that were not accepted.</param>
        /// <returns><c>true</c> if the target can accept the items and each item is acceptable.</returns>
        internal static bool CanAcceptFiles(IFileContainer destination, IEnumerable<INTV.Core.Model.Program.ProgramDescription> programs, IList<System.Tuple<string, string>> rejectedRoms)
        {
            string reasonForFailure;
            var accept = (programs != null) && programs.Any() && FileSystemCanAcceptMoreItems(destination, LfsEntityType.File, programs.Count(), true, out reasonForFailure);
            if (!accept)
            {
                if (rejectedRoms != null)
                {
                    rejectedRoms.Add(new System.Tuple<string, string>(destination.LongName, Resources.Strings.AddItemRejected_TooManyItems));
                }
            }
            if (accept)
            {
                // Ensure at least one of the ROMs is valid.
                foreach (var program in programs)
                {
                    var rom = program.GetRom();
                    var file = rom.RomPath;
                    var reasonForRejection = string.Empty;
                    var acceptableRom = IsAcceptableRom(program, out reasonForRejection);
                    if (!acceptableRom && (rejectedRoms != null))
                    {
                        rejectedRoms.Add(new System.Tuple<string, string>(file, reasonForRejection));
                    }
                    accept |= acceptableRom;
                }
            }
            return accept;
        }

        /// <summary>
        /// Determines if the given program ROM can be accepted.
        /// </summary>
        /// <returns><c>true</c> if the ROM can be accepted in the menu; otherwise, <c>false</c>.</returns>
        /// <param name="program">A program ROM.</param>
        /// <param name="reasonForRejection">Receives a reason for why a ROM was not accepted.</param>
        /// <remarks>ROMs that are too large, or are identified as 'System' ROMs are rejected.</remarks>
        internal static bool IsAcceptableRom(ProgramDescription program, out string reasonForRejection)
        {
            reasonForRejection = null;
            var rom = program.GetRom();
            var file = rom.RomPath;
            var accept = !program.Features.GeneralFeatures.HasFlag(GeneralFeatures.SystemRom);
            if (!accept)
            {
                reasonForRejection = Resources.Strings.AddItemRejected_NotPlayable;
            }
            if (accept)
            {
                accept = !string.IsNullOrWhiteSpace(file);
                if (!accept)
                {
                    reasonForRejection = Resources.Strings.AddItemRejected_NoFileName;
                }
            }
            if (accept)
            {
                accept = System.IO.File.Exists(file);
                if (!accept)
                {
                    reasonForRejection = Resources.Strings.AddItemRejected_FileNotFound;
                }
            }
            if (accept)
            {
                var fileInfo = new System.IO.FileInfo(file);
                accept = (fileInfo.Length <= INTV.Core.Model.Rom.MaxROMSize) && (fileInfo.Length <= Device.TotalRAMSize);
                if (!accept)
                {
                    reasonForRejection = string.Format(Resources.Strings.AddItemRejected_FileTooLargeFormat, fileInfo.Length, System.Math.Min(INTV.Core.Model.Rom.MaxROMSize, Device.TotalRAMSize));
                }
            }
            return accept;
        }

        /// <summary>
        /// Checks to see if the given destination can accept more items.
        /// </summary>
        /// <param name="destination">The destination for another file system entry.</param>
        /// <param name="entityType">The type if the file system entry (fork, file, directory).</param>
        /// <param name="count">The number of new entries of the given type.</param>
        /// <param name="addingNewItems">If <c>true</c>, indicates new entries are being added; if <c>false</c>, items are moving from one location to another.</param>
        /// <param name="reasonForFailure">Receives error description.</param>
        /// <returns><c>true</c> if <paramref name="destination"/> is able to accept at least <paramref name="count"/> new items.</returns>
        internal static bool FileSystemCanAcceptMoreItems(IFile destination, LfsEntityType entityType, int count, bool addingNewItems, out string reasonForFailure)
        {
            reasonForFailure = string.Empty;
            var directory = destination as Folder;
            var file = destination as FileNode;
            var canAccept = (directory != null) || (file != null);
            if (canAccept)
            {
                var destinationType = (directory != null) ? LfsEntityType.Directory : ((file != null) ? LfsEntityType.File : LfsEntityType.Unknown);
                System.Diagnostics.Debug.Assert(destinationType != LfsEntityType.Unknown, "Unknown destination for new file system entry.");
                var fileSystem = file.FileSystem;
                switch (entityType)
                {
                    case LfsEntityType.Directory:
                        // If we're operating on directories, they must go inside another directory.
                        canAccept = (count + directory.Size) <= FileSystemConstants.MaxItemCount;
                        if (!canAccept)
                        {
                            reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyFiles_Format, FileSystemConstants.MaxItemCount);
                        }
                        if (addingNewItems && canAccept)
                        {
                            // New directory requires space in GDT and GFT.
                            canAccept &= (count + fileSystem.Directories.ItemsInUse) <= GlobalDirectoryTable.TableSize;
                            if (!canAccept)
                            {
                                reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyFolders_Format, GlobalDirectoryTable.TableSize);
                            }
                            if (canAccept)
                            {
                                canAccept &= (count + fileSystem.Files.ItemsInUse) <= GlobalFileTable.TableSize;
                                if (!canAccept)
                                {
                                    reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyFileSystemFiles_Format, GlobalFileTable.TableSize);
                                }
                            }
                        }
                        break;
                    case LfsEntityType.File:
                        // If we're operating on files, they must go inside a directory.
                        canAccept = (count + directory.Size) <= FileSystemConstants.MaxItemCount;
                        if (!canAccept)
                        {
                            reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyFiles_Format, FileSystemConstants.MaxItemCount);
                        }
                        if (addingNewItems && canAccept)
                        {
                            // New files require space in the GFT and GKT -- useful files typically use *at least* one fork for our purposes,
                            // though, strictly speaking, not all files require a fork. (E.g. the file for the root directory.)
                            canAccept &= (count + fileSystem.Files.ItemsInUse) <= GlobalFileTable.TableSize;
                            if (!canAccept)
                            {
                                reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyFileSystemFiles_Format, GlobalFileTable.TableSize);
                            }
                            if (canAccept)
                            {
                                canAccept &= (count + fileSystem.Forks.ItemsInUse) <= GlobalForkTable.TableSize;
                                if (!canAccept)
                                {
                                    reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyForks_Format, GlobalForkTable.TableSize);
                                }
                            }
                        }
                        break;
                    case LfsEntityType.Fork:
                        // Forks go within files - so we only need to check fork table.
                        if (addingNewItems)
                        {
                            canAccept = (count + fileSystem.Forks.ItemsInUse) <= GlobalForkTable.TableSize;
                            if (!canAccept)
                            {
                                reasonForFailure = string.Format(Resources.Strings.AddItemRejected_TooManyForks_Format, GlobalForkTable.TableSize);
                            }
                        }
                        break;
                    default:
                        canAccept = false;
                        reasonForFailure = string.Format(Resources.Strings.AddItemRejected_UnknownFileSystemEntityType_Format, entityType);
                        break;
                }
            }
            else
            {
                if ((directory == null) && (file == null))
                {
                    reasonForFailure = Resources.Strings.AddItemRejected_InvalidFolderAndFile;
                }
                else if (directory == null)
                {
                    reasonForFailure = Resources.Strings.AddItemRejected_InvalidFolder;
                }
                else if (file == null)
                {
                    reasonForFailure = Resources.Strings.AddItemRejected_InvalidFile;
                }
            }
            return canAccept;
        }

        /// <summary>
        /// Factory used to create the appropriate ViewModel implementation.
        /// </summary>
        /// <param name="file">The model object for which to create a view model.</param>
        /// <returns>The appropriate ViewModel for the given model.</returns>
        protected static FileNodeViewModel Factory(IFile file)
        {
            if (file is IFileContainer)
            {
                return new FolderViewModel() { Model = file };
            }
            else
            {
                return new ProgramViewModel() { Model = file };
            }
        }

        /// <summary>
        /// Add items to a specific destination folder.
        /// </summary>
        /// <param name="menuLayout">The menu layout to add items to.</param>
        /// <param name="destination">The folder to which items are to be added.</param>
        /// <param name="items">The items to add.</param>
        /// <param name="insertIndex">The location at which to insert the new items.</param>
        protected static void AddItems(MenuLayoutViewModel menuLayout, IFileContainer destination, IEnumerable<ProgramDescription> items, int insertIndex)
        {
            var taskData = new AddRomsToMenuData(menuLayout, destination, items, insertIndex);
            var addRomsTask = new AsyncTaskWithProgress(Resources.Strings.AddItems_ProgressTitle, true, false);
            addRomsTask.RunTask(taskData, AddItems, AddItemsComplete);
        }

        /// <summary>
        /// Gets the icon for the item in its current state, for the given color.
        /// </summary>
        /// <param name="color">The color of the desired icon.</param>
        /// <returns>The icon.</returns>
        internal abstract OSImage GetIconForColor(INTV.Core.Model.Stic.Color color);

        /// <summary>
        /// This method is called when the color of the item is being changed.
        /// </summary>
        /// <param name="newColor">The new color for the file system entry.</param>
        protected abstract void OnColorChanged(INTV.Core.Model.Stic.Color newColor);

        /// <summary>
        /// This method is called when the underlying model has been modified.
        /// </summary>
        protected virtual void OnModelChanged()
        {
            OnShortNameChanged(false);
            OnLongNameChanged(false);
            OnColorChanged(false);
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            INotifyPropertyChangedHelpers.RaisePropertyChanged(this, PropertyChanged, propertyName);
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T">Data type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="customAction">Custom action to execute when property value changes, executed after the PropertyChanged event is raised.</param>
        /// <param name="newValue">New value of the property.</param>
        protected void RaisePropertyChanged<T>(string name, System.Action<string, T> customAction, T newValue)
        {
            INotifyPropertyChangedHelpers.RaisePropertyChanged(this, PropertyChanged, name, customAction, newValue);
        }

        /// <summary>
        /// Assigns and updates the property.
        /// </summary>
        /// <typeparam name="T">Data type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="newValue">New value of the property.</param>
        /// <param name="currentValue">Current value of the property.</param>
        /// <returns><c>true</c>, if the property value changed (and was assigned), <c>false</c> otherwise.</returns>
        protected bool AssignAndUpdateProperty<T>(string name, T newValue, ref T currentValue)
        {
            return AssignAndUpdateProperty(name, newValue, ref currentValue, null);
        }

        /// <summary>
        /// Assigns and updates the property.
        /// </summary>
        /// <typeparam name="T">Data type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="newValue">New value of the property.</param>
        /// <param name="currentValue">Current value of the property.</param>
        /// <param name="customAction">Custom action to execute when property value changes, executed prior to raising the PropertyChanged event.</param>
        /// <returns><c>true</c>, if the property value changed (and was assigned), <c>false</c> otherwise.</returns>
        protected bool AssignAndUpdateProperty<T>(string name, T newValue, ref T currentValue, System.Action<string, T> customAction)
        {
            return INotifyPropertyChangedHelpers.AssignAndUpdateProperty(this, PropertyChanged, name, newValue, ref currentValue, customAction);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="customAction">Custom action to execute prior to raising the property changed event.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes before the PropertyChange event is raised.</remarks>
        protected bool UpdateProperty<T>(string name, System.Action<string, T> customAction, T newValue, T currentValue)
        {
            return INotifyPropertyChangedHelpers.UpdateProperty(this, PropertyChanged, name, customAction, newValue, currentValue);
        }

        private static void AddItems(AsyncTaskData taskData)
        {
            var args = (AddRomsToMenuData)taskData;
            args.MenuLayout.StartItemsUpdate();
            var insertIndex = args.InsertLocation;
            var destination = args.Destination;
            var destinationDirectories = (args.DestinationDirectories == null) ? null : args.DestinationDirectories.ToList();
            var numItemsToAdd = args.ItemsToAdd.Count();
            var numAdded = 0;
            foreach (var item in args.ItemsToAdd)
            {
                if (args.AcceptCancelIfRequested())
                {
                    break;
                }

                if (destinationDirectories != null)
                {
                    destination = args.MenuLayout.MenuLayout as Folder;
                    var folders = destinationDirectories[numAdded].Split(new[] { System.IO.Path.DirectorySeparatorChar }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var folder in folders)
                    {
                        var existingFolder = (Folder)((Folder)destination).Files.FirstOrDefault(f => (f is Folder) && (f.Name == folder));
                        if (existingFolder == null)
                        {
                            string reasonForFailure;
                            if (FileSystemCanAcceptMoreItems(destination, LfsEntityType.Directory, 1, true, out reasonForFailure))
                            {
                                var newParentFolder = args.MenuLayout.MenuLayout.CreateFolder(folder);
                                args.UIDispatcher.Invoke(new System.Action(() => destination.AddChild(newParentFolder, true)), OSDispatcherPriority.Background);
                                destination = newParentFolder;
                            }
                            else
                            {
                                AddFailedEntry(args.FailedToAdd, destination, folder, reasonForFailure, item.Rom.RomPath);
                            }
                        }
                        else
                        {
                            destination = existingFolder;
                        }
                    }
                }
                if (destination == null)
                {
                    // default to the root
                    destination = args.MenuLayout.MenuLayout;
                }

                ++numAdded;
                args.UpdateTaskProgress(((double)numAdded / numItemsToAdd) * 100, item.Name);
                var reasonForRejection = string.Empty;
                var accepted = IsAcceptableRom(item, out reasonForRejection);
                if (!accepted)
                {
                    AddFailedEntry(args.FailedToAdd, destination, item.Name, reasonForRejection, item.Rom.RomPath);
                }
                else
                {
                    string reasonForFailure;
                    accepted = FileSystemCanAcceptMoreItems(destination, LfsEntityType.File, 1, true, out reasonForFailure);
                    if (!accepted)
                    {
                        AddFailedEntry(args.FailedToAdd, destination, item.Name, reasonForFailure, item.Rom.RomPath);
                    }
                }
                if (accepted)
                {
                    try
                    {
                        var program = args.MenuLayout.MenuLayout.CreateProgram(item);
                        if (insertIndex < 0)
                        {
                            args.UIDispatcher.Invoke(new System.Action(() => destination.AddChild(program, true)), OSDispatcherPriority.Background);
                        }
                        else
                        {
                            args.UIDispatcher.Invoke(new System.Action(() => destination.InsertChild(insertIndex, program, true)), OSDispatcherPriority.Background);
                            ++insertIndex;
                        }
                    }
                    catch (LuigiFileGenerationException exception)
                    {
                        AddFailedEntry(args.FailedToAdd, destination, item.Name, Resources.Strings.AddItemRejected_FailedToCreateLUIGIFile, item.Rom.RomPath);
                        if (args.FirstFilePreparationError == null)
                        {
                            args.FirstFilePreparationError = exception;
                        }
                    }
                }
            }
        }

        private static void AddFailedEntry(IDictionary<string, IDictionary<string, IList<System.Tuple<string, string>>>> failedToAdd, IFileContainer destination, string itemName, string reason, string filePath)
        {
            var fileNode = destination as FileNode;
            var locationInMenu = fileNode.GetMenuPath();
            IDictionary<string, IList<System.Tuple<string, string>>> existingErrors;
            if (!failedToAdd.TryGetValue(locationInMenu, out existingErrors))
            {
                existingErrors = new Dictionary<string, IList<System.Tuple<string, string>>>();
                failedToAdd[locationInMenu] = existingErrors;
            }
            IList<System.Tuple<string, string>> existingFilesForError;
            if (!existingErrors.TryGetValue(reason, out existingFilesForError))
            {
                existingFilesForError = new List<System.Tuple<string, string>>();
                existingErrors[reason] = existingFilesForError;
            }
            existingFilesForError.Add(new System.Tuple<string, string>(itemName, filePath));
        }

        private static void AddItemsComplete(AsyncTaskData taskData)
        {
            var args = (AddRomsToMenuData)taskData;
            args.MenuLayout.FinishItemsUpdate(args.Error == null);
            if (args.Error == null)
            {
                if (args.Destination != null)
                {
                    var insertFolder = args.MenuLayout.FindViewModelForModel(args.Destination);
                    FileNodeViewModel highlightItem = insertFolder;

                    // NOTE: Multi-Mode Tree will need to fix this!
                    if (insertFolder.IsOpen || (insertFolder == args.MenuLayout))
                    {
                        var items = insertFolder.Items;
                        var highlightIndex = args.InsertLocation;
                        if (highlightIndex < 0)
                        {
                            highlightIndex = System.Math.Max(0, items.Count - 1);
                        }
                        if (highlightIndex < items.Count)
                        {
                            highlightItem = items[highlightIndex];
                            highlightItem.IsSelected = true;
                            ////args.MenuLayout.RetainFocus = items[highlightIndex].GetHashCode();
                        }
                    }
                    else
                    {
                        insertFolder.IsSelected = true;
                        ////args.MenuLayout.RetainFocus = insertFolder.GetHashCode();
                    }
                    ++args.MenuLayout.RetainFocus;
#if MAC
                    if (highlightItem != null)
                    {
                        args.MenuLayout.CurrentSelection = highlightItem;
                    }
#endif // MAC
                }
            }
            if ((args.Error != null) || args.FailedToAdd.Any())
            {
                var exception = args.Error;
                if (exception == null)
                {
                    exception = args.FirstFilePreparationError;
                }
                IRomHelpers.ReportAddItemsError(exception, args.FailedToAdd);
            }
        }

        private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case FileNode.LongNamePropertyName:
                    OnLongNameChanged(true);
                    break;
                case FileNode.ShortNamePropertyName:
                    OnShortNameChanged(true);
                    break;
                case FileNode.ColorPropertyName:
                    OnColorChanged(true);
                    break;
            }
            RaisePropertyChanged(e.PropertyName);
        }

#if false
        private bool DropItem(MenuLayoutViewModel menuLayout, IFile droppedItem)
        {
            var acceptedDroppedItem = droppedItem != null;
            if (acceptedDroppedItem)
            {
                if (menuLayout != null)
                {
                    menuLayout.StartItemsUpdate();
                }
                if (droppedItem.Parent != null)
                {
                    var selfAsFileContainer = Model as IFileContainer;
                    if (selfAsFileContainer != null)
                    {
                        if (!selfAsFileContainer.AddChild(droppedItem))
                        {
                            // we're moving to the bottom, so remove and re-add
                            selfAsFileContainer.RemoveChild(droppedItem);
                            selfAsFileContainer.AddChild(droppedItem);
                        }
                    }
                    else
                    {
                        // if an item is dropped on a non-file, insert before the drop target item.
                        var insertLocation = Parent.IndexOf(Model);
                        Parent.Insert(insertLocation, droppedItem);
                    }
                }
                if (menuLayout != null)
                {
                    menuLayout.FinishItemsUpdate();
                }
                CommandManager.InvalidateRequerySuggested();
            }
            return acceptedDroppedItem;
        }

        private void DropItems(object dragEventArgs)
        {
#if WIN
            var dragDropArgs = dragEventArgs as System.Windows.DragEventArgs;
            var dropData = dragDropArgs.Data;
            if ((dropData != null) && AcceptDragData(dropData))
            {
                var menuLayout = dropData.GetData(MenuLayoutViewModel.DragMenuLayoutDataFormat) as MenuLayoutViewModel;
                if (menuLayout == null)
                {
                    menuLayout = this as MenuLayoutViewModel;
                }
                if (menuLayout != null)
                {
                    menuLayout.StartItemsUpdate();
                }
                var droppedItem = dropData.GetData(MenuLayoutViewModel.DragDataFormat) as IFile;
                if (droppedItem != null)
                {
                    if (droppedItem.Parent != null)
                    {
                        var selfAsFileContainer = Model as IFileContainer;
                        if (selfAsFileContainer != null)
                        {
                            if (!droppedItem.Parent.MoveChildToNewParent(droppedItem, selfAsFileContainer))
                            {
                                throw new System.InvalidOperationException("failed to move");
                            }
                            //if (!selfAsFileContainer.AddChild(droppedItem))
                            //{
                            //    // we're moving to the bottom, so remove and re-add
                            //    selfAsFileContainer.RemoveChild(droppedItem);
                            //    selfAsFileContainer.AddChild(droppedItem);
                            //}
                        }
                        else
                        {
                            // if an item is dropped on a non-file, insert before the drop target item.
                            var insertLocation = Parent.IndexOf(Model);
                            Parent.Insert(insertLocation, droppedItem);
                        }
                    }
                }
                else
                {
                    var items = dropData.GetData(ProgramDescriptionViewModel.DragDataFormat) as IEnumerable<ProgramDescription>;
                    if ((items != null) && items.Any())
                    {
                        IFileContainer dropTarget = Model as IFileContainer;
                        int insertIndex = -1;
                        if (dropTarget == null)
                        {
                            dropTarget = Parent;
                            insertIndex = dropTarget.IndexOf(Model);
                        }
                        AddItems(menuLayout, dropTarget, items, insertIndex);
                    }
                }
                if (menuLayout != null)
                {
                    menuLayout.FinishItemsUpdate(true);
                }
                CommandManager.InvalidateRequerySuggested();
            }
#elif MAC
            ErrorReporting.ReportNotImplementedError("FileNodeFileModel.DropItems");
#endif // WIN
        }
#endif // false

        /// <summary>
        /// Determines if the item should accept a collection of dragged items.
        /// </summary>
        /// <param name="draggedItems">The items being dragged.</param>
        /// <returns><c>true</c> if the items should be accepted.</returns>
        internal bool ShouldAcceptDraggedItems(IEnumerable<IFile> draggedItems)
        {
            bool accept = !draggedItems.Any(draggedItem => !ShouldAcceptDraggedItem(draggedItem));
            return accept;
        }

        /// <summary>
        /// Determines whether to accept a dragged item.
        /// </summary>
        /// <returns><c>true</c>, if dragged item should be accepted, <c>false</c> otherwise.</returns>
        /// <param name="draggedItem">Dragged item.</param>
        /// <remarks>Implementation detail: Code really kind of expects draggedItem to be a model, not view model.
        /// Also, doesn't look for nonsense moves, like moving an item onto itself, before itself, or after itself
        /// so much. Perhaps this is OK in Windows already? Mac is checking separately for this.</remarks>
        private bool ShouldAcceptDraggedItem(IFile draggedItem)
        {
            bool accept = draggedItem != null;
            if (accept)
            {
                IFileContainer dropTarget = Model as IFileContainer;
                if (dropTarget == null)
                {
                    dropTarget = Parent;
                }
                var draggedItemParent = draggedItem.Parent;
                accept = draggedItem != this.Model; // don't drop on self
                if (accept && (dropTarget != null))
                {
                    var draggedContainer = draggedItem as IFileContainer;
                    if (draggedContainer != null)
                    {
                        accept = !draggedContainer.ContainsChild(dropTarget) && (draggedContainer != dropTarget);
                    }
                    if (accept)
                    {
                        accept = (dropTarget.Size < FileSystemConstants.MaxItemCount) || (dropTarget == draggedItemParent);
                    }
                }
            }
            return accept;
        }

        /// <summary>
        /// Determines if the item should accept a collection of dragged items.
        /// </summary>
        /// <param name="programDescriptions">The programs being dragged.</param>
        /// <returns><c>true</c> if the items should be accepted.</returns>
        internal bool ShouldAcceptDraggedItems(IEnumerable<ProgramDescription> programDescriptions)
        {
            IFileContainer dropTarget = null;
            if ((programDescriptions != null) && programDescriptions.Any())
            {
                dropTarget = Model as IFileContainer;
                if (dropTarget == null)
                {
                    dropTarget = Parent;
                }
            }
            return (dropTarget != null) && ((programDescriptions.Count() + dropTarget.Size) <= FileSystemConstants.MaxItemCount);
        }

        private void OnLongNameChanged(bool modelReportedChange)
        {
            if (!IsCustomShortName)
            {
                RaisePropertyChanged(FileNode.ShortNamePropertyName);
            }
            ShowShortName = (LongName != null) && (LongName.Length > FileSystemConstants.MaxShortNameLength || IsCustomShortName);
        }

        private void OnShortNameChanged(bool modelReportedChange)
        {
            IsCustomShortName = ((Model.ShortName != null) || (((FileNode)Model).GlobalFileNumber == GlobalFileTable.RootDirectoryFileNumber)) || (ShortName != GetDefaultShortName());
        }

        private void OnColorChanged(bool modelReportedChange)
        {
        }

        private string GetDefaultShortName()
        {
            string defaultShortName = string.IsNullOrEmpty(LongName) ? Model.ShortName : LongName.Substring(0, System.Math.Min(LongName.Length, FileSystemConstants.MaxShortNameLength));
            return defaultShortName;
        }
    }
}

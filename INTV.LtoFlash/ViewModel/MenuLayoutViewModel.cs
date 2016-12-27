// <copyright file="MenuLayoutViewModel.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

using IntvColor = INTV.Core.Model.Stic.Color;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for a MenuLayout.
    /// </summary>
    public partial class MenuLayoutViewModel : FolderViewModel
    {
        #region Property Names

        public const string CurrentSelectionPropertyName = "CurrentSelection";
        public const string FolderCountPropertyName = "FolderCount";
        public const string FileCountPropertyName = "FileCount";
        public const string ForkCountPropertyName = "ForkCount";
        public const string ShowOverlayPropertyName = "ShowOverlay";
        public const string OverlayTextPropertyName = "OverlayText";
        public const string OverallInUseRatioPropertyName = "OverallInUseRatio";
        public const string OverallUsageDetailsPropertyName = "OverallUsageDetails";

        #endregion // Property Names

        #region UI Strings

        /// <summary>
        /// Title for the menu layout area.
        /// </summary>
        public static readonly string Title = Resources.Strings.MenuLayout_Title;

        /// <summary>
        /// Tip strip for the New Folder button.
        /// </summary>
        public static readonly string NewFolderTip = Resources.Strings.MenuLayout_NewDirectoryTip;

        /// <summary>
        /// Tip strip for the Color chooser.
        /// </summary>
        public static readonly string ColorTip = Resources.Strings.MenuLayout_ColorPickerTip;

        /// <summary>
        /// Column name for an item's 'long' file name.
        /// </summary>
        public static readonly string LongNameHeader = Resources.Strings.MenuLayout_LongNameColumnHeader;

        /// <summary>
        /// Tip-strip for the long file name.
        /// </summary>
        public static readonly string LongNameTip = Resources.Strings.MenuLayout_LongNameTip;

        /// <summary>
        /// Column name for the item's 'short' file name.
        /// </summary>
        public static readonly string ShortNameHeader = Resources.Strings.MenuLayout_ShortNameColumnHeader;

        /// <summary>
        /// Tip-strip for the short file name.
        /// </summary>
        public static readonly string ShortNameTip = Resources.Strings.MenuLayout_ShortNameTip;

        /// <summary>
        /// Column name for an item's manual or directory info.
        /// </summary>
        public static readonly string ManualHeader = Resources.Strings.MenuLayout_ManualColumnHeader;

        /// <summary>
        /// Column name for an item's saved data file.
        /// </summary>
        public static readonly string SaveDataHeader = Resources.Strings.MenuLayout_SaveDataColumnHeader;

        /// <summary>
        /// String for showing storage used.
        /// </summary>
        public static readonly string StorageUsed = Resources.Strings.MenuLayout_StorageUsed;

        #endregion // UI Strings

        #region Drag and Drop Data Identifiers

        /// <summary>
        /// Identifies the data format for dragging items into the MenuLayout. The data must be an IFile.
        /// </summary>
        public static readonly string DragDataFormat = "Intellivision.LTOFlashFile";

        /// <summary>
        /// Identifies the data format used to track the MenuLayoutViewModel into which a ProgramDescription was dragged.
        /// </summary>
        public static readonly string DragMenuLayoutDataFormat = "Intellivision.MenuLayout";

        #endregion // Drag and Drop Data Identifiers

        /// <summary>
        /// The command to save the menu layout to disk.
        /// </summary>
        public static readonly RelayCommand SaveMenuLayoutCommand = new RelayCommand(SaveMenuLayout)
        {
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        /// <summary>
        /// These are the colors available to assign to menu items.
        /// </summary>
        internal static readonly IntvColor[] Colors = new IntvColor[]
        {
            IntvColor.White,
            IntvColor.Blue,
            IntvColor.Red,
            IntvColor.DarkGreen,
            IntvColor.Green,
            IntvColor.Yellow,
            IntvColor.Tan,
            IntvColor.Black
        };

        private static readonly string DeleteFolderIcon = ResourceHelpers.CreatePackedResourceString(typeof(MenuLayoutViewModel), "Resources/Images/delete_folder.png");
        private static readonly string DeleteFileIcon = ResourceHelpers.CreatePackedResourceString(typeof(MenuLayoutViewModel), "Resources/Images/delete_document_16xLG.png");

        private int _nextNewFolderNumber;
        private int _updatingItems;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MenuLayoutViewModel class.
        /// </summary>
        public MenuLayoutViewModel()
        {
            // This null check keeps the WPF XAML designer output clean.
            if (INTV.Shared.Utility.SingleInstanceApplication.Instance != null)
            {
                SelectedItems = new ObservableCollection<FileNodeViewModel>();
                _deleteSelectedItemIcon = DeleteFileIcon;
                var configuration = SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
                var savedMenuPath = configuration.MenuLayoutPath;
                _filePath = savedMenuPath;
                Model = LoadMenuLayout(savedMenuPath);
                MenuLayout.MenuLayoutSaved += HandleMenuLayoutSaved;
                var toolsLocation = SingleInstanceApplication.Instance.GetConfiguration<JzIntv.Model.Configuration>().ToolsDirectory;
                if (!System.IO.Directory.Exists(toolsLocation))
                {
                    throw new InvalidOperationException("Tools directory missing!");
                }
                Initialize();
                _availableColors = new ObservableCollection<FileNodeColorViewModel>(Colors.Select(c => FileNodeColorViewModel.GetColor(c)));
                UpdateOverlay(Resources.Strings.MenuLayout_NoItems, !MenuLayout.Items.Any());
                UpdateSystemContentsUsage(MenuLayout.FileSystem.Directories);
                UpdateSystemContentsUsage(MenuLayout.FileSystem.Files);
                UpdateSystemContentsUsage(MenuLayout.FileSystem.Forks);
                MenuLayout.FileSystem.Directories.CollectionChanged += FileSystemContentsChanged;
                MenuLayout.FileSystem.Files.CollectionChanged += FileSystemContentsChanged;
                MenuLayout.FileSystem.Forks.CollectionChanged += FileSystemContentsChanged;
            }
            else
            {
                Initialize();
            }
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the master LtoFlashViewModel.
        /// </summary>
        public LtoFlashViewModel LtoFlashViewModel { get; internal set; }

        #region Properties

        /// <summary>
        /// Gets the file save path for the menu layout.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            private set { AssignAndUpdateProperty("FilePath", value, ref _filePath, (n, p) => SaveMenuLayout(this)); }
        }
        private string _filePath;

        /// <summary>
        /// Gets the root of the menu layout.
        /// </summary>
        public FolderViewModel Root
        {
            get { return this; }
        }

        /// <summary>
        /// Gets or sets the tip strip to display for the Delete Selected Item command.
        /// </summary>
        public string DeleteSelectedItemTip
        {
            get { return string.IsNullOrEmpty(_deleteSelectedItemTip) ? Resources.Strings.RemoveFromMenuCommand_TipDescription : _deleteSelectedItemTip; }
            set { AssignAndUpdateProperty("DeleteSelectedItemTip", value, ref _deleteSelectedItemTip); }
        }
        private string _deleteSelectedItemTip;

        /// <summary>
        /// Gets or sets the image resource to display for the Delete Selected Item command.
        /// </summary>
        public string DeleteSelectedItemIcon
        {
            get { return _deleteSelectedItemIcon; }
            set { AssignAndUpdateProperty("DeleteSelectedItemIcon", value, ref _deleteSelectedItemIcon); }
        }
        private string _deleteSelectedItemIcon;

        #region Commands

        /// <summary>
        /// Gets the preview command to execute when a drag operation is about to enter the visual.
        /// </summary>
        public RelayCommand PreviewDragEnterCommand
        {
            get { return new RelayCommand(PreviewDragEnter); }
        }

        /// <summary>
        /// Gets the command to set the color of the currently selected item.
        /// </summary>
        public RelayCommand SetColorCommand
        {
            get { return new RelayCommand(SetColor, CanSetColor); }
        }

        /// <summary>
        /// Gets the command to add items to the menu layout. The specific items to add are provided by the entity triggering the command.
        /// </summary>
        public RelayCommand AddSelectedItemsCommand
        {
            get { return new RelayCommand(AddSelectedItems, CanAddSelectedItems); }
        }

        #endregion // Commands

        /// <summary>
        /// Gets the model as a MenuLayout.
        /// </summary>
        public MenuLayout MenuLayout
        {
            get
            {
                return Model as MenuLayout;
            }

            internal set
            {
                var current = MenuLayout;
                if (current != value)
                {
                    CurrentSelection = null; // clear selection
                    if (current != null)
                    {
                        current.FileSystem.Directories.CollectionChanged -= FileSystemContentsChanged;
                        current.FileSystem.Files.CollectionChanged -= FileSystemContentsChanged;
                        current.FileSystem.Forks.CollectionChanged -= FileSystemContentsChanged;
                    }
                    if (MenuLayout != null)
                    {
                        MenuLayout.MenuLayoutSaved -= HandleMenuLayoutSaved;
                    }
                    Model = value;
                    if (value != null)
                    {
                        UpdateOverlay(Resources.Strings.MenuLayout_NoItems, !value.Items.Any());
                        UpdateSystemContentsUsage(value.FileSystem.Directories);
                        UpdateSystemContentsUsage(value.FileSystem.Files);
                        UpdateSystemContentsUsage(value.FileSystem.Forks);
                        value.FileSystem.Directories.CollectionChanged += FileSystemContentsChanged;
                        value.FileSystem.Files.CollectionChanged += FileSystemContentsChanged;
                        value.FileSystem.Forks.CollectionChanged += FileSystemContentsChanged;
                        value.MenuLayoutSaved += HandleMenuLayoutSaved;
                    }
                    SaveMenuLayout(this);
                    RaisePropertyChanged("Items");
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item, as indicated by the visual.
        /// </summary>
        public FileNodeViewModel CurrentSelection
        {
            get { return _currentSelection; }
            set { AssignAndUpdateProperty(CurrentSelectionPropertyName, value, ref _currentSelection, (p, v) => UpdateSelectionRelatedProperties(v)); }
        }
        private FileNodeViewModel _currentSelection;

        /// <summary>
        /// Gets the currently selected items.
        /// </summary>
        /// <remarks>TODO: This will be used for multi-select, once it's implemented.</remarks>
        public ObservableCollection<FileNodeViewModel> SelectedItems { get; private set; }

        /// <summary>
        /// Gets or sets the retain focus value. The exact value is irrelevant - it only needs to be changed to retain focus in the menu layout.
        /// </summary>
        public int RetainFocus
        {
            get { return _forceRetainFocus; }
            set { AssignAndUpdateProperty("RetainFocus", value, ref _forceRetainFocus); }
        }
        private int _forceRetainFocus;

        /// <summary>
        /// Gets the total number of folders in use in the menu, which includes the root.
        /// </summary>
        public string FolderCount
        {
            get { return _folderCount; }
            private set { AssignAndUpdateProperty(FolderCountPropertyName, value, ref _folderCount); }
        }
        private string _folderCount;

        /// <summary>
        /// Gets the total number of files in use in the menu, which includes the file associated with the root folder.
        /// </summary>
        public string FileCount
        {
            get { return _fileCount; }
            private set { AssignAndUpdateProperty(FileCountPropertyName, value, ref _fileCount); }
        }
        private string _fileCount;

        /// <summary>
        /// Gets the total number of data forks in use, which includes ROMs, manuals, save-game data, and other data.
        /// </summary>
        public string ForkCount
        {
            get { return _forkCount; }
            private set { AssignAndUpdateProperty(ForkCountPropertyName, value, ref _forkCount); }
        }
        private string _forkCount;

        /// <summary>
        /// Gets the simplest representation of how full the Locutus File System is.
        /// </summary>
        /// <remarks>The value represents the 'most full' aspect of the file system based on the
        /// limits for forks, files, and directories.</remarks>
        public double OverallInUseRatio
        {
            get { return _overallInUseRatio; }
            private set { AssignAndUpdateProperty(OverallInUseRatioPropertyName, value, ref _overallInUseRatio); }
        }
        private double _overallInUseRatio;

        /// <summary>
        /// Gets a string describing device storage usage in detail.
        /// </summary>
        public string OverallUsageDetails
        {
            get { return _overallUsageDetails; }
            private set { AssignAndUpdateProperty(OverallUsageDetailsPropertyName, value, ref _overallUsageDetails); }
        }
        private string _overallUsageDetails;

        /// <summary>
        /// Gets the ratio (in the range [0.0 - 1.0] of LFS directories used, based on count.
        /// </summary>
        public double DirectoriesUsedRatio
        {
            get { return _directoriesUsedRatio; }
            private set { AssignAndUpdateProperty("DirectoriesUsedRatio", value, ref _directoriesUsedRatio); }
        }
        private double _directoriesUsedRatio;

        /// <summary>
        /// Gets the ratio (in the range [0.0 - 1.0] of LFS files used, based on count.
        /// </summary>
        public double FilesUsedRatio
        {
            get { return _filesUsedRatio; }
            private set { AssignAndUpdateProperty("FilesUsedRatio", value, ref _filesUsedRatio); }
        }
        private double _filesUsedRatio;

        /// <summary>
        /// Gets the ratio (in the range [0.0 - 1.0] of LFS forks used, based on count.
        /// </summary>
        public double ForksUsedRatio
        {
            get { return _forksUsedRatio; }
            private set { AssignAndUpdateProperty("ForksUsedRatio", value, ref _forksUsedRatio); }
        }
        private double _forksUsedRatio;

        /// <summary>
        /// Gets the ratio (in the range [0.0 - 1.0]) of flash storage space used.
        /// </summary>
        public double SpaceUsedRatio
        {
            get { return _spaceUsedRatio; }
            private set { AssignAndUpdateProperty("SpaceUsedRatio", value, ref _spaceUsedRatio); }
        }
        private double _spaceUsedRatio;

        /// <summary>
        /// Gets or sets the estimated storage used in bytes.
        /// </summary>
        public uint EstimatedStorageUsed
        {
            get { return _estimatedStorageUsed; }
            set { AssignAndUpdateProperty("EstimatedStorageUsed", value, ref _estimatedStorageUsed); }
        }
        private uint _estimatedStorageUsed;

        /// <summary>
        /// Gets or sets the estimated storage used in MB. (Old school not the new-fangled MibiBibiBobbetyBoop units. :P )
        /// </summary>
        public string EstimatedStorageUsedMB
        {
            get { return _estimatedStorageUsedMB; }
            set { AssignAndUpdateProperty("EstimatedStorageUsedMB", value, ref _estimatedStorageUsedMB); }
        }
        private string _estimatedStorageUsedMB;

        /// <summary>
        /// Gets the available menu item colors.
        /// </summary>
        public ObservableCollection<FileNodeColorViewModel> AvailableColors
        {
            get { return _availableColors; }
        }
        private ObservableCollection<FileNodeColorViewModel> _availableColors;

        /// <summary>
        /// Gets a value indicating whether the semi-transparent 'drop stuff here' overlay should be visible.
        /// </summary>
        public bool ShowOverlay
        {
            get { return _showOverlay; }
            internal set { AssignAndUpdateProperty(ShowOverlayPropertyName, value, ref _showOverlay); }
        }
        private bool _showOverlay;

        /// <summary>
        /// Gets the text to show in the semi-transparent overlay.
        /// </summary>
        public string OverlayText
        {
            get { return _overlayText; }
            internal set { AssignAndUpdateProperty(OverlayTextPropertyName, value, ref _overlayText); }
        }
        private string _overlayText;

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised upon completion of a save operation on the menu.
        /// </summary>
        public event System.EventHandler<MenuSaveCompleteEventArgs> MenuLayoutSaved;

        #endregion // Events

        /// <summary>
        /// Informs the menu layout that an update has started, and to suspend saving changes to the menu until FinishItemsUpdate has been called.
        /// </summary>
        public void StartItemsUpdate()
        {
            ++_updatingItems;
        }

        /// <summary>
        /// Indicates that an update has finished. As updates may nest, if this finishes the last menu update, the layout will be saved.
        /// </summary>
        /// <param name="save">If <c>true</c>, save changes to the menu layout.</param>
        public void FinishItemsUpdate(bool save)
        {
            --_updatingItems;
            if (_updatingItems < 0)
            {
                ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "MenuLayoutViewModel.FinishItemsUpdate", "MenuLayoutViewModel");
            }
            if (save && (_updatingItems == 0))
            {
                SaveMenuLayout(this);
            }
        }

        /// <summary>
        /// Highlight directories and files that have changed by comparing the device and host menu layouts to each other.
        /// </summary>
        /// <param name="deviceFileSystem">The device's file system.</param>
        /// <param name="mode">Which direction the differences should be computed.</param>
        internal void HighlightDifferencesFromDeviceFileSystem(FileSystem deviceFileSystem, MenuLayoutSynchronizationMode mode)
        {
            // first, clear all status
            ClearItemStates(LtoFlashViewModel.AttachedPeripherals);
            LtoFlashViewModel.SyncMode = mode;

            LfsDifferences differences = null;
            switch (LtoFlashViewModel.SyncMode)
            {
                case MenuLayoutSynchronizationMode.ToLtoFlash:
                    using (var comparer = RomComparer.GetComparer(RomComparison.Strict))
                    {
                        differences = MenuLayout.FileSystem.CompareTo(deviceFileSystem, LtoFlashViewModel.ActiveLtoFlashDevice.Device);
                        foreach (var dirToAdd in differences.DirectoryDifferences.ToAdd)
                        {
                            var viewModel = FindViewModelForModel((IFileContainer)dirToAdd);
                            viewModel.State = Core.Model.Program.ProgramSupportFileState.New;
                        }
                        foreach (var dirToUpdate in differences.DirectoryDifferences.ToUpdate)
                        {
                            var viewModel = FindViewModelForModel((IFileContainer)dirToUpdate);
                            viewModel.State = Core.Model.Program.ProgramSupportFileState.PresentButModified;
                        }
                        foreach (var fileToAdd in differences.FileDifferences.ToAdd)
                        {
                            UpdatePreviewState(fileToAdd, (v) => !(v is FolderViewModel), ProgramSupportFileState.New, differences.FileDifferences.FailedOperations, comparer);
                        }
                        foreach (var fileToUpdate in differences.FileDifferences.ToUpdate)
                        {
                            UpdatePreviewState(fileToUpdate, (v) => !(v is FolderViewModel) || (v.State == ProgramSupportFileState.None), ProgramSupportFileState.PresentButModified, differences.FileDifferences.FailedOperations, comparer);
                        }
                        if (differences.ForkDifferences.ToUpdate.Any())
                        {
                            var filesWithForksToUpdate = MenuLayout.FileSystem.GetAllFilesUsingForks(differences.ForkDifferences.ToUpdate);
                            foreach (var filesToUpdate in filesWithForksToUpdate.Values)
                            {
                                foreach (var fileToUpdate in filesToUpdate)
                                {
                                    UpdatePreviewState(fileToUpdate, (v) => !(v is FolderViewModel) && (v.State == ProgramSupportFileState.None), ProgramSupportFileState.PresentButModified, differences.FileDifferences.FailedOperations, comparer);
                                }
                            }
                        }
                    }
                    break;
                case MenuLayoutSynchronizationMode.FromLtoFlash:
                    differences = deviceFileSystem.CompareTo(MenuLayout.FileSystem);
                    foreach (var gdnToDelete in differences.DirectoryDifferences.ToDelete)
                    {
                        var dirToDelete = MenuLayout.FileSystem.Directories[(int)gdnToDelete];
                        var viewModel = FindViewModelForModel((IFileContainer)dirToDelete);
                        viewModel.State = Core.Model.Program.ProgramSupportFileState.Deleted;
                    }
                    foreach (var dirToUpdate in differences.DirectoryDifferences.ToUpdate)
                    {
                        var localModel = MenuLayout.FileSystem.Directories[dirToUpdate.GlobalDirectoryNumber];
                        var viewModel = FindViewModelForModel((IFileContainer)localModel);
                        viewModel.State = Core.Model.Program.ProgramSupportFileState.PresentButModified;
                    }
                    foreach (var gfnToDelete in differences.FileDifferences.ToDelete)
                    {
                        var fileToDelete = MenuLayout.FileSystem.Files[(int)gfnToDelete];
                        UpdatePreviewState(fileToDelete, (v) => !(v is FolderViewModel), ProgramSupportFileState.Deleted, null, null);
                    }
                    foreach (var fileToUpdate in differences.FileDifferences.ToUpdate)
                    {
                        var localModel = MenuLayout.FileSystem.Files[fileToUpdate.GlobalFileNumber];
                        UpdatePreviewState(localModel, (v) => !(v is FolderViewModel) || (v.State == ProgramSupportFileState.None), ProgramSupportFileState.PresentButModified, null, null);
                    }
                    if (differences.ForkDifferences.ToUpdate.Any())
                    {
                        var filesWithForksToUpdate = MenuLayout.FileSystem.GetAllFilesUsingForks(differences.ForkDifferences.ToUpdate);
                        foreach (var filesToUpdate in filesWithForksToUpdate.Values)
                        {
                            foreach (var fileToUpdate in filesToUpdate)
                            {
                                var localModel = MenuLayout.FileSystem.Files[fileToUpdate.GlobalFileNumber];
                                UpdatePreviewState(localModel, (v) => !(v is FolderViewModel) && (v.State == ProgramSupportFileState.None), ProgramSupportFileState.PresentButModified, null, null);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Creates a new directory (folder) in the menu layout based on current selection.
        /// </summary>
        internal void NewFolder()
        {
            var item = CurrentSelection as FileNodeViewModel;
            IFileContainer destination = GetTargetForFolderOperation(item, false);
            var newFolderName = string.Format(Resources.Strings.MenuLayout_DefaultDirectoryNameFormat, _nextNewFolderNumber++);
            var newFolder = MenuLayout.CreateFolder(newFolderName);
            var insertIndex = (item == null) ? -1 : destination.IndexOfChild(item.Model) + 1;
            bool added = false;
            if ((insertIndex < 0) || (insertIndex >= destination.Size))
            {
                added = destination.AddChild(newFolder, true);
            }
            else
            {
                destination.InsertChild(insertIndex, newFolder, true);
                added = true;
            }
            if (added)
            {
                if (insertIndex < 0)
                {
                    insertIndex = destination.Items.Count() - 1;
                }
                var folderViewModel = FindViewModelForModel(destination);
                var newFolderViewModel = folderViewModel.Items[insertIndex];
                newFolderViewModel.IsSelected = true;
                CurrentSelection = newFolderViewModel;
                ++RetainFocus;
            }
        }

        /// <summary>
        /// Determines whether it's possible to create a new directory.
        /// </summary>
        /// <param name="reasonForFailure">Receives a text description of the error if a problem occurs creating the directory.</param>
        /// <returns><c>true</c> if it is safe to create a new directory; otherwise, <c>false</c>.</returns>
        internal bool CanCreateNewDirectory(out string reasonForFailure)
        {
            IFileContainer destination = GetTargetForFolderOperation(CurrentSelection, false);
            bool canCreateFolder = FileSystemCanAcceptMoreItems(destination, LfsEntityType.Directory, 1, true, out reasonForFailure);
            return canCreateFolder;
        }

        /// <summary>
        /// Delete the currently selected items.
        /// </summary>
        internal void DeleteItems()
        {
            StartItemsUpdate();
            try
            {
                if ((SelectedItems != null) && SelectedItems.Any())
                {
                    var modelElementsToDelete = SelectedItems.Select(i => i.Model);
                    var foldersToDelete = modelElementsToDelete.OfType<Folder>().ToList();
                    var remainingItemsToDelete = modelElementsToDelete.Except(foldersToDelete).ToList();

                    // Delete folders first...
                    foreach (var folder in foldersToDelete)
                    {
                        var parent = folder.Parent;
                        parent.RemoveChild(folder, true);
                    }

                    // Then, files. (Is there some improvement we could have... such as skipping things whose parents
                    // have been deleted?
                    foreach (var item in remainingItemsToDelete)
                    {
                        var parent = item.Parent;
                        parent.RemoveChild(item, true);
                    }

                    ++RetainFocus;
                    _currentSelection = null;
                    CommandManager.InvalidateRequerySuggested();
                }
                else
                {
                    var itemToDelete = CurrentSelection as FileNodeViewModel;
                    if (itemToDelete != null)
                    {
                        var modelElementToDelete = itemToDelete.Model;
                        var deletedItemParentModel = modelElementToDelete.Parent;
                        var indexOfElementToHighlight = deletedItemParentModel.IndexOfChild(modelElementToDelete);
                        if (indexOfElementToHighlight == (deletedItemParentModel.Items.Count() - 1))
                        {
                            --indexOfElementToHighlight;
                        }
                        if (modelElementToDelete.Parent.RemoveChild(modelElementToDelete, true))
                        {
                            if (deletedItemParentModel.Items.Any())
                            {
                                var parentViewModel = FindViewModelForModel(deletedItemParentModel);
                                var itemToHighlight = parentViewModel.Items[indexOfElementToHighlight];
                                itemToHighlight.IsSelected = true;
#if MAC
                                CurrentSelection = itemToHighlight;
#endif
                            }
                            ++RetainFocus;
                            if (!deletedItemParentModel.Items.Any())
                            {
                                _currentSelection = null;
                            }
                            CommandManager.InvalidateRequerySuggested();
                        }
                    }
                }
            }
            finally
            {
                FinishItemsUpdate(true);
            }
        }

        /// <summary>
        /// Determines whether it is safe to delete the currently selected items from the menu.
        /// </summary>
        /// <returns><c>true</c> if it is safe to delete items; otherwise, <c>false</c>.</returns>
        internal bool CanDeleteItems()
        {
            return (CurrentSelection != null) && (CurrentSelection is FileNodeViewModel);
        }

        /// <summary>
        /// Determines whether the selected items can be added to the menu
        /// </summary>
        /// <returns><c>true</c> if the selected items can be added; otherwise, <c>false</c>.</returns>
        /// <param name="parameter">The items to be checked to see if it's safe to add.</param>
        internal bool CanAddSelectedItems(object parameter)
        {
            var items = parameter as ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescriptionViewModel>;
            var targetForAddingIems = GetTargetForFolderOperation(CurrentSelection, true);
            var canAddItems = (items != null) && CanAcceptFiles(targetForAddingIems, items.Select(vm => vm.Model), null);
            return canAddItems;
        }

        /// <summary>
        /// Add the selected items to the current location in the menu.
        /// </summary>
        /// <param name="parameter">The items to add.</param>
        internal void AddSelectedItems(object parameter)
        {
            var items = parameter as ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescriptionViewModel>;
            IFileContainer destination = null;
            int insertIndex = -1;
            if (CurrentSelection != null)
            {
                destination = CurrentSelection.Model as IFileContainer;
                if (destination == null)
                {
                    destination = CurrentSelection.Parent;
                    insertIndex = destination.IndexOfChild(CurrentSelection.Model) + 1;
                    if (insertIndex >= destination.Size)
                    {
                        insertIndex = -1;
                    }
                }
            }
            if (destination == null)
            {
                destination = Root.Model as IFileContainer;
            }
            AddItems(this, destination, items.Select(p => p.Model), insertIndex); // Saves the menu upon completion.
        }

        /// <inheritdoc />
        protected override void OnContentsChanged(object sender, EventArgs e)
        {
            ClearItemStates(LtoFlashViewModel.AttachedPeripherals);
            base.OnContentsChanged(sender, e);
            SaveMenuLayout(this);
        }

        private static bool IsFileSystemEntryInFailureList(FileNodeViewModel viewModel, IDictionary<string, FailedOperationException> failures, RomComparer comparer)
        {
            var programViewModel = viewModel as ProgramViewModel;
            var romToCheck = programViewModel == null ? null : programViewModel.ProgramDescription.Rom;
            var isInFailureList = (failures != null) && (romToCheck != null) && failures.FirstOrDefault(f => (f.Value is IncompatibleRomException) && ((IncompatibleRomException)f.Value).Rom.IsEquivalentTo(romToCheck, comparer)).Value != null;
            return isInFailureList;
        }

        private static MenuLayout LoadMenuLayout(object parameter)
        {
            var filePath = parameter as string;
            MenuLayout menuLayout = null;
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    menuLayout = MenuLayout.Load(filePath);
                }
                catch (Exception e)
                {
                    // BeginInvoke to give the app a chance to finish launching.
                    var backupCopyPath = filePath.GetUniqueBackupFilePath();
                    var badFile = System.IO.Path.ChangeExtension(backupCopyPath, ".bad");
                    var errorDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.MenuLayout_LoadFailed_Title, Resources.Strings.MenuLayout_LoadFailed_Message);
                    errorDialog.ReportText = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.MenuLayout_LoadFailed_Error_Format, backupCopyPath, e);
                    errorDialog.BeginInvokeDialog(
                        Resources.Strings.MenuLayout_LoadFailed_BackupButton_Text,
                        new Action<bool?>((result) =>
                        {
                            if (result.HasValue && result.Value)
                            {
                                try
                                {
                                    System.IO.File.Copy(filePath, backupCopyPath);
                                }
                                catch (Exception x)
                                {
                                    var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.MenuLayout_LoadFailed_BackupFailed_Message_Format, x.Message);
                                    OSMessageBox.Show(message, Resources.Strings.MenuLayout_LoadFailed_Title, x, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                                }
                            }

                            // Move the file out of the way -- it's bad.
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Move(filePath, badFile);
                            }
                        }));
                }
            }
            if (menuLayout == null)
            {
                menuLayout = new MenuLayout();
            }
            return menuLayout;
        }

        private static void SaveMenuLayout(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (menuLayoutViewModel._updatingItems == 0)
            {
                menuLayoutViewModel.MenuLayout.Save(menuLayoutViewModel.FilePath);
            }
        }

        private void UpdatePreviewState(ILfsFileInfo file, Predicate<FileNodeViewModel> updateCondition, ProgramSupportFileState defaultState, IDictionary<string, FailedOperationException> failures, RomComparer comparer)
        {
            var viewModel = FindViewModelForModel((IFile)file);
            if ((viewModel != null) && updateCondition(viewModel))
            {
                var isIncompatible = IsFileSystemEntryInFailureList(viewModel, failures, comparer);
                viewModel.State = isIncompatible ? Core.Model.Program.ProgramSupportFileState.RequiredPeripheralIncompatible : defaultState;
                if (isIncompatible)
                {
                    ((ProgramViewModel)viewModel).RefreshTipStrip();
                }
            }
        }

        private void HandleMenuLayoutSaved(object sender, MenuSaveCompleteEventArgs e)
        {
            var menuLayoutSaved = MenuLayoutSaved;
            if (menuLayoutSaved != null)
            {
                menuLayoutSaved(this, e);
            }
        }

        private void PreviewDragEnter(object dragEventArgs)
        {
#if WIN
            var dragArgs = dragEventArgs as System.Windows.DragEventArgs;
            if (!dragArgs.Handled)
            {
                if (AcceptDragData(dragArgs))
                {
                    if (!dragArgs.Data.GetDataPresent(DragMenuLayoutDataFormat))
                    {
                        dragArgs.Data.SetData(DragMenuLayoutDataFormat, this);
                    }
                }
            }
#elif MAC
            ErrorReporting.ReportNotImplementedError("FileNodeFileModel.DragManualEnter");
#endif
        }

        private void SetColor(object newColor)
        {
            CurrentSelection.Color = newColor as FileNodeColorViewModel;
        }

        private bool CanSetColor(object newColor)
        {
            return CurrentSelection != null;
        }

        private IFileContainer GetTargetForFolderOperation(FileNodeViewModel currentSelection, bool insertIntoSelection)
        {
            IFileContainer destination = (Root == null) ? null : Root.Model as IFileContainer;
            if (currentSelection != null)
            {
                // On Mac, when deleting the *last* item in a menu layout, we're not getting a selection changed
                // notification. As a result, we temporarily have a stale current selection, whose Model
                // is set to null. If this happens, just revert to the root.
                destination = currentSelection.Model as IFileContainer;
                if ((!insertIntoSelection || (destination == null)) && (currentSelection.Model != null))
                {
                    destination = currentSelection.Parent;
                }
                else
                {
                    destination = Root.Model as IFileContainer;
                }
            }
            return destination;
        }

        private void UpdateSelectionRelatedProperties(FileNodeViewModel newlySelectedItem)
        {
            DeleteSelectedItemIcon = (newlySelectedItem is IFileContainer) ? DeleteFolderIcon : DeleteFileIcon;
            DeleteSelectedItemTip = ((newlySelectedItem == null) || string.IsNullOrWhiteSpace(newlySelectedItem.LongName)) ? null : string.Format(Resources.Strings.MenuLayout_DeleteTipFormat, newlySelectedItem.LongName);
            RaisePropertyChanged("IsColorEditable");
        }

        private void FileSystemContentsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSystemContentsUsage(sender as INTV.Shared.Utility.IFixedSizeCollection);
        }

        private void UpdateOverlay(string overlayText, bool showOverlay)
        {
            if (!string.IsNullOrEmpty(overlayText))
            {
                OverlayText = overlayText;
            }
            ShowOverlay = showOverlay;
        }

        private void UpdateSystemContentsUsage(INTV.Shared.Utility.IFixedSizeCollection collection)
        {
            bool isFolderTable = collection is GlobalDirectoryTable;
            bool isFileTable = collection is GlobalFileTable;
            bool isForkTable = collection is GlobalForkTable;
            string formatString = null;
            if (isFolderTable)
            {
                formatString = Resources.Strings.MenuLayout_RemainingFoldersFormat;
            }
            else if (isFileTable)
            {
                formatString = Resources.Strings.MenuLayout_RemainingFilesFormat;
            }
            else if (isForkTable)
            {
                formatString = Resources.Strings.MenuLayout_RemainingForksFormat;
                EstimatedStorageUsed = ((GlobalForkTable)collection).EstimatedStorageRequired;
                SpaceUsedRatio = (double)EstimatedStorageUsed / Device.TotalFlashStorageSpace;
                var estimatedStorageUsedMB = (double)EstimatedStorageUsed / 0x100000;
                EstimatedStorageUsedMB = string.Format(CultureInfo.CurrentCulture, Resources.Strings.MenuLayout_EstimatedUsageMBFormat, estimatedStorageUsedMB, Device.TotalFlashStorageSpace / 0x100000);
            }
            var numEntriesUsed = collection.ItemsInUse;
            var totalNumEntries = collection.Size;
            var numEntriesRemaining = collection.ItemsRemaining;
            var inUseRatio = (double)numEntriesUsed / totalNumEntries;
            var status = string.Format(System.Globalization.CultureInfo.CurrentCulture, formatString, numEntriesUsed, totalNumEntries, numEntriesRemaining);
            if (isFolderTable)
            {
                FolderCount = status;
                DirectoriesUsedRatio = inUseRatio;
            }
            else if (isFileTable)
            {
                UpdateOverlay(Resources.Strings.MenuLayout_NoItems, !(collection.ItemsInUse > 1));
                FileCount = status;
                FilesUsedRatio = inUseRatio;
            }
            else if (isForkTable)
            {
                ForkCount = status;
                ForksUsedRatio = inUseRatio;
            }
            OverallInUseRatio = Math.Max(Math.Max(Math.Max(DirectoriesUsedRatio, FilesUsedRatio), ForksUsedRatio), SpaceUsedRatio);
            var overallUsageDetails = new System.Text.StringBuilder();
            overallUsageDetails.AppendFormat("{0} ({1:P2})", EstimatedStorageUsedMB, SpaceUsedRatio).AppendLine();
            overallUsageDetails.AppendFormat("{0} ({1:P2})", FolderCount, DirectoriesUsedRatio).AppendLine();
            overallUsageDetails.AppendFormat("{0} ({1:P2})", FileCount, FilesUsedRatio).AppendLine();
            overallUsageDetails.AppendFormat("{0} ({1:P2})", ForkCount, ForksUsedRatio);
            OverallUsageDetails = overallUsageDetails.ToString();
        }

        /// <summary>
        /// Platform-specific initialization.
        /// </summary>
        partial void Initialize();
    }
}

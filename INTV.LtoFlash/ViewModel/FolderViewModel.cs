// <copyright file="FolderViewModel.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

using IntvColor = INTV.Core.Model.Stic.Color;

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
    /// ViewModel for a Folder.
    /// </summary>
    public partial class FolderViewModel : FileNodeViewModel, IFileContainer
    {
        #region Property Names

        public const string StatusPropertyName = "Status";

        #endregion // Property Names

        private static Dictionary<Tuple<IntvColor, ProgramSupportFileState>, OSImage> _openFolderIcons = new Dictionary<Tuple<IntvColor, ProgramSupportFileState>, OSImage>();
        private static Dictionary<Tuple<IntvColor, ProgramSupportFileState>, OSImage> _closedFolderIcons = new Dictionary<Tuple<IntvColor, ProgramSupportFileState>, OSImage>();

        private ObservableViewModelCollection<FileNodeViewModel, IFile> _items;
        private bool _isOpen;
        private OSImage _icon;
        private string _status;

        #region Constructors

        /// <summary>
        /// Initializes an instance of a FolderViewModel.
        /// </summary>
        public FolderViewModel()
        {
            _icon = GetIconForColor(Color.IntvColor);
        }

        /// <summary>
        /// Initializes an instance of a FolderViewModel.
        /// </summary>
        /// <param name="folder">The folder model object.</param>
        public FolderViewModel(IFileContainer folder)
            : this()
        {
            Model = folder;
        }

        #endregion // Constructors

        #region IFileContainer Events

        /// <inheritdoc />
        public event EventHandler<EventArgs> ContentsChanged;

        #endregion // IFileContainer Events

        #region Properties

        /// <summary>
        /// Gets a value indicating the status of the folder. The status typically displays how full the folder is.
        /// </summary>
        [OSExport("Status")]
        public string Status
        {
            get { return _status; }
            private set { AssignAndUpdateProperty(StatusPropertyName, value, ref _status); }
        }

        #region IFileContainer Properties

        /// <inheritdoc />
        IEnumerable<IFile> IFileContainer.Items
        {
            get { return Folder.Items; }
        }

        /// <inheritdoc />
        public int Size
        {
            get { return Folder.Size; }
        }

        #endregion // IFileContainer Properties

        #region FileNodeViewModel Properties

        /// <inheritdoc />
        public sealed override OSImage Icon
        {
            get { return _icon; }
            protected set { AssignAndUpdateProperty("Icon", value, ref _icon, (p, v) => RaisePropertyChanged(IconTipStripPropertyName)); }
        }

        /// <inheritdoc />
        public override bool IsOpen
        {
            get
            {
                return _isOpen;
            }

            set
            {
                if (AssignAndUpdateProperty("IsOpen", value, ref _isOpen))
                {
                    // already updated icon state, use current color
                    Icon = GetIconForColor(Color.IntvColor);
                }
            }
        }

        /// <inheritdoc />
        public override string IconTipStrip
        {
            get
            {
                string tip = null;
                switch (GetState())
                {
                    case ProgramSupportFileState.New:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tip = Resources.Strings.MenuLayout_WillAddDirectory_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.Missing:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tip = Resources.Strings.MenuLayout_MissingItem_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.PresentButModified:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tip = Resources.Strings.MenuLayout_WillUpdateDirectory_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.Deleted:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tip = Resources.Strings.MenuLayout_WillDeleteDirectory_Tip;
                                break;
                        }
                        break;
                }
                return tip;
            }
        }

        /// <inheritdoc />
        public override bool HasSupportFiles
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override string Manual
        {
            get { return null; }
            set { }
        }

        /// <inheritdoc />
        public override string SaveData
        {
            get { return null; }
            set { }
        }

        /// <inheritdoc />
        public override ObservableViewModelCollection<FileNodeViewModel, IFile> Items
        {
            get { return _items; }
        }

        /// <inheritdoc />
        public sealed override IFile Model
        {
            get
            {
                return base.Model;
            }

            set
            {
                if (base.Model != value)
                {
                    var folder = value as Folder;
                    var currentFolder = base.Model as Folder;
                    if (currentFolder != null)
                    {
                        currentFolder.ContentsChanged -= OnContentsChanged;
                        currentFolder.Files.CollectionChanged -= FilesCollectionChanged;
                    }
                    var oldItems = _items;
                    if (folder != null)
                    {
                        _items = new ObservableViewModelCollection<FileNodeViewModel, IFile>(folder.Files, FileNodeViewModel.Factory, ViewModelRemoved);
                    }
                    else
                    {
                        _items = new ObservableViewModelCollection<FileNodeViewModel, IFile>(FileNodeViewModel.Factory, ViewModelRemoved);
                    }
                    base.Model = value;
                    if (folder != null)
                    {
                        OnColorChanged(folder.Color);
                        FilesCollectionChanged(null, null);
                        folder.ContentsChanged += OnContentsChanged;
                        folder.Files.CollectionChanged += FilesCollectionChanged;
                    }
                    ReplacedItemsCollection(oldItems);
                }
            }
        }

        #endregion // FileNodeViewModel Properties

        private IFileContainer Folder
        {
            get { return base.Model as IFileContainer; }
        }

        #endregion // Properties

        /// <summary>
        /// Marks all items that refer to the given description as missing their ROM fork -- and therefore are to
        /// be ignored during sync operations.
        /// </summary>
        /// <param name="description">The program description to mark as missing.</param>
        /// <param name="peripherals">The peripherals attached to the system, used for compatibility checks.</param>
        public void MarkProgramMissing(IProgramDescription description, IEnumerable<IPeripheral> peripherals)
        {
            RefreshProgramAvailabilityState(description, peripherals, ProgramSupportFileState.Missing);
        }

        /// <summary>
        /// Marks all items that refer to the given description as available.
        /// </summary>
        /// <param name="description">The program description to mark as available.</param>
        /// <param name="peripherals">The peripherals attached to the system, used for compatibility checks.</param>
        public void MarkProgramAvailable(IProgramDescription description, IEnumerable<IPeripheral> peripherals)
        {
            RefreshProgramAvailabilityState(description, peripherals, ProgramSupportFileState.None);
        }

        #region IFileContainer Methods

        /// <inheritdoc />
        public bool AddChild(IFile child, bool updateFileSystemTables)
        {
            return Folder.AddChild(child, updateFileSystemTables);
        }

        /// <inheritdoc />
        public void InsertChild(int index, IFile child, bool updateFileSystemTables)
        {
            Folder.InsertChild(index, child, updateFileSystemTables);
        }

        /// <inheritdoc />
        public bool MoveChildToNewParent(IFile child, IFileContainer newParent, bool updateFileSystemTables)
        {
            return Folder.MoveChildToNewParent(child, newParent, updateFileSystemTables);
        }

        /// <inheritdoc />
        public bool MoveChildToNewParent(IFile child, IFileContainer newParent, int locationInNewParent, bool updateFileSystemTables)
        {
            return Folder.MoveChildToNewParent(child, newParent, locationInNewParent, updateFileSystemTables);
        }

        /// <inheritdoc />
        public bool RemoveChild(IFile child, bool updateFileSystemTables)
        {
            return Folder.RemoveChild(child, updateFileSystemTables);
        }

        /// <inheritdoc />
        public void RemoveChildFromHierarchy(Predicate<IFile> condition)
        {
            Folder.RemoveChildFromHierarchy(condition);
        }

        /// <inheritdoc />
        public bool IsParentOfChild(IFile child)
        {
            return Folder.IsParentOfChild(child);
        }

        /// <inheritdoc />
        public bool ContainsChild(IFile child)
        {
            return Folder.ContainsChild(child);
        }

        /// <inheritdoc />
        public int IndexOfChild(IFile child)
        {
            return Folder.IndexOfChild(child);
        }

        /// <inheritdoc />
        public IEnumerable<IFile> FindChildren(Predicate<IFile> filter, bool recurse)
        {
            var matches = new List<IFile>();
            FindChildren(filter, recurse, matches);
            return matches;
        }

        #endregion // IFileContainer Methods

        /// <summary>
        /// Find the FolderViewModel for a given FolderModel.
        /// </summary>
        /// <param name="folderModel">The IFileContainer model for which to locate a ViewModel.</param>
        /// <returns>The corresponding ViewModel for the given model object, or <c>null</c> if not found.</returns>
        internal FolderViewModel FindViewModelForModel(IFileContainer folderModel)
        {
            var folderViewModel = (Model == folderModel) ? this : null;
            if (folderViewModel == null)
            {
                folderViewModel = Items.FirstOrDefault(i => i.Model == folderModel) as FolderViewModel;
                if (folderViewModel == null)
                {
                    foreach (var folder in Items.OfType<FolderViewModel>())
                    {
                        folderViewModel = folder.FindViewModelForModel(folderModel);
                        if (folderViewModel != null)
                        {
                            break;
                        }
                    }
                }
            }
            return folderViewModel;
        }

        /// <summary>
        /// Find the FileNodeViewModel for a given FileModel.
        /// </summary>
        /// <param name="fileModel">The IFile model for which to locate a ViewModel.</param>
        /// <returns>The corresponding ViewModel for the given model object, or <c>null</c> if not found.</returns>
        internal FileNodeViewModel FindViewModelForModel(IFile fileModel)
        {
            var fileNodeViewModel = (Model == fileModel) ? (FileNodeViewModel)this : null;
            if (fileNodeViewModel == null)
            {
                fileNodeViewModel = Items.FirstOrDefault(i => i.Model == fileModel);
                if (fileNodeViewModel == null)
                {
                    foreach (var folder in Items.OfType<FolderViewModel>())
                    {
                        fileNodeViewModel = folder.FindViewModelForModel(fileModel);
                        if (fileNodeViewModel != null)
                        {
                            break;
                        }
                    }
                }
            }
            return fileNodeViewModel;
        }

        /// <summary>
        /// Sorts the items in the folder by long name.
        /// </summary>
        /// <param name="ascending">If set to <c>true</c> sorts in ascending order, otherwise in descending order.</param>
        internal void SortByName(bool ascending)
        {
#if REPORT_PERFORMANCE
            System.Diagnostics.Debug.WriteLine("SORT: " + LongName + " START");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
            var folder = Model as Folder;
            if (folder != null)
            {
                folder.SortByName(ascending);
            }
            else
            {
                // throw?
            }
#if REPORT_PERFORMANCE
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine("SORT: " + LongName + " FINISH: Duration: " + stopwatch.Elapsed);
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Clear all item states, so the will no longer display any 'dirty' glyph.
        /// </summary>
        /// <param name="peripherals">The peripherals attached to the system, used for compatibility checks.</param>
        internal virtual void ClearItemStates(IEnumerable<IPeripheral> peripherals)
        {
            LtoFlashViewModel.SyncMode = MenuLayoutSynchronizationMode.RomList;
            foreach (var item in Items)
            {
                var program = item as ProgramViewModel;
                if (program != null)
                {
                    program.RefreshValidationState(peripherals);
                }
                else
                {
                    item.State = Core.Model.Program.ProgramSupportFileState.None;
                }
                var folder = item as FolderViewModel;
                if (folder != null)
                {
                    folder.ClearItemStates(peripherals);
                }
            }
        }

        /// <inheritdoc />
        internal override sealed OSImage GetIconForColor(INTV.Core.Model.Stic.Color color)
        {
            OSImage icon = null;
            var key = new Tuple<IntvColor, ProgramSupportFileState>(color, GetState());
            var icons = IsOpen ? _openFolderIcons : _closedFolderIcons;
            if (!icons.TryGetValue(key, out icon))
            {
                icon = ResourceHelpers.LoadImageResource(this.GetType(), MakeIconResourceName(IsOpen, color));
                icons[key] = icon;
            }
            return icon;
        }

        /// <inheritdoc />
        protected override void OnColorChanged(INTV.Core.Model.Stic.Color newColor)
        {
            var iconForColor = GetIconForColor(newColor);
            if (Icon != iconForColor)
            {
                Icon = iconForColor;
            }
        }

        /// <summary>
        /// Event handler for the contents of the folder changing.
        /// </summary>
        /// <param name="sender">The folder whose contents changed.</param>
        /// <param name="e">Arguments (unused).</param>
        /// <remarks>Should rename this to HandleContentsChanged.</remarks>
        protected virtual void OnContentsChanged(object sender, EventArgs e)
        {
            var contentsChanged = ContentsChanged;
            if (contentsChanged != null)
            {
                contentsChanged(this, e);
            }
        }

        private static void ViewModelRemoved(FileNodeViewModel viewModel)
        {
            viewModel.Model = null;
        }

        private void FindChildren(Predicate<IFile> filter, bool recurse, List<IFile> matches)
        {
            matches.AddRange(Items.Where(i => filter(i)));
            if (recurse)
            {
                var folders = Items.OfType<FolderViewModel>();
                foreach (var folder in folders)
                {
                    folder.FindChildren(filter, recurse, matches);
                }
            }
        }

        private void RefreshProgramAvailabilityState(IProgramDescription description, IEnumerable<IPeripheral> peripherals, ProgramSupportFileState newState)
        {
            using (var comparer = RomComparer.GetComparer(RomComparer.DefaultCompareMode))
            {
                // Should we be using the comparer here?
                foreach (var program in Items.OfType<ProgramViewModel>().Where(p => (p.ProgramDescription.Crc == description.Crc) && (p.ProgramDescription.Rom.RomPath == description.Rom.RomPath)))
                {
                    program.RefreshValidationState(peripherals);
                }
            }
            foreach (var directory in Items.OfType<FolderViewModel>())
            {
                directory.RefreshProgramAvailabilityState(description, peripherals, newState);
            }
        }

        private void FilesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Status = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.MenuLayout_FolderStatusMessageFormat, Items.Count, FileSystemConstants.MaxItemCount);
        }

        private ProgramSupportFileState GetState()
        {
            var state = State;
            switch (LtoFlashViewModel.SyncMode)
            {
                case MenuLayoutSynchronizationMode.None:
                case MenuLayoutSynchronizationMode.RomList:
                    state = ProgramSupportFileState.None;
                    break;
            }
            return state;
        }

        private string MakeIconResourceName(bool isOpen, IntvColor color)
        {
            var iconString = string.Empty;
            switch (LtoFlashViewModel.SyncMode)
            {
                case MenuLayoutSynchronizationMode.ToLtoFlash:
                case MenuLayoutSynchronizationMode.FromLtoFlash:
                    switch (GetState())
                    {
                        case ProgramSupportFileState.PresentButModified:
                            iconString += "modified_";
                            break;
                        case ProgramSupportFileState.Missing:
                            iconString += "missing_";
                            break;
                        case ProgramSupportFileState.New:
                            iconString += "added_";
                            break;
                        case ProgramSupportFileState.Deleted:
                            iconString += "deleted_";
                            break;
                    }
                    break;
            }
            iconString += isOpen ? "Open_" : "Closed_";
            if ((color != IntvColor.NotAColor) && MenuLayoutViewModel.Colors.Contains(color))
            {
                iconString += color.ToString() + "_";
            }
            return this.CreatePackedResourceString("Resources/Images/folder_" + iconString + "16xLG.png");
        }

        /// <summary>
        /// Platform-specific code to execute when items in the collection are replaced.
        /// </summary>
        /// <param name="oldItems">Items that were replaced.</param>
        partial void ReplacedItemsCollection(ObservableViewModelCollection<FileNodeViewModel, IFile> oldItems);

        /// <summary>
        /// Platform-specific status update method.
        /// </summary>
        /// <param name="newStatus">New status string.</param>
        partial void UpdateStatus(string newStatus);
    }
}

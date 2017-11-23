// <copyright file="RomListViewModel.cs" company="INTV Funhouse">
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

////#define ENABLE_ROMS_PATCH

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the main ROM list.
    /// </summary>
    ////[System.ComponentModel.Composition.Export(typeof(INTV.Shared.ComponentModel.IPrimaryComponent))]
    public partial class RomListViewModel : ViewModelBase, System.Collections.Specialized.INotifyCollectionChanged, IPrimaryComponent
    {
        /// <summary>
        /// Identifier for this component.
        /// </summary>
        public const string ComponentId = "INTV.Shared.RomList";

        #region Property Names

        public const string SortColumnPropertyName = "SortColumn";
        public const string SortDirectionPropertyName = "SortDirection";
        public const string CurrentSelectionPropertyName = "CurrentSelection";

        #endregion // Property Names

        #region UI Strings

        public static readonly string SelectManualFilter = Resources.Strings.FileDialog_SelectManualFilter;
        public static readonly string SelectManualPromptFormat = Resources.Strings.FileDialog_SelectManualPromptFormat;
        public static readonly string SelectJlpSaveDataFilter = Resources.Strings.FileDialog_SelectJlpDataFilter;
        public static readonly string SelectJlpSavePromptFormat = Resources.Strings.FileDialog_SelectJlpDataPromptFormat;

        public static readonly string InvalidManualTitle = Resources.Strings.RomListViewModel_InvalidManualMessageTitle;
        public static readonly string InvalidManualMessage = Resources.Strings.RomListViewModel_InvalidManualMessage;

        public static readonly string InvalidSaveDataTitle = Resources.Strings.RomListViewModel_InvalidSaveDataMessageTitle;
        public static readonly string InvalidSaveDataMessage = Resources.Strings.RomListViewModel_InvalidSaveDataMessage;

        /// <summary>
        /// String for header of Title column in table.
        /// </summary>
        public static readonly string TitleHeader = Resources.Strings.RomListViewModel_GridViewColumn_TitleHeader;

        /// <summary>
        /// String for header of Company column in table.
        /// </summary>
        public static readonly string CompanyHeader = Resources.Strings.RomListViewModel_GridViewColumn_VendorHeader;

        /// <summary>
        /// String for header of Year column in table.
        /// </summary>
        public static readonly string YearHeader = Resources.Strings.RomListViewModel_GridViewColumn_YearHeader;

        /// <summary>
        /// String for header of Features column in table.
        /// </summary>
        public static readonly string FeaturesHeader = Resources.Strings.RomListViewModel_GridViewColumn_FeaturesHeader;

        /// <summary>
        /// String for header of ROM file name column in table.
        /// </summary>
        public static readonly string RomFileHeader = Resources.Strings.RomListViewModel_GridViewColumn_RomFileHeader;

        /// <summary>
        /// Text to display when no content exists in the table.
        /// </summary>
        public static readonly string DropFilesHere = Resources.Strings.RomListViewModel_DropFilesHere;

        public static readonly string NumItemsFormat = Resources.Strings.RomListViewModel_NumItemsFormat;
        public static readonly string NumItemsSelectedFormat = Resources.Strings.RomListViewModel_NumItemsSelectedFormat;

        #endregion // UI Strings

        private SearchDirectories _searchDirectories;
        private List<System.WeakReference> _peripherals;

#if ENABLE_ROMS_PATCH
        private FixJlpAndOtherFlags _fixRomList;
        private FixCfgFilesPatch _fixCfgFiles;
#endif // ENABLE_ROMS_PATCH

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RomListViewModel.
        /// </summary>
        public RomListViewModel()
        {
            _peripherals = new List<System.WeakReference>();
            CurrentSelection = new ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescriptionViewModel>(NoOpFactory, null);
            CurrentSelection.CollectionChanged += HandleCurrentSelectionChanged;
            OSInitialize();
            CompositionHelpers.Container.ComposeExportedValue<IPrimaryComponent>(this);
            CompositionHelpers.Container.ComposeExportedValue<RomListViewModel>(this);
#if ENABLE_ROMS_PATCH
            _fixRomList = new FixJlpAndOtherFlags(this);
            _fixRomList.Register();
            _fixCfgFiles = new FixCfgFilesPatch(this);
            _fixCfgFiles.Register();
#endif // ENABLE_ROMS_PATCH
            Peripheral.PeripheralAttached += HandlePeripheralAttached;
            Peripheral.PeripheralDetached += HandlePeripheralDetached;
        }

        #endregion // Constructors

        #region Events

        /// <summary>
        /// Raised when the ROM collection changes.
        /// </summary>
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Gets or sets the file save path for the ROM list.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { AssignAndUpdateProperty("FilePath", value, ref _filePath); }
        }
        private string _filePath;

        /// <summary>
        /// Gets or sets programs to list in the table.
        /// </summary>
        public ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescription> Programs
        {
            get { return _programs; }
            set { AssignAndUpdateProperty("Programs", value, ref _programs); }
        }
        private ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescription> _programs;

        /// <summary>
        /// Gets a bindable enumerable of the currently selected items in the list.
        /// </summary>
        public ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescriptionViewModel> CurrentSelection { get; private set; }

        /// <summary>
        /// Gets or sets the hash of the last deleted item.
        /// </summary>
        public int RetainFocus
        {
            get { return _forceRetainFocus; }
            set { AssignAndUpdateProperty("RetainFocus", value, ref _forceRetainFocus); }
        }
        private int _forceRetainFocus;

        /// <summary>
        /// Gets or sets a value indicating whether ROMs in the list are editable.
        /// </summary>
        public bool CanEditElements
        {
            get { return Model.CanEditElements; }
            set { Model.CanEditElements = value; }
        }

        #region IPrimaryComponent Properties

        /// <inheritdoc/>
        public string UniqueId
        {
            get { return ComponentId; }
        }

        #endregion // IPrimaryComponent Properties

        #region Commands

        /// <summary>
        /// Gets the command to execute when files are dropped into the table.
        /// </summary>
        /// <remarks>TODO: REMOVE THIS! Make it part of the new command system.</remarks>
        public RelayCommand DropFilesCommand
        {
            get { return new RelayCommand(FilesDropped); }
        }

        #endregion // Commands

        /// <summary>
        /// Gets the model for which this is a ViewModel.
        /// </summary>
        public ProgramCollection Model
        {
            get { return _programDescriptions; }
            private set { AssignAndUpdateProperty("Model", value, ref _programDescriptions, (s, p) => _programs = new ObservableViewModelCollection<ProgramDescriptionViewModel, ProgramDescription>(p, Factory, null)); }
        }
        private ProgramCollection _programDescriptions;

        /// <summary>
        /// Gets the attached peripherals.
        /// </summary>
        internal IEnumerable<IPeripheral> AttachedPeripherals
        {
            get
            {
                lock (_peripherals)
                {
                    var peripherals = new List<IPeripheral>();
                    _peripherals.RemoveAll(p => !p.IsAlive);
                    foreach (var weakPeripheralReference in _peripherals)
                    {
                        var peripheral = weakPeripheralReference.Target as IPeripheral;
                        if (weakPeripheralReference.IsAlive && (peripheral != null))
                        {
                            peripherals.Add(peripheral);
                        }
                    }
                    return peripherals;
                }
            }
        }

        #endregion // Properties

        #region IPrimaryComponent

        /// <inheritdoc />
        public void Initialize()
        {
            // TODO: How to deal with Alternates?
            foreach (var program in Programs)
            {
                uint cfgCrc;
                Core.Model.Rom.GetRefreshedCrcs(program.Model.Files.RomImagePath, program.Model.Files.RomConfigurationFilePath, out cfgCrc);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ComponentVisual> GetVisuals()
        {
            // TODO: This is wrong... ViewModel should not create the visual - it should be the other way around
            if (_visual == null)
            {
                var view = OSInitializeVisual().AsType<RomListView>();
                _visual = new System.WeakReference(view);
            }
            RomListView romListView = _visual.IsAlive ? _visual.Target as RomListView : null; // ugh... .NET 4.0 doesn't have better Generic version of WeakReference<T> :(
            var componentVisual = new ComponentVisual(RomListView.Id, romListView, Resources.Strings.RomListSettingsPage_Title);
            yield return componentVisual;
        }
        private System.WeakReference _visual;

        #endregion // IPrimaryComponent

        /// <summary>
        /// Initialize the ROM list from the contents of a file.
        /// </summary>
        /// <param name="filePath">Absolute path to a file containing a serialized ProgramCollection.</param>
        public void InitializeRomList(string filePath)
        {
            Model.Initialize(filePath);
        }

        /// <summary>
        /// Save the ROM list.
        /// </summary>
        /// <param name="handleErrors">If set to <c>true</c> handle errors if an error handler is installed.</param>
        /// <remarks>Most usages should pass <c>true</c> to this function. Some invocations, typically those made from
        /// asynchronous tasks, may choose to pass <c>false</c> in order to customize the error handling process.
        /// Note that in such situations, it will not be possible to restore the backup copy of the list, generally.</remarks>
        public void SaveRomList(bool handleErrors)
        {
            Model.Save(FilePath, handleErrors);
        }

        /// <summary>
        /// Clears the ROM list.
        /// </summary>
        public void ClearRomList()
        {
            Model.Clear();
        }

        private static ProgramCollection LoadRomList(object parameter)
        {
            var romListFile = parameter as string;
            ProgramCollection.InitializeFromFile(romListFile);
            var programs = ProgramCollection.Roms;
            return programs;
        }

        private static ProgramDescriptionViewModel Factory(ProgramDescription programDescription)
        {
            return new ProgramDescriptionViewModel(programDescription);
        }

        private static ProgramDescriptionViewModel NoOpFactory(ProgramDescriptionViewModel programDescriptionViewModel)
        {
            return programDescriptionViewModel;
        }

        /// <summary>
        /// Prompt to add ROMs to the ROM list.
        /// </summary>
        /// <param name="parameter">If a non-<c>null</c> string that can be parsed as a Boolean value,
        /// the Boolean value indicates whether to prompt for files (false) or folders (true).</param>
        internal void AddRoms(object parameter)
        {
            var fileBrowser = FileDialogHelpers.Create();
            bool isFolderBrowser = false;
            if (parameter is bool)
            {
                isFolderBrowser = (bool)parameter;
            }
            else if (!bool.TryParse(parameter as string, out isFolderBrowser))
            {
                isFolderBrowser = false;
            }
            fileBrowser.IsFolderBrowser = isFolderBrowser;
            if (!fileBrowser.IsFolderBrowser)
            {
                fileBrowser.AddFilter(Resources.Strings.FileDialog_SelectRomFilesFilter, ProgramFileKind.Rom.FileExtensions());
#if !MAC
                fileBrowser.AddFilter(FileDialogHelpers.AllFilesFilter, new string[] { ".*" });
#endif // !MAC
            }
            fileBrowser.Title = fileBrowser.IsFolderBrowser ? Resources.Strings.FileDialog_SelectFoldersPrompt : Resources.Strings.FileDialog_SelectFilesPrompt;
            fileBrowser.EnsureFileExists = true;
            fileBrowser.EnsurePathExists = true;
            fileBrowser.Multiselect = true;
            var result = fileBrowser.ShowDialog();
            if (result == FileBrowserDialogResult.Ok)
            {
                bool updatedSearchDirectories = false;
                if (isFolderBrowser)
                {
                    foreach (var folder in fileBrowser.FileNames)
                    {
                        updatedSearchDirectories |= _searchDirectories.Add(folder);
                    }
                }
                AddRoms(fileBrowser.FileNames, false);
                if (updatedSearchDirectories)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Add ROMs to the ROM list given a list of files or directories.
        /// </summary>
        /// <param name="fileNames">An enumerable of absolute paths to files or directories.</param>
        /// <param name="addingStarterRoms">If set to <c>true</c>, the method is invoked to add "starter ROMs".</param>
        internal void AddRoms(IEnumerable<string> fileNames, bool addingStarterRoms)
        {
            var options = RomDiscoveryOptions.AddNewRoms | RomDiscoveryOptions.AccumulateRejectedRoms;
            var taskData = new RomDiscoveryData(fileNames, Programs.ModelCollection, Resources.Strings.RomListViewModel_Progress_Title, options) { AddingStarterRoms = addingStarterRoms };
            AddPrograms(taskData);
        }

        /// <summary>
        /// Removes the selected ROMs from the list.
        /// </summary>
        /// <param name="parameter">This argument is not used.</param>
        internal void RemoveRoms(object parameter)
        {
            var itemsToDelete = new List<ProgramDescription>(CurrentSelection.Select(p => p.Model));
            var deletedItems = new List<ProgramDescription>();
            foreach (var item in itemsToDelete)
            {
                if (Model.Remove(item))
                {
                    deletedItems.Add(item);
                }
            }
            if (deletedItems.Any())
            {
                RetainFocus = deletedItems.GetHashCode();
                var collectionChanged = CollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, deletedItems));
                }
                SaveRomList(true);
            }
        }

        /// <summary>
        /// Refresh the ROM list by visiting the directories saved in preferences.
        /// </summary>
        /// <param name="parameter">If this value is a valid <see cref="RomDiscoveryData"/>, it defines
        /// customized behavior for searching for ROMs.</param>
        internal void RefreshRoms(object parameter)
        {
            var taskData = parameter as RomDiscoveryData;
            if (taskData == null)
            {
                var options = RomDiscoveryOptions.AddNewRoms | RomDiscoveryOptions.DetectNewRoms | RomDiscoveryOptions.DetectChanges | RomDiscoveryOptions.DetectMissingRoms;
                taskData = new RomDiscoveryData(Properties.Settings.Default.RomListSearchDirectories, _programs.ModelCollection, Resources.Strings.RomListViewModel_ScanningForRoms_Title, options);
            }
            AddPrograms(taskData);
        }

        /// <summary>
        /// Determines whether refreshing the ROM list is currently possible.
        /// </summary>
        /// <param name="parameter">Not used.</param>
        /// <returns><c>true</c> if it is possible to refresh the ROM list; otherwise, <c>false</c>.</returns>
        internal bool CanRefreshRoms(object parameter)
        {
            return (_searchDirectories != null) && _searchDirectories.Any();
        }

        /// <summary>
        /// Gets the list of files that have been dropped into the ROM list visual via a drag and drop operation in the UI.
        /// </summary>
        /// <param name="dropArgs">OS-specific drag and drop data.</param>
        /// <param name="droppedFiles">The files that were dropped are added to this list.</param>
        partial void GetFilesDropped(object dropArgs, List<string> droppedFiles);

        private void AddPrograms(RomDiscoveryData taskData)
        {
            var cancel = Model.BeginAddRomsFromFiles(taskData.AddingStarterRoms);
            if (!cancel)
            {
                var gatherRomsTask = new AsyncTaskWithProgress(taskData.Title, true, true);
                gatherRomsTask.RunTask(taskData, GatherRoms, GatherRomsComplete);

                // EndAddRomsFromFiles is called in GatherRomsComplete().
            }
            else
            {
                Model.EndAddRomsFromFiles(Enumerable.Empty<string>());
            }
        }

        private void GatherRoms(AsyncTaskData taskData)
        {
            var args = (RomDiscoveryData)taskData;
            var actualRoms = args.PotentialRoms.IdentifyRomFiles(() => args.AcceptCancelIfRequested(), f => UpdateProgressText(args, Resources.Strings.RomListViewModel_Progress_Checking_Message_Format, f));
            var discoveredRoms = ProgramCollection.GatherRomsFromFileList(actualRoms, args.CurrentRoms, args.DuplicateRomPaths, () => args.AcceptCancelIfRequested(), f => UpdateProgressText(args, Resources.Strings.RomListViewModel_Progress_Identifying_Message_Format, f), n => UpdateProgressTitle(args, n), AcceptRom);
            args.NewRoms = discoveredRoms;
        }

        private bool AcceptRom(IProgramInformation programInfo)
        {
            bool accept = !programInfo.Features.GeneralFeatures.HasFlag(GeneralFeatures.SystemRom);
            return accept;
        }

        private void GatherRomsComplete(AsyncTaskData taskData)
        {
            var args = (RomDiscoveryData)taskData;
            var discoveredRoms = args.NewRoms;
            var addedRoms = Model.AddNewItemsFromList(discoveredRoms);
            if (addedRoms.Any())
            {
                var collectionChanged = CollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, addedRoms as System.Collections.IList));
                }
                SaveRomList(true);
            }
            Model.EndAddRomsFromFiles(args.DuplicateRomPaths);
            if (taskData.Error != null)
            {
                OSMessageBox.Show(Resources.Strings.AddRomsErrorMessage, Resources.Strings.AddRomsErrorTitle, taskData.Error, null);
            }
        }

        private void UpdateProgressText(AsyncTaskData taskData, string format, string content)
        {
            taskData.UpdateTaskProgress(0, string.Format(System.Globalization.CultureInfo.CurrentCulture, format, content));
        }

        private void UpdateProgressTitle(AsyncTaskData taskData, int numDiscovered)
        {
            var args = (RomDiscoveryData)taskData;
            taskData.UpdateTaskTitle(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomListViewModel_Progress_Title_Format, args.Title, numDiscovered));
        }

        private void HandleCurrentSelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(CurrentSelectionPropertyName);
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            var programDescription = (ProgramDescriptionViewModel)item;
                            var index = Programs.IndexOf(programDescription);
                            if (index >= 0)
                            {
                                Model.SelectionIndexes.Add(index);
                            }
                            programDescription.PropertyChanged += HandleProgramDescriptionChanged;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var programDescription = (ProgramDescriptionViewModel)item;
                            var index = Programs.IndexOf(programDescription);
                            if (index >= 0)
                            {
                                Model.SelectionIndexes.Remove(index);
                            }
                            foreach (var outOfRangeIndex in Model.SelectionIndexes.Where(i => i >= Model.Count).ToList())
                            {
                                // Unable to locate the item in the list. Clean up indexes as necessary.
                                Model.SelectionIndexes.Remove(outOfRangeIndex);
                            }
                            programDescription.PropertyChanged -= HandleProgramDescriptionChanged;
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    throw new System.NotImplementedException();
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private bool IsPersistedDescriptionProperty(string propertyName)
        {
            // This relies on the ViewModel having properties that are pass-through to the Model for anything that is persisted.
            // This would mean Name, ShortName, Crc, paths, etc. Perhaps there should be a more direct mechanism... But, we "know" that
            // the active selection in the ROM list (which is what users can edit) will pass through all model property changes.
            // See ProgramDescriptionViewModel.OnPropertyChanged().
            var propertyInfos = typeof(ProgramDescription).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty);
            var isPersisted = propertyInfos.FirstOrDefault(p => p.CanWrite && (p.Name == propertyName));
            return isPersisted != null;
        }

        private void HandleProgramDescriptionChanged(object sender, PropertyChangedEventArgs e)
        {
            var needsSave = IsPersistedDescriptionProperty(e.PropertyName);
            if (needsSave)
            {
                SaveRomList(true);
            }
        }

        private void HandleProgramCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ProgramCollection.CanEditElementsPropertyName)
            {
                RaisePropertyChanged(e.PropertyName);
            }
        }

        private void HandleSaveFailed(object sender, INTV.Shared.Model.Program.ProgramCollectionSaveFailedEventArgs e)
        {
            // FIXME: If this is ever invoked from a non-UI thread -- I'm looking at you, Mac, it may be trouble.
            var title = Resources.Strings.RomList_SaveFailed_Title;
            var message = Resources.Strings.RomList_SaveFailed_Message;
            var buttons = OSMessageBoxButton.OK;
            if (!string.IsNullOrEmpty(e.RomListBackupPath) && System.IO.File.Exists(e.RomListBackupPath))
            {
                message += Resources.Strings.RomList_SaveFailed_PromptToRestoreMessage;
                buttons = OSMessageBoxButton.YesNo;
            }
            var result = OSMessageBox.Show(message, title, e.Error, buttons, OSMessageBoxIcon.Error);
            if ((result == OSMessageBoxResult.Yes) && !string.IsNullOrEmpty(e.RomListBackupPath) && System.IO.File.Exists(e.RomListBackupPath))
            {
                var romsConfiguration = INTV.Shared.Model.RomListConfiguration.Instance;
                System.IO.File.Replace(e.RomListPath, romsConfiguration.RomFilesPath, null);
                InitializeRomList(romsConfiguration.RomFilesPath);
            }
        }

        private void HandlePeripheralAttached(object sender, PeripheralEventArgs e)
        {
            lock (_peripherals)
            {
                _peripherals.RemoveAll(p => !p.IsAlive);
                if (_peripherals.FirstOrDefault(p => p.IsAlive && object.ReferenceEquals(p.Target, sender)) == null)
                {
                    _peripherals.Add(new System.WeakReference(sender));
                }
            }
            RefreshLtoFlashOnlyRoms();
        }

        private void HandlePeripheralDetached(object sender, PeripheralEventArgs e)
        {
            lock (_peripherals)
            {
                _peripherals.RemoveAll(p => !p.IsAlive || object.ReferenceEquals(p.Target, sender));
            }
            RefreshLtoFlashOnlyRoms();
        }

        private void RefreshLtoFlashOnlyRoms()
        {
            lock (_peripherals)
            {
                var ltoFlashOnlyRoms = Programs.Where(p => p.Model.Rom.IsLtoFlashOnlyRom());
                foreach (var ltoFlashOnlyRom in ltoFlashOnlyRoms)
                {
                    ltoFlashOnlyRom.RefreshFileStatus(AttachedPeripherals);
                }
            }
        }

        private string RomListColumnToPropertyName(RomListColumn column)
        {
            var sortProperty = string.Empty;
            switch (column)
            {
                case RomListColumn.Title:
                    sortProperty = "Name";
                    break;
                case RomListColumn.Vendor:
                case RomListColumn.Year:
                case RomListColumn.Features:
                    sortProperty = column.ToString();
                    break;
                case RomListColumn.RomFile:
                    sortProperty = "Rom.RomPath";
                    break;
                default:
                    break;
            }
            return sortProperty;
        }

        /// <summary>
        /// Operating system-specific initialization code.
        /// </summary>
        partial void OSInitialize();

#if ENABLE_ROMS_PATCH
        /// <summary>
        /// Reset JLP, LTO Flash!, Bee3, and Hive flags to "Incompatible" as defaults.
        /// </summary>
        private class FixJlpAndOtherFlags : OneShotLaunchTask
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.Shared.ViewModel.RomListViewModel+FixJlpAndOtherFlags"/> class.
            /// </summary>
            /// <param name="romList">The ViewModel for the ROM list.</param>
            public FixJlpAndOtherFlags(RomListViewModel romList)
                : base("FixJlpAndOtherFlags")
            {
                RomList = romList;
            }

            private RomListViewModel RomList { get; set; }

            /// <inheritdoc/>
            protected override void Run()
            {
                foreach (var program in RomList.Model)
                {
                    program.Features.Jlp = JlpFeaturesHelpers.Default;
                    program.Features.LtoFlash = LtoFlashFeaturesHelpers.Default;
                    program.Features.Bee3 = Bee3FeaturesHelpers.Default;
                    program.Features.Hive = HiveFeaturesHelpers.Default;
                }
                RomList.SaveRomList(true);
                RomList._fixRomList = null;
                RomList = null;
            }
        }

        /// <summary>
        /// Patch up the misspelled XML element name for .cfg files.
        /// </summary>
        private class FixCfgFilesPatch : OneShotLaunchTask
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="INTV.Shared.ViewModel.RomListViewModel+FixCfgFilesPatch"/> class.
            /// </summary>
            /// <param name="romList">The ViewModel for the ROM list.</param>
            internal FixCfgFilesPatch(RomListViewModel romList)
                : base("FixCfgFilesPatch")
            {
                RomList = romList;
            }

            private RomListViewModel RomList { get; set; }

            /// <inheritdoc/>
            protected override void Run()
            {
                foreach (var program in RomList.Model)
                {
                    if (program.Rom.Format == INTV.Core.Model.RomFormat.Bin)
                    {
                        if (string.IsNullOrEmpty(program.Rom.ConfigPath))
                        {
                            var cfgFilePath = System.IO.Path.ChangeExtension(program.Rom.RomPath, ProgramFileKind.CfgFile.FileExtension());
                            if (!System.IO.File.Exists(cfgFilePath))
                            {
                                // try stock .cfg file
                                cfgFilePath = program.Rom.GetStockCfgFile(program.ProgramInformation);
                            }
                            if (System.IO.File.Exists(cfgFilePath))
                            {
                                INTV.Core.Model.Rom.ReplaceCfgPath(program.Rom, cfgFilePath);
                            }
                        }
                    }
                }
                RomList.SaveRomList(true);
                RomList._fixRomList = null;
                RomList = null;
            }
        }
#endif // ENABLE_ROMS_PATCH
    }
}

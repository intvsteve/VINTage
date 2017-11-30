// <copyright file="LtoFlashViewModel.cs" company="INTV Funhouse">
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

////#define ENABLE_DEVICE_DETECTION_TRACE
////#define ENABLE_ROMS_PATCH
////#define ENABLE_DRIVER_NAG
////#define MULTIPLE_DEVICE_ENHANCEMENTS
#define USE_SIMPLE_FILE_SYSTEM_COMPARE

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.Interop.DeviceManagement;
using INTV.Shared.Model.Device;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

#if WIN
using ExitEventArgs = System.Windows.ExitEventArgs;
using MenuLayoutView = INTV.LtoFlash.View.MenuTreeView;
using OSWindow = System.Windows.Window;
#elif MAC || GTK
using ExitEventArgs = INTV.Shared.Utility.ExitEventArgs;
using MenuLayoutView = INTV.LtoFlash.View.MenuLayoutView;
using OSWindow = INTV.Shared.View.SerialPortSelectorDialog;
#endif // WIN

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// This class acts as a general ViewModel for one or more Locutus devices in the system.
    /// </summary>
    /// <remarks>The current MenuLayoutViewModel probably belongs here.</remarks>
    ////[System.ComponentModel.Composition.Export(typeof(IPrimaryComponent))]
    public partial class LtoFlashViewModel : FolderViewModel, IPrimaryComponent, IPartImportsSatisfiedNotification
    {
        #region Constants

        public const string ComponentId = "INTV.LtoFlash";

        public const string ActiveLtoFlashDevicePropertyName = "ActiveLtoFlashDevice";

        public const string ShowFileSystemsDifferIconPropertyName = "ShowFileSystemsDifferIcon";

        #endregion // Constants

        private static readonly MenuLayout EmptyMenuLayout = new MenuLayout(new FileSystem(FileSystemOrigin.None), string.Empty);

        private ConnectionMonitor _connectionMonitor;

        private List<ProgramDescription> _programsToAdd;

        /// <summary>
        /// Cache the notion of 'dirtiness' for performance reasons.
        /// </summary>
        /// <remarks>Even with the SimpleCompare, some menu layouts can result in slower-than-desired comparisons. To avoid this,
        /// treat any change made by the user as dirtying. It should remain 'dirty' until the user performs a sync operation, or
        /// disconnects the active device.</remarks>
        private int? _cachedFileSystemsCompareResult = null;

#if ENABLE_ROMS_PATCH
        private FixJlpAndOtherFlagsInMenu _fixRoms;
        private FixCfgFilesPatchInMenu _fixCfgFiles;
#endif // ENABLE_ROMS_PATCH

        #region UI Strings

        /// <summary>The contents-not-in-sync tool tip.</summary>
        public static readonly string ContentsNotInSyncToolTip = Resources.Strings.MenuLayout_Device_NotInSync;

        #endregion // UI Strings

        #region Constructors

        static LtoFlashViewModel()
        {
            INTV.Shared.View.OSMessageBox.RegisterExceptionFilter(MessageBoxExceptionFilter);
        }

        /// <summary>
        /// Initializes a new instance of LtoFlashViewModel.
        /// </summary>
        public LtoFlashViewModel()
        {
            Model = EmptyMenuLayout;
            HostPCMenuLayout = new MenuLayoutViewModel();
            HostPCMenuLayout.LtoFlashViewModel = this;
            HostPCMenuLayout.MenuLayoutSaved += HandleHostPCMenuLayoutSaved;
            UpdateSystemContentsUsage(EmptyMenuLayout.FileSystem.Directories);
            UpdateSystemContentsUsage(EmptyMenuLayout.FileSystem.Files);
            UpdateSystemContentsUsage(EmptyMenuLayout.FileSystem.Forks);
            _ltoDeviceIconUri = DeviceViewModel.NoConnectedDevices;
            _activeLtoFlashDevice = DeviceViewModel.InvalidDevice;
            _activeLtoFlashDevices = new ObservableCollection<DeviceViewModel>();
            _connectionMonitor = new ConnectionMonitor();
            _devices = new ObservableViewModelCollection<DeviceViewModel, Device>(new ObservableCollection<Device>(), DeviceViewModel.Factory, null);
            _fileSystemStatistics = new FileSystemStatisticsViewModel();
            _firmwareRevisions = new FirmwareRevisionsViewModel();
            _connectionMonitor.Peripherals.CollectionChanged += HandlePeripheralsCollectionChanged;
            _connectionMonitor.RegisterPeripheralFactory(Device.GetLtoFlashDevice); // One day, these will be discovered via MEF (yeah, right)
            DeviceChange.DeviceAdded += HandleDeviceAdded;
            DeviceChange.DeviceRemoved += HandleDeviceRemoved;

            // This can happen in XAML designer in Visual Studio.
            if (SingleInstanceApplication.Instance != null)
            {
                SingleInstanceApplication.Instance.Exit += HandleApplicationExit;
                SingleInstanceApplication.Instance.Roms.AddRomsFromFilesBegin += HandleAddRomsFromFilesBegin;
                SingleInstanceApplication.Instance.Roms.AddRomsFromFilesEnd += HandleAddRomsFromFilesEnd;
            }
            INTV.Shared.Model.Program.ProgramCollection.Roms.AddInvokeProgramHandler(HandleInvokeProgram, 0.1);
            INTV.Shared.Model.Program.ProgramCollection.Roms.ProgramFeaturesChanged += HandleProgramFeaturesChanged;
            INTV.Shared.Model.Program.ProgramCollection.Roms.ProgramStatusChanged += HandleProgramStatusChanged;

            OSInitialize();

#if ENABLE_DRIVER_NAG
            var ftdiDriverVersion = INTV.LtoFlash.Utility.FTDIUtilities.DriverVersion;
            if (Properties.Settings.Default.PromptToInstallFTDIDriver && (ftdiDriverVersion.Major == 0) && LtoFlashCommandGroup.LaunchFtdiDriverInstallerCommand.CanExecute(null))
            {
                var startupAction = new System.Action(() => LtoFlashCommandGroup.LaunchFtdiDriverInstallerCommand.Execute(Resources.Strings.LaunchFtdiDriverInstallerCommand_NoDriverPromptMessageFormat));
                SingleInstanceApplication.Instance.AddStartupAction("CheckFTDIDriver", startupAction, StartupTaskPriority.HighestAsyncTaskPriority - 20);
            }
#endif // ENABLE_DRIVER_NAG

            Properties.Settings.Default.PropertyChanged += HandlePreferenceChanged;
            DeviceMonitor.Start();
            if ((SingleInstanceApplication.Instance != null) && INTV.LtoFlash.Properties.Settings.Default.SearchForDevicesAtStartup)
            {
                var checkForDevices = new CheckForDevicesTaskData(INTV.LtoFlash.Properties.Settings.Default.LastActiveDevicePort, this);
                var startupAction = new System.Action(() => checkForDevices.Start());
                SingleInstanceApplication.Instance.AddStartupAction("SearchForDevices", startupAction, StartupTaskPriority.HighestAsyncTaskPriority - 4);
                if (Properties.Settings.Default.PromptToImportStarterRoms)
                {
                    SingleInstanceApplication.Instance.AddStartupAction("ImportStarterRoms", ImportStarterRoms, StartupTaskPriority.LowestSyncTaskPriority);
                }
            }
            SyncMode = MenuLayoutSynchronizationMode.RomList;
            CompositionHelpers.Container.ComposeExportedValue<IPrimaryComponent>(this);
            CompositionHelpers.Container.ComposeExportedValue<LtoFlashViewModel>(this);

#if ENABLE_ROMS_PATCH
            _fixRoms = new FixJlpAndOtherFlagsInMenu(this);
            _fixRoms.Register();
            _fixCfgFiles = new FixCfgFilesPatchInMenu(this);
            _fixCfgFiles.Register();
#endif // ENABLE_ROMS_PATCH
            this.DoImport();
        }

        #endregion // Constructors

        #region Properties

        #region IPrimaryComponent Properties

        /// <inheritdoc/>
        public string UniqueId
        {
            get { return ComponentId; }
        }

        #endregion // IPrimaryComponent Properties

        /// <summary>
        /// Gets or sets the 'synchronization mode' used to reflect differences between the host and target menu layout.
        /// </summary>
        internal static MenuLayoutSynchronizationMode SyncMode { get; set; }

        /// <summary>
        /// Gets the known Locutus devices currently attached to the system.
        /// </summary>
        public ObservableViewModelCollection<DeviceViewModel, Device> Devices
        {
            get { return _devices; }
        }
        private ObservableViewModelCollection<DeviceViewModel, Device> _devices;

#if MULTIPLE_DEVICE_ENHANCEMENTS
        /// <summary>
        /// Gets the menu layouts for known devices attached to the system.
        /// </summary>
        public ObservableViewModelCollection<MenuLayoutViewModel, MenuLayout> DeviceMenuLayouts
        {
            get { return _deviceMenuLayouts; }
        }
        private ObservableViewModelCollection<MenuLayoutViewModel, MenuLayout> _deviceMenuLayouts;
#endif // MULTIPLE_DEVICE_ENHANCEMENTS

        /// <summary>
        /// Gets the available ports. Users may choose one to see if there is a device connected.
        /// </summary>
        public ObservableCollection<DeviceConnectionViewModel> PotentialDevicePorts
        {
            get { return _potentialDevicePorts; }
        }
        private ObservableCollection<DeviceConnectionViewModel> _potentialDevicePorts;

        /// <summary>
        /// Gets the available device LTO Flash! ports.
        /// </summary>
        public IEnumerable<string> AvailableDevicePorts
        {
            get { return SerialPortConnection.GetAvailablePorts(IsLtoFlashSerialPort); }
        }

        /// <summary>
        /// Gets the active Locutus device.
        /// </summary>
        public DeviceViewModel ActiveLtoFlashDevice
        {
            get { return _activeLtoFlashDevice; }
            internal set { UpdateProperty(ActiveLtoFlashDevicePropertyName, (s, d) => _activeLtoFlashDevice = UpdateActiveDevice(_activeLtoFlashDevice, d), value, _activeLtoFlashDevice); }
        }
        private DeviceViewModel _activeLtoFlashDevice;

        /// <summary>
        /// Gets the active Locutus devices.
        /// </summary>
        public ObservableCollection<DeviceViewModel> ActiveLtoFlashDevices
        {
            get { return _activeLtoFlashDevices; }
        }
        private ObservableCollection<DeviceViewModel> _activeLtoFlashDevices;

        /// <summary>
        /// Gets the file system statistics from a Locutus device.
        /// </summary>
        public FileSystemStatisticsViewModel FileSystemStatistics
        {
            get { return _fileSystemStatistics; }
        }
        private FileSystemStatisticsViewModel _fileSystemStatistics;

        /// <summary>
        /// Gets the view model for firmware revision data.
        /// </summary>
        public FirmwareRevisionsViewModel FirmwareRevisions
        {
            get { return _firmwareRevisions; }
        }
        private FirmwareRevisionsViewModel _firmwareRevisions;

        /// <summary>
        /// Gets the root of the menu layout.
        /// </summary>
        public FolderViewModel Root
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the model as a MenuLayout.
        /// </summary>
        public MenuLayout MenuLayout
        {
            get { return Model as MenuLayout; }
        }

        /// <summary>
        /// Gets the total number of folders in use in the menu, which includes the root.
        /// </summary>
        public string FolderCount { get; private set; }

        /// <summary>
        /// Gets the total number of files in use in the menu, which includes the file associated with the root folder.
        /// </summary>
        public string FileCount { get; private set; }

        /// <summary>
        /// Gets the total number of data forks in use, which includes ROMs, manuals, save-game data, and other data.
        /// </summary>
        public string ForkCount { get; private set; }

        /// <summary>
        /// Gets the image resource to use to display an image for whether a Locutus is attached.
        /// </summary>
        public string LtoDeviceConnectedImage
        {
            get { return _ltoDeviceIconUri; }
            private set { AssignAndUpdateProperty("LtoDeviceConnectedImage", value, ref _ltoDeviceIconUri); }
        }
        private string _ltoDeviceIconUri;

        /// <summary>
        /// Gets or sets the current selection in the ROM list so it's accessible here.
        /// </summary>
        /// <remarks>THIS IS A TOTAL HACK! Should create a binding or other better mechanism than the hack in MainWindowViewModel.</remarks>
        public ObservableCollection<ProgramDescriptionViewModel> CurrentSelection { get; set; }

        /// <summary>
        /// Gets the host PC menu layout.
        /// </summary>
        public MenuLayoutViewModel HostPCMenuLayout { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current HostPCMenuLayout is different than the active device's.
        /// </summary>
        public bool ShowFileSystemsDifferIcon
        {
            get { return _showFileSystemsDifferIcon; }
            internal set { AssignAndUpdateProperty(ShowFileSystemsDifferIconPropertyName, value, ref _showFileSystemsDifferIcon); }
        }
        private bool _showFileSystemsDifferIcon;

        /// <summary>
        /// Gets or sets the connection sharing policies.
        /// </summary>
        [System.ComponentModel.Composition.ImportMany(typeof(IConnectionSharingPolicy))]
        public IEnumerable<System.Lazy<IConnectionSharingPolicy>> ConnectionSharingPolicies { get; set; }

        /// <summary>
        /// Gets the attached peripherals.
        /// </summary>
        internal IEnumerable<IPeripheral> AttachedPeripherals
        {
            get { return ActiveLtoFlashDevices.Select(vm => vm.Device).Where(m => m.IsValid); }
        }

        private bool SelectionDialogShowing { get; set; }

        #endregion // Properties

        #region IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            _potentialDevicePorts = new ObservableCollection<DeviceConnectionViewModel>(DeviceConnectionViewModel.GetAvailableConnections(this));
        }

        #endregion // IPartImportsSatisfiedNotification Members

        #region IPrimaryComponent

        /// <inheritdoc />
        public void Initialize()
        {
            // Freeze so we don't do the expensive work yet.
            bool? wasFrozen = MenuLayout.FileSystem.Frozen;
            try
            {
                // Copy out programs and forks.
                IEnumerable<Fork> forks;
                IEnumerable<Program> programs;
                lock (MenuLayout.FileSystem)
                {
                    MenuLayout.FileSystem.Frozen = true;
                    programs = MenuLayout.FileSystem.Files.OfType<Program>().Where(p => p != null).ToList();
                    forks = MenuLayout.FileSystem.Forks.Where(f => (f != null) && !string.IsNullOrEmpty(f.FilePath)).ToList();
                    MenuLayout.FileSystem.Frozen = wasFrozen.Value;
                    wasFrozen = null;
                }
                foreach (var fork in forks)
                {
                    if (!string.IsNullOrEmpty(fork.FilePath))
                    {
                        INTV.Core.Utility.Crc24.OfFile(fork.FilePath);
                    }
                }

                // TODO: How to deal with Alternates?
                foreach (var program in programs)
                {
                    uint cfgCrc;
                    Core.Model.Rom.GetRefreshedCrcs(program.Description.Files.RomImagePath, program.Description.Files.RomConfigurationFilePath, out cfgCrc);
                }
            }
            finally
            {
                if (wasFrozen != null)
                {
                    MenuLayout.FileSystem.Frozen = wasFrozen.Value;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ComponentVisual> GetVisuals()
        {
            // TODO: This is wrong... ViewModel should not create the visual - it should be the other way around
            if (_visual == null)
            {
                OSCreateVisuals();
            }
            MenuLayoutView menuLayoutView = _visual.IsAlive ? _visual.Target as MenuLayoutView : null;
            var componentVisual = new ComponentVisual(MenuLayoutView.Id, menuLayoutView, "LTO Flash! Menu Layout");
            yield return componentVisual;
        }

        // Disable never assigned to warning, since whether this value is assigned to depends on the OS.
        // TODO: Eventually, Mac and Windows will use this... Eventually.
#pragma warning disable 649
        private System.WeakReference _visual;
#pragma warning restore 649

        #endregion // IPrimaryComponent

        /// <summary>
        /// Filter function for serial ports.
        /// </summary>
        /// <param name="connection">The IConnection to test.</param>
        /// <returns><c>true</c>, if the given connection is a serial port on LTO Flash! hardware, <c>false</c> otherwise.</returns>
        internal bool IsLtoFlashSerialPort(IConnection connection)
        {
            var isValidPort = !Properties.Settings.Default.VerifyVIDandPIDBeforeConnecting;
            if (!isValidPort)
            {
                foreach (var policy in ConnectionSharingPolicies)
                {
                    isValidPort = policy.Value.ExclusiveAccess(connection);
                    if (!isValidPort)
                    {
                        break;
                    }
                }
            }
            return isValidPort;
        }

        /// <summary>
        /// Show a dialog to select a device or switch to another one.
        /// </summary>
        /// <param name="ports">The ports to include in the selection dialog.</param>
        internal void PromptForDeviceSelection(IList<string> ports)
        {
            if (!SelectionDialogShowing && (SingleInstanceApplication.Current != null) && (SingleInstanceApplication.Current.MainWindow != null))
            {
                try
                {
                    SelectionDialogShowing = true;
                    SingleInstanceApplication.Instance.IsBusy = true;
                    OSWindow dialog = null;
                    var multiSelect = ports.Count() > 1;
                    var title = multiSelect ? Resources.Strings.SelectDeviceDialog_Title : Resources.Strings.ConnectToDevice_Title;
                    var message = multiSelect ? Resources.Strings.SelectDeviceDialog_Message : Resources.Strings.ConnectToDevice_Message;
                    var disabledPorts = new List<string>();
                    if (ActiveLtoFlashDevice.IsValid)
                    {
                        disabledPorts.Add(ActiveLtoFlashDevice.Device.Port.Name);
                    }
                    dialog = INTV.Shared.View.SerialPortSelectorDialog.Create(title, message, ports, disabledPorts, Device.DefaultBaudRate, IsLtoFlashSerialPort);

                    var viewModel = dialog.DataContext as SerialPortSelectorDialogViewModel;
                    var result = dialog.ShowDialog();
                    SelectionDialogShowing = false;
                    if (result.HasValue && result.Value)
                    {
                        var existingDevice = Devices.FirstOrDefault(d => (d.Device != null) && (d.Device.Port != null) && (d.Device.Port.Name == viewModel.SelectedSerialPort));
                        var creationInfo = new DeviceCreationInfo(true, true, ActivationMode.ForceActivate);
                        if (existingDevice != null)
                        {
                            existingDevice.Device.CreationInfo = creationInfo; // ensure we fully validate and report errors (happens if showing due to user plugging in device)
                            ActiveLtoFlashDevice = existingDevice;
                        }
                        else
                        {
                            var configInfo = new Dictionary<string, object>() { { DeviceCreationInfo.ConfigName, creationInfo } };
                            INTV.Shared.Interop.DeviceManagement.DeviceChange.ReportDeviceAdded(this, viewModel.SelectedSerialPort, Core.Model.Device.ConnectionType.Serial, configInfo);
                        }
                    }
                    else
                    {
                        DebugDeviceDetectionOutput("()()()()()()() Active: " + ActiveLtoFlashDevice.DisplayName + " isValid: " + ActiveLtoFlashDevice.IsValid);
                        foreach (var device in Devices)
                        {
                            DebugDeviceDetectionOutput(device.DisplayName + " isValid: " + device.IsValid);
                        }
                    }
                }
                finally
                {
                    SelectionDialogShowing = false;
                    SingleInstanceApplication.Instance.IsBusy = false;
                }
            }
        }

        /// <summary>
        /// Reset the cached result of a file system comparison to ensure a re-do of comparison can be done.
        /// </summary>
        internal void ResetCachedFileSystemsCompareResult()
        {
            _cachedFileSystemsCompareResult = null;
        }

        private static bool MessageBoxExceptionFilter(System.Exception exception)
        {
            var report = true;
            var deviceCommandFailedException = exception as DeviceCommandFailedException;
            if ((deviceCommandFailedException != null) && (deviceCommandFailedException.ErrorLog != null))
            {
                var isDecodeError = deviceCommandFailedException.ErrorLog.ErrorIds.All(id => id == ErrorLogId.Luigi);
                report = !isDecodeError;
            }
            return report;
        }

        [System.Diagnostics.Conditional("ENABLE_DEVICE_DETECTION_TRACE")]
        private static void DebugDeviceDetectionOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void ImportStarterRoms()
        {
            // This is a one-shot deal, unless soneone resets the preference.
            Properties.Settings.Default.PromptToImportStarterRoms = false;

            // Look for ROMs embedded in the app build. Specifically, they must be in *this* assembly.
            var starterRomsResourcePrefix = "INTV.LtoFlash.Resources.StarterROMs.";
            var starterRomResources = typeof(LtoFlashViewModel).GetResources(starterRomsResourcePrefix);
            if (starterRomResources.Any())
            {
                var starterRomsDirectory = Configuration.Instance.StarterRomsDirectory;
                var starterRoms = typeof(LtoFlashViewModel).ExtractResourcesToFiles(starterRomResources, starterRomsResourcePrefix, starterRomsDirectory);
                if (starterRoms.Any())
                {
                    var message = string.Format(Resources.Strings.PromptToAddStarterRoms_MessageFormat, starterRomsDirectory);
                    var addStarterRoms = INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.PromptToAddStarterRoms_Title, INTV.Shared.View.OSMessageBoxButton.YesNo);
                    if (addStarterRoms == INTV.Shared.View.OSMessageBoxResult.Yes)
                    {
                        INTV.Shared.Commands.RomListCommandGroup.AddRomFoldersCommand.Execute(new[] { starterRomsDirectory });
                    }
                }
            }
        }

        private void HandleAddRomsFromFilesBegin(object sender, AddRomsFromFilesBeginEventArgs e)
        {
            if (!e.AddingStarterRoms && Properties.Settings.Default.PromptToAddRomsToMenu)
            {
                var dialog = INTV.LtoFlash.View.PromptToAddMenuItemsForNewRoms.Create();
                var result = dialog.ShowDialog();
                e.Cancel = !result.HasValue || !result.Value; // result of false means !OK means Cancel
            }
            if (e.AddingStarterRoms || Properties.Settings.Default.AddRomsToMenu)
            {
                _programsToAdd = new List<ProgramDescription>();
                SingleInstanceApplication.Instance.Roms.CollectionChanged += HandleRomAdded;
            }
        }

        private void HandleRomAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var program = e.NewItems[i] as ProgramDescription;
                        if (_programsToAdd != null)
                        {
                            _programsToAdd.Add(program);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleAddRomsFromFilesEnd(object sender, AddRomsFromFilesEndEventArgs e)
        {
            if (Properties.Settings.Default.AddRomsToMenu)
            {
                SingleInstanceApplication.Instance.Roms.CollectionChanged -= HandleRomAdded;
                if ((_programsToAdd != null) && _programsToAdd.Any())
                {
                    var paths = _programsToAdd.Select(p => p.Rom.RomPath).ToList();
                    if (e.DuplicateRomPaths != null)
                    {
                        ////paths.AddRange(e.DuplicateRomPaths);
                    }
                    var longestCommonPath = PathUtils.GetCommonPath(paths);
                    List<string> directoriesForRoms = null;
                    if (!string.IsNullOrEmpty(longestCommonPath) && (_programsToAdd.Count > 1))
                    {
                        directoriesForRoms = paths.Select(p => System.IO.Path.GetDirectoryName(p).Remove(0, longestCommonPath.Length)).ToList();
                    }
                    FileNodeViewModel.AddItems(HostPCMenuLayout, directoriesForRoms, _programsToAdd);
                }
            }
            _programsToAdd = null;
        }

        private void HandleHostPCMenuLayoutSaved(object sender, MenuSaveCompleteEventArgs e)
        {
            if (e.Error == null)
            {
                if (!string.IsNullOrEmpty(e.BackupPath) && System.IO.File.Exists(e.BackupPath))
                {
                    FileUtilities.DeleteFile(e.BackupPath, false, 10);
                }
                if (ActiveLtoFlashDevice.IsValid)
                {
                    // Operate on a clone of the just-saved menu so we don't trigger an infinite number of saves.
                    var configuration = Configuration.Instance;
                    var menuLayout = ((MenuLayoutViewModel)sender).MenuLayout.FileSystem.Clone().Directories[GlobalDirectoryTable.RootDirectoryNumber] as MenuLayout;
                    menuLayout.Save(configuration.GetMenuLayoutPath(ActiveLtoFlashDevice.UniqueId), true);

                    // ^^^^ The above is weird... what we do here is, after the 'main' menu finishes saving,
                    // we spawn saving a copy of it in a device-specific directory -- when a device is connected.
                }

                // Either force compare w/ device... OR... assume that any 'save' is 'dirty' until re-sync.
                if (ActiveLtoFlashDevice.IsValid && !e.NonDirtying)
                {
                    _cachedFileSystemsCompareResult = 1;
                }
                UpdateFileSystemsInSync(true); // always do the refresh after a save
            }
            else
            {
                var title = Resources.Strings.SaveMenuError_Title;
                var message = Resources.Strings.SaveMenuError_Message;
                var reportErrorResult = INTV.Shared.View.OSMessageBox.Show(message, title, e.Error, INTV.Shared.View.OSMessageBoxButton.YesNo);
                if (reportErrorResult == INTV.Shared.View.OSMessageBoxResult.Yes)
                {
                    System.IO.File.Replace(e.BackupPath, e.MenuPath, null);
                    var restoreMenu = MenuLayout.Load(e.MenuPath);
                    this.HostPCMenuLayout.MenuLayout = restoreMenu;
                }
            }
        }

        private void HandleInvokeProgram(object sender, InvokeProgramEventArgs e)
        {
            if ((e.Program != null) && DownloadCommandGroup.DownloadAndPlayCommand.CanExecute(this))
            {
                e.Handled = true;
                DownloadCommandGroup.DownloadAndPlay(ActiveLtoFlashDevice.Device, e.Program);
            }
        }

        private void HandleProgramFeaturesChanged(object sender, ProgramFeaturesChangedEventArgs e)
        {
            // Force LUIGI regeneration here.
            var errors = new Dictionary<IRom, System.Exception>();
            HostPCMenuLayout.StartItemsUpdate();
            foreach (var rom in e.UpdatedRoms)
            {
                try
                {
                    var updateMode = e.ResetToDefault ? LuigiGenerationMode.Reset : LuigiGenerationMode.FeatureUpdate;
                    var updatedLuigiFile = rom.PrepareForDeployment(updateMode);
                    var newUid = INTV.Core.Utility.Crc24.OfFile(updatedLuigiFile);
                    var programsToUpdate = HostPCMenuLayout.MenuLayout.FindPrograms(p => p.Description.Rom.RomPath == rom.RomPath);
                    foreach (var program in programsToUpdate.Where(p => p.Rom.Uid != newUid))
                    {
                        program.Rom.Crc24 = newUid;
                    }
                }
                catch (System.IO.IOException exception)
                {
                    errors[rom] = exception;
                }
                catch (LuigiFileGenerationException exception)
                {
                    errors[rom] = exception;
                }
            }
            HostPCMenuLayout.FinishItemsUpdate(true);
            if (errors.Any())
            {
                var showDetails = SingleInstanceApplication.SharedSettings.ShowDetailedErrors;
                var message = Resources.Strings.UpdateRomFeatures_ErrorMessage;
                var errorDetailBuilder = new System.Text.StringBuilder(Resources.Strings.UpdateRomFeatures_ErrorDetailHeader).AppendLine().AppendLine();
                foreach (var error in errors)
                {
                    errorDetailBuilder.AppendFormat(Resources.Strings.UpdateRomFeatures_ErrorDetailRomPathFormat, error.Key.RomPath).AppendLine();
                    errorDetailBuilder.AppendFormat(Resources.Strings.UpdateRomFeatures_ErrorDetailRomTypeFormat, error.Key.Format).AppendLine();
                    errorDetailBuilder.AppendFormat(Resources.Strings.UpdateRomFeatures_ErrorDetailMessageFormat, error.Value.Message).AppendLine();
                    if (showDetails)
                    {
                        errorDetailBuilder.AppendLine(Resources.Strings.UpdateRomFeatures_ErrorDetailExceptionHeader).AppendLine(error.Value.ToString());
                    }
                    errorDetailBuilder.AppendLine();
                }
                var errorDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.UpdateRomFeatures_ErrorTitle, message);
                errorDialog.ReportText = errorDetailBuilder.ToString();
                errorDialog.ShowSendEmailButton = false;
                errorDialog.BeginInvokeDialog(Resources.Strings.OK, null);
            }
        }

        private void HandleProgramStatusChanged(object sender, ProgramFeaturesChangedEventArgs e)
        {
            var programs = HostPCMenuLayout.MenuLayout.FileSystem.Files.OfType<Program>();
            foreach (var rom in e.UpdatedRoms)
            {
                var programStatesToUpdate = programs.Where(p => p.Description.Rom.RomPath == rom.RomPath && p.Description.Rom.ConfigPath == rom.ConfigPath);
                foreach (var program in programStatesToUpdate)
                {
                    var viewModel = HostPCMenuLayout.FindViewModelForModel(program) as ProgramViewModel;
                    if (viewModel != null)
                    {
                        viewModel.RefreshValidationState(AttachedPeripherals);
                    }
                }
            }
        }

        private void HandlePeripheralsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // BUG BUG BUG If you try to connect to something that then fails to be a valid device, the peripheral
            // won't be removed here!
            if (!SelectionDialogShowing)
            {
                DeviceViewModel newActiveViewModel = null;
                bool collectionChanged = false;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        collectionChanged = true;
                        var forcedActivation = false;
                        Devices.ModelCollection.SynchronizeCollection(e); // ensure new ViewModels are created
                        var newDevicePorts = new List<string>();
                        for (int i = 0; i < e.NewItems.Count; ++i)
                        {
                            var newDevice = e.NewItems[i] as Device;
                            if (newDevice != null)
                            {
                                newDevicePorts.Add(newDevice.Port.Name);
                                var index = Devices.ModelCollection.IndexOf(newDevice);
                                var deviceViewModel = Devices[index];
                                deviceViewModel.PropertyChanged += HandleDevicePropertyChanged;
                                if ((!ActiveLtoFlashDevice.IsValid && Properties.Settings.Default.AutomaticallyConnectToDevices) || (newDevice.CreationInfo.ActivationMode == ActivationMode.ForceActivate))
                                {
                                    newActiveViewModel = deviceViewModel;
                                    forcedActivation = true;
                                }
                            }
                        }
                        if ((!Properties.Settings.Default.AutomaticallyConnectToDevices && (newActiveViewModel == null)) || (!forcedActivation && (Properties.Settings.Default.AutomaticallyConnectToDevices && ActiveLtoFlashDevice.IsValid) && !SelectionDialogShowing))
                        {
                            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new System.Action(() => PromptForDeviceSelection(newDevicePorts)));
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        var removedActiveDevice = false;
                        collectionChanged = true;
                        for (int i = 0; i < e.OldItems.Count; ++i)
                        {
                            var oldDevice = e.OldItems[i] as Device;
                            if (oldDevice != null)
                            {
                                var index = Devices.ModelCollection.IndexOf(oldDevice);
                                if (index >= 0)
                                {
                                    var deviceViewModel = Devices[index];
                                    deviceViewModel.PropertyChanged -= HandleDevicePropertyChanged;
                                    removedActiveDevice |= deviceViewModel == ActiveLtoFlashDevice;
                                }
                                oldDevice.Disconnect(false);
                            }
                        }
                        Devices.ModelCollection.SynchronizeCollection(e); // ensure old ViewModels are removed
                        if (removedActiveDevice)
                        {
                            newActiveViewModel = Devices.FirstOrDefault(d => d.IsValid);
                            if (newActiveViewModel == null)
                            {
                                newActiveViewModel = Devices.FirstOrDefault();
                            }
                        }
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
                if (collectionChanged)
                {
                    RaisePropertyChanged("Devices");
                    if (newActiveViewModel != null)
                    {
                        ActiveLtoFlashDevice = newActiveViewModel;
                    }
                    else if (!Devices.Any())
                    {
                        ActiveLtoFlashDevice = INTV.LtoFlash.ViewModel.DeviceViewModel.InvalidDevice;
                    }
                    LtoDeviceConnectedImage = Devices.Any() ? INTV.LtoFlash.ViewModel.DeviceViewModel.ConnectedDevices : INTV.LtoFlash.ViewModel.DeviceViewModel.NoConnectedDevices;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private DeviceViewModel UpdateActiveDevice(DeviceViewModel oldDevice, DeviceViewModel newDevice)
        {
            FirmwareRevisions.FirmwareRevisions = LtoFlash.Model.FirmwareRevisions.Unavailable;
            FileSystemStatistics.FileSystemStatistics = null;
            Model = EmptyMenuLayout;
            if ((oldDevice != null) && (oldDevice.Device != null))
            {
                oldDevice.Device.Disconnect(false);
                _activeLtoFlashDevices.Remove(oldDevice);
            }
            if ((newDevice != null) && (newDevice.Device != null))
            {
                if ((newDevice.Device.Port != null) && !newDevice.Device.Port.IsOpen && (newDevice.Device.CreationInfo.ActivationMode == ActivationMode.ForceActivate))
                {
                    newDevice.Device.ValidateDevice();
                }
                if (newDevice.IsValid && (newDevice.Device.FileSystem != null))
                {
                    var model = new MenuLayout(new FileSystem(), newDevice.UniqueId);
                    try
                    {
                        model = new MenuLayout(newDevice.Device.FileSystem, newDevice.UniqueId);
                    }
                    catch (System.Exception e)
                    {
                        SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() => DeviceViewModel.ReportFileSystemInconsistencies(newDevice.Device.FileSystem, e));
                    }
                    Model = model;
                }
                _activeLtoFlashDevices.Add(newDevice);
            }
            UpdateItemStatesAsNecessary(newDevice);
            OSDeviceArrivalDepartureActiveChanged();
            return newDevice;
        }

        private void HandleDeviceAdded(object sender, DeviceChangeEventArgs e)
        {
            if ((SerialConnectionPolicy.Instance != null) && (e.Type == Core.Model.Device.ConnectionType.Serial) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                SerialConnectionPolicy.Instance.Reset();
            }
            if ((e.Type == Core.Model.Device.ConnectionType.Serial) && IsLtoFlashSerialPort(Connection.CreatePseudoConnection(e.Name, e.Type)))
            {
                if (!PotentialDevicePorts.Any(c => c.Name == e.Name))
                {
                    var firstAfter = PotentialDevicePorts.FirstOrDefault(p => p.Name.CompareTo(e.Name) > 0);
                    var index = PotentialDevicePorts.IndexOf(firstAfter);
                    if (index >= 0)
                    {
                        PotentialDevicePorts.Insert(index, new DeviceConnectionViewModel(this, e.Name));
                    }
                    else
                    {
                        PotentialDevicePorts.Add(new DeviceConnectionViewModel(this, e.Name));
                    }
                }
                PotentialDevicePorts.Remove(DeviceConnectionViewModel.NoneAvailable);
                OSDeviceArrivalDepartureActiveChanged();
            }
        }

        private void HandleDeviceRemoved(object sender, DeviceChangeEventArgs e)
        {
            object creationInfo = null;
            if ((SerialConnectionPolicy.Instance != null) && (e.Type == Core.Model.Device.ConnectionType.Serial) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                SerialConnectionPolicy.Instance.Reset();
            }
            e.State.TryGetValue(DeviceCreationInfo.ConfigName, out creationInfo);
            if ((e.Type == Core.Model.Device.ConnectionType.Serial) && !(creationInfo is DeviceCreationInfo) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                var match = PotentialDevicePorts.FirstOrDefault(c => c.Name == e.Name);
                PotentialDevicePorts.Remove(match);
                if (!PotentialDevicePorts.Any())
                {
                    PotentialDevicePorts.Add(DeviceConnectionViewModel.NoneAvailable);
                }
                OSDeviceArrivalDepartureActiveChanged();
            }
        }

        private void HandleDevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var device = sender as DeviceViewModel;
            switch (e.PropertyName)
            {
                case Device.IsValidPropertyName:
                    if ((device == ActiveLtoFlashDevice) && device.IsValid && ((INTV.Core.Model.Device.Connection)device.Device.Port).Type == Core.Model.Device.ConnectionType.Serial)
                    {
                        INTV.LtoFlash.Properties.Settings.Default.LastActiveDevicePort = device.Device.Port.Name;
                        INTV.LtoFlash.Properties.Settings.Default.Save();
                        if (device.Device.FirmwareRevisions.Current > LtoFlash.Model.FirmwareRevisions.UnavailableFirmwareVersion)
                        {
                            PromptForFirmwareUpgrade(device);
                        }
                    }
                    if (_cachedFileSystemsCompareResult.HasValue && ((device == null) || !device.IsValid))
                    {
                        ResetCachedFileSystemsCompareResult();
                    }
                    UpdateItemStatesAsNecessary(device);
                    break;
                case Device.FileSystemPropertyName:
                    if (ActiveLtoFlashDevice.IsValid && (ActiveLtoFlashDevice.Device.FileSystem != null))
                    {
                        var fileSystem = ActiveLtoFlashDevice.Device.FileSystem;
                        var addedSaveDataForks = HostPCMenuLayout.MenuLayout.FileSystem.PopulateSaveDataForksFromDevice(fileSystem);
                        System.Diagnostics.Debug.WriteLineIf(addedSaveDataForks, "Added SaveData forks from LTO Flash!.");
                        var reportedProblems = false;
                        System.Exception corruptFileSystem = null;
                        if (!fileSystem.Validate(out corruptFileSystem))
                        {
                            reportedProblems = DeviceViewModel.ReportFileSystemInconsistencies(fileSystem, corruptFileSystem);
                        }
                        if ((corruptFileSystem == null) && Properties.Settings.Default.ReconcileDeviceMenuWithLocalMenu)
                        {
                            HostPCMenuLayout.HighlightDifferencesFromDeviceFileSystem(fileSystem, SyncMode);
                        }
                        try
                        {
                            var model = new MenuLayout();
                            try
                            {
                                model = new MenuLayout(fileSystem, ActiveLtoFlashDevice.Device.UniqueId);
                            }
                            catch (System.Exception exception)
                            {
                                corruptFileSystem = exception;
                            }
                            if ((fileSystem.Status != LfsDirtyFlags.None) || (corruptFileSystem != null))
                            {
                                if (reportedProblems)
                                {
                                    // already reported the problem, so clear the flag
                                    fileSystem.Status = LfsDirtyFlags.None;
                                }
                                else
                                {
                                    reportedProblems = DeviceViewModel.ReportFileSystemInconsistencies(fileSystem, corruptFileSystem);
                                }
                            }
                            Model = model;
                        }
                        catch (InconsistentFileSystemException)
                        {
                            if (!reportedProblems)
                            {
                                reportedProblems = DeviceViewModel.ReportFileSystemInconsistencies(fileSystem, null);
                            }
                        }
                        RaisePropertyChanged(FileNode.LongNamePropertyName);
                        RaisePropertyChanged("Root");
                        UpdateSystemContentsUsage(fileSystem.Directories);
                        UpdateSystemContentsUsage(fileSystem.Files);
                        UpdateSystemContentsUsage(fileSystem.Forks);
                    }
                    else
                    {
                        Model = EmptyMenuLayout;
                    }
                    UpdateFileSystemsInSync(true); // always do after file system changes
                    break;
                case Device.FileSystemStatisticsPropertyName:
                    FileSystemStatistics.FileSystemStatistics = ActiveLtoFlashDevice.Device.FileSystemStatistics;
                    break;
                case Device.FirmwareRevisionsPropertyName:
                    FirmwareRevisions.FirmwareRevisions = ActiveLtoFlashDevice.IsValid ? ActiveLtoFlashDevice.Device.FirmwareRevisions : null;
                    break;
                case Device.OwnerPropertyName:
                case Device.CustomNamePropertyName:
                    break;
                default:
                    break;
            }
        }

        // TODO: MOVE OFF-THREAD!
        private void UpdateFileSystemsInSync(bool doFileSystemCompare)
        {
            var showFileSystemsDifferIcon = _cachedFileSystemsCompareResult.HasValue && (_cachedFileSystemsCompareResult.Value != 0);
            if (!_cachedFileSystemsCompareResult.HasValue)
            {
                if (doFileSystemCompare && ActiveLtoFlashDevice.IsValid && (ActiveLtoFlashDevice.Device.FileSystem != null))
                {
                    var deviceFileSystem = ActiveLtoFlashDevice.Device.FileSystem; // .Clone();
                    var hostFileSystem = HostPCMenuLayout.MenuLayout.FileSystem.Clone();
                    hostFileSystem.SuppressRootFileNameDifferences(ActiveLtoFlashDevice.Device); // TODO: Instead of this, offer option to ignore file name compares on root?
                    hostFileSystem.RemoveMenuPositionData();
                    hostFileSystem.PopulateSaveDataForksFromDevice(deviceFileSystem);
#if USE_SIMPLE_FILE_SYSTEM_COMPARE
                    // TODO: If this is still too slow, farm off to another thread!
                    deviceFileSystem.RemoveMenuPositionData();
                    _cachedFileSystemsCompareResult = hostFileSystem.SimpleCompare(deviceFileSystem, ActiveLtoFlashDevice.Device);
                    showFileSystemsDifferIcon = _cachedFileSystemsCompareResult.Value != 0;
#else
                    var deviceSaveMenuPositionFork = deviceFileSystem.RemoveMenuPositionData();
                    var differences = hostFileSystem.CompareTo(deviceFileSystem, ActiveLtoFlashDevice.Device, true); // TODO: reevaluate if this is needed
                    if (deviceSaveMenuPositionFork != null)
                    {
                        deviceFileSystem.SetMenuPositionData(deviceSaveMenuPositionFork);
                    }

                    // Now, ignore things due to incompatibilities.
                    if (differences.GetAllFailures(null).Any())
                    {
                        // HERE: Instead of cloning and doing everything all over again, how about, as invalid entries are discovered, scoop out things from the *already in-hand diff!* :B
                        hostFileSystem = hostFileSystem.Clone();
                        hostFileSystem.CleanUpInvalidEntries(deviceFileSystem, differences, FileSystemHelpers.ShouldRemoveInvalidEntry, null);
                        differences = hostFileSystem.CompareTo(deviceFileSystem, ActiveLtoFlashDevice.Device, false); // TODO: reevaluate if this is OK - ideally unnecessary!
                    }
                    showFileSystemsDifferIcon = differences.Any();
#endif // USE_SIMPLE_FILE_SYSTEM_COMPARE
                }
                else if (!ActiveLtoFlashDevice.IsValid || (ActiveLtoFlashDevice.Device.FileSystem == null))
                {
                    ResetCachedFileSystemsCompareResult();
                    showFileSystemsDifferIcon = false;
                }
            }
            ShowFileSystemsDifferIcon = showFileSystemsDifferIcon;
        }

        private void UpdateItemStatesAsNecessary(DeviceViewModel newDevice)
        {
            if ((newDevice != null) && newDevice.IsValid)
            {
                var ltoFlashOnlyRoms = HostPCMenuLayout.FindChildren((p) => (p is ProgramViewModel) && ((ProgramViewModel)p).ProgramDescription.Rom.IsLtoFlashOnlyRom(), true).OfType<ProgramViewModel>();
                foreach (var ltoFlashOnlyRom in ltoFlashOnlyRoms)
                {
                    ltoFlashOnlyRom.RefreshValidationState(AttachedPeripherals);
                }
                OSOnActiveDeviceChanged();
            }
            if ((newDevice == null) || !newDevice.IsValid)
            {
                HostPCMenuLayout.ClearItemStates(AttachedPeripherals);
            }
            UpdateFileSystemsInSync(Properties.Settings.Default.ReconcileDeviceMenuWithLocalMenu);
        }

        private void PromptForFirmwareUpgrade(DeviceViewModel newDevice)
        {
            var firmwareUpdatePrefix = "INTV.LtoFlash.Resources.FirmwareUpdates.";
            var embeddedFirmwareUpdates = typeof(LtoFlashViewModel).GetResources(firmwareUpdatePrefix);
            typeof(LtoFlashViewModel).ExtractResourcesToFiles(embeddedFirmwareUpdates, firmwareUpdatePrefix, Configuration.Instance.FirmwareUpdatesDirectory);
            if (newDevice.IsValid && Properties.Settings.Default.PromptForFirmwareUpgrade && FirmwareCommandGroup.UpdateFirmwareCommand.CanExecute(this))
            {
                if (System.IO.Directory.Exists(Configuration.Instance.FirmwareUpdatesDirectory))
                {
                    string newestFirmwareFile = null;
                    var newestVersion = INTV.LtoFlash.Model.FirmwareRevisions.UnavailableFirmwareVersion;
                    foreach (var firmwareUpdateFile in System.IO.Directory.EnumerateFiles(Configuration.Instance.FirmwareUpdatesDirectory, "*.*").Where(f => FirmwareCommandGroup.UpgradeFileExtensions.Contains(System.IO.Path.GetExtension(f))))
                    {
                        var isValid = false;
                        var version = FirmwareCommandGroup.ExtractFirmwareUpdateVersion(firmwareUpdateFile, out isValid);
                        if (isValid && (version > newestVersion))
                        {
                            newestVersion = version;
                            newestFirmwareFile = firmwareUpdateFile;
                        }
                    }

                    // Strip off the secondary FW version bit -- we don't really care about that.
                    var currentVersion = newDevice.Device.FirmwareRevisions.Current & ~INTV.LtoFlash.Model.FirmwareRevisions.SecondaryMask;
                    if ((newestVersion > currentVersion) && FirmwareCommandGroup.UpdateFirmwareCommand.CanExecute(this))
                    {
                        var newerFirmwareFound = Resources.Strings.FirmwareUpdateAvailable_MessagePrefix;
                        var disablePrompt = Resources.Strings.FirmwareUpdateAvailable_MessageSuffix;
                        SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() =>
                            {
                                if (FirmwareCommandGroup.UpdateFirmwareCommand.CanExecute(this))
                                {
                                    FirmwareCommandGroup.UpdateFirmware(newDevice.Device, newestFirmwareFile, newerFirmwareFound, disablePrompt);
                                }
                            });
                    }
                }
            }
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
            }
            int numEntriesUsed = collection.ItemsInUse;
            int totalNumEntries = collection.Size;
            int numEntriesRemaining = collection.ItemsRemaining;
            var status = string.Format(System.Globalization.CultureInfo.CurrentCulture, formatString, numEntriesUsed, totalNumEntries, numEntriesRemaining);
            string propertyChangedName = null;
            if (isFolderTable)
            {
                FolderCount = status;
                propertyChangedName = "FolderCount";
            }
            else if (isFileTable)
            {
                FileCount = status;
                propertyChangedName = "FileCount";
            }
            else if (isForkTable)
            {
                ForkCount = status;
                propertyChangedName = "ForkCount";
            }
            RaisePropertyChanged(propertyChangedName);
        }

        private void HandleApplicationExit(object sender, ExitEventArgs e)
        {
            HandleApplicationExit(sender, (System.EventArgs)e);
        }

        private void HandleApplicationExit(object sender, System.EventArgs e)
        {
            var devicesToDisconnect = Devices.ModelCollection.ToArray(); // On Mac during shutdown, collection gets modified
            foreach (var device in devicesToDisconnect)
            {
                device.Disconnect(true);
            }
        }

        // UNDONE Not sure what the plan was here...
        private void HandlePreferenceChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReconcileDeviceMenuWithLocalMenu")
            {
                System.Diagnostics.Debug.WriteLine("ReconcileDeviceMenuWithLocalMenu CHANGED");
            }
            if (e.PropertyName == "VerifyVIDandPIDBeforeConnecting")
            {
                var newPotentialPorts = DeviceConnectionViewModel.GetAvailableConnections(this);
                var itemsToRemove = _potentialDevicePorts.Where(p => !newPotentialPorts.Select(n => n.Name).Contains(p.Name)).ToList();
                foreach (var itemToRemove in itemsToRemove)
                {
                    _potentialDevicePorts.Remove(itemToRemove);
                }
                var itemsToAdd = newPotentialPorts.Where(n => !_potentialDevicePorts.Select(p => p.Name).Contains(n.Name)).ToList();
                foreach (var itemToAdd in itemsToAdd)
                {
                    _potentialDevicePorts.Add(itemToAdd);
                }
            }
        }

        /// <summary>
        /// Called from the constructor to perform any OS-specific initialization.
        /// </summary>
        partial void OSInitialize();

        /// <summary>
        /// Called when the active device changes to a non-null value to take any OS-specific action.
        /// </summary>
        partial void OSOnActiveDeviceChanged();

        /// <summary>
        /// Called when a potential device arrives or departs, or active device changes.
        /// </summary>
        partial void OSDeviceArrivalDepartureActiveChanged();

        /// <summary>
        /// OS-specific implementation to create the visuals.
        /// </summary>
        partial void OSCreateVisuals();

#if DEBUG
#if WIN

#endif // Win

#endif // DEBUG

#if ENABLE_ROMS_PATCH
        /// <summary>
        /// Reset JLP, LTO Flash!, Bee3, and Hive flags to "Incompatible" as defaults.
        /// </summary>
        private class FixJlpAndOtherFlagsInMenu : OneShotLaunchTask
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.LtoFlash.ViewModel.LtoFlashViewModel+FixJlpAndOtherFlagsInMenu"/> class.
            /// </summary>
            /// <param name="ltoFlashViewModel">LTO Flash view model.</param>
            public FixJlpAndOtherFlagsInMenu(LtoFlashViewModel ltoFlashViewModel)
                : base("FixJlpAndOtherFlagsInMenu")
            {
                LtoFlashViewModel = ltoFlashViewModel;
            }

            private LtoFlashViewModel LtoFlashViewModel { get; set; }

            /// <inheritdoc/>
            protected override void Run()
            {
                DeviceCommandGroup.ClearCache(null);
                var menuLayout = LtoFlashViewModel.HostPCMenuLayout.MenuLayout;
                var files = menuLayout.FileSystem.Files;
                foreach (var file in files)
                {
                    var program = file as Program;
                    if (program != null)
                    {
                        if (program.Description != null)
                        {
                            program.Description.Features.Jlp = JlpFeaturesHelpers.Default;
                            program.Description.Features.LtoFlash = LtoFlashFeaturesHelpers.Default;
                            program.Description.Features.Bee3 = Bee3FeaturesHelpers.Default;
                            program.Description.Features.Hive = HiveFeaturesHelpers.Default;
                        }
                    }
                }
                menuLayout.Save(Configuration.Instance.MenuLayoutPath);
                LtoFlashViewModel._fixRoms = null;
                LtoFlashViewModel = null;
            }
        }

        /// <summary>
        /// Patch up the misspelled XML element name for .cfg files.
        /// </summary>
        private class FixCfgFilesPatchInMenu : OneShotLaunchTask
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.LtoFlash.ViewModel.LtoFlashViewModel+FixCfgFilesPatchInMenu"/> class.
            /// </summary>
            /// <param name="ltoFlashViewModel">LTO Flash view model.</param>
            public FixCfgFilesPatchInMenu(LtoFlashViewModel ltoFlashViewModel)
                : base("FixCfgFilesPatchInMenu")
            {
                LtoFlashViewModel = ltoFlashViewModel;
            }

            private LtoFlashViewModel LtoFlashViewModel { get; set; }

            /// <inheritdoc/>
            protected override void Run()
            {
                DeviceCommandGroup.ClearCache(null);
                var menuLayout = LtoFlashViewModel.HostPCMenuLayout.MenuLayout;
                var files = menuLayout.FileSystem.Files;
                foreach (var file in files)
                {
                    var program = file as Program;
                    var rom = (program != null) && (program.Description != null) ? program.Description.Rom : null;
                    if ((rom != null) && (rom.Format == INTV.Core.Model.RomFormat.Bin))
                    {
                        if (string.IsNullOrEmpty(rom.ConfigPath))
                        {
                            var cfgFilePath = System.IO.Path.ChangeExtension(rom.RomPath, ProgramFileKind.CfgFile.FileExtension());
                            if (!System.IO.File.Exists(cfgFilePath))
                            {
                                // try stock .cfg file
                                cfgFilePath = rom.GetStockCfgFile(program.Description.ProgramInformation);
                            }
                            if (System.IO.File.Exists(cfgFilePath))
                            {
                                INTV.Core.Model.Rom.ReplaceCfgPath(rom, cfgFilePath);
                            }
                        }
                    }
                }
                menuLayout.Save(Configuration.Instance.MenuLayoutPath);
                LtoFlashViewModel._fixCfgFiles = null;
                LtoFlashViewModel = null;
            }
        }
#endif // ENABLE_ROMS_PATCH
    }
}

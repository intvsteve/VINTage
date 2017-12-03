// <copyright file="Device.cs" company="INTV Funhouse">
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

////#define ENABLE_DIAGNOSTIC_OUTPUT
////#define ENABLE_PING_ONLY_OPTION
////#define REPORT_PERFORMANCE

using System;
using System.Collections.Generic;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Defines a model for a Locutus device.
    /// </summary>
    public sealed class Device : INTV.Core.Model.Device.Peripheral, IDisposable
    {
        /// <summary>
        /// This delegate can be used to report errors to an interested party that occur for various reasons.
        /// </summary>
        /// <param name="deviceStatusFlags">Errors related to setting device flags are identified here.</param>
        /// <param name="commandId">The low-level protocol command that failed.</param>
        /// <param name="errorMessage">An error message describing the nature of the failure.</param>
        /// <param name="exception">The exception that caused the error, if applicable.</param>
        /// <returns><c>true</c> if the error was handled and should not be passed along.</returns>
        public delegate bool DeviceErrorHandler(DeviceStatusFlagsLo deviceStatusFlags, ProtocolCommandId commandId, string errorMessage, Exception exception);

        #region Constants

        /// <summary>
        /// The USB vendor ID for LTO Flash!.
        /// </summary>
        /// <remarks>On Mac, use this value with the idVendor for IOKit operations.</remarks>
        public const int UsbVendorId = 0x403;

        /// <summary>
        /// The USB product ID for LTO Flash!.
        /// </summary>
        /// <remarks>On Mac, use this value with idProduct for IOKit operations.</remarks>
        public const int UsbProductId = 0x6015;

        /// <summary>
        /// The USB vendor name for LTO Flash!.
        /// </summary>
        /// <remarks>On Mac, use this value when verifying a USB device's "USB Vendor Name" in IOKit.</remarks>
        public const string UsbVendorName = "Left Turn Only"; // "USB Vendor Name"

        /// <summary>
        /// The USB product name for LTO Flash!.
        /// </summary>
        /// <remarks>On Mac, use this value when verifying a USB device's "USB Product Name", "Product Name", "USB Interface Name", IORegistryEntryName, or IOObjectClass in IOKit.</remarks>
        public const string UsbProductName = "LTO Flash!";

        #region Property Names

        public const string PortPropertyName = "Port";
        public const string CustomNamePropertyName = "CustomName";
        public const string OwnerPropertyName = "Owner";
        public const string ConnectionStatePropertyName = "ConnectionState";
        public const string UniqueIdPropertyName = "UniqueId";
        public const string IsIntellivisionConnectedPropertyName = "IsIntellivisionConnected";
        public const string FileSystemPropertyName = "FileSystem";
        public const string IsValidPropertyName = "IsValid";
        public const string HardwareStatusPropertyName = "HardwareStatus";
        public const string FileSystemStatisticsPropertyName = "FileSystemStatistics";
        public const string FileSystemStatusPropertyName = "FileSystemStatus";
        public const string EcsCompatibilityPropertyName = "EcsCompatibility";
        public const string IntvIICompatibilityPropertyName = "IntvIICompatibility";
        public const string FirmwareRevisionsPropertyName = "FirmwareRevisions";
        public const string ErrorLogPropertyName = "ErrorLog";
        public const string CrashLogPropertyName = "CrashLog";
        public const string ShowTitleScreenPropertyName = "ShowTitleScreen";
        public const string SaveMenuPositionPropertyName = "SaveMenuPosition";
        public const string BackgroundGCPropertyName = "BackgroundGC";
        public const string KeyclicksPropertyName = "Keyclicks";
        public const string DeviceStatusUpdatePeriodPropertyName = "DeviceStatusUpdatePeriod";

        #endregion // Property Names

        /// <summary>
        /// How often to execute garbage collect commands on the device's file system (in milliseconds).
        /// </summary>
        public const int DefaultGarbageCollectionPeriod = 1000;

        /// <summary>
        /// How often to ping the device (in milliseconds), if garbage collection is turned off.
        /// </summary>
        public const int DefaultPingPeriod = 1000;

        /// <summary>
        /// How long to wait between wait-for-beacon attempts.
        /// </summary>
        public const int DefaultBeaconPeriod = 500;

        /// <summary>
        /// How long to wait when system is idle before resuming with a WaitForBeacon. This is usually the
        /// state if the UI is displaying an error dialog after a command has failed.
        /// </summary>
        internal const int IdlePeriod = 150;

        /// <summary>
        /// Total RAM available on the device.
        /// </summary>
        internal const int TotalRAMSize = 0x00100000;

        /// <summary>
        /// Default baud rate to use for the connection.
        /// </summary>
        internal const int DefaultBaudRate = 2000000; // 230400; // Production baud rate: 2000000

        /// <summary>
        /// The handshake mode to use for the serial port.
        /// </summary>
        internal const System.IO.Ports.Handshake Handshake = System.IO.Ports.Handshake.RequestToSend;

        #endregion // Constants

        /// <summary>
        /// The name of the peripheral.
        /// </summary>
        public static readonly string PeripheralName = "LTO Flash!";

        /// <summary>
        /// Beacon string broadcast by the device as a recognition value.
        /// </summary>
        public static readonly string Beacon = "LOCUTUS\n";

        /// <summary>
        /// The total flash storage space (32 MB).
        /// </summary>
        public static readonly int TotalFlashStorageSpace = 0x02000000;

        /// <summary>
        /// Beacon string as an array of characters.
        /// </summary>
        public static readonly char[] BeaconCharacters = Beacon.ToCharArray();

        private static bool _infoUpdatePosted;

        private object _lock = new object();
        private bool _dummyDevice;

        #region Constructors

        /// <summary>
        /// Creates a dummy device used for device history purposes.
        /// </summary>
        /// <param name="uniqueId">Cached uniqueId of a device.</param>
        /// <remarks>The dummy mode is fragile, and should only be used for device history purposes. Setting any properties will simply fail.</remarks>
        internal Device(string uniqueId)
        {
            UniqueId = uniqueId;
            _dummyDevice = true;
        }

        private Device(IStreamConnection port, DeviceCreationInfo creationInfo)
        {
            Properties.Settings.Default.PropertyChanged += SettingsPropertyChanged;
            Name = Resources.Strings.DeviceMultistageCommand_Validating_Title;
            CustomName = Name;
            UniqueId = "LTO Flash-" + port.Name; // default
            CreationInfo = creationInfo;
            CommandAvailability = new DeviceCommandAvailability();
            if (((Connection)port).Type != ConnectionType.Serial)
            {
                CommandAvailability.ChangeCommandAvailablility(ProtocolCommandId.DownloadErrorLog, HardwareStatusFlags.AllFlags, Core.Model.Program.FeatureCompatibility.Requires);
                CommandAvailability.ChangeCommandAvailablility(ProtocolCommandId.FirmwareGetRevisions, HardwareStatusFlags.AllFlags, Core.Model.Program.FeatureCompatibility.Requires);
            }
            ShouldOpenPort = true;
            Port = port;
            UpdateLogger();
            DeviceStatusUpdatePeriod = DefaultGarbageCollectionPeriod;
            _firmwareRevisions = FirmwareRevisions.Unavailable;
            ValidateDevice();
            _showTitleScreen = ShowTitleScreenFlags.Default;
            _keyclicks = false;
            _backgroundGC = true;
            _saveMenuPosition = SaveMenuPositionFlags.Default;
            DeviceStatusFlagsHi = DeviceStatusFlagsHi.Default;
            UpdateFileSystemStatsDuringHeartbeat = Properties.Settings.Default.ShowFileSystemDetails;
        }

        ~Device()
        {
            Dispose(false);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the device's unique hardware identifier.
        /// </summary>
        public string UniqueId { get; private set; }

        /// <summary>
        /// Gets or sets the user-personalized name for the device.
        /// </summary>
        public string CustomName
        {
            get { return _customName; }
            set { AssignAndUpdateProperty(CustomNamePropertyName, value, ref _customName); }
        }
        private string _customName;

        /// <summary>
        /// Gets or sets the owner's name.
        /// </summary>
        public string Owner
        {
            get { return _owner; }
            set { AssignAndUpdateProperty(OwnerPropertyName, value, ref _owner); }
        }
        private string _owner;

        /// <summary>
        /// Gets the port to use to communicate with the device.
        /// </summary>
        public IStreamConnection Port { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the device is connected plugged into a powered-on Intellivision unit while connected to a PC.
        /// </summary>
        public bool IsConnectedToIntellivision
        {
            get { return HardwareStatus.HasFlag(HardwareStatusFlags.ConsolePowerOn); }
        }

        /// <summary>
        /// Gets a value reporting the last known hardware status.
        /// </summary>
        public HardwareStatusFlags HardwareStatus { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating how the Locutus device treats programs with known ECS compatibility problems.
        /// </summary>
        public EcsStatusFlags EcsCompatibility
        {
            get { return _ecsCompatibility; }
            set { AssignAndUpdateProperty(EcsCompatibilityPropertyName, value, ref _ecsCompatibility, (p, v) => UpdateEcsConfiguration(v, true)); }
        }
        private EcsStatusFlags _ecsCompatibility;

        /// <summary>
        /// Gets or sets a value indicating how the Locutus device treats programs with known Intellivision II compatibility problems.
        /// </summary>
        public IntellivisionIIStatusFlags IntvIICompatibility
        {
            get { return _intvIICompatibility; }
            set { AssignAndUpdateProperty(IntvIICompatibilityPropertyName, value, ref _intvIICompatibility, (p, v) => UpdateIntellivisionIIConfiguration(v, true)); }
        }
        private IntellivisionIIStatusFlags _intvIICompatibility;

        /// <summary>
        /// Gets or sets a value indicating when the Locutus device shows its titles screen when rebooting.
        /// </summary>
        public ShowTitleScreenFlags ShowTitleScreen
        {
            get { return _showTitleScreen; }
            set { AssignAndUpdateProperty(ShowTitleScreenPropertyName, value, ref _showTitleScreen, (p, v) => UpdateShowTitleScreen(v, true)); }
        }
        private ShowTitleScreenFlags _showTitleScreen;

        /// <summary>
        /// Gets or sets a value indicating how the Locutus device saves menu position data.
        /// </summary>
        public SaveMenuPositionFlags SaveMenuPosition
        {
            get { return _saveMenuPosition; }
            set { AssignAndUpdateProperty(SaveMenuPositionPropertyName, value, ref _saveMenuPosition, (p, v) => UpdateSaveMenuPosition(v, true)); }
        }
        private SaveMenuPositionFlags _saveMenuPosition;

        /// <summary>
        /// Gets or sets a value indicating whether the Locutus device runs background garbage collection at the menu screen.
        /// </summary>
        public bool BackgroundGC
        {
            get { return _backgroundGC; }
            set { AssignAndUpdateProperty(BackgroundGCPropertyName, value, ref _backgroundGC, (p, v) => UpdateBackgroundGC(v, true)); }
        }
        private bool _backgroundGC;

        /// <summary>
        /// Gets or sets a value indicating whether the Locutus device makes 'key click' sounds when navigating the menu.
        /// </summary>
        public bool Keyclicks
        {
            get { return _keyclicks; }
            set { AssignAndUpdateProperty(KeyclicksPropertyName, value, ref _keyclicks, (p, v) => UpdateKeyclicks(v, true)); }
        }
        private bool _keyclicks;

        /// <summary>
        /// Gets a value indicating whether this is a valid Locutus device.
        /// </summary>
        public bool IsValid
        {
            get { return _isLtoFlashDevice; }
            private set { AssignAndUpdateProperty(IsValidPropertyName, value, ref _isLtoFlashDevice, (p, v) => IsValidChanged(v)); }
        }
        private bool _isLtoFlashDevice;

        /// <summary>
        /// Gets the file system status flags of the device.
        /// </summary>
        public LfsDirtyFlags FileSystemFlags
        {
            get { return _fileSystemStatusFlags; }
            internal set { AssignAndUpdateProperty(FileSystemStatusPropertyName, value, ref _fileSystemStatusFlags); }
        }
        private LfsDirtyFlags _fileSystemStatusFlags;

        /// <summary>
        /// Gets the file system usage statistics. Note that this merely reads the cached value.
        /// </summary>
        public FileSystemStatistics FileSystemStatistics
        {
            get { return _fileSystemStatistics; }
            internal set { AssignAndUpdateProperty(FileSystemStatisticsPropertyName, value, ref _fileSystemStatistics); }
        }
        private FileSystemStatistics _fileSystemStatistics;

        /// <summary>
        /// Gets the file system tables from the device.
        /// </summary>
        public FileSystem FileSystem
        {
            get
            {
                return _fileSystem;
            }

            internal set
            {
                if (value.SimpleCompare(_fileSystem, null) != 0)
                {
                    AssignAndUpdateProperty(FileSystemPropertyName, value, ref _fileSystem, (p, v) => UpdateNameAndOwner(v));
                }
            }
        }
        private FileSystem _fileSystem;

        /// <summary>
        /// Gets the device firmware revision information.
        /// </summary>
        public FirmwareRevisions FirmwareRevisions
        {
            get { return _firmwareRevisions; }
            internal set { AssignAndUpdateProperty(FirmwareRevisionsPropertyName, value ?? FirmwareRevisions.Unavailable, ref _firmwareRevisions, (p, v) => UpdateFirmwareVersion(v)); }
        }
        private FirmwareRevisions _firmwareRevisions;

        /// <summary>
        /// Gets the last error log retrieved from the device.
        /// </summary>
        public ErrorLog ErrorLog
        {
            get { return _errorLog; }
            internal set { AssignAndUpdateProperty(ErrorLogPropertyName, value, ref _errorLog); }
        }
        private ErrorLog _errorLog;

        /// <summary>
        /// Gets the last crash log retrieved from the device.
        /// </summary>
        public CrashLog CrashLog
        {
            get { return _crashLog; }
            internal set { AssignAndUpdateProperty(CrashLogPropertyName, value, ref _crashLog); }
        }
        private CrashLog _crashLog;

        /// <summary>
        /// Gets or sets a custom error handler, typically installed by the ViewModel.
        /// </summary>
        internal DeviceErrorHandler ErrorHandler { get; set; }

        /// <summary>
        /// Gets or sets how often the GarbageCollect command is sent to the device.
        /// </summary>
        /// <remarks>The value is how long to wait between reissues of the command, in milliseconds.</remarks>
        internal int DeviceStatusUpdatePeriod
        {
            get { return _garbageCollectorPeriod; }
            set { AssignAndUpdateProperty(DeviceStatusUpdatePeriodPropertyName, value, ref _garbageCollectorPeriod, (p, v) => UpdateTimerPeriod(v)); }
        }
        private int _garbageCollectorPeriod;

        /// <summary>
        /// Gets or sets a value indicating whether the device is in the midst of handling the 'heartbeat' timer.
        /// </summary>
        /// <remarks>NOTE: Trying this without using a lock. The only setter is the timer proc itself.</remarks>
        internal bool InTimer
        {
            get
            {
                return _inTimer > 0;
            }

            set
            {
                if (value)
                {
                    ++_inTimer;
                }
                else
                {
                    _inTimer = System.Math.Max(--_inTimer, 0);
                }
            }
        }
        private int _inTimer;

        /// <summary>
        /// Gets or sets a value indicating whether a command is being executed.
        /// </summary>
        internal bool IsCommandInProgress
        {
            get
            {
                lock (_lock)
                {
                    return (_commandNestingLevel > 0) || (ConnectionState == Model.ConnectionState.WaitForBeacon);
                }
            }
            set
            {
                lock (_lock)
                {
                    if (value)
                    {
                        ++_commandNestingLevel;
                    }
                    else
                    {
                        --_commandNestingLevel;
                        _commandNestingLevel = System.Math.Max(0, _commandNestingLevel);
                    }
                }
            }
        }
        private int _commandNestingLevel;

        /// <summary>
        /// Gets or sets the data determining how to behave during device creation, initialization and validation.
        /// </summary>
        internal DeviceCreationInfo CreationInfo { get; set; }

        /// <summary>
        /// Gets or sets the connection state of the device.
        /// </summary>
        internal ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { AssignAndUpdateProperty(ConnectionStatePropertyName, value, ref _connectionState, (p, v) => OnConnectionStateChanged(v)); }
        }
        private ConnectionState _connectionState;

        /// <summary>
        /// Gets or sets a value indicating whether the heartbeat actions also collect file system information.
        /// </summary>
        internal bool UpdateFileSystemStatsDuringHeartbeat { get; set; }

        private DeviceStatusFlagsLo DeviceStatusFlagsLo
        {
            get { return this.ComposeStatusFlags(); }
        }

        private DeviceStatusFlagsHi DeviceStatusFlagsHi { get; set; }

        private bool ShouldOpenPort { get; set; }

        private DeviceCommandAvailability CommandAvailability { get; set; }

#if DEBUG

        private bool GenerateFakeFirmwareCrash
        {
            get
            {
                lock (_lock)
                {
                    var generateFakeFirmwareCrash = _generateFakeFirmwareCrash;
                    _generateFakeFirmwareCrash = false;
                    return generateFakeFirmwareCrash;
                }
            }

            set
            {
                lock (_lock)
                {
                    _generateFakeFirmwareCrash = value;
                }
            }
        }
        private bool _generateFakeFirmwareCrash;

#endif // DEBUG

        #region IPeripheral

        /// <summary>
        /// Gets the connections that this peripheral supports.
        /// </summary>
        public override IEnumerable<INTV.Core.Model.Device.IConnection> Connections
        {
            get
            {
                yield return Port as IConnection;
            }
            protected set
            {
                throw new InvalidOperationException();
            }
        }

        #endregion // IPeripheral

        #endregion // Properties

        /// <summary>
        /// Factory function to create a new Locutus device model from a connection.
        /// </summary>
        /// <param name="connection">The communication conduit to the device.</param>
        /// <param name="state">State information. In this case, whether the device has already been verified.</param>
        /// <returns>A new instance of the Locutus model, if it is a supported connection type.</returns>
        public static IPeripheral GetLtoFlashDevice(IConnection connection, object state)
        {
            Device ltoFlash = null;
            var port = connection as IStreamConnection;
            var validConnection = false;
            switch (connection.Type)
            {
                case ConnectionType.Serial:
                    // We're essentially skipping the other potential implementations of sharing policy and smugly assuming that if *ours* says it's OK, then just do it.
                    validConnection = !Properties.Settings.Default.VerifyVIDandPIDBeforeConnecting || SerialConnectionPolicy.Instance.ExclusiveAccess(connection);
                    break;
                case ConnectionType.NamedPipe:
                    validConnection = true; // skipping validation
                    break;
            }
            if (validConnection)
            {
                var creationInfo = state as DeviceCreationInfo;
                if (creationInfo == null)
                {
                    var stateDictionaryData = state as Dictionary<string, object>;
                    object creationInfoObject = null;
                    if ((stateDictionaryData != null) && stateDictionaryData.TryGetValue(DeviceCreationInfo.ConfigName, out creationInfoObject))
                    {
                        creationInfo = (DeviceCreationInfo)creationInfoObject;
                    }
                }
                if (creationInfo == null)
                {
                    creationInfo = new DeviceCreationInfo(Properties.Settings.Default.AutomaticallyConnectToDevices, false, Properties.Settings.Default.AutomaticallyConnectToDevices ? ActivationMode.ActivateIfFirst : ActivationMode.UserSettings);
                }
                ltoFlash = new Device(port, creationInfo);
            }
            return ltoFlash;
        }

        #region IPeripheral

        /// <inheritdoc />
        public override bool IsRomCompatible(IProgramDescription programDescription)
        {
            var isCompatible = true;
            if (isCompatible && programDescription.Rom.IsLtoFlashOnlyRom())
            {
                isCompatible = programDescription.Rom.CanExecuteOnDevice(UniqueId);
            }
            return isCompatible;
        }

        #endregion IPeripheral

        /// <summary>
        /// Closes the device's communication port, rendering the device inoperable.
        /// </summary>
        /// <param name="isApplictionExiting">If <c>true</c>, informs the device that the application is about to exit.</param>
        /// <remarks>A device typically disconnects if the connection goes bad, or the application is about to exit. Also,
        /// a new connection will be probed to identify whether the hardware at the other end is a Locutus device. If it is
        /// not, the connection will be closed as well.</remarks>
        public void Disconnect(bool isApplictionExiting)
        {
            StopTimer();
            if ((Port != null) && Port.IsOpen)
            {
                Port.Close();
            }
            IsValid = false;
            ConnectionState = ConnectionState.Disconnected;
            Name = Resources.Strings.StatusBar_NoDevice;
            CustomName = Name;
            ShouldOpenPort = !isApplictionExiting;
            if (Port != null)
            {
                var configData = new Dictionary<string, object>() { { DeviceCreationInfo.ConfigName, CreationInfo } };
                INTV.Shared.Interop.DeviceManagement.DeviceChange.ReportDeviceRemoved(null, Port.Name, ((INTV.Core.Model.Device.Connection)Port).Type, configData);
            }
        }

        public bool AllCommandsAvailable(IEnumerable<ProtocolCommandId> commands)
        {
            bool validState = false;
            switch (ConnectionState)
            {
                case ConnectionState.Ping:
                case ConnectionState.GarbageCollect:
                    validState = true;
                    break;
            }
            return validState && CommandAvailability.AllProtocolCommandsAvailable(commands, HardwareStatus);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable

        /// <summary>
        /// Updates the ECS configuration.
        /// </summary>
        /// <param name="newEcsConfiguration">New ECS configuration.</param>
        /// <param name="sendToHardware">If set to <c>true</c> send to Locutus.</param>
        internal void UpdateEcsConfiguration(EcsStatusFlags newEcsConfiguration, bool sendToHardware)
        {
            if (sendToHardware)
            {
                var newConfigurationLo = this.UpdateStatusFlags(newEcsConfiguration);
                this.SetConfiguration(newConfigurationLo, DeviceStatusFlagsHi, (m, e) => ErrorHandler(DeviceStatusFlagsLo.EcsStatusMask, ProtocolCommandId.SetConfiguration, m, e));
            }
            else
            {
                if (_ecsCompatibility != newEcsConfiguration)
                {
                    _ecsCompatibility = newEcsConfiguration;
                    RaisePropertyChanged(EcsCompatibilityPropertyName);
                }
            }
        }

        /// <summary>
        /// Updates the Intellivision II configuration.
        /// </summary>
        /// <param name="newIntellivisionIIConfiguration">New Intellivision II configuration.</param>
        /// <param name="sendToHardware">If set to <c>true</c> send to Locutus.</param>
        internal void UpdateIntellivisionIIConfiguration(IntellivisionIIStatusFlags newIntellivisionIIConfiguration, bool sendToHardware)
        {
            if (sendToHardware)
            {
                var newConfigurationLo = this.UpdateStatusFlags(newIntellivisionIIConfiguration);
                this.SetConfiguration(newConfigurationLo, DeviceStatusFlagsHi, (m, e) => ErrorHandler(DeviceStatusFlagsLo.IntellivisionIIStatusMask, ProtocolCommandId.SetConfiguration, m, e));
            }
            else
            {
                if (_intvIICompatibility != newIntellivisionIIConfiguration)
                {
                    _intvIICompatibility = newIntellivisionIIConfiguration;
                    RaisePropertyChanged(IntvIICompatibilityPropertyName);
                }
            }
        }

        /// <summary>
        /// Updates the show title screen setting.
        /// </summary>
        /// <param name="newShowTitleScreen">New show title screen setting.</param>
        /// <param name="sendToHardware">If set to <c>true</c> send to Locutus.</param>
        internal void UpdateShowTitleScreen(ShowTitleScreenFlags newShowTitleScreen, bool sendToHardware)
        {
            if (sendToHardware)
            {
                var newConfigurationLo = this.UpdateStatusFlags(newShowTitleScreen);
                this.SetConfiguration(newConfigurationLo, DeviceStatusFlagsHi, (m, e) => ErrorHandler(DeviceStatusFlagsLo.ShowTitleScreenMask, ProtocolCommandId.SetConfiguration, m, e));
            }
            else
            {
                if (_showTitleScreen != newShowTitleScreen)
                {
                    _showTitleScreen = newShowTitleScreen;
                    RaisePropertyChanged(ShowTitleScreenPropertyName);
                }
            }
        }

        /// <summary>
        /// Updates the setting to save menu position.
        /// </summary>
        /// <param name="newSaveMenuPosition">New save menu position setting.</param>
        /// <param name="sendToHardware">If set to <c>true</c> send to Locutus.</param>
        internal void UpdateSaveMenuPosition(SaveMenuPositionFlags newSaveMenuPosition, bool sendToHardware)
        {
            if (sendToHardware)
            {
                var newConfigurationLo = this.UpdateStatusFlags(newSaveMenuPosition);
                this.SetConfiguration(newConfigurationLo, DeviceStatusFlagsHi, (m, e) => ErrorHandler(DeviceStatusFlagsLo.SaveMenuPositionMask, ProtocolCommandId.SetConfiguration, m, e));
            }
            else
            {
                if (_saveMenuPosition != newSaveMenuPosition)
                {
                    _saveMenuPosition = newSaveMenuPosition;
                    RaisePropertyChanged(SaveMenuPositionPropertyName);
                }
            }
        }

        /// <summary>
        /// Updates the whether to run background garbage collection.
        /// </summary>
        /// <param name="newBackgroundGC">If set to <c>true</c> background garbage collection is enabled.</param>
        /// <param name="sendToHardware">If set to <c>true</c> send to Locutus.</param>
        internal void UpdateBackgroundGC(bool newBackgroundGC, bool sendToHardware)
        {
            if (sendToHardware)
            {
                var newConfigurationLo = this.ComposeStatusFlags();
                if (newBackgroundGC)
                {
                    newConfigurationLo |= DeviceStatusFlagsLo.BackgroundGC;
                }
                else
                {
                    newConfigurationLo &= ~DeviceStatusFlagsLo.BackgroundGC;
                }
                this.SetConfiguration(newConfigurationLo, DeviceStatusFlagsHi, (m, e) => ErrorHandler(DeviceStatusFlagsLo.BackgroundGC, ProtocolCommandId.SetConfiguration, m, e));
            }
            else
            {
                if (_backgroundGC != newBackgroundGC)
                {
                    _backgroundGC = newBackgroundGC;
                    RaisePropertyChanged(BackgroundGCPropertyName);
                }
            }
        }

        /// <summary>
        /// Updates the keyclicks setting.
        /// </summary>
        /// <param name="newKeyclicks">If set to <c>true</c> keyclicks are enabled.</param>
        /// <param name="sendToHardware">If set to <c>true</c> send to Locutus.</param>
        internal void UpdateKeyclicks(bool newKeyclicks, bool sendToHardware)
        {
            if (sendToHardware)
            {
                var newConfigurationLo = this.ComposeStatusFlags();
                if (newKeyclicks)
                {
                    newConfigurationLo |= DeviceStatusFlagsLo.Keyclicks;
                }
                else
                {
                    newConfigurationLo &= ~DeviceStatusFlagsLo.Keyclicks;
                }
                this.SetConfiguration(newConfigurationLo, DeviceStatusFlagsHi, (m, e) => ErrorHandler(DeviceStatusFlagsLo.Keyclicks, ProtocolCommandId.SetConfiguration, m, e));
            }
            else
            {
                if (_keyclicks != newKeyclicks)
                {
                    _keyclicks = newKeyclicks;
                    RaisePropertyChanged(KeyclicksPropertyName);
                }
            }
        }

#if DEBUG

        /// <summary>
        /// Injects a fake firmware crash into the device communication.
        /// </summary>
        internal void InjectFirmwareCrash()
        {
            GenerateFakeFirmwareCrash = true;
        }

#endif // DEBUG

        private static void DeviceStatusUpdateTimerCallback(object state)
        {
            var device = state as Device;
            var entered = false;
            try
            {
                if ((SingleInstanceApplication.Current != null) && (device != null) && !device.InTimer && (!device.IsCommandInProgress || (device.ConnectionState == ConnectionState.WaitForBeacon)) && device.IsValid && (device.Port != null) && device.Port.IsOpen)
                {
                    device.InTimer = true;
                    entered = true;
                    device.Port.LogPortMessage("<<<< TIMER PROC ENTERED; device in state: " + device.ConnectionState);
                    int period = System.Threading.Timeout.Infinite;
                    device.DeviceStatusUpdatePeriod = System.Threading.Timeout.Infinite; // no more timer ticks until this one is done

                    var success = false;
                    FileSystemStatistics newDeviceFileSystemStatistics = null;
                    DeviceStatusResponse newDeviceStatus = null;
                    var deviceState = device.ConnectionState;
                    bool raiseConnectionStateChange = false;
                    switch (deviceState)
                    {
                        case ConnectionState.Ping:
                        case ConnectionState.GarbageCollect:
                            period = DefaultGarbageCollectionPeriod;
                            ProtocolCommand command = GarbageCollect.Instance;

                            // Always do GC...
#if ENABLE_PING_ONLY_OPTION
                            if (device.ConnectionState == ConnectionState.Ping))
                            {
                                period = DefaultPingPeriod;
                                command = Ping.Instance;
                            }
#endif // ENABLE_PING_ONLY_OPTION

                            newDeviceStatus = (DeviceStatusResponse)command.Execute(device.Port, null, out success);
                            if (success && newDeviceStatus.HardwareStatus.HasFlag(HardwareStatusFlags.NewErrorLogAvailable) && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.DownloadErrorLog, newDeviceStatus.HardwareStatus))
                            {
                                System.Diagnostics.Debug.WriteLine("NEW ERROR LOG AVAILABLE!");
                            }
                            if (success && newDeviceStatus.HardwareStatus.HasFlag(HardwareStatusFlags.NewCrashLogAvailable) && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.DownloadCrashLog, newDeviceStatus.HardwareStatus))
                            {
                                System.Diagnostics.Debug.WriteLine("NEW CRASH LOG AVAILABLE!");
                            }
                            if (success)
                            {
                                newDeviceFileSystemStatistics = device.PollFileSystemStatistics(newDeviceStatus.HardwareStatus, out success);
                            }
                            if (!success)
                            {
                                device.ConnectionState = ConnectionState.WaitForBeacon;
                            }
                            break;
                        case ConnectionState.WaitForBeacon:
                            period = DefaultBeaconPeriod;
                            if (device.WaitForBeacon(1200))
                            {
                                // This transition usually occurs when connected to a powered-on Intellivision and Locutus makes the
                                // transition from running a program back to the menu, or is turned off. In the long reset to menu
                                // scenario, we've reestablished communication with Locutus, and want to transition back to Ping/GC mode.
                                // The UI will also need to refresh its command availability indicators. We directly collect new state
                                // info here to generate hardware status update *and* state change. We don't change the connection state
                                // via the property setter because we actually *want* the "lag" in raising the value changed event used
                                // in the UI code. If we don't do this, commands will update based on stale hardware state.
                                // This usually manifested in the UI as a grayed out "Play" button. In Windows, it would re-enable if you
                                // switched away from LUI, then reactivated the window. On all platforms, it would re-enable when you
                                // selected a different ROM in the ROM list.

                                // Similarly, if we go from game play directly to power off mode, if we don't do this, we'll see an odd
                                // pulse in the command states in the UI. The "Play" button would re-enable because the connection state
                                // change fired before the hardware update - and the hardware state paradoxically reported that the
                                // power was still ON.
                                newDeviceStatus = (DeviceStatusResponse)Ping.Instance.Execute(device.Port, null, out success);
                                period = DefaultGarbageCollectionPeriod;
                                raiseConnectionStateChange = true;
                                device._connectionState = ConnectionState.GarbageCollect;
#if false
                                period = Properties.Settings.Default.RunGCWhenConnected ? DefaultGarbageCollectionPeriod : DefaultPingPeriod;
                                device.ConnectionState = Properties.Settings.Default.RunGCWhenConnected ? ConnectionState.GarbageCollect : ConnectionState.Ping;
#endif // false
                            }
                            break;
                        case ConnectionState.Idle:
                            period = IdlePeriod;
                            break;
                    }

                    if (SingleInstanceApplication.Current != null)
                    {
                        if (success && (raiseConnectionStateChange || (newDeviceStatus != null)) && !_infoUpdatePosted)
                        {
                            _infoUpdatePosted = true;
                            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() =>
                            {
                                if (newDeviceStatus != null)
                                {
                                    device.UpdateDeviceStatus(newDeviceStatus);
                                }
                                if (newDeviceFileSystemStatistics != null)
                                {
                                    device.FileSystemStatistics = newDeviceFileSystemStatistics;
                                }
                                if (raiseConnectionStateChange)
                                {
                                    device.RaisePropertyChanged(ConnectionStatePropertyName);
                                }
                                _infoUpdatePosted = false;
                            }));
                        }

                        // Restart the timer.
                        SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() => device.RestartTimer(deviceState, period)));
                    }
                }
                else
                {
                    device.Port.LogPortMessage("<<<< TIMER PROC !!!NOT!!! ENTERED; device in state: " + device.ConnectionState + ">>>>");
                }
            }
            catch (InvalidOperationException)
            {
                // This usually happens when the device goes away. Disconnect it.
                device.Disconnect(false);
            }
            catch (UnauthorizedAccessException)
            {
                // Have encountered this if something else has the port open, though other bugs and error conditions may cause it, too.
                device.Disconnect(false);
            }
            catch (System.IO.IOException)
            {
                // Some kind of error during I/O that wasn't a simple timeout, e.g. cord pulled from the computer or the cart.
                device.Disconnect(false);
            }
            catch (TimeoutException)
            {
                // This may happen, for example, if you interrupt a file system synchronization operation by turning on the console during the process.
                device.ConnectionState = ConnectionState.WaitForBeacon;
                SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() => device.RestartTimer(ConnectionState.WaitForBeacon, DefaultBeaconPeriod)));
            }
            catch (DeviceCommandExecuteFailedException)
            {
                // This happens when mucking around in the debugger and injecting false NAK and other fake command failures - you can start causing real ones.
                device.ConnectionState = ConnectionState.WaitForBeacon;
                SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() => device.RestartTimer(ConnectionState.WaitForBeacon, DefaultBeaconPeriod)));
            }
            finally
            {
                device.InTimer = false;
                if (entered)
                {
                    // LogPortMessage is a null-safe extension method.
                    device.Port.LogPortMessage(">>>> TIMER PROC EXIT");
                }
            }
        }

        private static void ValidateDevice(AsyncTaskData taskData)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var validationResponse = new DeviceValidationResponse();
            var validationGuidance = data.Data as DeviceCreationInfo;
            data.Result = validationResponse;
            data.UpdateTaskProgress(0, Resources.Strings.ProtocolCommandId_WaitForBeaconPseudoCommand_Title);

            try
            {
#if false
                device.Port.ReadTimeout = 1000;
                device.Port.WriteTimeout = 1000;
                var serialPort = device.Port as INTV.Shared.Model.Device.SerialPortConnection;
                if (serialPort != null)
                {
                    serialPort.Port.BaudRate = DefaultBaudRate;
                    serialPort.Port.Handshake = System.IO.Ports.Handshake.RequestToSend;
                }
                device.Port.Open();
#endif // false
                data.Succeeded = device.WaitForBeacon(ProtocolCommand.WaitForBeaconTimeout);
                DebugOutput("Beacon: " + data.Succeeded);
                if (validationGuidance.PerformFullValidation)
                {
                    if (data.Succeeded)
                    {
                        var succeeded = false;
                        var retryCount = 0;
                        do
                        {
                            validationResponse.Status = Ping.Instance.Execute(device.Port, data, out succeeded) as DeviceStatusResponse;
                            if (data.Succeeded)
                            {
                                device.HardwareStatus = validationResponse.Status.HardwareStatus;
                            }
                            ++retryCount;
                        }
                        while (!succeeded && retryCount < 5);
                    }
                    DebugOutput("Ping: " + data.Succeeded);
#if false // don't immediately download error log -- if one is there, we'll noticed it and retrieve it later
                    if (data.Succeeded && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.DownloadErrorLog, device.HardwareStatus))
                    {
                        validationResponse.ErrorLog.Add(DownloadErrorLog.Instance.Execute<ErrorLog>(device.Port, data));
                        DebugOutput("ErrorLog: " + data.Succeeded);
                    }
#endif // false
                    if (data.Succeeded && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.FirmwareGetRevisions, device.HardwareStatus))
                    {
                        validationResponse.FirmwareRevisions = QueryFirmwareRevisions.Instance.Execute<FirmwareRevisions>(device.Port, data);
                        DebugOutput("GetFirmware: " + data.Succeeded);
                        if (data.Succeeded)
                        {
                            device.UpdateFirmwareVersion(validationResponse.FirmwareRevisions);
                        }
                    }
                    if (data.Succeeded && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.LfsGetFileSystemStatusFlags, device.HardwareStatus))
                    {
                        validationResponse.FileSystemFlags = GetDirtyFlags.Instance.Execute<LfsDirtyFlags>(device.Port, data);
                        DebugOutput("DirtyFlags: " + data.Succeeded);
                    }
                    if (data.Succeeded && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.LfsDownloadGlobalTables, device.HardwareStatus))
                    {
                        validationResponse.FileSystem = DownloadFileSystemTables.Instance.Execute<FileSystem>(device.Port, data);
                        DebugOutput("GetFileSystem: " + data.Succeeded);
                        validationResponse.FileSystem.Status = validationResponse.FileSystemFlags;
                    }
                    if (data.Succeeded && device.CommandAvailability.IsCommandAvailable(ProtocolCommandId.LfsGetStatistics, device.HardwareStatus))
                    {
                        validationResponse.FileSystemStatistics = GetFileSystemStatistics.Instance.Execute<FileSystemStatistics>(device.Port, data);
                        DebugOutput("FileSystemStats: " + data.Succeeded);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Access is denied to the port or the current process, or another process on the system,
                // already has the specified COM port open either by a SerialPort instance or in unmanaged code.

                // Have forced this to happen in weird rapid device insert/remove or multiple devices inserted/removed rapidly in succession.
                DebugOutput("**** ValidateDevice2 **** NO ACCESS");
            }
            catch (System.IO.IOException)
            {
                // The port is in an invalid state, or an attempt to set the state of the underlying port failed.
                // For example, the parameters passed from this SerialPort object were invalid.

                // Have forced this to happen when inserting / removing many times quickly.
                DebugOutput("**** ValidateDevice2 **** IOException");
            }
            catch (InvalidOperationException)
            {
                // The specified port on the current instance of the SerialPort is already open.

                // Haven't forced, just being cautious.
                DebugOutput("**** ValidateDevice2 **** InvalidOperationException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // One or more of the properties for this instance are invalid. For example, the Parity, DataBits,
                // or Handshake properties are not valid values; the BaudRate is less than or equal to zero; the
                // ReadTimeout or WriteTimeout property is less than zero and is not InfiniteTimeout.

                // Since the app is specifying this explicitly, it should not happen.
            }
            catch (ArgumentException)
            {
                // The port name is not valid, or the file type of the port is not supported.

                // One user who was having troubles w/ the system recognizing LTO Flash! encountered this.
            }
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.CompareTo() took: + " + stopwatch.Elapsed.ToString());
            }
#endif // REPORT_PERFORMANCE
        }

        private static void ValidateDeviceComplete(Device device, bool isValid, DeviceValidationResponse deviceStatusResponse)
        {
            device.ShouldOpenPort = false;
            device.IsCommandInProgress = false;
            device.IsValid = isValid;
            if (isValid)
            {
                INTV.LtoFlash.Commands.LtoFlashCommandGroup.UpdateDriverVersionCommandStrings(INTV.LtoFlash.Utility.FTDIUtilities.DriverVersionString);
                if (Properties.Settings.Default.RunGCWhenConnected)
                {
                    device.ConnectionState = ConnectionState.GarbageCollect;
                }
                else
                {
                    device.ConnectionState = ConnectionState.Ping;
                }
                if (device.CreationInfo.PerformFullValidation)
                {
                    device.Name = PeripheralName;
                    device.UpdateDeviceStatus(deviceStatusResponse.Status);
                    if (Properties.Settings.Default.ReconcileDeviceMenuWithLocalMenu || deviceStatusResponse.FileSystemFlags.HasFlag(LfsDirtyFlags.FileSystemUpdateInProgress))
                    {
                        // If we detect interrupted update, always trigger 'file system changed' - in such a case we want to do the extra work.
                        device.FileSystem = deviceStatusResponse.FileSystem;
                    }
                    else
                    {
                        // Don't use the setter - it will trigger file system compare. Instead, just directly assign the file system to the backing field.
                        device._fileSystem = deviceStatusResponse.FileSystem;
                    }
                    device.FileSystemFlags = deviceStatusResponse.FileSystemFlags;
                    device.FirmwareRevisions = deviceStatusResponse.FirmwareRevisions;
                    device.FileSystemStatistics = deviceStatusResponse.FileSystemStatistics;

                    var customName = device.Name;
                    string owner = null;
                    if (deviceStatusResponse.FileSystem != null)
                    {
                        var rootFile = deviceStatusResponse.FileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
                        if (rootFile != null)
                        {
                            customName = rootFile.ShortName;
                            owner = rootFile.LongName;
                        }
                    }
                    device.CustomName = customName;
                    device.Owner = owner;
                    device.RaisePropertyChanged(IsValidPropertyName);
                }
                else
                {
                    device.ConnectionState = ConnectionState.Disconnected;
                    device.Port.Close();
                    device.ShouldOpenPort = true;
                }

                // Raise this again, because it may have been raised before the UniqueId becomes valid.
                Peripheral.RaisePeripheralAttached(device, device.UniqueId);
            }
            else
            {
                device.Disconnect(false);
            }
        }

        private static bool ValidateDeviceFailed(Device device, string errorMessage, Exception exception)
        {
            ValidateDeviceComplete(device, false, null);
            var errorHandled = true;
            if (device.CreationInfo.ReportValidationError)
            {
                errorHandled = device.ErrorHandler(DeviceStatusFlagsLo.None, ProtocolCommandId.UnknownCommand, errorMessage, exception);
            }
            return errorHandled;
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        #region IDisposable

        private void Dispose(bool disposing)
        {
            if (!_dummyDevice)
            {
                Properties.Settings.Default.PropertyChanged -= SettingsPropertyChanged;
                StopTimer();
                var disposable = Port as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                Port = null;
            }
        }

        #endregion // IDisposable

        private void UpdateLogger()
        {
            if (Port != null)
            {
                if (Properties.Settings.Default.EnablePortLogging)
                {
                    Port.EnableLogging(Configuration.Instance.GetPortLogPath(Port.Name));
                }
                else
                {
                    Port.DisableLogging();
                }
            }
        }

        private void UpdateDeviceStatus(DeviceStatusResponse newDeviceStatus)
        {
            var newUniqueId = UniqueId;
            var newHardwareStatus = HardwareStatus;
            var newIntyIIStatus = IntvIICompatibility;
            var newEcsStatus = EcsCompatibility;
            var newShowTitleScreenStatus = ShowTitleScreen;
            var newSaveMenuPositionStatus = SaveMenuPosition;
            var newKeyclicksStatus = Keyclicks;
            var newBackgroundGCStatus = BackgroundGC;
            var newDeviceStatusFlagsHigh = DeviceStatusFlagsHi;
            if (newDeviceStatus != null)
            {
                newUniqueId = newDeviceStatus.UniqueId;
                newHardwareStatus = newDeviceStatus.HardwareStatus & ~HardwareStatusFlags.ReservedMask;
                newIntyIIStatus = newDeviceStatus.IntellivisionIIStatus & ~IntellivisionIIStatusFlags.ReservedMask;
                newEcsStatus = newDeviceStatus.EcsStatus & ~EcsStatusFlags.ReservedMask;
                newShowTitleScreenStatus = newDeviceStatus.ShowTitleScreen;
                newSaveMenuPositionStatus = newDeviceStatus.SaveMenuPosition;
                newKeyclicksStatus = newDeviceStatus.Keyclicks;
                newBackgroundGCStatus = newDeviceStatus.BackgroundGC;
                newDeviceStatusFlagsHigh = newDeviceStatus.DeviceStatusHigh;
            }
            UpdateUniqueId(newUniqueId);
            UpdateHardwareFlags(newHardwareStatus);
            UpdateIntellivisionIIConfiguration(newIntyIIStatus, false);
            UpdateEcsConfiguration(newEcsStatus, false);
            UpdateShowTitleScreen(newShowTitleScreenStatus, false);
            UpdateSaveMenuPosition(newSaveMenuPositionStatus, false);
            UpdateBackgroundGC(newBackgroundGCStatus, false);
            UpdateKeyclicks(newKeyclicksStatus, false);
            DeviceStatusFlagsHi = newDeviceStatusFlagsHigh;
        }

        private void UpdateUniqueId(string uniqueId)
        {
            if (UniqueId != uniqueId)
            {
                UniqueId = uniqueId;
                try
                {
                    Configuration.Instance.RecordDeviceArrival(uniqueId);
                }
                catch (Exception)
                {
                    // Suppress any error that may have arisen.
                }
                RaisePropertyChanged(UniqueIdPropertyName);
            }
        }

        private void UpdateNameAndOwner(FileSystem fileSystem)
        {
            if (fileSystem != null)
            {
                var rootFile = fileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
                if (rootFile != null)
                {
                    CustomName = rootFile.ShortName;
                    Owner = rootFile.LongName;
                }
            }
        }

        private void UpdateHardwareFlags(HardwareStatusFlags newHardwareStatus)
        {
#if DEBUG
            if (GenerateFakeFirmwareCrash)
            {
                newHardwareStatus |= HardwareStatusFlags.NewCrashLogAvailable | HardwareStatusFlags.NewErrorLogAvailable;
            }
#endif // DEBUG
            if (newHardwareStatus != HardwareStatus)
            {
                bool powerStateChanged = newHardwareStatus.HasFlag(HardwareStatusFlags.ConsolePowerOn) != HardwareStatus.HasFlag(HardwareStatusFlags.ConsolePowerOn);
                HardwareStatus = newHardwareStatus;
                RaisePropertyChanged(HardwareStatusPropertyName);
                if (powerStateChanged)
                {
                    RaisePropertyChanged(IsIntellivisionConnectedPropertyName);
                    if ((FirmwareRevisions == null) && CommandAvailability.IsCommandAvailable(ProtocolCommandId.FirmwareGetRevisions, newHardwareStatus))
                    {
                        this.GetFirmwareRevisions(null, (m, e) => INTV.LtoFlash.Commands.FirmwareCommandGroup.ErrorHandler(LtoFlash.Model.Commands.ProtocolCommandId.FirmwareGetRevisions, m, e));
                    }
                }
            }
        }

        private void UpdateFirmwareVersion(FirmwareRevisions firmwareRevisions)
        {
            var newFirmwareVersion = firmwareRevisions == null ? 0 : firmwareRevisions.Current >> 2;
            CommandAvailability.UpdateCommandAvailabilityForFirmwareVersion(newFirmwareVersion);
        }

        private void UpdateTimerPeriod(int newPeriod)
        {
            if (_statusUpdateTimer != null)
            {
                try
                {
                    _statusUpdateTimer.Change(newPeriod, newPeriod);
                }
                catch (ObjectDisposedException)
                {
                    // This can happen if we initiated a time period change from the timer proc, but before processing it, stopped the timer.
                }
            }
        }
        private System.Threading.Timer _statusUpdateTimer;

        private void SettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EnablePortLogging")
            {
                UpdateLogger();
            }
            if (e.PropertyName == "ShowFileSystemDetails")
            {
                UpdateFileSystemStatsDuringHeartbeat = Properties.Settings.Default.ShowFileSystemDetails;
            }
        }

        private void OnConnectionStateChanged(ConnectionState newState)
        {
            DebugOutput("^^^^ DEVICE CONNECTION STATE CHANGE to: " + newState + " at " + System.DateTime.Now.ToString("HH:mm:ss.ffff"));
        }

        /// <summary>
        /// Restarts the timer with the given period, correcting for potential state changes between when the request to restart was made.
        /// </summary>
        /// <param name="stateAtTimeOfRequest">The device state used to determine the value of <paramref name="period"/>.</param>
        /// <param name="period">The new period for the timer.</param>
        /// <remarks>Because requests to restart the timer can be made from another thread, and asynchronously, the device's actual connection
        /// state may have changed between when the restart request was posted and when it is actually processed. This method attempts to correct
        /// for such a change without using a lock. It is specifically interested in cases in which the requested period is infinite. Perhaps this
        /// special check is never necessary, and this method should always simply make its determination based solely on the current value of
        /// ConnectionState. It's still possible to hit race conditions in which the connection state change during the execution of this function
        /// due to other threads modifying ConnectionState, but unless / until a problem is identified, let's live on the edge!</remarks>
        private void RestartTimer(ConnectionState stateAtTimeOfRequest, int period)
        {
            if ((stateAtTimeOfRequest != ConnectionState) && (period < 0) && (_statusUpdateTimer != null))
            {
                if (stateAtTimeOfRequest == ConnectionState.Disconnected)
                {
                    switch (ConnectionState)
                    {
                        case ConnectionState.Ping:
                        case ConnectionState.GarbageCollect:
                            period = DefaultGarbageCollectionPeriod;
                            break;
                        case ConnectionState.Idle:
                            period = IdlePeriod;
                            break;
                        case ConnectionState.WaitForBeacon:
                        default:
                            period = DefaultBeaconPeriod;
                            break;
                    }
                }
            }
            if (ConnectionState == ConnectionState.Disconnected)
            {
                period = System.Threading.Timeout.Infinite;
            }
            DeviceStatusUpdatePeriod = period;
        }

        private void IsValidChanged(bool isValid)
        {
            StopTimer();
            if (isValid && CreationInfo.PerformFullValidation)
            {
                _statusUpdateTimer = new System.Threading.Timer(DeviceStatusUpdateTimerCallback, this, 0, DeviceStatusUpdatePeriod);
            }
            if (isValid)
            {
                Peripheral.RaisePeripheralAttached(this, UniqueId);
            }
            else
            {
                Peripheral.RaisePeripheralDetached(this, UniqueId);
            }
        }

        private void StopTimer()
        {
            if (_statusUpdateTimer != null)
            {
                using (var wait = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset))
                {
                    try
                    {
                        if (_statusUpdateTimer != null)
                        {
                            _statusUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                            _statusUpdateTimer.Dispose(wait);
                            wait.WaitOne();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    finally
                    {
                        _statusUpdateTimer = null;
                    }
                }
            }
        }

        /// <summary>
        /// Validates the device.
        /// </summary>
        internal void ValidateDevice()
        {
            if (!IsCommandInProgress && (Port != null))
            {
                if (ShouldOpenPort && !Port.IsOpen)
                {
                    var numRetries = 2;
                    var failed = true;
                    for (var i = 0; (i < numRetries) && failed; ++i)
                    {
                        failed = false;
                        try
                        {
#if true
                            Port.ReadTimeout = 1000;
                            DebugOutput("**** ValidateDevice **** Set ReadTimeout");
                            Port.WriteTimeout = 1000;
                            DebugOutput("**** ValidateDevice **** Set WriteTimeout");

                            var serialPort = Port as INTV.Shared.Model.Device.SerialPortConnection;
                            if (serialPort != null)
                            {
                                serialPort.BaudRate = DefaultBaudRate;
                                DebugOutput("**** ValidateDevice **** Set BaudRate");
                                serialPort.Handshake = System.IO.Ports.Handshake.RequestToSend;
                                DebugOutput("**** ValidateDevice **** Set Handshake");
                            }
                            DebugOutput("**** ValidateDevice **** Opening port; internal: " + serialPort.IsOpen);
                            Port.Open();
#endif // true
                            DebugOutput("**** ValidateDevice **** Opened port");
                            var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(this, ProtocolCommandId.MultistagePseudoCommand)
                            {
                                Title = Resources.Strings.DeviceMultistageCommand_Validating_Title,
                                Data = CreationInfo,
                                OnFailure = (m, e) => ValidateDeviceFailed(this, m, e), // If validation fails, that's OK
                                FailureMessage = Resources.Strings.DeviceMultistageCommand_ValidationFailed
                            };
                            executeCommandTaskData.OnSuccess = (c, p, r) => ValidateDeviceComplete(this, executeCommandTaskData.Succeeded, (DeviceValidationResponse)r);
                            executeCommandTaskData.StartTask(ValidateDevice);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Access is denied to the port or the current process, or another process on the system,
                            // already has the specified COM port open either by a SerialPort instance or in unmanaged code.

                            // Have forced this to happen in weird rapid device insert/remove or multiple devices inserted/removed rapidly in succession.
                            DebugOutput("**** ValidateDevice **** NO ACCESS");
                            failed = true;
                        }
                        catch (System.IO.IOException e)
                        {
                            // The port is in an invalid state, or an attempt to set the state of the underlying port failed.
                            // For example, the parameters passed from this SerialPort object were invalid.

                            // Have forced this to happen when inserting / removing many times quickly.
                            DebugOutput("**** ValidateDevice **** IOException " + Environment.NewLine + e.ToString());
                            failed = true;
                        }
                        catch (InvalidOperationException)
                        {
                            // The specified port on the current instance of the SerialPort is already open.

                            // Haven't forced, just being cautious.
                            DebugOutput("**** ValidateDevice **** InvalidOperationException");
                            failed = true;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // One or more of the properties for this instance are invalid. For example, the Parity, DataBits,
                            // or Handshake properties are not valid values; the BaudRate is less than or equal to zero; the
                            // ReadTimeout or WriteTimeout property is less than zero and is not InfiniteTimeout.

                            // Since the app is specifying this explicitly, it should not happen.
                            DebugOutput("**** ValidateDevice **** ArgumentOutOfRangeException");
                            failed = true;
                        }
                        catch (ArgumentException)
                        {
                            // The port name is not valid, or the file type of the port is not supported.

                            // One user who was having troubles w/ the system recognizing LTO Flash! encountered this.
                            DebugOutput("**** ValidateDevice **** ArgumentException");
                            failed = true;
                        }
                        if (failed)
                        {
                            DebugOutput("**** ValidateDevice **** failed... retry...");
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                    if (failed)
                    {
                        ValidateDeviceComplete(this, false, null);
                    }
                }
            }
        }

        private FileSystemStatistics PollFileSystemStatistics(HardwareStatusFlags hardwareStatus, out bool success)
        {
            success = true;
            FileSystemStatistics newFileSystemStatistics = null;
            if (UpdateFileSystemStatsDuringHeartbeat && CommandAvailability.IsCommandAvailable(ProtocolCommandId.LfsGetStatistics, hardwareStatus))
            {
                newFileSystemStatistics = (FileSystemStatistics)GetFileSystemStatistics.Instance.Execute(Port, null, out success);
            }
            return newFileSystemStatistics;
        }

        private class DeviceValidationResponse
        {
            public DeviceValidationResponse()
            {
                ErrorLog = new List<ErrorLog>();
                FirmwareRevisions = FirmwareRevisions.Unavailable;
            }

            public List<ErrorLog> ErrorLog { get; private set; }
            public DeviceStatusResponse Status { get; set; }
            public LfsDirtyFlags FileSystemFlags { get; set; }
            public FileSystem FileSystem { get; set; }
            public FirmwareRevisions FirmwareRevisions { get; set; }
            public FileSystemStatistics FileSystemStatistics { get; set; }
        }
    }
}

// <copyright file="DeviceInformationController.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_REBOOT_TO_MENU_KEY_SELECTION

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Device;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Behavior;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;
#if __UNIFIED__
using AppKit;
using Foundation;
using ImageKit;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ImageKit;
#endif // __UNIFIED__

#if __UNIFIED__
using CGSize = CoreGraphics.CGSize;
using nint = System.nint;
#else
using CGSize = System.Drawing.SizeF;
using IIKImageBrowserItem = MonoMac.ImageKit.IKImageBrowserItem;
using nint = System.Int32;
#endif // __UNIFIED__

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Device information controller.
    /// </summary>
    public partial class DeviceInformationController : NSWindowController, System.ComponentModel.INotifyPropertyChanged
    {
        private const string DeviceNamePropertyName = "DeviceName";
        private const string DeviceOwnerPropertyName = "DeviceOwner";
        private const string DeviceIdPropertyName = "DeviceId";
        private const string ConnectionNamePropertyName = "ConnectionName";
        private const string FactoryFirmwareVersionPropertyName = "FactoryFirmwareVersion";
        private const string SecondaryFirmwareVersionPropertyName = "SecondaryFirmwareVersion";
        private const string CurrentFirmwareVersionPropertyName = "CurrentFirmwareVersion";
        private const string PhysBlocksAvailPropertyName = "PhysBlocksAvail";
        private const string PhysBlocksInUsePropertyName = "PhysBlocksInUse";
        private const string PhysCleanBlocksPropertyName = "PhysCleanBlocks";
        private const string PhysTotalBlocksPropertyName = "PhysTotalBlocks";
        private const string VirtBlocksAvailPropertyName = "VirtBlocksAvail";
        private const string VirtBlocksInUsePropertyName = "VirtBlocksInUse";
        private const string VirtTotalBlocksPropertyName = "VirtTotalBlocks";
        private const string PhysSectorErasuresPropertyName = "PhysSectorErasures";
        private const string MetadataSectorErasuresPropertyName = "MetadataSectorErasures";
        private const string VirtToPhysMapVerPropertyName = "VirtToPhysMapVer";
        private const string EcsCompatibilitySelectionPropertyName = "EcsCompatibilitySelection";
        private const string IntellivisionIICompatibilitySelectionPropertyName = "IntellivisionIICompatibilitySelection";

        private static readonly Lazy<IReadOnlyDictionary<DeviceInfoFieldToolTipTag, VisualRelayCommand>> AdditionalToolTipsMap = new Lazy<IReadOnlyDictionary<DeviceInfoFieldToolTipTag, VisualRelayCommand>>(InitializeAdditionalToolTipsMap);

        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public DeviceInformationController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public DeviceInformationController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public DeviceInformationController()
            : base("DeviceInformation")
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        /// <param name="viewModel">The ViewModel for the dialog.</param>
        public DeviceInformationController(LtoFlashViewModel viewModel)
            : base("DeviceInformation")
        {
            ViewModel = viewModel;
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            Initialize();
        }

        #endregion // Constructors

        #region INotifyPropertyChanged

        /// <inheritdoc/>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new DeviceInformation Window
        {
            get { return (DeviceInformation)base.Window; }
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public LtoFlashViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        [INTV.Shared.Utility.OSExport(DeviceNamePropertyName)]
        public NSString DeviceName
        {
            get { return _deviceName; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, DeviceNamePropertyName, value, ref _deviceName); }
        }
        private NSString _deviceName;

        /// <summary>
        /// Gets the device owner.
        /// </summary>
        [INTV.Shared.Utility.OSExport(DeviceOwnerPropertyName)]
        public NSString DeviceOwner
        {
            get { return _deviceOwner; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, DeviceOwnerPropertyName, value, ref _deviceOwner); }
        }
        private NSString _deviceOwner;

        /// <summary>
        /// Gets the device ID.
        /// </summary>
        [INTV.Shared.Utility.OSExport(DeviceIdPropertyName)]
        public NSString DeviceId
        {
            get { return _deviceId; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, DeviceIdPropertyName, value, ref _deviceId); }
        }
        private NSString _deviceId;

        /// <summary>
        /// Gets the connection information string for the device.
        /// </summary>
        [INTV.Shared.Utility.OSExport(ConnectionNamePropertyName)]
        public NSString ConnectionName
        {
            get { return _connectionName; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, ConnectionNamePropertyName, value, ref _connectionName); }
        }
        private NSString _connectionName;

        #region Firmware Info

        /// <summary>
        /// Gets the factory firmware version.
        /// </summary>
        [INTV.Shared.Utility.OSExport(FactoryFirmwareVersionPropertyName)]
        public NSString FactoryFirmwareVersion
        {
            get { return _factoryFirmwareVersion; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, FactoryFirmwareVersionPropertyName, value, ref _factoryFirmwareVersion); }
        }
        private NSString _factoryFirmwareVersion;

        /// <summary>
        /// Gets or sets the secondary firmware version.
        /// </summary>
        [INTV.Shared.Utility.OSExport(SecondaryFirmwareVersionPropertyName)]
        public NSString SecondaryFirmwareVersion
        {
            get { return _secondaryFirmwareVersion; }
            set { this.AssignAndUpdateProperty(PropertyChanged, SecondaryFirmwareVersionPropertyName, value, ref _secondaryFirmwareVersion); }
        }
        private NSString _secondaryFirmwareVersion;

        /// <summary>
        /// Gets the current firmware version.
        /// </summary>
        [INTV.Shared.Utility.OSExport(CurrentFirmwareVersionPropertyName)]
        public NSString CurrentFirmwareVersion
        {
            get { return _currentFirmwareVersion; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, CurrentFirmwareVersionPropertyName, value, ref _currentFirmwareVersion); }
        }
        private NSString _currentFirmwareVersion;

        #endregion Firmware Info

        #region File System Info

        /// <summary>
        /// Gets file system physical blocks available.
        /// </summary>
        [INTV.Shared.Utility.OSExport(PhysBlocksAvailPropertyName)]
        public NSString PhysBlocksAvail
        {
            get { return _physBlocksAvail; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, PhysBlocksAvailPropertyName, value, ref _physBlocksAvail); }
        }
        private NSString _physBlocksAvail;

        /// <summary>
        /// Gets file system physical blocks in use.
        /// </summary>
        [INTV.Shared.Utility.OSExport(PhysBlocksInUsePropertyName)]
        public NSString PhysBlocksInUse
        {
            get { return _physBlocksInUse; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, PhysBlocksInUsePropertyName, value, ref _physBlocksInUse); }
        }
        private NSString _physBlocksInUse;

        /// <summary>
        /// Gets file system physical clean blocks that are clean.
        /// </summary>
        [INTV.Shared.Utility.OSExport(PhysCleanBlocksPropertyName)]
        public NSString PhysCleanBlocks
        {
            get { return _physCleanBlocks; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, PhysCleanBlocksPropertyName, value, ref _physCleanBlocks); }
        }
        private NSString _physCleanBlocks;

        /// <summary>
        /// Gets file system total physical blocks.
        /// </summary>
        [INTV.Shared.Utility.OSExport(PhysTotalBlocksPropertyName)]
        public NSString PhysTotalBlocks
        {
            get { return _phsyTotalBlocks; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, PhysTotalBlocksPropertyName, value, ref _phsyTotalBlocks); }
        }
        private NSString _phsyTotalBlocks;

        /// <summary>
        /// Gets file system virtual blocks available.
        /// </summary>
        [INTV.Shared.Utility.OSExport(VirtBlocksAvailPropertyName)]
        public NSString VirtBlocksAvail
        {
            get { return _virtBlocksAvail; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, VirtBlocksAvailPropertyName, value, ref _virtBlocksAvail); }
        }
        private NSString _virtBlocksAvail;

        /// <summary>
        /// Gets file system virtual blocks in use.
        /// </summary>
        [INTV.Shared.Utility.OSExport(VirtBlocksInUsePropertyName)]
        public NSString VirtBlocksInUse
        {
            get { return _virtBlocksInUse; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, VirtBlocksInUsePropertyName, value, ref _virtBlocksInUse); }
        }
        private NSString _virtBlocksInUse;

        /// <summary>
        /// Gets total number of virutal blocks.
        /// </summary>
        [INTV.Shared.Utility.OSExport(VirtTotalBlocksPropertyName)]
        public NSString VirtTotalBlocks
        {
            get { return _virtTotalBlocks; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, VirtTotalBlocksPropertyName, value, ref _virtTotalBlocks); }
        }
        private NSString _virtTotalBlocks;

        /// <summary>
        /// Gets number of physical sector erasures.
        /// </summary>
        [INTV.Shared.Utility.OSExport(PhysSectorErasuresPropertyName)]
        public NSString PhysSectorErasures
        {
            get { return _physSectorErasures; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, PhysSectorErasuresPropertyName, value, ref _physSectorErasures); }
        }
        private NSString _physSectorErasures;

        /// <summary>
        /// Gets number of metadata sector erasures.
        /// </summary>
        [INTV.Shared.Utility.OSExport(MetadataSectorErasuresPropertyName)]
        public NSString MetadataSectorErasures
        {
            get { return _metadataSectorErasures; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, MetadataSectorErasuresPropertyName, value, ref _metadataSectorErasures); }
        }
        private NSString _metadataSectorErasures;

        /// <summary>
        /// Gets virtual to physical journal map version.
        /// </summary>
        [INTV.Shared.Utility.OSExport(VirtToPhysMapVerPropertyName)]
        public NSString VirtToPhysMapVer
        {
            get { return _virtToPhysMapVer; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, VirtToPhysMapVerPropertyName, value, ref _virtToPhysMapVer); }
        }
        private NSString _virtToPhysMapVer;

        /// <summary>
        /// Gets number of physical block erasures.
        /// </summary>
        [OSExport("PBlockErasures")]
        public NSString PBlockErasures
        {
            get { return _percentPBlockErasures; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, "PBlockErasures", value, ref _percentPBlockErasures); }
        }
        private NSString _percentPBlockErasures;

        /// <summary>
        /// Gets wrap count of virtual to physical log.
        /// </summary>
        [OSExport("VtoPLogWraps")]
        public NSString VtoPLogWraps
        {
            get { return _percentVtoPLogWraps; }
            private set { this.AssignAndUpdateProperty(PropertyChanged, "VtoPLogWraps", value, ref _percentVtoPLogWraps); }
        }
        private NSString _percentVtoPLogWraps;

        /// <summary>
        /// Gets or sets the approximate percent lifetime remaining.
        /// </summary>
        [OSExport("LifeRemaining")]
        public NSString LifeRemaining
        {
            get { return _percentLifeRemaining; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "LifeRemaining", value, ref _percentLifeRemaining); }
        }
        private NSString _percentLifeRemaining;

        #endregion File System Info

        #region Device Settings

        /// <summary>
        /// Gets or sets a value indicating whether background garbage collection runs when menu displayed on console.
        /// </summary>
        [INTV.Shared.Utility.OSExport(Device.BackgroundGCPropertyName)]
        public bool BackgroundGC
        {
            get { return ViewModel.ActiveLtoFlashDevice.BackgroundGC; }
            set { ViewModel.ActiveLtoFlashDevice.BackgroundGC = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the keyclick sound is enabled in the menu.
        /// </summary>
        [INTV.Shared.Utility.OSExport(Device.KeyclicksPropertyName)]
        public bool Keyclicks
        {
            get { return ViewModel.ActiveLtoFlashDevice.Keyclicks; }
            set { ViewModel.ActiveLtoFlashDevice.Keyclicks = value; }
        }

        [INTV.Shared.Utility.OSExport(Device.EnableConfigMenuOnCartPropertyName)]
        public bool EnableConfigMenuOnCart
        {
            get { return ViewModel.ActiveLtoFlashDevice.EnableConfigMenuOnCart; }
            set { ViewModel.ActiveLtoFlashDevice.EnableConfigMenuOnCart = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to randomize RAM when launching a program from Locutus.
        /// </summary>
        [INTV.Shared.Utility.OSExport(DeviceViewModel.RandomizeLtoFlashRamPropertyName)]
        public bool RandomizeLtoFlashRam
        {
            get { return ViewModel.ActiveLtoFlashDevice.RandomizeLtoFlashRam; }
            set { ViewModel.ActiveLtoFlashDevice.RandomizeLtoFlashRam = value; }
        }

        /// <summary>
        /// Gets or sets the selected controller button to hold for reset to menu.
        /// </summary>
        /// <remarks>This feature has not been added to firmware.</remarks>
        [INTV.Shared.Utility.OSExport("SelectedButton")]
        public NSIndexSet SelectedButton
        {
            get
            {
                return _selectedButton;
            }

            set
            {
                if (_selectedButton != null)
                {
                    var prevIndex = (int)_selectedButton.FirstIndex;
                    var prevItem = this.ControllerButtonsGrid.ItemAtIndex(prevIndex);
                    var selectedItem = prevItem.RepresentedObject as ControllerElementViewModel;
                    if (selectedItem != null)
                    {
                        selectedItem.Selected = false;
                    }
                }

                if (value != null)
                {
                    var newIndex = (int)value.FirstIndex;
                    var newItem = ControllerButtonsGrid.ItemAtIndex(newIndex);
                    var selectedItem = newItem.RepresentedObject as ControllerElementViewModel;
                    if ((selectedItem != null) && (selectedItem.Image != null))
                    {
                        selectedItem.Selected = true;
                    }
                }

                _selectedButton = value;
            }
        }
        private NSIndexSet _selectedButton;

        /// <summary>
        /// Gets or sets the controller buttons for choosing reboot to menu key combination.
        /// </summary>
        /// <remarks>This feature has not been implemented in firmware.</remarks>
        [INTV.Shared.Utility.OSExport("ControllerElements")]
        public NSMutableArray ControllerElements
        {
            get { return _controllerElements; }
            set { _controllerElements = value; }
        }
        private NSMutableArray _controllerElements;

        /// <summary>
        /// Gets or sets when to show cartridge title screen.
        /// </summary>
        [INTV.Shared.Utility.OSExport("TitleScreenSelection")]
        public int TitleScreenSelection
        {
            get { return _titleScreenSelection; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "TitleScreenSelection", value, ref _titleScreenSelection, (p, v) => ViewModel.ActiveLtoFlashDevice.ShowTitleScreen = (ShowTitleScreenFlags)(int)ShowTitleScreenButton.SelectedTag); }
        }
        private int _titleScreenSelection;

        /// <summary>
        /// Gets or sets whether to save menu position.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SaveMenuPositionSelection")]
        public int SaveMenuPositionSelection
        {
            get { return _saveMenuPositionSelection; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "SaveMenuPositionSelection", value, ref _saveMenuPositionSelection, (p, v) => ViewModel.ActiveLtoFlashDevice.SaveMenuPosition = (SaveMenuPositionFlags)(int)SaveMenuPositionButton.SelectedTag); }
        }
        private int _saveMenuPositionSelection;

        /// <summary>
        /// Gets or sets ECS compatibility.
        /// </summary>
        [INTV.Shared.Utility.OSExport(EcsCompatibilitySelectionPropertyName)]
        public int EcsCompatibilitySelection
        {
            get { return _ecsCompatibilitySelection; }
            set { this.AssignAndUpdateProperty(PropertyChanged, EcsCompatibilitySelectionPropertyName, value, ref _ecsCompatibilitySelection, (p, v) => ViewModel.ActiveLtoFlashDevice.EcsCompatibility = (EcsStatusFlags)(int)ECSCompatibilityButton.SelectedTag); }
        }
        private int _ecsCompatibilitySelection;

        /// <summary>
        /// Gets or sets Intellivision II compatibility.
        /// </summary>
        [INTV.Shared.Utility.OSExport(IntellivisionIICompatibilitySelectionPropertyName)]
        public int IntellivisionIICompatibilitySelection
        {
            get { return _intellivisionIICompatibilitySelection; }
            set { this.AssignAndUpdateProperty(PropertyChanged, IntellivisionIICompatibilitySelectionPropertyName, value, ref _intellivisionIICompatibilitySelection, (p, v) => ViewModel.ActiveLtoFlashDevice.IntvIICompatibility = (IntellivisionIIStatusFlags)(int)IntellivisionIICompatibilityButton.SelectedTag); }
        }
        private int _intellivisionIICompatibilitySelection;

        #endregion // Device Settings

        /// <summary>
        /// Gets or sets the index of the selected page in the dialog.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedPageIndex")]
        public int SelectedPageIndex
        {
            get
            {
                return _selectedPageIndex;
            }

            set
            {
                _selectedPageIndex = value;
                _lastSelectedPageIndex = value;
            }
        }
        private int _selectedPageIndex;
        private static int _lastSelectedPageIndex;

        private TextCellInPlaceEditor InPlaceEditor { get; set; }

        private Dictionary<VisualRelayCommand, bool> _blockWhenBusy = new Dictionary<VisualRelayCommand, bool>();

        private DeviceViewModel _device;

        private Dictionary<NSControl, VisualRelayCommand> _controlCommandMap = new Dictionary<NSControl, VisualRelayCommand>();

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            INTV.Shared.Utility.SingleInstanceApplication.Instance.IsBusy = true;
            ControllerButtonsGrid.BackgroundColors = new NSColor[] { ColorHelpers.IntellivisionGold };
            _controllerElements = new NSMutableArray();
            ControllerElementsArrayController.SelectsInsertedObjects = false;
            ControllerButtonsGrid.MinItemSize = new CGSize(32, 32);
            ControllerButtonsGrid.MaxItemSize = new CGSize(36, 36);
            var buttons = new ControllerKeys[4, 5]
            {
                { ControllerKeys.None, ControllerKeys.Keypad1, ControllerKeys.Keypad2, ControllerKeys.Keypad3, ControllerKeys.None },
                { ControllerKeys.ActionKeyTop, ControllerKeys.Keypad4, ControllerKeys.Keypad5, ControllerKeys.Keypad6, ControllerKeys.ActionKeyTop | ControllerKeys.NoneActive },
                { ControllerKeys.ActionKeyBottomLeft, ControllerKeys.Keypad7, ControllerKeys.Keypad8, ControllerKeys.Keypad9, ControllerKeys.ActionKeyBottomRight },
                { ControllerKeys.None, ControllerKeys.KeypadClear, ControllerKeys.Keypad0, ControllerKeys.KeypadEnter, ControllerKeys.None }
            };

#if ENABLE_REBOOT_TO_MENU_KEY_SELECTION

            string[,] buttonImageResources = new string[4, 5]
            {
                {
                    null,
                    "controller_button_1_x64.png",
                    "controller_button_2_x64.png",
                    "controller_button_3_x64.png",
                    null
                }, {
                    "controller_action_topleft_24x64.png",
                    "controller_button_4_x64.png",
                    "controller_button_5_x64.png",
                    "controller_button_6_x64.png",
                    "controller_action_topright_24x64.png"
                }, {
                    "controller_action_left_24x64.png",
                    "controller_button_7_x64.png",
                    "controller_button_8_x64.png",
                    "controller_button_9_x64.png",
                    "controller_action_right_24x64.png"
                }, {
                    null,
                    "controller_button_clear_x64.png",
                    "controller_button_0_x64.png",
                    "controller_button_enter_x64.png",
                    null
                },
            };
            var images = new NSImage[20];
            var i = 0;
            foreach (var imageResource in buttons)
            {
                {
                    if (imageResource != null)
                    {
                        var resource = "ViewModel/Resources/Images/" + imageResource;
                        var image = typeof(ControllerElementViewModel).LoadImageResource(resource);
                        images[i] = image;
                        var currentSize = image.Size;
                        image.Size = new System.Drawing.SizeF(currentSize.Width/2, currentSize.Height/2);
                        ControllerElementsArrayController.AddObject(image);
                    }
                    else
                    {
                        images[i] = new NSImage();
                        ControllerElementsArrayController.AddObject(new NSImage());
                    }
                }
                ++i;
            }

#endif // ENABLE_REBOOT_TO_MENU_KEY_SELECTION

            foreach (var button in buttons)
            {
                var controllerElement = new ControllerElementViewModel(button);
                var image = controllerElement.Image;
                if (image != null)
                {
                    var currentSize = image.Size;
                    ((NSImage)image).Size = new CGSize(currentSize.Width / 2, currentSize.Height / 2);
                }
                ControllerElementsArrayController.AddObject(controllerElement);
            }

            DeviceNameEntry.RefusesFirstResponder = true;
            DeviceOwnerEntry.RefusesFirstResponder = true;

            DeviceNameEntry.TextShouldBeginEditing = ShouldBeginEditingDeviceName;
            DeviceNameEntry.TextShouldEndEditing = ShouldEndEditingDeviceName;
            DeviceOwnerEntry.TextShouldBeginEditing = ShouldBeginEditingDeviceOwner;
            DeviceOwnerEntry.TextShouldEndEditing = ShouldEndEditingDeviceOwner;

            DeviceCommandGroup.PopulateEcsCompatibilityMenu(ECSCompatibilityButton);
            DeviceCommandGroup.PopulateIntellivisionIICompatibilityMenu(IntellivisionIICompatibilityButton);
            DeviceCommandGroup.PopulateShowTitleMenu(ShowTitleScreenButton);
            DeviceCommandGroup.PopulateSaveMenuPositionMenu(SaveMenuPositionButton);
            EcsCompatibilitySelection = (int)ECSCompatibilityButton.IndexOfItem((int)ViewModel.ActiveLtoFlashDevice.EcsCompatibility);
            IntellivisionIICompatibilitySelection = (int)IntellivisionIICompatibilityButton.IndexOfItem((int)ViewModel.ActiveLtoFlashDevice.IntvIICompatibility);
            TitleScreenSelection = (int)ShowTitleScreenButton.IndexOfItem((int)ViewModel.ActiveLtoFlashDevice.ShowTitleScreen);
            SaveMenuPositionSelection = (int)ShowTitleScreenButton.IndexOfItem((int)ViewModel.ActiveLtoFlashDevice.SaveMenuPosition);

            InitializeCommandVisualsToCommandsMap();

            CommandManager.RequerySuggested += HandleRequerySuggested;
            HandleRequerySuggested(this, System.EventArgs.Empty);
            SelectedPageIndex = _lastSelectedPageIndex;
            this.RaiseChangeValueForKey("SelectedPageIndex");
            Properties.Settings.Default.ShowFileSystemDetails = true;

            this.BeginInvokeOnMainThread(() =>
                {
                    DeviceNameEntry.RefusesFirstResponder = false;
                    DeviceOwnerEntry.RefusesFirstResponder = false;
                });
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (_device != null)
            {
                _device.PropertyChanged -= HandleDevicePropertyChanged;
            }
            CommandManager.RequerySuggested -= HandleRequerySuggested;

            // Restore the block when busy settings.
            foreach (var blockWhenBusyEntry in _blockWhenBusy)
            {
                var command = blockWhenBusyEntry.Key;
                var blockWhenBusy = blockWhenBusyEntry.Value;
                command.BlockWhenAppIsBusy = blockWhenBusy;
            }

            // Unbind the Cocoa binding for the ToolTip text we added in AwakeFromNib.
            foreach (var controlCommandMapEntry in _controlCommandMap.Reverse())
            {
                var commandVisual = controlCommandMapEntry.Key;
                if (!AdditionalToolTipsMap.Value.ContainsKey((DeviceInfoFieldToolTipTag)commandVisual.Tag))
                {
                    commandVisual.UnbindCommandVisualFromToolTipDescription();
                }
            }

            ViewModel.PropertyChanged -= HandleViewModelPropertyChanged;

            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }

        private static IReadOnlyDictionary<DeviceInfoFieldToolTipTag, VisualRelayCommand> InitializeAdditionalToolTipsMap()
        {
            var additionalToolTipsMap = new Dictionary<DeviceInfoFieldToolTipTag, VisualRelayCommand>()
            {
                { DeviceInfoFieldToolTipTag.Name, DeviceCommandGroup.SetDeviceNameCommand },
                { DeviceInfoFieldToolTipTag.Owner, DeviceCommandGroup.SetDeviceOwnerCommand },
                { DeviceInfoFieldToolTipTag.UniqueId, DeviceCommandGroup.DeviceUniqueIdCommand },
                { DeviceInfoFieldToolTipTag.ShowTitleScreen, DeviceCommandGroup.SetShowTitleScreenCommand },
                { DeviceInfoFieldToolTipTag.IntellivisionIICompatibility, DeviceCommandGroup.SetIntellivisionIICompatibilityCommand },
                { DeviceInfoFieldToolTipTag.EcsCompatibilty, DeviceCommandGroup.SetEcsCompatibilityCommand },
                { DeviceInfoFieldToolTipTag.SaveMenuPosition, DeviceCommandGroup.SetSaveMenuPositionCommand },
                { DeviceInfoFieldToolTipTag.FactoryFirmware, FirmwareCommandGroup.FactoryFirmwareCommand },
                { DeviceInfoFieldToolTipTag.UpdatedFirmware, FirmwareCommandGroup.SecondaryFirmwareCommand },
                { DeviceInfoFieldToolTipTag.CurrentFirmware, FirmwareCommandGroup.CurrentFirmwareCommand },
            };
            return additionalToolTipsMap;
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            UpdateDisplay();
        }

        private void InitializeCommandVisualsToCommandsMap()
        {
            _blockWhenBusy[DeviceCommandGroup.SetDeviceNameCommand] = DeviceCommandGroup.SetDeviceNameCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetDeviceOwnerCommand] = DeviceCommandGroup.SetDeviceOwnerCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetShowTitleScreenCommand] = DeviceCommandGroup.SetShowTitleScreenCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetEcsCompatibilityCommand] = DeviceCommandGroup.SetEcsCompatibilityCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetIntellivisionIICompatibilityCommand] = DeviceCommandGroup.SetIntellivisionIICompatibilityCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetKeyclicksCommand] = DeviceCommandGroup.SetKeyclicksCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetEnableConfigMenuOnCartCommand] = DeviceCommandGroup.SetEnableConfigMenuOnCartCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetRandomizeLtoFlashRamCommand] = DeviceCommandGroup.SetRandomizeLtoFlashRamCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetSaveMenuPositionCommand] = DeviceCommandGroup.SetSaveMenuPositionCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetBackgroundGarbageCollectCommand] = DeviceCommandGroup.SetBackgroundGarbageCollectCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[FirmwareCommandGroup.UpdateFirmwareCommand] = FirmwareCommandGroup.UpdateFirmwareCommand.BlockWhenAppIsBusy;

            // Settings page commands
            _controlCommandMap[ECSCompatibilityButton] = DeviceCommandGroup.SetEcsCompatibilityCommand;
            _controlCommandMap[IntellivisionIICompatibilityButton] = DeviceCommandGroup.SetIntellivisionIICompatibilityCommand;
            _controlCommandMap[ShowTitleScreenButton] = DeviceCommandGroup.SetShowTitleScreenCommand;
            _controlCommandMap[SaveMenuPositionButton] = DeviceCommandGroup.SetSaveMenuPositionCommand;
            _controlCommandMap[KeyclicksCheckBox] = DeviceCommandGroup.SetKeyclicksCommand;
            _controlCommandMap[BackgroundGCCheckBox] = DeviceCommandGroup.SetBackgroundGarbageCollectCommand;
            _controlCommandMap[EnableConfigMenuOnCartCheckBox] = DeviceCommandGroup.SetEnableConfigMenuOnCartCommand;
            _controlCommandMap[RandomizeLtoFlashRamCheckBox] = DeviceCommandGroup.SetRandomizeLtoFlashRamCommand;

            // Firmware page commands
            _controlCommandMap[UpdateFirmwareButton] = FirmwareCommandGroup.UpdateFirmwareCommand;

            // Additional commands to map
            var additionalItemsMap = AdditionalToolTipsMap.Value;
            var tabView = Window.ContentView.FindChild<NSTabView>();
            foreach (var tabViewItem in tabView.Items)
            {
                foreach (var textField in tabViewItem.View.FindChildren<NSTextField>(f => f.Tag != 0))
                {
                    VisualRelayCommand command;
                    if (additionalItemsMap.TryGetValue((DeviceInfoFieldToolTipTag)textField.Tag, out command))
                    {
                        _controlCommandMap[textField] = command;
                    }
                }
            }

            foreach (var controlCommandMapEntry in _controlCommandMap)
            {
                var commandVisual = controlCommandMapEntry.Key;
                var controlCommand = controlCommandMapEntry.Value;
                var tag = (DeviceInfoFieldToolTipTag)commandVisual.Tag;
                if (additionalItemsMap.ContainsKey(tag))
                {
                    var toolTip = controlCommand.ToolTipDescription;
                    switch (tag)
                    {
                        case DeviceInfoFieldToolTipTag.EcsCompatibilty:
                            toolTip = Resources.Strings.SetEcsCompatibilityCommand_TipDescription;
                            break;
                        case DeviceInfoFieldToolTipTag.IntellivisionIICompatibility:
                            toolTip = Resources.Strings.SetIntellivisionIICompatibilityCommand_TipDescription;
                            break;
                        case DeviceInfoFieldToolTipTag.SaveMenuPosition:
                            toolTip = Resources.Strings.SetSaveMenuPositionCommand_TipDescription;
                            break;
                        case DeviceInfoFieldToolTipTag.ShowTitleScreen:
                            toolTip = Resources.Strings.SetShowTitleScreenCommand_TipDescription;
                            break;
                        default:
                            break;
                    }
                    commandVisual.ToolTip = toolTip;
                }
                else
                {
                    controlCommand.BindCommandVisualToToolTipDescription(commandVisual);
                }
            }

            foreach (var command in _blockWhenBusy.Keys)
            {
                command.BlockWhenAppIsBusy = false;
            }
        }

        #region DeviceOwnerEditing

        private string CloseButtonKey { get; set; }

        private bool ShouldBeginEditingDeviceName(NSControl control, NSText fieldEditor)
        {
            return ShouldBeginEditingDeviceInfo(control, fieldEditor, DeviceCommandGroup.SetDeviceNameCommand);
        }

        private bool ShouldBeginEditingDeviceOwner(NSControl control, NSText fieldEditor)
        {
            return ShouldBeginEditingDeviceInfo(control, fieldEditor, DeviceCommandGroup.SetDeviceOwnerCommand);
        }

        private DeviceInfoField EditingField { get; set; }

        private bool ShouldBeginEditingDeviceInfo(NSControl control, NSText fieldEditor, VisualDeviceCommand deviceInfoEditCommand)
        {
            EditingField = DeviceInfoField.None;
            var editDeviceName = deviceInfoEditCommand.UniqueId == DeviceCommandGroup.SetDeviceNameCommand.UniqueId;
            var editDeviceOwner = deviceInfoEditCommand.UniqueId == DeviceCommandGroup.SetDeviceOwnerCommand.UniqueId;
            var shouldEdit = editDeviceName || editDeviceOwner;
            if (shouldEdit)
            {
                shouldEdit = deviceInfoEditCommand.CanExecute(ViewModel);
            }
            if (shouldEdit)
            {
                if (editDeviceName)
                {
                    EditingField = DeviceInfoField.Name;
                }
                else if (editDeviceOwner)
                {
                    EditingField = DeviceInfoField.Owner;
                }

                CloseButtonKey = CloseButtonControl.KeyEquivalent;
                CloseButtonControl.KeyEquivalent = string.Empty;
                if (InPlaceEditor == null)
                {
                    InPlaceEditor = new TextCellInPlaceEditor(editDeviceName ? DeviceNameEntry : DeviceOwnerEntry);
                }
                InPlaceEditor.EditingObject = ViewModel.ActiveLtoFlashDevice.Device;
                InPlaceEditor.EditedElement = fieldEditor;
                InPlaceEditor.InitialValue = editDeviceName ? DeviceName : DeviceOwner;
                InPlaceEditor.MaxLength = editDeviceName ? INTV.LtoFlash.Model.FileSystemConstants.MaxShortNameLength : INTV.LtoFlash.Model.FileSystemConstants.MaxLongNameLength;
                InPlaceEditor.IsValidCharacter = (c) => INTV.Core.Model.Grom.Characters.Contains(c);
                InPlaceEditor.EditorClosed += InPlaceEditor_EditorClosed;
                InPlaceEditor.BeginEdit();
            }
            return shouldEdit;
        }

        private bool ShouldEndEditingDeviceName(NSControl control, NSText fieldEditor)
        {
            return ShouldEndEditingDeviceInfo(control, fieldEditor, DeviceCommandGroup.SetDeviceNameCommand);
        }

        private bool ShouldEndEditingDeviceOwner(NSControl control, NSText fieldEditor)
        {
            return ShouldEndEditingDeviceInfo(control, fieldEditor, DeviceCommandGroup.SetDeviceOwnerCommand);
        }

        private bool ShouldEndEditingDeviceInfo(NSControl control, NSText fieldEditor, VisualDeviceCommand deviceInfoEditCommand)
        {
            if (InPlaceEditor != null)
            {
                InPlaceEditor.CommitEdit();
                if (InPlaceEditor != null)
                {
                    InPlaceEditor.EditorClosed -= InPlaceEditor_EditorClosed;
                }
            }

            // If we don't BeginInvoke here, if we're ending edit due to Escape key, the
            // button will pick it up and close the Device Information dialog prematurely.
            OSDispatcher.Current.BeginInvoke(() => CloseButtonControl.KeyEquivalent = CloseButtonKey);
            return true;
        }

        private void InPlaceEditor_EditorClosed(object sender, InPlaceEditorClosedEventArgs e)
        {
            if (InPlaceEditor != null)
            {
                InPlaceEditor.EditorClosed -= InPlaceEditor_EditorClosed;
                var textEditor = InPlaceEditor.EditedElement as NSText;
                InPlaceEditor = null;

                var editDeviceName = EditingField == DeviceInfoField.Name;
                var editDeviceOwner = EditingField == DeviceInfoField.Owner;

                if (editDeviceName)
                {
                    DeviceNameEntry.Cell.EndEditing(textEditor);
                }
                else if (editDeviceOwner)
                {
                    DeviceOwnerEntry.Cell.EndEditing(textEditor);
                }
                if (e.CommitedChanges && (editDeviceName || editDeviceOwner))
                {
                    if (editDeviceName)
                    {
                        ViewModel.ActiveLtoFlashDevice.Name = DeviceNameEntry.StringValue;
                        DeviceCommandGroup.SetDeviceNameCommand.Execute(ViewModel);
                    }
                    else if (editDeviceOwner)
                    {
                        ViewModel.ActiveLtoFlashDevice.Owner = DeviceOwnerEntry.StringValue;
                        DeviceCommandGroup.SetDeviceOwnerCommand.Execute(ViewModel);
                    }
                }
            }
            EditingField = DeviceInfoField.None;
        }

        #endregion // DeviceOwnerEditing

        private void HandleRequerySuggested(object sender, System.EventArgs args)
        {
            var canEdit = DeviceCommandGroup.SetDeviceNameCommand.CanExecute(ViewModel);
            if (DeviceNameEntry.Editable != canEdit)
            {
                DeviceNameEntry.Editable = canEdit;
            }

            canEdit = DeviceCommandGroup.SetDeviceOwnerCommand.CanExecute(ViewModel);
            if (DeviceOwnerEntry.Editable != canEdit)
            {
                DeviceOwnerEntry.Editable = canEdit;
            }

            foreach (var controlCommand in _controlCommandMap)
            {
                var commandVisual = controlCommand.Key;
                if (!AdditionalToolTipsMap.Value.ContainsKey((DeviceInfoFieldToolTipTag)commandVisual.Tag))
                {
                    var enable = controlCommand.Value.CanExecute(ViewModel);
                    commandVisual.Enabled = enable;
                }
            }
        }

        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleViewModelPropertyChangedCore);
        }

        private void HandleViewModelPropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case LtoFlashViewModel.ActiveLtoFlashDevicePropertyName:
                    UpdateDisplay();
                    break;
                default:
                    break;
            }
        }

        private void HandleDevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleDevicePropertyChangedCore);
        }

        private void HandleDevicePropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DisplayName":
                case Device.NamePropertyName:
                case Device.OwnerPropertyName:
                case Device.UniqueIdPropertyName:
                    UpdateInfo();
                    break;
                case Device.FirmwareRevisionsPropertyName:
                    UpdateFirmwareInfo();
                    break;
                case Device.FileSystemStatisticsPropertyName:
                    UpdateFileSystemInfo();
                    break;
                case Device.EcsCompatibilityPropertyName:
                    ECSCompatibilityButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.EcsCompatibility);
                    break;
                case Device.IntvIICompatibilityPropertyName:
                    IntellivisionIICompatibilityButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.IntvIICompatibility);
                    break;
                case Device.ShowTitleScreenPropertyName:
                    ShowTitleScreenButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.ShowTitleScreen);
                    break;
                case Device.SaveMenuPositionPropertyName:
                    SaveMenuPositionButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.SaveMenuPosition);
                    break;
                case Device.BackgroundGCPropertyName:
                case Device.KeyclicksPropertyName:
                case Device.EnableConfigMenuOnCartPropertyName:
                case DeviceViewModel.RandomizeLtoFlashRamPropertyName:
                    this.RaiseChangeValueForKey(e.PropertyName);
                    break;
                default:
                    break;
            }
        }

        private void UpdateDisplay()
        {
            if (_device != null)
            {
                _device.PropertyChanged -= HandleDevicePropertyChanged;
            }
            _device = ViewModel.ActiveLtoFlashDevice;
            if (_device != null)
            {
                _device.PropertyChanged += HandleDevicePropertyChanged;
            }

            UpdateInfo();
            UpdateFirmwareInfo();
            UpdateFileSystemInfo();
        }

        private void UpdateInfo()
        {
            var connectionName = Resources.Strings.NoDevice;
            if (ViewModel.ActiveLtoFlashDevice.IsValid)
            {
                var port = ViewModel.ActiveLtoFlashDevice.Device.Port;
                connectionName = port.ToString();
            }
            ConnectionName = new NSString(connectionName);

            DeviceOwner = new NSString(ViewModel.ActiveLtoFlashDevice.Owner.SafeString());
            DeviceName = new NSString(ViewModel.ActiveLtoFlashDevice.Name.SafeString());
            DeviceId = new NSString(ViewModel.ActiveLtoFlashDevice.UniqueId.SafeString());
        }

        private void UpdateFirmwareInfo()
        {
            FactoryFirmwareVersion = new NSString(ViewModel.FirmwareRevisions.Primary.SafeString());
            SecondaryFirmwareVersion = new NSString(ViewModel.FirmwareRevisions.Secondary.SafeString());
            CurrentFirmwareVersion = new NSString(ViewModel.FirmwareRevisions.Current.SafeString());
        }

        private void UpdateFileSystemInfo()
        {
            PhysBlocksAvail = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksAvailable);
            PhysBlocksInUse = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksInUse);
            PhysCleanBlocks = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksClean);
            PhysTotalBlocks = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksTotal);
            VirtBlocksAvail = new NSString(ViewModel.FileSystemStatistics.VirtualBlocksAvailable);
            VirtBlocksInUse = new NSString(ViewModel.FileSystemStatistics.VirtualBlocksInUse);
            VirtTotalBlocks = new NSString(ViewModel.FileSystemStatistics.VirtualBlocksTotal);
            PhysSectorErasures = new NSString(ViewModel.FileSystemStatistics.PhysicalSectorErasures);
            MetadataSectorErasures = new NSString(ViewModel.FileSystemStatistics.MetadataSectorErasures);
            VirtToPhysMapVer = new NSString(ViewModel.FileSystemStatistics.VirtualToPhysicalMapVersion);
            PBlockErasures = new NSString(ViewModel.FileSystemStatistics.PercentFlashLifetimeUsedByPhysicalBlockErasures);
            VtoPLogWraps = new NSString(ViewModel.FileSystemStatistics.PercentageFlashLifetimeUsedByVirtualToPhysicalMap);
            LifeRemaining = new NSString(ViewModel.FileSystemStatistics.PercentageLifetimeRemaining);
        }

        /// <summary>
        /// Handles the update firmware button click.
        /// </summary>
        /// <param name="sender">The update firmware button.</param>
        partial void OnUpdateFirmware(NSObject sender)
        {
            FirmwareCommandGroup.UpdateFirmwareCommand.Execute(ViewModel);
        }

        /// <summary>
        /// Handles close button click.
        /// </summary>
        /// <param name="sender">The cluse button.</param>
        partial void OnClose(NSObject sender)
        {
            if (InPlaceEditor != null)
            {
                InPlaceEditor.CommitEdit();
            }
            Window.EndDialog(NSRunResponse.Aborted);
            Properties.Settings.Default.ShowFileSystemDetails = false;
            INTV.Shared.Utility.SingleInstanceApplication.Instance.IsBusy = false;
            CommandManager.RequerySuggested -= HandleRequerySuggested;
        }

        private enum DeviceInfoField
        {
            /// <summary>No device info.</summary>
            None,

            /// <summary>Device name field.</summary>
            Name,

            /// <summary>Device owner field./// </summary>
            Owner
        }

        private enum DeviceInfoFieldToolTipTag
        {
            /// <summary>Not a valid tag.</summary>
            None,

            /// <summary>Tag for device name.</summary>
            Name = 1,

            /// <summary>Tag for device owner.</summary>
            Owner,

            /// <summary>Tag for DRUID.</summary>
            UniqueId,

            /// <summary>Tag for show title screen.</summary>
            ShowTitleScreen = 101,

            /// <summary>Tag for Intellivision II compatibility.</summary>
            IntellivisionIICompatibility,

            /// <summary>Tag for ECS compatibility.</summary>
            EcsCompatibilty,

            /// <summary>Tag for save menu position.</summary>
            SaveMenuPosition,

            /// <summary>Tag for factory firmware version.</summary>
            FactoryFirmware = 201,

            /// <summary>Tag for secondary firmware version.</summary>
            UpdatedFirmware,

            /// <summary>Tag for current firmware version.</summary>
            CurrentFirmware,

            /// <summary>Tag for physical blocks in use.</summary>
            PhysicalBlocksInUse = 301,

            /// <summary>Tag for physical clean blocks.</summary>
            PhysicalCleanBlocks,

            /// <summary>Tag for total physical blocks.</summary>
            PhysicalTotalBlocks,

            /// <summary>Tag for physical sector erasures.</summary>
            PhysicalSectorErasures,

            /// <summary>Tag for metadata erasures.</summary>
            PhysicalMetadataSectorErasures,

            /// <summary>Tag for V to P map version.</summary>
            VirutalToPhysicalMapVersion,

            /// <summary>Tag for virtual blcoks in use.</summary>
            VirtualBlocksInUse,

            /// <summary>Tag for total virutal blocks.</summary>
            VirtualTotalBlocks,

            /// <summary>Tag for available virtual blocks.</summary>
            VirtualBlocksAvailable,

            /// <summary>Tag for physical block erasures.</summary>
            PhysicalBlockErasures,

            /// <summary>Tag for lifetime remaining in terms of V to P wraps.</summary>
            VirtualToPhysicalWraps,

            /// <summary>Tag for lifetime remaining.</summary>
            LifetimeRemaining
        }

        private class ControllerButtonsData : IKImageBrowserDataSource
        {
            #region IKImageBrowserDataSource

            public override nint ItemCount(IKImageBrowserView aBrowser)
            {
                throw new System.NotImplementedException();
            }

            public override IIKImageBrowserItem GetItem(IKImageBrowserView aBrowser, nint index)
            {
                throw new System.NotImplementedException();
            }

            #endregion // IKImageBrowserDataSource
        }

        private class ControllerButtonItem : IKImageBrowserItem
        {
            #region IKImageBrowserItem

            public override string ImageUID
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            public override NSString ImageRepresentationType
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            public override NSObject ImageRepresentation
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            #endregion // IKImageBrowserItem
        }
    }
}

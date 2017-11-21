// <copyright file="DeviceCommandGroup.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.ComponentModel;
using INTV.Shared.Commands;

#if __UNIFIED__
using CGSize = CoreGraphics.CGSize;
#else
using CGSize = System.Drawing.SizeF;
#endif // __UNIFIED__

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class DeviceCommandGroup
    {
        #region DeviceInformationCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualDeviceCommand DeviceInformationCommand = new VisualDeviceCommand(OnDeviceInformation)
        {
            UniqueId = UniqueNameBase + ".DeviceInformationCommand",
            Name = Resources.Strings.DeviceInformationCommand_Name,
            PreferredParameterType = typeof(LtoFlashViewModel),
            Weight = 0.015,
            KeyboardShortcutKey = "i",
            KeyboardShortcutModifiers = OSModifierKeys.Menu
        };

        private static void OnDeviceInformation(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var dialog = INTV.LtoFlash.View.DeviceInformation.Create(viewModel);
            dialog.ShowWindow();
        }

        #endregion // DeviceInformationCommand

        #region ConnectToDeviceSubmenuCommand

        /// <summary>
        /// Submenu pseudo-command for connecting to a device.
        /// </summary>
        public static readonly VisualDeviceCommand ConnectToDeviceSubmenuCommand = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ConnectToDeviceSubmenuCommand",
            Name = Resources.Strings.ConnectToDevice_GroupName,
            Weight = 0.011,
            MenuParent = RootCommandGroup.ToolsMenuCommand
        };

        #endregion // ConnectToDeviceSubmenuCommand

        /// <summary>
        /// Populates the menu for a <see cref="NSPopUpButton"/> for selecting Intellivision II compatibility mode.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        public static void PopulateIntellivisionIICompatibilityMenu(NSPopUpButton button)
        {
            var intyIIItems = new[] {
                Resources.Strings.IntellivisionIICompatibilityMode_Disabled,
                Resources.Strings.IntellivisionIICompatibilityMode_Limited,
                Resources.Strings.IntellivisionIICompatibilityMode_Full
            };
            var intyIIItemTags = new[] {
                (int)IntellivisionIIStatusFlags.None,
                (int)IntellivisionIIStatusFlags.Conservative,
                (int)IntellivisionIIStatusFlags.Aggressive
            };
            var intyIIItemTips = new[] {
                Resources.Strings.IntellivisionIICompatibilityMode_Disabled_ToolTipDescription,
                Resources.Strings.IntellivisionIICompatibilityMode_Limited_ToolTipDescription,
                Resources.Strings.IntellivisionIICompatibilityMode_Full_ToolTipDescription
            };
            SetIntellivisionIICompatibilityCommand.PopulatePopUpButton(button, intyIIItems, intyIIItemTags, intyIIItemTips);
        }

        /// <summary>
        /// Populates the menu for a <see cref="NSPopUpButton"/> for selecting ECS compatibility mode.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        public static void PopulateEcsCompatibilityMenu(NSPopUpButton button)
        {
            var ecsItems = new[] {
                Resources.Strings.EcsCompatibilityMode_Enabled,
                Resources.Strings.EcsCompatibilityMode_Limited,
                Resources.Strings.EcsCompatibilityMode_Strict,
                Resources.Strings.EcsCompatibilityMode_Disabled
            };
            var ecsItemTags = new[] {
                (int)EcsStatusFlags.None,
                (int)EcsStatusFlags.EnabledForRequiredAndOptional,
                (int)EcsStatusFlags.EnabledForRequired,
                (int)EcsStatusFlags.Disabled
            };
            var ecsItemTips = new[] {
                Resources.Strings.EcsCompatibilityMode_Enabled_ToolTipDescription,
                Resources.Strings.EcsCompatibilityMode_Limited_ToolTipDescription,
                Resources.Strings.EcsCompatibilityMode_Strict_ToolTipDescription,
                Resources.Strings.EcsCompatibilityMode_Disabled_ToolTipDescription
            };
            SetEcsCompatibilityCommand.PopulatePopUpButton(button, ecsItems, ecsItemTags, ecsItemTips);
        }

        /// <summary>
        /// Populates the menu for a <see cref="NSPopUpButton"/> for selecting title screen display mode.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        public static void PopulateShowTitleMenu(NSPopUpButton button)
        {
            var showTitleItems = new[] {
                SetShowTitleScreenCommandChoiceAlways.MenuItemName, // Resources.Strings.ShowTitleScreen_Always,
                SetShowTitleScreenCommandChoiceOnPowerUp.MenuItemName, // Resources.Strings.ShowTitleScreen_OnPowerUp,
                SetShowTitleScreenCommandChoiceNever.MenuItemName, // Resources.Strings.ShowTitleScreen_Never
            };
            var showTitleTags = new[] {
                (int)ShowTitleScreenFlags.Always,
                (int)ShowTitleScreenFlags.OnPowerUp,
                (int)ShowTitleScreenFlags.Never
            };
            var showTitleTips = new[] {
                SetShowTitleScreenCommandChoiceAlways.ToolTip, // Resources.Strings.ShowTitleScreen_Always_ToolTipDescription,
                SetShowTitleScreenCommandChoiceOnPowerUp.ToolTip, // Resources.Strings.ShowTitleScreen_OnPowerUp_ToolTipDescription,
                SetShowTitleScreenCommandChoiceNever.ToolTip // Resources.Strings.ShowTitleScreen_Never_ToolTipDescription
            };
            SetShowTitleScreenCommand.PopulatePopUpButton(button, showTitleItems, showTitleTags, showTitleTips);
        }

        /// <summary>
        /// Populates the menu for a <see cref="NSPopUpButton"/> for selecting save menu position behavior.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        public static void PopulateSaveMenuPositionMenu(NSPopUpButton button)
        {
            var showTitleItems = new[] {
                SetSaveMenuPositionCommandChoiceAlways.MenuItemName, // Resources.Strings.ShowTitleScreen_Always,
                SetSaveMenuPositionCommandChoiceDuringSession.MenuItemName, // Resources.Strings.ShowTitleScreen_OnPowerUp,
                SetSaveMenuPositionCommandChoiceNever.MenuItemName,
            };
            var showTitleTags = new[] {
                (int)SaveMenuPositionFlags.Always,
                (int)SaveMenuPositionFlags.DuringSessionOnly,
                (int)SaveMenuPositionFlags.Never
            };
            var showTitleTips = new[] {
                SetSaveMenuPositionCommandChoiceAlways.ToolTip,
                SetSaveMenuPositionCommandChoiceDuringSession.ToolTip,
                SetSaveMenuPositionCommandChoiceNever.ToolTip
            };
            SetSaveMenuPositionCommand.PopulatePopUpButton(button, showTitleItems, showTitleTags, showTitleTips);
        }

        private static ConnectionMenuDelegate MenuDelegate { get; set; }

        /// <summary>
        /// Populates the submenu used to select a serial port that may be connected to LTO Flash! hardware.
        /// </summary>
        /// <param name="viewModel"></param>
        public void InitializeConnectionMenu(LtoFlashViewModel viewModel)
        {
            var menuItem = ConnectToDeviceSubmenuCommand.MenuItem;
            if (menuItem.Submenu == null)
            {
                var menu = new NSMenu();
                MenuDelegate = new ConnectionMenuDelegate(viewModel, this);
                menu.Delegate = MenuDelegate;
                menu.AutoEnablesItems = false;
                MenuDelegate.InitializeMenu(menu);
                menuItem.Submenu = menu;
            }
        }

#if DEBUG

        private static VisualRelayCommand ToggleConsolePowerCommand = new VisualRelayCommand(OnTogglePower, CanTogglePower)
        {
            UniqueId = UniqueNameBase + ".ToggleConsolePowerCommand",
            Name = "Toggle Console Power",
            KeyboardShortcutModifiers = OSModifierKeys.Menu | OSModifierKeys.Ctrl | OSModifierKeys.Alt,
            KeyboardShortcutKey = "I",
            MenuParent = INTV.Shared.Commands.DebugCommandGroup.DebugMenuCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnTogglePower(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var deviceViewModel = viewModel.ActiveLtoFlashDevice;
            var hardwareFlags = deviceViewModel.Device.HardwareStatus ^ HardwareStatusFlags.ConsolePowerOn;
            LtoFlashViewModel.SetHardwareStatus(deviceViewModel.Device, hardwareFlags);
        }

        private static bool CanTogglePower(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var deviceViewModel = viewModel.ActiveLtoFlashDevice;
            return deviceViewModel.IsValid;
        }

#endif // DEBUG

        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return LtoFlashCommandGroup.Group.Context; }
        }

        #endregion // CommandGroup

        #region ICommandGroup

        /// <inheritdoc />
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            var visual = base.CreateVisualForCommand(command);
            var visualCommand = (VisualRelayCommand)command;
            var toolbarItem = visual as NSToolbarItem;
            switch (visualCommand.UniqueId)
            {
                case UniqueNameBase + ".DeviceNameCommand":
                case UniqueNameBase + ".DeviceIdCommand":
                case UniqueNameBase + ".DeviceOwnerCommand":
                    var textField = new NSTextField();
                    toolbarItem.View = textField;
                    toolbarItem.MinSize = new CGSize(128, 22);
                    break;
                case UniqueNameBase + ".SetEcsCompatibilityCommand":
                    // Used to have NSToolbar control for this
                    break;
                case UniqueNameBase + ".SetIntellivisionIICompatibilityCommand":
                    // Used to have NSToolbar control for this.
                    break;
                default:
                    break;
            }
            return visual;
        }

        partial void AddPlatformCommands()
        {
#if DEBUG
            CommandList.Add(ToggleConsolePowerCommand);
#endif // DEBUG

            SearchForDevicesCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            SearchForDevicesCommand.VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand;

            CommandList.Add(ConnectToDeviceSubmenuCommand);

            DisconnectDeviceCommand.Weight = 0.012;
            DisconnectDeviceCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            DisconnectDeviceCommand.KeyboardShortcutKey = "d";
            DisconnectDeviceCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;
            DisconnectDeviceCommand.VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand;

            DeviceInformationCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            CommandList.Add(DeviceInformationCommand);

            CommandList.Add(DeviceInformationCommand.CreateSeparator(CommandLocation.After));

            CommandList.Add(OpenErrorLogsDirectoryCommand.CreateSeparator(CommandLocation.Before));

            CommandList.Add(ReformatCommand.CreateSeparator(CommandLocation.Before));

            SetEcsCompatibilityCommand.Weight = 0.6;
            SetIntellivisionIICompatibilityCommand.Weight = 0.61;

            SetActiveDeviceCommand.MenuParent = ConnectToDeviceSubmenuCommand;
        }

        #endregion // ICommandGroup

        /// <remarks>See https://bugzilla.xamarin.com/show_bug.cgi?id=39507 for notes about
        /// a hard crash due to bad binding defined for HasKeyEquivalentForEvent().</remarks>
        private class ConnectionMenuDelegate : NSMenuDelegate
        {
            private DeviceCommandGroup _group;
            private LtoFlashViewModel _ltoFlash;
            private Dictionary<int, DeviceConnectionViewModel> _deviceConnectionViewModels = new Dictionary<int, DeviceConnectionViewModel>();

            public ConnectionMenuDelegate(LtoFlashViewModel ltoFlash, DeviceCommandGroup commandGroup)
            {
                _group = commandGroup;
                _ltoFlash = ltoFlash;
                CommandManager.RequerySuggested += HandleRequerySuggested;
            }

            /// <inheritdoc/>
            public override void MenuWillHighlightItem (NSMenu menu, NSMenuItem item)
            {
            }

            /// <inheritdoc/>
            public override void MenuWillOpen(NSMenu menu)
            {
                InitializeMenu(menu);
            }

            /// <summary>
            /// Initializes the menu.
            /// </summary>
            /// <param name="menu">The menu whose items are to be set.</param>
            internal void InitializeMenu(NSMenu menu)
            {
                _deviceConnectionViewModels.Clear();
                var ports = DeviceConnectionViewModel.GetAvailableConnections(_ltoFlash);
                menu.RemoveAllItems();
                var shortcutNumber = 0;
                foreach (var port in ports)
                {
                    ++shortcutNumber;
                    var menuItem = SetActiveDeviceCommand.CreateMenuItemForCommand(_group, true, null);
                    menuItem.Title = port.Name;
                    menuItem.Activated += HandleSetActiveDeviceCommandActivated; // doesn't use standard approach
                    menuItem.Enabled = SetActiveDeviceCommand.CanExecute(port);
                    if (shortcutNumber < 10)
                    {
                        menuItem.KeyEquivalent = shortcutNumber.ToString();
                        menuItem.KeyEquivalentModifierMask  = NSEventModifierMask.CommandKeyMask;
                    }
                    _deviceConnectionViewModels[menuItem.GetHashCode()] = port;
                }
            }

            private void HandleSetActiveDeviceCommandActivated(object sender, System.EventArgs e)
            {
                DeviceConnectionViewModel deviceConnection = null;
                if (_deviceConnectionViewModels.TryGetValue(sender.GetHashCode(), out deviceConnection))
                {
                    SetActiveDeviceCommand.Execute(deviceConnection);
                }
            }

            private void HandleRequerySuggested(object sender, System.EventArgs e)
            {
                var connectionsMenuItem = ConnectToDeviceSubmenuCommand.MenuItem;
                if ((connectionsMenuItem != null) && connectionsMenuItem.HasSubmenu && (connectionsMenuItem.Submenu != null))
                {
                    foreach (var menuItem in connectionsMenuItem.Submenu.ItemArray())
                    {
                        DeviceConnectionViewModel deviceConnection = null;
                        if (_deviceConnectionViewModels.TryGetValue(menuItem.GetHashCode(), out deviceConnection))
                        {
                            menuItem.Enabled = SetActiveDeviceCommand.CanExecute(deviceConnection);
                        }
                    }
                }
            }
        }
    }
}

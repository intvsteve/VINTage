// <copyright file="DeviceCommandGroup.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// GTK-specific implementation.
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

        #region SearchForDevicesToolbarCommand

        /// <summary>
        /// Container Gtk.MenuToolButton command that has actual commands added at run-time.
        /// </summary>
        public static readonly VisualRelayCommand SearchForDevicesToolbarCommand = new VisualRelayCommand(OnSearchForDevices, CanSearchForDevices)
            {
                UniqueId = UniqueNameBase + ".SearchForDevicesToolbarCommand",
                Name = SearchForDevicesCommand.Name,
                ToolTip = SearchForDevicesCommand.ToolTip,
                ToolTipTitle = SearchForDevicesCommand.ToolTipTitle,
                ToolTipDescription = SearchForDevicesCommand.ToolTipDescription,
                ToolTipIcon = SearchForDevicesCommand.ToolTipIcon,
                LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_search_32xLG.png"),
                VisualParent = RootCommandGroup.RootCommand,
                KeyboardShortcutKey = SearchForDevicesCommand.KeyboardShortcutKey,
                KeyboardShortcutModifiers = SearchForDevicesCommand.KeyboardShortcutModifiers,
                Weight = 0.12,
            };

        #endregion // SearchForDevicesToolbarCommand

        #region ConnectToDeviceSubmenuCommand

        /// <summary>
        /// Submenu pseudo-command for connecting to a device.
        /// </summary>
        public static readonly VisualDeviceCommand ConnectToDeviceSubmenuCommand = new VisualDeviceCommand(RelayCommand.NoOp)
            {
                UniqueId = UniqueNameBase + ".ConnectToDeviceSubmenuCommand",
                Name = Resources.Strings.ConnectToDevice_GroupName,
                Weight = 0.121,
                MenuParent = RootCommandGroup.ToolsMenuCommand
            };

        #endregion // ConnectToDeviceSubmenuCommand

        #region CommandGroup

        /// <summary>
        /// Gets the general data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return LtoFlashCommandGroup.Group.Context; }
        }

        /// <summary>
        /// Creates the toolbar item for command.
        /// </summary>
        /// <param name="command">The command whose toolbar visual is created.</param>
        /// <returns>The toolbar visual for the given command.</returns>
        public override OSVisual CreateToolbarItemForCommand(ICommand command)
        {
            OSVisual visual;
            var visualRelayCommand = command as VisualRelayCommand;
            if (visualRelayCommand.UniqueId == SearchForDevicesToolbarCommand.UniqueId)
            {
                var menuToolButton = visualRelayCommand.CreateMenuToolButtonForCommand(true, InitializeAvailableDevicesMenu);
                visual = menuToolButton;
            }
            else
            {
                visual = base.CreateToolbarItemForCommand(command);
            }
            return visual;
        }

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item must be created.</param>
        /// <returns>The menu item.</returns>
        public override OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            var menuItem = base.CreateMenuItemForCommand(command);
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand.UniqueId == ConnectToDeviceSubmenuCommand.UniqueId)
            {
                var submenu = new Gtk.Menu() { Name = visualCommand.UniqueId };
                menuItem.NativeMenuItem.Submenu = submenu;
                menuItem.NativeMenuItem.Activated += HandleConnectToDevicesSubmenuActivated;
                InitializeAvailableDevicesMenu(submenu, true);
            }
            return menuItem;
        }

        private static void InitializeAvailableDevicesMenu(Gtk.Menu menu)
        {
            InitializeAvailableDevicesMenu(menu, false);
        }

        /// <summary>
        /// Initializes the menu.
        /// </summary>
        /// <param name="menu">The menu whose items are to be set.</param>
        /// <param name="addShortcuts">If <c>true</c>, add menu item shortcuts to the items.</param>
        private static void InitializeAvailableDevicesMenu(Gtk.Menu menu, bool addShortcuts)
        {
            var items = menu.Children;
            for (var i = 0; i < items.Length; ++i)
            {
                var menuItem = items[i] as Gtk.MenuItem;
                if (menuItem != null)
                {
                    // Remove any existing accelerators
                    if (addShortcuts)
                    {
                        var key = Gdk.Key.Key_0 + i + 1;
                        var modifiers = Gdk.ModifierType.ControlMask;
                        var acceleratorKey = new Gtk.AccelKey(key, modifiers, Gtk.AccelFlags.Visible);
                        SetActiveDeviceCommand.RemoveAccelerator(menuItem, acceleratorKey);
                    }
                    menu.Remove(menuItem);
                }
            }

            var ports = DeviceConnectionViewModel.GetAvailableConnections(Group.Context as LtoFlashViewModel);
            var shortcutNumber = 0;
            foreach (var port in ports)
            {
                ++shortcutNumber;
                var menuItem = SetActiveDeviceCommand.CreateMenuItem(menu, ConnectToDeviceSubmenuCommand, Gtk.StockItem.Zero, null, null);
                menuItem.SetName(port.Name);
                menuItem.NativeMenuItem.Sensitive = SetActiveDeviceCommand.CanExecute(port);
                if (addShortcuts && (shortcutNumber < 10))
                {
                    var key = Gdk.Key.Key_0 + shortcutNumber;
                    var modifiers = Gdk.ModifierType.ControlMask;
                    var acceleratorKey = new Gtk.AccelKey(key, modifiers, Gtk.AccelFlags.Visible);
                    SetActiveDeviceCommand.AddAccelerator(menuItem, acceleratorKey);
                }
                menuItem.SetValue(IFakeDependencyObjectHelpers.DataContextPropertyName, port);
            }
        }

        private void HandleConnectToDevicesSubmenuActivated(object sender, System.EventArgs e)
        {
            var menuItem = sender as Gtk.MenuItem;
            var submenu = menuItem.Submenu as Gtk.Menu;
            InitializeAvailableDevicesMenu(submenu, true);
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            CommandList.Add(SearchForDevicesToolbarCommand);
            CommandList.Add(SearchForDevicesToolbarCommand.CreateToolbarSeparator(CommandLocation.Before));

            SearchForDevicesCommand.Weight = 0.12;
            SearchForDevicesCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;

            CommandList.Add(ConnectToDeviceSubmenuCommand);

            DisconnectDeviceCommand.Weight = 0.122;
            DisconnectDeviceCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            DisconnectDeviceCommand.KeyboardShortcutKey = "d";
            DisconnectDeviceCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;
            DisconnectDeviceCommand.VisualParent = RootCommandGroup.RootCommand;

            DeviceInformationCommand.Weight = 0.123;
            DeviceInformationCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            CommandList.Add(DeviceInformationCommand);

#if false
            SetShowTitleScreenCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetShowTitleScreenCommand.Weight = 0;

            SetEcsCompatibilityCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetEcsCompatibilityCommand.Weight = 0.1;

            SetIntellivisionIICompatibilityCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetIntellivisionIICompatibilityCommand.Weight = 0.12;

            CommandList.Add(SetIntellivisionIICompatibilityCommand.CreateRibbonSeparator(CommandLocation.After));

            SetSaveMenuPositionCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetSaveMenuPositionCommand.Weight = 0.13;

            SetKeyclicksCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetKeyclicksCommand.Weight = 0.14;

            SetBackgroundGarbageCollectCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetBackgroundGarbageCollectCommand.Weight = 0.15;

            SetDeviceNameCommand.VisualParent = ActiveDeviceRibbonGroupCommand;
            SetDeviceNameCommand.Weight = 0;

            SetDeviceOwnerCommand.VisualParent = ActiveDeviceRibbonGroupCommand;
            SetDeviceOwnerCommand.Weight = 0.1;

            DeviceUniqueIdCommand.VisualParent = ActiveDeviceRibbonGroupCommand;
            DeviceUniqueIdCommand.Weight = 0.2;

            AdvancedGroupCommand.Weight = 0.15;

            CommandList.Add(ActiveDeviceRibbonGroupCommand);
            CommandList.Add(DeviceSettingsRibbonGroupCommand);
            CommandList.Add(SetShowTitleScreenCommand);
            CommandList.Add(SetSaveMenuPositionCommand);
            CommandList.Add(SetKeyclicksCommand);
            CommandList.Add(SetBackgroundGarbageCollectCommand);
            CommandList.Add(SetDeviceNameCommand);
            CommandList.Add(SetDeviceOwnerCommand);
            CommandList.Add(DeviceUniqueIdCommand);
            CommandList.Add(BackupRibbonSplitButtonCommand);
            CommandList.Add(ShowFileSystemDetailsCommand);
            CommandList.Add(DisconnectDeviceCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(OpenDeviceBackupsDirectoryCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(ClearCacheCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(AdvancedGroupCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
#endif
            CommandList.Add(AdvancedGroupCommand);
            CommandList.Add(OpenDeviceBackupsDirectoryCommand.CreateSeparator(CommandLocation.After));
            CommandList.Add(ReformatCommand.CreateSeparator(CommandLocation.Before));
        }

        #endregion // CommandGroup
    }
}

// <copyright file="DeviceCommandGroup.WPF.cs" company="INTV Funhouse">
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

using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class DeviceCommandGroup
    {
        #region ActiveDeviceRibbonGroupCommand

        /// <summary>
        /// Pseudo-command for the Active Device ribbon group.
        /// </summary>
        public static readonly VisualDeviceCommand ActiveDeviceRibbonGroupCommand = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ActiveDeviceRibbonGroupCommand",
            Name = Resources.Strings.ActiveDevice_Header,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/device_info_32xMD.png"),
            Weight = 0.4,
            VisualParent = RootCommandGroup.HomeRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // ActiveDeviceRibbonGroupCommand

        #region DeviceSettingsRibbonGroupCommand

        /// <summary>
        /// Pseudo-command for the Device Settings ribbon group.
        /// </summary>
        public static readonly VisualDeviceCommand DeviceSettingsRibbonGroupCommand = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DeviceSettingsRibbonGroupCommand",
            Name = Resources.Strings.DeviceSettingsRibbonGroupCommand_Name,
            LargeIcon = typeof(INTV.Shared.Commands.CommandGroup).LoadImageResource("ViewModel/Resources/Images/settings_32xMD.png"), // resource is in INTV.Shared
            Weight = 0.1,
            VisualParent = LtoFlashCommandGroup.LtoFlashRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // DeviceSettingsRibbonGroupCommand

        #region ShowFileSystemDetailsCommand

        /// <summary>
        /// The command to show file system details in the UI.
        /// </summary>
        /// <remarks>NOTE: This command is bound to a preference.</remarks>
        public static readonly VisualRelayCommand ShowFileSystemDetailsCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ShowFileSystemDetailsCommand",
            Name = Resources.Strings.ShowFileSystemDetailsCommand_Name,
            MenuItemName = Resources.Strings.ShowFileSystemDetailsCommand_MenuItemName,
            ToolTipTitle = Resources.Strings.ShowFileSystemDetailsCommand_TipTitle,
            ToolTipDescription = Resources.Strings.ShowFileSystemDetailsCommand_TipDescription,
            Weight = 0.35,
            VisualParent = DownloadCommandGroup.FileSystemRibbonGroupCommand,
            UseXamlResource = true
        };

        #endregion // ShowFileSystemDetailsCommand

        #region BackupRibbonSplitButtonCommand

        /// <summary>
        /// The command to initiate a backup of the contents of an LTO Flash!
        /// </summary>
        public static readonly VisualDeviceCommand BackupRibbonSplitButtonCommand = new VisualDeviceCommand(OnBackup, CanBackup)
        {
            UniqueId = UniqueNameBase + ".BackupRibbonSplitButtonCommand",
            Name = Resources.Strings.BackupCommand_Name,
            MenuItemName = Resources.Strings.BackupCommand_MenuItemName,
            ToolTip = Resources.Strings.BackupCommand_TipDescription,
            ToolTipTitle = Resources.Strings.BackupCommand_TipTitle,
            ToolTipDescription = Resources.Strings.BackupCommand_TipDescription + CommandProviderHelpers.RibbonSplitButtonExtraToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/backup_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/backup_16xLG.png"),
            Weight = 0.34,
            VisualParent = DownloadCommandGroup.FileSystemRibbonGroupCommand,
            UseXamlResource = true
        };

        #endregion // BackupRibbonSplitButtonCommand

        #region CommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            SearchForDevicesCommand.MenuParent = LtoFlashCommandGroup.LtoFlashGroupCommand;
            SearchForDevicesCommand.VisualParent = LtoFlashCommandGroup.LtoFlashHomeRibbonGroupCommand;
            SearchForDevicesCommand.UseXamlResource = true;

            DisconnectDeviceCommand.MenuParent = LtoFlashCommandGroup.LtoFlashGroupCommand;
            DisconnectDeviceCommand.VisualParent = LtoFlashCommandGroup.LtoFlashHomeRibbonGroupCommand;
            DisconnectDeviceCommand.Weight = 0.12;
            DisconnectDeviceCommand.UseXamlResource = true;

            SetShowTitleScreenCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetShowTitleScreenCommand.UseXamlResource = true;
            SetShowTitleScreenCommand.Weight = 0;

            SetEcsCompatibilityCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetEcsCompatibilityCommand.UseXamlResource = true;
            SetEcsCompatibilityCommand.Weight = 0.1;

            SetIntellivisionIICompatibilityCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetIntellivisionIICompatibilityCommand.UseXamlResource = true;
            SetIntellivisionIICompatibilityCommand.Weight = 0.12;

            CommandList.Add(SetIntellivisionIICompatibilityCommand.CreateRibbonSeparator(CommandLocation.After));

            SetSaveMenuPositionCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetSaveMenuPositionCommand.UseXamlResource = true;
            SetSaveMenuPositionCommand.Weight = 0.13;

            SetKeyclicksCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetKeyclicksCommand.UseXamlResource = true;
            SetKeyclicksCommand.Weight = 0.14;

            SetBackgroundGarbageCollectCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetBackgroundGarbageCollectCommand.UseXamlResource = true;
            SetBackgroundGarbageCollectCommand.Weight = 0.15;

            CommandList.Add(SetBackgroundGarbageCollectCommand.CreateRibbonSeparator(CommandLocation.After));

            SetRandomizeJlpRamCommand.VisualParent = DeviceSettingsRibbonGroupCommand;
            SetRandomizeJlpRamCommand.UseXamlResource = true;
            SetRandomizeJlpRamCommand.Weight = 0.16;

            SetDeviceNameCommand.VisualParent = ActiveDeviceRibbonGroupCommand;
            SetDeviceNameCommand.UseXamlResource = true;
            SetDeviceNameCommand.Weight = 0;

            SetDeviceOwnerCommand.VisualParent = ActiveDeviceRibbonGroupCommand;
            SetDeviceOwnerCommand.UseXamlResource = true;
            SetDeviceOwnerCommand.Weight = 0.1;

            DeviceUniqueIdCommand.VisualParent = ActiveDeviceRibbonGroupCommand;
            DeviceUniqueIdCommand.UseXamlResource = true;
            DeviceUniqueIdCommand.Weight = 0.2;

            AdvancedGroupCommand.Weight = 0.15;

            CommandList.Add(ActiveDeviceRibbonGroupCommand);
            CommandList.Add(DeviceSettingsRibbonGroupCommand);
            CommandList.Add(SetShowTitleScreenCommand);
            CommandList.Add(SetSaveMenuPositionCommand);
            CommandList.Add(SetKeyclicksCommand);
            CommandList.Add(SetBackgroundGarbageCollectCommand);
            CommandList.Add(SetRandomizeJlpRamCommand);
            CommandList.Add(SetDeviceNameCommand);
            CommandList.Add(SetDeviceOwnerCommand);
            CommandList.Add(DeviceUniqueIdCommand);
            CommandList.Add(BackupRibbonSplitButtonCommand);
            CommandList.Add(ShowFileSystemDetailsCommand);
            CommandList.Add(DisconnectDeviceCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(AdvancedGroupCommand);
            CommandList.Add(OpenDeviceBackupsDirectoryCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(ClearCacheCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(AdvancedGroupCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
        }

        #endregion // CommandGroup
    }
}

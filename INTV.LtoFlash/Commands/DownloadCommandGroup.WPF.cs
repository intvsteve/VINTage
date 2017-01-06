// <copyright file="DownloadCommandGroup.WPF.cs" company="INTV Funhouse">
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

using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class DownloadCommandGroup
    {
        #region FileSystemRibbonGroupCommand

        /// <summary>
        /// Pseudo-command for the File System ribbon group.
        /// </summary>
        public static readonly VisualDeviceCommand FileSystemRibbonGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FileSystemRibbonGroupCommand",
            Name = Resources.Strings.FileSystemCommandGroup_Name,
            LargeIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/filesystem_32xLG.png"),
            Weight = 0,
            VisualParent = LtoFlashCommandGroup.LtoFlashRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // FileSystemRibbonGroupCommand

        #region SyncDeviceToHostRibbonButtonCommand

        /// <summary>
        /// The ribbon split button for device to host sync commands.
        /// </summary>
        public static readonly VisualDeviceCommand SyncDeviceToHostRibbonButtonCommand = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SyncDeviceToHostRibbonButtonCommand",
            Name = Resources.Strings.SyncDeviceToHostCommand_Name,
            MenuItemName = Resources.Strings.SyncDeviceToHostCommand_MenuItemName,
            ToolTip = Resources.Strings.SyncDeviceToHostCommand_Tip,
            ToolTipTitle = Resources.Strings.SyncDeviceToHostCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SyncDeviceToHostCommand_Tip + CommandProviderHelpers.RibbonSplitButtonExtraToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_from_device_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_from_device_16xLG.png"),
            Weight = 0.033,
            VisualParent = FileSystemRibbonGroupCommand,
            UseXamlResource = true
        };

        #endregion // SyncDeviceToHostRibbonButtonCommand

        #region CommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            DownloadAndPlayCommand.VisualParent = RomListCommandGroup.RunProgramRibbonCommand;
            DownloadAndPlayCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            DownloadAndPlayCommand.UseXamlResource = true;
            DownloadAndPlayPromptCommand.VisualParent = RomListCommandGroup.RunProgramRibbonCommand;
            DownloadAndPlayPromptCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            DownloadAndPlayPromptCommand.UseXamlResource = true;

            SyncHostToDevicePreviewCommand.VisualParent = MenuLayoutCommandGroup.SyncHostToDeviceRibbonButtonCommand;
            SyncHostToDeviceCommand.VisualParent = MenuLayoutCommandGroup.SyncHostToDeviceRibbonButtonCommand;
            SyncClearChangesPreviewCommand.VisualParent = MenuLayoutCommandGroup.SyncHostToDeviceRibbonButtonCommand;

            CommandList.Add(DownloadAndPlayPromptCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(FileSystemRibbonGroupCommand);
            CommandList.Add(SyncDeviceToHostRibbonButtonCommand);
        }

        #endregion // CommandGroup
    }
}

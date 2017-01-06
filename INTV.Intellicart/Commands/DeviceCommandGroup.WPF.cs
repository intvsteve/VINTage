// <copyright file="DeviceCommandGroup.WPF.cs" company="INTV Funhouse">
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

using System.Windows;
using System.Windows.Controls;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Intellicart.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class DeviceCommandGroup
    {
        #region IntellicartRibbonTabCommand

        /// <summary>
        /// Pseudo-command for the Intellicart! ribbon tab.
        /// </summary>
        public static readonly VisualRelayCommand IntellicartRibbonTabCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".IntellicartRibbonTabCommand",
            Name = Resources.Strings.Intellicart,
            VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand,
            Weight = 0.1,
            UseXamlResource = true
        };

        #endregion // IntellicartRibbonTabCommand

        #region DeviceRibbonGroupCommand

        /// <summary>
        /// Command to act as the ribbon group for specific device commands.
        /// </summary>
        public static readonly VisualRelayCommand DeviceRibbonGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DeviceRibbonGroupCommand",
            Name = Resources.Strings.DeviceGroupCommand_Name,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/intellicart_32xMD.png"),
            Weight = 0,
            VisualParent = IntellicartRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // DeviceRibbonGroupCommand

        #region SerialRibbonGroupCommand

        /// <summary>
        /// Command to act as the ribbon group for serial configuration commands.
        /// </summary>
        public static readonly VisualRelayCommand SerialRibbonGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SerialRibbonGroupCommand",
            Name = Resources.Strings.SettingsPage_SerialPortGroup_Name,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/port-icon_32x32.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/port-icon_16x16.png"),
            Weight = 0.1,
            VisualParent = IntellicartRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // SerialRibbonGroupCommand

        #region DownloadRibbonButtonCommand

        /// <summary>
        /// Command to load a ROM onto an Intellicart! to be inserted into the Play button in the Home tab.
        /// </summary>
        public static readonly VisualRelayCommand DownloadRibbonButtonCommand = new VisualRelayCommand(OnDownload, CanDownload)
        {
            UniqueId = UniqueNameBase + ".DownloadRibbonButtonCommand",
            Name = Resources.Strings.DownloadCommand_RibbonButtonName,
            MenuItemName = Resources.Strings.DownloadCommand_RibbonButtonName,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
            ToolTip = Resources.Strings.DownloadCommand_TipDescription,
            ToolTipTitle = Resources.Strings.DownloadCommand_TipTitle,
            ToolTipDescription = Resources.Strings.DownloadCommand_TipDescription + CommandProviderHelpers.RibbonSplitButtonExtraToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.2,
            VisualParent = RomListCommandGroup.RunProgramRibbonCommand,
            UseXamlResource = true
        };

        #endregion // DownloadRibbonButtonCommand

        #region BrowseAndDownloadRibbonButtonCommand

        /// <summary>
        /// The command to browse for a ROM, then load it for immediate execution on the Intellicart, inserted to Play button in the Home tab.
        /// </summary>
        public static readonly VisualRelayCommand BrowseAndDownloadRibbonButtonCommand = new VisualRelayCommand(BrowseAndDownload, CanBrowseAndDownload)
        {
            UniqueId = UniqueNameBase + ".BrowseAndDownloadRibbonButtonCommand",
            Name = Resources.Strings.BrowseAndDownloadCommand_Name,
            ToolTip = Resources.Strings.BrowseAndDownloadCommand_TipDescription,
            ToolTipTitle = Resources.Strings.BrowseAndDownloadCommand_Name,
            ToolTipDescription = Resources.Strings.BrowseAndDownloadCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/browse_download_play_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/browse_download_play_16xLG.png"),
            Weight = 0.21,
            VisualParent = RomListCommandGroup.RunProgramRibbonCommand,
            UseXamlResource = true
        };

        #endregion // BrowseAndDownloadRibbonButtonCommand

        #region DownloadRibbonSplitButtonCommand

        /// <summary>
        /// Command for the Play ribbon split button in the Intellicart's tab.
        /// </summary>
        public static readonly VisualRelayCommand DownloadRibbonSplitButtonCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DownloadRibbonSplitButtonCommand",
            Weight = 0,
            VisualParent = DeviceRibbonGroupCommand,
            UseXamlResource = true
        };

        #endregion // DownloadRibbonSplitButtonCommand

        #region CommandGroup

        /// <inheritdoc />
        /// <remarks>Excluding the Intellicart submenu. It's bulky and isn't necessary. The submenu items would also need to be
        /// broken out as separate menu items from the ones already elsewhere to get better cosmetics. (The current version uses
        /// the smaller icon and inflates to larger size, looking blocky / blurry.)</remarks>
        partial void AddPlatformCommands()
        {
            ////IntellicartToolsMenuCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            SetPortCommand.VisualParent = SerialRibbonGroupCommand;
            SetPortCommand.UseXamlResource = true;
            SetBaudRateCommand.VisualParent = SerialRibbonGroupCommand;
            SetBaudRateCommand.UseXamlResource = true;
            SetWriteTimeoutCommand.VisualParent = SerialRibbonGroupCommand;
            SetWriteTimeoutCommand.UseXamlResource = true;

            CommandList.Add(DownloadRibbonButtonCommand);
            CommandList.Add(BrowseAndDownloadRibbonButtonCommand);
            CommandList.Add(DownloadRibbonButtonCommand.CreateRibbonMenuSeparator(CommandLocation.Before, false));
            CommandList.Add(BrowseAndDownloadCommand);
            CommandList.Add(IntellicartRibbonTabCommand);
            CommandList.Add(DeviceRibbonGroupCommand);
            CommandList.Add(SerialRibbonGroupCommand);
            CommandList.Add(DownloadRibbonSplitButtonCommand);
            CommandList.Add(SetPortCommand);
            CommandList.Add(SetBaudRateCommand);
            CommandList.Add(SetWriteTimeoutCommand);
            ////CommandList.Add(IntellicartToolsMenuCommand.CreateRibbonMenuSeparator(CommandLocation.Before, true));
        }

        #endregion // CommandGroup
    }
}

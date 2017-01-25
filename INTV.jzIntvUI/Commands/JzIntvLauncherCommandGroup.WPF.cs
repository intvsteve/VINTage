// <copyright file="JzIntvLauncherCommandGroup.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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

namespace INTV.JzIntvUI.Commands
{
    /// <summary>
    /// WPF-specific commands.
    /// </summary>
    public partial class JzIntvLauncherCommandGroup
    {
        #region JzIntvRibbonTabCommand

        /// <summary>
        /// Pseudo-command for the jzIntv ribbon tab.
        /// </summary>
        public static readonly VisualRelayCommand JzIntvRibbonTabCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".JzIntvRibbonTabCommand",
            Name = Resources.Strings.JzIntv,
            Weight = 0.09,
            VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand,
            UseXamlResource = true
        };

        #endregion // JzIntvRibbonTabCommand

        #region LaunchRibbonGroupCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualRelayCommand LaunchRibbonGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".LaunchRibbonGroupCommand",
            Name = Resources.Strings.LaunchRibbonGroupCommand_Name,
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/jzIntvUI_32xMD.png"),
            VisualParent = JzIntvRibbonTabCommand,
            Weight = 0,
            UseXamlResource = true
        };

        #endregion // LaunchRibbonGroupCommand

        #region LaunchInJzIntvRibbonSplitButtonCommand

        /// <summary>
        /// Command for the Run in jzIntv ribbon split button in the jzIntv tab.
        /// </summary>
        public static readonly VisualRelayCommand LaunchInJzIntvRibbonSplitButtonCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".LaunchInJzIntvRibbonSplitButtonCommand",
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
            ToolTip = Resources.Strings.LaunchInJzIntvCommand_TipDescription,
            ToolTipTitle = Resources.Strings.LaunchInJzIntvCommand_TipTitle,
            ToolTipDescription = Resources.Strings.LaunchInJzIntvCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0,
            VisualParent = LaunchRibbonGroupCommand,
            UseXamlResource = true
        };

        #endregion // DownloadRibbonSplitButtonCommand

        #region DownloadRibbonButtonCommand

        /// <summary>
        /// Command to load a ROM onto an Intellicart! to be inserted into the Play button in the Home tab.
        /// </summary>
        public static readonly VisualRelayCommand LaunchInJzIntvRibbonButtonCommand = new VisualRelayCommand(OnLaunch, CanLaunch)
        {
            UniqueId = UniqueNameBase + ".LaunchInJzIntvRibbonButtonCommand",
            Name = Resources.Strings.LaunchInJzIntvCommand_MenuItemName,
            MenuItemName = Resources.Strings.LaunchInJzIntvCommand_MenuItemName,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
            ToolTip = Resources.Strings.LaunchInJzIntvCommand_TipDescription,
            ToolTipTitle = Resources.Strings.LaunchInJzIntvCommand_TipTitle,
            ToolTipDescription = Resources.Strings.LaunchInJzIntvCommand_TipDescription + CommandProviderHelpers.RibbonSplitButtonExtraToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.3,
            VisualParent = RomListCommandGroup.RunProgramRibbonCommand,
            UseXamlResource = true
        };

        #endregion // LaunchInJzIntvRibbonButtonCommand

        #region BrowseAndLaunchInJzIntvRibbonButtonCommand

        /// <summary>
        /// The command to browse for a ROM, then load it for immediate execution on the Intellicart, inserted to Play button in the Home tab.
        /// </summary>
        public static readonly VisualRelayCommand BrowseAndLaunchInJzIntvRibbonButtonCommand = new VisualRelayCommand(BrowseAndDownload, CanBrowseAndDownload)
        {
            UniqueId = UniqueNameBase + ".BrowseAndLaunchInJzIntvRibbonButtonCommand",
            Name = Resources.Strings.BrowseAndLaunchInJzIntvCommand_Name,
            ToolTip = Resources.Strings.BrowseAndLaunchInJzIntvCommand_TipDescription,
            ToolTipTitle = Resources.Strings.BrowseAndLaunchInJzIntvCommand_Name,
            ToolTipDescription = Resources.Strings.BrowseAndLaunchInJzIntvCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/browse_download_play_32xLG.png"),
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/browse_download_play_16xLG.png"),
            Weight = 0.31,
            VisualParent = RomListCommandGroup.RunProgramRibbonCommand,
            UseXamlResource = true
        };

        #endregion // BrowseAndLaunchInJzIntvRibbonButtonCommand

        #region CommandGroup

        /// <summary>
        /// WPF-specific command setup.
        /// </summary>
        partial void AddPlatformCommands()
        {
            CommandList.Add(JzIntvRibbonTabCommand);
            CommandList.Add(LaunchRibbonGroupCommand);
            CommandList.Add(LaunchInJzIntvRibbonSplitButtonCommand);
            CommandList.Add(LaunchInJzIntvRibbonButtonCommand.CreateRibbonMenuSeparator(CommandLocation.Before, false));
            CommandList.Add(LaunchInJzIntvRibbonButtonCommand);
            CommandList.Add(BrowseAndLaunchInJzIntvRibbonButtonCommand);
        }

        #endregion // CommandGroup
    }
}

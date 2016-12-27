// <copyright file="DownloadCommandGroup.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
#endif
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class DownloadCommandGroup
    {
        #region SyncToDeviceSubmenuCommand

        /// <summary>
        /// Submenu pseudo-command for sending menu and files to LTO Flash!.
        /// </summary>
        public static readonly VisualRelayCommand SyncToDeviceSubmenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SyncToDeviceSubmenuCommand",
            Name = Resources.Strings.SyncHostToDeviceCommand_SubmenuName,
            Weight = 0.03,
            MenuParent = RootCommandGroup.ToolsMenuCommand
        };

        #endregion // SyncToDeviceSubmenuCommand

        #region SyncFromDeviceSubmenuCommand

        /// <summary>
        /// Submenu pseudo-command for getting menu and files from LTO Flash!.
        /// </summary>
        public static readonly VisualRelayCommand SyncFromDeviceSubmenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SyncFromDeviceSubmenuCommand",
            Name = Resources.Strings.SyncDeviceToHostCommand_SubmenuName,
            Weight = 0.031,
            MenuParent = RootCommandGroup.ToolsMenuCommand
        };

        #endregion // SyncFromDeviceSubmenuCommand

        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return LtoFlashCommandGroup.Group.Context; }
        }

        #endregion // CommandGroup

        #region ICommandGroup

        partial void AddPlatformCommands()
        {
            DownloadAndPlayCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            DownloadAndPlayCommand.Weight = 0.02;

            DownloadAndPlayPromptCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            DownloadAndPlayPromptCommand.Weight = 0.03;

            CommandList.Add(DownloadAndPlayPromptCommand.CreateSeparator(CommandLocation.After));

            SyncHostToDeviceCommand.MenuParent = SyncToDeviceSubmenuCommand;
            SyncHostToDevicePreviewCommand.Weight = SyncHostToDeviceCommand.Weight;
            SyncHostToDevicePreviewCommand.MenuParent = SyncToDeviceSubmenuCommand;

            SyncDeviceToHostCommand.VisualParent = null; // do not put into toolbar
            SyncDeviceToHostCommand.MenuParent = SyncFromDeviceSubmenuCommand;
            SyncDeviceToHostPreviewCommand.Weight = SyncDeviceToHostCommand.Weight;
            SyncDeviceToHostPreviewCommand.MenuParent = SyncFromDeviceSubmenuCommand;

            SyncClearChangesPreviewCommand.Weight = 0.34;
            SyncClearChangesPreviewCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
        }

        #endregion // ICommandGroup
    }
}

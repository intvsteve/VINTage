// <copyright file="DownloadCommandGroup.Gtk.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// GTK-specific implementation.
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
                Weight = 0.2,
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
                Weight = 0.21,
                MenuParent = RootCommandGroup.ToolsMenuCommand
            };

        #endregion // SyncFromDeviceSubmenuCommand

        /// <inheritdoc/>
        public override object Context
        {
            get { return LtoFlashCommandGroup.Group.Context; }
        }

        #region CommandGroup

        /// <inheritdoc/>
        public override INTV.Shared.View.OSVisual CreateVisualForCommand(ICommand command)
        {
            var visualCommand = command as VisualRelayCommand;
            if ((visualCommand.UniqueId == DownloadAndPlayCommand.UniqueId) ||
                (visualCommand.UniqueId == DownloadAndPlayPromptCommand.UniqueId))
            {
                var parentCommand = visualCommand.VisualParent as VisualRelayCommand;
                var menuToolbarButton = parentCommand.Visual.AsType<Gtk.MenuToolButton>();
                if (menuToolbarButton.Menu == null)
                {
                    menuToolbarButton.Menu = new Gtk.Menu();
                }
                Gtk.MenuItem menuItem = visualCommand.CreateMenuItem(menuToolbarButton.Menu as Gtk.MenuShell, parentCommand, Gtk.StockItem.Zero);
                return menuItem;
            }
            return base.CreateVisualForCommand(command);
        }

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        partial void AddPlatformCommands()
        {
            CommandList.Add(SyncToDeviceSubmenuCommand);
            CommandList.Add(SyncFromDeviceSubmenuCommand);
            CommandList.Add(SyncToDeviceSubmenuCommand.CreateSeparator(CommandLocation.Before));

            SyncHostToDeviceCommand.Weight = 0.111;
            SyncHostToDeviceCommand.VisualParent = RootCommandGroup.RootCommand; // put into toolbar
            SyncHostToDeviceCommand.MenuParent = SyncToDeviceSubmenuCommand;
            SyncHostToDevicePreviewCommand.Weight = SyncHostToDeviceCommand.Weight;
            SyncHostToDevicePreviewCommand.MenuParent = SyncToDeviceSubmenuCommand;

            CommandList.Add(SyncHostToDeviceCommand.CreateToolbarSeparator(CommandLocation.Before));

            SyncDeviceToHostCommand.VisualParent = null; // do not put into toolbar
            SyncDeviceToHostCommand.MenuParent = SyncFromDeviceSubmenuCommand;
            SyncDeviceToHostPreviewCommand.Weight = SyncDeviceToHostCommand.Weight;
            SyncDeviceToHostPreviewCommand.MenuParent = SyncFromDeviceSubmenuCommand;

            DownloadAndPlayCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            DownloadAndPlayCommand.Weight = 0.02;
            DownloadAndPlayCommand.VisualParent = RomListCommandGroup.RunProgramToolbarCommand; // put into toolbar

            DownloadAndPlayPromptCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            DownloadAndPlayPromptCommand.Weight = 0.03;
            DownloadAndPlayPromptCommand.VisualParent = RomListCommandGroup.RunProgramToolbarCommand; // put into toolbar

            CommandList.Add(DownloadAndPlayPromptCommand.CreateSeparator(CommandLocation.After));

            SyncClearChangesPreviewCommand.Weight = 0.34;
            SyncClearChangesPreviewCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
        }

        #endregion // CommandGroup
    }
}

// <copyright file="DeviceCommandGroup.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.Intellicart.Commands
{
    /// <summary>
    /// Mac-specific implementation for device commands.
    /// </summary>
    public partial class DeviceCommandGroup
    {
        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return IntellicartViewModel; }
        }

        #endregion // CommandGroup

        partial void AddPlatformCommands()
        {
            IntellicartToolsMenuCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            var fileDownloadCommand = DownloadCommand.Clone();
            fileDownloadCommand.Weight = 0.04;
            fileDownloadCommand.MenuItemName = DownloadCommand.ContextMenuItemName;
            fileDownloadCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            var fileBrowseAndDownloadCommand = BrowseAndDownloadCommand.Clone();
            fileBrowseAndDownloadCommand.Weight = 0.05;
            fileBrowseAndDownloadCommand.MenuItemName = BrowseAndDownloadCommand.ContextMenuItemName;
            fileBrowseAndDownloadCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            CommandList.Add(IntellicartToolsMenuCommand.CreateSeparator(CommandLocation.Before));
            CommandList.Add(fileDownloadCommand);
            CommandList.Add(fileBrowseAndDownloadCommand);
            CommandList.Add(fileBrowseAndDownloadCommand.CreateSeparator(CommandLocation.After));
        }
    }
}

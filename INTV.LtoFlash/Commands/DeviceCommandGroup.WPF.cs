// <copyright file="DeviceCommandGroup.WPF.cs" company="INTV Funhouse">
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
            Weight = 0
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
            Weight = 0.1
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
        };

        #endregion // ShowFileSystemDetailsCommand
    }
}

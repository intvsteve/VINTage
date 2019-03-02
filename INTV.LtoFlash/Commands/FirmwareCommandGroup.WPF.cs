// <copyright file="FirmwareCommandGroup.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class FirmwareCommandGroup
    {
        #region FirmwareRibbonGroupCommand

        /// <summary>
        /// Pseudo-command for the Firmware grouping of commands in the ribbon.
        /// </summary>
        public static readonly VisualDeviceCommand FirmwareRibbonGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FirmwareRibbonGroupCommand",
            Name = Resources.Strings.Firmware_GroupHeader,
            LargeIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/firmware_32xLG.png"),
            SmallIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/firmware_16xLG.png"),
            Weight = 0.2,
            VisualParent = LtoFlashCommandGroup.LtoFlashRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // FirmwareRibbonGroupCommand

        #region CommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            FirmwareGroupCommand.MenuParent = LtoFlashCommandGroup.LtoFlashGroupCommand;
            FirmwareGroupCommand.Weight = 0.14;
            UpdateFirmwareCommand.Weight = 0.14;
            UpdateFirmwareCommand.VisualParent = FirmwareRibbonGroupCommand;
            UpdateFirmwareCommand.UseXamlResource = true;
            FactoryFirmwareCommand.VisualParent = FirmwareRibbonGroupCommand;
            FactoryFirmwareCommand.UseXamlResource = true;
            SecondaryFirmwareCommand.VisualParent = FirmwareRibbonGroupCommand;
            SecondaryFirmwareCommand.UseXamlResource = true;
            CurrentFirmwareCommand.VisualParent = FirmwareRibbonGroupCommand;
            CurrentFirmwareCommand.UseXamlResource = true;

            CommandList.Add(FirmwareGroupCommand);
            CommandList.Add(FirmwareRibbonGroupCommand);
            CommandList.Add(FactoryFirmwareCommand);
            CommandList.Add(SecondaryFirmwareCommand);
            CommandList.Add(CurrentFirmwareCommand);

            CommandList.Add(CheckForFirmwareUpdateCommand.CreateRibbonMenuSeparator(CommandLocation.Before, forAppMenu: true));
        }

        #endregion // CommandGroup
    }
}

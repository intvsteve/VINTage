// <copyright file="CommandHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

using System.Globalization;
using System.Linq;
using System.Text;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.Resources;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Commands
{
    public static class CommandHelpers
    {
        /// <summary>
        /// Helper method to check if a command can execute.
        /// </summary>
        /// <param name="ltoFlash">The top-level view model whose active device will be used to test command availability.</param>
        /// <param name="command">The command whose availability is being tested.</param>
        /// <returns><c>true</c> if the command can execute, otherwise <c>false</c>.</returns>
        internal static bool CanExecuteCommand(this LtoFlashViewModel ltoFlash, VisualDeviceCommand command)
        {
            bool canExecute = (ltoFlash != null) && (ltoFlash.ActiveLtoFlashDevice != null) && (ltoFlash.ActiveLtoFlashDevice.Device != null);
            if (canExecute)
            {
                canExecute = ltoFlash.ActiveLtoFlashDevice.Device.CanExecuteCommand(command);
            }
            return canExecute;
        }

        /// <summary>
        /// Helper method to check if a command can execute.
        /// </summary>
        /// <param name="device">The target device for the given command.</param>
        /// <param name="command">The command whose availability is being tested.</param>
        /// <returns><c>true</c> if the command can execute, otherwise <c>false</c>.</returns>
        internal static bool CanExecuteCommand(this Device device, VisualDeviceCommand command)
        {
            var canExecute = (device != null) && device.IsValid && !device.IsCommandInProgress && device.AllCommandsAvailable(command.RequiredProtocolCommands);
            if (canExecute && (command.ConfigurationBits != DeviceStatusFlags.None))
            {
                canExecute = command.ConfigurationBits.IsConfigurableFeatureAvailable(FirmwareRevisions.GetFirmwareVersion(device.FirmwareRevisions.Current));
            }
            command.UpdateDeviceConfigurationCommandTipDescription(device, canExecute);
            return canExecute;
        }

        /// <summary>
        /// Update the tool tip description of a command if feature cannot be configured due to firmware version of connected device.
        /// </summary>
        /// <param name="deviceCommand">The command whose tool tip description may be updated.</param>
        /// <param name="device">The currently active device, if any.</param>
        /// <param name="canExecute">The status of the 'can execute' of <paramref name="deviceCommand"/>.</param>
        internal static void UpdateDeviceConfigurationCommandTipDescription(this VisualDeviceCommand deviceCommand, Device device, bool canExecute)
        {
            if (deviceCommand.ConfigurationBits != DeviceStatusFlags.None)
            {
                var descriptionResourceKey = deviceCommand.UniqueId.Split(new[] { '.' }).Last() + "_TipDescription";
                var description = typeof(CommandHelpers).GetResourceString(descriptionResourceKey);
                if (!canExecute && (device != null))
                {
                    var requiredFirmareVersion = deviceCommand.ConfigurationBits.GetMinimumRequiredFirmareVersionForFeature();
                    var currentFirmwareVersion = FirmwareRevisions.GetFirmwareVersion(device.FirmwareRevisions.Current);
                    if ((currentFirmwareVersion > 0) && (requiredFirmareVersion > 0))
                    {
                        var modifedDescription = new StringBuilder(description).AppendLine().AppendLine();
                        modifedDescription.AppendFormat(
                            CultureInfo.CurrentCulture,
                            Strings.ConfigurableFeatureUnavailable_Message_Format,
                            requiredFirmareVersion,
                            currentFirmwareVersion);
                        description = modifedDescription.ToString();
                    }
                }
                deviceCommand.ToolTipDescription = description;
            }
        }
    }
}

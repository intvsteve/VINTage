// <copyright file="DeviceCommandGroup.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using INTV.Core.Model;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Device;
using INTV.Shared.Utility;
using INTV.Shared.View;

#if WIN
using OSWindow = System.Windows.Window;
#elif MAC || GTK
using OSWindow = INTV.Shared.View.IFakeDependencyObject;
#endif // WIN

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Encapsulates device-related commands.
    /// </summary>
    public partial class DeviceCommandGroup : CommandGroup
    {
        /// <summary>
        /// The command group.
        /// </summary>
        internal static readonly DeviceCommandGroup Group = new DeviceCommandGroup();

        private const string UniqueNameBase = "INTV.LtoFlash.Commands.DeviceCommandGroup";

        private DeviceCommandGroup()
            : base(UniqueNameBase, Resources.Strings.Toolbar_LtoFlash_Device_Header, 0.2)
        {
            TabName = Resources.Strings.LtoFlashCommandGroup_Name;
        }

        #region DeviceCommandGroupCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualDeviceCommand DeviceGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DeviceCommandGroup",
            Name = Resources.Strings.LtoFlashCommandGroup_Name,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_32xLG.png"),
            Weight = 0
        };

        #endregion // DeviceCommandGroupCommand

        #region SetEcsCompatibilityCommand

        /// <summary>
        /// The command to set ECS compatibility mode.
        /// </summary>
        public static readonly VisualDeviceCommand SetEcsCompatibilityCommand = new VisualDeviceCommand(RelayCommand.NoOp, CanSetEcsCompatibilityCommand)
        {
            UniqueId = UniqueNameBase + ".SetEcsCompatibilityCommand",
            Name = Resources.Strings.SetEcsCompatibilityCommand_Name,
            SmallIcon = typeof(INTV.Shared.Commands.CommandGroup).LoadImageResource("ViewModel/Resources/Images/ecs_16xLG.png"),
            ToolTipTitle = Resources.Strings.SetEcsCompatibilityCommand_Name,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            ToolTipDescription = Resources.Strings.SetEcsCompatibilityCommand_TipDescription,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        private static bool CanSetEcsCompatibilityCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            var toolTipTitle = Resources.Strings.SetEcsCompatibilityCommand_Name;
            var toolTipDescription = Resources.Strings.SetEcsCompatibilityCommand_TipDescription;
            if ((viewModel != null) && (viewModel.ActiveLtoFlashDevice != null) && (device != null) && device.IsValid)
            {
                var ecsCompatibility = viewModel.ActiveLtoFlashDevice.EcsCompatibility;
                toolTipTitle = string.Format(CultureInfo.CurrentCulture, Resources.Strings.EcsCompatibilityMode_ToolTipTitleFormat, ecsCompatibility.ToDisplayString());
                switch (ecsCompatibility)
                {
                    case EcsStatusFlags.None:
                        toolTipDescription = Resources.Strings.EcsCompatibilityMode_Enabled_ToolTipDescription;
                        break;
                    case EcsStatusFlags.EnabledForRequiredAndOptional:
                        toolTipDescription = Resources.Strings.EcsCompatibilityMode_Limited_ToolTipDescription;
                        break;
                    case EcsStatusFlags.EnabledForRequired:
                        toolTipDescription = Resources.Strings.EcsCompatibilityMode_Strict_ToolTipDescription;
                        break;
                    case EcsStatusFlags.Disabled:
                        toolTipDescription = Resources.Strings.EcsCompatibilityMode_Disabled_ToolTipDescription;
                        break;
                    default:
                        break;
                }
            }
            SetEcsCompatibilityCommand.ToolTipTitle = toolTipTitle;
            SetEcsCompatibilityCommand.ToolTipDescription = toolTipDescription;
            return device.CanExecuteCommand(SetEcsCompatibilityCommand);
        }

        /// <summary>
        /// Command for choosing to always leave the ECS fully enabled.
        /// </summary>
        public static readonly VisualDeviceCommand SetEcsCompatibilityChoiceEnabled = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEcsCompatibilityChoiceEnabled",
            Name = Resources.Strings.EcsCompatibilityMode_Enabled,
            ToolTip = Resources.Strings.EcsCompatibilityMode_Enabled_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to leave the ECS enabled only for games requiring it, or those that optionally support it.
        /// </summary>
        public static readonly VisualDeviceCommand SetEcsCompatibilityChoiceLimited = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEcsCompatibilityChoiceLimited",
            Name = Resources.Strings.EcsCompatibilityMode_Limited,
            ToolTip = Resources.Strings.EcsCompatibilityMode_Limited_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to enable the ECS only for games that require it.
        /// </summary>
        public static readonly VisualDeviceCommand SetEcsCompatibilityChoiceStrict = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEcsCompatibilityChoiceStrict",
            Name = Resources.Strings.EcsCompatibilityMode_Strict,
            ToolTip = Resources.Strings.EcsCompatibilityMode_Strict_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to disable ECS ROMs at all times.
        /// </summary>
        public static readonly VisualDeviceCommand SetEcsCompatibilityChoiceDisabled = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEcsCompatibilityChoiceDisabled",
            Name = Resources.Strings.EcsCompatibilityMode_Disabled,
            ToolTip = Resources.Strings.EcsCompatibilityMode_Disabled_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        #endregion // SetEcsCompatibilityCommand

        #region SetIntellivisionIICompatibilityCommand

        /// <summary>
        /// The command to set Intellivision II compatibility mode.
        /// </summary>
        public static readonly VisualDeviceCommand SetIntellivisionIICompatibilityCommand = new VisualDeviceCommand(OnSetIntellivisionIICompatibility, CanSetIntellivisionIICompatibility)
        {
            UniqueId = UniqueNameBase + ".SetIntellivisionIICompatibilityCommand",
            Name = Resources.Strings.SetIntellivisionIICompatibilityCommand_Name,
            SmallIcon = typeof(INTV.Shared.Commands.CommandGroup).LoadImageResource("ViewModel/Resources/Images/intv_ii_16xLG.png"),
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            ToolTipDescription = Resources.Strings.SetIntellivisionIICompatibilityCommand_TipDescription,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        // TODO Remove this, and have command use a No-Op. This is essentially a 'grouper' for the dropdown ring.
        private static void OnSetIntellivisionIICompatibility(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            System.Diagnostics.Debug.WriteLine("SetIntellivisionIICompatibilityCommand");
        }

        private static bool CanSetIntellivisionIICompatibility(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(SetIntellivisionIICompatibilityCommand);
        }

        /// <summary>
        /// Command for choosing to disable Intellivision II compatibility correction.
        /// </summary>
        public static readonly VisualDeviceCommand SetIntellivisionIICompatibilityChoiceDisabled = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetIntellivisionIICompatibilityChoiceDisabled",
            Name = Resources.Strings.IntellivisionIICompatibilityMode_Disabled,
            ToolTip = Resources.Strings.IntellivisionIICompatibilityMode_Disabled_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to enable Intellivision II compatibility correction only for games requiring it.
        /// </summary>
        public static readonly VisualDeviceCommand SetIntellivisionIICompatibilityChoiceLimited = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetIntellivisionIICompatibilityChoiceLimited",
            Name = Resources.Strings.IntellivisionIICompatibilityMode_Limited,
            ToolTip = Resources.Strings.IntellivisionIICompatibilityMode_Limited_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to always enable Intellivision II compatibility correction.
        /// </summary>
        public static readonly VisualDeviceCommand SetIntellivisionIICompatibilityChoiceFull = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetIntellivisionIICompatibilityChoiceFull",
            Name = Resources.Strings.IntellivisionIICompatibilityMode_Full,
            ToolTip = Resources.Strings.IntellivisionIICompatibilityMode_Full_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        #endregion // SetIntellivisionIICommand

        #region SetShowTitleScreenCommand

        /// <summary>
        /// The command to set whether or not to show the title screen.
        /// </summary>
        public static readonly VisualDeviceCommand SetShowTitleScreenCommand = new VisualDeviceCommand(OnSetShowTitleScreenCommand, CanSetShowTitleScreenCommand)
        {
            UniqueId = UniqueNameBase + ".SetShowTitleScreenCommand",
            Name = Resources.Strings.SetShowTitleScreenCommand_Name,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_title_16xLG.png"),
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            ToolTipDescription = Resources.Strings.SetShowTitleScreenCommand_TipDescription,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        // TODO Remove this, and have command use a No-Op. This is essentially a 'grouper' for the dropdown ring.
        private static void OnSetShowTitleScreenCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            System.Diagnostics.Debug.WriteLine("SetShowTitleScreenCommand");
        }

        private static bool CanSetShowTitleScreenCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (viewModel != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return (device != null) && device.CanExecuteCommand(SetShowTitleScreenCommand);
        }

        /// <summary>
        /// Command for choosing to always show the title screen.
        /// </summary>
        public static readonly VisualDeviceCommand SetShowTitleScreenCommandChoiceAlways = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetShowTitleScreenCommandChoiceAlways",
            Name = Resources.Strings.ShowTitleScreen_Always,
            ToolTip = Resources.Strings.ShowTitleScreen_Always_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to show the title screen at console power-up.
        /// </summary>
        public static readonly VisualDeviceCommand SetShowTitleScreenCommandChoiceOnPowerUp = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetShowTitleScreenCommandChoiceOnPowerUp",
            Name = Resources.Strings.ShowTitleScreen_OnPowerUp,
            ToolTip = Resources.Strings.ShowTitleScreen_OnPowerUp_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to never show the title screen.
        /// </summary>
        public static readonly VisualDeviceCommand SetShowTitleScreenCommandChoiceNever = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetShowTitleScreenCommandChoiceNever",
            Name = Resources.Strings.ShowTitleScreen_Never,
            ToolTip = Resources.Strings.ShowTitleScreen_Never_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        #endregion // SetShowTitleScreenCommand

        #region SetSaveMenuPositionCommand

        /// <summary>
        /// The command to set whether to retain last selected menu position.
        /// </summary>
        public static readonly VisualDeviceCommand SetSaveMenuPositionCommand = new VisualDeviceCommand(OnSetSaveMenuPositionCommand, CanSetSaveMenuPositionCommand)
        {
            UniqueId = UniqueNameBase + ".SetSaveMenuPositionCommand",
            Name = Resources.Strings.SetSaveMenuPositionCommand_Name,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/save_menu_position_16xLG.png"),
            ToolTipTitle = Resources.Strings.SetSaveMenuPositionCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetSaveMenuPositionCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        // TODO Remove this, and have command use a No-Op. This is essentially a 'grouper' for the dropdown ring.
        private static void OnSetSaveMenuPositionCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            System.Diagnostics.Debug.WriteLine("SetSaveMenuPositionCommand");
        }

        private static bool CanSetSaveMenuPositionCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(SetSaveMenuPositionCommand);
        }

        /// <summary>
        /// Command for choosing to always show the title screen.
        /// </summary>
        public static readonly VisualDeviceCommand SetSaveMenuPositionCommandChoiceAlways = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetSaveMenuPositionCommandChoiceAlways",
            Name = Resources.Strings.SaveMenuPosition_Always,
            ToolTip = Resources.Strings.SaveMenuPosition_Always_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to show the title screen at console power-up.
        /// </summary>
        public static readonly VisualDeviceCommand SetSaveMenuPositionCommandChoiceDuringSession = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetSaveMenuPositionCommandChoiceDuringSession",
            Name = Resources.Strings.SaveMenuPosition_SessionOnly,
            ToolTip = Resources.Strings.SaveMenuPosition_SessionOnly_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        /// <summary>
        /// Command for choosing to never show the title screen.
        /// </summary>
        public static readonly VisualDeviceCommand SetSaveMenuPositionCommandChoiceNever = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetSaveMenuPositionCommandChoiceNever",
            Name = Resources.Strings.SaveMenuPosition_Never,
            ToolTip = Resources.Strings.SaveMenuPosition_Never_ToolTipDescription,
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        #endregion // SetSaveMenuPositionCommand

        #region SetBackgroundGarbageCollectCommand

        /// <summary>
        /// The command to enable or disable whether garbage collection runs in the background when in menu mode.
        /// </summary>
        public static readonly VisualDeviceCommand SetBackgroundGarbageCollectCommand = new VisualDeviceCommand(OnSetBackgroundGarbageCollectCommand, CanSetBackgroundGarbageCollectCommand)
        {
            UniqueId = UniqueNameBase + ".SetBackgroundGarbageCollectCommand",
            Name = Resources.Strings.SetBackgroundGarbageCollectCommand_Name,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/background_gc_16xMD.png"),
            ToolTipTitle = Resources.Strings.SetBackgroundGarbageCollectCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetBackgroundGarbageCollectCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        // TODO Remove this, and have command use a No-Op.
        private static void OnSetBackgroundGarbageCollectCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            System.Diagnostics.Debug.WriteLine("SetBackgroundGarbageCollectCommand");
        }

        private static bool CanSetBackgroundGarbageCollectCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(SetBackgroundGarbageCollectCommand);
        }

        #endregion // SetBackgroundGarbageCollectCommand

        #region SetKeyclicksCommand

        /// <summary>
        /// The command to set whether key clicks are enabled.
        /// </summary>
        public static readonly VisualDeviceCommand SetKeyclicksCommand = new VisualDeviceCommand(OnSetKeyclicksCommand, CanSetKeyclicksCommand)
        {
            UniqueId = UniqueNameBase + ".SetKeyclicksCommand",
            Name = Resources.Strings.SetKeyclicksCommand_Name,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/keyclicks_16xLG.png"),
            ToolTipTitle = Resources.Strings.SetKeyclicksCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetKeyclicksCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands
        };

        // TODO Remove this, and have command use a No-Op.
        private static void OnSetKeyclicksCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            System.Diagnostics.Debug.WriteLine("SetKeyclicksCommand");
        }

        private static bool CanSetKeyclicksCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(SetKeyclicksCommand);
        }

        #endregion // SetKeyclicksCommand

        #region SetEnableConfigMenuOnCartCommand

        /// <summary>
        /// The command to set whether the on-cartridge configuration menu is accessible from the console.
        /// </summary>
        public static readonly VisualDeviceCommand SetEnableConfigMenuOnCartCommand = new VisualDeviceCommand(OnSetEnableConfigMenuOnCartCommand, CanSetEnableConfigMenuOnCartCommand)
        {
            UniqueId = UniqueNameBase + ".SetEnableConfigMenuOnCartCommand",
            Name = Resources.Strings.SetEnableConfigMenuOnCartCommand_Name,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/settings_16xLG.png"),
            ToolTipTitle = Resources.Strings.SetEnableConfigMenuOnCartCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetEnableConfigMenuOnCartCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands,
            ConfigurationBits = DeviceStatusFlags.EnableCartConfig
        };

        private static void OnSetEnableConfigMenuOnCartCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
        }

        private static bool CanSetEnableConfigMenuOnCartCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(SetEnableConfigMenuOnCartCommand);
        }

        #endregion // SetEnableConfigMenuOnCartCommand

        #region SetRandomizeLtoFlashRamCommand

        /// <summary>
        /// The command to set whether or not to randomize LTO Flash! RAM when launching a ROM.
        /// </summary>
        public static readonly VisualDeviceCommand SetRandomizeLtoFlashRamCommand = new VisualDeviceCommand(OnSetRandomizeLtoFlashRamCommand, CanSetRandomizeLtoFlashRamCommand)
        {
            UniqueId = UniqueNameBase + ".SetRandomizeLtoFlashRamCommand",
            Name = Resources.Strings.SetRandomizeLtoFlashRamCommand_Name,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/randomize_ram_16xLG.png"),
            ToolTipTitle = Resources.Strings.SetRandomizeLtoFlashRamCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetRandomizeLtoFlashRamCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SetConfigurationProtocolCommands,
            ConfigurationBits = DeviceStatusFlags.ZeroRamBeforeLoad
        };

        private static void OnSetRandomizeLtoFlashRamCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
        }

        private static bool CanSetRandomizeLtoFlashRamCommand(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(SetRandomizeLtoFlashRamCommand);
        }

        #endregion // SetRandomizeLtoFlashRamCommand

        #region SetDeviceOwnerCommand

        /// <summary>
        /// The command to set the name of the device owner.
        /// </summary>
        public static readonly VisualDeviceCommand SetDeviceOwnerCommand = new VisualDeviceCommand(OnSetDeviceOwner, CanSetDeviceOwner)
        {
            UniqueId = UniqueNameBase + ".SetDeviceOwnerCommand",
            Name = Resources.Strings.SetDeviceOwnerCommand_Name,
            ToolTipTitle = Resources.Strings.SetDeviceOwnerCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetDeviceOwnerCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.UpdateDeviceOwnerProtocolCommands
        };

        private static void OnSetDeviceOwner(object parameter)
        {
            // In Windows, due to the odd way in which this command can be invoked, it's possible
            // that this won't arise until we've already started the update, so bail if that's the case.
            var viewModel = parameter as LtoFlashViewModel;
            if (viewModel.ActiveLtoFlashDevice.IsValid && CanSetDeviceOwner(parameter))
            {
                // Don't bother if value stays the same.
                var currentDeviceOwner = viewModel.ActiveLtoFlashDevice.Device.Owner;
                var newDeviceOwner = viewModel.ActiveLtoFlashDevice.Owner;
            }
        }

        private static bool CanSetDeviceOwner(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var canExecute = (viewModel != null) && viewModel.CanExecuteCommand(SetDeviceOwnerCommand);
            return canExecute;
        }

        /// <summary>
        /// The error handler to use when setting the device owner.
        /// </summary>
        /// <returns><c>true</c> if the error should be considered handled. If <c>false</c>, application will exit.</returns>
        /// <param name="errorMessage">A brief error message to report to the user.</param>
        /// <param name="exception">If an exception occurred when setting the device owner, this will be non-<c>null</c>.</param>
        internal static bool SetDeviceOwnerCommandErrorHandler(string errorMessage, System.Exception exception)
        {
            OSMessageBox.Show(errorMessage, Resources.Strings.SetDeviceOwnerCommand_FailedTitle, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, OSMessageBoxButton.OK);
            CommandManager.InvalidateRequerySuggested();
            return true;
        }

        #endregion // SetDeviceOwnerCommand

        #region SetDeviceNameCommand

        /// <summary>
        /// The command to set the name of the device.
        /// </summary>
        public static readonly VisualDeviceCommand SetDeviceNameCommand = new VisualDeviceCommand(OnSetDeviceName, CanSetDeviceName)
        {
            UniqueId = UniqueNameBase + ".SetDeviceNameCommand",
            Name = Resources.Strings.SetDeviceNameCommand_Name,
            ToolTipTitle = Resources.Strings.SetDeviceNameCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SetDeviceNameCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.6,
            ////VisualParent = RootCommandGroup.RootCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.UpdateDeviceNameProtocolCommands
        };

        private static void OnSetDeviceName(object parameter)
        {
            // In Windows, due to the odd way in which this command can be invoked, it's possible
            // that this won't arise until we've already started the update, so bail if that's the case.
            var viewModel = parameter as LtoFlashViewModel;
            if (viewModel.ActiveLtoFlashDevice.IsValid && CanSetDeviceName(parameter))
            {
                // Don't bother if value stays the same.
                var currentDeviceName = viewModel.ActiveLtoFlashDevice.Device.CustomName;
                var newDeviceName = viewModel.ActiveLtoFlashDevice.Name;
            }
        }

        private static bool CanSetDeviceName(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var canExecute = viewModel.CanExecuteCommand(SetDeviceNameCommand);
            return canExecute;
        }

        /// <summary>
        /// The error handler to use when setting the device name.
        /// </summary>
        /// <returns><c>true</c> if the error should be considered handled. If <c>false</c>, application will exit.</returns>
        /// <param name="errorMessage">A brief error message to report to the user.</param>
        /// <param name="exception">If an exception occurred when setting the device name, this will be non-<c>null</c>.</param>
        internal static bool SetDeviceNameCommandErrorHandler(string errorMessage, System.Exception exception)
        {
            OSMessageBox.Show(errorMessage, Resources.Strings.SetDeviceNameCommand_FailedTitle, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, OSMessageBoxButton.OK);
            CommandManager.InvalidateRequerySuggested();
            return true;
        }

        #endregion // SetDeviceNameCommand

        #region DeviceUniqueIdCommand

        /// <summary>
        /// Pseudo-command to present the device's unique ID in the user interface.
        /// </summary>
        public static readonly VisualDeviceCommand DeviceUniqueIdCommand = new VisualDeviceCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DeviceUniqueIdCommand",
            Name = Resources.Strings.DeviceUniqueIdCommand_Name,
            ToolTipTitle = Resources.Strings.DeviceUniqueIdCommand_Name,
            ToolTipDescription = Resources.Strings.DeviceUniqueIdCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.7
        };

        #endregion // DeviceUniqueIdCommand

        #region AdvancedGroupCommand

        /// <summary>
        /// A grouper "command".
        /// </summary>
        public static readonly VisualDeviceCommand AdvancedGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".AdvancedGroupCommand",
            Name = Resources.Strings.AdvancedGroupCommand_Header,
            Weight = 0.11,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/settings_32xMD.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/settings_16xLG.png"),
            MenuParent = LtoFlashCommandGroup.LtoFlashGroupCommand
        };

        #endregion // AdvancedGroupCommand

        #region BackupCommand

        /// <summary>
        /// The command to initiate a backup of the contents of an LTO Flash!
        /// </summary>
        public static readonly VisualDeviceCommand BackupCommand = new VisualDeviceCommand(OnBackup, CanBackup)
        {
            UniqueId = UniqueNameBase + ".BackupCommand",
            Name = Resources.Strings.BackupCommand_Name,
            MenuItemName = Resources.Strings.BackupCommand_MenuItemName,
            ToolTip = Resources.Strings.BackupCommand_TipDescription,
            ToolTipTitle = Resources.Strings.BackupCommand_TipTitle,
            ToolTipDescription = Resources.Strings.BackupCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/backup_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/backup_16xLG.png"),
            Weight = 0.01,
            MenuParent = AdvancedGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.BackupFileSystemProtocolCommands
        };

        private static void OnBackup(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            var configuration = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
            var backupDirectory = configuration.GetUniqueBackupDataDataFilePath(device.UniqueId);
            device.BackupFileSystem(backupDirectory, (c, p, r) => BackupCommandCompleteHandler(c, backupDirectory, (FileSystemSyncErrors)r), (m, e) => BackupCommandErrorHandler(backupDirectory, m, e));
        }

        private static bool CanBackup(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(BackupCommand);
        }

        private static void BackupCommandCompleteHandler(bool cancelled, string backupDirectory, FileSystemSyncErrors backupErrors)
        {
            var application = SingleInstanceApplication.Instance;
            if ((application != null) && (application.MainWindow != null))
            {
                if (!backupErrors.Any)
                {
                    var message = cancelled ? Resources.Strings.BackupCommandComplete_Cancelled_Message : Resources.Strings.BackupCommandComplete_Message;
                    var reportCompleteDialog = INTV.Shared.View.ReportDialog.Create(cancelled ? Resources.Strings.BackupCommandCancelled_Title : Resources.Strings.BackupCommandComplete_Title, message);
                    reportCompleteDialog.ReportText = backupDirectory;
                    reportCompleteDialog.TextWrapping = OSTextWrapping.Wrap;
                    reportCompleteDialog.ShowSendEmailButton = false;
                    reportCompleteDialog.Attachments.Add(backupDirectory);
                    reportCompleteDialog.BeginInvokeDialog(Resources.Strings.OK, null);
                }
                else
                {
                    var errorStringBuilder = new System.Text.StringBuilder(Resources.Strings.BackupCommandComplete_EncounteredErrors);
                    if (backupErrors.InitialDirtyFlags != LfsDirtyFlags.None)
                    {
                        errorStringBuilder.AppendLine().Append("  ").Append(Resources.Strings.SyncErrors_DirtyFileSystemMessage);
                    }
                    if (backupErrors.OrphanedForks.Any())
                    {
                        errorStringBuilder.AppendLine().Append("  ").AppendFormat(Resources.Strings.SyncErrors_OrphanedForksFormat, backupErrors.OrphanedForks.Count);
                    }
                    if (backupErrors.UnsupportedForks.Any())
                    {
                        errorStringBuilder.AppendLine().Append("  ").AppendFormat(Resources.Strings.SyncErrors_UnsupportedForksFormat, backupErrors.UnsupportedForks.Count);
                        errorStringBuilder.AppendLine().AppendLine().AppendLine(Resources.Strings.SyncErrors_UnsupportedForksInfo);
                    }
                    errorStringBuilder.AppendLine().AppendLine(Resources.Strings.BackupCommandComplete_DireWarning);
                    backupErrors.RecordErrors(backupDirectory, "BACKUP");
                    PromptToDeleteBackup(errorStringBuilder.ToString(), Resources.Strings.BackupCommandComplete_ErrorsOccurredTitle, backupDirectory);
                }
            }
        }

        private static bool BackupCommandErrorHandler(string backupDirectory, string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.BackupCommand_Failed_Message_Format, errorMessage);
            if (exception != null)
            {
                string fileSystemErrorMessageFormat = null;
                var exceptionType = exception.GetType();
                if (exceptionType == typeof(System.UnauthorizedAccessException))
                {
                    fileSystemErrorMessageFormat = Resources.Strings.BackupCommand_UnauthorizedAccessErrorMessageFormat;
                }
                else if (exceptionType == typeof(System.IO.IOException))
                {
                    fileSystemErrorMessageFormat = Resources.Strings.BackupCommandErrorHandler_IOExceptionErrorMessageFormat;
                }
                else if (exceptionType == typeof(System.IO.PathTooLongException))
                {
                    fileSystemErrorMessageFormat = Resources.Strings.BackupCommandErrorHandler_PathTooLongErrorMessageFormat;
                }
                else if (exceptionType == typeof(System.IO.DirectoryNotFoundException))
                {
                    fileSystemErrorMessageFormat = Resources.Strings.BackupCommandErrorHandler_DirectoryNotFoundErrorMessageFormat;
                }
                else if (exceptionType == typeof(System.ArgumentException))
                {
                    fileSystemErrorMessageFormat = Resources.Strings.BackupCommandErrorHandler_ArgumentErrorMessageFormat;
                }
                var dialog = ReportDialog.Create(Resources.Strings.BackupCommand_Failed_Title, Resources.Strings.BackupCommand_ExceptionMessage);
                if (!string.IsNullOrEmpty(fileSystemErrorMessageFormat))
                {
                    dialog.ReportText = string.Format(CultureInfo.CurrentCulture, fileSystemErrorMessageFormat, errorMessage);
                }
                else
                {
                    dialog.ReportText = errorMessage;
                }
                if (SingleInstanceApplication.SharedSettings.ShowDetailedErrors)
                {
                    dialog.Exception = exception;
                }
                else
                {
                    dialog.TextWrapping = OSTextWrapping.Wrap;
                }
                dialog.ShowDialog(Resources.Strings.OK);
            }
            PromptToDeleteBackup(message, Resources.Strings.BackupCommand_Failed_Title, backupDirectory);
            return true;
        }

        private static void PromptToDeleteBackup(string message, string title, string backupDirectory)
        {
            var result = OSMessageBox.Show(message, title, null, OSMessageBoxButton.YesNo, OSMessageBoxIcon.Information);
            if ((result == OSMessageBoxResult.No) && System.IO.Directory.Exists(backupDirectory))
            {
                result = OSMessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.BackupCommand_Failed_Delete_Message_Format, backupDirectory), Resources.Strings.BackupCommand_Failed_Delete_Title, OSMessageBoxButton.YesNo, OSMessageBoxIcon.Question);
                if (result == OSMessageBoxResult.Yes)
                {
                    try
                    {
                        FileUtilities.DeleteDirectory(backupDirectory, true, 4);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        // didn't actually get far enough to create the directory yet
                    }
                }
            }
        }

        #endregion // BackupCommand

        #region RestoreCommand

        /// <summary>
        /// The command to initiate a restore of the contents of an LTO Flash! from a previously performed backup.
        /// </summary>
        public static readonly VisualDeviceCommand RestoreCommand = new VisualDeviceCommand(OnRestore, CanRestore)
        {
            UniqueId = UniqueNameBase + ".RestoreCommand",
            Name = Resources.Strings.RestoreCommand_Name,
            MenuItemName = Resources.Strings.RestoreCommand_MenuItemName,
            ToolTip = Resources.Strings.RestoreCommand_Tip,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/restore_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/restore_16xLG.png"),
            Weight = 0.011,
            MenuParent = AdvancedGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.RestoreFileSystemProtocolCommands
        };

        private static void OnRestore(object parameter)
        {
            var message = Resources.Strings.RestoreCommand_WarningMessage;
            var result = OSMessageBox.Show(message, Resources.Strings.RestoreCommand_Dialog_Title, OSMessageBoxButton.YesNo);
            if (result == OSMessageBoxResult.Yes)
            {
                var ltoFlashViewModel = parameter as LtoFlashViewModel;
                var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                var configuration = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
                var backupDirectory = configuration.GetDeviceBackupDataAreaPath(device.UniqueId);
                var backupFileName = INTV.LtoFlash.Model.Configuration.Instance.DefaultMenuLayoutFileName;
                var luigiExtension = RomFormat.Luigi.FileExtension();
                var selectBackupDialog = SelectBackupDialog.Create(backupDirectory, backupFileName, new[] { luigiExtension }, true);
                selectBackupDialog.Title = Resources.Strings.RestoreMenuLayoutCommand_SelectBackupTitle;
                var doRestoreResult = selectBackupDialog.ShowDialog();
                if (doRestoreResult.HasValue && doRestoreResult.Value)
                {
                    backupDirectory = selectBackupDialog.SelectedBackupDirectory;
                    if (System.IO.Directory.Exists(backupDirectory))
                    {
                        device.RestoreFileSystem(backupDirectory, (c, p, r) => RestoreCommandCompleteHandler(device, c, backupDirectory, (Tuple<FileSystem, IDictionary<string, FailedOperationException>>)r), (m, e) => RestoreCommandErrorHandler(backupDirectory, m, e));
                    }
                    else
                    {
                        message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.RestoreCommand_BackupNotFound_ErrorFormat, backupDirectory);
                        OSMessageBox.Show(message, Resources.Strings.RestoreCommand_BackupNotFound_ErrorTitle);
                    }
                }
            }
        }

        private static bool CanRestore(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            var device = (parameter != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            var canRestore = device.CanExecuteCommand(RestoreCommand);
            if (canRestore)
            {
                var configuration = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
                var backupDirectory = configuration.GetDeviceBackupDataAreaPath(device.UniqueId);
                canRestore = !string.IsNullOrWhiteSpace(backupDirectory) && System.IO.Directory.Exists(backupDirectory);
            }
            return canRestore;
        }

        private static void RestoreCommandCompleteHandler(Device device, bool cancelled, string backupDirectory, Tuple<FileSystem, IDictionary<string, FailedOperationException>> result)
        {
            var updatedDeviceFileSystem = result.Item1;
            var message = cancelled ? Resources.Strings.RestoreCommand_CancelledMessage : Resources.Strings.RestoreCommand_CompleteMessage;
            var title = cancelled ? Resources.Strings.RestoreCommand_CancelledTitle : Resources.Strings.RestoreCommand_CompleteTitle;
            var warnings = result.Item2;
            if (warnings.Any())
            {
                var reportDialog = ReportDialog.Create(title, message);
                reportDialog.ShowSendEmailButton = false;
                var warningsBuilder = new System.Text.StringBuilder(Resources.Strings.RestoreMenuLayoutCommand_WarningsHeader).AppendLine().AppendLine();
                foreach (var warning in warnings)
                {
                    warningsBuilder.AppendFormat(Resources.Strings.RestoreMenuLayoutCommand_WarningMessageDetailFormat, warning.Key, warning.Value.Message).AppendLine().AppendLine();
                }
                warningsBuilder.AppendLine(Resources.Strings.RestoreMenuLayoutCommand_WarningDetailHeader).AppendLine("------------------------------------------------");
                foreach (var exception in warnings.Values)
                {
                    warningsBuilder.AppendLine(exception.ToString()).AppendLine();
                }
                reportDialog.ReportText = warningsBuilder.ToString();
                reportDialog.ShowDialog(Resources.Strings.OK);
            }
            else
            {
                OSMessageBox.Show(message, title);
            }
            CommandManager.InvalidateRequerySuggested();
            device.FileSystem = updatedDeviceFileSystem;
        }

        private static bool RestoreCommandErrorHandler(string backupDirectory, string errorMessage, System.Exception exception)
        {
            OSMessageBox.Show(errorMessage, Resources.Strings.RestoreCommand_FailedTitle, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, OSMessageBoxButton.OK);
            CommandManager.InvalidateRequerySuggested();
            return true;
        }

        #endregion // RestoreCommand

        #region OpenDeviceBackupsDirectoryCommand

        /// <summary>
        /// Command to open device backup data directory.
        /// </summary>
        public static readonly VisualRelayCommand OpenDeviceBackupsDirectoryCommand = new VisualRelayCommand(OnOpenDeviceBackupsDirectory)
        {
            UniqueId = UniqueNameBase + ".OpenDeviceBackupsDirectoryCommand",
            Name = Resources.Strings.OpenDeviceBackupsDirectoryCommand_Name,
            ToolTipDescription = Resources.Strings.OpenDeviceBackupsDirectoryCommand_TipDescription,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/folder_closed_32xMD.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/folder_closed_16xMD.png"),
            Weight = 0.0111,
            MenuParent = AdvancedGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnOpenDeviceBackupsDirectory(object parameter)
        {
            string backupDir = null;
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            if (ltoFlashViewModel != null)
            {
                var configuration = INTV.LtoFlash.Model.Configuration.Instance;
                if (ltoFlashViewModel.ActiveLtoFlashDevice.IsValid)
                {
                    backupDir = configuration.GetDeviceBackupDataAreaPath(ltoFlashViewModel.ActiveLtoFlashDevice.Device.UniqueId);
                }
                else
                {
                    backupDir = configuration.ApplicationConfigurationPath;
                }
            }
            if (!string.IsNullOrEmpty(backupDir) && System.IO.Directory.Exists(backupDir))
            {
                try
                {
                    RunExternalProgram.OpenInDefaultProgram(backupDir);
                }
                catch (InvalidOperationException)
                {
                    // Silently fail.
                }
            }
        }

        #endregion // OpenDeviceBackupsDirectoryCommand

        #region OpenErrorLogsDirectoryCommand

        /// <summary>
        /// Command to open the error log directory.
        /// </summary>
        public static readonly VisualRelayCommand OpenErrorLogsDirectoryCommand = new VisualRelayCommand(OnOpenErrorLogsDirectory)
        {
            UniqueId = UniqueNameBase + ".OpenErrorLogsDirectoryCommand",
            Name = Resources.Strings.OpenErrorLogsDirectoryCommand_Name,
            ToolTipDescription = Resources.Strings.OpenErrorLogsDirectoryCommand_TipDescription,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/folder_closed_32xMD.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/folder_closed_16xMD.png"),
            Weight = 0.0115,
            MenuParent = AdvancedGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnOpenErrorLogsDirectory(object parameter)
        {
            var romsConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.Shared.Model.RomListConfiguration>();
            if (System.IO.Directory.Exists(romsConfiguration.ErrorLogDirectory))
            {
                try
                {
                    RunExternalProgram.OpenInDefaultProgram(romsConfiguration.ErrorLogDirectory);
                }
                catch (InvalidOperationException)
                {
                    // Silently fail.
                }
            }
        }

        #endregion // OpenErrorLogsDirectoryCommand

        #region ClearCacheCommand

        /// <summary>
        /// Command to clear the ROMsCache directory.
        /// </summary>
        public static readonly VisualDeviceCommand ClearCacheCommand = new VisualDeviceCommand(OnClearCache)
        {
            UniqueId = UniqueNameBase + ".ClearCacheCommand",
            Name = Resources.Strings.ClearCacheCommand_Name,
            MenuItemName = Resources.Strings.ClearCacheCommand_Name,
            ToolTip = Resources.Strings.ClearCacheCommand_Tip,
            Weight = 0.012,
            MenuParent = AdvancedGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
        };

        private static void OnClearCache(object parameter)
        {
            var clearCacheTask = new AsyncTaskWithProgress("ClearCache", false, true);
            clearCacheTask.UpdateTaskTitle(Resources.Strings.ClearCacheCommand_Title);
            var clearCacheTaskData = new ClearCacheTaskData(clearCacheTask);
            clearCacheTask.RunTask(clearCacheTaskData, ClearCache, ClearCacheComplete);
        }

        private static bool CanClearCache(object parameter)
        {
            var romsStagingArea = Configuration.Instance.RomsStagingAreaPath;
            return System.IO.Directory.Exists(romsStagingArea);
        }

        /// <summary>
        /// Clears the ROMs cache.
        /// </summary>
        /// <param name="taskData">Used to indicate process. May be <c>null</c> if a progress update is not desired.</param>
        internal static void ClearCache(AsyncTaskData taskData)
        {
            var romsStagingArea = Configuration.Instance.RomsStagingAreaPath;
            if (System.IO.Directory.Exists(romsStagingArea))
            {
                foreach (var entry in System.IO.Directory.EnumerateFileSystemEntries(romsStagingArea))
                {
                    if (taskData != null)
                    {
                        taskData.UpdateTaskProgress(0, string.Format(CultureInfo.CurrentCulture, Resources.Strings.ClearCacheCommand_ProgressFormat, entry));
                    }
                    if (System.IO.Directory.Exists(entry))
                    {
                        FileUtilities.DeleteDirectory(entry, false, 2);
                    }
                    else
                    {
                        FileUtilities.DeleteFile(entry, false, 2);
                    }
                }
            }
            CacheIndex.Instance.Entries.Clear();
        }

        private static void ClearCacheComplete(AsyncTaskData taskData)
        {
            var dialogMessage = Resources.Strings.ClearCacheCommand_CompleteMessage;
            if (taskData.Error != null)
            {
                if (taskData.Error is System.IO.DirectoryNotFoundException)
                {
                    dialogMessage = Resources.Strings.ClearCacheCommand_DirectoryNotFoundMessage;
                }
                else
                {
                    dialogMessage = taskData.Error.Message;
                }
            }
            OSMessageBox.Show(dialogMessage, Resources.Strings.ClearCacheCommand_Name);
        }

        private class ClearCacheTaskData : AsyncTaskData
        {
            public ClearCacheTaskData(AsyncTaskWithProgress task)
                : base(task)
            {
            }
        }

        #endregion // ClearCacheCommand

        #region ReformatCommand

        /// <summary>
        /// The command to reformat the file system on a Locutus device.
        /// </summary>
        public static readonly VisualDeviceCommand ReformatCommand = new VisualDeviceCommand(OnReformat, CanReformat)
        {
            UniqueId = UniqueNameBase + ".ReformatCommand",
            Name = Resources.Strings.ReformatCommand_Name,
            MenuItemName = Resources.Strings.ReformatCommand_MenuItemName,
            ToolTip = Resources.Strings.ReformatCommand_Tip,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/reformat_32x.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/reformat_16x.png"),
            Weight = 0.02,
            MenuParent = AdvancedGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.ReformatFileSystemProtocolCommands
        };

        private static void OnReformat(object ltoFlashViewModel)
        {
            var warningDialogResult = OSMessageBox.Show(
                Resources.Strings.Reformat_Message,
                Resources.Strings.Reformat_Title,
                OSMessageBoxButton.YesNo,
                OSMessageBoxIcon.Exclamation,
                OSMessageBoxResult.No);
            if (warningDialogResult == OSMessageBoxResult.Yes)
            {
                // Actually reformat the device!
                var viewModel = ltoFlashViewModel as LtoFlashViewModel;
                var device = viewModel.ActiveLtoFlashDevice.Device;
                device.ReformatFileSystem(ReformatCommandFailed, () => ReformatCommandSucceeded(viewModel));
            }
        }

        private static bool CanReformat(object ltoFlashViewModel)
        {
            var viewModel = ltoFlashViewModel as LtoFlashViewModel;
            var device = (ltoFlashViewModel != null) ? viewModel.ActiveLtoFlashDevice.Device : null;
            return device.CanExecuteCommand(ReformatCommand);
        }

        private static bool ReformatCommandFailed(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ReformatCommand_Failed_Message_Format, errorMessage);
            OSMessageBox.Show(message, Resources.Strings.ReformatCommand_Failed_Title, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, (r) => { });
            return true;
        }

        private static void ReformatCommandSucceeded(LtoFlashViewModel viewModel)
        {
            OSMessageBox.Show(Resources.Strings.Reformat_SucceededMessage, Resources.Strings.Reformat_Title);

            // TODO: enable the 'Sync Host to Device' command if there's anything in the menu.
        }

        #endregion // ReformatCommand

        #region SearchForDevicesCommand

        /// <summary>
        /// Command to search for attached Locutus devices.
        /// </summary>
        public static readonly VisualDeviceCommand SearchForDevicesCommand = new VisualDeviceCommand(OnSearchForDevices, CanSearchForDevices)
        {
            UniqueId = UniqueNameBase + ".SearchForDevicesCommand",
            Name = Resources.Strings.SearchForDevicesCommand_Name,
            MenuItemName = Resources.Strings.SearchForDevicesCommand_MenuItemName,
            ToolTip = Resources.Strings.SearchForDevicesCommand_TipDescription,
            ToolTipTitle = Resources.Strings.SearchForDevicesCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.SearchForDevicesCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_search_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_search_16xLG.png"),
            KeyboardShortcutKey = "f",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnSearchForDevices(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var knownPorts = ltoFlashViewModel.Devices.Where(d => d.IsValid && (d.Device.Port is SerialPortConnection)).Select(d => d.Device.Port.Name);
            var checkForDevices = new CheckForDevicesTaskData(INTV.LtoFlash.Properties.Settings.Default.LastActiveDevicePort, ltoFlashViewModel, knownPorts, false);
            checkForDevices.ReportNoneFound = true;
            checkForDevices.Start();
        }

        private static bool CanSearchForDevices(object parameter)
        {
            return true; // It's always OK to ask.
        }

        #endregion // SearchForDevicesCommand

        #region DisconnectDeviceCommand

        /// <summary>
        /// Command to disconnect the active Locutus device.
        /// </summary>
        /// <remarks>This will need to be tweaked if / when multiple device support is added.</remarks>
        public static readonly VisualDeviceCommand DisconnectDeviceCommand = new VisualDeviceCommand(OnDisconnectDeviceCommand, CanDisconnectDeviceCommand)
        {
            UniqueId = UniqueNameBase + ".DisconnectDeviceCommand",
            Name = Resources.Strings.DisconnectDeviceCommand_Name,
            MenuItemName = Resources.Strings.DisconnectDeviceCommand_MenuItemName,
            ToolTip = Resources.Strings.DisconnectDeviceCommand_TipDescription,
            ToolTipTitle = Resources.Strings.DisconnectDeviceCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.DisconnectDeviceCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_disconnect_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/lto_flash_disconnect_16xLG.png"),
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnDisconnectDeviceCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            ltoFlashViewModel.ActiveLtoFlashDevice.Device.Disconnect(false);
        }

        private static bool CanDisconnectDeviceCommand(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return (ltoFlashViewModel != null) && (ltoFlashViewModel.ActiveLtoFlashDevice != null) && ltoFlashViewModel.ActiveLtoFlashDevice.IsValid;
        }

        #endregion // DisconnectDeviceCommand

        #region SetActiveDeviceCommand

        /// <summary>
        /// Command to set the active device.
        /// </summary>
        /// <remarks>This is really only partially working -- won't do anything if we already have an active device.</remarks>
        public static readonly VisualDeviceCommand SetActiveDeviceCommand = new VisualDeviceCommand(OnSetActiveDeviceCommand, CanSetActiveDeviceCommand)
        {
            UniqueId = UniqueNameBase + ".SetActiveDeviceCommand",
            Name = Resources.Strings.SetActiveDeviceCommand_Name,
            PreferredParameterType = typeof(DeviceConnectionViewModel)
        };

        private static void OnSetActiveDeviceCommand(object parameter)
        {
            // Does this hide a deeper problem?
            if (CanSetActiveDeviceCommand(parameter))
            {
                var window = parameter as OSWindow;
                if (window != null)
                {
#if WIN
                    var viewModel = window.DataContext as DeviceSelectionDialogViewModel;
                    var result = viewModel.SelectedDevice != null;
                    window.DialogResult = result;
#else
                    throw new System.NotImplementedException();
#endif // WIN
                }
                else
                {
                    var deviceConnectionViewModel = parameter as DeviceConnectionViewModel;
                    if (deviceConnectionViewModel != DeviceConnectionViewModel.NoneAvailable)
                    {
                        var creationInfo = new Dictionary<string, object>() { { DeviceCreationInfo.ConfigName, new DeviceCreationInfo(true, true, ActivationMode.ForceActivate) } };
                        INTV.Shared.Interop.DeviceManagement.DeviceChange.ReportDeviceAdded(parameter, deviceConnectionViewModel.Name, Core.Model.Device.ConnectionType.Serial, creationInfo);
                    }
                }
            }
        }

        private static bool CanSetActiveDeviceCommand(object parameter)
        {
            var window = parameter as OSWindow;
            var viewModel = (window != null) ? window.DataContext as DeviceSelectionDialogViewModel : null;
            var canSetActiveDevice = (viewModel != null) && (viewModel.SelectedDevice != null) && viewModel.AvailableDevicePorts.Any();
            if (!canSetActiveDevice && (viewModel == null))
            {
                var deviceConnectionViewModel = parameter as DeviceConnectionViewModel;
                canSetActiveDevice = (deviceConnectionViewModel != null) && (deviceConnectionViewModel != DeviceConnectionViewModel.NoneAvailable);
                if (canSetActiveDevice)
                {
                    var activeDevice = deviceConnectionViewModel.LtoFlash.ActiveLtoFlashDevice.Device;
                    canSetActiveDevice = (activeDevice == null) || (activeDevice.Port == null) || (activeDevice.Port.Name != deviceConnectionViewModel.Name);
                }
            }
            return canSetActiveDevice;
        }

        #endregion // SetActiveDeviceCommand

        #region CommandGroup

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(SearchForDevicesCommand);
            CommandList.Add(DisconnectDeviceCommand);
            CommandList.Add(SetEcsCompatibilityCommand);
            CommandList.Add(SetIntellivisionIICompatibilityCommand);
            CommandList.Add(BackupCommand);
            CommandList.Add(RestoreCommand);
            CommandList.Add(OpenDeviceBackupsDirectoryCommand);
            CommandList.Add(OpenErrorLogsDirectoryCommand);
            CommandList.Add(ClearCacheCommand);
            CommandList.Add(ReformatCommand);
            AddPlatformCommands();
        }

        /// <summary>
        /// Perform platform-specific adjustments to commands or add any platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}

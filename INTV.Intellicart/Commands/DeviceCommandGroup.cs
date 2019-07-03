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
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Intellicart.Model;
using INTV.Intellicart.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Intellicart.Commands
{
    /// <summary>
    /// Commands for using an Intellicart.
    /// </summary>
    public partial class DeviceCommandGroup : CommandGroup
    {
        private const string UniqueNameBase = "INTV.Intellicart.Commands.DeviceCommandGroup";

        private static IntellicartViewModel IntellicartViewModel
        {
            get
            {
                if (_intellicartViewModel == null)
                {
                    _intellicartViewModel = new IntellicartViewModel();
                }
                return _intellicartViewModel;
            }
        }
        private static IntellicartViewModel _intellicartViewModel;

        /// <summary>
        /// The instance of the command group.
        /// </summary>
        internal static readonly DeviceCommandGroup Group = new DeviceCommandGroup();

        /// <summary>
        /// Gets or sets a value indicating whether the serial port selection dialog is shown.
        /// </summary>
        internal bool SelectPortFromPreferences { get; set; }

        private DeviceCommandGroup()
            : base(UniqueNameBase, "Intellicart!")
        {
        }

        #region DeviceGroupCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualRelayCommand DeviceGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DeviceGroupCommand",
            Name = Resources.Strings.DeviceGroupCommand_Name,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/intellicart_32xMD.png"),
            Weight = 0
        };

        #endregion // DeviceGroupCommand

        #region IntellicartToolsMenuCommand

        /// <summary>
        /// Grouping command for Intellicart-related commands.
        /// </summary>
        public static readonly VisualRelayCommand IntellicartToolsMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".IntellicartToolsMenuCommand",
            Name = Resources.Strings.Intellicart,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/intellicart_32xMD.png"),
            Weight = 0.85,
        };

        #endregion // IntellicartToolsMenuCommand

        #region SerialCommandGroupCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualRelayCommand SerialCommandGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SerialCommandGroupCommand",
            Name = Resources.Strings.SettingsPage_SerialPortGroup_Name,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/port-icon_32x32.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/port-icon_16x16.png"),
            Weight = 0
        };

        #endregion // SerialCommandGroupCommand

        #region SelectPortCommand

        /// <summary>
        /// Command to show a dialog prompt to select the serial port to use to communicate with an Intellicart.
        /// </summary>
        public static readonly VisualRelayCommand SelectPortCommand = new VisualRelayCommand(OnSelectPort, CanSelectPort)
        {
            UniqueId = UniqueNameBase + ".SelectPortCommand",
            Name = Resources.Strings.SelectPortCommand_Name,
            ToolTip = Resources.Strings.SetPortCommand_TipDescription,
            ToolTipTitle = Resources.Strings.SelectPortCommand_Name,
            ToolTipDescription = Resources.Strings.SetPortCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.1,
            MenuParent = IntellicartToolsMenuCommand,
            PreferredParameterType = typeof(IntellicartViewModel)
        };

        private static void OnSelectPort(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            var ports = intellicart.SerialPorts.Select(p => p.PortName).ToList();
            var currentPort = intellicart.SerialPort;
            var disabledPorts = new List<string>();

            if (!string.IsNullOrEmpty(currentPort))
            {
                if (!ports.Contains(currentPort))
                {
                    ports.Add(currentPort);
                    disabledPorts.Add(currentPort);
                }
            }

            var defaultBaudRate = intellicart.BaudRate;
            if (defaultBaudRate <= 0)
            {
                defaultBaudRate = IntellicartModel.DefaultBaudRate;
            }

            var dialog = INTV.Shared.View.SerialPortSelectorDialog.Create(null, Resources.Strings.SelectSerialPortDialog_Message, ports, disabledPorts, currentPort, Model.IntellicartModel.BaudRates, defaultBaudRate, true, IntellicartViewModel.IsNotExclusivePort);
            var setPort = dialog.ShowDialog(!DeviceCommandGroup.Group.SelectPortFromPreferences);

            if (setPort.HasValue && setPort.Value)
            {
                var selectedPort = dialog.SelectedPort;
                var selectedBaudRate = dialog.SelectedBaudRate;

                intellicart.SerialPort = selectedPort;
                intellicart.BaudRate = selectedBaudRate;
            }
        }

        private static bool CanSelectPort(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            return intellicart != null;
        }

        #endregion // SelectPortCommand

        #region SetPortCommand

        /// <summary>
        /// Command to set the serial port on an Intellicart.
        /// </summary>
        /// <remarks>At this point, this command is not executed, but only used for resources.</remarks>
        public static readonly VisualRelayCommand SetPortCommand = new VisualRelayCommand(RelayCommand.NoOp, CanSetPort)
        {
            UniqueId = UniqueNameBase + ".SetPortCommand",
            Name = Resources.Strings.SetSerialPortCommand_Name,
            MenuItemName = Resources.Strings.SetSerialPortCommand_Name,
            ToolTip = Resources.Strings.SetPortCommand_TipDescription,
            ToolTipTitle = Resources.Strings.SetSerialPortCommand_Name,
            ToolTipDescription = Resources.Strings.SetPortCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            SmallIcon = typeof(CommandGroup).LoadImageResource("ViewModel/Resources/Images/serialport_16x16.png"),
        };

        private static bool CanSetPort(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            return intellicart != null;
        }

        #endregion // SetPortCommand

        #region SetBaudRateCommand

        /// <summary>
        /// Command to set the serial port baud rate on an Intellicart.
        /// </summary>
        /// <remarks>At this point, this command is not executed, but only used for resources.</remarks>
        public static readonly VisualRelayCommand SetBaudRateCommand = new VisualRelayCommand(RelayCommand.NoOp, CanSetBaudRate)
        {
            UniqueId = UniqueNameBase + ".SetBaudRateCommand",
            Name = Resources.Strings.SetBaudRateCommand_Name,
            MenuItemName = Resources.Strings.SetBaudRateCommand_Name,
            ToolTip = Resources.Strings.SetBaudRateCommand_Name,
            ToolTipTitle = Resources.Strings.SetBaudRateCommand_Name,
            ToolTipDescription = Resources.Strings.SetBaudRateCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            SmallIcon = typeof(CommandGroup).LoadImageResource("ViewModel/Resources/Images/baudrate_16xLG.png"),
        };

        private static bool CanSetBaudRate(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            return intellicart != null;
        }

        #endregion SetBaudRateCommand

        #region SetWriteTimeoutCommand

        /// <summary>
        /// Command to set the serial port write timeout on an Intellicart.
        /// </summary>
        /// <remarks>At this point, this command is not executed, but only used for resources.</remarks>
        public static readonly VisualRelayCommand SetWriteTimeoutCommand = new VisualRelayCommand(OnSetWriteTimeout, CanSetWriteTimeout)
        {
            UniqueId = UniqueNameBase + ".SetWriteTimeoutCommand",
            Name = Resources.Strings.SetWriteTimeoutCommand,
            MenuItemName = Resources.Strings.SetWriteTimeoutCommand,
            ToolTip = Resources.Strings.SetWriteTimeoutCommand_TipDescription,
            ToolTipTitle = Resources.Strings.SetWriteTimeoutCommand,
            ToolTipDescription = Resources.Strings.SetWriteTimeoutCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            SmallIcon = typeof(CommandGroup).LoadImageResource("ViewModel/Resources/Images/timeout_16xLG.png"),
        };

        private static void OnSetWriteTimeout(object parameter)
        {
            ////var intellicart = parameter as IntellicartViewModel;
        }

        private static bool CanSetWriteTimeout(object parameter)
        {
            ////var intellicart = parameter as IntellicartViewModel;
            return true;
        }

        #endregion // SetWriteTimeoutCommand

        #region DownloadCommand

        /// <summary>
        /// Command to load a ROM onto an Intellicart.
        /// </summary>
        public static readonly VisualRelayCommand DownloadCommand = new VisualRelayCommand(OnDownload, CanDownload)
        {
            UniqueId = UniqueNameBase + ".DownloadCommand",
            Name = Resources.Strings.DownloadCommand_Name,
            ContextMenuItemName = Resources.Strings.DownloadCommand_ContextMenuItemName,
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
            ToolTip = Resources.Strings.DownloadCommand_TipDescription,
            ToolTipTitle = Resources.Strings.DownloadCommand_TipTitle,
            ToolTipDescription = Resources.Strings.DownloadCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.2,
            MenuParent = IntellicartToolsMenuCommand,
            KeyboardShortcutKey = "i",
            KeyboardShortcutModifiers = OSModifierKeys.Menu | OSModifierKeys.Alt,
        };

        private static void OnDownload(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            var roms = intellicart.Roms;
            var program = roms.SelectionIndexes.Any() ? roms[roms.SelectionIndexes.First()] : null;
            DownloadRom(intellicart, program);
        }

        private static void DownloadRom(IntellicartViewModel intellicart, IProgramDescription program)
        {
            if (program != null)
            {
                var isCompatible = intellicart.Model.IsRomCompatible(program);
                if (!isCompatible)
                {
                    var message = string.Format(Resources.Strings.DownloadCommand_IncompatibleRom_Message_Format, program.Name);
                    isCompatible = OSMessageBox.Show(message, Resources.Strings.DownloadCommand_IncompatibleRom_Title, OSMessageBoxButton.YesNo) == OSMessageBoxResult.Yes;
                }
                if (isCompatible)
                {
                    if ((program != null) && (program.Rom != null) && !program.Rom.Validate())
                    {
                        OSMessageBox.Show(string.Format(Resources.Strings.DownloadCommand_Missing_Message_Format, program.Name, program.Rom.RomPath), string.Format(Resources.Strings.DownloadCommand_Failed_Title_Format, program.Name));
                    }
                    else
                    {
                        intellicart.Model.DownloadRom(program.Name, program.Rom, OnDownloadError);
                    }
                }
            }
        }

        private static bool CanDownload(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            var canExecute = (intellicart != null) && intellicart.IsConfiguredPortAvailable && intellicart.Roms.SelectionIndexes.Any();
            return canExecute;
        }

        private static void OnDownloadError(string message, Exception exception)
        {
            OSMessageBox.Show(message, Resources.Strings.DownloadCommandErrorMessage_Title, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, null);
        }

        #endregion // DownloadCommand

        #region BrowseAndDownloadCommand

        /// <summary>
        /// The command to browse for a ROM, then load it for immediate execution on the Intellicart.
        /// </summary>
        public static readonly VisualRelayCommand BrowseAndDownloadCommand = new VisualRelayCommand(BrowseAndDownload, CanBrowseAndDownload)
        {
            UniqueId = UniqueNameBase + ".BrowseAndDownloadCommand",
            Name = Resources.Strings.BrowseAndDownloadCommand_Name,
            ToolTip = Resources.Strings.BrowseAndDownloadCommand_TipDescription,
            ToolTipTitle = Resources.Strings.BrowseAndDownloadCommand_Name,
            ToolTipDescription = Resources.Strings.BrowseAndDownloadCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/browse_download_play_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/browse_download_play_16xLG.png"),
            Weight = 0.21,
            MenuParent = IntellicartToolsMenuCommand,
            KeyboardShortcutKey = "I",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
        };

        private static void BrowseAndDownload(object parameter)
        {
            if (CanBrowseAndDownload(parameter))
            {
                var intellicart = parameter as IntellicartViewModel;
                var selectedFile = INTV.Shared.Model.IRomHelpers.BrowseForRoms(false, Resources.Strings.BrowseAndDownloadCommand_BrowseDialogPrompt).FirstOrDefault();
                if (selectedFile != null)
                {
                    var selectedLocation = new StorageLocation(selectedFile);
                    var rom = selectedLocation.GetRomFromPath();
                    IProgramDescription programDescription = null;
                    if (rom != null)
                    {
                        var programInfo = rom.GetProgramInformation();
                        programDescription = new ProgramDescription(rom.Crc, rom, programInfo);
                    }
                    if (programDescription != null)
                    {
                        DownloadRom(intellicart, programDescription);
                    }
                    else
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.BrowseAndDownloadCommand_Failed_MessageFormat, selectedFile);
                        OSMessageBox.Show(message, string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.BrowseAndDownloadCommand_Failed_Title));
                    }
                }
            }
        }

        private static bool CanBrowseAndDownload(object parameter)
        {
            var intellicart = parameter as IntellicartViewModel;
            var canExecute = (intellicart != null) && intellicart.IsConfiguredPortAvailable;
            return canExecute;
        }

        #endregion // BrowseAndDownloadCommand

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            // A NULL target is allowed for the case of an empty list.
            if (((target is INTV.Shared.ViewModel.ProgramDescriptionViewModel) || (target == null)) && (context is INTV.Shared.ViewModel.RomListViewModel))
            {
                yield return CreateContextMenuCommand(null, DownloadCommand, IntellicartViewModel, null, 0.022);
            }
        }

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(IntellicartToolsMenuCommand);
            CommandList.Add(SelectPortCommand);
            CommandList.Add(DownloadCommand);
            CommandList.Add(BrowseAndDownloadCommand);
            AddPlatformCommands();
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}

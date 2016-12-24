// <copyright file="LtoFlashCommandGroup.cs" company="INTV Funhouse">
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

using System;
using INTV.LtoFlash.View;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// LTO Flash! command group.
    /// </summary>
    public partial class LtoFlashCommandGroup : INTV.Shared.Commands.CommandGroup
    {
        /// <summary>
        /// The command group.
        /// </summary>
        internal static readonly LtoFlashCommandGroup Group = new LtoFlashCommandGroup();

        private const string UniqueNameBase = "INTV.LtoFlash.Commands.LtoFlashCommandGroup";

        private LtoFlashCommandGroup()
            : base(UniqueNameBase, Resources.Strings.LtoFlashCommandGroup_Name, 0.1)
        {
        }

        #region LtoFlashGroupCommand

        /// <summary>
        /// Group command for LTO Flash!
        /// </summary>
        public static readonly VisualDeviceCommand LtoFlashGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".LtoFlashGroupCommand",
            Name = Resources.Strings.LtoFlashCommandGroup_Name,
            LargeIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/lto_flash_32xLG.png"),
            SmallIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/lto_flash_16xLG.png"),
            Weight = 0.1,
        };

        #endregion // LtoFlashGroupCommand

        #region FtdiGroupCommand

        /// <summary>
        /// Group command for LTO Flash!
        /// </summary>
        public static readonly VisualDeviceCommand FtdiGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FtdiGroupCommand",
            Name = Resources.Strings.FtdiGroupCommand_Name,
            MenuItemName = Resources.Strings.FtdiGroupCommand_MenuItemName,
            LargeIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_32xLG.png"),
            SmallIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_16xLG.png"),
            Weight = 0.9,
            MenuParent = LtoFlashGroupCommand
        };

        #endregion // FtdiGroupCommand

        #region GoToFTDIDriverPageCommand

        /// <summary>
        /// Command to navigate to the FTDI Virtual COM Port drivers page.
        /// </summary>
        public static readonly VisualDeviceCommand GoToFTDIDriverPageCommand = new VisualDeviceCommand(OnGoToFTDIDriverPage)
        {
            UniqueId = UniqueNameBase + ".GoToFTDIDriverPageCommand",
            Name = Resources.Strings.GoToFTDIDriverPageCommand_Name,
            ToolTip = Resources.Strings.GoToFTDIDriverPageCommand_TipDescription,
            LargeIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_32xLG.png"),
            SmallIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_16xLG.png"),
            Weight = 0.51,
            MenuParent = FtdiGroupCommand
        };

        private static void OnGoToFTDIDriverPage(object parameter)
        {
            System.Diagnostics.Process.Start("http://www.ftdichip.com/Drivers/VCP.htm");
        }

        #endregion // GoToFTDIDriverPageCommand

        #region LastKnownFTDIDriverVersionCommand

        /// <summary>
        /// Command to display the last known FTDI VCP driver version.
        /// </summary>
        public static readonly VisualDeviceCommand FtdiDriverVersionCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp, CheckFtdiDriverVersionCommand)
        {
            UniqueId = UniqueNameBase + ".FtdiDriverVersionCommand",
            Name = Resources.Strings.FtdiDriverVersionCommand_Name,
            MenuItemName = Resources.Strings.FtdiDriverVersionUnknown,
            ToolTip = Resources.Strings.FtdiDriverVersionUnknown,
            ToolTipTitle = Resources.Strings.FtdiDriverVersionCommand_TipTitle,
            ToolTipDescription = Resources.Strings.FtdiDriverVersionCommand_TipDescription,
            ToolTipIcon = INTV.Shared.ComponentModel.VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_32xLG.png"),
            SmallIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_16xLG.png"),
            Weight = 0.5,
            MenuParent = FtdiGroupCommand
        };

        private static bool CheckFtdiDriverVersionCommand(object parameter)
        {
            if (string.IsNullOrEmpty(FtdiDriverVersionCommand.MenuItemName) || FtdiDriverVersionCommand.MenuItemName.Equals(Resources.Strings.FtdiDriverVersionUnknown))
            {
                UpdateDriverVersionCommandStrings(INTV.LtoFlash.Utility.FTDIUtilities.DriverVersionString);
            }
            return false;
        }

        /// <summary>
        /// Updates the driver version strings.
        /// </summary>
        /// <param name="version">The FTDI driver version.</param>
        internal static void UpdateDriverVersionCommandStrings(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                version = Resources.Strings.FtdiDriverVersionUnknown;
            }
            FtdiDriverVersionCommand.ToolTip = version;
            FtdiDriverVersionCommand.MenuItemName = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FtdiDriverVersionCommand_Format, version);
        }

        #endregion // LastKnownFTDIDriverVersionCommand

        #region LaunchFtdiDriverInstallerCommand

        /// <summary>
        /// Command to launch the FTDI driver installer.
        /// </summary>
        public static readonly VisualDeviceCommand LaunchFtdiDriverInstallerCommand = new VisualDeviceCommand(OnLaunchFtdiDriverInstaller, CanLaunchFtdiDriverInstaller)
        {
            UniqueId = UniqueNameBase + ".LaunchFtdiDriverInstallerCommand",
            Name = Resources.Strings.LaunchFtdiDriverInstallerCommand_Name,
            ToolTip = Resources.Strings.LaunchFtdiDriverInstallerCommand_TipDescription,
            LargeIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_32xLG.png"),
            SmallIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/ftdi_16xLG.png"),
            Weight = 0.51,
            MenuParent = FtdiGroupCommand
        };

        private static void OnLaunchFtdiDriverInstaller(object parameter)
        {
            var installDriver = false;
            var driverInstallMessage = parameter as string;
            if (driverInstallMessage == null)
            {
                driverInstallMessage = Resources.Strings.LaunchFtdiDriverInstallerCommand_PromptMessage;
                var installDriverResult = INTV.Shared.View.OSMessageBox.Show(driverInstallMessage, Resources.Strings.LaunchFtdiDriverInstallerCommand_PromptTitle, Shared.View.OSMessageBoxButton.YesNo);
                installDriver = installDriverResult == Shared.View.OSMessageBoxResult.Yes;
            }
            else
            {
                var promptToInstallDialog = PromptToInstallFtdiDriverDialog.Create();
                installDriver = promptToInstallDialog.ShowDialog(true) == true;
            }
            if (installDriver)
            {
                try
                {
                    LaunchFtdiDriverInstaller();
                }
                catch (Exception)
                {
                    driverInstallMessage = "Unable to launch FTDI VCP driver installer. Download and install the driver directly from FTDI.";
                    INTV.Shared.View.OSMessageBox.Show(driverInstallMessage, Resources.Strings.LaunchFtdiDriverInstallerCommand_PromptTitle);
                }
            }
        }

        private static bool CanLaunchFtdiDriverInstaller(object parameter)
        {
            return System.IO.File.Exists(GetFtdiDriverInstallerPath());
        }

        #endregion // LaunchFtdiDriverInstallerCommand

        /// <summary>
        /// Gets the actual value to use as context for commands.
        /// </summary>
        internal static LtoFlashViewModel LtoFlashViewModel
        {
            get
            {
                if ((_ltoFlashViewModel == null) && (SingleInstanceApplication.Instance != null))
                {
                    _ltoFlashViewModel = INTV.Shared.ComponentModel.CompositionHelpers.Container.GetExportedValueOrDefault<LtoFlashViewModel>();
                }
                return _ltoFlashViewModel;
            }
        }
        private static LtoFlashViewModel _ltoFlashViewModel;

        #region CommandGroup

        /// <inheritdoc />
        protected override void AddCommands()
        {
            UpdateDriverVersionCommandStrings(INTV.LtoFlash.Utility.FTDIUtilities.DriverVersionString);
            CommandList.Add(FtdiGroupCommand);
            CommandList.Add(FtdiDriverVersionCommand);
            CommandList.Add(GoToFTDIDriverPageCommand);
            ////CommandList.Add(LaunchFtdiDriverInstallerCommand); // Not worth it. Lots of problems on Mac (signing) and probably old.
            AddPlatformCommands();
        }

        #endregion // CommandGroup

        /// <summary>
        /// Do any platform-specific command initialization.
        /// </summary>
        partial void AddPlatformCommands();
    }
}

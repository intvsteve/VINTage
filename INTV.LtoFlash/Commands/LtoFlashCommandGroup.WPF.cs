// <copyright file="LtoFlashCommandGroup.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
    public partial class LtoFlashCommandGroup
    {
        #region LtoFlashRibbonTabCommand

        /// <summary>
        /// Pseudo-command for the LTO Flash! ribbon tab.
        /// </summary>
        public static readonly VisualDeviceCommand LtoFlashRibbonTabCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".LtoFlashRibbonTabCommand",
            Name = Resources.Strings.LtoFlashCommandGroup_Name,
            Weight = 0.1,
            VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand,
            UseXamlResource = true
        };

        #endregion // LtoFlashRibbonTabCommand

        #region LtoFlashHomeRibbonGroupCommand

        /// <summary>
        /// Group command for LTO Flash!
        /// </summary>
        public static readonly VisualDeviceCommand LtoFlashHomeRibbonGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".LtoFlashHomeRibbonGroupCommand",
            Name = Resources.Strings.LtoFlashCommandGroup_Name,
            LargeIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/lto_flash_32xLG.png"),
            SmallIcon = typeof(LtoFlashCommandsProvider).LoadImageResource("Resources/Images/lto_flash_16xLG.png"),
            Weight = 0.3,
            VisualParent = RootCommandGroup.HomeRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // LtoFlashHomeRibbonGroupCommand

        private static void LaunchFtdiDriverInstaller()
        {
            var ftdiDriverInstallerPath = GetFtdiDriverInstallerPath();
            using (var installer = INTV.Shared.Utility.RunExternalProgram.Launch(ftdiDriverInstallerPath, null, System.IO.Path.GetDirectoryName(ftdiDriverInstallerPath), false, false, true))
            {
                installer.Exited += (s, e) => UpdateDriverVersionCommandStrings(INTV.LtoFlash.Utility.FTDIUtilities.DriverVersionString);
            }
        }

        private static string GetFtdiDriverInstallerPath()
        {
            var ftdiDriverInstallerDir = INTV.LtoFlash.Model.Configuration.Instance.RedistributablesPath;
            var ftdiDriverInstallerPath = System.IO.Path.Combine(ftdiDriverInstallerDir, "CDM v2.12.06 WHQL Certified.exe");
            return ftdiDriverInstallerPath;
        }

        #region CommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            LtoFlashGroupCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            LtoFlashGroupCommand.Weight = 0.102;
            CommandList.Add(LtoFlashGroupCommand);
            CommandList.Add(LtoFlashRibbonTabCommand);
            CommandList.Add(LtoFlashHomeRibbonGroupCommand);
        }

        #endregion // CommandGroup
    }
}

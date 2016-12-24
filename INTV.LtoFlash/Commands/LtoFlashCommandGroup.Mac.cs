// <copyright file="LtoFlashCommandGroup.Mac.cs" company="INTV Funhouse">
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

using System;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class LtoFlashCommandGroup
    {
        #region LaunchFtdiDriverInstallerCommand

        private static void LaunchFtdiDriverInstaller()
        {
            var ftdiDriverInstallerPath = GetFtdiDriverInstallerPath();
            if (INTV.Shared.Utility.OSVersion.Current.Minor < INTV.LtoFlash.Utility.FTDIUtilities.UseNewDriverOSMinorVersion)
            {
                ftdiDriverInstallerPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(ftdiDriverInstallerPath));
            }
            try
            {
                INTV.Shared.Utility.RunExternalProgram.OpenInDefaultProgram(ftdiDriverInstallerPath);
            }
            catch (InvalidOperationException)
            {
                // Silently fail.
            }
        }

        private static string GetFtdiDriverInstallerPath()
        {
            var installerFileName = "FTDIUSBSerialDriver.pkg";
            if (INTV.Shared.Utility.OSVersion.Current.Minor < INTV.LtoFlash.Utility.FTDIUtilities.UseNewDriverOSMinorVersion)
            {
                installerFileName = "FTDIUSBSerialDriver10_8.mpkg/Contents/distribution.dist";
            }
            var ftdiDriverInstallerDir = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "../Resources", INTV.LtoFlash.Model.Configuration.RedistributablesDirectoryName);
            var ftdiDriverInstallerPath = System.IO.Path.Combine(ftdiDriverInstallerDir, installerFileName);
            return ftdiDriverInstallerPath;
        }

        #endregion // LaunchFtdiDriverInstallerCommand

        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return LtoFlashViewModel; }
        }

        #endregion // CommandGroup

        #region ICommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            LtoFlashGroupCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            LtoFlashGroupCommand.Weight = 0.8;
            CommandList.Add(LtoFlashGroupCommand.CreateSeparator(CommandLocation.Before));

#if DEBUG
            LtoFlashViewModel.InjectFirmwareCrashCommand.MenuParent = DebugCommandGroup.DebugMenuCommand;
            CommandList.Add(LtoFlashViewModel.InjectFirmwareCrashCommand);
            LtoFlashViewModel.InjectCommandFailureCommand.MenuParent = DebugCommandGroup.DebugMenuCommand;
            CommandList.Add(LtoFlashViewModel.InjectCommandFailureCommand);
            LtoFlashViewModel.InjectCommandNakCommand.MenuParent = DebugCommandGroup.DebugMenuCommand;
            CommandList.Add(LtoFlashViewModel.InjectCommandNakCommand);
            LtoFlashViewModel.WaitForBeaconCommand.MenuParent = DebugCommandGroup.DebugMenuCommand;
            CommandList.Add(LtoFlashViewModel.WaitForBeaconCommand);
#endif
        }

        #endregion // ICommandGroup
    }
}

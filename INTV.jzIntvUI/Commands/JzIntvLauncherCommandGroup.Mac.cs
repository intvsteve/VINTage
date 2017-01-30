// <copyright file="JzIntvLauncherCommandGroup.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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
using System.IO;
using System.Linq;
using System.Xml;
using INTV.JzIntv.Model;
using INTV.Shared.Commands;
using INTV.Shared.Utility;
using INTV.Shared.View;

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif

namespace INTV.JzIntvUI.Commands
{
    public partial class JzIntvLauncherCommandGroup
    {
        #region LaunchInJzIntvCommand

        private const int SDLNotLoadedError = 133;

        static partial void OSErrorHandler(Emulator emulator, string message, int exitCode, Exception exception)
        {
            if (exitCode == SDLNotLoadedError)
            {
                if (OSMessageBox.Show(Resources.Strings.SDLNotFound_ErrorMessage, Resources.Strings.SDLNotFound_ErrorTitle, OSMessageBoxButton.YesNo) == OSMessageBoxResult.Yes)
                {
                    var installSDLTask = new AsyncTaskWithProgress("InstallSDL", false, true, 0);
                    installSDLTask.UpdateTaskTitle(Resources.Strings.SDLInstall_Title);
                    var installSDLTaskData = new InstallSDLTaskData(installSDLTask);
                    installSDLTask.RunTask(installSDLTaskData, InstallSDL, InstallSDLComplete);
                }
            }
        }

        private static void InstallSDL(AsyncTaskData taskData)
        {
            var data = (InstallSDLTaskData)taskData;
            data.UpdateTaskProgress(0, Resources.Strings.SDLInstall_Update_Mounting);
            var baseResourcePath = System.AppDomain.CurrentDomain.BaseDirectory;
            var sdlDmgPath = Path.Combine(baseResourcePath, "jzIntv/Mac/bin/SDL-1.2.15.dmg");
            string sdlDev = string.Empty;
            try
            {
                var result = RunExternalProgram.CallAndReturnStdOut("/usr/bin/hdiutil", "attach -noautoopen -nobrowse -readonly -plist \"" + sdlDmgPath + "\"", null);
                var plist = new XmlDocument();
                plist.LoadXml(result);
                var mountInfo = GetSDLMountVolume(plist);
                var mountVolume = mountInfo == null ? null : mountInfo.Item2;
                sdlDev = mountInfo == null ? null : mountInfo.Item1;
                if (!string.IsNullOrEmpty(mountVolume) && Directory.Exists(mountVolume))
                {
                    data.UpdateTaskProgress(0, Resources.Strings.SDLInstall_Update_Copying);
                    var sdlSource = NSUrl.FromFilename(mountVolume).Append("SDL.framework", true);
                    NSError error;
                    var userLibraryDir = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User, null, true, out error);
                    if ((userLibraryDir != null) && (error == null) && Directory.Exists(userLibraryDir.Path))
                    {
                        // If the user doesn't already have a Frameworks directory, create it.
                        var userLibraryFrameworksDir = userLibraryDir.Append("Frameworks", true);
                        if (Directory.Exists(userLibraryFrameworksDir.Path) || NSFileManager.DefaultManager.CreateDirectory(userLibraryFrameworksDir, true, null, out error))
                        {
                            var sdlDestination = userLibraryFrameworksDir.Append("SDL.framework", true);
                            if (!NSFileManager.DefaultManager.Copy(sdlSource, sdlDestination, out error))
                            {
                                data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_Failed_Format, error.LocalizedDescription);
                            }
                        }
                        else
                        {
                            data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_Failed_Format, error.LocalizedDescription);
                        }
                    }
                    else
                    {
                        if (error != null)
                        {
                            data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_Failed_Format, error.LocalizedDescription);
                        }
                        else if (userLibraryDir == null)
                        {
                            data.ErrorMessage = Resources.Strings.SDLInstall_UnableToGetUserLibraryDirectory;
                        }
                        else
                        {
                            data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_UserLibraryDirectoryDoesNotExist_Format, userLibraryDir.Path);
                        }
                    }
                }
                else
                {
                    data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_FailedToMount_Format, sdlDmgPath, result);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(sdlDev))
                {
                    data.UpdateTaskProgress(0, Resources.Strings.SDLInstall_Update_Unmounting);
                    var result = RunExternalProgram.CallAndReturnStdOut("/usr/bin/hdiutil", "detach -force " + sdlDev, null);
                    System.Diagnostics.Debug.WriteLine(result);
                }
            }
        }

        private static void InstallSDLComplete(AsyncTaskData taskData)
        {
            var data = (InstallSDLTaskData)taskData;
            if (!string.IsNullOrEmpty(data.ErrorMessage) || (data.Error != null))
            {
                var message = Resources.Strings.SDLInstall_GeneralFailureMessage;
                var dialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.SDLInstall_Failure_Title, message);
                dialog.TextWrapping = OSTextWrapping.Wrap;
                dialog.ShowSendEmailButton = false;
                dialog.ReportText = data.ErrorMessage;
                dialog.Exception = data.Error;
                dialog.ShowDialog(Resources.Strings.ReportErrorDialogButtonText);
            }
            else
            {
                OSMessageBox.Show(Resources.Strings.SDLInstall_Success_Message, Resources.Strings.SDLInstall_Success_Title);
            }
        }

        private static Tuple<string, string> GetSDLMountVolume(XmlNode node)
        {
            Tuple<string, string> mountInfo = null;
            if (node.HasChildNodes)
            {
                var children = node.ChildNodes.Cast<XmlNode>().ToList();
                var mountVolume = string.Empty;
                var mountPointIndex = children.FindIndex(c => (c.LocalName == "key") && (c.InnerXml == "mount-point"));
                if (mountPointIndex >= 0)
                {
                    var mountPointNode = children[mountPointIndex + 1];
                    mountVolume = mountPointNode.InnerXml;
                }
                var mountDevice = string.Empty;
                mountPointIndex = children.FindIndex(c => (c.LocalName == "key") && (c.InnerXml == "dev-entry"));
                if (mountPointIndex >= 0)
                {
                    var mountPointNode = children[mountPointIndex + 1];
                    mountDevice = mountPointNode.InnerXml;
                }
                if (!string.IsNullOrEmpty(mountVolume) && !string.IsNullOrEmpty(mountDevice))
                {
                    mountInfo = new Tuple<string, string>(mountDevice, mountVolume);
                }
                if (mountInfo == null)
                {
                    foreach (var child in children)
                    {
                        mountInfo = GetSDLMountVolume(child);
                        if (mountInfo != null)
                        {
                            break;
                        }
                    }
                }
            }
            return mountInfo;
        }

        private class InstallSDLTaskData : AsyncTaskData
        {
            internal InstallSDLTaskData(AsyncTaskWithProgress task)
                : base(task)
            {
            }

            internal string ErrorMessage { get; set; }
        }

        #endregion // LaunchInJzIntvCommand

        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return null; }
        }

        #endregion // CommandGroup

        partial void AddPlatformCommands()
        {
            JzIntvToolsMenuCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            BrowseAndLaunchInJzIntvCommand.MenuParent = JzIntvToolsMenuCommand;
            var fileLaunchInJzIntvCommand = LaunchInJzIntvCommand.Clone();
            fileLaunchInJzIntvCommand.Weight = 0.06;
            fileLaunchInJzIntvCommand.MenuItemName = LaunchInJzIntvCommand.ContextMenuItemName;
            fileLaunchInJzIntvCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            var fileBrowseAndLaunchInJzIntvCommand = BrowseAndLaunchInJzIntvCommand.Clone();
            fileBrowseAndLaunchInJzIntvCommand.Weight = 0.07;
            fileBrowseAndLaunchInJzIntvCommand.MenuItemName = BrowseAndLaunchInJzIntvCommand.ContextMenuItemName;
            fileBrowseAndLaunchInJzIntvCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            CommandList.Add(JzIntvToolsMenuCommand.CreateSeparator(CommandLocation.Before));
            CommandList.Add(BrowseAndLaunchInJzIntvCommand);
            CommandList.Add(fileLaunchInJzIntvCommand);
            CommandList.Add(fileBrowseAndLaunchInJzIntvCommand);
            CommandList.Add(fileBrowseAndLaunchInJzIntvCommand.CreateSeparator(CommandLocation.After));
        }
    }
}

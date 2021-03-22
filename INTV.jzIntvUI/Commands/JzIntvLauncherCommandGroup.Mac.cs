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
using System.Globalization;
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
#endif // __UNIFIED__

namespace INTV.JzIntvUI.Commands
{
    /// <summary>
    /// Mac-specific implementation
    /// </summary>
    public partial class JzIntvLauncherCommandGroup
    {
        #region LaunchInJzIntvCommand

        private const int SDLNotLoadedError = 133;
        private const int SDLNotLoadedErrorSierra = 134;
        private const string DiskImageFileExtension = ".dmg";
        private static readonly Lazy<SDLDiskImageInfo> IncludedSDLInfo = new Lazy<SDLDiskImageInfo>(GetIncludedSDLVersion);

        private static SDLDiskImageInfo IncludedSDLVersion { get { return IncludedSDLInfo.Value; } }

        /// <summary>
        /// Mac-specific error handler.
        /// </summary>
        /// <param name="emulator">The emulator instance that was running.</param>
        /// <param name="message">The error message.</param>
        /// <param name="exitCode">The exit code of the emulator.</param>
        /// <param name="exception">The exception that occurred.</param>
        static partial void OSErrorHandler(Emulator emulator, string message, int exitCode, Exception exception)
        {
            if (IsSDLMissingError(exitCode))
            {
                var sdlVersionInfo = IncludedSDLVersion;
                var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLNotFound_ErrorMessage, sdlVersionInfo.Version);

                // Check to see if we can determine the missing SDL version from the error message.
                // If the missing version is not the same as the included version, we must get the "other" version.
                // Also, check to see if we already have the included SDL installed. If we do, but the failure still indicates
                // that it's missing, then it's possible the version of jzIntv being run requires the "other" version of SDL.
                // Switch over to the 'other' version for reporting the error if such is the case.
                var missingVersion = GetMissingSDLVersionFromErrorMessage(message);
                if ((missingVersion != SDLVersion.Unknown) && (missingVersion != IncludedSDLVersion.Version)
                   || sdlVersionInfo.IsAlreadyInstalled)
                {
                    sdlVersionInfo = sdlVersionInfo.GetOtherSDLVersionInfo();
                    errorMessage = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Strings.SDLNotFound_DownloadMissingVersion_Format,
                        sdlVersionInfo.Version,
                        sdlVersionInfo.VersionNumber,
                        sdlVersionInfo.DownloadUrl);
                }

                var errorTitle = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLNotFound_ErrorTitle, sdlVersionInfo.Version);
                if (OSMessageBox.Show(errorMessage, errorTitle, OSMessageBoxButton.YesNo) == OSMessageBoxResult.Yes)
                {
                    var installSDLTask = new AsyncTaskWithProgress("InstallSDL", allowsCancel: false, isIndeterminate: true, progressDisplayDelay: 0);
                    var installTitle = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Title, sdlVersionInfo.Version);
                    installSDLTask.UpdateTaskTitle(installTitle);
                    var installSDLTaskData = new InstallSDLTaskData(installSDLTask, sdlVersionInfo);
                    installSDLTask.RunTask(installSDLTaskData, InstallSDL, InstallSDLComplete);
                }
            }
        }

        private static bool IsSDLMissingError(int exitCode)
        {
            var macOSVersion = OSVersion.Current;
            var isSDLMissingError = false;
            if (macOSVersion.Minor >= 12)
            {
                isSDLMissingError = exitCode == SDLNotLoadedErrorSierra;
            }
            else
            {
                isSDLMissingError = exitCode == SDLNotLoadedError;
            }
            return isSDLMissingError;
        }

        private static SDLVersion GetMissingSDLVersionFromErrorMessage(string errorMessage)
        {
            // Often when SDL is missing, the error output will contain some text of the form:
            //   "dyld: Library not loaded: @rpath/SDL.framework/Versions/A/SDL"
            // from which the missing SDL version can be ascertained. If it does, we'll try to
            // determine the exact missing SDL and prompt accordingly to install.
            var missingSDL = SDLVersion.Unknown;
            const string rpath = "@rpath/";
            var index = errorMessage.IndexOf(rpath, StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                index += rpath.Length;
                var nextSlashIndex = errorMessage.IndexOf('/', index);
                if (nextSlashIndex > index)
                {
                    var remainder = errorMessage.Substring(index, nextSlashIndex - index);
                    var sdlVersionName = remainder.Split('.').First();
                    if (!Enum.TryParse(sdlVersionName, out missingSDL))
                    {
                        missingSDL = SDLVersion.Unknown;
                    }
                }
            }
            return missingSDL;
        }

        private static void InstallSDL(AsyncTaskData taskData)
        {
            var data = (InstallSDLTaskData)taskData;
            var diskImageInfo = data.DiskImageInfo;
            var progressMessage = string.Empty;
            if ((diskImageInfo.DownloadUrl != null) && !File.Exists(diskImageInfo.Path))
            {
                progressMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Strings.SDLNotFound_Downloading_Format,
                    diskImageInfo.Version,
                    diskImageInfo.DownloadUrl,
                    diskImageInfo.Path);
                data.UpdateTaskProgress(0, progressMessage);
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    var response = httpClient.GetAsync(diskImageInfo.DownloadUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        using (var sdkDiskImageFile = new FileStream(diskImageInfo.Path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            stream.CopyTo(sdkDiskImageFile);
                        }
                    }
                }
            }
            progressMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Update_Mounting, diskImageInfo.FileName);
            data.UpdateTaskProgress(0, progressMessage);
            var sdlDmgPath = diskImageInfo.Path;
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
                    progressMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Update_Copying, diskImageInfo.FrameworkName);
                    data.UpdateTaskProgress(0, progressMessage);
                    var sdlSource = NSUrl.FromFilename(mountVolume).Append(diskImageInfo.FrameworkName, isDirectory: true);
                    string errorMessage;
                    var userLibraryDir = diskImageInfo.GetUserLibraryDirectory(out errorMessage);
                    if ((userLibraryDir != null) && string.IsNullOrEmpty(errorMessage))
                    {
                        // If the user doesn't already have a Frameworks directory, create it.
                        NSError error;
                        var userLibraryFrameworksDir = userLibraryDir.Append("Frameworks", true);
                        if (Directory.Exists(userLibraryFrameworksDir.Path) || NSFileManager.DefaultManager.CreateDirectory(userLibraryFrameworksDir, true, null, out error))
                        {
                            var sdlDestination = userLibraryFrameworksDir.Append(diskImageInfo.FrameworkName, isDirectory: true);
                            if (!NSFileManager.DefaultManager.Copy(sdlSource, sdlDestination, out error))
                            {
                                data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_Failed_Format, diskImageInfo.Version, error.LocalizedDescription);
                            }
                        }
                        else
                        {
                            data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_Failed_Format, diskImageInfo.Version, error.LocalizedDescription);
                        }
                    }
                    else
                    {
                        data.ErrorMessage = errorMessage;
                    }
                }
                else
                {
                    data.ErrorMessage = string.Format(Resources.Strings.SDLInstall_FailedToMount_Format, diskImageInfo.Version, sdlDmgPath, result);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(sdlDev))
                {
                    progressMessage = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Update_Unmounting, diskImageInfo.FileName);
                    data.UpdateTaskProgress(0, progressMessage);
                    var result = RunExternalProgram.CallAndReturnStdOut("/usr/bin/hdiutil", "detach -force " + sdlDev, null);
                    System.Diagnostics.Debug.WriteLine(result);
                }
            }
        }

        private static void InstallSDLComplete(AsyncTaskData taskData)
        {
            var data = (InstallSDLTaskData)taskData;
            var diskImageInfo = data.DiskImageInfo;
            if (!string.IsNullOrEmpty(data.ErrorMessage) || (data.Error != null))
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_GeneralFailureMessage, diskImageInfo.Version);
                var title = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Failure_Title, diskImageInfo.Version);
                var dialog = INTV.Shared.View.ReportDialog.Create(title, message);
                dialog.TextWrapping = OSTextWrapping.Wrap;
                dialog.ShowSendEmailButton = false;
                dialog.ReportText = data.ErrorMessage;
                dialog.Exception = data.Error;
                dialog.ShowDialog(Resources.Strings.ReportErrorDialogButtonText);
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Success_Message, diskImageInfo.Version);
                var title = string.Format(CultureInfo.CurrentCulture, Resources.Strings.SDLInstall_Success_Title, diskImageInfo.Version);
                OSMessageBox.Show(message, title);
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

        private static SDLDiskImageInfo GetIncludedSDLVersion()
        {
            var sdlVersionInfo = new SDLDiskImageInfo();
            var sdlImageResourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jzIntv/Mac/bin");
            if (Directory.Exists(sdlImageResourcePath))
            {
                var includedSDLDiskImages = PathUtils.EnumerateFilesWithPattern(sdlImageResourcePath, DiskImageFileExtension);
                if (includedSDLDiskImages.Count() == 1)
                {
                    var includedSDLDiskImage = includedSDLDiskImages.First();
                    var fileName = Path.GetFileNameWithoutExtension(includedSDLDiskImage);
                    var fileNameParts = fileName.Split('-');
                    var sdlVersion = SDLVersion.Unknown;
                    if ((fileNameParts.Length > 1) && Enum.TryParse(fileNameParts[0], out sdlVersion))
                    {
                        sdlVersionInfo = new SDLDiskImageInfo(sdlVersion, fileNameParts[1], includedSDLDiskImage);
                    }
                }
            }
            return sdlVersionInfo;
        }

        /// <summary>
        /// Task data used for the installation of SDL.
        /// </summary>
        private class InstallSDLTaskData : AsyncTaskData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InstallSDLTaskData"/> class.
            /// </summary>
            /// <param name="task">The installation task.</param>
            /// <param name="diskImageInfo">Description of the SDL disk image.</param>
            internal InstallSDLTaskData(AsyncTaskWithProgress task, SDLDiskImageInfo diskImageInfo)
                : base(task)
            {
                DiskImageInfo = diskImageInfo;
            }

            /// <summary>
            /// Gets the disk image information describing the SDL to install.
            /// </summary>
            internal SDLDiskImageInfo DiskImageInfo { get; private set; }

            /// <summary>
            /// Gets or sets the error message.
            /// </summary>
            internal string ErrorMessage { get; set; }
        }

        /// <summary>
        /// This enum is used to identify the version of SDL the embedded jzIntv uses.
        /// </summary>
        private enum SDLVersion
        {
            /// <summary>Not a known version of the SDL.</summary>
            Unknown = 0,

            /// <summary>Version 1 of the SDL.</summary>
            SDL = 1,

            /// <summary>Version 2 of the SDL.</summary>
            SDL2 = 2
        }

        /// <summary>
        /// Convenience type to provide version information for the included SDL image.
        /// </summary>
        private class SDLDiskImageInfo : Tuple<SDLVersion, string, string>
        {
            private const string FallbackSDLVersion = "1.2.15";
            private const string FallbackSDL2Version = "2.0.14";

            /// <summary>
            /// Initializes a new default instance of the <see cref="SDLDiskImageInfo"/> class.
            /// </summary>
            internal SDLDiskImageInfo()
                : base(SDLVersion.Unknown, null, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SDLDiskImageInfo"/> class.
            /// </summary>
            /// <param name="version">The SDL version of the disk image.</param>
            /// <param name="versionNumber">The specific version number of the disk image.</param>
            /// <param name="path">The absolute path to the disk image resource.</param>
            internal SDLDiskImageInfo(SDLVersion version, string versionNumber, string path)
                : base(version, versionNumber, path)
            {
            }

            /// <summary>
            /// Gets the SDL version of the disk image resource.
            /// </summary>
            public SDLVersion Version { get { return Item1; } }

            /// <summary>
            /// Gets the specific version number of the disk image resource.
            /// </summary>
            public string VersionNumber { get { return Item2; } }

            /// <summary>
            /// Gets the absolute path of the disk image resource.
            /// </summary>
            public string Path { get { return Item3; } }

            /// <summary>
            /// Gets the file name of the disk image resource.
            /// </summary>
            public string FileName { get { return (Path == null) ? null : System.IO.Path.GetFileName(Path); } }

            /// <summary>
            /// Gets the file name of the disk image resource.
            /// </summary>
            public string FrameworkName { get { return Version + ".framework"; } }

            /// <summary>
            /// Gets the local URL for the SDL framework.
            /// </summary>
            public NSUrl LibraryUrl { get { return GetLibraryUrl(); } }

            /// <summary>
            /// Gets a value indicating whether or not the current SDL is already on the system.
            /// </summary>
            public bool IsAlreadyInstalled { get { return GetIsAlreadyInstalled(); } }

            /// <summary>
            /// Gets the URL from which to download the SDL disk image.
            /// </summary>
            /// <remarks>This value is only set for 'other' versions of SDL, not the default version.</remarks>
            public Uri DownloadUrl { get; private set; }

            /// <summary>
            /// Gets the user library directory.
            /// </summary>
            /// <returns>The user library directory.</returns>
            /// <param name="errorMessage">Receives localized error message if getting the URL fails.</param>
            internal NSUrl GetUserLibraryDirectory(out string errorMessage)
            {
                errorMessage = null;
                NSError error;
                var userLibraryUrl = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User, null, true, out error);
                if (error != null)
                {
                    errorMessage = string.Format(Resources.Strings.SDLInstall_Failed_Format, Version, error.LocalizedDescription);
                }
                else if (userLibraryUrl == null)
                {
                    errorMessage = Resources.Strings.SDLInstall_UnableToGetUserLibraryDirectory;
                }
                else if (!Directory.Exists(userLibraryUrl.Path))
                {
                    errorMessage = string.Format(Resources.Strings.SDLInstall_UserLibraryDirectoryDoesNotExist_Format, userLibraryUrl.Path);
                }
                return userLibraryUrl;
            }

            /// <summary>
            /// Gets the "other" SDL version information.
            /// </summary>
            /// <returns>The other SDL version data, including a download URL.</returns>
            internal SDLDiskImageInfo GetOtherSDLVersionInfo()
            {
                var otherVersion = SDLVersion.Unknown;
                var otherVersionNumber = string.Empty;

                switch (Version)
                {
                    case SDLVersion.SDL:
                        otherVersion = SDLVersion.SDL2;
                        otherVersionNumber = FallbackSDL2Version;
                        break;
                    case SDLVersion.SDL2:
                        otherVersion = SDLVersion.SDL;
                        otherVersionNumber = FallbackSDLVersion;
                        break;
                    default:
                        break;
                }

                var otherSDLVersionInfo = new SDLDiskImageInfo();
                if (otherVersion != SDLVersion.Unknown)
                {
                    var downloadPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DownloadsDirectory, NSSearchPathDomain.All)?.FirstOrDefault();
                    var otherVersionFileName = $"{otherVersion}-{otherVersionNumber}{DiskImageFileExtension}";
                    otherSDLVersionInfo = new SDLDiskImageInfo(otherVersion, otherVersionNumber, downloadPath?.Append(otherVersionFileName, isDirectory: false)?.Path)
                    {
                        DownloadUrl = new Uri($"https://www.libsdl.org/release/{otherVersionFileName}", UriKind.Absolute)
                    };
                }
                return otherSDLVersionInfo;
            }

            private NSUrl GetLibraryUrl()
            {
                string error;
                var libraryUrl = GetUserLibraryDirectory(out error);
                if ((libraryUrl != null) && string.IsNullOrEmpty(error))
                {
                    libraryUrl = libraryUrl?.Append("Frameworks", isDirectory: true)?.Append(FrameworkName, isDirectory: true);
                }
                else
                {
                    libraryUrl = null;
                }
                return libraryUrl;
            }

            private bool GetIsAlreadyInstalled()
            {
                bool alreadyInstalled = false;
                var libraryUrl = LibraryUrl;
                if (libraryUrl != null)
                {
                    alreadyInstalled = NSFileManager.DefaultManager.FileExists(libraryUrl.Path);
                }
                return alreadyInstalled;
            }
        }

        #endregion // LaunchInJzIntvCommand

        #region CommandGroup

        /// <summary>
        /// General data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return null; }
        }

        #endregion // CommandGroup

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            JzIntvToolsMenuCommand.MenuParent = RootCommandGroup.ToolsMenuCommand;
            LaunchInJzIntvCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            LaunchInJzIntvCommand.MenuItemName = LaunchInJzIntvCommand.ContextMenuItemName;
            LaunchInJzIntvCommand.Weight = 0.06;
            BrowseAndLaunchInJzIntvCommand.MenuParent = RootCommandGroup.FileMenuCommand; // JzIntvToolsMenuCommand;
            BrowseAndLaunchInJzIntvCommand.MenuItemName = BrowseAndLaunchInJzIntvCommand.ContextMenuItemName;
            BrowseAndLaunchInJzIntvCommand.Weight = 0.07;
            ShowJzIntvCommandLineCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            ShowJzIntvCommandLineCommand.MenuItemName = ShowJzIntvCommandLineCommand.ContextMenuItemName;
            ShowJzIntvCommandLineCommand.Weight = 0.08;
            CommandList.Add(ShowJzIntvCommandLineCommand.CreateSeparator(CommandLocation.After));

            var toolsLaunchInJzIntvCommand = LaunchInJzIntvCommand.Clone();
            toolsLaunchInJzIntvCommand.Weight = 0.2;
            toolsLaunchInJzIntvCommand.MenuItemName = Resources.Strings.LaunchInJzIntvCommand_MenuItemName; // LaunchInJzIntvCommand.ContextMenuItemName;
            toolsLaunchInJzIntvCommand.MenuParent = JzIntvToolsMenuCommand;

            var toolsBrowseAndLaunchInJzIntvCommand = BrowseAndLaunchInJzIntvCommand.Clone();
            toolsBrowseAndLaunchInJzIntvCommand.Weight = 0.21;
            toolsBrowseAndLaunchInJzIntvCommand.MenuItemName = Resources.Strings.BrowseAndLaunchInJzIntvCommand_Name; // BrowseAndLaunchInJzIntvCommand.ContextMenuItemName;
            toolsBrowseAndLaunchInJzIntvCommand.MenuParent = JzIntvToolsMenuCommand; // RootCommandGroup.FileMenuCommand;

            var toolsShowJzIntvCommandLineCommand = ShowJzIntvCommandLineCommand.Clone();
            toolsShowJzIntvCommandLineCommand.Weight = 0.22;
            toolsShowJzIntvCommandLineCommand.MenuItemName = Resources.Strings.ShowJzIntvCommandLineCommand_MenuItemName;
            toolsShowJzIntvCommandLineCommand.MenuParent = JzIntvToolsMenuCommand;

            CommandList.Add(BrowseAndLaunchInJzIntvCommand);
            CommandList.Add(ShowJzIntvCommandLineCommand);

            CommandList.Add(JzIntvToolsMenuCommand.CreateSeparator(CommandLocation.Before));
            CommandList.Add(toolsLaunchInJzIntvCommand);
            CommandList.Add(toolsBrowseAndLaunchInJzIntvCommand);
            CommandList.Add(toolsShowJzIntvCommandLineCommand);
            CommandList.Add(toolsShowJzIntvCommandLineCommand.CreateSeparator(CommandLocation.After));
        }
    }
}

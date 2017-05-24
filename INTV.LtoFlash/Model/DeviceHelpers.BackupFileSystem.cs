// <copyright file="DeviceHelpers.BackupFileSystem.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for BackupFileSystem.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region BackupFileSystem

        /// <summary>
        /// Executes a backup of the file system on a Locutus device.
        /// </summary>
        /// <param name="device">The device whose data is to be backed up.</param>
        /// <param name="backupDirectory">The directory in which to do the backup.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Function to call if an error occurs. If it returns <c>true</c>, the error is considered handled</param>
        public static void BackupFileSystem(this Device device, string backupDirectory, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_BackupFileSystem_Title,
                    ProgressUpdateMode = ExecuteDeviceCommandProgressUpdateMode.Custom,
                    Data = backupDirectory,
                    OnSuccess = onCompleteHandler,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(BackupFileSystem, true, 0.5);
            }
        }

        /// <summary>
        /// Protocol commands necessary for BackupFileSystem.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> BackupFileSystemProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsGetFileSystemStatusFlags,
            ProtocolCommandId.LfsDownloadGlobalTables,
            ProtocolCommandId.LfsCopyForkToRam,
            ProtocolCommandId.LfsDownloadDataBlockFromRam,
            ProtocolCommandId.LfsChecksumDataBlockInRam
        };

        private static void BackupFileSystem(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var succeeded = false;

            try
            {
                var configuration = SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
                var romsConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.Shared.Model.RomListConfiguration>();
                var roms = SingleInstanceApplication.Instance.Roms;
                var device = data.Device;
                data.UpdateTaskProgress(0, Resources.Strings.DeviceMultistageCommand_BackupFileSystem_Syncing);
                var deviceFileSystemFlags = GetDirtyFlags.Instance.Execute<LfsDirtyFlags>(device.Port, data);

                // Always re-fetch the file system. When we're done, tell the UI to refresh.
                var fileSystem = DownloadFileSystemTables.Instance.Execute<FileSystem>(device.Port, data);
                fileSystem.Status = deviceFileSystemFlags;
                data.Result = new FileSystemSyncErrors(deviceFileSystemFlags);

                // Retrieve the forks.
                var backupDirectory = data.Data as string;
                var forksToBackup = fileSystem.Forks.Where(f => f != null);
                if (forksToBackup.Any() && !data.AcceptCancelIfRequested())
                {
                    var forkFileNames = new List<string>();
                    foreach (var fork in forksToBackup)
                    {
                        bool dontCare;
                        var backupDir = backupDirectory; // we don't want to modify backupDirectory
                        var fileName = GetPathForFork(data, fork, fileSystem, roms, romsConfiguration, ref backupDir, out dontCare);
                        fileName = System.IO.Path.GetFileName(fileName);
                        forkFileNames.Add(fileName);
                    }
                    succeeded = device.RetrieveForkData(data, forksToBackup, backupDirectory, forkFileNames);
                }
                else
                {
                    succeeded = data.CancelRequsted;
                }
                if (succeeded && !data.AcceptCancelIfRequested())
                {
                    data.UpdateTaskProgress(0, Resources.Strings.DeviceMultistageCommand_BackupFileSystem_CreatingMenuLayout);
                    var menuBackupPath = System.IO.Path.Combine(backupDirectory, configuration.DefaultMenuLayoutFileName);
                    var menuLayout = new MenuLayout(fileSystem, device.UniqueId);
                    menuLayout.LoadComplete(false);
                    menuLayout.Save(menuBackupPath, true);
                }
                else if (data.CancelRequsted && succeeded)
                {
                    succeeded = true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                succeeded = false;
                if (!string.IsNullOrWhiteSpace(data.FailureMessage))
                {
                    var builder = new System.Text.StringBuilder(Resources.Strings.DeviceMultistageCommand_BackupFileSystem_Failed);
                    builder.AppendLine().AppendLine(data.FailureMessage);
                    data.FailureMessage = builder.ToString();
                }
                else
                {
                    data.FailureMessage = Resources.Strings.DeviceMultistageCommand_BackupFileSystem_Failed;
                }
                throw;
            }
            finally
            {
                data.Succeeded = succeeded;
            }
        }

        #endregion // BackupFileSystem
    }
}

// <copyright file="DeviceHelpers.SyncDeviceToHost.cs" company="INTV Funhouse">
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
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Model;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for SyncDeviceToHost.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region SyncDeviceToHost

        /// <summary>
        /// Synchronizes the host system's MenuLayout to match the contents of a Locutus device.
        /// </summary>
        /// <param name="device">The Locutus device whose file system is to be imposed upon the Menu Layout of the host PC.</param>
        /// <param name="hostMenuLayout">The host PC MenuLayout to be brought in sync with Locutus.</param>
        /// <param name="ignoreInconsistentFileSystem">If <c>true</c> and the device's file system is in an inconsistent state, do the sync anyway.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void SyncDeviceToHost(this Device device, MenuLayout hostMenuLayout, bool ignoreInconsistentFileSystem, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                ////var configuration = SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
                var customData = new Tuple<MenuLayout, bool>(hostMenuLayout, ignoreInconsistentFileSystem);
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_SyncingToFiles_Title,
                    ProgressUpdateMode = ExecuteDeviceCommandProgressUpdateMode.Custom,
                    Data = customData,
                    OnSuccess = onCompleteHandler,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(SyncDeviceToHost, true, 1);
            }
        }

        /// <summary>
        /// Protocol commands necessary for SyncDeviceToHost.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> SyncFromDeviceProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsGetFileSystemStatusFlags,
            ProtocolCommandId.LfsCopyForkToRam,
            ProtocolCommandId.LfsDownloadDataBlockFromRam,
            ProtocolCommandId.LfsDownloadGlobalTables
        };

        private static void SyncDeviceToHost(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            data.Task.UpdateTaskProgress(0, Resources.Strings.DeviceMultistageCommand_UpdatingMenuLayout_ComputingChangesProgress);

            var customData = (Tuple<MenuLayout, bool>)data.Data;
            var currentDirtyFlags = GetDirtyFlags.Instance.Execute<LfsDirtyFlags>(device.Port, data);
            var syncErrors = new FileSystemSyncErrors(currentDirtyFlags);
            data.Result = syncErrors;

            // If customData.Item2 is true, that means we should ignore file system inconsistencies, ergo if it's false, we should NOT ignore them.
            if (currentDirtyFlags.HasFlag(LfsDirtyFlags.FileSystemUpdateInProgress) && !customData.Item2)
            {
                throw new InconsistentFileSystemException(Resources.Strings.DeviceMultistageCommand_UpdatingMenuLayout_InconsistentState, device.UniqueId);
            }
            var deviceFileSystem = DownloadFileSystemTables.Instance.Execute<FileSystem>(device.Port, data);
            deviceFileSystem.Status = currentDirtyFlags;
            if (data.AcceptCancelIfRequested())
            {
                return;
            }
            var hostFileSystem = customData.Item1.FileSystem;
            deviceFileSystem.RemoveMenuPositionData(); // we should not preserve the menu position fork
            hostFileSystem.PopulateSaveDataForksFromDevice(deviceFileSystem);
            var allDifferences = deviceFileSystem.CompareTo(hostFileSystem);

            if (!allDifferences.Any())
            {
                data.Task.CancelTask();
            }

            // We're going to apply the changes to a clone of the original file system.
            if (data.AcceptCancelIfRequested())
            {
                return;
            }
            var fileSystemToModify = hostFileSystem.Clone();
            if (data.AcceptCancelIfRequested())
            {
                return;
            }

            // Before we change anything, create backups of the ROM list and current menu layout.
            var configuration = SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
            var romsConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.Shared.Model.RomListConfiguration>();
            if (System.IO.File.Exists(configuration.MenuLayoutPath) || System.IO.File.Exists(romsConfiguration.RomFilesPath))
            {
                var backupTimestamp = INTV.Shared.Utility.PathUtils.GetTimeString();
                var backupSubdirectory = configuration.SyncFromDeviceBackupFilenameFragment + "-" + backupTimestamp;
                var backupDirectory = System.IO.Path.Combine(configuration.HostBackupDataAreaPath, backupSubdirectory);
                if (!System.IO.Directory.Exists(backupDirectory))
                {
                    System.IO.Directory.CreateDirectory(backupDirectory);
                }
                if (System.IO.File.Exists(configuration.MenuLayoutPath))
                {
                    var backupMenuLayoutPath = System.IO.Path.Combine(backupDirectory, configuration.DefaultMenuLayoutFileName);
                    System.IO.File.Copy(configuration.MenuLayoutPath, backupMenuLayoutPath);
                }
                if (System.IO.File.Exists(romsConfiguration.RomFilesPath))
                {
                    var backupRomListPath = System.IO.Path.Combine(backupDirectory, romsConfiguration.DefaultRomsFileName);
                    System.IO.File.Copy(romsConfiguration.RomFilesPath, backupRomListPath);
                }
            }

            // First, directory deletion.
            if (!data.CancelRequsted)
            {
                foreach (var directory in allDifferences.DirectoryDifferences.ToDelete)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // We don't use RemoveAt because of the collateral damage it may cause.
                    fileSystemToModify.Directories[(int)directory] = null;
                }
            }

            // Then, we process file deletion.
            if (!data.CancelRequsted)
            {
                foreach (var file in allDifferences.FileDifferences.ToDelete)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // We don't use RemoveAt because of the collateral damage it may cause.
                    fileSystemToModify.Files[(int)file] = null;
                }
            }

            // Finally, fork deletion.
            if (!data.CancelRequsted)
            {
                foreach (var fork in allDifferences.ForkDifferences.ToDelete)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // We don't use RemoveAt because of the collateral damage it may cause.
                    fileSystemToModify.Forks[(int)fork] = null;
                }
            }

            var roms = SingleInstanceApplication.Instance.Roms;
            var romsToAdd = new HashSet<string>();
            var forkNumberMap = new Dictionary<ushort, ushort>(); // maps device -> local
            var forkSourceFileMap = new Dictionary<ushort, string>(); // maps local fork number -> source file for fork

            // Add new forks.
            if (!data.CancelRequsted)
            {
                foreach (var fork in allDifferences.ForkDifferences.ToAdd)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // Forks on device but not in our menu must be added.
                    System.Diagnostics.Debug.WriteLine("Fork adds! This means CRC24 of a LUIGI -> ???");

                    Fork newLocalFork = null;
                    var forkSourcePath = GetSourcePathForFork(data, fork, deviceFileSystem, roms, romsConfiguration); // retrieves the fork if necessary
                    if (forkSourcePath != null)
                    {
                        newLocalFork = new Fork(forkSourcePath) { GlobalForkNumber = fork.GlobalForkNumber };
                        romsToAdd.Add(forkSourcePath);
                    }
                    else
                    {
                        // Is this code even reachable any more??? Looking at GetSourcePathForFork, I would think not.
                        newLocalFork = new Fork(fork.Crc24, fork.Size, fork.GlobalForkNumber);
                        var cacheEntry = CacheIndex.Find(fork.Crc24, fork.Size);
                        if (cacheEntry != null)
                        {
                            newLocalFork.FilePath = System.IO.Path.Combine(configuration.RomsStagingAreaPath, cacheEntry.LuigiPath);
                        }
                    }
                    newLocalFork.FileSystem = fileSystemToModify;
                    fileSystemToModify.Forks.AddAndRelocate(newLocalFork);
                    forkNumberMap[fork.GlobalForkNumber] = newLocalFork.GlobalForkNumber;
                    if (string.IsNullOrEmpty(forkSourcePath))
                    {
                        System.Diagnostics.Debug.WriteLine("Bad path in Fork add.");
                        syncErrors.UnableToRetrieveForks.Add(newLocalFork);
                    }
                    forkSourceFileMap[newLocalFork.GlobalForkNumber] = forkSourcePath;
                }
            }

            // Update forks.
            if (!data.CancelRequsted)
            {
                foreach (var fork in allDifferences.ForkDifferences.ToUpdate)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // Forks on the device don't store file paths.
                    var localFork = fileSystemToModify.Forks[fork.GlobalForkNumber];
                    forkNumberMap[fork.GlobalForkNumber] = localFork.GlobalForkNumber;
                    var forkSourcePath = GetSourcePathForFork(data, fork, deviceFileSystem, roms, romsConfiguration);
                    if (string.IsNullOrEmpty(forkSourcePath))
                    {
                        System.Diagnostics.Debug.WriteLine("Bad path in Fork update.");
                        syncErrors.UnableToRetrieveForks.Add(localFork);
                    }
                    forkSourceFileMap[localFork.GlobalForkNumber] = forkSourcePath;
                    localFork.FilePath = forkSourcePath;
                    if ((localFork.Crc24 != fork.Crc24) || (localFork.Size != fork.Size))
                    {
                        localFork.FilePath = null; // May need to regenerate the LUIGI file...
                        System.Diagnostics.Debug.WriteLine("Fork at path doesn't match! " + forkSourcePath);
                        syncErrors.UnableToRetrieveForks.Add(localFork);
                    }
                    ProgramDescription description = null;
                    var forkKind = deviceFileSystem.GetForkKind(fork);
                    if (SyncForkData(localFork, forkKind, forkSourceFileMap, null, ref description))
                    {
                        IEnumerable<ILfsFileInfo> filesUsingFork;
                        if (fileSystemToModify.GetAllFilesUsingForks(new[] { localFork }).TryGetValue(localFork, out filesUsingFork))
                        {
                            foreach (var program in filesUsingFork.OfType<Program>())
                            {
                                // This situation can arise when we have a ROM on the local system that has the
                                // same CRC as the one in the device's file system, but whose .cfg file has drifted
                                // from what was in place when the LUIGI file on the device was initially created
                                // and deployed. What we need to do in this case, then, is to force the programs
                                // pointing to this fork to actually use the LUIGI file now.
                                // The LUIGI file has already been put into the right place -- it's now a matter of
                                // forcing the program to actually point to it. This runs a bit counter to how several
                                // IRom implementations are wrappers around other types of ROMs.
                                romsToAdd.Add(forkSourcePath);
                            }
                        }
                    }
                    else
                    {
                        syncErrors.UnableToRetrieveForks.Add(localFork);
                    }
                }
            }

            var recoveredRomFiles = romsToAdd.IdentifyRomFiles(data.AcceptCancelIfRequested, (f) => data.UpdateTaskProgress(0, f));
            var recoveredRoms = ProgramCollection.GatherRomsFromFileList(recoveredRomFiles, roms, null, data.AcceptCancelIfRequested, (f) => data.UpdateTaskProgress(0, f), null, null);
            if (roms.AddNewItemsFromList(recoveredRoms).Any())
            {
                roms.Save(romsConfiguration.RomFilesPath, false); // this may throw an error, which, in this case, will terminate the operation
            }

            // Add files.
            if (!data.CancelRequsted)
            {
                foreach (var file in allDifferences.FileDifferences.ToAdd)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }
                    var fileNode = FileNode.Create((LfsFileInfo)file);
                    fileNode.FileSystem = fileSystemToModify;
                    fileSystemToModify.Files.AddAndRelocate(fileNode);
                    if (file.FileType == FileType.Folder)
                    {
                        fileSystemToModify.Directories.AddAndRelocate((IDirectory)fileNode);
                    }
                    SyncFileData(fileNode, file, false, forkNumberMap, forkSourceFileMap, syncErrors);
                }
            }

            // Update files.
            var fixups = new Dictionary<FileNode, ILfsFileInfo>();
            if (!data.CancelRequsted)
            {
                foreach (var file in allDifferences.FileDifferences.ToUpdate)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }
                    var localFile = (FileNode)fileSystemToModify.Files[file.GlobalFileNumber];
                    if (localFile.FileType != file.FileType)
                    {
                        // We've got a case of a file changing to / from a directory, which must be handled differently.
                        // Simply null out the entry to prevent unwanted collateral damage, such as deleted forks or other files.
                        fileSystemToModify.Files[localFile.GlobalFileNumber] = null;
                        var localParent = (Folder)localFile.Parent;
                        var localGdn = localFile.GlobalDirectoryNumber;
                        var indexInParent = localParent.IndexOfChild(localFile);
                        localFile = FileNode.Create((LfsFileInfo)file);
                        localFile.FileSystem = fileSystemToModify;
                        localFile.GlobalFileNumber = file.GlobalFileNumber;
                        fileSystemToModify.Files.Add(localFile);
                        SyncFileData(localFile, file, true, forkNumberMap, forkSourceFileMap, syncErrors);

                        switch (file.FileType)
                        {
                            case FileType.File:
                                ////System.Diagnostics.Debug.Assert(localFile.FileType == file.FileType, "File type mutation! Need to implement!");
                                // The directory on the local file system is being replaced with a file. It's possible that the directory
                                // itself has been reparented to a new location, so we do not 'destructively' remove it. Instead, we will
                                // null out its entry in the GDT / GFT and replace the GFT entry with a new file.
                                fileSystemToModify.Directories[localGdn] = null; // so we don't accidentally nuke files and their forks - directories will be updated later
                                break;
                            case FileType.Folder:
                                // The file on the local file system is a standard file, but on the device, it's now a directory.
                                // Need to remove the file and create a directory in its place. We'll also need to populate the directory.
                                // The directory population will need to happen after we've completely finished all the adds / updates.
                                localFile.GlobalDirectoryNumber = file.GlobalDirectoryNumber;
                                fileSystemToModify.Directories.Add((Folder)localFile);
                                break;
                        }
                        localParent.Files[indexInParent] = localFile;
                        fixups[localFile] = file;
                    }
                    else
                    {
                        SyncFileData(localFile, file, true, forkNumberMap, forkSourceFileMap, syncErrors);
                    }
                }
            }

            // Add directories.
            if (!data.CancelRequsted)
            {
                foreach (var directory in allDifferences.DirectoryDifferences.ToAdd)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // Directory itself may already be in the file system. Now, we need to set contents correctly.
                    var localFolder = (Folder)fileSystemToModify.Directories[directory.GlobalDirectoryNumber];
                    if (localFolder == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Where's my dir?");
                        syncErrors.FailedToCreateEntries.Add(directory);
                    }
                    for (var i = 0; i < directory.PresentationOrder.ValidEntryCount; ++i)
                    {
                        if (data.AcceptCancelIfRequested())
                        {
                            break;
                        }
                        var childToAdd = fileSystemToModify.Files[directory.PresentationOrder[i]];
                        localFolder.AddChild((IFile)childToAdd, false);
                    }
                }
            }

            // Update directories.
            if (!data.CancelRequsted)
            {
                foreach (var directory in allDifferences.DirectoryDifferences.ToUpdate)
                {
                    // Need to keep contents in sync. All other changes were handled by file update.
                    var localFolder = (Folder)fileSystemToModify.Directories[directory.GlobalDirectoryNumber];
                    if (localFolder == null)
                    {
                        localFolder = new Folder(fileSystemToModify, directory.GlobalDirectoryNumber, string.Empty);
                    }
                    var localNumEntries = localFolder.PresentationOrder.ValidEntryCount;
                    var devicePresentationOrder = directory.PresentationOrder;
                    for (var i = devicePresentationOrder.ValidEntryCount; !data.CancelRequsted && (i < localNumEntries); ++i)
                    {
                        if (data.AcceptCancelIfRequested())
                        {
                            break;
                        }
                        var prevFile = localFolder.Files[devicePresentationOrder.ValidEntryCount];
                        var prevParent = prevFile.Parent;
                        localFolder.Files.RemoveAt(devicePresentationOrder.ValidEntryCount);
                        prevFile.Parent = prevParent;
                    }
                    localNumEntries = localFolder.PresentationOrder.ValidEntryCount;
                    for (var i = 0; !data.CancelRequsted && (i < (devicePresentationOrder.ValidEntryCount - localNumEntries)); ++i)
                    {
                        if (data.AcceptCancelIfRequested())
                        {
                            break;
                        }
                        localFolder.Files.Add((FileNode)fileSystemToModify.Files[devicePresentationOrder[i]]);
                    }
                    System.Diagnostics.Debug.Assert(localFolder.Files.Count == devicePresentationOrder.ValidEntryCount, "Incorrect number of children in directory!");
                    for (var i = 0; !data.CancelRequsted && (i < devicePresentationOrder.ValidEntryCount); ++i)
                    {
                        if (data.AcceptCancelIfRequested())
                        {
                            break;
                        }
                        var localFile = (FileNode)fileSystemToModify.Files[devicePresentationOrder[i]];
                        if (!ReferenceEquals(localFolder.Files[i], localFile))
                        {
                            var prevFile = localFolder.Files[i];
                            var prevFileParent = prevFile == null ? null : prevFile.Parent;
                            localFolder.Files[i] = localFile;
                            localFile.Parent = localFolder;
                            if (prevFile != null)
                            {
                                // The default behavior of item replacement is to null the parent of the existing item.
                                // Therefore, because some item rearrangement results in a file shuffling 'up' or 'down'
                                // in the same folder, we need to retain the parent. So if the item is still in here,
                                // reset its parent.
                                prevFile.Parent = prevFileParent;
                            }
                        }
                    }
                }
            }

            // Now, pass back the new MenuLayout so UI thread can update and save.
            if (!data.CancelRequsted && data.Succeeded)
            {
                syncErrors.Data = (MenuLayout)fileSystemToModify.Directories[GlobalDirectoryTable.RootDirectoryNumber];
            }
        }

        private static string GetSourcePathForFork(ExecuteDeviceCommandAsyncTaskData data, Fork fork, FileSystem deviceFileSystem, IEnumerable<ProgramDescription> roms, RomListConfiguration romsConfiguration)
        {
            bool retrievalNecessary;
            string destinationDir = null;
            var forkSourcePath = GetPathForFork(data, fork, deviceFileSystem, roms, romsConfiguration, ref destinationDir, out retrievalNecessary);
            if (retrievalNecessary)
            {
                if (!data.Device.RetrieveForkData(data, new[] { fork }, destinationDir, new[] { System.IO.Path.GetFileName(forkSourcePath) }))
                {
                    var syncErrors = data.Result as FileSystemSyncErrors;
                    syncErrors.UnableToRetrieveForks.Add(fork);
                    forkSourcePath = null;
                }
            }
            return forkSourcePath;
        }

        private static string GetPathForFork(ExecuteDeviceCommandAsyncTaskData data, Fork fork, FileSystem deviceFileSystem, IEnumerable<ProgramDescription> roms, RomListConfiguration romsConfiguration, ref string destinationDir, out bool retrievalNecessary)
        {
            retrievalNecessary = false;
            string forkPath = null;
            var forkFileKind = ProgramFileKind.None;
            var crc = 0u;
            var cfgCrc = 0u;
            var errors = data.Result as FileSystemSyncErrors;

            // Determine what kind of fork this is.
            ILfsFileInfo fileContainingFork = null;
            var forkKind = deviceFileSystem.GetForkKind(fork, out fileContainingFork);
            switch (forkKind)
            {
                case ForkKind.Program:
                    // Try to fetch LUIGI header from the fork.
                    forkFileKind = ProgramFileKind.LuigiFile;
                    forkPath = GetRomPathForForkFromRomList(data, fork, roms, romsConfiguration.RomsDirectory, out crc, out cfgCrc);
                    if (forkPath == null)
                    {
                        forkPath = GetRomPathForForkFromCache(fork, romsConfiguration.RomsDirectory);
                    }
                    if ((forkPath == null) && string.IsNullOrEmpty(destinationDir))
                    {
                        destinationDir = romsConfiguration.RomsDirectory;
                    }
                    break;
                case ForkKind.JlpFlash:
                    if (string.IsNullOrEmpty(destinationDir))
                    {
                        destinationDir = romsConfiguration.RomsDirectory; // seems sensible to keep save data file(s) next to the ROM
                    }
                    forkFileKind = ProgramFileKind.SaveData;
                    break;
                case ForkKind.Manual:
                    if (string.IsNullOrEmpty(destinationDir))
                    {
                        destinationDir = romsConfiguration.ManualsDirectory;
                    }
                    forkFileKind = ProgramFileKind.ManualText;
                    break;
                case ForkKind.Vignette:
                    if (string.IsNullOrEmpty(destinationDir))
                    {
                        destinationDir = Configuration.Instance.VignetteDataAreaPath; // keep next to ROM?
                    }
                    forkFileKind = ProgramFileKind.Vignette;
                    break;
                case ForkKind.Reserved4:
                case ForkKind.Reserved5:
                case ForkKind.Reserved6:
                    if (string.IsNullOrEmpty(destinationDir))
                    {
                        destinationDir = Configuration.Instance.ReservedDataAreaPath; // keep next to ROM?
                    }
                    forkFileKind = ProgramFileKind.GenericSupportFile;
                    errors.UnsupportedForks.Add(new Tuple<ILfsFileInfo, Fork>(fileContainingFork, fork));
                    ////throw new UnsupportedForkKindException(forkKind);
                    break;
                case ForkKind.None:
                    // An orphaned fork. Retrieve it, but we can't really do much with it.
                    if (string.IsNullOrEmpty(destinationDir))
                    {
                        destinationDir = Configuration.Instance.RecoveredDataAreaPath; // orphaned fork
                    }
                    forkFileKind = ProgramFileKind.None;
                    errors.OrphanedForks.Add(fork);
                    break;
                default:
                    throw new UnsupportedForkKindException(forkKind);
            }

            if ((destinationDir != null) && (forkPath == null))
            {
                retrievalNecessary = true;
                var forkFileBaseName = (fileContainingFork == null) ? Configuration.Instance.GetForkDataFileName(fork.GlobalForkNumber) : fileContainingFork.LongName.EnsureValidFileName();
                var extension = forkFileKind.FileExtension();

                // For the menu position fork, use the default extension; since it's in the manual fork slot, we want
                // to override the .txt extension.
                if (string.IsNullOrEmpty(extension) || (fork.Uid == Fork.MenuPositionForkUid))
                {
                    extension = Configuration.ForkExtension;
                }
                var forkFileName = System.IO.Path.ChangeExtension(forkFileBaseName, extension);
                var destFile = new StorageLocation(System.IO.Path.Combine(destinationDir, forkFileName));
                if (destFile.Exists())
                {
                    var existingCrc = 0u;
                    var existingCfgCrc = 0u;
                    var luigiHeader = LuigiFileHeader.GetHeader(destFile);
                    if ((luigiHeader != null) && (luigiHeader.Version > 0))
                    {
                        existingCrc = luigiHeader.OriginalRomCrc32;
                        existingCfgCrc = luigiHeader.OriginalCfgCrc32;
                    }
                    if (existingCrc == 0)
                    {
                        existingCrc = Crc32.OfFile(destFile);
                    }
                    if (existingCfgCrc == 0)
                    {
                        var destCfgFile = destFile.ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                        if (destCfgFile.Exists())
                        {
                            existingCfgCrc = Crc32.OfFile(destCfgFile);
                        }
                    }

                    // This is the equivalent of RomComparerStrict: We skip retrieval only if both the ROM CRCs match and, if available, the .cfg CRCs match.
                    if ((crc != 0) && (existingCrc == crc) && ((cfgCrc == 0) || (existingCfgCrc == cfgCrc)))
                    {
                        retrievalNecessary = false;
                        forkPath = destFile.Path;
                    }
                    else
                    {
                        forkPath = destFile.EnsureUnique().Path;
                    }
                }
                else
                {
                    forkPath = destFile.Path;
                }
            }
            if (!string.IsNullOrEmpty(forkPath) && !System.IO.File.Exists(forkPath))
            {
                retrievalNecessary = true;
                destinationDir = System.IO.Path.GetDirectoryName(forkPath);
            }

            return forkPath;
        }

        private static string GetRomPathForForkFromRomList(ExecuteDeviceCommandAsyncTaskData data, Fork fork, IEnumerable<ProgramDescription> roms, string destDir, out uint crc32, out uint cfgCrc32)
        {
            string romPath = null;
            crc32 = 0;
            cfgCrc32 = 0;
            var luigiHeader = GetLuigiHeaderForFork(data, fork);
            if ((luigiHeader != null) && (luigiHeader.Version > 0))
            {
                crc32 = luigiHeader.OriginalRomCrc32;
                cfgCrc32 = luigiHeader.OriginalCfgCrc32;
                var rom = roms.FirstOrDefault(r => (r.Crc == luigiHeader.OriginalRomCrc32) && ((luigiHeader.OriginalCfgCrc32 == 0) || (luigiHeader.OriginalCfgCrc32 == r.Rom.CfgCrc)));
                if (rom != null)
                {
                    romPath = rom.Rom.RomPath.Path;
                }
            }
            return romPath;
        }

        private static string GetRomPathForForkFromCache(Fork fork, string destDir)
        {
            string romPath = null;
            var entry = CacheIndex.Find(fork.Crc24, fork.Size);
            if (entry != null)
            {
                var destFile = System.IO.Path.Combine(destDir, System.IO.Path.GetFileName(entry.RomPath));
                if (System.IO.File.Exists(destFile))
                {
                    var crc = Crc32.OfFile(new StorageLocation(destFile));
                    if (crc == entry.RomCrc32)
                    {
                        romPath = destFile;
                    }
                    else
                    {
                        destFile = destFile.EnsureUniqueFileName();
                    }
                }
                if (romPath == null)
                {
                    var copySourceDir = System.IO.Path.GetDirectoryName(CacheIndex.Path);
                    var sourceFile = System.IO.Path.Combine(copySourceDir, entry.RomPath);
                    System.IO.File.Copy(sourceFile, destFile);
                    romPath = destFile;
                }
            }
            return romPath;
        }

        private static LuigiFileHeader GetLuigiHeaderForFork(ExecuteDeviceCommandAsyncTaskData data, Fork fork)
        {
            LuigiFileHeader luigiHeader = null;
            using (var memory = new System.IO.MemoryStream())
            {
                const uint Address = 0u;
                var bytesToRead = LuigiFileHeader.MaxHeaderSize;
                var succeeded = ReadForkToRam.Create(Address, fork.GlobalForkNumber, 0u, bytesToRead).Execute<bool>(data.Device.Port, data);
                byte[] dataRead = null;
                if (succeeded)
                {
                    dataRead = (byte[])DownloadDataBlockFromRam.Create(Address, bytesToRead).Execute(data.Device.Port, data, out succeeded);
                }
                if (succeeded)
                {
                    memory.Write(dataRead, 0, bytesToRead);
                    memory.Seek(0, System.IO.SeekOrigin.Begin);
                    try
                    {
                        luigiHeader = LuigiFileHeader.Inflate(memory);
                    }
                    catch (INTV.Core.UnexpectedFileTypeException)
                    {
                    }
                }
            }
            return luigiHeader;
        }

        private static void SyncFileData(FileNode localFile, ILfsFileInfo deviceFile, bool updateOnly, Dictionary<ushort, ushort> forkNumberMap, Dictionary<ushort, string> forkSourceFileMap, FileSystemSyncErrors syncErrors)
        {
            localFile.Color = deviceFile.Color;
            localFile.LongName = deviceFile.LongName;
            localFile.ShortName = deviceFile.ShortName;
            System.Diagnostics.Debug.Assert(localFile.GlobalFileNumber == deviceFile.GlobalFileNumber, "Need to figure out how to set GFN");
            System.Diagnostics.Debug.Assert(localFile.GlobalDirectoryNumber == deviceFile.GlobalDirectoryNumber, "Need to figure out how to set GDN");
            switch (deviceFile.FileType)
            {
                case FileType.File:
                    var program = (Program)localFile;
                    var forks = new ushort[(int)ForkKind.NumberOfForkKinds];
                    for (int i = 0; i < forks.Length; ++i)
                    {
                        ushort forkNumber;
                        if (!forkNumberMap.TryGetValue(deviceFile.ForkNumbers[i], out forkNumber))
                        {
                            forkNumber = GlobalForkTable.InvalidForkNumber;
                        }
                        forks[i] = forkNumber;
                    }
                    program.SetForks(deviceFile.ForkNumbers);
                    ProgramDescription description = null;
                    SyncForkData(localFile.Rom, ForkKind.Program, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    if (description != null)
                    {
                        program.Description = description;
                    }
                    SyncForkData(localFile.Manual, ForkKind.Manual, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    SyncForkData(localFile.JlpFlash, ForkKind.JlpFlash, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    SyncForkData(localFile.Vignette, ForkKind.Vignette, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    SyncForkData(localFile.ReservedFork4, ForkKind.Reserved4, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    SyncForkData(localFile.ReservedFork5, ForkKind.Reserved5, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    SyncForkData(localFile.ReservedFork6, ForkKind.Reserved6, forkSourceFileMap, (f, k) => ReportForkSyncError(f, k, localFile, syncErrors), ref description);
                    break;
                case FileType.Folder:
                    for (var i = 0; i < deviceFile.ForkNumbers.Length; ++i)
                    {
                        ((Folder)localFile).ForkNumbers[i] = deviceFile.ForkNumbers[i];
                    }
                    break;
                default:
                    throw new System.InvalidOperationException(Resources.Strings.FileSystem_InvalidFileType);
            }
        }

        private static bool SyncForkData(Fork fork, ForkKind forkKind, Dictionary<ushort, string> forkSourceFileMap, Action<Fork, ForkKind> errorAction, ref ProgramDescription description)
        {
            var succeeded = true;
            if (fork != null)
            {
                var crc = 0u;
                switch (forkKind)
                {
                    case ForkKind.Program:
                        var cfgCrc = 0u;
                        var cacheEntry = CacheIndex.Find(fork.Crc24, fork.Size);
                        if (cacheEntry != null)
                        {
                            crc = cacheEntry.RomCrc32;
                            cfgCrc = cacheEntry.CfgCrc32;
                        }
                        else
                        {
                            string romPath;
                            if (forkSourceFileMap.TryGetValue(fork.GlobalForkNumber, out romPath) && !string.IsNullOrEmpty(romPath) && System.IO.File.Exists(romPath))
                            {
                                if (LuigiFileHeader.PotentialLuigiFile(new StorageLocation(romPath)))
                                {
                                    var luigiHeader = LuigiFileHeader.GetHeader(new StorageLocation(romPath));
                                    if (luigiHeader.Version > 0)
                                    {
                                        crc = luigiHeader.OriginalRomCrc32;
                                        cfgCrc = luigiHeader.OriginalCfgCrc32;
                                    }
                                }
                                if (crc == 0u)
                                {
                                    crc = Crc32.OfFile(new StorageLocation(romPath));
                                }
                            }
                            else
                            {
                                System.Console.WriteLine("SDFS");
                            }
                        }
                        if (crc != 0)
                        {
                            var romListDescriptions = INTV.Shared.Model.Program.ProgramCollection.Roms.Where(d => d.Crc == crc);
                            if (romListDescriptions.Any())
                            {
                                var romListDescription = romListDescriptions.FirstOrDefault(d => (d.Rom != null) && (d.Rom.CfgCrc == cfgCrc));
                                if (romListDescription != null)
                                {
                                    description = romListDescription.Copy();
                                }
                            }
                        }
                        if (description == null)
                        {
                            var rom = fork.Rom;
                            if (rom == null)
                            {
                                string romPath;
                                succeeded = forkSourceFileMap.TryGetValue(fork.GlobalForkNumber, out romPath);
                                if (succeeded)
                                {
                                    rom = Rom.Create(new StorageLocation(romPath), StorageLocation.InvalidLocation);
                                    fork.Rom = rom;
                                    if (string.IsNullOrEmpty(fork.FilePath))
                                    {
                                        fork.FilePath = romPath;
                                    }
                                }
                            }
                            if (rom != null)
                            {
                                var programInfo = rom.GetProgramInformation();
                                description = new ProgramDescription(rom.Crc, rom, programInfo);
                            }
                        }
                        break;
                    case ForkKind.Manual:
                        string filePath;
                        if (forkSourceFileMap.TryGetValue(fork.GlobalForkNumber, out filePath) && System.IO.File.Exists(filePath))
                        {
                            fork.FilePath = filePath;
                            if (description != null)
                            {
                                description.Files.DefaultManualTextLocation = new StorageLocation(fork.FilePath);
                            }
                        }
                        break;
                    case ForkKind.JlpFlash:
                        // TODO / FIXME : We don't do anything with JLP save data forks when syncing from the file system.
                        break;
                    case ForkKind.Vignette:
                    case ForkKind.Reserved4:
                    case ForkKind.Reserved5:
                    case ForkKind.Reserved6:
                        succeeded = false;
                        break;
                }
            }
            if (!succeeded && (errorAction != null))
            {
                errorAction(fork, forkKind);
            }
            return succeeded;
        }

        private static void ReportForkSyncError(Fork fork, ForkKind forkKind, ILfsFileInfo localFile, FileSystemSyncErrors syncErrors)
        {
            System.Diagnostics.Debug.WriteLine("What to do with " + forkKind + " for file number " + localFile.GlobalFileNumber + "??");
            syncErrors.UnsupportedForks.Add(new Tuple<ILfsFileInfo, Fork>(localFile, fork));
        }

        #endregion // SyncDeviceToHost
    }
}

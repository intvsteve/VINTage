// <copyright file="DeviceHelpers.SyncHostToDevice.cs" company="INTV Funhouse">
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

#define USE_SMART_COPY

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Utility;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for SyncHostToDevice.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region SyncHostToDevice

        /// <summary>
        /// Downloads the host system's MenuLayout file system content to a Locutus device.
        /// </summary>
        /// <param name="device">The target Locutus device whose file system is to be updated to match that of the host PC.</param>
        /// <param name="hostMenuLayout">The host PC MenuLayout to push down to Locutus.</param>
        /// <param name="onCompleteHandler">Called upon successful completion of the operation. This argument may be <c>null</c>.</param>
        /// <param name="errorHandler">Error handler, used to report errors to the user.</param>
        public static void SyncHostToDevice(this Device device, MenuLayout hostMenuLayout, DeviceCommandCompleteHandler onCompleteHandler, DeviceCommandErrorHandler errorHandler)
        {
            if (device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Title = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_Title,
                    ProgressUpdateMode = ExecuteDeviceCommandProgressUpdateMode.Custom,

                    // First bool is whether to reset menu position data (true => REMOVE IT).
                    // Second bool is whether to update root file name (false => RETAIN IT).
                    Data = new Tuple<MenuLayout, bool, bool>(hostMenuLayout, true, false),
                    OnSuccess = onCompleteHandler,
                    OnFailure = errorHandler
                };
                executeCommandTaskData.StartTask(SyncHostToDevice, true, 1);
            }
        }

        /// <summary>
        /// Protocol commands necessary for SyncHostToDevice.
        /// </summary>
        internal static readonly IEnumerable<ProtocolCommandId> SyncHostToDeviceProtocolCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.LfsGetFileSystemStatusFlags,
            ProtocolCommandId.LfsSetFileSystemStatusFlags,
            ProtocolCommandId.LfsUploadDataBlockToRam,
            ProtocolCommandId.LfsChecksumDataBlockInRam,
            ProtocolCommandId.LfsDeleteDirectory,
            ProtocolCommandId.LfsDeleteFile,
            ProtocolCommandId.LfsDeleteFork,
            ProtocolCommandId.LfsCreateForkFromRam,
            ProtocolCommandId.LfsUpdateGftFromRam,
            ProtocolCommandId.LfsUpdateGdtFromRam,
            ProtocolCommandId.LfsDownloadGlobalTables
        };

        // NOTE: The specific task data (ExecuteDeviceCommandAsyncTaskData.Data) must be a Tuple<MenuLayout, bool, bool>.
        // The first Boolean value indicates whether the operation should attempt to force the removal of menu position data.
        // The second Boolean value indicates whether the operation should update the root file's names (device name, owner).
        private static void SyncHostToDevice(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            data.Task.UpdateTaskProgress(0, Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ComputingChangesProgress);
            var currentDirtyFlags = GetDirtyFlags.Instance.Execute<LfsDirtyFlags>(device.Port, data);
            var deviceFileSystem = DownloadFileSystemTables.Instance.Execute<FileSystem>(device.Port, data);
            deviceFileSystem.Status = currentDirtyFlags;
            if (data.AcceptCancelIfRequested())
            {
                return;
            }

            var hostData = (Tuple<MenuLayout, bool, bool>)data.Data;
            var hostFileSystem = hostData.Item1.FileSystem;
            var resetSaveMenuPosition = hostData.Item2;
            if (resetSaveMenuPosition)
            {
                hostFileSystem.ForceRemovalOfMenuPositionData(deviceFileSystem);
            }
            hostFileSystem.SuppressRootFileNameDifferences(device);
            hostFileSystem.PopulateSaveDataForksFromDevice(deviceFileSystem);

            // The first pass gathers all differences, including errors.
            var allDifferencesWithErrors = hostFileSystem.CompareTo(deviceFileSystem, device);

#if USE_SMART_COPY
            // Now, use a clone of the original file system to compute the actual work to do, and use the failures for a post-op error report.
            var hostFileSystemWorkingCopy = hostFileSystem.Clone();

            var ignoreRootFileNames = !hostData.Item3;
            if (ignoreRootFileNames)
            {
                hostFileSystemWorkingCopy.Files[GlobalFileTable.RootDirectoryFileNumber].ShortName = deviceFileSystem.Files[GlobalFileTable.RootDirectoryFileNumber].ShortName;
                hostFileSystemWorkingCopy.Files[GlobalFileTable.RootDirectoryFileNumber].LongName = deviceFileSystem.Files[GlobalFileTable.RootDirectoryFileNumber].LongName;
            }

            var partialErrors = hostFileSystemWorkingCopy.RemoveInvalidEntries(allDifferencesWithErrors, FileSystemHelpers.ShouldRemoveInvalidEntry, ShouldIncludeError);
            var allDifferences = hostFileSystemWorkingCopy.CompareTo(deviceFileSystem, device);
#else
            // This will try to copy all ROMs, even incompatible ones, and should eventually be removed.
            var hostFileSystemWorkingCopy = hostFileSystem;
            var partialErrors = allDifferencesWithErrors.GetAllFailures(ShouldIncludeError);
            var allDifferences = allDifferencesWithErrors;
#endif // USE_SMART_COPY

            var succeeded = true;
            var phaseNumber = 0;
            var updateString = string.Empty;

            // Run any delete operations...
            var total = allDifferences.DirectoryDifferences.ToDelete.Count + allDifferences.FileDifferences.ToDelete.Count + allDifferences.ForkDifferences.ToDelete.Count + allDifferences.ForkDifferences.ToUpdate.Count;
            var numComplete = 0;
            if (data.AcceptCancelIfRequested())
            {
                return;
            }
            if (total > 0)
            {
                ++phaseNumber;

                if (!data.CancelRequsted && succeeded && allDifferences.DirectoryDifferences.ToDelete.Any())
                {
                    var deleteOpData = new DeleteOperationData(data, LfsEntityType.Directory)
                    {
                        Factory = (gdn) => DeleteFolder.Create((byte)(uint)gdn[0]),
                        UpdateTitleFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat,
                        UpdateTitleInfo = Resources.Strings.LfsOperation_Remove,
                        UpdateProgressFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_DeletingItemProgressFormat,
                        UpdateProgressInfo = Resources.Strings.DirectoriesInSentence,
                        Phase = phaseNumber, Total = total, Current = numComplete
                    };
                    currentDirtyFlags |= DeleteEntries(allDifferences.DirectoryDifferences.ToDelete, deleteOpData, currentDirtyFlags, deviceFileSystem);
                    numComplete = deleteOpData.Current;
                    succeeded = data.Succeeded;
                }

                if (!data.CancelRequsted && succeeded && allDifferences.FileDifferences.ToDelete.Any())
                {
                    var deleteOpData = new DeleteOperationData(data, LfsEntityType.File)
                    {
                        Factory = (gfn) => DeleteFile.Create((ushort)(uint)gfn[0]),
                        UpdateTitleFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat,
                        UpdateTitleInfo = Resources.Strings.LfsOperation_Remove,
                        UpdateProgressFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_DeletingItemProgressFormat,
                        UpdateProgressInfo = Resources.Strings.FilesInSentence,
                        Phase = phaseNumber, Total = total, Current = numComplete
                    };
                    currentDirtyFlags |= DeleteEntries(allDifferences.FileDifferences.ToDelete, deleteOpData, currentDirtyFlags, deviceFileSystem);
                    numComplete = deleteOpData.Current;
                    succeeded = data.Succeeded;
                }

                if (!data.CancelRequsted && succeeded && (allDifferences.ForkDifferences.ToDelete.Any() || allDifferences.ForkDifferences.ToUpdate.Any()))
                {
                    var deleteOpData = new DeleteOperationData(data, LfsEntityType.Fork)
                    {
                        Factory = (gkn) => DeleteFork.Create((ushort)(uint)gkn[0]),
                        UpdateTitleFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat,
                        UpdateTitleInfo = Resources.Strings.LfsOperation_Remove,
                        UpdateProgressFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_DeletingItemProgressFormat,
                        UpdateProgressInfo = Resources.Strings.DeviceMultistageCommand_RemovingFileContents,
                        Phase = phaseNumber, Total = total, Current = numComplete
                    };
                    var forks = allDifferences.ForkDifferences.ToDelete.ToList();
                    var updatedForksToTreatAsDeletions = allDifferences.ForkDifferences.ToUpdate.Select(f => (uint)f.GlobalForkNumber).Where(f => !allDifferences.ForkDifferences.FailedOperations.Any(o => (o.Value.GlobalFileSystemNumber == f) && FileSystemHelpers.IsMissingForkError(o.Value)));
                    forks.AddRange(updatedForksToTreatAsDeletions);
                    currentDirtyFlags |= DeleteEntries(forks, deleteOpData, currentDirtyFlags, deviceFileSystem);
                    numComplete = deleteOpData.Current;
                    succeeded = data.Succeeded;
                }
            }

            if (data.AcceptCancelIfRequested())
            {
                return;
            }

            data.CurrentlyExecutingCommand = ProtocolCommandId.UnknownCommand;
            data.FailureMessage = Resources.Strings.SyncHostToDeviceCommand_GatherUpdatesFailedMessage;
            var updateOperations = hostFileSystemWorkingCopy.GatherAllUpdates(allDifferences, device.UniqueId);
            if (updateOperations.Any())
            {
                do
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }
                    LfsUpdateOperation operation = null;
                    data.FailureMessage = Resources.Strings.SyncHostToDevice_FetchUpdateOperationFailedMessage;
                    updateOperations = updateOperations.FetchUpdateOperation(out operation);
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    ++phaseNumber;
                    numComplete = 0;
                    total = operation.Forks.Count + operation.Files.Count + operation.Directories.Count;
                    foreach (var failure in operation.Failures)
                    {
                        partialErrors[failure.Description] = failure.Exception;
                    }

                    // Load forks.
                    var address = 0u;
                    if (!data.CancelRequsted && succeeded && operation.Forks.Any())
                    {
                        data.FailureMessage = null;
                        var uploadOpData = new UploadDataOperationData(data, address)
                        {
                            TargetType = LfsEntityType.Fork,
                            Factory = (upl) => UploadDataBlockToRam.Create((uint)upl[0], (ByteSerializer)upl[1], (uint)upl[2]),
                            GetSerializer = FileSystemHelpers.ToForkByteSerializer,
                            ShouldUpdateProgress = (k) => true,
                            UpdateTitleFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat,
                            UpdateTitleInfo = operation.ProgressTitle,
                            UpdateProgressFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_TransferToRamProgressFormat,
                            Phase = phaseNumber, Total = total, Current = numComplete
                        };
                        address = UploadEntries(operation.FileSystem.Forks, operation.Forks, uploadOpData);
                        numComplete = uploadOpData.Current;
                        succeeded = data.Succeeded;
                    }

                    // Load GFT update.
                    if (!data.CancelRequsted && succeeded && operation.Files.Any())
                    {
                        data.FailureMessage = null;
                        var fileRange = operation.GfnRange;
                        var files = Enumerable.Range(fileRange.Minimum, (fileRange.Maximum - fileRange.Minimum) + 1); // slower than a loop, but who cares?

                        var uploadOpData = new UploadDataOperationData(data, address)
                        {
                            TargetType = LfsEntityType.File,
                            Factory = (upl) => UploadDataBlockToRam.Create((uint)upl[0], (ByteSerializer)upl[1], (uint)upl[2]),
                            GetSerializer = FileSystemHelpers.ToFileByteSerializer,
                            ShouldUpdateProgress = (f) => (f != null) && operation.Files.Contains(((ILfsFileInfo)f).GlobalFileNumber),
                            UpdateTitleFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat,
                            UpdateTitleInfo = operation.ProgressTitle,
                            UpdateProgressFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_TransferToRamProgressFormat,
                            Phase = phaseNumber,
                            Total = total,
                            Current = numComplete
                        };

                        // THIS COULD BE DONE WITH ONE LARGE BLOCK INSTEAD OF MANY SMALLER ONES
                        address = UploadEntries(operation.FileSystem.Files, files, uploadOpData);
                        numComplete = uploadOpData.Current;
                        succeeded = data.Succeeded;
                    }

                    // Load GDT update.
                    if (!data.CancelRequsted && succeeded && operation.Directories.Any())
                    {
                        data.FailureMessage = null;
                        var directoryRange = operation.GdnRange;
                        var directories = Enumerable.Range(directoryRange.Minimum, (directoryRange.Maximum - directoryRange.Minimum) + 1); // slower than a loop, but who cares?

                        var uploadOpData = new UploadDataOperationData(data, address)
                        {
                            TargetType = LfsEntityType.Directory,
                            Factory = (upl) => UploadDataBlockToRam.Create((uint)upl[0], (ByteSerializer)upl[1], (uint)upl[2]),
                            GetSerializer = FileSystemHelpers.ToDirectoryByteSerializer,
                            ShouldUpdateProgress = (d) => (d != null) && operation.Files.Contains(((IDirectory)d).GlobalDirectoryNumber),
                            UpdateTitleFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat,
                            UpdateTitleInfo = operation.ProgressTitle,
                            UpdateProgressFormat = Resources.Strings.DeviceMultistageCommand_UpdatingFiles_TransferToRamProgressFormat,
                            Phase = phaseNumber,
                            Total = total,
                            Current = numComplete
                        };

                        // THIS COULD BE DONE WITH ONE LARGE BLOCK INSTEAD OF MANY SMALLER ONES
                        address = UploadEntries(operation.FileSystem.Directories, directories, uploadOpData);
                        numComplete = uploadOpData.Current;
                        succeeded = data.Succeeded;
                    }
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }

                    // Everything is in RAM now, so execute commands.
                    address = 0; // All the data was initially loaded at location zero.
                    numComplete = 0;
                    total = operation.Forks.Count;
                    if (operation.Files.Count > 0)
                    {
                        ++total;
                    }
                    if (operation.Directories.Count > 0)
                    {
                        ++total;
                    }

                    // Create forks.
                    if (!data.CancelRequsted && succeeded && operation.Forks.Any())
                    {
                        data.FailureMessage = null;
                        var numCreated = 0;
                        var numToCreate = operation.Forks.Count;
                        foreach (var gkn in operation.Forks)
                        {
                            if (data.AcceptCancelIfRequested())
                            {
                                break;
                            }
                            address = address.Align();
                            var fork = operation.FileSystem.Forks[gkn];
                            updateString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceMultistageCommand_UpdatingFiles_ProgressTitleFormat, operation.ProgressTitle, ++numComplete, total, phaseNumber);
                            data.Task.UpdateTaskTitle(updateString);
                            updateString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceMultistageCommand_UpdatingFiles_CreatingItemProgressFormat, fork.Name, ++numCreated, numToCreate);
                            data.Task.UpdateTaskProgress((double)numCreated / numToCreate, updateString);
                            currentDirtyFlags |= currentDirtyFlags.UpdateFileSystemDirtyState(deviceFileSystem, data, gkn, LfsOperations.Add, LfsEntityType.Fork);
                            CreateForkFromRam.Create(address, fork).Execute(device.Port, data, out succeeded);
                            address += (uint)fork.SerializeByteCount;
                        }
                        address = address.Align();
                    }

                    // Update GFT.
                    if (!data.CancelRequsted && succeeded && operation.Files.Any())
                    {
                        data.FailureMessage = null;
                        if (data.AcceptCancelIfRequested())
                        {
                            break;
                        }
                        var gftRange = operation.GfnRange;
                        if (operation.Files.Count == 1)
                        {
                            updateString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceMultistageCommand_UpdatingFileSystemTablesOneEntry_ProgressTitleFormat, Resources.Strings.File, gftRange.Minimum, phaseNumber);
                        }
                        else
                        {
                            updateString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceMultistageCommand_UpdatingFileSystemTables_ProgressTitleFormat, Resources.Strings.File, gftRange.Minimum, gftRange.Maximum, phaseNumber);
                        }
                        data.Task.UpdateTaskTitle(updateString);
                        data.Task.UpdateTaskProgress((double)(++numComplete) / total, Resources.Strings.DeviceMultistageCommand_UpdatingGlobalFilesTable);
                        currentDirtyFlags |= currentDirtyFlags.UpdateFileSystemDirtyState(deviceFileSystem, data, gftRange.Minimum, LfsOperations.Update, LfsEntityType.File);
                        UpdateGlobalFileTable.Create(address, gftRange).Execute(device.Port, data, out succeeded);
                        address += (uint)(gftRange.Maximum - gftRange.Minimum + 1) * LfsFileInfo.FlatSizeInBytes;
                    }

                    // Update GDT.
                    if (!data.CancelRequsted && succeeded && operation.Directories.Any())
                    {
                        data.FailureMessage = null;
                        if (data.AcceptCancelIfRequested())
                        {
                            break;
                        }
                        var gdtRange = operation.GdnRange;
                        if (operation.Directories.Count == 1)
                        {
                            updateString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceMultistageCommand_UpdatingFileSystemTablesOneEntry_ProgressTitleFormat, Resources.Strings.Directory, gdtRange.Minimum, phaseNumber);
                        }
                        else
                        {
                            updateString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceMultistageCommand_UpdatingFileSystemTables_ProgressTitleFormat, Resources.Strings.Directory, gdtRange.Minimum, gdtRange.Maximum, phaseNumber);
                        }
                        data.Task.UpdateTaskTitle(updateString);
                        data.Task.UpdateTaskProgress((double)(++numComplete) / total, Resources.Strings.DeviceMultistageCommand_UpdatingGlobalDirectoriesTable);
                        currentDirtyFlags |= currentDirtyFlags.UpdateFileSystemDirtyState(deviceFileSystem, data, gdtRange.Minimum, LfsOperations.Update, LfsEntityType.Directory);
                        UpdateGlobalDirectoryTable.Create(address, gdtRange).Execute(device.Port, data, out succeeded);
                        address += (uint)(gdtRange.Maximum - gdtRange.Minimum + 1) * Directory.FlatSizeInBytes;
                    }
                }
                while (!data.CancelRequsted && succeeded && updateOperations.Any());
            }

            if (!data.CancelRequsted && data.Succeeded)
            {
                SetDirtyFlags.Create(LfsDirtyFlags.None).Execute(device.Port, data, out succeeded);
                deviceFileSystem.Status = LfsDirtyFlags.None;
            }

            if (!data.CancelRequsted && data.Succeeded)
            {
                var updatedFileSystem = DownloadFileSystemTables.Instance.Execute(device.Port, data, out succeeded) as FileSystem;
#if USE_SMART_COPY
                partialErrors = allDifferences.CombineAllFailures(partialErrors, ShouldIncludeError);
#endif // USE_SMART_COPY
                data.Result = new Tuple<FileSystem, IDictionary<string, FailedOperationException>>(updatedFileSystem, partialErrors);
            }
        }

#if DEBUG
        /// <summary>When set to <c>true</c>, incompatible ROMs encountered during copy-to-Locutus operations will be reported via the error dialog.</summary>
        private static bool _reportIncompatibleRoms = false;
#endif

        private static bool ShouldIncludeError(Exception error)
        {
            var includeError = !(error is IncompatibleRomException);
#if DEBUG
            includeError |= _reportIncompatibleRoms;
#endif // DEBUG
            return includeError;
        }

        private static LfsDirtyFlags DeleteEntries<T>(IList<T> entries, DeleteOperationData deleteOpData, LfsDirtyFlags currentDirtyFlags, FileSystem deviceFileSystem)
        {
            var data = deleteOpData.TaskData;
            var device = data.Device;
            var numDeleted = 0;
            var numToDelete = entries.Count;
            var succeeded = false;
            foreach (var entry in entries)
            {
                if (data.AcceptCancelIfRequested())
                {
                    break;
                }
                deleteOpData.UpdateTitle();
                deleteOpData.UpdateStatus(numToDelete, ref numDeleted);
                currentDirtyFlags |= currentDirtyFlags.UpdateFileSystemDirtyState(deviceFileSystem, data, Convert.ToUInt32(entry), deleteOpData.Operation, deleteOpData.TargetType);
                deleteOpData.CreateCommand(entry).Execute(device.Port, data, out succeeded);
            }
            return currentDirtyFlags;
        }

        private static uint UploadEntries<TFileSystemEntry, TFileTableIndex>(FixedSizeCollection<TFileSystemEntry> fileSystemTable, IEnumerable<TFileTableIndex> entryIndexes, UploadDataOperationData uploadOpData)
            where TFileSystemEntry : class, IGlobalFileSystemEntry
        {
            var data = uploadOpData.TaskData;
            var device = data.Device;
            var crc = 0u; // TODO : ELIMINATE THIS
            var numUploaded = 0;
            var numToUpload = entryIndexes.Count();
            var succeeded = false;
            foreach (var entryIndex in entryIndexes)
            {
                if (data.AcceptCancelIfRequested())
                {
                    break;
                }
                var entry = fileSystemTable[Convert.ToInt32(entryIndex)];
                if (uploadOpData.ShouldUpdateProgress(entry))
                {
                    uploadOpData.UpdateTitle();
                    uploadOpData.UpdateStatus(entry.Name, numToUpload, ref numUploaded);
                }
                var serializableEntry = uploadOpData.GetSerializer(entry);
                uploadOpData.CreateCommand(uploadOpData.Address, serializableEntry, crc).Execute(device.Port, data, out succeeded);
                uploadOpData.Address += (uint)serializableEntry.SerializeByteCount;
            }

            return uploadOpData.Address;
        }

        /// <summary>
        /// If the operation should update the file system's dirty flags, and hasn't done so already, do so.
        /// </summary>
        /// <param name="currentDirtyFlags">The current dirty flags on the file system.</param>
        /// <param name="fileSystem">The file system being updated.</param>
        /// <param name="taskData">Task data for displaying updates, tracking success, etc.</param>
        /// <param name="globalFileSystemNumber">If an error occurs, the entity type and its global file system number value are used to report the error.</param>
        /// <param name="operation">The kind of file system operation being done.</param>
        /// <param name="targetType">The kind of file system entity involved in the operation.</param>
        /// <returns>The new dirty flags.</returns>
        private static LfsDirtyFlags UpdateFileSystemDirtyState(this LfsDirtyFlags currentDirtyFlags, FileSystem fileSystem, ExecuteDeviceCommandAsyncTaskData taskData, uint globalFileSystemNumber, LfsOperations operation, LfsEntityType targetType)
        {
            if (taskData.Succeeded && !currentDirtyFlags.HasFlag(LfsDirtyFlags.FileSystemUpdateInProgress))
            {
                currentDirtyFlags |= LfsDirtyFlags.FileSystemUpdateInProgress;
                taskData.Succeeded = SetDirtyFlags.Create(currentDirtyFlags).Execute<bool>(taskData.Device.Port, taskData);
            }
            if (!taskData.Succeeded && currentDirtyFlags.HasFlag(LfsDirtyFlags.FileSystemUpdateInProgress))
            {
                var errorFormatString = Resources.Strings.FileSystem_InconsistencyError_Format;
                if (operation == LfsOperations.Remove)
                {
                    errorFormatString = Resources.Strings.FileSystem_InconsistencyError_Deleting_Format;
                }
                else if (operation == LfsOperations.Add)
                {
                    errorFormatString = Resources.Strings.FileSystem_InconsistencyError_Creating_Format;
                }
                else if (operation == LfsOperations.Update)
                {
                    errorFormatString = Resources.Strings.FileSystem_InconsistencyError_Updating_Format;
                }
                throw new InconsistentFileSystemException(targetType, globalFileSystemNumber, errorFormatString);
            }
            fileSystem.Status = currentDirtyFlags;
            return currentDirtyFlags;
        }

        /// <summary>
        /// Consolidates data used in file system update operations.
        /// </summary>
        private abstract class FileSystemOperationData
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of FileSystemOperationData.
            /// </summary>
            /// <param name="operation">The kind of operation being performed.</param>
            /// <param name="taskData">Task data used for error reporting, et. al.</param>
            protected FileSystemOperationData(LfsOperations operation, ExecuteDeviceCommandAsyncTaskData taskData)
            {
                Operation = operation;
                TaskData = taskData;
            }

            #endregion

            /// <summary>
            /// Gets the underlying asynchronous task data.
            /// </summary>
            public ExecuteDeviceCommandAsyncTaskData TaskData { get; private set; }

            /// <summary>
            /// Gets the type of operation being performed.
            /// </summary>
            public LfsOperations Operation { get; private set; }

            /// <summary>
            /// Gets or sets the type of entity the operation is performed upon.
            /// </summary>
            public LfsEntityType TargetType { get; set; }

            /// <summary>
            /// Gets or sets the function used to create the command to execute.
            /// </summary>
            public Func<object[], ProtocolCommand> Factory { get; set; }

            public int Phase { get; set; }

            public int Total { get; set; }

            public int Current { get; set; }

            /// <summary>
            /// Gets or sets the format string used to update the progress bar's title.
            /// </summary>
            public string UpdateTitleFormat { get; set; }

            /// <summary>
            /// Gets or sets the target type of the operation, used to update the progress text.
            /// </summary>
            public string UpdateTitleInfo { get; set; }

            /// <summary>
            /// Gets or sets the format string used to update progress bar's detail information.
            /// </summary>
            public string UpdateProgressFormat { get; set; }

            /// <summary>
            /// Gets or sets additional string information used in the progress text.
            /// </summary>
            public string UpdateProgressInfo { get; set; }

            /// <summary>
            /// Creates the command to execute.
            /// </summary>
            /// <param name="args">Arguments needed to create the command.</param>
            /// <returns>The command to be executed.</returns>
            public virtual ProtocolCommand CreateCommand(params object[] args)
            {
                return Factory(args);
            }

            /// <summary>
            /// Update the title in the progress bar.
            /// </summary>
            public void UpdateTitle()
            {
                if (TaskData != null)
                {
                    var newTitle = string.Format(UpdateTitleFormat, UpdateTitleInfo, ++Current, Total, Phase);
                    TaskData.UpdateTaskTitle(newTitle);
                }
            }

            /// <summary>
            /// Update the detailed progress text.
            /// </summary>
            /// <param name="total">Total number of entries in the sub-operation.</param>
            /// <param name="numComplete">Current number of completed entries, incremented prior to updating progress.</param>
            public void UpdateStatus(int total, ref int numComplete)
            {
                if (TaskData != null)
                {
                    var newStatus = string.Format(System.Globalization.CultureInfo.CurrentCulture, UpdateProgressFormat, UpdateProgressInfo, ++numComplete, total);
                    TaskData.UpdateTaskProgress((double)numComplete / total, newStatus);
                }
            }

            public void UpdateStatus(string progressDetail, int total, ref int numComplete)
            {
                if (TaskData != null)
                {
                    var newStatus = string.Format(System.Globalization.CultureInfo.CurrentCulture, UpdateProgressFormat, progressDetail, ++numComplete, total);
                    TaskData.UpdateTaskProgress((double)numComplete / total, newStatus);
                }
            }
        }

        /// <summary>
        /// This class delivers data for file system delete operations.
        /// </summary>
        private class DeleteOperationData : FileSystemOperationData
        {
            /// <summary>
            /// Initializes a new instance of DeleteOperationData.
            /// </summary>
            /// <param name="taskData">Task data for error reporting.</param>
            /// <param name="targetType">The entry type being deleted.</param>
            public DeleteOperationData(ExecuteDeviceCommandAsyncTaskData taskData, LfsEntityType targetType)
                : base(LfsOperations.Remove, taskData)
            {
                TargetType = targetType;
            }
        }

        private class UploadDataOperationData : FileSystemOperationData
        {
            public UploadDataOperationData(ExecuteDeviceCommandAsyncTaskData taskData, uint address)
                : base(LfsOperations.None, taskData)
            {
                Address = address;
            }

            public uint Address
            {
                get
                {
                    return _address.Align();
                }

                set
                {
                    _address = value;
                }
            }
            private uint _address;

            /// <summary>
            /// Gets or sets the ByteSerializer to use for update operations.
            /// </summary>
            public Func<IGlobalFileSystemEntry, ByteSerializer> GetSerializer { get; set; }

            /// <summary>
            /// Gets or sets a function to call to indicate whether progress should be updated.
            /// </summary>
            public Func<IGlobalFileSystemEntry, bool> ShouldUpdateProgress { get; set; }
        }

        #endregion // SyncHostToDevice
    }
}

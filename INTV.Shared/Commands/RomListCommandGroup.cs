// <copyright file="RomListCommandGroup.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Commands for operating on ROMs and the ROM list.
    /// </summary>
    public partial class RomListCommandGroup : CommandGroup
    {
        /// <summary>
        /// The single instance of this command group.
        /// </summary>
        /// <remarks>Could this be done via MEF?</remarks>
        internal static readonly RomListCommandGroup Group = new RomListCommandGroup();

        private const string UniqueNameBase = "INTV.Shared.Commands.RomListCommandGroup";

        private RomListCommandGroup()
            : base(UniqueNameBase, Resources.Strings.RomListCommandGroup_Name, 0.15)
        {
            TabName = Resources.Strings.HomeRibbonTabCommand_Name;
        }

        /// <summary>
        /// Gets the ViewModel.
        /// </summary>
        /// <remarks>TODO: THIS IS UNUSED IN WINDOWS -- IS IT NEEDED ON MAC?</remarks>
        private static RomListViewModel RomListViewModel
        {
            get
            {
                if ((_romListViewModel == null) && (SingleInstanceApplication.Instance != null))
                {
                    _romListViewModel = INTV.Shared.ComponentModel.CompositionHelpers.Container.GetExportedValueOrDefault<RomListViewModel>();
                }
                return _romListViewModel;
            }
        }
        private static RomListViewModel _romListViewModel;

        #region RomListGroupCommand

        /// <summary>
        /// Group pseudo-command for ROM list-related commands.
        /// </summary>
        public static readonly VisualRelayCommand RomListGroupCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RomListGroupCommand",
            Name = Resources.Strings.RomListCommandGroup_Name,
            MenuItemName = Resources.Strings.RomListGroupCommand_MenuItemName,
            LargeIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/rom_32xMD.png"),
            ////SmallIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/.png"),
            Weight = 0.199
        };

        #endregion // RomListGroupCommand

        #region BackupRomListCommand

        /// <summary>
        /// Command to create a backup of the ROM list.
        /// </summary>
        public static readonly VisualRelayCommand BackupRomListCommand = new VisualRelayCommand(OnBackupRomList, CanBackupRomList)
        {
            UniqueId = UniqueNameBase + ".BackupRomListCommand",
            Name = Resources.Strings.BackupRomListCommand_Name,
            MenuItemName = Resources.Strings.BackupRomListCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.BackupRomListCommand_ToolTipDescription,
            LargeIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/backup_32xLG.png"),
            SmallIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/backup_16xLG.png"),
            Weight = 0.1,
            MenuParent = RomListGroupCommand,
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static void OnBackupRomList(object parameter)
        {
            var configuration = INTV.Shared.Model.RomListConfiguration.Instance;
            var backupDirectory = configuration.GetROMListBackupDirectory();
            if (!System.IO.Directory.Exists(backupDirectory))
            {
                System.IO.Directory.CreateDirectory(backupDirectory);
            }
            string backupRomListPath = null;
            if (System.IO.File.Exists(configuration.RomFilesPath))
            {
                backupRomListPath = System.IO.Path.Combine(backupDirectory, configuration.DefaultRomsFileName);
                System.IO.File.Copy(configuration.RomFilesPath, backupRomListPath);
            }
            var message = Resources.Strings.BackupRomListCommand_Complete_Message;
            var title = Resources.Strings.BackupRomListCommand_Complete_Title;
            var dialog = INTV.Shared.View.ReportDialog.Create(title, message);
            dialog.TextWrapping = OSTextWrapping.Wrap;
            dialog.ShowSendEmailButton = false;
            dialog.ReportText = backupDirectory;
            if (!string.IsNullOrEmpty(backupRomListPath))
            {
                dialog.Attachments.Add(backupRomListPath);
            }
            dialog.ShowDialog(Resources.Strings.OKButton_Text);
        }

        private static bool CanBackupRomList(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            return viewModel != null;
        }

        #endregion // BackupRomListCommand

        #region RestoreRomListCommand

        /// <summary>
        /// Command to restore the ROM list from a backup.
        /// </summary>
        public static readonly VisualRelayCommand RestoreRomListCommand = new VisualRelayCommand(OnRestoreRomList, CanRestoreRomList)
        {
            UniqueId = UniqueNameBase + ".RestoreRomListCommand",
            Name = Resources.Strings.RestoreRomListCommand_Name,
            MenuItemName = Resources.Strings.RestoreRomListCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.RestoreRomListCommand_ToolTipDescription,
            LargeIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/restore_32xLG.png"),
            SmallIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/restore_16xLG.png"),
            Weight = 0.13,
            MenuParent = RomListGroupCommand,
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static void OnRestoreRomList(object parameter)
        {
            var configuration = INTV.Shared.Model.RomListConfiguration.Instance;
            var message = Resources.Strings.RestoreRomListCommand_Message;
            var title = Resources.Strings.RestoreRomListCommand_MessageTitle;
            var doRestore = OSMessageBox.Show(message, title, OSMessageBoxButton.YesNo, OSMessageBoxIcon.Exclamation);
            if (doRestore == OSMessageBoxResult.Yes)
            {
                var backupDirectory = configuration.BackupDataDirectory;
                var backupFileName = INTV.Shared.Model.RomListConfiguration.Instance.DefaultRomsFileName;
                var selectBackupDialog = SelectBackupDialog.Create(backupDirectory, backupFileName, null, false);
                selectBackupDialog.Title = Resources.Strings.RestoreRomListCommand_SelectBackupTitle;
                var doRestoreResult = selectBackupDialog.ShowDialog();
                if (doRestoreResult == true)
                {
                    var romsConfiguration = INTV.Shared.Model.RomListConfiguration.Instance;
                    var backupRomListFile = System.IO.Path.Combine(selectBackupDialog.SelectedBackupDirectory, romsConfiguration.DefaultRomsFileName);
                    var romsFileExists = System.IO.File.Exists(backupRomListFile);
                    if (romsFileExists)
                    {
                        var restoreRomListTask = new AsyncTaskWithProgress("RestoreRomList", false, true, 0);
                        restoreRomListTask.UpdateTaskTitle(Resources.Strings.RestoreRomListCommand_MessageTitle);
                        var restoreMenuLayoutTaskData = new RestoreRomListTaskData(restoreRomListTask, backupRomListFile, parameter as RomListViewModel);
                        restoreRomListTask.RunTask(restoreMenuLayoutTaskData, RestoreRomList, RestoreRomListComplete);
                    }
                    else
                    {
                        var errorMessage = new System.Text.StringBuilder(Resources.Strings.RestoreRomListCommand_MissingFileErrorMessage).AppendLine().AppendLine();
                        errorMessage.AppendLine(selectBackupDialog.SelectedBackupDirectory).AppendLine();
                        if (!romsFileExists)
                        {
                            errorMessage.AppendLine(romsConfiguration.DefaultRomsFileName);
                        }
                        message = errorMessage.ToString();
                        title = Resources.Strings.RestoreRomListCommand_MissingFileErrorTitle;
                        OSMessageBox.Show(message, title, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                    }
                }
            }
        }

        private static bool CanRestoreRomList(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            return viewModel != null;
        }

        private class RestoreRomListTaskData : AsyncTaskData
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.Shared.Commands.RomListCommandGroup+RestoreRomListTaskData"/> class.
            /// </summary>
            /// <param name="task">The asynchronous task to execute the action.</param>
            /// <param name="fileToRestore">The ROM list backup file to restore.</param>
            /// <param name="romListViewModel">The ViewModel to inform of the restoration when complete.</param>
            internal RestoreRomListTaskData(AsyncTaskWithProgress task, string fileToRestore, RomListViewModel romListViewModel)
                : base(task)
            {
                FileToRestore = fileToRestore;
                RomListViewModel = romListViewModel;
            }

            /// <summary>
            /// Gets the absolute path to the ROM list file to restore.
            /// </summary>
            internal string FileToRestore { get; private set; }

            /// <summary>
            /// Gets or sets the backup path to use in case a problem occurs when saving.
            /// </summary>
            internal string BackupPath { get; set; }

            /// <summary>
            /// Gets the ViewModel to restore the ROM list to.
            /// </summary>
            internal RomListViewModel RomListViewModel { get; private set; }
        }

        private static void RestoreRomList(AsyncTaskData taskData)
        {
            var data = (RestoreRomListTaskData)taskData;
            var romsConfiguration = INTV.Shared.Model.RomListConfiguration.Instance;
            data.BackupPath = romsConfiguration.RomFilesPath.GetUniqueBackupFilePath();
            System.IO.File.Copy(romsConfiguration.RomFilesPath, data.BackupPath); // back up the current file
            data.RomListViewModel.InitializeRomList(data.FileToRestore);
            data.RomListViewModel.SaveRomList(false);
        }

        private static void RestoreRomListComplete(AsyncTaskData taskData)
        {
            var data = (RestoreRomListTaskData)taskData;
            if (taskData.Error == null)
            {
                RestoreRomListComplete(data.RomListViewModel);
                if (!string.IsNullOrEmpty(data.BackupPath) && System.IO.File.Exists(data.BackupPath))
                {
                    FileUtilities.DeleteFile(data.BackupPath, false, 10);
                }
            }
            else
            {
                var title = Resources.Strings.RestoreRomListCommand_MissingFileErrorTitle;
                var message = Resources.Strings.RestoreRomListCommand_FailedMessage;
                var buttons = OSMessageBoxButton.OK;
                if (!string.IsNullOrEmpty(data.BackupPath) && System.IO.File.Exists(data.BackupPath))
                {
                    message += Resources.Strings.RestoreRomListCommand_PromptToRestoreMessage;
                    buttons = OSMessageBoxButton.YesNo;
                }
                var result = OSMessageBox.Show(message, title, taskData.Error, buttons, OSMessageBoxIcon.Error);
                if ((result == OSMessageBoxResult.Yes) && !string.IsNullOrEmpty(data.BackupPath) && System.IO.File.Exists(data.BackupPath))
                {
                    var romsConfiguration = INTV.Shared.Model.RomListConfiguration.Instance;
                    System.IO.File.Replace(data.BackupPath, romsConfiguration.RomFilesPath, null);
                    data.RomListViewModel.InitializeRomList(romsConfiguration.RomFilesPath);
                }
            }
        }

        /// <summary>
        /// Platform-specific code to execute when restoration of a ROM list completes.
        /// </summary>
        /// <param name="viewModel">The ROM list ViewModel.</param>
        static partial void RestoreRomListComplete(RomListViewModel viewModel);

        #endregion // RestoreRomListCommand

        #region EmptyRomListCommand

        /// <summary>
        /// Command to replace the ROM list with an empty one.
        /// </summary>
        public static readonly VisualRelayCommand EmptyRomListCommand = new VisualRelayCommand(OnEmptyRomList, CanEmptyRomList)
        {
            UniqueId = UniqueNameBase + ".EmptyRomListCommand",
            Name = Resources.Strings.EmptyRomListCommand_Name,
            MenuItemName = Resources.Strings.EmptyRomListCommand_MenuItemName,
            ContextMenuItemName = Resources.Strings.EmptyRomListCommand_ContextMenuItemName,
            ToolTipDescription = Resources.Strings.EmptyRomListCommand_ToolTipDescription,
            Weight = 0.14,
            MenuParent = RomListGroupCommand,
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static void OnEmptyRomList(object parameter)
        {
            var message = Resources.Strings.EmptyRomListCommand_WarningMessage;
            var title = Resources.Strings.EmptyRomListCommand_WarningTitle;
            if (OSMessageBox.Show(message, title, OSMessageBoxButton.YesNo, OSMessageBoxIcon.Exclamation) == OSMessageBoxResult.Yes)
            {
                var viewModel = parameter as RomListViewModel;
                viewModel.ClearRomList();
                viewModel.SaveRomList(true);
            }
        }

        private static bool CanEmptyRomList(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            return (viewModel != null) && (viewModel.Model != null) && viewModel.Model.Any();
        }

        #endregion // EmptyRomListCommand

        #region AddRomFilesCommand

        /// <summary>
        /// Command to add ROM files to the ROM list.
        /// </summary>
        public static readonly VisualRelayCommand AddRomFilesCommand = new VisualRelayCommand(AddRomFiles)
        {
            UniqueId = UniqueNameBase + ".AddRomFilesCommand",
            Name = Resources.Strings.AddRomFilesCommand_Name,
            MenuItemName = Resources.Strings.AddRomFilesCommand_MenuItemName,
            ToolTip = Resources.Strings.AddRomFilesCommand_ToolTipDescription,
            ToolTipTitle = Resources.Strings.AddRomFilesCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.AddRomFilesCommand_ToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.09,
            LargeIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/add_file_32xLG.png"),
            KeyboardShortcutKey = "o",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
            PreferredParameterType = typeof(RomListViewModel)
        };

        /// <summary>
        /// Adds all the ROMs from a directory.
        /// </summary>
        /// <param name="parameter">The command parameter, which must be a RomListViewModel.</param>
        internal static void AddRomFiles(object parameter)
        {
            var romListViewModel = (RomListViewModel)parameter;
            romListViewModel.AddRoms("false");
        }

        #endregion // AddRomFilesCommand

        #region AddRomFoldersCommand

        /// <summary>
        /// Command to add ROMs to the ROM list by recursively visiting directories on disk.
        /// </summary>
        public static readonly VisualRelayCommand AddRomFoldersCommand = new VisualRelayCommand(AddRomFolders)
        {
            UniqueId = UniqueNameBase + ".AddRomFoldersCommand",
            Name = Resources.Strings.AddRomFoldersCommand_Name,
            ToolTip = Resources.Strings.AddRomFoldersCommand_ToolTipDescription,
            ToolTipTitle = Resources.Strings.AddRomFoldersCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.AddRomFoldersCommand_ToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.1,
            LargeIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/add_files_32xLG.png"),
            KeyboardShortcutKey = "O",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
            PreferredParameterType = typeof(RomListViewModel)
        };

        /// <summary>
        /// Adds all the ROMs that can be discovered in a directory, recursing into the directory's contents.
        /// </summary>
        /// <param name="parameter">The command parameter, which must be a RomListViewModel.</param>
        private static void AddRomFolders(object parameter)
        {
            var romListViewModel = parameter as RomListViewModel;
            if (romListViewModel != null)
            {
                romListViewModel.AddRoms("true");
            }
            else if ((RomListViewModel != null) && (parameter is IEnumerable<string>))
            {
                RomListViewModel.AddRoms(parameter as IEnumerable<string>, true);
            }
        }

        #endregion // AddRomFoldersCommand

        #region RemoveRomsCommand

        /// <summary>
        /// Command to remove ROMs from the ROM list.
        /// </summary>
        public static readonly VisualRelayCommand RemoveRomsCommand = new VisualRelayCommand(RemoveRoms, CanRemoveRoms)
        {
            UniqueId = UniqueNameBase + ".RemoveRomsCommand",
            Name = Resources.Strings.RemoveRomsCommand_Name,
            MenuItemName = Resources.Strings.RemoveRomsCommand_MenuItemName,
            ContextMenuItemName = Resources.Strings.RemoveRomsCommand_ContextMenuItemName,
            ToolTip = Resources.Strings.RemoveRomsCommand_ToolTipDescription,
            ToolTipTitle = Resources.Strings.RemoveRomsCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.RemoveRomsCommand_ToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.12,
            LargeIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "Resources/Images/remove_file_32xLG.png"),
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/remove_file_16xLG.png"),
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static void RemoveRoms(object parameter)
        {
            var romListViewModel = (RomListViewModel)parameter;
            romListViewModel.RemoveRoms(null);
        }

        private static bool CanRemoveRoms(object parameter)
        {
            bool canRemoveRoms = false;
            if (parameter != null)
            {
                canRemoveRoms = ((RomListViewModel)parameter).CurrentSelection.Count > 0;
            }
            return canRemoveRoms;
        }

        #endregion // RemoveRomsCommand

        #region RefreshRomsCommand

        /// <summary>
        /// Command to refresh the list of ROMs by revisiting the cached list of ROM directories .
        /// </summary>
        public static readonly VisualRelayCommand RefreshRomsCommand = new VisualRelayCommand(RefreshRomList, CanRefreshRomList)
        {
            UniqueId = UniqueNameBase + ".RefreshRomsCommand",
            Name = Resources.Strings.RefreshRomsCommand_Name,
            ToolTip = Resources.Strings.RefreshRomsCommand_ToolTipDescription,
            ToolTipTitle = Resources.Strings.RefreshRomsCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.RefreshRomsCommand_ToolTipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.13,
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/romsearch_16xLG.png"),
            LargeIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/romsearch_32xMD.png"),
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static void RefreshRomList(object parameter)
        {
            var romListViewModel = (RomListViewModel)parameter;
            romListViewModel.RefreshRoms(null);
        }

        private static bool CanRefreshRomList(object parameter)
        {
            var canRefresh = false;
            if (parameter != null)
            {
                var romListViewModel = (RomListViewModel)parameter;
                canRefresh = romListViewModel.CanRefreshRoms(null);
            }
            return canRefresh;
        }

        #endregion // RefreshRomsCommand

        #region ValidateRomsCommand

        /// <summary>
        /// Command to validate the existing ROMs in the ROM list.
        /// </summary>
        public static readonly VisualRelayCommand ValidateRomsCommand = new VisualRelayCommand(ValidateRoms)
        {
            UniqueId = UniqueNameBase + ".ValidateRomsCommand",
            Name = Resources.Strings.ValidateRomsCommand_Name,
            ToolTip = Resources.Strings.ValidateRomsCommand_ToolTip,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.129,
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/refresh_16xLG.png"),
            LargeIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/refresh_32xMD.png"),
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static void ValidateRoms(object parameter)
        {
            var romListViewModel = parameter as RomListViewModel;
            if (romListViewModel != null)
            {
                var validateRomsTask = new AsyncTaskWithProgress("ValidateRoms", false, false);
                validateRomsTask.UpdateTaskTitle(Resources.Strings.ValidateRomsCommand_ProgressTitle);
                var validateRomsTaskData = new ValidateRomsTaskData(validateRomsTask, romListViewModel);
                validateRomsTask.RunTask(validateRomsTaskData, ValidateRoms, ValidateRomsComplete);
            }
        }

        private static void ValidateRoms(AsyncTaskData taskData)
        {
            var data = (ValidateRomsTaskData)taskData;
            var numUpdated = 0;
            foreach (var rom in RomListViewModel.Programs)
            {
                taskData.UpdateTaskProgress((++numUpdated / RomListViewModel.Programs.Count) * 100, string.Format(Resources.Strings.ValidateRomsCommand_ProgressDetailFormat, rom.Name, rom.Model.Rom.RomPath));
                if (rom.RefreshFileStatus(RomListViewModel.AttachedPeripherals))
                {
                    data.UpdatedRoms.Add(rom.Model);
                }
            }
        }

        private static void ValidateRomsComplete(AsyncTaskData taskData)
        {
            var data = (ValidateRomsTaskData)taskData;
            ValidateRomsComplete();
            if (data.UpdatedRoms.Any())
            {
                data.RomListViewModel.Model.ReportProgramStatusChanged(data.UpdatedRoms);
            }
        }

        /// <summary>
        /// Called when ROM list validation finishes to perform any platform-specific work.
        /// </summary>
        static partial void ValidateRomsComplete();

        private class ValidateRomsTaskData : AsyncTaskData
        {
            /// <summary>
            /// Initializes a new instance of ValidateRomsTaskData.
            /// </summary>
            /// <param name="task">The task that executes, using an instance of this type as data.</param>
            /// <param name="romListViewModel">The ROM list ViewModel being refreshed.</param>
            internal ValidateRomsTaskData(AsyncTaskWithProgress task, RomListViewModel romListViewModel)
                : base(task)
            {
                RomListViewModel = romListViewModel;
                UpdatedRoms = new List<INTV.Core.Model.Program.ProgramDescription>();
            }

            /// <summary>
            /// Gets the ROMs that were updated as part of the validation operation.
            /// </summary>
            internal IList<INTV.Core.Model.Program.ProgramDescription> UpdatedRoms { get; private set; }

            /// <summary>
            /// Gets the ROM list ViewModel being updated.
            /// </summary>
            internal RomListViewModel RomListViewModel { get; private set; }
        }

        #endregion // ValidateRomsCommand

        #region CancelRomsImportCommand

        /// <summary>
        /// The command to execute to cancel a ROM import operation. Typically this becomes available if AddRomFoldersCommand is running and takes awhile.
        /// </summary>
        public static readonly VisualRelayCommand CancelRomsImportCommand = new VisualRelayCommand(OnCancelRomsImport, OnCanCancelRomsImport)
        {
            UniqueId = UniqueNameBase + ".CancelRomsImportCommand",
            Name = Resources.Strings.ProgressIndicator_CancelButtonText,
            BlockWhenAppIsBusy = false,
            PreferredParameterType = typeof(ProgressIndicatorViewModel)
        };

        private static void OnCancelRomsImport(object parameter)
        {
            var viewModel = (ProgressIndicatorViewModel)parameter;
            viewModel.OnCancel(parameter);
        }

        private static bool OnCanCancelRomsImport(object parameter)
        {
            var canCancel = false;
            var viewModel = parameter as ProgressIndicatorViewModel;
            if (viewModel != null)
            {
                canCancel = viewModel.AllowsCancel;
            }
            return canCancel;
        }

        #endregion // CancelRomsImportCommand

        #region ShowInFileSystemCommand

        /// <summary>
        /// Command to show a file in the file system.
        /// </summary>
        public static readonly VisualRelayCommand ShowInFileSystemCommand = new VisualRelayCommand(OnShowInFileSystem, CanShowInFileSystem)
        {
            UniqueId = UniqueNameBase + ".ShowInFileSystemCommand",
            Name = Resources.Strings.ShowInFileSystemCommand_Name,
            ToolTipTitle = Resources.Strings.ShowInFileSystemCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.ShowInFileSystemCommand_ToolTipDescription,
            Weight = 0.2
        };

        private static void OnShowInFileSystem(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var item = (viewModel == null) || !viewModel.CurrentSelection.Any() ? null : viewModel.CurrentSelection.First();
            if ((item != null) && System.IO.File.Exists(item.Rom.RomPath))
            {
                item.Rom.RomPath.RevealInFileSystem();
            }
        }

        private static bool CanShowInFileSystem(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var item = (viewModel == null) || !viewModel.CurrentSelection.Any() ? null : viewModel.CurrentSelection.First();
            return (viewModel != null) && (item != null) && System.IO.File.Exists(item.Rom.RomPath) && (viewModel.CurrentSelection.Count == 1);
        }

        #endregion // ShowInFileSystemCommand

        #region ShowRomInfoCommand

        /// <summary>
        /// Command to show ROM details in the UI.
        /// </summary>
        public static readonly VisualRelayCommand ShowRomInfoCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ShowRomInfoCommand",
            Name = Resources.Strings.ShowRomInfoCommand_Name,
            MenuItemName = Resources.Strings.ShowRomInfoCommand_MenuItemName,
            ToolTipTitle = Resources.Strings.ShowRomInfoCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.ShowRomInfoCommand_ToolTipDescription,
            SmallIcon = typeof(RomListCommandGroup).LoadImageResource("ViewModel/Resources/Images/intvfunhouse_info_16xLG.png")
        };

        #endregion ShowRomInfoCommand

        #region EditProgramNameCommand

        /// <summary>
        /// Command to initiate the process of editing the name of a ROM in the ROM list.
        /// </summary>
        /// <remarks>The implementation of the execute method, OnEditProgramName, is platform-specific.</remarks>
        public static readonly VisualRelayCommand EditProgramNameCommand = new VisualRelayCommand(OnEditProgramName, CanEditProgramName)
        {
            UniqueId = UniqueNameBase + ".EditProgramNameCommand",
            Name = Resources.Strings.EditProgramNameCommand_Name,
            MenuItemName = Resources.Strings.EditProgramNameCommand_MenuItemName,
            ContextMenuItemName = Resources.Strings.EditProgramNameCommand_ContextMenuItemName,
            Weight = 0.10,
            SmallIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/rename_16xLG.png"),
            ToolTipTitle = Resources.Strings.EditProgramNameCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.EditProgramNameCommand_ToolTipDescription,
            PreferredParameterType = typeof(RomListViewModel)
        };

        private static bool CanEditProgramName(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var canEdit = (viewModel != null) && viewModel.CanEditElements && (viewModel.CurrentSelection.Count == 1);
            return canEdit;
        }

        #endregion // EditProgramNameCommand

        #region EditRomFeaturesCommand

        /// <summary>
        /// Command to invoke the ROM features editor.
        /// </summary>
        public static readonly VisualRelayCommand EditRomFeaturesCommand = new VisualRelayCommand(OnEditRomFeatures, CanEditRomFeatures)
        {
            UniqueId = UniqueNameBase + ".EditRomFeaturesCommand",
            Name = Resources.Strings.EditRomFeaturesCommand_Name,
            MenuItemName = Resources.Strings.EditRomFeaturesCommand_MenuItemName,
            PreferredParameterType = typeof(RomListViewModel),
            Weight = 0.11
        };

        private static bool CanEditRomFeatures(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var itemToEdit = (viewModel != null) ? viewModel.CurrentSelection.FirstOrDefault() : null;
            return (itemToEdit != null) && (itemToEdit.Rom != null) && itemToEdit.Rom.Validate() && (viewModel.CurrentSelection.Count == 1);
        }

        #endregion // EditRomFeaturesCommand

        #region RevertToDatabaseFeaturesCommand

        /// <summary>
        /// Command to undo changes made to ROM features to match the database's default values.
        /// </summary>
        public static readonly VisualRelayCommand RevertToDatabaseFeaturesCommand = new VisualRelayCommand(OnRevertToDatabaseFeatures)
        {
            UniqueId = UniqueNameBase + ".RevertToDatabaseFeaturesCommand",
            Name = Resources.Strings.ProgramFeaturesEditor_RevertToDatabase,
            ToolTip = Resources.Strings.ProgramFeaturesEditor_RevertToDatabase,
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/refresh_16xLG.png"),
            PreferredParameterType = typeof(RomFeaturesConfigurationViewModel)
        };

        private static void OnRevertToDatabaseFeatures(object parameter)
        {
            var viewModel = parameter as RomFeaturesConfigurationViewModel;
            if (viewModel != null)
            {
                viewModel.RevertToDatabase();
            }
        }

        private static bool CanRevertToDatabaseFeaturesCommand(object parameter)
        {
            var viewModel = parameter as RomFeaturesConfigurationViewModel;
            var canRevertToDatabaseFeatures = (viewModel != null) && viewModel.ShowRevertToDefault;
            return canRevertToDatabaseFeatures;
        }

        #endregion // RevertToDatabaseFeaturesCommand

        #region RevertProgramFeaturesCommand

        /// <summary>
        /// Command to revert changes made while editing a ROM's features back to the values at the beginning of the edit session.
        /// </summary>
        public static readonly VisualRelayCommand RevertProgramFeaturesCommand = new VisualRelayCommand(OnRevertProgramFeatures)
        {
            UniqueId = UniqueNameBase + ".ResetProgramFeaturesCommand",
            Name = Resources.Strings.ProgramFeaturesEditor_Revert,
            ToolTip = Resources.Strings.ProgramFeaturesEditor_Revert,
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/undo_16x.png"),
            PreferredParameterType = typeof(RomFeaturesConfigurationViewModel)
        };

        private static void OnRevertProgramFeatures(object parameter)
        {
            var viewModel = parameter as RomFeaturesConfigurationViewModel;
            if (viewModel != null)
            {
                viewModel.RevertChanges();
            }
        }

        #endregion // RevertProgramFeaturesCommand

        #region UpdateProgramFeaturesCommand

        /// <summary>
        /// Command to update a ROMs features, committing them to the locally stored ROM list.
        /// </summary>
        public static readonly VisualRelayCommand UpdateProgramFeaturesCommand = new VisualRelayCommand(OnUpdateProgramFeatures)
        {
            UniqueId = UniqueNameBase + ".UpdateProgramFeaturesCommand",
            Name = Resources.Strings.ProgramFeaturesEditor_Commit,
            ToolTip = Resources.Strings.ProgramFeaturesEditor_Commit,
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/ok_16xLG.png"),
            PreferredParameterType = typeof(RomFeaturesConfigurationViewModel)
        };

        private static void OnUpdateProgramFeatures(object parameter)
        {
            var viewModel = parameter as RomFeaturesConfigurationViewModel;
            if (viewModel != null)
            {
                viewModel.InPlaceEditor.CommitEdit();
            }
        }

        #endregion // UpdateProgramFeaturesCommand

        #region CancelUpdateProgramFeaturesCommand

        /// <summary>
        /// Command to cancel a ROMs feature edit session.
        /// </summary>
        public static readonly VisualRelayCommand CancelUpdateProgramFeaturesCommand = new VisualRelayCommand(OnCancelUpdateProgramFeatures)
        {
            UniqueId = UniqueNameBase + ".CancelUpdateProgramFeaturesCommand",
            Name = Resources.Strings.ProgramFeaturesEditor_Cancel,
            ToolTip = Resources.Strings.ProgramFeaturesEditor_Cancel,
            SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/cancel_16xLG.png"),
            PreferredParameterType = typeof(RomFeaturesConfigurationViewModel)
        };

        private static void OnCancelUpdateProgramFeatures(object parameter)
        {
            var viewModel = parameter as RomFeaturesConfigurationViewModel;
            if (viewModel != null)
            {
                viewModel.InPlaceEditor.CancelEdit();
            }
        }

        #endregion // CancelUpdateProgramFeaturesCommand

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            // A NULL target is allowed for the case of an empty list.
            if (((target is ProgramDescriptionViewModel) || (target == null)) && (context is RomListViewModel))
            {
                yield return CreateContextMenuCommand(target, EditProgramNameCommand.CreateSeparator(CommandLocation.Before), context);
                yield return CreateContextMenuCommand(target, EditProgramNameCommand, context);
                yield return CreateContextMenuCommand(target, EditRomFeaturesCommand, context);
                yield return CreateContextMenuCommand(target, RemoveRomsCommand, context);
                yield return CreateContextMenuCommand(target, EmptyRomListCommand, context);
                yield return CreateContextMenuCommand(target, ShowInFileSystemCommand.CreateSeparator(CommandLocation.Before), context);
                yield return CreateContextMenuCommand(target, AddRomFilesCommand, context, null, ShowInFileSystemCommand.Weight);
                yield return CreateContextMenuCommand(target, AddRomFoldersCommand, context, null, ShowInFileSystemCommand.Weight);
                yield return CreateContextMenuCommand(target, ShowInFileSystemCommand, context);
            }
        }

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(RomListGroupCommand);
            CommandList.Add(AddRomFilesCommand);
            CommandList.Add(AddRomFoldersCommand);
            CommandList.Add(RemoveRomsCommand);
            CommandList.Add(ValidateRomsCommand);
            CommandList.Add(RefreshRomsCommand);
            CommandList.Add(CancelRomsImportCommand);
            CommandList.Add(ShowRomInfoCommand);
            CommandList.Add(EditProgramNameCommand);
            CommandList.Add(BackupRomListCommand);
            CommandList.Add(RestoreRomListCommand);
            CommandList.Add(EmptyRomListCommand);
            AddPlatformCommands();
        }

        #endregion // CommandGroup

        /// <summary>
        /// Add platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();
    }
}

// <copyright file="MenuLayoutCommandGroup.cs" company="INTV Funhouse">
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
using System.Globalization;
using INTV.Core.Model.Program;
using INTV.Core.Model.Stic;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Menu layout commands.
    /// </summary>
    public partial class MenuLayoutCommandGroup : CommandGroup
    {
        /// <summary>
        /// The group.
        /// </summary>
        internal static readonly MenuLayoutCommandGroup Group = new MenuLayoutCommandGroup();

        private const string UniqueNameBase = "INTV.LtoFlash.Commands.MenuLayoutCommandGroup";

        private MenuLayoutCommandGroup()
            : base(UniqueNameBase)
        {
        }

        #region MenuLayoutGroupCommand

        /// <summary>
        /// Pseudo-command for grouping menu layout commands.
        /// </summary>
        public static readonly VisualRelayCommand MenuLayoutGroupCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".MenuLayoutGroupCommand",
            Name = Resources.Strings.MenuLayout_Title,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/menu_layout_32xMD.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/menu_layout_16xMD.png"),
            Weight = 0.2
        };

        #endregion // MenuLayoutGroupCommand

        #region BackupMenuLayoutCommand

        /// <summary>
        /// The command to make a backup of the current menu layout (along with ROM list.)
        /// </summary>
        public static readonly VisualRelayCommand BackupMenuLayoutCommand = new VisualRelayCommand(OnBackupMenuLayout, CanBackupMenuLayout)
        {
            UniqueId = UniqueNameBase + ".BackupMenuLayoutCommand",
            Name = Resources.Strings.BackupMenuLayoutCommand_Name,
            MenuItemName = Resources.Strings.BackupMenuLayoutCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.BackupMenuLayoutCommand_TipDescription,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/backup_32xLG.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/backup_16xLG.png"),
            Weight = 0.1,
            MenuParent = MenuLayoutGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnBackupMenuLayout(object parameter)
        {
            var configuration = INTV.LtoFlash.Model.Configuration.Instance;
            var backupDirectory = configuration.GetMenuLayoutBackupDirectory();
            var romsConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.Shared.Model.RomListConfiguration>();
            if (!System.IO.Directory.Exists(backupDirectory))
            {
                System.IO.Directory.CreateDirectory(backupDirectory);
            }
            var backupFiles = new List<string>();
            if (System.IO.File.Exists(configuration.MenuLayoutPath))
            {
                var backupMenuLayoutPath = System.IO.Path.Combine(backupDirectory, configuration.DefaultMenuLayoutFileName);
                System.IO.File.Copy(configuration.MenuLayoutPath, backupMenuLayoutPath);
                backupFiles.Add(backupMenuLayoutPath);
            }
            if (System.IO.File.Exists(romsConfiguration.RomFilesPath))
            {
                var backupRomListPath = System.IO.Path.Combine(backupDirectory, romsConfiguration.DefaultRomsFileName);
                System.IO.File.Copy(romsConfiguration.RomFilesPath, backupRomListPath);
                backupFiles.Add(backupRomListPath);
            }
            var message = Resources.Strings.BackupMenuLayoutCommand_Message;
            var title = Resources.Strings.BackupMenuLayoutCommand_Title;
            var dialog = ReportDialog.Create(title, message);
            dialog.TextWrapping = OSTextWrapping.Wrap;
            dialog.ShowSendEmailButton = false;
            dialog.ReportText = backupDirectory;
            foreach (var backupFile in backupFiles)
            {
                dialog.Attachments.Add(backupFile);
            }
            dialog.ShowDialog(Resources.Strings.OK);
        }

        private static bool CanBackupMenuLayout(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel != null;
        }

        #endregion // BackupMenuLayoutCommand

        #region RestoreMenuLayoutCommand

        /// <summary>
        /// The command to restore a menu layout from a backup copy.
        /// </summary>
        public static readonly VisualRelayCommand RestoreMenuLayoutCommand = new VisualRelayCommand(OnRestoreMenuLayout, CanRestoreMenuLayout)
        {
            UniqueId = UniqueNameBase + ".RestoreMenuLayoutCommand",
            Name = Resources.Strings.RestoreMenuLayoutCommand_Name,
            MenuItemName = Resources.Strings.RestoreMenuLayoutCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.RestoreMenuLayoutCommand_TipDescription,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/restore_32xLG.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/restore_16xLG.png"),
            Weight = 0.2,
            MenuParent = MenuLayoutGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnRestoreMenuLayout(object parameter)
        {
            var configuration = INTV.LtoFlash.Model.Configuration.Instance;
            var message = Resources.Strings.RestoreMenuLayoutCommand_WarningMessage;
            var title = Resources.Strings.RestoreMenuLayoutCommand_WarningTitle;
            var doRestore = OSMessageBox.Show(message, title, OSMessageBoxButton.YesNo, OSMessageBoxIcon.Exclamation);
            if (doRestore == OSMessageBoxResult.Yes)
            {
                var backupDirectory = configuration.MenuLayoutBackupDataAreaPath;
                var backupFileName = INTV.LtoFlash.Model.Configuration.Instance.DefaultMenuLayoutFileName;
                var selectBackupDialog = INTV.Shared.View.SelectBackupDialog.Create(backupDirectory, backupFileName, null, false);
                selectBackupDialog.Title = Resources.Strings.SelectBackupDialog_Title;
                var doRestoreResult = selectBackupDialog.ShowDialog();
                if (doRestoreResult == true)
                {
                    var romsConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.Shared.Model.RomListConfiguration>();
                    var backupRomListFile = System.IO.Path.Combine(selectBackupDialog.SelectedBackupDirectory, romsConfiguration.DefaultRomsFileName);
                    var backupMenuLayoutFile = System.IO.Path.Combine(selectBackupDialog.SelectedBackupDirectory, configuration.DefaultMenuLayoutFileName);
                    var romsFileExists = System.IO.File.Exists(backupRomListFile);
                    var menuLayoutFileExists = System.IO.File.Exists(backupMenuLayoutFile);
                    if (romsFileExists && menuLayoutFileExists)
                    {
                        var restoreMenuLayoutTask = new AsyncTaskWithProgress("RestoreMenuLayout", false, true, 0.0);
                        restoreMenuLayoutTask.UpdateTaskTitle(Resources.Strings.RestoreMenuLayoutCommand_WarningTitle);
                        var restoreMenuLayoutTaskData = new RestoreMenuLayoutTaskData(restoreMenuLayoutTask, backupMenuLayoutFile, backupRomListFile, parameter as LtoFlashViewModel);
                        restoreMenuLayoutTask.RunTask(restoreMenuLayoutTaskData, RestoreMenuLayout, RestoreMenuLayoutComplete);
                    }
                    else
                    {
                        var errorMessage = new System.Text.StringBuilder(Resources.Strings.RestoreMenuLayoutCommand_MissingFilesErrorMessage).AppendLine().AppendLine();
                        errorMessage.AppendLine(selectBackupDialog.SelectedBackupDirectory).AppendLine();
                        if (!romsFileExists)
                        {
                            errorMessage.AppendLine(romsConfiguration.DefaultRomsFileName);
                        }
                        if (!menuLayoutFileExists)
                        {
                            errorMessage.AppendLine(configuration.DefaultMenuLayoutFileName);
                        }
                        message = errorMessage.ToString();
                        title = Resources.Strings.RestoreMenuLayoutCommand_MissingFilesErrorTitle;
                        OSMessageBox.Show(message, title, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                    }
                }
            }
        }

        private static bool CanRestoreMenuLayout(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel != null;
        }

        private class RestoreMenuLayoutTaskData : AsyncTaskData
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.LtoFlash.Commands.MenuLayoutCommandGroup+RestoreMenuLayoutTaskData"/> class.
            /// </summary>
            /// <param name="task">The asynchronous task to execute the action.</param>
            /// <param name="menuBackupFile">The menu backup file to restore.</param>
            /// <param name="romListBackupFile">The ROM list backup file to restore.</param>
            /// <param name="ltoFlashViewModel">The ViewModel to inform of the restoration when complete.</param>
            internal RestoreMenuLayoutTaskData(AsyncTaskWithProgress task, string menuBackupFile, string romListBackupFile, LtoFlashViewModel ltoFlashViewModel)
                : base(task)
            {
                MenuLayoutPath = menuBackupFile;
                RomFilesPath = romListBackupFile;
                LtoFlashViewModel = ltoFlashViewModel;
            }

            /// <summary>
            /// Gets the absolute path to the menu layout file to restore.
            /// </summary>
            internal string MenuLayoutPath { get; private set; }

            /// <summary>
            /// Gets the absolute path to the ROM list file to restore.
            /// </summary>
            internal string RomFilesPath { get; private set; }

            /// <summary>
            /// Gets the ViewModel to restore a menu layout to.
            /// </summary>
            internal LtoFlashViewModel LtoFlashViewModel { get; private set; }

            /// <summary>
            /// Gets or sets the new menu layout that was restored.
            /// </summary>
            internal MenuLayout NewMenuLayout { get; set; }
        }

        private static void RestoreMenuLayout(AsyncTaskData taskData)
        {
            var data = (RestoreMenuLayoutTaskData)taskData;
            INTV.Shared.Model.Program.ProgramCollection.InitializeFromFile(data.RomFilesPath);
            var newMenuLayout = INTV.LtoFlash.Model.MenuLayout.Load(data.MenuLayoutPath);
            data.NewMenuLayout = newMenuLayout;
        }

        private static void RestoreMenuLayoutComplete(AsyncTaskData taskData)
        {
            if (taskData.Error == null)
            {
                var data = (RestoreMenuLayoutTaskData)taskData;
                data.LtoFlashViewModel.HostPCMenuLayout.MenuLayout = data.NewMenuLayout;
                var roms = SingleInstanceApplication.Instance.Roms;
                var romsConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.Shared.Model.RomListConfiguration>();
                roms.Save(romsConfiguration.RomFilesPath, true); // this may throw an error, which should be handled by the RomListViewModel
                RestoreMenuLayoutComplete(data.LtoFlashViewModel);
            }
            else
            {
                var title = Resources.Strings.RestoreMenuLayoutCommand_MissingFilesErrorTitle;
                OSMessageBox.Show(Resources.Strings.RestoreMenuLayoutCommand_ErrorMessage, title, taskData.Error, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Platform-specific code to execute when menu restoration finishes.
        /// </summary>
        /// <param name="viewModel">The view model to use for the completion method.</param>
        static partial void RestoreMenuLayoutComplete(LtoFlashViewModel viewModel);

        #endregion // RestoreMenuLayoutCommand

        #region EmptyMenuLayoutCommand

        /// <summary>
        /// The command to empty the menu layout.
        /// </summary>
        public static readonly VisualRelayCommand EmptyMenuLayoutCommand = new VisualRelayCommand(OnEmptyMenuLayout, CanEmptyMenuLayout)
        {
            UniqueId = UniqueNameBase + ".EmptyMenuLayoutCommand",
            Name = Resources.Strings.EmptyMenuLayoutCommand_Name,
            MenuItemName = Resources.Strings.EmptyMenuLayoutCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.EmptyMenuLayoutCommand_TipDescription,
            Weight = 0.23,
            MenuParent = MenuLayoutGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnEmptyMenuLayout(object parameter)
        {
            var message = Resources.Strings.EmptyMenuLayoutCommand_WarningMessage;
            var title = Resources.Strings.EmptyMenuLayoutCommand_WarningTitle;
            if (OSMessageBox.Show(message, title, OSMessageBoxButton.YesNo, OSMessageBoxIcon.Exclamation) == OSMessageBoxResult.Yes)
            {
                var ltoFlashViewModel = parameter as LtoFlashViewModel;
                ltoFlashViewModel.HostPCMenuLayout.MenuLayout = new INTV.LtoFlash.Model.MenuLayout();
            }
        }

        private static bool CanEmptyMenuLayout(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel != null;
        }

        #endregion // EmptyMenuLayoutCommand

        #region OpenMenuLayoutBackupsDirectoryCommand

        /// <summary>
        /// The command to show the menu layout backups directory in the file system.
        /// </summary>
        public static readonly VisualRelayCommand OpenMenuLayoutBackupsDirectoryCommand = new VisualRelayCommand(OnOpenMenuLayoutBackupsDirectory, CanOpenMenuLayoutBackupsDirectory)
        {
            UniqueId = UniqueNameBase + ".OpenMenuLayoutBackupsDirectoryCommand",
            Name = Resources.Strings.OpenMenuLayoutBackupsDirectoryCommand_Name,
            ToolTipDescription = Resources.Strings.OpenDeviceBackupsDirectoryCommand_TipDescription,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/folder_closed_32xMD.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/folder_closed_16xMD.png"),
            Weight = 0.4,
            MenuParent = MenuLayoutGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };
        
        private static void OnOpenMenuLayoutBackupsDirectory(object parameter)
        {
            var configuration = INTV.LtoFlash.Model.Configuration.Instance;
            if (System.IO.Directory.Exists(configuration.MenuLayoutBackupDataAreaPath))
            {
                try
                {
                    RunExternalProgram.OpenInDefaultProgram(configuration.MenuLayoutBackupDataAreaPath);
                }
                catch (InvalidOperationException)
                {
                    // Silently fail.
                }
            }
        }

        private static bool CanOpenMenuLayoutBackupsDirectory(object parameter)
        {
            if (_checkForBackupDir)
            {
                var configuration = INTV.LtoFlash.Model.Configuration.Instance;
                _checkForBackupDir = !System.IO.Directory.Exists(configuration.MenuLayoutBackupDataAreaPath);
            }
            return !_checkForBackupDir;
        }
        private static bool _checkForBackupDir = true;

        #endregion // OpenMenuLayoutBackupsDirectoryCommand

        #region NewDirectoryCommand

        /// <summary>
        /// The command to create a new directory in a MenuLayout.
        /// </summary>
        public static readonly VisualRelayCommand NewDirectoryCommand = new VisualRelayCommand(OnNewDirectory, CanCreateNewDirectory)
        {
            UniqueId = UniqueNameBase + ".NewDirectoryCommand",
            Name = Resources.Strings.NewDirectoryCommand_Name,
            ToolTipTitle = Resources.Strings.NewDirectoryCommand_ToolTipTitle,
            ToolTip = Resources.Strings.NewDirectoryCommand_ToolTipDescription,
            ToolTipDescription = Resources.Strings.NewDirectoryCommand_ToolTipDescription,
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/new_folder.png"),
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/new_folder_32xMD.png"),
            Weight = 0.201,
            KeyboardShortcutKey = "D",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void OnNewDirectory(object parameter)
        {
            var viewModel = (MenuLayoutViewModel)parameter;
            viewModel.NewFolder();
            OSDispatcher.Current.BeginInvoke(() =>
                {
                    if (EditLongNameCommand.CanExecute(parameter))
                    {
                        EditLongNameCommand.Execute(parameter);
                    }
                });
        }

        private static bool CanCreateNewDirectory(object parameter)
        {
            var viewModel = parameter as MenuLayoutViewModel;
            var canCreateFolder = viewModel != null;
            if (canCreateFolder)
            {
                canCreateFolder = viewModel.CanCreateNewDirectory();
            }
            return canCreateFolder;
        }

        #endregion // NewDirectoryCommand

        #region AddRomsToMenuCommand

        /// <summary>
        /// The command to add selected ROMs in the ROM list to a MenuLayout.
        /// </summary>
        public static readonly VisualRelayCommand AddRomsToMenuCommand = new VisualRelayCommand(OnAddItems, CanAddItems)
        {
            UniqueId = UniqueNameBase + ".AddRomsToMenuCommand",
            Name = Resources.Strings.AddRomsToMenuCommand_Name,
            ToolTip = Resources.Strings.AddRomsToMenuCommand_Tip,
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/addtomenu_32x.png"),
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/addtomenu_16x.png"),
            Weight = 0.21,
            PreferredParameterType = typeof(LtoFlashViewModel)
        };

        private static void OnAddItems(object parameter)
        {
            var viewModel = parameter as LtoFlashViewModel;
            viewModel.HostPCMenuLayout.AddSelectedItems(viewModel.CurrentSelection);
        }

        private static bool CanAddItems(object parameter)
        {
            var canAddItems = false;
            var viewModel = parameter as LtoFlashViewModel;
            if (viewModel != null)
            {
                canAddItems = viewModel.HostPCMenuLayout.CanAddSelectedItems(viewModel.CurrentSelection);
            }
            return canAddItems;
        }

        #endregion // AddRomsToMenuCommand

        #region DeleteItemsCommand

        /// <summary>
        /// The command to delete selected items from the menu.
        /// </summary>
        public static readonly VisualRelayCommand DeleteItemsCommand = new VisualRelayCommand(OnDeleteItems, CanDeleteItems)
        {
            UniqueId = UniqueNameBase + ".DeleteItemsCommand",
            Name = Resources.Strings.RemoveFromMenuCommand_Name,
            MenuItemName = Resources.Strings.RemoveFromMenuCommand_MenuItemName,
            ToolTipTitle = Resources.Strings.RemoveFromMenuCommand_TipTitle,
            ToolTip = Resources.Strings.RemoveFromMenuCommand_TipDescription,
            ToolTipDescription = Resources.Strings.RemoveFromMenuCommand_ToolTipDescription,
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/remove_16xLG.png"),
            LargeIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/remove_32xLG.png"),
            Weight = 0.215,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void OnDeleteItems(object parameter)
        {
            var viewModel = (MenuLayoutViewModel)parameter;
            viewModel.DeleteItems();
        }

        private static bool CanDeleteItems(object parameter)
        {
            var viewModel = parameter as MenuLayoutViewModel;
            bool canDeleteItems = viewModel != null;
            if (canDeleteItems)
            {
                canDeleteItems = viewModel.CanDeleteItems();
            }
            return canDeleteItems;
        }

        #endregion // DeleteItemsCommand

        #region EditLongNameCommand

        /// <summary>
        /// The command to edit the long name of a menu item.
        /// </summary>
        /// <remarks>The execute method is platform-specific.</remarks>
        public static readonly VisualRelayCommand EditLongNameCommand = new VisualRelayCommand(OnEditLongName, CanEditLongName)
        {
            UniqueId = UniqueNameBase + ".EditLongNameCommand",
            Name = Resources.Strings.EditLongNameCommand_Name,
            Weight = 0.24,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static bool CanEditLongName(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            bool canEdit = (menuLayoutViewModel != null) && (menuLayoutViewModel.CurrentSelection != null);
            if (canEdit && menuLayoutViewModel.LtoFlashViewModel.ActiveLtoFlashDevice.IsValid)
            {
                canEdit = !menuLayoutViewModel.LtoFlashViewModel.ActiveLtoFlashDevice.Device.IsConnectedToIntellivision;
            }
            return canEdit;
        }

        #endregion // EditLongNameCommand

        #region EditShortNameCommand

        /// <summary>
        /// The command to edit the short name of a menu item.
        /// </summary>
        /// <remarks>The execute method is platform-specific.</remarks>
        public static readonly VisualRelayCommand EditShortNameCommand = new VisualRelayCommand(OnEditShortName, CanEditShortName)
        {
            UniqueId = UniqueNameBase + ".EditShortNameCommand",
            Name = Resources.Strings.EditShortNameCommand_Name,
            Weight = 0.245,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static bool CanEditShortName(object parameter)
        {
            return CanEditLongName(parameter);
        }

        #endregion // EditShortNameCommand

        #region SetColorCommand

        /// <summary>
        /// The command to set the color of the currently selected menu items.
        /// </summary>
        public static readonly VisualRelayCommand SetColorCommand = new VisualRelayCommand(OnSetColor, CanSetColor)
        {
            UniqueId = UniqueNameBase + ".SetColorCommand",
            Name = Resources.Strings.SetColorCommand_Name,
            ToolTip = Resources.Strings.SetColorCommand_ToolTipDescription,
            ToolTipDescription = Resources.Strings.SetColorCommand_ToolTipDescription,
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/color_palette_16xLG.png"),
            Weight = 0.25,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void OnSetColor(object parameter)
        {
            OSSetColor(parameter);
        }

        private static bool CanSetColor(object parameter)
        {
            return CanEditLongName(parameter);
        }

        private static void SetFileColor(object parameter)
        {
            var data = parameter as Tuple<MenuLayoutViewModel, FileNodeViewModel, Color>;
            var fileNode = data.Item2;
            var color = data.Item3;
            fileNode.Color = FileNodeColorViewModel.GetColor(color);
        }

        private static bool CanSetFileColor(object parameter)
        {
            var data = parameter as Tuple<MenuLayoutViewModel, FileNodeViewModel, Color>;
            var canEdit = (data != null) && (data.Item1 != null) && (data.Item2 != null);
            return canEdit;
        }

        /// <summary>
        /// Platform-specific method to set color.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        static partial void OSSetColor(object parameter);

        #endregion // SetColorCommand

        #region SetManualCommand

        /// <summary>
        /// The command to associate a manual file with the selected menu items.
        /// </summary>
        public static readonly VisualRelayCommand SetManualCommand = new VisualRelayCommand(OnSetManual, CanSetManual)
        {
            UniqueId = UniqueNameBase + ".SetManualCommand",
            Name = Resources.Strings.SetManualCommand_Name,
            MenuItemName = Resources.Strings.SetManualCommand_MenuItemName,
            ToolTip = Resources.Strings.MenuLayout_ManualTip,
            Weight = 0.26,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void OnSetManual(object parameter)
        {
            // NOTE: This operates on the current selection in the menu layout.
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (menuLayoutViewModel != null)
            {
                var currentSelection = menuLayoutViewModel.CurrentSelection;
                INTV.LtoFlash.ViewModel.ProgramViewModel.SetManual(currentSelection);
            }
        }

        private static bool CanSetManual(object parameter)
        {
            var canSetManual = false;
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (menuLayoutViewModel != null)
            {
                var currentSelection = menuLayoutViewModel.CurrentSelection;
                canSetManual = INTV.LtoFlash.ViewModel.ProgramViewModel.CanSetManual(currentSelection);
            }
            return canSetManual;
        }

        #endregion // SetManualCommand

        #region OpenManualCommand

        /// <summary>
        /// The command to open a manual in the default program on the computer.
        /// </summary>
        public static readonly VisualRelayCommand OpenManualCommand = new VisualRelayCommand(OnOpenManual, CanOpenManual)
        {
            UniqueId = UniqueNameBase + ".OpenManualCommand",
            Name = Resources.Strings.OpenManualCommand_Name,
            ToolTip = Resources.Strings.OpenManualCommand_ToolTip,
            Weight = 0.262,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void OnOpenManual(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (CanOpenManual(parameter))
            {
                var program = menuLayoutViewModel.CurrentSelection as ProgramViewModel;
                try
                {
                    RunExternalProgram.OpenInDefaultProgram(program.Manual);
                }
                catch (InvalidOperationException)
                {
                    // Silently fail.
                }
            }
        }

        private static bool CanOpenManual(object parameter)
        {
            var canOpenManual = false;
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (menuLayoutViewModel != null)
            {
                var program = menuLayoutViewModel.CurrentSelection as ProgramViewModel;
                canOpenManual = (program != null) && !string.IsNullOrWhiteSpace(program.Manual) && System.IO.File.Exists(program.Manual);
            }
            return canOpenManual;
        }

        #endregion // OpenManualCommand

        #region RemoveManualCommand

        /// <summary>
        /// The command to remove the manual file associated with the selected menu items.
        /// </summary>
        public static readonly VisualRelayCommand RemoveManualCommand = new VisualRelayCommand(RemoveManual, CanRemoveManual)
        {
            UniqueId = UniqueNameBase + ".RemoveManualCommand",
            Name = Resources.Strings.RemoveManualCommand_Name,
            ToolTip = Resources.Strings.MenuLayout_RemoveManualTip,
            Weight = 0.265,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void RemoveManual(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (menuLayoutViewModel != null)
            {
                var currentSelection = menuLayoutViewModel.CurrentSelection;
                INTV.LtoFlash.ViewModel.ProgramViewModel.RemoveManual(currentSelection);
            }
        }

        private static bool CanRemoveManual(object parameter)
        {
            var canRemoveManual = false;
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            if (menuLayoutViewModel != null)
            {
                var currentSelection = menuLayoutViewModel.CurrentSelection;
                canRemoveManual = INTV.LtoFlash.ViewModel.ProgramViewModel.CanRemoveManual(currentSelection);
            }
            return canRemoveManual;
        }

        #endregion // RemoveManualCommand

        #region SetSaveDataCommand

        /// <summary>
        /// The command to associate a data file with a program.
        /// </summary>
        public static readonly VisualRelayCommand SetSaveDataCommand = new VisualRelayCommand(OnSetSaveData, CanSetSaveData)
        {
            UniqueId = UniqueNameBase + ".SetSaveDataCommand",
            Name = Resources.Strings.SetSaveDataCommand_Name,
            MenuItemName = Resources.Strings.SetSaveDataCommand_MenuItemName,
            ToolTip = Resources.Strings.SetSaveDataCommand_Tip,
            Weight = 0.27,
            PreferredParameterType = typeof(ProgramViewModel)
        };

        private static void OnSetSaveData(object parameter)
        {
            var program = (ProgramViewModel)parameter;
            var saveData = ProgramViewModel.BrowseForSupportFile(program, ProgramFileKind.SaveData);
            if (!string.IsNullOrEmpty(saveData))
            {
                if (ProgramViewModel.AcceptDataFile(saveData))
                {
                    program.SaveData = saveData;
                }
                else
                {
                    OSMessageBox.Show(RomListViewModel.InvalidSaveDataMessage, RomListViewModel.InvalidSaveDataTitle, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                }
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private static bool CanSetSaveData(object parameter)
        {
            return (parameter != null) && (parameter is ProgramViewModel);
        }

        #endregion // SetSaveDataCommand

        #region RemoveSaveDataCommand

        /// <summary>
        /// The command to remove a data file's association from a program.
        /// </summary>
        public static readonly VisualRelayCommand RemoveSaveDataCommand = new VisualRelayCommand(OnRemoveSaveData, CanRemoveSaveData)
        {
            UniqueId = UniqueNameBase + ".RemoveSaveDataCommand",
            Name = Resources.Strings.RemoveSaveDataCommand_Name,
            ToolTip = Resources.Strings.RemoveSaveDataCommand_Tip,
            Weight = 0.275,
            PreferredParameterType = typeof(ProgramViewModel)
        };

        private static void OnRemoveSaveData(object parameter)
        {
            var program = (ProgramViewModel)parameter;
            program.SaveData = null;
            CommandManager.InvalidateRequerySuggested();
        }

        private static bool CanRemoveSaveData(object parameter)
        {
            var program = parameter as ProgramViewModel;
            return (program != null) && !string.IsNullOrWhiteSpace(program.SaveData);
        }

        #endregion // RemoveSaveDataCommand

        #region ReadmeCommand

        /// <summary>
        /// The command to show the readme file.
        /// </summary>
        public static readonly VisualRelayCommand ReadmeCommand = new VisualRelayCommand(OnReadme)
        {
            UniqueId = UniqueNameBase + ".ReadmeCommand",
            Name = Resources.Strings.ReadmeCommand_Name,
            Weight = 0.05,
            LargeIcon = typeof(CommandGroup).LoadImageResource("Resources/Images/help_32xMD.png"), // NOTE: this resource is in INTV.Shared!
            SmallIcon = typeof(CommandGroup).LoadImageResource("Resources/Images/help_16xLG.png"), // NOTE: this resource is in INTV.Shared!
        };

        private static void OnReadme(object parameter)
        {
            var programDir = SingleInstanceApplication.Instance.ProgramDirectory;
            var readmeFile = System.IO.Path.Combine(programDir, UIReadmeFilename);
            try
            {
                RunExternalProgram.OpenInDefaultProgram(readmeFile);
            }
            catch (System.IO.FileNotFoundException)
            {
#if MAC
                // Look for the default one embedded in the application.
                readmeFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, UIReadmeFilename);
#endif
                try
                {
                    RunExternalProgram.OpenInDefaultProgram(readmeFile);
                }
                catch (System.IO.FileNotFoundException)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.Strings.ReadmeCommand_ErrorMessageFormat, UIReadmeFilename);
                    var title = string.Format(CultureInfo.CurrentCulture, Resources.Strings.ReadmeCommand_ErrorMessageTitleFormat, UIReadmeFilename);
                    OSMessageBox.Show(message, title);
                }
                catch (InvalidOperationException)
                {
                    // Silently fail.
                }
            }
            catch (InvalidOperationException)
            {
                // Silently fail.
            }
        }

        #endregion // ReadmeCommand

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            // A NULL target is allowed for the case of an empty list.
            if (((target is INTV.Shared.ViewModel.ProgramDescriptionViewModel) || (target == null)) && (context is INTV.Shared.ViewModel.RomListViewModel))
            {
                yield return CreateContextMenuCommand(target, AddRomsToMenuCommand, context, null, 0.119);
            }
            else if (((target is FileNodeViewModel) || (target == null)) && (context is MenuLayoutViewModel))
            {
                yield return CreateContextMenuCommand(null, EditLongNameCommand.CreateSeparator(CommandLocation.Before), null);
                yield return CreateContextMenuCommand(target, EditLongNameCommand, context);
                yield return CreateContextMenuCommand(target, EditShortNameCommand, context);
                yield return CreateContextMenuCommand(target, SetColorCommand, context);
                yield return CreateContextMenuCommand(null, SetManualCommand.CreateSeparator(CommandLocation.Before), null);
                yield return CreateContextMenuCommand(target, SetManualCommand, context);
                yield return CreateContextMenuCommand(target, OpenManualCommand, context);
                yield return CreateContextMenuCommand(target, RemoveManualCommand, context);
                yield return CreateContextMenuCommand(null, NewDirectoryCommand.CreateSeparator(CommandLocation.Before), null);
                yield return CreateContextMenuCommand(target, NewDirectoryCommand, context);
                ////yield return CreateContextMenuCommand(target, AddRomsToMenuCommand, context, Resources.Strings.AddRomsToMenuCommand_ContextMenuItemName);
                yield return CreateContextMenuCommand(target, DeleteItemsCommand, context);
                yield return CreateContextMenuCommand(target, EmptyMenuLayoutCommand, context);
            }
        }

        /// <inheritdoc/>
        public override ICommand CreateContextMenuCommand(object target, ICommand command, object context, string name = null, double weight = double.NaN)
        {
            var contextMenuCommand = base.CreateContextMenuCommand(target, command, context, name, weight);
            var visualRelayCommand = contextMenuCommand as VisualRelayCommand;
            if ((visualRelayCommand != null) && (visualRelayCommand.UniqueId == SetColorCommand.UniqueId))
            {
                var fileNode = target as FileNodeViewModel;
                foreach (var color in MenuLayoutViewModel.Colors)
                {
                    var commandName = color.ToDisplayString();
                    var colorCommand = new VisualRelayCommand(SetFileColor, CanSetFileColor)
                    {
                        UniqueId = UniqueNameBase + ".SetFileNodeColorCommand." + color,
                        Name = commandName,
                    };
                    if (fileNode != null)
                    {
                        colorCommand.SmallIcon = fileNode.GetIconForColor(color);
                    }
                    var submenuCommand = base.CreateContextMenuCommand(target, colorCommand, context) as VisualRelayCommand;
                    var menuLayoutViewModel = context as MenuLayoutViewModel;
                    var commandContext = new Tuple<MenuLayoutViewModel, FileNodeViewModel, Color>(menuLayoutViewModel, fileNode, color);
                    AddSetColorSubmenuItem(visualRelayCommand.MenuItem, submenuCommand, commandContext);
                }
            }
            return contextMenuCommand;
        }

        /// <inheritdoc/>
        protected override object GetContextForCommand(object target, ICommand command, object context)
        {
            var relayCommand = command as RelayCommand;
            if (relayCommand != null)
            {
                if ((relayCommand.UniqueId == AddRomsToMenuCommand.UniqueId) ||
                    (relayCommand.UniqueId == BackupMenuLayoutCommand.UniqueId) ||
                    (relayCommand.UniqueId == RestoreMenuLayoutCommand.UniqueId) ||
                    (relayCommand.UniqueId == EmptyMenuLayoutCommand.UniqueId) ||
                    (relayCommand.UniqueId == OpenMenuLayoutBackupsDirectoryCommand.UniqueId))
                {
                    var menuLayoutViewModel = context as MenuLayoutViewModel;
                    return menuLayoutViewModel == null ? LtoFlashCommandGroup.LtoFlashViewModel : menuLayoutViewModel.LtoFlashViewModel;
                }
            }
            return base.GetContextForCommand(target, command, context);
        }

        /// <inheritdoc/>
        protected override void AddCommands()
        {
            CommandList.Add(NewDirectoryCommand);
            CommandList.Add(DeleteItemsCommand);
            CommandList.Add(AddRomsToMenuCommand);
            CommandList.Add(SetColorCommand);
            CommandList.Add(SetManualCommand);
            CommandList.Add(RemoveManualCommand);
            CommandList.Add(ReadmeCommand);
            AddPlatformCommands();
        }

        #endregion

        /// <summary>
        /// Platform-specific commands and adjustments.
        /// </summary>
        partial void AddPlatformCommands();
    }
}

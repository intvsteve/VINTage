// <copyright file="SelectBackupDialogViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for SelectBackupDialog.
    /// </summary>
    public class SelectBackupDialogViewModel : ViewModelBase
    {
        #region UI Strings

        /// <summary>The 'Cancel' button text.</summary>
        public static readonly string CancelButtonText = Resources.Strings.CancelButtonText;

        /// <summary>The 'Restore' button text.</summary>
        public static readonly string RestoreButtonText = Resources.Strings.RestoreCommand_Name;

        /// <summary>The 'Date' column title.</summary>
        public static readonly string DateColumnTitle = Resources.Strings.SelectBackupDialog_DateColumnTitle;

        /// <summary>The 'Number of Items' column title.</summary>
        public static readonly string NumberOfItemsColumnTitle = Resources.Strings.SelectBackupDialog_NumberOfItemsColumnTitle;

        /// <summary>The 'Path' column title.</summary>
        public static readonly string PathColumnTitle = Resources.Strings.SelectBackupDialog_PathColumnTitle;

        #endregion // UI Strings

        #region CancelSelectBackupCommand

        /// <summary>
        /// The command that cancels the backup selection dialog.
        /// </summary>
        public static readonly RelayCommand CancelSelectBackupCommand = new RelayCommand(OnCancel)
        {
            UniqueId = "INTV.Shared.ViewModel.SelectBackupDialogViewModel.CancelSelectBackupCommand",
            BlockWhenAppIsBusy = false
        };

        private static void OnCancel(object parameter)
        {
            var viewModel = parameter as SelectBackupDialogViewModel;
            viewModel.DialogResult = false;
        }

        #endregion // CancelSelectBackupCommand

        #region SelectBackupCommand

        /// <summary>
        /// The command to select a backup and dismiss the dialog.
        /// </summary>
        public static readonly RelayCommand SelectBackupCommand = new RelayCommand(OnSelectBackupToRestore, CanSelectBackupToRestore)
        {
            UniqueId = "INTV.Shared.ViewModel.SelectBackupDialogViewModel.SelectBackupCommand",
            BlockWhenAppIsBusy = false
        };

        private static void OnSelectBackupToRestore(object parameter)
        {
            var viewModel = parameter as SelectBackupDialogViewModel;
            viewModel.DialogResult = true;
        }

        private static bool CanSelectBackupToRestore(object parameter)
        {
            var viewModel = parameter as SelectBackupDialogViewModel;
            return (viewModel != null) && !string.IsNullOrEmpty(viewModel.SelectedBackupDirectory);
        }

        #endregion // SelectBackupCommand

        /// <summary>
        /// Initializes a new instance of the SelectBackupDialogViewModel type.
        /// </summary>
        public SelectBackupDialogViewModel()
        {
            Title = Resources.Strings.SelectBackupDialog_Title;
            Prompt = Resources.Strings.SelectBackupDialog_Prompt;
            Backups = new ObservableCollection<BackupInfoViewModel>();
        }

        /// <summary>
        /// Initialize the ViewModel to examine the specified device backup directory.
        /// </summary>
        /// <param name="deviceBackupDirectory">Device backup directory.</param>
        /// <param name="backupFileName">The name of the backup file used to populate the dialog.</param>
        /// <param name="fileExtensions">An enumerable of file extensions used to count how many items will be restored by a  backup, if <paramref name="showItemsCount"/> is <c>true</c>.</param>
        /// <param name="showItemsCount">If <c>true</c>, the 'Number of Items' column will be displayed.</param>
        internal void Initialize(string deviceBackupDirectory, string backupFileName, IEnumerable<string> fileExtensions, bool showItemsCount)
        {
            SelectedIndex = -1;
            ShowItemsCount = showItemsCount;
            if (System.IO.Directory.Exists(deviceBackupDirectory))
            {
                var backupDirectories = System.IO.Directory.EnumerateDirectories(deviceBackupDirectory);
                foreach (var backupDirectory in backupDirectories)
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(backupDirectory, backupFileName)))
                    {
                        try
                        {
                            Backups.Add(new BackupInfoViewModel(backupDirectory, fileExtensions));
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            // Ignore out-of-range or ill-formatted entries
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumerable of the backups available for the given device.
        /// </summary>
        public ObservableCollection<BackupInfoViewModel> Backups { get; private set; }

        /// <summary>
        /// Gets or sets the title for the dialog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the dialog.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected backup.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the item count column.
        /// </summary>
        public bool ShowItemsCount
        {
            get { return _showItemsCount; }
            set { AssignAndUpdateProperty("ShowItemsCount", value, ref _showItemsCount); }
        }
        private bool _showItemsCount = true;

        /// <summary>
        /// Gets or sets the currently selected BackupViewModel.
        /// </summary>
        public BackupInfoViewModel SelectedBackupViewModel
        {
            get { return string.IsNullOrEmpty(SelectedBackupDirectory) ? null : Backups[SelectedIndex]; }
            set { SelectedIndex = Backups.IndexOf(value); }
        }

        /// <summary>
        /// Gets the selected backup directory.
        /// </summary>
        public string SelectedBackupDirectory
        {
            get
            {
                string selectedBackupDirectory = null;
                if ((SelectedIndex >= 0) && (SelectedIndex < Backups.Count))
                {
                    selectedBackupDirectory = Backups[SelectedIndex].Path;
                }
                return selectedBackupDirectory;
            }
        }

        /// <summary>
        /// Gets or sets the dialog's result. OK (<c>true</c>), Cancel (<c>false</c>), or abort (<c>null</c>).
        /// </summary>
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { AssignAndUpdateProperty("DialogResult", value, ref _dialogResult); }
        }
        private bool? _dialogResult;
    }
}

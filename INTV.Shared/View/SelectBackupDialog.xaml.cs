// <copyright file="SelectBackupDialog.xaml.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for SelectBackupDialog.xaml
    /// </summary>
    public partial class SelectBackupDialog : System.Windows.Window
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of SelectBackupDialog.
        /// </summary>
        public SelectBackupDialog()
        {
            InitializeComponent();
        }

        #endregion // Constructor

        /// <summary>
        /// Creates an instance of the dialog pointed to backup data in the specified directory.
        /// </summary>
        /// <param name="backupDirectory">The backup directory.</param>
        /// <param name="backupFileName">Name of the backup file to identify valid backup subdirectories.</param>
        /// <param name="fileExtensions">If <paramref name="showItemsCount"/> is <c>true</c>, these file extensions are used to identify the number of backed up items in the backup directory.</param>
        /// <param name="showItemsCount">If <c>true</c>, include item count in selection dialog.</param>
        /// <returns>A new instance of the dialog.</returns>
        public static SelectBackupDialog Create(string backupDirectory, string backupFileName, IEnumerable<string> fileExtensions, bool showItemsCount)
        {
            var dialog = System.Windows.Application.Current.MainWindow.Create<SelectBackupDialog>();
            ((SelectBackupDialogViewModel)dialog.DataContext).Initialize(backupDirectory, backupFileName, fileExtensions, showItemsCount);
            return dialog;
        }
    }
}

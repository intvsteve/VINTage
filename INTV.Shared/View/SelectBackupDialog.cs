// <copyright file="SelectBackupDialog.cs" company="INTV Funhouse">
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

using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Dialog to select a data backup, presumably for restoration purposes.
    /// </summary>
    public partial class SelectBackupDialog
    {
        #region Properties

        /// <summary>
        /// Gets the selected backup directory.
        /// </summary>
        public string SelectedBackupDirectory
        {
            get { return ViewModel.SelectedBackupDirectory; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the item count column.
        /// </summary>
        public bool ShowItemsCount
        {
            get { return ViewModel.ShowItemsCount; }
            set { ViewModel.ShowItemsCount = value; }
        }

        private SelectBackupDialogViewModel ViewModel
        {
            get { return DataContext as SelectBackupDialogViewModel; }
        }

        #endregion // Properties
    }
}

// <copyright file="SelectBackupDialog.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using System.ComponentModel;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class SelectBackupDialog : Gtk.Dialog, INotifyPropertyChanged, IFakeDependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SelectBackupDialog"/> class.
        /// </summary>
        public SelectBackupDialog()
            : base(Resources.Strings.SelectBackupDialog_Title, INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow, Gtk.DialogFlags.Modal)
        {
            // TODO: Implement this
            DataContext = new SelectBackupDialogViewModel();
            this.Build();
        }

        #region INotifyPropertyChanged

        /// <inheritdoc/>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
        }

        #endregion // IFakeDependencyObject Properties

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
            var dialog = new SelectBackupDialog();
            ((SelectBackupDialogViewModel)dialog.DataContext).Initialize(backupDirectory, backupFileName, fileExtensions, showItemsCount);
            return dialog;
        }

        #region IFakeDependencyObject Methods

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject Methods
    }
}

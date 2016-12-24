// <copyright file="FileBrowserDialog.WPF.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// Implement the IFileBrowserDialog interface for Windows Vista and later. This is built on the versatile WindowsAPICodePack CommonFileDialog.
    /// </summary>
    public class FileBrowserDialog : FileBrowserDialogBase, IDisposable
    {
        private Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog _dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();

        ~FileBrowserDialog()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public override IEnumerable<string> FileNames
        {
            get { return _dialog.FileNames; }
        }

        /// <inheritdoc />
        public override void AddFilter(string filterName, IEnumerable<string> fileExtensions)
        {
            if (!IsFolderBrowser)
            {
                string extensions = string.Join(";", fileExtensions);
                var filter = new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter(filterName, extensions);
                _dialog.Filters.Add(filter);
            }
        }

        /// <inheritdoc />
        public override FileBrowserDialogResult ShowDialog()
        {
            _dialog.Title = Title;
            _dialog.IsFolderPicker = IsFolderBrowser;
            _dialog.Multiselect = Multiselect;
            _dialog.EnsureFileExists = EnsureFileExists;
            _dialog.EnsurePathExists = EnsurePathExists;
            return (FileBrowserDialogResult)_dialog.ShowDialog();
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dialog != null)
                {
                    _dialog.Dispose();
                }
            }
        }
    }
}

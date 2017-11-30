// <copyright file="FileBrowserDialog.Gtk.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public class FileBrowserDialog : FileBrowserDialogBase
    {
        private string[] _files;
        private Dictionary<string, IEnumerable<string>> _filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.FileBrowserDialog"/> class.
        /// </summary>
        public FileBrowserDialog()
        {
            _files = new string[] { };
            _filters = new Dictionary<string, IEnumerable<string>>();
        }

        #region IFileBrowserDialog

        /// <inheritdoc/>
        public override IEnumerable<string> FileNames
        {
            get { return _files; }
        }

        /// <inheritdoc/>
        public override void AddFilter(string filterName, IEnumerable<string> fileExtensions)
        {
            _filters[filterName] = fileExtensions;
        }

        /// <inheritdoc/>
        public override FileBrowserDialogResult ShowDialog()
        {
            var fileChooserAction = IsFolderBrowser ? Gtk.FileChooserAction.SelectFolder : Gtk.FileChooserAction.Open;
            var buttons = new object[] { Gtk.Stock.Cancel, Gtk.ResponseType.Cancel, Gtk.Stock.Open, Gtk.ResponseType.Accept };
            using (var dialog = new Gtk.FileChooserDialog(Title, SingleInstanceApplication.Instance.MainWindow, fileChooserAction, buttons))
            {
                dialog.SelectMultiple = Multiselect;
                foreach (var filter in _filters)
                {
                    var fileFilter = new Gtk.FileFilter() { Name = filter.Key };
                    fileFilter.AddCustom(Gtk.FileFilterFlags.Filename, info => FileFilter(dialog, info));
                    dialog.AddFilter(fileFilter);
                }

                var response = (Gtk.ResponseType)dialog.Run();
                _files = dialog.Filenames;
                VisualHelpers.Close(dialog);
                var dialogResult = FileBrowserDialogResult.None;
                switch (response)
                {
                    case Gtk.ResponseType.Accept:
                        dialogResult = FileBrowserDialogResult.Ok;
                        break;
                    case Gtk.ResponseType.Cancel:
                        dialogResult = FileBrowserDialogResult.Cancel;
                        break;
                    default:
                        break;
                }
                return dialogResult;
            }
        }

        #endregion // IFileBrowserDialog

        private bool FileFilter(Gtk.FileChooserDialog dialog, Gtk.FileFilterInfo filterInfo)
        {
            var match = false;
            var filter = dialog.Filter;
            IEnumerable<string> fileExtensions;
            if (_filters.TryGetValue(filter.Name, out fileExtensions))
            {
                foreach (var fileExtension in fileExtensions)
                {
                    if (fileExtension == ".*")
                    {
                        match = true;
                    }
                    else
                    {
                        match = filterInfo.Filename.EndsWith(fileExtension, System.StringComparison.OrdinalIgnoreCase);
                    }
                    if (match)
                    {
                        break;
                    }
                }
            }
            return match;
        }
    }
}

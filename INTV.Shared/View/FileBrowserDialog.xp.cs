// <copyright file="FileBrowserDialog.xp.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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
using System.Text;
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// Implement the IFileBrowserDialog interface for Windows xp. It just wraps the (crappy) Windows Forms dialogs.
    /// </summary>
    public class FileBrowserDialog : FileBrowserDialogBase
    {
        private IEnumerable<string> _files = Enumerable.Empty<string>();
        private Dictionary<string, IEnumerable<string>> _filters = new Dictionary<string, IEnumerable<string>>();

        /// <inheritdoc />
        public override IEnumerable<string> FileNames
        {
            get { return _files; }
        }

        /// <inheritdoc />
        public override void AddFilter(string filterName, IEnumerable<string> fileExtensions)
        {
            _filters[filterName] = fileExtensions;
        }

        /// <inheritdoc />
        public override FileBrowserDialogResult ShowDialog()
        {
            System.Windows.Forms.CommonDialog dialog = null;
            if (IsFolderBrowser)
            {
                var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderDialog.Description = Title;
                folderDialog.ShowNewFolderButton = false;
                folderDialog.SelectedPath = Properties.Settings.Default.LastBrowseFolder;
                dialog = folderDialog;
            }
            else
            {
                var fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Title = Title;
                fileDialog.Multiselect = Multiselect;
                fileDialog.CheckFileExists = EnsureFileExists;
                fileDialog.CheckPathExists = EnsurePathExists;
                fileDialog.InitialDirectory = Properties.Settings.Default.LastBrowseFolder;
                if (_filters.Any())
                {
                    StringBuilder filterBuilder = new StringBuilder();
                    var baseFormatString = "{0} ({1})|{1}";
                    var formatString = baseFormatString;
                    foreach (var filter in _filters)
                    {
                        var extensions = "*" + string.Join(";*", filter.Value);
                        filterBuilder.AppendFormat(formatString, filter.Key, extensions);
                        formatString = "|" + baseFormatString;
                    }
                    fileDialog.Filter = filterBuilder.ToString();
                }
                dialog = fileDialog;
            }

            var result = (FileBrowserDialogResult)dialog.ShowDialog();

            if (result == FileBrowserDialogResult.Ok)
            {
                if (IsFolderBrowser)
                {
                    Properties.Settings.Default.LastBrowseFolder = (dialog as System.Windows.Forms.FolderBrowserDialog).SelectedPath;
                    _files = new List<string>() { Properties.Settings.Default.LastBrowseFolder };
                }
                else
                {
                    _files = (dialog as System.Windows.Forms.OpenFileDialog).FileNames;
                    Properties.Settings.Default.LastBrowseFolder = System.IO.Path.GetDirectoryName(_files.First());
                }
            }

            return result;
        }
    }
}

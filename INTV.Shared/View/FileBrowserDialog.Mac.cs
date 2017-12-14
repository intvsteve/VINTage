// <copyright file="FileBrowserDialog.Mac.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;
#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation of the file browser dialog.
    /// </summary>
    public class FileBrowserDialog : FileBrowserDialogBase
    {
        private NSOpenPanel _fileDialog = new NSOpenPanel();
        private List<string> _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.FileBrowserDialog"/> class.
        /// </summary>
        public FileBrowserDialog()
        {
        }

        /// <inheritdoc />
        public override IEnumerable<string> FileNames
        {
            get { return _files; }
        }
        
        /// <inheritdoc />
        public override void AddFilter(string filterName, IEnumerable<string> fileExtensions)
        {
            List<string> allowedTypes = null;
            if (_fileDialog.AllowedFileTypes != null)
            {
                allowedTypes = _fileDialog.AllowedFileTypes.ToList();
            }
            else
            {
                allowedTypes = new List<string>();
            }
            var macFormatExtensions = fileExtensions.Select(e => e.Substring(1));
            foreach (var fileExtension in macFormatExtensions)
            {
                if (!allowedTypes.Contains(fileExtension, System.StringComparer.InvariantCultureIgnoreCase))
                {
                    allowedTypes.Add(fileExtension);
                }
            }
            _fileDialog.AllowedFileTypes = allowedTypes.ToArray();
        }
        
        /// <inheritdoc />
        public override FileBrowserDialogResult ShowDialog()
        {
            _fileDialog.Title = Title;
            _fileDialog.CanChooseDirectories = IsFolderBrowser;
            _fileDialog.CanChooseFiles = !IsFolderBrowser;
            _fileDialog.AllowsMultipleSelection = Multiselect;
            var dialogResult = FileBrowserDialogResult.None;
            var result = (NSPanelButtonType)(int)_fileDialog.RunModal();
            if (result == NSPanelButtonType.Cancel)
            {
                dialogResult = FileBrowserDialogResult.Cancel;
                _files = new List<string>();
            }
            else if (result == NSPanelButtonType.Ok)
            {
                dialogResult = FileBrowserDialogResult.Ok;
                _files = _fileDialog.Urls.Select(f => f.Path).ToList();
            }
            _fileDialog.Close();
            _fileDialog.Dispose();
            _fileDialog = null;
            return dialogResult;
        }
    }
}

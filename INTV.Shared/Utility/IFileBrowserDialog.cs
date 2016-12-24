// <copyright file="IFileBrowserDialog.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Interface for a file / folder browser dialog.
    /// </summary>
    public interface IFileBrowserDialog
    {
        /// <summary>
        /// Gets or sets the title for the browser dialog.
        /// </summary>
        /// <remarks>Must be set prior to calling ShowDialog().</remarks>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog will browse for files or folders.
        /// </summary>
        /// <remarks>Must be set prior to calling ShowDialog().</remarks>
        bool IsFolderBrowser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether selecting multiple items is allowed.
        /// </summary>
        /// <remarks>Must be set prior to calling ShowDialog().</remarks>
        /// <remarks>NOTE: This setting has no effect for folder browsers in Windows xp.</remarks>
        bool Multiselect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog requires that the file exists.
        /// </summary>
        /// <remarks>Must be set prior to calling ShowDialog().</remarks>
        bool EnsureFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog requires that the path exists.
        /// </summary>
        /// <remarks>Must be set prior to calling ShowDialog().</remarks>
        bool EnsurePathExists { get; set; }

        /// <summary>
        /// Gets the selected files or folders.
        /// </summary>
        /// <remarks>Will be empty if read prior to calling ShowDialog(), or if ShowDialog returns any value other than <c>FileBrowserDialogResult.Ok</c>.</remarks>
        IEnumerable<string> FileNames { get; }

        /// <summary>
        /// Adds a filter to the dialog to restrict the files displayed in the dialog.
        /// </summary>
        /// <param name="filterName">The user-friendly name for the filter.</param>
        /// <param name="fileExtensions">The file extension(s) to associate with the filter (e.g. .bin).</param>
        /// <remarks>Must be called prior to calling ShowDialog().</remarks>
        void AddFilter(string filterName, IEnumerable<string> fileExtensions);

        /// <summary>
        /// Displays the browser dialog, returning when the user has made a selection.
        /// </summary>
        /// <returns><c>FileBrowserDialogResult.Ok</c> if the user makes a valid selection, or <c>FileBrowserDialogResult.Cancel</c> if the results should be ignored.</returns>
        FileBrowserDialogResult ShowDialog();
    }
}

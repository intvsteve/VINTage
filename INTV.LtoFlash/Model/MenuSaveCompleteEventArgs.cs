// <copyright file="MenuSaveCompleteEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Argument sent when Menu Layout save finishes.
    /// </summary>
    public class MenuSaveCompleteEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.MenuSaveCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="originalPath">The original path for the menu layout to save to.</param>
        /// <param name="error">If an error occurred during save, this is it.</param>
        /// <param name="backupPath">The backup path to use in case an error occurred.</param>
        /// <param name="nonDirtying">If <c>true</c>, indicates save was not due to user edits, but some other operation.</param>
        /// <remarks>It's presumed that the menu layout stored at <paramref name="backupPath"/> is valid.</remarks>
        public MenuSaveCompleteEventArgs(string originalPath, System.Exception error, string backupPath, bool nonDirtying)
        {
            MenuPath = originalPath;
            Error = error;
            BackupPath = backupPath;
            NonDirtying = nonDirtying;
        }

        /// <summary>
        /// Gets the absolute path the menu was intended to be saved to.
        /// </summary>
        public string MenuPath { get; private set; }

        /// <summary>
        /// Gets the error that occurred during the save, which is <c>null</c> upon successful save.
        /// </summary>
        public System.Exception Error { get; private set; }

        /// <summary>
        /// Gets the backup path to use in case of a save error.
        /// </summary>
        public string BackupPath { get; private set; }

        /// <summary>
        /// Gets a value indicating that the save operation was incidental, and not due to user edits.
        /// </summary>
        public bool NonDirtying { get; private set; }
    }
}

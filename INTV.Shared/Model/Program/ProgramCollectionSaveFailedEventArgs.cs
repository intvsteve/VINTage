// <copyright file="ProgramCollectionSaveFailedEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Event data sent when a ProgramCollection.Save operation fails.
    /// </summary>
    public class ProgramCollectionSaveFailedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="INTV.Shared.Model.Program.ProgramCollectionSaveFailedEventArgs"/> class.
        /// </summary>
        /// <param name="romListPath">Absolute path to the ROM list that was being saved.</param>
        /// <param name="error">The error that occurred.</param>
        /// <param name="romListBackupPath">Absolute path to a backup copy of the ROM list.</param>
        public ProgramCollectionSaveFailedEventArgs(string romListPath, System.Exception error, string romListBackupPath)
        {
            RomListPath = romListPath;
            Error = error;
            RomListBackupPath = romListBackupPath;
        }

        /// <summary>
        /// Gets the absolute path to the ROM list that was being saved, whose save operation failed.
        /// </summary>
        public string RomListPath { get; private set; }

        /// <summary>
        /// Gets the error that occurred during the save operation.
        /// </summary>
        public System.Exception Error { get; private set; }

        /// <summary>
        /// Gets the absolute path to a backup copy of the ROM list.
        /// </summary>
        public string RomListBackupPath { get; private set; }
    }
}

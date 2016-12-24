// <copyright file="AddRomsFromFilesBeginEventArgs.cs" company="INTV Funhouse">
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
    /// Event argument passed when about to add new ROMs.
    /// </summary>
    public class AddRomsFromFilesBeginEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Model.Program.AddRomsFromFilesBeginEventArgs"/> class.
        /// </summary>
        public AddRomsFromFilesBeginEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are adding starter ROMs.
        /// </summary>
        public bool AddingStarterRoms { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the add ROMs operation.
        /// </summary>
        public bool Cancel { get; set; }
    }
}

// <copyright file="ExecuteDeviceCommandProgressUpdateMode.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Different progress bar update modes for ExecuteDeviceCommandAsyncTaskData.
    /// </summary>
    internal enum ExecuteDeviceCommandProgressUpdateMode
    {
        /// <summary>
        /// Default mode, in which the progress bar's title is updated using a command-specific 'Title' string.
        /// </summary>
        Default,

        /// <summary>
        /// A single title string is used, with each command's individual 'Title' string shown in the 'progress' area.
        /// </summary>
        Multistage,

        /// <summary>
        /// No automatic updating is done -- it's up to the task itself to update it.
        /// </summary>
        Custom,
    }
}

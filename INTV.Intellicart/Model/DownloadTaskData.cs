// <copyright file="DownloadTaskData.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Shared.Utility;

namespace INTV.Intellicart.Model
{
    /// <summary>
    /// Asynchronous task data for loading a ROM onto an Intellicart.
    /// </summary>
    internal class DownloadTaskData : AsyncTaskData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Intellicart.Model.DownloadTaskData"/> class.
        /// </summary>
        /// <param name="task">The asynchronous task that will use this data.</param>
        /// <param name="intellicart">The Intellicart model to which a ROM is sent.</param>
        /// <param name="name">The name of the ROM being sent.</param>
        /// <param name="rom">The ROM being sent to an Intellicart.</param>
        internal DownloadTaskData(AsyncTaskWithProgress task, IntellicartModel intellicart, string name, IRom rom)
            : base(task)
        {
            Intellicart = intellicart;
            Name = name;
            Rom = rom;
        }

        /// <summary>
        /// Gets the target Intellicart for the load.
        /// </summary>
        internal IntellicartModel Intellicart { get; private set; }

        /// <summary>
        /// Gets the "friendly" name for the ROM to load.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets the ROM to load onto an Intellicart.
        /// </summary>
        internal IRom Rom { get; private set; }

        /// <summary>
        /// Gets or sets the error handler used to report errors.
        /// </summary>
        internal Action<string, Exception> ErrorHandler { get; set; }

        /// <summary>
        /// Gets or sets a specific error message to report.
        /// </summary>
        internal string Message { get; set; }
    }
}

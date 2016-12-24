// <copyright file="StartupTaskPriority.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Startup task priority values.
    /// </summary>
    /// <remarks>This value provides sentinals for more fine-grained control over the ordering of startup tasks.
    /// Note that although there are different possible priorities, there are no guarantees about the specific
    /// order of execution or completion for those tasks, only that they will be schedule in the order of their priority.</remarks>
    public enum StartupTaskPriority : int
    {
        /// <summary>
        /// The last asynchronous startup task priority.
        /// </summary>
        /// <remarks>If multiple startup tasks are registered with this priority, their order of
        /// execution is indeterminate.</remarks>
        LowestAsyncTaskPriority = int.MinValue,

        /// <summary>
        /// The highest asynchronous startup task priority.
        /// </summary>
        /// <remarks>Any startup task at or below this priorty will be scheduled on the main application thread
        /// to execute asynchronously.</remarks>
        HighestAsyncTaskPriority = -1,

        /// <summary>
        /// The lowest synchronous startup task priority.
        /// </summary>
        /// <remarks>Any startup task at or above this priority will be schedule to run synchronously
        /// on the main application thread when the application finishes launching.</remarks>
        LowestSyncTaskPriority = 0,

        /// <summary>
        /// The highest synchronous task priority.
        /// </summary>
        /// <remarks>If multiple startup tasks are registered with this priority, their order of
        /// execution is indeterminate.</remarks>
        HighestSyncTaskPriority = int.MaxValue
    }
}

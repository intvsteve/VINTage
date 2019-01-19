// <copyright file="StatusMessageEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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
using System.Collections.Generic;

namespace INTV.Core.Model
{
    /// <summary>
    /// Event data provided when status messages are reported.
    /// </summary>
    public sealed class StatusMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMessageEventArgs"/> class.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public StatusMessageEventArgs(IEnumerable<StatusMessage> messages)
        {
            Messages = messages;
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public IEnumerable<StatusMessage> Messages { get; private set; }
    }
}

// <copyright file="IONotificationEventArgs.cs" company="INTV Funhouse">
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
using MonoMac.Foundation;

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// This class is used to notify interested parties about notifications received from the IOKit.
    /// </summary>
    public class IONotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of IONotificationEventArgs.
        /// </summary>
        /// <param name="refcon">Custom data provided when the notification was registered.</param>
        /// <param name="iterator">The data from the IOKit notification system.</param>
        public IONotificationEventArgs(NSObject refcon, IOIterator iterator)
        {
            Refcon = refcon;
            Iterator = iterator;
        }

        /// <summary>
        /// Gets the custom notification data.
        /// </summary>
        public NSObject Refcon { get; private set; }

        /// <summary>
        /// Gets the data supplied by the IOKit notification.
        /// </summary>
        public IOIterator Iterator { get; private set; }
    }
}

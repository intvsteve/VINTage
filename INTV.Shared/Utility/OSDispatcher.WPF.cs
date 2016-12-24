// <copyright file="OSDispatcher.WPF.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// WPF-specific implementation of OSDispatcher.
    /// </summary>
    public partial struct OSDispatcher
    {
        /// <summary>
        /// Gets the dispatcher on the calling thread.
        /// </summary>
        public static OSDispatcher Current
        {
            get
            {
                return new OSDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dispatcher is on the main thread of the application.
        /// </summary>
        public static bool IsMainThread
        {
            get
            {
                var mainThreadId = SingleInstanceApplication.MainThreadDispatcher.NativeDispatcher.Thread.ManagedThreadId;
                return mainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId;
            }
        }

        /// <summary>
        /// Invokes an action on the dispatcher at a future time.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void BeginInvoke(System.Action action)
        {
            if (NativeDispatcher != null)
            {
                NativeDispatcher.BeginInvoke(action);
            }
        }

        /// <summary>
        /// Synchronously invokes an action on the dispatcher with a given priority.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="priority">The priority of the action to execute.</param>
        public void Invoke(System.Action action, OSDispatcherPriority priority)
        {
            if (NativeDispatcher != null)
            {
                NativeDispatcher.Invoke(action, (System.Windows.Threading.DispatcherPriority)priority);
            }
        }
    }
}

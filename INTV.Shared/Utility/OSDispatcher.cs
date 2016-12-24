// <copyright file="OSDispatcher.cs" company="INTV Funhouse">
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

#if WIN
using NativeDispatcher = System.Windows.Threading.Dispatcher;
#elif MAC
using NativeDispatcher = MonoMac.Foundation.NSThread;
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Abstraction of a dispatcher. We just use NSThread on Mac. Perhaps NSRunLoop would be more appropriate?
    /// </summary>
    public partial struct OSDispatcher
    {
        private NativeDispatcher _nativeDispatcher;

        private OSDispatcher(NativeDispatcher nativeDispatcher)
        {
            _nativeDispatcher = nativeDispatcher;
        }

        /// <summary>
        /// Gets the dispatcher in its native form.
        /// </summary>
        public NativeDispatcher NativeDispatcher
        {
            get { return _nativeDispatcher; }
        }

        /// <summary>
        /// Invoke the given action synchronously on the application's main dispatcher (a.k.a. UI thread).
        /// </summary>
        /// <param name="action">The delegate to execute.</param>
        public void InvokeOnMainDispatcher(System.Action action)
        {
            if (IsMainThread)
            {
                action();
            }
            else
            {
                SingleInstanceApplication.MainThreadDispatcher.Invoke(action, OSDispatcherPriority.Normal);
            }
        }
    }
}

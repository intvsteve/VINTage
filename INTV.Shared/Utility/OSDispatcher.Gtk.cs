// <copyright file="OSDispatcher.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

////#define ENABLE_DEBUG_OUTPUT

using System.Threading;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial struct OSDispatcher
    {
        private static readonly int MainThreadId = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// Gets the thread's dispatcher.
        /// </summary>
        public static OSDispatcher Current
        {
            get
            {
                return new OSDispatcher(Thread.CurrentThread);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the caller is on the main thread of the application.
        /// </summary>
        public static bool IsMainThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == MainThreadId;
            }
        }

        /// <summary>
        /// Sets up an action to execute on the dispatcher's thread at the next opportunity. The action is not execute immediately.
        /// </summary>
        /// <param name="action">The action to execute at a later time.</param>
        public void BeginInvoke(System.Action action)
        {
            // NOTE: This essentially ALWAYS puts on main thread!
            DebugOutput("*$*$*$ INVOKING from thread: " + Thread.CurrentThread.ManagedThreadId + ", MAIN: " + MainThreadId);
            GLib.Idle.Add(() => BeginInvokeHandler(action));

            // Alternative: Gtk.Application.Invoke(). Also on main thread.
        }

        /// <summary>
        /// Synchronously invokes the action on the dispatcher.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="priority">The importance of the action to execute.</param>
        public void Invoke(System.Action action, OSDispatcherPriority priority)
        {
            if (NativeDispatcher.ManagedThreadId == MainThreadId)
            {
                // We're the main thread dispatcher -- but if not called from main thread dispatcher,
                // we need to properly direct the call.
                if (IsMainThread)
                {
                    action.Invoke();
                }
                else
                {
                    // Oy... Application.Invoke is *NOT* synchronous -- names are important! So,
                    // adopt the mechanism from here: https://stackoverflow.com/questions/2213243/synchronous-blocking-application-invoke-for-gtk
                    var wait = new ManualResetEventSlim();
                    Gtk.Application.Invoke((s, e) =>
                        {
                            action();
                            wait.Set();
                        });
                    wait.Wait();
                }
            }
            else
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Invokes a function on the main (UI) thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="priority">The priority at which to execute <paramref name="action"/> on the main thread.</param>
        public void InvokeOnMainThread(System.Action action, OSDispatcherPriority priority)
        {
            throw new System.NotImplementedException("InvokeOnMainThread(System.Action action, OSDispatcherPriority priority)");
        }

        /// <summary>
        /// Invokes a function on the main (UI) thread.
        /// </summary>
        /// <typeparam name="T">The data type of the given function's return value.</typeparam>
        /// <param name="function">A function to execute.</param>
        /// <param name="priority">The priority at which to execute <paramref name="function"/> on the main thread.</param>
        /// <returns>The return value of the function.</returns>
        public T InvokeOnMainThread<T>(System.Func<T> function, OSDispatcherPriority priority)
        {
            throw new System.NotImplementedException("InvokeOnMainThread<T>(Func<T> function, OSDispatcherPriority priority)");
        }

        private static bool BeginInvokeHandler(System.Action action)
        {
            action();
            return false;
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

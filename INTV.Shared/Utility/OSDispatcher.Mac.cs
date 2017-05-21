// <copyright file="OSDispatcher.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation for OSDispatcher.
    /// </summary>
    public partial struct OSDispatcher
    {
        /// <summary>
        /// Gets the thread's dispatcher.
        /// </summary>
        public static OSDispatcher Current
        {
            get
            {
                return new OSDispatcher(NSThread.Current);
            }
        }

        /// <summary>
        /// Gets whether the dispatcher is on the main thread of the application.
        /// </summary>
        public static bool IsMainThread
        {
            get
            {
                return NSThread.IsMain;
            }
        }

        /// <summary>
        /// Sets up an action to execute on the dispatcher's thread at the next opportunity. The action is not execute immediately.
        /// </summary>
        /// <param name="action">The action to execute at a later time.</param>
        public void BeginInvoke(System.Action action)
        {
            DebugOutput("*$*$*$ INVOKING from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
          _dispatcherObject.Invoke(this, action, false);
        }

        /// <summary>
        /// Synchronously invokes the action on the dispatcher.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="priority">The importance of the action to execute.</param>
        public void Invoke(System.Action action, OSDispatcherPriority priority)
        {
          _dispatcherObject.Invoke(this, action, true);
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

        private static readonly ActionSelector _dispatcherObject = new ActionSelector();

        private const string ActionSelectorName = "InvokedAction:";

        private class ActionSelector : NSObject
        {
            public void Invoke(OSDispatcher dispatcher, System.Action action, bool waitUntilDone)
            {
                DebugOutput("^^^^^^^^^^^^^^^ PERFORM from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle + ", DISPATCHER: " + dispatcher.NativeDispatcher.Handle);
                var currentThread = NSThread.Current;
                if ((dispatcher.NativeDispatcher == currentThread) && waitUntilDone)
                {
                    action();
                }
                else
                {
                    _dispatcherObject.PerformSelector(new MonoMac.ObjCRuntime.Selector(OSDispatcher.ActionSelectorName), dispatcher.NativeDispatcher, new SelectorArgument(action), waitUntilDone);
                }
            }

            [Action(OSDispatcher.ActionSelectorName)]
            public void Action(NSObject arg)
            {
                try
                {
                    DebugOutput("++++++++++++++++++ EXECUTING ACTION from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
                    ((SelectorArgument)arg).Action();
                }
                catch(System.Exception e)
                {
                    ErrorReporting.ReportError(ReportMechanism.Console, e.Message);
                }
            }

            class SelectorArgument : NSObject
            {
                public System.Action Action { get; private set; }

                internal SelectorArgument(System.Action action)
                {
                    Action = action;
                }
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

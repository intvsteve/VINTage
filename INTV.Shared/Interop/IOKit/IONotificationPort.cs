// <copyright file="IONotificationPort.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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

////#define ENABLE_DIAGNOSTIC_OUTPUT

using System;
#if __UNIFIED__
using CoreFoundation;
using ObjCRuntime;
#else
using MonoMac.CoreFoundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// Class to act as a C# version of the IONotificationPort IOKit type.
    /// </summary>
    public class IONotificationPort : INativeObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IONotificationPort"/> class.
        /// </summary>
        public IONotificationPort()
        {
            Handle = NativeMethods.IONotificationPortCreate(IntPtr.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IONotificationPort"/> class.
        /// </summary>
        /// <param name="nativeObject">The native IOKit object pointer.</param>
        public IONotificationPort(IntPtr nativeObject)
        {
            Handle = nativeObject;
        }

        #region INativeObject implementation

        /// <summary>
        /// Gets the native object handle.
        /// </summary>
        /// <value>The handle.</value>
        public IntPtr Handle
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Gets the CFRunLoopSource as a native object pointer.
        /// </summary>
        /// <remarks>Can't find a way to create the Mono CFRunLoopSource from IntPtr, the way NSObject has FromObject.</remarks>
        public IntPtr RunLoopSource
        {
            get
            {
                var runLoopSourcePtr = NativeMethods.IONotificationPortGetRunLoopSource(Handle);
                return runLoopSourcePtr;
            }
        }

        #region IDisposable implementation

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
            DebugOutput("**** IONotificationPort.Dispose()");
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="INTV.Shared.Interop.IOKit.IONotificationPort"/> is disposed.
        /// </summary>
        protected bool Disposed { get; set; }

        /// <summary>
        /// Disposes native resources.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing directly, otherwise called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                DebugOutput("**** IONotificationPort.Dispose(bool) marking disposed");
                Disposed = true;
                if (Handle != IntPtr.Zero)
                {
                    if (disposing)
                    {
                        var handle = Handle;
                        Handle = IntPtr.Zero;
                        NativeMethods.IONotificationPortDestroy(handle);
                        DebugOutput("**** IONotificationPort.Dispose(bool) for " + GetType() + " called IONotificationPortDestroy");
                    }
                }
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

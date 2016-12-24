// <copyright file="IOKitObject.cs" company="INTV Funhouse">
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

////#define ENABLE_DEBUG_OUTPUT

using System;
using MonoMac.ObjCRuntime;
using System.Runtime.InteropServices;

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// IO kit object.
    /// </summary>
    /// <remarks>C# class wrapping IOKit's io_object_t.</remarks>
    public class IOKitObject : INativeObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOKitObject"/> class.
        /// </summary>
        /// <param name="nativeObject">The native object.</param>
        protected IOKitObject(IntPtr nativeObject)
        {
            Handle = nativeObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOKitObject"/> class.
        /// </summary>
        /// <param name="initializer">A custom initialization function.</param>
        protected IOKitObject(Func<IntPtr> initializer)
        {
            Handle = initializer();
        }

        /// <summary>
        /// Gets the retain count.
        /// </summary>
        public uint RetainCount
        {
            get { return NativeMethods.IOObjectGetUserRetainCount(Handle); }
        }

        #region INativeObject implementation

        /// <summary>
        /// Gets the native object handle.
        /// </summary>
        public IntPtr Handle
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Creates an instance of an IOKit object.
        /// </summary>
        /// <typeparam name="T">Data type of the IOKit object to create.</typeparam>
        /// <param name="handle">Handle.</param>
        /// <returns>The IOKit object.</returns>
        public static T CreateIOKitObject<T>(IntPtr handle) where T : IOKitObject, new()
        {
            var ioKitObject = new T();
            ioKitObject.Handle = handle;
            return ioKitObject;
        }

        #region IDisposable implementation

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize (this);
#if ENABLE_DEBUG_OUTPUT
            System.Diagnostics.Debug.WriteLine("**** IOKitObject.Dispose()");
#endif // ENABLE_DEBUG_OUTPUT
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="INTV.Shared.Interop.IOKit.IOKitObject"/> is disposed.
        /// </summary>
        protected bool Disposed { get; set; }

        /// <summary>
        /// Disposes native resources.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing directly; otherwise via finalizer.</param>
        protected virtual void Dispose (bool disposing)
        {
            if (!Disposed)
            {
#if ENABLE_DEBUG_OUTPUT
                System.Diagnostics.Debug.WriteLine("**** IOKitObject.Dispose(bool) for " + GetType() + " marking disposed, RetainCount: " + RetainCount);
#endif // ENABLE_DEBUG_OUTPUT
                Disposed = true;
                if (Handle != IntPtr.Zero)
                {
                    if (disposing)
                    {
                        var handle = Handle;
                        Handle = IntPtr.Zero;
                        NativeMethods.IOObjectRelease(handle);
#if ENABLE_DEBUG_OUTPUT
                        System.Diagnostics.Debug.WriteLine("**** IOKitObject.Dispose(bool) for " + GetType() + " called IOObjectRelease, RetainCount: " + NativeMethods.IOObjectGetUserRetainCount(handle));
#endif // ENABLE_DEBUG_OUTPUT
                    }
                }
            }
        }
    }
}

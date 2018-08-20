// <copyright file="IOConnect.cs" company="INTV Funhouse">
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

////#define ENABLE_DEBUG_OUTPUT

#if __UNIFIED__
using Foundation;
using nint = System.nint;
#else
using MonoMac.Foundation;
using nint = System.Int32;
#endif // __UNIFIED__

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// IO service interest callback.
    /// </summary>
    /// <param name="refcon">Custom (native) data passed to the delegate when initially registered and then when invoked.</param>
    /// <param name="service">The native object pointer of the service whose state has changed.</param>
    /// <param name="messageType">The type of the message - see <see cref="IOMessage"/> for details.</param>
    /// <param name="messageArgument">Data specific to the <paramref name="messageType"/> argument.</param>
    public delegate void IOServiceInterestCallback(System.IntPtr refcon, System.IntPtr service, uint messageType, System.IntPtr messageArgument);

    /// <summary>
    /// A partial wrapper for the parts of io_connect_t that is needed.
    /// </summary>
    public class IOConnect : IOKitObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOConnect"/> class.
        /// </summary>
        public IOConnect()
            : this(System.IntPtr.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOConnect"/> class.
        /// </summary>
        /// <param name="nativeObject">Native object.</param>
        internal IOConnect(System.IntPtr nativeObject)
            : base(nativeObject)
        {
        }

        /// <summary>
        /// Gets the notification port returned when the connection was created.
        /// </summary>
        public IONotificationPort NotificationPort { get; private set; }

        private IOKitObject Notifier { get; set; }

        /// <summary>
        /// Creates the system power monitor connection.
        /// </summary>
        /// <param name="systemPowerDelegate">System power delegate.</param>
        /// <param name="callbackData">Callback data.</param>
        /// <returns>The system power monitor connection.</returns>
        /// <remarks>The caller must add the <see cref="IOConnect.NotificationPort"/> to a CFRunLoop in order for
        /// <paramref name="systemPowerDelegate"/> to be called. When the consumer of the returned IO connection
        /// is finished, first, remove the NotificationPort from the CFRunLoop, then call the Dispose() method.</remarks>
        public static IOConnect CreateSystemPowerMonitorConnection(IOServiceInterestCallback systemPowerDelegate, NSObject callbackData)
        {
            return IORegisterForSystemPower(systemPowerDelegate, callbackData);
        }

        private static IOConnect IORegisterForSystemPower(IOServiceInterestCallback systemPowerDelegate, NSObject refcon)
        {
            var ioNotificationPort = System.IntPtr.Zero;
            var notifierHandle = System.IntPtr.Zero;
            var callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(systemPowerDelegate);
            var nativeIOConnect = NativeMethods.IORegisterForSystemPower(refcon.Handle, out ioNotificationPort, callback, out notifierHandle);

            var ioConnect = IOKitObject.CreateIOKitObject<IOConnect>(nativeIOConnect);
            ioConnect.NotificationPort = new IONotificationPort(ioNotificationPort);
            ioConnect.Notifier = IOKitObject.CreateIOKitObject<IOKitObject>(notifierHandle, n => IODeregisterForSystemPower(n));
            return ioConnect;
        }

        /// <summary>
        /// The caller acknowledges notification of a power state change on a device it has registered
        /// for notifications for via IORegisterForSystemPower.
        /// </summary>
        /// <param name="notificationId">Notification identifier.</param>
        /// <returns>The result of the funcion call - 0 indicates success, otherwise an error code.</returns>
        public int AllowPowerChange(nint notificationId)
        {
            return NativeMethods.IOAllowPowerChange(Handle, notificationId);
        }

        /// <summary>
        /// The caller wishes to cancel a change in poewr state. The request may or may not be
        /// honored, depending on system state.
        /// </summary>
        /// <param name="notificationId">Notification identifier.</param>
        /// <returns>The result of the funcion call - 0 indicates success, otherwise an error code.</returns>
        public int CancelPowerChange(nint notificationId)
        {
            return NativeMethods.IOCancelPowerChange(Handle, notificationId);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                DebugOutput("**** IOConnect.Dispose(bool) for " + GetType() + " marking disposed, RetainCount: " + RetainCount);
                Disposed = true;

                // The order of disposal here is based on the example at:
                // https://developer.apple.com/library/content/qa/qa1340/_index.html
                // Alternative ordering has not been tested.
                // Also, this code assumes that the NotificationPort has been removed from any run loops.

                // First, de-register the notifier.
                if (Notifier != null)
                {
                    Notifier.Dispose();
                }
                Notifier = null;

                // Now, close this IO connection.
                var handle = Handle;
                if (handle != System.IntPtr.Zero)
                {
                    IOService.Close(this);
                    ClearHandle();
                }

                // Finally, destroy the notification port itself.
                if (NotificationPort != null)
                {
                    NotificationPort.Dispose();
                }
                NotificationPort = null;

                DebugOutput("**** IOConnect.Dispose(bool) for " + GetType() + " called IOServerClose, RetainCount: " + NativeMethods.IOObjectGetUserRetainCount(handle));
            }
        }

        /// <summary>
        /// Deregisters a notifier created via <see cref="IORegisterForSystemPower"/>.
        /// </summary>
        /// <param name="notifier">The notifier to de-register.</param>
        /// <returns>The result of the deregister operation.</returns>
        private static int IODeregisterForSystemPower(IOKitObject notifier)
        {
            var handle = notifier.Handle;
            var result = NativeMethods.IODeregisterForSystemPower(ref handle);
            DebugOutput("IOService: IODeregisterForSystemPower() returned: " + result);

            // Should we throw if an error occurs? Called from Dispose() -- would that be a bad idea?
            return result;
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

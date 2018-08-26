// <copyright file="IOService.cs" company="INTV Funhouse">
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

#define USE_IOKIT_NOTIFICATIONS
////#define ENABLE_DEBUG_OUTPUT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using INTV.Shared.Interop.DeviceManagement;
#if __UNIFIED__
using AppKit;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__

#if __UNIFIED__
using CFRunLoopString = Foundation.NSString;
using nint = System.nint;
#else
using CFRunLoopString = System.String;
using nint = System.Int32;
#endif // __UNIFIED__

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// These flags describe which IOServices have some level of support.
    /// </summary>
    [Flags]
    public enum IOServices
    {
        /// <summary>No services.</summary>
        None = 0,

        /// <summary>Report serial port arrival / departure.</summary>
        SerialPort = 1 << 0,

        /// <summary>Report system power events (enter/exit sleep / low power).</summary>
        SystemPower = 1 << 1,

        /// <summary>All available services.</summary>
        All = SerialPort | SystemPower
    }

    /// <summary>
    /// This class acts partially as a C# version of the IOKit IOService type.
    /// </summary>
    /// <remarks>Maybe this should be reworked and split up, because this most definitely does NOT
    /// act as a partial exposure of the IOService API. At this point, it does two very non-generic
    /// things:
    /// 1. Monitors for the arrival and departure of serial ports in the system
    /// 2. Provides access to the power domain IOService</remarks>
    public class IOService : IORegistryEntry
    {
        #region IOService notification types

        public static readonly string KIOPublishNotification = "IOServicePublish";
        public static readonly string KIOFirstPublishNotification = "IOServiceFirstPublish";
        public static readonly string KIOMatchedNotification = "IOServiceMatched";
        public static readonly string KIOFirstMatchNotification = "IOServiceFirstMatch";
        public static readonly string KIOTerminatedNotification = "IOServiceTerminate";

        #endregion // IOService notification types

        private delegate void IONotificationPortCallback(IntPtr refcon, IntPtr iterator);

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOService"/> class.
        /// </summary>
        public IOService()
        {
        }

        private ISerialPortNotifier PortNotifier { get; set; }

        private IOServices Services { get; set; }

        /// <summary>
        /// Gets the IOServices matching the given name.
        /// </summary>
        /// <returns>The IOServices matching the given name.</returns>
        /// <param name="name">The name filter to use to locate IOServices.</param>
        public static NSMutableDictionary MatchingServicesFromName(string name)
        {
            NSMutableDictionary dictionary = null;
            var dictionaryPointer = NativeMethods.IOServiceMatching(name);
            if (dictionaryPointer != IntPtr.Zero)
            {
                dictionary = Runtime.GetNSObject(dictionaryPointer) as NSMutableDictionary;
            }
            return dictionary;
        }

        /// <summary>
        /// Close a connection to an IOService and destroy the connect handle. This should be called
        /// via a custom dispose action installed when the IOKitObject was created, rather than directly.
        /// </summary>
        /// <param name="connect">The IOKit connection to close.</param>
        /// <returns>The result of the function call.</returns>
        public static int Close(IOKitObject connect)
        {
            var result = NativeMethods.IOServiceClose(connect.Handle);
            DebugOutput("IOService: Close() returned: " + result);

            // How to report if an error occurs? This is typically called from Dispose() -- throwing is a bad idea.
            return result;
        }

        /// <summary>
        /// Start the specified services.
        /// </summary>
        /// <param name="services">Services to start.</param>
        /// <remarks>The serial port and power services are joined at the hip and rather inseparable at this point.</remarks>
        public void StartServices(IOServices services)
        {
            Services = services;
            if (services.HasFlag(IOServices.SerialPort) || services.HasFlag(IOServices.SystemPower))
            {
                StartSerialPortMonitor();
            }
        }

        /// <summary>
        /// Stops all running IOServices.
        /// </summary>
        public void StopAllServices()
        {
            if (Services.HasFlag(IOServices.SerialPort) || Services.HasFlag(IOServices.SystemPower))
            {
                StopSerialPortMonitor();
            }
        }

        private static ISerialPortNotifier CreatePortNotifier()
        {
            ISerialPortNotifier portNotifier = null;

            var portNotifierKind = DeviceManagementInterfaceKindHelpers.GetKind();
            switch (portNotifierKind)
            {
                case DeviceManagementInterfaceKind.IOKit:
                    portNotifier = new IOKitNotificationPort();
                    break;
                case DeviceManagementInterfaceKind.Dev:
                    portNotifier = new FileSystemNotifcationPort();
                    break;
            }

            return portNotifier;
        }

        private static void ReportPortArrival(string portName)
        {
            DebugOutput("IOService: Arrival: port: " + portName);
            DeviceChange.SystemReportsDeviceAdded(null, portName, INTV.Core.Model.Device.ConnectionType.Serial);
        }

        private static void ReportPortDeparture(string portName)
        {
            DebugOutput("IOService: Departure: port: " + portName);
            DeviceChange.SystemReportsDeviceRemoved(null, portName, INTV.Core.Model.Device.ConnectionType.Serial);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Starts the serial port monitor.
        /// </summary>
        private void StartSerialPortMonitor()
        {
            DebugOutput("IOService: StartSerialPortMonitor");
            PortNotifier = CreatePortNotifier();
            var systemPowerManagement = PortNotifier as ISystemPowerManagement;
            if (systemPowerManagement != null)
            {
                systemPowerManagement.SystemWillSleep += HandleSystemWillSleep;
                systemPowerManagement.SystemWillPowerOff += HandleSystemWillPowerOff;
                systemPowerManagement.SystemDidPowerOn += HandleSystemDidPowerOn;
            }
            PortNotifier.Start();
        }

        /// <summary>
        /// Stops the serial port monitor.
        /// </summary>
        private void StopSerialPortMonitor()
        {
            DebugOutput("IOService: StopSerialPortMonitor");
            var systemPowerManagement = PortNotifier as ISystemPowerManagement;
            if (systemPowerManagement != null)
            {
                systemPowerManagement.SystemDidPowerOn -= HandleSystemDidPowerOn;
                systemPowerManagement.SystemWillPowerOff -= HandleSystemWillPowerOff;
                systemPowerManagement.SystemWillSleep -= HandleSystemWillSleep;
            }
            PortNotifier.Stop();
            PortNotifier = null;
        }

        private void HandleSystemWillSleep(object sender, SystemWillSleepEventArgs args)
        {
            DeviceChange.RaiseSystemWillSleepEvent(sender, args);
        }

        private void HandleSystemWillPowerOff(object sender, EventArgs args)
        {
            DeviceChange.RaiseSystemWillPowerOffEvent(sender, args);
        }

        private void HandleSystemDidPowerOn(object sender, EventArgs args)
        {
            DeviceChange.RaiseSystemDidPowerOnEvent(sender, args);
        }

        private class PortNotifierThread : NSThread
        {
            public PortNotifierThread(SerialPortNotifier portNotifier)
            {
                PortNotifier = portNotifier;
            }

            private SerialPortNotifier PortNotifier { get; set; }

            public override void Main()
            {
                DebugOutput("IOService: PortNotifier thread enter.");
                using (var pool = new NSAutoreleasePool())
                {
                    PortNotifier.StartInThread();
                    DebugOutput("IOService: PortNotifier run loop running");
                    var runLoop = NSRunLoop.Current;
                    runLoop.Run();
                }
                DebugOutput("IOService: PortNotifier thread exit.");
            }

            public void Stop()
            {
                this.PerformSelector(new Selector("StopPortMonitor:"), this, this, true);
                PortNotifier = null;
                this.Cancel();
            }

            [Export("StopPortMonitor:")]
            private void StopPortMonitor(NSObject data)
            {
                PortNotifier.StopInThread();
            }
        }

        /// <summary>
        /// Provides access to system power state events implemented using NSWorkspace observers.
        /// The observers must run in the UI thread. This acts as a proxy implementation of the
        /// ISystemPowerManagement interface for non-IOKit serial port notification services.
        /// </summary>
        private class NSWorkspaceSystemPowerObserver
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.Shared.Interop.IOKit.IOService+NSWorkspaceSystemPowerObserver"/> class.
            /// </summary>
            /// <param name="willSleepHander">The handler to call when the system will enter sleep mode.</param>
            /// <param name="willPowerOffHandler">The handler to call when the system Will power off.</param>
            /// <param name="didPowerOnHandler">The handler to call when the system has powered on.</param>
            public NSWorkspaceSystemPowerObserver(Action<SystemWillSleepEventArgs> willSleepHander, Action willPowerOffHandler, Action didPowerOnHandler)
            {
                WillSleepHandler = willSleepHander;
                WillPowerOffHandler = willPowerOffHandler;
                DidPowerOnHandler = didPowerOnHandler;
            }

            private NSObject WillSleepObserver { get; set; }
            private NSObject WillPowerOffObserver { get; set; }
            private NSObject DidWakeObserver { get; set; }
            private Action<SystemWillSleepEventArgs> WillSleepHandler { get; set; }
            private Action WillPowerOffHandler { get; set; }
            private Action DidPowerOnHandler { get; set; }

            /// <summary>
            /// Start observers for NSWorkspace system power events.
            /// </summary>
            public void Start()
            {
                WillPowerOffObserver = NSWorkspace.Notifications.ObserveWillPowerOff(OnWillPowerOff);
                WillSleepObserver = NSWorkspace.Notifications.ObserveWillSleep(OnWillSleep);
                DidWakeObserver = NSWorkspace.Notifications.ObserveDidWake(OnDidWake);
            }

            // Stop observers for NSWorkspace system power events.
            public void Stop()
            {
                NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(DidWakeObserver);
                NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(WillSleepObserver);
                NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(WillPowerOffObserver);
                DidWakeObserver = null;
                WillSleepObserver = null;
                WillPowerOffObserver = null;
            }

            private void OnWillPowerOff(object sender, NSNotificationEventArgs e)
            {
                DebugOutput("NSWorkspaceSystemPowerObserver.OnWillPowerOff: name: " + e.Notification.Name);
                WillPowerOffHandler();
            }

            private void OnWillSleep(object sender, NSNotificationEventArgs e)
            {
                DebugOutput("NSWorkspaceSystemPowerObserver.OnWillSleep: name: " + e.Notification.Name);
                WillSleepHandler(new SystemWillSleepEventArgs(false));
            }

            private void OnDidWake(object sender, NSNotificationEventArgs e)
            {
                DebugOutput("NSWorkspaceSystemPowerObserver.OnDidWake: name: " + e.Notification.Name);
                DidPowerOnHandler();
            }
        }

        /// <summary>
        /// A simple interface for starting and stopping serial port arrival / departure notifications.
        /// </summary>
        private interface ISerialPortNotifier
        {
            /// <summary>
            /// Start the serial port notification service.
            /// </summary>
            void Start();

            /// <summary>
            /// Stop the serial port notification service.
            /// </summary>
            void Stop();
        }

        /// <summary>
        /// Provides common implementation for different incarnations of serial port notifications. If a
        /// particular mechanism for detecing port arrival / departure proves unreliable, it's possible to
        /// choose an alternative implementation.
        /// </summary>
        private abstract class SerialPortNotifier : NSObject, ISerialPortNotifier, ISystemPowerManagement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOService+SerialPortNotifier"/> class.
            /// </summary>
            protected SerialPortNotifier()
            {
                NotifierThread = new PortNotifierThread(this);
            }

            /// <summary>
            /// Gets the notifier thread upon which the notification work runs.
            /// </summary>
            protected PortNotifierThread NotifierThread { get; private set; }

            /// <inheritdoc/>
            public event EventHandler<SystemWillSleepEventArgs> SystemWillSleep;

            /// <inheritdoc/>
            public event EventHandler<EventArgs> SystemWillPowerOff;

            /// <inheritdoc/>
            public event EventHandler<EventArgs> SystemDidPowerOn;

            /// <inheritdoc/>
            public virtual void Start()
            {
                NotifierThread.Start();
            }

            /// <inheritdoc/>
            public virtual void Stop()
            {
                NotifierThread.Stop();
            }

            /// <summary>
            /// Starts the implementation's worker thread.
            /// </summary>
            internal abstract void StartInThread();

            /// <summary>
            /// Stops the implementation's worker thread.
            /// </summary>
            internal abstract void StopInThread();

            /// <summary>
            /// Raises the system will sleep event.
            /// </summary>
            /// <param name="args">The arguments to send for the event.</param>
            protected virtual void OnSystemWillSleep(SystemWillSleepEventArgs args)
            {
                DebugOutput("IOServer.SerialPortNotifier raising: OnSystemWillSleep: CanCancel: " + args.CanCancel);
                var systemWillSleep = SystemWillSleep;
                if (systemWillSleep != null)
                {
                    systemWillSleep(this, args);
                }
            }

            /// <summary>
            /// Raises the system will power off event.
            /// </summary>
            protected virtual void OnSystemWillPowerOff()
            {
                DebugOutput("IOServer.SerialPortNotifier raising: OnSystemWillPowerOff");
                var systemWillPowerOff = SystemWillPowerOff;
                if (systemWillPowerOff != null)
                {
                    systemWillPowerOff(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Raises the system did power on event.
            /// </summary>
            protected virtual void OnSystemDidPowerOn()
            {
                DebugOutput("IOServer.SerialPortNotifier raising: OnSystemDidPowerOn");
                var systemDidPowerOn = SystemDidPowerOn;
                if (systemDidPowerOn != null)
                {
                    systemDidPowerOn(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Implements serial port arrival and departure notifications using a file system watcher.
        /// </summary>
        private class FileSystemNotifcationPort : SerialPortNotifier
        {
            private System.IO.FileSystemWatcher DevWatcher { get; set; }
            private RunLoopSource CFRLSource { get; set; }
            private NSWorkspaceSystemPowerObserver SystemPowerObserver { get; set; }

            /// <inheritdoc/>
            public override void Start()
            {
                base.Start();
                SystemPowerObserver = new NSWorkspaceSystemPowerObserver(OnSystemWillSleep, OnSystemWillPowerOff, OnSystemDidPowerOn);
                SystemPowerObserver.Start();
            }

            /// <inheritdoc/>
            public override void Stop()
            {
                SystemPowerObserver.Stop();
                SystemPowerObserver = null;
                base.Stop();
            }

            /// <inheritdoc/>
            internal override void StartInThread()
            {
                // We use our own custom (no-op) CFRunLoopSource so the NSThread we're started in won't immediately return.
                // Strictly speaking, this isn't necessary, since the FileSystemWatcher maintains its own thread and the
                // callbacks all occur on that thread. This is done to maintain consistency so the shutdown can be done
                // on the same thread as the start.
                CFRLSource = new RunLoopSource();
                CFRunLoop.Current.AddSource(CFRLSource, (NSString)CFRunLoop.ModeDefault);

                // The standard operation of FileSystemWatcher tries to open
                // all the files (which turns out to be all the ports) in a
                // blocking fasion -- specifically:
                // fd = open (fullPathNoLastSlash, O_EVTONLY, 0)
                // Wonder if adding O_NONBLOCK would help?
                System.Environment.SetEnvironmentVariable("MONO_MANAGED_WATCHER", "1");
                DevWatcher = new System.IO.FileSystemWatcher("/dev", "cu.*");
                DevWatcher.IncludeSubdirectories = false;
                DevWatcher.Created += DevTtyCreated;
                DevWatcher.Deleted += DevTtyDeleted;
                DevWatcher.NotifyFilter = (System.IO.NotifyFilters)0;
                DevWatcher.EnableRaisingEvents = true;
            }

            /// <inheritdoc/>
            internal override void StopInThread()
            {
                DebugOutput("IOService.FileSystemNotifcationPort: StopInThread");

                DevWatcher.EnableRaisingEvents = false;

                DevWatcher.Deleted -= DevTtyDeleted;
                DevWatcher.Created -= DevTtyCreated;

                DevWatcher.Dispose();
                DevWatcher = null;

                CFRunLoop.Current.RemoveSource(CFRLSource, (NSString)CFRunLoop.ModeDefault);
                CFRLSource = null;
            }

            private void DevTtyCreated(object sender, System.IO.FileSystemEventArgs e)
            {
                DebugOutput("IOService.FileSystemNotifcationPort: DevTtyCreated: " + e.FullPath);
                ReportPortArrival(e.FullPath);
            }

            private void DevTtyDeleted(object sender, System.IO.FileSystemEventArgs e)
            {
                DebugOutput("IOService.FileSystemNotifcationPort: DevTtyDeleted: " + e.FullPath);
                ReportPortDeparture(e.FullPath);
            }

            private class RunLoopSource : CFRunLoopSourceCustom
            {
                protected override void OnSchedule(CFRunLoop loop, CFRunLoopString mode)
                {
                    DebugOutput("IOService.FileSystemNotifcationPort.OnSchedule()");
                }

                protected override void OnPerform()
                {
                    DebugOutput("IOService.FileSystemNotifcationPort.OnPerform()");
                }

                protected override void OnCancel(CFRunLoop loop, CFRunLoopString mode)
                {
                    DebugOutput("IOService.FileSystemNotifcationPort.OnCancel()");
                }
            }
        }

        /// <summary>
        /// Implements serial port arrival and departure notifications using the IORegistry.
        /// </summary>
        private class IORegNotification : SerialPortNotifier
        {
            private NSWorkspaceSystemPowerObserver SystemPowerObserver { get; set; }

            #region SerialPortNotifier

            /// <inheritdoc/>
            public override void Start()
            {
                base.Start();
                SystemPowerObserver = new NSWorkspaceSystemPowerObserver(OnSystemWillSleep, OnSystemWillPowerOff, OnSystemDidPowerOn);
                SystemPowerObserver.Start();
            }

            /// <inheritdoc/>
            public override void Stop()
            {
                SystemPowerObserver.Stop();
                SystemPowerObserver = null;
                base.Stop();
            }

            /// <inheritdoc/>
            internal override void StartInThread()
            {
                throw new NotImplementedException("IORegNotification: StartInThread not implemented.");
            }

            /// <inheritdoc/>
            internal override void StopInThread()
            {
                throw new NotImplementedException("IORegNotification: StopInThread not implemented.");
            }

            #endregion // SerialPortNotifier
        }

        private class IOKitNotificationPort : SerialPortNotifier
        {
            private IONotificationPort NotificationPort { get; set; }
            private IOIterator PublishNotificationIterator { get; set; }
            private IOIterator TerminateNotificationIterator { get; set; }
            private IOConnect IOConnectionPort { get; set; }

            /// <inheritdoc/>
            internal override void StartInThread()
            {
                NotificationPort = new IONotificationPort();
                Interop.NativeMethods.CFRunLoopAddSource(CFRunLoop.Current, NotificationPort.RunLoopSource, (NSString)CFRunLoop.ModeDefault);

                var systemPowerDelegate = new IOServiceInterestCallback(SystemPowerInterestCallback);
                IOConnectionPort = IOConnect.CreateSystemPowerMonitorConnection(systemPowerDelegate, this);
                Interop.NativeMethods.CFRunLoopAddSource(CFRunLoop.Current, IOConnectionPort.NotificationPort.RunLoopSource, (NSString)CFRunLoop.ModeCommon);

                var servicesDictionary = IOMachPort.GetRS232SerialMatchDictionary();
#if __UNIFIED__
                servicesDictionary.DangerousRetain(); // retain an extra time because we're using it twice
#else
                servicesDictionary.Retain(); // retain an extra time because we're using it twice
#endif // __UNIFIED__
                var publishDelegate = new IONotificationPortCallback(FirstMatchNotification);
                var callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(publishDelegate);

                IntPtr iterator;
                var result = NativeMethods.IOServiceAddMatchingNotification(NotificationPort.Handle, KIOFirstMatchNotification, servicesDictionary.Handle, callback, IntPtr.Zero, out iterator);
                System.Diagnostics.Debug.Assert(result == NativeMethods.Success, "IOService.IOKitNotificationPort: Failed to add notification.");

                if (result == NativeMethods.Success)
                {
                    PublishNotificationIterator = new IOIterator(iterator);

                    var terminateDelegate = new IONotificationPortCallback(TerminateNotification);
                    callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(terminateDelegate);
                    result = NativeMethods.IOServiceAddMatchingNotification(NotificationPort.Handle, KIOTerminatedNotification, servicesDictionary.Handle, callback, IntPtr.Zero, out iterator);
                    TerminateNotificationIterator = new IOIterator(iterator);

                    // The iterators returned when adding the matching notifications must be iterated to
                    // completion in order to arm the notifications. Otherwise, they will never fire.
                    PublishNotificationIterator.EnumerateSerialPorts(null);
                    TerminateNotificationIterator.EnumerateSerialPorts(null);
                }
            }

            internal override void StopInThread()
            {
                PublishNotificationIterator.Dispose();
                PublishNotificationIterator = null;

                TerminateNotificationIterator.Dispose();
                TerminateNotificationIterator = null;

                Interop.NativeMethods.CFRunLoopRemoveSource(CFRunLoop.Current, NotificationPort.RunLoopSource, (NSString)CFRunLoop.ModeDefault);

                NotificationPort.Dispose();
                NotificationPort = null;

                Interop.NativeMethods.CFRunLoopRemoveSource(CFRunLoop.Current, IOConnectionPort.NotificationPort.RunLoopSource, (NSString)CFRunLoop.ModeCommon);

                IOConnectionPort.Dispose();
                IOConnectionPort = null;
            }

            private static void FirstMatchNotification(IntPtr refcon, IntPtr iteratorPtr)
            {
                DebugOutput("IOService.IOKitNotificationPort: FirstMatchNotification");
                var iterator = new IOIterator(iteratorPtr);
                var newPorts = iterator.EnumerateSerialPorts(IOKitHelpers.BluetoothPortsExclusionFilter);
                foreach (var port in newPorts)
                {
                    ReportPortArrival(port);
                }
            }

            private static void TerminateNotification(IntPtr refcon, IntPtr iteratorPtr)
            {
                DebugOutput("IOService.IOKitNotificationPort: TerminateNotification");
                var iterator = new IOIterator(iteratorPtr);
                var newPorts = iterator.EnumerateSerialPorts(IOKitHelpers.BluetoothPortsExclusionFilter);
                foreach (var port in newPorts)
                {
                    ReportPortDeparture(port);
                }
            }

            // see https://developer.apple.com/library/content/qa/qa1340/_index.html
            // see https://developer.apple.com/documentation/iokit/1557114-ioregisterforsystempower?preferredLanguage=occ
            private static void SystemPowerInterestCallback(IntPtr refcon, IntPtr service, uint messageType, IntPtr messageArgument)
            {
                var message = (IOMessage)messageType;
                DebugOutput("IOService: SystemPowerInterestCallback: " + message);
                var refconObject = (NSObject)Runtime.GetNSObject(refcon);
                var serialPortNotifier = refconObject as IOKitNotificationPort;
                switch (message)
                {
                    case IOMessage.KIOMessageCanSystemSleep:
                        // From documentation for IORegisterForSystemPower
                        // -----------------------------------------------
                        // Indicates the system is pondering an idle sleep, but gives apps the chance to veto that sleep attempt.
                        // Caller must acknowledge kIOMessageCanSystemSleep by calling IOAllowPowerChange or IOCancelPowerChange.
                        // Calling IOAllowPowerChange will not veto the sleep; any app that calls IOCancelPowerChange will veto
                        // the idle sleep. A kIOMessageCanSystemSleep notification will be followed up to 30 seconds later by
                        // a kIOMessageSystemWillSleep message, or a kIOMessageSystemWillNotSleep message.
                        // -----------------------------------------------

                        // From example code:
                        // ------------------
                        // Idle sleep is about to kick in. This message will not be sent for forced sleep.
                        // Applications have a chance to prevent sleep by calling IOCancelPowerChange.
                        // Most applications should not prevent idle sleep.

                        // Power Management waits up to 30 seconds for you to either allow or deny idle
                        // sleep. If you don't acknowledge this power change by calling either
                        // IOAllowPowerChange or IOCancelPowerChange, the system will wait 30
                        // seconds then go to sleep.
                        // ------------------
                        var systemWillSleepArgs = new SystemWillSleepEventArgs(canCancel: true);
                        serialPortNotifier.OnSystemWillSleep(systemWillSleepArgs);
                        DebugOutput("IOService: SystemPowerInterestCallback: " + message + " CancelSleep: " + systemWillSleepArgs.Cancel);
                        if (systemWillSleepArgs.Cancel)
                        {
                            serialPortNotifier.IOConnectionPort.CancelPowerChange((nint)messageArgument);
                        }
                        else
                        {
                            serialPortNotifier.IOConnectionPort.AllowPowerChange((nint)messageArgument);
                        }
                        break;
                    case IOMessage.KIOMessageSystemWillSleep:
                        // From documentation for IORegisterForSystemPower
                        // -----------------------------------------------
                        // Delivered at the point the system is initiating a non-abortable sleep.
                        // Callers MUST acknowledge this event by calling IOAllowPowerChange.
                        // If a caller does not acknowledge the sleep notification, the sleep will continue anyway
                        // after a 30 second timeout (resulting in bad user experience). Delivered before any
                        // hardware is powered off.
                        // -----------------------------------------------

                        // From example code:
                        // ------------------
                        // The system WILL go to sleep. If you do not call IOAllowPowerChange or
                        // IOCancelPowerChange to acknowledge this message, sleep will be
                        // delayed by 30 seconds.

                        // NOTE: If you call IOCancelPowerChange to deny sleep it returns
                        // kIOReturnSuccess, however the system WILL still go to sleep.
                        // ------------------
                        serialPortNotifier.OnSystemWillSleep(new SystemWillSleepEventArgs(canCancel: false));
                        serialPortNotifier.IOConnectionPort.AllowPowerChange((nint)messageArgument);
                        break;
                    case IOMessage.KIOMessageSystemWillNotSleep:
                        // From documentation for IORegisterForSystemPower
                        // -----------------------------------------------
                        // Delivered when some app client has vetoed an idle sleep request.
                        // kIOMessageSystemWillNotSleep may follow a kIOMessageCanSystemSleep notification,
                        // but will not otherwise be sent. Caller must NOT acknowledge kIOMessageSystemWillNotSleep;
                        // the caller must simply return from its handler.
                        // -----------------------------------------------
                        break;
                    case IOMessage.KIOMessageSystemWillPowerOn:
                        // From documentation for IORegisterForSystemPower
                        // -----------------------------------------------
                        // Delivered at early wakeup time, before most hardware has been powered on.
                        // Be aware that any attempts to access disk, network, the display, etc. may result in
                        // errors or blocking your process until those resources become available. Caller
                        // must NOT acknowledge kIOMessageSystemWillPowerOn; the caller must simply return from its handler.
                        // -----------------------------------------------

                        // From example code:
                        // ------------------
                        // System has started the wake up process...
                        // ------------------
                        break;
                    case IOMessage.KIOMessageSystemHasPoweredOn:
                        // From documentation for IORegisterForSystemPower
                        // -----------------------------------------------
                        // Delivered at wakeup completion time, after all device drivers and hardware have handled the wakeup event.
                        // Expect this event 1-5 or more seconds after initiating system wakeup.
                        // Caller must NOT acknowledge kIOMessageSystemHasPoweredOn; the caller must simply return from its handler.
                        // -----------------------------------------------

                        // From example code:
                        // ------------------
                        // System has finished waking up...
                        // ------------------
                        serialPortNotifier.OnSystemDidPowerOn();
                        break;
                    default:
                        break;
                }
            }

            [Export("StopPortMonitor:")]
            private void StopPortMonitor(NSObject data)
            {
                StopInThread();
            }
        }

#if USES_PLIST_PARSER
        /// <summary>
        /// A handy class for getting information from a .plist file.
        /// </summary>
        /// <remarks>The following is from: http://www.codeproject.com/Tips/406235/A-Simple-PList-Parser-in-Csharp </remarks>
        public class PList : Dictionary<string, dynamic>
        {
            public PList()
            {
            }

            public PList(string file)
            {
                Load(file);
            }

            public void Load(string file)
            {
                var doc = XDocument.Load(file);
                Initialize(doc);
            }

            public void Parse(string text)
            {
                var doc = XDocument.Parse(text);
                Initialize(doc);
            }

            private void Initialize(XDocument doc)
            {
                Clear();
                var plist = doc.Element("plist");
                var dict = plist.Element("dict");
                var dictElements = dict.Elements();
                Parse(this, dictElements);
            }

            private void Parse(PList dict, IEnumerable<XElement> elements)
            {
                for (int i = 0; i < elements.Count(); i += 2)
                {
                    XElement key = elements.ElementAt(i);
                    XElement val = elements.ElementAt(i + 1);

                    dict[key.Value] = ParseValue(val);
                }
            }

            private List<dynamic> ParseArray(IEnumerable<XElement> elements)
            {
                List<dynamic> list = new List<dynamic>();
                foreach (XElement e in elements)
                {
                    dynamic one = ParseValue(e);
                    list.Add(one);
                }

                return list;
            }

            private dynamic ParseValue(XElement val)
            {
                switch (val.Name.ToString())
                {
                    case "string":
                        return val.Value;
                    case "integer":
                        return long.Parse(val.Value);
//                        return int.Parse(val.Value);
                    case "real":
                        return float.Parse(val.Value);
                    case "true":
                        return true;
                    case "false":
                        return false;
                    case "dict":
                        PList plist = new PList();
                        Parse(plist, val.Elements());
                        return plist;
                    case "array":
                        List<dynamic> list = ParseArray(val.Elements());
                        return list;
                    case "data":
                        var nsData = NSData.FromString(val.Value);
                        return nsData;
                    default:
                        DebugOutput("Unknown type: " + val.Name);
                        throw new ArgumentException("Unsupported");
                }
            }
        }
#endif // USES_PLIST_PARSER

#if EXAMPLE_OBJC_CODE
        // From: http://www.cocoabuilder.com/archive/cocoa/116275-serial-port-added-notification.html

        void FTSWSerialPortRegistry_SerialPortServicePublished(void *refcon, io_iterator_t iterator)
        {
            io_object_t serialPortService;
            while (serialPortService = IOIteratorNext(iterator))
            {
                [(FTSWSerialPortRegistry*)refcon serialPortServicePublished:serialPortService];
                IOObjectRelease(serialPortService);
            }
        }

        void FTSWSerialPortRegistry_SerialPortServiceTerminated(void *refcon, io_iterator_t iterator)
        {
            io_object_t serialPortService;
            while (serialPortService = IOIteratorNext(iterator))
            {
                [(FTSWSerialPortRegistry*)refcon serialPortServiceTerminated:serialPortService];
                IOObjectRelease(serialPortService);
            }
        }

        - (void) updateDynamicDiscovery
        {
            if (dynamicDiscovery)
            {
                notifcationPort = IONotificationPortCreate(kIOMasterPortDefault);

                CFRunLoopAddSource(CFRunLoopGetCurrent(), IONotificationPortGetRunLoopSource(notifcationPort), kCFRunLoopDefaultMode);

                CFMutableDictionaryRef serialPortMatchDictionary = IOServiceMatching(kIOSerialBSDServiceValue);

                CFDictionarySetValue(serialPortMatchDictionary, CFSTR(kIOSerialBSDTypeKey), CFSTR(kIOSerialBSDAllTypes));

                CFRetain(serialPortMatchDictionary); //the following method consume references, so we need 2

                IOServiceAddMatchingNotification(notifcationPort, kIOFirstMatchNotification, serialPortMatchDictionary,
                    FTSWSerialPortRegistry_SerialPortServicePublished, self, &publishedNotifcationIterator);

                IOServiceAddMatchingNotification(notifcationPort, kIOTerminatedNotification, serialPortMatchDictionary, 
                    FTSWSerialPortRegistry_SerialPortServiceTerminated, self, &terminatedNotficationIterator);

                io_object_t serialPortService;

                while (serialPortService = IOIteratorNext(publishedNotifcationIterator))
                {
                    [self serialPortServicePublished:serialPortService];
                    IOObjectRelease(serialPortService);
                }

                while (serialPortService = IOIteratorNext(terminatedNotficationIterator))
                {
                    [self serialPortServiceTerminated:serialPortService];
                    IOObjectRelease(serialPortService);
                }
            }
            else
            {
                IOObjectRelease(publishedNotifcationIterator);
                publishedNotifcationIterator = NULL;

                IOObjectRelease(terminatedNotficationIterator);
                terminatedNotficationIterator = NULL;

                CFRunLoopRemoveSource(CFRunLoopGetCurrent(), IONotificationPortGetRunLoopSource(notifcationPort), kCFRunLoopDefaultMode);

                IONotificationPortDestroy(notifcationPort);
                notifcationPort = NULL;
            }
        }

        - (void)serialPortServicePublished:(io_object_t)serialPortService
        {
            CFTypeRef ttyName =
                IORegistryEntryCreateCFProperty(serialPortService,

                    CFSTR(kIOTTYDeviceKey),

                    kCFAllocatorDefault,
                    0);

                ...blah...

                CFRelease(ttyName);
        }

        - (void)serialPortServiceTerminated:(io_object_t)serialPortService
        {
            CFTypeRef ttyName =
                IORegistryEntryCreateCFProperty(serialPortService,

                    CFSTR(kIOTTYDeviceKey),

                    kCFAllocatorDefault,
                    0);

                ...blah...

                CFRelease(ttyName);
        }
#endif // EXAMPLE_OBJC_CODE
    }
}

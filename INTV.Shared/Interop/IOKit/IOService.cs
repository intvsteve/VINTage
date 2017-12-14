// <copyright file="IOService.cs" company="INTV Funhouse">
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

#define USE_IOKIT_NOTIFICATIONS
////#define ENABLE_DEBUG_OUTPUT

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using INTV.Shared.Interop.DeviceManagement;
#if __UNIFIED__
using CoreFoundation;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__

#if __UNIFIED__
using CFRunLoopString = Foundation.NSString;
#else
using CFRunLoopString = System.String;
#endif // __UNIFIED__

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// Class to act as a C# version of the IOKit IOService type.
    /// </summary>
    public class IOService : IORegistryEntry
    {
        #region IOService notification types

        public static readonly string KIOPublishNotification = "IOServicePublish";
        public static readonly string KIOFirstPublishNotification = "IOServiceFirstPublish";
        public static readonly string KIOMatchedNotification = "IOServiceMatched";
        public static readonly string KIOFirstMatchNotification = "IOServiceFirstMatch";
        public static readonly string KIOTerminatedNotification = "IOServiceTerminate";

        #endregion // IOService notification types

        private delegate void IONotificationPortCallback(System.IntPtr refcon, System.IntPtr iterator);

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOService"/> class.
        /// </summary>
        public IOService()
        {
        }

        private ISerialPortNotifier PortNotifier { get; set; }

        /// <summary>
        /// Gets the IOServices matching the given name.
        /// </summary>
        /// <returns>The IOServices matching the given name.</returns>
        /// <param name="name">The name filter to use to locate IOServices.</param>
        public static NSMutableDictionary MatchingServicesFromName(string name)
        {
            NSMutableDictionary dictionary = null;
            var dictionaryPointer = NativeMethods.IOServiceMatching(name);
            if (dictionaryPointer != System.IntPtr.Zero)
            {
                dictionary = Runtime.GetNSObject(dictionaryPointer) as NSMutableDictionary;
            }
            return dictionary;
        }

        /// <summary>
        /// Starts the serial port monitor.
        /// </summary>
        public void StartSerialPortMonitor()
        {
            DebugOutput("IOService: StartSerialPortMonitor");
            PortNotifier = CreatePortNotifier();
            PortNotifier.Start();
        }

        /// <summary>
        /// Stops the serial port monitor.
        /// </summary>
        public void StopSerialPortMonitor()
        {
            DebugOutput("IOService: StopSerialPortMonitor");
            PortNotifier.Stop();
            PortNotifier = null;
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

        private interface ISerialPortNotifier
        {
            void Start();
            void Stop();
        }

        private abstract class SerialPortNotifier : NSObject, ISerialPortNotifier
        {
            protected PortNotifierThread NotifierThread { get; private set; }

            protected SerialPortNotifier()
            {
                NotifierThread = new PortNotifierThread(this);
            }

            public void Start()
            {
                NotifierThread.Start();
            }

            public void Stop()
            {
                NotifierThread.Stop();
            }

            internal abstract void StartInThread();

            internal abstract void StopInThread();
        }

        private class FileSystemNotifcationPort : SerialPortNotifier
        {
            private System.IO.FileSystemWatcher DevWatcher { get; set; }
            private RunLoopSource CFRLSource { get; set; }

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

        private class IORegNotification : SerialPortNotifier
        {
            #region SerialPortNotifier

            internal override void StartInThread()
            {
                throw new System.NotImplementedException("IORegNotification: StartInThread not implemented.");
            }
            internal override void StopInThread()
            {
                throw new System.NotImplementedException("IORegNotification: StopInThread not implemented.");
            }

            #endregion // SerialPortNotifier
        }

        private class IOKitNotificationPort : SerialPortNotifier
        {
            private IONotificationPort NotificationPort { get; set; }
            private IOIterator PublishNotificationIterator { get; set; }
            private IOIterator TerminateNotificationIterator { get; set; }

            internal override void StartInThread()
            {
                NotificationPort = new IONotificationPort();
                Interop.NativeMethods.CFRunLoopAddSource(CFRunLoop.Current, NotificationPort.RunLoopSource, (NSString)CFRunLoop.ModeDefault);
                var servicesDictionary = IOMachPort.GetRS232SerialMatchDictionary();
#if __UNIFIED__
                servicesDictionary.DangerousRetain(); // retain an extra time because we're using it twice
#else
                servicesDictionary.Retain(); // retain an extra time because we're using it twice
#endif // __UNIFIED__
                var publishDelegate = new IONotificationPortCallback(FirstMatchNotification);
                var callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(publishDelegate);

                System.IntPtr iterator;
                var result = NativeMethods.IOServiceAddMatchingNotification(NotificationPort.Handle, KIOFirstMatchNotification, servicesDictionary.Handle, callback, System.IntPtr.Zero, out iterator);
                System.Diagnostics.Debug.Assert(result == NativeMethods.Success, "Failed to add notification.");

                if (result == NativeMethods.Success)
                {
                    PublishNotificationIterator = new IOIterator(iterator);

                    var terminateDelegate = new IONotificationPortCallback(TerminateNotification);
                    callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(terminateDelegate);
                    result = NativeMethods.IOServiceAddMatchingNotification(NotificationPort.Handle, KIOTerminatedNotification, servicesDictionary.Handle, callback, System.IntPtr.Zero, out iterator);
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
            }

            private static void FirstMatchNotification(System.IntPtr refcon, System.IntPtr iteratorPtr)
            {
                DebugOutput("IOService.IOKitNotificationPort: FirstMatchNotification");
                var iterator = new IOIterator(iteratorPtr);
                var newPorts = iterator.EnumerateSerialPorts(IOKitHelpers.BluetoothPortsExclusionFilter);
                foreach (var port in newPorts)
                {
                    ReportPortArrival(port);
                }
            }

            private static void TerminateNotification(System.IntPtr refcon, System.IntPtr iteratorPtr)
            {
                DebugOutput("IOService.IOKitNotificationPort: TerminateNotification");
                var iterator = new IOIterator(iteratorPtr);
                var newPorts = iterator.EnumerateSerialPorts(IOKitHelpers.BluetoothPortsExclusionFilter);
                foreach (var port in newPorts)
                {
                    ReportPortDeparture(port);
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
                        System.Diagnostics.Debug.WriteLine("Unknown type: " + val.Name);
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

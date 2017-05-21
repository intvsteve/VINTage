// <copyright file="DeviceMonitor.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_DIAGNOSTIC_OUTPUT

using System.Collections.Generic;
#if __UNIFIED__
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__
using INTV.Shared.Interop.DeviceManagement;
using INTV.Shared.Interop.IOKit;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Monitors for device arrival and departure in the system.
    /// </summary>
    public class DeviceMonitor : NSObject
    {
        private DeviceMonitor()
        {
        }

        private static INTV.Shared.Interop.IOKit.IOService IOService { get; set; }
        private static NSObject WillPowerOffObserver { get; set; }
        private static NSObject WillSleepObserver { get; set; }
        private static NSObject DidWakeObserver { get; set; }

        /// <summary>
        /// Starts the device monitor. This includes observing power state changes in the system.
        /// </summary>
        public static void Start()
        {
            WillPowerOffObserver = NSWorkspace.Notifications.ObserveWillPowerOff(OnWillPowerOff);
            WillSleepObserver = NSWorkspace.Notifications.ObserveWillSleep(OnWillSleep);
            DidWakeObserver = NSWorkspace.Notifications.ObserveDidWake(OnDidWake);

            INTV.Shared.Utility.SingleInstanceApplication.Current.Exit += ApplicationWillExit;

            IOService = new IOService();
            IOService.StartSerialPortMonitor();
        }

        private static void ApplicationWillExit (object sender, ExitEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Stops the device monitor.
        /// </summary>
        public static void Stop()
        {
            IOService.StopSerialPortMonitor();
            IOService = null;

            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(DidWakeObserver);
            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(WillSleepObserver);
            NSWorkspace.SharedWorkspace.NotificationCenter.RemoveObserver(WillPowerOffObserver);
            DidWakeObserver = null;
            WillSleepObserver = null;
            WillPowerOffObserver = null;
        }

        private static void OnWillPowerOff(object sender, NSNotificationEventArgs e)
        {
            DebugOutput("OnWillPowerOff: name: " + e.Notification.Name);
            OnWillSleep(sender, e);
        }

        private static void OnWillSleep(object sender, NSNotificationEventArgs e)
        {
            DebugOutput("OnWillSleep: name: " + e.Notification.Name);
            foreach (var port in INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts)
            {
                DeviceChange.SystemReportsDeviceRemoved(null, port, INTV.Core.Model.Device.ConnectionType.Serial);
            }
        }

        private static void OnDidWake(object sender, NSNotificationEventArgs e)
        {
            DebugOutput("OnDidWake: name: " + e.Notification.Name);
            foreach (var port in INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts)
            {
                DeviceChange.SystemReportsDeviceAdded(null, port, INTV.Core.Model.Device.ConnectionType.Serial);
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private class PortNotifierThread : NSThread
        {
            public override void Main()
            {
                var runLoop = NSRunLoop.Current;
                runLoop.Run();
                DebugOutput("DeviceMonitor thread exit.");
            }

            public void Stop()
            {
                this.PerformSelector(new Selector("StopPortMonitor"), this, this, true);
            }

            [Export("StopPortMonitor")]
            private void StopPortMonitor(NSObject data)
            {
            }
        }
    }
}

// <copyright file="DeviceMonitor.Mac.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.Shared.Interop.DeviceManagement;
using INTV.Shared.Interop.IOKit;
using INTV.Shared.Utility;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

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
        private static Func<IEnumerable<Device>> GetDevices { get; set; }

        /// <summary>
        /// Starts the device monitor. This includes observing power state changes in the system.
        /// </summary>
        /// <param name="getDevices">The delegate to use to get the list of Locutus devices.</param>
        public static void Start(Func<IEnumerable<Device>> getDevices)
        {
            GetDevices = getDevices;
            DeviceChange.SystemWillSleep += HandleSystemWillSleep;
            DeviceChange.SystemWillPowerOff += HandleSystemWillPowerOff;
            DeviceChange.SystemDidPowerOn += HandleSystemDidWake;

            INTV.Shared.Utility.SingleInstanceApplication.Current.Exit += ApplicationWillExit;

            IOService = new IOService();
            IOService.StartServices(IOServices.All);
        }

        private static void ApplicationWillExit(object sender, ExitEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Stops the device monitor.
        /// </summary>
        public static void Stop()
        {
            IOService.StopAllServices();
            IOService = null;

            DeviceChange.SystemDidPowerOn -= HandleSystemDidWake;
            DeviceChange.SystemWillSleep -= HandleSystemWillSleep;
            DeviceChange.SystemWillPowerOff -= HandleSystemWillPowerOff;
        }

        private static void HandleSystemWillPowerOff(object sender, EventArgs e)
        {
            DebugOutput("DeviceMonitor.HandleSystemWillPowerOff");
            HandleSystemWillSleep(sender, new SystemWillSleepEventArgs(canCancel: false));
        }

        private static void HandleSystemWillSleep(object sender, SystemWillSleepEventArgs e)
        {
            DebugOutput("DeviceMonitor.HandleSystemWillSleep CanCancel: " + e.CanCancel + " PreventSystemSleepDuringDeviceCommands: " + Properties.Settings.Default.PreventSystemSleepDuringDeviceCommands);
            var cancelSleep = false;
            if (e.CanCancel && Properties.Settings.Default.PreventSystemSleepDuringDeviceCommands)
            {
                var portBeingUsed = GetDevices().FirstOrDefault(d => d.IsValid && (d.Port != null) && d.Port.IsOpen && d.Port.IsInUse);
                cancelSleep = portBeingUsed != null;
                e.Cancel = cancelSleep;
                DebugOutput("DeviceMonitor.HandleSystemWillSleep will cancel: " + cancelSleep + " port being used: " + (cancelSleep ? portBeingUsed.Name : "<null>"));
            }
            if (!cancelSleep)
            {
                foreach (var port in INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts)
                {
                    DeviceChange.SystemReportsDeviceRemoved(null, port, INTV.Core.Model.Device.ConnectionType.Serial);
                }
            }
        }

        private static void HandleSystemDidWake(object sender, EventArgs e)
        {
            DebugOutput("DeviceMonitor.HandleSystemDidWake");
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
    }
}

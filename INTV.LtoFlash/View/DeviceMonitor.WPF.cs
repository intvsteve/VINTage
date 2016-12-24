// <copyright file="DeviceMonitor.WPF.cs" company="INTV Funhouse">
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

////#define ENABLE_DIAGNOSTIC_OUTPUT

using System;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implements a message window used to receive Device Monitor messages for device arrival and departure.
    /// </summary>
    public class DeviceMonitor : System.Windows.Window
    {
        private System.Windows.Interop.HwndSource _windowSource;

        private DeviceMonitor()
        {
            Width = 0;
            Height = 0;
            WindowStyle = System.Windows.WindowStyle.None;
            ShowInTaskbar = false;
            ShowActivated = false;
            Visibility = System.Windows.Visibility.Hidden;
            Title = "VINTage Device Monitor";
        }

        /// <summary>
        /// Starts the device monitor window.
        /// </summary>
        public static void Start()
        {
            System.Windows.Application.Current.Exit += HandleApplicationExit;
            Microsoft.Win32.SystemEvents.PowerModeChanged += HandlePowerModeChanged;
            Microsoft.Win32.SystemEvents.SessionEnding += HandleSessionEnding;
            var monitorThread = new System.Threading.Thread(MonitorThread);
            monitorThread.SetApartmentState(System.Threading.ApartmentState.STA);
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        private static void MonitorThread()
        {
            var window = new DeviceMonitor();
            window.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        private static void HandleSessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            DebugOutput("Session ending: " + e.Reason);
            HandlePowerModeChanged(sender, new Microsoft.Win32.PowerModeChangedEventArgs(Microsoft.Win32.PowerModes.Suspend));
        }

        private static void HandlePowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            DebugOutput("Power state change: " + e.Mode);
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Resume:
                    foreach (var port in INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts)
                    {
                        INTV.Shared.Interop.DeviceManagement.DeviceChange.SystemReportsDeviceAdded(null, port, INTV.Core.Model.Device.ConnectionType.Serial);
                    }
                    break;
                case Microsoft.Win32.PowerModes.Suspend:
                    foreach (var port in INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts)
                    {
                        INTV.Shared.Interop.DeviceManagement.DeviceChange.SystemReportsDeviceRemoved(null, port, INTV.Core.Model.Device.ConnectionType.Serial);
                    }
                    break;
                default:
                    break;
            }
        }

        private static void HandleApplicationExit(object sender, System.Windows.ExitEventArgs e)
        {
            Microsoft.Win32.SystemEvents.SessionEnding -= HandleSessionEnding;
            Microsoft.Win32.SystemEvents.PowerModeChanged -= HandlePowerModeChanged;
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            // When actually debugging this, using INTV.Shared.Utility.Logger is the way to go.
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <inheritdoc />
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowSource = System.Windows.Interop.HwndSource.FromDependencyObject(this) as System.Windows.Interop.HwndSource;
            Visibility = System.Windows.Visibility.Hidden;
            _windowSource.AddHook(INTV.Shared.Interop.DeviceManagement.DeviceChange.Handler);
        }
    }
}

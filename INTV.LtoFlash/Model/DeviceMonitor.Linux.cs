// <copyright file="DeviceMonitor.Linux.cs" company="INTV Funhouse">
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

using INTV.Shared.Interop.DeviceManagement;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    public class DeviceMonitor
    {
        private static ISerialPortNotifier _serialPortNotifier;

        private DeviceMonitor()
        {
        }

        /// <summary>
        /// Starts the device monitor. This includes observing power state changes in the system.
        /// </summary>
        public static void Start()
        {
            // TODO: Refactor the file system watcher approach to be in INTV.Shared ... coordinate
            // with changes to Mac. This is totally generic and should not be hidden here in the
            // LTO Flash! component. The FileSystemMonitor could likely also be reworked to be shared
            // between Mac and Linux, though the Mac's IOKit implementation is holding up nicely.
            DebugOutput("!!!!!DeviceMonitor.Start()");
            if (_serialPortNotifier == null)
            {
                _serialPortNotifier = new FileSystemSerialPortNotifier();
                _serialPortNotifier.Start();
                SingleInstanceApplication.Instance.Exit += HandleApplicationExit;
            }
        }

        /// <summary>
        /// Stops the device monitor.
        /// </summary>
        public static void Stop()
        {
            var serialPortNotifier = _serialPortNotifier;
            if (serialPortNotifier != null)
            {
                _serialPortNotifier = null;
                serialPortNotifier.Stop();
            }
        }

        private static void HandleApplicationExit (object sender, ExitEventArgs e)
        {
            Stop();
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private interface ISerialPortNotifier
        {
            /// <summary>
            /// Start the notifier.
            /// </summary>
            void Start();

            /// <summary>
            /// Stop the notifier.
            /// </summary>
            void Stop();
        }

        /// <summary>
        /// Borrowed and modified from the Mac version.
        /// </summary>
        private abstract class SerialPortNotifier : ISerialPortNotifier
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.DeviceMonitor+SerialPortNotifier"/> class.
            /// </summary>
            protected SerialPortNotifier()
            {
            }

            /// <inheritdoc/>
            public abstract void Start();

            /// <inheritdoc/>
            public abstract void Stop();
        }

        /// <summary>
        /// File system-based serial port notifier.
        /// </summary>
        private class FileSystemSerialPortNotifier : SerialPortNotifier
        {
            private System.IO.FileSystemWatcher DevWatcher { get; set; }

            /// <inheritdoc/>
            public sealed override void Start()
            {
                DebugOutput("DeviceMonitor.FileSystemNotifcationPort: Start");

                // The standard operation of FileSystemWatcher tries to open
                // all the files (which turns out to be all the ports) in a
                // blocking fasion -- specifically:
                // fd = open (fullPathNoLastSlash, O_EVTONLY, 0)
                // Wonder if adding O_NONBLOCK would help?
                //System.Environment.SetEnvironmentVariable ("MONO_MANAGED_WATCHER", "1");
                DevWatcher = new System.IO.FileSystemWatcher("/dev", "tty*");
                DevWatcher.IncludeSubdirectories = false;
                DevWatcher.Created += DevTtyCreated;
                DevWatcher.Deleted += DevTtyDeleted;
                DevWatcher.NotifyFilter = System.IO.NotifyFilters.CreationTime; // (System.IO.NotifyFilters)0;
                DevWatcher.EnableRaisingEvents = true;
            }

            /// <inheritdoc/>
            public sealed override void Stop()
            {
                DebugOutput("DeviceMonitor.FileSystemNotifcationPort: Stop");

                DevWatcher.EnableRaisingEvents = false;

                DevWatcher.Deleted -= DevTtyDeleted;
                DevWatcher.Created -= DevTtyCreated;

                DevWatcher.Dispose();
                DevWatcher = null;
            }

            private void DevTtyCreated (object sender, System.IO.FileSystemEventArgs e)
            {
                if (e.ChangeType.HasFlag(System.IO.WatcherChangeTypes.Created))
                {
                    DebugOutput("DeviceMonitor.FileSystemNotifcationPort: DevTtyCreated: " + e.FullPath);
                    DeviceChange.SystemReportsDeviceAdded(this, e.FullPath, INTV.Core.Model.Device.ConnectionType.Serial);
                }
            }

            private void DevTtyDeleted (object sender, System.IO.FileSystemEventArgs e)
            {
                if (e.ChangeType.HasFlag(System.IO.WatcherChangeTypes.Deleted))
                {
                    DebugOutput("DeviceMonitor.FileSystemNotifcationPort: DevTtyDeleted: " + e.FullPath);
                    DeviceChange.SystemReportsDeviceRemoved(this, e.FullPath, INTV.Core.Model.Device.ConnectionType.Serial);
                }
            }
        }
    }
}

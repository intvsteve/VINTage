// <copyright file="DeviceChange.WPF.cs" company="INTV Funhouse">
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

using System;
using System.Windows.Interop;

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public static partial class DeviceChange
    {
        /// <summary>
        /// This function may be used as a hook for an HwndSource.AddHook() call in order to receive WM_DEVICECHANGE notifications.
        /// </summary>
        /// <param name="windowHandle">The Win32 window that receives a Win32 message.</param>
        /// <param name="message">The Win32 message</param>
        /// <param name="wordParameter">The Win32 WORD parameter for the message.</param>
        /// <param name="longParameter">the Win32 LPARAM parameter for the message.</param>
        /// <param name="handled">Unchanged (set to true if the function handles the message and wishes to prevent further processing).</param>
        /// <returns>This function returns <c>IntPtr.Zero</c>.</returns>
        public static IntPtr Handler(IntPtr windowHandle, int message, IntPtr wordParameter, IntPtr longParameter, ref bool handled)
        {
            if (message == NativeMethods.WM_DEVICECHANGE)
            {
                var deviceChangeEvent = (DeviceChangeEvent)wordParameter.ToInt32();

                switch (deviceChangeEvent)
                {
                    case DeviceChangeEvent.DBT_DEVICEARRIVAL:
                        var deviceHeader = (DeviceBroadcastHeader)System.Runtime.InteropServices.Marshal.PtrToStructure(longParameter, typeof(DeviceBroadcastHeader));
                        if (deviceHeader.DeviceType == DeviceType.DBT_DEVTYP_PORT)
                        {
                            var devicePortHeader = (DeviceBroadcastPort)System.Runtime.InteropServices.Marshal.PtrToStructure(longParameter, typeof(DeviceBroadcastPort));
                            SystemReportsDeviceAdded(HwndSource.FromHwnd(windowHandle).RootVisual, devicePortHeader.Name, Core.Model.Device.ConnectionType.Serial);
                        }
                        break;
                    case DeviceChangeEvent.DBT_DEVICEREMOVECOMPLETE:
                        deviceHeader = (DeviceBroadcastHeader)System.Runtime.InteropServices.Marshal.PtrToStructure(longParameter, typeof(DeviceBroadcastHeader));
                        if (deviceHeader.DeviceType == DeviceType.DBT_DEVTYP_PORT)
                        {
                            var devicePortHeader = (DeviceBroadcastPort)System.Runtime.InteropServices.Marshal.PtrToStructure(longParameter, typeof(DeviceBroadcastPort));
                            SystemReportsDeviceRemoved(HwndSource.FromHwnd(windowHandle).RootVisual, devicePortHeader.Name, Core.Model.Device.ConnectionType.Serial);
                        }
                        break;
                    default:
                        break;
                }
            }
            return IntPtr.Zero;
        }

        private static class NativeMethods
        {
            /// <summary>
            /// The Windows WM_DEVICECHANGE message.
            /// </summary>
            public const int WM_DEVICECHANGE = 0x219;
        }
    }
}

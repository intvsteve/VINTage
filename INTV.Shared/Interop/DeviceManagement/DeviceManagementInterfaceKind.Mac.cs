// <copyright file="DeviceManagementInterfaceKind.Mac.cs" company="INTV Funhouse">
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

#define USE_IOKIT_NOTIFICATIONS

namespace INTV.Shared.Interop.DeviceManagement
{
    /// <summary>
    /// Different ways to perform device management on Mac.
    /// </summary>
    /// <remarks>MonoMac does not provide IOKit bindings. This library contains enough to implement
    /// communication with serial devices, and detect port arrival and departure. As an escape hatch,
    /// in case things go terribly wrong, a more class, UNIX-style 'dev' API is available as well.
    /// The default is to use the IOKit implementation. However, if trouble in the field arises,
    /// the user may modify the info.plist of the application to use the device approach instead.
    /// This is done by setting an environment variable.</remarks>
    public enum DeviceManagementInterfaceKind
    {
        /// <summary>
        /// Use the IOKit bindings to detect device connect / disconnect.
        /// </summary>
        IOKit,

        /// <summary>
        /// Use a file system watcher on the /dev directory to detect device connect / disconnect.
        /// </summary>
        Dev
    }

    /// <summary>
    /// Helper methods for using DeviceManagementInterfaceKind.
    /// </summary>
    public static class DeviceManagementInterfaceKindHelpers
    {
#if USE_IOKIT_NOTIFICATIONS
        private const DeviceManagementInterfaceKind DefaultKind = DeviceManagementInterfaceKind.IOKit;
#else
        private const DeviceManagementInterfaceKind DefaultKind = DeviceManagementInterfaceKind.Dev;
#endif

        private const string EnvironmentVariableName = "VINTAGE_PORT_NOTIFIER_KIND";

        /// <summary>
        /// Gets the mechanism used for performing device management (i.e. how we identify serial ports).
        /// </summary>
        /// <returns>The device management API type.</returns>
        public static DeviceManagementInterfaceKind GetKind()
        {
            var interfaceKind = DefaultKind;

            ////System.Environment.SetEnvironmentVariable(EnvironmentVariableName, "Dev");
            var portNotifierSetting = System.Environment.GetEnvironmentVariable(EnvironmentVariableName);
            if (!string.IsNullOrEmpty(portNotifierSetting))
            {
                int portKind = 0;
                if (int.TryParse(portNotifierSetting, out portKind))
                {
                    if (System.Enum.IsDefined(typeof(DeviceManagementInterfaceKind), portKind))
                    {
                        interfaceKind = (DeviceManagementInterfaceKind)portKind;
                    }
                }
                else
                {
                    System.Enum.TryParse(portNotifierSetting, out interfaceKind);
                }
            }

            return interfaceKind;
        }
    }
}

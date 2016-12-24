// <copyright file="DeviceConnectionViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for selecting a potential device from a dropdown button.
    /// </summary>
    public class DeviceConnectionViewModel : INTV.Shared.ViewModel.ViewModelBase
    {
        /// <summary>
        /// Placeholder for when no devices can be found.
        /// </summary>
        public static readonly DeviceConnectionViewModel NoneAvailable = new DeviceConnectionViewModel(null, Resources.Strings.DeviceConnection_NoneAvailable) { Icon = DeviceViewModel.NoConnectedDevices };

        /// <summary>
        /// Initializes a new instance of DeviceConnectionViewModel.
        /// </summary>
        /// <param name="ltoFlashViewModel">The master ViewModel.</param>
        /// <param name="name">Name of the port.</param>
        public DeviceConnectionViewModel(LtoFlashViewModel ltoFlashViewModel, string name)
        {
            LtoFlash = ltoFlashViewModel;
            Name = name;
            Icon = DeviceViewModel.ConnectedDevices;
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        public string Name
        {
            get { return _name; }
            private set { AssignAndUpdateProperty("Name", value, ref _name); }
        }
        private string _name;

        /// <summary>
        /// Gets the icon to display.
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            private set { AssignAndUpdateProperty("Icon", value, ref _icon); }
        }
        private string _icon;

        /// <summary>
        /// Gets the master ViewModel.
        /// </summary>
        internal LtoFlashViewModel LtoFlash { get; private set; }

        /// <summary>
        /// Initialized a new collection of device connections.
        /// </summary>
        /// <param name="ltoFlashViewModel">The master ViewModel for LTO Flash!</param>
        /// <returns>An enumerable (ordered) of the devices that might be an LTO Flash!</returns>
        public static IEnumerable<DeviceConnectionViewModel> GetAvailableConnections(LtoFlashViewModel ltoFlashViewModel)
        {
            var ports = new List<DeviceConnectionViewModel>();
            var available = INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts.OrderBy(p => p);
            if (available.Any())
            {
                ports.AddRange(available.Select(p => new DeviceConnectionViewModel(ltoFlashViewModel, p)));
            }
            else
            {
                ports.Add(NoneAvailable);
            }
            return ports;
        }
    }
}

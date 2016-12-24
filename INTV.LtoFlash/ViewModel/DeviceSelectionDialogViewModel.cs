// <copyright file="DeviceSelectionDialogViewModel.cs" company="INTV Funhouse">
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

using System.Collections.ObjectModel;
using INTV.Shared.ComponentModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Common base class for some device selection dialogs.
    /// </summary>
    public abstract class DeviceSelectionDialogViewModel : INTV.Shared.ViewModel.ViewModelBase
    {
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected DeviceSelectionDialogViewModel()
        {
            AvailableDevicePorts = new ObservableCollection<string>();
            INTV.Shared.Interop.DeviceManagement.DeviceChange.DeviceAdded += DeviceAdded;
            INTV.Shared.Interop.DeviceManagement.DeviceChange.DeviceRemoved += DeviceRemoved;
        }

        /// <summary>
        /// Gets the title for the dialog.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets or sets an informational message for the dialog.
        /// </summary>
        public abstract string Message { get; protected set; }

        /// <summary>
        /// Gets or sets the selected device.
        /// </summary>
        public string SelectedDevice
        {
            get { return _selectedDevice; }
            set { AssignAndUpdateProperty("SelectedDevice", value, ref _selectedDevice); }
        }
        private string _selectedDevice;

        /// <summary>
        /// Gets a value indicating whether selection from a list of devices is supported.
        /// </summary>
        public abstract bool SupportsMultipleDevices { get; }

        /// <summary>
        /// Gets the available device ports.
        /// </summary>
        public ObservableCollection<string> AvailableDevicePorts { get; private set; }

        /// <summary>
        /// Gets the text for the button that selects a device.
        /// </summary>
        public abstract string SelectButtonText { get; }

        private void DeviceAdded(object sender, Shared.Interop.DeviceManagement.DeviceChangeEventArgs e)
        {
            if (!AvailableDevicePorts.Contains(e.Name))
            {
                AvailableDevicePorts.Add(e.Name);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private void DeviceRemoved(object sender, Shared.Interop.DeviceManagement.DeviceChangeEventArgs e)
        {
            AvailableDevicePorts.Remove(e.Name);
            CommandManager.InvalidateRequerySuggested();
        }
    }
}

// <copyright file="SwitchToDeviceDialogViewModel.cs" company="INTV Funhouse">
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

using System.Linq;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Specialize the DeviceSelectionViewModel to switch to a specific device.
    /// </summary>
    public class SwitchToDeviceDialogViewModel : DeviceSelectionDialogViewModel
    {
        /// <summary>
        /// The text for the 'Ignore' button on the dialog.
        /// </summary>
        public static readonly string IgnoreText = Resources.Strings.Ignore;

        /// <summary>
        /// Initializes a new instance of SwitchToDeviceDialogViewModel.
        /// </summary>
        public SwitchToDeviceDialogViewModel()
        {
            AvailableDevicePorts.CollectionChanged += AvailableDevicePortsCollectionChanged;
        }

        #region DeviceSelectionDialogViewModel

        /// <inheritdoc />
        public override string Title
        {
            get { return Resources.Strings.ConnectToDevice_Title; }
        }

        /// <inheritdoc />
        public override string Message
        {
            get { return _message; }
            protected set { AssignAndUpdateProperty("Message", value, ref _message); }
        }
        private string _message;

        /// <inheritdoc />
        public override bool SupportsMultipleDevices
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override string SelectButtonText
        {
            get { return Resources.Strings.Connect; }
        }

        #endregion // DeviceSelectionDialogViewModel

        private void AvailableDevicePortsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!AvailableDevicePorts.Any())
            {
                SelectedDevice = null;
            }
            else
            {
                Message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ConnectToDevice_Message_Format, AvailableDevicePorts.First());
                SelectedDevice = AvailableDevicePorts.First();
            }
        }
    }
}

// <copyright file="IntellicartViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Core.Model.Device;
using INTV.Intellicart.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.Interop.DeviceManagement;
using INTV.Shared.Model.Device;
using INTV.Shared.Model.Program;
using INTV.Shared.ViewModel;

namespace INTV.Intellicart.ViewModel
{
    /// <summary>
    /// Implements the ViewModel for the Intellicart device model.
    /// </summary>
    public class IntellicartViewModel : ViewModelBase, System.ComponentModel.Composition.IPartImportsSatisfiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Intellicart.ViewModel.IntellicartViewModel"/> class.
        /// </summary>
        public IntellicartViewModel()
        {
            DeviceChange.DeviceAdded += DeviceAdded;
            DeviceChange.DeviceRemoved += DeviceRemoved;
            BaudRates = new ObservableCollection<BaudRateViewModel>(IntellicartModel.BaudRates.Select(r => new BaudRateViewModel(r)));
            this.DoImport();
        }

        #region // Properties

        /// <summary>
        /// Gets an observable enumeration of the serial ports that can be chosen from to assign to an Intellicart.
        /// </summary>
        public ObservableCollection<SerialPortViewModel> SerialPorts { get; private set; }

        /// <summary>
        /// Gets or sets the ViewModel for the selected port assigned to an Intellicart. Updates the model's configuration.
        /// </summary>
        public SerialPortViewModel Port
        {
            get
            {
                var port = SerialPorts.FirstOrDefault(p => p.PortName == SerialPort);
                return port;
            }
            set
            {
                if (value != null)
                {
                    SerialPort = value.PortName;
                }
            }
        }

        /// <summary>
        /// Gets the baud rates supported by an Intellicart.
        /// </summary>
        public ObservableCollection<BaudRateViewModel> BaudRates
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the ViewModel for the selected baud rate. Updates the model's configuration.
        /// </summary>
        public BaudRateViewModel SelectedBaudRateViewModel
        {
            get
            {
                return BaudRates.FirstOrDefault(b => b.BaudRate == BaudRate);
            }
            set
            {
                BaudRate = value.BaudRate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SerialPorts"/> property is valid.
        /// </summary>
        public bool IsPortValid
        {
            get { return SerialPorts.Any(); }
        }

        /// <summary>
        /// Gets a value indicating whether the configured serial port is available for use.
        /// </summary>
        public bool IsConfiguredPortAvailable
        {
            get
            {
                var portAvailable = !string.IsNullOrWhiteSpace(SerialPort) && SerialPortConnection.AvailablePorts.Contains(SerialPort);
                portAvailable = portAvailable && !SerialPortConnection.PortsInUse.Contains(SerialPort);
                portAvailable = portAvailable && SerialPorts.Select(p => p.PortName).Contains(SerialPort);
                return portAvailable;
            }
        }

        /// <summary>
        /// Gets or sets the connection sharing policies.
        /// </summary>
        [System.ComponentModel.Composition.ImportMany(typeof(IConnectionSharingPolicy))]
        public IEnumerable<System.Lazy<IConnectionSharingPolicy>> ConnectionSharingPolicies { get; set; }

        #region Model Access

        /// <summary>
        /// Gets the model.
        /// </summary>
        [System.ComponentModel.Composition.Import(typeof(IntellicartModel))]
        public IntellicartModel Model { get; private set; }

        /// <summary>
        /// Gets or sets name of the serial port used to communicate with the device.
        /// </summary>
        public string SerialPort
        {
            get { return Model.SerialPort; }
            set { Model.SerialPort = value; }
        }

        /// <summary>
        /// Gets or sets the baud rate used to communicate with the device.
        /// </summary>
        public int BaudRate
        {
            get { return Model.BaudRate; }
            set { Model.BaudRate = value; }
        }

        /// <summary>
        /// Gets or sets the write timeout for the serial port used to communicate with the device.
        /// </summary>
        public int Timeout
        {
            get { return Model.Timeout; }
            set { Model.Timeout = value; }
        }

        [System.ComponentModel.Composition.Import]
        internal ProgramCollection Roms { get; set; }

        #endregion // Model Access

        #endregion // Properties

        #region IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            Model.PropertyChanged += HandlePropertyChanged;
            var ports = INTV.Shared.Model.Device.SerialPortConnection.GetAvailablePorts(IsNotExclusivePort);
            SerialPorts = new ObservableCollection<SerialPortViewModel>(ports.OrderBy(p => p).Select(p => new SerialPortViewModel(p)));
            if (!string.IsNullOrWhiteSpace(Model.SerialPort) && (SerialPorts.FirstOrDefault(p => p.PortName == Model.SerialPort) == null))
            {
                SerialPorts.Add(new SerialPortViewModel(Model.SerialPort));
            }
        }

        #endregion //  IPartImportsSatisfiedNotification Members

        /// <summary>
        /// Filter function for serial ports, which rejects exclusive ports.
        /// </summary>
        /// <param name="connection">The IConnection to test.</param>
        /// <returns><c>true</c>, if exclusive ports was rejected, <c>false</c> otherwise.</returns>
        internal bool IsNotExclusivePort(IConnection connection)
        {
            var acceptPort = false;
            if (connection.Type == ConnectionType.Serial)
            {
                foreach (var policy in ConnectionSharingPolicies)
                {
                    acceptPort = !policy.Value.ExclusiveAccess(connection);
                    if (!acceptPort)
                    {
                        break;
                    }
                }
            }
            return acceptPort;
        }

        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Forward along changes to model properties as reflected in the ViewModel.
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "BaudRate":
                    RaisePropertyChanged("SelectedBaudRateViewModel");
                    break;
                case "SerialPort":
                    RaisePropertyChanged("Port");
                    break;
            }
        }

        private void DeviceAdded(object sender, DeviceChangeEventArgs e)
        {
            if ((e.Type == INTV.Core.Model.Device.ConnectionType.Serial) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                var acceptPort = IsNotExclusivePort(Connection.CreatePseudoConnection(e.Name, ConnectionType.Serial));
                if (acceptPort && !SerialPorts.Any(p => p.PortName == e.Name))
                {
                    SerialPorts.Add(new SerialPortViewModel(e.Name));
                }
                INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
            }
            RaisePropertyChanged("IsPortValid");
        }

        private void DeviceRemoved(object sender, DeviceChangeEventArgs e)
        {
            if ((e.Type == INTV.Core.Model.Device.ConnectionType.Serial) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                var removedDevice = SerialPorts.FirstOrDefault(p => p.PortName == e.Name);
                if ((removedDevice != null) && SerialPorts.Remove(removedDevice))
                {
                    INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                }
            }
            RaisePropertyChanged("IsPortValid");
        }
    }
}

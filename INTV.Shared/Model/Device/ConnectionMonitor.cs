// <copyright file="ConnectionMonitor.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Core.Model.Device;
using INTV.Shared.Interop.DeviceManagement;

namespace INTV.Shared.Model.Device
{
    /// <summary>
    /// This class provides a means to track peripherals connected to the system via different means.
    /// </summary>
    public class ConnectionMonitor : INTV.Core.ComponentModel.ModelBase
    {
        private HashSet<Func<IConnection, object, IPeripheral>> _factories;

        /// <summary>
        /// Initializes a new instance of the ConnectionMonitor class.
        /// </summary>
        public ConnectionMonitor()
        {
            _factories = new HashSet<Func<IConnection, object, IPeripheral>>();
            _peripherals = new ObservableCollection<IPeripheral>();
            DeviceChange.DeviceAdded += ConnectionAdded;
            DeviceChange.DeviceRemoved += ConnectionRemoved;
        }

        /// <summary>
        /// Gets the currently connected peripherals.
        /// </summary>
        public ObservableCollection<IPeripheral> Peripherals
        {
            get { return _peripherals; }
        }
        private ObservableCollection<IPeripheral> _peripherals;

        /// <summary>
        /// Registers a peripheral factory that will be called when a new connection becomes available in the system.
        /// </summary>
        /// <param name="factory">The factory to register.</param>
        /// <returns><c>true</c> if the factory was registered, <c>false</c> if the factory was already registered.</returns>
        public bool RegisterPeripheralFactory(Func<IConnection, object, IPeripheral> factory)
        {
            return _factories.Add(factory);
        }

        /// <summary>
        /// Removes a peripheral factory from the system.
        /// </summary>
        /// <param name="factory">The factory to remove.</param>
        /// <returns><c>true</c> if the factory was unregistered, <c>false</c> if the factory had not been previously registered.</returns>
        public bool UnregisterPeripheralFactory(Func<IConnection, object, IPeripheral> factory)
        {
            return _factories.Remove(factory);
        }

        private static IConnection ConnectionFactory(ConnectionType type, string name, IDictionary<string, object> creationData)
        {
            IConnection connection = null;
            switch (type)
            {
                case ConnectionType.Serial:
                    connection = SerialPortConnection.Create(creationData);
                    break;
                case ConnectionType.NamedPipe:
                    connection = NamedPipeConnection.Create(creationData);
                    break;
                default:
                    break;
            }
            return connection;
        }

        private void ConnectionAdded(object sender, DeviceChangeEventArgs e)
        {
            IConnection connection = ConnectionFactory(e.Type, e.Name, e.State);
            if (connection != null)
            {
                INTV.Shared.Utility.OSDispatcher.Current.BeginInvoke(new Action(() =>
                    {
                        foreach (var factory in _factories)
                        {
                            var peripheral = factory(connection, e.State);
                            if (peripheral != null)
                            {
                                // yay!
                                _peripherals.Add(peripheral);
                                RaisePropertyChanged("Peripherals");
                            }
                        }
                    }));
            }
        }

        private void ConnectionRemoved(object sender, DeviceChangeEventArgs e)
        {
            // yay?
            var peripheralsToRemove = _peripherals.Where(p => p.Connections.FirstOrDefault(c => c.Name == e.Name) != null).ToList();
            foreach (var peripheral in peripheralsToRemove)
            {
                _peripherals.Remove(peripheral);
                var disposable = peripheral as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            if (peripheralsToRemove.Any())
            {
                RaisePropertyChanged("Peripherals");
            }
        }
    }
}

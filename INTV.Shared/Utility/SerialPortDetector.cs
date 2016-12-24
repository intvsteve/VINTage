// <copyright file="SerialPortDetector.cs" company="INTV Funhouse">
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
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;

#if WIN
using ExitEventArgs = System.Windows.ExitEventArgs;
#elif MAC
using ExitEventArgs = INTV.Shared.Utility.ExitEventArgs;
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Implements a polling-based mechanism to detect new serial ports in the system.
    /// </summary>
    public sealed class SerialPortDetector : IDisposable
    {
        private AutoResetEvent _stopper = new AutoResetEvent(false);
        private Thread _detectorThread;
        private ObservableCollection<string> _ports;
        private ReadOnlyObservableCollection<string> _availablePorts;
        private OSDispatcher _dispatcher;

        ~SerialPortDetector()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the available ports in the system.
        /// </summary>
        public ReadOnlyObservableCollection<string> AvailablePorts
        {
            get { return _availablePorts; }
        }

        /// <summary>
        /// Starts the serial port detector.
        /// </summary>
        public void Start()
        {
            SingleInstanceApplication.Current.Exit += OnApplicationExit;
            _dispatcher = OSDispatcher.Current;
            _ports = new ObservableCollection<string>();
            _availablePorts = new ReadOnlyObservableCollection<string>(_ports);
            _detectorThread = new Thread(new ThreadStart(this.Detect));
            _detectorThread.Start();
        }

        /// <summary>
        /// Stops the serial port detector.
        /// </summary>
        public void Stop()
        {
            _stopper.Set();
            _detectorThread.Join();
            _detectorThread = null;
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stopper != null)
                {
                    _stopper.Dispose();
                }
            }
        }

        #endregion // IDisposable

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            OnApplicationExit(sender, (EventArgs)e);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            SingleInstanceApplication.Current.Exit -= OnApplicationExit;
            Stop();
        }

        private void Detect()
        {
            while (!_stopper.WaitOne(500))
            {
                var ports = SerialPort.GetPortNames();
                _dispatcher.BeginInvoke(new Action(() =>
                {
                    var newPorts = ports.Except(_ports);
                    var oldPorts = _ports.Except(ports).ToArray();
                    foreach (var port in oldPorts)
                    {
                        _ports.Remove(port);
                    }
                    foreach (var port in newPorts)
                    {
                        _ports.Add(port);
                    }
                }));
            }
        }
    }
}

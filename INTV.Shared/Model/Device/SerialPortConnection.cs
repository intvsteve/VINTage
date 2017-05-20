// <copyright file="SerialPortConnection.cs" company="INTV Funhouse">
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

////#define TRACK_PORT_LIFETIMES

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using INTV.Core.Model.Device;
using INTV.Shared.Utility;

namespace INTV.Shared.Model.Device
{
    /// <summary>
    /// Models a serial port connection to a device, exposing a versatile stream interface.
    /// </summary>
    public partial class SerialPortConnection : INTV.Core.Model.Device.Connection, IStreamConnection, IDisposable
    {
        #region Configuration Data Parameter Names

        /// <summary>
        /// Key to use for a serial port's name in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be the name of the port to open, such as COM4 or /dev/tty4.</remarks>
        public const string PortNameConfigDataName = "Name";

        /// <summary>
        /// Key to use for serial port's baud rate in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be an int, which specifies the baud rate to use for serial port communication.</remarks>
        public const string BaudRateConfigDataName = "BaudRate";

        /// <summary>
        /// Key to use for serial port's baud rate in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be an int, specifying serial port read timeout in milliseconds.</remarks>
        public const string ReadTimeoutConfigDataName = "ReadTimeout";

        /// <summary>
        /// Key to use for serial port's baud rate in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be an int, specifying serial port write timeout in milliseconds.</remarks>
        public const string WriteTimeoutConfigDataName = "WriteTimeout";

        /// <summary>
        /// Key to use for serial port's baud rate in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be of type <see cref="System.IO.Ports.Parity"/>, and specifies the stop bits to use in serial port communication.</remarks>
        public const string ParityConfigDataName = "Parity";

        /// <summary>
        /// Key to use for serial port's stop bits in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be of type <see cref="System.IO.Ports.StopBits"/>, and specifies the stop bits to use in serial port communication.</remarks>
        public const string StopBitsConfigDataName = "StopBits";

        /// <summary>
        /// Key to use for serial port's handshake setting in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be of type <see cref="System.IO.Ports.Handshake"/>, and specifies the handshake mode to use in serial communication.</remarks>
        public const string HandshakeConfigDataName = "Handshake";

        #endregion // Configuration Data Parameter Names

        /// <summary>
        /// The name of an invalid serial port.
        /// </summary>
        public static readonly string InvalidPortName = Resources.Strings.InvalidSerialPortName;

        private static readonly ConcurrentDictionary<string, WeakReference> _portsInUse = new ConcurrentDictionary<string, WeakReference>();

#if TRACK_PORT_LIFETIMES
        private static readonly ConcurrentDictionary<string, WeakReference> _createdPorts = new ConcurrentDictionary<string, WeakReference>();
#endif // TRACK_PORT_LIFETIMES

        #region Constructors

        /// <summary>
        /// Create a new instance of the SerialPortConnection class.
        /// </summary>
        /// <param name="portName">The name of the serial port.</param>
        private SerialPortConnection(string portName)
            : base(portName, Core.Model.Device.ConnectionType.Serial)
        {
            Port = new SerialPort(portName);
#if TRACK_PORT_LIFETIMES
            InstallCommand();
            UpdateTracker("CREATING", portName);
            WeakReference portInstance = null;
            System.Diagnostics.Debug.WriteLineIf(_createdPorts.TryGetValue(portName, out portInstance), "WTH? Port " + portName + " is already here?!");
            _createdPorts[portName] = new WeakReference(this);
            UpdateTracker("CREATED", portName);
#endif // TRACK_PORT_LIFETIMES
        }

        #endregion

        #region Finalizer

        ~SerialPortConnection()
        {
            Dispose(false);
        }

        #endregion // Finalizer

        #region Properties

        /// <summary>
        /// Gets the available serial ports.
        /// </summary>
        public static IEnumerable<string> AvailablePorts
        {
            get
            {
                UpdateAvailablePorts();
                return _availablePorts;
            }
        }
        private static string[] _availablePorts;

        /// <summary>
        /// Gets the ports in use.
        /// </summary>
        public static IEnumerable<string> PortsInUse
        {
            get
            {
                UpdateWeakDictionary(_portsInUse);
                var portsInUse = _portsInUse.Keys;
                return portsInUse;
            }
        }

        #region IStreamConnection

        /// <inheritdoc />
        string IStreamConnection.Name
        {
            get { return IsValid ? Port.PortName : InvalidPortName; }
        }

        /// <inheritdoc />
        public bool IsOpen
        {
            get { return IsValid && Port.IsOpen; }
        }

        /// <inheritdoc />
        public System.IO.Stream ReadStream
        {
            get { return IsValid ? Port.BaseStream : null; }
        }

        /// <inheritdoc />
        public System.IO.Stream WriteStream
        {
            get { return IsValid ? Port.BaseStream : null; }
        }

        /// <inheritdoc />
        public int ReadTimeout
        {
            get
            {
                return IsValid ? Port.ReadTimeout : 0;
            }

            set
            {
                if (IsValid)
                {
                    Port.ReadTimeout = value;
                }
            }
        }

        /// <inheritdoc />
        public int WriteTimeout
        {
            get
            {
                return IsValid ? Port.WriteTimeout : 0;
            }

            set
            {
                if (IsValid)
                {
                    Port.WriteTimeout = value;
                }
            }
        }

        #endregion // IStreamConnection

        /// <summary>
        /// Gets a value indicating whether this instance has a valid serial port.
        /// </summary>
        public bool IsValid
        {
            get { return Port != null; }
        }

        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate
        {
            get { return IsValid ? Port.BaudRate : -1; }
            set { Port.BaudRate = value; }
        }

        /// <summary>
        /// Gets or sets the standard number of stop bits per byte.
        /// </summary>
        public StopBits StopBits
        {
            get { return IsValid ? Port.StopBits : StopBits.One; }
            set { Port.StopBits = value; }
        }

        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        public Parity Parity
        {
            get { return IsValid ? Port.Parity : Parity.None; }
            set { Port.Parity = value; }
        }

        /// <summary>
        /// Gets or sets the handshaking protocol for serial port transmission of data.
        /// </summary>
        public Handshake Handshake
        {
            get { return IsValid ? Port.Handshake : Handshake.None; }
            set { Port.Handshake = value; }
        }

        /// <summary>
        /// Gets or sets the underlying C# System.IO.Ports.SerialPort.
        /// </summary>
        private SerialPort Port
        {
            get { return _port; }
            set { _port = value; }
        }
        private SerialPort _port;

        /// <summary>
        /// Gets or sets the logger used by the port.
        /// </summary>
        private INTV.Shared.Utility.Logger Logger { get; set; }

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of SerialPortConnection using the given configuration.
        /// </summary>
        /// <param name="configurationData">The configuration data for the port.</param>
        /// <returns>A new instance of SerialPortConnection configured with the supplied <paramref name="configurationData"/>.</returns>
        /// <remarks>The port name is required, which is a string value supplied via the key <see cref="PortNameConfigDataName"/>.</remarks>
        public static SerialPortConnection Create(IDictionary<string, object> configurationData)
        {
            var serialPortConnection = new SerialPortConnection((string)configurationData[PortNameConfigDataName]);
            serialPortConnection.Configure(configurationData);
            return serialPortConnection;
        }

        /// <summary>
        /// Gets the available ports by applying the supplied filter.
        /// </summary>
        /// <param name="filter">A predicate to filter out ports that should not be included. If <c>null</c>, all ports in <see cref="AvailablePorts"/> are returned.</param>
        /// <returns>The available ports, excluding those that do not pass the given filter.</returns>
        public static IEnumerable<string> GetAvailablePorts(Predicate<INTV.Core.Model.Device.IConnection> filter)
        {
            var availablePorts = filter == null ? AvailablePorts : AvailablePorts.Where(p => filter(Connection.CreatePseudoConnection(p, ConnectionType.Serial)));
            return availablePorts;
        }

        #region IStreamConnection

        /// <inheritdoc />
        public void Open()
        {
            Log("OPEN port");
            OpenPort();
            _portsInUse[Name] = new WeakReference(this);
        }

        /// <inheritdoc />
        public void Close()
        {
            Log("CLOSE port");
            Port.Close();
            WeakReference closedPort;
            _portsInUse.TryRemove(Port.PortName, out closedPort);
        }

        /// <inheritdoc />
        public void Configure(IDictionary<string, object> configurationData)
        {
            object configValue = null;
            if (configurationData.TryGetValue(BaudRateConfigDataName, out configValue))
            {
                BaudRate = (int)configValue;
            }
            if (configurationData.TryGetValue(ParityConfigDataName, out configValue))
            {
                Parity = (Parity)configValue;
            }
            if (configurationData.TryGetValue(StopBitsConfigDataName, out configValue))
            {
                StopBits = (StopBits)configValue;
            }
            if (configurationData.TryGetValue(HandshakeConfigDataName, out configValue))
            {
                Handshake = (Handshake)configValue;
            }
            if (configurationData.TryGetValue(ReadTimeoutConfigDataName, out configValue))
            {
                ReadTimeout = (int)configValue;
            }
            if (configurationData.TryGetValue(WriteTimeoutConfigDataName, out configValue))
            {
                WriteTimeout = (int)configValue;
            }
        }

        /// <inheritdoc />
        public int EstimateDataTransferTime(long numberOfBytes)
        {
            if ((numberOfBytes < 0) || ((ulong)numberOfBytes > ((ulong)long.MaxValue / 8)))
            {
                throw new ArgumentOutOfRangeException("Invalid number of bytes.");
            }
            var estimate = -1;
            if (IsValid)
            {
                var numberOfBits = numberOfBytes * 8; // we're assuming no overflow here
                var rawEstimate = Math.Max(1, numberOfBits / BaudRate) * 1000;
                if (rawEstimate > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("Timeout too large.");
                }
                estimate = (int)rawEstimate;
            }
            return estimate;
        }

        /// <inheritdoc />
        public void EnableLogging(string logPath)
        {
            if (Logger == null)
            {
                Logger = new Logger(logPath);
            }
        }

        /// <inheritdoc />
        public void DisableLogging()
        {
            Logger = null;
        }

        /// <inheritdoc />
        public void Log(string message)
        {
            var logger = Logger;
            if (logger != null)
            {
                logger.Log(message + " " + (Port == null ? "<null>" : Port.PortName));
            }
        }

        #endregion // IStreamConnection

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable

        #region object Overrides

        /// <inheritdoc/>
        public override string ToString()
        {
            var displayString = Port == null ? "<null>" : string.Format("{0}: {1},{2},{3},{4}", Port.PortName, Port.BaudRate, Port.DataBits, Port.Parity.ToString()[0], Port.StopBits.ToDisplayString());
            return displayString;
        }

        #endregion // object Overrides

        /// <summary>
        /// Generates output to the debug stream showing serial port connection data - if TRACK_PORT_LIFETIMES is enabled.
        /// </summary>
        /// <param name="action">The activity to report.</param>
        /// <param name="portName">The name of the port the activity took place on.</param>
        [System.Diagnostics.Conditional("TRACK_PORT_LIFETIMES")]
        internal static void UpdateTracker(string action, string portName)
        {
#if TRACK_PORT_LIFETIMES
            if (_createdPorts != null)
            {
                UpdateWeakDictionary(_createdPorts);
                System.Diagnostics.Debug.WriteLine("***** " + action + " SERIALPORTCONNECTION: " + portName + ", LiveObjects: " + _createdPorts.Count);
            }
#endif // TRACK_PORT_LIFETIMES
        }

        /// <summary>
        /// Ensures the underlying serial port resources are correctly disposed.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, Dispose() is being called, otherwise, the finalizer is executing.</param>
        protected virtual void Dispose(bool disposing)
        {
            var message = disposing ? "DISPOSE(" + disposing + ")" : "FINALIZE";
            Log(message);
            UpdateTracker(message, Port == null ? "<null>" : Port.PortName);
            if (Port != null)
            {
                Port.Dispose();
                Port = null;
            }
        }

        private static IList<string> UpdateWeakDictionary(ConcurrentDictionary<string, WeakReference> dictionary)
        {
            GC.Collect(); // Risky? Forcing a GC here...
            var deadObjects = dictionary.Where(p => !p.Value.IsAlive).Select(p => p.Key).ToList();
            foreach (var deadOjbect in deadObjects)
            {
                WeakReference deadPort;
                dictionary.TryRemove(deadOjbect, out deadPort);
            }
            return deadObjects;
        }

        /// <summary>
        /// Updates the available ports.
        /// </summary>
        static partial void UpdateAvailablePorts();

        [System.Diagnostics.Conditional("TRACK_PORT_LIFETIMES")]
        private static void InstallCommand()
        {
            var application = INTV.Shared.Utility.SingleInstanceApplication.Instance;
            if (application != null)
            {
                INTV.Shared.Utility.SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!installedCommand && (INTV.Shared.Utility.SingleInstanceApplication.Instance != null) && (INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow != null))
                        {
                            installedCommand = true;
                            PlatformInstallCommand();
                        }
                    }));
            }
        }
        private static bool installedCommand;

        /// <summary>
        /// Platform-specific setup for port lifetime tracking.
        /// </summary>
        [System.Diagnostics.Conditional("TRACK_PORT_LIFETIMES")]
        static partial void PlatformInstallCommand();
    }
}

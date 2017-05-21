// <copyright file="IntellicartModel.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.Shared.Model.Device;

namespace INTV.Intellicart.Model
{
    /// <summary>
    /// Intellicart model.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IntellicartModel))]
    [System.ComponentModel.Composition.Export(typeof(INTV.Shared.ComponentModel.IPrimaryComponent))]
    public sealed class IntellicartModel : Peripheral, IDisposable, INTV.Shared.ComponentModel.IPrimaryComponent
    {
        /// <summary>
        /// Max ROM size is ~128KB: 256 * (512+4) + 53 = 132149 bytes.
        /// </summary>
        public const int MaxROMSize = 132149;

        /// <summary>
        /// Default baud rate.
        /// </summary>
        public const int DefaultBaudRate = 38400;

        /// <summary>
        /// Default port write timeout.
        /// </summary>
        public const int DefaultWriteTimeout = 30;

        /// <summary>
        /// Maximum supported serial port write timeout (seconds).
        /// </summary>
        public const int MaxWriteTimeout = 240;

        /// <summary>
        /// Minimum supported serial port write timeout (seconds).
        /// </summary>
        public const int MinWriteTimeout = 4;

        #region Property Names

        /// <summary>
        /// Name of the SerialPort property for property changed handlers.
        /// </summary>
        public const string SerialPortPropertyName = "SerialPort";

        /// <summary>
        /// Name of the BaudRate property for property changed handlers.
        /// </summary>
        public const string BaudRatePropertyName = "BaudRate";

        /// <summary>
        /// Name of the Timeout property for property changed handlers.
        /// </summary>
        public const string TimeoutPropertyName = "Timeout";

        #endregion // Property Names

        private const string JustCopy = "!!::copy::!!"; // TODO Consolidate this goofy !!::copy::!! pseudo-tool notion.

        /// <summary>
        /// Baud rates supported by the Intellicart.
        /// </summary>
        /// <remarks>The 56K speed always reports a CRC error on my Intellicart. Don't know if it works on others.</remarks>
        public static readonly int[] BaudRates = new int[] { 2400, 4800, 9600, 14400, 19200, 38400, 57600 };

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Intellicart.Model.IntellicartModel"/> class.
        /// </summary>
        public IntellicartModel()
        {
            Name = "Intellicart";
            _serialPort = INTV.Intellicart.Properties.Settings.Default.SerialPort;
            _baudRate = INTV.Intellicart.Properties.Settings.Default.BaudRate;
            _timeout = INTV.Intellicart.Properties.Settings.Default.Timeout;
            Properties.Settings.Default.PropertyChanged += HandleIntellicartSettingChanged;
            UpdateSerialPort(_serialPort, _baudRate, _timeout);
        }

        ~IntellicartModel()
        {
            Dispose(false);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the serial port name.
        /// </summary>
        public string SerialPort
        {
            get { return _serialPort; }
            set { AssignAndUpdateProperty(SerialPortPropertyName, value, ref _serialPort, (s, v) => UpdateSerialPort(v, BaudRate, Timeout)); }
        }
        private string _serialPort;

        /// <summary>
        /// Gets or sets the baud rate for the serial port.
        /// </summary>
        public int BaudRate
        {
            get { return _baudRate; }
            set { AssignAndUpdateProperty(BaudRatePropertyName, value, ref _baudRate, (s, v) => UpdateSerialPort(SerialPort, v, Timeout)); }
        }
        private int _baudRate;

        /// <summary>
        /// Gets or sets the write timeout for the serial port.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { AssignAndUpdateProperty(TimeoutPropertyName, value, ref _timeout, (s, v) => UpdateSerialPort(SerialPort, BaudRate, v)); }
        }
        private int _timeout;

        /// <summary>
        /// Gets a value indicating whether the serial port is considered to be a valid configuration.
        /// </summary>
        public bool IsPortValid
        {
            get
            {
                var isValid = BaudRates.Contains(BaudRate) && (Timeout >= MinWriteTimeout) && (Timeout <= MaxWriteTimeout) && SerialPortConnection.AvailablePorts.Contains(SerialPort);
                return isValid;
            }
        }

        #region IPeripheral

        /// <inheritdoc />
        /// <remarks>>Since the serial port is only used during load, this enumeration is always empty.</remarks>
        public override IEnumerable<IConnection> Connections
        {
            get { return Enumerable.Empty<IConnection>(); }
            protected set { }
        }

        #endregion // IPeripheral

        #endregion // Properties

        #region IPrimaryComponent

        /// <inheritdoc />
        public void Initialize()
        {
        }

        #endregion // IPrimaryComponent

        #region IPeripheral

        /// <inheritdoc />
        public override bool IsRomCompatible(IProgramDescription programDescription)
        {
            var isCompatible = programDescription.Features.Intellicart != IntellicartCC3Features.Incompatible;
            if (isCompatible)
            {
                // Some LUIGI ROMs cannot run on Intellicart! - specifically, LUIGI ROMs that are LTO Flash!-only. Check for that.
                isCompatible = !programDescription.Rom.IsLtoFlashOnlyRom();
            }
            return isCompatible;
        }

        #endregion // IPeripheral

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
        }

        #endregion // IDisposable

        // TODO Delete this. This was an old implementation of sending ROM to Intellicart.
#if IMPLEMENT_DOWNLOAD
        /// <summary>
        /// Loads the ROM at the given path onto the Intellicart.
        /// </summary>
        /// <remarks>The ROM at <paramref name="romPath"/> must be in the .rom format
        /// specified by the Intellicart documentation.</remarks>
        public void DownloadRom(string romPath)
        {
            using (var rom = FileUtilities.OpenFileStream(romPath))
            {
                using (var port = new SerialPortConnection(SerialPort))
                {
                    port.Port.BaudRate = BaudRate;
                    port.WriteTimeout = Timeout;
                    // default port settings are 8,N,1 with no handshaking
                    port.Open();
                    rom.CopyTo(port.WriteStream);

                    // If we close the port too soon after writing, the Intellicart
                    // will time out reading data from the stream. This is likely
                    // due to buffering, and that the streams get disposed when
                    // the port and file streams are disposed.
                    System.Threading.Thread.Sleep(4000);
                }
            }
        }
#endif // IMPLEMENT_DOWNLOAD

        private void UpdateSerialPort(string portName, int baudRate, int timeout)
        {
            if (!string.IsNullOrEmpty(portName) && (baudRate > 0))
            {
                INTV.Intellicart.Properties.Settings.Default.SerialPort = portName;
                INTV.Intellicart.Properties.Settings.Default.BaudRate = baudRate;
                if (timeout < MinWriteTimeout)
                {
                    timeout = MinWriteTimeout;
                }
                if (timeout > MaxWriteTimeout)
                {
                    timeout = MaxWriteTimeout;
                }
                INTV.Intellicart.Properties.Settings.Default.Timeout = timeout;
            }
            RaisePropertyChanged("IsPortValid");
        }

        private void HandleIntellicartSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BaudRate":
                case ViewModel.SettingsPageViewModel.IntellicartBaudRatePropertyName:
                    BaudRate = Properties.Settings.Default.BaudRate;
                    break;
                case "Timeout":
                case ViewModel.SettingsPageViewModel.IntellicartWriteTimeoutPropertyName:
                    Timeout = Properties.Settings.Default.Timeout;
                    break;
                case "SerialPort":
                case ViewModel.SettingsPageViewModel.IntellicartSerialPortPropertyName:
                    SerialPort = Properties.Settings.Default.SerialPort;
                    break;
            }
#if WIN
            ((System.Configuration.ApplicationSettingsBase)Configuration.Instance.Settings).Save();
#endif // WIN
        }
    }
}

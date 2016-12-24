// <copyright file="NamedPipeConnection.cs" company="INTV Funhouse">
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
using System.IO.Pipes;
using System.Threading;

namespace INTV.Shared.Model.Device
{
    /// <summary>
    /// Provides a named pipe connection implementation. Assumes and requires that the input and output pipes are named differently.
    /// </summary>
    public class NamedPipeConnection : INTV.Core.Model.Device.Connection, IStreamConnection, IDisposable
    {
        #region Configuration Data Parameter Names

        /// <summary>
        /// Key to use for a value that is used to define the named pipe's individual input and output pipes in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be the name of the port to open, such as COM4 or /dev/tty4.</remarks>
        public const string PipeNameConfigDataName = "Name";

        /// <summary>
        /// Key to use for defining a delegate to call prior to opening the named pipe's ports when included in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be of type Func&lt;bool&gt;.</remarks>
        public const string PreOpenPortConfigDataName = "PreOpenPort";

        /// <summary>
        /// Key to use for defining a delegate to call after opening the named pipe's ports when included in a configuration data dictionary.
        /// </summary>
        /// <remarks>The value stored with this key must be of type Action&lt;IStreamConnection&gt;.</remarks>
        public const string PostOpenPortConfigDataName = "PostOpenPort";

        #endregion // Configurable Data Parameter Names

        private const long MinimumAssumedSpeed = 2000000; // Really, we've *got* to be at least as fast as a modern serial port.
        private static long _cachedBestGuessDataTransferSpeed = -1;

        #region Constructors

        private NamedPipeConnection(string name)
            : base(name, Core.Model.Device.ConnectionType.NamedPipe)
        {
        }

        #endregion

        /// <summary>
        /// Gets the name of the stream used to write data to the device.
        /// </summary>
        public string DeviceInputPipeName { get; private set; }

        /// <summary>
        /// Gets the name of the stream used to read data from the device.
        /// </summary>
        public string DeviceOutputPipeName { get; private set; }

        /// <summary>
        /// Gets the name pipe used to send commands to the device.
        /// </summary>
        public PipeStream SendCommandPipe { get; private set; }

        /// <summary>
        /// Gets the named pipe used to receive responses from the device.
        /// </summary>
        public PipeStream ReceiveResponsePipe { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the read and write pipes have been opened.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return (SendCommandPipe != null) && (ReceiveResponsePipe != null) && SendCommandPipe.IsConnected && ReceiveResponsePipe.IsConnected;
            }
        }

        private Func<bool> PreOpenPort { get; set; }

        private Action<IStreamConnection> PostOpenPort { get; set; }

        private INTV.Shared.Utility.Logger Logger { get; set; }

        #region IStreamConnection Properties

        /// <inheritdoc />
        string IStreamConnection.Name
        {
            get { return this.Name; }
        }

        /// <inheritdoc />
        public bool IsOpen
        {
            get { return IsConnected; }
        }

        /// <inheritdoc />
        public System.IO.Stream ReadStream
        {
            get { return ReceiveResponsePipe; }
        }

        /// <inheritdoc />
        public System.IO.Stream WriteStream
        {
            get { return SendCommandPipe; }
        }

        /// <inheritdoc />
        /// <remarks>Named pipes do not support this value.</remarks>
        public int ReadTimeout
        {
            get { return -1; }
            set { }
        }

        /// <inheritdoc />
        /// <remarks>Named pipes do not support this value.</remarks>
        public int WriteTimeout
        {
            get { return 0; }
            set { }
        }

        #endregion IStreamConnection Properties

        /// <summary>
        /// Creates and initializes a new instance of a NamedPipeConnection.
        /// </summary>
        /// <param name="configurationData">The configuration data for the named pipe.</param>
        /// <returns>A new instance of NamedPipeConnection configured with the supplied <paramref name="configurationData"/>.</returns>
        /// <remarks>The required configuration data is as follows:
        /// PipeName_ConfigDataName - string : The name of the connection, and the base name for the pipes.
        /// PreOpenPort_ConfigDataName - Func&lt;bool&gt; : A non-<c>null</c> value specifies this is a server pipe, otherwise it's a client.
        /// PostOpenPort_ConfigDataName - Action&lt;IStreamConnection&gt;
        /// The input pipe (pipe used to SEND input TO a device) is named [name]Input, and the output pipe
        /// (used to RECEIVE output FROM a device) is named [name]Output</remarks>
        public static NamedPipeConnection Create(IDictionary<string, object> configurationData)
        {
            var namedPipeConnection = new NamedPipeConnection((string)configurationData[PipeNameConfigDataName]);
            namedPipeConnection.Configure(configurationData);
            return namedPipeConnection;
        }

        #region IStreamConnection

        /// <inheritdoc />
        public void Open()
        {
#if DEBUG && WIN
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
#endif //  DEBUG && WIN
            if (!IsConnected)
            {
                if (SendCommandPipe == null)
                {
                    if (PreOpenPort != null)
                    {
                        SendCommandPipe = new NamedPipeServerStream(DeviceInputPipeName, PipeDirection.Out);
                    }
                    else
                    {
                        SendCommandPipe = new NamedPipeClientStream(".", DeviceInputPipeName, PipeDirection.Out);
                    }
                }
                if (ReceiveResponsePipe == null)
                {
                    if (PreOpenPort != null)
                    {
                        ReceiveResponsePipe = new NamedPipeServerStream(DeviceOutputPipeName, PipeDirection.In);
                    }
                    else
                    {
                        ReceiveResponsePipe = new NamedPipeClientStream(".", DeviceOutputPipeName, PipeDirection.In);
                    }
                }
                if (PreOpenPort != null)
                {
                    using (var syncUp = new CountdownEvent(2))
                    {
                        if (PreOpenPort())
                        {
                            if (!SendCommandPipe.IsConnected)
                            {
                                ((NamedPipeServerStream)SendCommandPipe).WaitForConnection();
                            }
                            if (!ReceiveResponsePipe.IsConnected)
                            {
                                ((NamedPipeServerStream)ReceiveResponsePipe).WaitForConnection();
                            }
                        }
                    }
                }
                else
                {
                    ((NamedPipeClientStream)ReceiveResponsePipe).Connect(1000);
                    ((NamedPipeClientStream)SendCommandPipe).Connect(1000);
                }
                if (PostOpenPort != null)
                {
                    PostOpenPort(this);
                }
            }
        }

        /// <inheritdoc />
        public void Close()
        {
            if (SendCommandPipe != null)
            {
                SendCommandPipe.Dispose();
                SendCommandPipe = null;
            }
            if (ReceiveResponsePipe != null)
            {
                ReceiveResponsePipe.Dispose();
                ReceiveResponsePipe = null;
            }
        }

        /// <inheritdoc />
        public void Configure(IDictionary<string, object> configurationData)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("You cannot change the configuration of a NamedPipeConnection that is already in use!");
            }

            object configData = null;
            if (configurationData.TryGetValue(PipeNameConfigDataName, out configData))
            {
                DeviceInputPipeName = MakeInputPipeName((string)configData);
                DeviceOutputPipeName = MakeOutputPipeName((string)configData);
            }
            if (configurationData.TryGetValue(PreOpenPortConfigDataName, out configData))
            {
                PreOpenPort = (Func<bool>)configData;
            }
            if (configurationData.TryGetValue(PostOpenPortConfigDataName, out configData))
            {
                PostOpenPort = (Action<IStreamConnection>)configData;
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
            if (_cachedBestGuessDataTransferSpeed < 0)
            {
                // We don't have a guess yet at transfer speed. Let's assume the worst pipe ever.
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    {
                        switch (networkInterface.OperationalStatus)
                        {
                            case System.Net.NetworkInformation.OperationalStatus.Dormant:
                            case System.Net.NetworkInformation.OperationalStatus.Up:
                                if (networkInterface.Speed > 0)
                                {
                                    // Note: The stupid MONO interface HARD CODES this at 1000000 bps! Really! At least, in the code here:
                                    // https://github.com/mono/mono/blob/9c351d5028d37c81443bf375c75640b986319907/mcs/class/System/System.Net.NetworkInformation/NetworkInterface.cs
                                    // as of August 15, 2016.
                                    if (_cachedBestGuessDataTransferSpeed < 0)
                                    {
                                        _cachedBestGuessDataTransferSpeed = networkInterface.Speed;
                                    }
                                    else
                                    {
                                        _cachedBestGuessDataTransferSpeed = System.Math.Min(_cachedBestGuessDataTransferSpeed, networkInterface.Speed);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Now, take the slowest speed of the *network* interfaces and divide that down. We will assume that a
                // named pipe can acheive at least MinimumAssumedSpeed. Really, this is totally overkill, isn't it?
                _cachedBestGuessDataTransferSpeed = System.Math.Max(_cachedBestGuessDataTransferSpeed / 4, MinimumAssumedSpeed);
            }
            if (_cachedBestGuessDataTransferSpeed > 0)
            {
                var numberOfBits = numberOfBytes * 8; // we're assuming no overflow here
                var rawEstimate = Math.Max(1, numberOfBits / _cachedBestGuessDataTransferSpeed) * 1000;
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
                Logger = new INTV.Shared.Utility.Logger(logPath);
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
                logger.Log(message + " " + (!IsOpen ? "<null>" : Name));
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

        /// <summary>
        /// Ensures the underlying named pipe resources are correctly disposed.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, Dispose() is being called, otherwise, the finalizer is executing.</param>
        protected virtual void Dispose(bool disposing)
        {
            Close();
        }

        #endregion // IDisposable

        #region object Overrides

        /// <inheritdoc/>
        public override string ToString()
        {
            var displayName = string.Format("Pipe In: {0}, Out: {1}", DeviceInputPipeName, DeviceOutputPipeName);
            return displayName;
        }

        #endregion // object Overrides

        private static string MakeInputPipeName(string baseName)
        {
            return baseName + "Input";
        }

        private static string MakeOutputPipeName(string baseName)
        {
            return baseName + "Output";
        }
    }
}

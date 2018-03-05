// <copyright file="IStreamConnection.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using System.IO;

namespace INTV.Shared.Model
{
    /// <summary>
    /// Defines a versatile System.IO.Stream-based interface for device communication.
    /// </summary>
    public interface IStreamConnection
    {
        /// <summary>
        /// Gets the name of the communication port. The specific significance of this name depends on the concrete implementation.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the port is open for communication.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is actively in use.
        /// </summary>
        bool IsInUse { get; }

        /// <summary>
        /// Gets the stream to use for read operations on the connection.
        /// </summary>
        Stream ReadStream { get; }

        /// <summary>
        /// Gets the stream to use for write operations on the connection.
        /// </summary>
        Stream WriteStream { get; }

        /// <summary>
        /// Gets or sets the timeout value, in milliseconds, for read operations on the connection.
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout value, in milliseconds, for write operations on the connection.
        /// </summary>
        int WriteTimeout { get; set; }

        /// <summary>
        /// Opens the connection for use.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Configure the connection with implementation-specific data.
        /// </summary>
        /// <param name="configurationData">Configuration data for the connection. The expected format is in name-value pairs.</param>
        void Configure(IDictionary<string, object> configurationData);

        /// <summary>
        /// Sets the stream connection as being in use until the returned value is disposed.
        /// </summary>
        /// <param name="inUseEnded">Called when the port is no longer in use. May be <c>null</c>.</param>
        /// <returns>An instance of an IDisposable that marks the connection as being in active use until disposed.</returns>
        IDisposable SetInUse(Action inUseEnded);

        /// <summary>
        /// Estimates the amount of time it will take to transfer the given number of bytes, in milliseconds.
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes in the data transfer.</param>
        /// <returns>The estimated data transfer time in milliseconds.</returns>
        int EstimateDataTransferTime(long numberOfBytes);

        /// <summary>
        /// Enables logging on the port.
        /// </summary>
        /// <param name="logPath">Absolute path to the log file.</param>
        void EnableLogging(string logPath);

        /// <summary>
        /// Disables logging on the port.
        /// </summary>
        void DisableLogging();

        /// <summary>
        /// Log an activity message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <remarks>It is up to the implementation to define what this function does.</remarks>
        void Log(string message);
    }
}

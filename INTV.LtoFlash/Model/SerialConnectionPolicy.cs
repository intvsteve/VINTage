// <copyright file="SerialConnectionPolicy.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using INTV.Core.Model.Device;
using INTV.Shared.Model.Device;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implements IConnectionSharingPolicy for LTO Flash! usage of a serial port.
    /// </summary>
    /// <remarks>This implementation will inspect a given IConnection to determine if it is a serial
    /// port provided by a LTO Flash! device. If it is, then the connection should be considered exclusive
    /// to LTO Flash! software components, and not used by others. The device identification and connection
    /// code should ignore ports that are not considered 'exclusive'.</remarks>
    [System.ComponentModel.Composition.Export(typeof(IConnectionSharingPolicy))]
    public sealed partial class SerialConnectionPolicy : IConnectionSharingPolicy
    {
        /// <summary>The name of the policy.</summary>
        public const string PolicyName = "LTO Flash!";

        /// <summary>
        /// Gets the sharing policy. Convenient for use within the component.
        /// </summary>
        internal static SerialConnectionPolicy Instance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.SerialConnectionPolicy"/> class.
        /// </summary>
        /// <remarks>Only want MEF to create instances of this.</remarks>
        private SerialConnectionPolicy()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return PolicyName; }
        }

        /// <inheritdoc/>
        public Type PeripheralType
        {
            get { return typeof(Device); }
        }

        /// <inheritdoc/>
        public Type ConnectionType
        {
            get { return typeof(SerialPortConnection); }
        }

        /// <inheritdoc/>
        public bool ExclusiveAccess(IConnection connection)
        {
            var exclusive = false;
            if (connection.Type == INTV.Core.Model.Device.ConnectionType.Serial)
            {
                exclusive = OSExclusiveAccess(connection);
            }
            return exclusive;
        }
    }
}

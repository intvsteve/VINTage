// <copyright file="SerialPortConnectionViewModel.cs" company="INTV Funhouse">
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

using System.Linq;
using INTV.Shared.Model.Device;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel.LtoFlash
{
    /// <summary>
    /// ViewModel for a serial port connection.
    /// </summary>
    public class SerialPortConnectionViewModel : ViewModelBase, INTV.Core.Model.Device.IConnection
    {
        private SerialPortConnection _port;

        /// <summary>
        /// Initializes a new instance of SerialPortConnectionViewModel.
        /// </summary>
        /// <param name="port">The serial port connection model.</param>
        public SerialPortConnectionViewModel(SerialPortConnection port)
        {
            _port = port;
        }

        #region IConnection

        /// <inheritdoc />
        public string Name
        {
            get { return _port.Name; }
        }

        /// <inheritdoc />
        public INTV.Core.Model.Device.ConnectionType Type
        {
            get { return _port.Type; }
        }

        #endregion // IConnection

        /// <summary>
        /// Gets a value indicating whether or not the port is considered in use.
        /// </summary>
        public bool InUse
        {
            get { return SerialPortConnection.PortsInUse.Contains(Name); }
        }

        /// <summary>
        /// Gets the icon to display.
        /// </summary>
        private OSImage Icon
        {
            get { return null; }
        }
    }
}

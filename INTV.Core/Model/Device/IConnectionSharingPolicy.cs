// <copyright file="IConnectionSharingPolicy.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Implementations of this interface offer guidance for whether instances of a connection
    /// of a given type should be considered for use exclusively by particular devices (peripherals).
    /// For example, a serial port is a generic connection, and multiple devices may be interested
    /// in using it. Consider three such extant devices: Intellicart!, CuttleCart 3, and LTO Flash!.
    /// The LTO Flash!, though physically connected via USB, manifests as a serial port when connected
    /// to a computer. It has a specific command-response protocol. The Intellicart! and CuttleCart 3
    /// are passive, in that they require (Intellicart!) or may use (CuttleCart 3) a serial port.
    /// .
    /// Most newer computers no longer have traditional serial port hardware, so when using devices
    /// such as Intellicart! or CuttleCart 3's serial connection, a USB to Serial converter is used.
    /// When a software program detects the arrival of a serial port in the system via one of these
    /// devices, it may wish to limit which features will expose access to the newly arrived serial port.
    /// .
    /// Continuing with this trio of devices, and, as an example, the LTO Flash! User Interface Software
    /// built using this library, let's explore how this interface is intended to be used in detail. When
    /// the user plugs in a generic USB to Serial converter, software modules to configure communication
    /// with Intellicart! or CuttleCart 3 devices will be interested in offering the port as available
    /// for communication. On the other hand, because such a port does not belong to a LTO Flash!
    /// device, the software component for LTO Flash! should avoid probing the serial port to establish
    /// communication with the device -- it's not LTO Flash! hardware, after all, so it will not
    /// understand or respond to the communication protocol of the device. So essentially two different
    /// decisions are being made:
    ///   1) If the device can be identified as LTO Flash! hardware *without establishing communication*
    ///      then an implementation of IConnectionSharingPolicy tailored to LTO Flash! would return <c>true</c>
    ///      in its implementation of ExclusiveAccess() for the connection.
    ///   2) If an implementation such as Intellicart! support is made, then it would only be interested
    ///      in non-exclusive serial port connections.
    /// Unfortunately, this means that the decision-making needs to be implemented for any add-in module
    /// that cares about such things. 
    /// </summary>
    public interface IConnectionSharingPolicy
    {
        /// <summary>
        /// Gets the name of the policy - typically associated with a specific type of IPeripheral.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of the peripheral associated with this sharing policy.
        /// </summary>
        Type PeripheralType { get; }

        /// <summary>
        /// Gets the type of IConnection to which the policy applies.
        /// </summary>
        /// <value>The type of the connection.</value>
        Type ConnectionType { get; }

        /// <summary>
        /// Gets whether or not the policy requires that the connection can be used by any IPeripheral, or only that specified by ConnectionType.
        /// </summary>
        /// <param name="connection">The connection to check.</param>
        /// <returns><c>true</c>, if use of <paramref name="connection"/> should be restricted to a specific class of devices, <c>false</c> otherwise.</returns>
        bool ExclusiveAccess(INTV.Core.Model.Device.IConnection connection);
    }
}

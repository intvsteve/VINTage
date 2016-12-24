// <copyright file="DeviceCommandExecuteFailedException.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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

using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash
{
    /// <summary>
    /// This exception is used to report that the device returned an error directly when executing a command.
    /// </summary>
    [System.Serializable]
    public sealed class DeviceCommandExecuteFailedException : System.Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommandExecuteFailedException"/> type.
        /// </summary>
        /// <param name="command">The command that failed.</param>
        /// <param name="arg0">First argument to the command.</param>
        /// <param name="arg1">Second argument to the command.</param>
        /// <param name="arg2">Third argument to the command.</param>
        /// <param name="arg3">Fourth argument to the command.</param>
        /// <param name="deviceResponse">The response returned from the device.</param>
        public DeviceCommandExecuteFailedException(ProtocolCommandId command, uint arg0, uint arg1, uint arg2, uint arg3, byte deviceResponse)
            : base(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceCommandExecuteFailed_ExceptionMessageFormat, command, arg0, arg1, arg2, arg3, deviceResponse))
        {
            Command = command;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            DeviceResponse = deviceResponse;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private DeviceCommandExecuteFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the command that failed to execute.
        /// </summary>
        public ProtocolCommandId Command { get; private set; }

        /// <summary>
        /// Gets the first data argument to the command that failed to execute.
        /// </summary>
        public uint Arg0 { get; private set; }

        /// <summary>
        /// Gets the second data argument to the command that failed to execute.
        /// </summary>
        public uint Arg1 { get; private set; }

        /// <summary>
        /// Gets the third data argument to the command that failed to execute.
        /// </summary>
        public uint Arg2 { get; private set; }

        /// <summary>
        /// Gets the fourth data argument to the command that failed to execute.
        /// </summary>
        public uint Arg3 { get; private set; }

        /// <summary>
        /// Gets the response byte that was returned from the command.
        /// </summary>
        public byte DeviceResponse { get; private set; }

        #endregion // Properties

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
    }
}

// <copyright file="DeviceCommandFailedException.cs" company="INTV Funhouse">
// Copyright (c) 2014-2021 All Rights Reserved
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

using INTV.LtoFlash.Model;
using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash
{
    /// <summary>
    /// This exception is used to report failures during the execution of device commands.
    /// </summary>
    [System.Serializable]
    public sealed class DeviceCommandFailedException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the DeviceCommandFailedException class.
        /// </summary>
        /// <param name="command">The command that failed to execute.</param>
        /// <param name="message">The message that describes the error.</param>
        public DeviceCommandFailedException(ProtocolCommandId command, string message)
            : this(command, message, (string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeviceCommandFailedException class.
        /// </summary>
        /// <param name="command">The command that failed to execute.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorDetail">Specific information in addition to the standard message, to be included prior to stack trace.</param>
        public DeviceCommandFailedException(ProtocolCommandId command, string message, string errorDetail)
            : this(command, message, null, errorDetail)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeviceCommandFailedException class.
        /// </summary>
        /// <param name="command">The command that failed to execute.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no such exception exists.</param>
        /// <param name="errorDetail">Specific information in addition to the standard message, to be included prior to stack trace.</param>
        public DeviceCommandFailedException(ProtocolCommandId command, string message, System.Exception innerException, string errorDetail)
            : base(message, innerException)
        {
            Command = command;
            ErrorDetail = errorDetail;
        }

        /// <summary>
        /// Initializes a new instance of the DeviceCommandFailedException class.
        /// </summary>
        /// <param name="command">The command that failed to execute.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no such exception exists.</param>
        /// <param name="errorDetail">Additional information to include in the output when displaying the exception as a string.</param>
        public DeviceCommandFailedException(ProtocolCommandId command, System.Exception innerException, string errorDetail)
            : base(string.Format(Resources.Strings.DeviceCommand_Generic_FailedFormat, command), innerException)
        {
            Command = command;
            ErrorDetail = errorDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.DeviceCommandFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorLog">An error log downloaded from Locutus.</param>
        /// <param name="originalError">The exception that is the cause of the current exception, or <c>null</c> if no such exception exists.</param>
        public DeviceCommandFailedException(string message, ErrorLog errorLog, DeviceCommandExecuteFailedException originalError)
            : base(message, originalError)
        {
            Command = originalError.Command;
            ErrorLog = errorLog;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private DeviceCommandFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        /// <summary>
        /// Gets the command that failed to execute.
        /// </summary>
        public ProtocolCommandId Command { get; private set; }

        /// <summary>
        /// Gets the error log associated with the command, if available.
        /// </summary>
        public ErrorLog ErrorLog { get; private set; }

        private string ErrorDetail { get; set; }

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable

        #region object overrides

        /// <inheritdoc />
        public override string ToString()
        {
            var toString = string.Empty;
            if (!string.IsNullOrEmpty(ErrorDetail))
            {
                toString = ErrorDetail + System.Environment.NewLine + System.Environment.NewLine;
            }
            toString += base.ToString();
            return toString;
        }

        #endregion // object overrides
    }
}

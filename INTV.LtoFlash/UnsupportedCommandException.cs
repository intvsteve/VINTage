// <copyright file="UnsupportedCommandException.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash
{
    [System.Serializable]
    public sealed class UnsupportedCommandException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of of the UnsupportedCommandException class.
        /// </summary>
        /// <param name="commandId">The unsupported command.</param>
        public UnsupportedCommandException(INTV.LtoFlash.Model.Commands.ProtocolCommandId commandId)
            : base(string.Format(Resources.Strings.ProtocolCommand_UnexpectedCommandError_Format, commandId))
        {
            CommandId = commandId;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private UnsupportedCommandException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        /// <summary>
        /// Gets the ID of the command that is in an unsupported state.
        /// </summary>
        public INTV.LtoFlash.Model.Commands.ProtocolCommandId CommandId { get; private set; }

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
    }
}

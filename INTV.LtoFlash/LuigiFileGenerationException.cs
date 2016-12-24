// <copyright file="LuigiFileGenerationException.cs" company="INTV Funhouse">
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
    /// <summary>
    /// This exception is used to report various errors that may occur when creating a LUIGI file.
    /// </summary>
    [System.Serializable]
    public sealed class LuigiFileGenerationException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the LuigiFileGenerationException class.
        /// </summary>
        /// <param name="message">A brief message describing the exception.</param>
        /// <param name="description">A more detailed description of the exception.</param>
        public LuigiFileGenerationException(string message, string description)
            : base(message)
        {
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the LuigiFileGenerationException class.
        /// </summary>
        /// <param name="message">A brief message describing the exception.</param>
        /// <param name="description">A more detailed description of the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LuigiFileGenerationException(string message, string description, System.Exception innerException)
            : base(message, innerException)
        {
            Description = description;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private LuigiFileGenerationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        /// <summary>
        /// Gets a summary style description to display in the crash dialog above the full exception.
        /// </summary>
        public string Description { get; private set; }

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
    }
}

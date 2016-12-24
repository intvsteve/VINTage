// <copyright file="UnexpectedFileTypeException.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.Core
{
    /// <summary>
    /// Exception used to report unexpected file types when parsing a file.
    /// </summary>
#if !PCL
    [System.Serializable]
#endif // !PCL
    public sealed class UnexpectedFileTypeException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of UnexpectedFileTypeException.
        /// </summary>
        /// <param name="expectedTypeName">The type of file that was expected, but not found.</param>
        public UnexpectedFileTypeException(string expectedTypeName)
            : base()
        {
            ExpectedTypeName = expectedTypeName;
        }

        /// <summary>
        /// Initializes a new instance of UnexpectedFileTypeException.
        /// </summary>
        /// <param name="expectedTypeName">The type of file that was expected, but not found.</param>
        /// <param name="message">The message that describes the error.</param>
        public UnexpectedFileTypeException(string expectedTypeName, string message)
            : base(message)
        {
            ExpectedTypeName = expectedTypeName;
        }

#if !PCL
        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private UnexpectedFileTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable
#endif // !PCL

        /// <summary>
        /// Gets the type of file that was expected.
        /// </summary>
        public string ExpectedTypeName { get; private set; }

#if !PCL
        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
#endif // !PCL
    }
}

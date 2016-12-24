// <copyright file="UnsupportedForkKindException.cs" company="INTV Funhouse">
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

using INTV.LtoFlash.Model;

namespace INTV.LtoFlash
{
    /// <summary>
    /// This exception is used to report actions that attempt to use a kind of fork that is not
    /// yet supported in the software.
    /// </summary>
    [System.Serializable]
    public sealed class UnsupportedForkKindException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of of the UnsupportedForkKindException class.
        /// </summary>
        /// <param name="forkKind">The unsupported fork.</param>
        public UnsupportedForkKindException(ForkKind forkKind)
            : base(string.Format(Resources.Strings.UnsupportedForkKindException_MessageFormat, forkKind))
        {
            ForkKind = forkKind;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private UnsupportedForkKindException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        /// <summary>
        /// Gets the type of fork that is not supported.
        /// </summary>
        public ForkKind ForkKind { get; private set; }

        #region ISerializable

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable

        #endregion // ISerializable
    }
}

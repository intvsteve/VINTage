// <copyright file="InconsistentFileSystemException.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
    /// This exception is used to report that the file system on a Locutus device may be in an inconsistent state.
    /// </summary>
    [System.Serializable]
    public sealed class InconsistentFileSystemException : FailedOperationException
    {
        /// <summary>
        /// Initializes a new instance of the InconsistentFileSystemException class with a custom message.
        /// </summary>
        /// <param name="message">A message describing the cause of the exception.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        public InconsistentFileSystemException(string message, string targetDeviceId)
            : this(message, LfsEntityType.Unknown, uint.MaxValue, targetDeviceId)
        {
        }

        /// <summary>
        /// Initializes a new instance of of the InconsistentFileSystemException class.
        /// </summary>
        /// <param name="entityType">The type of the file system entity that could not be located.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        public InconsistentFileSystemException(LfsEntityType entityType, string targetDeviceId)
            : this(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FileSystem_InconsistencyError_MissingEntry_Format, entityType), entityType, uint.MaxValue, targetDeviceId)
        {
        }

        /// <summary>
        /// Initializes a new instance of of the InconsistentFileSystemException class.
        /// </summary>
        /// <param name="entityType">The type of the file system entity that could not be located.</param>
        /// <param name="globalFileSystemNumber">The global file system identifier of the entity that could not be located.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        public InconsistentFileSystemException(LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId)
            : this(entityType, globalFileSystemNumber, targetDeviceId, Resources.Strings.FileSystem_InconsistencyError_Format)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.InconsistentFileSystemException"/> class.
        /// </summary>
        /// <param name="message">Custom message for the inconsistency.</param>
        /// <param name="entityType">The type of the file system entity that could not be located.</param>
        /// <param name="globalFileSystemNumber">The global file system identifier of the entity that could not be located.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        public InconsistentFileSystemException(string message, LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId)
            : this(message, entityType, globalFileSystemNumber, targetDeviceId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of of the InconsistentFileSystemException class.
        /// </summary>
        /// <param name="entityType">The type of the file system entity that could not be located.</param>
        /// <param name="globalFileSystemNumber">The global file system identifier of the entity that could not be located.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        /// <param name="customErrorFormat">Custom error format for the message. Must have {0} for entity type and {1} for global file system number.</param>
        public InconsistentFileSystemException(LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId, string customErrorFormat)
            : this(string.Format(customErrorFormat, entityType, globalFileSystemNumber), entityType, globalFileSystemNumber, targetDeviceId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of of the InconsistentFileSystemException class.
        /// </summary>
        /// <param name="entityType">The type of the file system entity that could not be located.</param>
        /// <param name="globalFileSystemNumber">The global file system identifier of the entity that could not be located.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no such exception exists.</param>
        public InconsistentFileSystemException(LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId, System.Exception innerException)
            : this(string.Format(Resources.Strings.FileSystem_InconsistencyError_Format, entityType, globalFileSystemNumber), entityType, globalFileSystemNumber, targetDeviceId, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of of the InconsistentFileSystemException class.
        /// </summary>
        /// <param name="message">Custom message for the inconsistency.</param>
        /// <param name="entityType">The type of the file system entity that could not be located.</param>
        /// <param name="globalFileSystemNumber">The global file system identifier of the entity that could not be located.</param>
        /// <param name="targetDeviceId">The unique ID of the device whose file system is in an inconsistent state.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no such exception exists.</param>
        private InconsistentFileSystemException(string message, LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId, System.Exception innerException)
            : base(message, entityType, globalFileSystemNumber, targetDeviceId, innerException)
        {
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private InconsistentFileSystemException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
    }
}

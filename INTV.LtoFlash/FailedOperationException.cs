// <copyright file="FailedOperationException.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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
    /// This exception acts as the base class for reporting errors that may arise during file system operations.
    /// </summary>
    [System.Serializable]
    public class FailedOperationException : System.Exception
    {
        /// <summary>
        /// Creates a new instance of the exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="entityType">The kind of file system entity the error pertains to.</param>
        /// <param name="globalFileSystemNumber">The file system number (GDN, GFN, GKN) of the entry that the error pertains to.</param>
        /// <param name="targetDeviceId">Unique ID of the target device of the operation when the error occurred. If <c>null</c> or empty, target device information is not available, or no target was involved.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        protected FailedOperationException(string message, LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId, System.Exception innerException)
            : base(message, innerException)
        {
            EntityType = entityType;
            GlobalFileSystemNumber = globalFileSystemNumber;
            TargetDeviceId = targetDeviceId;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        protected FailedOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        /// <summary>
        /// Gets the kind of file system element that could not be located.
        /// </summary>
        public LfsEntityType EntityType { get; private set; }

        /// <summary>
        /// Gets the global file system number (directory, file, or fork) of the entity that could not be located.
        /// </summary>
        public uint GlobalFileSystemNumber { get; private set; }

        /// <summary>
        /// Gets the unique ID (DRUID) of the target device.
        /// </summary>
        public string TargetDeviceId { get; private set; }

        /// <summary>
        /// Wraps the given exception as the InnerException if it is not already a FailedOperationException.
        /// </summary>
        /// <param name="error">The error to wrap.</param>
        /// <param name="entityType">The kind of file system entity the error pertains to.</param>
        /// <param name="globalFileSystemNumber">The file system number (GDN, GFN, GKN) of the entry that the error pertains to.</param>
        /// <param name="targetDeviceId">The unique ID of the device involved in the operation. If empty or <c>null</c>, the error is not targeting actual hardware.</param>
        /// <returns>If <paramref name="error"/> is a <see cref="FailedOperationException"/>, then it is returned unchanged, otherwise it is the inner exception for a new <see cref="FailedOperationException"/>.</returns>
        public static FailedOperationException WrapIfNeeded(System.Exception error, LfsEntityType entityType, uint globalFileSystemNumber, string targetDeviceId)
        {
            var failedComparison = error as FailedOperationException;
            if (failedComparison == null)
            {
                failedComparison = new FailedOperationException(error.Message, entityType, globalFileSystemNumber, targetDeviceId, error);
            }
            return failedComparison;
        }

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
    }
}

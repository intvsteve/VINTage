// <copyright file="IncompatibleRomException.cs" company="INTV Funhouse">
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

using INTV.Core.Model;
using INTV.LtoFlash.Model;

namespace INTV.LtoFlash
{
    /// <summary>
    /// This exception is raised when attempting to place a ROM onto a device that cannot execute the ROM.
    /// </summary>
    [System.Serializable]
    public class IncompatibleRomException : FailedOperationException
    {
        /// <summary>
        /// Initialize a new instance of IncompatibleRomException.
        /// </summary>
        /// <param name="rom">The ROM that cannot execute on the target device.</param>
        /// <param name="requiredDeviceId">Unique ID of the device needed to execute the ROM.</param>
        /// <param name="targetDeviceId">Unique ID of the device upon which ROM execution will fail.</param>
        /// <param name="entityType">The type of the file system entity that is incompatible.</param>
        /// <param name="globalFileSystemNumber">The global file system identifier of the entity that is incompatible.</param>
        public IncompatibleRomException(IRom rom, string requiredDeviceId, string targetDeviceId, LfsEntityType entityType, uint globalFileSystemNumber)
            : base(string.Format(Resources.Strings.IncompatibleRomException_MessageFormat, rom.RomPath, requiredDeviceId, targetDeviceId), entityType, globalFileSystemNumber, targetDeviceId, null)
        {
            Rom = rom;
            RequiredDeviceId = requiredDeviceId;
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private IncompatibleRomException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion // ISerializable

        /// <summary>
        /// Gets the ROM that cannot be executed on the target device.
        /// </summary>
        public IRom Rom { get; private set; }

        /// <summary>
        /// Gets the unique ID (DRUID) of the device needed to execute the ROM.
        /// </summary>
        public string RequiredDeviceId { get; private set; }

        #region ISerializable

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable
    }
}

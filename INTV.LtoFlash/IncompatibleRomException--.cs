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

namespace INTV.LtoFlash
{
    /// <summary>
    /// This exception is raised when attempting to place a ROM onto a device that cannot execute the ROM.
    /// </summary>
    [System.Serializable]
    public sealed class IncompatibleRomException : System.Exception
    {
        public IncompatibleRomException(IRom rom, string requiredDeviceId, string targetDeviceId)
            : base(string.Format("The ROM at path '{0}' can only execute on the LTO Flash! device with unique ID '{1}'. The target device has unique ID '{2}'.", rom.RomPath, requiredDeviceId, targetDeviceId))
        {
            Rom = rom;
            RequiredDeviceId = requiredDeviceId;
        }

        /// <summary>
        /// Gets the ROM that cannot be executed on the device.
        /// </summary>
        public IRom Rom { get; private set; }

        /// <summary>
        /// Gets the unique ID of the device that can execute the ROM.
        /// </summary>
        public string RequiredDeviceId { get; private set; }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private IncompatibleRomException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}

// <copyright file="DeviceStatusResponse.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// This class encapsulates the response data from the device's Ping and GarbageCollect commands.
    /// </summary>
    internal sealed class DeviceStatusResponse : INTV.Core.Utility.ByteSerializer
    {
        /// <summary>
        /// Size of the serialized form of the structure, in bytes.
        /// </summary>
        public const int FlatSizeInBytes = UniqueIdSize + StatusBytesSize;

        /// <summary>
        /// The size of the unique identifier, in bytes.
        /// </summary>
        internal const int UniqueIdSize = 16;

        private const int StatusBytesSize = sizeof(DeviceStatusFlagsLo) + sizeof(DeviceStatusFlagsHi);

        #region Properties

        /// <summary>
        /// Gets the unique ID of the device, presented as a string.
        /// </summary>
        public string UniqueId { get; private set; }

        /// <summary>
        /// Gets the low half of the device status bits.
        /// </summary>
        public DeviceStatusFlagsLo DeviceStatusLow { get; private set; }

        /// <summary>
        /// Gets the high half of the device status bits.
        /// </summary>
        public DeviceStatusFlagsHi DeviceStatusHigh { get; private set; }

        /// <summary>
        /// Gets the hardware status flags of the device.
        /// </summary>
        public HardwareStatusFlags HardwareStatus
        {
            get { return DeviceStatusLow.ToHardwareStatusFlags(); }
        }

        /// <summary>
        /// Gets the Intellivision II-specific status flags of the device.
        /// </summary>
        public IntellivisionIIStatusFlags IntellivisionIIStatus
        {
            get { return DeviceStatusLow.ToIntellivisionIICompatibilityFlags(); }
        }

        /// <summary>
        /// Gets the ECS-specific status flags of the device.
        /// </summary>
        public EcsStatusFlags EcsStatus
        {
            get { return DeviceStatusLow.ToEcsCompatibilityFlags(); }
        }

        /// <summary>
        /// Gets the flags indicating the title screen behavior of the device.
        /// </summary>
        public ShowTitleScreenFlags ShowTitleScreen
        {
            get { return DeviceStatusLow.ToShowTitleScreenFlags(); }
        }

        /// <summary>
        /// Gets the flags indicating the save menu position behavior of the device.
        /// </summary>
        public SaveMenuPositionFlags SaveMenuPosition
        {
            get { return DeviceStatusLow.ToSaveMenuPositionFlags(); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the device runs garbage collection on the file system in the background while at the menu.
        /// </summary>
        public bool BackgroundGC
        {
            get { return DeviceStatusLow.ToBackgroundGC(); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the device emits keyclicks when navigating the menu.
        /// </summary>
        public bool Keyclicks
        {
            get { return DeviceStatusLow.ToKeyclicks(); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the device allows access to its onboard configuration menu.
        /// </summary>
        public bool EnableConfigMenuOnCart
        {
            get { return DeviceStatusLow.ToEnableOnboardConfigMenu(); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the device sets LTO Flash! RAM to zero when loading a ROM.
        /// </summary>
        public bool ZeroLtoFlashRam
        {
            get { return DeviceStatusLow.ToZeroLtoFlashRam(); }
        }

        #region ByteSerializer Properties

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        #endregion // ByteSerializer Properties

        #endregion // Properties

        /// <summary>
        /// Creates a new instance of a DeviceStatusResponse by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a FileSystemStatistics.</returns>
        public static DeviceStatusResponse Inflate(System.IO.Stream stream)
        {
            return Inflate<DeviceStatusResponse>(stream);
        }

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override int Deserialize(Core.Utility.BinaryReader reader)
        {
            var responseBuffer = new byte[UniqueIdSize];
            var numRead = reader.Read(responseBuffer, 0, UniqueIdSize);
            var lowPart = System.BitConverter.ToUInt64(responseBuffer, 0);
            var highPart = System.BitConverter.ToUInt64(responseBuffer, 8);
            UniqueId = highPart.ToString("X16") + lowPart.ToString("X16");
            DeviceStatusLow = (DeviceStatusFlagsLo)reader.ReadUInt64();
            DeviceStatusHigh = (DeviceStatusFlagsHi)reader.ReadUInt64();
            numRead += StatusBytesSize;

            // TODO: throw here?
            System.Diagnostics.Debug.Assert(numRead == DeserializeByteCount, "Failed to deserialize correct number of bytes in DeviceStatusResponse.");
            return numRead;
        }

        #endregion // ByteSerializer
    }
}

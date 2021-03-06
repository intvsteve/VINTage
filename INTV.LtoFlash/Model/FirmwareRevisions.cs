﻿// <copyright file="FirmwareRevisions.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Firmware revision information from a Locutus device.
    /// </summary>
    /// <remarks>From the specification:
    /// int32_t     primary revision
    /// int32_t     secondary revision, -1 means "not present"
    /// int32_t     active revision (should equal primary or secondary)
    /// </remarks>
    public class FirmwareRevisions : INTV.Core.Utility.ByteSerializer
    {
        #region Constants

        /// <summary>
        /// The flat size in bytes.
        /// </summary>
        public const int FlatSizeInBytes = sizeof(int) * 3;

        /// <summary>
        /// Indicates that a firmware version number is not available.
        /// </summary>
        public const int UnavailableFirmwareVersion = -1;

        /// <summary>
        /// Mask to check if firmware version indicates primary vs. secondary.
        /// </summary>
        public const int SecondaryMask = 1 << 0;

        /// <summary>
        /// Mask to check if firmware version was built with code not yet submitted.
        /// </summary>
        public const int UnofficialReleaseMask = 1 << 1;

        /// <summary>
        /// Mask to apply to a raw version number to remove additional information flags.
        /// </summary>
        public const int BaseVersionMask = ~(SecondaryMask | UnofficialReleaseMask);

        /// <summary>
        /// Number of descriptive bits in firmware version number (least significant bits).
        /// </summary>
        public const int BaseVersionBitOffset = 2;

        /// <summary>
        /// Used to indicate that firmware information is not available from a Locutus device.
        /// </summary>
        public static readonly FirmwareRevisions Unavailable = new FirmwareRevisions();

        /// <summary>
        /// Prefix used to locate embedded resources related to firmware updates and error databases.
        /// </summary>
        internal const string FirmwareUpdateResourcePrefix = "INTV.LtoFlash.Resources.FirmwareUpdates.";

        private const long FirmwareUpdateVersionOffset = 0xC0;
        private const int FirmwareVersionSizeInBytes = 3;
        private const int FirmwareRevisionNeedsDoublingAfter = 0x08A2;

        #endregion // Constants

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.FirmwareRevisions"/> class.
        /// </summary>
        public FirmwareRevisions()
        {
            Primary = UnavailableFirmwareVersion;
            Secondary = UnavailableFirmwareVersion;
            Current = UnavailableFirmwareVersion;
        }

        #region Properites

        /// <summary>
        /// Gets the primary firmware version number. This is the 'factory' firmware.
        /// </summary>
        /// <remarks>NOTE: This value includes secondary and unofficial release information flags.</remarks>
        public int Primary { get; private set; }

        /// <summary>
        /// Gets the secondary firmware version number. This is the 'updated' firmware, if applicable.
        /// </summary>
        /// <remarks>NOTE: This value includes secondary and unofficial release information flags.</remarks>
        public int Secondary { get; private set; }

        /// <summary>
        /// Gets the currently running firmware.
        /// </summary>
        /// <remarks>NOTE: This value includes secondary and unofficial release information flags.</remarks>
        public int Current { get; private set; }

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

        public static int GetFirmwareVersion(int rawFirmwareVersion)
        {
            var version = (rawFirmwareVersion & BaseVersionMask) >> BaseVersionBitOffset;
            return version;
        }

        /// <summary>
        /// Converts a raw firmware revision value to a display string.
        /// </summary>
        /// <param name="version">The value to convert.</param>
        /// <param name="useRawValue">If <c>true</c>, appends suffix to indicate if the version indicates secondary or unreleased firmware.</param>
        /// <returns>The version as a string.</returns>
        public static string FirmwareVersionToString(int version, bool useRawValue)
        {
            var valueToConvert = useRawValue ? version : GetFirmwareVersion(version);
            var versionString = Resources.Strings.FileSystemStatisticsView_Unavailable;
            if (version != LtoFlash.Model.FirmwareRevisions.UnavailableFirmwareVersion)
            {
                versionString = valueToConvert.ToString();
                var secondarySuffix = string.Empty;
                var unreleasedSuffix = string.Empty;
                if (useRawValue && ((version & FirmwareRevisions.SecondaryMask) == FirmwareRevisions.SecondaryMask))
                {
                    secondarySuffix = Resources.Strings.FirmwareRevision_ToStringSecondarySuffix;
                    versionString += " (secondary)";
                }
                if ((version & FirmwareRevisions.UnofficialReleaseMask) == FirmwareRevisions.UnofficialReleaseMask)
                {
                    unreleasedSuffix = Resources.Strings.FirmwareRevision_ToStringUnreleasedSuffix;
                    versionString += "+";
                }
                versionString = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FirmwareRevision_ToStringFormat, valueToConvert, secondarySuffix, unreleasedSuffix);
            }
            return versionString;
        }

        /// <summary>
        /// Extracts the version of the firmware from its data stream.
        /// </summary>
        /// <param name="firmwareImageDataStream">A data stream that contains a firmware image for a Locutus device.</param>
        /// <returns>The firmware version.</returns>
        public static int GetFirmwareVersionFromBinaryImage(System.IO.Stream firmwareImageDataStream)
        {
            firmwareImageDataStream.Seek(FirmwareUpdateVersionOffset, System.IO.SeekOrigin.Begin);
            var versionBuffer = new byte[4];
            firmwareImageDataStream.Read(versionBuffer, 0, FirmwareVersionSizeInBytes);
            var firmwareVersion = System.BitConverter.ToInt32(versionBuffer, 0);
            if (firmwareVersion > FirmwareRevisionNeedsDoublingAfter)
            {
                firmwareVersion *= 2;
            }
            return firmwareVersion;
        }

        #region ByteSerializer

        /// <summary>
        /// Creates a new instance of a FirmwareRevisions by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a FirmwareRevisions.</returns>
        public static FirmwareRevisions Inflate(System.IO.Stream stream)
        {
            return Inflate<FirmwareRevisions>(stream);
        }

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override int Deserialize(Core.Utility.BinaryReader reader)
        {
            Primary = reader.ReadInt32();
            Secondary = reader.ReadInt32();
            Current = reader.ReadInt32();
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer
    }
}

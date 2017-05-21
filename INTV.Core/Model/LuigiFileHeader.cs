// <copyright file="LuigiFileHeader.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

using System;
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Describes the fixed header portion of a LUIGI file, per the specification.
    /// </summary>
    public class LuigiFileHeader : INTV.Core.Utility.ByteSerializer
    {
        /// <summary>
        /// Version of the header currently supported. All previous versions will also be supported.
        /// </summary>
        public const byte CurrentVersion = 1;

        #region Constants for all LUIGI header versions (Version 0)

        /// <summary>
        /// Size of feature flags (in bytes) in the base version of the header.
        /// </summary>
        internal const int BaseVersionFlagsSize = sizeof(LuigiFeatureFlags);

        /// <summary>
        /// Offset (in bytes) into the header to the feature data.
        /// </summary>
        internal const int FeatureBytesOffset = MagicKeySize + sizeof(byte);

        /// <summary>
        /// Number of reserved bytes in the header.
        /// </summary>
        internal const int ReservedHeaderBytesSize = 3;

        /// <summary>
        /// Header checksum size in bytes.
        /// </summary>
        internal const int HeaderChecksumSize = sizeof(byte);

        private const int BaseFlatSize = FeatureBytesOffset + BaseVersionFlagsSize + ReservedHeaderBytesSize + HeaderChecksumSize;
        private const int MagicKeySize = 3;

        #endregion // Constants for all LUIGI header versions (Version 0)

        #region Constants for LUIGI header version 1 and newer

        /// <summary>
        /// Size of the UID member in bytes.
        /// </summary>
        internal const int VersionOneUidSize = sizeof(ulong);

        /// <summary>
        /// Byte offset to the UID of the LUIGI header.
        /// </summary>
        internal const int UidByteOffset = FeatureBytesOffset + BaseVersionFlagsSize + VersionOneAdditionalFeaturesSize;

        /// <summary>
        /// Size of additional feature flags (in bytes) defined in version 1 and later.
        /// </summary>
        internal const int VersionOneAdditionalFeaturesSize = sizeof(LuigiFeatureFlags2);

        private const int OriginalRomKeySize = sizeof(uint);
        private const int OriginalRomCrc32Size = sizeof(uint);
        private const int VersionOneAdditionalSize = VersionOneAdditionalFeaturesSize + VersionOneUidSize;

        private const int VersionOneFlatSize = BaseFlatSize + VersionOneAdditionalSize;

        #endregion // Constants for LUIGI header version 1 and newer

        /// <summary>
        /// Gets the size of the largest known LUIGI header.
        /// </summary>
        public static readonly int MaxHeaderSize = Math.Max(BaseFlatSize, VersionOneFlatSize);

        private static readonly LuigiHeaderMemo Memos = new LuigiHeaderMemo();

        #region Key data for all LUIGI header versions (Version 0)

        /// <summary>
        /// The "magic key" identifying file contents.
        /// </summary>
        private static readonly byte[] MagicKey = new byte[3] { (byte)'L', (byte)'T', (byte)'O' };

        #endregion // Key data for all LUIGI header versions (Version 0)

        #region Key data for LUIGI header version 1

        private static readonly byte[] RomKey = new byte[4] { (byte)'.', (byte)'R', (byte)'O', (byte)'M' };
        private static readonly byte[] BinKey = new byte[4] { (byte)'.', (byte)'B', (byte)'I', (byte)'N' };

        #endregion // Key data for LUIGI header version 1

        #region Properties

        /// <summary>
        /// Gets the magic key identifier of the file.
        /// </summary>
        public byte[] Magic { get; private set; }

        /// <summary>
        /// Gets the version of the LUIGI file format.
        /// </summary>
        public byte Version { get; private set; }

        /// <summary>
        /// Gets the ROM's features.
        /// </summary>
        public LuigiFeatureFlags Features { get; private set; }

        /// <summary>
        /// Gets the ROM's features.
        /// </summary>
        public LuigiFeatureFlags2 Features2 { get; private set; }

        /// <summary>
        /// Gets the unique ID of the file.
        /// </summary>
        /// <remarks>Not set in version 0; only valid in versions 1 and later.</remarks>
        public ulong Uid { get; private set; }

        /// <summary>
        /// Gets the format of the ROM used to create the LUIGI file.
        /// </summary>
        /// <remarks>This value is set to RomFormat.None for LUIGI files created in early versions of the tools. Only set in version 1 and later.</remarks>
        public RomFormat OriginalRomFormat { get; private set; }

        /// <summary>
        /// Gets the 32-bit ZIP-style CRC of the original ROM or BIN format.
        /// </summary>
        /// <remarks>This value is only valid if the LUIGI file was created using version 1 or later, and from a .rom or .bin format ROM.</remarks>
        public uint OriginalRomCrc32 { get; private set; }

        /// <summary>
        /// Gets the 32-bit ZIP-style CRC of the original CFG file for a BIN format ROM.
        /// </summary>
        /// <remarks>This value is only valid if the LUIGI file was created using version 1a or later, and from a .bin format ROM.</remarks>
        public uint OriginalCfgCrc32 { get; private set; }

        /// <summary>
        /// Gets or sets the unused (reserved) portion of the header.
        /// </summary>
        public byte[] Reserved { get; set; }

        /// <summary>
        /// Gets the DOWCRC (8-bit CRC) of the header.
        /// </summary>
        public byte Crc { get; private set; }

        #region ByteSerializer Properties

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return -1; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return _deserializeByteCount; }
        }

        private int _deserializeByteCount = -1;

        #endregion // ByteSerializer Properties

        #endregion // Properties

        /// <summary>
        /// Determines whether the given file contents begin with a LUIGI file header.
        /// </summary>
        /// <param name="filePath">The file to check to see if it could be a LUIGI file.</param>
        /// <returns><c>true</c> if the file contents begin with the 'magic' for a LUIGI header.</returns>
        public static bool PotentialLuigiFile(string filePath)
        {
            LuigiFileHeader header;
            bool isPotentialLuigiFile = Memos.CheckAddMemo(filePath, null, out header) && (header != null);
            return isPotentialLuigiFile;
        }

        /// <summary>
        /// Get the LUIGI file header from the ROM at the given absolute path.
        /// </summary>
        /// <param name="filePath">Absolute path to the ROM whose LUIGI header is desired.</param>
        /// <returns>The LUIGI header, or <c>null</c> if the ROM is not of the expected format.</returns>
        public static LuigiFileHeader GetHeader(string filePath)
        {
            LuigiFileHeader header;
            Memos.CheckAddMemo(filePath, null, out header);
            return header;
        }

        /// <summary>
        /// Creates a new instance of a LuigiFileHeader by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a LuigiFileHeader.</returns>
        public static LuigiFileHeader Inflate(System.IO.Stream stream)
        {
            return Inflate<LuigiFileHeader>(stream);
        }

        /// <summary>
        /// Creates a new instance of a LuigiFileHeader by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a LuigiFileHeader.</returns>
        public static LuigiFileHeader Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<LuigiFileHeader>(reader);
        }

        /// <summary>
        /// Determines whether the given features would cause a change to this header's feature bits.
        /// </summary>
        /// <param name="features">The proposed new features to apply to the header.</param>
        /// <param name="forceFeatureUpdate">If set to <c>true</c> force feature update, even if the features were set explicitly by a .cfg file.</param>
        /// <returns><c>true</c>, if the new features differ from existing ones, and would cause an update, <c>false</c> otherwise.</returns>
        public bool WouldModifyFeatures(LuigiFeatureFlags features, bool forceFeatureUpdate)
        {
            var willModifyFeatures = false;
            var hasFlagsFromCfgFile = Features.HasFlag(LuigiFeatureFlags.FeatureFlagsExplicitlySet);
            if (!hasFlagsFromCfgFile || forceFeatureUpdate)
            {
                var currentFlags = Features & ~(LuigiFeatureFlags.UnusedMask | LuigiFeatureFlags.FeatureFlagsExplicitlySet);
                var newFlags = features & ~(LuigiFeatureFlags.UnusedMask | LuigiFeatureFlags.FeatureFlagsExplicitlySet);
                willModifyFeatures = currentFlags != newFlags;
            }
            return willModifyFeatures;
        }

        /// <summary>
        /// Update the feature flags.
        /// </summary>
        /// <param name="features">The new feature flags.</param>
        /// <param name="forceFeatureUpdate">If <c>true</c>, apply ROM features even if already set via config file.</param>
        /// <remarks>This also updates the CRC of the header.</remarks>
        public void UpdateFeatures(LuigiFeatureFlags features, bool forceFeatureUpdate)
        {
            if (WouldModifyFeatures(features, forceFeatureUpdate))
            {
                Features = features;
                var flatData = SerializeToBuffer();
                Crc = Crc8.OfBlock(flatData);
            }
        }

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            writer.Write(MagicKey);
            var bytesWritten = MagicKey.Length;

            writer.Write(Version);
            bytesWritten += sizeof(byte);

            writer.Write((ulong)Features);
            bytesWritten += sizeof(LuigiFeatureFlags);

            if (Version > 0)
            {
                writer.Write((ulong)Features2);
                bytesWritten += sizeof(LuigiFeatureFlags2);
                writer.Write(Uid);
                bytesWritten += sizeof(ulong);
            }

            writer.Write(Reserved);
            bytesWritten += Reserved.Length;

            writer.Write(Crc);
            bytesWritten += HeaderChecksumSize;

            return bytesWritten;
        }

        /// <inheritdoc />
        protected override int Deserialize(Core.Utility.BinaryReader reader)
        {
            var header = reader.ReadBytes(MagicKey.Length);
            var bytesRead = header.Length;
            if (!header.SequenceEqual(MagicKey))
            {
                throw new INTV.Core.UnexpectedFileTypeException("LUIGI");
            }

            Version = reader.ReadByte();
            bytesRead += sizeof(byte);
            if (Version > LuigiFileHeader.CurrentVersion)
            {
                // Perhaps throwing here is a bad idea...
                throw new System.InvalidOperationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UnsupportedLuigiVersion_Format, Version));
            }

            Features = (LuigiFeatureFlags)reader.ReadUInt64();
            bytesRead += sizeof(LuigiFeatureFlags);
            if (Version > 0)
            {
                Features2 = (LuigiFeatureFlags2)reader.ReadUInt64();
                bytesRead += sizeof(LuigiFeatureFlags2);
                var uidBytes = reader.ReadBytes(VersionOneUidSize);
                Uid = BitConverter.ToUInt64(uidBytes, 0);
                bytesRead += sizeof(ulong);
                var originalFormatKey = uidBytes.Skip(OriginalRomCrc32Size).Take(OriginalRomKeySize);
                var originalCrcData = uidBytes.Take(OriginalRomCrc32Size).ToArray();
                if (originalFormatKey.SequenceEqual(RomKey))
                {
                    OriginalRomFormat = RomFormat.Rom;
                    OriginalRomCrc32 = BitConverter.ToUInt32(originalCrcData, 0);
                }
                else
                {
                    OriginalRomFormat = RomFormat.Bin;
                    OriginalRomCrc32 = BitConverter.ToUInt32(originalCrcData, 0);
                    if (!originalFormatKey.SequenceEqual(BinKey))
                    {
                        OriginalCfgCrc32 = BitConverter.ToUInt32(originalFormatKey.ToArray(), 0);
                    }
                }
            }

            Reserved = reader.ReadBytes(ReservedHeaderBytesSize);
            bytesRead += ReservedHeaderBytesSize;

            Crc = reader.ReadByte();
            bytesRead += HeaderChecksumSize;

#if DEBUG
            System.Diagnostics.Debug.Assert((bytesRead == BaseFlatSize) || (bytesRead == VersionOneFlatSize), "Invalid LUIGI header size.");
            reader.BaseStream.Seek(-bytesRead, System.IO.SeekOrigin.Current);
            var data = reader.ReadBytes(bytesRead - 1);
            var crc8 = Crc8.OfBlock(data);
            reader.BaseStream.Seek(bytesRead, System.IO.SeekOrigin.Begin);
            System.Diagnostics.Debug.Assert(crc8 == Crc, "Failed to correctly compute DOWCRC for LUIGI header!");
#endif
            _deserializeByteCount = bytesRead;
            return bytesRead;
        }

        private byte[] SerializeToBuffer()
        {
            byte[] buffer = null;
            var memory = new System.IO.MemoryStream();
            using (var writer = new BinaryWriter(memory))
            {
                writer.Write(MagicKey);
                var bytesWritten = MagicKey.Length;

                writer.Write(Version);
                bytesWritten += sizeof(byte);

                writer.Write((ulong)Features);
                bytesWritten += sizeof(LuigiFeatureFlags);

                if (Version > 0)
                {
                    writer.Write((ulong)Features2);
                    bytesWritten += sizeof(LuigiFeatureFlags2);
                    writer.Write(Uid);
                    bytesWritten += sizeof(ulong);
                }

                writer.Write(Reserved);
                bytesWritten += Reserved.Length;
            }
            buffer = memory.ToArray();
            return buffer;
        }

        #endregion // ByteSerializer

        private class LuigiHeaderMemo : FileMemo<LuigiFileHeader>
        {
            /// <inheritdoc />
            protected override LuigiFileHeader DefaultMemoValue
            {
                get { return null; }
            }

            /// <inheritdoc />
            protected override LuigiFileHeader GetMemo(string filePath, object data)
            {
                LuigiFileHeader luigiHeader = null;
                try
                {
                    using (var file = filePath.OpenFileStream())
                    {
                        var reader = new INTV.Core.Utility.BinaryReader(file);
                        byte[] header = reader.ReadBytes(MagicKey.Length);
                        if (header.SequenceEqual(MagicKey))
                        {
                            file.Seek(0, System.IO.SeekOrigin.Begin);
                            luigiHeader = LuigiFileHeader.Inflate(file);
                        }
                    }
                }
                catch (INTV.Core.UnexpectedFileTypeException)
                {
                    // Just in case the header looks OK, but turns out to be bad.
                }
                return luigiHeader;
            }

            /// <inheritdoc />
            protected override bool IsValidMemo(LuigiFileHeader memo)
            {
                return memo != null;
            }
        }
    }
}

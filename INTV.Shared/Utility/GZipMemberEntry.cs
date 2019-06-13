// <copyright file="GZipMemberEntry.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Operating systems listed in the GZIP specification here: http://www.zlib.org/rfc-gzip.html
    /// </summary>
    internal enum GZipOS : byte
    {
        /// <summary>FAT filesystem (MS-DOS, OS/2, NT/Win32)</summary>
        Windows = 0,

        /// <summary>Amiga OS</summary>
        Amiga,

        /// <summary>VMS (or OpenVMS)</summary>
        VMS,

        /// <summary>Unix OS</summary>
        Unix,

        /// <summary>VM/CMS OS</summary>
        VMCMS,

        /// <summary>Atari TOS</summary>
        Atari,

        /// <summary>HPFS filesystem (OS/2, NT)</summary>
        HPFS,

        /// <summary>Macintosh operating systems</summary>
        Macintosh,

        /// <summary>Z-System OS</summary>
        ZSystem,

        /// <summary>CP/M OS</summary>
        CPM,

        /// <summary>TOPS-20 OS</summary>
        TOPS20,

        /// <summary>NTFS filesystem (NT)</summary>
        NTFS,

        /// <summary>QDOS OS</summary>
        QDOS,

        /// <summary>Acorn RISCOS</summary>
        AcornRISCOS,

        /// <summary>Unknown or unidentified OS</summary>
        Unknown = 255
    }

    /// <summary>
    /// Provides an implementation of <see cref="ICompressedArchiveEntry"/> that attempts to parse out additional information about
    /// the contents of a GZIP member based on the specification.
    /// </summary>
    /// <remarks>This type's <see cref="Inflate(Stream)"/> and similar overloads will parse out the standard information that may (or may not) be provided
    /// in the header block of a GZIP file, per RFC 1952 here: http://www.zlib.org/rfc-gzip.html 
    /// If multiple GZIP files are concatenated together to form one large multi-member file, the <see cref="GetMemberEntries(Stream, int)"/>
    /// method provides a potentially brittle attempt to identify them. The mechanism has some unreliability in that it does not inflate the
    /// compressed data blocks after the header, rather it linearly searches for the next potential header. Therefore, false matches may be encountered.
    /// The behavior in such a scenario has not been rigorously tested, though the intent is, perhaps one day, to keep searching.
    /// Note that because the original CRC and file length data is stored in the footer of the compressed member, and the deflated data is not
    /// formally inflated, the reliability of the <see cref="Crc32"/> and <see cref="Length"/> values is dependent upon the quality of the input GZIP.
    /// Members with extraneous data at the end of the file will return invalid values for those properties.
    /// </remarks>
    internal sealed class GZipMemberEntry : INTV.Core.Utility.ByteSerializer, ICompressedArchiveEntry
    {
        /// <summary>
        /// IDdentifier as listed in the specification here: http://www.zlib.org/rfc-gzip.html
        /// </summary>
        private static readonly byte[] MagicIdentifier = new byte[] { 0x1F, 0x8B };

        private static readonly string StockEntryName = "file.dat";

        #region ByteSerializer

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
        private int _deserializeByteCount = 0;

        #endregion // ByteSerializer

        #region ICompressedArchiveEntry

        /// <inheritdoc />
        /// <remarks>If a GZIP member was originally compressed to include this information, it may be provided.</remarks>
        public string Name
        {
            get { return _name; }
        }
        private string _name;

        /// <inheritdoc />
        /// <remarks>If a GZIP member was originally compressed to include this information, it may be provided. See class notes.</remarks>
        public long Length
        {
            get { return _length; }
        }
        private long _length = -1;

        /// <inheritdoc />
        /// <remarks>If a GZIP member was originally compressed to include this information, it may be provided.</remarks>
        public DateTime LastModificationTime
        {
            get { return _lastModificationTime; }
        }
        private DateTime _lastModificationTime = DateTime.MinValue;

        /// <inheritdoc />
        public bool IsDirectory
        {
            get { return false; }
        }

        #endregion // ICompressedArchiveEntry

        /// <summary>
        /// Gets the operating system (more akin to file system) under which the GZIP was created.
        /// </summary>
        /// <remarks>macOS reports as Unix, Windows still reports as Windows, and Linux reports as Unix.</remarks>
        public GZipOS OperatingSystem { get; private set; }

        /// <summary>
        /// Gets the comment attached to the GZIP entry, if any.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Gets the CRC-C32 (a.k.a. ZIP) of the original file prior to compression via GZIP.
        /// </summary>
        public uint Crc32 { get; set; }

        /// <summary>
        /// Gets the offset into the original stream to locate the entry.
        /// </summary>
        internal long Offset { get; private set; }

        private MetadataFlags Flags { get; set; }

        private ExtraFlags CompressionHint { get; set; }

        private CompressionMethod Compression { get; set; }

        private SubfieldId SubfieldIdentifier { get; set; }

        private ushort Crc16 { get; set; }

        /// <summary>
        /// Attempts to identify the members in a GZIP stream, even when multiple GZIPped files have been concatenated.
        /// </summary>
        /// <param name="stream">The stream containing GZIP entries.</param>
        /// <param name="maxNumberOfEntries">The maximum number of entries to attempt to identify. A value less than zero indicates to attempt to identify all members.</param>
        /// <returns>An enumerable of identifiable GZIP entries.</returns>
        /// <remarks>This method will, with the default parameters, visit every byte in <paramref name="stream"/>. It is incumbent
        /// upon the caller to determine if, for the sake of performance, the <paramref name="maxNumberOfEntries"/> should be limited.
        /// The position within <paramref name="stream"/> will be restored to its initial location. Note that the accuracy of the Crc32
        /// and Length values is not guaranteed. If an error is encountered during the 'member walk' the method stops and returns
        /// the known entries.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The BinaryReader constructor keeps the input stream open.")]
        public static IEnumerable<GZipMemberEntry> GetMemberEntries(Stream stream, int maxNumberOfEntries = -1)
        {
            var length = stream.Length;
            var initialPosition = stream.Position;
            var currentPosition = stream.Seek(0, SeekOrigin.Begin);
            var memberEntries = new List<GZipMemberEntry>();
            if (maxNumberOfEntries < 0)
            {
                maxNumberOfEntries = int.MaxValue;
            }

            var entryOffset = 0L;
            while ((memberEntries.Count < maxNumberOfEntries) && (stream.Position < length))
            {
                try
                {
                    var entry = Inflate(stream);
                    entry.Offset = entryOffset;
                    memberEntries.Add(entry);
                    if (memberEntries.Count < maxNumberOfEntries)
                    {
                        using (var reader = new Core.Utility.BinaryReader(stream))
                        {
                            currentPosition = FindNextEntry(reader, entry);
                            entryOffset = currentPosition;
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // We thought we found another entry, but failed to parse the header - so we hit
                    // a false positive. We *could* resume the search and keep on searching... but
                    // this is all pretty fragile so let's just give up.
                    break;
                }
            }

            var defaultFileName = GetDefaultEntryName(stream);
            var baseFileName = Path.GetFileNameWithoutExtension(defaultFileName);
            var originalFileExtension = Path.GetExtension(defaultFileName);
            for (int i = 0; i < memberEntries.Count; ++i)
            {
                var entry = memberEntries[i];
                if (!entry.Flags.HasFlag(MetadataFlags.FileName) && (memberEntries.FindIndex(e => e.Name == defaultFileName) != i))
                {
                    entry._name = string.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", baseFileName, i, originalFileExtension);
                }
            }

            stream.Seek(initialPosition, SeekOrigin.Begin);
            return memberEntries;
        }

        #region ByteSerializer

        /// <summary>
        /// Creates a new instance of a GZipMemberEntry by parsing it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a GZipMemberEntry.</returns>
        /// <remarks>NOTE: The Length and Crc32 values will not be set.
        /// Do not confuse this with actually inflating the data that the GZIP has compressed! this method
        /// only harvests the metadata from a GZIP stream.</remarks>
        public static GZipMemberEntry Inflate(Stream stream)
        {
            return Inflate<GZipMemberEntry>(stream);
        }

        /// <summary>
        /// Creates a new instance of a GZipMemberEntry by parsing it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a GZipMemberEntry.</returns>
        /// <remarks>NOTE: The Length and Crc32 values will not be set.
        /// Do not confuse this with actually inflating the data that the GZIP has compressed! this method
        /// only harvests the metadata from a GZIP stream.</remarks>
        public static GZipMemberEntry Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            return Inflate<GZipMemberEntry>(reader);
        }

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override int Deserialize(Core.Utility.BinaryReader reader)
        {
            int bytesRead = 0;

            CheckHeader(reader, ref bytesRead);
            Compression = ReadCompressionMethod(reader, ref bytesRead);
            Flags = ReadFlags(reader, ref bytesRead);
            _lastModificationTime = ReadLastModifiedTime(reader, ref bytesRead);
            CompressionHint = ReadExtraFlags(reader, ref bytesRead);
            OperatingSystem = ReadOperatingSystem(reader, ref bytesRead);
            SubfieldIdentifier = ReadExtraFields(reader, Flags, ref bytesRead);
            _name = ReadFileName(reader, Flags, ref bytesRead);
            Comment = ReadComment(reader, Flags, ref bytesRead);
            Crc16 = ReadCrc16(reader, Flags, ref bytesRead);
            _deserializeByteCount = bytesRead;

            return bytesRead;
        }

        #endregion ByteSerializer

        private static long FindNextEntry(Core.Utility.BinaryReader reader, GZipMemberEntry currentEntry)
        {
            var foundMagic = false;
            var positionOfNextEntry = -1L;
            try
            {
                var magic = reader.ReadByte();
                while (true)
                {
                    if (magic == MagicIdentifier[0])
                    {
                        magic = reader.ReadByte();
                        if (magic == MagicIdentifier[1])
                        {
                            foundMagic = true;
                            break;
                        }
                    }
                    else
                    {
                        magic = reader.ReadByte();
                    }
                }
            }
            catch (EndOfStreamException)
            {
                positionOfNextEntry = reader.BaseStream.Length;
            }
            catch (IOException)
            {
            }

            if (foundMagic || (positionOfNextEntry == reader.BaseStream.Length))
            {
                // go back 4 (CRC32) + 4 (size mod 2^32) [+ 2 (magic)]
                var offset = sizeof(uint) + sizeof(int);
                var origin = SeekOrigin.End;
                if (foundMagic)
                {
                    offset += MagicIdentifier.Length;
                    origin = SeekOrigin.Current;
                }
                reader.BaseStream.Seek(-offset, origin);
                currentEntry.Crc32 = reader.ReadUInt32();
                currentEntry._length = reader.ReadInt32();
                positionOfNextEntry = reader.BaseStream.Position;
            }

            return positionOfNextEntry;
        }

        private static void CheckHeader(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var identifier = reader.ReadBytes(MagicIdentifier.Length);
            bytesRead += identifier.Length;
            if (!identifier.SequenceEqual(MagicIdentifier))
            {
                throw new InvalidOperationException(Resources.Strings.GZipHeaderError_InvalidMagic);
            }
        }

        private static CompressionMethod ReadCompressionMethod(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var compressionMethod = (CompressionMethod)reader.ReadByte();
            bytesRead += sizeof(CompressionMethod);
            if (compressionMethod != CompressionMethod.Deflate)
            {
                throw new InvalidOperationException(Resources.Strings.GZipHeaderError_InvalidCompression);
            }
            return compressionMethod;
        }

        private static MetadataFlags ReadFlags(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var flags = (MetadataFlags)reader.ReadByte();
            bytesRead += sizeof(MetadataFlags);
            if (flags.HasFlag(MetadataFlags.Reserved5 | MetadataFlags.Reserved6 | MetadataFlags.Reserved7))
            {
                throw new InvalidOperationException(Resources.Strings.GZipHeaderError_ReservedFlagsSet);
            }
            return flags;
        }

        private static DateTime ReadLastModifiedTime(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var lastModifiedTime = DateTime.MinValue;
            var modifiedTimeUnixUtc = reader.ReadUInt32();
            if (modifiedTimeUnixUtc != 0)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                lastModifiedTime = epoch.AddSeconds(modifiedTimeUnixUtc);
            }
            bytesRead += sizeof(uint);
            return lastModifiedTime;
        }

        private static ExtraFlags ReadExtraFlags(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var extraFlags = (ExtraFlags)reader.ReadByte();
            bytesRead += sizeof(ExtraFlags);
            return extraFlags;
        }

        private static GZipOS ReadOperatingSystem(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var operatingSystem = (GZipOS)reader.ReadByte();
            bytesRead += sizeof(GZipOS);
            return operatingSystem;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private static SubfieldId ReadExtraFields(Core.Utility.BinaryReader reader, MetadataFlags flags, ref int bytesRead)
        {
            var subfieldIdentifier = SubfieldId.None;
            if (flags.HasFlag(MetadataFlags.ExtraFieldsPresent))
            {
                subfieldIdentifier = (SubfieldId)reader.ReadUInt16();
                bytesRead += sizeof(SubfieldId);

                var subfieldDataLength = reader.ReadUInt16(); // little-endian
                bytesRead += sizeof(ushort);
                reader.BaseStream.Seek(subfieldDataLength, SeekOrigin.Current); // just skip
            }
            return subfieldIdentifier;
        }

        private static string ReadFileName(Core.Utility.BinaryReader reader, MetadataFlags flags, ref int bytesRead)
        {
            string name = null;
            if (flags.HasFlag(MetadataFlags.FileName))
            {
                name = ReadNullTerminatedString(reader, ref bytesRead);
            }
            else
            {
                name = GetDefaultEntryName(reader.BaseStream);
            }
            return name;
        }

        private static string GetDefaultEntryName(Stream stream)
        {
            var defaultEntryName = StockEntryName;
            var fileStream = stream as FileStream;
            if (fileStream != null)
            {
                if (!string.IsNullOrEmpty(fileStream.Name))
                {
                    defaultEntryName = Path.GetFileNameWithoutExtension(Path.GetFileName(fileStream.Name));
                }
            }
            return defaultEntryName;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private static string ReadComment(Core.Utility.BinaryReader reader, MetadataFlags flags, ref int bytesRead)
        {
            string comment = null;
            if (flags.HasFlag(MetadataFlags.Comment))
            {
                comment = ReadNullTerminatedString(reader, ref bytesRead);
            }
            return comment;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private static ushort ReadCrc16(Core.Utility.BinaryReader reader, MetadataFlags flags, ref int bytesRead)
        {
            ushort crc16 = 0;
            if (flags.HasFlag(MetadataFlags.Crc16Present))
            {
                crc16 = reader.ReadUInt16();
                bytesRead += sizeof(ushort);
            }
            return crc16;
        }

        private static string ReadNullTerminatedString(Core.Utility.BinaryReader reader, ref int bytesRead)
        {
            var fileNameBytes = new List<byte>();
            byte currentCharacter = 0;
            do
            {
                currentCharacter = reader.ReadByte();
                ++bytesRead;
                fileNameBytes.Add(currentCharacter);
            }
            while (currentCharacter != 0);

            var encoding = Encoding.GetEncoding("iso-8859-1");

            var stringValue = encoding.GetString(fileNameBytes.ToArray(), 0, fileNameBytes.Count - 1);
            return stringValue;
        }

        private enum CompressionMethod : byte
        {
            /// <summary>Reserved compression method.</summary>
            Reserved0,

            /// <summary>Reserved compression method.</summary>
            Reserved1,

            /// <summary>Reserved compression method.</summary>
            Reserved2,

            /// <summary>Reserved compression method.</summary>
            Reserved3,

            /// <summary>Reserved compression method.</summary>
            Reserved4,

            /// <summary>Reserved compression method.</summary>
            Reserved5,

            /// <summary>Reserved compression method.</summary>
            Reserved6,

            /// <summary>Reserved compression method.</summary>
            Reserved7,

            /// <summary>The standard 'Deflate' compression method.</summary>
            Deflate = 8
        }

        /// <summary>
        /// Flags in the GZIP member header.
        /// </summary>
        [Flags]
        private enum MetadataFlags : byte
        {
            /// <summary>No flags set.</summary>
            None = 0,

            /// <summary>If set, compressed member is probably an ASCII text file.</summary>
            Text = 1 << 0,

            /// <summary>If set, a CRC16 is present immediately before the compressed member. The CRC16 is defined to be the two least significant bytes of the
            /// total CRC32 of the GZIP header up to - but not including - the CRC16 header itself.</summary>
            Crc16Present = 1 << 1,

            /// <summary>If set, optional extra fields are present.</summary>
            ExtraFieldsPresent = 1 << 2,

            /// <summary>If set, the file name is present as a NULL-terminated string using ISO 8859-1 (LATIN-1) characters.</summary>
            FileName = 1 << 3,

            /// <summary>If set, a NULL-terminated string using ISO 8859-1 (LATIN-1) characters is present. Line breaks always use line feed (0x0A).</summary>
            Comment = 1 << 4,

            /// <summary>Reserved. Must be zero.</summary>
            Reserved5 = 1 << 5,

            /// <summary>Reserved. Must be zero.</summary>
            Reserved6 = 1 << 6,

            /// <summary>Reserved. Must be zero.</summary>
            Reserved7 = 1 << 7
        }

        /// <summary>
        /// Extra flags used by compression algorithm.
        /// </summary>
        [Flags]
        private enum ExtraFlags : byte
        {
            /// <summary>No value set.</summary>
            None,

            /// <summary>Maximum compression, slowest speed algorithm was used..</summary>
            MaximumCompression = 1 << 1,

            /// <summary>Fastest compression algorithm was used..</summary>
            FastestCompression = 1 << 2
        }

        /// <summary>
        /// Some known subfield types.
        /// </summary>
        private enum SubfieldId : ushort
        {
            /// <summary>No subfield ID.</summary>
            None = 0,

            /// <summary>Apollo file type information.</summary>
            ApolloFileTypeInformation = 0x4170, // Ap

            /// <summary>Random Access -- dictzip </summary>
            DictionaryField = 0x5241, // RA
        }
    }
}

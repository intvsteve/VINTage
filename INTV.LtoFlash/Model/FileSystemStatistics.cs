// <copyright file="FileSystemStatistics.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// File system information from a Locutus device.
    /// </summary>
    /// <remarks>From the specification:
    /// struct lfs_info
    /// {
    ///     uint16_t    vblks_avail;    // Number of virtual blocks available
    ///     uint16_t    vblks_total;    // Total number of virtual blocks
    ///     uint16_t    psect_clean;    // Number of clean physical sectors
    ///     uint16_t    pblks_total;    // Total number of physical blocks
    ///     uint32_t    psect_erase;    // Number of physical sector erasures
    ///     uint32_t    msect_erase;    // Number of metadata sector erasures
    ///     uint32_t    v2pm_version;   // Number of V2PM log wraps
    ///     uint8_t     reserved[236];  // Round out entire data structure to 256
    ///     ... other FTL health details TBD.
    /// };
    /// Size of this structure (to allow future expansion):  256 bytes.
    /// </remarks>
    public class FileSystemStatistics : INTV.Core.Utility.ByteSerializer
    {
        #region Constants

        /// <summary>
        /// The size of the flattened binary structure, in bytes.
        /// </summary>
        public const int FlatSizeInBytes = 256;

        private const int ReservedSize = FlatSizeInBytes - ((sizeof(ushort) * 4) + (sizeof(uint) * 3));

        #endregion // Constants

        #region Properties

        /// <summary>
        /// Gets a value reporting the number of virtual blocks available.
        /// </summary>
        public ushort VirtualBlocksAvailable { get; private set; }

        /// <summary>
        /// Gets a value reporting the number of virtual blocks in use.
        /// </summary>
        public ushort VirtualBlocksInUse
        {
            get { return (ushort)(VirtualBlocksTotal - VirtualBlocksAvailable); }
        }

        /// <summary>
        /// Gets a value reporting the total number of virtual blocks.
        /// </summary>
        public ushort VirtualBlocksTotal { get; private set; }

        /// <summary>
        ///  Gets a value reporting the number of free physical sectors that are clean.
        /// </summary>
        public ushort PhysicalSectorsClean { get; private set; }

        /// <summary>
        ///  Gets a value reporting the number of free physical blocks that are clean.
        /// </summary>
        public uint PhysicalBlocksClean
        {
            get { return (uint)(PhysicalSectorsClean * 8); }
        }

        /// <summary>
        ///  Gets a value reporting the number of physical blocks in use.
        /// </summary>
        public uint PhysicalBlocksInUse
        {
            get { return PhysicalBlocksTotal - PhysicalBlocksClean; }
        }

        /// <summary>
        ///  Gets a value reporting the total number of physical blocks.
        /// </summary>
        public ushort PhysicalBlocksTotal { get; private set; }

        /// <summary>
        ///  Gets a value reporting the number of physical sector erasures.
        /// </summary>
        public uint PhysicalSectorErasures { get; private set; }

        /// <summary>
        ///  Gets a value reporting the number of metadata sector erasures.
        /// </summary>
        public uint MetadataSectorErasures { get; private set; }

        /// <summary>
        /// Gets the number of Virtual to Physical log wraps.
        /// </summary>
        public uint VirtualToPhysicalMapVersion { get; private set; }

        /// <summary>
        /// Gets the reserved data in the data structure.
        /// </summary>
        public byte[] ReservedData { get; private set; }

        /// <summary>
        /// Gets a value approximating the portion of the flash memory's lifetime used due to physical sector erasures.
        /// </summary>
        /// <remarks>NOTE: This value is already in percent!</remarks>
        public double LifetimeUsedDueByPhysicalSectorErasure
        {
            get
            {
                var percentLifetimeUsed = (3.0 * PhysicalSectorErasures) / 65536;
                return percentLifetimeUsed;
            }
        }

        /// <summary>
        /// Gets a value approximating the portion of the flash memory's lifetime used due to the VtoP map.
        /// </summary>
        /// <remarks>NOTE: This value is already in percent!</remarks>
        public double LifetimeUsedByVirtualToPhysicalMapLog
        {
            get
            {
                var percentLifteimeUsed = (128.0 * VirtualToPhysicalMapVersion) / 65536;
                return percentLifteimeUsed;
            }
        }

        /// <summary>
        /// Gets a value approximating the portion of lifetime remaining for the flash memory, with 1.0 meaning all lifetime remaining.
        /// </summary>
        /// <remarks>NOTE: This value is already in percent!</remarks>
        public double RemainingFlashLifetime
        {
            get
            {
                var remainingLifetime = System.Math.Max(100.0 - System.Math.Max(LifetimeUsedDueByPhysicalSectorErasure, LifetimeUsedByVirtualToPhysicalMapLog), 0);
                return remainingLifetime;
            }
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

        #endregion Properties

        #region ByteSerializer

        /// <summary>
        /// Creates a new instance of a FileSystemStatistics by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a FileSystemStatistics.</returns>
        public static FileSystemStatistics Inflate(System.IO.Stream stream)
        {
            return Inflate<FileSystemStatistics>(stream);
        }

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var areEqual = base.Equals(obj);
            var other = obj as FileSystemStatistics;
            if (other != null)
            {
                areEqual = (VirtualBlocksAvailable == other.VirtualBlocksAvailable) &&
                           (VirtualBlocksTotal == other.VirtualBlocksTotal) &&
                           (PhysicalSectorsClean == other.PhysicalSectorsClean) && 
                           (PhysicalBlocksTotal == other.PhysicalBlocksTotal) &&
                           (PhysicalSectorErasures == other.PhysicalSectorErasures) &&
                           (MetadataSectorErasures == other.MetadataSectorErasures) &&
                           (VirtualToPhysicalMapVersion == other.VirtualToPhysicalMapVersion);

                           // ignoring ReservedData
            }
            return areEqual;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = (VirtualBlocksAvailable << 16) | VirtualBlocksTotal;
            hashCode ^= (PhysicalSectorsClean << 16) | PhysicalBlocksTotal;
            hashCode ^= (int)(PhysicalSectorErasures ^ MetadataSectorErasures ^ VirtualToPhysicalMapVersion);
            return hashCode;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            VirtualBlocksAvailable = reader.ReadUInt16();
            VirtualBlocksTotal = reader.ReadUInt16();
            PhysicalSectorsClean = reader.ReadUInt16();
            PhysicalBlocksTotal = reader.ReadUInt16();
            PhysicalSectorErasures = reader.ReadUInt32();
            MetadataSectorErasures = reader.ReadUInt32();
            VirtualToPhysicalMapVersion = reader.ReadUInt32();
            ReservedData = reader.ReadBytes(ReservedSize);
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer
    }
}

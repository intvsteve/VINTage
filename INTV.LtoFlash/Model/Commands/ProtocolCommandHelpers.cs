// <copyright file="ProtocolCommandHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using System.Linq;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Useful commands for Locutus command execution.
    /// </summary>
    internal static class ProtocolCommandHelpers
    {
        /// <summary>
        /// Ensures an address is at least word aligned.
        /// </summary>
        /// <param name="address">The address to align.</param>
        /// <returns>The aligned address.</returns>
        internal static uint Align(this uint address)
        {
            if ((address & 1) == 1)
            {
                ++address;
            }
            return address;
        }

        /// <summary>
        /// Validate an address and block size to ensure it can be uploaded to a Locutus device.
        /// </summary>
        /// <param name="address">The destination address in RAM on the Locutus device.</param>
        /// <param name="blockLength">The size of the potential data block, in bytes.</param>
        internal static void ValidateDataBlockSizeAndAddress(uint address, int blockLength)
        {
            ValidateDataBlockSize(blockLength);
            ValidateDataBlockAddress(address);
            if ((address + (uint)blockLength) > Device.TotalRAMSize)
            {
                throw new InsufficientMemoryException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ValidateDataBlock_BadSizeOrAddressFormat, blockLength, address));
            }
        }

        /// <summary>
        /// Ensures that the data block will fit in the RAM on a Locutus device.
        /// </summary>
        /// <param name="blockLength">The size of the potential data block, in bytes.</param>
        private static void ValidateDataBlockSize(int blockLength)
        {
            if (blockLength > Device.TotalRAMSize)
            {
                throw new InsufficientMemoryException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ValidateDataBlock_BlockTooLargeFormat, blockLength));
            }
            else if (blockLength == 0)
            {
                throw new ArgumentOutOfRangeException(Resources.Strings.ValidateDataBlock_ZeroBlockSize);
            }
        }

        /// <summary>
        /// Ensures that the given address is valid to upload data to a Locutus device.
        /// </summary>
        /// <param name="address">The destination address in RAM on the Locutus device.</param>
        private static void ValidateDataBlockAddress(uint address)
        {
            if (!((address >= 0) && (address < (Device.TotalRAMSize - 1))))
            {
                throw new ArgumentOutOfRangeException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ValidateDataBlock_BadAddressFormat, address));
            }
            if ((address & 1) == 1)
            {
                throw new DataMisalignedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ValidateDataBlock_BadAddressAlignmentFormat, address));
            }
        }

        /// <summary>
        /// Given a collection of LFS objects, coalesce them into contiguous blocks that can be uploaded to the device.
        /// </summary>
        /// <typeparam name="T">The type of LFS object to coalesce.</typeparam>
        /// <param name="initialAddress">The initial location in Locutus RAM to start the upload.</param>
        /// <param name="entries">The entries to arrange into 'chunks'.</param>
        /// <param name="getGlobalTableNumber">The function to get the appropriate global identifier for the file system object.</param>
        /// <returns>A queue containing 'chunks' of data to be uploaded to a Locutus device.</returns>
        /// <remarks>This function arranges the objects in order of their global file system numbers, as these adjacent
        /// elements can be updated in bulk operations, reducing the traffic between the host PC and the device. After sorting by
        /// file system number, as many adjacent elements as possible are fit into a block of memory, with the next available
        /// address beginning the next 'chunk'.</remarks>
        internal static Queue<FileSystemTableUploadBlock<T>> PrepareGlobalFileSystemTableEntriesForUpload<T>(uint initialAddress, IEnumerable<T> entries, Func<T, uint> getGlobalTableNumber) where T : IGlobalFileSystemEntry
        {
            // Break entries up into contiguous chunks to deploy. These are arranged in a dictionary in which the base
            // global file system number is the key, and an entry is the address in RAM to upload to along with the
            // file system entities to be loaded into RAM.
            var orderedEntries = entries.OrderBy(d => getGlobalTableNumber(d)).ToArray();
            var chunks = new Dictionary<uint, ContiguousFileSystemEntries<T>>();
            var chunkEntries = new List<T>() { orderedEntries[0] };
            var chunkAddress = initialAddress;
            chunks[getGlobalTableNumber(orderedEntries[0])] = new ContiguousFileSystemEntries<T>(chunkAddress, chunkEntries);

            int runningChunkSize = orderedEntries[0].EntryUpdateSize;
            ValidateDataBlockSizeAndAddress(chunkAddress, runningChunkSize);

            for (int i = 1; i < orderedEntries.Length; ++i)
            {
                var isContiguous = getGlobalTableNumber(orderedEntries[i]) == (getGlobalTableNumber(orderedEntries[i - 1]) + 1);
                var fitsIntoRam = (chunkAddress + runningChunkSize + orderedEntries[i].EntryUpdateSize) < Device.TotalRAMSize;
                var runningChunkSizeIsWordAligned = (runningChunkSize & 1) == 0;
                if (isContiguous && fitsIntoRam && runningChunkSizeIsWordAligned)
                {
                    // Entry is next GDN and will fit into RAM, so allow the chunk to expand.
                    chunkEntries.Add(orderedEntries[i]);
                    runningChunkSize += orderedEntries[i].EntryUpdateSize;
                }
                else
                {
                    // Either the entry is not contiguous with the previous, or it won't fit into RAM. So we need to start a new chunk.
                    if ((!isContiguous || !runningChunkSizeIsWordAligned) && fitsIntoRam)
                    {
                        // Start a new chunk. Keep appending it to the existing chunk of RAM.
                        chunkAddress = chunkAddress + (uint)runningChunkSize;
                        if (!runningChunkSizeIsWordAligned)
                        {
                            ++chunkAddress;
                            fitsIntoRam = (chunkAddress + runningChunkSize + orderedEntries[i].EntryUpdateSize) < Device.TotalRAMSize;
                        }
                    }
                    if (!fitsIntoRam)
                    {
                        // Start a new chunk at the beginning of RAM.
                        chunkAddress = 0;
                    }
                    runningChunkSize = orderedEntries[i].EntryUpdateSize;
                    ValidateDataBlockSizeAndAddress(chunkAddress, runningChunkSize);
                    chunkEntries = new List<T>() { orderedEntries[i] };
                    chunks[getGlobalTableNumber(orderedEntries[i])] = new ContiguousFileSystemEntries<T>(chunkAddress, chunkEntries);
                }
            }

            var queue = new Queue<FileSystemTableUploadBlock<T>>();
            foreach (var chunk in chunks)
            {
                queue.Enqueue(new FileSystemTableUploadBlock<T>(chunk.Key, chunk.Value));
            }
            return queue;
        }
    }
}

// <copyright file="FileSystemHelpers.cs" company="INTV Funhouse">
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

#define IGNORE_EMPTY_FORK_PATHS
////#define DEBUG_ENTRY_REMOVAL
////#define ENABLE_ACTIVITY_LOGGER
////#define REPORT_PERFORMANCE
////#define RECORD_PREPARE_FOR_DEPLOYMENT_VISITS
#define USE_SPECIALIZED_SIMPLE_COMPARE

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Extension methods to assist with sending and receiving LFS information.
    /// </summary>
    internal static class FileSystemHelpers
    {
#if REPORT_PERFORMANCE
        internal static INTV.Shared.Utility.Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger(System.IO.Path.Combine(Configuration.Instance.ErrorLogDirectory, "FileSystemCompareLog.txt"));
                }
                return _logger;
            }
        }
        private static INTV.Shared.Utility.Logger _logger;

        private static TimeSpan _accumulatedForkValidation = TimeSpan.Zero;
        private static TimeSpan _accumulatedCanExecuteOnDevice = TimeSpan.Zero;
        private static TimeSpan _accumulatedForkCompare = TimeSpan.Zero;
        private static TimeSpan _accumulatedFilesUsingForks = TimeSpan.Zero;
#endif // REPORT_PERFORMANCE

        /// <summary>
        /// Decompresses entries from a LFS table.
        /// </summary>
        /// <param name="reader">A binary reader containing data that should contain a compressed file system table.</param>
        /// <param name="destination">The destination stream that receives the decompressed file system table entries.</param>
        /// <param name="header">The header identifying the table.</param>
        /// <param name="entrySize">The size (in bytes) of the entries in the table.</param>
        /// <param name="maxNumEntries">The maximum number of entries that can fit in the table.</param>
        public static void DecompressedFixedSizeCollection(ASCIIBinaryReader reader, System.IO.Stream destination, byte[] header, int entrySize, int maxNumEntries)
        {
            using (var writer = new ASCIIBinaryWriter(destination))
            {
                var previousReadTimeout = System.Threading.Timeout.Infinite;
                if (reader.BaseStream.CanTimeout)
                {
                    previousReadTimeout = reader.BaseStream.ReadTimeout;
                }
                try
                {
                    if (reader.BaseStream.CanTimeout)
                    {
                        reader.BaseStream.ReadTimeout = 500;
                    }
                    var headerBytes = reader.ReadBytes(header.Length);
                    if (!header.SequenceEqual(headerBytes))
                    {
                        var errorMessage = string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            Resources.Strings.DeviceCommand_InvalidFileSystemHeader_Format,
                            System.Text.ASCIIEncoding.Default.GetString(header),
                            BitConverter.ToString(headerBytes).Replace("-", string.Empty));
                        throw new DeviceCommandFailedException(Commands.ProtocolCommandId.LfsDownloadGlobalTables, errorMessage);
                    }
                    writer.Write(headerBytes);
                    ushort numEntries = reader.ReadUInt16();
                    if (numEntries > maxNumEntries)
                    {
                        var errorMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceCommand_InvalidFileSystemEntryCount_Format, numEntries, maxNumEntries);
                        throw new DeviceCommandFailedException(Commands.ProtocolCommandId.LfsDownloadGlobalTables, errorMessage);
                    }
                    writer.Write(numEntries);

                    int numChunksToRead = (int)Math.Ceiling((double)numEntries / 8);
                    for (int i = 0; i < numChunksToRead; ++i)
                    {
                        var validEntriesMask = reader.ReadByte();
                        writer.Write(validEntriesMask);
                        for (int e = 0; e < 8; ++e)
                        {
                            byte mask = (byte)(1 << e);
                            if ((mask & validEntriesMask) == mask)
                            {
                                writer.Write(reader.ReadBytes(entrySize));
                            }
                        }
                    }
                }
                finally
                {
                    // If we somehow lose the stream, it could go null.
                    if ((reader.BaseStream != null) && reader.BaseStream.CanTimeout)
                    {
                        reader.BaseStream.ReadTimeout = previousReadTimeout;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the GKNs of forks that represent JLP Flash save data in the given file system.
        /// </summary>
        /// <param name="fileSystem">The file system to get save data global fork numbers from.</param>
        /// <returns>An enumerable of the save data global fork numbers.</returns>
        public static IEnumerable<ushort> GetSaveDataForkNumbers(this FileSystem fileSystem)
        {
            var saveDataForkNumbers = fileSystem.GetSaveDataForks().Select(k => k.GlobalForkNumber);
            return saveDataForkNumbers;
        }

        /// <summary>
        /// Gets the forks that represent JLP Flash save data in the given file system.
        /// </summary>
        /// <param name="fileSystem">The file system to get save data global fork numbers from.</param>
        /// <returns>An enumerable of the save data forks.</returns>
        public static IEnumerable<Fork> GetSaveDataForks(this FileSystem fileSystem)
        {
            var saveDataForks = fileSystem.GetFilesWithSaveDataFork().Select(f => f.JlpFlash).Distinct();
            return saveDataForks;
        }

        /// <summary>
        /// Gets the GKNs of forks that match a certain kind of fork (ROM, JLP Flash, et. al.).
        /// </summary>
        /// <param name="fileSystem">The file system in which to locate certain kinds of forks.</param>
        /// <param name="forkKind">The kind of forks to get.</param>
        /// <returns>An enumerable of the global fork numbers of the given kind.</returns>
        public static IEnumerable<ushort> GetForkNumbersOfKind(this FileSystem fileSystem, ForkKind forkKind)
        {
            var forkNumbersOfKindInUse = fileSystem.GetFilesWithValidForkOfKind(forkKind).Select(f => f.ForkNumbers[(int)forkKind]).Distinct();
            return forkNumbersOfKindInUse;
        }

        /// <summary>
        /// Gets the forks of the given kind in the given file system.
        /// </summary>
        /// <param name="fileSystem">The file system in which to locate certain kinds of forks.</param>
        /// <param name="forkKind">The kind of forks to get.</param>
        /// <returns>An enumerable of the forks of the given kind.</returns>
        public static IEnumerable<Fork> GetForksOfKind(this FileSystem fileSystem, ForkKind forkKind)
        {
            var forks = fileSystem.GetForkNumbersOfKind(forkKind).Select(k => fileSystem.Forks[k]);
            return forks;
        }

        /// <summary>
        /// Gets the files that have a valid save data fork.
        /// </summary>
        /// <param name="fileSystem">The file system to search for files that have a JLP Flash fork.</param>
        /// <returns>An enumerable of the files with a save data fork.</returns>
        public static IEnumerable<ILfsFileInfo> GetFilesWithSaveDataFork(this FileSystem fileSystem)
        {
            return fileSystem.GetFilesWithValidForkOfKind(ForkKind.JlpFlash);
        }

        /// <summary>
        /// Gets the files that have a valid fork of the given kind.
        /// </summary>
        /// <param name="fileSystem">The file system to search.</param>
        /// <param name="forkKind">The kind of fork that should be valid to include the file in the output.</param>
        /// <returns>Enumerable of the files with valid fork of the given kind.</returns>
        public static IEnumerable<ILfsFileInfo> GetFilesWithValidForkOfKind(this FileSystem fileSystem, ForkKind forkKind)
        {
            var files = fileSystem.Files.Where(f => (f != null) && (f.ForkNumbers[(int)forkKind] != GlobalForkTable.InvalidForkNumber));
            return files;
        }

        /// <summary>
        /// Gets a dictionary containing the first file found that uses a given fork. The forks act as keys for the returned dictionary.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="forks">The forks for which files are being located.</param>
        /// <returns>A dictionary that contains key value pairs that consist of a fork with at least one file that refers to
        /// the fork that acts as the key.</returns>
        public static Dictionary<Fork, ILfsFileInfo> GetFilesUsingForks(this FileSystem fileSystem, IEnumerable<Fork> forks)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            var filesForForks = new Dictionary<Fork, ILfsFileInfo>();
            foreach (var fork in forks)
            {
                var file = (fork == null) ? null : fileSystem.Files.FirstOrDefault(f => (f != null) && f.ForkNumbers.Contains(fork.GlobalForkNumber));
                if (file != null)
                {
                    filesForForks[fork] = file;
                }
            }
            return filesForForks;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.GetFilesUsingForks() took: + " + stopwatch.Elapsed.ToString());
                _accumulatedFilesUsingForks += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Gets a dictionary containing all files that refer to each of the given forks. The forks act as keys to the returned dictionary.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="forks">The forks for which files are being located.</param>
        /// <returns>A dictionary that includes key value pairs in which the keys are forks that have at least one file
        /// referring to them. The corresponding value for each key is an enumerable of all the files in the given file
        /// system that refer to the fork.</returns>
        public static Dictionary<Fork, IEnumerable<ILfsFileInfo>> GetAllFilesUsingForks(this FileSystem fileSystem, IEnumerable<Fork> forks)
        {
            var allFilesForForks = new Dictionary<Fork, IEnumerable<ILfsFileInfo>>();
            foreach (var fork in forks)
            {
                var files = (fork == null) ? null : fileSystem.Files.Where(f => (f != null) && (f.ForkNumbers != null) && f.ForkNumbers.Contains(fork.GlobalForkNumber));
                if ((files != null) && files.Any())
                {
                    allFilesForForks[fork] = files;
                }
            }
            return allFilesForForks;
        }

        /// <summary>
        /// Gets the kind of the given fork.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="fork">The fork whose kind is desired.</param>
        /// <returns>The fork kind.</returns>
        public static ForkKind GetForkKind(this FileSystem fileSystem, Fork fork)
        {
            ILfsFileInfo file = null;
            return fileSystem.GetForkKind(fork, out file);
        }

        /// <summary>
        /// Gets the kind of the fork and the first file located that refers to the given fork.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="fork">The fork whose kind is desired.</param>
        /// <param name="file">Receives the first file in <paramref name="fileSystem"/> that refers to the given fork.</param>
        /// <returns>The fork kind.</returns>
        public static ForkKind GetForkKind(this FileSystem fileSystem, Fork fork, out ILfsFileInfo file)
        {
            file = null;
            var forkKind = ForkKind.None;
            if ((fork != null) && (fork.GlobalForkNumber != GlobalForkTable.InvalidForkNumber))
            {
                file = fileSystem.Files.FirstOrDefault(f => (f != null) && f.ForkNumbers.Contains(fork.GlobalForkNumber));
                if (file != null)
                {
                    var index = file.ForkNumbers.ToList().IndexOf(fork.GlobalForkNumber);
                    if (index >= 0)
                    {
                        forkKind = (ForkKind)index;
                    }
                }
            }
            return forkKind;
        }

#if NOT_IMPLEMENTED

        /// <summary>
        /// Removes the save data fork differences.
        /// </summary>
        /// <param name="referenceFileSystem">The reference file system.</param>
        /// <param name="otherFileSystem">The other file system.</param>
        /// <param name="differences">Differences between the two file systems.</param>
        /// <returns>The file system differences with all differences due to save data forks removed.</returns>
        public static LfsDifferences RemoveSaveDataForkDifferences(this FileSystem referenceFileSystem, FileSystem otherFileSystem, LfsDifferences differences)
        {
            // Examine the differences and remove any that are due to JLP Flash forks. If the reference file system
            // is from a host PC, then this means to remove all differences that would appear as fork removals and instead keep the GKNs
            // unchanged from the values in the other file system.
            otherFileSystem.GetSaveDataForks();

            return differences;
        }

#endif // NOT IMPLEMENTED

        /// <summary>
        /// Use this method to force a subsequent file system comparison to delete the saved menu position fork.
        /// </summary>
        /// <param name="hostFileSystem">The host file system.</param>
        /// <param name="deviceFileSystem">The device file system.</param>
        public static void ForceRemovalOfMenuPositionData(this FileSystem hostFileSystem, FileSystem deviceFileSystem)
        {
            // Menu position data is stored in the manual fork of the root file.
            var rootFileOnDevice = deviceFileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
            var menuPositionForkOnDevice = rootFileOnDevice.Manual;
            if ((menuPositionForkOnDevice != null) && (menuPositionForkOnDevice.Uid == Fork.MenuPositionForkUid))
            {
                // Ensure that the host file system has no save menu data fork. It shouldn't, but let's be sure.
                hostFileSystem.RemoveMenuPositionData();
            }
        }

        /// <summary>
        /// Removes the menu position data fork from the file system.
        /// </summary>
        /// <param name="fileSystem">The file system from which to remove the menu position data fork.</param>
        /// <returns>The previous menu position data, or <c>null</c> if none was present.</returns>
        public static Fork RemoveMenuPositionData(this FileSystem fileSystem)
        {
            // Menu position data is stored in the manual fork of the root file.
            var rootFile = fileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
            var menuPositionFork = rootFile.Manual;
            if ((menuPositionFork != null) && (menuPositionFork.Uid == Fork.MenuPositionForkUid))
            {
                rootFile.Manual = null;
                fileSystem.Forks.LogActivity("Removing menu position fork : " + menuPositionFork);
                fileSystem.Forks.Remove(menuPositionFork);
            }
            return menuPositionFork;
        }

        /// <summary>
        /// Suppresses the root file name differences.
        /// </summary>
        /// <param name="fileSystem">The file system whose root long and short file names are to be stomped.</param>
        /// <param name="device">The device whose names are to be adopted.</param>
        /// <remarks>This is used to suppress "false positive" differences that arise through the series of events that
        /// cause a MenuLayout to store a specific device name / owner. If you sync from device to the host computer,
        /// the MenuLayout will store the long and short names of the root file. If you later connect another device
        /// with a different name / owner, the differences are reported as changes. However, they should be ignored.</remarks>
        public static void SuppressRootFileNameDifferences(this FileSystem fileSystem, Device device)
        {
            var rootFile = fileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
            rootFile.LongName = device.Owner;
            rootFile.ShortName = device.CustomName;
        }

        /// <summary>
        /// Sets the menu position data in the given file system.
        /// </summary>
        /// <param name="fileSystem">The file system upon which menu position data is to be set.</param>
        /// <param name="saveMenuPositionData">The save menu position data fork.</param>
        public static void SetMenuPositionData(this FileSystem fileSystem, Fork saveMenuPositionData)
        {
            fileSystem.Forks.Add(saveMenuPositionData);
            var rootFile = fileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
            rootFile.Manual = saveMenuPositionData;
        }

        /// <summary>
        /// This function will adjust a host file system instance to contain JLP Flash 'pseudo forks' so it can correctly account for
        /// them and avoid replacing or removing them during a file sync operation. It also removes any SaveData forks on the local
        /// file system that don't have a file associated with them.
        /// </summary>
        /// <param name="hostFileSystem">A file system on a host PC, which is intended to be updated to account for JLP Flash forks.</param>
        /// <param name="deviceFileSystem">A file system from a Locutus device, intended as a source for JLP Flash forks.</param>
        /// <returns><c>true</c> if any changes were made to the host file system.</returns>
        /// <remarks>It is incumbent upon the caller to update any persistent representations of the file systems in question to ensure
        /// the changes are correctly preserved. If any entries were added or relocated, this function will return <c>true</c>.</remarks>
        public static bool PopulateSaveDataForksFromDevice(this FileSystem hostFileSystem, FileSystem deviceFileSystem)
        {
            System.Diagnostics.Debug.Assert(hostFileSystem.Origin == FileSystemOrigin.HostComputer, "PopulateSaveDataForksFromDevice: hostFileSystem argument must be a host PC file system.");
            System.Diagnostics.Debug.Assert(deviceFileSystem.Origin == FileSystemOrigin.LtoFlash, "PopulateSaveDataForksFromDevice: deviceFileSystem argument must be a device file system.");

            // Do a simple sanity check -- will accommodating the JLP flash forks cause too many forks to be needed?
            // First, just get all the forks in use.
            var numForksInUseOnHost = hostFileSystem.Forks.ItemsInUse;
            var jlpFlashForksOnHost = Enumerable.Empty<Fork>();
            var jlpFlashForksToDeleteOnHost = Enumerable.Empty<Fork>();
            var jlpFlashForksToExclude = Enumerable.Empty<Fork>();
            var jlpFlashForksToAddToHost = Enumerable.Empty<Fork>();
            var jlpFlashForksToKeepOnHost = Enumerable.Empty<Fork>();
            var numJlpFlashForksOnHost = 0;

            // In the worst case, every JLP Flash fork will need to be added, so do the easiest check first.
            var jlpFlashForksOnDevice = deviceFileSystem.GetSaveDataForks();
            var numJlpFlashForksOnDevice = jlpFlashForksOnDevice.Count();

            var haveEnoughRoom = numForksInUseOnHost + numJlpFlashForksOnDevice <= FileSystemConstants.GlobalForkTableSize;

            if (!haveEnoughRoom)
            {
                // We need to do a more detailed check. Check to see if we already have JLP Flash forks on the host. If we do,
                // then we'll only need enough room for the 'delta' in the number of JLP Flash! forks -- we don't really care
                // about the specific contents of them at this point.
                jlpFlashForksOnHost = hostFileSystem.GetSaveDataForks();
                numJlpFlashForksOnHost = jlpFlashForksOnHost.Count();
                haveEnoughRoom = numJlpFlashForksOnHost >= numJlpFlashForksOnDevice;
            }

            if (!haveEnoughRoom)
            {
                // All hope is not yet lost. We can check to see if we can get rid of some JLP Flash forks on the host.
                jlpFlashForksToDeleteOnHost = jlpFlashForksOnHost.Where(k => !jlpFlashForksOnDevice.Any(d => d.GlobalForkNumber == k.GlobalForkNumber));
                jlpFlashForksToKeepOnHost = jlpFlashForksOnHost.Where(k => jlpFlashForksOnDevice.Any(d => d.GlobalForkNumber == k.GlobalForkNumber));
                jlpFlashForksToAddToHost = jlpFlashForksOnDevice.Where(d => !jlpFlashForksToKeepOnHost.Any(k => k.GlobalForkNumber == d.GlobalForkNumber));
                haveEnoughRoom = numForksInUseOnHost - jlpFlashForksToDeleteOnHost.Count() + jlpFlashForksToAddToHost.Count() <= FileSystemConstants.GlobalForkTableSize;
            }

            if (!haveEnoughRoom)
            {
                // We simply don't have room for the JLP Flash forks. We don't want to remove any from the device,
                // so this means that we're going to have to tell the user about the problem.
                throw new System.IO.IOException(Resources.Strings.FileSystem_OutOfForksError);
            }

            var modified = false;

            // Now, we need to put forks into our GKT in the appropriate locations. Fortunately, this is pretty simple. The host
            // file system refers to forks by object instance, so all we need to do if there is a collision is rearrange the
            // entries in the GKT and update the GKNs of the actual Fork instances.
            if (!jlpFlashForksToDeleteOnHost.Any())
            {
                jlpFlashForksToDeleteOnHost = jlpFlashForksOnHost.Except(jlpFlashForksOnDevice);
            }
            if (!jlpFlashForksToKeepOnHost.Any())
            {
                jlpFlashForksToKeepOnHost = jlpFlashForksOnHost.Intersect(jlpFlashForksOnDevice);
            }
            if (!jlpFlashForksToAddToHost.Any())
            {
                jlpFlashForksToAddToHost = jlpFlashForksOnDevice.Except(jlpFlashForksToKeepOnHost);
            }

#if IGNORE_EMPTY_FORK_PATHS
            jlpFlashForksToExclude = hostFileSystem.GetSaveDataForks().Where(k => string.IsNullOrWhiteSpace(k.FilePath)).Distinct();
#endif // IGNORE_EMPTY_FORK_PATHS

            // Now that we have JLP Flash forks to remove and add, let's do that. Do the removes first.
            foreach (var fork in jlpFlashForksToDeleteOnHost.Concat(jlpFlashForksToExclude))
            {
                modified |= true;
                var filesUsingFork = hostFileSystem.Files.Where(f => (f != null) && (f.JlpFlash == fork));
                foreach (var file in filesUsingFork)
                {
                    file.JlpFlash = null;
                }
                hostFileSystem.Forks.LogActivity("Removing fork from file system in PopulateSaveDataForks: " + ((fork == null) ? "<null>" : fork.ToString()));
                hostFileSystem.Forks.Remove(fork); // NOTE: Triggers OnCollectionChanged. :/
            }

            // Now apply modifications to forks. The forks are from the host file system, so copy content from device file system instance.
            // FIXME : GetFilesUsingForks returns only the *FIRST* file found using a given fork! If the same fork
            // is referred to by multiple files, it's quite possible the following code does not do what's intended!
            var filesAndForksOfInterest = deviceFileSystem.GetFilesUsingForks(jlpFlashForksToKeepOnHost);
            foreach (var forkAndFile in filesAndForksOfInterest)
            {
                modified |= true;
                var fork = forkAndFile.Key;
                var fileOnDevice = forkAndFile.Value;
                var fileOnHost = hostFileSystem.Files[fileOnDevice.GlobalFileNumber];
                if ((fileOnHost != null) && (fileOnHost.FileType == FileType.Executable) && (fileOnHost.Rom.Crc24 == fileOnDevice.Rom.Crc24))
                {
                    var deviceFork = deviceFileSystem.Forks[fork.GlobalForkNumber];
                    fork.Size = deviceFork.Size;
                    fork.Crc24 = deviceFork.Crc24;
                    fork.FilePath = null; // we don't keep any file paths
                    // fork.StartingVirtualBlock = deviceFork.StartingVirtualBlock; // don't care about starting vblock
                }
            }

            // Finally, add the new ones. If this causes lots of fork relocations, it'll suck.
            // FIXME : GetFilesUsingForks returns only the *FIRST* file found using a given fork! If the same fork
            // is referred to by multiple files, it's quite possible the following code does not do what's intended!
            filesAndForksOfInterest = deviceFileSystem.GetFilesUsingForks(jlpFlashForksToAddToHost);
            ////var unusedJlpFlashForks = new Dictionary<Fork, ILfsFileInfo>();
            foreach (var forkAndFile in filesAndForksOfInterest)
            {
                modified |= true;
                var fork = forkAndFile.Key;
                var fileOnDevice = forkAndFile.Value;
                var fileOnHost = hostFileSystem.Files[fileOnDevice.GlobalFileNumber];
                if ((fileOnHost != null) && (fileOnHost.FileType == fileOnDevice.FileType) && ((fileOnDevice.FileType == FileType.Folder) || (((fileOnHost.Rom != null) && (fileOnDevice.Rom != null)) && (fileOnHost.Rom.Crc24 == fileOnDevice.Rom.Crc24))))
                {
                    var newDataFork = new Fork()
                        {
                            FileSystem = hostFileSystem,
                            GlobalForkNumber = fork.GlobalForkNumber,
                            Size = fork.Size,
                            Crc24 = fork.Crc24
                        };
                    hostFileSystem.Forks.AddAndRelocate(newDataFork);
                    fileOnHost.JlpFlash = newDataFork;
                }
            }

            return modified;
        }

        private static bool IgnoreRootFork(Fork fork)
        {
            bool ignore = false;
            if (fork.GlobalForkNumber != GlobalForkTable.InvalidForkNumber)
            {
                var rootFile = fork.FileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
                ignore = (rootFile.Manual != null) && ((rootFile.Manual.GlobalForkNumber == fork.GlobalForkNumber) || (rootFile.JlpFlash.GlobalForkNumber == fork.GlobalForkNumber));
            }
            return ignore;
        }

        /// <summary>
        /// Do a fast and simple comparison between two file systems. This will NOT validate the integrity of the files on the local system!
        /// </summary>
        /// <param name="referenceFileSystem">The reference file system.</param>
        /// <param name="otherFileSystem">The file system to compare against the reference file system.</param>
        /// <param name="targetDevice">The device corresponding to <paramref name="otherFileSystem"/> used to validate entries from <paramref name="referenceFileSystem"/>.</param>
        /// <returns>If the two file systems should be considered equivalent, returns 0. A nonzero return value indicates the two file systems are different in some way.</returns>
        /// <remarks>This comparison may not be completely accurate. For example, if in-memory data differs from what is current in the file system,
        /// results will be inaccurate in some circumstances. Consider a case in which a device file system was populated with a .bin format ROM, but locally, the
        /// .cfg file of that ROM has changed after the application was started. This comparison will NOT detect such a change. The fully-featured <see cref="FileSystemHelpers.CompareTo"/>
        /// method will detect such a change. However, that comparison method can be orders of magnitude slower, due to necessary access to the file system.</remarks>
        public static int SimpleCompare(this FileSystem referenceFileSystem, FileSystem otherFileSystem, Device targetDevice)
        {
            var difference = object.ReferenceEquals(referenceFileSystem, otherFileSystem) ? 0 : -2;
            if (difference != 0)
            {
                if (referenceFileSystem == null)
                {
                    return -1;
                }
                else if (otherFileSystem == null)
                {
                    return 1;
                }
#if REPORT_PERFORMANCE
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                Logger.Log("FileSystem.SimpleCompare() BEGIN ---------------------------------");
#endif // REPORT_PERFORMANCE
                var referenceFileSystemWasFrozen = referenceFileSystem.Frozen;
                var otherFileSystemWasFrozen = otherFileSystem.Frozen;
                try
                {
                    referenceFileSystem.Frozen = true;
                    otherFileSystem.Frozen = true;
                    var gdtDescriptor = new GatherDifferencesDescriptor<IDirectory>(LfsEntityType.Directory, otherFileSystem.Directories, FileSystemHelpers.CompareIDirectories);
#if USE_SPECIALIZED_SIMPLE_COMPARE
                    difference = referenceFileSystem.Directories.SimpleCompare(gdtDescriptor, targetDevice);
#else
                    difference = referenceFileSystem.Directories.GatherDifferences(gdtDescriptor, targetDevice, false, GatherExitOnFirst<IDirectory>).Any() ? 1 : 0;
#endif // USE_SPECIALIZED_SIMPLE_COMPARE
#if REPORT_PERFORMANCE
                    Logger.Log("FileSystem.SimpleCompare(): GDN --> diff: " + difference + " : ELAPSED: " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE

                    if (difference == 0)
                    {
                        var gftDescriptor = new GatherDifferencesDescriptor<ILfsFileInfo>(LfsEntityType.File, otherFileSystem.Files, FileSystemHelpers.CompareILfsFileInfo, ValidateFile);
#if USE_SPECIALIZED_SIMPLE_COMPARE
                        difference = referenceFileSystem.Files.SimpleCompare(gftDescriptor, targetDevice);
#else
                        difference = referenceFileSystem.Files.GatherDifferences(gftDescriptor, targetDevice, false, GatherExitOnFirst<ILfsFileInfo>).Any() ? 1 : 0;
#endif // USE_SPECIALIZED_SIMPLE_COMPARE
#if REPORT_PERFORMANCE
                        Logger.Log("FileSystem.SimpleCompare(): GFN --> diff: " + difference + " : ELAPSED: " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE
                    }

                    if (difference == 0)
                    {
                        var gktDescriptor = new GatherDifferencesDescriptor<Fork>(LfsEntityType.Fork, otherFileSystem.Forks, FileSystemHelpers.CompareForks, ValidateFork, IgnoreRootFork);
#if USE_SPECIALIZED_SIMPLE_COMPARE
                        difference = referenceFileSystem.Forks.SimpleCompare(gktDescriptor, targetDevice);
#else
                        difference = referenceFileSystem.Forks.GatherDifferences(gktDescriptor, targetDevice, false, GatherExitOnFirst<Fork>).Any() ? 1 : 0;
#endif // USE_SPECIALIZED_SIMPLE_COMPARE
#if REPORT_PERFORMANCE
                        Logger.Log("FileSystem.SimpleCompare(): GKN -->  diff: " + difference + " : ELAPSED: " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE
                    }

                    return difference;
                }
                finally
                {
                    otherFileSystem.Frozen = otherFileSystemWasFrozen;
                    referenceFileSystem.Frozen = referenceFileSystemWasFrozen;
#if REPORT_PERFORMANCE
                    stopwatch.Stop();
                    System.Diagnostics.Debug.WriteLine("FileSystem.SimpleCompare() took: + " + stopwatch.Elapsed.ToString());
                    Logger.Log("FileSystem.SimpleCompare() FINISH ---------------------------------: diff: " + difference + " : " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE
                }
            }
            return difference;
        }

        /// <summary>
        /// Compares two Locutus File Systems.
        /// </summary>
        /// <param name="referenceFileSystem">The reference file system.</param>
        /// <param name="otherFileSystem">The file system to compare against the reference file system.</param>
        /// <returns>The differences between the two file systems.</returns>
        public static LfsDifferences CompareTo(this FileSystem referenceFileSystem, FileSystem otherFileSystem)
        {
            return referenceFileSystem.CompareTo(otherFileSystem, null, true);
        }

        /// <summary>
        /// Compares two Locutus File Systems with additional device validation.
        /// </summary>
        /// <param name="referenceFileSystem">The reference file system.</param>
        /// <param name="otherFileSystem">The file system to compare against the reference file system.</param>
        /// <param name="targetDevice">The device corresponding to <paramref name="otherFileSystem"/> used to validate entries from <paramref name="referenceFileSystem"/>.</param>
        /// <param name="refresh">If <c>true</c>, force a refresh of the file system entries before comparing.</param>
        /// <returns>The differences between the two file systems.</returns>
        /// <remarks>Entries from <paramref name="referenceFileSystem"/> that are part of update or add operations, but which fail validation checks against
        /// <paramref name="targetDevice"/> will be reported as errors in the difference result.</remarks>
        public static LfsDifferences CompareTo(this FileSystem referenceFileSystem, FileSystem otherFileSystem, Device targetDevice, bool refresh)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Log("FileSystem.CompareTo() BEGIN ---------------------------------");
#endif // REPORT_PERFORMANCE
            var referenceFileSystemWasFrozen = referenceFileSystem.Frozen;
            var otherFileSystemWasFrozen = otherFileSystem.Frozen;
            try
            {
#if false
                // Consider freezing the file systems based on the 'refresh' option.
                referenceFileSystem.Frozen = !refresh;
                otherFileSystem.Frozen = !refresh;
#endif
                var gdtDescriptor = new GatherDifferencesDescriptor<IDirectory>(LfsEntityType.Directory, otherFileSystem.Directories, FileSystemHelpers.CompareIDirectories);
                var gdtDiff = referenceFileSystem.Directories.GatherDifferences(gdtDescriptor, targetDevice, refresh, GatherExitNever<IDirectory>);
#if REPORT_PERFORMANCE
                Logger.Log("FileSystem.CompareTo() ELAPSED: GDN --> " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE

                var gftDescriptor = new GatherDifferencesDescriptor<ILfsFileInfo>(LfsEntityType.File, otherFileSystem.Files, FileSystemHelpers.CompareILfsFileInfo, ValidateFile);
                var gftDiff = referenceFileSystem.Files.GatherDifferences(gftDescriptor, targetDevice, refresh, GatherExitNever<ILfsFileInfo>);
#if REPORT_PERFORMANCE
                Logger.Log("FileSystem.CompareTo() ELAPSED: GFN --> " + stopwatch.Elapsed.ToString() + "\nFileSystem.CompareTo() START: GKN ...");
                IRomHelpers.ResetAccumulatedTimes();
                Fork.ResetAccumulatedTimes();
                _accumulatedForkValidation = TimeSpan.Zero;
                _accumulatedCanExecuteOnDevice = TimeSpan.Zero;
                _accumulatedForkCompare = TimeSpan.Zero;
                _accumulatedFilesUsingForks = TimeSpan.Zero;
                IRomHelpers.ResetPrepareForDeploymentVisits();
                var forkwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE

                var gktDescriptor = new GatherDifferencesDescriptor<Fork>(LfsEntityType.Fork, otherFileSystem.Forks, FileSystemHelpers.CompareForks, ValidateFork);
                var gktDiff = referenceFileSystem.Forks.GatherDifferences(gktDescriptor, targetDevice, refresh, GatherExitNever<Fork>);
#if REPORT_PERFORMANCE
                forkwatch.Stop();
                Logger.Log("FileSystem.CompareTo() FINISH: GKN --> " + forkwatch.Elapsed.ToString() + "\nFileSystem.CompareTo() ELAPSED: GKN --> " + stopwatch.Elapsed.ToString());
#endif // REPORT_PERFORMANCE

                var allDifferences = new LfsDifferences(gdtDiff, gftDiff, gktDiff);
                return allDifferences;
            }
            finally
            {
                otherFileSystem.Frozen = otherFileSystemWasFrozen;
                referenceFileSystem.Frozen = referenceFileSystemWasFrozen;
#if REPORT_PERFORMANCE
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.CompareTo() took: " + stopwatch.Elapsed.ToString());
                Logger.Log("FileSystem.CompareTo() FINISH ---------------------------------: " + stopwatch.Elapsed.ToString());
                IRomHelpers.ReportAccumulatedTimes(Logger, "FileSystem.CompareTo()");
                Logger.Log("FileSystem.CompareTo() Total ValidateFork ---------------------: " + _accumulatedForkValidation.ToString());
                Fork.ReportAccumulatedTimes(Logger, "FileSystem.CompareTo()");
                Logger.Log("FileSystem.CompareTo() Total CanExecute -----------------------: " + _accumulatedCanExecuteOnDevice.ToString());
                Logger.Log("FileSystem.CompareTo() Total FilesUsingForks ------------------: " + _accumulatedFilesUsingForks.ToString());
                Logger.Log("FileSystem.CompareTo() Total CompareFork ----------------------: " + _accumulatedForkCompare.ToString());
                IRomHelpers.ReportPrepareForDeploymentVisits(Logger);
#endif // REPORT_PERFORMANCE
            }
        }

        /// <summary>
        /// Removes entries from / repairs entries in <paramref name="referenceFileSystem"/> that are identified as causing errors in the
        /// provided <paramref name="differences"/>. Removal is subject to the predicate supplied via <paramref name="shouldRemoveInvalidEntry"/>.
        /// A repair is used to resolve problems in cases of a missing 'secondary' fork, such as a manual, in which case the current
        /// data on the target file system is retained. Missing "modified" forks may also be "repaired". The idea here is that the modifications
        /// are a precursor to re-comparing the two file systems.
        /// </summary>
        /// <param name="referenceFileSystem">The file system whose error-prone entries are to be removed or repaired. This should be the reference file system used in a call to <see cref="CompareTo"/>.</param>
        /// <param name="targetFileSystem">The target file system from the comparison that produced the differences. Used to repair unwanted changes.</param>
        /// <param name="differences">A difference computed between two <see cref="FileSystem"/> instances.</param>
        /// <param name="shouldRemoveInvalidEntry">If <c>null</c>, all entries reporting an error in <paramref name="differences"/> are removed; otherwise only errors that pass the provided predicate are removed from <paramref name="fileSystem"/>.</param>
        /// <param name="errorFilter">If <c>null</c>, all errors reported in <paramref name="differences"/> are included; otherwise only errors that pass the provided filter are included in the return value.</param>
        /// <returns>A dictionary containing all errors satisfy the provided <paramref name="errorFilter"/>.</returns>
        /// <remarks>Note that <paramref name="errorFilter"/> has no effect on which error-inducing entries are purged from <paramref name="fileSystem"/>! It only
        /// filters the results included in the return value. Similarly, the predicate <paramref name="shouldRemoveInvalidEntry"/> only determines which entries should
        /// be removed from <paramref name="referenceFileSystem"/>.
        /// It is also assumed that modifications to <paramref name="referenceFileSystem"/> are safe to do. BE CAUTIOUS in this regard -- you
        /// likely do not want to pass in a file system that is being edited by the user here!</remarks>
        public static IDictionary<string, FailedOperationException> CleanUpInvalidEntries(this FileSystem referenceFileSystem, FileSystem targetFileSystem, LfsDifferences differences, Predicate<FailedOperationException> shouldRemoveInvalidEntry, Predicate<Exception> errorFilter)
        {
            // We should never have a failed directory comparison, really...
            foreach (var failedDirectoryComparison in differences.DirectoryDifferences.FailedOperations.Where(f => (shouldRemoveInvalidEntry == null) || shouldRemoveInvalidEntry(f.Value)))
            {
                // Use RemoveChild() instead of RemoveAt() based on index. RemoveChild() ensures file system consistency across the GDT, GFT, and GKT. RemoveAt() does not.
                var entry = referenceFileSystem.Directories[(int)failedDirectoryComparison.Value.GlobalFileSystemNumber] as Folder;
                if (entry != null)
                {
                    referenceFileSystem.Directories.LogActivity("Failed directory comparison!? " + failedDirectoryComparison.ToString());
                    var parent = entry.Parent;
                    parent.RemoveChild(entry, true);
                }
            }
            foreach (var failedFileComparison in differences.FileDifferences.FailedOperations.Where(f => (shouldRemoveInvalidEntry == null) || shouldRemoveInvalidEntry(f.Value)))
            {
                // Use RemoveChild() instead of RemoveAt() based on index. RemoveChild() ensures file system consistency across the GDT, GFT, and GKT. RemoveAt() does not.
                var entry = referenceFileSystem.Files[(int)failedFileComparison.Value.GlobalFileSystemNumber] as FileNode;
                if (entry != null)
                {
                    referenceFileSystem.Files.LogActivity("Failed file comparison: " + failedFileComparison.ToString() + ", failed compare message: " + failedFileComparison.Value.Message);
                    var parent = entry.Parent;
                    parent.RemoveChild(entry, true);
                }
            }
            foreach (var failedForkComparison in differences.ForkDifferences.FailedOperations.Where(f => ShouldCleanUpBadFork(referenceFileSystem, f.Value, differences, shouldRemoveInvalidEntry)))
            {
                // There is no analog for RemoveChild() here, because a Fork does not have a "parent".
                // BUG is possible here because of this. Specifically, if there is a bug in the differences generator, such that we'd leave
                // files in the system that refer to a fork, but for some reason decide to remove the fork anyway, we'll make a bad menu layout.
                // I.e. the entry will appear on Locutus, but crash when you try to run it.
                // Let's hope that doesn't happen.
                // In fact, this loop should never run! That's because if we're going to delete a Fork, *all* files referring to that fork must
                // have been deleted previously. And, because we used RemoveChild(), when the last FileNode referring to a specific Fork is deleted,
                // the Fork should be removed automatically.
                // Worst case, using the UI to delete / re-add offenders or clean up things *should* work. This potential bug has not
                // yet been encountered. Let's assume that the difference generator doesn't contain the bug that would cause this, OK? :D
                // This code is expecting the supplied 'shouldRemoveInvalidEntry' predicate to protect from cases in which a fork does cause an error
                // to manifest (e.g. it's source ROM file is missing), but we don't want to remove it.
                // ---- UPDATE ----
                // A missing fork, such as a manual, can arise, in which case we run into the case of a file that has no changes,
                // a case in which a fork "update" thinks it should occur. The "update" operation results because entries for the fork
                // exist in both the source and destination GKTs - but we have a "failed operation" because of the missing local file. The
                // desired outcome here is that the existing fork on the device remains UNCHANGED. There are at least two ways we
                // could deal with this:
                // 1) Pretend there is no difference at all, either by tweaking fork comparison, or by tweaking the difference generator
                // 2) Change the "decomposition" in the file system synchronization operation to avoid deleting the fork on the target.
                // For the time being, the chosen resolution is option (2). In the DeviceHelpers.SyncHostToDevice code, the fork deletion has
                // been modified to avoid deleting forks that also have missing fork errors in the error list.
                // There may STILL be another case not covered. Namely a FILE DELETION which should result in a FORK DELETION, in which
                // the fork is missing. HOWEVER, actual DELETE operations do not filter for the missing fork error - so it SHOULD be OK.
                if (IsMissingForkError(failedForkComparison.Value))
                {
                    // For missing ROM forks, just remove any files that are in the 'to add' list by removing the entire
                    // file from the reference file system.
                    var filesToRemove = differences.FileDifferences.ToAdd.Where(f => (f.Rom != null) && (f.Rom.GlobalForkNumber == failedForkComparison.Value.GlobalFileSystemNumber));
                    foreach (var fileToRemove in filesToRemove)
                    {
                        var entry = referenceFileSystem.Files[fileToRemove.GlobalFileNumber] as FileNode;
                        if (entry != null)
                        {
                            referenceFileSystem.Forks.LogActivity("Missing Fork Error; file to remove: " + fileToRemove.ToString());
                            var parent = entry.Parent;
                            parent.RemoveChild(entry, true);
                        }
                    }

                    // UPDATE 2: There *ARE* more cases. A file *add* for example, in which the ROM is fine, but another fork, such as
                    // a manual, is not OK. For these ancillary forks, we want to be sure to leave the data UNCHANGED. Also, any fork
                    // that would appear as an UPDATE to an existing file should be ignored as well.
                    var missingFork = referenceFileSystem.Forks[(int)failedForkComparison.Value.GlobalFileSystemNumber];
                    var missingForkKind = referenceFileSystem.GetForkKind(missingFork);
                    if (missingFork == null)
                    {
                        referenceFileSystem.Forks.LogActivity("Missing fork is null - GKN: " + failedForkComparison.Value.GlobalFileSystemNumber);
                    }

                    var filesReferringToMissingFork = referenceFileSystem.GetAllFilesUsingForks(new[] { missingFork });
                    foreach (var fileReferringToMissingFork in filesReferringToMissingFork)
                    {
                        System.Console.WriteLine(fileReferringToMissingFork);
                        foreach (var file in fileReferringToMissingFork.Value)
                        {
                            var targetFileSystemFile = targetFileSystem.Files[file.GlobalFileNumber];

                            // Ensure the file's fork numbers match up. If the fork is "to add" and the
                            // existing file on target doesn't already point to it, then tweak the source
                            // file system to also not refer to the fork. We're counting on not hitting the
                            // bizarre and self-contradictory case of a fork that's "missing" also being in the
                            // "ToDelete" list. That would just be too weird. If it does happen, then there's
                            // another bug to be fixed.
                            var localGkn = file.ForkNumbers[(int)missingForkKind];
                            var targetGkn = (targetFileSystemFile == null) ? GlobalForkTable.InvalidForkNumber : targetFileSystemFile.ForkNumbers[(int)missingForkKind];
                            if (localGkn != targetGkn)
                            {
                                // Put the target GKN into the local file. This should suppress the difference
                                // between the *files* when the diff with "cleaned up" file systems runs.
                                file.ForkNumbers[(int)missingForkKind] = targetGkn;
                                if (targetGkn == GlobalForkTable.InvalidForkNumber)
                                {
                                    missingFork = null;
                                }
                                switch (missingForkKind)
                                {
                                    case ForkKind.Program:
                                        file.Rom = missingFork;
                                        break;
                                    case ForkKind.JlpFlash:
                                        // file.JlpFlash = missingFork; // Should we ever do this???
                                        break;
                                    case ForkKind.Manual:
                                        file.Manual = missingFork;
                                        break;
                                    case ForkKind.Vignette:
                                        file.Vignette = missingFork;
                                        break;
                                    case ForkKind.Reserved4:
                                        file.ReservedFork4 = missingFork;
                                        break;
                                    case ForkKind.Reserved5:
                                        file.ReservedFork5 = missingFork;
                                        break;
                                    case ForkKind.Reserved6:
                                        file.ReservedFork6 = missingFork;
                                        break;
                                    default:
                                        // Should we throw an error here?
                                        break;
                                }
                            }
                            else
                            {
                                // The local and target GKNs are the same -- so it's likely we have the problem of
                                // trying to update a fork that should not be getting updated. This is a little trickier,
                                // because forks are *really* picky about ensuring the source file is present.
                                var targetFileSystemFork = targetFileSystem.Forks[targetGkn];
                                if (targetFileSystemFork != null)
                                {
                                    if (missingFork != null)
                                    {
                                        missingFork.Crc24 = targetFileSystemFork.Crc24;
                                        missingFork.Size = targetFileSystemFork.Size;
                                    }
                                }
                                else
                                {
                                    // Target file system is already kinda messed up... so, null out the local file system's
                                    // fork entry at the same location.
                                    referenceFileSystem.Forks[localGkn] = null;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (failedForkComparison.Value is IncompatibleRomException)
                    {
                        // In a 'Get menu from device' operation, these can arise, so purge file additions that don't belong.
                        var filesToRemove = differences.FileDifferences.ToAdd.Where(f => (f.Rom != null) && (f.Rom.GlobalForkNumber == failedForkComparison.Value.GlobalFileSystemNumber));
                        foreach (var fileToRemove in filesToRemove)
                        {
                            var entry = referenceFileSystem.Files[fileToRemove.GlobalFileNumber] as FileNode;
                            if (entry != null)
                            {
                                var parent = entry.Parent;
                                parent.RemoveChild(entry, true);
                            }
                        }
                        referenceFileSystem.Forks.RemoveAt((int)failedForkComparison.Value.GlobalFileSystemNumber);
                    }
                    else
                    {
                        // TODO What? Complain somehow, I suppose...
#if DEBUG
                        System.Diagnostics.Debug.Assert(referenceFileSystem.Forks[(int)failedForkComparison.Value.GlobalFileSystemNumber] == null, "File system inconsistency! Why are we deleting this fork if files still refer to it?");
#endif
                    }
                }
            }
            return differences.GetAllFailures(errorFilter);
        }

        /// <summary>
        /// Function used to determine whether an error-causing entry should be removed.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        /// <returns><c>true</c> if the entry should be removed, <c>false</c> otherwise.</returns>
        internal static bool ShouldRemoveInvalidEntry(FailedOperationException error)
        {
            // FileNotFound errors for forks usually arise because of lost files. We want to report these problems, but avoid
            // removing the fork from the file table if the user hasn't removed the files referring to it.
            var isMissingFork = IsMissingForkError(error);
            return !isMissingFork;
        }

        /// <summary>
        /// Determines if the error is due to a missing fork.
        /// </summary>
        /// <param name="error">The error to check.</param>
        /// <returns><c>true</c> if the error describes a missing fork; otherwise, <c>false</c>.</returns>
        internal static bool IsMissingForkError(FailedOperationException error)
        {
            var isMissingFork = (error.EntityType == LfsEntityType.Fork) && (error.InnerException != null) && (error.InnerException is System.IO.FileNotFoundException);
            return isMissingFork;
        }

        private static bool ShouldCleanUpBadFork(FileSystem fileSystem, FailedOperationException error, LfsDifferences differences, Predicate<FailedOperationException> customBadForkFilter)
        {
            var shouldCleanUp = (customBadForkFilter == null) || customBadForkFilter(error);
            if (!shouldCleanUp)
            {
                if (IsMissingForkError(error))
                {
                    // We want to prevent adding files that refer to a fork that cannot be found.
                    shouldCleanUp = differences.FileDifferences.ToAdd.Any(f => f.ForkNumbers.Contains((ushort)error.GlobalFileSystemNumber));
                    if (!shouldCleanUp)
                    {
                        // If any other files remain that refer to the now-missing fork, we should clean it up if we can.
                        var missingFork = fileSystem.Forks[(int)error.GlobalFileSystemNumber];
                        var filesUsingMissingFork = fileSystem.GetAllFilesUsingForks(new[] { missingFork });
                        shouldCleanUp = filesUsingMissingFork.Any();
                    }
                }
            }
            return shouldCleanUp;
        }

#if USE_SPECIALIZED_SIMPLE_COMPARE

        private static int SimpleCompare<T>(this FixedSizeCollection<T> sourceFileSystemTable, GatherDifferencesDescriptor<T> descriptor, Device targetDevice) where T : class, IGlobalFileSystemEntry
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
                var deviceId = targetDevice == null ? null : targetDevice.UniqueId;
                var result = 0;
                for (int i = 0; (result == 0) && (i < sourceFileSystemTable.Size); ++i)
                {
                    var sourceEntry = sourceFileSystemTable[i];
                    var targetEntry = descriptor.TargetFileSystemTable[i];
                    if ((sourceEntry == null) && (targetEntry != null))
                    {
                        if (!descriptor.Ignore(targetEntry))
                        {
                            --result; // An entry is not in the source file system, but is present in the target.
                        }
                    }
                    else if ((sourceEntry != null) && (targetEntry == null))
                    {
                        if (!descriptor.Ignore(sourceEntry))
                        {
                            ++result; // An entry is in the source file system, but is not in the target.
                        }
                    }
                    else if ((sourceEntry != null) && (targetEntry != null))
                    {
                        if (!descriptor.Ignore(sourceEntry) && !descriptor.Ignore(targetEntry))
                        {
                            string failedToUpdate;
                            Exception updateError;
                            /*
                            if (!descriptor.Validate(sourceEntry, targetDevice, false, out failedToUpdate, out updateError))
                            {
                                // entry is not compatible with the given device -- should this count as a "difference"? : NO - ignored during transfer operations anyway
                            }
                             */
                            if (!descriptor.Compare(sourceEntry, targetEntry, false, out failedToUpdate, out updateError))
                            {
                                // Entries are different;
                                ++result;
                            }
                            /*
                            if (updateError != null)
                            {
                                // an error occurred during the update -- should this count as a difference? : NO - informational only
                            }
                             */
                        }
                    }
                }
                return result;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.GatherDifferences<" + typeof(T) + ">() took: + " + stopwatch.Elapsed.ToString());
            }
#endif // REPORT_PERFORMANCE
        }

#else

        private static bool GatherExitOnFirst<T>(FileSystemDifferences<T> differences) where T : class, IGlobalFileSystemEntry
        {
            bool shouldStop = differences.Any();
            return shouldStop;
        }

#endif // USE_SPECIALIZED_SIMPLE_COMPARE

        private static bool GatherExitNever<T>(FileSystemDifferences<T> differences) where T : class, IGlobalFileSystemEntry
        {
            return false;
        }

        /// <summary>
        /// Gather up the differences between a reference FileSystem table and a target FileSystem to be brought into agreement.
        /// </summary>
        /// <typeparam name="T">The data type of the entries in the file system table of interest.</typeparam>
        /// <param name="sourceFileSystemTable">The global file system table from the host PC.</param>
        /// <param name="descriptor">The target file system and related information for producing the comparison result.</param>
        /// <param name="targetDevice">The device corresponding to <paramref name="otherFileSystem"/> used to validate entries from <paramref name="referenceFileSystem"/>.</param>
        /// <param name="forceUpdate">If true, refresh data before validating or comparing.</param>
        /// <param name="shouldStop">Predicate to call to see if the comparison loop should exit early.</param>
        /// <returns>The accumulated differences between the host file system table and corresponding Locutus file system table.</returns>
        private static FileSystemDifferences<T> GatherDifferences<T>(this FixedSizeCollection<T> sourceFileSystemTable, GatherDifferencesDescriptor<T> descriptor, Device targetDevice, bool forceUpdate, Predicate<FileSystemDifferences<T>> shouldStop) where T : class, IGlobalFileSystemEntry
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            var deviceId = targetDevice == null ? null : targetDevice.UniqueId;

            var differences = new FileSystemDifferences<T>();
            for (int i = 0; !shouldStop(differences) && (i < sourceFileSystemTable.Size); ++i)
            {
                var refresh = forceUpdate;
                var sourceEntry = sourceFileSystemTable[i];
                var targetEntry = descriptor.TargetFileSystemTable[i];
                if ((sourceEntry == null) && (targetEntry != null))
                {
                    if (!descriptor.Ignore(targetEntry))
                    {
                        differences.ToDelete.Add((uint)i);
                    }
                }
                else if ((sourceEntry != null) && (targetEntry == null))
                {
                    if (!descriptor.Ignore(sourceEntry))
                    {
                        string failedToUpdate;
                        Exception addError;
                        if (!descriptor.Validate(sourceEntry, targetDevice, forceUpdate, out failedToUpdate, out addError))
                        {
                            differences.FailedOperations[failedToUpdate] = FailedOperationException.WrapIfNeeded(addError, descriptor.EntityType, (uint)i, deviceId);
                        }
                        differences.ToAdd.Add(sourceEntry);
                    }
                }
                else if ((sourceEntry != null) && (targetEntry != null))
                {
                    if (!descriptor.Ignore(sourceEntry) && !descriptor.Ignore(targetEntry))
                    {
                        string failedToUpdate;
                        Exception updateError;
                        if (!descriptor.Validate(sourceEntry, targetDevice, refresh, out failedToUpdate, out updateError))
                        {
                            differences.FailedOperations[failedToUpdate] = FailedOperationException.WrapIfNeeded(updateError, descriptor.EntityType, (uint)i, deviceId);
                        }
                        if (refresh)
                        {
                            // Validate check just did a refresh - no need to force it again -- it's already been done
                            refresh = false;
                        }
                        if (!descriptor.Compare(sourceEntry, targetEntry, refresh, out failedToUpdate, out updateError))
                        {
                            differences.ToUpdate.Add(sourceEntry);
                        }
                        if (updateError != null)
                        {
                            differences.FailedOperations[failedToUpdate] = FailedOperationException.WrapIfNeeded(updateError, descriptor.EntityType, (uint)i, deviceId);
                        }
                    }
                }
            }
            return differences;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.GatherDifferences<" + typeof(T) + ">() took: + " + stopwatch.Elapsed.ToString());
                Logger.Log("FileSystem.GatherDifferences<" + typeof(T) + ">() took: + " + stopwatch.Elapsed.ToString());
            }
#endif // REPORT_PERFORMANCE
        }

        /// <summary>
        /// Creates a ByteSerializer for flattening a file to be sent to a Locutus device.
        /// </summary>
        /// <param name="fileInfo">The file to be sent to Locutus.</param>
        /// <returns>The ByteSerializer version of the object.</returns>
        public static INTV.Core.Utility.ByteSerializer ToFileByteSerializer(this IGlobalFileSystemEntry fileInfo)
        {
            LfsFileInfo serializableFileInfo = fileInfo as LfsFileInfo;
            if (serializableFileInfo == null)
            {
                var file = fileInfo as IFile;
                if (file == null)
                {
                    serializableFileInfo = LfsFileInfo.InvalidFile;
                }
                else
                {
                    serializableFileInfo = new LfsFileInfo(file);
                }
            }
            return serializableFileInfo;
        }

        /// <summary>
        /// Creates a ByteSerializer for flattening a directory to be sent to a Locutus device.
        /// </summary>
        /// <param name="directory">The directory to be sent to Locutus.</param>
        /// <returns>The ByteSerializer version of the object.</returns>
        public static INTV.Core.Utility.ByteSerializer ToDirectoryByteSerializer(this IGlobalFileSystemEntry directory)
        {
            Directory serializableDirectory = directory as Directory;
            if (serializableDirectory == null)
            {
                var folder = directory as IFileContainer;
                if (folder == null)
                {
                    serializableDirectory = Directory.InvalidDirectory;
                }
                else
                {
                    serializableDirectory = new Directory(folder);
                }
            }
            return serializableDirectory;
        }

        /// <summary>
        /// Creates a ByteSerializer for flattening a fork to be sent to a Locutus device.
        /// </summary>
        /// <param name="fork">The fork to be sent to Locutus.</param>
        /// <returns>The ByteSerializer version of the object.</returns>
        public static INTV.Core.Utility.ByteSerializer ToForkByteSerializer(this IGlobalFileSystemEntry fork)
        {
            return (Fork)fork;
        }

        /// <summary>
        /// Gathers all the necessary update steps given a reference file system and differences computed against it.
        /// </summary>
        /// <param name="fileSystem">The source file system.</param>
        /// <param name="differences">Differences to apply to a target file system.</param>
        /// <param name="targetDeviceId">The unique ID of the target device.</param>
        /// <returns>A set of operations, each of which, if executed completely, constitute a single update to an instance of the LFS that leaves the system intact.</returns>
        /// <remarks>For every single element in the differences, this function will construct a complete description of all the elements necessary
        /// to perform a single update that will leave the target LFS in a reasonable state. These differences are ordered by size, with the largest
        /// first in the result. The intent is to provide a way to coalesce as many update operations into as few operations as possible on the device.
        /// At the meta level, ever single difference could be described as a single update to apply to the target LFS. However, this would be terribly
        /// slower than needed. Instead, as many operations as possible are uploaded to device RAM, and then executed. For example, adding a new directory
        /// containing files and subdirectories with more files will produce operations that cover the entire hierarchy update, as well as branches or
        /// other elements within the hierarchy. This allows for decomposition of unacceptably large updates into smaller updates, each of which is valid.
        /// This can, however, still mean that extremely large updates, if interrupted, could leave the file system in an incomplete state.</remarks>
        public static IOrderedEnumerable<LfsUpdateOperation> GatherAllUpdates(this FileSystem fileSystem, LfsDifferences differences, string targetDeviceId)
        {
            var updateOperations = fileSystem.GatherDirectoryUpdateData(differences, LfsOperations.Add, targetDeviceId);
            updateOperations.AddRange(fileSystem.GatherDirectoryUpdateData(differences, LfsOperations.Update, targetDeviceId));
            updateOperations.AddRange(fileSystem.GatherFileUpdateData(differences, LfsOperations.Add, targetDeviceId));
            updateOperations.AddRange(fileSystem.GatherFileUpdateData(differences, LfsOperations.Update, targetDeviceId));
            updateOperations.AddRange(fileSystem.GatherForkUpdateData(differences, LfsOperations.Add, targetDeviceId));
            updateOperations.AddRange(fileSystem.GatherForkUpdateData(differences, LfsOperations.Update, targetDeviceId));
            updateOperations.IdentifyAllFailedOperations(fileSystem, targetDeviceId);
            return updateOperations.OrderByDescending(u => u.TotalUpdateSize);
        }

        private static void IdentifyAllFailedOperations(this List<LfsUpdateOperation> operations, FileSystem fileSystem, string targetDeviceId)
        {
            var failedOperations = new HashSet<LfsUpdateOperation>(operations.Where(op => op.Operation.HasFlag(LfsOperations.FailedOperation)));
            if (failedOperations.Any(op => op.Operation.HasFlag(LfsOperations.Remove)))
            {
                // For now, fail spectacularly -- there really should not be any failed deletes. If there are, WTF?
                throw new InvalidOperationException("Unexpected failed remove operation!");
            }

            // Gather up failed fork operations.
            var failedForkUpdates = Enumerable.Empty<ushort>();
            foreach (var failedOp in failedOperations.Where(op => op.Operation.HasFlag(LfsOperations.Update)))
            {
                failedForkUpdates = failedForkUpdates.Union(failedOp.Forks);
            }
            var failedForkAdds = Enumerable.Empty<ushort>();
            foreach (var failedOp in failedOperations.Where(op => op.Operation.HasFlag(LfsOperations.Add)))
            {
                failedForkAdds = failedForkAdds.Union(failedOp.Forks);
            }

            // Identify file operations that involve failed fork add operations. These have to
            // fail as well, since we don't want to have a file point to a nonexistent fork.
            var fileOperations = operations.Where(op => !op.Operation.HasFlag(LfsOperations.FailedOperation) && op.Operation.HasFlag(LfsOperations.Add | LfsOperations.Update) && op.Files.Any()).ToList();
            var failedFileAdds = new HashSet<ushort>();
            foreach (var fileOperation in fileOperations)
            {
                foreach (var gfn in fileOperation.Files)
                {
                    var file = fileSystem.Files[gfn];
                    if (file.ForkNumbers.Intersect(failedForkAdds).Any())
                    {
                        // The file in this operation refers to a fork that is in a failed add operation.
                        // Therefore, mark this file operation as failed as well.
                        var error = new InconsistentFileSystemException("Failed to update file due to failed fork addition.", LfsEntityType.File, gfn, targetDeviceId);
                        fileOperation.RecordFailure(file.Name, error);
                        failedOperations.Add(fileOperation);
                        failedFileAdds.Add(gfn);
                        break;
                    }
                }
            }

            // Now, examine directory operations to determine if any of them involve failed
            // file operations. We don't care about failed file updates, only failed file adds.
            var directoryOperations = operations.Where(op => !op.Operation.HasFlag(LfsOperations.FailedOperation) && op.Operation.HasFlag(LfsOperations.Add | LfsOperations.Update) && op.Directories.Any()).ToList();
            foreach (var directoryOperation in directoryOperations)
            {
                foreach (var gdn in directoryOperation.Directories)
                {
                    var directory = fileSystem.Directories[gdn];
                    if (directory.PresentationOrder.Intersect(failedFileAdds).Any())
                    {
                        // The directory in this operation refers to a file that is in a failed add operation.
                        // Therefore, mark this directory operation as failed as well.
                        var error = new InconsistentFileSystemException("Failed to update directory due to failed file addition.", LfsEntityType.Directory, gdn, targetDeviceId);
                        directoryOperation.RecordFailure(directory.Name, error);
                        failedOperations.Add(directoryOperation);
                    }
                }
            }
        }

        /// <summary>
        /// Compute the largest possible aggregate operation from the given set of update operations.
        /// </summary>
        /// <param name="operations">The set of remaining operations to execute.</param>
        /// <param name="operation">Receives the update operation, which will fit into device RAM, that should be executed.</param>
        /// <returns>The remaining work to do, ordered largest to smallest in RAM usage.</returns>
        public static IOrderedEnumerable<LfsUpdateOperation> FetchUpdateOperation(this IOrderedEnumerable<LfsUpdateOperation> operations, out LfsUpdateOperation operation)
        {
            operation = operations.First(u => u.TotalUpdateSize <= Device.TotalRAMSize); // get the largest operation that fits in device memory.
            operations = operations.RemoveUpdateOperation(operation); // remove the operation, which will also update all remaining operations that may include duplicate work.
            while (operations.Any() && (operation.TotalUpdateSize < Device.TotalRAMSize))
            {
                var numOps = operations.Count();
                var potentialOp = new LfsUpdateOperation(operation); // used to compute a larger operation

                // Skip any remaining operations that would, if merged with the existing, be too large to include in the update.
                var moreOps = operations.SkipWhile(u => u.TotalUpdateSize > Device.TotalRAMSize).TakeWhile(u => (u.TotalUpdateSize + potentialOp.TotalUpdateSize) <= Device.TotalRAMSize);
                foreach (var op in moreOps)
                {
                    var mergedOp = potentialOp.Merge(op);
                    if (mergedOp.TotalUpdateSize < Device.TotalRAMSize)
                    {
                        // After the merge operation, which may significantly expand the size of the necessary updates to the GFT/GDT,
                        // we still had an operation that fit into RAM.
                        operation = mergedOp;
                        potentialOp = new LfsUpdateOperation(operation);
                        operations = operations.RemoveUpdateOperation(op);
                    }
                }
                if (numOps == operations.Count())
                {
                    // There weren't any operations to merge, so we've done as much as we can this time.
                    break;
                }
            }
            return operations;
        }

        private static IOrderedEnumerable<LfsUpdateOperation> RemoveUpdateOperation(this IOrderedEnumerable<LfsUpdateOperation> operations, LfsUpdateOperation completedOperation)
        {
            var updatedOperations = operations.ToList();
            updatedOperations.Remove(completedOperation);
            foreach (var operation in updatedOperations)
            {
                operation.Prune(completedOperation);
            }
            updatedOperations.RemoveAll(u => u.Operation == LfsOperations.None);
            return updatedOperations.OrderByDescending(u => u.TotalUpdateSize);
        }

        private static List<LfsUpdateOperation> GatherDirectoryUpdateData(this FileSystem fileSystem, LfsDifferences differences, LfsOperations operation, string targetDeviceId)
        {
            var updateOperations = new List<LfsUpdateOperation>();
            IList<IDirectory> directories = null;
            switch (operation)
            {
                case LfsOperations.Add:
                    directories = differences.DirectoryDifferences.ToAdd;
                    break;
                case LfsOperations.Update:
                    directories = differences.DirectoryDifferences.ToUpdate;
                    break;
            }
            foreach (var directory in directories)
            {
                var opName = directory.Name;
                var updateOperation = new LfsUpdateOperation(fileSystem, operation, LfsEntityType.Directory, directory.GlobalDirectoryNumber);
                try
                {
                    directory.GatherUpdateData(updateOperation, differences, targetDeviceId);
                }
                catch (Exception e)
                {
                    updateOperation.RecordFailure(LfsEntityType.Directory, directory.GlobalDirectoryNumber, opName, e, targetDeviceId);
                }
                updateOperations.Add(updateOperation);
            }

            return updateOperations;
        }

        private static void GatherUpdateData(this IDirectory directory, LfsUpdateOperation updateOperation, LfsDifferences differences, string targetDeviceId)
        {
            if (differences.Contains(directory, updateOperation.Operation) && updateOperation.Directories.Add(directory.GlobalDirectoryNumber))
            {
                var subdirectories = new List<IDirectory>();
                for (var i = 0; i < directory.PresentationOrder.ValidEntryCount; ++i)
                {
                    var file = updateOperation.FileSystem.Files[directory.PresentationOrder[i]];
                    file.GatherUpdateData(updateOperation, differences, targetDeviceId);
                    if (file.FileType == FileType.Folder)
                    {
                        var subdirectory = updateOperation.FileSystem.Directories[file.GlobalDirectoryNumber];
                        subdirectories.Add(subdirectory);
                    }
                }
                foreach (var subdirectory in subdirectories)
                {
                    subdirectory.GatherUpdateData(updateOperation, differences, targetDeviceId);
                }
            }
        }

        private static List<LfsUpdateOperation> GatherFileUpdateData(this FileSystem fileSystem, LfsDifferences differences, LfsOperations operation, string targetDeviceId)
        {
            var updateOperations = new List<LfsUpdateOperation>();
            IList<ILfsFileInfo> files = null;
            switch (operation)
            {
                case LfsOperations.Add:
                    files = differences.FileDifferences.ToAdd;
                    break;
                case LfsOperations.Update:
                    files = differences.FileDifferences.ToUpdate;
                    break;
            }
            foreach (var file in files)
            {
                var opName = file.Name;
                var updateOperation = new LfsUpdateOperation(fileSystem, operation, LfsEntityType.File, file.GlobalFileNumber);
                try
                {
                    file.GatherUpdateData(updateOperation, differences, targetDeviceId);
                }
                catch (Exception e)
                {
                    updateOperation.RecordFailure(LfsEntityType.File, file.GlobalFileNumber, opName, e, targetDeviceId);
                }
                updateOperations.Add(updateOperation);
            }
            return updateOperations;
        }

        private static void GatherUpdateData(this ILfsFileInfo file, LfsUpdateOperation updateOperation, LfsDifferences differences, string targetDeviceId)
        {
            if (differences.Contains(file, updateOperation.Operation) && updateOperation.Files.Add(file.GlobalFileNumber))
            {
                foreach (var forkNumber in file.ForkNumbers)
                {
                    if (forkNumber != GlobalForkTable.InvalidForkNumber)
                    {
                        var fork = updateOperation.FileSystem.Forks[forkNumber];
                        var forkName = (fork.Rom == null) ? fork.FullName : fork.Rom.RomPath; // retain this in case of error
                        try
                        {
                            fork.GatherUpdateData(updateOperation, differences);
                        }
                        catch (Exception e)
                        {
                            updateOperation.RecordFailure(LfsEntityType.Fork, forkNumber, forkName, e, targetDeviceId);
                        }
                    }
                }
            }
        }

        private static List<LfsUpdateOperation> GatherForkUpdateData(this FileSystem fileSystem, LfsDifferences differences, LfsOperations operation, string targetDeviceId)
        {
            var updateOperations = new List<LfsUpdateOperation>();
            IList<Fork> forks = null;
            switch (operation)
            {
                case LfsOperations.Add:
                    forks = differences.ForkDifferences.ToAdd;
                    break;
                case LfsOperations.Update:
                    forks = differences.ForkDifferences.ToUpdate;
                    break;
            }
            foreach (var fork in forks)
            {
                var updateOperation = new LfsUpdateOperation(fileSystem, operation, LfsEntityType.Fork, fork.GlobalForkNumber);
                var forkName = (fork.Rom == null) ? fork.FullName : fork.Rom.RomPath; // retain this in case of error
                try
                {
                    fork.GatherUpdateData(updateOperation, differences);
                }
                catch (Exception e)
                {
                    updateOperation.RecordFailure(LfsEntityType.Fork, fork.GlobalForkNumber, forkName, e, targetDeviceId);
                }
                updateOperations.Add(updateOperation);
            }
            return updateOperations;
        }

        private static void GatherUpdateData(this Fork fork, LfsUpdateOperation updateOperation, LfsDifferences differences)
        {
            if ((fork.GlobalForkNumber != GlobalForkTable.InvalidForkNumber) && differences.Contains(fork, updateOperation.Operation))
            {
                var fullName = fork.FullName;
                var forkPath = fork.FilePath; // has side effects
                if (System.IO.File.Exists(forkPath) && (fork.Size <= Device.TotalRAMSize))
                {
                    updateOperation.Forks.Add(fork.GlobalForkNumber);
                }
                else
                {
                    var pathForError = GetPathForError(fork, forkPath, fullName);
                    string errorMessage;
                    if (!System.IO.File.Exists(forkPath))
                    {
                        errorMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UpdateFileSystemError_MissingFileErrorFormat, pathForError);
                    }
                    else if (fork.Size > Device.TotalRAMSize)
                    {
                        errorMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UpdateFileSystemError_InvalidSizeErrorFormat, pathForError, fork.Size);
                    }
                    else
                    {
                        errorMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UpdateFileSystemError_UnexpectedErrorFormat, pathForError);
                    }
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private static string GetPathForError(Fork fork, string forkPath, string fallback)
        {
            var pathForError = (fork.Rom != null) ? fork.Rom.RomPath : forkPath;
            if (string.IsNullOrEmpty(pathForError) && !string.IsNullOrEmpty(fallback))
            {
                pathForError = fallback;
            }
            if (string.IsNullOrEmpty(pathForError))
            {
                pathForError = "<File Unknown> Global Fork ID: " + fork.GlobalForkNumber;
            }
            return pathForError;
        }

        /// <summary>
        /// Compares two IDirectory objects.
        /// </summary>
        /// <param name="lhs">The "left hand" side object.</param>
        /// <param name="rhs">The "right hand" side object.</param>
        /// <param name="forceUpdate">If true, refresh entry data prior to comparison.</param>
        /// <param name="failedCompareEntryName">Receives the name of the directory whose compare operation failed.</param>
        /// <param name="error">Receives the error that occurred during the comparison.</param>
        /// <returns><c>true</c> if the two objects should be considered the same.</returns>
        /// <remarks>>**NOTE:** GDNs are not compared because this method is used for comparing file systems directly,
        /// via a GDT table walk. By definition, they will always match. They are not part of LFS, they are implicit.</remarks>
        private static bool CompareIDirectories(IDirectory lhs, IDirectory rhs, bool forceUpdate, out string failedCompareEntryName, out Exception error)
        {
            failedCompareEntryName = null;
            error = null;
            var areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual)
            {
                // There's no clear reason here to require the casting to Folder vs. Directory. Perhaps this was written before PresentationOrder was unified?
#if false
                var hostDirectory = lhs as Folder;
                if (hostDirectory == null)
                {
                    hostDirectory = rhs as Folder;
                }
                var ltoFlashDirectory = rhs as Directory;
                if (ltoFlashDirectory == null)
                {
                    ltoFlashDirectory = lhs as Directory;
                }
                areEqual = hostDirectory.ParentDirectoryGlobalFileNumber == ltoFlashDirectory.ParentDirectoryGlobalFileNumber;
                areEqual &= hostDirectory.PresentationOrder == ltoFlashDirectory.PresentationOrder;
#else
                areEqual = lhs.ParentDirectoryGlobalFileNumber == rhs.ParentDirectoryGlobalFileNumber;
                areEqual &= lhs.PresentationOrder == rhs.PresentationOrder;
#endif
            }
            return areEqual;
        }

        private static readonly ForkKind[] DefaultForksToIgnore = new[] { ForkKind.JlpFlash };
        private static readonly ForkKind[] RootFileDefaultForksToIgnore = new[] { ForkKind.JlpFlash, ForkKind.Manual };

        /// <summary>
        /// Compares two ILfsFileInfo objects.
        /// </summary>
        /// <param name="lhs">The "left hand" side object.</param>
        /// <param name="rhs">The "right hand" side object.</param>
        /// <param name="forceUpdate">If true, refresh entry data prior to comparison.</param>
        /// <param name="failedCompareEntryName">Receives the name of the file whose compare operation failed.</param>
        /// <param name="error">Receives the error that occurred during the comparison.</param>
        /// <returns><c>true</c> if the two objects should be considered the same.</returns>
        private static bool CompareILfsFileInfo(ILfsFileInfo lhs, ILfsFileInfo rhs, bool forceUpdate, out string failedCompareEntryName, out Exception error)
        {
            // For compares, always ignore the fork number for JLP Flash for now.
            var isRootFile = (lhs != null) && object.ReferenceEquals(lhs.FileSystem.Files[GlobalFileTable.RootDirectoryFileNumber], lhs);

            var forksToIgnore = isRootFile ? RootFileDefaultForksToIgnore : DefaultForksToIgnore;
            return CompareILfsFileInfo(lhs, rhs, forceUpdate, forksToIgnore, out failedCompareEntryName, out error);
        }

        /// <summary>
        /// Compares two ILfsFileInfo objects.
        /// </summary>
        /// <param name="lhs">The "left hand" side object.</param>
        /// <param name="rhs">The "right hand" side object.</param>
        /// <param name="forceUpdate">If true, refresh entry data prior to comparison.</param>
        /// <param name="forkKindsToIgnore">Forks to ignore during the file system comparison.</param>
        /// <param name="failedCompareEntryName">Receives the name of the file whose compare operation failed.</param>
        /// <param name="error">Receives the error that occurred during the comparison.</param>
        /// <returns><c>true</c> if the two objects should be considered the same.</returns>
        /// <remarks>**NOTE:** The JLP Flash Save Data forks are **IGNORED** in this comparison, as well as any other
        /// types of forks explicitly passed to this function.
        /// **NOTE:** GFNs are not compared because this method is used for comparing file systems directly,
        /// via a GFT table walk. By definition, they will always match. They are not part of LFS, they are implicit.</remarks>
        private static bool CompareILfsFileInfo(ILfsFileInfo lhs, ILfsFileInfo rhs, bool forceUpdate, IEnumerable<ForkKind> forkKindsToIgnore, out string failedCompareEntryName, out Exception error)
        {
            failedCompareEntryName = null;
            error = null;
            var areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual)
            {
                // This was probably written before ILfsFileInfo fully supported everything in the compare...
#if false
                var hostFile = lhs as FileNode;
                if (hostFile == null)
                {
                    hostFile = rhs as FileNode;
                }
                var ltoFlashFile = rhs as LfsFileInfo;
                if (ltoFlashFile == null)
                {
                    ltoFlashFile = lhs as LfsFileInfo;
                }
                areEqual = (hostFile.FileType == ltoFlashFile.FileType) && (hostFile.Color == ltoFlashFile.Color) && (hostFile.GlobalDirectoryNumber == ltoFlashFile.GlobalDirectoryNumber);
                areEqual = areEqual && (hostFile.LongName == ltoFlashFile.LongName) && (hostFile.Reserved == ltoFlashFile.Reserved);
#endif
                areEqual = (lhs.FileType == rhs.FileType) && (lhs.Color == rhs.Color) && (lhs.GlobalDirectoryNumber == rhs.GlobalDirectoryNumber);
                areEqual = areEqual && (lhs.LongName == rhs.LongName) && (lhs.Reserved == rhs.Reserved);
                if (areEqual)
                {
                    // On host file systems, we may use NULL as the short name if short and long name are the same.
                    // The on-device LFS maintains these explicitly separately, so we need to ensure valid short name.
                    var lhsShortName = lhs.ShortName;
                    if (lhsShortName == null)
                    {
                        lhsShortName = string.Empty;
                        if (!string.IsNullOrEmpty(lhs.LongName))
                        {
                            lhsShortName = lhs.LongName.Substring(0, Math.Min(lhs.LongName.Length, FileSystemConstants.MaxShortNameLength));
                        }
                    }
                    var rhsShortName = rhs.ShortName;
                    if (rhsShortName == null)
                    {
                        rhsShortName = string.Empty;
                        if (!string.IsNullOrEmpty(rhs.LongName))
                        {
                            rhsShortName = rhs.LongName.Substring(0, Math.Min(rhs.LongName.Length, FileSystemConstants.MaxShortNameLength));
                        }
                    }
                    areEqual = lhsShortName == rhsShortName;
#if false
                    if (hostFile.ShortName == null)
                    {
                        // On the host, we may use NULL as the short name if short and long name are the same.
                        // The on-device LFS maintains these explicitly separately, so we need to ensure valid short name.
                        var localShortName = string.Empty;
                        if (!string.IsNullOrEmpty(hostFile.LongName))
                        {
                            localShortName = hostFile.LongName.Substring(0, Math.Min(hostFile.LongName.Length, FileSystemConstants.MaxShortNameLength));
                        }
                        areEqual = localShortName == ltoFlashFile.ShortName;
                    }
                    else
                    {
                        areEqual = hostFile.ShortName == ltoFlashFile.ShortName;
                    }
#endif
                }
#if false
                for (int k = 0; areEqual && (k < hostFile.ForkNumbers.Length); ++k)
                {
                    // For compares, always ignore the fork number for JLP Flash for now.
                    if (!forkKindsToIgnore.Contains((ForkKind)k) && k != (int)ForkKind.JlpFlash)
                    {
                        areEqual = hostFile.ForkNumbers[k] == ltoFlashFile.ForkNumbers[k];
                    }
                }
#endif
                for (int k = 0; areEqual && (k < lhs.ForkNumbers.Length); ++k)
                {
                    // For compares, always ignore the fork number for JLP Flash for now.
                    var forkKind = (ForkKind)k;
                    if (!forkKindsToIgnore.Contains(forkKind) && (forkKind != ForkKind.JlpFlash))
                    {
                        areEqual = lhs.ForkNumbers[k] == rhs.ForkNumbers[k];
                    }
                }
            }
            return areEqual;
        }

        private static bool ValidateFile(ILfsFileInfo file, Device targetDevice, bool forceUpdate, out string failedValidationEntryName, out Exception error)
        {
            failedValidationEntryName = null;
            error = null;
            var valid = true;
            if ((file.FileSystem.Origin == FileSystemOrigin.HostComputer) && (targetDevice != null))
            {
                var program = file as Program;
                if ((program != null) && (file.Rom != null))
                {
                    var rom = program.Description.Rom;
                    if (!rom.CanExecuteOnDevice(targetDevice.UniqueId))
                    {
                        valid = false;
                        failedValidationEntryName = program.GetMenuPath();
                        if (!valid)
                        {
                            file.FileSystem.Files.LogActivity("File validation failed for " + program.ToString() + ", rDRUID: " + rom.GetTargetDeviceUniqueId() + ", tDRUID: " + targetDevice.UniqueId);
                        }
                        error = new IncompatibleRomException(rom, rom.GetTargetDeviceUniqueId(), targetDevice.UniqueId, LfsEntityType.File, file.GlobalFileNumber);
                    }
                }
            }
            return valid;
        }

        /// <summary>
        /// Compares two Fork objects.
        /// </summary>
        /// <param name="lhs">The "left hand" side object.</param>
        /// <param name="rhs">The "right hand" side object.</param>
        /// <param name="forceUpdate">If true, refresh entry data prior to comparison.</param>
        /// <param name="failedCompareEntryName">Receives the name of the fork whose compare operation failed.</param>
        /// <param name="error">Receives the error that occurred during the comparison.</param>
        /// <returns><c>true</c> if the two objects should be considered the same.</returns>
        /// <remarks>**NOTE:** LUI does not use, assign, or maintain the StartingVirtualBlock - that is used by the
        /// on-device LFS only, and under its control.
        /// **NOTE:** GKNs are not compared because this method is only used for comparing file systems directly,
        /// via a GKT table walk. By definition, they will always match. They are not part of LFS, they are implicit.</remarks>
        private static bool CompareForks(Fork lhs, Fork rhs, bool forceUpdate, out string failedCompareEntryName, out Exception error)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            failedCompareEntryName = null;
            error = null;
            if (lhs.FileSystem.Origin == FileSystemOrigin.HostComputer)
            {
                lhs.Refresh(forceUpdate, out error, out failedCompareEntryName);
            }
            if (rhs.FileSystem.Origin == FileSystemOrigin.HostComputer)
            {
                rhs.Refresh(forceUpdate, out error, out failedCompareEntryName);
            }
            var areEqual = (lhs.Crc24 == rhs.Crc24) && (lhs.Size == rhs.Size);
            return areEqual;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.CompareForks() took: + " + stopwatch.Elapsed.ToString());
                _accumulatedForkCompare += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }

        private static bool ValidateFork(Fork fork, Device targetDevice, bool forceUpdate, out string failedValidationEntryName, out Exception error)
        {
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            failedValidationEntryName = null;
            error = null;
            var valid = true;
            if (fork.FileSystem.Origin == FileSystemOrigin.HostComputer)
            {
                fork.Refresh(forceUpdate, out error, out failedValidationEntryName);
                valid = error == null;
                var program = fork.FileSystem.GetFilesUsingForks(new[] { fork }).FirstOrDefault().Value as Program;
                if (program != null)
                {
                    var rom = program.Description.Rom;
                    if ((targetDevice != null) && !rom.CanExecuteOnDevice(targetDevice.UniqueId))
                    {
#if REPORT_PERFORMANCE
                        var canexecstopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_PERFORMANCE
                        valid = false;
                        failedValidationEntryName = program.Name;
                        error = new IncompatibleRomException(rom, rom.GetTargetDeviceUniqueId(), targetDevice.UniqueId, LfsEntityType.Fork, fork.GlobalForkNumber);
#if REPORT_PERFORMANCE
                        canexecstopwatch.Stop();
                        _accumulatedCanExecuteOnDevice += canexecstopwatch.Elapsed;
#endif // REPORT_PERFORMANCE
                    }
                }
            }
            return valid;
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(">>FileSystem.ValidateFork() took: + " + stopwatch.Elapsed.ToString());
                _accumulatedForkValidation += stopwatch.Elapsed;
            }
#endif // REPORT_PERFORMANCE
        }
    }
}

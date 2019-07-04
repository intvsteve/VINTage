// <copyright file="DeviceHelpers.RetrieveForkData.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implementation for RetrieveForkData.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region RetrieveForkData

#if NEED_RAW_RETRIEVEFORKDATA

        /// <summary>
        /// Retrieves the data associated with forks from a Locutus device.
        /// </summary>
        /// <param name="device">The device upon which the data resides.</param>
        /// <param name="forksToRetrieve">The data forks to retrieve.</param>
        public static void RetrieveForkData(this Device device, IEnumerable<Fork> forksToRetrieve)
        {
            if (forksToRetrieve.Any() && device.IsSafeToStartCommand())
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, ProtocolCommandId.MultistagePseudoCommand)
                {
                    Data = forksToRetrieve
                };
                executeCommandTaskData.StartTask(RetrieveForkData);
            }
        }

        private static void RetrieveForkData(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            var configuration = SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
            var forks = (IEnumerable<Fork>)data.Data;
            var recoveryDirectory = configuration.GetDeviceDataAreaPath(device.UniqueId);
            var succeeded = device.RetrieveForkData(data, forks, recoveryDirectory);
            data.Succeeded = succeeded;
        }

#endif // NEED_RAW_RETRIEVEFORKDATA

        private static bool RetrieveForkData(this Device device, ExecuteDeviceCommandAsyncTaskData data, IEnumerable<Fork> forksToRetrieve, string destinationDirectory, IEnumerable<string> fileNames)
        {
            var configuration = SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
            var succeeded = false;
            var numForks = forksToRetrieve.Count();
            var forkFilenames = fileNames == null ? null : fileNames.ToList();
            var forkBeingRetrieved = 0;
            foreach (var fork in forksToRetrieve)
            {
                if (data.AcceptCancelIfRequested())
                {
                    break;
                }
                ++forkBeingRetrieved;
                if (data != null)
                {
                    data.UpdateTaskProgress((double)forkBeingRetrieved / numForks, string.Format(Resources.Strings.DeviceMultistageCommand_BackupFileSystem_RetrievingFiles_Format, forkBeingRetrieved, numForks));
                }
                var bytesRemaining = (int)fork.Size;
                var offset = 0;
                succeeded = false;
                using (var memory = new System.IO.MemoryStream())
                {
                    do
                    {
                        const uint Address = 0u;
                        var bytesToRead = System.Math.Min(bytesRemaining, Device.TotalRAMSize);
                        succeeded = ReadForkToRam.Create(Address, fork.GlobalForkNumber, (uint)offset, bytesToRead).Execute<bool>(device.Port, data);
                        byte[] dataRead = null;
                        if (succeeded)
                        {
                            dataRead = (byte[])DownloadDataBlockFromRam.Create(Address, bytesToRead).Execute(device.Port, data, out succeeded);
                        }
                        if (succeeded)
                        {
                            memory.Write(dataRead, offset, bytesToRead);
                            bytesRemaining -= bytesToRead;
                            offset += bytesToRead;
                        }
                    }
                    while (succeeded && (bytesRemaining > 0));

                    if (data != null)
                    {
                        data.UpdateTaskProgress((double)forkBeingRetrieved / numForks, string.Format(Resources.Strings.DeviceMultistageCommand_BackupFileSystem_SavingFiles_Format, forkBeingRetrieved, numForks));
                    }
                    memory.Seek(0, System.IO.SeekOrigin.Begin);
                    using (var tempFile = FileSystemFile.Inflate(memory))
                    {
                        var fileName = (forkFilenames == null) ? configuration.GetForkDataFileName(fork.GlobalForkNumber) : forkFilenames[forkBeingRetrieved - 1];
                        var forkPath = new StorageLocation(System.IO.Path.Combine(destinationDirectory, fileName));
                        forkPath = forkPath.EnsureUnique();
                        data.FailureMessage = forkPath.Path;
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(forkPath.Path));
                        try
                        {
                            // Test to see if this is a Fork containing a LUIGI file.
                            memory.Seek(0, System.IO.SeekOrigin.Begin);
                            LuigiFileHeader.Inflate(memory);
                            forkPath = forkPath.ChangeExtension(ProgramFileKind.LuigiFile.FileExtension());
                        }
                        catch (INTV.Core.UnexpectedFileTypeException)
                        {
                            // This is OK... we only want to execute certain functions if the file is a LUIGI file.
                        }
                        if (forkPath.Exists())
                        {
                            var crcOfTarget = Crc32.OfFile(forkPath);
                            var crcOfSource = Crc32.OfFile(new StorageLocation(tempFile.FileInfo.FullName));
                            if (crcOfTarget != crcOfSource)
                            {
                                forkPath = forkPath.EnsureUnique();
                                System.IO.File.Copy(tempFile.FileInfo.FullName, forkPath.Path);
                            }
                        }
                        else
                        {
                            System.IO.File.Copy(tempFile.FileInfo.FullName, forkPath.Path);
                        }
                        fork.FilePath = forkPath.Path;
                    }
                }
                if (!succeeded)
                {
                    break;
                }
            }
            return succeeded;
        }

        #endregion // RetrieveForkData
    }
}

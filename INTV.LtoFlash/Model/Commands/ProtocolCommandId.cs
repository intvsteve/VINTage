// <copyright file="ProtocolCommandId.cs" company="INTV Funhouse">
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

////#define TEST_COMMAND_STRINGS

using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// These values define the commands sent to a LTO Flash! device. All values not
    /// explicitly stated here shall be considered reserved for future use. Details
    /// are in the wire_protocol.txt document.
    /// </summary>
    public enum ProtocolCommandId : byte
    {
        #region General Commands

        /// <summary>
        /// First general-purpose command.
        /// </summary>
        GeneralCommandsBegin = 0x00,

        /// <summary>
        /// Ping command - provides no response, should always succeed.
        /// </summary>
        Ping = GeneralCommandsBegin,

        /// <summary>
        /// Periodically sending this command instructs Locutus to perform any necessary
        /// file system erase operations in the background.
        /// </summary>
        GarbageCollect = 0x01,

        /// <summary>
        /// This command loads a LUIGI format ROM to Locutus, at which point the ROM
        /// will be launched to run on the console.
        /// </summary>
        DownloadAndPlay = 0x02,

        /// <summary>
        /// Set configuration bits of the Locutus device features that can be modified.
        /// </summary>
        SetConfiguration = 0x03,

        /// <summary>
        /// Downloads the error log from the device.
        /// </summary>
        DownloadErrorLog = 0x04,

        /// <summary>
        /// Downloads the crash log from the device.
        /// </summary>
        DownloadCrashLog = 0x05,

        /// <summary>
        /// Erases the crash log on the device.
        /// </summary>
        EraseCrashLog = 0x06,

        /// <summary>
        /// First reserved command in general-purpose range.
        /// </summary>
        GeneralCommandsReservedBegin = 0x07,

        /// <summary>
        /// Last reserved command in general-purpose range.
        /// </summary>
        GeneralCommandReservedEnd = 0x0C,

        /// <summary>
        /// Backdoor command, which is used to decode a LUIGI fork.
        /// </summary>
        /// <remarks>This command is not implemented.</remarks>
        DecodeLuigiFromFork = 0x0D,

        /// <summary>
        /// Backdoor command only supported by Locutus simulator, which is used to inject random failure modes into the simulator.
        /// </summary>
        DebugSetRandomDropConnection = 0x0E,

        /// <summary>
        /// Backdoor command only supported by Locutus simulator, which is used to set "hardware" status flags.
        /// </summary>
        DebugSetHardwareStatus = 0x0F,

        #endregion // General Commands

        #region File System Commands

        /// <summary>
        /// Get the file system's statistics.
        /// </summary>
        LfsGetStatistics = 0x10,

        /// <summary>
        /// Get LUI-defined dirty flags.
        /// </summary>
        LfsGetFileSystemStatusFlags = 0x11,

        /// <summary>
        /// Set LUI-defined dirty flags.
        /// </summary>
        LfsSetFileSystemStatusFlags = 0x12,

        /// <summary>
        /// Download the global file system tables (Global Directory Table (GDT), Global File Table (GFT) and Global Fork Table (GKT).
        /// </summary>
        LfsDownloadGlobalTables = 0x13,

        /// <summary>
        /// Upload a data block to Locutus onboard RAM.
        /// </summary>
        LfsUploadDataBlockToRam = 0x14,

        /// <summary>
        /// Download a data block from Locutus onboard RAM.
        /// </summary>
        LfsDownloadDataBlockFromRam = 0x15,

        /// <summary>
        /// Compute the checksum of the data in Locutus's onboard RAM.
        /// </summary>
        LfsChecksumDataBlockInRam = 0x16,

        /// <summary>
        /// Update the Global Directory Table from a data block in onboard RAM.
        /// </summary>
        LfsUpdateGdtFromRam = 0x17,

        /// <summary>
        /// Update the Global File Table from a data block in onboard RAM.
        /// </summary>
        LfsUpdateGftFromRam = 0x18,

        /// <summary>
        /// Create a data fork from a data block in onboard RAM.
        /// </summary>
        LfsCreateForkFromRam = 0x19,

        /// <summary>
        /// Copy a data fork to a data block in onboard RAM.
        /// </summary>
        LfsCopyForkToRam = 0x1A,

        /// <summary>
        /// Update the unique identifier of a fork.
        /// </summary>
        LfsUpdateForkUid = 0x1B,

        /// <summary>
        /// Delete a fork from a file in the file system.
        /// </summary>
        LfsDeleteFork = 0x1C,

        /// <summary>
        /// Delete a file from the file system.
        /// </summary>
        LfsDeleteFile = 0x1D,

        /// <summary>
        /// Delete a directory from the file system.
        /// </summary>
        LfsDeleteDirectory = 0x1E,

        /// <summary>
        /// Reinitialize the entire file system. All data on the device will be lost.
        /// </summary>
        LfsReformatFileSystem = 0x1F,

        #endregion // File System Commands

        #region Firmware Commands

        /// <summary>
        /// Gets the primary, secondary, and active firmware revisions.
        /// </summary>
        FirmwareGetRevisions = 0x20,

        /// <summary>
        /// Validate the firmware image that has been uploaded to RAM.
        /// </summary>
        FirmwareValidateImageInRam = 0x21,

        /// <summary>
        /// Erase the secondary firmware, which restores the device to factory-default.
        /// </summary>
        FirmwareEraseSecondary = 0x22,

        /// <summary>
        /// Apply a firmware update.
        /// </summary>
        FirmwareProgramSecondary = 0x23,

        #endregion // Firmware Commands

        // A note regarding reserved commands: This UI software never sends these commands to the device,
        // as they do not exist in the firmware. This is an abuse of the design, as it is using command IDs
        // from the reserved space for its own internal purposes.
        #region Reserved Commands

        /// <summary>
        /// Sentinel value indicating start of reserved command ID range.
        /// </summary>
        UnusedCommandsBegin = 0x24,

        /// <summary>
        /// Pseudo-command ID representing a command that requires multiple distinct commands.
        /// </summary>
        MultistagePseudoCommand = 0xFD,

        /// <summary>
        /// Pseudo-command ID representing a wait for a complete beacon to be received.
        /// </summary>
        WaitForBeaconPseudoCommand = 0xFE,

        /// <summary>
        /// Represents an unknown command.
        /// </summary>
        UnknownCommand = 0xFF,

        #endregion // Reserved Commands
    }

    /// <summary>
    /// Extension methods for the ProtocolCommandId type.
    /// </summary>
    internal static class ProtocolCommandIdHelpers
    {
        /// <summary>
        /// Gets a string to use as a progress bar title when running a command.
        /// </summary>
        /// <param name="commandId">The command ID for which to get a string.</param>
        /// <returns>The string to display.</returns>
        public static string GetProgressTitle(this ProtocolCommandId commandId)
        {
            return commandId.GetStringForCommandId(Suffix.Title);
        }

        /// <summary>
        /// Gets a string to report that a command failed.
        /// </summary>
        /// <param name="commandId">The command ID for which to get a failure string.</param>
        /// <returns>The failure string.</returns>
        public static string GetFailureString(this ProtocolCommandId commandId)
        {
            return commandId.GetStringForCommandId(Suffix.Failed);
        }

        /// <summary>
        /// Verifies that all the expected resource strings are defined.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void CheckCommandResourceStrings()
        {
#if TEST_COMMAND_STRINGS
            var commandsToIgnore = new ProtocolCommandId[]
            {
                ProtocolCommandId.GeneralCommandsBegin,
                ProtocolCommandId.GeneralCommandsReservedBegin,
                ProtocolCommandId.GeneralCommandReservedEnd,
                ProtocolCommandId.UnusedCommandsBegin
            };
            var values = (System.Collections.Generic.IEnumerable<ProtocolCommandId>)System.Enum.GetValues(typeof(ProtocolCommandId));
            foreach (var commandId in values)
            {
                if (System.Array.IndexOf(commandsToIgnore, commandId) < 0)
                {
                    var resourceString = commandId.GetProgressTitle();
                    System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(resourceString));
                    System.Diagnostics.Debug.WriteLine(resourceString);
                    resourceString = commandId.GetFailureString();
                    System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(resourceString));
                    System.Diagnostics.Debug.WriteLine(resourceString);
                }
            }
#endif // TEST_COMMAND_STRINGS
        }

        private static string GetStringForCommandId(this ProtocolCommandId commandId, Suffix suffix)
        {
            var resourceKey = typeof(ProtocolCommandId).Name + "_" + commandId + "_" + suffix;
            var result = typeof(ProtocolCommandId).GetResourceString(resourceKey);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new System.ArgumentOutOfRangeException("commandId", string.Format(Resources.Strings.MissingCommandIdStringFormat, suffix, commandId));
            }
            return result;
        }

        private enum Suffix
        {
            /// <summary>
            /// Title suffix to append to command string.
            /// </summary>
            Title,

            /// <summary>
            /// Failure suffix to append to command string.
            /// </summary>
            Failed
        }
    }
}

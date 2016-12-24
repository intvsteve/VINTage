// <copyright file="DeviceCommandAvailability.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.LtoFlash.Model.Commands;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class is used to track the availability of device commands.
    /// </summary>
    internal class DeviceCommandAvailability
    {
        private static readonly Dictionary<ProtocolCommandId, Tuple<Range<int>, HardwareStatusFlags, FeatureCompatibility>> NewFirmwareCommands = new Dictionary<ProtocolCommandId, Tuple<Range<int>, HardwareStatusFlags, FeatureCompatibility>>()
        {
            { ProtocolCommandId.DownloadCrashLog, new Tuple<Range<int>, HardwareStatusFlags, FeatureCompatibility>(new Range<int>(1438, int.MaxValue), HardwareStatusFlags.NewCrashLogAvailable, FeatureCompatibility.Requires) },
            { ProtocolCommandId.EraseCrashLog, new Tuple<Range<int>, HardwareStatusFlags, FeatureCompatibility>(new Range<int>(1438, int.MaxValue), HardwareStatusFlags.None, FeatureCompatibility.Tolerates) }
        };

        private static readonly Dictionary<ProtocolCommandId, Range<int>> AlwaysAvailableInFirmwareVersion = new Dictionary<ProtocolCommandId, Range<int>>()
        {
            { ProtocolCommandId.DownloadErrorLog, new Range<int>(1246, int.MaxValue) },
            { ProtocolCommandId.FirmwareGetRevisions, new Range<int>(1246, int.MaxValue) },
            { ProtocolCommandId.LfsGetStatistics, new Range<int>(1416, int.MaxValue) },
            { ProtocolCommandId.LfsGetFileSystemStatusFlags, new Range<int>(2034, int.MaxValue) },
            { ProtocolCommandId.LfsDownloadGlobalTables, new Range<int>(2034, int.MaxValue) }
        };

        /// <summary>
        /// Commands that should always be available.
        /// </summary>
        private static readonly List<ProtocolCommandId> AlwaysAvailableCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.Ping,
            ProtocolCommandId.GarbageCollect,
            ProtocolCommandId.DownloadErrorLog, // since FW version 1246
            ProtocolCommandId.FirmwareGetRevisions // since FW version 1246
        };

        /// <summary>
        /// Commands that should never be available.
        /// </summary>
        private static readonly List<ProtocolCommandId> NeverAvailableCommands = new List<ProtocolCommandId>()
        {
            ProtocolCommandId.DebugSetHardwareStatus,
            ProtocolCommandId.DebugSetRandomDropConnection,
        };

        // Crap -- can't use IReadOnlyDictionary due to stupid .NET 4.0 for xp.
        private static readonly IDictionary<ProtocolCommandId, Tuple<HardwareStatusFlags, FeatureCompatibility>> DefaultCommandAvailability = new Dictionary<ProtocolCommandId, Tuple<HardwareStatusFlags, FeatureCompatibility>>()
        {
            { ProtocolCommandId.Ping, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.None, FeatureCompatibility.Tolerates) },
            { ProtocolCommandId.GarbageCollect, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.None, FeatureCompatibility.Tolerates) },
            { ProtocolCommandId.DownloadAndPlay, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Requires) },
            { ProtocolCommandId.SetConfiguration, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            ////{ ProtocolCommandId.DownloadErrorLog, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.DownloadErrorLog, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.None, FeatureCompatibility.Tolerates) }, // since FW version 1246
            { ProtocolCommandId.DebugSetRandomDropConnection, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.None, FeatureCompatibility.Tolerates) },
            { ProtocolCommandId.DebugSetHardwareStatus, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsGetStatistics, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsGetFileSystemStatusFlags, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Tolerates) }, // since FW version 2034
            { ProtocolCommandId.LfsSetFileSystemStatusFlags, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsDownloadGlobalTables, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Tolerates) }, // since FW version 2034
            { ProtocolCommandId.LfsUploadDataBlockToRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsDownloadDataBlockFromRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsChecksumDataBlockInRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsUpdateGdtFromRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsUpdateGftFromRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsCreateForkFromRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsCopyForkToRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsUpdateForkUid, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsDeleteFork, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsDeleteFile, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsDeleteDirectory, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.LfsReformatFileSystem, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            ////{ ProtocolCommandId.FirmwareGetRevisions, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.FirmwareGetRevisions, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Tolerates) }, // since FW version 1246
            { ProtocolCommandId.FirmwareValidateImageInRam, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.FirmwareEraseSecondary, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
            { ProtocolCommandId.FirmwareProgramSecondary, new Tuple<HardwareStatusFlags, FeatureCompatibility>(HardwareStatusFlags.ConsolePowerOn, FeatureCompatibility.Incompatible) },
        };

        private static readonly Dictionary<ProtocolCommandId, CommandAvailable> DefaultCommandsSupported = new Dictionary<ProtocolCommandId, CommandAvailable>()
        {
            { ProtocolCommandId.Ping, CommandAvailable.Always },
            { ProtocolCommandId.GarbageCollect, CommandAvailable.Always },
            { ProtocolCommandId.DownloadAndPlay, CommandAvailable.No },
            { ProtocolCommandId.SetConfiguration, CommandAvailable.No },
            { ProtocolCommandId.DownloadErrorLog, CommandAvailable.No },
            { ProtocolCommandId.DebugSetRandomDropConnection, CommandAvailable.Never },
            { ProtocolCommandId.DebugSetHardwareStatus, CommandAvailable.Never },
            { ProtocolCommandId.LfsGetStatistics, CommandAvailable.No },
            { ProtocolCommandId.LfsGetFileSystemStatusFlags, CommandAvailable.No },
            { ProtocolCommandId.LfsSetFileSystemStatusFlags, CommandAvailable.No },
            { ProtocolCommandId.LfsDownloadGlobalTables, CommandAvailable.No },
            { ProtocolCommandId.LfsUploadDataBlockToRam, CommandAvailable.No },
            { ProtocolCommandId.LfsDownloadDataBlockFromRam, CommandAvailable.No },
            { ProtocolCommandId.LfsChecksumDataBlockInRam, CommandAvailable.No },
            { ProtocolCommandId.LfsUpdateGdtFromRam, CommandAvailable.No },
            { ProtocolCommandId.LfsUpdateGftFromRam, CommandAvailable.No },
            { ProtocolCommandId.LfsCreateForkFromRam, CommandAvailable.No },
            { ProtocolCommandId.LfsCopyForkToRam, CommandAvailable.No },
            { ProtocolCommandId.LfsUpdateForkUid, CommandAvailable.No },
            { ProtocolCommandId.LfsDeleteFork, CommandAvailable.No },
            { ProtocolCommandId.LfsDeleteFile, CommandAvailable.No },
            { ProtocolCommandId.LfsDeleteDirectory, CommandAvailable.No },
            { ProtocolCommandId.LfsReformatFileSystem, CommandAvailable.No },
            { ProtocolCommandId.FirmwareGetRevisions, CommandAvailable.No },
            { ProtocolCommandId.FirmwareValidateImageInRam, CommandAvailable.No },
            { ProtocolCommandId.FirmwareEraseSecondary, CommandAvailable.No },
            { ProtocolCommandId.FirmwareProgramSecondary, CommandAvailable.No },
        };

        private readonly Dictionary<ProtocolCommandId, Tuple<HardwareStatusFlags, FeatureCompatibility>> _commandAvailability = new Dictionary<ProtocolCommandId, Tuple<HardwareStatusFlags, FeatureCompatibility>>(DefaultCommandAvailability);

        /// <summary>
        /// Alter the availability of a command. This may be used, for example, to modify the availability of commands not implemented
        /// in the simulator, or only available in hardware, et. al.
        /// </summary>
        /// <param name="command">The command whose availability is being changed.</param>
        /// <param name="hardwareStatus">Hardware status associated with command availability.</param>
        /// <param name="compatibility">Whether the given hardware flag(s) are required, or must not be set.</param>
        public void ChangeCommandAvailablility(ProtocolCommandId command, HardwareStatusFlags hardwareStatus, FeatureCompatibility compatibility)
        {
            _commandAvailability[command] = new Tuple<HardwareStatusFlags, FeatureCompatibility>(hardwareStatus, compatibility);
        }

        /// <summary>
        /// Resets the command availability to its default value.
        /// </summary>
        /// <param name="command">The command whose availability is being reset.</param>
        public void ResetCommandAvailabilityToDefault(ProtocolCommandId command)
        {
            _commandAvailability[command] = DefaultCommandAvailability[command];
            _commandsSupported[command] = DefaultCommandsSupported[command];
        }

        /// <summary>
        /// Update the command availability tables when the firmware version changes.
        /// </summary>
        /// <param name="firmwareVersion">Active firmware version.</param>
        public void UpdateCommandAvailabilityForFirmwareVersion(int firmwareVersion)
        {
            var commandsToUpdate = AlwaysAvailableInFirmwareVersion.Where(c => c.Value.IsValueInRange(firmwareVersion)).Select(c => c.Key);
            foreach (var command in commandsToUpdate)
            {
                ChangeCommandAvailablility(command, HardwareStatusFlags.None, FeatureCompatibility.Tolerates);
                ChangeCommandAvailablility(command, CommandAvailable.Always);
            }

            var commandsToAdd = NewFirmwareCommands.Where(c => c.Value.Item1.IsValueInRange(firmwareVersion));
            foreach (var commandToAdd in commandsToAdd)
            {
                _commandAvailability[commandToAdd.Key] = new Tuple<HardwareStatusFlags, FeatureCompatibility>(commandToAdd.Value.Item2, commandToAdd.Value.Item3);
            }

            var commandsToRestoreToDefault = AlwaysAvailableInFirmwareVersion.Where(c => !c.Value.IsValueInRange(firmwareVersion)).Select(c => c.Key);
            foreach (var command in commandsToRestoreToDefault)
            {
                ResetCommandAvailabilityToDefault(command);
            }

            var commandsToRemove = NewFirmwareCommands.Where(c => !c.Value.Item1.IsValueInRange(firmwareVersion)).Select(c => c.Key);
            foreach (var command in commandsToRemove)
            {
                _commandAvailability.Remove(command);
            }
        }

        /// <summary>
        /// Checks to see if a specific command is available, given the current hardware status.
        /// </summary>
        /// <param name="command">The command whose availability is being tested.</param>
        /// <param name="hardwareStatus">Current hardware status of the device.</param>
        /// <returns><c>true</c> if the command can be executed under the given hardware status.</returns>
        public bool IsCommandAvailable(ProtocolCommandId command, HardwareStatusFlags hardwareStatus)
        {
            bool commandAvailable = AlwaysAvailableCommands.Contains(command);
            if (!commandAvailable)
            {
                Tuple<HardwareStatusFlags, FeatureCompatibility> commandAvailability = null;
                if (_commandAvailability.TryGetValue(command, out commandAvailability))
                {
                    commandAvailable = commandAvailability.Item1 == HardwareStatusFlags.None;
                    if (!commandAvailable)
                    {
                        if (commandAvailability.Item2 == FeatureCompatibility.Incompatible)
                        {
                            commandAvailable = (hardwareStatus & commandAvailability.Item1) == HardwareStatusFlags.None;
                        }
                        else if (commandAvailability.Item2 == FeatureCompatibility.Requires)
                        {
                            commandAvailable = (hardwareStatus & commandAvailability.Item1) == commandAvailability.Item1;
                        }
                    }
                }
            }
            return commandAvailable;
        }

        /// <summary>
        /// Verifies that all the commands in an enumerable of commands can be executed.
        /// </summary>
        /// <param name="commands">The commands whose total availability is being tested.</param>
        /// <param name="hardwareStatus">Current hardware status of the device.</param>
        /// <returns><c>true</c> if all of the commands can be executed under the given hardware status.</returns>
        public bool AllProtocolCommandsAvailable(IEnumerable<ProtocolCommandId> commands, HardwareStatusFlags hardwareStatus)
        {
            var allCommandsAvailable = (commands == null) || !commands.Any();
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    allCommandsAvailable = IsCommandAvailable(command, hardwareStatus);
                    if (!allCommandsAvailable)
                    {
                        break;
                    }
                }
            }
            return allCommandsAvailable;
        }

        private readonly Dictionary<ProtocolCommandId, CommandAvailable> _commandsSupported = new Dictionary<ProtocolCommandId, CommandAvailable>(DefaultCommandsSupported);

        /// <summary>
        /// Update the availability of commands given a packed bit array, where the very first bit corresponds to the command at value 0.
        /// </summary>
        /// <param name="availabilityMask">The bytes that represent all 256 potential commands.</param>
        public void UpdateCommandSupport(byte[] availabilityMask)
        {
            var knownCommandValues = Enum.GetValues(typeof(ProtocolCommandId));
            System.Diagnostics.Debug.Assert(availabilityMask.Length == 32, "Availability mask is wrong size.");
            byte commandNumber = 0;
            foreach (var commandMask in availabilityMask)
            {
                for (var i = 0; i < 8; ++i)
                {
                    var command = (ProtocolCommandId)commandNumber;
                    bool available = ((1 << i) & commandMask) != 0;
                    if (available)
                    {
                        if ((Array.IndexOf(knownCommandValues, command) < 0) || NeverAvailableCommands.Contains(command))
                        {
                            throw new UnsupportedCommandException(command);
                        }
                        _commandsSupported[command] = CommandAvailable.Yes;
                    }
                    else
                    {
                        if (AlwaysAvailableCommands.Contains(command))
                        {
                            throw new UnsupportedCommandException(command);
                        }
                        _commandsSupported[command] = CommandAvailable.No;
                    }
                    ++commandNumber;
                }
            }
        }

        /// <summary>
        /// Explicitly updates the availability of a command.
        /// </summary>
        /// <param name="command">The command whose availability is to be updated.</param>
        /// <param name="availability">The new availability state.</param>
        public void ChangeCommandAvailablility(ProtocolCommandId command, CommandAvailable availability)
        {
            _commandsSupported[command] = availability;
        }

        /// <summary>
        /// Checks to see if a specific command is available, given the current hardware status.
        /// </summary>
        /// <param name="command">The command whose availability is to be checked.</param>
        /// <returns><c>true</c> if the command can be executed under the current circumstances.</returns>
        public bool IsCommandAvailable(ProtocolCommandId command)
        {
            var available = false;
            var availability = CommandAvailable.Never;
            if (_commandsSupported.TryGetValue(command, out availability))
            {
                available = (availability == CommandAvailable.Yes) || (availability == CommandAvailable.Always);
            }
            return available;
        }

        /// <summary>
        /// Verifies that all the commands in an enumerable of commands can be executed.
        /// </summary>
        /// <param name="commands">The commands whose total availability is being tested.</param>
        /// <returns><c>true</c> if all of the commands can be executed under the current circumstances.</returns>
        public bool AllProtocolCommandsAvailable(IEnumerable<ProtocolCommandId> commands)
        {
            var allCommandsAvailable = (commands == null) || !commands.Any();
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    allCommandsAvailable = IsCommandAvailable(command);
                    if (!allCommandsAvailable)
                    {
                        break;
                    }
                }
            }
            return allCommandsAvailable;
        }
    }
}

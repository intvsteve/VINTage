// <copyright file="LtoFlashViewModel.Debug.cs" company="INTV Funhouse">
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

using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Debugging-related commands and code.
    /// </summary>
    public partial class LtoFlashViewModel
    {
#if DEBUG
        #region InjectCommandFailureCommand

        /// <summary>
        /// Command to inject bogus data in the next Locutus protocol command sent. Requires some command that sends data to Locutus.
        /// </summary>
        internal static readonly VisualDeviceCommand InjectCommandFailureCommand = new VisualDeviceCommand(OnInjectCommandFailure)
        {
            UniqueId = "INTV.LtoFlash.ViewModel.LtoFlashViewModel.InjectCommandFailureCommand",
            Name = "Inject Device Command Failure",
            KeyboardShortcutKey = "C",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            RequiredProtocolCommands = DeviceHelpers.GetErrorLogProtocolCommands
        };

        private static void OnInjectCommandFailure(object parameter)
        {
            INTV.LtoFlash.Model.Commands.ProtocolCommand.InjectCommandFailure();
        }

        #endregion // InjectCommandFailureCommand

        #region InjectCommandNakCommand

        /// <summary>
        /// Command to force a "real" NAK on the next command.
        /// </summary>
        internal static readonly VisualRelayCommand InjectCommandNakCommand = new VisualRelayCommand(OnInjectCommandNak)
        {
            UniqueId = "INTV.LtoFlash.ViewModel.LtoFlashViewModel.InjectCommandNakCommand",
            Name = "Inject NAK Next Command",
            KeyboardShortcutKey = "K",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt
        };

        private static void OnInjectCommandNak(object parameter)
        {
            INTV.LtoFlash.Model.Commands.ProtocolCommand.InjectCommandNak();
        }

        #endregion // InjectNakCommand

        #region InjectFirmwareCrashCommand

        /// <summary>
        /// Command to force LUI to act as if a firmware crash was reported.
        /// </summary>
        /// <remarks>NOTE: REQUIRES hack to DeviceHelpers.GetErrorAndCrashLogs() to force crash and error log retrieval! Those are usually gated by
        /// actual status bits in the heartbeat data stream. The implementation of this command to trigger the fake crash uses ephemeral data,
        /// so by the time the asynchronous response executes in DeviceHelpers.GetErrorAndCrashLogs(), it's lost. See more notes there.</remarks>
        internal static readonly VisualDeviceCommand InjectFirmwareCrashCommand = new VisualDeviceCommand(OnInjectFirmwareCrash)
        {
            UniqueId = "INTV.LtoFlash.ViewModel.LtoFlashViewModel.InjectFirmwareCrashCommand",
            Name = "Inject Firmware Crash",
            KeyboardShortcutKey = "W",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            RequiredProtocolCommands = DeviceHelpers.GetErrorAndCrashLogsProtocolCommands
        };

        private static void OnInjectFirmwareCrash(object parameter)
        {
            var ltoFlash = CompositionHelpers.Container.GetExportedValueOrDefault<LtoFlashViewModel>();
            if (ltoFlash != null && ltoFlash.ActiveLtoFlashDevice.IsValid)
            {
                // NOTE: Be sure to ensure that there will be something to report by forcing the
                // implementation in DeviceHelpers.GetErrorAndCrashLogs(AsyncTaskData taskData)
                // to actually read the data! That method does an additional JIT-check of the flags
                // to see if it's necessary to read the error and crash logs.
                ltoFlash.ActiveLtoFlashDevice.Device.InjectFirmwareCrash();
            }
        }

        #endregion // InjectFirmwareCrashCommand

        #region WaitForBeaconCommand

        /// <summary>
        /// Command to force a 'wait for beacon'.
        /// </summary>
        internal static readonly VisualRelayCommand WaitForBeaconCommand = new VisualRelayCommand(OnWaitForBeacon)
        {
            UniqueId = "INTV.LtoFlash.ViewModel.LtoFlashViewModel.WaitForBeaconCommand",
            Name = "Wait for Beacon"
        };

        private static void OnWaitForBeacon(object parameter)
        {
            var ltoFlash = CompositionHelpers.Container.GetExportedValueOrDefault<LtoFlashViewModel>();
            if (ltoFlash != null && ltoFlash.ActiveLtoFlashDevice.IsValid)
            {
                ltoFlash.ActiveLtoFlashDevice.Device.ConnectionState = ConnectionState.WaitForBeacon;
            }
        }

        #endregion // WaitForBeaconCommand
#endif // DEBUG
    }
}

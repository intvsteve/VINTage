// <copyright file="DebugCommandGroup.cs" company="INTV Funhouse">
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

// !!NOTE!! TRACK_PORT_LIFETIMES must be defined in BOTH this file AND SerialPortConnection.cs!!
////#define TRACK_PORT_LIFETIMES

using INTV.Shared.ComponentModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Commands available only in Debug builds for testing purposes.
    /// </summary>
    public partial class DebugCommandGroup : CommandGroup
    {
        /// <summary>
        /// The single instance of this command group.
        /// </summary>
        /// <remarks>Could this be done via MEF?</remarks>
        internal static readonly DebugCommandGroup Group = new DebugCommandGroup();

        private const string UniqueNameBase = "INTV.Shared.Commands.DebugCommandGroup";

        private DebugCommandGroup()
            : base(UniqueNameBase, string.Empty) // Looks like a bogus weight?
        {
        }

        #region DebugMenuCommand

        /// <summary>
        /// Command for the Debug submenu.
        /// </summary>
        public static readonly VisualRelayCommand DebugMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".DebugMenu",
            Weight = 0.22,
            Name = "Debug",
            MenuParent = RootCommandGroup.RootMenuCommand
        };

        #endregion // DebugCommandGroup

        #region ForceGarbageCollectCommand

        private static readonly VisualRelayCommand ForceGarbageCollectCommand = new VisualRelayCommand(OnForceGarbageCollect, (p) => true)
        {
            UniqueId = UniqueNameBase + ".ForceGarbageCollect",
            Name = "Force GC",
            KeyboardShortcutKey = "G",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            MenuParent = DebugMenuCommand,
            BlockWhenAppIsBusy = false
        };

        private static void OnForceGarbageCollect(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("Performing System.GC.Collect.");
            System.GC.Collect();
        }

        #endregion // ForceGarbageCollectCommand

        #region ForceCrashCommand

        private static readonly VisualRelayCommand ForceCrashCommand = new VisualRelayCommand(OnForceCrash)
        {
            UniqueId = UniqueNameBase + ".ForceCrash",
            Name = "Force Crash",
            KeyboardShortcutKey = "X",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            MenuParent = DebugMenuCommand,
            BlockWhenAppIsBusy = false
        };

        private static void OnForceCrash(object parameter)
        {
            throw new System.InvalidOperationException("This is a Crash of the Emergency Crash Me System. This is only a Crash!");
        }

        #endregion // ForceCrashCommand

        #region TriggerSerialPortReportCommand

        /// <summary>
        /// Command to trigger a 'live objects' report for serial ports.
        /// </summary>
        private static readonly VisualRelayCommand TriggerSerialPortReportCommand = new VisualRelayCommand(OnTriggerSerialPortReport)
        {
            UniqueId = UniqueNameBase + ".TriggerSerialPortReport",
            Name = "Serial Port Lifetime Report",
            KeyboardShortcutKey = "P",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            MenuParent = DebugMenuCommand,
            BlockWhenAppIsBusy = false
        };

        /// <summary>Called when serial port information is to be dumped.</summary>
        /// <param name="parameter">Unused command parameter.</param>
        /// <remarks>!!NOTE!! TRACK_PORT_LIFETIMES must be defined in BOTH this file AND SerialPortConnection.cs!!</remarks>
        private static void OnTriggerSerialPortReport(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("Generating Serial Port Lifetime Report...");
            INTV.Shared.Model.Device.SerialPortConnection.UpdateTracker("REPORT", "<null>");
            System.Diagnostics.Debug.WriteLine("Serial Port Lifetime Report complete.");
        }

        #endregion // TriggerSerialPortReportCommand

        #region CommandGroup

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(ForceGarbageCollectCommand);
            CommandList.Add(ForceCrashCommand);
            CommandList.Add(TriggerSerialPortReportCommand);
            AddPlatformCommands();
        }

        /// <summary>
        /// Implement to provide platform-specific commands and behaviors.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}

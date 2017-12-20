// <copyright file="DebugCommandGroup.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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

using INTV.Shared.ComponentModel;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class DebugCommandGroup
    {
        #region ToggleGCPressureCommand

        private static readonly VisualRelayCommand ToggleGCPressureCommand = new VisualRelayCommand(OnToggleGCPressure)
        {
            UniqueId = UniqueNameBase + ".ToggleGCPressure",
            Name = "Toggle GC Pressure",
            KeyboardShortcutKey = "P",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            MenuParent = DebugMenuCommand,
            BlockWhenAppIsBusy = false
        };

        private static void OnToggleGCPressure(object parameter)
        {
            _keepUpThePressure = !_keepUpThePressure;
            if (_keepUpThePressure)
            {
                new System.Threading.Thread(GCPressure).Start();
            }
        }

        private static void GCPressure()
        {
            while (_keepUpThePressure)
            {
                System.Threading.Thread.Sleep(1000);
                System.Diagnostics.Debug.WriteLine("Performing System.GC.Collect (Under Pressure).");
                System.GC.Collect();
            }
        }

        private static bool _keepUpThePressure;

        #endregion // ToggleGCPressureCommand

        #region DumpWindowListCommand

        private static readonly VisualRelayCommand DumpWindowListCommand = new VisualRelayCommand(OnDumpWindowList)
        {
            UniqueId = UniqueNameBase + ".DumpWindowListCommand",
            Name = "Dump Window List",
            KeyboardShortcutKey = "L",
            KeyboardShortcutModifiers = INTV.Shared.Commands.OSModifierKeys.Menu | INTV.Shared.Commands.OSModifierKeys.Ctrl | INTV.Shared.Commands.OSModifierKeys.Alt,
            MenuParent = DebugMenuCommand,
            BlockWhenAppIsBusy = false
        };

        private static void OnDumpWindowList(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("Current Window List");
            System.Diagnostics.Debug.WriteLine("------------------------------------------------");
            foreach (var window in NSApplication.SharedApplication.Windows)
            {
                System.Diagnostics.Debug.WriteLine(window.Title + " (" + window.Handle.ToString("X8") + "); RetainCount: [" + window.RetainCount + "]");
            }
            System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        #endregion // DumpWindowListCommand

        #region CommandGroup

        /// <summary>
        /// General data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return null; }
        }

        /// <summary>
        /// Creates the visual for a command, if applicable.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <returns>The visual for the command.</returns>
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            var window = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
            var visualCommand = (VisualRelayCommand)command;
            var rootMenu = (NSMenu)RootCommandGroup.RootMenuCommand.Visual;
            NSObject visual = null;
            switch (visualCommand.UniqueId)
            {
                case UniqueNameBase + ".DebugMenuCommand":
                    visual = rootMenu.ItemWithTag((int)DebugMenuCommand.GetHashCode());
                    if (visual == null)
                    {
                        visual = CreateMenuItemForCommand(command);
                    }
                    break;
                default:
                    visual = base.CreateVisualForCommand(command);
                    break;
            }
            return visual;
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            CommandList.Add(DumpWindowListCommand);
            CommandList.Add(ToggleGCPressureCommand);
        }

        #endregion // CommandGroup
    }
}

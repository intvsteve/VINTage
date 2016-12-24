// <copyright file="LtoFlashViewModel.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class LtoFlashViewModel
    {
        private static bool _registeredTextBlockEditor = INTV.Shared.View.TextBlockEditorAdorner.RegisterInPlaceEditor();

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public System.Configuration.ApplicationSettingsBase Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        partial void OSInitialize()
        {
#if DEBUG
            // Check some string resources.
            INTV.LtoFlash.Model.Commands.ProtocolCommandIdHelpers.CheckCommandResourceStrings();

            // These commands provides hooks to the Locutus simulator application or a means to debug specific commands on hardware.
            StartLocutusSimulatorCommand = new INTV.Shared.ComponentModel.RelayCommand(OnStartSimulator, (p) => true, this);
            GetFileSystemTablesCommand = new INTV.Shared.ComponentModel.VisualRelayCommand((d) => OnGetFileSystemTables(ActiveLtoFlashDevice), (d) => CanGetFileSystemTables(ActiveLtoFlashDevice));
            GetFileSystemStatisticsCommand = new INTV.Shared.ComponentModel.VisualRelayCommand((d) => OnGetFileSystemStatistics(ActiveLtoFlashDevice), (d) => CanGetFileSystemStatistics(ActiveLtoFlashDevice));
            GetErrorLogCommand = new INTV.Shared.ComponentModel.VisualRelayCommand((d) => OnGetErrorLog(ActiveLtoFlashDevice), (d) => CanGetErrorLog(ActiveLtoFlashDevice));
            ToggleConsolePowerCommand = new Shared.ComponentModel.RelayCommand((d) => OnTogglePower(ActiveLtoFlashDevice), (d) => CanTogglePower(ActiveLtoFlashDevice));
            ToggleDirtyFlagCommand = new Shared.ComponentModel.RelayCommand((d) => OnToggleDirtyFlag(ActiveLtoFlashDevice), (d) => CanToggleDirtyFlag(ActiveLtoFlashDevice));
            GetDirtyFlagsCommand = new Shared.ComponentModel.RelayCommand((d) => OnGetDirtyFlags(ActiveLtoFlashDevice), (d) => CanGetDirtyFlags(ActiveLtoFlashDevice));

            // This null check keeps the WPF XAML designer output clean.
            if ((System.Windows.Application.Current != null) && (System.Windows.Application.Current.MainWindow != null))
            {
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(StartLocutusSimulatorCommand, System.Windows.Input.Key.L, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(GetFileSystemTablesCommand, System.Windows.Input.Key.F, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(GetFileSystemStatisticsCommand, System.Windows.Input.Key.S, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(GetErrorLogCommand, System.Windows.Input.Key.E, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(GetErrorAndCrashLogsCommand, System.Windows.Input.Key.H, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(ToggleConsolePowerCommand, System.Windows.Input.Key.I, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(ToggleDirtyFlagCommand, System.Windows.Input.Key.D, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(GetDirtyFlagsCommand, System.Windows.Input.Key.G, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(InjectCommandFailureCommand, System.Windows.Input.Key.C, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(InjectCommandNakCommand, System.Windows.Input.Key.K, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(InjectFirmwareCrashCommand, System.Windows.Input.Key.W, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(WaitForBeaconCommand, System.Windows.Input.Key.B, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
            }
#endif
        }
    }
}

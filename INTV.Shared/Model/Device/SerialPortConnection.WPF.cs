// <copyright file="SerialPortConnection.WPF.cs" company="INTV Funhouse">
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

// #define TRACK_PORT_LIFETIMES

namespace INTV.Shared.Model.Device
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class SerialPortConnection
    {
#if TRACK_PORT_LIFETIMES
        private static INTV.Shared.ComponentModel.ICommand TriggerPortReportCommand = new INTV.Shared.ComponentModel.RelayCommand((p) => UpdateTracker("REPORT", "<null>"));

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        static partial void PlatformInstallCommand()
        {
            System.Windows.Application.Current.MainWindow.InputBindings.Add(new System.Windows.Input.KeyBinding(TriggerPortReportCommand, System.Windows.Input.Key.P, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Alt | System.Windows.Input.ModifierKeys.Shift));
        }
#endif // TRACK_PORT_LIFETIMES

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        static partial void UpdateAvailablePorts()
        {
            _availablePorts = System.IO.Ports.SerialPort.GetPortNames();
        }

        private void OpenPort()
        {
#if DEBUG
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
#endif //  DEBUG
            Port.Open();
        }
    }
}

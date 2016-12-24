// <copyright file="SingleInstanceApplicationCommands.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Applications applicable to SingleInstanceApplication implementations.
    /// </summary>
    /// <remarks>TODO: This is not used on Mac. Perhaps it can be removed, or relocated to Windows-specific code.</remarks>
    public static class SingleInstanceApplicationCommands
    {
        /// <summary>
        /// Gets the command to show the settings dialog.
        /// </summary>
        public static readonly ICommand SettingsDialogCommand = new VisualRelayCommand(OnShowSettingsDialog)
        {
            UniqueId = "INTV.Shared.ViewModel.SingleInstanceApplicationCommands.SettingsDialogCommand",
            Name = Resources.Strings.SingleApplicationCommands_Settings,
            SmallIcon = typeof(SingleInstanceApplicationCommands).LoadImageResource("ViewModel/Resources/Images/settings_16xLG.png"),
            LargeIcon = typeof(SingleInstanceApplicationCommands).LoadImageResource("ViewModel/Resources/Images/settings_32xMD.png")
        };

        /// <summary>
        /// Called when the <see cref="SettingsDialogCommand"/> executes.
        /// </summary>
        /// <param name="parameter">This parameter is not used.</param>
        internal static void OnShowSettingsDialog(object parameter)
        {
#if WIN
            var settingsDialog = new INTV.Shared.View.SettingsDialog();
            settingsDialog.Owner = System.Windows.Application.Current.MainWindow;
            settingsDialog.ShowDialog();
#elif MAC
            ErrorReporting.ReportNotImplementedError("SingleInstanceApplicationCommands.OnShowSettingsDialog");
#endif
        }
    }
}

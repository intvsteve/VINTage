// <copyright file="PromptToInstallFtdiDriverDialogViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.LtoFlash.Commands;
using INTV.Shared.ComponentModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for the dialog that prompts the user to install FTDI drivers.
    /// </summary>
    public class PromptToInstallFtdiDriverDialogViewModel : INTV.Shared.ViewModel.ViewModelBase
    {
        #region UI Strings

        public static readonly string InstallYes = Resources.Strings.PromptToInstallFtdiDriver_Yes;
        public static readonly string InstallNo = Resources.Strings.PromptToInstallFtdiDriver_No;
        public static readonly string InstallNoAskAgain = Resources.Strings.PromptToInstallFtdiDriver_DoNotAskAgain;

        #endregion // UI Strings

        #region InstallCommand

        /// <summary>
        /// The command to invoke to launch the FTDI installer prompt dialog.
        /// </summary>
        public static readonly RelayCommand InstallCommand = new RelayCommand(OnInstall) { BlockWhenAppIsBusy = false };

        private static void OnInstall(object parameter)
        {
            var viewModel = (PromptToInstallFtdiDriverDialogViewModel)parameter;
            viewModel.DialogResult = true;
        }

        #endregion // InstallCommand

        #region DoNotInstallCommand

        /// <summary>
        /// The command to decline installing the FTDI driver.
        /// </summary>
        public static readonly RelayCommand DoNotInstallCommand = new RelayCommand(OnDoNotInstall) { BlockWhenAppIsBusy = false };

        private static void OnDoNotInstall(object parameter)
        {
            var viewModel = (PromptToInstallFtdiDriverDialogViewModel)parameter;
            viewModel.DialogResult = false;
        }

        #endregion // DoNotInstallCommand

        #region DoNotAskToInstallAgainCommand

        /// <summary>
        /// The command to instruct LUI to stop asking to install the FTDI driver.
        /// </summary>
        public static readonly RelayCommand DoNotAskToInstallAgainCommand = new RelayCommand(OnDoNotToInstallAgain) { BlockWhenAppIsBusy = false };

        private static void OnDoNotToInstallAgain(object parameter)
        {
            var viewModel = (PromptToInstallFtdiDriverDialogViewModel)parameter;
            Properties.Settings.Default.PromptToInstallFTDIDriver = false;
            Properties.Settings.Default.Save();
            viewModel.DialogResult = false;
        }

        #endregion // DoNotAskToInstallAgainCommand

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="INTV.LtoFlash.ViewModel.PromptToInstallFtdiDriverDialogViewModel"/> class.
        /// </summary>
        public PromptToInstallFtdiDriverDialogViewModel()
        {
            var menuPathElements = new List<string>();
            var command = LtoFlashCommandGroup.GoToFTDIDriverPageCommand as VisualRelayCommand;
            while (command != null)
            {
                menuPathElements.Add(command.MenuItemName);
                command = command.MenuParent as VisualRelayCommand;
            }

            menuPathElements.Reverse();
            var menuPath = string.Join(">>", menuPathElements);
            Message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.LaunchFtdiDriverInstallerCommand_NoDriverPromptMessageFormat, menuPath);
        }

        /// <summary>
        /// Gets the title for the dialog.
        /// </summary>
        public string Title
        {
            get { return Resources.Strings.LaunchFtdiDriverInstallerCommand_PromptTitle; }
        }

        /// <summary>
        /// Gets the message to display in the dialog.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the dialog's result.
        /// </summary>
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { AssignAndUpdateProperty("DialogResult", value, ref _dialogResult); }
        }
        private bool? _dialogResult;
    }
}

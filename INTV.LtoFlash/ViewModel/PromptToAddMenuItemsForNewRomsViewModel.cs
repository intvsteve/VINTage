// <copyright file="PromptToAddMenuItemsForNewRomsViewModel.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for the PromptToAddMenuItemsForNewRoms dialog.
    /// </summary>
    public partial class PromptToAddMenuItemsForNewRomsViewModel : INTV.Shared.ViewModel.ViewModelBase
    {
        public static readonly string Title = Resources.Strings.PromptToAddMenuItems_Title;
        public static readonly string Message = Resources.Strings.PromptToAddMenuItems_Message;
        public static readonly string DoNotAskAgainText = Resources.Strings.PromptToAddMenuItems_DoNotAskAgain;
        public static readonly string OKButtonText = Resources.Strings.OK;

        /// <summary>
        /// The command that closes the dialog box window.
        /// </summary>
        public static readonly RelayCommand CloseDialogCommand = new RelayCommand(OnCloseDialogCommand) { BlockWhenAppIsBusy = false };

        public PromptToAddMenuItemsForNewRomsViewModel()
        {
            _addRomsToMenu = Properties.Settings.Default.AddRomsToMenu;
            _doNotAskAgain = !Properties.Settings.Default.PromptToAddRomsToMenu;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add new ROMs to the menu.
        /// </summary>
        public bool AddRomsToMenu
        {
            get { return _addRomsToMenu; }
            set { AssignAndUpdateProperty("AddRomsToMenu", value, ref _addRomsToMenu, (n, v) => Properties.Settings.Default.AddRomsToMenu = v); }
        }
        private bool _addRomsToMenu;

        /// <summary>
        /// Gets or sets a value indicating whether to show a prompt to add ROMs to menu.
        /// </summary>
        public bool DoNotAskAgain
        {
            get { return _doNotAskAgain; }
            set { AssignAndUpdateProperty("DoNotAskAgain", value, ref _doNotAskAgain, (n, v) => Properties.Settings.Default.PromptToAddRomsToMenu = !v); }
        }
        private bool _doNotAskAgain;

        private static void OnCloseDialogCommand(object parameter)
        {
            (parameter as System.Windows.Window).DialogResult = true;
        }
    }
}

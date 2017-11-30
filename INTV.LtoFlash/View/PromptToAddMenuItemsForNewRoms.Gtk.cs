// <copyright file="PromptToAddMenuItemsForNewRoms.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.View
{
    public partial class PromptToAddMenuItemsForNewRoms : Gtk.Dialog
    {
        private PromptToAddMenuItemsForNewRoms()
            : base(Resources.Strings.PromptToAddMenuItems_Title, SingleInstanceApplication.Instance.MainWindow, Gtk.DialogFlags.Modal)
        {
            this.Build();
            _message.Text = Resources.Strings.PromptToAddMenuItems_Message;
            _addToMenu.Label = Resources.Strings.SettingsPage_AddRomsToMenu;
            _addToMenu.Active = Properties.Settings.Default.AddRomsToMenu;
            _doNotAskAgain.Label = Resources.Strings.PromptToAddMenuItems_DoNotAskAgain;
            _doNotAskAgain.Active = !Properties.Settings.Default.PromptToAddRomsToMenu;
            Properties.Settings.Default.PropertyChanged += HandleSettingsChanged;
        }

        /// <summary>
        /// Creates an instance of the PromptToAddMenuItemsForNewRoms dialog.
        /// </summary>
        /// <returns>A new instance of the PromptToAddMenuItemsForNewRoms dialog.</returns>
        public static PromptToAddMenuItemsForNewRoms Create()
        {
            var dialog = new PromptToAddMenuItemsForNewRoms();
            return dialog;
        }

        protected void HandleAddToMenuToggled(object sender, System.EventArgs e)
        {
            if (_addToMenu.Active != Properties.Settings.Default.AddRomsToMenu)
            {
                Properties.Settings.Default.AddRomsToMenu = _addToMenu.Active;
            }
        }

        protected void HandleDoNotAskToggled(object sender, System.EventArgs e)
        {
            var shouldAsk = !_doNotAskAgain.Active;
            if (shouldAsk != Properties.Settings.Default.PromptToAddRomsToMenu)
            {
                Properties.Settings.Default.PromptToAddRomsToMenu = shouldAsk;
            }
        }

        private void HandleSettingsChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Properties.Settings.AddRomsToMenuPropertyName:
                    _addToMenu.Active = Properties.Settings.Default.AddRomsToMenu;
                    break;
                case Properties.Settings.PromptToAddRomsToMenuPropertyName:
                    _doNotAskAgain.Active = !Properties.Settings.Default.PromptToAddRomsToMenu;
                    break;
                default:
                    break;
            }
        }
    }
}
